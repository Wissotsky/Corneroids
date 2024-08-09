// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ComboboxScreen`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Screen
{
  public class ComboboxScreen<T> : MenuScreen
  {
    private ComboboxLayer<T> combobox;
    private Action<T> result;
    private TextBoxLayer textBox;
    private ScrollbarLayer scrollbar;

    public ComboboxScreen(ComboboxLayer<T> combobox, Action<T> chosenItem)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.combobox = combobox != null ? combobox : throw new ArgumentNullException();
      this.result = chosenItem;
    }

    public override void Dispose() => base.Dispose();

    public override void Load()
    {
      this.textBox = new TextBoxLayer(new Rectangle(this.combobox.Location.X, this.combobox.Location.Y + (int) this.combobox.Size.Y, (int) this.combobox.Size.X - 5, 150), this.combobox.MaxLength, 5);
      List<string> stringList = new List<string>();
      if (this.combobox.ItemsToChoose != null)
      {
        foreach (T obj in this.combobox.ItemsToChoose)
          stringList.Add(obj.ToString());
      }
      this.textBox.TextLines = stringList;
      this.scrollbar = (ScrollbarLayer) new VerticalScrollbarLayer(new Rectangle(this.textBox.Location.X + (int) this.textBox.Size.X, this.textBox.Location.Y, 25, (int) this.textBox.Size.Y), this.textBox.TextLines.Count * 30);
      this.textBox.SetScrollbar(this.scrollbar);
    }

    public override void Render()
    {
      base.Render();
      this.textBox.Render();
      this.scrollbar.Render();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.scrollbar.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick() && !this.Mouse.RightClick() || this.scrollbar.PositionAndSize.Contains(this.Mouse.Position))
        return;
      if (this.textBox.PositionAndSize.Contains(this.Mouse.Position))
      {
        int num = this.textBox.ScrollPosition + (this.Mouse.Position.Y - this.textBox.Location.Y) / (int) ((double) this.textBox.Size.Y / (double) this.textBox.MaxLines);
        if (num >= 0 && num < this.combobox.ItemsToChoose.Count)
        {
          this.combobox.SelectedIndex = num;
          if (this.result != null && this.combobox.SelectedIndex > -1)
            this.result(this.combobox.SelectedItem);
        }
        this.CloseScreen();
      }
      else
        this.CloseScreen();
    }
  }
}
