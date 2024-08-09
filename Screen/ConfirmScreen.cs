// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ConfirmScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class ConfirmScreen : MenuScreen
  {
    private ButtonLayer yesButton;
    private ButtonLayer noButton;
    private WindowLayer windowLayer;
    private Action<bool> resultFunction;
    private string caption;

    public ConfirmScreen(Action<bool> resultFunction, string caption)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.resultFunction = resultFunction;
      this.caption = caption;
    }

    public override void Load()
    {
      Vector2 vector2 = Engine.Font.MeasureString(this.caption);
      Rectangle position = new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - Math.Max(200, (int) vector2.X + 20) / 2, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - 50, Math.Max(200, (int) vector2.X + 20), 100);
      this.windowLayer = new WindowLayer(position);
      this.yesButton = new ButtonLayer(new Rectangle(position.X + position.Width / 2 - 90, position.Y + position.Height - 40, 80, 30), "Yes");
      this.noButton = new ButtonLayer(new Rectangle(position.X + position.Width / 2 + 10, position.Y + position.Height - 40, 80, 30), "No");
    }

    public override void Render()
    {
      this.windowLayer.Render();
      this.yesButton.Render();
      this.noButton.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      Vector2 position = new Vector2((float) (this.windowLayer.PositionAndSize.X + this.windowLayer.PositionAndSize.Width / 2) - Engine.Font.MeasureString(this.caption).X / 2f, (float) (this.windowLayer.PositionAndSize.Y + 10));
      spriteBatch.DrawString(Engine.Font, this.caption, position, Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.yesButton.UpdateInput(this.Mouse);
      this.noButton.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.yesButton.Contains(this.Mouse.Position))
      {
        if (this.resultFunction != null)
          this.resultFunction(true);
        this.CloseScreen();
      }
      else
      {
        if (!this.noButton.Contains(this.Mouse.Position))
          return;
        if (this.resultFunction != null)
          this.resultFunction(false);
        this.CloseScreen();
      }
    }
  }
}
