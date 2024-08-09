// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.CraftingLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace.GUI
{
  public class CraftingLayer : CaptionWindowLayer
  {
    private ButtonLayer craftButton;
    private InventoryLayer resultLayer;
    private Point craftingGridSize;
    private InventoryLayer ingredientsLayer;

    public CraftingLayer(int gridWidth, int gridHeight, Point location)
      : base(new Rectangle(location.X, location.Y, gridWidth * 64 + 64, gridHeight * 64 + 64 + 120), "Crafting")
    {
      this.craftingGridSize = new Point(gridWidth, gridHeight);
      Inventory inventory1 = new Inventory((uint) this.craftingGridSize.X, (uint) this.craftingGridSize.Y, (Player) null);
      Inventory inventory2 = new Inventory(1U, 1U, (Player) null);
      this.ingredientsLayer = new InventoryLayer(inventory1, Point.Zero);
      this.resultLayer = new InventoryLayer(inventory2, Point.Zero);
      this.craftButton = new ButtonLayer(new Rectangle(this.Location.X + (int) this.Size.X / 2 - 50, this.Location.Y + (int) this.ingredientsLayer.Size.Y + 48, 100, 25), "Craft!");
      Vector2 vector2_1 = this.Position + new Vector2((float) ((double) this.Size.X / 2.0 - (double) this.ingredientsLayer.Size.X / 2.0), 32f);
      Vector2 vector2_2 = new Vector2((float) (this.Location.X + (int) this.Size.X / 2 - (int) this.resultLayer.Size.X / 2), (float) (this.Location.Y + (int) this.Size.Y - 125 + 32));
      this.ingredientsLayer.Position = vector2_1;
      this.resultLayer.Position = vector2_2;
      this.layers.Add((Layer) this.craftButton);
      this.layers.Add((Layer) this.ingredientsLayer);
      this.layers.Add((Layer) this.resultLayer);
    }

    public override void Render()
    {
      base.Render();
      this.craftButton.Render();
      this.ingredientsLayer.Render();
      this.resultLayer.Render();
    }

    public InventoryLayer CraftingResultLayer => this.resultLayer;

    public InventoryLayer CraftingIngredientsLayer => this.ingredientsLayer;

    public override Point Location
    {
      set => base.Location = value;
    }
  }
}
