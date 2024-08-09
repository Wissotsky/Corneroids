// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientControlDataListener
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientControlDataListener : DataEventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;

    public ClientControlDataListener(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client, NetworkPeer.MessageID.ControlKeyChanged, NetworkPeer.MessageID.ControlNewBind, NetworkPeer.MessageID.ControlUnbind, NetworkPeer.MessageID.PlayerMounted)
    {
      this.client = client;
    }

    public void RequestBindBlockToConsole(
      ControlBlock console,
      TriggerBlock target,
      Color newColor)
    {
      if (console == null || target == null)
        return;
      NetOutgoingMessage messageTemplate = this.client.CreateMessageTemplate(NetworkPeer.MessageID.ControlBindRequest);
      messageTemplate.Write(this.GetOwnerEntityId(console));
      messageTemplate.Write(console.PositionInEntitySpace.X);
      messageTemplate.Write(console.PositionInEntitySpace.Y);
      messageTemplate.Write(console.PositionInEntitySpace.Z);
      messageTemplate.Write(target.PositionInShipSpace.X);
      messageTemplate.Write(target.PositionInShipSpace.Y);
      messageTemplate.Write(target.PositionInShipSpace.Z);
      messageTemplate.Write(newColor.R);
      messageTemplate.Write(newColor.G);
      messageTemplate.Write(newColor.B);
      this.client.SendOrderedDataMessage(messageTemplate, (NetConnection) null, 7);
    }

    public void RequestUnbindBlockFromConsole(ControlBlock console, TriggerBlock target)
    {
      if (console == null || target == null)
        return;
      NetOutgoingMessage message = this.client.CreateMessage();
      message.Write((byte) 39);
      message.Write(this.GetOwnerEntityId(console));
      message.Write(console.PositionInEntitySpace.X);
      message.Write(console.PositionInEntitySpace.Y);
      message.Write(console.PositionInEntitySpace.Z);
      message.Write(target.PositionInShipSpace.X);
      message.Write(target.PositionInShipSpace.Y);
      message.Write(target.PositionInShipSpace.Z);
      this.client.SendOrderedDataMessage(message, (NetConnection) null, 7);
    }

    public void RequestChangeColorKey(ControlBlock console, ColorKeyGroup newKey)
    {
      if (console == null)
        return;
      NetOutgoingMessage message = this.client.CreateMessage();
      message.Write((byte) 37);
      message.Write(this.GetOwnerEntityId(console));
      message.Write(console.PositionInEntitySpace.X);
      message.Write(console.PositionInEntitySpace.Y);
      message.Write(console.PositionInEntitySpace.Z);
      message.Write(newKey.Color.R);
      message.Write(newKey.Color.G);
      message.Write(newKey.Color.B);
      message.Write((byte) newKey.Key);
      this.client.SendOrderedDataMessage(message, (NetConnection) null, 7);
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Failed to parse control block data: " + e.Message);
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      int entityId = message.ReadInt32();
      Vector3 vector3 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
      switch (dataType)
      {
        case NetworkPeer.MessageID.ControlKeyChanged:
          Color color = new Color(message.ReadByte(), message.ReadByte(), message.ReadByte());
          Keys key = (Keys) message.ReadByte();
          this.client.SignalControlBlockKeyChanged(entityId, vector3, color, key);
          break;
        case NetworkPeer.MessageID.ControlNewBind:
          Vector3 triggerPosition1 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
          Color newKey = new Color(message.ReadByte(), message.ReadByte(), message.ReadByte());
          this.client.SignalControlBlockNewBind(entityId, vector3, triggerPosition1, newKey);
          break;
        case NetworkPeer.MessageID.ControlUnbind:
          Vector3 triggerPosition2 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
          this.client.SignalControlBlockBlockUnbound(entityId, vector3, triggerPosition2);
          break;
        case NetworkPeer.MessageID.PlayerMounted:
          int playerId = message.ReadInt32();
          bool isMounted = message.ReadBoolean();
          this.client.SignalPlayerMountedStatusChanged(playerId, entityId, vector3, isMounted);
          break;
      }
    }

    private int GetOwnerEntityId(ControlBlock console)
    {
      return console.OwnerSector != null && console.OwnerSector.Owner != null ? console.OwnerSector.Owner.Id : -1;
    }
  }
}
