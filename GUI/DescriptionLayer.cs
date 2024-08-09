// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.DescriptionLayer
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
  public class DescriptionLayer : WindowLayer
  {
    private List<string> lines;
    private float lineSize;

    public DescriptionLayer(List<string> lines)
      : base(Rectangle.Empty)
    {
      this.lines = new List<string>();
      this.Lines = lines;
    }

    public void Addline(string line)
    {
      if (!string.IsNullOrEmpty(line))
        this.lines.Add(line);
      this.UpdateLines();
    }

    public override void Render()
    {
      base.Render();
      if (this.lines == null)
        return;
      Vector2 position = this.Position;
      Engine.SpriteBatch.Begin();
      foreach (string line in this.lines)
      {
        if (!string.IsNullOrEmpty(line))
        {
          Engine.SpriteBatch.DrawString(Engine.Font, line, position, Color.White);
          position += Vector2.UnitY * this.lineSize;
        }
      }
      Engine.SpriteBatch.End();
    }

    public void SetLine(string line)
    {
      this.lines.Clear();
      if (!string.IsNullOrEmpty(line))
        this.lines.Add(line);
      this.UpdateLines();
    }

    public List<string> Lines
    {
      set
      {
        this.lines.Clear();
        if (value == null)
          return;
        foreach (string str in value)
          this.lines.Add(str);
        this.UpdateLines();
      }
    }

    private void UpdateLines()
    {
      Point point = new Point(10, 10);
      foreach (string line in this.lines)
      {
        if (!string.IsNullOrEmpty(line))
        {
          Vector2 vector2 = Engine.Font.MeasureString(line);
          point = new Point((int) Math.Max((float) point.X, vector2.X), (int) ((double) point.Y + (double) vector2.Y));
          this.lineSize = (float) (int) vector2.Y;
        }
      }
      this.Size = new Vector2((float) point.X, (float) point.Y);
    }
  }
}
