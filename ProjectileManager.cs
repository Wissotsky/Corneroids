// Decompiled with JetBrains decompiler
// Type: CornerSpace.ProjectileManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;

#nullable disable
namespace CornerSpace
{
  public abstract class ProjectileManager : IDisposable
  {
    protected CharacterManager characterManager;
    protected SpaceEntityManager entityManager;
    protected EnvironmentManager environmentManager;
    protected LightingManager lightingManager;

    public ProjectileManager(
      SpaceEntityManager entityManager,
      EnvironmentManager environmentManager,
      LightingManager lightingManager)
    {
      this.entityManager = entityManager != null && environmentManager != null ? entityManager : throw new ArgumentNullException();
      this.environmentManager = environmentManager;
      this.lightingManager = lightingManager;
    }

    public abstract bool AddProjectile(Projectile projectile);

    public virtual void BindToClient(NetworkClientManager client)
    {
      throw new InvalidOperationException("Client manager cannot be bound to this projectile manager!");
    }

    public virtual void BindToServer(NetworkServerManager server)
    {
      throw new InvalidOperationException("Server manager cannot be bound to this projectile manager!");
    }

    public abstract void Dispose();

    public abstract void Render(IRenderCamera camera);

    public abstract void Update();

    public CharacterManager CharacterManager
    {
      set => this.characterManager = value;
    }
  }
}
