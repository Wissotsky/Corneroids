// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.DefaultItemInventoryLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace.GUI
{
  public class DefaultItemInventoryLayer : InventoryLayer
  {
    public DefaultItemInventoryLayer(Point location, Item defaultItem)
      : base(new Inventory(1U, 1U, (Player) null), location)
    {
      this.Inventory.AddItems(defaultItem, 1);
    }

    public override int AddItem(Point location, Item item, int count) => 0;

    public override ItemSlot PopItems(Point location, int count) => this.PeekItem(location);
  }
}
