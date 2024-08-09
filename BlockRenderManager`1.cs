// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockRenderManager`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class BlockRenderManager<T> : IDisposable where T : Block
  {
    private const int maxIndexCount = 147456;
    private const int maxVertexCount = 98304;
    private const int vertexbufferVertexOverhead = 2400;
    private const int indexbufferIndexOverhead = 3600;
    private bool isDisposed;
    protected int numberOfIndices;
    protected int numberOfVertices;
    protected Matrix sectorMatrix;
    protected Effect fullEffect;
    protected Effect reducedEffect;
    protected Vector3 sectorPosition;
    protected bool updateRequired;
    protected IndexBufferObj indexBuffer;
    protected VertexBufferObj vertexBuffer;
    protected BlockOctTree vertexStructureTree;
    private static short[] indexArray = new short[147456];
    private static BlockVertex[] vertexArray = new BlockVertex[98304];

    public BlockRenderManager()
    {
      this.isDisposed = false;
      this.sectorMatrix = Matrix.Identity;
      this.updateRequired = false;
      this.numberOfVertices = 0;
    }

    public virtual void Dispose()
    {
      BlockSector.GraphicBufferManager.ReleaseBlockVertexBuffer(this.vertexBuffer);
      BlockSector.GraphicBufferManager.ReleaseBlockIndexBuffer(this.indexBuffer);
      this.vertexBuffer = (VertexBufferObj) null;
      this.indexBuffer = (IndexBufferObj) null;
      this.isDisposed = true;
    }

    public virtual void ForceUpdate() => this.updateRequired = true;

    public virtual void Render(IRenderCamera camera, byte quality)
    {
      if (this.updateRequired)
      {
        BlockSector.GraphicBufferManager.GetPermissionToCreateNewBuffers((System.Action) (() =>
        {
          if (this.isDisposed)
            return;
          this.numberOfVertices = 0;
          this.numberOfIndices = 0;
          this.vertexStructureTree.GetReducedVerticesAndIndices(BlockRenderManager<T>.vertexArray, ref this.numberOfVertices, BlockRenderManager<T>.indexArray, ref this.numberOfIndices, quality);
          if (!this.UpdateBuffers(this.numberOfVertices, this.numberOfIndices, quality == (byte) 0) || this.numberOfVertices <= 0)
            return;
          if (this.numberOfIndices <= 0)
            return;
          try
          {
            this.vertexBuffer.SetData<BlockVertex>(BlockRenderManager<T>.vertexArray, 0, this.numberOfVertices);
            this.indexBuffer.SetData<short>(BlockRenderManager<T>.indexArray, 0, this.numberOfIndices);
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Failed to set data to buffers: " + ex.Message);
          }
        }), (object) this);
        this.updateRequired = false;
      }
      if (this.numberOfVertices <= 0 || this.vertexBuffer == null || this.indexBuffer == null)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      Effect effect = (quality == (byte) 0 ? this.fullEffect : this.reducedEffect) ?? this.fullEffect;
      graphicsDevice.VertexDeclaration = this.vertexBuffer.Declaration;
      graphicsDevice.Vertices[0].SetSource((VertexBuffer) this.vertexBuffer, 0, (int) BlockVertex.SizeInBytes);
      graphicsDevice.Indices = (IndexBuffer) this.indexBuffer;
      Matrix matrix = this.sectorMatrix * camera.ViewMatrix * camera.ProjectionMatrix;
      effect.Parameters["World"].SetValue(this.sectorMatrix);
      effect.Parameters["WVP"].SetValue(matrix);
      effect.Begin();
      effect.CurrentTechnique.Passes[0].Begin();
      graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.numberOfVertices, 0, this.numberOfIndices / 3);
      effect.CurrentTechnique.Passes[0].End();
      effect.End();
    }

    public void SetVertexStructureTree(BlockOctTree rootNode)
    {
      this.vertexStructureTree = rootNode;
    }

    public bool HasGeometry => this.numberOfVertices > 0 && this.numberOfIndices > 0;

    public Matrix SectorMatrix
    {
      get => this.sectorMatrix;
      set => this.sectorMatrix = value;
    }

    public Vector3 SectorPosition
    {
      set => this.sectorPosition = value;
    }

    protected bool BlockDataFitsBuffers(int vertexCount, int indexCount)
    {
      return this.vertexBuffer != null && this.indexBuffer != null && vertexCount <= this.vertexBuffer.VertexCount && indexCount <= this.indexBuffer.IndexCount;
    }

    protected void EvaluateBufferSizes(
      int vertexCount,
      int indexCount,
      out int vbSize,
      out int ibSize)
    {
      vbSize = vertexCount + 1200;
      ibSize = indexCount + 1800;
    }

    protected virtual bool CreateBuffers(
      ref VertexBufferObj vb,
      ref IndexBufferObj ib,
      int vbSize,
      int ibSize)
    {
      vb = BlockSector.GraphicBufferManager.CreateBlockVertexBuffer(vbSize);
      ib = BlockSector.GraphicBufferManager.CreateBlockIndexBuffer(ibSize);
      return vb != null && ib != null;
    }

    protected bool UpdateBuffers(int vertexCount, int indexCount, bool fullQuality)
    {
      if (vertexCount == 0 || indexCount == 0)
      {
        BlockSector.GraphicBufferManager.ReleaseBlockVertexBuffer(this.vertexBuffer);
        BlockSector.GraphicBufferManager.ReleaseBlockIndexBuffer(this.indexBuffer);
        this.vertexBuffer = (VertexBufferObj) null;
        this.indexBuffer = (IndexBufferObj) null;
        return false;
      }
      if (this.BlockDataFitsBuffers(vertexCount, indexCount))
        return true;
      BlockSector.GraphicBufferManager.ReleaseBlockVertexBuffer(this.vertexBuffer);
      BlockSector.GraphicBufferManager.ReleaseBlockIndexBuffer(this.indexBuffer);
      int vbSize;
      int ibSize;
      this.EvaluateBufferSizes(vertexCount, indexCount, out vbSize, out ibSize);
      return this.CreateBuffers(ref this.vertexBuffer, ref this.indexBuffer, vbSize, ibSize);
    }
  }
}
