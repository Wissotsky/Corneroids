// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.InventoryLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class InventoryLayer : ItemLayer
  {
    private const int cellSize = 64;
    private const int width = 464;
    private const int height = 314;
    private Vector2 gridPosition;
    private Inventory inventory;

    public InventoryLayer(Inventory inventory, Point location)
      : base(new Rectangle(location.X, location.Y, (int) inventory.Width * 64, (int) inventory.Height * 64))
    {
      this.inventory = inventory;
      this.gridPosition = Vector2.One * 0.0f;
    }

    public bool GetInventoryCell(Point mouseLocation, out int x, out int y)
    {
      Vector2 vector2 = new Vector2((float) mouseLocation.X, (float) mouseLocation.Y) - (this.Position + this.gridPosition);
      x = (int) Math.Floor((double) vector2.X / 64.0);
      y = (int) Math.Floor((double) vector2.Y / 64.0);
      return this.inventory.ValidIndex(x, y);
    }

    public override void Render()
    {
      this.RenderGrid(this.Position + this.gridPosition, new Vector2((float) this.inventory.Width, (float) this.inventory.Height), 64);
      this.RenderItems();
    }

    public override int AddItem(Point location, Item item, int count)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      int num = 0;
      int x;
      int y;
      if (this.PointToIndex(location, out x, out y))
        num = this.inventory.AddItems(item, count, x, y);
      return num;
    }

    public override void GetIndicesOfPosition(Point location, out byte x, out byte y)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      int x1;
      int y1;
      this.PointToIndex(location, out x1, out y1);
      x = (byte) x1;
      y = (byte) y1;
    }

    public override bool IsValidIndex(Point location)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      return this.PointToIndex(location, out int _, out int _);
    }

    public override ItemSlot PeekItem(Point location)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      int x;
      int y;
      return this.PointToIndex(location, out x, out y) ? this.inventory.PeekItem(x, y) : (ItemSlot) null;
    }

    public override ItemSlot PopItems(Point location, int count)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      int x;
      int y;
      return this.PointToIndex(location, out x, out y) && this.inventory.PeekItem(x, y) != null ? this.inventory.PopItem(x, y, count) : new ItemSlot((Item) null, 0);
    }

    public Inventory Inventory => this.inventory;

    private bool PointToIndex(Point location, out int x, out int y)
    {
      x = 0;
      y = 0;
      Point point = new Point(location.X - (int) this.gridPosition.X, location.Y - (int) this.gridPosition.Y);
      if (point.X < 0 || (long) point.X > (long) (64U * this.inventory.Width) || point.Y < 0 || (long) point.Y > (long) (64U * this.inventory.Height))
        return false;
      x = point.X / 64;
      y = point.Y / 64;
      return true;
    }

    private void RenderItems()
    {
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
      Engine.SetPointSamplerStateForSpritebatch();
      Rectangle positionAndSize = this.PositionAndSize;
      for (int index1 = 0; (long) index1 < (long) this.inventory.Height; ++index1)
      {
        for (int index2 = 0; (long) index2 < (long) this.inventory.Width; ++index2)
        {
          if (this.inventory.Items[index2, index1] != null)
          {
            Rectangle destinationRectangle = new Rectangle(positionAndSize.X + (int) this.gridPosition.X + index2 * 64 + 4, positionAndSize.Y + (int) this.gridPosition.Y + index1 * 64 + 4, 56, 56);
            spriteBatch.Draw(this.inventory.Items[index2, index1].Item.SpriteAtlasTexture, destinationRectangle, new Rectangle?(this.inventory.Items[index2, index1].Item.SpriteCoordsRect), Color.White);
            spriteBatch.DrawString(Engine.Font, this.inventory.Items[index2, index1].Count.ToString(), new Vector2((float) (positionAndSize.X + (int) this.gridPosition.X + index2 * 64), (float) (positionAndSize.Y + (int) this.gridPosition.Y + index1 * 64)), Color.White);
          }
        }
      }
      spriteBatch.End();
    }
  }
}
