// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ScrollbarLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public abstract class ScrollbarLayer : Layer
  {
    protected int scrollableArea;
    protected int scrollbarPosition;
    protected int scrollbarScrollArea;
    protected FieldLayer backgroundLayer;
    protected ButtonLayer buttonLayerOne;
    protected ButtonLayer buttonLayerTwo;
    protected WindowLayer scrollButton;

    public ScrollbarLayer(Rectangle positionAndSize, int scrollableArea)
      : base(positionAndSize)
    {
      if (positionAndSize.Width <= 0 || positionAndSize.Height <= 0)
        throw new ArgumentException();
      this.scrollableArea = scrollableArea;
      this.scrollbarPosition = 0;
    }

    public override void Render()
    {
      if (this.backgroundLayer != null)
        this.backgroundLayer.Render();
      if (this.scrollButton != null)
        this.scrollButton.Render();
      this.buttonLayerOne.Render();
      this.buttonLayerTwo.Render();
    }

    public void ScrollUp()
    {
      this.ScrollPosition -= (int) (0.10000000149011612 * (double) this.scrollableArea);
    }

    public void ScrollDown()
    {
      this.ScrollPosition += (int) (0.10000000149011612 * (double) this.scrollableArea);
    }

    public abstract void UpdateInput(MouseDevice mouse);

    public abstract void UpdateScrollableArea(int value);

    public event System.Action ScrollbarMovedEvent;

    public virtual int ScrollPosition
    {
      get => this.scrollbarPosition;
      set => this.scrollbarPosition = value;
    }

    public float ScrollPositionPercentage
    {
      get
      {
        return this.scrollableArea == 0 ? 0.0f : (float) this.ScrollPosition / (float) this.scrollableArea;
      }
      set
      {
        this.ScrollPosition = (int) ((double) MathHelper.Clamp(value, 0.0f, 1f) * (double) this.scrollableArea);
      }
    }

    protected void LaunchMovedEvent()
    {
      if (this.ScrollbarMovedEvent == null)
        return;
      this.ScrollbarMovedEvent();
    }

    public enum Direction
    {
      Horizontal,
      Vertical,
    }
  }
}
