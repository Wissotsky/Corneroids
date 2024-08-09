// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ItemToolbarLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace CornerSpace.GUI
{
  public class ItemToolbarLayer : ItemLayer
  {
    private const int windowWidth = 512;
    private const int windowHeight = 64;
    private Inventory inventory;
    private Texture2D gridTexture;
    private Texture2D selectorTexture;
    private sbyte selectedBlockbarIndex;

    public ItemToolbarLayer()
      : base(new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 256, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight - 64 - 64, 512, 64))
    {
      this.gridTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/gridCell");
      this.selectorTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/square");
      this.selectedBlockbarIndex = (sbyte) 0;
      this.inventory = new Inventory(8U, 1U, (Player) null);
    }

    public override int AddItem(Point location, Item item, int count)
    {
      int index = this.PointToIndex(new Point(location.X - this.Location.X, location.Y - this.Location.Y));
      return index >= 0 ? this.AddItem(index, item, count) : 0;
    }

    public int AddItem(int index, Item item, int count)
    {
      return this.inventory.AddItems(item, count, index, 0);
    }

    public int AddItems(Item item, int count) => this.inventory.AddItems(item, count);

    public override void GetIndicesOfPosition(Point location, out byte x, out byte y)
    {
      x = y = (byte) 0;
      if (!this.IsValidIndex(location))
        return;
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      x = (byte) this.PointToIndex(location);
    }

    public override bool IsValidIndex(Point location)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      return this.PointToIndex(location) != -1;
    }

    public ItemSlot PeekItem(int index) => this.inventory.PeekItem(index, 0);

    public override ItemSlot PeekItem(Point location)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      return this.PeekItem(this.PointToIndex(location));
    }

    public ItemSlot PopItems(int index, int count) => this.inventory.PopItem(index, 0, count);

    public override ItemSlot PopItems(Point location, int count)
    {
      location = new Point(location.X - this.Location.X, location.Y - this.Location.Y);
      return this.PopItems(this.PointToIndex(location), count);
    }

    public override void Render()
    {
      Engine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
      Engine.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
      Engine.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
      for (int index = 0; index < 8; ++index)
      {
        ItemSlot itemSlot = this.inventory.Items[index, 0];
        Rectangle destinationRectangle = new Rectangle(this.PositionAndSize.X + index * 64, this.PositionAndSize.Y, 64, 64);
        Engine.SpriteBatch.Draw(this.gridTexture, destinationRectangle, Color.White);
        if (index == (int) this.selectedBlockbarIndex)
          Engine.SpriteBatch.Draw(this.selectorTexture, destinationRectangle, new Rectangle?(new Rectangle(0, 0, 32, 32)), Color.White);
        if (itemSlot != null)
        {
          Texture2D spriteAtlasTexture = itemSlot.Item.SpriteAtlasTexture;
          destinationRectangle.X += 4;
          destinationRectangle.Y += 4;
          destinationRectangle.Width -= 8;
          destinationRectangle.Height -= 8;
          Engine.SpriteBatch.Draw(spriteAtlasTexture, destinationRectangle, new Rectangle?(itemSlot.Item.SpriteCoordsRect), Color.White);
          Engine.SpriteBatch.DrawString(Engine.Font, itemSlot.Count.ToString(), new Vector2((float) destinationRectangle.X, (float) destinationRectangle.Y), Color.White);
        }
      }
      Engine.SpriteBatch.End();
    }

    public void Reposition()
    {
      this.PositionAndSize = new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 256, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight - 64 - 64, 512, 64);
    }

    public bool SpaceLeftForItem(Item item, int count)
    {
      return this.inventory.SpaceLeftForItem(item, count);
    }

    public sbyte GetClickedIndex(InputFrame input)
    {
      if (input.KeyDown(Keys.D1))
        return 0;
      if (input.KeyDown(Keys.D2))
        return 1;
      if (input.KeyDown(Keys.D3))
        return 2;
      if (input.KeyDown(Keys.D4))
        return 3;
      if (input.KeyDown(Keys.D5))
        return 4;
      if (input.KeyDown(Keys.D6))
        return 5;
      if (input.KeyDown(Keys.D7))
        return 6;
      return input.KeyDown(Keys.D8) ? (sbyte) 7 : (sbyte) -1;
    }

    public bool ValidIndex(int index) => index >= 0 && index < 8;

    public Inventory Items => this.inventory;

    public sbyte SelectedIndex
    {
      get => this.selectedBlockbarIndex;
      set => this.selectedBlockbarIndex = value;
    }

    private int PointToIndex(Point point)
    {
      return point.Y < 0 || point.Y > 64 || point.X < 0 || point.X > 512 ? -1 : point.X / 64;
    }
  }
}
