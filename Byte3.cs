// Decompiled with JetBrains decompiler
// Type: CornerSpace.Byte3
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public struct Byte3
  {
    public byte X;
    public byte Y;
    public byte Z;

    public Byte3(byte x, byte y, byte z)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public static Byte3 operator +(Byte3 b1, Byte3 b2)
    {
      return new Byte3((byte) ((uint) b1.X + (uint) b2.X), (byte) ((uint) b1.Y + (uint) b2.Y), (byte) ((uint) b1.Z + (uint) b2.Z));
    }

    public static Byte3 operator -(Byte3 b1, Byte3 b2)
    {
      return new Byte3((byte) ((uint) b1.X - (uint) b2.X), (byte) ((uint) b1.Y - (uint) b2.Y), (byte) ((uint) b1.Z - (uint) b2.Z));
    }

    public static Byte3 operator *(Byte3 b1, Byte3 b2)
    {
      return new Byte3((byte) ((uint) b1.X * (uint) b2.X), (byte) ((uint) b1.Y * (uint) b2.Y), (byte) ((uint) b1.Z * (uint) b2.Z));
    }

    public static Byte3 operator *(int i, Byte3 b)
    {
      return new Byte3((byte) ((uint) b.X * (uint) i), (byte) ((uint) b.Y * (uint) i), (byte) ((uint) b.Z * (uint) i));
    }

    public static explicit operator Vector3(Byte3 b)
    {
      return new Vector3((float) b.X, (float) b.Y, (float) b.Z);
    }

    public static explicit operator Byte3(Vector3 v)
    {
      return new Byte3((byte) v.X, (byte) v.Y, (byte) v.Z);
    }
  }
}
