// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ColorKeyGroupLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace CornerSpace.GUI
{
  public class ColorKeyGroupLayer : Layer
  {
    private Color color;
    private Texture2D texture;
    private Keys key;

    public ColorKeyGroupLayer(Rectangle positionAndSize, string name, Color color)
      : base(positionAndSize)
    {
      this.Name = name;
      this.color = color;
      this.texture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
    }

    public override void Render()
    {
      Rectangle positionAndSize = this.PositionAndSize;
      Rectangle destinationRectangle1 = new Rectangle(positionAndSize.X, positionAndSize.Y, 30, 30);
      Rectangle destinationRectangle2 = new Rectangle(positionAndSize.X + 5, positionAndSize.Y + 5, 30, 30);
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.Draw(this.texture, destinationRectangle2, Color.Gray);
      spriteBatch.Draw(this.texture, destinationRectangle1, this.color);
      spriteBatch.DrawString(Engine.Font, "=", new Vector2((float) (positionAndSize.X + 45), (float) (positionAndSize.Y + 5)), Color.White);
      spriteBatch.DrawString(Engine.Font, this.key.ToString(), new Vector2((float) (positionAndSize.X + 65), (float) (positionAndSize.Y + 5)), Color.White);
      spriteBatch.End();
    }

    public Keys Key
    {
      get => this.key;
      set => this.key = value;
    }

    public Color Color => this.color;

    private enum ControlKey
    {
      Q = 81, // 0x00000051
      W = 87, // 0x00000057
    }
  }
}
