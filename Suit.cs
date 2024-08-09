// Decompiled with JetBrains decompiler
// Type: CornerSpace.Suit
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Networking;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class Suit : ISuitRatings, ISynchronizable, IByteSerializable
  {
    private ArmorRating armorRating;
    private int healthRating;
    private int powerRating;
    private Dictionary<WearableItem.Slot, WearableItem> slotsAndItems;

    public Suit() => this.slotsAndItems = new Dictionary<WearableItem.Slot, WearableItem>();

    public void Extract(byte[] data)
    {
      try
      {
        if (data == null || data.Length % 3 != 0)
          return;
        this.slotsAndItems.Clear();
        int num = data.Length / 3;
        for (int index1 = 0; index1 < num; ++index1)
        {
          int index2 = index1 * 3;
          WearableItem.Slot key = (WearableItem.Slot) data[index2];
          WearableItem wearableItem = Engine.LoadedWorld.Itemset.GetItem((0 | (int) data[index2 + 1]) << 8 | (int) data[index2 + 2]) as WearableItem;
          if ((Item) wearableItem != (Item) null)
            this.slotsAndItems.Add(key, wearableItem);
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to extract suit data from byte array: " + ex.Message);
      }
    }

    public int?[] GetItemsIdArrayy()
    {
      int?[] itemsIdArrayy = new int?[7];
      for (int index = 0; index < itemsIdArrayy.Length; ++index)
        itemsIdArrayy[index] = new int?();
      foreach (KeyValuePair<WearableItem.Slot, WearableItem> slotsAndItem in this.slotsAndItems)
      {
        int key = (int) slotsAndItem.Key;
        int itemId = slotsAndItem.Value.ItemId;
        if (key >= 0 && key < itemsIdArrayy.Length)
          itemsIdArrayy[key] = new int?(itemId);
      }
      return itemsIdArrayy;
    }

    public WearableItem PeekItem(WearableItem.Slot slot)
    {
      return this.slotsAndItems.ContainsKey(slot) ? this.slotsAndItems[slot] : (WearableItem) null;
    }

    public WearableItem ReplaceItem(WearableItem item)
    {
      if (!((Item) item != (Item) null))
        return (WearableItem) null;
      WearableItem wearableItem = this.UnwearItem(item.ItemSlot);
      this.WearItem(item);
      return wearableItem;
    }

    public byte[] Serialize()
    {
      try
      {
        byte[] numArray = new byte[this.slotsAndItems.Keys.Count * 3];
        int index = 0;
        foreach (KeyValuePair<WearableItem.Slot, WearableItem> slotsAndItem in this.slotsAndItems)
        {
          numArray[index] = (byte) slotsAndItem.Key;
          int itemId = (Item) slotsAndItem.Value != (Item) null ? slotsAndItem.Value.ItemId : 0;
          numArray[index + 1] = (byte) (itemId >> 8 & (int) byte.MaxValue);
          numArray[index + 2] = (byte) (itemId & (int) byte.MaxValue);
          index += 3;
        }
        return numArray;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to serialize suit: " + ex.Message);
      }
      return (byte[]) null;
    }

    public WearableItem UnwearItem(WearableItem.Slot slot)
    {
      if (!this.slotsAndItems.ContainsKey(slot))
        return (WearableItem) null;
      WearableItem slotsAndItem = this.slotsAndItems[slot];
      this.slotsAndItems.Remove(slot);
      slotsAndItem.RemovedFromSuit((ISuitRatings) this);
      if (this.DataChanged != null)
        this.DataChanged();
      return slotsAndItem;
    }

    public bool WearItem(WearableItem item)
    {
      if (!((Item) item != (Item) null) || this.slotsAndItems.ContainsKey(item.ItemSlot))
        return false;
      this.slotsAndItems.Add(item.ItemSlot, item);
      item.AddedToSuit((ISuitRatings) this);
      if (this.DataChanged != null)
        this.DataChanged();
      return true;
    }

    public event System.Action DataChanged;

    public ArmorRating ArmorRating
    {
      get => this.armorRating;
      set => this.armorRating = value;
    }

    public int HealthRating
    {
      get => this.healthRating;
      set => this.healthRating = value;
    }

    public int PowerRechargeRate
    {
      get => this.powerRating;
      set => this.powerRating = value;
    }
  }
}
