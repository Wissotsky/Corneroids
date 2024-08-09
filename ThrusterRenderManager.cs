// Decompiled with JetBrains decompiler
// Type: CornerSpace.ThrusterRenderManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class ThrusterRenderManager : IDisposable
  {
    private const int cTimeBetweenFlamesMS = 33;
    private const int cMaxFlamesPerSector = 50;
    private Effect effect;
    private List<EngineBlock> engineBlocks;
    private SpaceEntity ownerEntity;
    private int timeSinceLastFlameMS;
    private Random random;
    private VertexBufferObj vertexBuffer;
    private IndexBufferObj indexBuffer;
    private VertexDeclaration vertexDeclaration;
    private static VertexPositionColor[] flameVertices;
    private static short[] flameIndices;
    private static VertexPositionColor[] renderVertices;
    private static short[] renderIndices;
    private static EnvironmentManager environmentManager;

    public ThrusterRenderManager(SpaceEntity owner)
    {
      this.engineBlocks = new List<EngineBlock>();
      this.effect = Engine.ShaderPool[7];
      this.vertexDeclaration = Engine.VertexDeclarationPool[1];
      this.random = new Random();
      this.timeSinceLastFlameMS = 0;
      this.ownerEntity = owner;
      ThrusterRenderManager.renderVertices = new VertexPositionColor[250];
      ThrusterRenderManager.renderIndices = new short[600];
      ThrusterRenderManager.CreateFlameModel();
    }

    public bool AddBlock(EngineBlock block)
    {
      if (block == null)
        return false;
      this.engineBlocks.Add(block);
      this.UpdateBuffers();
      return true;
    }

    public void ClearBlocks()
    {
      this.engineBlocks.Clear();
      this.UpdateBuffers();
    }

    public void Dispose()
    {
    }

    public void RenderFullQuality(IRenderCamera camera, ref Matrix transformMatrix)
    {
      if (ThrusterRenderManager.environmentManager == null || this.ownerEntity == null)
        return;
      this.timeSinceLastFlameMS += (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.timeSinceLastFlameMS < 33)
        return;
      foreach (EngineBlock engineBlock in this.engineBlocks)
      {
        if (engineBlock.Triggered)
        {
          EngineBlockType blockType = engineBlock.GetBlockType() as EngineBlockType;
          Vector3 size = (Vector3) blockType.Size;
          Byte3 modelSize = blockType.ModelSize;
          Vector3 positionInShipSpace = engineBlock.PositionInShipSpace;
          for (int index = 0; index < 1; ++index)
          {
            Position3 position3 = this.ownerEntity.Position + Vector3.Transform(positionInShipSpace + new Vector3(((float) this.random.NextDouble() - 0.5f) * (float) modelSize.X, 0.0f, ((float) this.random.NextDouble() - 0.5f) * (float) modelSize.Z), this.ownerEntity.TransformMatrix);
            int lifetime = 100 + this.random.Next(300);
            Vector3 speed = Vector3.TransformNormal(blockType.ThrottleForce * Vector3.TransformNormal(Vector3.Up, engineBlock.OrientationMatrix) * 0.1f, transformMatrix) + this.ownerEntity.GetSpeedAt(position3);
            ThrusterRenderManager.environmentManager.AddThrusterFlame(position3, speed, Color.White, lifetime, 0.5f * (float) ((int) modelSize.Z * (int) modelSize.X));
          }
        }
      }
      this.timeSinceLastFlameMS %= 33;
    }

    public bool RemoveBlock(EngineBlock block)
    {
      if (!this.engineBlocks.Remove(block))
        return false;
      this.UpdateBuffers();
      return true;
    }

    public static EnvironmentManager EnvironmentManager
    {
      set => ThrusterRenderManager.environmentManager = value;
    }

    private static void CreateFlameModel()
    {
      ThrusterRenderManager.flameVertices = new VertexPositionColor[5];
      ThrusterRenderManager.flameVertices[0] = new VertexPositionColor(new Vector3(-0.5f, 0.0f, -0.5f), Color.White);
      ThrusterRenderManager.flameVertices[1] = new VertexPositionColor(new Vector3(0.5f, 0.0f, -0.5f), Color.White);
      ThrusterRenderManager.flameVertices[2] = new VertexPositionColor(new Vector3(0.5f, 0.0f, 0.5f), Color.White);
      ThrusterRenderManager.flameVertices[3] = new VertexPositionColor(new Vector3(-0.5f, 0.0f, 0.5f), Color.White);
      ThrusterRenderManager.flameVertices[4] = new VertexPositionColor(Vector3.UnitY * 1f, Color.White);
      ThrusterRenderManager.flameIndices = new short[12];
      ThrusterRenderManager.flameIndices[0] = (short) 1;
      ThrusterRenderManager.flameIndices[1] = (short) 0;
      ThrusterRenderManager.flameIndices[2] = (short) 4;
      ThrusterRenderManager.flameIndices[3] = (short) 2;
      ThrusterRenderManager.flameIndices[4] = (short) 1;
      ThrusterRenderManager.flameIndices[5] = (short) 4;
      ThrusterRenderManager.flameIndices[6] = (short) 3;
      ThrusterRenderManager.flameIndices[7] = (short) 2;
      ThrusterRenderManager.flameIndices[8] = (short) 4;
      ThrusterRenderManager.flameIndices[9] = (short) 0;
      ThrusterRenderManager.flameIndices[10] = (short) 3;
      ThrusterRenderManager.flameIndices[11] = (short) 4;
    }

    private void UpdateBuffers()
    {
      int vertexCount = Math.Min(this.engineBlocks.Count + 5, 50) * 5;
      int indexCount = Math.Min(this.engineBlocks.Count + 5, 50) * 12;
      if (this.vertexBuffer == null || this.indexBuffer == null)
      {
        if (this.vertexBuffer == null && vertexCount > 0)
          this.vertexBuffer = new VertexBufferObj(Engine.GraphicsDevice, vertexCount, VertexPositionColor.SizeInBytes, this.vertexDeclaration, BufferUsage.WriteOnly);
        if (this.indexBuffer != null || indexCount <= 0)
          return;
        this.indexBuffer = new IndexBufferObj(Engine.GraphicsDevice, indexCount, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      }
      else
      {
        if (this.vertexBuffer.VertexCount < this.engineBlocks.Count * 5 || this.vertexBuffer.VertexCount > (this.engineBlocks.Count + 10) * 5)
        {
          this.vertexBuffer.Dispose();
          this.vertexBuffer = (VertexBufferObj) null;
          if (vertexCount > 0)
            this.vertexBuffer = new VertexBufferObj(Engine.GraphicsDevice, vertexCount, VertexPositionColor.SizeInBytes, this.vertexDeclaration, BufferUsage.WriteOnly);
        }
        if (this.indexBuffer.IndexCount >= this.engineBlocks.Count * 12 && this.indexBuffer.IndexCount <= (this.engineBlocks.Count + 10) * 12)
          return;
        this.indexBuffer.Dispose();
        this.indexBuffer = (IndexBufferObj) null;
        if (indexCount <= 0)
          return;
        this.indexBuffer = new IndexBufferObj(Engine.GraphicsDevice, indexCount, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
      }
    }
  }
}
