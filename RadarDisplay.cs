// Decompiled with JetBrains decompiler
// Type: CornerSpace.RadarDisplay
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
  public class RadarDisplay : IDisposable
  {
    private const float cMaximumScanLength = 256f;
    private const int cMaxEntityCount = 256;
    private BillboardBatch billboardBatch;
    private Texture2D blankTexture;
    private Camera camera;
    //private DepthStencilBuffer depthBuffer;
    private List<PhysicalObject> entities;
    private Quaternion orientation;
    private Vector3 origo;
    private Point size;
    private Viewport viewport;
    private Effect effect;
    private VertexBufferObj vertexBuffer;
    private VertexDeclaration vertexDeclaration;

    public RadarDisplay(Point size)
    {
      this.effect = Engine.ShaderPool[11];
      this.vertexDeclaration = Engine.VertexDeclarationPool[2];
      this.blankTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
      if (this.effect != null)
        this.effect.Parameters["Texture"].SetValue((Texture) Engine.ContentManager.Load<Texture2D>("Textures/Sprites/radar"));
      this.billboardBatch = new BillboardBatch(100);
      this.entities = new List<PhysicalObject>();
      this.orientation = Quaternion.Identity;
      this.size = size;
      if (size.X <= 0 || size.Y <= 0)
        throw new ArgumentException();
      this.viewport = this.CreateRadarViewport(size);
      this.camera = new Camera(this.viewport.AspectRatio, 1.57079637f, 0.01f, 100f);
      throw new NotImplementedException();
    }

    public void Clear() => this.entities.Clear();

    public void Dispose()
    {
      if (this.billboardBatch != null)
        this.billboardBatch.Dispose();
      if (this.vertexBuffer == null)
        return;
      this.vertexBuffer.Dispose();
    }

    public void Render(Point screenPosition)
    {
      if (this.camera == null)
        return;
      this.viewport.X = screenPosition.X;
      this.viewport.Y = screenPosition.Y;
      this.effect.Parameters["World"].SetValue(Matrix.CreateScale(2f, 1f, 2f));
      this.effect.Parameters["View"].SetValue(this.camera.ViewMatrix);
      this.effect.Parameters["Projection"].SetValue(this.camera.ProjectionMatrix);
      //Engine.GraphicsDevice.VertexDeclaration = this.vertexDeclaration;
      //Engine.GraphicsDevice.Vertices[0].SetSource((VertexBuffer) this.vertexBuffer, 0, VertexPositionTexture.SizeInBytes);
      Engine.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
      Engine.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
      Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
      Engine.GraphicsDevice.BlendState = BlendState.Opaque;
      Viewport viewport = Engine.GraphicsDevice.Viewport;
      Engine.GraphicsDevice.Viewport = this.viewport;
      this.effect.CurrentTechnique.Passes[0].Apply();
      Engine.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
      if (this.entities.Count > 0)
      {
        Color red = Color.Red;
        Color darkRed = Color.DarkRed;
        this.billboardBatch.Begin();
        if (0 < this.entities.Count)
          throw new NotImplementedException();
        this.billboardBatch.End((IRenderCamera)this.camera);
      }
      Engine.GraphicsDevice.Viewport = viewport;
    }

    public void SetCoordinatesOrientation(Quaternion orientation) => this.orientation = orientation;

    public void SetOrigo(Vector3 position) => this.origo = position;

    public bool TrackObject(PhysicalObject entity)
    {
      if (entity == null || this.entities.Contains(entity) || this.entities.Count >= 100)
        return false;
      this.entities.Add(entity);
      return true;
    }

    public bool UntrackObject(PhysicalObject entity)
    {
      return entity != null && this.entities.Remove(entity);
    }

    public Point Size => this.size;

    private void CreateAndPopulateVertexBuffer()
    {
      if (this.vertexBuffer != null)
        return;
      try
      {
        this.vertexBuffer = new VertexBufferObj(Engine.GraphicsDevice, Engine.VertexDeclarationPool[2], 6, BufferUsage.WriteOnly);
        this.vertexBuffer.SetData<VertexPositionTexture>(new List<VertexPositionTexture>()
        {
          new VertexPositionTexture(new Vector3(-0.5f, 0.0f, -0.5f), Vector2.Zero),
          new VertexPositionTexture(new Vector3(0.5f, 0.0f, -0.5f), Vector2.UnitX),
          new VertexPositionTexture(new Vector3(0.5f, 0.0f, 0.5f), Vector2.One),
          new VertexPositionTexture(new Vector3(0.5f, 0.0f, 0.5f), Vector2.One),
          new VertexPositionTexture(new Vector3(-0.5f, 0.0f, 0.5f), Vector2.UnitY),
          new VertexPositionTexture(new Vector3(-0.5f, 0.0f, -0.5f), Vector2.Zero)
        }.ToArray());
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to initialize the radar: " + ex.Message);
        this.vertexBuffer = (VertexBufferObj) null;
      }
    }

    private Viewport CreateRadarViewport(Point size)
    {
      return new Viewport()
      {
        Width = size.X,
        Height = size.Y,
        MinDepth = 0.0f,
        MaxDepth = 0.1f
      };
    }

    private bool ValidateRectangle(Rectangle screenPosition)
    {
      return screenPosition.Width > 0 && screenPosition.Height > 0;
    }
  }
}
