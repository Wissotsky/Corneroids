// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.TrashInventoryLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class TrashInventoryLayer : InventoryLayer
  {
    private Texture2D trashTexture;

    public TrashInventoryLayer(Point location)
      : base(new Inventory(1U, 1U, (Player) null), location)
    {
      this.trashTexture = Engine.ContentManager.Load<Texture2D>("Textures\\Sprites\\litterbin");
    }

    public override int AddItem(Point location, Item item, int count) => count;

    public override ItemSlot PopItems(Point location, int count) => new ItemSlot((Item) null, 0);

    public override void Render()
    {
      if (this.trashTexture != null)
      {
      Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
      Engine.SpriteBatch.Draw(this.trashTexture, this.PositionAndSize, Color.White);
      Engine.SpriteBatch.End();
      }
      base.Render();
    }
  }
}
