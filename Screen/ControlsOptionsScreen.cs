// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ControlsOptionsScreen
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
  public class ControlsOptionsScreen : MenuScreen
  {
    private const string KEYBINDINGS = "Key bindings:";
    private const string SENSITIVITY = "Mouse sensitivity:";
    private readonly ControlsManager controls;
    private CaptionWindowLayer backgroundLayer;
    private FieldLayer bindsBackground;
    private ButtonLayer buttonBack;
    private HorizontalScrollbarLayer scrollbarSensitivity;
    private FieldLayer sensitivityBg;
    private string sensitivityString;

    public ControlsOptionsScreen()
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.controls = Engine.SettingsManager.ControlsManager;
      this.sensitivityString = this.controls.MouseSensitivity.ToString();
    }

    public override void Dispose()
    {
      base.Dispose();
      this.scrollbarSensitivity.ScrollbarMovedEvent -= new System.Action(this.SensitivityScroll);
    }

    public override void Load()
    {
      base.Load();
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(500, 605), "Controls");
      this.bindsBackground = new FieldLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 150, (int) this.backgroundLayer.Size.X - 40, (int) this.backgroundLayer.Size.Y - 150 - 60));
      this.buttonBack = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 80, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 45, 160, 25), "Back");
      this.scrollbarSensitivity = new HorizontalScrollbarLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 70, 300, 30), 1000);
      this.sensitivityBg = new FieldLayer(new Rectangle(this.scrollbarSensitivity.Location.X + (int) this.scrollbarSensitivity.Size.X + 30, this.scrollbarSensitivity.Location.Y, 60, 30));
      this.scrollbarSensitivity.ScrollbarMovedEvent += new System.Action(this.SensitivityScroll);
      this.scrollbarSensitivity.ScrollPosition = (int) ((double) this.controls.MouseSensitivity / 3.0 * 1000.0);
    }

    public override void Render()
    {
      base.Render();
      this.backgroundLayer.Render();
      this.bindsBackground.Render();
      this.buttonBack.Render();
      this.scrollbarSensitivity.Render();
      this.sensitivityBg.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Mouse sensitivity:", this.backgroundLayer.Position + new Vector2(20f, 40f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Key bindings:", this.bindsBackground.Position - Vector2.UnitY * 25f, Color.White);
      spriteBatch.DrawString(Engine.Font, this.sensitivityString, this.sensitivityBg.Position + new Vector2(5f, 3f), Color.White);
      for (int index = 0; index < this.controls.AllSpecialKeys.Length; ++index)
      {
        ControlsManager.SpecialKey key = (ControlsManager.SpecialKey) index;
        spriteBatch.DrawString(Engine.Font, this.controls.ControlStringPresentation(key), this.bindsBackground.Position + new Vector2(10f, (float) (10 + index * 25)), Color.White);
        spriteBatch.DrawString(Engine.Font, this.controls.KeyStringPresentation(this.controls.GetSpecialKey(key)), this.bindsBackground.Position + new Vector2(200f, (float) (10 + index * 25)), Color.White);
      }
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonBack.UpdateInput(this.Mouse);
      this.scrollbarSensitivity.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonBack.Contains(this.Mouse.Position))
      {
        this.CloseScreen();
      }
      else
      {
        if (!this.bindsBackground.Contains(this.Mouse.Position))
          return;
        int num = (this.Mouse.Position.Y - this.bindsBackground.Location.Y - 10) / 25;
        if (num < 0 || num >= this.controls.AllSpecialKeys.Length)
          return;
        ControlsManager.SpecialKey keyEnum = (ControlsManager.SpecialKey) num;
        Engine.LoadNewScreen((GameScreen) new AskKeyScreen((Action<Keys>) (k => this.controls.SetSpecialKey(keyEnum, k))));
      }
    }

    private void SensitivityScroll()
    {
      this.controls.MouseSensitivity = MathHelper.Clamp(this.scrollbarSensitivity.ScrollPositionPercentage, 0.0f, 1f) * 3f;
      this.sensitivityString = this.controls.MouseSensitivity.ToString();
    }
  }
}
