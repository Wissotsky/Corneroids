// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientEntityReceiver
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientEntityReceiver : SequencedEventHandler<NetworkClient>
  {
    private NetworkClientManager client;
    private int entityId;
    private List<PackedControlBlock> receivedControlBlocks;
    private List<BlockEvent> unhandledEvents;
    private int id;
    private bool isDisposed;
    private int receivedBlockSectorCount;
    private PackedBlockSector[] receivedBlockSectors;
    private int blockSectorCount;
    private Quaternion orientation;
    private Position3 position;
    private Action<PackedEntityContainer> resultCallback;

    public ClientEntityReceiver(
      NetConnection connection,
      NetworkClientManager client,
      int entityId,
      Action<PackedEntityContainer> resultCallback)
      : base(connection, (NetworkClient) client)
    {
      ClientEntityReceiver clientEntityReceiver = this;
      this.client = client;
      this.entityId = entityId;
      this.receivedBlockSectorCount = 0;
      this.resultCallback = resultCallback;
      this.isDisposed = false;
      this.receivedControlBlocks = new List<PackedControlBlock>();
      this.unhandledEvents = new List<BlockEvent>();
      if (resultCallback == null)
        throw new ArgumentNullException(nameof (resultCallback));
      this.AddNewSequenceFunction("odotetaan entityn perusdataa, kuten sijaintia ja kulmia", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 23 || message.ReadInt32() != this.entityId)
          return;
        this.ReadInitializationData(message);
        if (this.receivedBlockSectors == null)
        {
          this.Unregister();
        }
        else
        {
          this.Ack(this.entityId);
          this.MoveToNextState();
        }
      })).AddNewSequenceFunction("Vastaanotetaan palikkasektoreita", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 27 || message.ReadInt32() != clientEntityReceiver.entityId)
          return;
        int num1 = message.ReadInt32();
        int num2 = message.ReadInt32();
        for (int sectorIndex = num1; sectorIndex <= num2; ++sectorIndex)
          clientEntityReceiver.ParseBlockSector(message, sectorIndex);
        clientEntityReceiver.receivedBlockSectorCount = num2 + 1;
        if (clientEntityReceiver.receivedBlockSectorCount >= clientEntityReceiver.blockSectorCount)
          clientEntityReceiver.MoveToNextState();
        clientEntityReceiver.Ack(entityId);
      })).AddNewSequenceFunction("Odotetaan ohjainpalikoiden dataa", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 28 || message.ReadInt32() != clientEntityReceiver.entityId)
          return;
        clientEntityReceiver.ReadControlsData(message);
        clientEntityReceiver.Ack(entityId);
        clientEntityReceiver.MoveToNextState();
      })).AddNewSequenceFunction("Odotetaan finalisaatiodataa", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 25 || message.ReadInt32() != this.entityId)
          return;
        this.ReadFinalizationData(message);
        NetOutgoingMessage message1 = this.networkInterface.CreateMessage();
        message1.Write((byte) 26);
        int num = (int) this.Key.SendMessage(message1, NetDeliveryMethod.ReliableOrdered, 6);
        if (this.isDisposed)
          return;
        this.resultCallback(new PackedEntityContainer()
        {
          Id = this.entityId,
          BlockSectors = this.receivedBlockSectors,
          ControlBlocks = this.receivedControlBlocks.ToArray(),
          EntityOrientation = this.orientation,
          EntityPosition = this.position
        });
      }));
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Failed to receive entity data: " + e.Message);
    }

    protected override void HandleMessageOutsideOfSequence(NetIncomingMessage message)
    {
      switch ((NetworkPeer.MessageID) message.ReadByte())
      {
        case NetworkPeer.MessageID.AddNewBlock:
          ClientBlockDataHandler.ParseAddBlockFromMessage(message, (Action<int, byte, byte, Vector3>) ((entityId, blockId, orientation, position) =>
          {
            if (entityId != this.entityId)
              return;
            this.unhandledEvents.Add(new BlockEvent()
            {
              EntityId = entityId,
              BlockAction = BlockEvent.Action.Add,
              BlockId = (int) blockId,
              Position = position,
              Orientation = orientation
            });
          }));
          break;
        case NetworkPeer.MessageID.RemoveBlock:
          ClientBlockDataHandler.ParseRemoveBlockFromMessage(message, (Action<int, Vector3, bool>) ((entityId, position, destroy) =>
          {
            if (entityId != this.entityId)
              return;
            this.unhandledEvents.Add(new BlockEvent()
            {
              BlockAction = BlockEvent.Action.Remove,
              EntityId = entityId,
              Position = position
            });
          }));
          break;
      }
    }

    public int EntityId => this.entityId;

    public List<BlockEvent> UnhandledEvents => this.unhandledEvents;

    private void Ack(int entityId)
    {
      NetOutgoingMessage message = this.networkInterface.CreateMessage();
      message.Write((byte) 24);
      message.Write(entityId);
      int num = (int) this.Key.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 6);
    }

    private void ParseBlockSector(NetIncomingMessage message, int sectorIndex)
    {
      try
      {
        Vector3int vector3int = new Vector3int(message.ReadInt32(), message.ReadInt32(), message.ReadInt32());
        ushort numberOfBytes1 = message.ReadUInt16();
        byte[] numArray1 = message.ReadBytes((int) numberOfBytes1);
        ushort numberOfBytes2 = message.ReadUInt16();
        byte[] numArray2 = message.ReadBytes((int) numberOfBytes2);
        ushort numberOfBytes3 = message.ReadUInt16();
        byte[] numArray3 = message.ReadBytes((int) numberOfBytes3);
        PackedBlockSector packedBlockSector = new PackedBlockSector()
        {
          CompressedIds = numArray2,
          CompressedFormations = numArray1,
          CompressedOrientations = numArray3,
          SectorPosition = vector3int
        };
        if (sectorIndex < 0 || sectorIndex >= this.blockSectorCount)
          return;
        this.receivedBlockSectors[sectorIndex] = packedBlockSector;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to parse compressed block sector: " + ex.Message);
      }
    }

    private void ReadControlsData(NetIncomingMessage message)
    {
      try
      {
        int num1 = message.ReadInt32();
        for (int index1 = 0; index1 < num1; ++index1)
        {
          PackedControlBlock packedControlBlock = new PackedControlBlock();
          packedControlBlock.PositionInEntitySpace = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
          int num2 = message.ReadInt32();
          for (int index2 = 0; index2 < num2; ++index2)
          {
            Color color = new Color(message.ReadByte(), message.ReadByte(), message.ReadByte());
            Keys key = (Keys) message.ReadByte();
            Vector3 position = Vector3.Zero;
            packedControlBlock.BindKeyToColor(color, key);
            int num3 = message.ReadInt32();
            for (int index3 = 0; index3 < num3; ++index3)
            {
              position = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
              packedControlBlock.BindBlock(position, color);
            }
          }
          if (message.ReadByte() == (byte) 1)
          {
            Vector3 vector3 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
            packedControlBlock.BoundCamera = new Vector3?(vector3);
          }
          else
            packedControlBlock.BoundCamera = new Vector3?();
          this.receivedControlBlocks.Add(packedControlBlock);
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to read console data from netmessage: " + ex.Message);
      }
    }

    private void ReadInitializationData(NetIncomingMessage message)
    {
      this.position = new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64());
      this.orientation = new Quaternion(message.ReadFloat(), message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
      this.blockSectorCount = message.ReadInt32();
      if (this.blockSectorCount <= 0)
        return;
      this.receivedBlockSectors = new PackedBlockSector[this.blockSectorCount];
    }

    private void ReadFinalizationData(NetIncomingMessage message)
    {
      try
      {
        this.isDisposed = message.ReadBoolean();
        message.ReadPadBits();
        this.position = new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64());
        this.orientation = new Quaternion(message.ReadFloat(), message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to read finalization data of block sector: " + ex.Message);
      }
    }
  }
}
