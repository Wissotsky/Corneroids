// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.EditLightBlockLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class EditLightBlockLayer : CaptionWindowLayer
  {
    private const string cPulseSequence = "Light pulse sequence:";
    private const string csSample = "Sample:";
    private const int windowWidth = 300;
    private const int windowHeight = 300;
    private new Texture2D blankTexture;
    private Color lightColor;
    private byte pulseSequence;
    private FieldLayer sampleLayer;
    private int sampleTime;
    private FieldLayer sequenceBgLayer;

    public EditLightBlockLayer()
      : base(Layer.EvaluateMiddlePosition(300, 300), "Edit light block")
    {
      FieldLayer fieldLayer = new FieldLayer(new Rectangle(this.Location.X + 30, this.Location.Y + 60, 240, 80));
      fieldLayer.Name = "sequenceLayer";
      this.sequenceBgLayer = fieldLayer;
      this.sampleLayer = new FieldLayer(new Rectangle(this.Location.X + 300 - 80, this.Location.Y + 60 + 100, 50, 50));
      this.layers.Add((Layer) this.sequenceBgLayer);
      this.layers.Add((Layer) this.sampleLayer);
      this.blankTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
      this.lightColor = Color.White;
    }

    public override void Render()
    {
      base.Render();
      foreach (Layer layer in this.layers)
        layer.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      SpriteFont font = Engine.Font;
      spriteBatch.Begin();
      spriteBatch.DrawString(font, "Light pulse sequence:", this.Position + Vector2.One * 30f, Color.White);
      spriteBatch.DrawString(font, "Sample:", this.Position + new Vector2(120f, 170f), Color.White);
      byte num1 = 128;
      for (int index = 0; index < 8; ++index)
      {
        Vector2 vector2 = this.sequenceBgLayer.Position + new Vector2((float) (index * 30), this.sequenceBgLayer.Size.Y - 30f);
        if (((int) num1 >> index & (int) this.pulseSequence) != 0)
          vector2 -= Vector2.UnitY * 40f;
        spriteBatch.Draw(this.blankTexture, new Rectangle((int) vector2.X, (int) vector2.Y, 20, 20), this.lightColor);
      }
      float num2 = this.sequenceBgLayer.Size.X / 2000f * (float) this.sampleTime;
      spriteBatch.Draw(this.blankTexture, new Rectangle(this.sequenceBgLayer.Location.X + (int) num2, this.sequenceBgLayer.Location.Y, 5, (int) this.sequenceBgLayer.Size.Y), Color.White);
      if (((int) this.pulseSequence & 128 >> (int) (byte) ((double) this.sampleTime / 2000.0 * 8.0)) != 0)
        spriteBatch.Draw(this.blankTexture, this.sampleLayer.PositionAndSize, this.lightColor);
      spriteBatch.End();
    }

    public Color LightColor
    {
      set => this.lightColor = value;
    }

    public byte PulseSequence
    {
      get => this.pulseSequence;
      set => this.pulseSequence = value;
    }

    public int SampleTime
    {
      set => this.sampleTime = value % 2000;
    }
  }
}
