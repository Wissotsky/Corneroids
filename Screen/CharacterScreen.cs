// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.CharacterScreen
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
  public class CharacterScreen : ItemScreen
  {
    private CaptionWindowLayer characterBackgroundLayer;
    private CharacterLayer characterLayer;
    private DefaultItemInventoryLayer defaultCraftingBlockInventoryLayer;
    private DefaultItemInventoryLayer defaultDrillInventoryLayer;
    private DefaultItemInventoryLayer defaultSmelterInventoryLayer;
    private CaptionWindowLayer inventoryBackgroundLayer;
    private InventoryLayer inventoryLayer;
    private LocalPlayer player;
    private TrashInventoryLayer trashInventoryLayer;

    public CharacterScreen(LocalPlayer player, EnvironmentManager environmentManager)
      : base(GameScreen.Type.Popup, true, (Character) player.Astronaut, environmentManager)
    {
      this.player = player;
    }

    public override void Load()
    {
      Suit suit = this.player != null ? this.player.Astronaut.Suit : (Suit) null;
      this.inventoryLayer = new InventoryLayer(this.player.Inventory, Point.Zero);
      this.characterLayer = new CharacterLayer(Point.Zero, (Character) this.player.Astronaut, suit);
      this.characterBackgroundLayer = new CaptionWindowLayer(new Rectangle(0, 0, (int) this.characterLayer.Size.X + 64, (int) this.characterLayer.Size.Y + 64), "Character");
      this.inventoryBackgroundLayer = new CaptionWindowLayer(new Rectangle(0, 0, (int) this.inventoryLayer.Size.X + 64 + 72, (int) this.inventoryLayer.Size.Y + 64), "Inventory");
      this.defaultDrillInventoryLayer = new DefaultItemInventoryLayer(Point.Zero, (Item) Engine.LoadedWorld.Itemset.DefaultDrill);
      this.defaultCraftingBlockInventoryLayer = new DefaultItemInventoryLayer(Point.Zero, (Item) Engine.LoadedWorld.Itemset.DefaultCraftingTableBlock);
      this.defaultSmelterInventoryLayer = new DefaultItemInventoryLayer(Point.Zero, (Item) Engine.LoadedWorld.Itemset.DefaultSmelterBlock);
      this.trashInventoryLayer = new TrashInventoryLayer(Point.Zero);
      Rectangle middlePosition = Layer.EvaluateMiddlePosition((int) this.inventoryBackgroundLayer.Size.X + (int) this.characterBackgroundLayer.Size.X + 50, (int) Math.Max(this.inventoryBackgroundLayer.Size.Y, this.characterBackgroundLayer.Size.Y));
      this.characterBackgroundLayer.Location = new Point(middlePosition.X, middlePosition.Y);
      this.characterLayer.Location = new Point(middlePosition.X + 32, middlePosition.Y + 32);
      this.inventoryBackgroundLayer.Location = new Point(middlePosition.X + 50 + (int) this.characterBackgroundLayer.Size.X, middlePosition.Y);
      this.inventoryLayer.Location = new Point(middlePosition.X + 50 + 32 + (int) this.characterBackgroundLayer.Size.X, middlePosition.Y + 32);
      this.defaultDrillInventoryLayer.Location = new Point(this.inventoryLayer.Location.X + (int) this.inventoryLayer.Size.X + 16, this.inventoryLayer.Location.Y + (int) this.inventoryLayer.Size.Y - 64);
      this.defaultCraftingBlockInventoryLayer.Location = new Point(this.inventoryLayer.Location.X + (int) this.inventoryLayer.Size.X + 16, this.inventoryLayer.Location.Y + (int) this.inventoryLayer.Size.Y - 64 - 64);
      this.defaultSmelterInventoryLayer.Location = new Point(this.inventoryLayer.Location.X + (int) this.inventoryLayer.Size.X + 16, this.inventoryLayer.Location.Y + (int) this.inventoryLayer.Size.Y - 64 - 64 - 64);
      this.trashInventoryLayer.Location = new Point(this.defaultDrillInventoryLayer.Location.X, this.defaultDrillInventoryLayer.Location.Y - 96 - 64 - 64);
      this.AddItemLayer((ItemLayer) this.inventoryLayer);
      this.AddItemLayer((ItemLayer) this.player.Toolbar);
      this.AddItemLayer((ItemLayer) this.characterLayer);
      this.AddItemLayer((ItemLayer) this.defaultDrillInventoryLayer);
      this.AddItemLayer((ItemLayer) this.defaultCraftingBlockInventoryLayer);
      this.AddItemLayer((ItemLayer) this.defaultSmelterInventoryLayer);
      this.AddItemLayer((ItemLayer) this.trashInventoryLayer);
    }

    public override void Render()
    {
      this.characterBackgroundLayer.Render();
      this.characterLayer.Render();
      this.inventoryBackgroundLayer.Render();
      this.inventoryLayer.Render();
      this.defaultDrillInventoryLayer.Render();
      this.defaultCraftingBlockInventoryLayer.Render();
      this.defaultSmelterInventoryLayer.Render();
      this.trashInventoryLayer.Render();
      this.player.Toolbar.Render();
      base.Render();
    }

    public override void UpdateInput()
    {
      if (this.InputFrame.KeyClicked(Keys.Escape) || this.InputFrame.KeyClicked(ControlsManager.SpecialKey.Inventory))
        this.CloseScreen();
      base.UpdateInput();
    }

    public override void Update()
    {
      base.Update();
      if (this.ItemBeingDragged != (Item) null)
        this.characterLayer.HighlightItemSlot(this.ItemBeingDragged as WearableItem);
      else
        this.characterLayer.HighlightItemSlot((WearableItem) null);
    }

    protected override Inventory.Type GetInventoryTypeOfLayer(ItemLayer layer, Point location)
    {
      if (layer == this.inventoryLayer)
        return Inventory.Type.Inventory;
      if (layer == this.player.Toolbar)
        return Inventory.Type.Toolbar;
      if (layer == this.defaultDrillInventoryLayer)
        return Inventory.Type.DefaultDrill;
      if (layer == this.trashInventoryLayer)
        return Inventory.Type.Trash;
      if (layer == this.defaultSmelterInventoryLayer)
        return Inventory.Type.Smelter;
      if (layer == this.defaultCraftingBlockInventoryLayer)
        return Inventory.Type.Crafting;
      CharacterLayer characterLayer = this.characterLayer;
      return Inventory.Type.Inventory;
    }
  }
}
