// Decompiled with JetBrains decompiler
// Type: CornerSpace.CharacterManagerServer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class CharacterManagerServer : CharacterManager
  {
    private const int cSyncValueSendFrequency = 1000;
    private Action<Player, StateFrame> playerUpdateEvent;
    private System.Action tickEvent;
    private Action<Player, Player.ItemModifiedArgs> playerNewItemAdded;
    private Action<Player, Player.ItemModifiedArgs> playerItemRemoved;
    private ItemOperationManager itemOperationManager;
    private List<NetworkPlayer> networkPlayers;
    private NetworkServerManager server;

    public CharacterManagerServer(
      SpaceEntityManager entityManager,
      EnvironmentManager environmentManager,
      ProjectileManager projectileManager)
      : base(entityManager, environmentManager, projectileManager)
    {
      this.itemOperationManager = new ItemOperationManager();
      this.networkPlayers = new List<NetworkPlayer>();
      this.EnemiesEnabled = false;
      this.CreateEventDelegates();
    }

    public override void AddPlayer(Player player)
    {
      base.AddPlayer(player);
      if (player == null)
        return;
      player.Astronaut.MountedToControlBlock += new Action<ControlBlock, Astronaut>(this.PlayerMountedToControlBlock);
      player.Astronaut.UnMountedFromControlBlock += new Action<ControlBlock, Astronaut>(this.PlayerUnmounted);
      player.Astronaut.DiedEvent += new Action<Character>(this.PlayerAstronautDied);
      player.ItemAddedToInventory += this.playerNewItemAdded;
      player.ItemRemovedFromInventory += this.playerItemRemoved;
      player.BlockUsed += new Action<Player, Block>(this.PlayerBlockUsed);
      this.server.SynchronizePlayerInventories(player as NetworkPlayer);
    }

    public override void BindToServer(NetworkServerManager server)
    {
      if (server == null)
        return;
      this.server = server;
      this.playerUpdateEvent = (Action<Player, StateFrame>) ((player, frame) => player.Astronaut.StateBuffer.AddState(frame));
      this.tickEvent = (System.Action) (() =>
      {
        foreach (Player player in this.players)
          this.server.SendPlayerState(player);
      });
      this.server.RequestSpawnPlayer += (Action<Player>) (player =>
      {
        if (player.Astronaut.Alive)
          return;
        this.SpawnPlayer(player);
      });
      this.server.PlayerStatusUpdateReceived += this.playerUpdateEvent;
      this.server.StateUpdateTick += this.tickEvent;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.server == null)
        return;
      if (this.playerUpdateEvent != null)
        this.server.PlayerStatusUpdateReceived -= this.playerUpdateEvent;
      if (this.tickEvent == null)
        return;
      this.server.StateUpdateTick -= this.tickEvent;
    }

    public override void RemovePlayer(Player player)
    {
      base.RemovePlayer(player);
      if (player == null)
        return;
      player.Astronaut.MountedToControlBlock -= new Action<ControlBlock, Astronaut>(this.PlayerMountedToControlBlock);
      player.Astronaut.UnMountedFromControlBlock -= new Action<ControlBlock, Astronaut>(this.PlayerUnmounted);
      player.Astronaut.DiedEvent -= new Action<Character>(this.PlayerAstronautDied);
      player.ItemAddedToInventory -= this.playerNewItemAdded;
      player.ItemRemovedFromInventory -= this.playerItemRemoved;
      player.BlockUsed -= new Action<Player, Block>(this.PlayerBlockUsed);
    }

    private void CreateEventDelegates()
    {
      this.playerNewItemAdded = (Action<Player, Player.ItemModifiedArgs>) ((player, itemInfo) =>
      {
        if (player is LocalPlayer)
          return;
        ItemSlot itemSlot = (ItemSlot) null;
        if (itemInfo.InventoryType == Inventory.Type.Inventory)
          itemSlot = player.Inventory.PeekItem(itemInfo.X, itemInfo.Y);
        else if (itemInfo.InventoryType == Inventory.Type.Toolbar)
          itemSlot = player.Toolbar.Items.PeekItem(itemInfo.X, itemInfo.Y);
        this.server.SendSetItem(player, new Player.ItemModifiedArgs()
        {
          ItemSlot = itemSlot,
          X = itemInfo.X,
          Y = itemInfo.Y,
          InventoryType = itemInfo.InventoryType
        });
      });
    }

    private void PlayerAstronautDied(Character astronaut)
    {
      Player player = this.players.Find((Predicate<Player>) (p => p.Astronaut == astronaut));
      if (player == null)
        return;
      this.server.SendKillPlayer(player);
    }

    private void PlayerBlockUsed(Player player, Block block)
    {
      if (player == null || block == null || !(block is ContainerBlock block1))
        return;
      this.server.SendOpenContainer(player, block1);
    }

    private void PlayerMountedToControlBlock(ControlBlock block, Astronaut astronaut)
    {
      if (astronaut == null || block == null)
        return;
      this.server.SendPlayerMountedStatusChanged(this.players.Find((Predicate<Player>) (p => p.Astronaut == astronaut)), block, astronaut.MountedTo == block);
    }

    private void PlayerUnmounted(ControlBlock block, Astronaut astronaut)
    {
      if (astronaut == null)
        return;
      this.server.SendPlayerMountedStatusChanged(this.players.Find((Predicate<Player>) (p => p.Astronaut == astronaut)), block, false);
    }

    protected override void SpawnPlayer(Player player)
    {
      base.SpawnPlayer(player);
      this.server.SendEventSpawnPlayer(player);
    }
  }
}
