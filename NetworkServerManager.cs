// Decompiled with JetBrains decompiler
// Type: CornerSpace.NetworkServerManager
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
using System.Linq;

#nullable disable
namespace CornerSpace
{
  public class NetworkServerManager : NetworkServer, IDisposable, IPlayerList
  {
    private List<Player> connectedPlayers;
    private LocalPlayer localPlayer;
    private StateFrame[] stateFrames;
    private World world;
    private EntityTransferHelper entityTransferHelper;
    private ServerControlDataHandler controlDataHandler;
    private ServerDataSynchronizer dataSynchronizer;

    public NetworkServerManager(
      World playedWorld,
      int maxPlayers,
      int port,
      string serverName,
      string password)
      : base(Engine.GameVersion, maxPlayers, port, serverName, playedWorld.Token)
    {
      this.Password = password;
      this.connectedPlayers = new List<Player>();
      this.stateFrames = new StateFrame[64];
      this.world = playedWorld;
      this.entityTransferHelper = new EntityTransferHelper(this);
    }

    public void Chat(string message)
    {
      message = this.ParseChatMessage(message);
      if (string.IsNullOrEmpty(message))
        return;
      NetOutgoingMessage message1 = this.CreateMessage();
      message1.Write((byte) 14);
      message1.Write(this.localPlayer.Id);
      message1.Write(message);
      this.SendDataMessage(message1, (NetConnection) null);
    }

    public void BeginEntityTransaction(NetworkPlayer player, SpaceEntity entity)
    {
      if (entity == null || player == null)
        return;
      this.entityTransferHelper.EnqueueEntityForTransfer(entity, player);
    }

    public override bool ClientNameAvailable(string name)
    {
      return !string.IsNullOrEmpty(name) && this.connectedPlayers.Find((Predicate<Player>) (c => c.Name == name)) == null;
    }

    public virtual void Dispose()
    {
      this.Disconnect();
      this.entityTransferHelper.Dispose();
    }

    public override bool Prepare()
    {
      if (!base.Prepare())
        return false;
      this.RegisterNewEventHandler((EventHandler<NetConnection, NetworkServer>) new ServerChatMessageHandler(this));
      this.RegisterNewEventHandler((EventHandler<NetConnection, NetworkServer>) new ServerPlayerUpdateListener(this));
      this.RegisterNewEventHandler((EventHandler<NetConnection, NetworkServer>) new ServerPlayerRequestListener(this));
      this.dataSynchronizer = new ServerDataSynchronizer(this);
      this.controlDataHandler = new ServerControlDataHandler(this);
      this.RegisterNewEventHandler((EventHandler<NetConnection, NetworkServer>) this.dataSynchronizer);
      this.RegisterNewEventHandler((EventHandler<NetConnection, NetworkServer>) this.controlDataHandler);
      this.InitializeEvents();
      return true;
    }

    public void SetLocalPlayer(LocalPlayer player)
    {
      this.localPlayer = player;
      this.connectedPlayers.Add((Player) player);
      this.OwnerName = this.localPlayer.Name;
    }

    public void SignalChatMessageReceived(string message, NetConnection sender)
    {
      if (string.IsNullOrEmpty(message) || sender == null)
        return;
      ServerToClientConnection connection = this.GetConnection(sender);
      if (connection == null)
        return;
      if (this.ChatMessageReceivedEvent != null)
        this.ChatMessageReceivedEvent(this.GetPlayer(connection.Id), message);
      foreach (ServerToClientConnection clientConnection in this.connections.Where<ServerToClientConnection>((Func<ServerToClientConnection, bool>) (c => c.Connection != sender)))
      {
        NetOutgoingMessage message1 = this.CreateMessage();
        message1.Write((byte) 14);
        message1.Write(connection.Id);
        message1.Write(message);
        this.SendDataMessage(message1, clientConnection.Connection);
      }
    }

    public void SignalPlayerStateReceived(int id, StateFrame state)
    {
      if (this.PlayerStatusUpdateReceived == null)
        return;
      Player player = this.GetPlayer(id);
      if (player == null)
        return;
      this.PlayerStatusUpdateReceived(player, state);
    }

    public override void Disconnect()
    {
      base.Disconnect();
      this.connectedPlayers.Clear();
    }

