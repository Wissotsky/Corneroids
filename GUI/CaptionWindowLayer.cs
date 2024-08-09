// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.CaptionWindowLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class CaptionWindowLayer : WindowLayer
  {
    private Color borderColor;
    private string caption;
    private bool drawBorders;

    public CaptionWindowLayer(Rectangle position, string caption)
      : base(position)
    {
      this.caption = caption;
      this.borderColor = Color.DarkGray;
      this.drawBorders = true;
    }

    public override void Render()
    {
      base.Render();
      if (!this.drawBorders || this.blankTexture == null || (double) this.Size.X < 20.0 || (double) this.Size.Y < 20.0)
        return;
      int num = 0;
      Rectangle? nullable = new Rectangle?();
      Rectangle positionAndSize = this.PositionAndSize;
      if (!string.IsNullOrEmpty(this.caption))
      {
        Vector2 vector2 = Engine.Font.MeasureString(this.caption);
        num = 5;
        nullable = new Rectangle?(new Rectangle(positionAndSize.X + 20, positionAndSize.Y + 5 + num, (int) vector2.X + 10, 5));
      }
      Rectangle destinationRectangle1 = new Rectangle(positionAndSize.X + 5, positionAndSize.Y + 5 + num, positionAndSize.Width - 10, positionAndSize.Height - 10 - num);
      Rectangle destinationRectangle2 = new Rectangle(positionAndSize.X + 10, positionAndSize.Y + 10 + num, positionAndSize.Width - 20, positionAndSize.Height - 20 - num);
      Engine.SpriteBatch.Begin();
      Engine.SpriteBatch.Draw(this.blankTexture, destinationRectangle1, this.borderColor);
      Engine.SpriteBatch.Draw(this.blankTexture, destinationRectangle2, this.backgroundColor);
      if (nullable.HasValue)
      {
        Engine.SpriteBatch.Draw(this.blankTexture, nullable.Value, this.backgroundColor);
        if (!string.IsNullOrEmpty(this.caption))
          Engine.SpriteBatch.DrawString(Engine.Font, this.caption, this.Position + new Vector2(25f, 0.0f), Color.White);
      }
      Engine.SpriteBatch.End();
    }

    public Color BorderColor
    {
      get => this.borderColor;
      set => this.borderColor = value;
    }

    public string Caption
    {
      get => this.caption;
      set => this.caption = value;
    }

    public bool DrawBorders
    {
      set => this.drawBorders = value;
    }
  }
}
