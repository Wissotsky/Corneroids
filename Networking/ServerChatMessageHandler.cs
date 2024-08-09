// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerChatMessageHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerChatMessageHandler : EventHandler<NetConnection, NetworkServer>
  {
    private NetworkServerManager serverManager;

    public ServerChatMessageHandler(NetworkServerManager server)
      : base((NetConnection) null, (NetworkServer) server)
    {
      this.serverManager = server;
    }

    public override void ErrorCaught(Exception e)
    {
    }

    public override void Handle(NetIncomingMessage message)
    {
      if (message.MessageType != NetIncomingMessageType.Data || message.ReadByte() != (byte) 14)
        return;
      this.serverManager.SignalChatMessageReceived(message.ReadString(), message.SenderConnection);
    }
  }
}
