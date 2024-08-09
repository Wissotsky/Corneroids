// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.TextBoxLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

#nullable disable
namespace CornerSpace.GUI
{
  public class TextBoxLayer : FieldLayer
  {
    private List<string> textLines;
    private uint maxLength;
    private int maxLines;
    private bool scrollbar;
    private int scrollPosition;
    private ScrollbarLayer scrollbarLayer;
    private StringBuilder stringBuilder;

    public TextBoxLayer(Rectangle position, uint maxLength, int maxLines)
      : base(position)
    {
      this.maxLength = maxLength;
      this.textLines = new List<string>();
      this.stringBuilder = new StringBuilder();
      this.maxLines = maxLines;
      this.scrollPosition = 0;
      if (maxLines > 0)
        return;
      maxLines = 1;
    }

    public override void Render()
    {
      base.Render();
      if (this.textLines.Count == 0)
        return;
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      int num1 = 0;
      int num2 = 0;
      Rectangle positionAndSize = this.PositionAndSize;
      for (int scrollPosition = this.scrollPosition; scrollPosition < Math.Min(this.textLines.Count, this.scrollPosition + this.maxLines); ++scrollPosition)
      {
        string textLine = this.textLines[scrollPosition];
        if (num2 < this.maxLines)
        {
          this.stringBuilder.Remove(0, this.stringBuilder.Length);
          for (int index = textLine.Length - 1; index >= 0; --index)
          {
            this.stringBuilder.Insert(0, textLine[index]);
            if ((double) Engine.Font.MeasureString(this.stringBuilder).X >= (double) positionAndSize.Width)
              break;
          }
          Vector2 vector2 = Engine.Font.MeasureString(this.stringBuilder);
          int y = positionAndSize.Y + num1++ * 30;
          if ((double) (positionAndSize.Y + positionAndSize.Height) >= (double) y + (double) vector2.Y)
            spriteBatch.DrawString(Engine.Font, this.stringBuilder.ToString(), new Vector2((float) positionAndSize.X, (float) y), Color.White);
          ++num2;
        }
      }
      spriteBatch.End();
    }

    public void SetScrollbar(ScrollbarLayer scrollbar)
    {
      if (this.scrollbarLayer != null)
        this.scrollbarLayer.ScrollbarMovedEvent -= new System.Action(this.Scrolled);
      this.scrollbarLayer = scrollbar;
      this.scrollbarLayer.ScrollbarMovedEvent += new System.Action(this.Scrolled);
    }

    public uint MaxLength => this.maxLength;

    public int MaxLines => this.maxLines;

    public virtual List<string> TextLines
    {
      get => this.textLines;
      set
      {
        this.textLines.Clear();
        foreach (string str in value)
        {
          if ((long) str.Length > (long) this.maxLength)
            this.textLines.Add(str.Substring(0, (int) this.maxLength));
          else
            this.textLines.Add(str);
        }
      }
    }

    public virtual string Text
    {
      set
      {
        this.textLines.Clear();
        if (value == null)
          return;
        if ((long) value.Length > (long) this.maxLength)
          this.textLines.Add(value.Substring(0, (int) this.maxLength));
        else
          this.textLines.Add(value);
      }
      get => this.textLines.Count > 0 ? this.textLines[0] : string.Empty;
    }

    public int ScrollPosition => this.scrollPosition;

    private void Scrolled()
    {
      this.scrollPosition = (int) Math.Max(this.scrollbarLayer.ScrollPositionPercentage * (float) (this.textLines.Count - this.maxLines), 0.0f);
    }
  }
}
