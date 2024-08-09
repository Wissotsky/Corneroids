// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ItemScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Screen
{
  public abstract class ItemScreen : GameScreen
  {
    private bool buttonReleasedFlag;
    private EnvironmentManager environmentManager;
    private KeyValuePair<Item, int> itemBeingDragged;
    private WindowLayer itemDescLayer;
    private List<string> itemDescription;
    private List<ItemLayer> itemLayers;
    private Character tosser;

    public ItemScreen(
      GameScreen.Type screenType,
      bool pausable,
      Character tosser,
      EnvironmentManager environmentManager)
      : base(screenType, pausable, MouseDevice.Behavior.Free)
    {
      this.buttonReleasedFlag = false;
      this.itemLayers = new List<ItemLayer>();
      this.itemDescription = new List<string>();
      this.itemDescLayer = new WindowLayer(Rectangle.Empty);
      this.tosser = tosser;
      this.environmentManager = environmentManager;
    }

    protected override void Closing()
    {
      base.Closing();
      if (this.environmentManager != null && this.tosser != null && this.itemBeingDragged.Key != (Item) null)
        this.environmentManager.AddFloatingItem(new ItemSlot(this.itemBeingDragged.Key, this.itemBeingDragged.Value), this.tosser.Position, this.tosser.GetLookatVector() * 2f, (ushort) 2000);
      this.WaitApprovalForItemAdd = (System.Action<ItemScreen.Action, System.Action<bool>>) null;
      this.WaitApprovalForItemPop = (System.Action<ItemScreen.Action, System.Action<bool>>) null;
      this.WaitApprovalForItemSwap = (System.Action<ItemScreen.Action, System.Action<bool>>) null;
    }

    public override void Render()
    {
      if (this.itemBeingDragged.Key != (Item) null)
      {
        Rectangle spriteCoordsRect = this.itemBeingDragged.Key.SpriteCoordsRect;
        Texture2D spriteAtlasTexture = this.itemBeingDragged.Key.SpriteAtlasTexture;
        Rectangle destinationRectangle = new Rectangle(this.Mouse.Position.X - 32, this.Mouse.Position.Y - 32, 64, 64);
        SpriteBatch spriteBatch = Engine.SpriteBatch;
        spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
        Engine.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
        Engine.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
        Engine.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
        spriteBatch.Draw(spriteAtlasTexture, destinationRectangle, new Rectangle?(spriteCoordsRect), Color.White);
        spriteBatch.DrawString(Engine.Font, this.itemBeingDragged.Value.ToString(), new Vector2((float) destinationRectangle.X, (float) destinationRectangle.Y), Color.White);
        spriteBatch.End();
      }
      this.RenderItemDescription(this.Mouse.Position);
    }

    public override void UpdateInput()
    {
      if (this.Mouse.LeftDown() || this.Mouse.RightDown())
      {
        if (!this.buttonReleasedFlag)
          return;
        bool flag1 = false;
        foreach (ItemLayer itemLayer in this.itemLayers)
        {
          Point point = new Point(this.Mouse.Position.X, this.Mouse.Position.Y);
          bool flag2 = itemLayer.IsValidIndex(point);
          flag1 |= flag2;
          if (flag2)
          {
            if (this.itemBeingDragged.Key == (Item) null)
            {
              ItemSlot itemSlot = itemLayer.PeekItem(point);
              if (itemSlot != null)
              {
                int quantity = 0;
                if (this.Mouse.LeftDown())
                  quantity = itemSlot.Count;
                else if (this.Mouse.RightDown())
                  quantity = itemSlot.Count != 1 ? itemSlot.Count / 2 : 1;
                this.PopItemsWithApproval(itemLayer, point, quantity);
                break;
              }
              break;
            }
            if (this.Mouse.LeftDown())
            {
              ItemSlot itemSlot = itemLayer.PeekItem(point);
              if (itemSlot == null)
              {
                this.AddItemsWithApproval(itemLayer, point, this.itemBeingDragged.Value);
                break;
              }
              if (itemSlot.Item == this.itemBeingDragged.Key && (long) itemSlot.Count < (long) this.itemBeingDragged.Key.MaxStackSize)
              {
                int quantity = (int) Math.Min((long) this.itemBeingDragged.Key.MaxStackSize - (long) itemSlot.Count, (long) this.itemBeingDragged.Value);
                this.AddItemsWithApproval(itemLayer, point, quantity);
                break;
              }
              this.SwapItemsWithApproval(itemLayer, point);
              break;
            }
            if (this.Mouse.RightDown())
            {
              ItemSlot itemSlot = itemLayer.PeekItem(point);
              if (itemSlot != null)
              {
                if ((long) itemSlot.Count >= (long) itemSlot.Item.MaxStackSize)
                  break;
              }
              this.AddItemsWithApproval(itemLayer, point, 1);
              break;
            }
            break;
          }
        }
        if (!flag1 && this.itemBeingDragged.Key != (Item) null && this.environmentManager != null && this.tosser != null)
        {
          this.environmentManager.TossItems(this.tosser, new ItemSlot(this.itemBeingDragged.Key, this.itemBeingDragged.Value));
          this.itemBeingDragged = new KeyValuePair<Item, int>((Item) null, 0);
        }
        this.buttonReleasedFlag = false;
      }
      else
        this.buttonReleasedFlag = true;
    }

    public event System.Action<ItemScreen.Action, System.Action<bool>> WaitApprovalForItemAdd;

    public event System.Action<ItemScreen.Action, System.Action<bool>> WaitApprovalForItemPop;

    public event System.Action<ItemScreen.Action, System.Action<bool>> WaitApprovalForItemSwap;

    protected Item ItemBeingDragged => this.itemBeingDragged.Key;

    public List<ItemLayer> ItemLayers => this.itemLayers;

    protected void AddItemLayer(ItemLayer layer)
    {
      if (this.itemLayers.Contains(layer))
        return;
      this.itemLayers.Add(layer);
    }

    private void AddItemToSlot(ItemLayer layer, Point position, int quantity)
    {
      layer.AddItem(position, this.itemBeingDragged.Key, quantity);
      if (quantity == this.itemBeingDragged.Value)
        this.itemBeingDragged = new KeyValuePair<Item, int>((Item) null, 0);
      else
        this.itemBeingDragged = new KeyValuePair<Item, int>(this.itemBeingDragged.Key, this.itemBeingDragged.Value - quantity);
    }

    private void AddItemsWithApproval(ItemLayer layer, Point position, int quantity)
    {
      if (this.WaitApprovalForItemAdd != null)
      {
        byte x = 0;
        byte y = 0;
        layer.GetIndicesOfPosition(position, out x, out y);
        this.WaitApprovalForItemAdd(new ItemScreen.Action()
        {
          Type = ItemScreen.ActionType.Addition,
          X = x,
          Y = y,
          Count = quantity,
          InventoryType = this.GetInventoryTypeOfLayer(layer, position),
          ItemId = this.itemBeingDragged.Key.ItemId
        }, (System.Action<bool>) (approved =>
        {
          if (!approved)
            return;
          this.AddItemToSlot(layer, position, quantity);
        }));
      }
      else
        this.AddItemToSlot(layer, position, quantity);
    }

    protected abstract Inventory.Type GetInventoryTypeOfLayer(ItemLayer layer, Point location);

    private Point GetRectangleSize(List<string> lines)
    {
      if (lines == null)
        return Point.Zero;
      Point rectangleSize = Point.Zero;
      foreach (string line in lines)
      {
        if (!string.IsNullOrEmpty(line))
        {
          Vector2 vector2 = Engine.Font.MeasureString(line);
          rectangleSize = new Point(Math.Max((int) vector2.X, rectangleSize.X), rectangleSize.Y + (int) vector2.Y);
        }
      }
      return rectangleSize;
    }

    private void PopItemsWithApproval(ItemLayer layer, Point position, int quantity)
    {
      ItemSlot pickedItem = (ItemSlot) null;
      if (this.WaitApprovalForItemPop != null)
      {
        byte x = 0;
        byte y = 0;
        layer.GetIndicesOfPosition(position, out x, out y);
        this.WaitApprovalForItemPop(new ItemScreen.Action()
        {
          Type = ItemScreen.ActionType.Removal,
          X = x,
          Y = y,
          Count = quantity,
          InventoryType = this.GetInventoryTypeOfLayer(layer, position)
        }, (System.Action<bool>) (approved =>
        {
          if (!approved)
            return;
          pickedItem = layer.PopItems(position, quantity);
          if (pickedItem == null)
            return;
          this.itemBeingDragged = new KeyValuePair<Item, int>(pickedItem.Item, pickedItem.Count);
        }));
      }
      else
      {
        pickedItem = layer.PopItems(position, quantity);
        if (pickedItem == null)
          return;
        this.itemBeingDragged = new KeyValuePair<Item, int>(pickedItem.Item, pickedItem.Count);
      }
    }

    protected void RenderItemDescription(Point mouseLocation)
    {
      if (this.itemBeingDragged.Key != (Item) null)
        return;
      foreach (ItemLayer itemLayer in this.itemLayers)
      {
        Point location = new Point(this.Mouse.Position.X, this.Mouse.Position.Y);
        if (itemLayer.IsValidIndex(location))
        {
          ItemSlot itemSlot = itemLayer.PeekItem(location);
          if (itemSlot != null)
          {
            this.itemDescription.Clear();
            itemSlot.Item.GetStatistics(this.itemDescription, false);
            Point rectangleSize = this.GetRectangleSize(this.itemDescription);
            Rectangle rectangle = new Rectangle(mouseLocation.X + 30, mouseLocation.Y, rectangleSize.X, rectangleSize.Y);
            rectangle = new Rectangle(Math.Min(Math.Max(rectangle.X, 0), Engine.GraphicsDevice.PresentationParameters.BackBufferWidth - rectangle.Width), Math.Min(Math.Max(rectangle.Y, 0), Engine.GraphicsDevice.PresentationParameters.BackBufferHeight - rectangle.Height), rectangle.Width, rectangle.Height);
            this.itemDescLayer.PositionAndSize = rectangle;
            this.itemDescLayer.Render();
            Engine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            for (int index = 0; index < this.itemDescription.Count; ++index)
              Engine.SpriteBatch.DrawString(Engine.Font, this.itemDescription[index] ?? string.Empty, new Vector2((float) rectangle.X, (float) (rectangle.Y + index * rectangleSize.Y / this.itemDescription.Count)), Color.White);
            Engine.SpriteBatch.End();
            break;
          }
        }
      }
    }

    private void SwapItems(ItemLayer layer, Point position)
    {
      ItemSlot itemSlot1 = layer.PeekItem(position);
      if (itemSlot1 == null)
        return;
      ItemSlot itemSlot2 = layer.PopItems(position, itemSlot1.Count);
      int count = layer.AddItem(position, this.itemBeingDragged.Key, this.itemBeingDragged.Value);
      if (count < this.itemBeingDragged.Value)
      {
        layer.PopItems(position, count);
        layer.AddItem(position, itemSlot2.Item, itemSlot2.Count);
      }
      else
        this.itemBeingDragged = new KeyValuePair<Item, int>(itemSlot2.Item, itemSlot2.Count);
    }

    private void SwapItemsWithApproval(ItemLayer layer, Point position)
    {
      if (this.WaitApprovalForItemSwap != null)
      {
        byte x = 0;
        byte y = 0;
        layer.GetIndicesOfPosition(position, out x, out y);
        this.WaitApprovalForItemSwap(new ItemScreen.Action()
        {
          Type = ItemScreen.ActionType.Swap,
          X = x,
          Y = y,
          InventoryType = this.GetInventoryTypeOfLayer(layer, position),
          ItemId = this.itemBeingDragged.Key.ItemId,
          Count = this.itemBeingDragged.Value
        }, (System.Action<bool>) (approved =>
        {
          if (!approved)
            return;
          this.SwapItems(layer, position);
        }));
      }
      else
        this.SwapItems(layer, position);
    }

    public class Action
    {
      public ItemScreen.ActionType Type;
      public byte X;
      public byte Y;
      public int Count;
      public Inventory.Type InventoryType;
      public int ItemId;
    }

    public delegate void ApproveAction(bool result, int realizedValue);

    public enum ActionType
    {
      Addition,
      Removal,
      Swap,
    }

    public enum Behavior
    {
      Items_can_be_added = 1,
      Items_can_be_removed = 2,
    }
  }
}