    public override void Update(int elapsedMilliSeconds)
    {
      base.Update(elapsedMilliSeconds);
      if (this.IsMessageCycle() && this.StateUpdateTick != null)
        this.StateUpdateTick();
      this.ManageEntityTransactions();
    }

    public void SendAddFloatingItem(ItemSlot item, int itemId, Position3 position, Vector3 speed)
    {
      if (item == null || !(item.Item != (Item) null) || item.Count <= 0)
        return;
      NetOutgoingMessage eventMessageTemplate = this.CreateEventMessageTemplate(NetworkPeer.GameEventId.AddItem);
      eventMessageTemplate.Write(itemId);
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

    public void SendControlBlockBlockUnbound(ControlBlock control, TriggerBlock trigger)
    {
      this.controlDataHandler.ControlBlockBlockUnbound(control, trigger);
    }

    public void SendControlBlockNewBlockBound(
      ControlBlock console,
      TriggerBlock block,
      Color newColor)
    {
      this.controlDataHandler.ControlBlockNewBlockBound(console, block, newColor);
    }

    public void SendControlBlockKeyChanged(ControlBlock block, ColorKeyGroup newKey)
    {
      this.controlDataHandler.ControlBlockKeyChanged(block, newKey);
    }

    public void SendCreateBlock(
      SpaceEntity entity,
      Block block,
      Vector3 entityPosition,
      Player creator)
    {
      if (entity == null || block == null)
        return;
      int id1 = entity.Id;
      byte id2 = block.Id;
      byte orientation = block.Orientation;
      NetOutgoingMessage message = this.CreateMessage();
      message.Write((byte) 19);
      message.Write(creator != null ? creator.Id : -1);
      message.Write(id1);
      message.Write(id2);
      message.Write(orientation);
      message.Write(entityPosition.X);
      message.Write(entityPosition.Y);
      message.Write(entityPosition.Z);
      this.SendOrderedDataMessage(message, (NetConnection) null, 4);
    }

    public void SendCreateEntity(NetworkPlayer client, SpaceEntity entity)
    {
      if (entity == null)
        return;
      NetOutgoingMessage message = this.CreateMessage();
      Position3 position = entity.Position;
      int id = entity.Id;
      message.Write((byte) 17);
      message.Write(id);
      message.Write(position.X);
      message.Write(position.Y);
      message.Write(position.Z);
      NetConnection receiver = (NetConnection) null;
      if (client != null)
      {
        ServerToClientConnection connectionOfId = this.GetConnectionOfId(client.Id);
        if (connectionOfId != null)
          receiver = connectionOfId.Connection;
      }
      this.SendOrderedDataMessage(message, receiver, 4);
    }

    public void SendCreateProjectile(Projectile projectile, Vector3 positionCorrection)
    {
      if (projectile == null)
        return;
      Position3 position3 = projectile.Position + positionCorrection;
      Vector3 direction = projectile.Direction;
      NetOutgoingMessage message = this.CreateMessage();
      message.Write((byte) 34);
      message.Write(projectile.ProjectileType.Id);
      message.Write(position3.X);
      message.Write(position3.Y);
      message.Write(position3.Z);
      message.Write(direction.X);
      message.Write(direction.Y);
      message.Write(direction.Z);
      this.SendDataMessage(message, (NetConnection) null);
    }

    public void SendEntityState(SpaceEntity entity, NetworkPlayer player)
    {
      if (entity == null || player == null)
        return;
      ServerToClientConnection connectionOfId = this.GetConnectionOfId(player.Id);
      if (connectionOfId == null)
        return;
      int id = entity.Id;
      Position3 position = entity.Position;
      Quaternion orientation = entity.Orientation;
      int blockCount = (int) entity.BlockCount;
      Vector3 centerOfMass = entity.CenterOfMass;
      NetOutgoingMessage message = this.CreateMessage();
      message.Write((byte) 32);
      message.Write(id);
      message.Write(Engine.FrameCounter.Ticks);
      message.Write(position.X);
      message.Write(position.Y);
      message.Write(position.Z);
      message.Write(orientation.X);
      message.Write(orientation.Y);
      message.Write(orientation.Z);
      message.Write(orientation.W);
      message.Write(centerOfMass.X);
      message.Write(centerOfMass.Y);
      message.Write(centerOfMass.Z);
      this.WriteActiveControlBlocksToMessage(message, entity);
      this.SendDataMessage(message, connectionOfId.Connection);
    }

    public void SendKillPlayer(Player player)
    {
      if (player == null)
        return;
      NetOutgoingMessage eventMessageTemplate = this.CreateEventMessageTemplate(NetworkPeer.GameEventId.KillPlayer);
      eventMessageTemplate.Write(player.Id);
      this.SendOrderedDataMessage(eventMessageTemplate, (NetConnection) null, 8);
    }

    public void SendItemPickedByPlayer(Player player, ItemSlot item, int environmentId)
    {
      if (player == null || item == null || !(item.Item != (Item) null) || item.Count <= 0)
        return;
      NetOutgoingMessage eventMessageTemplate = this.CreateEventMessageTemplate(NetworkPeer.GameEventId.PickItem);
      eventMessageTemplate.Write(player.Id);
      eventMessageTemplate.Write(item.Item.ItemId);
      eventMessageTemplate.Write(item.Count);
      eventMessageTemplate.Write(environmentId);
      this.SendOrderedDataMessage(eventMessageTemplate, (NetConnection) null, 8);
    }

    public void SendOpenContainer(Player player, ContainerBlock block)
    {
      if (player == null || block == null)
        return;
      this.dataSynchronizer.SendOpenContainer(player, block);
    }

    public void SendPlayerMountedStatusChanged(Player player, ControlBlock block, bool isMounted)
    {
      try
      {
        if (player == null || block == null)
          return;
        int id = block.OwnerSector.Owner.Id;
        Vector3 positionInEntitySpace = block.PositionInEntitySpace;
        NetOutgoingMessage message = this.CreateMessage();
        message.Write((byte) 33);
        message.Write(id);
        message.Write(positionInEntitySpace.X);
        message.Write(positionInEntitySpace.Y);
        message.Write(positionInEntitySpace.Z);
        message.Write(player.Id);
        message.Write(isMounted);
        this.SendOrderedDataMessage(message, (NetConnection) null, 7);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to send mount information to clients: " + ex.Message);
      }
    }

    public void SendPlayerState(Player player)
    {
      if (player == null)
        return;
      NetOutgoingMessage message = this.CreateMessage();
      message.Write((byte) 15);
      Astronaut astronaut = player.Astronaut;
      Item activeItem = astronaut.ActiveItem;
      Position3 position = astronaut.Position;
      Vector3 positionOnBoardedEntity = astronaut.PositionOnBoardedEntity;
      SpaceEntity boardedEntity = astronaut.BoardedEntity;
      short health = astronaut.Health;
      Quaternion orientation = astronaut.UsedCamera.Orientation;
      float powerPreUpdate = astronaut.PowerPreUpdate;
      Vector3 speed = astronaut.Speed;
      Vector3 speedOnBoardedEntity = astronaut.SpeedOnBoardedEntity;
      long latestMessageTicks = player.LatestMessageTicks;
      StateFrame latestStateFrame = player.LatestStateFrame;
      uint storedValue = player.SynchronizationValue.StoredValue;
      message.Write(player.Id);
      message.Write(latestMessageTicks);
      message.Write(activeItem != (Item) null ? activeItem.ItemId : -1);
      message.Write(latestStateFrame.ButtonsStates);
      message.Write(storedValue);
      message.Write(health);
      message.Write(powerPreUpdate);
      message.Write(boardedEntity != null ? boardedEntity.Id : -1);
      if (boardedEntity == null)
      {
        message.Write(position.X);
        message.Write(position.Y);
        message.Write(position.Z);
        message.Write(speed.X);
        message.Write(speed.Y);
        message.Write(speed.Z);
      }
      else
      {
        message.Write(positionOnBoardedEntity.X);
        message.Write(positionOnBoardedEntity.Y);
        message.Write(positionOnBoardedEntity.Z);
        message.Write(speedOnBoardedEntity.X);
        message.Write(speedOnBoardedEntity.Y);
        message.Write(speedOnBoardedEntity.Z);
      }
      message.Write(orientation.X);
      message.Write(orientation.Y);
      message.Write(orientation.Z);
      message.Write(orientation.W);
      this.SendDataMessage(message, (NetConnection) null);
    }

    public void SendRemoveBlock(SpaceEntity entity, Vector3 position, bool destroy, int senderId)
    {
      if (entity == null)
        return;
      NetOutgoingMessage message = this.CreateMessage();
      message.Write((byte) 20);
      message.Write(senderId);
      message.Write(entity.Id);
      message.Write(position.X);
      message.Write(position.Y);
      message.Write(position.Z);
      message.Write(destroy);
      this.SendOrderedDataMessage(message, (NetConnection) null, 4);
    }

    public void SendRemoveEntity(NetworkPlayer player, int entityId)
    {
      ServerToClientConnection connectionOfPlayer = this.GetConnectionOfPlayer((Player) player);
      NetOutgoingMessage messageTemplate = this.CreateMessageTemplate(NetworkPeer.MessageID.RemoveEntity);
      messageTemplate.Write(entityId);
      this.SendOrderedDataMessage(messageTemplate, connectionOfPlayer?.Connection, 4);
    }

    public void SendSetItem(Player player, Player.ItemModifiedArgs itemInfo)
    {
      if (player == null || itemInfo == null)
        return;
      ServerToClientConnection connectionOfPlayer = this.GetConnectionOfPlayer(player);
      if (connectionOfPlayer == null)
        return;
      NetOutgoingMessage eventMessageTemplate = this.CreateEventMessageTemplate(NetworkPeer.GameEventId.SetItem);
      eventMessageTemplate.Write(player.Id);
      Item obj = itemInfo.ItemSlot != null ? itemInfo.ItemSlot.Item : (Item) null;
      eventMessageTemplate.Write(obj != (Item) null ? obj.ItemId : -1);
      eventMessageTemplate.Write(itemInfo.ItemSlot != null ? itemInfo.ItemSlot.Count : 0);
      eventMessageTemplate.Write((byte) itemInfo.X);
      eventMessageTemplate.Write((byte) itemInfo.Y);
      eventMessageTemplate.Write((byte) itemInfo.InventoryType);
      this.SendOrderedDataMessage(eventMessageTemplate, connectionOfPlayer.Connection, 9);
    }

    public void SynchronizePlayerInventories(NetworkPlayer player)
    {
      if (player == null)
        return;
      this.dataSynchronizer.SynchronizePlayerInventories(player);
    }

    public void SendEventSpawnPlayer(Player player)
    {
      if (player == null)
        return;
      int id = player.Id;
      Position3 position = player.Astronaut.Position;
      NetOutgoingMessage eventMessageTemplate = this.CreateEventMessageTemplate(NetworkPeer.GameEventId.SpawnPlayer);
      eventMessageTemplate.Write(id);
      eventMessageTemplate.Write(position.X);
      eventMessageTemplate.Write(position.Y);
      eventMessageTemplate.Write(position.Z);
      this.SendOrderedDataMessage(eventMessageTemplate, (NetConnection) null, 2);
    }

    public void SignalRequestBindBlockToConsole(
      int entityId,
      Vector3 consolePos,
      Vector3 targetPos,
      Color newColor)
    {
      if (this.RequestBindBlockToConsole == null)
        return;
      this.RequestBindBlockToConsole(entityId, consolePos, targetPos, newColor);
    }

    public void SignalRequestUnbindBlockFromConsole(
      int entityId,
      Vector3 consolePos,
      Vector3 targetPos)
    {
      if (this.RequestUnbindBlockFromConsole == null)
        return;
      this.RequestUnbindBlockFromConsole(entityId, consolePos, targetPos);
    }

    public void SignalRequestChangeConsoleKey(
      int entityId,
      Vector3 consolePos,
      Color color,
      byte key)
    {
      if (this.RequestChangeConsoleColorKey == null)
        return;
      this.RequestChangeConsoleColorKey(entityId, consolePos, color, (Keys) key);
    }

    public void SignalRequestSpawnPlayer(int playerId)
    {
      if (this.RequestSpawnPlayer == null)
        return;
      Player player = this.connectedPlayers.Find((Predicate<Player>) (p => p.Id == playerId));
      if (player == null)
        return;
      this.RequestSpawnPlayer(player);
    }

    public void SignalRequestTossItem(int itemId, int quantity, Position3 position, Vector3 speed)
    {
      if (this.RequestTossItem == null)
        return;
      Item obj = Engine.LoadedWorld.Itemset.GetItem(itemId);
      if (!(obj != (Item) null))
        return;
      this.RequestTossItem(new ItemSlot(obj, quantity), position, speed);
    }

    public event Action<Player, string> ChatMessageReceivedEvent;

    public event Action<NetworkPlayer> PlayerConnected;

    public event Action<Player, string> PlayerDisconnected;

    public event Action<Player, StateFrame> PlayerStatusUpdateReceived;

    public event System.Action StateUpdateTick;

    public event Action<int, Vector3, Vector3, Color> RequestBindBlockToConsole;

    public event Action<int, Vector3, Color, Keys> RequestChangeConsoleColorKey;

    public event Action<Player> RequestSpawnPlayer;

    public event Action<ItemSlot, Position3, Vector3> RequestTossItem;

    public event Action<int, Vector3, Vector3> RequestUnbindBlockFromConsole;

    public IEnumerable<Player> Players => (IEnumerable<Player>) this.connectedPlayers;

    private void ManageEntityTransactions()
    {
      ServerEntityTransactionHandler senderForQueuedEntity = this.entityTransferHelper.GetEntitySenderForQueuedEntity();
      if (senderForQueuedEntity == null)
        return;
      this.RegisterNewEventHandler((EventHandler<NetConnection, NetworkServer>) senderForQueuedEntity);
    }

    private NetworkPlayerOnServer CreatePlayerWithToken(int id, string token, string name)
    {
      NetworkPlayerOnServer playerWithToken = this.world.GetPlayerWithToken(token);
      if (playerWithToken == null)
      {
        NetworkPlayerOnServer networkPlayerOnServer = new NetworkPlayerOnServer();
        networkPlayerOnServer.Token = token;
        networkPlayerOnServer.Name = name;
        playerWithToken = networkPlayerOnServer;
      }
      playerWithToken.Name = name;
      playerWithToken.Id = id;
      return playerWithToken;
    }

    private Player GetPlayer(int id)
    {
      return this.connectedPlayers.Find((Predicate<Player>) (p => p.Id == id));
    }

    private void InitializeEvents()
    {
      this.UserConnectedEvent += (Action<ServerToClientConnection>) (connection =>
      {
        NetworkPlayerOnServer playerOfClient = this.CreatePlayerWithToken(connection.Id, connection.Token, connection.ClientName);
        this.RegisterNewEventHandler((EventHandler<NetConnection, NetworkServer>) new ServerSendWorldDataHandler(connection.Connection, (NetworkServer) this, this.world, (Player) playerOfClient, (Action<bool>) (result =>
        {
          if (result)
          {
            NetworkPlayerOnServer networkPlayerOnServer = playerOfClient;
            this.connectedPlayers.Add((Player) networkPlayerOnServer);
            if (this.PlayerConnected == null)
              return;
            this.PlayerConnected((NetworkPlayer) networkPlayerOnServer);
          }
          else
          {
            Engine.Console.WriteErrorLine("Client failed to recieve itemset/player!");
            this.DisconnectUser(connection.Connection, "Failed to initialize world");
          }
        })));
      });
      this.UserDisconnectedEvent += (Action<string, int>) ((reason, id) =>
      {
        reason = string.IsNullOrEmpty(reason) ? "Unknown reason" : reason;
        Player player = this.GetPlayer(id);
        if (player == null)
          return;
        this.connectedPlayers.Remove(player);
        this.entityTransferHelper.PlayerDisconnected(id);
        if (this.PlayerDisconnected == null)
          return;
        this.PlayerDisconnected(player, reason);
      });
    }

    private void WriteActiveControlBlocksToMessage(NetOutgoingMessage message, SpaceEntity entity)
    {
      if (message == null || entity == null)
        return;
      ushort source = 0;
      foreach (ControlBlock controlBlock in entity.ControlBlocks)
      {
        if (controlBlock.ActiveButtons != (short) 0)
          ++source;
      }
      message.Write(source);
      if (source <= (ushort) 0)
        return;
      foreach (ControlBlock controlBlock in entity.ControlBlocks)
      {
        if (controlBlock.ActiveButtons != (short) 0)
        {
          message.Write(controlBlock.PositionInEntitySpace.X);
          message.Write(controlBlock.PositionInEntitySpace.Y);
          message.Write(controlBlock.PositionInEntitySpace.Z);
          message.Write(controlBlock.ActiveButtons);
        }
      }
    }
  }
}
