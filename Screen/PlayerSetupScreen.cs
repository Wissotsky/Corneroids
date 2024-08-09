// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.PlayerSetupScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.Screen
{
  public class PlayerSetupScreen : MenuScreen
  {
    private const string nicknameString = "Player name:";
    private CaptionWindowLayer bgLayer;
    private TextBoxLayer texboxNickname;
    private ButtonLayer buttonOk;
    private ButtonLayer buttonBack;

    public PlayerSetupScreen()
      : base(GameScreen.Type.Popup, true, true, false)
    {
    }

    public override void Load()
    {
      this.bgLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(430, 130), "Player setup");
      this.texboxNickname = new TextBoxLayer(new Rectangle(this.bgLayer.Location.X + 170, this.bgLayer.Location.Y + 30, 240, 30), 20U, 1);
      this.buttonOk = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 - 110, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 45, 100, 25), "Ok");
      this.buttonBack = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 + 11, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 45, 100, 25), "Cancel");
      this.texboxNickname.Text = Engine.StoredValues.RetrieveValue<string>("playerName");
      if (!string.IsNullOrEmpty(this.texboxNickname.Text))
        return;
      this.texboxNickname.Text = "Player";
    }

    public override void Render()
    {
      this.bgLayer.Render();
      this.texboxNickname.Render();
      this.buttonOk.Render();
      this.buttonBack.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Player name:", this.bgLayer.Position + new Vector2(20f, 30f), Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (!this.InputFrame.LeftClick)
        return;
      if (this.texboxNickname.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.texboxNickname, (ReadInputScreen.ReadingFinished) null));
      else if (this.buttonBack.Contains(this.Mouse.Position))
      {
        this.CloseScreen();
      }
      else
      {
        if (!this.buttonOk.Contains(this.Mouse.Position))
          return;
        Engine.StoredValues.StoreValue<string>("playerName", this.texboxNickname.Text);
        this.CloseScreen();
      }
    }
  }
}
