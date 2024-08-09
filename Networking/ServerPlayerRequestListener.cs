// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerPlayerRequestListener
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerPlayerRequestListener : EventHandler<NetConnection, NetworkServer>
  {
    private NetworkServerManager server;

    public ServerPlayerRequestListener(NetworkServerManager server)
      : base((NetConnection) null, (NetworkServer) server)
    {
      this.server = server;
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Error caught in serverplayerrequestlistener: " + e.Message);
    }

    public override void Handle(NetIncomingMessage message)
    {
      if (message.ReadByte() != (byte) 36)
        return;
      this.HandleEvent((NetworkPeer.GameEventId) message.ReadByte(), message);
    }

    private void HandleEvent(NetworkPeer.GameEventId eventType, NetIncomingMessage message)
    {
      switch (eventType)
      {
        case NetworkPeer.GameEventId.RequestSpawn:
          this.RequestSpawn(message);
          break;
        case NetworkPeer.GameEventId.RequestTossItem:
          this.RequestTossItem(message);
          break;
      }
    }

    private void RequestSpawn(NetIncomingMessage message)
    {
      this.server.SignalRequestSpawnPlayer(message.ReadInt32());
    }

    private void RequestTossItem(NetIncomingMessage message)
    {
      this.server.SignalRequestTossItem(message.ReadInt32(), message.ReadInt32(), new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64()), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()));
    }
  }
}
