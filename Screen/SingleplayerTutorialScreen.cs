// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.SingleplayerTutorialScreen
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
  public class SingleplayerTutorialScreen : MenuScreen
  {
    private const int screenWidth = 600;
    private const int screenHeight = 500;
    private WindowLayer backgroundLayer;
    private FieldLayer backgroundFieldLayer;
    private ButtonLayer buttonStart;
    private Item[] exampleBlocks;

    public SingleplayerTutorialScreen()
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.exampleBlocks = new Item[4];
    }

    public override void Load()
    {
      base.Load();
      this.backgroundLayer = new WindowLayer(Layer.EvaluateMiddlePosition(620, 520));
      this.backgroundFieldLayer = new FieldLayer(Layer.EvaluateMiddlePosition(600, 500));
      this.buttonStart = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 100, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 50, 200, 30), "Start!");
      if (Engine.LoadedWorld == null)
        return;
      Itemset itemset = Engine.LoadedWorld.Itemset;
      this.exampleBlocks = new Item[3]
      {
        ((IEnumerable<Item>) itemset.Items).Where<Item>((Func<Item, bool>) (i => i is PowerBlockType)).First<Item>(),
        ((IEnumerable<Item>) itemset.Items).Where<Item>((Func<Item, bool>) (i => i is EngineBlockType)).First<Item>(),
        ((IEnumerable<Item>) itemset.Items).Where<Item>((Func<Item, bool>) (i => i is GunBlockType)).First<Item>()
      };
    }

    public override void Render()
    {
      this.backgroundLayer.Render();
      this.backgroundFieldLayer.Render();
      this.buttonStart.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
      Engine.SetPointSamplerStateForSpritebatch();
      spriteBatch.DrawString(Engine.Font, "Welcome to Corneroids!", this.backgroundFieldLayer.Position + Vector2.One * 10f, Color.White);
      spriteBatch.DrawString(Engine.Font, "In this sandbox game mode you can mine, craft, ", this.backgroundFieldLayer.Position + new Vector2(10f, 50f), Color.White);
      spriteBatch.DrawString(Engine.Font, "build, fight and explore the almost infinite galaxy!", this.backgroundFieldLayer.Position + new Vector2(10f, 75f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Some special blocks:", this.backgroundFieldLayer.Position + new Vector2(10f, 130f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Some blocks such as thrusters and cannons", this.backgroundFieldLayer.Position + new Vector2(80f, 170f), Color.White);
      spriteBatch.DrawString(Engine.Font, "requires power to use. Build power generators", this.backgroundFieldLayer.Position + new Vector2(80f, 195f), Color.White);
      spriteBatch.DrawString(Engine.Font, "to give them power!", this.backgroundFieldLayer.Position + new Vector2(80f, 220f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Thruster blocks are used to create motion!", this.backgroundFieldLayer.Position + new Vector2(80f, 260f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Combine them with console blocks", this.backgroundFieldLayer.Position + new Vector2(80f, 285f), Color.White);
      spriteBatch.DrawString(Engine.Font, "to navigate around the space.", this.backgroundFieldLayer.Position + new Vector2(80f, 310f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Equip your spaceship with cannons and show", this.backgroundFieldLayer.Position + new Vector2(80f, 350f), Color.White);
      spriteBatch.DrawString(Engine.Font, "the true firepower of your ship to your enemies!", this.backgroundFieldLayer.Position + new Vector2(80f, 375f), Color.White);
      spriteBatch.DrawString(Engine.Font, "You can reopen this screen by pressing F1", this.backgroundFieldLayer.Position + new Vector2(10f, 415f), Color.White);
      if (this.exampleBlocks[0] != (Item) null)
        spriteBatch.Draw(this.exampleBlocks[0].SpriteAtlasTexture, new Rectangle(this.backgroundFieldLayer.Location.X + 10, this.backgroundFieldLayer.Location.Y + 170, 64, 64), new Rectangle?(this.exampleBlocks[0].SpriteCoordsRect), Color.White);
      if (this.exampleBlocks[1] != (Item) null)
        spriteBatch.Draw(this.exampleBlocks[1].SpriteAtlasTexture, new Rectangle(this.backgroundFieldLayer.Location.X + 10, this.backgroundFieldLayer.Location.Y + 260, 64, 64), new Rectangle?(this.exampleBlocks[1].SpriteCoordsRect), Color.White);
      if (this.exampleBlocks[2] != (Item) null)
        spriteBatch.Draw(this.exampleBlocks[2].SpriteAtlasTexture, new Rectangle(this.backgroundFieldLayer.Location.X + 10, this.backgroundFieldLayer.Location.Y + 350, 64, 64), new Rectangle?(this.exampleBlocks[2].SpriteCoordsRect), Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonStart.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick() || !this.buttonStart.Contains(this.Mouse.Position))
        return;
      this.CloseScreen();
    }
  }
}
