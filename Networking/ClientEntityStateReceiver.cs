// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientEntityStateReceiver
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientEntityStateReceiver : DataEventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;
    private EntityStateContainer entityState;
    private ControlBlocksStates controlBlocksStates;

    public ClientEntityStateReceiver(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client, NetworkPeer.MessageID.EntityStateUpdate)
    {
      this.client = client;
      this.entityState = new EntityStateContainer();
      this.controlBlocksStates = new ControlBlocksStates();
    }

    public override void ErrorCaught(Exception e) => Engine.Console.WriteErrorLine(e.Message);

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      this.controlBlocksStates.Clear();
      int num1 = message.ReadInt32();
      long num2 = message.ReadInt64();
      Position3 position3 = new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64());
      Quaternion quaternion = new Quaternion(message.ReadFloat(), message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
      Vector3 vector3 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
      ushort num3 = message.ReadUInt16();
      for (int index = 0; index < (int) num3; ++index)
        this.controlBlocksStates.AddBlock(new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()), message.ReadInt16());
      this.entityState.CenterOfMass = vector3;
      this.entityState.EntityId = num1;
      this.entityState.Orientation = quaternion;
      this.entityState.Position = position3;
      this.entityState.Ticks = num2;
      this.client.SignalEntityStateReceived(this.entityState, this.controlBlocksStates);
    }
  }
}
