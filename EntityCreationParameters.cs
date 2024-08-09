// Decompiled with JetBrains decompiler
// Type: CornerSpace.EntityCreationParameters
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class EntityCreationParameters
  {
    private BoundingBoxI positionBounds;
    private Vector3 preferredSize;
    private int seed;

    public virtual void RandomizeParameters(int seed)
    {
      this.preferredSize = Vector3.One * 32f;
      this.seed = seed;
    }

    public BoundingBoxI PositionBounds
    {
      get => this.positionBounds;
      set => this.positionBounds = value;
    }

    public Vector3 PreferredSize
    {
      get => this.preferredSize;
      set => this.preferredSize = value;
    }

    public int Seed
    {
      get => this.seed;
      set => this.seed = value;
    }
  }
}
