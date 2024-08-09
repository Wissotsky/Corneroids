// Decompiled with JetBrains decompiler
// Type: CornerSpace.ItemWithModel
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class ItemWithModel : Item
  {
    private const float cModelUnitSize = 0.03125f;
    private const int cVBSize = 900;
    private const int cIBSize = 1800;
    private Effect effect;
    private float modelDepth;
    private VertexPositionNormalTexture[] modelVertices;
    private short[] modelIndices;
    private Vector3 modelOffset;
    private VertexDeclaration vertexDeclaration;
    private static VertexBufferObj modelVB;
    private static IndexBufferObj modelIB;
    private static ItemWithModel modelInBuffers;

    public ItemWithModel()
    {
      this.effect = Engine.ShaderPool[8];
      this.vertexDeclaration = Engine.VertexDeclarationPool[7];
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.modelDepth = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 1f, "modelDepth");
      this.modelDepth = Math.Max(1f, Math.Min(this.modelDepth, 7f));
    }

    public override void RenderFirstPersonWithLighting(IRenderCamera camera)
    {
      Matrix matrix = Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateTranslation(this.modelOffset) * Matrix.CreateTranslation(0.1f, 0.02f, -0.3f);
      this.effect.Parameters["CameraView"].SetValue(Matrix.CreateFromQuaternion(camera.Orientation));
      this.effect.Parameters["World"].SetValue(Matrix.Identity);
      this.effect.Parameters["View"].SetValue(matrix);
      this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      this.RenderModel();
    }

    public override void RenderThirdPersonWithLighting(IRenderCamera camera, Matrix holderMatrix)
    {
      this.effect.Parameters["World"].SetValue(Matrix.CreateScale(3f, 3f, 3f) * Matrix.CreateRotationY(-1.57079637f) * Matrix.CreateTranslation(this.modelOffset) * Matrix.CreateTranslation(0.1f, 0.02f, -0.8f) * holderMatrix);
      this.effect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      this.RenderModel();
    }

    protected Vector3 ModelOffset
    {
      set => this.modelOffset = value;
      get => this.modelOffset;
    }

    protected void CreateModel(Rectangle coords, Vector4 coordsUV, Texture2D texture)
    {
      try
      {
        if (coords.Width <= 0 || coords.Height <= 0 || texture == null)
          return;
        Color[] data = new Color[coords.Width * coords.Height];
        texture.GetData<Color>(0, new Rectangle?(coords), data, 0, data.Length);
        int? nullable1 = new int?();
        int? nullable2 = new int?();
        int? nullable3 = new int?();
        List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
        List<short> indices = new List<short>();
        Vector3 vector3 = new Vector3((float) (-coords.Width / 2), (float) (coords.Height / 2), 0.0f);
        for (int y = 0; y < coords.Height; ++y)
        {
          for (int index = 0; index < coords.Width; ++index)
          {
            if (!nullable1.HasValue)
            {
              if (data[y * coords.Width + index].A > (byte) 0)
                nullable1 = new int?(index);
            }
            else if (data[y * coords.Width + index].A == (byte) 0)
            {
              this.CreateZsquare(vertices, indices, nullable1.Value, index, y, 1f, coords, coordsUV);
              this.CreateZsquare(vertices, indices, index, nullable1.Value, y, -this.modelDepth, coords, coordsUV);
              nullable1 = new int?();
            }
            if (!nullable2.HasValue)
            {
              if (data[y * coords.Width + index].A > (byte) 0 && (y <= 0 || data[Math.Max(0, (y - 1) * coords.Width + index)].A == (byte) 0))
                nullable2 = new int?(index);
            }
            else if (data[y * coords.Width + index].A == (byte) 0 || y > 0 && data[Math.Max(0, (y - 1) * coords.Width + index)].A > (byte) 0)
            {
              this.CreateYsquare(vertices, indices, nullable2.Value, index + 1, y, this.modelDepth, coords, coordsUV);
              nullable2 = new int?();
            }
          }
          if (nullable1.HasValue)
          {
            this.CreateZsquare(vertices, indices, nullable1.Value, coords.Width, y, 1f, coords, coordsUV);
            this.CreateZsquare(vertices, indices, coords.Width, nullable1.Value, y, -this.modelDepth, coords, coordsUV);
            nullable1 = new int?();
          }
          if (nullable2.HasValue)
          {
            this.CreateYsquare(vertices, indices, nullable2.Value, coords.Width, y, this.modelDepth, coords, coordsUV);
            nullable2 = new int?();
          }
        }
        for (int index = coords.Width - 1; index >= 0; --index)
        {
          for (int y = 0; y < coords.Height; ++y)
          {
            if (!nullable3.HasValue)
            {
              if (data[y * coords.Height + index].A > (byte) 0 && (index >= coords.Width || data[Math.Min(coords.Width * coords.Height - 1, y * coords.Width + index + 1)].A == (byte) 0))
                nullable3 = new int?(y);
            }
            else if (data[y * coords.Width + index].A == (byte) 0 || index < coords.Width && data[Math.Min(coords.Width * coords.Height - 1, y * coords.Width + index + 1)].A > (byte) 0)
            {
              this.CreateXsquare(vertices, indices, nullable3.Value, index + 1, y, this.modelDepth, coords, coordsUV);
              nullable3 = new int?();
            }
          }
        }
        if (vertices.Count <= 0 || indices.Count <= 0)
          return;
        this.modelVertices = vertices.ToArray();
        this.modelIndices = indices.ToArray();
        Matrix scale = Matrix.CreateScale((float) coords.Width * 0.1f / (float) texture.Width);
        for (int index = 0; index < this.modelVertices.Length; ++index)
          this.modelVertices[index].Position = Vector3.Transform(this.modelVertices[index].Position, scale);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a model for an item: " + ex.Message);
      }
    }

    private void CreateZsquare(
      List<VertexPositionNormalTexture> vertices,
      List<short> indices,
      int rectangleStartPosition,
      int x,
      int y,
      float z,
      Rectangle coords,
      Vector4 coordsUV)
    {
      SpriteTextureAtlas spriteTextureAtlas = Engine.LoadedWorld.SpriteTextureAtlas;
      Vector2 vector2_1 = new Vector2(1f / (float) spriteTextureAtlas.Texture.Width, 1f / (float) spriteTextureAtlas.Texture.Height);
      Vector2 vector2_2 = new Vector2(coordsUV.X, coordsUV.Y);
      VertexPositionNormalTexture positionNormalTexture1 = new VertexPositionNormalTexture(new Vector3((float) rectangleStartPosition, (float) -y, z), Vector3.UnitZ, vector2_2 + new Vector2((float) rectangleStartPosition, (float) y) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture2 = new VertexPositionNormalTexture(new Vector3((float) x, (float) -y, z), Vector3.UnitZ, vector2_2 + new Vector2((float) x, (float) y) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture3 = new VertexPositionNormalTexture(new Vector3((float) x, (float) (-y - 1), z), Vector3.UnitZ, vector2_2 + new Vector2((float) x, (float) (y + 1)) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture4 = new VertexPositionNormalTexture(new Vector3((float) rectangleStartPosition, (float) (-y - 1), z), Vector3.UnitZ, vector2_2 + new Vector2((float) rectangleStartPosition, (float) (y + 1)) * vector2_1);
      int count = vertices.Count;
      vertices.Add(positionNormalTexture1);
      vertices.Add(positionNormalTexture2);
      vertices.Add(positionNormalTexture3);
      vertices.Add(positionNormalTexture4);
      indices.Add((short) count);
      indices.Add((short) (count + 1));
      indices.Add((short) (count + 2));
      indices.Add((short) (count + 2));
      indices.Add((short) (count + 3));
      indices.Add((short) count);
    }

    private void CreateYsquare(
      List<VertexPositionNormalTexture> vertices,
      List<short> indices,
      int rectangleStartPosition,
      int x,
      int y,
      float depth,
      Rectangle coords,
      Vector4 coordsUV)
    {
      SpriteTextureAtlas spriteTextureAtlas = Engine.LoadedWorld.SpriteTextureAtlas;
      Vector2 vector2_1 = new Vector2(1f / (float) spriteTextureAtlas.Texture.Width, 1f / (float) spriteTextureAtlas.Texture.Height);
      Vector2 vector2_2 = new Vector2(coordsUV.X, coordsUV.Y);
      VertexPositionNormalTexture positionNormalTexture1 = new VertexPositionNormalTexture(new Vector3((float) rectangleStartPosition, (float) -y, -depth), Vector3.UnitY, vector2_2 + new Vector2((float) rectangleStartPosition, (float) y) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture2 = new VertexPositionNormalTexture(new Vector3((float) x, (float) -y, -depth), Vector3.UnitY, vector2_2 + new Vector2((float) x, (float) y) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture3 = new VertexPositionNormalTexture(new Vector3((float) x, (float) -y, 1f), Vector3.UnitY, vector2_2 + new Vector2((float) x, (float) (y + 1)) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture4 = new VertexPositionNormalTexture(new Vector3((float) rectangleStartPosition, (float) -y, 1f), Vector3.UnitY, vector2_2 + new Vector2((float) rectangleStartPosition, (float) (y + 1)) * vector2_1);
      int count = vertices.Count;
      vertices.Add(positionNormalTexture1);
      vertices.Add(positionNormalTexture2);
      vertices.Add(positionNormalTexture3);
      vertices.Add(positionNormalTexture4);
      indices.Add((short) count);
      indices.Add((short) (count + 1));
      indices.Add((short) (count + 2));
      indices.Add((short) (count + 2));
      indices.Add((short) (count + 3));
      indices.Add((short) count);
    }

    private void CreateXsquare(
      List<VertexPositionNormalTexture> vertices,
      List<short> indices,
      int rectangleStartPosition,
      int x,
      int y,
      float depth,
      Rectangle coords,
      Vector4 coordsUV)
    {
      SpriteTextureAtlas spriteTextureAtlas = Engine.LoadedWorld.SpriteTextureAtlas;
      Vector2 vector2_1 = new Vector2(1f / (float) spriteTextureAtlas.Texture.Width, 1f / (float) spriteTextureAtlas.Texture.Height);
      Vector2 vector2_2 = new Vector2(coordsUV.X, coordsUV.Y);
      VertexPositionNormalTexture positionNormalTexture1 = new VertexPositionNormalTexture(new Vector3((float) x, (float) -rectangleStartPosition, 1f), Vector3.UnitX, vector2_2 + new Vector2((float) (x - 1), (float) rectangleStartPosition) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture2 = new VertexPositionNormalTexture(new Vector3((float) x, (float) -rectangleStartPosition, -depth), Vector3.UnitX, vector2_2 + new Vector2((float) x, (float) rectangleStartPosition) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture3 = new VertexPositionNormalTexture(new Vector3((float) x, (float) -y, -depth), Vector3.UnitX, vector2_2 + new Vector2((float) x, (float) y) * vector2_1);
      VertexPositionNormalTexture positionNormalTexture4 = new VertexPositionNormalTexture(new Vector3((float) x, (float) -y, 1f), Vector3.UnitX, vector2_2 + new Vector2((float) (x - 1), (float) y) * vector2_1);
      int count = vertices.Count;
      vertices.Add(positionNormalTexture1);
      vertices.Add(positionNormalTexture2);
      vertices.Add(positionNormalTexture3);
      vertices.Add(positionNormalTexture4);
      indices.Add((short) count);
      indices.Add((short) (count + 1));
      indices.Add((short) (count + 2));
      indices.Add((short) (count + 2));
      indices.Add((short) (count + 3));
      indices.Add((short) count);
    }

    private void InitializeGraphicsBuffers()
    {
      try
      {
        if (!((Item) ItemWithModel.modelInBuffers != (Item) this))
          return;
        bool flag1;
        if (ItemWithModel.modelVB == null)
        {
          flag1 = true;
        }
        else
        {
          if (ItemWithModel.modelVB != null)
            ItemWithModel.modelVB.Dispose();
          flag1 = true;
        }
        bool flag2;
        if (ItemWithModel.modelIB == null)
        {
          flag2 = true;
        }
        else
        {
          if (ItemWithModel.modelIB != null)
            ItemWithModel.modelIB.Dispose();
          flag2 = true;
        }
        if (flag1)
          ItemWithModel.modelVB = new VertexBufferObj(Engine.GraphicsDevice, 900, VertexPositionNormalTexture.SizeInBytes, Engine.VertexDeclarationPool[7], BufferUsage.WriteOnly);
        if (flag2)
          ItemWithModel.modelIB = new IndexBufferObj(Engine.GraphicsDevice, 1800, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
        ItemWithModel.modelVB.SetData<VertexPositionNormalTexture>(this.modelVertices);
        ItemWithModel.modelIB.SetData<short>(this.modelIndices);
        ItemWithModel.modelInBuffers = this;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create item model buffers: " + ex.Message);
      }
    }

    private void RenderModel()
    {
      if (this.modelVertices == null || this.modelIndices == null)
        this.CreateModel(this.SpriteCoordsRect, this.SpriteCoordsUV, Engine.LoadedWorld.SpriteTextureAtlas.Texture);
      if (this.effect == null || this.modelVertices == null || this.modelIndices == null)
        return;
      this.InitializeGraphicsBuffers();
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      graphicsDevice.VertexDeclaration = this.vertexDeclaration;
      graphicsDevice.Vertices[0].SetSource((VertexBuffer) ItemWithModel.modelVB, 0, VertexPositionNormalTexture.SizeInBytes);
      graphicsDevice.Indices = (IndexBuffer) ItemWithModel.modelIB;
      graphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
      graphicsDevice.RenderState.DepthBufferEnable = true;
      graphicsDevice.RenderState.DepthBufferWriteEnable = true;
      graphicsDevice.RenderState.AlphaBlendEnable = false;
      graphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      this.effect.Begin();
      this.effect.CurrentTechnique.Passes[0].Begin();
      graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.modelVertices.Length, 0, this.modelIndices.Length / 3);
      this.effect.CurrentTechnique.Passes[0].End();
      this.effect.End();
    }
  }
}
