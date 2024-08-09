// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientGameEventListener
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientGameEventListener : EventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;

    public ClientGameEventListener(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client)
    {
      this.client = client;
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Error caught in ClientGameEventListener: " + e.Message);
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
        case NetworkPeer.GameEventId.KillPlayer:
          this.EventKillPlayer(message);
          break;
        case NetworkPeer.GameEventId.AddItem:
          this.EventAddFloatingItem(message);
          break;
        case NetworkPeer.GameEventId.SpawnPlayer:
          this.EventSpawnPlayer(message);
          break;
        case NetworkPeer.GameEventId.PickItem:
          this.EventPickItem(message);
          break;
      }
    }

    private void EventAddFloatingItem(NetIncomingMessage message)
    {
      int environmentId = message.ReadInt32();
      int itemId = message.ReadInt32();
      int quantity = message.ReadInt32();
      Position3 position = new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64());
      Vector3 speed = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
      this.client.SignalAddFloatingItem(itemId, quantity, environmentId, position, speed);
    }

    private void EventKillPlayer(NetIncomingMessage message)
    {
      this.client.SignalAstronautKilled(message.ReadInt32());
    }

    private void EventPickItem(NetIncomingMessage message)
    {
      this.client.SignalItemPickedByPlayer(message.ReadInt32(), message.ReadInt32(), message.ReadInt32(), message.ReadInt32());
    }

    private void EventSpawnPlayer(NetIncomingMessage message)
    {
      this.client.SignalPlayerSpawned(message.ReadInt32(), new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64()));
    }
  }
}
