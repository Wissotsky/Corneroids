// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ReadInputScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class ReadInputScreen : MenuScreen
  {
    private ReadInputScreen.ReadingFinished finishedFunction;
    private TextBoxLayer textbox;
    private string readText;
    private Texture2D inputTexture;

    public ReadInputScreen(TextBoxLayer layer, ReadInputScreen.ReadingFinished finishedFunction)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      if (layer == null)
        throw new ArgumentNullException("Textbox layer cannot be null");
      this.finishedFunction = finishedFunction;
      this.readText = string.Empty;
      this.textbox = layer;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.inputTexture == null || this.inputTexture.IsDisposed)
        return;
      this.inputTexture.Dispose();
    }

    public override void Load()
    {
      this.inputTexture = this.CreateInputTexture();
      this.InputManager.InputReader.Begin();
      if (this.textbox == null || this.textbox.TextLines.Count <= 0)
        return;
      this.readText = this.textbox.TextLines[0];
      this.InputManager.InputReader.ReadString = this.readText;
    }

    public override void Render()
    {
      if (this.inputTexture == null)
        return;
      Vector2 vector2_1 = Engine.Font.MeasureString("B");
      Vector2 vector2_2 = new Vector2((float) this.textbox.PositionAndSize.X + Math.Min(Engine.Font.MeasureString(this.readText).X, (float) this.textbox.PositionAndSize.Width), (float) this.textbox.PositionAndSize.Y);
      Engine.SpriteBatch.Begin();
      Engine.SpriteBatch.Draw(this.inputTexture, new Rectangle((int) vector2_2.X, (int) vector2_2.Y, (int) vector2_1.X, (int) vector2_1.Y - 2), Color.White);
      Engine.SpriteBatch.End();
    }

    public override void Update()
    {
      if (this.textbox == null || this.InputManager.InputReader.ReadString.Equals(this.readText))
        return;
      this.readText = this.InputManager.InputReader.ReadString;
      this.textbox.Text = this.readText;
      this.InputManager.InputReader.ReadString = this.textbox.Text;
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (this.Keyboard.KeyPressed(Keys.Enter))
      {
        if (this.finishedFunction != null)
          this.finishedFunction();
        this.InputManager.InputReader.End();
        this.CloseScreen();
      }
      if (!this.Mouse.LeftClick() && !this.Mouse.RightClick() || this.textbox == null || this.textbox.GetClickedLayer(this.Mouse.Position) == this.textbox)
        return;
      this.CloseScreen();
    }

    private Texture2D CreateInputTexture()
    {
      try
      {
        Texture2D inputTexture = new Texture2D(Engine.GraphicsDevice, 1, 1);
        Color white = Color.White;
        inputTexture.SetData<Color>(new Color[1]{ white });
        return inputTexture;
      }
      catch
      {
        return (Texture2D) null;
      }
    }

    public delegate void ReadingFinished();
  }
}
