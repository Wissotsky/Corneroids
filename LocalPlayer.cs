// Decompiled with JetBrains decompiler
// Type: CornerSpace.LocalPlayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Screen;
using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class LocalPlayer : Player
  {
    private const string csToEdit = " to edit";
    private const string csToUse = " to use";
    private const string csSpeed = "Speed: ";
    private const string csUnits = " Units/s";
    private Texture2D boardedTexture;
    private int buttonStates;
    private Texture2D damageIndicatorTexture;
    private Position3? damageTaken;
    private int damageTakenCounterMS;
    private SpriteImage defaultCrosshair;
    private UnitBarLayer healthBarLayer;
    private UnitBarLayer powerBarLayer;
    private SpeedOMeter speedOMeter;
    private Rectangle speedOMeterPosition;
    private WindowLayer toolbarBackgroundLayer;

    public LocalPlayer()
    {
      Engine.SettingsManager.ResolutionChangedEvent += new Action<Point>(this.ResolutionChanged);
      this.toolbarBackgroundLayer = new WindowLayer(this.Toolbar.PositionAndSize);
      this.buttonStates = 0;
      this.speedOMeter = new SpeedOMeter();
      this.speedOMeterPosition = this.EvaluateSpeedOMeterPosition();
      this.powerBarLayer = new UnitBarLayer(new Rectangle(0, 0, 230, 30), Color.Purple, 100U, "Power");
      this.healthBarLayer = new UnitBarLayer(new Rectangle(0, 0, 230, 30), Color.Red, 100U, "Health")
      {
        BlinkPercentages = new float?(0.2f),
        BlinkColor = Color.DarkRed
      };
      this.boardedTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/boarded");
      this.damageIndicatorTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/damageIndicator");
      this.RepositionLayers();
      this.defaultCrosshair = new SpriteImage(Engine.ContentManager.Load<Texture2D>("Textures/Sprites/Crosshairs/default"));
    }

    public override void DamageTaken(Position3 worldPosition)
    {
      this.damageTaken = new Position3?(worldPosition);
      this.damageTakenCounterMS = 400;
    }

    public override void Dispose()
    {
      base.Dispose();
      Engine.SettingsManager.ResolutionChangedEvent -= new Action<Point>(this.ResolutionChanged);
    }

    public override void RenderGUI()
    {
      if (this.Astronaut.MountedTo == null)
      {
        this.toolbarBackgroundLayer.Render();
        this.Toolbar.Render();
        if (this.Astronaut.BoardedEntity != null)
          this.DrawBoardedSprite();
        Block pickedBlock = this.PickedBlock;
        if (Engine.SettingsManager.ShowHelpTexts)
          this.DrawHelpTexts();
      }
      else
      {
        Vector3 speed = this.Astronaut.Speed + this.Astronaut.MountedTo.Speed;
        float num = SimpleMath.RoundTwoDecimals(speed.Length());
        Quaternion orientation = Quaternion.Concatenate(this.Astronaut.Orientation, this.Astronaut.CoordinateSpace);
        this.speedOMeter.Render(this.speedOMeterPosition, speed, orientation);
        Engine.SpriteBatch.Begin();
        Engine.SpriteBatch.DrawString(Engine.Font, "Speed: " + num.ToString() + " Units/s", this.powerBarLayer.Position + Vector2.UnitY * 40f, new Color((byte) 20, (byte) 110, (byte) 10, (byte) 125));
        Engine.SpriteBatch.End();
      }
      if (this.damageTaken.HasValue)
        this.CheckAndRenderDamageIndicators();
      this.healthBarLayer.Render();
      this.powerBarLayer.Render();
      this.DrawAstronautBuffs();
      this.RenderCrosshair();
    }

    public override void Update(
      float deltaTime,
      CharacterManager characterManager,
      SpaceEntityManager entityManager)
    {
      base.Update(deltaTime, characterManager, entityManager);
      Astronaut astronaut = this.Astronaut;
      characterManager.CheckPlayerEntityCollisions((Player) this, true);
      this.UpdateDamageTakenMarker();
      if (Engine.SettingsManager.ViewBobbing)
        this.UpdateCameraShaking();
      this.UpdateHealthAndPowerBars();
      this.LatestStateFrame = new StateFrame()
      {
        ActiveItemId = astronaut.ActiveItem != (Item) null ? astronaut.ActiveItem.ItemId : -1,
        ButtonsStates = this.buttonStates,
        Position = astronaut.Position,
        Orientation = astronaut.Orientation,
        Speed = astronaut.Speed,
        Tick = Engine.FrameCounter.Ticks
      };
      this.LatestMessageTicks = Engine.FrameCounter.Ticks;
      this.buttonStates = 0;
    }

    public override void UpdateInput(InputFrame input)
    {
      bool flag = false;
      float deltaTime = Engine.FrameCounter.DeltaTime;
      this.Astronaut.MaximumSpeed = 4f;
      Astronaut astronaut = this.Astronaut;
      Block pickedBlock = this.PickedBlock;
      this.buttonStates = new StateFrame(input).ButtonsStates;
      astronaut.PowerPreUpdate = astronaut.Power;
      ControlsManager controlsManager = Engine.SettingsManager.ControlsManager;
      astronaut.UsedCamera.Rotate((float) (-(double) input.TranslationX * (double) controlsManager.MouseSensitivity * 0.0099999997764825821), (float) (-(double) input.TranslationY * (double) controlsManager.MouseSensitivity * 0.0099999997764825821));
      astronaut.GetLookatVector();
      if (!astronaut.Alive)
        return;
      if (input.KeyClicked(ControlsManager.SpecialKey.Flashlight))
        this.Astronaut.Flashlight.Enabled = !this.Astronaut.Flashlight.Enabled;
      if (input.KeyDown(ControlsManager.SpecialKey.RollLeft))
        astronaut.Roll(2f * Engine.FrameCounter.DeltaTime);
      else if (input.KeyDown(ControlsManager.SpecialKey.RollRight))
        astronaut.Roll(-2f * Engine.FrameCounter.DeltaTime);
      if (astronaut.MountedTo != null && input.KeyClicked(ControlsManager.SpecialKey.Use) && astronaut.MountedTo != null)
      {
        astronaut.MountedTo.UnMount();
        flag = true;
      }
      if (astronaut.MountedTo == null)
      {
        if (input.KeyClicked(ControlsManager.SpecialKey.BeaconTool))
        {
          this.Astronaut.ActiveItem = (Item) new BeaconTool();
          this.Toolbar.SelectedIndex = (sbyte) -1;
        }
        if (input.KeyClicked(ControlsManager.SpecialKey.Inventory))
          this.OpenCharacterScreen();
        if (input.KeyClicked(ControlsManager.SpecialKey.Edit) && astronaut.MountedTo == null && pickedBlock != null && pickedBlock.CanBeEdited)
          pickedBlock.EditButtonClicked((Player) this);
        if (input.KeyClicked(ControlsManager.SpecialKey.Use) && astronaut.MountedTo == null && pickedBlock != null && pickedBlock.CanBeUsed && !flag)
          pickedBlock.UseButtonClicked((Player) this);
        if (input.KeyDown(ControlsManager.SpecialKey.Boost))
          this.ActivateBoost(deltaTime);
        else if (input.KeyDown(ControlsManager.SpecialKey.Forward))
          astronaut.ApplyForce(Vector3.Zero, astronaut.GetLookatVector() * 5f, deltaTime);
        if (input.KeyDown(ControlsManager.SpecialKey.Backward))
          astronaut.ApplyForce(Vector3.Zero, -astronaut.GetLookatVector() * 5f, deltaTime);
        if (input.KeyDown(ControlsManager.SpecialKey.StrafeLeft))
          astronaut.ApplyForce(Vector3.Zero, astronaut.GetStrafeVector() * -5f, deltaTime);
        if (input.KeyDown(ControlsManager.SpecialKey.StrafeRight))
          astronaut.ApplyForce(Vector3.Zero, astronaut.GetStrafeVector() * 5f, deltaTime);
        Vector3 vector3 = this.Astronaut.CameraMode == Camera.Mode.Space ? this.Astronaut.GetUpVector() : Vector3.Transform(Vector3.Up, this.Astronaut.CoordinateSpace);
        if (input.KeyDown(ControlsManager.SpecialKey.Up))
          astronaut.ApplyForce(Vector3.Zero, vector3 * 5f, deltaTime);
        else if (input.KeyDown(ControlsManager.SpecialKey.Down))
          astronaut.ApplyForce(Vector3.Zero, -vector3 * 5f, deltaTime);
        this.SetItemSynchronizationValue(Engine.FrameCounter.Ticks);
        this.UpdateActiveItem(input);
        if (input.RightClick)
        {
          astronaut.ActiveItem = (Item) null;
          this.Toolbar.SelectedIndex = (sbyte) -1;
        }
        this.UpdateToolbarItem(input);
      }
      if (astronaut.MountedTo == null)
        return;
      astronaut.MountedTo.ActiveButtons = (short) 0;
      astronaut.MountedTo.Update(input, astronaut);
    }

    private void CheckAndRenderDamageIndicators()
    {
      if (!this.damageTaken.HasValue)
        return;
      Vector3 vector3_1 = Vector3.Transform(this.damageTaken.Value - this.Astronaut.Position, this.Astronaut.ViewMatrix);
      Vector3 vector1 = vector3_1 != Vector3.Zero ? Vector3.Normalize(vector3_1) : Vector3.Up;
      if ((double) Vector3.Dot(vector1, -Vector3.UnitZ) >= 0.93000000715255737)
      {
        this.RenderDamageIndicators(true, true, true, true);
      }
      else
      {
        Vector3 vector3_2 = new Vector3(vector1.X, 0.0f, vector1.Z);
        Vector3 vector3_3 = new Vector3(0.0f, vector1.Y, vector1.Z);
        Vector3 vector2_1 = vector3_2 != Vector3.Zero ? Vector3.Normalize(vector3_2) : Vector3.UnitY;
        Vector3 vector2_2 = vector3_3 != Vector3.Zero ? Vector3.Normalize(vector3_3) : Vector3.UnitX;
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        if ((double) Vector3.Dot(-Vector3.UnitX, vector2_1) >= 0.25)
        {
          int num = left ? 1 : 0;
          left = true;
        }
        if ((double) Vector3.Dot(Vector3.UnitX, vector2_1) >= 0.25)
        {
          int num = right ? 1 : 0;
          right = true;
        }
        if ((double) Vector3.Dot(Vector3.UnitY, vector2_2) >= 0.25)
        {
          int num = up ? 1 : 0;
          up = true;
        }
        if ((double) Vector3.Dot(-Vector3.UnitY, vector2_2) >= 0.25)
        {
          int num = down ? 1 : 0;
          down = true;
        }
        this.RenderDamageIndicators(left, right, up, down);
      }
    }

    private void DrawAstronautBuffs()
    {
      List<Buff> buffs = this.Astronaut.Buffs;
      if (buffs == null || buffs.Count <= 0)
        return;
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
      Engine.SetPointSamplerStateForSpritebatch();
      for (int index = 0; index < buffs.Count; ++index)
      {
        Buff buff = buffs[index];
        Rectangle buffSpriteCoords = buff.BuffSpriteCoords;
        Rectangle destinationRectangle = new Rectangle(this.healthBarLayer.Location.X + index * 80, this.healthBarLayer.Location.Y - 100, 64, 64);
        spriteBatch.Draw(Engine.LoadedWorld.SpriteTextureAtlas.Texture, destinationRectangle, new Rectangle?(buffSpriteCoords), Color.White);
        spriteBatch.DrawString(Engine.Font, (buff.DurationMS / 1000).ToString() + "s", new Vector2((float) (this.healthBarLayer.Location.X + index * 80), (float) (this.healthBarLayer.Location.Y - 120)), Color.White);
      }
      spriteBatch.End();
    }

    private void DrawBoardedSprite()
    {
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
      Engine.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
      Rectangle destinationRectangle = new Rectangle(this.Toolbar.Location.X + (int) this.Toolbar.Size.X + 30, this.Toolbar.Location.Y, 64, 64);
      spriteBatch.Draw(this.boardedTexture, destinationRectangle, Color.White);
      spriteBatch.End();
    }

    private void DrawControlBlockHelpTexts()
    {
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      Astronaut astronaut = this.Astronaut;
      Block pickedBlock = this.PickedBlock;
      if (pickedBlock == null || astronaut.MountedTo != null || !pickedBlock.CanBeEdited && !pickedBlock.CanBeUsed)
        return;
      spriteBatch.Begin();
      Vector2 position = new Vector2((float) (Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 50), (float) (Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 + 50));
      if (pickedBlock.CanBeEdited)
      {
        spriteBatch.DrawString(Engine.Font, Engine.SettingsManager.ControlsManager.GetSpecialKey(ControlsManager.SpecialKey.Edit).ToString() + " to edit", position, Color.White);
        position += Vector2.UnitY * 20f;
      }
      if (pickedBlock.CanBeUsed)
        spriteBatch.DrawString(Engine.Font, Engine.SettingsManager.ControlsManager.GetSpecialKey(ControlsManager.SpecialKey.Use).ToString() + " to use", position, Color.White);
      spriteBatch.End();
    }

    private void DrawHelpTexts()
    {
      Astronaut astronaut = this.Astronaut;
      Block pickedBlock = this.PickedBlock;
      bool flag = false;
      if (pickedBlock != null && astronaut.MountedTo == null && (pickedBlock.CanBeEdited || pickedBlock.CanBeUsed) && !(astronaut.ActiveItem is BindTriggerToControlTool))
      {
        this.DrawControlBlockHelpTexts();
        flag = true;
      }
      if (flag || !(astronaut.ActiveItem != (Item) null))
        return;
      astronaut.ActiveItem.DrawHelpTexts();
    }

    private Rectangle EvaluateSpeedOMeterPosition()
    {
      return new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 160, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight - 160 - 100, 320, 320);
    }

    protected void OpenCharacterScreen()
    {
      CharacterScreen screen = new CharacterScreen(this, this.EnvironmentManager);
      Engine.LoadNewScreen((GameScreen) screen);
      this.OnCharacterScreenOpened(screen);
    }

    private void RenderCrosshair()
    {
      SpriteImage spriteImage1 = (SpriteImage) null;
      if (this.Astronaut.ActiveItem != (Item) null)
        spriteImage1 = this.Astronaut.ActiveItem.Crosshair;
      SpriteImage spriteImage2 = spriteImage1 ?? this.defaultCrosshair;
      if (spriteImage2 == null)
        return;
      Rectangle middlePosition = Layer.EvaluateMiddlePosition(32, 32);
      spriteImage2.Render(middlePosition);
    }

    private void RenderDamageIndicators(bool left, bool right, bool up, bool down)
    {
      if (!left && !right && !up && !down)
        return;
      Engine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
      Engine.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
      Point point = new Point(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2);
      if (left)
      {
        Rectangle destinationRectangle = new Rectangle(point.X - 100 - 32, point.Y - 64, 32, 128);
        Engine.SpriteBatch.Draw(this.damageIndicatorTexture, destinationRectangle, Color.White);
      }
      if (right)
      {
        Rectangle destinationRectangle = new Rectangle(point.X + 100, point.Y - 64, 32, 128);
        Engine.SpriteBatch.Draw(this.damageIndicatorTexture, destinationRectangle, new Rectangle?(), Color.White, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.0f);
      }
      if (up)
      {
        Rectangle destinationRectangle = new Rectangle(point.X + 64, point.Y - 100 - 32, 32, 128);
        Engine.SpriteBatch.Draw(this.damageIndicatorTexture, destinationRectangle, new Rectangle?(), Color.White, 1.57079637f, Vector2.Zero, SpriteEffects.None, 0.0f);
      }
      if (down)
      {
        Rectangle destinationRectangle = new Rectangle(point.X - 64, point.Y + 100 + 32, 32, 128);
        Engine.SpriteBatch.Draw(this.damageIndicatorTexture, destinationRectangle, new Rectangle?(), Color.White, 4.712389f, Vector2.Zero, SpriteEffects.None, 0.0f);
      }
      Engine.SpriteBatch.End();
    }

    private void RepositionLayers()
    {
      this.Toolbar.Reposition();
      this.toolbarBackgroundLayer.PositionAndSize = this.Toolbar.PositionAndSize;
      this.healthBarLayer.Location = new Point(this.Toolbar.Location.X, this.Toolbar.Location.Y - 50);
      this.powerBarLayer.Location = new Point(this.healthBarLayer.Location.X + 256 + 26, this.healthBarLayer.Location.Y);
    }

    private void ResolutionChanged(Point resolution)
    {
      this.RepositionLayers();
      this.speedOMeterPosition = this.EvaluateSpeedOMeterPosition();
    }

    protected void UpdateCameraShaking()
    {
      float num1 = this.Astronaut.Speed.Length();
      if ((double) num1 > 4.0)
      {
        float num2 = MathHelper.Lerp(0.0f, 0.02f, (float) (((double) num1 - 4.0) / 4.0));
        this.Astronaut.ViewMatrixModification = Matrix.CreateFromYawPitchRoll((float) this.Random.NextDouble() * num2, (float) this.Random.NextDouble() * num2, (float) this.Random.NextDouble() * num2);
      }
      else
        this.Astronaut.ViewMatrixModification = Matrix.Identity;
    }

    protected void UpdateDamageTakenMarker()
    {
      Astronaut astronaut = this.Astronaut;
      if (!this.damageTaken.HasValue)
        return;
      this.damageTakenCounterMS -= (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.damageTakenCounterMS <= 0)
        this.damageTaken = new Position3?();
      if (astronaut.MountedTo == null)
        return;
      astronaut.MountedTo.UnMount();
    }

    protected void UpdateHealthAndPowerBars()
    {
      this.healthBarLayer.RealValue = (uint) this.Astronaut.Health;
      this.healthBarLayer.MaxValue = (uint) this.Astronaut.MaximumHealth;
      this.powerBarLayer.RealValue = (uint) this.Astronaut.Power;
      this.powerBarLayer.MaxValue = (uint) this.Astronaut.MaximumPower;
    }
  }
}
