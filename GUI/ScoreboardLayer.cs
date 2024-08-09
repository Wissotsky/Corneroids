// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ScoreboardLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class ScoreboardLayer : CaptionWindowLayer
  {
    private const int windowWidth = 400;
    private const int windowHeight = 300;
    private FieldLayer bgLayer;
    private IPlayerList players;

    public ScoreboardLayer(IPlayerList players)
      : base(Layer.EvaluateMiddlePosition(400, 300), "Players")
    {
      this.players = players;
      this.bgLayer = new FieldLayer(new Rectangle(this.Location.X + 20, this.Location.Y + 30, (int) this.Size.X - 40, (int) this.Size.Y - 50));
    }

    public override void Render()
    {
      base.Render();
      this.bgLayer.Render();
      Vector2 position = this.bgLayer.Position + Vector2.One * 5f;
      Engine.SpriteBatch.Begin();
      foreach (Player player in this.players.Players)
      {
        Engine.SpriteBatch.DrawString(Engine.Font, player.Name, position, Color.White);
        position += Vector2.UnitY * 20f;
      }
      Engine.SpriteBatch.End();
    }
  }
}
