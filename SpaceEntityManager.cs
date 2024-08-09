// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceEntityManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class SpaceEntityManager : IDisposable, ISpaceEntities
  {
    private const long cVecToPos = 8388608;
    protected const float distanceToEntityUnload = 768f;
    private const float spaceSectorMarginals = 32f;
    private object constructionQueuesLock = new object();
    private object disposeLock = new object();
    private Queue<SpaceEntity> constructedEntitiesA;
    private HashSet<BeaconObject> beacons;
    protected CharacterManager characterManager;
    private bool disposedA;
    private HashSet<SpaceEntity> entitiesInSpaceAsync;
    protected List<SpaceEntity> entityListAsync;
    private Comparison<SpaceEntity> entitySortDelegate;
    private EnvironmentManager environmentManager;
    private List<SpaceEntity> resultList;
    private Vector3int? playerInSpaceSector;
    private RadarDisplay radarDisplay;
    private List<SpaceEntity> sweepAndPruneActiveList;
    private World world;
    private HashSet<EventHandler> entityAddedListeners;

    public SpaceEntityManager(World world)
    {
      this.beacons = new HashSet<BeaconObject>();
      this.constructedEntitiesA = new Queue<SpaceEntity>();
      this.entitiesInSpaceAsync = new HashSet<SpaceEntity>();
      this.entityListAsync = new List<SpaceEntity>();
      this.resultList = new List<SpaceEntity>();
      this.sweepAndPruneActiveList = new List<SpaceEntity>();
      this.entityAddedListeners = new HashSet<EventHandler>();
      this.world = world;
      this.CreateDelegateFunctions();
    }

    public virtual SpaceEntity AddBlock(Block block, Position3 worldPosition, Player sender)
    {
      if (block != null)
      {
        if (this.characterManager != null && !this.characterManager.SpaceForBlock(worldPosition, block))
          return (SpaceEntity) null;
        SpaceEntity entityForNewBlock = this.GetSpaceEntityForNewBlock(block, worldPosition, true);
        if (entityForNewBlock != null && entityForNewBlock.AddBlock(block, worldPosition))
          return entityForNewBlock;
      }
      return (SpaceEntity) null;
    }

    public void AddAsyncConstructedEntity(SpaceEntity entity)
    {
      if (entity == null)
        return;
      lock (this.disposeLock)
      {
        if (this.disposedA)
        {
          entity.Dispose();
        }
        else
        {
          lock (this.constructionQueuesLock)
            this.constructedEntitiesA.Enqueue(entity);
        }
      }
    }

    public bool AddBeacon(BeaconObject beacon, bool addToDatabase)
    {
      if (beacon == null || !this.beacons.Add(beacon))
        return false;
      if (addToDatabase)
        this.world.AddBeacon(beacon);
      return true;
    }

    public virtual bool AddEntityToSpace(SpaceEntity entity)
    {
      if (entity == null || !this.entitiesInSpaceAsync.Add(entity))
        return false;
      this.entityListAsync.Add(entity);
      if (entity.Id == -1)
        entity.Id = this.world.GetId();
      if (this.EntityAddedEvent != null)
        this.EntityAddedEvent(entity);
      return true;
    }

    public virtual void BindToClient(NetworkClientManager client)
    {
      throw new ArgumentException("Cannot bind a client to singleplayer manager!");
    }

    public virtual void BindToServer(NetworkServerManager server)
    {
      throw new ArgumentException("Cannot bind a server to singleplayer manager!");
    }

    public virtual Block DestroyBlock(Position3 position) => this.RemoveBlock(position, true, -1);

    public void Dispose()
    {
      lock (this.disposeLock)
        this.disposedA = true;
      foreach (PhysicalObject physicalObject in this.entityListAsync)
        physicalObject.Dispose();
      this.entityListAsync.Clear();
      this.entitiesInSpaceAsync.Clear();
      this.entityAddedListeners.Clear();
      this.EntityCreatedEvent = (Action<SpaceEntity>) null;
      this.EntityAddedEvent = (Action<SpaceEntity>) null;
      lock (this.constructionQueuesLock)
      {
        while (this.constructedEntitiesA.Count > 0)
          this.constructedEntitiesA.Dequeue().Dispose();
      }
    }

    public virtual bool DrillBlock(Position3 position, int drillPower, int senderId)
    {
      BlockCell blockCell = this.PickBlock(position);
      return blockCell != null && (int) blockCell.Block.GetBlockType().PowerToDrill <= drillPower && this.RemoveBlock(position, false, senderId) != null;
    }

    public List<SpaceEntity> GetCloseEntities(Position3 position, float marginal)
    {
      this.resultList.Clear();
      for (int index = 0; index < this.entityListAsync.Count; ++index)
      {
        BoundingSphereI boundingSphere = this.entityListAsync[index].BoundingSphere;
        if ((double) (boundingSphere.Center - position).Length() <= (double) boundingSphere.Radius + (double) marginal)
          this.resultList.Add(this.entityListAsync[index]);
      }
      return this.resultList;
    }

    public SpaceEntity GetClosestEntity(Position3 position, float marginal)
    {
      List<SpaceEntity> closeEntities = this.GetCloseEntities(position, marginal);
      SpaceEntity closestEntity = (SpaceEntity) null;
      float num1 = float.MaxValue;
      for (int index = 0; index < closeEntities.Count; ++index)
      {
        Position3? positionOfClosestBlock = closeEntities[index].GetPositionOfClosestBlock(position);
        if (positionOfClosestBlock.HasValue)
        {
          float num2 = (positionOfClosestBlock.Value - position).LengthSquared();
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            closestEntity = closeEntities[index];
          }
        }
      }
      return closestEntity;
    }

    public SpaceEntity GetEntity(int id)
    {
      return this.entityListAsync.Find((Predicate<SpaceEntity>) (e => e.Id == id));
    }

    public BlockCell PickBlock(Position3 position)
    {
      List<SpaceEntity> closeEntities = this.GetCloseEntities(position, 8f);
      for (int index = 0; index < closeEntities.Count; ++index)
      {
        BlockCell blockCell = closeEntities[index].GetBlockCell(position);
        if (blockCell != null)
          return blockCell;
      }
      return (BlockCell) null;
    }

    public void RemoveBeacon(BeaconObject beacon)
    {
      if (beacon == null)
        return;
      this.beacons.Remove(beacon);
      this.world.RemoveBeacon(beacon);
    }

    public void RemoveEntityFromSpace(SpaceEntity entity)
    {
      this.RemoveEntityFromSpace(entity, true);
    }

    public void Render(IRenderCamera camera, LightingManager lightingManager)
    {
      BoundingFrustum viewFrustum = camera.ViewFrustum;
      BlockSector.SetRenderstates();
      foreach (SpaceEntity spaceEntity in this.entitiesInSpaceAsync)
        spaceEntity.Render(camera, lightingManager);
    }

    public void Save()
    {
      foreach (SpaceEntity entity in this.entitiesInSpaceAsync)
        this.world.StoreEntity(entity);
    }

    public virtual void Update(IEnumerable<Player> players)
    {
      this.UpdateSpaceContents(players);
      this.CheckCreationQueues();
      this.UpdateEntities();
      this.SweepAndPrune();
    }

    public event Action<SpaceEntity> EntityAddedEvent;

    public event Action<SpaceEntity> EntityCreatedEvent;

    public event Action<SpaceEntity> EntityRemovedEvent;

    public HashSet<BeaconObject> Beacons => this.beacons;

    public CharacterManager CharacterManager
    {
      set => this.characterManager = value;
    }

    public List<SpaceEntity> Entities => this.entityListAsync;

    public int EntityCount => this.entityListAsync.Count;

    public EnvironmentManager EnvironmentManager
    {
      set => this.environmentManager = value;
      protected get => this.environmentManager;
    }

    protected SpaceEntity AddBlockToExistingEntity(
      Block block,
      Position3 worldPosition,
      Player sender)
    {
      if (block != null)
      {
        if (this.characterManager != null && !this.characterManager.SpaceForBlock(worldPosition, block))
          return (SpaceEntity) null;
        SpaceEntity entityForNewBlock = this.GetSpaceEntityForNewBlock(block, worldPosition, false);
        if (entityForNewBlock != null && entityForNewBlock.AddBlock(block, worldPosition))
          return entityForNewBlock;
      }
      return (SpaceEntity) null;
    }

    private int BinarySearch(Vector3 position) => -1;

    private void CheckCreationQueues()
    {
      lock (this.constructionQueuesLock)
      {
        while (this.constructedEntitiesA.Count > 0)
          this.AddEntityToSpace(this.constructedEntitiesA.Dequeue());
      }
    }

    private void CreateDelegateFunctions()
    {
      this.entitySortDelegate = (Comparison<SpaceEntity>) ((s1, s2) =>
      {
        if (s1 == s2)
          return 0;
        if (s1 == null)
          return -1;
        if (s2 == null)
          return 1;
        BoundingSphereI boundingSphere1 = s1.BoundingSphere;
        BoundingSphereI boundingSphere2 = s2.BoundingSphere;
        return Math.Sign(boundingSphere1.Center.X - (long) boundingSphere1.Radius * 8388608L - (boundingSphere2.Center.X - (long) boundingSphere2.Radius * 8388608L));
      });
    }

    protected SpaceEntity CreateNewEntity(Position3 position)
    {
      SpaceEntity entity = new SpaceEntity();
      this.AddEntityToSpace(entity);
      entity.Position = position;
      if (this.EntityCreatedEvent != null)
        this.EntityCreatedEvent(entity);
      return entity;
    }

    private float DistanceToSpaceSector(Vector3int sectorPos, Vector3 point)
    {
      BoundingBox boundingBox = new BoundingBox((Vector3) sectorPos * (float) this.world.SectorSize, ((Vector3) sectorPos + Vector3.One) * (float) this.world.SectorSize);
      if (boundingBox.Contains(point) == ContainmentType.Contains)
        return 0.0f;
      Vector3 min = boundingBox.Min;
      Vector3 max = boundingBox.Max;
      return (new Vector3()
      {
        X = Math.Min(Math.Max(point.X, min.X), max.X),
        Y = Math.Min(Math.Max(point.Y, min.Y), max.Y),
        Z = Math.Min(Math.Max(point.Z, min.Z), max.Z)
      } - point).Length();
    }

    protected SpaceEntity GetSpaceEntityForNewBlock(
      Block block,
      Position3 worldPosition,
      bool createIfNone)
    {
      if (block != null)
      {
        SpaceEntity closestEntity = this.GetClosestEntity(worldPosition, 8f);
        if (closestEntity != null)
        {
          Position3? positionOfClosestBlock = closestEntity.GetPositionOfClosestBlock(worldPosition);
          if (positionOfClosestBlock.HasValue)
          {
            if ((double) (positionOfClosestBlock.Value - worldPosition).LengthSquared() <= 256.0)
              return closestEntity;
            return !createIfNone ? (SpaceEntity) null : this.CreateNewEntity(worldPosition - Vector3.One * 8f);
          }
          return !createIfNone ? (SpaceEntity) null : this.CreateNewEntity(worldPosition - Vector3.One * 8f);
        }
        if (createIfNone)
          return this.CreateNewEntity(worldPosition - Vector3.One * 8f);
      }
      return (SpaceEntity) null;
    }

    private void LoadSpaceEntities(Vector3int middlePosition, Position3 playerPosition)
    {
      this.world.LoadSpaceEntities(middlePosition - new Vector3int(1, 1, 1), middlePosition + new Vector3int(1, 1, 1), playerPosition, (Action<SpaceEntity>) (entity =>
      {
        if (entity == null)
          return;
        this.AddAsyncConstructedEntity(entity);
      }));
    }

    protected virtual Block RemoveBlock(Position3 position, bool destroy, int senderId)
    {
      foreach (SpaceEntity closeEntity in this.GetCloseEntities(position, 8f))
      {
        Vector3 entityCoords = closeEntity.WorldCoordsToEntityCoords(position);
        Block block = this.RemoveBlock(closeEntity, entityCoords, destroy, senderId);
        if (block != null)
          return block;
      }
      return (Block) null;
    }

    protected virtual Block RemoveBlock(
      SpaceEntity entity,
      Vector3 positionInEntity,
      bool destroy,
      int senderId)
    {
      if (entity != null)
      {
        Position3 worldCoords1 = entity.EntityCoordsToWorldCoords(positionInEntity);
        Position3? blockWorldPosition = entity.GetBlockWorldPosition(worldCoords1);
        Block block = entity.DestroyBlockEntitySpace(positionInEntity);
        if (block != null)
        {
          if (blockWorldPosition.HasValue)
            this.environmentManager.BreakBlock(blockWorldPosition.Value, block, entity.Orientation);
          if (!destroy && this.environmentManager != null)
          {
            Position3 worldCoords2 = entity.EntityCoordsToWorldCoords(BlockSector.GetBlockPositionInBlockCoordinates(positionInEntity));
            this.environmentManager.AddFloatingItem(new ItemSlot((Item) block.GetBlockType(), 1), worldCoords2, Vector3.Zero, (ushort) 0);
          }
          if (entity.BlockCount == 0U)
          {
            this.RemoveEntityFromSpace(entity, true);
            entity.Dispose();
          }
          return block;
        }
      }
      return (Block) null;
    }

    protected virtual void RemoveDistantEntities(IEnumerable<Player> players)
    {
      for (int index = this.entityListAsync.Count - 1; index >= 0; --index)
      {
        SpaceEntity entity = this.entityListAsync[index];
        bool flag = false;
        foreach (Player player in players)
          flag |= player.CanSeeEntity(entity, 589824f);
        if (!flag)
        {
          this.world.StoreEntity(entity);
          this.RemoveEntityFromSpace(entity, false);
          entity.Dispose();
        }
      }
    }

    protected virtual void RemoveEntityFromSpace(SpaceEntity entity, bool removeFromDatabase)
    {
      if (entity == null || !this.entitiesInSpaceAsync.Remove(entity))
        return;
      this.entityListAsync.Remove(entity);
      if (removeFromDatabase)
        this.world.RemoveEntity(entity.Id);
      if (this.EntityRemovedEvent == null)
        return;
      this.EntityRemovedEvent(entity);
    }

    protected void SweepAndPrune()
    {
      this.sweepAndPruneActiveList.Clear();
      SimpleMath.InsertionSort<SpaceEntity>(this.entityListAsync, this.entitySortDelegate);
      CollisionManager.Instance.BeginContactGroup();
      for (int index1 = 0; index1 < this.entityListAsync.Count; ++index1)
      {
        BoundingSphereI boundingSphere1 = this.entityListAsync[index1].BoundingSphere;
        for (int index2 = this.sweepAndPruneActiveList.Count - 1; index2 >= 0; --index2)
        {
          BoundingSphereI boundingSphere2 = this.sweepAndPruneActiveList[index2].BoundingSphere;
          if ((boundingSphere1.Center - Vector3.UnitX * boundingSphere1.Radius).X > (boundingSphere2.Center + Vector3.UnitX * boundingSphere2.Radius).X)
            this.sweepAndPruneActiveList.RemoveAt(index2);
          else
            CollisionManager.Instance.CollisionCheckWithBroadPhase(this.entityListAsync[index1], this.sweepAndPruneActiveList[index2]);
        }
        this.sweepAndPruneActiveList.Add(this.entityListAsync[index1]);
      }
      CollisionManager.Instance.EndContactGroup();
      CollisionManager.Instance.ResolveEntityCollisions();
    }

    protected void UpdateEntities()
    {
      for (int index = 0; index < this.entityListAsync.Count; ++index)
        this.entityListAsync[index].Update(Engine.FrameCounter.DeltaTime);
    }

    private bool UpdateSpaceSectorPosition(Position3 position)
    {
      Vector3int sectorCoords = this.world.GetSectorCoords(position);
      if (this.playerInSpaceSector.HasValue)
      {
        Vector3int vector3int = this.playerInSpaceSector.Value;
        if (sectorCoords.X != vector3int.X || sectorCoords.Y != vector3int.Y || sectorCoords.Z != vector3int.Z)
        {
          BoundingBox boundingBox = new BoundingBox((Vector3) vector3int * (float) this.world.SectorSize, (Vector3) (vector3int + new Vector3int(1, 1, 1)) * (float) this.world.SectorSize);
          if ((double) position.X < (double) boundingBox.Min.X - 32.0 || (double) position.X > (double) boundingBox.Max.X + 32.0)
          {
            this.playerInSpaceSector = new Vector3int?(sectorCoords);
            return true;
          }
          if ((double) position.Y < (double) boundingBox.Min.Y - 32.0 || (double) position.Y > (double) boundingBox.Max.Y + 32.0)
          {
            this.playerInSpaceSector = new Vector3int?(sectorCoords);
            return true;
          }
          if ((double) position.Z < (double) boundingBox.Min.Z - 32.0 || (double) position.Z > (double) boundingBox.Max.Z + 32.0)
          {
            this.playerInSpaceSector = new Vector3int?(sectorCoords);
            return true;
          }
        }
        return false;
      }
      this.playerInSpaceSector = new Vector3int?(sectorCoords);
      return true;
    }

    private void UpdateSpaceContents(IEnumerable<Player> players)
    {
      int num = Engine.FrameCounter.FrameNumber % 60;
      foreach (Player player in players)
      {
        Position3 position = player.Astronaut.Position;
        if (num == 30 && this.UpdateSpaceSectorPosition(position))
          this.LoadSpaceEntities(this.playerInSpaceSector.Value, position);
        if (num == 0)
          this.RemoveDistantEntities(players);
      }
    }

    public struct ConstructedBlockSector
    {
      public BlockSector Sector;
      public SpaceEntity Entity;
      public Vector3 Position;

      public ConstructedBlockSector(BlockSector bs, SpaceEntity e, Vector3 p)
      {
        this.Sector = bs;
        this.Entity = e;
        this.Position = p;
      }
    }

    public struct ConstructedSpaceEntity
    {
      public SpaceEntity Entity;
      public Vector3 Position;

      public ConstructedSpaceEntity(SpaceEntity entity, Vector3 position)
      {
        this.Entity = entity;
        this.Position = position;
      }
    }

    private delegate bool Intersects(BoundingSphere b1, BoundingSphere b2);

    private delegate int EntityContainsVector(SpaceEntity spaceship, Vector3 position);

    private enum AxisToSort : byte
    {
      X,
      Y,
      Z,
    }
  }
}
