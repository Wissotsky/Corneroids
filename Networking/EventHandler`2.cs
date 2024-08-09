// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.EventHandler`2
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public abstract class EventHandler<T, Y> where Y : INetwork
  {
    private bool unregister;
    public readonly T Key;
    protected Y networkInterface;

    public EventHandler(T key, Y networkInterface)
    {
      this.Key = key;
      this.networkInterface = networkInterface;
      this.unregister = false;
    }

    public virtual void ClientDisconnected(int clientId)
    {
    }

    public abstract void Handle(NetIncomingMessage message);

    public abstract void ErrorCaught(Exception e);

    public virtual void Unregister() => this.unregister = true;

    public bool MarkedForUnregister => this.unregister;
  }
}
