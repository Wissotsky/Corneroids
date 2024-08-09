// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.MainMenuLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace.GUI
{
  public class MainMenuLayer : CaptionWindowLayer
  {
    private const int windowWidth = 400;
    private const int windowHeight = 400;

    public MainMenuLayer()
      : base(new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 200, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - 200, 400, 400), "Corneroids")
    {
      this.layers.Add((Layer) new ButtonLayer(new Rectangle((int) this.Position.X + 20, (int) this.Position.Y + 30, 360, 25), "Singleplayer"));
      this.layers.Add((Layer) new ButtonLayer(new Rectangle((int) this.Position.X + 20, (int) this.Position.Y + 70, 360, 25), "Multiplayer"));
      this.layers.Add((Layer) new ButtonLayer(new Rectangle((int) this.Position.X + 20, (int) this.Position.Y + 110, 360, 25), "Options"));
      this.layers.Add((Layer) new ButtonLayer(new Rectangle((int) this.Position.X + 20, (int) this.Position.Y + 400 - 50, 360, 25), "Exit game"));
    }

    public override void Render()
    {
      base.Render();
      foreach (Layer layer in this.layers)
        layer.Render();
    }
  }
}
