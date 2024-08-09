// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.PasswordScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class PasswordScreen : MenuScreen
  {
    private ButtonLayer buttonOk;
    private ButtonLayer buttonCancel;
    private CaptionWindowLayer bgLayer;
    private TextBoxLayer textBoxPassword;
    private Action<string> resultDelegate;

    public PasswordScreen(Action<string> result)
      : base(GameScreen.Type.Popup, true, false, false)
    {
      this.resultDelegate = result;
      if (result == null)
        throw new ArgumentNullException(result.ToString());
    }

    public override void Load()
    {
      this.bgLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(400, 120), "Password required");
      this.textBoxPassword = new TextBoxLayer(new Rectangle(this.bgLayer.Location.X + 20, this.bgLayer.Location.Y + 30, (int) this.bgLayer.Size.X - 40, 30), 20U, 1);
      this.buttonOk = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 - 110, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 45, 100, 25), "Ok");
      this.buttonCancel = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 + 10, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 45, 100, 25), "Cancel");
    }

    public override void Render()
    {
      this.bgLayer.Render();
      this.textBoxPassword.Render();
      this.buttonOk.Render();
      this.buttonCancel.Render();
    }

    public override void UpdateInput()
    {
      this.buttonOk.UpdateInput(this.Mouse);
      this.buttonCancel.UpdateInput(this.Mouse);
      if (this.Keyboard.KeyPressed(Keys.Escape))
      {
        this.resultDelegate((string) null);
        this.CloseScreen();
      }
      if (!this.InputFrame.LeftClick)
        return;
      if (this.buttonCancel.Contains(this.Mouse.Position))
      {
        this.resultDelegate((string) null);
        this.CloseScreen();
      }
      else if (this.textBoxPassword.Contains(this.Mouse.Position))
      {
        Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textBoxPassword, (ReadInputScreen.ReadingFinished) null));
      }
      else
      {
        if (!this.buttonOk.Contains(this.Mouse.Position))
          return;
        this.resultDelegate(this.textBoxPassword.Text ?? string.Empty);
        this.CloseScreen();
      }
    }
  }
}
