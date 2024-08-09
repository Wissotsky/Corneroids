// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.PowerNetworkInfoLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class PowerNetworkInfoLayer : CaptionWindowLayer
  {
    private const string csBlockCount = "Block count:  ";
    private const string csPowerBalance = "Power balance:  ";
    private const int windowWidth = 300;
    private const int windowHeight = 135;
    private int blockCount;
    private int balance;

    public PowerNetworkInfoLayer(int blocksInNetwork, int powerBalance)
      : base(Layer.EvaluateMiddlePosition(300, 135), "Power network")
    {
      this.blockCount = blocksInNetwork;
      this.balance = powerBalance;
    }

    public override void Render()
    {
      base.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      Rectangle positionAndSize = this.PositionAndSize;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Block count:  " + this.blockCount.ToString(), new Vector2((float) (positionAndSize.X + 20), (float) (positionAndSize.Y + 40)), Color.White);
      spriteBatch.DrawString(Engine.Font, "Power balance:  " + this.balance.ToString(), new Vector2((float) (positionAndSize.X + 20), (float) (positionAndSize.Y + 80)), Color.White);
      spriteBatch.End();
      foreach (Layer layer in this.layers)
        layer.Render();
    }
  }
}
