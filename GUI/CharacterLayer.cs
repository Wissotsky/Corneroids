// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.CharacterLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.GUI
{
  public class CharacterLayer : ItemLayer
  {
    private WearableItem highlightedItem;
    private FieldLayer fieldLayer;
    private WearableInventoryLayer headItemLayer;
    private WearableInventoryLayer shouldersItemLayer;
    private WearableInventoryLayer chestItemLayer;
    private WearableInventoryLayer legsItemLayer;
    private WearableInventoryLayer feetItemLayer;
    private WearableInventoryLayer handsItemLayer;
    private WearableInventoryLayer batteryItemLayer;
    private Character character;
    private Suit suit;
    private int healthRating;
    private int armorRating;
    private int rechargeRate;
    private Dictionary<WearableInventoryLayer, WearableItem.Slot> slotItemMappings;
    private Texture2D characterBackgroundTexture;
    private Texture2D highlightTexture;

    public CharacterLayer(Point position, Character character, Suit suit)
      : base(new Rectangle(position.X, position.Y, 240, 350))
    {
      this.character = character;
      this.suit = suit;
      this.EvaluateCharacterVariables();
      Point point1 = new Point(240, 288);
      this.fieldLayer = new FieldLayer(new Rectangle(position.X, position.Y, point1.X, point1.Y));
      this.characterBackgroundTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/characterBackground");
      this.highlightTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/square");
      WearableInventoryLayer wearableInventoryLayer1 = new WearableInventoryLayer(new Inventory(1U, 1U, (Player) null), Point.Zero, WearableItem.Slot.Head, suit);
      wearableInventoryLayer1.Name = "head";
      this.headItemLayer = wearableInventoryLayer1;
      WearableInventoryLayer wearableInventoryLayer2 = new WearableInventoryLayer(new Inventory(1U, 1U, (Player) null), Point.Zero, WearableItem.Slot.Shoulders, suit);
      wearableInventoryLayer2.Name = "shoulder";
      this.shouldersItemLayer = wearableInventoryLayer2;
      WearableInventoryLayer wearableInventoryLayer3 = new WearableInventoryLayer(new Inventory(1U, 1U, (Player) null), Point.Zero, WearableItem.Slot.Chest, suit);
      wearableInventoryLayer3.Name = "chest";
      this.chestItemLayer = wearableInventoryLayer3;
      WearableInventoryLayer wearableInventoryLayer4 = new WearableInventoryLayer(new Inventory(1U, 1U, (Player) null), Point.Zero, WearableItem.Slot.Legs, suit);
      wearableInventoryLayer4.Name = "legs";
      this.legsItemLayer = wearableInventoryLayer4;
      WearableInventoryLayer wearableInventoryLayer5 = new WearableInventoryLayer(new Inventory(1U, 1U, (Player) null), Point.Zero, WearableItem.Slot.Feet, suit);
      wearableInventoryLayer5.Name = "feet";
      this.feetItemLayer = wearableInventoryLayer5;
      WearableInventoryLayer wearableInventoryLayer6 = new WearableInventoryLayer(new Inventory(1U, 1U, (Player) null), Point.Zero, WearableItem.Slot.Hands, suit);
      wearableInventoryLayer6.Name = "hands";
      this.handsItemLayer = wearableInventoryLayer6;
      WearableInventoryLayer wearableInventoryLayer7 = new WearableInventoryLayer(new Inventory(1U, 1U, (Player) null), Point.Zero, WearableItem.Slot.PowerSource, suit);
      wearableInventoryLayer7.Name = "battery";
      this.batteryItemLayer = wearableInventoryLayer7;
      Point point2 = new Point(68, 16);
      Point point3 = new Point(4, 48);
      Point point4 = new Point(132, 48);
      Point point5 = new Point(68, 80);
      Point point6 = new Point(68, 144);
      Point point7 = new Point(68, 208);
      Point point8 = new Point(4, 112);
      Point point9 = new Point(164, 160);
      this.headItemLayer.Location = point2;
      this.shouldersItemLayer.Location = point4;
      this.chestItemLayer.Location = point5;
      this.legsItemLayer.Location = point6;
      this.feetItemLayer.Location = point7;
      this.handsItemLayer.Location = point8;
      this.batteryItemLayer.Location = point9;
      this.layers.Add((Layer) this.headItemLayer);
      this.layers.Add((Layer) this.shouldersItemLayer);
      this.layers.Add((Layer) this.chestItemLayer);
      this.layers.Add((Layer) this.legsItemLayer);
      this.layers.Add((Layer) this.feetItemLayer);
      this.layers.Add((Layer) this.handsItemLayer);
      this.layers.Add((Layer) this.batteryItemLayer);
      this.slotItemMappings = new Dictionary<WearableInventoryLayer, WearableItem.Slot>();
      this.slotItemMappings.Add(this.headItemLayer, WearableItem.Slot.Head);
      this.slotItemMappings.Add(this.shouldersItemLayer, WearableItem.Slot.Shoulders);
      this.slotItemMappings.Add(this.chestItemLayer, WearableItem.Slot.Chest);
      this.slotItemMappings.Add(this.legsItemLayer, WearableItem.Slot.Legs);
      this.slotItemMappings.Add(this.handsItemLayer, WearableItem.Slot.Hands);
      this.slotItemMappings.Add(this.feetItemLayer, WearableItem.Slot.Feet);
      this.slotItemMappings.Add(this.batteryItemLayer, WearableItem.Slot.PowerSource);
    }

    public override int AddItem(Point location, Item item, int count)
    {
      WearableInventoryLayer clickedSlotLayer = this.GetClickedSlotLayer(location);
      if (clickedSlotLayer == null)
        return 0;
      int num = clickedSlotLayer.AddItem(location, item, count);
      this.EvaluateCharacterVariables();
      return num;
    }

    public void HighlightItemSlot(WearableItem item) => this.highlightedItem = item;

    public override bool IsValidIndex(Point location)
    {
      WearableInventoryLayer clickedSlotLayer = this.GetClickedSlotLayer(location);
      return clickedSlotLayer != null && clickedSlotLayer.IsValidIndex(location);
    }

    public override ItemSlot PeekItem(Point location)
    {
      return this.GetClickedSlotLayer(location)?.PeekItem(location);
    }

    public override ItemSlot PopItems(Point location, int count)
    {
      WearableInventoryLayer clickedSlotLayer = this.GetClickedSlotLayer(location);
      if (clickedSlotLayer == null)
        return (ItemSlot) null;
      ItemSlot itemSlot = clickedSlotLayer.PopItems(location, count);
      this.EvaluateCharacterVariables();
      return itemSlot;
    }

    public override void Render()
    {
      this.fieldLayer.Render();
      Engine.SpriteBatch.Begin();
      Engine.SpriteBatch.Draw(this.characterBackgroundTexture, new Rectangle(this.Location.X, this.Location.Y, 240, 288), Color.White);
      Engine.SpriteBatch.End();
      foreach (Layer layer in this.layers)
        layer.Render();
      Vector2 position = new Vector2(this.Position.X, (float) ((double) this.Position.Y + (double) this.Size.Y - 50.0));
      Engine.SpriteBatch.Begin();
      Engine.SpriteBatch.DrawString(Engine.Font, "Health: " + (object) this.healthRating, position, Color.White);
      Engine.SpriteBatch.DrawString(Engine.Font, "Armor: " + (object) this.armorRating, position + Vector2.UnitY * 20f, Color.White);
      Engine.SpriteBatch.DrawString(Engine.Font, "Power recharge: " + (object) this.rechargeRate, position + Vector2.UnitY * 40f, Color.White);
      Engine.SpriteBatch.End();
      this.RenderHighlightBox();
    }

    public override Point Location
    {
      set
      {
        base.Location = value;
        this.fieldLayer.Location = new Point(this.fieldLayer.Location.X + value.X, this.fieldLayer.Location.Y + value.Y);
      }
    }

    private void EvaluateCharacterVariables()
    {
      if (this.character == null || this.suit == null)
        return;
      this.healthRating = (int) this.character.MaximumHealth;
      this.armorRating = (int) this.character.ArmorRating.Rating;
      this.rechargeRate = (int) this.character.PowerRechargeRate;
    }

    private WearableInventoryLayer GetClickedSlotLayer(Point location)
    {
      foreach (WearableInventoryLayer key in this.slotItemMappings.Keys)
      {
        if (key.PositionAndSize.Contains(location))
          return key;
      }
      return (WearableInventoryLayer) null;
    }

    private void RenderHighlightBox()
    {
      if (!((Item) this.highlightedItem != (Item) null) || this.highlightTexture == null)
        return;
      Engine.SpriteBatch.Begin();
      foreach (KeyValuePair<WearableInventoryLayer, WearableItem.Slot> slotItemMapping in this.slotItemMappings)
      {
        if (slotItemMapping.Value == this.highlightedItem.ItemSlot)
        {
          Rectangle destinationRectangle = slotItemMapping.Key.PositionAndSize;
          destinationRectangle = new Rectangle(destinationRectangle.X, destinationRectangle.Y, 64, 64);
          Engine.SpriteBatch.Draw(this.highlightTexture, destinationRectangle, Color.White);
        }
      }
      Engine.SpriteBatch.End();
    }
  }
}
