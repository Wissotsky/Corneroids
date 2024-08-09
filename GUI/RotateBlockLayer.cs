// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.RotateBlockLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class RotateBlockLayer : CaptionWindowLayer
  {
    private const int windowWidth = 260;
    private const int windowHeight = 110;

    public RotateBlockLayer()
      : base(new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 130, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 + 110, 260, 110), "Rotate block")
    {
      Point location = this.Location;
      this.layers.Add((Layer) new ButtonLayer(new Rectangle(20 + location.X, 40 + location.Y, 60, 50), "yaw"));
      this.layers.Add((Layer) new ButtonLayer(new Rectangle(100 + location.X, 40 + location.Y, 60, 50), "pitch"));
      this.layers.Add((Layer) new ButtonLayer(new Rectangle(180 + location.X, 40 + location.Y, 60, 50), "roll"));
    }

    public override void Render()
    {
      base.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      foreach (Layer layer in this.layers)
        layer.Render();
    }
  }
}
