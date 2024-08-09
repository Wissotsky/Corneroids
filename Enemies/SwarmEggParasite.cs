// Decompiled with JetBrains decompiler
// Type: CornerSpace.Enemies.SwarmEggParasite
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.Enemies
{
  public class SwarmEggParasite : EggParasite<NeedleParasite>
  {
    private const float cGooSpeed = 3f;
    private const float cHatchDistance = 5f;
    private const int cHatchingTimeMS = 5000;
    private const int cHealth = 5;
    private const float cMaxSkewAngle = 0.1f;
    private const int cMaxSpawnsPerCycle = 2;
    private const float cSkewAmount = 0.05f;
    private const float cSkewAnglePerSec = 5f;
    private const int cSpawnTimerMS = 1000;
    private const int cTimeBetweenGooMS = 50;
    private const int cUpdateIntervalMS = 500;
    private int enemySpawningTimerMS;
    private int hatchGooTimerMS;
    private int hatchingTimeMS;
    private Random random;
    private float skewAngle;
    private SwarmEggParasite.State state;

    public SwarmEggParasite()
      : base("Models\\egg", "Textures\\swarmEgg", 0.3f, new NeedleParasite[3]
      {
        new NeedleParasite(),
        new NeedleParasite(),
        new NeedleParasite()
      })
    {
      this.InitializationMatrix = Matrix.CreateRotationX(3.14159274f);
      this.random = new Random();
      this.state = SwarmEggParasite.State.Idling;
      this.InitialMaximumHealth = (short) 5;
      this.Health = (short) 5;
    }

    public override void AICycle(SpaceEntityManager entityManager)
    {
      if (this.state != SwarmEggParasite.State.Idling)
        return;
      Player closestPlayer = this.ManagerInterface.GetClosestPlayer(this.Position);
      if (closestPlayer == null || (double) (this.Position - closestPlayer.Astronaut.Position).Length() > 5.0)
        return;
      this.state = SwarmEggParasite.State.Hatching;
      this.Target = (PhysicalObject) closestPlayer.Astronaut;
    }

    public override bool CollisionCheckRequired() => false;

    public override void ShotBy(Projectile projectile)
    {
      if (this.state != SwarmEggParasite.State.Idling || projectile == null)
        return;
      PhysicalObject shotBy = projectile.ShotBy;
      this.state = SwarmEggParasite.State.Hatching;
      this.Target = (PhysicalObject) (shotBy as Character);
    }

    public override void SpawnAt(Position3 position, SpaceEntityManager entityManager)
    {
      this.Position = position;
      this.LookAt(this.eggNormal);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      this.ManagerInterface.GetClosestPlayer(this.Position);
      if (this.state == SwarmEggParasite.State.Idling)
      {
        this.skewAngle = MathHelper.WrapAngle(this.skewAngle + 5f * deltaTime);
        this.ModificationMatrix = Matrix.CreateScale(Vector3.One + Vector3.UnitZ * (float) Math.Sin((double) this.skewAngle) * 0.05f);
      }
      else
      {
        if (this.state != SwarmEggParasite.State.Hatching)
          return;
        this.hatchGooTimerMS += (int) Engine.FrameCounter.DeltaTimeMS;
        if (this.hatchGooTimerMS > 50)
          this.ManagerInterface.EnvironmentManager.AddThrusterFlame(this.Position, (this.eggNormal + new Vector3((float) (this.random.NextDouble() - 0.5) * 0.5f, (float) (this.random.NextDouble() - 0.5) * 0.5f, (float) (this.random.NextDouble() - 0.5) * 0.5f)) * 3f, Color.Green, 1000, 0.1f);
        this.hatchGooTimerMS %= 50;
        this.hatchingTimeMS += (int) Engine.FrameCounter.DeltaTimeMS;
        if (this.hatchingTimeMS >= 5000)
          this.ManagerInterface.RemoveCharacterSafe((AICharacter) this);
        this.enemySpawningTimerMS += (int) Engine.FrameCounter.DeltaTimeMS;
        if (this.enemySpawningTimerMS <= 1000 || this.hatchesTo.Count <= 0)
          return;
        NeedleParasite enemy = this.hatchesTo[this.hatchesTo.Count - 1];
        this.hatchesTo.RemoveAt(this.hatchesTo.Count - 1);
        enemy.Position = this.Position;
        enemy.Speed = this.eggNormal * 7f;
        enemy.Target = this.Target;
        this.LookAt(this.eggNormal);
        this.ManagerInterface.AddCharacterSafe((AICharacter) enemy);
        this.enemySpawningTimerMS %= 1000;
      }
    }

    public override int AIUpdateCycle => 500;

    public override int MaxHostileCountPerSpawnCycle => 2;

    private new enum State
    {
      Hatching,
      Idling,
    }
  }
}
