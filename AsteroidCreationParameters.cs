// Decompiled with JetBrains decompiler
// Type: CornerSpace.AsteroidCreationParameters
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class AsteroidCreationParameters : EntityCreationParameters
  {
    private byte smoothness = 5;
    private byte rarity;

    public override void RandomizeParameters(int seed)
    {
      Random random = new Random(seed);
      this.PreferredSize = Vector3.One * (float) random.Next(32, 96);
      this.smoothness = (byte) random.Next(4, 7);
      this.rarity = (byte) 0;
      seed = random.Next();
    }

    public byte Smoothness
    {
      get => this.smoothness;
      set => this.smoothness = value;
    }

    public byte Rarity
    {
      get => this.rarity;
      set => this.rarity = value;
    }
  }
}
