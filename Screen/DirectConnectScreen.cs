// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.DirectConnectScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Net;

#nullable disable
namespace CornerSpace.Screen
{
  public class DirectConnectScreen : MenuScreen
  {
    private const string ip = "ip:";
    private const string port = "port:";
    private const int windowWidth = 520;
    private const int windowHeight = 130;
    private const int defaultPort = 17100;
    private string playerName;
    private CaptionWindowLayer bglayer;
    private TextBoxLayer ipTextBox;
    private TextBoxLayer portTextBox;
    private ButtonLayer connectButton;
    private ButtonLayer backButton;

    public DirectConnectScreen()
      : base(GameScreen.Type.Popup, true, true, false)
    {
    }

    public override void Load()
    {
      this.bglayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(520, 130), "Direct connect");
      this.ipTextBox = new TextBoxLayer(new Rectangle(this.bglayer.Location.X + 60, this.bglayer.Location.Y + 30, 260, 30), 15U, 1);
      this.portTextBox = new TextBoxLayer(new Rectangle(this.ipTextBox.Location.X + (int) this.ipTextBox.Size.X + 80, this.ipTextBox.Location.Y, 100, 30), 6U, 1);
      this.connectButton = new ButtonLayer(new Rectangle(this.bglayer.Location.X + (int) this.bglayer.Size.X / 2 - 110, this.bglayer.Location.Y + (int) this.bglayer.Size.Y - 50, 100, 30), "Connect");
      this.backButton = new ButtonLayer(new Rectangle(this.bglayer.Location.X + (int) this.bglayer.Size.X / 2 + 10, this.bglayer.Location.Y + (int) this.bglayer.Size.Y - 50, 100, 30), "Back");
      this.portTextBox.Text = 17100.ToString();
      this.ipTextBox.Text = Engine.StoredValues.RetrieveValue<string>("lastJoinedIp");
      this.portTextBox.Text = Engine.StoredValues.RetrieveValue<string>("lastJoinedPort") ?? 17100.ToString();
    }

    public override void Render()
    {
      base.Render();
      this.bglayer.Render();
      this.ipTextBox.Render();
      this.portTextBox.Render();
      this.connectButton.Render();
      this.backButton.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "ip:", this.bglayer.Position + new Vector2(20f, 30f), Color.White);
      spriteBatch.DrawString(Engine.Font, "port:", this.ipTextBox.Position + Vector2.UnitX * (this.ipTextBox.Size.X + 20f), Color.White);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.connectButton.UpdateInput(this.Mouse);
      this.backButton.UpdateInput(this.Mouse);
      if (!this.InputFrame.LeftClick)
        return;
      if (this.ipTextBox.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.ipTextBox, (ReadInputScreen.ReadingFinished) null));
      else if (this.portTextBox.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.portTextBox, (ReadInputScreen.ReadingFinished) null));
      else if (this.backButton.Contains(this.Mouse.Position))
      {
        this.CloseScreen();
      }
      else
      {
        if (!this.connectButton.Contains(this.Mouse.Position))
          return;
        if (!IPAddress.TryParse(this.ipTextBox.Text, out IPAddress _))
          Engine.MessageBoxShow("Invalid ip address");
        else if (string.IsNullOrEmpty(this.portTextBox.Text))
        {
          Engine.MessageBoxShow("Invalid port");
        }
        else
        {
          Engine.StoredValues.StoreValue<string>("lastJoinedIp", this.ipTextBox.Text);
          Engine.StoredValues.StoreValue<string>("lastJoinedPort", this.portTextBox.Text);
          string clientName = Engine.StoredValues.RetrieveValue<string>("playerName");
          if (!string.IsNullOrEmpty(clientName))
            Engine.LoadNewScreen((GameScreen) new ConnectingScreen(this.ipTextBox.Text, Convert.ToInt32(this.portTextBox.Text), clientName));
          else
            Engine.MessageBoxShow("Please set a player name first!");
        }
      }
    }
  }
}
