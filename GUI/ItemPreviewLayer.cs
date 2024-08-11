// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ItemPreviewLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class ItemPreviewLayer : Layer
  {
    private Texture2D background;
    private BasicEffect effect;
    private Quaternion orientation;
    private Block previewBlock;

    public ItemPreviewLayer(Rectangle positionAndSize)
      : base(positionAndSize)
    {
      this.orientation = Quaternion.Identity;
      try
      {
        this.background = new Texture2D(Engine.GraphicsDevice, 2, 2);
        this.background.SetData<Color>(new Color[4]
        {
          Color.Black,
          Color.Black,
          Color.Black,
          Color.Black
        });
      }
      catch
      {
      }
    }

    public override void Render()
    {
      Rectangle positionAndSize = this.PositionAndSize;
      Rectangle destinationRectangle = new Rectangle(positionAndSize.X, positionAndSize.Y, positionAndSize.Width, positionAndSize.Height);
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      if (this.background != null)
      {
        spriteBatch.Begin();
        spriteBatch.Draw(this.background, destinationRectangle, Color.Black);
        spriteBatch.End();
      }
      if (this.previewBlock == null || this.effect == null)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      this.orientation *= Quaternion.CreateFromYawPitchRoll(1f * Engine.FrameCounter.DeltaTime, 1f * Engine.FrameCounter.DeltaTime, 0.0f);
      Byte3 size = this.previewBlock.Size;
      float num = (float)Math.Max(Math.Max(size.X, size.Z), size.Y);
      Viewport viewport1 = new Viewport()
      {
        X = destinationRectangle.X,
        Y = destinationRectangle.Y,
        Width = destinationRectangle.Width,
        Height = destinationRectangle.Height,
        MinDepth = 0,
        MaxDepth = 1
      };
      this.effect.World = Matrix.CreateFromQuaternion(this.orientation) * Matrix.CreateTranslation(0.0f, 0.0f, -(num + 0.5f));
      this.effect.View = Matrix.Identity;
      this.effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, viewport1.AspectRatio, 0.1f, 100f);
      graphicsDevice.DepthStencilState = DepthStencilState.Default;
      Viewport viewport2 = graphicsDevice.Viewport;
      graphicsDevice.Viewport = viewport1;
      graphicsDevice.Viewport = viewport2;
    }

    public BasicEffect Effect
    {
      set => this.effect = value;
    }

    public Block BlockToPreview
    {
      set => this.previewBlock = value;
    }
  }
}
