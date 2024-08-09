// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.GraphicsOptionScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Screen
{
  public class GraphicsOptionScreen : MenuScreen
  {
    private const string sFieldOfView = "Field of view:";
    private const string sFullscreen = "Fullscreen:";
    private const string sLighting = "Lighting";
    private const string sResolution = "Resolution:";
    private const string sSSAO = "SSAO:";
    private const string sViewDistance = "View distance:";
    private const string sVSync = "V-Sync:";
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonCancel;
    private ButtonLayer buttonOk;
    private CheckboxLayer checkboxFullscreen;
    private CheckboxLayer checkboxLighting;
    private ComboboxLayer<SettingsManager.Quality> comboboxSSAO;
    private CheckboxLayer checkboxVSync;
    private ComboboxLayer<KeyValuePair<string, int>> comboboxViewDistance;
    private ComboboxLayer<GraphicsOptionScreen.SupportedResolution> comboboxResolution;
    private HorizontalScrollbarLayer scrollbarFov;
    private TextBoxLayer textboxFov;
    private float fov;

    public GraphicsOptionScreen()
      : base(GameScreen.Type.Popup, false, true, false)
    {
    }

    public override void Load()
    {
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(600, 450), "Graphics");
      this.comboboxResolution = new ComboboxLayer<GraphicsOptionScreen.SupportedResolution>(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 40, 350, 30), 30U);
      this.buttonOk = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 110, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 50, 100, 30), "Ok");
      this.buttonCancel = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 + 10, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 50, 100, 30), "Cancel");
      this.checkboxFullscreen = new CheckboxLayer(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 90, 30, 30));
      this.checkboxVSync = new CheckboxLayer(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 140, 30, 30));
      this.checkboxLighting = new CheckboxLayer(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 190, 30, 30));
      this.comboboxSSAO = new ComboboxLayer<SettingsManager.Quality>(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 240, 350, 30), 10U);
      this.comboboxViewDistance = new ComboboxLayer<KeyValuePair<string, int>>(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 290, 350, 30), 30U);
      this.scrollbarFov = new HorizontalScrollbarLayer(new Rectangle(this.backgroundLayer.Location.X + 200, this.backgroundLayer.Location.Y + 340, 200, 30), 500);
      this.textboxFov = new TextBoxLayer(new Rectangle(this.backgroundLayer.Location.X + 430, this.backgroundLayer.Location.Y + 340, 85, 30), 7U, 1);
      this.PopulateResolutionCombobox();
      this.PopulateViewDistanceCombobox();
      this.PopulateFovTextbox();
      this.PopulateSSAOCombobox();
      this.checkboxFullscreen.Checked = Engine.SettingsManager.Fullscreen;
      this.checkboxVSync.Checked = Engine.SettingsManager.VSync;
      this.comboboxSSAO.SelectedItem = Engine.SettingsManager.SSAO;
      this.checkboxLighting.Checked = Engine.SettingsManager.Lighting;
      this.comboboxViewDistance.SelectedItem = this.comboboxViewDistance.ItemsToChoose[(int) MathHelper.Clamp((float) ((Engine.SettingsManager.CurrentViewDistance - 1) / 50), 0.0f, 2f)];
      this.fov = MathHelper.ToDegrees(Engine.SettingsManager.CameraFoV);
      List<GraphicsOptionScreen.SupportedResolution> itemsToChoose = this.comboboxResolution.ItemsToChoose;
      Point resolution = Engine.SettingsManager.Resolution;
      GraphicsOptionScreen.SupportedResolution supportedResolution = itemsToChoose.Find((Predicate<GraphicsOptionScreen.SupportedResolution>) (r => r.Width == resolution.X && r.Height == resolution.Y));
      if (supportedResolution != null)
        this.comboboxResolution.SelectedItem = supportedResolution;
      else
        this.comboboxResolution.SelectedIndex = 0;
    }

    public override void Render()
    {
      this.backgroundLayer.Render();
      this.buttonCancel.Render();
      this.buttonOk.Render();
      this.checkboxFullscreen.Render();
      this.checkboxLighting.Render();
      this.comboboxSSAO.Render();
      this.checkboxVSync.Render();
      this.comboboxResolution.Render();
      this.comboboxViewDistance.Render();
      this.scrollbarFov.Render();
      this.textboxFov.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Resolution:", this.backgroundLayer.Position + new Vector2(30f, 40f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Fullscreen:", this.backgroundLayer.Position + new Vector2(30f, 90f), Color.White);
      spriteBatch.DrawString(Engine.Font, "V-Sync:", this.backgroundLayer.Position + new Vector2(30f, 140f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Lighting", this.backgroundLayer.Position + new Vector2(30f, 190f), Color.White);
      spriteBatch.DrawString(Engine.Font, "SSAO:", this.backgroundLayer.Position + new Vector2(30f, 240f), Color.White);
      spriteBatch.DrawString(Engine.Font, "View distance:", this.backgroundLayer.Position + new Vector2(30f, 290f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Field of view:", this.backgroundLayer.Position + new Vector2(30f, 340f), Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.scrollbarFov.UpdateInput(this.Mouse);
      this.buttonCancel.UpdateInput(this.Mouse);
      this.buttonOk.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonOk.PositionAndSize.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new ConfirmScreen((Action<bool>) (r =>
        {
          if (r)
            this.CheckAndSave();
          this.CloseScreen();
        }), "Save changes?"));
      if (this.buttonCancel.PositionAndSize.Contains(this.Mouse.Position))
        this.CloseScreen();
      if (this.comboboxResolution.PositionAndSize.Contains(this.Mouse.Position))
        this.comboboxResolution.Expand();
      else if (this.comboboxViewDistance.PositionAndSize.Contains(this.Mouse.Position))
        this.comboboxViewDistance.Expand();
      if (this.checkboxFullscreen.PositionAndSize.Contains(this.Mouse.Position))
        this.checkboxFullscreen.Checked = !this.checkboxFullscreen.Checked;
      else if (this.checkboxVSync.Contains(this.Mouse.Position))
        this.checkboxVSync.Checked = !this.checkboxVSync.Checked;
      else if (this.checkboxLighting.Contains(this.Mouse.Position))
      {
        this.checkboxLighting.Checked = !this.checkboxLighting.Checked;
      }
      else
      {
        if (!this.comboboxSSAO.Contains(this.Mouse.Position))
          return;
        this.comboboxSSAO.Expand();
      }
    }

    private void CheckAndSave()
    {
      try
      {
        bool flag = false;
        if (this.comboboxResolution.SelectedItem != null && (Engine.SettingsManager.Resolution.X != this.comboboxResolution.SelectedItem.Width || Engine.SettingsManager.Resolution.Y != this.comboboxResolution.SelectedItem.Height))
        {
          Engine.SettingsManager.SetResolution(this.comboboxResolution.SelectedItem.Width, this.comboboxResolution.SelectedItem.Height);
          flag = true;
        }
        if (Engine.SettingsManager.Fullscreen != this.checkboxFullscreen.Checked)
        {
          Engine.SettingsManager.SetFullScreen(this.checkboxFullscreen.Checked);
          flag = true;
        }
        if (Engine.SettingsManager.VSync != this.checkboxVSync.Checked)
        {
          Engine.SettingsManager.SetVSync(this.checkboxVSync.Checked);
          flag = true;
        }
        if (Engine.SettingsManager.SSAO != this.comboboxSSAO.SelectedItem)
        {
          Engine.SettingsManager.SSAO = this.comboboxSSAO.SelectedItem;
          flag = true;
        }
        if (this.comboboxViewDistance.SelectedItem.Value != Engine.SettingsManager.CurrentViewDistance)
        {
          Engine.SettingsManager.SetViewDistance(this.comboboxViewDistance.SelectedItem.Value);
          flag = true;
        }
        if ((double) this.fov != (double) MathHelper.ToDegrees(Engine.SettingsManager.CameraFoV))
        {
          Engine.SettingsManager.SetFieldOfView(MathHelper.ToRadians(this.fov));
          flag = true;
        }
        if (this.checkboxLighting.Checked != Engine.SettingsManager.Lighting)
        {
          Engine.SettingsManager.Lighting = this.checkboxLighting.Checked;
          flag = true;
        }
        if (!flag)
          return;
        Engine.SettingsManager.ApplyChanges();
      }
      catch
      {
        Engine.LoadNewScreen((GameScreen) new MessageScreen("Failed to save changes!", "Error", true));
        this.CloseScreen();
      }
    }

    private void PopulateFovTextbox()
    {
      float degrees = MathHelper.ToDegrees(Engine.SettingsManager.CameraFoV);
      this.textboxFov.Text = degrees.ToString();
      this.scrollbarFov.ScrollPositionPercentage = (float) (((double) degrees - 45.0) / 90.0);
      this.scrollbarFov.ScrollbarMovedEvent += (System.Action) (() =>
      {
        this.fov = (float) (45.0 + 90.0 * (double) this.scrollbarFov.ScrollPositionPercentage);
        this.textboxFov.Text = this.fov.ToString();
      });
    }

    private void PopulateResolutionCombobox()
    {
      if (this.comboboxResolution == null)
        return;
      List<DisplayMode> supportedResolutions = Engine.SettingsManager.GetSupportedResolutions();
      List<GraphicsOptionScreen.SupportedResolution> supportedResolutionList = new List<GraphicsOptionScreen.SupportedResolution>();
      foreach (DisplayMode displayMode in supportedResolutions)
      {
        GraphicsOptionScreen.SupportedResolution supportedResolution1 = new GraphicsOptionScreen.SupportedResolution()
        {
          Width = displayMode.Width,
          Height = displayMode.Height
        };
        bool flag = false;
        foreach (GraphicsOptionScreen.SupportedResolution supportedResolution2 in supportedResolutionList)
          flag = ((flag ? 1 : 0) | (supportedResolution2.Width != supportedResolution1.Width ? 0 : (supportedResolution2.Height == supportedResolution1.Height ? 1 : 0))) != 0;
        if (!flag)
          supportedResolutionList.Add(supportedResolution1);
      }
      if (supportedResolutionList.Count > 2)
        supportedResolutionList.Sort((Comparison<GraphicsOptionScreen.SupportedResolution>) ((r1, r2) => r1.Width * r1.Height - r2.Width * r2.Height));
      this.comboboxResolution.ItemsToChoose = supportedResolutionList;
    }

    private void PopulateSSAOCombobox()
    {
      if (this.comboboxSSAO == null)
        return;
      this.comboboxSSAO.ItemsToChoose = new List<SettingsManager.Quality>()
      {
        SettingsManager.Quality.Off,
        SettingsManager.Quality.Low,
        SettingsManager.Quality.High
      };
    }

    private void PopulateViewDistanceCombobox()
    {
      if (this.comboboxViewDistance == null)
        return;
      this.comboboxViewDistance.ItemsToChoose = new List<KeyValuePair<string, int>>()
      {
        new KeyValuePair<string, int>("Short", 50),
        new KeyValuePair<string, int>("Medium", 100),
        new KeyValuePair<string, int>("Far", 150)
      };
    }

    public class SupportedResolution
    {
      public int Width;
      public int Height;

      public override string ToString() => this.Width.ToString() + " x " + this.Height.ToString();
    }
  }
}
