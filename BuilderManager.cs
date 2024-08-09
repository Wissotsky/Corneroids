// Decompiled with JetBrains decompiler
// Type: CornerSpace.BuilderManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public abstract class BuilderManager
  {
    private SpaceEntityManager entityManager;

    public BuilderManager(SpaceEntityManager entityManager) => this.entityManager = entityManager;

    public virtual void AddBlock(Block block, Vector3 position)
    {
    }

    public virtual void RemoveBlock(Vector3 position)
    {
    }

    protected SpaceEntityManager EntityManager => this.entityManager;
  }
}
