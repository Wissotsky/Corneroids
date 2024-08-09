// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.BeaconScreen
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
  public class BeaconScreen : MenuScreen
  {
    private const string sName = "Beacon name:";
    private Action<string> callback;
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonOk;
    private ButtonLayer buttonCancel;
    private TextBoxLayer textBoxLayer;

    public BeaconScreen(Action<string> nameCallback)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.callback = nameCallback;
    }

    public override void Load()
    {
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(500, 150), "Beacon");
      this.textBoxLayer = new TextBoxLayer(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 35, (int) this.backgroundLayer.Size.X - 240, 30), 20U, 1);
      this.buttonOk = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 100, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 50, 90, 30), "Ok");
      this.buttonCancel = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 + 10, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 50, 90, 30), "Cancel");
    }

    public override void Render()
    {
      this.backgroundLayer.Render();
      this.textBoxLayer.Render();
      this.buttonOk.Render();
      this.buttonCancel.Render();
      Engine.SpriteBatch.Begin();
      Engine.SpriteBatch.DrawString(Engine.Font, "Beacon name:", this.backgroundLayer.Position + new Vector2(20f, 35f), Color.White);
      Engine.SpriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonCancel.UpdateInput(this.Mouse);
      this.buttonOk.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.textBoxLayer.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textBoxLayer, (ReadInputScreen.ReadingFinished) null));
      if (this.buttonOk.Contains(this.Mouse.Position))
      {
        if (this.callback != null)
          this.callback(this.textBoxLayer.Text);
        this.CloseScreen();
      }
      else
      {
        if (!this.buttonCancel.Contains(this.Mouse.Position))
          return;
        this.CloseScreen();
      }
    }
  }
}
