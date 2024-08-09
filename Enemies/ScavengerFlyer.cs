// Decompiled with JetBrains decompiler
// Type: CornerSpace.Enemies.ScavengerFlyer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.Enemies
{
  public class ScavengerFlyer : SmallFlyerCharacter
  {
    private const int cAIUpdateCycleIntervalMS = 1000;
    private const float cAttackDistance = 24f;
    private const int cHealth = 20;
    private const float cRotateSpeed = 0.7853982f;
    private const int cReloadTimeMS = 2000;
    private const float cTravelSpeed = 1f;
    private Vector3 lookingAt;
    private Vector3 movementDirection;
    private PointLight light;
    private int reloadTimerMS;
    private ScavengerFlyer.State state;
    private Position3 targetPosition;
    private static ProjectileType projectileType;

    public ScavengerFlyer()
      : base("Models\\scavenger", "Textures\\scavenger", 1f)
    {
      this.state = ScavengerFlyer.State.Wandering_around;
      this.lookingAt = Vector3.UnitX;
      PointLight pointLight = new PointLight(Color.Red, 5f);
      pointLight.Enabled = true;
      this.light = pointLight;
      this.InitializeProjectileType();
      this.InitialMaximumHealth = this.Health = (short) 20;
      this.InitializationMatrix = Matrix.CreateRotationX(1.57079637f);
    }

    public override void AICycle(SpaceEntityManager entityManager)
    {
      if (this.state != ScavengerFlyer.State.Wandering_around)
        return;
      Player closestPlayer = this.ManagerInterface.GetClosestPlayer(this.Position);
      if (closestPlayer != null && (double) (this.Position - closestPlayer.Astronaut.Position).Length() <= 24.0)
      {
        this.Target = (PhysicalObject) closestPlayer.Astronaut;
        if (closestPlayer.Astronaut.MountedTo != null)
          return;
        this.state = ScavengerFlyer.State.Attacking;
        Vector3 vector = new Vector3((float) this.Random.NextDouble(), (float) this.Random.NextDouble(), (float) this.Random.NextDouble());
        Vector3 normal = Vector3.Normalize(closestPlayer.Astronaut.Position - this.Position);
        this.movementDirection = vector - SimpleMath.VectorProjection(vector, normal);
        if (!(this.movementDirection != Vector3.Zero))
          return;
        this.movementDirection.Normalize();
      }
      else
      {
        if ((double) (this.Position - this.targetPosition).Length() > 2.0)
          return;
        this.targetPosition = this.Position + new Vector3((float) this.Random.NextDouble() * 10f, (float) this.Random.NextDouble() * 10f, (float) this.Random.NextDouble() * 10f);
      }
    }

    public override bool CollisionCheckRequired() => true;

    public override void RenderThirdPerson(
      IRenderCamera camera,
      ILightingInterface lightingInterface)
    {
      base.RenderThirdPerson(camera, lightingInterface);
      lightingInterface?.DrawLight((LightSource) this.light);
    }

    public override void ShotBy(Projectile projectile)
    {
      if (this.state != ScavengerFlyer.State.Wandering_around || !(projectile.ShotBy is Astronaut shotBy))
        return;
      this.Target = (PhysicalObject) shotBy;
      this.state = ScavengerFlyer.State.Attacking;
    }

    public override void SpawnAt(Position3 position, SpaceEntityManager entityManager)
    {
      this.Position = position;
      this.targetPosition = position;
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (this.state == ScavengerFlyer.State.Attacking && this.Target != null)
      {
        Astronaut target = this.Target as Astronaut;
        Vector3 vector2_1 = this.Target.Position - this.Position;
        Vector3 vector2_2 = vector2_1 != Vector3.Zero ? Vector3.Normalize(vector2_1) : Vector3.Zero;
        float num1 = (float) Math.Acos((double) Vector3.Dot(this.lookingAt, vector2_2));
        float num2 = vector2_1.Length();
        if (target != null && vector2_1 != Vector3.Zero)
        {
          vector2_1.Normalize();
          if ((double) num1 > 0.0)
          {
            if ((double) num1 <= 0.78539818525314331 * (double) deltaTime)
            {
              this.lookingAt = vector2_1;
            }
            else
            {
              Matrix fromAxisAngle = Matrix.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(this.lookingAt, vector2_1)), 0.7853982f * deltaTime);
              Vector3.TransformNormal(ref this.lookingAt, ref fromAxisAngle, out this.lookingAt);
            }
          }
        }
        this.ApplyForce(Vector3.Zero, this.movementDirection, Engine.FrameCounter.DeltaTime);
        if ((double) num2 >= 0.0)
          this.ApplyForce(Vector3.Zero, vector2_2 * (num2 - 16f), Engine.FrameCounter.DeltaTime);
        this.reloadTimerMS -= (int) Engine.FrameCounter.DeltaTimeMS;
        this.reloadTimerMS = Math.Max(this.reloadTimerMS, 0);
        if (this.reloadTimerMS == 0 && (double) num1 <= 0.39269909262657166)
        {
          this.ManagerInterface.ProjectileManager.AddProjectile(new Projectile(ScavengerFlyer.projectileType, this.Position + vector2_2 * 2f, this.lookingAt));
          this.reloadTimerMS = 2000;
        }
      }
      else if (this.state == ScavengerFlyer.State.Wandering_around)
      {
        Vector3 vector3 = Vector3.Normalize(this.targetPosition - this.Position);
        this.lookingAt = vector3;
        ScavengerFlyer scavengerFlyer = this;
        scavengerFlyer.Position = scavengerFlyer.Position + vector3 * 1f * Engine.FrameCounter.DeltaTime;
      }
      this.LookAt(this.lookingAt);
      this.light.Position = this.Position + this.lookingAt * 2f;
    }

    public override int AIUpdateCycle => 1000;

    public override int MaxHostileCountPerSpawnCycle => 1;

    private void InitializeProjectileType()
    {
      if (ScavengerFlyer.projectileType != null)
        return;
      ScavengerFlyer.projectileType = new ProjectileType()
      {
        Color = Color.YellowGreen,
        Damage = (byte) 5,
        Lifetime = 3000,
        Size = new Vector3(0.1f, 0.7f, 0.1f),
        Speed = 20f
      };
      ScavengerFlyer.projectileType.CreateProjectileStructure(ScavengerFlyer.projectileType.Size);
    }

    private new enum State
    {
      Attacking,
      Wandering_around,
    }
  }
}
