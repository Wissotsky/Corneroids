// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceEntityManagerServer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class SpaceEntityManagerServer : SpaceEntityManager
  {
    private const float distanceToLoadEntity = 512f;
    private NetworkServerManager server;

    public SpaceEntityManagerServer(World world)
      : base(world)
    {
    }

    public override bool AddEntityToSpace(SpaceEntity entity)
    {
      bool space = base.AddEntityToSpace(entity);
      if (space)
      {
        entity.ControlBlockKeyChanged += new Action<ControlBlock, ColorKeyGroup>(this.OnControlBlockKeyChanged);
        entity.ControlBlockNewBlockBound += new Action<ControlBlock, TriggerBlock, Color>(this.OnControlBlockNewBlockBound);
        entity.ControlBlockBlockUnbound += new Action<ControlBlock, TriggerBlock>(this.OnControlBlockBlockUnbound);
      }
      return space;
    }

    public override SpaceEntity AddBlock(Block block, Position3 worldPosition, Player user)
    {
      if (block != null)
      {
        if (this.characterManager != null && !this.characterManager.SpaceForBlock(worldPosition, block))
          return (SpaceEntity) null;
        SpaceEntity entityForNewBlock = this.GetSpaceEntityForNewBlock(block, worldPosition, true);
        if (entityForNewBlock != null)
        {
          Vector3 entityCoords = entityForNewBlock.WorldCoordsToEntityCoords(worldPosition);
          if (entityForNewBlock.AddBlock(block, worldPosition))
          {
            this.server.SendCreateBlock(entityForNewBlock, block, entityCoords, user);
            return entityForNewBlock;
          }
        }
      }
      return (SpaceEntity) null;
    }

    public override void BindToServer(NetworkServerManager server)
    {
      this.server = server;
      this.EntityCreatedEvent += (Action<SpaceEntity>) (newEntity =>
      {
        foreach (Player player in this.server.Players)
        {
          if (player is NetworkPlayer client2 && this.CheckIfEntityShouldBeSent(client2, newEntity))
          {
            client2.WorldView.Add(newEntity.Id);
            this.server.SendCreateEntity(client2, newEntity);
          }
        }
      });
      this.server.RequestBindBlockToConsole += (Action<int, Vector3, Vector3, Color>) ((entityId, consolePos, targetPos, newColor) =>
      {
        SpaceEntity entity = this.GetEntity(entityId);
        if (entity == null)
          return;
        ControlBlock blockOfType1 = entity.GetBlockOfType<ControlBlock>(consolePos);
        TriggerBlock blockOfType2 = entity.GetBlockOfType<TriggerBlock>(targetPos);
        if (blockOfType1 == null || blockOfType2 == null)
          return;
        if (blockOfType2 is CameraBlock camera2)
          blockOfType1.BindCamera(camera2, true);
        else
          blockOfType1.BindBlock(blockOfType2, newColor, true);
      });
      this.server.RequestUnbindBlockFromConsole += (Action<int, Vector3, Vector3>) ((entityId, consolePos, targetPos) =>
      {
        SpaceEntity entity = this.GetEntity(entityId);
        if (entity == null)
          return;
        ControlBlock blockOfType3 = entity.GetBlockOfType<ControlBlock>(consolePos);
        TriggerBlock blockOfType4 = entity.GetBlockOfType<TriggerBlock>(targetPos);
        if (blockOfType3 == null || blockOfType4 == null)
          return;
        if (blockOfType4 is CameraBlock)
          blockOfType3.UnbindCamera(true);
        else
          blockOfType3.UnbindBlock(blockOfType4, true);
      });
      this.server.RequestChangeConsoleColorKey += (Action<int, Vector3, Color, Keys>) ((entityId, consolePos, color, newKey) => this.GetEntity(entityId)?.GetBlockOfType<ControlBlock>(consolePos)?.SetKeyForColor(color, newKey, true));
    }

    protected override Block RemoveBlock(
      SpaceEntity entity,
      Vector3 positionInEntity,
      bool destroy,
      int senderId)
    {
      Block block = base.RemoveBlock(entity, positionInEntity, destroy, senderId);
      if (block != null)
      {
        positionInEntity = BlockSector.GetBlockPositionInBlockCoordinates(positionInEntity);
        this.server.SendRemoveBlock(entity, positionInEntity, destroy, senderId);
      }
      return block;
    }

    protected override void RemoveEntityFromSpace(SpaceEntity entity, bool removeFromDatabase)
    {
      base.RemoveEntityFromSpace(entity, removeFromDatabase);
      if (entity == null)
        return;
      entity.ControlBlockKeyChanged -= new Action<ControlBlock, ColorKeyGroup>(this.OnControlBlockKeyChanged);
      entity.ControlBlockNewBlockBound -= new Action<ControlBlock, TriggerBlock, Color>(this.OnControlBlockNewBlockBound);
      entity.ControlBlockBlockUnbound -= new Action<ControlBlock, TriggerBlock>(this.OnControlBlockBlockUnbound);
      this.server.SendRemoveEntity((NetworkPlayer) null, entity.Id);
    }

    public override void Update(IEnumerable<Player> players)
    {
      foreach (Player player in players)
        this.UpdateClientSpaceState(player as NetworkPlayer);
      if (this.server.IsMessageCycle())
        this.SendEntityStatesToPlayers(players);
      base.Update(players);
    }

    private bool CheckIfEntityShouldBeRemoved(NetworkPlayer client, SpaceEntity entity)
    {
      return client != null && entity != null && client.WorldView.Contains(entity.Id) && (double) (client.Astronaut.Position - entity.Position).LengthSquared() > 589824.0;
    }

    private bool CheckIfEntityShouldBeSent(NetworkPlayer client, SpaceEntity entity)
    {
      return client != null && entity != null && !client.WorldView.Contains(entity.Id) && (double) (client.Astronaut.Position - entity.Position).LengthSquared() < 262144.0;
    }

    private void OnControlBlockBlockUnbound(ControlBlock control, TriggerBlock trigger)
    {
      this.server.SendControlBlockBlockUnbound(control, trigger);
    }

    private void OnControlBlockKeyChanged(ControlBlock block, ColorKeyGroup newKey)
    {
      this.server.SendControlBlockKeyChanged(block, newKey);
    }

    private void OnControlBlockNewBlockBound(
      ControlBlock control,
      TriggerBlock trigger,
      Color newColor)
    {
      this.server.SendControlBlockNewBlockBound(control, trigger, newColor);
    }

    private void SendEntityStatesToPlayers(IEnumerable<Player> players)
    {
      foreach (Player player1 in players)
      {
        if (player1 is NetworkPlayer player2)
        {
          foreach (int entity1 in (IEnumerable<int>) player2.WorldView.Entities)
          {
            SpaceEntity entity2 = this.GetEntity(entity1);
            if (entity2 != null)
              this.server.SendEntityState(entity2, player2);
          }
        }
      }
    }

    private void UpdateClientSpaceState(NetworkPlayer client)
    {
      if (client == null)
        return;
      foreach (SpaceEntity entity in this.entityListAsync)
      {
        if (this.CheckIfEntityShouldBeSent(client, entity))
        {
          client.WorldView.Add(entity.Id);
          this.server.BeginEntityTransaction(client, entity);
        }
      }
      for (int index = client.WorldView.Entities.Count - 1; index >= 0; --index)
      {
        int entity1 = client.WorldView.Entities[index];
        SpaceEntity entity2 = this.GetEntity(entity1);
        if (entity2 != null)
        {
          if (this.CheckIfEntityShouldBeRemoved(client, entity2))
          {
            this.server.SendRemoveEntity(client, entity1);
            client.WorldView.Remove(entity1);
          }
        }
        else
        {
          this.server.SendRemoveEntity(client, entity1);
          client.WorldView.Remove(entity1);
        }
      }
    }
  }
}
