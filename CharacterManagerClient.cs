// Decompiled with JetBrains decompiler
// Type: CornerSpace.CharacterManagerClient
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Networking;
using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class CharacterManagerClient : CharacterManager
  {
    private Action<Player, CharacterScreen> characterScreenOpened;
    private NetworkClientManager client;
    private List<NetworkPlayer> networkPlayers;
    private Action<Player, StateFrame> playerUpdateEvent;

    public CharacterManagerClient(
      SpaceEntityManager entityManager,
      EnvironmentManager environmentManager,
      ProjectileManager projectileManager)
      : base(entityManager, environmentManager, projectileManager)
    {
      this.networkPlayers = new List<NetworkPlayer>();
      this.EnemiesEnabled = false;
      this.CreateEventDelegates();
    }

    public override void AddPlayer(Player player) => base.AddPlayer(player);

    public override void BindToClient(NetworkClientManager client)
    {
      if (client == null)
        return;
      this.client = client;
      this.playerUpdateEvent = (Action<Player, StateFrame>) ((player, newState) => player.Astronaut.StateBuffer.AddState(newState));
      this.client.PlayerMountedStatusChanged += (Action<int, int, Vector3, bool>) ((playerId, entityId, position, isMounted) =>
      {
        SpaceEntity entity = this.entityManager.GetEntity(entityId);
        Player player = this.Players.Find((Predicate<Player>) (p => p.Id == playerId));
        if (entity == null || player == null || !(entity.GetBlockOfType<Block>(position) is IMountable<Astronaut> blockOfType2))
          return;
        if (isMounted)
          blockOfType2.Mount((IController<Astronaut>) player.Astronaut);
        else
          blockOfType2.UnMount();
      });
      this.client.PlayerAstronautKilled += (Action<Player>) (player =>
      {
        player.Astronaut.Kill();
        LocalPlayer localPlayer = player as LocalPlayer;
        if (localPlayer == null)
          return;
        Engine.LoadNewScreen((GameScreen) new MessageScreen("You have died", "Oh noes!", true, "Respawn", (System.Action) (() => this.client.RequestSpawn(localPlayer))));
      });
      this.client.PlayerSpawned += (Action<int, Position3>) ((playerId, position) =>
      {
        Player player = this.players.Find((Predicate<Player>) (p => p.Id == playerId));
        if (player == null)
          return;
        player.Astronaut.Spawn(position);
        player.Astronaut.ModificationMatrix = Matrix.Identity;
        this.EnvironmentManager.AddSpawnParticles(position);
      });
      this.client.SetItem += (Action<Player, Player.ItemModifiedArgs>) ((player, itemInfo) =>
      {
        if (itemInfo.InventoryType == Inventory.Type.Inventory)
        {
          player.Inventory.SetItem(itemInfo);
        }
        else
        {
          if (itemInfo.InventoryType != Inventory.Type.Toolbar)
            return;
          player.Toolbar.Items.SetItem(itemInfo);
        }
      });
      this.client.OpenContainer += (Action<int, Vector3, Action<Inventory>, Action<int>>) ((entityId, position, inventoryCallback, doneCallback) =>
      {
        SpaceEntity entity = this.entityManager.GetEntity(entityId);
        if (entity == null)
          return;
        ContainerBlock blockOfType3 = entity.GetBlockOfType<ContainerBlock>(position);
        if (blockOfType3 != null)
        {
          inventoryCallback(blockOfType3.Inventory);
          ContainerBlockScreen screen = new ContainerBlockScreen(this.LocalPlayer, blockOfType3);
          screen.ScreenClosing += (System.Action) (() => doneCallback(this.LocalPlayer.Id));
          Engine.LoadNewScreen((GameScreen) screen);
        }
        else
          doneCallback(this.LocalPlayer.Id);
      });
      this.client.SynchronizeInventories += (Action<Action<ISynchronizable, ISynchronizable, ISynchronizable>>) (inventoriesCallback => inventoriesCallback((ISynchronizable) this.LocalPlayer.Inventory, (ISynchronizable) this.LocalPlayer.Toolbar.Items, (ISynchronizable) this.LocalPlayer.Astronaut.Suit));
      this.client.PlayerStateUpdateReceivedEvent += this.playerUpdateEvent;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.client == null || this.playerUpdateEvent == null)
        return;
      this.client.PlayerStateUpdateReceivedEvent -= this.playerUpdateEvent;
    }

    public override void RemovePlayer(Player player) => base.RemovePlayer(player);

    public override void UpdateInput(InputFrame input) => this.LocalPlayer.UpdateInput(input);

    public override void Update()
    {
      base.Update();
      this.client.SendPlayerState(this.LocalPlayer);
    }

    private void BoardPlayer(NetworkPlayer player, int entityId)
    {
      if (player == null)
        return;
      SpaceEntity entity = this.entityManager.GetEntity(entityId);
      if (entity == player.Astronaut.BoardedEntity)
        return;
      player.Astronaut.BoardedEntity = entity;
    }

    private void CreateEventDelegates()
    {
      this.characterScreenOpened = (Action<Player, CharacterScreen>) ((player, screen) => { });
    }

    protected override void ShowDiedDialog(Player player)
    {
    }
  }
}
