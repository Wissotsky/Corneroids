// Decompiled with JetBrains decompiler
// Type: CornerSpace.AICharacter
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class AICharacter : Character
  {
    private int damageInflicted;
    private bool inflictsDamageOnTouch;
    private IAICharacterManagement managerInterface;
    private Random random;
    protected static readonly Vector3[] directions = new Vector3[6]
    {
      Vector3.UnitX,
      -Vector3.UnitX,
      Vector3.UnitY,
      -Vector3.UnitY,
      Vector3.UnitZ,
      -Vector3.UnitZ
    };

    public AICharacter(string modelPath, string texturePath, float radius)
      : base(modelPath, texturePath, radius)
    {
      this.random = new Random();
    }

    public abstract void AICycle(SpaceEntityManager entityManager);

    public abstract bool CollisionCheckRequired();

    public virtual void CollidedWithEntity()
    {
    }

    public virtual void CollidedWithPlayer()
    {
    }

    public virtual void ShotBy(Projectile projectile)
    {
    }

    public abstract void SpawnAt(Position3 position, SpaceEntityManager entityManager);

    public abstract Position3? CheckPositionForSpawning(
      Position3 worldPosition,
      SpaceEntity closestEntity);

    public abstract int AIUpdateCycle { get; }

    public int InflictedDamage
    {
      get => this.damageInflicted;
      protected set => this.damageInflicted = value;
    }

    public bool InflictsDamageOnTouch
    {
      get => this.inflictsDamageOnTouch;
      protected set => this.inflictsDamageOnTouch = value;
    }

    public IAICharacterManagement ManagerInterface
    {
      set => this.managerInterface = value;
      protected get => this.managerInterface;
    }

    public abstract float MaxUpkeepDistance { get; }

    protected Random Random => this.random;
  }
}
