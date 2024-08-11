// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerEntityTransactionHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerEntityTransactionHandler : SequencedEventHandler<NetworkServer>
  {
    private const int sectorsPerPacket = 8;
    private NetworkPlayer player;
    private PackedEntity entity;
    private Action<bool> readyFunction;
    private int sentSectorCounter;

    public ServerEntityTransactionHandler(
      NetConnection connection,
      NetworkServer server,
      PackedEntity entitySend,
      NetworkPlayer player,
      Action<bool> entitySentDelegate)
      : base(connection, server)
    {
      ServerEntityTransactionHandler transactionHandler = this;
      this.entity = entitySend != null && player != null ? entitySend : throw new ArgumentNullException(nameof (entitySend));
      this.player = player;
      this.readyFunction = entitySentDelegate;
      this.sentSectorCounter = 0;
      this.RequestTransfer(this.entity.SpaceEntity.Id);
      Engine.Console.LogNetworkTraffic("Beginning entity " + (object) this.entity.SpaceEntity.Id + " transaction to client " + (object) player.Id);
      this.AddNewSequenceFunction("suostuuko clientti ottamaan datan vastaan?", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 22)
          return;
        if (!message.ReadBoolean())
        {
          this.Unregister();
          if (this.readyFunction == null)
            return;
          this.readyFunction(false);
        }
        else
        {
          this.SendEntityInitializationData(this.entity.SpaceEntity);
          this.MoveToNextState();
        }
      })).AddNewSequenceFunction("odotetaan clientin kuittausta", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 24)
          return;
        int source1 = message.ReadInt32();
        if (source1 != this.entity.SpaceEntity.Id)
          return;
        int sentSectorCounter = this.sentSectorCounter;
        int source2 = Math.Min(this.sentSectorCounter + 8 - 1, this.entity.PackedSectors.Count - 1);
        NetOutgoingMessage message1 = this.networkInterface.CreateMessage();
        message1.Write((byte) 27);
        message1.Write(source1);
        message1.Write(sentSectorCounter);
        message1.Write(source2);
        for (int index = sentSectorCounter; index <= source2; ++index)
          this.WriteSectorToMessage(message1, this.entity.PackedSectors[index]);
        int num = (int) this.Key.SendMessage(message1, NetDeliveryMethod.ReliableOrdered, 6);
        this.sentSectorCounter = source2 + 1;
        if (this.sentSectorCounter < this.entity.PackedSectors.Count)
          return;
        this.SendControlsData(this.entity.SpaceEntity);
        this.MoveToNextState();
      })).AddNewSequenceFunction("Odotetaan kuittausta ohjainpalikoiden datasta", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 24 || message.ReadInt32() != this.entity.SpaceEntity.Id)
          return;
        this.SendEntityFinalizationData(this.entity.SpaceEntity);
        this.MoveToNextState();
      })).AddNewSequenceFunction("Viimeinen kuittaus viimeistelee lähetyksen", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 26)
          return;
        transactionHandler.Unregister();
        if (transactionHandler.readyFunction != null)
          transactionHandler.readyFunction(true);
        Engine.Console.LogNetworkTraffic("Entity " + (object) transactionHandler.entity.SpaceEntity.Id + " transaction to client " + (object) player.Id + " finished!");
      }));
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Error while sending entity data to client: " + e.Message);
    }

    private void RequestTransfer(int entityId)
    {
      NetOutgoingMessage message = this.networkInterface.CreateMessage();
      message.Write((byte) 21);
      message.Write(entityId);
      int num = (int) this.Key.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 6);
    }

    private void SendControlsData(SpaceEntity entity)
    {
      NetOutgoingMessage message = this.networkInterface.CreateMessage();
      message.Write((byte) 28);
      message.Write(entity.Id);
      List<ControlBlock> controlBlocks = new List<ControlBlock>();
      entity.BlockSectorManager.SectorList.ForEach((Action<BlockSector>) (s => s.ControlBlocks.ForEach((Action<ControlBlock>) (c => controlBlocks.Add(c)))));
      message.Write(controlBlocks.Count);
      foreach (ControlBlock controlBlock in controlBlocks)
      {
        Dictionary<Color, HashSet<TriggerBlock>> controlledBlocks = controlBlock.ControlledBlocks;
        List<ColorKeyGroup> colorKeyGroups = controlBlock.ColorKeyGroups;
        message.Write(controlBlock.PositionInEntitySpace.X);
        message.Write(controlBlock.PositionInEntitySpace.Y);
        message.Write(controlBlock.PositionInEntitySpace.Z);
        message.Write(colorKeyGroups.Count);
        foreach (ColorKeyGroup colorKeyGroup in colorKeyGroups)
        {
          Color color = colorKeyGroup.Color;
          message.Write(color.R);
          message.Write(color.G);
          message.Write(color.B);
          message.Write((byte) colorKeyGroup.Key);
          if (controlledBlocks.ContainsKey(color))
          {
            HashSet<TriggerBlock> triggerBlockSet = controlledBlocks[color];
            message.Write(triggerBlockSet.Count);
            foreach (TriggerBlock triggerBlock in triggerBlockSet)
            {
              message.Write(triggerBlock.PositionInShipSpace.X);
              message.Write(triggerBlock.PositionInShipSpace.Y);
              message.Write(triggerBlock.PositionInShipSpace.Z);
            }
          }
          else
            message.Write(0);
        }
        CameraBlock cameraBlock = controlBlock.CameraBlock;
        message.Write(cameraBlock != null ? (byte) 1 : (byte) 0);
        if (cameraBlock != null)
        {
          message.Write(cameraBlock.PositionInShipSpace.X);
          message.Write(cameraBlock.PositionInShipSpace.Y);
          message.Write(cameraBlock.PositionInShipSpace.Z);
        }
      }
      int num = (int) this.Key.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 6);
    }

    private void SendEntityFinalizationData(SpaceEntity entity)
    {
      NetOutgoingMessage message = this.networkInterface.CreateMessage();
      message.Write((byte) 25);
      message.Write(entity.Id);
      message.Write(entity.IsDisposed);
      message.WritePadBits();
      message.Write(entity.Position.X);
      message.Write(entity.Position.Y);
      message.Write(entity.Position.Z);
      message.Write(entity.Orientation.X);
      message.Write(entity.Orientation.Y);
      message.Write(entity.Orientation.Z);
      message.Write(entity.Orientation.W);
      int num = (int) this.Key.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 6);
    }

    private void SendEntityInitializationData(SpaceEntity entity)
    {
      NetOutgoingMessage message = this.networkInterface.CreateMessage();
      message.Write((byte) 23);
      message.Write(entity.Id);
      message.Write(entity.Position.X);
      message.Write(entity.Position.Y);
      message.Write(entity.Position.Z);
      message.Write(entity.Orientation.X);
      message.Write(entity.Orientation.Y);
      message.Write(entity.Orientation.Z);
      message.Write(entity.Orientation.W);
      message.Write(entity.BlockSectorManager.SectorList.Count);
      int num = (int) this.Key.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 6);
    }

    private void WriteSectorToMessage(NetOutgoingMessage message, PackedBlockSector sector)
    {
      if (sector.CompressedFormations == null || sector.CompressedIds == null || sector.CompressedOrientations == null || message == null)
        throw new ArgumentNullException("Cannot send block sector with null data");
      message.Write(sector.SectorPosition.X);
      message.Write(sector.SectorPosition.Y);
      message.Write(sector.SectorPosition.Z);
      message.Write((ushort) sector.CompressedFormations.Length);
      message.Write(sector.CompressedFormations);
      message.Write((ushort) sector.CompressedIds.Length);
      message.Write(sector.CompressedIds);
      message.Write((ushort) sector.CompressedOrientations.Length);
      message.Write(sector.CompressedOrientations);
    }
  }
}
