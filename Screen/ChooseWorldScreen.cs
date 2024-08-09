// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ChooseWorldScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Screen
{
  public class ChooseWorldScreen : MenuScreen
  {
    private const string sWorlds = "Worlds:";
    private Texture2D blankTexture;
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonBack;
    private ButtonLayer buttonCreate;
    private ButtonLayer buttonPlay;
    private ButtonLayer buttonRemove;
    private ScrollbarLayer scrollbarLayer;
    private CaptionWindowLayer selectedWorldLayer;
    private TextBoxLayer worldsLayer;
    private int clickedTextboxIndex;
    private List<World> savedWorlds;
    private World selectedWorld;
    private Action<World> resultDelegate;

    public ChooseWorldScreen(Action<World> resultDelegate)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.clickedTextboxIndex = -1;
      this.selectedWorld = (World) null;
      this.resultDelegate = resultDelegate;
    }

    public override void Load()
    {
      this.blankTexture = Engine.ContentManager.Load<Texture2D>("Textures\\sprites\\blank");
      this.savedWorlds = this.CreateWorlds(WorldIOManager.Instance.GetSavedWorlds(Engine.WorldVersion));
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(500, 655), "Choose world");
      this.worldsLayer = new TextBoxLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + 60, 460, 300), 50U, 10);
      this.selectedWorldLayer = new CaptionWindowLayer(new Rectangle(this.worldsLayer.Location.X, this.worldsLayer.Location.Y + (int) this.worldsLayer.Size.Y + 15, (int) this.worldsLayer.Size.X, 190), "Selected world:");
      this.buttonPlay = new ButtonLayer(new Rectangle(this.selectedWorldLayer.Location.X + 20, this.selectedWorldLayer.Location.Y + (int) this.selectedWorldLayer.Size.Y - 40, (int) this.selectedWorldLayer.Size.X - 40, 20), "Play!");
      this.buttonRemove = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 40, 280, 20), "Remove selected");
      this.buttonBack = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X - 185, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 40, 165, 20), "Back");
      this.buttonCreate = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + 20, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 75, 280, 20), "Create new");
      this.PopulateWorldsTextbox(this.savedWorlds);
      this.scrollbarLayer = (ScrollbarLayer) new VerticalScrollbarLayer(new Rectangle(this.worldsLayer.Location.X + (int) this.worldsLayer.Size.X - 25, this.worldsLayer.Location.Y, 25, (int) this.worldsLayer.Size.Y), this.worldsLayer.TextLines.Count * 30);
      this.worldsLayer.SetScrollbar(this.scrollbarLayer);
    }

    public override void Render()
    {
      this.backgroundLayer.Render();
      this.worldsLayer.Render();
      this.selectedWorldLayer.Render();
      this.buttonBack.Render();
      this.buttonRemove.Render();
      this.buttonCreate.Render();
      this.scrollbarLayer.Render();
      if (this.selectedWorld != null)
        this.buttonPlay.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Worlds:", this.backgroundLayer.Position + new Vector2(20f, 30f), Color.White);
      if (this.clickedTextboxIndex != -1 && this.blankTexture != null)
      {
        int num = this.clickedTextboxIndex - this.worldsLayer.ScrollPosition;
        if (num >= 0 && num < 10)
        {
          Rectangle destinationRectangle1 = new Rectangle(this.worldsLayer.Location.X, this.worldsLayer.Location.Y + 30 * num, 2, 30);
          Rectangle destinationRectangle2 = new Rectangle(this.worldsLayer.Location.X, this.worldsLayer.Location.Y + 30 * num, (int) this.worldsLayer.Size.X - 30, 2);
          Rectangle destinationRectangle3 = new Rectangle(this.worldsLayer.Location.X, this.worldsLayer.Location.Y + 30 * (num + 1), (int) this.worldsLayer.Size.X - 30, 2);
          Rectangle destinationRectangle4 = new Rectangle(this.worldsLayer.Location.X + (int) this.worldsLayer.Size.X - 32, this.worldsLayer.Location.Y + 30 * num, 2, 30);
          spriteBatch.Draw(this.blankTexture, destinationRectangle1, Color.White);
          spriteBatch.Draw(this.blankTexture, destinationRectangle2, Color.White);
          spriteBatch.Draw(this.blankTexture, destinationRectangle3, Color.White);
          spriteBatch.Draw(this.blankTexture, destinationRectangle4, Color.White);
        }
      }
      if (this.selectedWorld != null)
      {
        spriteBatch.DrawString(Engine.Font, this.selectedWorld.Name, this.selectedWorldLayer.Position + new Vector2(20f, 30f), Color.White);
        spriteBatch.DrawString(Engine.Font, "Itemset: " + this.selectedWorld.Itemset.ItemsetName, this.selectedWorldLayer.Position + new Vector2(20f, 60f), Color.White);
        spriteBatch.DrawString(Engine.Font, "Date created: " + this.selectedWorld.CreationTime.ToString(), this.selectedWorldLayer.Position + new Vector2(20f, 90f), Color.White);
      }
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonBack.UpdateInput(this.Mouse);
      this.buttonCreate.UpdateInput(this.Mouse);
      this.buttonPlay.UpdateInput(this.Mouse);
      this.buttonRemove.UpdateInput(this.Mouse);
      this.scrollbarLayer.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonBack.PositionAndSize.Contains(this.Mouse.Position))
        this.CloseScreen();
      else if (this.buttonCreate.PositionAndSize.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new CreateNewWorldScreen((Action<World>) (world =>
        {
          if (world == null)
            return;
          this.savedWorlds.Add(world);
          this.PopulateWorldsTextbox(this.savedWorlds);
        })));
      else if (this.buttonRemove.PositionAndSize.Contains(this.Mouse.Position))
      {
        if (this.selectedWorld != null)
          Engine.ConfirmBoxShow("Delete world " + this.selectedWorld.Name + "?", (Action<bool>) (result =>
          {
            if (!result)
              return;
            this.selectedWorld.Dispose();
            WorldIOManager.Instance.RequestDataStorageDeletion(this.selectedWorld);
            this.savedWorlds.Remove(this.selectedWorld);
            this.PopulateWorldsTextbox(this.savedWorlds);
            this.selectedWorld = (World) null;
            this.clickedTextboxIndex = -1;
          }));
      }
      else if (this.buttonPlay.PositionAndSize.Contains(this.Mouse.Position) && this.selectedWorld != null)
      {
        if (Engine.PlayWorld(this.selectedWorld))
        {
          if (this.selectedWorld.Load())
          {
            if (this.resultDelegate != null)
              this.resultDelegate(this.selectedWorld);
          }
          else
            Engine.LoadNewScreen((GameScreen) new MessageScreen("Failed to load the world:" + Environment.NewLine + "unable to recognize or open the binary file.", "Error", true));
        }
        else
          Engine.LoadNewScreen((GameScreen) new MessageScreen("Failed to load the world:" + Environment.NewLine + "failed to set textures to the device.", "Error", true));
      }
      if (this.scrollbarLayer.PositionAndSize.Contains(this.Mouse.Position) || this.worldsLayer.GetClickedLayer(this.Mouse.Position) != this.worldsLayer)
        return;
      int index = this.worldsLayer.ScrollPosition + (this.Mouse.Position.Y - this.worldsLayer.Location.Y) / 30;
      if (index >= 0 && index < this.worldsLayer.TextLines.Count)
      {
        this.clickedTextboxIndex = index;
        if (index < 0 || index >= this.savedWorlds.Count)
          return;
        this.selectedWorld = this.savedWorlds[index];
      }
      else
      {
        this.clickedTextboxIndex = -1;
        this.selectedWorld = (World) null;
      }
    }

    private List<World> CreateWorlds(WorldInfo[] infos)
    {
      if (infos == null)
        return (List<World>) null;
      List<World> worlds = new List<World>();
      foreach (WorldInfo info in infos)
      {
        World newLocalWorld = Engine.CreateNewLocalWorld(info);
        if (newLocalWorld != null)
          worlds.Add(newLocalWorld);
      }
      return worlds;
    }

    private void PopulateWorldsTextbox(List<World> worlds)
    {
      if (worlds == null || this.worldsLayer == null)
        return;
      List<string> stringList = new List<string>();
      foreach (World world in worlds)
        stringList.Add(world.Name);
      this.worldsLayer.TextLines = stringList;
    }
  }
}
