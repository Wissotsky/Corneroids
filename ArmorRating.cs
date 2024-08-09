// Decompiled with JetBrains decompiler
// Type: CornerSpace.ArmorRating
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public struct ArmorRating
  {
    private uint armorRating;
    private float reductionPercentage;

    public ArmorRating(uint armorRating)
    {
      this.armorRating = armorRating;
      this.reductionPercentage = 0.0f;
      this.EvaluateReductionPercentage();
    }

    public int ReducedValue(int originalValue)
    {
      return (int) ((1f - this.reductionPercentage) * (float) originalValue);
    }

    public uint Rating
    {
      get => this.armorRating;
      set
      {
        this.armorRating = value;
        this.EvaluateReductionPercentage();
      }
    }

    public float ReductionPercentage => this.reductionPercentage;

    public static ArmorRating operator +(ArmorRating a1, ArmorRating a2)
    {
      return new ArmorRating(a1.Rating + a2.Rating);
    }

    public static ArmorRating operator -(ArmorRating a1, ArmorRating a2)
    {
      return new ArmorRating(Math.Max(a1.armorRating - a2.armorRating, 0U));
    }

    private void EvaluateReductionPercentage()
    {
      double num1 = Math.Log((double) ((int) MathHelper.Clamp((float) this.armorRating, 0.0f, 60f) + 1), Math.E) / 6.0;
      double num2 = num1 < 0.0 ? 0.0 : num1;
      this.reductionPercentage = num2 > 1.0 ? 1f : (float) num2;
    }
  }
}
