// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ItemListScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace.Screen
{
  public class ItemListScreen : MenuScreen
  {
    private WindowLayer backgroundLayer;
    private VerticalScrollbarLayer scrollbarLayer;
    private Item[,] items;
    private Player player;
    private int scrollPosition;

    public ItemListScreen(Player player)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.player = player;
      this.scrollPosition = 0;
    }

    public override void Load()
    {
      base.Load();
      this.backgroundLayer = new WindowLayer(Layer.EvaluateMiddlePosition(400, 550));
      this.scrollbarLayer = new VerticalScrollbarLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X - 30, this.backgroundLayer.Location.Y, 30, (int) this.backgroundLayer.Size.Y), 1000);
      this.scrollbarLayer.ScrollbarMovedEvent += new System.Action(this.Scroll);
      if (Engine.LoadedWorld == null)
        return;
      List<Item> list = new List<Item>((IEnumerable<Item>) Engine.LoadedWorld.Itemset.Items).Where<Item>((Func<Item, bool>) (i => i != (Item) null)).ToList<Item>();
      int count = list.Count;
      int length1 = Math.Min(5, count);
      int length2 = count / 5 + 1;
      if (length2 <= 0 || length1 <= 0)
        return;
      this.items = new Item[length1, length2];
      for (int index = 0; index < list.Count; ++index)
        this.items[index % 5, index / 5] = list[index];
    }

    public override void Render()
    {
      base.Render();
      this.backgroundLayer.Render();
      this.scrollbarLayer.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
      Engine.SetPointSamplerStateForSpritebatch();
      for (int index1 = 0; index1 < 8; ++index1)
      {
        for (int index2 = 0; index2 < this.items.GetLength(0); ++index2)
        {
          if (index1 + this.scrollPosition >= 0 && index1 + this.scrollPosition < this.items.GetLength(1))
          {
            Item obj = this.items[index2, index1 + this.scrollPosition];
            if (obj != (Item) null)
              spriteBatch.Draw(obj.SpriteAtlasTexture, new Rectangle(this.backgroundLayer.Location.X + index2 * 64, this.backgroundLayer.Location.Y + index1 * 64, 64, 64), new Rectangle?(obj.SpriteCoordsRect), Color.White);
          }
        }
      }
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.scrollbarLayer.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick() && !this.Mouse.RightClick() || !this.backgroundLayer.Contains(this.Mouse.Position))
        return;
      int index = (this.Mouse.Position.X - this.backgroundLayer.Location.X) / 64;
      int num = (this.Mouse.Position.Y - this.backgroundLayer.Location.Y) / 64;
      if (index < 0 || index >= this.items.GetLength(0) || num + this.scrollPosition < 0 || num + this.scrollPosition >= this.items.GetLength(1))
        return;
      Item obj = this.items[index, num + this.scrollPosition];
      if (this.player == null || !(obj != (Item) null))
        return;
      int count = this.Mouse.RightClick() ? 64 : 1;
      this.player.Inventory.AddItems(obj, count);
    }

    private void Scroll()
    {
      float positionPercentage = this.scrollbarLayer.ScrollPositionPercentage;
      if (this.items.GetLength(1) <= 8)
        return;
      this.scrollPosition = (int) ((double) (this.items.GetLength(1) - 8) * (double) positionPercentage);
    }
  }
}
