// Decompiled with JetBrains decompiler
// Type: CornerSpace.Enemies.MineralParasite
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Enemies
{
  public class MineralParasite : ParasiteCharacter
  {
    private const float cAttackAccrelation = 10f;
    private const float cAttackDistance = 4f;
    private const int cEatingTimeOfBlockMS = 20000;
    private const int cAttackChargeTime = 1000;
    private const int cHealth = 20;
    private const float cMaxAttackDistance = 16f;
    private const int cMaxSpawnsPerCycle = 4;
    private const float cSpeed = 0.3f;
    private const float cSpeedWhenAttacking = 1f;
    private const int cUpdateIntervalMS = 500;
    private int attackTimerMS;
    private Block eatingBlock;
    private Position3 eatingBlockPosition;
    private int eatingTimeMS;
    private Vector3 normal;
    private Random rng;
    private MineralParasite.State state;
    private Position3 targetPosition;
    private new Matrix transformMatrix;
    private float wiggleAngle;

    public MineralParasite()
      : base("Models\\mineralParasite", "Textures\\mineralParasite", 0.4f)
    {
      this.InitializationMatrix = Matrix.CreateRotationX(3.14159274f);
      this.rng = new Random();
      this.InflictedDamage = 10;
      this.Health = (short) 20;
      this.InitialMaximumHealth = (short) 20;
      this.state = MineralParasite.State.Doing_nothing;
    }

    public override void AICycle(SpaceEntityManager entityManager)
    {
      if (this.BoardedEntity == null)
        return;
      if (this.state == MineralParasite.State.Eating_mineral)
        this.StateEatMineral(entityManager);
      else if (this.state == MineralParasite.State.Searching_minerals)
        this.StateSearchMinerals(entityManager);
      if (this.state == MineralParasite.State.Attacking_speeding_down || this.state == MineralParasite.State.Attacking_Speeding_up)
        return;
      Player closestPlayer = this.ManagerInterface.GetClosestPlayer(this.Position);
      if (closestPlayer == null || (double) (closestPlayer.Astronaut.Position - this.Position).Length() > 4.0)
        return;
      this.Target = (PhysicalObject) closestPlayer.Astronaut;
      Vector3 vector3 = this.Target.Position - this.Position;
      if (!(vector3 != Vector3.Zero))
        return;
      this.state = MineralParasite.State.Attacking_Speeding_up;
      this.attackTimerMS = 0;
      this.normal = Vector3.Normalize(vector3);
      this.InflictsDamageOnTouch = true;
      Action<Character> diedListener = (Action<Character>) null;
      diedListener = (Action<Character>) (character =>
      {
        this.state = MineralParasite.State.Searching_minerals;
        closestPlayer.Astronaut.DiedEvent -= diedListener;
      });
      closestPlayer.Astronaut.DiedEvent += diedListener;
    }

    public override bool CollisionCheckRequired()
    {
      return this.state != MineralParasite.State.Eating_mineral;
    }

    public override void ShotBy(Projectile projectile)
    {
      if (projectile == null || this.state == MineralParasite.State.Attacking_Speeding_up || this.state == MineralParasite.State.Attacking_speeding_down)
        return;
      PhysicalObject shotBy = projectile.ShotBy;
      if (shotBy == null)
        return;
      Astronaut shotByAstronaut = shotBy as Astronaut;
      if (shotByAstronaut == null || (double) (this.Position - shotByAstronaut.Position).LengthSquared() > 256.0)
        return;
      this.Target = (PhysicalObject) shotByAstronaut;
      Vector3 vector3 = this.Target.Position - this.Position;
      if (!(vector3 != Vector3.Zero))
        return;
      this.state = MineralParasite.State.Attacking_Speeding_up;
      this.attackTimerMS = 0;
      this.normal = Vector3.Normalize(vector3);
      this.InflictsDamageOnTouch = true;
      Action<Character> diedListener = (Action<Character>) null;
      diedListener = (Action<Character>) (astronaut =>
      {
        this.state = MineralParasite.State.Searching_minerals;
        shotByAstronaut.DiedEvent -= diedListener;
      });
      shotByAstronaut.DiedEvent += diedListener;
    }

    public override void SpawnAt(Position3 position, SpaceEntityManager entityManager)
    {
      this.Position = position;
      this.LookAt(this.normal);
      this.state = MineralParasite.State.Eating_mineral;
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      int deltaTimeMs = (int) Engine.FrameCounter.DeltaTimeMS;
      float deltaTime1 = Engine.FrameCounter.DeltaTime;
      this.wiggleAngle = MathHelper.WrapAngle(this.wiggleAngle + deltaTime1 * 3.14159274f);
      if (this.state != MineralParasite.State.Attacking_Speeding_up)
        this.transformMatrix = Matrix.CreateRotationX(3.14159274f) * Matrix.CreateScale((float) (1.0 + Math.Sin((double) this.wiggleAngle) * 0.05000000074505806), (float) (1.0 + Math.Sin((double) this.wiggleAngle + 1.5707963705062866) * 0.05000000074505806), (float) (1.0 + Math.Sin((double) this.wiggleAngle + 3.1415927410125732) * 0.05000000074505806));
      this.InitializationMatrix = this.transformMatrix;
      if (this.state == MineralParasite.State.Eating_mineral)
        this.eatingTimeMS += deltaTimeMs;
      else if (this.state == MineralParasite.State.Going_to_mineral)
      {
        Vector3 vector3 = this.targetPosition - this.Position;
        if ((double) vector3.LengthSquared() <= 0.010000000707805157)
        {
          this.state = MineralParasite.State.Eating_mineral;
          this.Position = this.targetPosition;
          this.LookAt(this.BoardedEntity.EntityNormalToWorldNormal(this.normal));
        }
        else
        {
          float val1 = 0.3f * deltaTime1;
          float val2 = vector3.Length();
          if (!(vector3 != Vector3.Zero))
            return;
          Vector3 normal = Vector3.Normalize(vector3);
          if (this.BoardedEntity == null)
          {
            MineralParasite mineralParasite = this;
            mineralParasite.Position = mineralParasite.Position + normal * Math.Min(val1, val2);
          }
          else
          {
            normal = this.BoardedEntity.WorldNormalToEntityNormal(normal);
            MineralParasite mineralParasite = this;
            mineralParasite.PositionOnBoardedEntity = mineralParasite.PositionOnBoardedEntity + normal * Math.Min(val1, val2);
          }
          this.LookAt(-normal);
        }
      }
      else if (this.state == MineralParasite.State.Wandering_around)
      {
        Vector3 vector3_1 = this.targetPosition - this.Position;
        if ((double) vector3_1.LengthSquared() <= 0.010000000707805157)
        {
          this.state = MineralParasite.State.Searching_minerals;
          this.Position = this.targetPosition;
        }
        else
        {
          float val1 = 0.3f * deltaTime1;
          float val2 = vector3_1.Length();
          if (!(vector3_1 != Vector3.Zero))
            return;
          Vector3 vector3_2 = Vector3.Normalize(vector3_1);
          MineralParasite mineralParasite = this;
          mineralParasite.Position = mineralParasite.Position + vector3_2 * Math.Min(val1, val2);
          this.LookAt(-vector3_2);
        }
      }
      else if (this.state == MineralParasite.State.Attacking_Speeding_up)
      {
        this.ApplyForce(Vector3.Zero, this.normal * 10f, Engine.FrameCounter.DeltaTime);
        this.LookAt(-this.normal);
        this.attackTimerMS += deltaTimeMs;
        if (this.attackTimerMS < 1000)
          return;
        this.state = MineralParasite.State.Attacking_speeding_down;
      }
      else
      {
        if (this.state != MineralParasite.State.Attacking_speeding_down || (double) this.Speed.LengthSquared() > 0.090000003576278687)
          return;
        if ((double) (this.Position - this.Target.Position).LengthSquared() <= 256.0)
        {
          Vector3 vector3 = this.Target.Position - this.Position;
          if (!(vector3 != Vector3.Zero))
            return;
          this.state = MineralParasite.State.Attacking_Speeding_up;
          this.normal = Vector3.Normalize(vector3);
          this.attackTimerMS = 0;
        }
        else
          this.state = MineralParasite.State.Doing_nothing;
      }
    }

    public override Position3? CheckPositionForSpawning(
      Position3 worldPosition,
      SpaceEntity closestEntity)
    {
      ParasiteCharacter.SpawningPosition? nullable = this.CheckSpawningPosition(worldPosition, closestEntity, typeof (MineralBlock), typeof (AsteroidBlock));
      if (!nullable.HasValue)
        return new Position3?();
      this.eatingBlock = nullable.Value.FloorBlock;
      this.normal = Vector3.Normalize(nullable.Value.SpawnPosition - nullable.Value.FloorBlockPosition);
      this.eatingBlockPosition = nullable.Value.FloorBlockPosition;
      return new Position3?(nullable.Value.SpawnPosition);
    }

    public override int AIUpdateCycle => 500;

    public override int MaxHostileCountPerSpawnCycle => 4;

    private void StateEatMineral(SpaceEntityManager entityManager)
    {
      BlockCell blockCell = this.BoardedEntity.GetBlockCell(this.eatingBlockPosition);
      if (blockCell != null)
      {
        if (blockCell.Block != this.eatingBlock)
        {
          this.state = MineralParasite.State.Searching_minerals;
        }
        else
        {
          if (this.eatingTimeMS < 20000)
            return;
          entityManager.DestroyBlock(this.eatingBlockPosition);
          this.eatingBlock = (Block) null;
          this.eatingTimeMS = 0;
          this.state = MineralParasite.State.Searching_minerals;
        }
      }
      else
        this.state = MineralParasite.State.Searching_minerals;
    }

    private void StateSearchMinerals(SpaceEntityManager entityManager)
    {
      int num = this.rng.Next(0, 5);
      Vector3 entityCoords = this.BoardedEntity.WorldCoordsToEntityCoords(this.Position);
      for (int index1 = 0; index1 < 6; ++index1)
      {
        int index2 = (num + index1) % 6;
        for (int index3 = 1; index3 < 3; ++index3)
        {
          Vector3 vector3_1 = entityCoords + AICharacter.directions[index2] * (float) index3;
          BlockCell cellEntityCoords = this.BoardedEntity.GetBlockCellEntityCoords(vector3_1);
          if (cellEntityCoords != null && (cellEntityCoords.Block is AsteroidBlock || cellEntityCoords.Block is MineralBlock))
          {
            this.state = MineralParasite.State.Going_to_mineral;
            this.eatingBlock = cellEntityCoords.Block;
            this.eatingBlockPosition = this.BoardedEntity.EntityCoordsToWorldCoords(vector3_1);
            this.normal = -AICharacter.directions[index2];
            this.targetPosition = this.BoardedEntity.EntityCoordsToWorldCoords(vector3_1 - AICharacter.directions[index2]);
            Vector3 vector3_2 = this.targetPosition - this.Position;
            return;
          }
        }
      }
      this.state = MineralParasite.State.Wandering_around;
      for (int index4 = 0; index4 < AICharacter.directions.Length; ++index4)
      {
        int index5 = (num + index4) % 6;
        if (this.BoardedEntity.GetBlockCellEntityCoords(entityCoords + AICharacter.directions[index5]) == null)
        {
          this.targetPosition = this.BoardedEntity.EntityCoordsToWorldCoords(entityCoords + AICharacter.directions[index5]);
          this.normal = -AICharacter.directions[index5];
        }
      }
    }

    private void UpdateOrientation()
    {
    }

    private new enum State
    {
      Attacking_Speeding_up,
      Attacking_speeding_down,
      Doing_nothing,
      Eating_mineral,
      Going_to_mineral,
      Hiding,
      Searching_minerals,
      Wandering_around,
    }
  }
}
