// Decompiled with JetBrains decompiler
// Type: CornerSpace.DynamicBlockRenderManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class DynamicBlockRenderManager : BlockRenderManager<Block>
  {
    private const int maxBlocksPerDrawCall = 50;
    private const int maxDynamicBlockVertices = 6144;
    private const int maxDynamicBlockIndices = 9216;
    private static Matrix[] transformMatrices;
    private static DynamicBlockVertex[] vertexArray = new DynamicBlockVertex[6144];
    private static short[] indexArray = new short[9216];
    private List<DynamicBlockRenderManager.DynamicBlockContainer> blockList;

    public DynamicBlockRenderManager()
    {
      ContentManager contentManager = Engine.ContentManager;
      this.blockList = new List<DynamicBlockRenderManager.DynamicBlockContainer>();
      this.fullEffect = Engine.ShaderPool[1];
      if (DynamicBlockRenderManager.transformMatrices != null)
        return;
      DynamicBlockRenderManager.transformMatrices = new Matrix[50];
    }

    public bool AddBlock(Block block, Vector3 positionInShipSpace)
    {
      if (!(block is IDynamicBlock))
        return false;
      DynamicBlockRenderManager.DynamicBlockContainer dynamicBlockContainer = new DynamicBlockRenderManager.DynamicBlockContainer(block);
      DynamicBlockVertex[] vertices;
      short[] indices;
      BlockBuilder.Factory.ConstructBlock(block, positionInShipSpace, (byte) 0, out vertices, out indices);
      dynamicBlockContainer.Vertices = vertices;
      dynamicBlockContainer.Indices = indices;
      dynamicBlockContainer.PositionEntitySpace = positionInShipSpace;
      this.blockList.Add(dynamicBlockContainer);
      this.updateRequired = true;
      return true;
    }

    public void ReconstructPowerBlocks()
    {
      for (int index = 0; index < this.blockList.Count; ++index)
      {
        if (this.blockList[index].Block is PowerBlock block1)
        {
          DynamicBlockRenderManager.DynamicBlockContainer block = this.blockList[index];
          DynamicBlockVertex[] vertices = block.Vertices;
          short[] indices = block.Indices;
          BlockBuilder.Factory.ConstructBlock((Block) block1, block.PositionEntitySpace, (byte) 0, out vertices, out indices);
          block.Vertices = vertices;
          block.Indices = indices;
          this.updateRequired = true;
        }
      }
    }

    public bool RemoveBlock(Block block)
    {
      if (!(block is IDynamicBlock))
        return false;
      try
      {
        for (int index = 0; index < this.blockList.Count; ++index)
        {
          if (this.blockList[index].Block == block)
          {
            this.blockList.RemoveAt(index);
            break;
          }
        }
        this.updateRequired = true;
        return true;
      }
      catch
      {
        return false;
      }
    }

    public override void Render(IRenderCamera camera, byte quality)
    {
      if (this.updateRequired)
      {
      int vertexCount = this.TotalNumberOfVertices();
      int indexCount = vertexCount * 6 / 4;
      if (this.UpdateBuffers(vertexCount, indexCount, true))
      {
        this.SetDataToBuffers();
        this.updateRequired = false;
      }
      }
      if (this.numberOfVertices <= 0 || this.vertexBuffer == null || this.indexBuffer == null)
      return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      graphicsDevice.SetVertexBuffer(this.vertexBuffer);
      graphicsDevice.Indices = this.indexBuffer;
      this.fullEffect.Parameters["World"].SetValue(this.sectorMatrix);
      this.fullEffect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.fullEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      for (int index1 = 0; index1 < this.blockList.Count / 50 + 1; ++index1)
      {
      int num1 = index1 * 50;
      int num2 = Math.Min(num1 + 50, this.blockList.Count);
      if (num2 > num1)
      {
        int num3 = 0;
        for (int index2 = num1; index2 < num2; ++index2)
        DynamicBlockRenderManager.transformMatrices[num3++] = this.blockList[index2].DynamicBlock.TransformMatrix;
        int startIndex = 36 * index1 * 50;
        int primitiveCount = (num2 - num1) * 36 / 3;
        this.fullEffect.Parameters["TransformMatrices"].SetValue(DynamicBlockRenderManager.transformMatrices);
        this.fullEffect.CurrentTechnique.Passes[0].Apply();
        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.numberOfVertices, startIndex, primitiveCount);
      }
      }
    }

    protected override bool CreateBuffers(
      ref VertexBufferObj vb,
      ref IndexBufferObj ib,
      int vbSize,
      int ibSize)
    {
      try
      {
        vb = new VertexBufferObj(Engine.GraphicsDevice, Engine.VertexDeclarationPool[4], vbSize, BufferUsage.WriteOnly);
        ib = new IndexBufferObj(Engine.GraphicsDevice, ibSize, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      }
      catch (Exception ex)
      {
        if (vb != null)
          vb.Dispose();
        if (ib != null)
          ib.Dispose();
        Engine.Console.WriteErrorLine("Failed to create vertexbuffer/indexbuffer for dynamic blocks: " + ex.Message);
        return false;
      }
      return true;
    }

    protected void SetDataToBuffers()
    {
      this.numberOfVertices = 0;
      this.numberOfIndices = 0;
      byte num = 0;
      foreach (DynamicBlockRenderManager.DynamicBlockContainer block in this.blockList)
      {
        if (DynamicBlockRenderManager.vertexArray.Length >= this.numberOfVertices + block.Vertices.Length)
        {
          if (DynamicBlockRenderManager.indexArray.Length >= this.numberOfIndices + block.Indices.Length)
          {
            for (int index = 0; index < block.Vertices.Length; ++index)
            {
              DynamicBlockRenderManager.vertexArray[this.numberOfVertices + index] = block.Vertices[index];
              DynamicBlockRenderManager.vertexArray[this.numberOfVertices + index].Index = (byte) ((uint) num % 50U);
            }
            for (int index = 0; index < block.Indices.Length; ++index)
              DynamicBlockRenderManager.indexArray[this.numberOfIndices + index] = (short) ((int) block.Indices[index] + this.numberOfVertices);
            this.numberOfVertices += block.Vertices.Length;
            this.numberOfIndices += block.Indices.Length;
            ++num;
          }
          else
            break;
        }
        else
          break;
      }
      if (!this.BlockDataFitsBuffers(this.numberOfVertices, this.numberOfIndices) || this.numberOfVertices <= 0)
        return;
      if (this.numberOfIndices <= 0)
        return;
      try
      {
        this.vertexBuffer.SetData<DynamicBlockVertex>(DynamicBlockRenderManager.vertexArray, 0, this.numberOfVertices);
        this.indexBuffer.SetData<short>(DynamicBlockRenderManager.indexArray, 0, this.numberOfIndices);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to set data to buffers for dynamic blocks: " + ex.Message);
      }
    }

    private int TotalNumberOfVertices() => this.blockList.Count * 24;

    public class DynamicBlockContainer
    {
      public Block Block;
      public IDynamicBlock DynamicBlock;
      public Vector3 PositionEntitySpace;
      public DynamicBlockVertex[] Vertices;
      public short[] Indices;

      public DynamicBlockContainer(Block block)
      {
        this.Block = block;
        this.DynamicBlock = block as IDynamicBlock;
      }
    }
  }
}
