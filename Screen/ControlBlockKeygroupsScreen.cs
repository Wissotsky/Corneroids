// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ControlBlockKeygroupsScreen
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
  public class ControlBlockKeygroupsScreen : MenuScreen
  {
    private ControlBlockKeygroupsLayer blockLayer;
    private ButtonLayer buttonBack;
    private ButtonLayer buttonSetCamera;
    private ControlBlock controlBlock;
    private System.Action dataChanged;
    private Player user;

    public ControlBlockKeygroupsScreen(ControlBlock controlBlock, Player user)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.controlBlock = controlBlock;
      this.user = user;
      this.dataChanged = (System.Action) (() => this.UpdateColorLayers());
      controlBlock.DataChanged += this.dataChanged;
    }

    protected override void Closing() => this.controlBlock.DataChanged -= this.dataChanged;

    public override void Load()
    {
      this.blockLayer = new ControlBlockKeygroupsLayer(this.controlBlock);
      this.buttonBack = new ButtonLayer(new Rectangle(0, 0, 100, 30), "Back");
      this.buttonSetCamera = new ButtonLayer(new Rectangle(0, 0, 150, 30), "Set camera");
      this.buttonSetCamera.Location = new Point(this.blockLayer.Location.X + (int) this.blockLayer.Size.X / 2 - 135, this.blockLayer.Location.Y + (int) this.blockLayer.Size.Y - 50);
      this.buttonBack.Location = new Point(this.buttonSetCamera.Location.X + 170, this.buttonSetCamera.Location.Y);
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonSetCamera.UpdateInput(this.Mouse);
      this.buttonBack.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      Layer clickedLayer = this.blockLayer.GetClickedLayer(this.Mouse.Position, typeof (ColorKeyGroupLayer), typeof (ButtonLayer));
      if (clickedLayer != null)
      {
        if (!(clickedLayer is ColorKeyGroupLayer))
          return;
        ColorKeyGroupLayer colorKeyLayer = clickedLayer as ColorKeyGroupLayer;
        Engine.LoadNewScreen((GameScreen) new AskKeyScreen((Action<Keys>) (clickedKey =>
        {
          colorKeyLayer.Key = clickedKey;
          this.controlBlock.SetKeyForColor(colorKeyLayer.Color, clickedKey, true);
        })));
      }
      else if (this.buttonBack.Contains(this.Mouse.Position))
      {
        this.CloseScreen();
      }
      else
      {
        if (!this.buttonSetCamera.Contains(this.Mouse.Position))
          return;
        this.CloseScreen();
        this.user.Astronaut.ActiveItem = (Item) new BindCameraToControlTool(this.controlBlock, this.user);
        this.user.Toolbar.SelectedIndex = (sbyte) -1;
      }
    }

    public override void Render()
    {
      this.blockLayer.Render();
      this.buttonBack.Render();
      this.buttonSetCamera.Render();
    }

    private void UpdateColorLayers() => this.blockLayer.UpdateLayerColors();
  }
}
