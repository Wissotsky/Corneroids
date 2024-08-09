// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ItemLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public abstract class ItemLayer : Layer
  {
    private Texture2D gridTexture;

    public ItemLayer(Rectangle window)
      : base(window)
    {
      this.gridTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/gridCell");
    }

    public abstract int AddItem(Point location, Item item, int count);

    public virtual void GetIndicesOfPosition(Point location, out byte x, out byte y)
    {
      x = y = (byte) 0;
    }

    public abstract bool IsValidIndex(Point location);

    public abstract ItemSlot PeekItem(Point location);

    public abstract ItemSlot PopItems(Point location, int count);

    protected void RenderGrid(Vector2 location, Vector2 size, int cellSize)
    {
      if (this.gridTexture == null)
        return;
      Engine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
      Engine.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
      for (int index1 = 0; (double) index1 < (double) size.Y; ++index1)
      {
        for (int index2 = 0; (double) index2 < (double) size.X; ++index2)
        {
          Rectangle destinationRectangle = new Rectangle((int) location.X + index2 * cellSize, (int) location.Y + index1 * cellSize, cellSize, cellSize);
          Engine.SpriteBatch.Draw(this.gridTexture, destinationRectangle, Color.White);
        }
      }
      Engine.SpriteBatch.End();
    }
  }
}
