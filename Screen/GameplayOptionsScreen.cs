// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.GameplayOptionsScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.Screen
{
  public class GameplayOptionsScreen : MenuScreen
  {
    private const int cWindowWidth = 500;
    private const int cWindowHeight = 420;
    private const string sGamma = "Gamma";
    private const string sTargetSquares = "Draw target squares";
    private const string sViewBobbing = "View bobbing";
    private const string sCameraStyle = "Fixed camera when aboard";
    private const string sShowHelp = "Show hints";
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonBack;
    private CheckboxLayer checkboxCameraStyle;
    private CheckboxLayer checkboxShowHelp;
    private CheckboxLayer checkboxTargetSquares;
    private CheckboxLayer checkboxViewBobbing;
    private FieldLayer fieldLayerGammaBG;
    private int gammaValue;
    private HorizontalScrollbarLayer scrollbarGamma;

    public GameplayOptionsScreen()
      : base(GameScreen.Type.Popup, false, true, false)
    {
    }

    public override void Load()
    {
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(500, 420), "Gameplay");
      this.buttonBack = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 80, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 45, 160, 25), "Back");
      this.checkboxTargetSquares = new CheckboxLayer(new Rectangle(this.backgroundLayer.Location.X + 40, this.backgroundLayer.Location.Y + 50, 30, 30));
      this.checkboxViewBobbing = new CheckboxLayer(new Rectangle(this.backgroundLayer.Location.X + 40, this.backgroundLayer.Location.Y + 50 + 60, 30, 30));
      this.checkboxCameraStyle = new CheckboxLayer(new Rectangle(this.backgroundLayer.Location.X + 40, this.backgroundLayer.Location.Y + 50 + 60 + 60, 30, 30));
      this.checkboxShowHelp = new CheckboxLayer(new Rectangle(this.backgroundLayer.Location.X + 40, this.backgroundLayer.Location.Y + 50 + 60 + 60 + 60, 30, 30));
      this.scrollbarGamma = new HorizontalScrollbarLayer(new Rectangle(this.backgroundLayer.Location.X + 40 + 90, this.backgroundLayer.Location.Y + 50 + 60 + 60 + 60 + 60, 250, 30), 1000);
      this.fieldLayerGammaBG = new FieldLayer(new Rectangle(this.scrollbarGamma.Location.X + (int) this.scrollbarGamma.Size.X + 20, this.scrollbarGamma.Location.Y, 70, 30));
      this.checkboxTargetSquares.Checked = Engine.SettingsManager.DrawTargetSquares;
      this.checkboxViewBobbing.Checked = Engine.SettingsManager.ViewBobbing;
      this.checkboxCameraStyle.Checked = Engine.SettingsManager.FixedCameraWhenAboard;
      this.checkboxShowHelp.Checked = Engine.SettingsManager.ShowHelpTexts;
      this.scrollbarGamma.ScrollbarMovedEvent += new System.Action(this.GammaScrollbarScrolled);
      this.gammaValue = Engine.SettingsManager.Gamma;
      this.scrollbarGamma.ScrollPositionPercentage = (float) this.gammaValue / 50f;
    }

    public override void Render()
    {
      this.backgroundLayer.Render();
      this.buttonBack.Render();
      this.checkboxTargetSquares.Render();
      this.checkboxViewBobbing.Render();
      this.checkboxCameraStyle.Render();
      this.checkboxShowHelp.Render();
      this.scrollbarGamma.Render();
      this.fieldLayerGammaBG.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Draw target squares", this.checkboxTargetSquares.Position + Vector2.UnitX * 50f, Color.White);
      spriteBatch.DrawString(Engine.Font, "View bobbing", this.checkboxViewBobbing.Position + Vector2.UnitX * 50f, Color.White);
      spriteBatch.DrawString(Engine.Font, "Fixed camera when aboard", this.checkboxCameraStyle.Position + Vector2.UnitX * 50f, Color.White);
      spriteBatch.DrawString(Engine.Font, "Show hints", this.checkboxShowHelp.Position + Vector2.UnitX * 50f, Color.White);
      spriteBatch.DrawString(Engine.Font, "Gamma", this.scrollbarGamma.Position - Vector2.UnitX * 95f, Color.White);
      spriteBatch.DrawString(Engine.Font, this.gammaValue.ToString(), this.fieldLayerGammaBG.Position + new Vector2(5f, 2f), Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonBack.UpdateInput(this.Mouse);
      this.scrollbarGamma.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonBack.Contains(this.Mouse.Position))
      {
        Engine.SettingsManager.SetGamma(this.gammaValue);
        Engine.SettingsManager.ApplyChanges();
        this.CloseScreen();
      }
      else if (this.checkboxTargetSquares.Contains(this.Mouse.Position))
      {
        this.checkboxTargetSquares.Checked = !this.checkboxTargetSquares.Checked;
        Engine.SettingsManager.DrawTargetSquares = this.checkboxTargetSquares.Checked;
      }
      else if (this.checkboxViewBobbing.Contains(this.Mouse.Position))
      {
        this.checkboxViewBobbing.Checked = !this.checkboxViewBobbing.Checked;
        Engine.SettingsManager.ViewBobbing = this.checkboxViewBobbing.Checked;
      }
      else if (this.checkboxCameraStyle.Contains(this.Mouse.Position))
      {
        this.checkboxCameraStyle.Checked = !this.checkboxCameraStyle.Checked;
        Engine.SettingsManager.SetCameraFixedWhenAboard(this.checkboxCameraStyle.Checked);
        Engine.SettingsManager.ApplyChanges();
      }
      else
      {
        if (!this.checkboxShowHelp.Contains(this.Mouse.Position))
          return;
        this.checkboxShowHelp.Checked = !this.checkboxShowHelp.Checked;
        Engine.SettingsManager.ShowHelpTexts = this.checkboxShowHelp.Checked;
      }
    }

    private void GammaScrollbarScrolled()
    {
      this.gammaValue = (int) ((double) this.scrollbarGamma.ScrollPositionPercentage * 50.0);
    }
  }
}
