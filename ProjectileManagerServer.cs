// Decompiled with JetBrains decompiler
// Type: CornerSpace.ProjectileManagerServer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  internal class ProjectileManagerServer : ProjectileManagerSP
  {
    private NetworkServerManager server;

    public ProjectileManagerServer(
      SpaceEntityManager entityManager,
      EnvironmentManager environmentManager,
      LightingManager lightingManager)
      : base(entityManager, environmentManager, lightingManager)
    {
    }

    public override void BindToServer(NetworkServerManager server) => this.server = server;
  }
}
