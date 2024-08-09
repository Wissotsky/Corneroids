// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientProjectileDataHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientProjectileDataHandler : DataEventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;

    public ClientProjectileDataHandler(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client, NetworkPeer.MessageID.CreateProjectile)
    {
      this.client = client;
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Projectile handler failure: " + e.Message);
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      this.client.SignalCreateProjectile(message.ReadByte(), new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64()), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()));
    }
  }
}
