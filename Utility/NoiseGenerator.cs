// Decompiled with JetBrains decompiler
// Type: CornerSpace.Utility.NoiseGenerator
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.Utility
{
  public class NoiseGenerator
  {
    private int baseSeed;
    public NoiseGenerator.MaskContainer Masks;

    public NoiseGenerator(int seed)
    {
      this.baseSeed = seed;
      this.Masks = new NoiseGenerator.MaskContainer();
    }

    public Texture2D CreateClouds(int width, int height, int octaves)
    {
      Texture2D clouds = (Texture2D) null;
      try
      {
        clouds = new Texture2D(Engine.GraphicsDevice, width, height);
        Color[] data = new Color[width * height];
        octaves = Math.Max(octaves, 0);
        float num1 = octaves == 1 ? 1f : 1f / (float) (2 << octaves - 1);
        for (int index1 = 0; index1 < height; ++index1)
        {
          for (int index2 = 0; index2 < width; ++index2)
          {
            float num2 = num1;
            float num3 = 0.5f;
            byte num4 = 0;
            for (int index3 = 0; index3 < octaves; ++index3)
            {
              num4 += (byte) ((double) this.PerlinNoise2D((float) index2 * num2, (float) index1 * num2) * (double) num3);
              num3 *= 0.5f;
              num2 *= 2f;
            }
            data[index1 * width + index2] = new Color(num4, num4, num4);
          }
        }
        clouds.SetData<Color>(data);
        return clouds;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a noise texture: " + ex.Message);
        clouds?.Dispose();
        return (Texture2D) null;
      }
    }

    public byte PerlinNoise2D(float x, float y)
    {
      int x1 = this.FastFloor(x);
      int y1 = this.FastFloor(y);
      float amount1 = x - (float) x1;
      float amount2 = y - (float) y1;
      byte x0_1 = (byte) this.PRNG(x1, y1);
      byte x1_1 = (byte) this.PRNG(x1 + 1, y1);
      byte x0_2 = (byte) this.PRNG(x1, y1 + 1);
      byte x1_2 = (byte) this.PRNG(x1 + 1, y1 + 1);
      return this.Interpolate(this.Interpolate(x0_1, x1_1, amount1), this.Interpolate(x0_2, x1_2, amount1), amount2);
    }

    public byte PerlinNoise3D(float x, float y, float z)
    {
      int x1 = this.FastFloor(x);
      int y1 = this.FastFloor(y);
      int z1 = this.FastFloor(z);
      float amount1 = x - (float) x1;
      float amount2 = y - (float) y1;
      float amount3 = z - (float) z1;
      byte x0_1 = (byte) this.PRNG(x1, y1, z1);
      byte x1_1 = (byte) this.PRNG(x1 + 1, y1, z1);
      byte x1_2 = (byte) this.PRNG(x1 + 1, y1, z1 + 1);
      byte x0_2 = (byte) this.PRNG(x1, y1, z1 + 1);
      byte x0_3 = (byte) this.PRNG(x1, y1 + 1, z1);
      byte x1_3 = (byte) this.PRNG(x1 + 1, y1 + 1, z1);
      byte x1_4 = (byte) this.PRNG(x1 + 1, y1 + 1, z1 + 1);
      byte x0_4 = (byte) this.PRNG(x1, y1 + 1, z1 + 1);
      return this.Interpolate(this.Interpolate(this.Interpolate(x0_1, x1_1, amount1), this.Interpolate(x0_3, x1_3, amount1), amount2), this.Interpolate(this.Interpolate(x0_2, x1_2, amount1), this.Interpolate(x0_4, x1_4, amount1), amount2), amount3);
    }

    public byte PerlinNoise3D(float x, float y, float z, int octaves)
    {
      octaves = Math.Max(octaves, 0);
      float num1 = octaves == 1 ? 1f : 1f / (float) (2 << octaves - 1);
      float num2 = 0.5f;
      byte num3 = 0;
      for (int index = 0; index < octaves; ++index)
      {
        num3 += (byte) ((double) this.PerlinNoise3D(x * num1, y * num1, z * num1) * (double) num2);
        num2 *= 0.5f;
        num1 *= 2f;
      }
      return num3;
    }

    public byte PerlinNoise3DWithMask(
      Vector3 pixel,
      ref Vector3 totalSize,
      int octaves,
      NoiseGenerator.Mask mask)
    {
      byte noiseValue = this.PerlinNoise3D(pixel.X, pixel.Y, pixel.Z, octaves);
      if (mask != null)
        noiseValue = mask(noiseValue, ref pixel, ref totalSize);
      return noiseValue;
    }

    public int PRNG(int n)
    {
      n = n << 13 ^ n;
      return n * (n * n * this.baseSeed + 789221) + 87461947 & int.MaxValue;
    }

    public int PRNG(int x, int y)
    {
      x = x << 13 ^ x;
      x = x * (x * x * this.baseSeed + 789221) + 87461947 & int.MaxValue;
      x += y;
      x = x << 13 ^ x;
      x = x * (x * x * this.baseSeed + 789221) + 87461947 & int.MaxValue;
      return x;
    }

    public int PRNG(int x, int y, int z)
    {
      x = x << 13 ^ x;
      x = x * (x * x * this.baseSeed + 789221) + 87461947 & int.MaxValue;
      x += y;
      x = x << 13 ^ x;
      x = x * (x * x * this.baseSeed + 789221) + 87461947 & int.MaxValue;
      x += z;
      x = x << 13 ^ x;
      x = x * (x * x * this.baseSeed + 789221) + 87461947 & int.MaxValue;
      return x;
    }

    public int PRNG(ref Vector3int position) => this.PRNG(position.X, position.Y, position.Z);

    private int FastFloor(float value) => (double) value < 0.0 ? (int) value - 1 : (int) value;

    private byte Interpolate(byte x0, byte x1, float amount)
    {
      float num = (float) ((1.0 - Math.Cos((double) (amount * 3.14159274f))) * 0.5);
      return (byte) MathHelper.Clamp((float) ((double) x0 * (1.0 - (double) num) + (double) x1 * (double) num), 0.0f, (float) byte.MaxValue);
    }

    public class MaskContainer
    {
      private float sphericalFalloffDistance;
      private bool sphericalGradient;

      public byte SphericalBurnMaskFunction(
        byte noiseValue,
        ref Vector3 pixelPosition,
        ref Vector3 regionSize)
      {
        Vector3 vector3 = 0.5f * regionSize;
        float result;
        Vector3.DistanceSquared(ref pixelPosition, ref vector3, out result);
        byte num = (byte) ((double) MathHelper.Clamp((float) (1.0 - (double) result / ((double) this.sphericalFalloffDistance * (double) this.sphericalFalloffDistance)), 0.0f, 1f) * (double) byte.MaxValue);
        return num == (byte) 0 ? (byte) 0 : (byte) Math.Max((int) byte.MaxValue - ((int) byte.MaxValue - (int) noiseValue << 8) / (int) num, 0);
      }

      public byte SphericalMaskFunction(
        byte noiseValue,
        ref Vector3 pixelPosition,
        ref Vector3 regionSize)
      {
        Vector3 vector3 = 0.5f * regionSize;
        float result;
        Vector3.DistanceSquared(ref pixelPosition, ref vector3, out result);
        float num1 = (float) (1.0 - (double) result / ((double) this.sphericalFalloffDistance * (double) this.sphericalFalloffDistance));
        float num2 = (double) num1 < 0.0 ? 0.0f : num1;
        if (this.sphericalGradient)
          noiseValue = (byte) ((double) noiseValue * (double) num2);
        return (double) result <= (double) this.sphericalFalloffDistance * (double) this.sphericalFalloffDistance ? noiseValue : (byte) 0;
      }

      public float SphericalFallofDistance
      {
        get => this.sphericalFalloffDistance;
        set => this.sphericalFalloffDistance = value;
      }

      public bool SphericalGradient
      {
        get => this.sphericalGradient;
        set => this.sphericalGradient = value;
      }
    }

    public delegate byte Mask(byte noiseValue, ref Vector3 pixelPosition, ref Vector3 imageSize);
  }
}
