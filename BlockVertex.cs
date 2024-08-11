// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockVertex
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public struct BlockVertex
  {
    public Byte4 byte1;
    public Byte4 byte2;

    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
      new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.Position, 0),
      new VertexElement(4, VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 0)
    );
    public static readonly byte SizeInBytes = 8;

    public BlockVertex(byte x, byte y, byte z, byte uv)
    {
      this.byte1 = new Byte4();
      this.byte2 = new Byte4();
      this.SetPosition(x, y, z);
      this.byte2.Y = uv;
    }

    public void ScalePosition(Byte3 scale)
    {
      this.byte1.X *= scale.X;
      this.byte1.Y *= scale.Y;
      this.byte1.Z *= scale.Z;
    }

    public void SetIndex(byte index) => this.byte2.X = index;

    public void SetNormal(Vector3 normal)
    {
      if (normal == Vector3.Left)
        this.byte1.W = (byte) 0;
      else if (normal == Vector3.Right)
        this.byte1.W = (byte) 1;
      else if (normal == Vector3.Down)
        this.byte1.W = (byte) 2;
      else if (normal == Vector3.Up)
        this.byte1.W = (byte) 3;
      else if (normal == Vector3.Forward)
        this.byte1.W = (byte) 4;
      else if (normal == Vector3.Backward)
        this.byte1.W = (byte) 5;
      else
        this.byte1.W = (byte) 0;
    }

    public void SetPosition(byte x, byte y, byte z)
    {
      this.byte1.X = x;
      this.byte1.Y = y;
      this.byte1.Z = z;
    }

    public void SetTextureCoordinates(byte u, byte v)
    {
      this.byte2.Y = u;
      this.byte2.Z = v;
    }

    public Byte3 Position
    {
      get => new Byte3(this.byte1.X, this.byte1.Y, this.byte1.Z);
      set
      {
        this.byte1.X = value.X;
        this.byte1.Y = value.Y;
        this.byte1.Z = value.Z;
      }
    }
  }
}
