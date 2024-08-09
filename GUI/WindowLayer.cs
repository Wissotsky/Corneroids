// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.WindowLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class WindowLayer : Layer
  {
    protected Color darkBorderColor = new Color((byte) 127, (byte) 127, (byte) 127);
    protected Color backgroundColor = new Color((byte) 190, (byte) 190, (byte) 190);
    protected Color lightBorderColor = new Color((byte) 228, (byte) 228, (byte) 228);
    protected Texture2D blankTexture;

    public WindowLayer(Rectangle position)
      : base(position)
    {
      this.blankTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
    }

    public override void Render()
    {
      Rectangle positionAndSize = this.PositionAndSize;
      Rectangle destinationRectangle1 = new Rectangle(positionAndSize.X - 5, positionAndSize.Y - 5, positionAndSize.Width + 10, positionAndSize.Height + 10);
      Rectangle destinationRectangle2 = new Rectangle(positionAndSize.X - 5, positionAndSize.Y - 5, positionAndSize.Width + 5, positionAndSize.Height + 5);
      Rectangle destinationRectangle3 = new Rectangle(positionAndSize.X, positionAndSize.Y, positionAndSize.Width, positionAndSize.Height);
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.Draw(this.blankTexture, destinationRectangle1, this.darkBorderColor);
      spriteBatch.Draw(this.blankTexture, destinationRectangle2, this.lightBorderColor);
      spriteBatch.Draw(this.blankTexture, destinationRectangle3, this.backgroundColor);
      spriteBatch.End();
    }
  }
}
