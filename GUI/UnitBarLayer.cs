// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.UnitBarLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.GUI
{
  public class UnitBarLayer : Layer
  {
    private Texture2D blankTexture;
    private Color blinkColor;
    private float? blinkLimitPercentage;
    private int blinkTimerMS;
    private int blinkTimerMax;
    private byte? borderSize;
    private string captionString;
    private Color color;
    private int maxValue;
    private int realValue;
    private bool valueAsPercentage;
    private string valueString;

    public UnitBarLayer(Rectangle positionAndSize, Color barColor, uint maxValue, string caption)
      : base(positionAndSize)
    {
      this.blinkColor = Color.Black;
      this.blinkTimerMS = 0;
      this.blinkTimerMax = 200;
      this.borderSize = new byte?((byte) 2);
      this.captionString = caption;
      this.color = barColor;
      this.maxValue = this.realValue = (int) maxValue;
      this.UpdateValueText();
      this.blankTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
    }

    public override void Render()
    {
      if ((double) this.Size.X <= 0.0 || (double) this.Size.Y <= 0.0)
        return;
      Color color = this.color;
      if (this.blinkLimitPercentage.HasValue && (double) this.realValue / (double) this.maxValue <= (double) this.blinkLimitPercentage.Value)
      {
        color = this.blinkTimerMS > this.blinkTimerMax / 2 ? this.color : this.blinkColor;
        this.blinkTimerMS = (this.blinkTimerMS + (int) Engine.FrameCounter.DeltaTimeMS) % this.blinkTimerMax;
      }
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      byte num = (byte) (this.borderSize ?? 0);
      if (num > 0)
      {
        Rectangle destinationRectangle = new Rectangle(this.Location.X, this.Location.Y, (int) this.Size.X, (int) num);
        spriteBatch.Draw(this.blankTexture, destinationRectangle, color);
        destinationRectangle = new Rectangle(this.Location.X, this.Location.Y, (int) num, (int) this.Size.Y);
        spriteBatch.Draw(this.blankTexture, destinationRectangle, color);
        destinationRectangle = new Rectangle(this.Location.X, this.Location.Y + (int) this.Size.Y - (int) num, (int) this.Size.X, (int) num);
        spriteBatch.Draw(this.blankTexture, destinationRectangle, color);
        destinationRectangle = new Rectangle(this.Location.X + (int) this.Size.X - (int) num, this.Location.Y, (int) num, (int) this.Size.Y);
        spriteBatch.Draw(this.blankTexture, destinationRectangle, color);
      }
      Rectangle destinationRectangle1 = new Rectangle(this.Location.X, this.Location.Y, (int) ((double) this.realValue / (double) this.maxValue * (double) this.Size.X), (int) this.Size.Y);
      if (destinationRectangle1.Width > 0)
        spriteBatch.Draw(this.blankTexture, destinationRectangle1, color);
      if (!string.IsNullOrEmpty(this.valueString))
        spriteBatch.DrawString(Engine.Font, this.valueString, this.Position, Color.White);
      if (!string.IsNullOrEmpty(this.captionString))
        spriteBatch.DrawString(Engine.Font, this.captionString, this.Position - Vector2.UnitY * 30f, Color.White);
      spriteBatch.End();
    }

    public Color BlinkColor
    {
      get => this.blinkColor;
      set => this.blinkColor = value;
    }

    public float? BlinkPercentages
    {
      get => this.blinkLimitPercentage;
      set => this.blinkLimitPercentage = value;
    }

    public int BlinkTimer
    {
      get => this.blinkTimerMax;
      set => this.blinkTimerMax = value;
    }

    public byte? BorderSize
    {
      get => this.borderSize;
      set => this.borderSize = value;
    }

    public Color BarColor
    {
      get => this.color;
      set => this.color = value;
    }

    public uint MaxValue
    {
      get => (uint) this.maxValue;
      set
      {
        if (this.maxValue == (int) value)
          return;
        this.maxValue = (int) value;
        this.realValue = Math.Min(this.realValue, this.maxValue);
        this.UpdateValueText();
      }
    }

    public uint RealValue
    {
      get => (uint) this.realValue;
      set
      {
        if (this.realValue == (int) value)
          return;
        this.realValue = (int) Math.Min((long) value, (long) this.maxValue);
        this.UpdateValueText();
      }
    }

    public bool ValueAsPercentage
    {
      get => this.valueAsPercentage;
      set
      {
        this.valueAsPercentage = value;
        this.UpdateValueText();
      }
    }

    private void UpdateValueText()
    {
      if (this.valueAsPercentage)
        this.valueString = ((int) (100.0 * (double) ((float) this.realValue / (float) this.maxValue))).ToString() + "% / 100%";
      else
        this.valueString = this.realValue.ToString() + " / " + this.maxValue.ToString();
    }
  }
}
