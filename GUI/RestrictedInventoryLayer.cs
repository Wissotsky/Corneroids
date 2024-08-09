// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.RestrictedInventoryLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class RestrictedInventoryLayer : InventoryLayer
  {
    private Predicate<Item> predicate;

    public RestrictedInventoryLayer(Inventory inventory, Point location, Predicate<Item> predicate)
      : base(inventory, location)
    {
      this.predicate = predicate;
    }

    public override int AddItem(Point location, Item item, int count)
    {
      if (this.predicate == null)
        return base.AddItem(location, item, count);
      return this.predicate(item) ? base.AddItem(location, item, count) : 0;
    }
  }
}
