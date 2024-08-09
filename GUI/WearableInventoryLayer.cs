// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.WearableInventoryLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class WearableInventoryLayer : InventoryLayer
  {
    private WearableItem.Slot slot;
    private Suit suit;

    public WearableInventoryLayer(
      Inventory inventory,
      Point location,
      WearableItem.Slot slot,
      Suit suit)
      : base(inventory, location)
    {
      this.slot = slot;
      this.suit = suit;
      if (suit == null)
        return;
      inventory.AddItems((Item) suit.PeekItem(slot), 1);
    }

    public override int AddItem(Point location, Item item, int count)
    {
      WearableItem wearableItem = item as WearableItem;
      if (!((Item) wearableItem != (Item) null) || wearableItem.ItemSlot != this.slot)
        return 0;
      ItemSlot itemSlot = this.PeekItem(location);
      if (itemSlot != null && itemSlot.Count == 1)
        count = 0;
      count = Math.Min(1, count);
      int num = base.AddItem(location, item, count);
      if (num > 0 && this.suit != null)
        this.suit.ReplaceItem(wearableItem);
      return num;
    }

    public override ItemSlot PopItems(Point location, int count)
    {
      ItemSlot itemSlot = base.PopItems(location, count);
      if (itemSlot != null && this.suit != null && itemSlot.Item != (Item) null)
        this.suit.UnwearItem(this.slot);
      return itemSlot;
    }
  }
}
