// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceEntityManagerClient
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
  public class SpaceEntityManagerClient : SpaceEntityManager
  {
    private List<Position3> addedBlocksWaitingAck;
    private NetworkClientManager client;

    public SpaceEntityManagerClient(World world)
      : base(world)
    {
      this.addedBlocksWaitingAck = new List<Position3>();
    }

    public override SpaceEntity AddBlock(Block block, Position3 worldPosition, Player sender)
    {
      if (sender is NetworkPlayer)
        return (SpaceEntity) null;
      SpaceEntity result = this.AddBlockToExistingEntity(block, worldPosition, sender);
      if (result != null)
      {
        Vector3 positionInEntitySpace = result.WorldCoordsToEntityCoords(worldPosition);
        Position3 roundedPosition = new Position3((long) Math.Floor((double) positionInEntitySpace.X), (long) Math.Floor((double) positionInEntitySpace.Y), (long) Math.Floor((double) positionInEntitySpace.Z));
        if (!this.addedBlocksWaitingAck.Contains(roundedPosition))
          this.addedBlocksWaitingAck.Add(roundedPosition);
        Engine.TimedEvents.ExecuteOnce(5000, (System.Action) (() =>
        {
          if (!this.addedBlocksWaitingAck.Contains(roundedPosition) || result.IsDisposed)
            return;
          result.RemoveBlockEntitySpace(positionInEntitySpace);
          this.addedBlocksWaitingAck.Remove(roundedPosition);
        }));
      }
      return result;
    }

    public override bool AddEntityToSpace(SpaceEntity entity)
    {
      if (!base.AddEntityToSpace(entity))
        return false;
      entity.StateBuffer.InitializationState(new StateFrame()
      {
        Position = entity.Position,
        Orientation = entity.Orientation
      });
      entity.ControlBlockBlockUnbound += new Action<ControlBlock, TriggerBlock>(this.OnControlBlockBlockUnbound);
      entity.ControlBlockKeyChanged += new Action<ControlBlock, ColorKeyGroup>(this.OnControlBlockKeyChanged);
      entity.ControlBlockNewBlockBound += new Action<ControlBlock, TriggerBlock, Color>(this.OnControlBlockNewBlockBound);
      return true;
    }

    public override void BindToClient(NetworkClientManager client)
    {
      this.client = client;
      this.client.NewEntityCreatedEvent += (Action<SpaceEntity>) (newEntity => this.AddEntityToSpace(newEntity));
      this.client.NewBlockAddedEvent += (Action<int, byte, byte, Vector3>) ((entityId, blockId, blockOrientation, entityPosition) =>
      {
        SpaceEntity entity = this.GetEntity(entityId);
        Block blockById = Engine.LoadedWorld.Itemset.GetBlockByID((int) blockId);
        if (entity == null || blockById == null)
          return;
        blockById.Orientation = blockOrientation;
        entity.AddBlockEntitySpace(blockById, entityPosition);
        this.addedBlocksWaitingAck.Remove(new Position3((long) Math.Floor((double) entityPosition.X), (long) Math.Floor((double) entityPosition.Y), (long) Math.Floor((double) entityPosition.Z)));
      });
      this.client.BlockRemovedEvent += (Action<int, Vector3, bool>) ((id, position, destroyed) =>
      {
        SpaceEntity entity = this.GetEntity(id);
        if (entity == null)
          return;
        entity.EntityCoordsToWorldCoords(position);
        this.RemoveBlock(entity, position, destroyed, -1);
      });
      this.client.ControlBlockKeyChanged += (Action<int, Vector3, Color, Keys>) ((entityId, blockPosition, color, key) => this.GetEntity(entityId)?.GetBlockOfType<ControlBlock>(blockPosition)?.SetKeyForColor(color, key, false));
      this.client.ControlBlockNewBind += (Action<int, Vector3, Vector3, Color>) ((entityId, consolePos, triggerPos, newColor) =>
      {
        SpaceEntity entity = this.GetEntity(entityId);
        if (entity == null)
          return;
        ControlBlock blockOfType1 = entity.GetBlockOfType<ControlBlock>(consolePos);
        TriggerBlock blockOfType2 = entity.GetBlockOfType<TriggerBlock>(triggerPos);
        if (blockOfType1 == null || blockOfType2 == null)
          return;
        blockOfType1.BindBlock(blockOfType2, newColor, false);
        if (!(blockOfType2 is CameraBlock camera2))
          return;
        blockOfType1.BindCamera(camera2, false);
      });
      this.client.ControlBlockBlockUnbound += (Action<int, Vector3, Vector3>) ((entityId, consolePos, triggerPos) =>
      {
        SpaceEntity entity = this.GetEntity(entityId);
        if (entity == null)
          return;
        ControlBlock blockOfType3 = entity.GetBlockOfType<ControlBlock>(consolePos);
        TriggerBlock blockOfType4 = entity.GetBlockOfType<TriggerBlock>(triggerPos);
        if (blockOfType3 == null || blockOfType4 == null)
          return;
        blockOfType3.UnbindBlock(blockOfType4, false);
        if (!(blockOfType4 is CameraBlock))
          return;
        blockOfType3.UnbindCamera(false);
      });
      this.client.EntityStateUpdateEvent += (Action<EntityStateContainer, ControlBlocksStates>) ((entityState, controlBlocksStates) =>
      {
        if (entityState == null || controlBlocksStates == null)
          return;
        SpaceEntity entity = this.GetEntity(entityState.EntityId);
        if (entity == null)
          return;
        StateFrame newState = new StateFrame()
        {
          CenterOfMass = entityState.CenterOfMass,
          Position = entityState.Position,
          Orientation = entityState.Orientation,
          Tick = entityState.Ticks
        };
        entity.ControlBlocks.ForEach((Action<ControlBlock>) (b => b.ActiveButtons = (short) 0));
        foreach (KeyValuePair<Vector3, short> controlBlock in controlBlocksStates.ControlBlocks)
        {
          ControlBlock blockOfType = entity.GetBlockOfType<ControlBlock>(controlBlock.Key);
          if (blockOfType != null)
            blockOfType.ActiveButtons = controlBlock.Value;
        }
        entity.StateBuffer.AddState(newState);
      });
      this.client.EntityRemoved += (Action<int>) (id => this.RemoveEntityFromSpace(this.GetEntity(id), false));
    }

    public override Block DestroyBlock(Position3 position) => (Block) null;

    public override bool DrillBlock(Position3 position, int drillPower, int senderId) => false;

    protected override void RemoveEntityFromSpace(SpaceEntity entity, bool removeFromDatabase)
    {
      base.RemoveEntityFromSpace(entity, removeFromDatabase);
      if (entity == null)
        return;
      entity.ControlBlockBlockUnbound -= new Action<ControlBlock, TriggerBlock>(this.OnControlBlockBlockUnbound);
      entity.ControlBlockKeyChanged -= new Action<ControlBlock, ColorKeyGroup>(this.OnControlBlockKeyChanged);
      entity.ControlBlockNewBlockBound -= new Action<ControlBlock, TriggerBlock, Color>(this.OnControlBlockNewBlockBound);
    }

    public override void Update(IEnumerable<Player> players)
    {
      this.UpdateEntities();
      this.UpdateSpaceEntitiesStates();
      this.UpdateReceivedControlBlockStates();
      this.SweepAndPrune();
    }

    private void OnControlBlockBlockUnbound(ControlBlock control, TriggerBlock trigger)
    {
      this.client.RequestUnbindBlockFromConsole(control, trigger);
    }

    private void OnControlBlockKeyChanged(ControlBlock block, ColorKeyGroup newKey)
    {
      this.client.RequestChangeConsoleColorKey(block, newKey);
    }

    private void OnControlBlockNewBlockBound(
      ControlBlock control,
      TriggerBlock trigger,
      Color newColor)
    {
      this.client.RequestBindBlockToConsole(control, trigger, newColor);
    }

    private void UpdateReceivedControlBlockStates()
    {
      foreach (SpaceEntity spaceEntity in this.entityListAsync)
      {
        foreach (ControlBlock controlBlock in spaceEntity.ControlBlocks)
        {
          if (controlBlock.MountedController != null)
          {
            Astronaut controller = controlBlock.MountedController.Controller;
            controlBlock.Update((InputFrame) null, controller);
            spaceEntity.Speed = Vector3.Zero;
            spaceEntity.Rotation = Vector3.Zero;
          }
        }
      }
    }

    private void UpdateSpaceEntitiesStates()
    {
      foreach (SpaceEntity spaceEntity in this.entityListAsync)
      {
        if (!spaceEntity.UpdateBasedOnLatestState())
          spaceEntity.InterpolatePosition();
        spaceEntity.EvaluateTransformMatrices();
      }
    }
  }
}
