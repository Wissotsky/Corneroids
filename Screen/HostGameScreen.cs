// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.HostGameScreen
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
  public class HostGameScreen : MenuScreen
  {
    private const string port = "Port:";
    private const string password = "Password:";
    private const string serverName = "Server name:";
    private const string maxPlayers = "Max players:";
    private const string world = "World:";
    private const int maxPlayerCount = 8;
    private CaptionWindowLayer bgLayer;
    private ButtonLayer buttonCreate;
    private ButtonLayer buttonBack;
    private TextBoxLayer textboxPassword;
    private TextBoxLayer textboxPort;
    private TextBoxLayer textboxServerName;
    private TextBoxLayer textboxMaxPlayers;
    private TextBoxLayer textboxWorld;
    private int playerCount;
    private ushort portNumber;
    private World worldToPlay;

    public HostGameScreen()
      : base(GameScreen.Type.Popup, true, true, false)
    {
      this.playerCount = 4;
      this.portNumber = (ushort) 17100;
    }

    public override void Load()
    {
      this.bgLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(550, 320), "Host game");
      this.textboxServerName = new TextBoxLayer(new Rectangle(this.bgLayer.Location.X + 170, this.bgLayer.Location.Y + 30, 350, 30), 30U, 1);
      this.textboxPassword = new TextBoxLayer(new Rectangle(this.bgLayer.Location.X + 170, this.bgLayer.Location.Y + 80, 350, 30), 20U, 1);
      this.textboxMaxPlayers = new TextBoxLayer(new Rectangle(this.bgLayer.Location.X + 170, this.bgLayer.Location.Y + 130, 100, 30), 2U, 1)
      {
        Text = this.playerCount.ToString()
      };
      this.textboxPort = new TextBoxLayer(new Rectangle(this.textboxMaxPlayers.Location.X + (int) this.textboxMaxPlayers.Size.X + 130, this.textboxMaxPlayers.Location.Y, 120, 30), 6U, 1)
      {
        Text = this.portNumber.ToString()
      };
      this.textboxWorld = new TextBoxLayer(new Rectangle(this.bgLayer.Location.X + 170, this.bgLayer.Location.Y + 200, 350, 30), 20U, 1);
      this.buttonCreate = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 - 110, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 50, 100, 25), "Play!");
      this.buttonBack = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 + 10, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 50, 100, 25), "Back");
      this.textboxServerName.Text = Engine.StoredValues.RetrieveValue<string>("hostServerName");
    }

    public override void Render()
    {
      this.bgLayer.Render();
      this.textboxServerName.Render();
      this.textboxPassword.Render();
      this.textboxMaxPlayers.Render();
      this.textboxWorld.Render();
      this.textboxPort.Render();
      this.buttonCreate.Render();
      this.buttonBack.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Server name:", this.bgLayer.Position + new Vector2(20f, 30f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Password:", this.bgLayer.Position + new Vector2(52f, 80f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Max players:", this.bgLayer.Position + new Vector2(26f, 130f), Color.White);
      spriteBatch.DrawString(Engine.Font, "World:", this.bgLayer.Position + new Vector2(92f, 200f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Port:", this.textboxPort.Position - new Vector2(65f, 0.0f), Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (this.InputFrame.LeftClick)
      {
        if (this.buttonBack.Contains(this.Mouse.Position))
          this.CloseScreen();
        else if (this.buttonCreate.Contains(this.Mouse.Position))
        {
          if (string.IsNullOrEmpty(this.textboxServerName.Text))
            Engine.MessageBoxShow("Invalid server name");
          else if (this.worldToPlay == null)
          {
            Engine.MessageBoxShow("Please choose a world!");
          }
          else
          {
            Engine.StoredValues.StoreValue<string>("hostServerName", this.textboxServerName.Text);
            Engine.LoadNewScreen((GameScreen) new AdventureScreenServer(this.playerCount, 17100, this.textboxServerName.Text, this.textboxPassword.Text));
          }
        }
        else if (this.textboxServerName.Contains(this.Mouse.Position))
          Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textboxServerName, (ReadInputScreen.ReadingFinished) null));
        else if (this.textboxPassword.Contains(this.Mouse.Position))
          Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textboxPassword, (ReadInputScreen.ReadingFinished) null));
        else if (this.textboxPort.Contains(this.Mouse.Position))
          Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textboxPort, (ReadInputScreen.ReadingFinished) (() =>
          {
            ushort result;
            if (ushort.TryParse(this.textboxPort.Text, out result))
              this.portNumber = result;
            else
              this.textboxPort.Text = this.portNumber.ToString();
          })));
        else if (this.textboxMaxPlayers.Contains(this.Mouse.Position))
          Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.textboxMaxPlayers, (ReadInputScreen.ReadingFinished) (() =>
          {
            int playerCount = this.playerCount;
            if (int.TryParse(this.textboxMaxPlayers.Text, out this.playerCount))
            {
              this.playerCount = Math.Min(Math.Max(1, this.playerCount), 8);
              this.textboxMaxPlayers.Text = this.playerCount.ToString();
            }
            else
            {
              this.playerCount = playerCount;
              this.textboxMaxPlayers.Text = this.playerCount.ToString();
            }
          })));
        else if (this.textboxWorld.Contains(this.Mouse.Position))
        {
          GameScreen chooseWorldScreen = (GameScreen) null;
          chooseWorldScreen = (GameScreen) new ChooseWorldScreen((Action<World>) (chosenWorld =>
          {
            this.worldToPlay = chosenWorld;
            if (chosenWorld != null)
              this.textboxWorld.Text = this.worldToPlay.Name;
            chooseWorldScreen.CloseScreen();
          }));
          Engine.LoadNewScreen(chooseWorldScreen);
        }
      }
      this.buttonCreate.UpdateInput(this.Mouse);
      this.buttonBack.UpdateInput(this.Mouse);
    }
  }
}
