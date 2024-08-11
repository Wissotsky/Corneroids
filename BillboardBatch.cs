// Decompiled with JetBrains decompiler
// Type: CornerSpace.BillboardBatch
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
  public class BillboardBatch : IDisposable
  {
    private Dictionary<Texture2D, BillboardBatch.TextureBillboards> texturesAndSprites;
    private Effect effect;
    private Vector2[] sizeArray;
    private BillboardVertex[] vertexArray;
    private readonly int maxBillboardsPerBatch;
    private readonly int maxBillboardsPerDrawCall;
    private DynamicVertexBuffer vertexBuffer;
    private readonly int vertexBufferSizeInVertices;
    private VertexDeclaration vertexDeclaration;
    public static bool Kill;

    public BillboardBatch(int maxBillBoardsPerBatch)
    {
      this.maxBillboardsPerBatch = maxBillBoardsPerBatch > 0 ? maxBillBoardsPerBatch : throw new ArgumentException();
      this.maxBillboardsPerDrawCall = 200;
      this.texturesAndSprites = new Dictionary<Texture2D, BillboardBatch.TextureBillboards>();
      this.effect = Engine.ShaderPool[6];
      this.vertexArray = new BillboardVertex[this.maxBillboardsPerDrawCall * 6];
      this.sizeArray = new Vector2[this.maxBillboardsPerDrawCall];
      this.vertexBufferSizeInVertices = this.maxBillboardsPerDrawCall * 6 * 4;
      this.vertexDeclaration = Engine.VertexDeclarationPool[6];
      this.InitializeVertexBuffer();
      Engine.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(this.GraphicsDeviceReseted);
    }

    public void Begin()
    {
      foreach (BillboardBatch.TextureBillboards textureBillboards in this.texturesAndSprites.Values)
        textureBillboards.Count = 0;
      if (this.texturesAndSprites.Keys.Count <= 100)
        return;
      this.texturesAndSprites.Clear();
    }

    public void Dispose()
    {
      if (this.vertexBuffer != null && !this.vertexBuffer.IsDisposed)
        this.vertexBuffer.Dispose();
      Engine.GraphicsDevice.DeviceReset -= this.GraphicsDeviceReseted;
    }

    public void DrawBillboard(
      Texture2D texture,
      Vector3 position,
      Vector4 uv,
      Vector2 size,
      Color color)
    {
      if (texture == null || size == Vector2.Zero)
        return;
      BillboardBatch.TextureBillboards textureBillboards;
      if (this.texturesAndSprites.ContainsKey(texture))
      {
        textureBillboards = this.texturesAndSprites[texture];
      }
      else
      {
        textureBillboards = new BillboardBatch.TextureBillboards(this.maxBillboardsPerBatch);
        this.texturesAndSprites.Add(texture, textureBillboards);
      }
      Vector2 vector2_1 = new Vector2(uv.X, uv.Y);
      Vector2 vector2_2 = new Vector2(uv.X + uv.Z, uv.Y);
      Vector2 vector2_3 = new Vector2(uv.X, uv.Y + uv.W);
      Vector2 vector2_4 = new Vector2(uv.X + uv.Z, uv.Y + uv.W);
      int count = textureBillboards.Count;
      int num = count % this.maxBillboardsPerDrawCall;
      if (count >= this.maxBillboardsPerBatch)
        return;
      if (textureBillboards.Billboards[count].Vertices == null)
        textureBillboards.Billboards[count].Vertices = new BillboardVertex[6];
      textureBillboards.Billboards[count].Vertices[0] = new BillboardVertex()
      {
        Position = position,
        TextureCoordinates = vector2_1,
        Index = 0.0f,
        Size = size,
        Color = color
      };
      textureBillboards.Billboards[count].Vertices[1] = new BillboardVertex()
      {
        Position = position,
        TextureCoordinates = vector2_2,
        Index = 1f,
        Size = size,
        Color = color
      };
      textureBillboards.Billboards[count].Vertices[2] = new BillboardVertex()
      {
        Position = position,
        TextureCoordinates = vector2_4,
        Index = 3f,
        Size = size,
        Color = color
      };
      textureBillboards.Billboards[count].Vertices[3] = new BillboardVertex()
      {
        Position = position,
        TextureCoordinates = vector2_4,
        Index = 3f,
        Size = size,
        Color = color
      };
      textureBillboards.Billboards[count].Vertices[4] = new BillboardVertex()
      {
        Position = position,
        TextureCoordinates = vector2_3,
        Index = 2f,
        Size = size,
        Color = color
      };
      textureBillboards.Billboards[count].Vertices[5] = new BillboardVertex()
      {
        Position = position,
        TextureCoordinates = vector2_1,
        Index = 0.0f,
        Size = size,
        Color = color
      };
      ++textureBillboards.Count;
    }

    public void End(IRenderCamera camera)
    {
      try
      {
        if (camera == null)
          throw new ArgumentNullException();
        if (this.vertexBuffer == null)
          return;
        this.effect.Parameters["View"].SetValue(camera.ViewMatrix);
        this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
        this.effect.Parameters["CameraPosition"].SetValue(Vector3.Zero);
        this.effect.Parameters["CameraUpVector"].SetValue(camera.GetUpVector());
        Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        Engine.GraphicsDevice.BlendState = BlendState.Opaque;
        Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        int num1 = 0;
        foreach (Texture2D key in this.texturesAndSprites.Keys)
        {
          BillboardBatch.TextureBillboards texturesAndSprite = this.texturesAndSprites[key];
          if (texturesAndSprite.Count > 0)
          {
            num1 += texturesAndSprite.Count;
            int num2 = 0;
            for (int index1 = 0; index1 < texturesAndSprite.Count / this.maxBillboardsPerDrawCall + 1; ++index1)
            {
              int elementCount = Math.Min(texturesAndSprite.Count - index1 * this.maxBillboardsPerDrawCall, this.maxBillboardsPerDrawCall) * 6;
              int num3 = index1 * this.maxBillboardsPerDrawCall;
              int num4 = Math.Min(num3 + this.maxBillboardsPerDrawCall, texturesAndSprite.Count);
              if (elementCount != 0)
              {
                for (int index2 = 0; index2 < num4 - num3; ++index2)
                  Array.Copy((Array)texturesAndSprite.Billboards[num3 + index2].Vertices, 0, (Array)this.vertexArray, index2 * 6, 6);
                this.effect.Parameters["Texture"].SetValue((Texture2D)key);
                int startVertex = num2;
                if (num2 == 0 || elementCount + num2 > this.vertexBufferSizeInVertices)
                {
                  this.vertexBuffer.SetData<BillboardVertex>(this.vertexArray, 0, elementCount, SetDataOptions.Discard);
                  num2 = elementCount;
                  startVertex = 0;
                }
                else
                {
                  this.vertexBuffer.SetData<BillboardVertex>(num2 * (int)BillboardVertex.SizeInBytes, this.vertexArray, 0, elementCount, (int)BillboardVertex.SizeInBytes, SetDataOptions.NoOverwrite);
                  num2 += elementCount;
                }
                Engine.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
                this.effect.CurrentTechnique.Passes[0].Apply();
                Engine.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, startVertex, elementCount / 3);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to render billboards: " + ex.Message);
      }
    }

    public int MaxBillboardsPerBatch => this.maxBillboardsPerBatch;

    private void GraphicsDeviceReseted(object sender, EventArgs args)
    {
      this.texturesAndSprites.Clear();
    }

    private void InitializeVertexBuffer()
    {
      try
      {
        if (this.vertexBuffer != null)
        {
          this.vertexBuffer.Dispose();
          this.vertexBuffer = null;
        }
        this.vertexBuffer = new DynamicVertexBuffer(Engine.GraphicsDevice, typeof(BillboardVertex), this.vertexBufferSizeInVertices, BufferUsage.WriteOnly);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a vertex buffer for billboard batch: " + ex.Message);
        if (this.vertexBuffer == null)
          return;
        this.vertexBuffer.Dispose();
        this.vertexBuffer = null;
      }
    }

    public class TextureBillboards
    {
      public BillboardBatch.Billboard[] Billboards;
      public int Count;

      public TextureBillboards(int maxBillboardsPerBatch)
      {
        this.Billboards = new BillboardBatch.Billboard[maxBillboardsPerBatch];
        this.Count = 0;
      }
    }

    public struct Billboard
    {
      public BillboardVertex[] Vertices;
    }
  }
}
