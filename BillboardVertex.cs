// Decompiled with JetBrains decompiler
// Type: CornerSpace.BillboardVertex
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CornerSpace
{
  public struct BillboardVertex : IVertexType
  {
    public Vector3 Position;
    public Vector2 TextureCoordinates;
    public Color Color;
    public float Index;
    public Vector2 Size;

    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
      new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
      new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
      new VertexElement(sizeof(float) * 5, VertexElementFormat.Color, VertexElementUsage.Color, 0),
      new VertexElement(sizeof(float) * 6, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
      new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2)
    );
    public static readonly byte SizeInBytes = 36;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
  }
}
