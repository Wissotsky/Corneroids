// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerControlDataHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerControlDataHandler : DataEventHandler<NetConnection, NetworkServer>
  {
    private NetworkServerManager server;

    public ServerControlDataHandler(NetworkServerManager server)
      : base((NetConnection) null, (NetworkServer) server, NetworkPeer.MessageID.ControlBindRequest, NetworkPeer.MessageID.ControlKeyChangeRequest, NetworkPeer.MessageID.ControlUnbindRequest)
    {
      this.server = server;
    }

    public void ControlBlockBlockUnbound(ControlBlock console, TriggerBlock block)
    {
      if (console == null || block == null)
        return;
      int id = console.OwnerSector.Owner.Id;
      NetOutgoingMessage message = this.server.CreateMessage();
      message.Write((byte) 31);
      message.Write(id);
      message.Write(console.PositionInEntitySpace.X);
      message.Write(console.PositionInEntitySpace.Y);
      message.Write(console.PositionInEntitySpace.Z);
      message.Write(block.PositionInShipSpace.X);
      message.Write(block.PositionInShipSpace.Y);
      message.Write(block.PositionInShipSpace.Z);
      this.server.SendOrderedDataMessage(message, (NetConnection) null, 7);
    }

    public void ControlBlockNewBlockBound(ControlBlock console, TriggerBlock block, Color newColor)
    {
      if (console == null || block == null)
        return;
      int id = console.OwnerSector.Owner.Id;
      NetOutgoingMessage message = this.server.CreateMessage();
      message.Write((byte) 30);
      message.Write(id);
      message.Write(console.PositionInEntitySpace.X);
      message.Write(console.PositionInEntitySpace.Y);
      message.Write(console.PositionInEntitySpace.Z);
      message.Write(block.PositionInShipSpace.X);
      message.Write(block.PositionInShipSpace.Y);
      message.Write(block.PositionInShipSpace.Z);
      message.Write(newColor.R);
      message.Write(newColor.G);
      message.Write(newColor.B);
      this.server.SendOrderedDataMessage(message, (NetConnection) null, 7);
    }

    public void ControlBlockKeyChanged(ControlBlock block, ColorKeyGroup newKey)
    {
      if (block == null || newKey == null)
        return;
      int id = block.OwnerSector.Owner.Id;
      NetOutgoingMessage message = this.server.CreateMessage();
      message.Write((byte) 29);
      message.Write(id);
      message.Write(block.PositionInEntitySpace.X);
      message.Write(block.PositionInEntitySpace.Y);
      message.Write(block.PositionInEntitySpace.Z);
      message.Write(newKey.Color.R);
      message.Write(newKey.Color.G);
      message.Write(newKey.Color.B);
      message.Write((byte) newKey.Key);
      this.server.SendOrderedDataMessage(message, (NetConnection) null, 7);
    }

    public override void ErrorCaught(Exception e)
    {
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      switch (dataType)
      {
        case NetworkPeer.MessageID.ControlKeyChangeRequest:
          this.HandleNewKeyRequest(message);
          break;
        case NetworkPeer.MessageID.ControlBindRequest:
          this.HandleBindRequest(message);
          break;
        case NetworkPeer.MessageID.ControlUnbindRequest:
          this.HandleUnbindRequest(message);
          break;
      }
    }

    private void HandleBindRequest(NetIncomingMessage message)
    {
      this.server.SignalRequestBindBlockToConsole(message.ReadInt32(), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()), new Color(message.ReadByte(), message.ReadByte(), message.ReadByte(), byte.MaxValue));
    }

    private void HandleUnbindRequest(NetIncomingMessage message)
    {
      this.server.SignalRequestUnbindBlockFromConsole(message.ReadInt32(), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()));
    }

    private void HandleNewKeyRequest(NetIncomingMessage message)
    {
      this.server.SignalRequestChangeConsoleKey(message.ReadInt32(), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()), new Color(message.ReadByte(), message.ReadByte(), message.ReadByte(), byte.MaxValue), message.ReadByte());
    }
  }
}
