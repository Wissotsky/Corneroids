// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.AdminPanelScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace.Screen
{
  public class AdminPanelScreen : MenuScreen
  {
    private int chosenPlayerIndex;
    private List<Player> players;
    private CaptionWindowLayer actionLayer;
    private CaptionWindowLayer bgLayer;
    private FieldLayer bgFieldLayer;
    private FieldLayer playerBgLayer;
    private CaptionWindowLayer serverInfoLayer;
    private TextBoxLayer passwordTextbox;
    private ButtonLayer buttonKick;
    private ButtonLayer buttonReset;
    private ButtonLayer buttonOk;
    private ButtonLayer buttonCancel;
    private ChatWindowLayer chat;
    private NetworkServerManager server;

    public AdminPanelScreen(NetworkServerManager server, ChatWindowLayer chat)
      : base(GameScreen.Type.Popup, true, true, false)
    {
      if (server == null || chat == null)
        throw new ArgumentNullException();
      this.chosenPlayerIndex = -1;
      this.chat = chat;
      this.server = server;
      this.players = server.Players.Where<Player>((Func<Player, bool>) (p => !(p is LocalPlayer))).ToList<Player>();
    }

    public override void Load()
    {
      this.bgLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(700, 550), "Admin panel");
      this.bgFieldLayer = new FieldLayer(new Rectangle(this.bgLayer.Location.X + 15, this.bgLayer.Location.Y + 30, (int) this.bgLayer.Size.X - 30, (int) this.bgLayer.Size.Y - 90));
      this.serverInfoLayer = new CaptionWindowLayer(new Rectangle(this.bgLayer.Location.X + 20, this.bgLayer.Location.Y + 35, (int) this.bgLayer.Size.X - 40, 80), "Server info");
      this.passwordTextbox = new TextBoxLayer(new Rectangle(this.serverInfoLayer.Location.X + 140, this.serverInfoLayer.Location.Y + 30, 250, 25), 20U, 1);
      this.actionLayer = new CaptionWindowLayer(new Rectangle(this.bgFieldLayer.Location.X + (int) this.bgFieldLayer.Size.X - 205, this.serverInfoLayer.Location.Y + (int) this.serverInfoLayer.Size.Y + 10, 200, 360), "Actions");
      this.playerBgLayer = new FieldLayer(new Rectangle(this.bgLayer.Location.X + 20, this.serverInfoLayer.Location.Y + (int) this.serverInfoLayer.Size.Y + 40, (int) this.bgFieldLayer.Size.X - (int) this.actionLayer.Size.X - 20, (int) this.actionLayer.Size.Y - 30));
      this.buttonKick = new ButtonLayer(new Rectangle(this.actionLayer.Location.X + 20, this.actionLayer.Location.Y + 30, (int) this.actionLayer.Size.X - 40, 25), "Kick player");
      this.buttonReset = new ButtonLayer(new Rectangle(this.actionLayer.Location.X + 20, this.actionLayer.Location.Y + 65, (int) this.actionLayer.Size.X - 40, 25), "Reset player");
      this.buttonOk = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 - 150, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 45, 140, 25), "Ok");
      this.buttonCancel = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + (int) this.bgLayer.Size.X / 2 + 10, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 45, 140, 25), "Cancel");
      this.passwordTextbox.Text = this.server.Password;
    }

    public override void Render()
    {
      base.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      this.bgLayer.Render();
      this.bgFieldLayer.Render();
      this.playerBgLayer.Render();
      this.serverInfoLayer.Render();
      this.passwordTextbox.Render();
      this.actionLayer.Render();
      this.buttonKick.Render();
      this.buttonOk.Render();
      this.buttonCancel.Render();
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Password:", this.serverInfoLayer.Position + new Vector2(20f, 30f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Players:", this.serverInfoLayer.Position + Vector2.UnitX * 5f + Vector2.UnitY * (this.serverInfoLayer.Size.Y + 10f), Color.White);
      this.RenderPlayers(spriteBatch);
      spriteBatch.End();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonCancel.UpdateInput(this.Mouse);
      this.buttonOk.UpdateInput(this.Mouse);
      this.buttonKick.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonCancel.Contains(this.Mouse.Position))
        this.CloseScreen();
      else if (this.buttonOk.Contains(this.Mouse.Position))
      {
        this.SaveSettings();
        this.CloseScreen();
      }
      else if (this.buttonKick.Contains(this.Mouse.Position) && this.chosenPlayerIndex >= 0 && this.chosenPlayerIndex < this.players.Count)
      {
        ServerToClientConnection connectionOfPlayer = this.server.GetConnectionOfPlayer(this.players[this.chosenPlayerIndex]);
        if (connectionOfPlayer != null)
          this.server.DisconnectUser(connectionOfPlayer.Connection, "Kicked by admin");
      }
      if (this.playerBgLayer.Contains(this.Mouse.Position))
      {
        this.chosenPlayerIndex = (this.Mouse.Position.Y - this.playerBgLayer.Location.Y) / 20;
        if (this.chosenPlayerIndex >= this.server.Players.ToList<Player>().Count)
          this.chosenPlayerIndex = -1;
      }
      if (!this.passwordTextbox.Contains(this.Mouse.Position))
        return;
      Engine.LoadNewScreen((GameScreen) new ReadInputScreen(this.passwordTextbox, (ReadInputScreen.ReadingFinished) null));
    }

    private void RenderPlayers(SpriteBatch sb)
    {
      Vector2 position = this.playerBgLayer.Position + Vector2.One * 10f;
      int num = 0;
      foreach (Player player in this.players)
      {
        ServerToClientConnection connectionOfPlayer = this.server.GetConnectionOfPlayer(player);
        if (connectionOfPlayer != null)
        {
          Color color = num == this.chosenPlayerIndex ? Color.Yellow : Color.White;
          sb.DrawString(Engine.Font, connectionOfPlayer.ClientName, position, color);
          sb.DrawString(Engine.Font, connectionOfPlayer.Connection.RemoteEndPoint.ToString(), position + Vector2.UnitX * 200f, color);
          position += Vector2.UnitY * 20f;
          ++num;
        }
      }
    }

    private void SaveSettings()
    {
      string text = this.passwordTextbox.Text;
      string message = (string) null;
      if (!string.IsNullOrEmpty(text))
      {
        if (string.IsNullOrEmpty(this.server.Password))
          message = "Password set";
        else if (this.server.Password != text)
          message = "Password changed";
      }
      else if (!string.IsNullOrEmpty(this.server.Password))
        message = "Password removed";
      this.server.Chat(message);
      this.chat.AddChatMessage("SERVER", message, Color.Yellow);
      this.server.Password = text;
    }
  }
}
