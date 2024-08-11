// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.OptionsScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class OptionsScreen : MenuScreen
  {
    private ButtonLayer buttonBack;
    private ButtonLayer buttonGameplay;
    private ButtonLayer buttonGraphics;
    private ButtonLayer buttonControls;
    private ButtonLayer buttonNetwork;
    private ButtonLayer buttonSounds;
    private CaptionWindowLayer backgroudLayer;

    public OptionsScreen()
      : base(GameScreen.Type.Popup, false, true, true)
    {
    }

    public override void Dispose()
    {
      Engine.SettingsManager.ResolutionChangedEvent -= new Action<Point>(ResolutionChanged);
    }

    public override void Load()
    {
      this.LoadLayers();
      Engine.SettingsManager.ResolutionChangedEvent += new Action<Point>(ResolutionChanged);
    }

    public override void Render()
    {
      this.backgroudLayer.Render();
      this.buttonBack.Render();
      this.buttonGameplay.Render();
      this.buttonGraphics.Render();
      this.buttonControls.Render();
      this.buttonNetwork.Render();
      this.buttonSounds.Render();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonBack.UpdateInput(this.Mouse);
      this.buttonGameplay.UpdateInput(this.Mouse);
      this.buttonGraphics.UpdateInput(this.Mouse);
      this.buttonControls.UpdateInput(this.Mouse);
      this.buttonNetwork.UpdateInput(this.Mouse);
      this.buttonSounds.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonBack.PositionAndSize.Contains(this.Mouse.Position))
        this.CloseScreen();
      else if (this.buttonGameplay.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new GameplayOptionsScreen());
      else if (this.buttonGraphics.PositionAndSize.Contains(this.Mouse.Position))
      {
        Engine.LoadNewScreen((GameScreen) new GraphicsOptionScreen());
      }
      else
      {
        if (!this.buttonControls.Contains(this.Mouse.Position))
          return;
        Engine.LoadNewScreen((GameScreen) new ControlsOptionsScreen());
      }
    }

    private void LoadLayers()
    {
      this.backgroudLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(300, 300), "Options");
      this.buttonBack = new ButtonLayer(new Rectangle(this.backgroudLayer.Location.X + (int) this.backgroudLayer.Size.X / 2 - 130, this.backgroudLayer.Location.Y + (int) this.backgroudLayer.Size.Y - 40, 260, 25), "Back");
      this.buttonGameplay = new ButtonLayer(new Rectangle(this.backgroudLayer.Location.X + 20, this.backgroudLayer.Location.Y + 30, 260, 25), "Gameplay");
      this.buttonGraphics = new ButtonLayer(new Rectangle(this.backgroudLayer.Location.X + 20, this.backgroudLayer.Location.Y + 70, 260, 25), "Graphics");
      this.buttonControls = new ButtonLayer(new Rectangle(this.backgroudLayer.Location.X + 20, this.backgroudLayer.Location.Y + 110, 260, 25), "Controls");
      this.buttonNetwork = new ButtonLayer(new Rectangle(this.backgroudLayer.Location.X + 20, this.backgroudLayer.Location.Y + 150, 260, 25), "Network");
      this.buttonSounds = new ButtonLayer(new Rectangle(this.backgroudLayer.Location.X + 20, this.backgroudLayer.Location.Y + 190, 260, 25), "Sounds");
    }

    protected override void ResolutionChanged(Point resolution) => this.LoadLayers();
  }
}
