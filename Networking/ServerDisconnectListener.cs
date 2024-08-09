// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerDisconnectListener
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerDisconnectListener : EventHandler<NetConnection, NetworkServer>
  {
    public ServerDisconnectListener(NetworkServer server)
      : base((NetConnection) null, server)
    {
    }

    public override void ErrorCaught(Exception e)
    {
    }

    public override void Handle(NetIncomingMessage message)
    {
      if (message.MessageType != NetIncomingMessageType.StatusChanged || message.ReadByte() != (byte) 7)
        return;
      this.networkInterface.UserDisconnected(message.SenderConnection, message.ReadString());
    }
  }
}
