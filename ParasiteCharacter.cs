// Decompiled with JetBrains decompiler
// Type: CornerSpace.ParasiteCharacter
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class ParasiteCharacter : HostileCharacter
  {
    private const float cMaxInstancesPerEntitySector = 1f;
    private const int cSpawnStepCount = 5;
    private const float cMaxUpkeepDistance = 32f;

    public ParasiteCharacter(string modelPath, string texturePath, float radius)
      : base(modelPath, texturePath, radius)
    {
    }

    public override void Dispose()
    {
      base.Dispose();
      this.BoardedEntity = (SpaceEntity) null;
    }

    public override SpaceEntity BoardedEntity
    {
      set
      {
        if (this.BoardedEntity != null)
        {
          --this.BoardedEntity.ParasiteHostileCount;
          this.BoardedEntity.DisposedEvent -= new System.Action(this.EntityRemovedDelegate);
        }
        base.BoardedEntity = value;
        if (value == null)
          return;
        ++value.ParasiteHostileCount;
        this.BoardedEntity.DisposedEvent += new System.Action(this.EntityRemovedDelegate);
      }
    }

    public static float MaxInstancesPerEntitySector => 1f;

    public override float MaxUpkeepDistance => 32f;

    protected ParasiteCharacter.SpawningPosition? CheckSpawningPosition(
      Position3 worldPosition,
      SpaceEntity closestEntity,
      params System.Type[] allowedBlockTypes)
    {
      if (allowedBlockTypes == null || allowedBlockTypes.Length == 0)
        return new ParasiteCharacter.SpawningPosition?();
      if (closestEntity == null)
        return new ParasiteCharacter.SpawningPosition?();
      BlockCell cell = closestEntity.GetBlockCell(worldPosition);
      if (cell != null)
      {
        Vector3 entityCoords = closestEntity.WorldCoordsToEntityCoords(worldPosition);
        for (int index1 = 0; index1 < AICharacter.directions.Length; ++index1)
        {
          Vector3 direction = AICharacter.directions[index1];
          Vector3 positionInEntityCoordinates1 = entityCoords;
          for (int index2 = 0; index2 < 5; ++index2)
          {
            positionInEntityCoordinates1 += direction;
            cell = closestEntity.GetBlockCellEntityCoords(positionInEntityCoordinates1);
            if (cell == null)
            {
              Vector3 positionInEntityCoordinates2 = positionInEntityCoordinates1 - direction;
              cell = closestEntity.GetBlockCellEntityCoords(positionInEntityCoordinates2);
              if (cell != null && Array.Find<System.Type>(allowedBlockTypes, (Predicate<System.Type>) (t => t == cell.Block.GetType())) != null)
              {
                ParasiteCharacter.SpawningPosition spawningPosition = new ParasiteCharacter.SpawningPosition();
                spawningPosition.FloorBlock = cell.Block;
                Vector3 position = new Vector3((float) SimpleMath.FastFloor(positionInEntityCoordinates2.X), (float) SimpleMath.FastFloor(positionInEntityCoordinates2.Y), (float) SimpleMath.FastFloor(positionInEntityCoordinates2.Z)) + Vector3.One * 0.5f;
                Vector3 vector3 = position;
                spawningPosition.FloorBlockPosition = closestEntity.EntityCoordsToWorldCoords(position);
                spawningPosition.SpawnPosition = closestEntity.EntityCoordsToWorldCoords(vector3 + direction);
                return new ParasiteCharacter.SpawningPosition?(spawningPosition);
              }
            }
          }
        }
      }
      return new ParasiteCharacter.SpawningPosition?();
    }

    private void EntityRemovedDelegate() => this.BoardedEntity = (SpaceEntity) null;

    protected struct SpawningPosition
    {
      public Position3 SpawnPosition;
      public Position3 FloorBlockPosition;
      public Block FloorBlock;
    }
  }
}
