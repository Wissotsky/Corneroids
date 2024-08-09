// Decompiled with JetBrains decompiler
// Type: CornerSpace.Enemies.NeedleParasite
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.Enemies
{
  public class NeedleParasite : ParasiteCharacter
  {
    private const float cChargeForce = 7f;
    private const int cInflictedDamage = 5;
    private const int cLifetimeMS = 4000;
    private const float cMaxSpeed = 10f;
    private const int cTrailSpriteIntervalMS = 100;
    private readonly Color cTrailColor = Color.Purple;
    private int lifeTimeCounter;
    private int trailTimer;

    public NeedleParasite()
      : base("Models\\needle", "Textures\\needle", 0.2f)
    {
      this.InitializationMatrix = Matrix.CreateRotationX(4.712389f);
      this.MaximumSpeed = 10f;
      this.lifeTimeCounter = 0;
      this.Health = (short) 3;
      this.InflictsDamageOnTouch = true;
      this.InflictedDamage = 5;
    }

    public override void AICycle(SpaceEntityManager entityManager)
    {
    }

    public override Position3? CheckPositionForSpawning(
      Position3 worldPosition,
      SpaceEntity closestEntity)
    {
      return new Position3?();
    }

    public override bool CollisionCheckRequired() => true;

    public override void RenderThirdPerson(
      IRenderCamera camera,
      ILightingInterface lightingInterface)
    {
      base.RenderThirdPerson(camera, lightingInterface);
    }

    public override void SpawnAt(Position3 position, SpaceEntityManager entityManager)
    {
      this.Position = position;
    }

    public override void CollidedWithEntity()
    {
      this.ManagerInterface.RemoveCharacterSafe((AICharacter) this);
    }

    public override void CollidedWithPlayer()
    {
      this.ManagerInterface.RemoveCharacterSafe((AICharacter) this);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (this.Target != null)
      {
        Vector3 vector3 = this.Target.Position - this.Position;
        if (vector3 != Vector3.Zero)
        {
          vector3.Normalize();
          this.ApplyForce(Vector3.Zero, vector3 * 7f - this.Speed, deltaTime);
          this.LookAt(this.Speed);
        }
      }
      else
        this.ManagerInterface.RemoveCharacterSafe((AICharacter) this);
      this.trailTimer += (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.trailTimer >= 100)
      {
        int lifetime = 1000 + this.Random.Next(-200, 200);
        this.ManagerInterface.EnvironmentManager.AddThrusterFlame(this.Position, this.Speed * 0.5f + new Vector3((float) this.Random.NextDouble(), (float) this.Random.NextDouble(), (float) this.Random.NextDouble()), this.cTrailColor, lifetime, 0.1f);
      }
      this.trailTimer %= 100;
      this.lifeTimeCounter += (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.lifeTimeCounter <= 4000)
        return;
      this.ManagerInterface.RemoveCharacterSafe((AICharacter) this);
    }

    public override int AIUpdateCycle => 10000;

    public override int MaxHostileCountPerSpawnCycle => 0;
  }
}
