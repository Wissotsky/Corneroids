// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ButtonLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class ButtonLayer : WindowLayer
  {
    private const float characterWidth = 9f;
    private bool clicked;
    private string buttonText;
    private bool forceChecked;
    private string parsedButtonText;
    private FieldLayer pressedLayer;

    public ButtonLayer(Rectangle position, string buttonText)
      : base(position)
    {
      this.clicked = false;
      this.buttonText = buttonText;
      this.parsedButtonText = buttonText;
      this.Name = buttonText;
      this.pressedLayer = new FieldLayer(position);
      this.layers.Add((Layer) this.pressedLayer);
      this.UpdateButtonText();
    }

    public override void Render()
    {
      if (!this.clicked && !this.forceChecked)
        base.Render();
      else
        this.pressedLayer.Render();
      if (string.IsNullOrEmpty(this.buttonText))
        return;
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      Vector2 vector2 = Engine.Font.MeasureString(this.parsedButtonText);
      Rectangle positionAndSize = base.PositionAndSize;
      Vector2 position = new Vector2((float) ((double) positionAndSize.X + (double) positionAndSize.Width / 2.0 - (double) vector2.X / 2.0), (float) (positionAndSize.Y + positionAndSize.Height / 2) - vector2.Y / 2f);
      spriteBatch.DrawString(Engine.Font, this.parsedButtonText, position, Color.White);
      spriteBatch.End();
    }

    public void UpdateInput(MouseDevice mouse)
    {
      this.clicked = false;
      if (!mouse.LeftDown() || !this.Contains(mouse.Position))
        return;
      this.clicked = true;
    }

    public string ButtonText
    {
      get => this.buttonText;
      set
      {
        this.buttonText = value;
        this.UpdateButtonText();
      }
    }

    public bool ForceChecked
    {
      set => this.forceChecked = value;
    }

    public override Rectangle PositionAndSize
    {
      get => base.PositionAndSize;
      set
      {
        base.PositionAndSize = value;
        this.UpdateButtonText();
        this.pressedLayer.PositionAndSize = value;
      }
    }

    public override Vector2 Size
    {
      get => base.Size;
      set
      {
        base.Size = value;
        this.UpdateButtonText();
      }
    }

    private void UpdateButtonText()
    {
      if (string.IsNullOrEmpty(this.buttonText))
        return;
      this.buttonText = this.buttonText.Trim();
      this.parsedButtonText = this.buttonText;
      Vector2 size = this.Size;
      for (Vector2 vector2 = Engine.Font.MeasureString(this.parsedButtonText); (double) vector2.X >= (double) size.X && (double) size.X >= 10.0 && this.parsedButtonText.Length > 0; vector2 = Engine.Font.MeasureString(this.parsedButtonText))
        this.parsedButtonText = this.parsedButtonText.Substring(0, this.parsedButtonText.Length - 1);
    }
  }
}
