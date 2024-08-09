// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ContainerBlockScreen
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
  public class ContainerBlockScreen : ItemScreen
  {
    private Inventory playerInventory;
    private Inventory containerInventory;
    private InventoryLayer playerInventoryLayer;
    private CaptionWindowLayer playerBgLayer;
    private InventoryLayer containerInventoryLayer;
    private CaptionWindowLayer containerBgLayer;
    private ItemToolbarLayer toolbarLayer;

    public ContainerBlockScreen(LocalPlayer player, ContainerBlock container)
      : base(GameScreen.Type.Popup, true, (Character) player.Astronaut, player.EnvironmentManager)
    {
      if (player == null || container == null)
        this.CloseScreen();
      this.playerInventory = player.Inventory;
      this.containerInventory = container.Inventory;
      this.toolbarLayer = player.Toolbar;
    }

    public override void Load()
    {
      base.Load();
      this.playerInventoryLayer = new InventoryLayer(this.playerInventory, Point.Zero);
      this.containerInventoryLayer = new InventoryLayer(this.containerInventory, Point.Zero);
      this.playerBgLayer = new CaptionWindowLayer(new Rectangle(0, 0, (int) this.playerInventoryLayer.Size.X + 64, (int) this.playerInventoryLayer.Size.Y + 64), "Player");
      this.containerBgLayer = new CaptionWindowLayer(new Rectangle(0, 0, (int) this.containerInventoryLayer.Size.X + 64, (int) this.containerInventoryLayer.Size.Y + 64), "Container");
      Rectangle middlePosition = Layer.EvaluateMiddlePosition((int) this.playerBgLayer.Size.X + (int) this.containerBgLayer.Size.X + 50, (int) Math.Max(this.playerBgLayer.Size.Y, this.containerBgLayer.Size.Y));
      this.playerBgLayer.Location = new Point(middlePosition.X, middlePosition.Y);
      this.playerInventoryLayer.Location = new Point(middlePosition.X + 32, middlePosition.Y + 32);
      this.containerBgLayer.Location = new Point(middlePosition.X + 50 + (int) this.playerBgLayer.Size.X, middlePosition.Y);
      this.containerInventoryLayer.Location = new Point(middlePosition.X + 50 + 32 + (int) this.playerBgLayer.Size.X, middlePosition.Y + 32);
      this.AddItemLayer((ItemLayer) this.playerInventoryLayer);
      this.AddItemLayer((ItemLayer) this.containerInventoryLayer);
      this.AddItemLayer((ItemLayer) this.toolbarLayer);
    }

    public override void Render()
    {
      this.playerBgLayer.Render();
      this.playerInventoryLayer.Render();
      this.containerBgLayer.Render();
      this.containerInventoryLayer.Render();
      base.Render();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (!this.Keyboard.KeyPressed(Keys.Escape) && !this.Keyboard.KeyPressed(Engine.SettingsManager.ControlsManager.GetSpecialKey(ControlsManager.SpecialKey.Use)))
        return;
      this.CloseScreen();
    }

    protected override Inventory.Type GetInventoryTypeOfLayer(ItemLayer layer, Point location)
    {
      Engine.MessageBoxShow("Feature not implemented yet");
      return Inventory.Type.Inventory;
    }
  }
}
