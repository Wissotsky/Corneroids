// Decompiled with JetBrains decompiler
// Type: CornerSpace.ItemOperationManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class ItemOperationManager
  {
    private Dictionary<Inventory.Type, ItemOperationManager.ExecuteAction> addDelegateLookup;
    private Dictionary<Inventory.Type, ItemOperationManager.ExecuteAction> popDelegateLookup;
    private Dictionary<Inventory.Type, ItemOperationManager.ExecuteAction> swapDelegateLookup;

    public ItemOperationManager() => this.InitializeDelegateDictionaries();

    public bool ExecuteOperation(Player player, ItemScreen.Action operation)
    {
      if (player != null)
      {
        if (operation != null)
        {
          try
          {
            switch (operation.Type)
            {
              case ItemScreen.ActionType.Addition:
                return this.addDelegateLookup.ContainsKey(operation.InventoryType) && this.addDelegateLookup[operation.InventoryType](player, operation);
              case ItemScreen.ActionType.Removal:
                return this.popDelegateLookup.ContainsKey(operation.InventoryType) && this.popDelegateLookup[operation.InventoryType](player, operation);
              case ItemScreen.ActionType.Swap:
                return this.swapDelegateLookup.ContainsKey(operation.InventoryType) && this.swapDelegateLookup[operation.InventoryType](player, operation);
            }
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Error while parsing client item request: " + ex.Message);
          }
          return false;
        }
      }
      return false;
    }

    private Item GetItemOfOperation(ItemScreen.Action operation)
    {
      return Engine.LoadedWorld.Itemset.GetItem(operation.ItemId);
    }

    private void InitializeDelegateDictionaries()
    {
      this.addDelegateLookup = new Dictionary<Inventory.Type, ItemOperationManager.ExecuteAction>()
      {
        {
          Inventory.Type.Inventory,
          new ItemOperationManager.ExecuteAction(this.AddItemInventory)
        },
        {
          Inventory.Type.Toolbar,
          new ItemOperationManager.ExecuteAction(this.AddItemToolbar)
        },
        {
          Inventory.Type.Trash,
          new ItemOperationManager.ExecuteAction(this.AddItemTrash)
        }
      };
      this.popDelegateLookup = new Dictionary<Inventory.Type, ItemOperationManager.ExecuteAction>()
      {
        {
          Inventory.Type.Inventory,
          new ItemOperationManager.ExecuteAction(this.PopItemInventory)
        },
        {
          Inventory.Type.Toolbar,
          new ItemOperationManager.ExecuteAction(this.PopItemToolbar)
        },
        {
          Inventory.Type.DefaultDrill,
          new ItemOperationManager.ExecuteAction(this.PopItemDefaultDrill)
        },
        {
          Inventory.Type.Crafting,
          new ItemOperationManager.ExecuteAction(this.PopItemCrafting)
        },
        {
          Inventory.Type.Smelter,
          new ItemOperationManager.ExecuteAction(this.PopItemSmelter)
        }
      };
      this.swapDelegateLookup = new Dictionary<Inventory.Type, ItemOperationManager.ExecuteAction>()
      {
        {
          Inventory.Type.Inventory,
          new ItemOperationManager.ExecuteAction(this.SwapItemInventory)
        },
        {
          Inventory.Type.Toolbar,
          new ItemOperationManager.ExecuteAction(this.SwapItemToolbar)
        }
      };
    }

    private bool AddItemInventory(Player player, ItemScreen.Action operation)
    {
      return player.Inventory.AddItems(this.GetItemOfOperation(operation), operation.Count, (int) operation.X, (int) operation.Y, false) == operation.Count;
    }

    private bool AddItemToolbar(Player player, ItemScreen.Action operation)
    {
      return player.Toolbar.Items.AddItems(this.GetItemOfOperation(operation), operation.Count, (int) operation.X, (int) operation.Y, false) == operation.Count;
    }

    private bool AddItemTrash(Player player, ItemScreen.Action operation) => true;

    private bool PopItemDefaultDrill(Player player, ItemScreen.Action operation) => true;

    private bool PopItemInventory(Player player, ItemScreen.Action operation)
    {
      ItemSlot itemSlot = player.Inventory.PopItem((int) operation.X, (int) operation.Y, operation.Count, false);
      return itemSlot != null && itemSlot.Count == operation.Count;
    }

    private bool PopItemToolbar(Player player, ItemScreen.Action operation)
    {
      ItemSlot itemSlot = player.Toolbar.Items.PopItem((int) operation.X, (int) operation.Y, operation.Count, false);
      return itemSlot != null && itemSlot.Count == operation.Count;
    }

    private bool PopItemCrafting(Player player, ItemScreen.Action operation) => true;

    private bool PopItemSmelter(Player player, ItemScreen.Action operation) => true;

    private bool SwapItemInventory(Player player, ItemScreen.Action operation)
    {
      Item itemOfOperation = this.GetItemOfOperation(operation);
      int count = operation.Count;
      if (itemOfOperation == (Item) null || count <= 0)
        return false;
      player.Inventory.PopItem((int) operation.X, (int) operation.Y, out int _, false);
      player.Inventory.AddItems(this.GetItemOfOperation(operation), operation.Count, (int) operation.X, (int) operation.Y, false);
      return true;
    }

    private bool SwapItemToolbar(Player player, ItemScreen.Action operation)
    {
      Item itemOfOperation = this.GetItemOfOperation(operation);
      int count = operation.Count;
      if (itemOfOperation == (Item) null || count <= 0)
        return false;
      player.Toolbar.Items.PopItem((int) operation.X, (int) operation.Y, out int _, false);
      player.Toolbar.Items.AddItems(this.GetItemOfOperation(operation), operation.Count, (int) operation.X, (int) operation.Y, false);
      return true;
    }

    private delegate bool ExecuteAction(Player player, ItemScreen.Action operation);
  }
}
