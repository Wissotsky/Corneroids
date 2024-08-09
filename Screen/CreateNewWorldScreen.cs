// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.CreateNewWorldScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class CreateNewWorldScreen : MenuScreen
  {
    private const string sAuthor = "Author:";
    private const string sItemset = "Itemset:";
    private const string sName = "Name:";
    private Action<World> createdWorldDelegate;
    private CaptionWindowLayer backgroundLayer;
    private ButtonLayer buttonOk;
    private ButtonLayer buttonCancel;
    private ComboboxLayer<Itemset> comboboxItemset;
    private TextBoxLayer textboxAuthor;
    private TextBoxLayer textboxName;

    public CreateNewWorldScreen(Action<World> resultFunction)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.createdWorldDelegate = resultFunction != null ? resultFunction : throw new ArgumentNullException();
    }

    public override void Load()
    {
      this.backgroundLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(400, 350), "Create new world");
      this.buttonOk = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 160, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 40, 150, 20), "Ok");
      this.buttonCancel = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 + 10, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 40, 150, 20), "Cancel");
      this.textboxName = new TextBoxLayer(new Rectangle(this.backgroundLayer.Location.X + 30, this.backgroundLayer.Location.Y + 60, 340, 30), 30U, 1);
      this.textboxAuthor = new TextBoxLayer(new Rectangle(this.backgroundLayer.Location.X + 30, this.backgroundLayer.Location.Y + 140, 340, 30), 30U, 1);
      this.comboboxItemset = new ComboboxLayer<Itemset>(new Rectangle(this.backgroundLayer.Location.X + 30, this.backgroundLayer.Location.Y + 220, 340, 30), 30U);
      if (Engine.LoadedItemsets == null)
        return;
      this.comboboxItemset.ItemsToChoose = Engine.LoadedItemsets;
    }

    public override void Render()
    {
      base.Render();
      this.backgroundLayer.Render();
      this.buttonOk.Render();
      this.buttonCancel.Render();
      this.comboboxItemset.Render();
      this.textboxName.Render();
      this.textboxAuthor.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Name:", this.backgroundLayer.Position + new Vector2(20f, 30f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Author:", this.textboxName.Position + new Vector2(0.0f, 50f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Itemset:", this.textboxAuthor.Position + new Vector2(0.0f, 50f), Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonCancel.PositionAndSize.Contains(this.Mouse.Position))
        this.CloseScreen();
      else if (this.textboxName.PositionAndSize.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textboxName, (ReadInputScreen.ReadingFinished) null));
      else if (this.textboxAuthor.PositionAndSize.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textboxAuthor, (ReadInputScreen.ReadingFinished) null));
      else if (this.comboboxItemset.PositionAndSize.Contains(this.Mouse.Position))
      {
        this.comboboxItemset.Expand();
      }
      else
      {
        if (!this.buttonOk.PositionAndSize.Contains(this.Mouse.Position))
          return;
        World world = (World) null;
        try
        {
          Random random = new Random();
          string text1 = this.textboxName.Text;
          string text2 = this.textboxAuthor.Text;
          Itemset selectedItem = this.comboboxItemset.SelectedItem;
          int seed = random.Next() + 171087;
          if (string.IsNullOrEmpty(text1))
            throw new Exception("Name must be set");
          if (string.IsNullOrEmpty(text2))
            throw new Exception("Author must be set");
          if (selectedItem == null)
            throw new Exception("Itemset must be chosen");
          world = Engine.CreateNewLocalWorld(text1, seed, selectedItem.Key, WorldIOManager.Instance.GetDatabasePathForWorld(text1), DateTime.Now);
          if (world == null)
            throw new Exception("Failed to create a world.");
          world.CreateStorage();
          this.createdWorldDelegate(world);
          this.CloseScreen();
        }
        catch (Exception ex)
        {
          Engine.MessageBoxShow(ex.Message);
          world?.Dispose();
        }
      }
    }
  }
}
