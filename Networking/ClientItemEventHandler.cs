// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientItemEventHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientItemEventHandler : DataEventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;
    private short freeRequestId;
    private List<ClientItemEventHandler.InventorySync> inventoriesToSync;
    private Dictionary<short, Action<bool>> pendingRequests;

    public ClientItemEventHandler(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client, NetworkPeer.MessageID.GameEvent)
    {
      this.client = client;
      this.pendingRequests = new Dictionary<short, Action<bool>>();
      this.inventoriesToSync = new List<ClientItemEventHandler.InventorySync>();
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Error in clientItemEventHandler: " + e.Message);
    }

    public void RequestItemTransaction(
      int playerId,
      ItemScreen.Action request,
      Action<bool> callback)
    {
      short newRequestId = this.GetNewRequestId();
      NetOutgoingMessage eventMessageTemplate = this.client.CreateEventMessageTemplate(NetworkPeer.GameEventId.RequestItemOperation);
      eventMessageTemplate.Write(playerId);
      eventMessageTemplate.Write(newRequestId);
      eventMessageTemplate.Write((byte) request.Type);
      eventMessageTemplate.Write(request.ItemId);
      eventMessageTemplate.Write((byte) request.InventoryType);
      eventMessageTemplate.Write(request.X);
      eventMessageTemplate.Write(request.Y);
      eventMessageTemplate.Write(request.Count);
      this.pendingRequests.Remove(newRequestId);
      this.pendingRequests.Add(newRequestId, callback);
      this.client.SendOrderedDataMessage(eventMessageTemplate, (NetConnection) null, 9);
    }

    private short GetNewRequestId() => this.freeRequestId++;

    private void DenySyncrhonization(int syncId, int clientId)
    {
      NetOutgoingMessage eventMessageTemplate = this.client.CreateEventMessageTemplate(NetworkPeer.GameEventId.EndSynchronization);
      eventMessageTemplate.Write(clientId);
      eventMessageTemplate.Write(syncId);
      this.client.SendOrderedDataMessage(eventMessageTemplate, (NetConnection) null, 9);
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      switch (message.ReadByte())
      {
        case 5:
          this.HandleSetItem(message);
          break;
        case 6:
          this.HandleAck(message);
          break;
        case 7:
          this.HandleOpenContainer(message);
          break;
        case 9:
          this.HandleInventoryData(message);
          break;
        case 10:
          this.HandleInventoriesSynchronization(message);
          break;
      }
    }

    private void EndSynchronization(int clientId, int syncId)
    {
      ClientItemEventHandler.InventorySync inventorySync = this.inventoriesToSync.Find((Predicate<ClientItemEventHandler.InventorySync>) (i => i.SyncId == syncId));
      if (inventorySync == null)
        return;
      this.inventoriesToSync.Remove(inventorySync);
      inventorySync.Inventory.DataChanged -= inventorySync.DataChanged;
      this.DenySyncrhonization(syncId, clientId);
    }

    private void StartSynchronization(int clientId, int syncId, ISynchronizable inventory)
    {
      if (inventory == null)
        return;
      System.Action action = (System.Action) (() => this.SynchronizeItems(syncId, inventory.Serialize()));
      inventory.DataChanged += action;
      this.inventoriesToSync.Add(new ClientItemEventHandler.InventorySync()
      {
        Inventory = inventory,
        SyncId = syncId,
        DataChanged = action
      });
    }

    private void HandleSetItem(NetIncomingMessage message)
    {
      this.client.SignalSetItem(message.ReadInt32(), message.ReadInt32(), message.ReadInt32(), message.ReadByte(), message.ReadByte(), message.ReadByte());
    }

    private void HandleAck(NetIncomingMessage message)
    {
      short key = message.ReadInt16();
      bool flag = message.ReadBoolean();
      if (!this.pendingRequests.ContainsKey(key))
        return;
      this.pendingRequests[key](flag);
      this.pendingRequests.Remove(key);
    }

    private void HandleInventoryData(NetIncomingMessage message)
    {
      int syncId = message.ReadInt32();
      int numberOfBytes = message.ReadInt32();
      byte[] data = message.ReadBytes(numberOfBytes);
      this.inventoriesToSync.Find((Predicate<ClientItemEventHandler.InventorySync>) (i => i.SyncId == syncId))?.Inventory.Extract(data);
    }

    private void HandleInventoriesSynchronization(NetIncomingMessage message)
    {
      int inventorySyncId = message.ReadInt32();
      int toolbarSyncId = message.ReadInt32();
      int suitSyncId = message.ReadInt32();
      this.client.SignalSynchronizeInventories((Action<ISynchronizable, ISynchronizable, ISynchronizable>) ((inventory, toolbar, suit) =>
      {
        if (inventory != null)
        {
          this.StartSynchronization(this.client.LocalPlayer.Id, inventorySyncId, inventory);
        }
        else
        {
          Engine.Console.WriteErrorLine("Could not synchronize client inventory with server: null value returned!");
          this.DenySyncrhonization(inventorySyncId, this.client.LocalPlayer.Id);
        }
        if (toolbar != null)
        {
          this.StartSynchronization(this.client.LocalPlayer.Id, toolbarSyncId, toolbar);
        }
        else
        {
          Engine.Console.WriteErrorLine("Could not synchronize client toolbar with server: null value returned!");
          this.DenySyncrhonization(toolbarSyncId, this.client.LocalPlayer.Id);
        }
        if (suit != null)
        {
          this.StartSynchronization(this.client.LocalPlayer.Id, suitSyncId, suit);
        }
        else
        {
          Engine.Console.WriteErrorLine("Could not synchronize client suit with server: null value returned!");
          this.DenySyncrhonization(suitSyncId, this.client.LocalPlayer.Id);
        }
      }));
    }

    private void HandleOpenContainer(NetIncomingMessage message)
    {
      int transactionId = message.ReadInt32();
      this.client.SignalOpenContainer(message.ReadInt32(), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()), (Action<Inventory>) (inventory => this.StartSynchronization(this.client.LocalPlayer.Id, transactionId, (ISynchronizable) inventory)), (Action<int>) (clientId => this.EndSynchronization(clientId, transactionId)));
    }

    private void SynchronizeItems(int syncId, byte[] itemsData)
    {
      if (itemsData == null || itemsData.Length <= 0)
        return;
      NetOutgoingMessage eventMessageTemplate = this.client.CreateEventMessageTemplate(NetworkPeer.GameEventId.InventoryData);
      eventMessageTemplate.Write(syncId);
      eventMessageTemplate.Write(itemsData.Length);
      eventMessageTemplate.Write(itemsData);
      this.client.SendOrderedDataMessage(eventMessageTemplate, (NetConnection) null, 9);
    }

    private class InventorySync
    {
      public ISynchronizable Inventory;
      public int SyncId;
      public System.Action DataChanged;
    }
  }
}
