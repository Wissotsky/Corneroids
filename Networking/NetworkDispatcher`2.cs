// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.NetworkDispatcher`2
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public class NetworkDispatcher<T, Y>
    where T : class
    where Y : INetwork
  {
    private List<EventHandler<T, Y>> eventHandlers;

    public NetworkDispatcher() => this.eventHandlers = new List<EventHandler<T, Y>>();

    public void ClientDisconnected(int clientId)
    {
      foreach (EventHandler<T, Y> eventHandler in this.eventHandlers)
        eventHandler.ClientDisconnected(clientId);
    }

    public void HandleIncomingMessage(NetIncomingMessage message)
    {
      if (message == null)
        return;
      NetConnection senderConnection = message.SenderConnection;
      for (int index = this.eventHandlers.Count - 1; index >= 0; --index)
      {
        if ((object) this.eventHandlers[index].Key != senderConnection)
        {
          if ((object) this.eventHandlers[index].Key != null)
            continue;
        }
        try
        {
          message.Position = 0L;
          this.eventHandlers[index].Handle(message);
        }
        catch (Exception ex)
        {
          try
          {
            Engine.Console.WriteErrorLine("Error in network event handler: " + ex.Message);
            this.eventHandlers[index].ErrorCaught(ex);
          }
          catch
          {
          }
        }
      }
    }

    public void RegisterEventHandler(EventHandler<T, Y> eventHandler)
    {
      if (eventHandler == null || this.eventHandlers.Contains(eventHandler))
        return;
      this.eventHandlers.Add(eventHandler);
    }

    public void UnregisterEventHandler(EventHandler<T, Y> eventHandler)
    {
      eventHandler?.Unregister();
    }

    public void UnregisterEventHandlersOf(T key)
    {
      foreach (EventHandler<T, Y> eventHandler in this.eventHandlers)
      {
        if ((object) eventHandler.Key == (object) key)
          eventHandler.Unregister();
      }
    }

    public void Update()
    {
      for (int index = this.eventHandlers.Count - 1; index >= 0; --index)
      {
        if (this.eventHandlers[index].MarkedForUnregister)
        {
          Engine.Console.LogNetworkTraffic("Event handler closed: " + this.eventHandlers[index].ToString());
          this.eventHandlers.RemoveAt(index);
        }
      }
    }

    public List<EventHandler<T, Y>> RegisteredHandlers => this.eventHandlers;
  }
}
