// Decompiled with JetBrains decompiler
// Type: CornerSpace.DynamicBlockVertex
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public struct DynamicBlockVertex : IVertexType
  {
    private Vector3 position;
    private Vector2 uv;
    private Vector3 normal;
    private Byte4 data;
    public static readonly int SizeInBytes = 36;
    public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
      new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
      new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
      new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
      new VertexElement(32, VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 1)
    );

    public DynamicBlockVertex(Vector3 pos, Vector2 uv, byte index)
    {
      this.position = pos;
      this.uv = uv;
      this.data = new Byte4(index, 0, 0, 0);
      this.normal = Vector3.Up;
    }

    public Vector3 Position
    {
      get => this.position;
      set => this.position = value;
    }

    public Vector2 UV
    {
      get => this.uv;
      set => this.uv = value;
    }

    public byte Index
    {
      get => this.data.X;
      set => this.data.X = value;
    }

    public Vector3 Normal
    {
      get => this.normal;
      set => this.normal = value;
    }

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
  }
}
