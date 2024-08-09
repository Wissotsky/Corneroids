// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.VerticalScrollbarLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class VerticalScrollbarLayer : ScrollbarLayer
  {
    public VerticalScrollbarLayer(Rectangle positionAndSize, int scrollableArea)
      : base(positionAndSize, scrollableArea)
    {
      Point point1 = new Point((int) this.Size.X, (int) this.Size.X);
      Point location = this.PositionAndSize.Location;
      Point point2 = new Point(this.Location.X, this.Location.Y + (int) this.Size.Y - point1.Y);
      this.buttonLayerOne = new ButtonLayer(new Rectangle(location.X, location.Y, point1.X, point1.Y), "^");
      this.buttonLayerTwo = new ButtonLayer(new Rectangle(point2.X, point2.Y, point1.X, point1.Y), "v");
      this.backgroundLayer = new FieldLayer(this.PositionAndSize);
      this.scrollbarScrollArea = Math.Max((int) this.Size.Y - 2 * point1.Y, 0);
      Point point3 = new Point((int) this.Size.X, (int) Math.Min(Math.Max(0.0f, (float) this.scrollbarScrollArea / (float) scrollableArea * (float) this.scrollbarScrollArea), (float) this.scrollbarScrollArea));
      this.scrollButton = new WindowLayer(new Rectangle(this.Location.X, this.Location.Y + point1.Y, point3.X, point3.Y));
    }

    public override void UpdateInput(MouseDevice mouse)
    {
      if (mouse.LeftDown())
      {
        if (this.buttonLayerOne.Contains(mouse.Position))
          this.ScrollUp();
        else if (this.buttonLayerTwo.Contains(mouse.Position))
          this.ScrollDown();
        else if (this.scrollButton.Contains(mouse.Position))
          this.ScrollPosition += (int) Math.Ceiling((double) mouse.GetTranslationY() * (double) this.scrollableArea / (double) ((float) this.scrollbarScrollArea - this.scrollButton.Size.Y));
        else if (this.backgroundLayer.Contains(mouse.Position))
        {
          int scrollbarScrollArea = this.scrollbarScrollArea;
          double y = (double) this.buttonLayerOne.Size.Y;
          this.ScrollPosition = (int) ((double) ((float) (mouse.Position.Y - (this.backgroundLayer.Location.Y + (int) this.buttonLayerOne.Size.Y)) / (float) this.scrollbarScrollArea) * (double) this.scrollableArea);
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
        Point point = new Point((int) this.Size.X, (int) Math.Min(Math.Max(0.0f, (float) this.scrollbarScrollArea / (float) this.scrollableArea * (float) this.scrollbarScrollArea), (float) this.scrollbarScrollArea));
        this.scrollButton = new WindowLayer(new Rectangle(this.Location.X, this.Location.Y + (int) this.buttonLayerOne.Size.Y, point.X, point.Y));
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
            this.scrollButton.Position = this.backgroundLayer.Position + Vector2.UnitY * this.buttonLayerOne.Size.Y;
          else
            this.scrollButton.Position = this.backgroundLayer.Position + Vector2.UnitY * this.buttonLayerOne.Size.Y + Vector2.UnitY * ((float) this.scrollbarScrollArea - this.scrollButton.Size.Y) / (float) this.scrollableArea * (float) this.scrollbarPosition;
        }
        this.LaunchMovedEvent();
      }
    }
  }
}
