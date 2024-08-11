// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.SpriteImage
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class SpriteImage
  {
    protected const int defaultSize = 32;
    private Rectangle positionAndSizeInSourceImage;
    private Texture2D sourceImage;

    public SpriteImage(Texture2D sourceImage)
    {
      if (sourceImage == null)
        throw new ArgumentNullException();
      this.positionAndSizeInSourceImage = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
      this.sourceImage = sourceImage;
    }

    public SpriteImage(Texture2D sourceImage, Rectangle positionAndSize)
    {
      this.positionAndSizeInSourceImage = positionAndSize;
      this.sourceImage = sourceImage;
    }

    public void Render(Rectangle destination)
    {
      if (this.sourceImage == null)
        return;
      try
      {
        SpriteBatch spriteBatch = Engine.SpriteBatch;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        Engine.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        spriteBatch.Draw(this.sourceImage, destination, this.positionAndSizeInSourceImage, Color.White);
        spriteBatch.End();
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to render a sprite image: " + ex.Message);
      }
    }

    public void SetNewRegion(Point point)
    {
      this.SetNewRegion(new Rectangle(point.X, point.Y, 32, 32));
    }

    public void SetNewRegion(Rectangle region) => this.positionAndSizeInSourceImage = region;

    public Point Size
    {
      get
      {
        return new Point(this.positionAndSizeInSourceImage.Width, this.positionAndSizeInSourceImage.Height);
      }
    }
  }
}
