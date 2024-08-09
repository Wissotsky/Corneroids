// Decompiled with JetBrains decompiler
// Type: CornerSpace.Inventory
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class Inventory : IEnumerable<ItemSlot>, IEnumerable, ISynchronizable, IByteSerializable
  {
    private uint width;
    private uint height;
    private ItemSlot[,] items;
    private Player owner;

    public Inventory(uint width, uint height, Player owner)
    {
      this.width = width;
      this.height = height;
      this.owner = owner;
      this.items = new ItemSlot[(int) (IntPtr) width, (int) (IntPtr) height];
    }

    public int AddItems(Item item, int count, int x, int y, bool triggerEvents)
    {
      if (item == (Item) null || !this.ValidIndex(x, y))
        return 0;
      if (this.items[x, y] == null)
      {
        int count1 = (int) Math.Min((long) item.MaxStackSize, (long) count);
        this.items[x, y] = new ItemSlot(item, count1);
        if (triggerEvents)
          this.LaunchAddedItemEvents(new ItemSlot(item, count1), x, y);
        return count1;
      }
      if (!(this.PeekItem(x, y).Item == item))
        return 0;
      int count2 = (int) Math.Min((long) item.MaxStackSize - (long) this.items[x, y].Count, (long) count);
      this.items[x, y].Count += count2;
      if (triggerEvents)
        this.LaunchAddedItemEvents(new ItemSlot(item, count2), x, y);
      return count2;
    }

    public int AddItems(Item item, int count, int x, int y)
    {
      return this.AddItems(item, count, x, y, true);
    }

    public int AddItems(Item item, int count)
    {
      int num = count;
      for (int y = 0; (long) y < (long) this.height; ++y)
      {
        for (int x = 0; (long) x < (long) this.width; ++x)
        {
          num -= this.AddItems(item, count, x, y);
          if (num <= 0)
            return count;
        }
      }
      return count - num;
    }

    public void Extract(byte[] data)
    {
      try
      {
        if ((long) data.Length != (long) (uint) ((int) this.width * (int) this.height * 4))
          return;
        this.Clear();
        for (int index1 = 0; (long) index1 < (long) this.height; ++index1)
        {
          for (int index2 = 0; (long) index2 < (long) this.width; ++index2)
          {
            int num1 = 0;
            int num2 = 0;
            int id = (num1 | (int) data[((long) index1 * (long) this.width + (long) index2) * 4L]) << 8 | (int) data[((long) index1 * (long) this.width + (long) index2) * 4L + 1L];
            int count = (num2 | (int) data[((long) index1 * (long) this.width + (long) index2) * 4L + 2L]) << 8 | (int) data[((long) index1 * (long) this.width + (long) index2) * 4L + 3L];
            Item obj = Engine.LoadedWorld.Itemset.GetItem(id);
            if (obj != (Item) null && count > 0)
              this.items[index2, index1] = new ItemSlot(obj, count);
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to extract inventory data from a byte array: " + ex.Message);
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<ItemSlot> GetEnumerator()
    {
      for (int y = 0; (long) y < (long) this.height; ++y)
      {
        for (int x = 0; (long) x < (long) this.width; ++x)
        {
          if (this.items[x, y] != null && this.items[x, y].Item != (Item) null && this.items[x, y].Count > 0)
            yield return this.items[x, y];
        }
      }
    }

    public bool ValidIndex(int x, int y)
    {
      return x >= 0 && y >= 0 && (long) x < (long) this.width && (long) y < (long) this.height;
    }

    public void LoadFromXml(XElement element) => throw new NotImplementedException();

    public ItemSlot PeekItem(int x, int y)
    {
      if (!this.ValidIndex(x, y))
        return (ItemSlot) null;
      return this.items[x, y] != null ? this.items[x, y] : (ItemSlot) null;
    }

    public ItemSlot PopItem(int x, int y, int count) => this.PopItem(x, y, count, true);

    public ItemSlot PopItem(int x, int y, int count, bool triggerEvents)
    {
      if (this.ValidIndex(x, y))
      {
        if (this.items[x, y] == null)
          return (ItemSlot) null;
        if (this.items[x, y].Count > 0)
        {
          ItemSlot itemSlot = this.items[x, y];
          int count1 = Math.Min(this.items[x, y].Count, count);
          this.items[x, y].Count -= count1;
          if (this.items[x, y].Count == 0)
            this.items[x, y] = (ItemSlot) null;
          if (triggerEvents)
            this.LaunchRemovedItemEvents(new ItemSlot(itemSlot.Item, count1), x, y);
          return new ItemSlot(itemSlot.Item, count1);
        }
      }
      return (ItemSlot) null;
    }

    public Item PopItem(int x, int y, out int count, bool triggerEvents)
    {
      if (this.ValidIndex(x, y) && this.items[x, y].Count > 0)
      {
        Item obj = this.items[x, y].Item;
        count = this.items[x, y].Count;
        this.items[x, y] = (ItemSlot) null;
        if (this.ItemsRemoved != null && triggerEvents)
          this.ItemsRemoved(new ItemSlot(obj, count), x, y);
        return obj;
      }
      count = 0;
      return (Item) null;
    }

    public int RemoveItem(Item item, int count)
    {
      if (item == (Item) null)
        return 0;
      int val2 = count;
      for (int index1 = 0; (long) index1 < (long) this.height; ++index1)
      {
        for (int index2 = 0; (long) index2 < (long) this.width; ++index2)
        {
          if (val2 <= 0)
            return count;
          if (this.items[index2, index1] != null && this.items[index2, index1].Item == item)
          {
            int count1 = Math.Min(this.items[index2, index1].Count, val2);
            this.items[index2, index1].Count -= count1;
            if (this.items[index2, index1].Count == 0)
              this.items[index2, index1] = (ItemSlot) null;
            val2 -= count1;
            if (this.ItemsRemoved != null)
              this.ItemsRemoved(new ItemSlot(item, count1), index2, index1);
          }
        }
      }
      return count - val2;
    }

    public void SetItem(Player.ItemModifiedArgs itemInfo)
    {
      if (itemInfo == null)
        return;
      this.SetItem(itemInfo.ItemSlot.Item, itemInfo.ItemSlot.Count, itemInfo.X, itemInfo.Y);
    }

    public void SetItem(Item item, int count, int x, int y)
    {
      if (!this.ValidIndex(x, y))
        return;
      this.items[x, y] = new ItemSlot(item, item != (Item) null ? (int) Math.Min((long) count, (long) item.MaxStackSize) : 0);
    }

    public byte[] Serialize()
    {
      byte[] numArray = new byte[(IntPtr) (uint) ((int) this.Width * (int) this.Height * 4)];
      for (int index1 = 0; (long) index1 < (long) this.height; ++index1)
      {
        for (int index2 = 0; (long) index2 < (long) this.width; ++index2)
        {
          ItemSlot itemSlot = this.items[index2, index1];
          if (itemSlot != null)
          {
            int itemId = itemSlot.Item.ItemId;
            int count = itemSlot.Count;
            numArray[((long) index1 * (long) this.width + (long) index2) * 4L] = (byte) (itemId >> 8 & (int) byte.MaxValue);
            numArray[((long) index1 * (long) this.width + (long) index2) * 4L + 1L] = (byte) (itemId & (int) byte.MaxValue);
            numArray[((long) index1 * (long) this.width + (long) index2) * 4L + 2L] = (byte) (count >> 8 & (int) byte.MaxValue);
            numArray[((long) index1 * (long) this.width + (long) index2) * 4L + 3L] = (byte) (count & (int) byte.MaxValue);
          }
        }
      }
      return numArray;
    }

    public int SpaceLeft(int x, int y)
    {
      if (this.ValidIndex(x, y))
      {
        ItemSlot itemSlot = this.PeekItem(x, y);
        if (itemSlot != null)
          return (int) itemSlot.Item.MaxStackSize - itemSlot.Count;
      }
      return 0;
    }

    public bool SpaceLeftForItem(Item item, int count)
    {
      if (item == (Item) null)
        return false;
      if (count <= 0)
        return true;
      int num = count;
      for (int y = 0; (long) y < (long) this.height; ++y)
      {
        for (int x = 0; (long) x < (long) this.width; ++x)
        {
          ItemSlot itemSlot = this.PeekItem(x, y);
          if (itemSlot == null)
            num -= (int) item.MaxStackSize;
          else if (itemSlot.Item == item)
            num -= (int) ((long) item.MaxStackSize - (long) itemSlot.Count);
          if (num <= 0)
            break;
        }
        if (num <= 0)
          break;
      }
      return num <= 0;
    }

    public event System.Action DataChanged;

    public event Action<ItemSlot, int, int> ItemsAdded;

    public event Action<ItemSlot, int, int> ItemsRemoved;

    public uint Height => this.height;

    public uint ItemCount
    {
      get
      {
        uint itemCount = 0;
        for (int index1 = 0; (long) index1 < (long) this.height; ++index1)
        {
          for (int index2 = 0; (long) index2 < (long) this.width; ++index2)
          {
            if (this.items[index2, index1] != null)
              itemCount += (uint) this.items[index2, index1].Count;
          }
        }
        return itemCount;
      }
    }

    public ItemSlot[] ItemsArray
    {
      get
      {
        List<ItemSlot> itemSlotList = new List<ItemSlot>();
        for (int index1 = 0; (long) index1 < (long) this.height; ++index1)
        {
          for (int index2 = 0; (long) index2 < (long) this.width; ++index2)
          {
            if (this.items[index2, index1] != null)
              itemSlotList.Add(new ItemSlot(this.items[index2, index1].Item, this.items[index2, index1].Count));
          }
        }
        return itemSlotList.ToArray();
      }
    }

    public ItemSlot[,] Items => this.items;

    public Player Owner => this.owner;

    public uint Width => this.width;

    private void Clear()
    {
      for (int index1 = 0; (long) index1 < (long) this.height; ++index1)
      {
        for (int index2 = 0; (long) index2 < (long) this.width; ++index2)
          this.items[index2, index1] = (ItemSlot) null;
      }
    }

    private void LaunchAddedItemEvents(ItemSlot slot, int x, int y)
    {
      if (this.ItemsAdded != null)
        this.ItemsAdded(slot, x, y);
      if (this.DataChanged == null)
        return;
      this.DataChanged();
    }

    private void LaunchRemovedItemEvents(ItemSlot slot, int x, int y)
    {
      if (this.ItemsRemoved != null)
        this.ItemsRemoved(slot, x, y);
      if (this.DataChanged == null)
        return;
      this.DataChanged();
    }

    public enum Type : byte
    {
      Toolbar,
      Inventory,
      DefaultDrill,
      Trash,
      Smelter,
      Crafting,
    }
  }
}
