// Decompiled with JetBrains decompiler
// Type: CornerSpace.ProjectileManagerClient
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;

#nullable disable
namespace CornerSpace
{
  public class ProjectileManagerClient : ProjectileManagerSP
  {
    private NetworkClientManager client;

    public ProjectileManagerClient(
      SpaceEntityManager entityManager,
      EnvironmentManager environmentManager,
      LightingManager lightingManager)
      : base(entityManager, environmentManager, lightingManager)
    {
    }

    public override void BindToClient(NetworkClientManager client) => this.client = client;

    protected override void CollisionResponseWithCharacter(
      Character character,
      Projectile projectile,
      Position3 position)
    {
      int num = (int) Math.Min(character.Health, (short) projectile.DamageLeft);
      projectile.DamageLeft -= (byte) num;
      this.environmentManager.AddBlood(position);
      if (projectile.DamageLeft > (byte) 0)
        return;
      this.environmentManager.AddExplosion(position, projectile.ProjectileType);
      projectile.Lifetime = 0;
    }
  }
}
