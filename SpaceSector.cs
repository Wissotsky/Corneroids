// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceSector
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class SpaceSector
  {
    private List<SpaceEntity> entities;
    private BoundingBox sectorBounds;
    private Vector3int sectorPosition;
    private int seed;

    public SpaceSector(Vector3int sectorPosition, int sectorSize)
    {
      this.sectorBounds = new BoundingBox(new Vector3((float) sectorPosition.X, (float) sectorPosition.Y, (float) sectorPosition.Z) * (float) sectorSize, new Vector3((float) (sectorPosition.X + 1), (float) (sectorPosition.Y + 1), (float) (sectorPosition.Z + 1)) * (float) sectorSize);
      this.sectorPosition = sectorPosition;
      this.entities = new List<SpaceEntity>();
    }

    public void AddDummyEntity(SpaceEntity entity) => throw new NotImplementedException();

    public float DistanceTo(Vector3 point)
    {
      if (this.sectorBounds.Contains(point) == ContainmentType.Contains)
        return 0.0f;
      Vector3 min = this.sectorBounds.Min;
      Vector3 max = this.sectorBounds.Max;
      return (new Vector3()
      {
        X = Math.Min(Math.Max(point.X, min.X), max.X),
        Y = Math.Min(Math.Max(point.Y, min.Y), max.Y),
        Z = Math.Min(Math.Max(point.Z, min.Z), max.Z)
      } - point).Length();
    }

    public List<SpaceEntity> Entities => this.entities;

    public Vector3int SectorPosition => this.sectorPosition;

    public BoundingBox SectorBounds => this.sectorBounds;

    public int Seed => this.seed;
  }
}
