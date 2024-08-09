// Decompiled with JetBrains decompiler
// Type: CornerSpace.AsteroidBeltCreationParameters
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class AsteroidBeltCreationParameters : AsteroidCreationParameters
  {
    private float innerRadius = 32f;
    private float outerRadius = 128f;
    private int numberOfAsteroids = 64;

    public override void RandomizeParameters(int seed)
    {
    }

    public float InnerRadius
    {
      get => this.innerRadius;
      set => this.innerRadius = value;
    }

    public float OuterRadius
    {
      get => this.outerRadius;
      set => this.outerRadius = value;
    }

    public int NumberOfAsteroids
    {
      get => this.numberOfAsteroids;
      set => this.numberOfAsteroids = value;
    }
  }
}
