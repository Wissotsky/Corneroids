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
  public struct DynamicBlockVertex
  {
    private Vector3 position;
    private Vector2 uv;
    private Vector3 normal;
    private Byte4 data;
    public static readonly byte SizeInBytes = 36;
    public static readonly VertexElement[] VertexElements = new VertexElement[4]
    {
      new VertexElement((short) 0, (short) 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, (byte) 0),
      new VertexElement((short) 0, (short) 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 0),
      new VertexElement((short) 0, (short) 20, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, (byte) 0),
      new VertexElement((short) 0, (short) 32, VertexElementFormat.Byte4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, (byte) 1)
    };

    public DynamicBlockVertex(Vector3 pos, Vector2 uv, byte index)
    {
      this.position = pos;
      this.uv = uv;
      this.data = new Byte4(index, (byte) 0, (byte) 0, (byte) 0);
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
  }
}
