// Decompiled with JetBrains decompiler
// Type: CornerSpace.BillboardVertex
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public struct BillboardVertex
  {
    public Vector3 Position;
    public Vector2 TextureCoordinates;
    public Color Color;
    public float Index;
    public Vector2 Size;
    public static readonly VertexElement[] VertexElements = new VertexElement[5]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 20, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 0),
      new VertexElement((short) 0, (short) 24, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1),
      new VertexElement((short) 0, (short) 28, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 2)
    };
    public static readonly byte SizeInBytes = 36;
  }
}
