// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ItemDescriptionLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.GUI
{
  public class ItemDescriptionLayer : WindowLayer
  {
    private Item item;
    private List<string> textLines;

    public ItemDescriptionLayer(Item item)
      : base(Rectangle.Empty)
    {
      this.item = item;
      this.textLines = new List<string>();
    }

    public override void Render()
    {
      if (!(this.item != (Item) null))
        return;
      base.Render();
      this.textLines.Clear();
      this.item.GetStatistics(this.textLines, false);
      Point rectangleSize = this.GetRectangleSize(this.textLines);
      this.Size = new Vector2((float) rectangleSize.X, (float) rectangleSize.Y);
      Engine.SpriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
      for (int index = 0; index < this.textLines.Count; ++index)
        Engine.SpriteBatch.DrawString(Engine.Font, this.textLines[index] ?? string.Empty, new Vector2((float) this.Location.X, (float) (this.Location.Y + index * rectangleSize.Y / this.textLines.Count)), Color.White);
      Engine.SpriteBatch.End();
    }

    public Item Item
    {
      set => this.item = value;
    }

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
  }
}
