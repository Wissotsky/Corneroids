// Decompiled with JetBrains decompiler
// Type: CornerSpace.ProjectileVertex
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public struct ProjectileVertex
  {
    public Vector3 Position;
    public Color Color;
    public float Index;
    public static readonly VertexElement[] VertexElements = new VertexElement[3]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, (byte) 0),
      new VertexElement((short) 0, (short) 16, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0)
    };
    public static readonly byte SizeInBytes = 20;

    public ProjectileVertex(Vector3 position, Color color)
    {
      this.Position = position;
      this.Color = color;
      this.Index = 0.0f;
    }
  }
}
