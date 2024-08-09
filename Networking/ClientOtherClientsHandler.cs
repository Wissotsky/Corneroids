// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientOtherClientsHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientOtherClientsHandler : EventHandler<NetConnection, NetworkClient>
  {
    public ClientOtherClientsHandler(NetworkClient client)
      : base((NetConnection) null, client)
    {
    }

    public override void ErrorCaught(Exception e)
    {
    }

    public override void Handle(NetIncomingMessage message)
    {
      if (message.MessageType != NetIncomingMessageType.Data)
        return;
      switch ((NetworkPeer.MessageID) message.ReadByte())
      {
        case NetworkPeer.MessageID.NewClientConnected:
          this.networkInterface.OtherClientConnected(message.ReadInt32(), message.ReadString());
          break;
        case NetworkPeer.MessageID.ClientDisconnected:
          this.networkInterface.OtherClientDisconnected(message.ReadInt32(), message.ReadString());
          break;
      }
    }
  }
}
