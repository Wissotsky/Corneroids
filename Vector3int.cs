// Decompiled with JetBrains decompiler
// Type: CornerSpace.Vector3int
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public struct Vector3int
  {
    public int X;
    public int Y;
    public int Z;

    public Vector3int(int x, int y, int z)
    {
      this.X = x;
      this.Y = y;
      this.Z = z;
    }

    public Vector3int(Vector3 vector)
    {
      this.X = (int) Math.Floor((double) vector.X);
      this.Y = (int) Math.Floor((double) vector.Y);
      this.Z = (int) Math.Floor((double) vector.Z);
    }

    public int DistanceSquared(Vector3int v)
    {
      Vector3int vector3int = this - v;
      return vector3int.X * vector3int.X + vector3int.Y * vector3int.Y + vector3int.Z * vector3int.Z;
    }

    public static Vector3int operator +(Vector3int c1, Vector3int c2)
    {
      return new Vector3int(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
    }

    public static Vector3int operator -(Vector3int c1, Vector3int c2)
    {
      return new Vector3int(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
    }

    public static Vector3 operator *(float v, Vector3int c)
    {
      return new Vector3((float) c.X * v, (float) c.Y * v, (float) c.Z * v);
    }

    public static Vector3 operator *(Vector3int c, float v) => v * c;

    public static Vector3int operator *(int v, Vector3int c)
    {
      return new Vector3int(c.X * v, c.Y * v, c.Z * v);
    }

    public static Vector3int operator *(Vector3int c, int v) => v * c;

    public static explicit operator Vector3int(Vector3 v)
    {
      return new Vector3int((int) Math.Floor((double) v.X), (int) Math.Floor((double) v.Y), (int) Math.Floor((double) v.Z));
    }

    public static explicit operator Vector3(Vector3int v)
    {
      return new Vector3((float) v.X, (float) v.Y, (float) v.Z);
    }

    public static Vector3int Zero => new Vector3int(0, 0, 0);
  }
}
