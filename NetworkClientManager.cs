// Decompiled with JetBrains decompiler
// Type: CornerSpace.NetworkClientManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Networking;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class NetworkClientManager : NetworkClient, IPlayerList
  {
    private const int cRate = 40;
    private List<Player> connectedPlayers;
    private ClientEntityDataManager entityDataManager;
    private ClientControlDataListener controlDataHandler;
    private EntityExtractor entityExtractor;
    private ClientItemEventHandler itemEventHandler;
    private List<SpaceEntity> spaceEntities;
    private LocalClientPlayer localPlayer;

    public NetworkClientManager(int portNumber, string clientName, int version)
      : base(version, portNumber, clientName)
    {
      this.connectedPlayers = new List<Player>();
      this.spaceEntities = new List<SpaceEntity>();
      this.entityExtractor = new EntityExtractor();
    }

    public void Chat(string message)
    {
      if (string.IsNullOrEmpty(message))
        return;
      string chatMessage = this.ParseChatMessage(message);
      if (string.IsNullOrEmpty(chatMessage))
        return;
      NetOutgoingMessage message1 = this.CreateMessage();
      message1.Write((byte) 14);
      message1.Write(chatMessage);
      this.SendDataMessage(message1, this.connection);
    }

    public override bool ConnectTo(string address, string password, string token)
    {
      if (!base.ConnectTo(address, password, token))
        return false;
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientReceiveWorldDataHandler(this.connection, this));
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientChatMessageHandler(this));
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientPlayerUpdateListener(this));
      this.itemEventHandler = new ClientItemEventHandler(this);
      this.controlDataHandler = new ClientControlDataListener(this);
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) this.itemEventHandler);
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) this.controlDataHandler);
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientEntityDataListener(this));
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientBlockDataHandler(this));
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientControlDataListener(this));
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientEntityStateReceiver(this));
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientProjectileDataHandler(this));
      this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientGameEventListener(this));
      return true;
    }

    public void Dispose()
    {
      this.Disconnect();
      this.entityExtractor.Dispose();
    }

    public bool HasEntityBeenLoaded(int entityId)
    {
      return this.spaceEntities.Find((Predicate<SpaceEntity>) (e => e.Id == entityId)) != null;
    }

    public override bool Prepare()
    {
      if (!base.Prepare())
        return false;
      this.SubscribeToEvents();
      return true;
    }

    public void SendPlayerState(CornerSpace.LocalPlayer player)
    {
      if (player == null)
        return;
      int id = player.Id;
      SpaceEntity boardedEntity = player.Astronaut.BoardedEntity;
      Quaternion orientation = player.Astronaut.UsedCamera.Orientation;
      Quaternion coordinateSpace = player.Astronaut.CoordinateSpace;
      StateFrame latestStateFrame = player.LatestStateFrame;
      NetOutgoingMessage message = this.CreateMessage();
      message.Write((byte) 16);
      message.Write(player.Id);
      message.Write(boardedEntity != null ? boardedEntity.Id : -1);
      message.Write(orientation.X);
      message.Write(orientation.Y);
      message.Write(orientation.Z);
      message.Write(orientation.W);
      message.Write(latestStateFrame.ActiveToolbarIndex);
      message.Write(latestStateFrame.ButtonsStates);
      message.Write(latestStateFrame.ControlButtonsStates);
      message.Write(latestStateFrame.SimulationTime);
      message.Write(latestStateFrame.Tick);
      this.SendDataMessage(message, this.connection);
    }

    public override void Update(int deltaTimeMS)
    {
      base.Update(deltaTimeMS);
      if (this.IsMessageCycle() && this.StateUpdateTick != null)
        this.StateUpdateTick();
      SpaceEntity spaceEntity;
      while ((spaceEntity = this.entityExtractor.FinishedEntity()) != null)
      {
        foreach (BlockEvent blockEvent in this.entityDataManager.MarkEntityTransferComplete(spaceEntity.Id))
        {
          if (blockEvent.BlockAction == BlockEvent.Action.Add)
          {
            Block blockById = Engine.LoadedWorld.Itemset.GetBlockByID(blockEvent.BlockId);
            if (blockById != null)
            {
              blockById.Orientation = blockEvent.Orientation;
              spaceEntity.AddBlockEntitySpace(blockById, blockEvent.Position);
            }
          }
          else if (blockEvent.BlockAction == BlockEvent.Action.Remove)
            spaceEntity.RemoveBlockEntitySpace(blockEvent.Position);
        }
        if (this.NewEntityCreatedEvent != null)
          this.NewEntityCreatedEvent(spaceEntity);
        else
          spaceEntity.Dispose();
      }
    }

    public void WorldDataLoaded(
      Itemset itemset,
      SpriteTextureAtlas spriteTextureAtlas,
      BlockTextureAtlas blockTextureAtlas,
      PlayerSerializedData playerItemData)
    {
      if (this.WorldDataReceivedEvent != null)
      {
        this.WorldDataReceivedEvent(itemset, spriteTextureAtlas, blockTextureAtlas, playerItemData);
      }
      else
      {
        spriteTextureAtlas?.Dispose();
        blockTextureAtlas?.Dispose();
      }
    }

    public void RequestBindBlockToConsole(
      ControlBlock console,
      TriggerBlock target,
      Color newColor)
    {
      this.controlDataHandler.RequestBindBlockToConsole(console, target, newColor);
    }

    public void RequestChangeConsoleColorKey(ControlBlock console, ColorKeyGroup newKey)
    {
      this.controlDataHandler.RequestChangeColorKey(console, newKey);
    }

    public void RequestSpawn(CornerSpace.LocalPlayer player)
    {
      if (player == null)
        return;
      int id = player.Id;
      NetOutgoingMessage eventMessageTemplate = this.CreateEventMessageTemplate(NetworkPeer.GameEventId.RequestSpawn);
      eventMessageTemplate.Write(id);
      this.SendOrderedDataMessage(eventMessageTemplate, this.connection, 8);
    }

    public void RequestTossItem(ItemSlot item, Position3 position, Vector3 speed)
    {
      if (item == null || !(item.Item != (Item) null) || item.Count <= 0)
        return;
      NetOutgoingMessage eventMessageTemplate = this.CreateEventMessageTemplate(NetworkPeer.GameEventId.RequestTossItem);
      eventMessageTemplate.Write(item.Item.ItemId);
      eventMessageTemplate.Write(item.Count);
      eventMessageTemplate.Write(position.X);
      eventMessageTemplate.Write(position.Y);
      eventMessageTemplate.Write(position.Z);
      eventMessageTemplate.Write(speed.X);
      eventMessageTemplate.Write(speed.Y);
      eventMessageTemplate.Write(speed.Z);
      this.SendOrderedDataMessage(eventMessageTemplate, (NetConnection) null, 8);
    }

    public void RequestUnbindBlockFromConsole(ControlBlock console, TriggerBlock target)
    {
      this.controlDataHandler.RequestUnbindBlockFromConsole(console, target);
    }

    public void SignalAddFloatingItem(
      int itemId,
      int quantity,
      int environmentId,
      Position3 position,
      Vector3 speed)
    {
      if (this.NewItemSpawned == null)
        return;
      Item obj = Engine.LoadedWorld.Itemset.GetItem(itemId);
      if (!(obj != (Item) null))
        return;
      this.NewItemSpawned(new ItemSlot(obj, quantity), position, speed, environmentId);
    }

    public void SignalAstronautKilled(int playerId)
    {
      if (this.PlayerAstronautKilled == null)
        return;
      Player player = this.connectedPlayers.Find((Predicate<Player>) (p => p.Id == playerId));
      if (player == null)
        return;
      this.PlayerAstronautKilled(player);
    }

    public void SignalChatMessageReceived(string message, int senderId)
    {
      if (string.IsNullOrEmpty(message) || this.ChatMessageReceivedEvent == null)
        return;
      Player player = this.GetPlayer(senderId);
      if (player == null)
        return;
      this.ChatMessageReceivedEvent(player, message);
    }

    public void SignalControlBlockBlockUnbound(
      int entityId,
      Vector3 consolePosition,
      Vector3 triggerPosition)
    {
      if (this.ControlBlockBlockUnbound == null)
        return;
      this.ControlBlockBlockUnbound(entityId, consolePosition, triggerPosition);
    }

    public void SignalControlBlockNewBind(
      int entityId,
      Vector3 consolePosition,
      Vector3 triggerPosition,
      Color newKey)
    {
      if (this.ControlBlockNewBind == null)
        return;
      this.ControlBlockNewBind(entityId, consolePosition, triggerPosition, newKey);
    }

    public void SignalControlBlockKeyChanged(
      int entityId,
      Vector3 controlBlockPosition,
      Color color,
      Keys key)
    {
      if (this.ControlBlockKeyChanged == null)
        return;
      this.ControlBlockKeyChanged(entityId, controlBlockPosition, color, key);
    }

    public void SignalCreateProjectile(byte projectileType, Position3 position, Vector3 direction)
    {
      if (this.NewProjectileCreated == null)
        return;
      this.NewProjectileCreated(projectileType, position, direction);
    }

    public void SignalEntityCreated(int id, Position3 position)
    {
      if (this.NewEntityCreatedEvent == null)
        return;
      SpaceEntity spaceEntity1 = new SpaceEntity();
      spaceEntity1.Id = id;
      spaceEntity1.Position = position;
      SpaceEntity spaceEntity2 = spaceEntity1;
      this.spaceEntities.Add(spaceEntity2);
      this.NewEntityCreatedEvent(spaceEntity2);
    }

    public void SignalEntityRemoved(int id)
    {
      if (this.EntityRemoved == null)
        return;
      this.EntityRemoved(id);
    }

    public void SignalEntityStateReceived(
      EntityStateContainer entityState,
      ControlBlocksStates controlBlocksState)
    {
      if (this.EntityStateUpdateEvent == null)
        return;
      this.EntityStateUpdateEvent(entityState, controlBlocksState);
    }

    public void SignalItemPickedByPlayer(int playerId, int itemId, int quantity, int envId)
    {
      if (this.ItemPickedUp == null)
        return;
      Item obj = Engine.LoadedWorld.Itemset.GetItem(itemId);
      Player player = this.connectedPlayers.Find((Predicate<Player>) (p => p.Id == playerId));
      if (!(obj != (Item) null) || player == null)
        return;
      ItemSlot itemSlot = new ItemSlot(obj, quantity);
      this.ItemPickedUp(player, itemSlot, envId);
    }

    public void SignalNewBlockAdded(
      int entityId,
      byte blockId,
      byte orientation,
      Vector3 position)
    {
      if (this.NewBlockAddedEvent == null)
        return;
      this.NewBlockAddedEvent(entityId, blockId, orientation, position);
    }

    public void SignalOpenContainer(
      int entityId,
      Vector3 blockPosition,
      Action<Inventory> inventoryCallback,
      Action<int> closedCallback)
    {
      if (this.OpenContainer != null)
        this.OpenContainer(entityId, blockPosition, inventoryCallback, closedCallback);
      else
        closedCallback(this.LocalPlayer.Id);
    }

    public void SignalRemoveBlock(int entityId, Vector3 position, bool destroy)
    {
      if (this.BlockRemovedEvent == null)
        return;
      this.BlockRemovedEvent(entityId, position, destroy);
    }

    public void SignalPlayerMountedStatusChanged(
      int playerId,
      int entityId,
      Vector3 position,
      bool isMounted)
    {
      if (this.PlayerMountedStatusChanged == null)
        return;
      this.PlayerMountedStatusChanged(playerId, entityId, position, isMounted);
    }

    public void SignalPlayerSpawned(int playerId, Position3 spawnPosition)
    {
      if (this.PlayerSpawned == null)
        return;
      this.PlayerSpawned(playerId, spawnPosition);
    }

    public void SignalPlayerStateReceived(int id, StateFrame frame)
    {
      if (this.PlayerStateUpdateReceivedEvent == null)
        return;
      Player player = this.GetPlayer(id);
      if (player == null)
        return;
      this.PlayerStateUpdateReceivedEvent(player, frame);
    }

    public void SignalSetItem(
      int playerId,
      int itemId,
      int quantity,
      byte slotX,
      byte slotY,
      byte inventoryType)
    {
      if (this.SetItem == null)
        return;
      Player player = this.GetPlayer(playerId);
      if (player == null)
        return;
      Item obj = Engine.LoadedWorld.Itemset.GetItem(itemId);
      Player.ItemModifiedArgs itemModifiedArgs = new Player.ItemModifiedArgs()
      {
        ItemSlot = new ItemSlot(obj, quantity),
        InventoryType = (Inventory.Type) inventoryType,
        X = (int) slotX,
        Y = (int) slotY
      };
      this.SetItem(player, itemModifiedArgs);
    }

    public void SignalSynchronizeInventories(
      Action<ISynchronizable, ISynchronizable, ISynchronizable> inventoriesCallback)
    {
      if (this.SynchronizeInventories != null)
        this.SynchronizeInventories(inventoriesCallback);
      else
        inventoriesCallback((ISynchronizable) null, (ISynchronizable) null, (ISynchronizable) null);
    }

    public IEnumerable<Player> Players => (IEnumerable<Player>) this.connectedPlayers;

    public LocalClientPlayer LocalPlayer => this.localPlayer;

    public event Action<Player, string> ChatMessageReceivedEvent;

    public event Action<int, Vector3, bool> BlockRemovedEvent;

    public event Action<LocalClientPlayer> ConnectedToSeverEvent;

    public event Action<int, Vector3, Vector3> ControlBlockBlockUnbound;

    public event Action<int, Vector3, Vector3, Color> ControlBlockNewBind;

    public event Action<int, Vector3, Color, Keys> ControlBlockKeyChanged;

    public event Action<int> EntityRemoved;

    public event Action<EntityStateContainer, ControlBlocksStates> EntityStateUpdateEvent;

    public event Action<Player, ItemSlot, int> ItemPickedUp;

    public event Action<int, byte, byte, Vector3> NewBlockAddedEvent;

    public event Action<ItemSlot, Position3, Vector3, int> NewItemSpawned;

    public event Action<SpaceEntity> NewEntityCreatedEvent;

    public event Action<byte, Position3, Vector3> NewProjectileCreated;

    public event Action<int, Vector3, Action<Inventory>, Action<int>> OpenContainer;

    public event Action<Player> PlayerAstronautKilled;

    public event Action<int, int, Vector3, bool> PlayerMountedStatusChanged;

    public event Action<NetworkPlayer> PlayerConnected;

    public event Action<Player, string> PlayerDisconnected;

    public event Action<int, Position3> PlayerSpawned;

    public event Action<Player, StateFrame> PlayerStateUpdateReceivedEvent;

    public event Action<Player, Player.ItemModifiedArgs> SetItem;

    public event System.Action StateUpdateTick;

    public event Action<Action<ISynchronizable, ISynchronizable, ISynchronizable>> SynchronizeInventories;

    public event Action<Itemset, SpriteTextureAtlas, BlockTextureAtlas, PlayerSerializedData> WorldDataReceivedEvent;

    private Player GetPlayer(int id)
    {
      return this.connectedPlayers.Find((Predicate<Player>) (p => p.Id == id));
    }

    private void ParseNewBlockData(NetIncomingMessage message)
    {
      if (this.NewBlockAddedEvent == null)
        return;
      this.NewBlockAddedEvent(message.ReadInt32(), message.ReadByte(), message.ReadByte(), new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat()));
    }

    private void ParseNewEntityData(NetIncomingMessage message)
    {
      if (message == null || this.NewEntityCreatedEvent == null)
        return;
      int num = message.ReadInt32();
      Position3 position3 = new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64());
      SpaceEntity spaceEntity = new SpaceEntity();
      spaceEntity.Id = num;
      spaceEntity.Position = position3;
      this.NewEntityCreatedEvent(spaceEntity);
    }

    private void SubscribeToEvents()
    {
      this.ConnectedEvent += (Action<int>) (id =>
      {
        if (this.ConnectedToSeverEvent == null)
          return;
        LocalClientPlayer localClientPlayer = new LocalClientPlayer()
        {
          Id = id,
          Name = this.ClientName
        };
        this.localPlayer = localClientPlayer;
        this.connectedPlayers.Add((Player) localClientPlayer);
        this.entityDataManager = new ClientEntityDataManager(this.connection, this, (Action<PackedEntityContainer>) (receivedEntity => this.entityExtractor.ExtractEntity(receivedEntity)));
        this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) this.entityDataManager);
        this.ConnectedToSeverEvent(localClientPlayer);
      });
      this.OtherClientConnectedEvent += (Action<int, string>) ((id, name) =>
      {
        NetworkPlayerOnClient networkPlayerOnClient = new NetworkPlayerOnClient()
        {
          Id = id,
          Name = name
        };
        this.connectedPlayers.Add((Player) networkPlayerOnClient);
        if (this.PlayerConnected == null)
          return;
        this.PlayerConnected((NetworkPlayer) networkPlayerOnClient);
      });
      this.OtherClientDisconnectedEvent += (Action<int, string>) ((id, reason) =>
      {
        Player player = this.GetPlayer(id);
        if (player == null)
          return;
        this.connectedPlayers.Remove(player);
        if (this.PlayerDisconnected == null)
          return;
        this.PlayerDisconnected(player, reason);
      });
    }
  }
}
