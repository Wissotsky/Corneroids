// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerDataSynchronizer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerDataSynchronizer : DataEventHandler<NetConnection, NetworkServer>
  {
    private IdGen idGenerator;
    private NetworkServerManager server;
    private List<ServerDataSynchronizer.DataSync> inventoriesToSync;

    public ServerDataSynchronizer(NetworkServerManager server)
      : base((NetConnection) null, (NetworkServer) server, NetworkPeer.MessageID.GameEvent)
    {
      this.idGenerator = new IdGen(0);
      this.server = server;
      this.inventoriesToSync = new List<ServerDataSynchronizer.DataSync>();
    }

    public override void ClientDisconnected(int clientId)
    {
      this.RemoveSynchronizationClientId(clientId);
    }

    public override void ErrorCaught(Exception e)
    {
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      switch (message.ReadByte())
      {
        case 8:
          this.EndSynchronization(message);
          break;
        case 9:
          this.HandleInventoryData(message);
          break;
      }
    }

    public void SendOpenContainer(Player player, ContainerBlock container)
    {
      ServerToClientConnection connectionOfPlayer = this.server.GetConnectionOfPlayer(player);
      if (connectionOfPlayer == null)
        return;
      NetOutgoingMessage eventMessageTemplate = this.server.CreateEventMessageTemplate(NetworkPeer.GameEventId.OpenContainer);
      int newId = this.idGenerator.GetNewId();
      eventMessageTemplate.Write(newId);
      eventMessageTemplate.Write(container.OwnerEntity.Id);
      eventMessageTemplate.Write(container.PositionInEntitySpace.X);
      eventMessageTemplate.Write(container.PositionInEntitySpace.Y);
      eventMessageTemplate.Write(container.PositionInEntitySpace.Z);
      this.server.SendOrderedDataMessage(eventMessageTemplate, connectionOfPlayer.Connection, 9);
      this.InitializeSynchronization(connectionOfPlayer.Connection, player.Id, (ISynchronizable) container.Inventory, newId);
    }

    public void SynchronizeControlBlock(NetworkPlayer player, ControlBlock controlBlock)
    {
      if (player == null)
        ;
    }

    public void SynchronizePlayerInventories(NetworkPlayer player)
    {
      ServerToClientConnection connectionOfPlayer = this.server.GetConnectionOfPlayer((Player) player);
      if (connectionOfPlayer == null)
        return;
      NetOutgoingMessage eventMessageTemplate = this.server.CreateEventMessageTemplate(NetworkPeer.GameEventId.SynchronizeInventories);
      int newId1 = this.idGenerator.GetNewId();
      int newId2 = this.idGenerator.GetNewId();
      int newId3 = this.idGenerator.GetNewId();
      eventMessageTemplate.Write(newId1);
      eventMessageTemplate.Write(newId2);
      eventMessageTemplate.Write(newId3);
      this.server.SendOrderedDataMessage(eventMessageTemplate, connectionOfPlayer.Connection, 9);
      this.InitializeSynchronization(connectionOfPlayer.Connection, player.Id, (ISynchronizable) player.Inventory, newId1);
      this.InitializeSynchronization(connectionOfPlayer.Connection, player.Id, (ISynchronizable) player.Toolbar.Items, newId2);
      this.InitializeSynchronization(connectionOfPlayer.Connection, player.Id, (ISynchronizable) player.Astronaut.Suit, newId3);
    }

    private void AckItemRequest(int playerId, bool result, short requestId)
    {
      ServerToClientConnection connectionOfId = this.server.GetConnectionOfId(playerId);
      if (connectionOfId == null)
        return;
      NetOutgoingMessage eventMessageTemplate = this.server.CreateEventMessageTemplate(NetworkPeer.GameEventId.AckItemRequest);
      eventMessageTemplate.Write(requestId);
      eventMessageTemplate.Write(result);
      this.server.SendOrderedDataMessage(eventMessageTemplate, connectionOfId.Connection, 9);
    }

    private void EndSynchronization(NetIncomingMessage message)
    {
      this.RemoveSynchronization(message.ReadInt32(), message.ReadInt32());
    }

    private void HandleInventoryData(NetIncomingMessage message)
    {
      int syncId = message.ReadInt32();
      int numberOfBytes = message.ReadInt32();
      byte[] data = message.ReadBytes(numberOfBytes);
      this.inventoriesToSync.Find((Predicate<ServerDataSynchronizer.DataSync>) (i => i.SyncId == syncId))?.DataToSync.Extract(data);
    }

    private void InitializeSynchronization(
      NetConnection client,
      int clientId,
      ISynchronizable inventory,
      int syncId)
    {
      System.Action action = (System.Action) (() => this.SynchronizeItems(client, syncId, inventory.Serialize()));
      inventory.DataChanged += action;
      this.inventoriesToSync.Add(new ServerDataSynchronizer.DataSync()
      {
        DataToSync = inventory,
        SyncId = syncId,
        Client = client,
        ClientId = clientId,
        DataChanged = action
      });
      this.SynchronizeItems(client, syncId, inventory.Serialize());
    }

    private void RemoveSynchronizationClientId(int clientId)
    {
      foreach (ServerDataSynchronizer.DataSync container in this.inventoriesToSync.Where<ServerDataSynchronizer.DataSync>((Func<ServerDataSynchronizer.DataSync, bool>) (i => i.ClientId == clientId)).ToList<ServerDataSynchronizer.DataSync>())
        this.RemoveSynchronization(container);
    }

    private void RemoveSynchronizationSyncId(int syncId)
    {
      foreach (ServerDataSynchronizer.DataSync container in this.inventoriesToSync.Where<ServerDataSynchronizer.DataSync>((Func<ServerDataSynchronizer.DataSync, bool>) (i => i.SyncId == syncId)).ToList<ServerDataSynchronizer.DataSync>())
        this.RemoveSynchronization(container);
    }

    private void RemoveSynchronization(int clientId, int syncId)
    {
      this.RemoveSynchronization(this.inventoriesToSync.Find((Predicate<ServerDataSynchronizer.DataSync>) (i => i.SyncId == syncId && i.ClientId == clientId)));
    }

    private void RemoveSynchronization(ServerDataSynchronizer.DataSync container)
    {
      if (container == null)
        return;
      container.DataToSync.DataChanged -= container.DataChanged;
      this.idGenerator.ReleaseID(container.SyncId);
      this.inventoriesToSync.Remove(container);
    }

    private void SynchronizeItems(NetConnection connection, int syncId, byte[] itemsData)
    {
      if (connection == null || itemsData == null || itemsData.Length <= 0)
        return;
      NetOutgoingMessage eventMessageTemplate = this.server.CreateEventMessageTemplate(NetworkPeer.GameEventId.InventoryData);
      eventMessageTemplate.Write(syncId);
      eventMessageTemplate.Write(itemsData.Length);
      eventMessageTemplate.Write(itemsData);
      this.server.SendOrderedDataMessage(eventMessageTemplate, connection, 9);
    }

    private class DataSync
    {
      public ISynchronizable DataToSync;
      public NetConnection Client;
      public int ClientId;
      public int SyncId;
      public System.Action DataChanged;
    }
  }
}
