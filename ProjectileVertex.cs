// Decompiled with JetBrains decompiler
// Type: CornerSpace.ProjectileVertex
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CornerSpace
{
  public struct ProjectileVertex : IVertexType
  {
    public Vector3 Position;
    public Color Color;
    public float Index;

    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
      new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
      new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
      new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0)
    );
    public static readonly byte SizeInBytes = 20;

    public ProjectileVertex(Vector3 position, Color color)
    {
      Position = position;
      Color = color;
      Index = 0.0f;
    }

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
  }
}
