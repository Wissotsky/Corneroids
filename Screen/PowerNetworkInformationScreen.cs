// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.PowerNetworkInformationScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;

#nullable disable
namespace CornerSpace.Screen
{
  public class PowerNetworkInformationScreen : MenuScreen
  {
    private PowerBlock powerblock;
    private PowerNetworkInfoLayer layer;

    public PowerNetworkInformationScreen(PowerBlock powerblock)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.powerblock = powerblock;
    }

    public override void Load()
    {
      this.layer = new PowerNetworkInfoLayer(this.powerblock.Network.PowerBlocks.Count, this.powerblock.Network.PowerLevel);
    }

    public override void Render() => this.layer.Render();

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (!this.Mouse.RightClick())
        return;
      this.CloseScreen();
    }
  }
}
