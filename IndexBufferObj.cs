// Decompiled with JetBrains decompiler
// Type: CornerSpace.IndexBufferObj
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public class IndexBufferObj : IndexBuffer
  {
    private int indexCount;

    public IndexBufferObj(
      GraphicsDevice device,
      int indexCount,
      BufferUsage usage,
      IndexElementSize elementSize)
      : base(device, indexCount * (elementSize == IndexElementSize.SixteenBits ? 2 : 4), usage, elementSize)
    {
      this.indexCount = indexCount;
    }

    public int IndexCount => this.indexCount;
  }
}
