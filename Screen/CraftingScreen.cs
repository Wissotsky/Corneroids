// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.CraftingScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace.Screen
{
  public class CraftingScreen : MenuScreen
  {
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonArmors;
    private ButtonLayer buttonConsoles;
    private ButtonLayer buttonPowers;
    private ButtonLayer buttonThrusters;
    private ButtonLayer buttonCannons;
    private ButtonLayer buttonLights;
    private ButtonLayer buttonDoors;
    private ButtonLayer buttonContainers;
    private ButtonLayer buttonWindows;
    private ButtonLayer buttonPlayerWeapons;
    private ButtonLayer buttonPlayerArmors;
    private ButtonLayer buttonPlayerTools;
    private ButtonLayer buttonPlayerConsumables;
    private ButtonLayer buttonMaterials;
    private ButtonLayer buttonBack;
    private ButtonLayer[] buttonsList;
    private List<Blueprint> blueprints;
    private Inventory inventory;

    public CraftingScreen(Inventory inventory, List<Blueprint> blueprints)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.blueprints = blueprints;
      this.inventory = inventory;
    }

    public override void Load()
    {
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(630, 350), "Crafting");
      Point point = new Point(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 60);
      ButtonLayer buttonLayer1 = new ButtonLayer(new Rectangle(point.X, point.Y, 90, 50), "Plating");
      buttonLayer1.Tag = (object) typeof (BasicBlockType);
      this.buttonArmors = buttonLayer1;
      ButtonLayer buttonLayer2 = new ButtonLayer(new Rectangle(point.X + 100, point.Y, 90, 50), "Power");
      buttonLayer2.Tag = (object) typeof (PowerBlockType);
      this.buttonPowers = buttonLayer2;
      ButtonLayer buttonLayer3 = new ButtonLayer(new Rectangle(point.X + 200, point.Y, 90, 50), "Thrusters");
      buttonLayer3.Tag = (object) typeof (EngineBlockType);
      this.buttonThrusters = buttonLayer3;
      ButtonLayer buttonLayer4 = new ButtonLayer(new Rectangle(point.X + 300, point.Y, 90, 50), "Consoles");
      buttonLayer4.Tag = (object) typeof (ControlBlockType);
      this.buttonConsoles = buttonLayer4;
      ButtonLayer buttonLayer5 = new ButtonLayer(new Rectangle(point.X + 400, point.Y, 90, 50), "Cannons");
      buttonLayer5.Tag = (object) typeof (GunBlockType);
      this.buttonCannons = buttonLayer5;
      ButtonLayer buttonLayer6 = new ButtonLayer(new Rectangle(point.X + 500, point.Y, 90, 50), "Lights");
      buttonLayer6.Tag = (object) typeof (PointLightBlockType);
      this.buttonLights = buttonLayer6;
      ButtonLayer buttonLayer7 = new ButtonLayer(new Rectangle(point.X, point.Y + 60, 90, 50), "Doors");
      buttonLayer7.Tag = (object) typeof (DoorBlockType);
      this.buttonDoors = buttonLayer7;
      ButtonLayer buttonLayer8 = new ButtonLayer(new Rectangle(point.X + 100, point.Y + 60, 90, 50), "Chests");
      buttonLayer8.Tag = (object) typeof (ContainerBlockType);
      this.buttonContainers = buttonLayer8;
      ButtonLayer buttonLayer9 = new ButtonLayer(new Rectangle(point.X + 200, point.Y + 60, 90, 50), "Windows");
      buttonLayer9.Tag = (object) typeof (WindowBlockType);
      this.buttonWindows = buttonLayer9;
      ButtonLayer buttonLayer10 = new ButtonLayer(new Rectangle(point.X, point.Y + 170, 90, 50), "Armors");
      buttonLayer10.Tag = (object) typeof (WearableItem);
      this.buttonPlayerArmors = buttonLayer10;
      ButtonLayer buttonLayer11 = new ButtonLayer(new Rectangle(point.X + 100, point.Y + 170, 90, 50), "Drills");
      buttonLayer11.Tag = (object) typeof (DrillBlockTool);
      this.buttonPlayerTools = buttonLayer11;
      ButtonLayer buttonLayer12 = new ButtonLayer(new Rectangle(point.X + 200, point.Y + 170, 90, 50), "Weapons");
      buttonLayer12.Tag = (object) typeof (WeaponItem);
      this.buttonPlayerWeapons = buttonLayer12;
      ButtonLayer buttonLayer13 = new ButtonLayer(new Rectangle(point.X + 300, point.Y + 170, 90, 50), "Buffs");
      buttonLayer13.Tag = (object) typeof (ConsumableItem);
      this.buttonPlayerConsumables = buttonLayer13;
      ButtonLayer buttonLayer14 = new ButtonLayer(new Rectangle(point.X + 400, point.Y + 170, 90, 50), "Materials");
      buttonLayer14.Tag = (object) typeof (InventoryItem);
      this.buttonMaterials = buttonLayer14;
      this.buttonBack = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 70, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 40, 140, 20), "Back");
      ButtonLayer[] buttonLayerArray1 = new ButtonLayer[18]
      {
        this.buttonArmors,
        this.buttonPowers,
        this.buttonThrusters,
        this.buttonConsoles,
        this.buttonCannons,
        this.buttonLights,
        this.buttonDoors,
        this.buttonContainers,
        this.buttonWindows,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null
      };
      ButtonLayer[] buttonLayerArray2 = buttonLayerArray1;
      ButtonLayer buttonLayer15 = new ButtonLayer(new Rectangle(point.X + 300, point.Y + 60, 90, 50), "Crafting");
      buttonLayer15.Tag = (object) typeof (CraftingBlockType);
      ButtonLayer buttonLayer16 = buttonLayer15;
      buttonLayerArray2[9] = buttonLayer16;
      ButtonLayer[] buttonLayerArray3 = buttonLayerArray1;
      ButtonLayer buttonLayer17 = new ButtonLayer(new Rectangle(point.X + 400, point.Y + 60, 90, 50), "Cameras");
      buttonLayer17.Tag = (object) typeof (CameraBlockType);
      ButtonLayer buttonLayer18 = buttonLayer17;
      buttonLayerArray3[10] = buttonLayer18;
      ButtonLayer[] buttonLayerArray4 = buttonLayerArray1;
      ButtonLayer buttonLayer19 = new ButtonLayer(new Rectangle(point.X + 500, point.Y + 60, 90, 50), "Gun Console");
      buttonLayer19.Tag = (object) typeof (GunControlBlockType);
      ButtonLayer buttonLayer20 = buttonLayer19;
      buttonLayerArray4[11] = buttonLayer20;
      buttonLayerArray1[12] = this.buttonPlayerArmors;
      buttonLayerArray1[13] = this.buttonPlayerConsumables;
      buttonLayerArray1[14] = this.buttonPlayerTools;
      buttonLayerArray1[15] = this.buttonPlayerWeapons;
      buttonLayerArray1[16] = this.buttonMaterials;
      buttonLayerArray1[17] = this.buttonBack;
      this.buttonsList = buttonLayerArray1;
    }

    public override void Render()
    {
      this.backgroundLayer.Render();
      foreach (Layer buttons in this.buttonsList)
        buttons.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Blocks:", this.backgroundLayer.Position + new Vector2(20f, 30f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Player items:", this.backgroundLayer.Position + new Vector2(20f, 200f), Color.White);
      spriteBatch.End();
      base.Render();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      foreach (ButtonLayer buttons in this.buttonsList)
        buttons.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonBack.Contains(this.Mouse.Position))
      {
        this.CloseScreen();
      }
      else
      {
        foreach (ButtonLayer buttons in this.buttonsList)
        {
          if (buttons.Contains(this.Mouse.Position))
          {
            System.Type itemType = buttons.Tag as System.Type;
            List<Blueprint> list = this.blueprints.Where<Blueprint>((Func<Blueprint, bool>) (blueprint => blueprint.Result.Item.GetType() == itemType)).ToList<Blueprint>();
            if (list.Count <= 0)
              break;
            Engine.LoadNewScreen((GameScreen) new BlueprintsScreen(this.inventory, list, "Craft: "));
            break;
          }
        }
      }
    }
  }
}
