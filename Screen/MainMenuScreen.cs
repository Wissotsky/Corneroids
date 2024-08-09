// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.MainMenuScreen
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
  public class MainMenuScreen : MenuScreen
  {
    private const string sCopyrights = "Copyright 2012 Mikko Pulkki";
    private const string sVersion = "Corneroids Alpha Release v1.2.0";
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonExit;
    private ButtonLayer buttonOptions;
    private ButtonLayer buttonSingleplayer;
    private ButtonLayer buttonMultiplayer;
    private Camera camera;
    private SpaceDustManager dustManager;
    private Position3 positionForDust;
    private SpaceModel spaceModel;

    public MainMenuScreen()
      : base(GameScreen.Type.Fullscreen, false, false, false)
    {
      this.positionForDust = Position3.Zero;
    }

    public override void Dispose()
    {
      base.Dispose();
      this.camera.Dispose();
      this.dustManager.Dispose();
      this.spaceModel.Dispose();
      Engine.SettingsManager.ResolutionChangedEvent -= new Action<Point>(((MenuScreen) this).ResolutionChanged);
    }

    public override void Load()
    {
      base.Load();
      this.camera = new Camera();
      this.spaceModel = new SpaceModel();
      this.dustManager = new SpaceDustManager();
      this.dustManager.InitializeDust(512);
      this.camera.Position = new Position3(Vector3.One * 128f);
      this.CreateLayers();
      Engine.SettingsManager.ResolutionChangedEvent += new Action<Point>(((MenuScreen) this).ResolutionChanged);
    }

    public override void Render()
    {
      this.spaceModel.Render((IRenderCamera) this.camera);
      this.dustManager.Render((IRenderCamera) this.camera);
      this.backgroundLayer.Render();
      this.buttonSingleplayer.Render();
      this.buttonMultiplayer.Render();
      this.buttonExit.Render();
      this.buttonOptions.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Copyright 2012 Mikko Pulkki", Vector2.One * 10f, Color.White);
      spriteBatch.DrawString(Engine.Font, "Corneroids Alpha Release v1.2.0", Vector2.One * 10f + Vector2.UnitY * 20f, Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonSingleplayer.UpdateInput(this.Mouse);
      this.buttonMultiplayer.UpdateInput(this.Mouse);
      this.buttonOptions.UpdateInput(this.Mouse);
      this.buttonExit.UpdateInput(this.Mouse);
      if (this.Mouse.LeftClick())
      {
        if (this.buttonExit.Contains(this.Mouse.Position))
          this.ConfirmExitGame();
        else if (this.buttonOptions.Contains(this.Mouse.Position))
          Engine.LoadNewScreen((GameScreen) new OptionsScreen());
        else if (this.buttonSingleplayer.Contains(this.Mouse.Position))
          Engine.LoadNewScreen((GameScreen) new ChooseWorldScreen((Action<World>) (world =>
          {
            Engine.ExitToMainMenu();
            Engine.LoadNewScreen((GameScreen) new AdventureScreenSP());
          })));
        else if (this.buttonMultiplayer.Contains(this.Mouse.Position))
          Engine.LoadNewScreen((GameScreen) new MultiplayerScreen());
      }
      if (!this.Keyboard.KeyPressed(Keys.S))
        return;
      WorldInfo[] savedWorlds = WorldIOManager.Instance.GetSavedWorlds(Engine.WorldVersion);
      if (savedWorlds.Length <= 0)
        return;
      World newLocalWorld = Engine.CreateNewLocalWorld(savedWorlds[3]);
      Engine.PlayWorld(newLocalWorld);
      if (!newLocalWorld.Load())
        return;
      Engine.LoadNewScreen((GameScreen) new AdventureScreenServer(2, 17100, "Testiservu", (string) null));
    }

    public override void Update()
    {
      base.Update();
      this.positionForDust += Vector3.UnitZ * 0.1f;
      this.dustManager.Update(this.camera.Position);
      this.camera.Update(Engine.FrameCounter.DeltaTime);
      this.camera.Move(Math.Max(Engine.FrameCounter.DeltaTime, 1f / 1000f) * 10f);
    }

    private void ConfirmExitGame()
    {
      Engine.LoadNewScreen((GameScreen) new ConfirmScreen((Action<bool>) (exit =>
      {
        if (!exit)
          return;
        Engine.ExitGame();
      }), "Exit Corneroids?"));
    }

    private void CreateLayers()
    {
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(300, 225), "Corneroids");
      this.buttonSingleplayer = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 30, (int) this.backgroundLayer.Size.X - 40, 25), "Singleplayer");
      this.buttonMultiplayer = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 70, (int) this.backgroundLayer.Size.X - 40, 25), "Multiplayer");
      this.buttonOptions = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 110, (int) this.backgroundLayer.Size.X - 40, 25), "Options");
      this.buttonExit = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 45, (int) this.backgroundLayer.Size.X - 40, 25), "Exit game");
    }

    protected override void ResolutionChanged(Point resolution) => this.CreateLayers();
  }
}
