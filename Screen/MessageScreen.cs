// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.MessageScreen
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
  public class MessageScreen : MenuScreen
  {
    private string buttonText;
    private System.Action callback;
    private string captionText;
    private string message;
    private ButtonLayer okButton;
    private bool useButton;
    private CaptionWindowLayer windowLayer;

    public MessageScreen(string message, string caption, bool useButton)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.buttonText = "Ok";
      this.captionText = caption;
      this.message = message;
      this.useButton = useButton;
    }

    public MessageScreen(
      string message,
      string caption,
      bool useButton,
      string buttonText,
      System.Action callback)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.buttonText = buttonText;
      this.callback = callback;
      this.captionText = caption;
      this.message = message;
      this.useButton = useButton;
    }

    public override void Load()
    {
      Vector2 vector2 = Engine.Font.MeasureString(this.message);
      Rectangle position = new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - Math.Max(200, (int) vector2.X + 20) / 2, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - 50, Math.Max(200, (int) vector2.X + 40), (int) vector2.Y + 50 + (this.useButton ? 50 : 0));
      this.windowLayer = new CaptionWindowLayer(position, this.captionText);
      Point point = new Point(Math.Max(80, (int) vector2.X + 10), 30);
      this.okButton = new ButtonLayer(new Rectangle(position.X + position.Width / 2 - point.X / 2, position.Y + position.Height - 50, point.X, point.Y), !string.IsNullOrEmpty(this.buttonText) ? this.buttonText : "Ok");
    }

    public override void Render()
    {
      this.windowLayer.Render();
      if (this.useButton)
        this.okButton.Render();
      Engine.SpriteBatch.Begin();
      Vector2 position = new Vector2((float) (this.windowLayer.PositionAndSize.X + this.windowLayer.PositionAndSize.Width / 2) - Engine.Font.MeasureString(this.message).X / 2f, (float) (this.windowLayer.PositionAndSize.Y + 30));
      Engine.SpriteBatch.DrawString(Engine.Font, this.message, position, Color.White);
      Engine.SpriteBatch.End();
    }

    public override void UpdateInput()
    {
      if (!this.useButton)
        return;
      this.okButton.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick() || !this.okButton.Contains(this.Mouse.Position))
        return;
      this.CloseScreen();
      if (this.callback == null)
        return;
      this.callback();
    }
  }
}
