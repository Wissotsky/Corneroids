// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.BlueprintLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace.GUI
{
  public class BlueprintLayer : Layer
  {
    private WindowLayer backgroundLayer;
    private Blueprint blueprint;
    private Inventory inventory;

    public BlueprintLayer(Inventory inventory)
      : base(Rectangle.Empty)
    {
      this.inventory = inventory;
    }

    public override void Render()
    {
      if (this.blueprint == null)
        return;
      Item obj = this.blueprint.Result.Item;
      int count = this.blueprint.Result.Count;
      this.backgroundLayer.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      Vector2 zero = Vector2.Zero;
      string text1 = obj.Name + " (x" + (object) count + ")";
      Vector2 vector2 = Engine.Font.MeasureString(text1);
      zero.X = Math.Max(zero.X, vector2.X);
      spriteBatch.DrawString(Engine.Font, text1, new Vector2((float) this.backgroundLayer.Location.X, (float) this.backgroundLayer.Location.Y), Color.White);
      spriteBatch.DrawString(Engine.Font, "Requires: ", new Vector2((float) this.backgroundLayer.Location.X, (float) (this.backgroundLayer.Location.Y + 40)), Color.White);
      Vector2 position = new Vector2((float) this.backgroundLayer.Location.X, (float) (this.backgroundLayer.Location.Y + 65));
      using (Dictionary<Item, int>.KeyCollection.Enumerator enumerator = this.blueprint.Ingredients.Keys.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Item item = enumerator.Current;
          int num = this.inventory.Where<ItemSlot>((Func<ItemSlot, bool>) (slot => slot.Item == item)).Sum<ItemSlot>((Func<ItemSlot, int>) (slot => slot.Count));
          int ingredient = this.blueprint.Ingredients[item];
          Color color = num >= ingredient ? Color.White : Color.Red;
          string text2 = item.Name + " (" + (object) num + "/" + (object) ingredient + ")";
          spriteBatch.DrawString(Engine.Font, text2, position, color);
          position.Y += 20f;
          zero.X = Math.Max(zero.X, Engine.Font.MeasureString(text2).X);
        }
      }
      spriteBatch.End();
      zero.Y = (float) ((double) position.Y - (double) this.backgroundLayer.Location.Y + 5.0);
      this.backgroundLayer.Size = zero;
    }

    public Blueprint Blueprint
    {
      get => this.blueprint;
      set => this.SetNewBlueprint(value);
    }

    public override Point Location
    {
      get => base.Location;
      set
      {
        base.Location = value;
        if (this.blueprint == null)
          return;
        this.backgroundLayer.Location = value;
      }
    }

    private void SetNewBlueprint(Blueprint blueprint)
    {
      if (this.blueprint != blueprint && blueprint != null)
      {
        Item obj = blueprint.Result.Item;
        int count = blueprint.Result.Count;
        string name = blueprint.Result.Item.Name;
        string displayName = count > 1 ? name + " (x" + (object) count + ")" : name;
        if (this.backgroundLayer == null)
          this.backgroundLayer = new WindowLayer(new Rectangle(0, 0, 200, 100));
      }
      this.blueprint = blueprint;
    }
  }
}
