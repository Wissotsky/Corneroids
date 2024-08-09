// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.BlueprintsScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace.Screen
{
  public class BlueprintsScreen : ItemScreen
  {
    private const int gridWidth = 9;
    private const int gridHeight = 7;
    private CaptionWindowLayer bgLayer;
    private Texture2D blankTexture;
    private List<ButtonLayer> blueprintButtonLayers;
    private BlueprintLayer blueprintLayer;
    private List<Blueprint> blueprintsPool;
    private ButtonLayer buttonBack;
    private FieldLayer fieldLayer;
    private ItemDescriptionLayer itemLayer;
    private Inventory inventory;
    private List<Blueprint> materialsFor;

    public BlueprintsScreen(Inventory inventory, List<Blueprint> blueprintsPool, string caption)
      : base(GameScreen.Type.Popup, false, (Character) null, (EnvironmentManager) null)
    {
      this.inventory = inventory != null ? inventory : throw new ArgumentNullException();
      this.blueprintsPool = blueprintsPool ?? new List<Blueprint>();
      this.materialsFor = new List<Blueprint>();
      this.blueprintButtonLayers = new List<ButtonLayer>();
      this.UpdateAffordableBlueprints();
      this.bgLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(616, 544), caption);
      this.fieldLayer = new FieldLayer(new Rectangle(this.bgLayer.Location.X + 20, this.bgLayer.Location.Y + 30, (int) this.bgLayer.Size.X - 40, (int) this.bgLayer.Size.Y - 85));
      this.buttonBack = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 - 80, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 40, 160, 20), "Back");
      this.blueprintLayer = new BlueprintLayer(this.inventory);
      this.itemLayer = new ItemDescriptionLayer((Item) null);
      for (int index = 0; index < this.blueprintsPool.Count; ++index)
      {
        int num1 = index % 9 * 64;
        int num2 = index / 9 * 64;
        List<ButtonLayer> blueprintButtonLayers = this.blueprintButtonLayers;
        ButtonLayer buttonLayer1 = new ButtonLayer(new Rectangle(this.fieldLayer.Location.X + 5 + num1, this.fieldLayer.Location.Y + 5 + num2, 54, 54), string.Empty);
        buttonLayer1.Tag = (object) blueprintsPool[index];
        ButtonLayer buttonLayer2 = buttonLayer1;
        blueprintButtonLayers.Add(buttonLayer2);
      }
      this.UpdateAffordableBlueprints();
      this.blankTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
    }

    public override void Dispose() => base.Dispose();

    public override void Render()
    {
      this.bgLayer.Render();
      this.fieldLayer.Render();
      this.buttonBack.Render();
      foreach (Layer blueprintButtonLayer in this.blueprintButtonLayers)
        blueprintButtonLayer.Render();
      Engine.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
      Engine.SetPointSamplerStateForSpritebatch();
      for (int index = 0; index < this.blueprintsPool.Count; ++index)
      {
        if (this.blueprintsPool[index].Result != null)
        {
          Item obj = this.blueprintsPool[index].Result.Item;
          int count = this.blueprintsPool[index].Result.Count;
          Rectangle destinationRectangle = new Rectangle(this.blueprintButtonLayers[index].Location.X, this.blueprintButtonLayers[index].Location.Y, 54, 54);
          Engine.SpriteBatch.Draw(this.blueprintsPool[index].Result.Item.SpriteAtlasTexture, destinationRectangle, new Rectangle?(this.blueprintsPool[index].Result.Item.SpriteCoordsRect), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
        }
      }
      Engine.SpriteBatch.End();
      this.blueprintLayer.Render();
      this.itemLayer.Render();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (this.Keyboard.KeyPressed(Keys.Escape))
        this.CloseScreen();
      this.itemLayer.Location = new Point(this.Mouse.Position.X + 30, this.Mouse.Position.Y);
      bool flag = false;
      foreach (ButtonLayer blueprintButtonLayer in this.blueprintButtonLayers)
      {
        blueprintButtonLayer.UpdateInput(this.Mouse);
        if (blueprintButtonLayer.Contains(this.Mouse.Position))
        {
          this.blueprintLayer.Blueprint = blueprintButtonLayer.Tag as Blueprint;
          this.blueprintLayer.Location = new Point(this.Mouse.Position.X + 30, this.itemLayer.Location.Y + (int) this.itemLayer.Size.Y + 10);
          this.itemLayer.Item = this.blueprintLayer.Blueprint.Result.Item;
          flag = true;
        }
      }
      if (!flag)
      {
        this.blueprintLayer.Blueprint = (Blueprint) null;
        this.itemLayer.Item = (Item) null;
      }
      this.buttonBack.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick() && !this.Mouse.RightClick())
        return;
      if (this.buttonBack.Contains(this.Mouse.Position))
      {
        this.CloseScreen();
      }
      else
      {
        foreach (ButtonLayer blueprintButtonLayer in this.blueprintButtonLayers)
        {
          if (blueprintButtonLayer.Contains(this.Mouse.Position))
          {
            Blueprint tag = blueprintButtonLayer.Tag as Blueprint;
            int num = 0;
            int resultMultiplier = tag.GetResultMultiplier(this.inventory.ItemsArray);
            if (this.Mouse.LeftClick())
              num = Math.Min(1, resultMultiplier);
            else if (this.Mouse.RightClick() && resultMultiplier > 0)
              num = Math.Max(resultMultiplier / 2, 1);
            if (this.inventory.SpaceLeftForItem(tag.Result.Item, num * tag.Result.Count))
            {
              this.inventory.AddItems(tag.Result.Item, num * tag.Result.Count);
              foreach (KeyValuePair<Item, int> ingredient in tag.Ingredients)
                this.inventory.RemoveItem(ingredient.Key, num * ingredient.Value);
              this.UpdateAffordableBlueprints();
              break;
            }
            Engine.MessageBoxShow("Inventory is full!");
            break;
          }
        }
      }
    }

    protected override Inventory.Type GetInventoryTypeOfLayer(ItemLayer layer, Point location)
    {
      Engine.MessageBoxShow("Feature not implemented yet");
      return Inventory.Type.Inventory;
    }

    private List<Blueprint> GetMatchingBlueprints(
      List<Blueprint> allBlueprints,
      Inventory inventory)
    {
      if (allBlueprints == null || inventory == null)
        return new List<Blueprint>();
      ItemSlot[] itemsInInventory = inventory.ItemsArray;
      return allBlueprints.Where<Blueprint>((Func<Blueprint, bool>) (b => b.GetResultMultiplier(itemsInInventory) > 0)).ToList<Blueprint>();
    }

    private void UpdateAffordableBlueprints()
    {
      ItemSlot[] items = this.inventory.ItemsArray;
      this.materialsFor = this.blueprintsPool.Where<Blueprint>((Func<Blueprint, bool>) (bp => bp.GetResultMultiplier(items) > 0)).ToList<Blueprint>();
      foreach (ButtonLayer blueprintButtonLayer in this.blueprintButtonLayers)
        blueprintButtonLayer.ForceChecked = !this.materialsFor.Contains(blueprintButtonLayer.Tag as Blueprint);
    }
  }
}
