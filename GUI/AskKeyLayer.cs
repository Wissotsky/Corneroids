// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.AskKeyLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class AskKeyLayer : WindowLayer
  {
    private const int windowWidth = 200;
    private const int windowHeight = 50;
    private const string cMessage = "Press any key";

    public AskKeyLayer()
      : base(new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 100, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - 25, 200, 50))
    {
    }

    public override void Render()
    {
      base.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      Vector2 vector2 = Engine.Font.MeasureString("Press any key");
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Press any key", this.Position + 0.5f * (this.Size - vector2), Color.White);
      spriteBatch.End();
    }
  }
}
