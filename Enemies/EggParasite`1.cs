// Decompiled with JetBrains decompiler
// Type: CornerSpace.Enemies.EggParasite`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Enemies
{
  public abstract class EggParasite<T> : ParasiteCharacter where T : PhysicalObject
  {
    protected Vector3 eggNormal;
    protected List<T> hatchesTo;

    public EggParasite(string modelPath, string texturePath, float radius, T[] hatchesTo)
      : base(modelPath, texturePath, radius)
    {
      this.hatchesTo = new List<T>();
      if (hatchesTo == null)
        return;
      this.hatchesTo.AddRange((IEnumerable<T>) hatchesTo);
    }

    public override Position3? CheckPositionForSpawning(
      Position3 worldPosition,
      SpaceEntity closestEntity)
    {
      ParasiteCharacter.SpawningPosition? nullable = this.CheckSpawningPosition(worldPosition, closestEntity, typeof (AsteroidBlock), typeof (MineralBlock));
      if (!nullable.HasValue)
        return new Position3?();
      this.eggNormal = Vector3.Normalize(nullable.Value.SpawnPosition - nullable.Value.FloorBlockPosition);
      return new Position3?(nullable.Value.SpawnPosition);
    }

    public override void Dispose()
    {
      base.Dispose();
      foreach (T obj in this.hatchesTo)
        obj.Dispose();
    }
  }
}
