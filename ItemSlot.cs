// Decompiled with JetBrains decompiler
// Type: CornerSpace.ItemSlot
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class ItemSlot : IBillboard
  {
    public Item Item;
    public int Count;

    public ItemSlot(Item item, int count)
    {
      this.Item = item;
      this.Count = count;
    }

    public int StoredValue
    {
      get
      {
        return this.Item != (Item) null && this.Count > 0 ? (this.Item.ItemId & (int) ushort.MaxValue) << 16 | this.Count & (int) ushort.MaxValue : 0;
      }
    }

    public Vector4 SpriteCoordsUV
    {
      get => this.Item != (Item) null ? this.Item.SpriteCoordsUV : Vector4.Zero;
    }
  }
}
