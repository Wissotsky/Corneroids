// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ConsoleLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace.GUI
{
  public class ConsoleLayer : WindowLayer
  {
    private const int windowWidth = 800;
    private const int windowHeight = 480;
    private WindowLayer commandLineLayer;

    public ConsoleLayer()
      : base(new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 400, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - 240, 800, 480))
    {
      this.commandLineLayer = new WindowLayer(new Rectangle(0, 450, 800, 30));
    }

    public override void Render()
    {
      base.Render();
      this.commandLineLayer.Render();
    }

    public Vector2 CommandLinePosition => this.Position + new Vector2(0.0f, 450f);
  }
}
