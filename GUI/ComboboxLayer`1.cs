// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ComboboxLayer`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.GUI
{
  public class ComboboxLayer<T> : TextBoxLayer
  {
    private List<T> choseableLines;
    private ButtonLayer expandButton;
    private int selectedIndex;

    public ComboboxLayer(Rectangle position, uint maxLength)
      : base(position, maxLength, 1)
    {
      this.expandButton = new ButtonLayer(new Rectangle(0, 0, 30, 30), "V");
      this.selectedIndex = 0;
      this.layers.Add((Layer) this.expandButton);
      this.UpdateExpandButtonPosition();
    }

    public void Expand()
    {
      Engine.LoadNewScreen((GameScreen) new ComboboxScreen<T>(this, (Action<T>) (result => { })));
    }

    public override void Render()
    {
      base.Render();
      foreach (Layer layer in this.layers)
        layer.Render();
    }

    public override Point Location
    {
      get => base.Location;
      set
      {
        base.Location = value;
        this.UpdateExpandButtonPosition();
      }
    }

    public override Vector2 Position
    {
      get => base.Position;
      set
      {
        base.Position = value;
        this.UpdateExpandButtonPosition();
      }
    }

    public override Vector2 Size
    {
      get => base.Size;
      set
      {
        base.Size = value;
        this.UpdateExpandButtonPosition();
      }
    }

    public override Rectangle PositionAndSize
    {
      get => base.PositionAndSize;
      set
      {
        base.PositionAndSize = value;
        this.UpdateExpandButtonPosition();
      }
    }

    public int SelectedIndex
    {
      get => this.selectedIndex;
      set
      {
        this.selectedIndex = value;
        if (this.selectedIndex >= 0 && this.selectedIndex < this.choseableLines.Count)
          this.Text = this.choseableLines[this.selectedIndex].ToString();
        else
          this.selectedIndex = -1;
      }
    }

    public T SelectedItem
    {
      get
      {
        int selectedIndex = this.SelectedIndex;
        return selectedIndex > -1 ? this.choseableLines[selectedIndex] : default (T);
      }
      set
      {
        int num = this.choseableLines.IndexOf(value);
        if (num < 0)
          return;
        this.SelectedIndex = num;
      }
    }

    public List<T> ItemsToChoose
    {
      get => this.choseableLines;
      set
      {
        this.choseableLines = value;
        this.SelectedIndex = 0;
      }
    }

    private void UpdateExpandButtonPosition()
    {
      if (this.expandButton == null)
        return;
      this.expandButton.Location = new Point(this.Location.X + (int) this.Size.X - 30, this.Location.Y);
    }
  }
}
