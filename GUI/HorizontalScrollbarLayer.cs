// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.HorizontalScrollbarLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class HorizontalScrollbarLayer : ScrollbarLayer
  {
    public HorizontalScrollbarLayer(Rectangle positionAndSize, int scrollableArea)
      : base(positionAndSize, scrollableArea)
    {
      Point point1 = new Point((int) this.Size.Y, (int) this.Size.Y);
      Point location = this.PositionAndSize.Location;
      Point point2 = new Point(this.Location.X + (int) this.Size.X - point1.X, this.Location.Y);
      this.buttonLayerOne = new ButtonLayer(new Rectangle(location.X, location.Y, point1.X, point1.Y), "<");
      this.buttonLayerTwo = new ButtonLayer(new Rectangle(point2.X, point2.Y, point1.X, point1.Y), ">");
      this.backgroundLayer = new FieldLayer(this.PositionAndSize);
      this.scrollbarScrollArea = Math.Max((int) this.Size.X - 2 * point1.X, 0);
      Point point3 = new Point((int) Math.Min(Math.Max(0.0f, (float) this.scrollbarScrollArea / (float) scrollableArea * (float) this.scrollbarScrollArea), (float) this.scrollbarScrollArea), (int) this.Size.Y);
      this.scrollButton = new WindowLayer(new Rectangle(this.Location.X + point1.X, this.Location.Y, point3.X, point3.Y));
    }

    public override void UpdateInput(MouseDevice mouse)
    {
      if (mouse.LeftDown())
      {
        if (this.buttonLayerOne.PositionAndSize.Contains(mouse.Position))
          this.ScrollUp();
        else if (this.buttonLayerTwo.PositionAndSize.Contains(mouse.Position))
          this.ScrollDown();
        else if (this.scrollButton.PositionAndSize.Contains(mouse.Position))
          this.ScrollPosition += (int) Math.Ceiling((double) mouse.GetTranslationX() * (double) this.scrollableArea / (double) ((float) this.scrollbarScrollArea - this.scrollButton.Size.X));
        else if (this.backgroundLayer.PositionAndSize.Contains(mouse.Position))
        {
          int scrollbarScrollArea = this.scrollbarScrollArea;
          double x = (double) this.buttonLayerOne.Size.X;
          this.ScrollPosition = (int) ((double) ((float) (mouse.Position.X - (this.backgroundLayer.Location.X + (int) this.buttonLayerOne.Size.X)) / (float) this.scrollbarScrollArea) * (double) this.scrollableArea);
        }
      }
      if (mouse.Scroll() > 0)
      {
        this.ScrollUp();
      }
      else
      {
        if (mouse.Scroll() >= 0)
          return;
        this.ScrollDown();
      }
    }

    public override void UpdateScrollableArea(int value)
    {
      this.scrollableArea = Math.Max(0, value);
      if (this.scrollButton != null)
      {
        Point point = new Point((int) Math.Min(Math.Max(0.0f, (float) this.scrollbarScrollArea / (float) this.scrollableArea * (float) this.scrollbarScrollArea), (float) this.scrollbarScrollArea), (int) this.Size.Y);
        this.scrollButton = new WindowLayer(new Rectangle(this.Location.X + (int) this.buttonLayerOne.Size.X, this.Location.Y, point.X, point.Y));
      }
      this.ScrollPosition = Math.Min(this.scrollableArea, this.ScrollPosition);
    }

    public override int ScrollPosition
    {
      get => base.ScrollPosition;
      set
      {
        this.scrollbarPosition = value;
        if (this.scrollbarPosition <= 0)
          this.scrollbarPosition = 0;
        else if (this.scrollbarPosition > this.scrollableArea)
          this.scrollbarPosition = this.scrollableArea;
        if (this.scrollButton != null)
        {
          if (this.scrollableArea == 0)
            this.scrollButton.Position = this.backgroundLayer.Position + Vector2.UnitX * this.buttonLayerOne.Size.X;
          else
            this.scrollButton.Position = this.backgroundLayer.Position + Vector2.UnitX * this.buttonLayerOne.Size.X + Vector2.UnitX * ((float) this.scrollbarScrollArea - this.scrollButton.Size.X) / (float) this.scrollableArea * (float) this.scrollbarPosition;
        }
        this.LaunchMovedEvent();
      }
    }
  }
}
