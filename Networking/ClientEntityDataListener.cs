// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientEntityDataListener
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientEntityDataListener : DataEventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;

    public ClientEntityDataListener(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client, NetworkPeer.MessageID.CreateNewEntity, NetworkPeer.MessageID.RemoveEntity)
    {
      this.client = client;
    }

    public override void ErrorCaught(Exception e)
    {
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      int id = message.ReadInt32();
      if (dataType == NetworkPeer.MessageID.CreateNewEntity)
      {
        long x = message.ReadInt64();
        long y = message.ReadInt64();
        long z = message.ReadInt64();
        this.client.SignalEntityCreated(id, new Position3(x, y, z));
      }
      else
      {
        if (dataType != NetworkPeer.MessageID.RemoveEntity)
          return;
        this.client.SignalEntityRemoved(id);
      }
    }
  }
}
