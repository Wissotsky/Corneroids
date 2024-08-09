// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.MultiplayerScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace.Screen
{
  public class MultiplayerScreen : MenuScreen
  {
    private CaptionWindowLayer bgLayer;
    private ButtonLayer buttonHost;
    private ButtonLayer buttonJoin;
    private ButtonLayer buttonBack;
    private ButtonLayer buttonPlayerSetup;

    public MultiplayerScreen()
      : base(GameScreen.Type.Popup, true, true, false)
    {
    }

    public override void Load()
    {
      this.bgLayer = new CaptionWindowLayer(Layer.EvaluateMiddlePosition(300, 225), "Multiplayer");
      this.buttonJoin = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + 20, this.bgLayer.Location.Y + 30, (int) this.bgLayer.Size.X - 40, 25), "Join game");
      this.buttonHost = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + 20, this.bgLayer.Location.Y + 70, (int) this.bgLayer.Size.X - 40, 25), "Host game");
      this.buttonPlayerSetup = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + 20, this.bgLayer.Location.Y + 110, (int) this.bgLayer.Size.X - 40, 25), "Player setup");
      this.buttonBack = new ButtonLayer(new Rectangle(this.bgLayer.Location.X + 20, this.bgLayer.Location.Y + (int) this.bgLayer.Size.Y - 45, (int) this.bgLayer.Size.X - 40, 25), "Back");
    }

    public override void Render()
    {
      this.bgLayer.Render();
      this.buttonJoin.Render();
      this.buttonHost.Render();
      this.buttonPlayerSetup.Render();
      this.buttonBack.Render();
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonBack.UpdateInput(this.Mouse);
      this.buttonHost.UpdateInput(this.Mouse);
      this.buttonPlayerSetup.UpdateInput(this.Mouse);
      this.buttonJoin.UpdateInput(this.Mouse);
      if (!this.InputFrame.LeftClick)
        return;
      if (this.buttonBack.Contains(this.Mouse.Position))
        this.CloseScreen();
      else if (this.buttonHost.Contains(this.Mouse.Position))
        Engine.LoadNewScreen((GameScreen) new HostGameScreen());
      else if (this.buttonJoin.Contains(this.Mouse.Position))
      {
        Engine.LoadNewScreen((GameScreen) new DirectConnectScreen());
      }
      else
      {
        if (!this.buttonPlayerSetup.Contains(this.Mouse.Position))
          return;
        Engine.LoadNewScreen((GameScreen) new PlayerSetupScreen());
      }
    }
  }
}
