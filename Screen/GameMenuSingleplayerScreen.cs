﻿// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.GameMenuSingleplayerScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class GameMenuSingleplayerScreen : MenuScreen
  {
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonBack;
    private ButtonLayer buttonExit;
    private ButtonLayer buttonOptions;
    private AdventureScreenSP singleplayerScreen;

    public GameMenuSingleplayerScreen(AdventureScreenSP singleplayerScreen)
      : base(GameScreen.Type.Popup, false, true, true)
    {
      this.singleplayerScreen = singleplayerScreen != null ? singleplayerScreen : throw new ArgumentNullException();
    }

    public override void Load() => this.CreateLayers();

    public override void Render()
    {
      this.backgroundLayer.Render();
      this.buttonOptions.Render();
      this.buttonExit.Render();
      this.buttonBack.Render();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonBack.UpdateInput(this.Mouse);
      this.buttonExit.UpdateInput(this.Mouse);
      this.buttonOptions.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonBack.PositionAndSize.Contains(this.Mouse.Position))
        this.CloseScreen();
      else if (this.buttonExit.PositionAndSize.Contains(this.Mouse.Position))
      {
        Engine.ConfirmBoxShow("Save game and exit to main menu?", (Action<bool>) (result =>
        {
          if (!result)
            return;
          this.singleplayerScreen.Save();
          Engine.LoadNewScreen((GameScreen) new SavingScreen((System.Action) (() =>
          {
            try
            {
              Engine.LoadedWorld.Save();
            }
            catch (Exception ex)
            {
              Engine.MessageBoxShow("Failed to save the world: " + ex.Message);
            }
            Engine.ExitToMainMenu();
          }), new SavingScreen.PollFunction(Engine.LoadedWorld.PollIsDatabaseWorkLeft)));
        }));
      }
      else
      {
        if (!this.buttonOptions.PositionAndSize.Contains(this.Mouse.Position))
          return;
        Engine.LoadNewScreen((GameScreen) new OptionsScreen());
      }
    }

    private void CreateLayers()
    {
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(300, 160), "Game menu");
      this.buttonOptions = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 30, (int) this.backgroundLayer.Size.X - 40, 20), "Options");
      this.buttonExit = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 65, (int) this.backgroundLayer.Size.X - 40, 20), "Save and exit");
      this.buttonBack = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 40, (int) this.backgroundLayer.Size.X - 40, 20), "Back to game");
    }

    protected override void ResolutionChanged(Point resolution) => this.CreateLayers();
  }
}
