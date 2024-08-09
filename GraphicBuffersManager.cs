// Decompiled with JetBrains decompiler
// Type: CornerSpace.GraphicBuffersManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class GraphicBuffersManager
  {
    private const uint cMaxBufferUpdatesPerFrame = 1;
    private const uint cMaxPoolSize = 500;
    private const int cMinBufferSize = 10000;
    private Queue<KeyValuePair<System.Action, object>> creationQueue;
    private List<IndexBufferObj> indexBufferPool;
    private List<VertexBufferObj> vertexBufferPool;
    private long numberOfCreatedVertexBuffers;
    private long numberOfReusedVertexBuffers;

    public GraphicBuffersManager()
    {
      this.numberOfCreatedVertexBuffers = 0L;
      this.numberOfReusedVertexBuffers = 0L;
      this.creationQueue = new Queue<KeyValuePair<System.Action, object>>();
      this.indexBufferPool = new List<IndexBufferObj>();
      this.vertexBufferPool = new List<VertexBufferObj>();
    }

    public IndexBufferObj CreateBlockIndexBuffer(int indexCount)
    {
      IndexBufferObj blockIndexBuffer = this.indexBufferPool.Find((Predicate<IndexBufferObj>) (b => b.IndexCount >= indexCount));
      try
      {
        if (blockIndexBuffer == null)
          blockIndexBuffer = new IndexBufferObj(Engine.GraphicsDevice, Math.Max(indexCount, 10000), BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
        else
          this.indexBufferPool.Remove(blockIndexBuffer);
        return blockIndexBuffer;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create an indexbuffer for blocks: " + ex.Message);
        return (IndexBufferObj) null;
      }
    }

    public VertexBufferObj CreateBlockVertexBuffer(int vertexCount)
    {
      VertexBufferObj blockVertexBuffer = this.vertexBufferPool.Find((Predicate<VertexBufferObj>) (b => b.VertexCount >= vertexCount));
      try
      {
        if (blockVertexBuffer == null)
        {
          blockVertexBuffer = new VertexBufferObj(Engine.GraphicsDevice, Math.Max(vertexCount, 10000), (int) BlockVertex.SizeInBytes, Engine.VertexDeclarationPool[0], BufferUsage.WriteOnly);
          ++this.numberOfCreatedVertexBuffers;
        }
        else
        {
          this.vertexBufferPool.Remove(blockVertexBuffer);
          ++this.numberOfReusedVertexBuffers;
        }
        return blockVertexBuffer;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a vertexbuffer for blocks: " + ex.Message);
        return (VertexBufferObj) null;
      }
    }

    public void GetPermissionToCreateNewBuffers(System.Action result, object sender)
    {
      if (this.SenderExists(sender))
        return;
      this.creationQueue.Enqueue(new KeyValuePair<System.Action, object>(result, sender));
    }

    public void ReleaseBlockIndexBuffer(IndexBufferObj buffer)
    {
      if (buffer == null)
        return;
      this.indexBufferPool.Add(buffer);
      SimpleMath.InsertionSort<IndexBufferObj>(this.indexBufferPool, (Comparison<IndexBufferObj>) ((i1, i2) => i1.IndexCount - i2.IndexCount));
      if (this.indexBufferPool.Count <= 500)
        return;
      this.indexBufferPool[0].Dispose();
      this.indexBufferPool.RemoveAt(0);
    }

    public void ReleaseBlockVertexBuffer(VertexBufferObj buffer)
    {
      if (buffer == null)
        return;
      this.vertexBufferPool.Add(buffer);
      SimpleMath.InsertionSort<VertexBufferObj>(this.vertexBufferPool, (Comparison<VertexBufferObj>) ((v1, v2) => v1.VertexCount - v2.VertexCount));
      if (this.vertexBufferPool.Count <= 500)
        return;
      this.vertexBufferPool[0].Dispose();
      this.vertexBufferPool.RemoveAt(0);
    }

    public void Update()
    {
      for (int index = 0; index < 1 && this.creationQueue.Count > 0; ++index)
        this.creationQueue.Dequeue().Key();
    }

    public int IndexBufferCount => this.indexBufferPool.Count;

    public long NumberOfCreatedVertexBuffers => this.numberOfCreatedVertexBuffers;

    public long NumberOfReusedVertexBuffers => this.numberOfReusedVertexBuffers;

    public int VertexBufferCount => this.vertexBufferPool.Count;

    private bool SenderExists(object sender)
    {
      foreach (KeyValuePair<System.Action, object> creation in this.creationQueue)
      {
        if (creation.Value == sender)
          return true;
      }
      return false;
    }
  }
}
