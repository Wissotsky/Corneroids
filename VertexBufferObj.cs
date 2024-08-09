// Decompiled with JetBrains decompiler
// Type: CornerSpace.VertexBufferObj
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public class VertexBufferObj : VertexBuffer
  {
    private VertexDeclaration declaration;
    private int vertexCount;

    public VertexBufferObj(
      GraphicsDevice device,
      int vertexCount,
      int vertexStride,
      VertexDeclaration declaration,
      BufferUsage usage)
      : base(device, vertexCount * vertexStride, usage)
    {
      this.declaration = declaration;
      this.vertexCount = vertexCount;
    }

    public VertexDeclaration Declaration => this.declaration;

    public int VertexCount => this.vertexCount;
  }
}
