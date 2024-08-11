// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceEntity
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class SpaceEntity : PhysicalObject, IDisposable, IBoardable, IRemarkable
  {
    private volatile bool isDisposed;
    private BlockSectorManager sectorManager;
    private uint blockCount;
    private List<ControlBlock> controlBlocks;
    private Vector3 centerOfMass;
    private Vector3[] coordinateAxises;
    private bool forcesAppliedOnThisFrame;
    private int id;
    private int parasiteHostileCount;
    private object tag;

    public SpaceEntity() => this.InitializeVariables();

    public bool AddBlock(Block block, Position3 position)
    {
      if (!this.AddBlock(block, position, true))
        return false;
      this.UpdatePhysicalProperties(true);
      return true;
    }

    public bool AddBlockEntitySpace(Block block, Vector3 entityPosition)
    {
      if (!this.AddBlock(block, entityPosition, true))
        return false;
      this.UpdatePhysicalProperties(true);
      return true;
    }

    public bool AddBlockFast(Block block, Position3 position)
    {
      return this.AddBlock(block, position, false);
    }

    public override void ApplyForce(Vector3 point, Vector3 force, float deltaTime)
    {
      if (Engine.FastTravel)
        force *= 100f;
      base.ApplyForce(point, force, deltaTime);
      Vector3 vector3_1 = Vector3.Transform(force, this.Orientation);
      SpaceEntity spaceEntity = this;
      spaceEntity.Speed = spaceEntity.Speed + vector3_1 / this.Mass * deltaTime;
      Vector3 vector3_2 = Vector3.Cross(point - this.centerOfMass, force) * deltaTime;
      if (vector3_2 != Vector3.Zero)
        this.Rotation = new Vector3(this.Rotation.X + vector3_2.X / this.InertiaTensor.X, this.Rotation.Y + vector3_2.Y / this.InertiaTensor.Y, this.Rotation.Z + vector3_2.Z / this.InertiaTensor.Z);
      this.forcesAppliedOnThisFrame = true;
    }

    public bool BlockFits(Block block, Position3 position) => throw new NotImplementedException();

    public void ForceConstruct() => this.sectorManager.ForceConstruct();

    public Block DestroyBlock(Position3 worldPosition)
    {
      Block block = this.RemoveBlock(worldPosition);
      block?.Destroyed();
      return block;
    }

    public Block DestroyBlockEntitySpace(Vector3 positionInEntity)
    {
      Block block = this.RemoveBlockEntitySpace(positionInEntity);
      block?.Destroyed();
      return block;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.sectorManager != null)
        this.sectorManager.Dispose();
      this.isDisposed = true;
      if (this.DisposedEvent == null)
        return;
      this.DisposedEvent();
    }

    public Vector3 EntityNormalToWorldNormal(Vector3 normal)
    {
      return Vector3.TransformNormal(normal, this.transformMatrix);
    }

    public Position3 EntityCoordsToWorldCoords(Vector3 position)
    {
      return new Position3(Vector3.Transform(position, this.transformMatrix)) + this.Position;
    }

    public void EvaluateTransformMatrices()
    {
      this.transformMatrix = Matrix.CreateTranslation(-this.centerOfMass) * Matrix.CreateFromQuaternion(this.Orientation);
      this.inverseTransformMatrix = Matrix.CreateFromQuaternion(Quaternion.Conjugate(this.Orientation)) * Matrix.CreateTranslation(this.centerOfMass);
    }

    public BlockCell GetBlockCell(Position3 position)
    {
      return this.sectorManager.GetBlock(this.WorldCoordsToEntityCoords(position));
    }

    public BlockCell GetBlockCellEntityCoords(Vector3 positionInEntityCoordinates)
    {
      return this.sectorManager.GetBlock(positionInEntityCoordinates);
    }

    public Position3? GetBlockWorldPosition(Position3 position)
    {
      Vector3 vector3 = this.WorldCoordsToEntityCoords(position);
      BlockCell block = this.sectorManager.GetBlock(vector3);
      if (block == null)
        return new Position3?();
      vector3 = new Vector3((float) (SimpleMath.FastFloor(vector3.X) - (int) block.X), (float) (SimpleMath.FastFloor(vector3.Y) - (int) block.Y), (float) (SimpleMath.FastFloor(vector3.Z) - (int) block.Z));
      vector3 += 0.5f * (Vector3) block.Block.Size;
      return new Position3?(this.EntityCoordsToWorldCoords(vector3));
    }

    public BlockCell GetCollision(Position3 worldPosition)
    {
      Vector3 entityCoords = this.WorldCoordsToEntityCoords(worldPosition);
      if (this.sectorManager.IsBlocked(entityCoords))
      {
        BlockCell block = this.sectorManager.GetBlock(entityCoords);
        if (block != null)
        {
          if (!(block.Block.ModelPlacement != Vector3.Zero))
            return block;
          Vector3 size = (Vector3) block.Block.Size;
          Vector3 modelSize = (Vector3) block.Block.ModelSize;
          Vector3 modelPlacement = block.Block.ModelPlacement;
          Vector3 vector3_1 = -new Vector3((float) block.X, (float) block.Y, (float) block.Z) + new Vector3((float) (int) Math.Floor((double) entityCoords.X), (float) (int) Math.Floor((double) entityCoords.Y), (float) (int) Math.Floor((double) entityCoords.Z));
          Vector3 vector3_2 = vector3_1 + size;
          Vector3 min = 0.5f * (vector3_1 + vector3_2) + modelPlacement - 0.5f * modelSize;
          Vector3 max = min + modelSize;
          if (new BoundingBox(min, max).Contains(entityCoords) == ContainmentType.Contains)
            return block;
        }
      }
      return (BlockCell) null;
    }

    public T GetBlockOfType<T>(Vector3 positionInEntitySpace) where T : Block
    {
      BlockCell cellEntityCoords = this.GetBlockCellEntityCoords(positionInEntitySpace);
      return cellEntityCoords != null ? cellEntityCoords.Block as T : default (T);
    }

    public string GetDescriptionTag() => ((int) this.Mass / 10).ToString() + " tn";

    public float? GetDistanceSquaredToClosestBlock(Position3 worldPosition)
    {
      Position3? positionOfClosestBlock = this.GetPositionOfClosestBlock(worldPosition);
      return positionOfClosestBlock.HasValue ? new float?((positionOfClosestBlock.Value - worldPosition).LengthSquared()) : new float?();
    }

    public Position3? GetPositionOfClosestBlock(Position3 worldPosition)
    {
      Vector3? positionOfClosestBlock = this.sectorManager.GetPositionOfClosestBlock(this.WorldCoordsToEntityCoords(worldPosition));
      return positionOfClosestBlock.HasValue ? new Position3?(this.EntityCoordsToWorldCoords(positionOfClosestBlock.Value)) : new Position3?();
    }

    public override Vector3 GetSpeedAt(Position3 position)
    {
      Vector3 speed = this.Speed;
      Vector3 vector2 = position - this.Position;
      if (vector2 != Vector3.Zero && this.Rotation != Vector3.Zero)
        speed += Vector3.Cross(this.Rotation, vector2);
      return speed;
    }

    public override void InterpolatePosition()
    {
      this.elapsedInterpolationTicks += Engine.FrameCounter.DeltaTimeMS;
      StateFrame stateFrame1 = this.StateBuffer.PeekLatestState();
      StateFrame stateFrame2 = this.StateBuffer.PeekHistoryState(1);
      Position3 position3_1 = stateFrame1.Position + Vector3.Transform(this.centerOfMass - stateFrame1.CenterOfMass, stateFrame1.Orientation);
      Position3 position3_2 = stateFrame2.Position + Vector3.Transform(this.centerOfMass - stateFrame2.CenterOfMass, stateFrame2.Orientation);
      int num = (int) (stateFrame1.Tick - stateFrame2.Tick);
      if (num <= 0)
        return;
      Vector3 vector3 = position3_1 - position3_2;
      float amount = MathHelper.Clamp((float) this.elapsedInterpolationTicks / (float) num, 0.0f, 1f);
      this.Position = position3_2 + vector3 * amount;
      this.Orientation = Quaternion.Lerp(stateFrame2.Orientation, stateFrame1.Orientation, amount);
    }

    public void OnControlBlockAdded(ControlBlock block)
    {
      if (block == null)
        return;
      block.KeyChanged += new Action<ControlBlock, ColorKeyGroup>(this.OnControlBlockKeyChanged);
      block.NewBlockBound += new Action<ControlBlock, TriggerBlock, Color>(this.OnControlBlockNewBlockBound);
      block.BlockUnbound += new Action<ControlBlock, TriggerBlock>(this.OnControlBlockBlockUnbound);
    }

    public void OnControlBlockRemoved(ControlBlock block)
    {
      if (block == null)
        return;
      block.KeyChanged -= new Action<ControlBlock, ColorKeyGroup>(this.OnControlBlockKeyChanged);
      block.NewBlockBound -= new Action<ControlBlock, TriggerBlock, Color>(this.OnControlBlockNewBlockBound);
      block.BlockUnbound -= new Action<ControlBlock, TriggerBlock>(this.OnControlBlockBlockUnbound);
    }

    public Block RemoveBlock(Position3 position)
    {
      return this.RemoveBlockEntitySpace(this.WorldCoordsToEntityCoords(position));
    }

    public Block RemoveBlockEntitySpace(Vector3 entityPosition)
    {
      BlockCell block = this.sectorManager.GetBlock(entityPosition);
      if (!this.sectorManager.RemoveBlock(entityPosition))
        return (Block) null;
      --this.blockCount;
      this.UpdatePhysicalProperties(true);
      if (this.BlockRemoved != null)
        this.BlockRemoved(this.position, block.Block);
      return block.Block;
    }

    public virtual void Render(IRenderCamera camera, LightingManager lightingManager)
    {
      Vector3 relativeToCamera = camera.GetPositionRelativeToCamera(this.Position);
      Position3 center = this.BoundingSphere.Center;
      float radius = this.BoundingSphere.Radius;
      Microsoft.Xna.Framework.BoundingSphere sphere1 = new Microsoft.Xna.Framework.BoundingSphere(camera.GetPositionRelativeToCamera(center), radius);
      ContainmentType containmentType = camera.ViewFrustum.Contains(sphere1);
      if (containmentType == ContainmentType.Disjoint)
        return;
      bool flag = containmentType != ContainmentType.Contains;
      foreach (BlockSector sector in this.sectorManager.SectorList)
      {
        Microsoft.Xna.Framework.BoundingSphere sphere2 = new Microsoft.Xna.Framework.BoundingSphere(Vector3.Transform(sector.PositionInShipCoordinates, this.transformMatrix) + relativeToCamera, (float) ((double) BlockSector.Size / 2.0 * 1.7300000190734863));
        float distance = sphere2.Center.Length();
        if (!flag || camera.ViewFrustum.Intersects(sphere2))
        {
          lightingManager?.DrawLightChunk((ILightChunk) sector);
          Matrix sectorTransformMatrix = this.transformMatrix * Matrix.CreateTranslation(relativeToCamera);
          sector.Render(camera, ref sectorTransformMatrix, distance);
        }
      }
    }

    public object Tag
    {
      get => (object) null;
      set => this.tag = value;
    }

    public void Update(float deltaTime)
    {
      this.UpdatePhysics(deltaTime);
      this.EvaluateTransformMatrices();
      this.UpdatePositionOfBoundingSphere();
      this.sectorManager.UpdateBlocks();
    }

    public override bool UpdateBasedOnLatestState()
    {
      if (!base.UpdateBasedOnLatestState())
        return false;
      this.StateBuffer.GetLatestState();
      StateFrame stateFrame = this.StateBuffer.PeekHistoryState(1);
      this.Position = stateFrame.Position + Vector3.Transform(this.centerOfMass - stateFrame.CenterOfMass, stateFrame.Orientation);
      this.Orientation = stateFrame.Orientation;
      this.elapsedInterpolationTicks = 0L;
      return true;
    }

    public void UpdatePhysicalProperties(bool updatePosition)
    {
      this.UpdateEntityBoundingSphere();
      this.UpdateTotalMass();
      Vector3 vector3 = this.UpdateCenterOfMass();
      if (updatePosition)
        this.UpdatePositionAfterCOMupdate(this.centerOfMass - vector3);
      this.UpdateInertiaTensor();
    }

    public override void UpdatePhysics(float deltaTime)
    {
      base.UpdatePhysics(deltaTime);
      if (!this.forcesAppliedOnThisFrame)
      {
        if (this.Speed != Vector3.Zero)
        {
          Vector3 vector3 = Vector3.Normalize(this.Speed);
          if ((double)this.Speed.Length() < 0.02500000037252903)
          {
            this.Speed = Vector3.Zero;
          }
          else
          {
            this.Speed -= vector3 * 0.25f * Engine.FrameCounter.DeltaTime;
          }
        }
        if (this.Rotation != Vector3.Zero)
        {
          Vector3 vector3 = Vector3.Normalize(this.Rotation);
          if ((double)this.Rotation.Length() < 3.0 / 1000.0)
          {
            this.Rotation = Vector3.Zero;
          }
          else
          {
            this.Rotation -= vector3 * 0.03f * Engine.FrameCounter.DeltaTime;
          }
        }
      }
      this.Position += this.Speed * Engine.FrameCounter.DeltaTime;
      if (this.Rotation != Vector3.Zero)
      {
        this.Orientation *= Quaternion.CreateFromAxisAngle(Vector3.Normalize(this.Rotation), this.Rotation.Length() * Engine.FrameCounter.DeltaTime);
      }
      this.coordinateAxises[0] = Vector3.Transform(Vector3.UnitX, this.Orientation);
      this.coordinateAxises[1] = Vector3.Transform(Vector3.UnitY, this.Orientation);
      this.coordinateAxises[2] = Vector3.Transform(Vector3.UnitZ, this.Orientation);
      this.forcesAppliedOnThisFrame = false;
    }

    public Vector3 WorldCoordsToEntityCoords(Position3 position)
    {
      return Vector3.Transform(position - this.Position, this.inverseTransformMatrix);
    }

    public Vector3 WorldNormalToEntityNormal(Vector3 normal)
    {
      return Vector3.TransformNormal(normal, this.inverseTransformMatrix);
    }

    public uint BlockCount => this.blockCount;

    public BlockSectorManager BlockSectorManager => this.sectorManager;

    public Vector3 CenterOfMass => this.centerOfMass;

    public List<ControlBlock> ControlBlocks => this.controlBlocks;

    public Vector3[] CoordinateAxises => this.coordinateAxises;

    public int Id
    {
      get => this.id;
      set => this.id = value;
    }

    public bool IsDisposed => this.isDisposed;

    public int ParasiteHostileCount
    {
      get => this.parasiteHostileCount;
      set => this.parasiteHostileCount = Math.Max(value, 0);
    }

    public event Action<Position3, Block> BlockRemoved;

    public event Action<ControlBlock, TriggerBlock> ControlBlockBlockUnbound;

    public event Action<ControlBlock, TriggerBlock, Color> ControlBlockNewBlockBound;

    public event Action<ControlBlock, ColorKeyGroup> ControlBlockKeyChanged;

    public event System.Action DisposedEvent;

    private bool AddBlock(Block block, Position3 position, bool construct)
    {
      Vector3 entityCoords = this.WorldCoordsToEntityCoords(position);
      return this.AddBlock(block, entityCoords, construct);
    }

    private bool AddBlock(Block block, Vector3 entityPosition, bool construct)
    {
      if (block == null || !this.sectorManager.AddBlock(block, entityPosition, construct))
        return false;
      ++this.blockCount;
      return true;
    }

    private Dictionary<PowerBlock, Vector3> GetAdjacentPowerBlocks(
      BlockCell cell,
      Position3 position)
    {
      throw new NotImplementedException();
    }

    private void InitializeVariables()
    {
      this.forcesAppliedOnThisFrame = false;
      this.blockCount = 0U;
      this.sectorManager = new BlockSectorManager(this);
      this.centerOfMass = Vector3.Zero;
      this.id = -1;
      this.coordinateAxises = new Vector3[3];
      this.parasiteHostileCount = 0;
      this.isDisposed = false;
      this.controlBlocks = new List<ControlBlock>();
      this.transformMatrix = Matrix.Identity;
      this.inverseTransformMatrix = Matrix.Identity;
    }

    private void OnControlBlockBlockUnbound(ControlBlock control, TriggerBlock trigger)
    {
      if (this.ControlBlockBlockUnbound == null)
        return;
      this.ControlBlockBlockUnbound(control, trigger);
    }

    private void OnControlBlockNewBlockBound(
      ControlBlock control,
      TriggerBlock trigger,
      Color newColor)
    {
      if (this.ControlBlockNewBlockBound == null)
        return;
      this.ControlBlockNewBlockBound(control, trigger, newColor);
    }

    private void OnControlBlockKeyChanged(ControlBlock block, ColorKeyGroup newKey)
    {
      if (this.ControlBlockKeyChanged == null)
        return;
      this.ControlBlockKeyChanged(block, newKey);
    }

    private void UpdatePositionAfterCOMupdate(Vector3 comDifference)
    {
      SpaceEntity spaceEntity = this;
      spaceEntity.Position = spaceEntity.Position + Vector3.TransformNormal(comDifference, this.transformMatrix);
    }

    private void UpdatePositionOfBoundingSphere()
    {
      BoundingBox bounds = this.sectorManager.GetBounds();
      float radius = (0.5f * (bounds.Max - bounds.Min)).Length();
      this.BoundingSphere = new BoundingSphereI(this.Position + Vector3.Transform(0.5f * (bounds.Min + bounds.Max), this.transformMatrix), radius);
    }

    private Vector3 UpdateCenterOfMass()
    {
      Vector3 zero = Vector3.Zero;
      foreach (BlockSector sector in this.sectorManager.SectorList)
        zero += sector.CenterOfMass * sector.Mass;
      if ((double) this.Mass > 0.0)
        zero /= this.Mass;
      Vector3 centerOfMass = this.centerOfMass;
      this.centerOfMass = zero;
      this.EvaluateTransformMatrices();
      return centerOfMass;
    }

    private void UpdateEntityBoundingSphere()
    {
    }

    private void UpdateTotalMass()
    {
      this.Mass = 0.0f;
      foreach (BlockSector sector in this.sectorManager.SectorList)
        this.Mass += sector.Mass;
    }

    private void UpdateInertiaTensor()
    {
      Vector3 vector3_1 = new Vector3();
      for (int index = 0; index < this.sectorManager.SectorList.Count; ++index)
      {
        Vector3 inertiaTensor = this.sectorManager.SectorList[index].InertiaTensor;
        Vector3 vector3_2 = this.sectorManager.SectorList[index].CenterOfMass - this.centerOfMass;
        float mass = this.sectorManager.SectorList[index].Mass;
        vector3_1.X += inertiaTensor.X + mass * vector3_2.X * vector3_2.X;
        vector3_1.Y += inertiaTensor.Y + mass * vector3_2.Y * vector3_2.Y;
        vector3_1.Z += inertiaTensor.Z + mass * vector3_2.Z * vector3_2.Z;
      }
      this.InertiaTensor = vector3_1;
    }
  }
}
