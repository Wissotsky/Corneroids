// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.CheckboxLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class CheckboxLayer : FieldLayer
  {
    private const string sChecked = "X";
    private bool boxChecked;

    public CheckboxLayer(Rectangle position)
      : base(position)
    {
    }

    public override void Render()
    {
      base.Render();
      if (!this.boxChecked)
        return;
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      Vector2 position = this.Position + this.Size * 0.5f - Engine.Font.MeasureString("X") * 0.5f;
      spriteBatch.DrawString(Engine.Font, "X", position, Color.White);
      spriteBatch.End();
    }

    public bool Checked
    {
      get => this.boxChecked;
      set => this.boxChecked = value;
    }
  }
}
