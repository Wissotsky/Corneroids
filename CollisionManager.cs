// Decompiled with JetBrains decompiler
// Type: CornerSpace.CollisionManager
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
  public class CollisionManager
  {
    private List<OctTreeCollisionPoint<BlockSector>> blockSectorPairList;
    private List<OctTreeCollisionPoint<Block>> blockPairList;
    private Queue<NodePair<OctTreeNode<BlockSector>>> blockSectorRecursionQueue;
    private bool contactGroupSelection;
    private List<KeyValuePair<SpaceEntity, SpaceEntity>> collisionEntityPairs;
    private static CollisionManager instance;

    private CollisionManager()
    {
      this.contactGroupSelection = false;
      this.collisionEntityPairs = new List<KeyValuePair<SpaceEntity, SpaceEntity>>();
      this.blockSectorPairList = new List<OctTreeCollisionPoint<BlockSector>>();
      this.blockPairList = new List<OctTreeCollisionPoint<Block>>();
      this.blockSectorRecursionQueue = new Queue<NodePair<OctTreeNode<BlockSector>>>();
    }

    public void BeginContactGroup()
    {
      this.contactGroupSelection = true;
      this.collisionEntityPairs.Clear();
    }

    public bool BroadPhaseCollisionCheck(PhysicalObject object1, PhysicalObject object2)
    {
      if (object1 == null || object2 == null)
        return false;
      BoundingSphereI boundingSphere1 = object1.BoundingSphere;
      BoundingSphereI boundingSphere2 = object2.BoundingSphere;
      double num = (double) (boundingSphere1.Center - boundingSphere2.Center).Length();
      return (double) (boundingSphere1.Center - boundingSphere2.Center).Length() <= (double) boundingSphere1.Radius + (double) boundingSphere2.Radius;
    }

    public bool CheckAstronautCharacterCollision(Astronaut astronaut, AICharacter character)
    {
      if (astronaut != null && character != null)
      {
        CollisionData collisionData = new CollisionData();
        if (this.CheckCharacterCharacterCollision((Character) astronaut, (Character) character, ref collisionData))
        {
          if (astronaut.BoardedEntity == null)
          {
            Astronaut astronaut1 = astronaut;
            astronaut1.Position = astronaut1.Position + collisionData.CollisionNormal * collisionData.CollisionDepth * 0.5f;
          }
          else
          {
            Astronaut astronaut2 = astronaut;
            astronaut2.PositionOnBoardedEntity = astronaut2.PositionOnBoardedEntity + astronaut.BoardedEntity.WorldNormalToEntityNormal(collisionData.CollisionNormal) * collisionData.CollisionDepth;
          }
          if (character.BoardedEntity == null)
          {
            AICharacter aiCharacter = character;
            aiCharacter.Position = aiCharacter.Position - collisionData.CollisionNormal * collisionData.CollisionDepth * 0.5f;
          }
          else
          {
            AICharacter aiCharacter = character;
            aiCharacter.PositionOnBoardedEntity = aiCharacter.PositionOnBoardedEntity - character.BoardedEntity.WorldNormalToEntityNormal(collisionData.CollisionNormal) * collisionData.CollisionDepth;
          }
          astronaut.CollisionWithCharacter(character);
        }
      }
      return false;
    }

    public bool CheckCharacterCharacterCollision(
      Character char1,
      Character char2,
      ref CollisionData collisionData)
    {
      if (char1 == null || char2 == null || char1 == char2)
        return false;
      float num1 = (char1.Position - char2.Position).Length();
      if ((double) num1 > (double) char2.BoundingSphere.Radius + (double) char1.BoundingSphere.Radius)
        return false;
      float num2 = char1.BoundingSphere.Radius + char2.BoundingSphere.Radius - num1;
      Vector3 vector3 = char1.Position - char2.Position;
      collisionData.CollisionDepth = num2;
      if (vector3 != Vector3.Zero)
        collisionData.CollisionNormal = Vector3.Normalize(vector3);
      return true;
    }

    public bool CheckCharacterEntityCollision(Character character, SpaceEntity entity)
    {
      if (character == null || entity == null)
        return false;
      CollisionData collisionData = new CollisionData();
      if (!this.GetCollision(entity, character, ref collisionData))
        return false;
      double num1 = (double) character.Speed.Length();
      Vector3 speed = character.Speed;
      Vector3 normal1 = collisionData.CollisionNormal * collisionData.CollisionDepth;
      Vector3 vector3_1 = Vector3.TransformNormal(collisionData.CollisionNormal, entity.TransformMatrix);
      Vector3 normal2 = Vector3.TransformNormal(normal1, entity.TransformMatrix);
      if (character.BoardedEntity != null)
      {
        Vector3 vector3_2 = Vector3.TransformNormal(normal2, character.BoardedEntity.InverseTransformMatrix);
        Vector3 vector2 = Vector3.TransformNormal(vector3_1, character.BoardedEntity.InverseTransformMatrix);
        character.PositionOnBoardedEntity += vector3_2;
        if (character.SpeedOnBoardedEntity != Vector3.Zero)
          character.SpeedOnBoardedEntity -= SimpleMath.FastMin(Vector3.Dot(character.SpeedOnBoardedEntity, vector2), 0.0f) * vector2;
      }
      else
      {
        Character character1 = character;
        character1.Position = character1.Position + normal2;
        if (character.Speed != Vector3.Zero)
        {
          Character character2 = character;
          character2.Speed = character2.Speed - SimpleMath.FastMin(Vector3.Dot(character.Speed, vector3_1), 0.0f) * vector3_1;
        }
      }
      float num2 = (Vector3.Dot(speed, vector3_1) * vector3_1).Length();
      if ((double) num2 >= (double) character.CollisionDamageSpeedTreshold)
      {
        float num3 = num2 - character.CollisionDamageSpeedTreshold;
        int num4 = (int) ((double) character.CollisionDamagePerSpeedUnit * (double) num3);
        if (num4 > 0)
        {
          character.ReduceHealth(num4);
          character.DamageTaken(character.Position + character.Speed);
        }
      }
      return true;
    }

    public Position3? CheckCharacterProjectileCollision(Character character, Projectile projectile)
    {
      if (projectile != null && character != null)
      {
        BoundingSphereI boundingSphere = character.BoundingSphere;
        boundingSphere.Radius *= 1.5f;
        float num1 = (projectile.Speed * Engine.FrameCounter.DeltaTime).Length();
        int num2 = SimpleMath.FastMax((int) ((double) num1 / 0.10000000149011612), 1);
        if (num2 > 0)
        {
          Position3 position3 = projectile.Position - projectile.Direction * num1;
          for (int index = 0; index < num2; ++index)
          {
            position3 += projectile.Direction * num1 / (float) num2;
            if ((double) (boundingSphere.Center - position3).Length() <= (double) boundingSphere.Radius)
              return new Position3?(position3);
          }
        }
      }
      return new Position3?();
    }

    public Position3? CheckEntityProjectileCollision(SpaceEntity entity, Projectile projectile)
    {
      if ((double) (entity.BoundingSphere.Center - projectile.Position).LengthSquared() <= (double) entity.BoundingSphere.Radius * (double) entity.BoundingSphere.Radius)
      {
        float num1 = (projectile.Speed * Engine.FrameCounter.DeltaTime).Length();
        int num2 = SimpleMath.FastMax((int) ((double) num1 / 0.30000001192092896), 1);
        if (num2 > 0)
        {
          Position3 worldPosition = projectile.Position - projectile.Direction * num1;
          for (int index = 0; index < num2; ++index)
          {
            worldPosition += projectile.Direction * num1 / (float) num2;
            if (entity.GetCollision(worldPosition) != null)
              return new Position3?(worldPosition);
          }
        }
      }
      return new Position3?();
    }

    public void CollisionCheckWithBroadPhase(SpaceEntity o1, SpaceEntity o2)
    {
      if (o1 == null || o2 == null || !this.contactGroupSelection || !this.BroadPhaseCollisionCheck((PhysicalObject) o1, (PhysicalObject) o2))
        return;
      this.collisionEntityPairs.Add(new KeyValuePair<SpaceEntity, SpaceEntity>(o1, o2));
    }

    public void EndContactGroup() => this.contactGroupSelection = false;

    public bool GetCollision(
      SpaceEntity entity,
      Character character,
      ref CollisionData collisionData)
    {
      return entity != null && character != null && this.GetCollision(entity, character.BoundingSphere, character.Speed, ref collisionData, true);
    }

    public bool GetCollision(
      SpaceEntity entity,
      BoundingSphereI boundingSphere,
      Vector3 sphereSpeed,
      ref CollisionData collisionData,
      bool notifyBlocks)
    {
      if (entity == null || (double) (entity.Position - boundingSphere.Center).Length() > (double) entity.BoundingSphere.Radius + (double) boundingSphere.Radius)
        return false;
      int num1 = SimpleMath.FastMax((int) ((double) ((sphereSpeed - entity.Speed) * Engine.FrameCounter.DeltaTime).Length() / 0.5), 1);
      for (int index = 0; index < num1; ++index)
      {
        int num2 = 0;
        Vector3 zero = Vector3.Zero;
        BoundingSphere boundingSphere1 = new BoundingSphere(Vector3.Zero, boundingSphere.Radius);
        boundingSphere1.Center = entity.WorldCoordsToEntityCoords(boundingSphere.Center);
        Vector3 vector3_1 = Vector3.One * 0.5f + new Vector3((float) Math.Floor((double) boundingSphere1.Center.X), (float) Math.Floor((double) boundingSphere1.Center.Y), (float) Math.Floor((double) boundingSphere1.Center.Z));
        int num3 = (int) Math.Ceiling((double) boundingSphere1.Radius);
        for (float z = vector3_1.Z - (float) num3; (double) z <= (double) vector3_1.Z + (double) num3; ++z)
        {
          for (float y = vector3_1.Y - (float) num3; (double) y <= (double) vector3_1.Y + (double) num3; ++y)
          {
            for (float x = vector3_1.X - (float) num3; (double) x <= (double) vector3_1.X + (double) num3; ++x)
            {
              Vector3 positionInEntityCoordinates = new Vector3(x, y, z);
              BlockCell cellEntityCoords = entity.GetBlockCellEntityCoords(positionInEntityCoordinates);
              if (cellEntityCoords != null && cellEntityCoords.Block.HasCollision)
              {
                Vector3 size = (Vector3) cellEntityCoords.Block.Size;
                Vector3 modelSize = (Vector3) cellEntityCoords.Block.ModelSize;
                Vector3 modelPlacement = cellEntityCoords.Block.ModelPlacement;
                Vector3 vector3_2 = -new Vector3((float) cellEntityCoords.X, (float) cellEntityCoords.Y, (float) cellEntityCoords.Z) + new Vector3((float) (int) Math.Floor((double) positionInEntityCoordinates.X), (float) (int) Math.Floor((double) positionInEntityCoordinates.Y), (float) (int) Math.Floor((double) positionInEntityCoordinates.Z));
                Vector3 vector3_3 = vector3_2 + size;
                if (cellEntityCoords.Block.ModelPlacement != Vector3.Zero)
                {
                  vector3_2 = 0.5f * (vector3_2 + vector3_3) + modelPlacement - 0.5f * modelSize;
                  vector3_3 = vector3_2 + modelSize;
                }
                Vector3 vector3_4 = new Vector3()
                {
                  X = Math.Min(Math.Max(boundingSphere1.Center.X, vector3_2.X), vector3_3.X),
                  Y = Math.Min(Math.Max(boundingSphere1.Center.Y, vector3_2.Y), vector3_3.Y),
                  Z = Math.Min(Math.Max(boundingSphere1.Center.Z, vector3_2.Z), vector3_3.Z)
                };
                Vector3 vector3_5 = boundingSphere1.Center - vector3_4;
                if ((double) vector3_5.Length() <= (double) boundingSphere1.Radius && vector3_5 != Vector3.Zero)
                {
                  Vector3 vector3_6 = Vector3.Normalize(vector3_5);
                  zero += vector3_6 * boundingSphere1.Radius - vector3_5;
                  ++num2;
                  if (notifyBlocks)
                    cellEntityCoords.Block.CollidingWithObject(-vector3_6);
                }
              }
            }
          }
        }
        if (num2 > 0)
        {
          collisionData.CollisionDepth = zero.Length() / (float) num2;
          collisionData.CollisionNormal = (double) collisionData.CollisionDepth <= 0.0 ? Vector3.Zero : Vector3.Normalize(zero);
          return true;
        }
      }
      return false;
    }

    public void ResolveEntityCollisions()
    {
      if (this.contactGroupSelection || this.collisionEntityPairs.Count <= 0)
        return;
      foreach (KeyValuePair<SpaceEntity, SpaceEntity> collisionEntityPair in this.collisionEntityPairs)
      {
        this.blockSectorPairList.Clear();
        this.blockPairList.Clear();
        SpaceEntity key = collisionEntityPair.Key;
        SpaceEntity e2 = collisionEntityPair.Value;
        if (!(key.Speed == Vector3.Zero) || !(key.Rotation == Vector3.Zero) || !(e2.Speed == Vector3.Zero) || !(e2.Rotation == Vector3.Zero))
        {
          Vector3[] coordinateAxises1 = key.CoordinateAxises;
          Vector3[] coordinateAxises2 = e2.CoordinateAxises;
          OctTree<OctTreeNode<BlockSector>, BlockSector> octTree1 = key.BlockSectorManager.OctTree;
          OctTree<OctTreeNode<BlockSector>, BlockSector> octTree2 = e2.BlockSectorManager.OctTree;
          Position3 worldCoords1 = key.EntityCoordsToWorldCoords(octTree1.Position);
          Position3 worldCoords2 = e2.EntityCoordsToWorldCoords(octTree2.Position);
          octTree1.GetCollidingLeafNodes(octTree2, coordinateAxises1, coordinateAxises2, worldCoords1, worldCoords2, this.blockSectorPairList, 16);
          for (int index = 0; index < this.blockSectorPairList.Count; ++index)
          {
            OctTreeLeafNode<BlockSector> nodeOne = this.blockSectorPairList[index].NodeOne as OctTreeLeafNode<BlockSector>;
            OctTreeLeafNode<BlockSector> nodeTwo = this.blockSectorPairList[index].NodeTwo as OctTreeLeafNode<BlockSector>;
            if (nodeOne != null && nodeTwo != null && nodeOne != nodeTwo)
              nodeOne.Data.OctTree.GetCollidingLeafNodes((OctTree<OctTreeNode<Block>, Block>) nodeTwo.Data.OctTree, coordinateAxises1, coordinateAxises2, this.blockSectorPairList[index].NodeOnePosition, this.blockSectorPairList[index].NodeTwoPosition, this.blockPairList, 1);
          }
          if (this.blockPairList.Count > 0)
            this.ResolveCollision(key, e2, this.blockPairList);
        }
      }
    }

    public static CollisionManager Instance
    {
      get
      {
        if (CollisionManager.instance == null)
          CollisionManager.instance = new CollisionManager();
        return CollisionManager.instance;
      }
    }

    private void ResolveCollision(
      SpaceEntity e1,
      SpaceEntity e2,
      List<OctTreeCollisionPoint<Block>> collidingBlocks)
    {
      Position3 position1 = e1.Position;
      Position3 position2 = e2.Position;
      Vector3[] coordinateAxises1 = e1.CoordinateAxises;
      Vector3[] coordinateAxises2 = e2.CoordinateAxises;
      Vector3 vector3_1 = e1.Speed * e1.Mass + e2.Speed * e2.Mass;
      Vector3 rotation1 = e1.Rotation;
      Vector3 rotation2 = e2.Rotation;
      float mass1 = e1.Mass;
      float mass2 = e2.Mass;
      Vector3 inertiaTensor1 = e1.InertiaTensor;
      Vector3 inertiaTensor2 = e2.InertiaTensor;
      float num1 = 0.0f;
      Vector3 zero1 = Vector3.Zero;
      Vector3 zero2 = Vector3.Zero;
      for (int index = 0; index < collidingBlocks.Count; ++index)
      {
        Position3 nodeOnePosition = collidingBlocks[index].NodeOnePosition;
        Position3 nodeTwoPosition = collidingBlocks[index].NodeTwoPosition;
        float num2 = (nodeOnePosition - nodeTwoPosition).Length();
        Position3 position3 = nodeOnePosition + (nodeTwoPosition - nodeOnePosition) * 0.5f;
        Vector3 vector3_2 = position3 - position1;
        Vector3 vector3_3 = position3 - position2;
        Vector3 vector3_4 = Vector3.Normalize(nodeOnePosition - nodeTwoPosition);
        Vector3 vector3_5 = e1.Speed + Vector3.Cross(rotation1, vector3_2);
        Vector3 vector3_6 = e2.Speed + Vector3.Cross(rotation2, vector3_3);
        float num3 = -1f * Vector3.Dot(vector3_4, vector3_5 - vector3_6);
        float num4 = 1f / mass1;
        float num5 = 1f / mass2;
        Vector3 vector3_7 = Vector3.Cross(vector3_2, vector3_4);
        Vector3 vector3_8 = Vector3.Cross(vector3_3, vector3_4);
        Vector3 vector1_1 = new Vector3(vector3_7.X / inertiaTensor1.X, vector3_7.Y / inertiaTensor1.Y, vector3_7.Z / inertiaTensor1.Z);
        Vector3 vector1_2 = new Vector3(vector3_8.X / inertiaTensor2.X, vector3_8.Y / inertiaTensor2.Y, vector3_8.Z / inertiaTensor2.Z);
        float num6 = num4 + num5 + Vector3.Dot(Vector3.Cross(vector1_1, vector3_2), vector3_4) + Vector3.Dot(Vector3.Cross(vector1_2, vector3_3), vector3_4);
        float num7 = num3 / num6;
        SpaceEntity spaceEntity1 = e1;
        spaceEntity1.Speed = spaceEntity1.Speed + SimpleMath.FastMax(num7 / mass1, 0.0f) * vector3_4;
        SpaceEntity spaceEntity2 = e1;
        spaceEntity2.Rotation = spaceEntity2.Rotation + vector1_1 * num7;
        SpaceEntity spaceEntity3 = e2;
        spaceEntity3.Speed = spaceEntity3.Speed - SimpleMath.FastMax(num7 / mass2, 0.0f) * vector3_4;
        SpaceEntity spaceEntity4 = e2;
        spaceEntity4.Rotation = spaceEntity4.Rotation - vector1_2 * num7;
        float num8 = 1.7321f - num2;
        if ((double) num8 > 0.0)
        {
          zero1 += Vector3.Normalize(nodeOnePosition - position3) * num8 * 0.5f * e1.Mass / (e1.Mass + e2.Mass);
          zero2 += Vector3.Normalize(nodeTwoPosition - position3) * num8 * 0.5f * e2.Mass / (e1.Mass + e2.Mass);
          ++num1;
        }
      }
      if ((double) num1 <= 0.0)
        return;
      SpaceEntity spaceEntity5 = e1;
      spaceEntity5.Position = spaceEntity5.Position + zero1 / num1;
      SpaceEntity spaceEntity6 = e2;
      spaceEntity6.Position = spaceEntity6.Position + zero2 / num1;
    }
  }
}
