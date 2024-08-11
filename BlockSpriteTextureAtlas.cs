// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockSpriteTextureAtlas
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
  public class BlockSpriteTextureAtlas : IDisposable
  {
    private const int cSpritesPerRow = 16;
    private const int cSpriteSizePixels = 32;
    private const int cTextureSizePixels = 512;
    private const float cTextureSizePixelsf = 512f;
    private Dictionary<BlockType, Rectangle> blockSprites;
    private Texture2D textureAtlas;

    public BlockSpriteTextureAtlas(BlockType[] blockTypes, BlockTextureAtlas blockTextureAtlas)
    {
      this.blockSprites = new Dictionary<BlockType, Rectangle>();
      this.textureAtlas = (Texture2D) null;
      if (blockTypes == null || blockTextureAtlas == null)
        throw new ArgumentNullException();
      for (int index = 0; index < Math.Min(blockTypes.Length, 256); ++index)
      {
        if ((Item) blockTypes[index] != (Item) null)
        {
          Rectangle? blockSprite = this.CreateBlockSprite(blockTypes[index], blockTextureAtlas);
          if (blockSprite.HasValue)
          {
            Rectangle rect = blockSprite.Value;
            Vector4 uv = new Vector4((float) rect.X / 512f, (float) rect.Y / 512f, (float) rect.Width / 512f, (float) rect.Height / 512f);
            blockTypes[index].SetSpriteCoordsInBlockAtlas(rect, uv);
          }
        }
      }
    }

    public Rectangle? CreateBlockSprite(BlockType blockType, BlockTextureAtlas blockTextureAtlas)
    {
      if (!((Item)blockType != (Item)null) || blockTextureAtlas == null)
        return new Rectangle?();
      if (this.blockSprites.ContainsKey(blockType))
        return new Rectangle?(this.blockSprites[blockType]);
      try
      {
        if (this.textureAtlas == null)
        {
          this.textureAtlas = new Texture2D(Engine.GraphicsDevice, 512, 512, false, SurfaceFormat.Color);
          Color[] data = new Color[262144];
          for (int index = 0; index < data.Length; ++index)
            data[index] = Color.Transparent;
          this.textureAtlas.SetData<Color>(data);
        }
        int count = this.blockSprites.Count;
        Rectangle rectangle = new Rectangle(count % 16 * 32, count / 16 * 32, 32, 32);
        BlockTextureCoordinates textureCoordinates = blockType.GetTextureCoordinates();
        Byte4 topCoordinates = textureCoordinates.TopCoordinates;
        Byte4 wallCoordinates = textureCoordinates.WallCoordinates;
        Rectangle sourceRect1 = new Rectangle((int)topCoordinates.X * 32, (int)topCoordinates.Y * 32, (int)topCoordinates.Z * 32, (int)topCoordinates.W * 32);
        Rectangle sourceRect2 = new Rectangle((int)wallCoordinates.X * 32, (int)wallCoordinates.Y * 32, (int)wallCoordinates.Z * 32, (int)wallCoordinates.W * 32);
        Color[] colorArray = new Color[1024];
        byte[] interpolationArray = new byte[1024];
        Matrix wallTransformMatrix1 = CreateLeftWallTransformMatrix(sourceRect2.Width, sourceRect2.Height);
        Matrix wallTransformMatrix2 = CreateRightWallTransformMatrix(sourceRect2.Width, sourceRect2.Height);
        Matrix roofTransformMatrix = CreateRoofTransformMatrix(sourceRect1.Width, sourceRect1.Height);
        CreateSprite(blockTextureAtlas.Texture, sourceRect1, colorArray, interpolationArray, ref roofTransformMatrix);
        CreateSprite(blockTextureAtlas.Texture, sourceRect2, colorArray, interpolationArray, ref wallTransformMatrix1);
        CreateSprite(blockTextureAtlas.Texture, sourceRect2, colorArray, interpolationArray, ref wallTransformMatrix2);
        this.textureAtlas.SetData<Color>(0, rectangle, colorArray, 0, 1024);
        this.blockSprites.Add(blockType, rectangle);
        return new Rectangle?(rectangle);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a sprite for a block: " + blockType.Name + "(" + ex.Message + ")");
        return new Rectangle?();
      }
    }

    public void Dispose()
    {
      if (this.textureAtlas == null || this.textureAtlas.IsDisposed)
        return;
      this.textureAtlas.Dispose();
    }

    public Texture2D Texture => this.textureAtlas;

    private Matrix CreateLeftWallTransformMatrix(int width, int height)
    {
      Matrix identity = Matrix.Identity with { M12 = 0.5f };
      return Matrix.CreateScale(32f / (float) width, 32f / (float) height, 1f) * Matrix.CreateScale(0.5f) * identity * Matrix.CreateTranslation(0.0f, 8f, 0.0f);
    }

    private Matrix CreateRightWallTransformMatrix(int width, int height)
    {
      Matrix identity = Matrix.Identity with { M12 = -0.5f };
      return Matrix.CreateScale(32f / (float) width, 32f / (float) height, 1f) * Matrix.CreateScale(0.5f) * identity * Matrix.CreateTranslation(16f, 16f, 8f);
    }

    private Matrix CreateRoofTransformMatrix(int width, int height)
    {
      return Matrix.CreateScale(32f / (float) width, 32f / (float) height, 1f) * Matrix.CreateTranslation(-16f, -16f, 0.0f) * Matrix.CreateRotationZ(0.7853982f) * Matrix.CreateScale(0.7072136f, 0.7072136f, 0.0f) * Matrix.CreateScale(1f, 0.5f, 1f) * Matrix.CreateTranslation(16f, 8f, 0.0f);
    }

    private void CreateSprite(
      Texture2D blockAtlas,
      Rectangle sourceRect,
      Color[] targetPixels,
      byte[] interpolationArray,
      ref Matrix transform)
    {
      if (blockAtlas == null || targetPixels == null || targetPixels.Length != 1024)
        return;
      if (interpolationArray != null)
      {
        for (int index = 0; index < interpolationArray.Length; ++index)
          interpolationArray[index] = (byte) 0;
      }
      Color[] data = new Color[sourceRect.Width * sourceRect.Height];
      blockAtlas.GetData<Color>(0, new Rectangle?(sourceRect), data, 0, data.Length);
      for (int y = 0; y < sourceRect.Height; ++y)
      {
        for (int x = 0; x < sourceRect.Width; ++x)
        {
          Point point = this.TransformPixel(new Point(x, y), ref transform);
          int index1 = Math.Max(Math.Min(y * sourceRect.Width + x, sourceRect.Width * sourceRect.Height - 1), 0);
          int index2 = Math.Max(Math.Min(point.Y * 32 + point.X, 1023), 0);
          byte interpolation = interpolationArray[index2];
          if (interpolation == (byte) 0)
          {
            targetPixels[index2] = data[index1];
          }
          else
          {
            Vector3int vector3int = new Vector3int((int) targetPixels[index2].R * (int) interpolation, (int) targetPixels[index2].G * (int) interpolation, (int) targetPixels[index2].B * (int) interpolation);
            targetPixels[index2] = new Color((byte) ((vector3int.X + (int) data[index1].R) / ((int) interpolation + 1)), (byte) ((vector3int.Y + (int) data[index1].G) / ((int) interpolation + 1)), (byte) ((vector3int.Z + (int) data[index1].B) / ((int) interpolation + 1)));
          }
          ++interpolationArray[index2];
        }
      }
    }

    private Point TransformPixel(Point pixel, ref Matrix transform)
    {
      Vector3 vector3 = Vector3.Transform(new Vector3((float) pixel.X, (float) pixel.Y, 0.0f), transform);
      return new Point((int) vector3.X, (int) vector3.Y);
    }
  }
}
