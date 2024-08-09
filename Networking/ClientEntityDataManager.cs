// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientEntityDataManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientEntityDataManager : DataEventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;
    private Action<PackedEntityContainer> resultCallback;
    private List<ClientEntityReceiver> entityRecieverHandlers;

    public ClientEntityDataManager(
      NetConnection connection,
      NetworkClientManager client,
      Action<PackedEntityContainer> callbackFunction)
      : base(connection, (NetworkClient) client, NetworkPeer.MessageID.EntityTransferRequest)
    {
      this.client = client;
      this.resultCallback = callbackFunction;
      this.entityRecieverHandlers = new List<ClientEntityReceiver>();
      if (this.resultCallback == null)
        throw new ArgumentNullException(nameof (resultCallback));
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Error while receiving entity data from the server: " + e.Message);
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      int entityId = message.ReadInt32();
      NetOutgoingMessage message1 = this.networkInterface.CreateMessage();
      message1.Write((byte) 22);
      if (!this.client.HasEntityBeenLoaded(entityId))
      {
        message1.Write(true);
        Engine.Console.LogNetworkTraffic("A request for new entity " + (object) entityId + " transaction recieved!");
        ClientEntityReceiver clientEntityReceiver = new ClientEntityReceiver(this.Key, this.client, entityId, this.resultCallback);
        this.client.RegisterNewEventHandler((EventHandler<NetConnection, NetworkClient>) clientEntityReceiver);
        this.entityRecieverHandlers.Add(clientEntityReceiver);
      }
      else
      {
        message1.Write(false);
        Engine.Console.LogNetworkTraffic("Server is sending new entity with id " + (object) entityId + " but was refused");
      }
      int num = (int) this.Key.SendMessage(message1, NetDeliveryMethod.ReliableOrdered, 6);
    }

    public List<BlockEvent> MarkEntityTransferComplete(int entityId)
    {
      ClientEntityReceiver clientEntityReceiver = this.entityRecieverHandlers.Find((Predicate<ClientEntityReceiver>) (h => h.EntityId == entityId));
      if (clientEntityReceiver == null)
        return new List<BlockEvent>();
      this.entityRecieverHandlers.Remove(clientEntityReceiver);
      clientEntityReceiver.Unregister();
      return clientEntityReceiver.UnhandledEvents;
    }

    public override void Unregister()
    {
      base.Unregister();
      this.entityRecieverHandlers.ForEach((Action<ClientEntityReceiver>) (h => h.Unregister()));
    }
  }
}
