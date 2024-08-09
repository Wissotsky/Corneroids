// Decompiled with JetBrains decompiler
// Type: CornerSpace.MouseDevice
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace CornerSpace
{
  public class MouseDevice : InputDevice
  {
    private MouseDevice.Behavior storedBehavior;
    private MouseDevice.Behavior behavior;
    private MouseState mouseState;
    private MouseState previousMouseState;
    private int middleX = 512;
    private int middleY = 384;

    public MouseDevice()
    {
      Mouse.SetPosition(this.middleX, this.middleY);
      this.storedBehavior = MouseDevice.Behavior.Free;
      this.behavior = MouseDevice.Behavior.Free;
    }

    public override void UpdateDevice()
    {
      this.previousMouseState = this.mouseState;
      this.mouseState = Mouse.GetState();
      if (this.behavior != MouseDevice.Behavior.Wrapped)
        return;
      Mouse.SetPosition(this.middleX, this.middleY);
    }

    public bool LeftClick()
    {
      return this.mouseState.LeftButton == ButtonState.Released && this.previousMouseState.LeftButton == ButtonState.Pressed;
    }

    public bool LeftDown() => this.mouseState.LeftButton == ButtonState.Pressed;

    public bool RightClick()
    {
      return this.mouseState.RightButton == ButtonState.Released && this.previousMouseState.RightButton == ButtonState.Pressed;
    }

    public bool RightDown() => this.mouseState.RightButton == ButtonState.Pressed;

    public bool MiddleClick()
    {
      return this.mouseState.MiddleButton == ButtonState.Released && this.previousMouseState.MiddleButton == ButtonState.Pressed;
    }

    public bool MiddleDown() => this.mouseState.MiddleButton == ButtonState.Pressed;

    public void RestoreState() => this.behavior = this.storedBehavior;

    public int Scroll()
    {
      return this.mouseState.ScrollWheelValue - this.previousMouseState.ScrollWheelValue;
    }

    public void StoreState() => this.storedBehavior = this.behavior;

    public int GetTranslationX()
    {
      return this.behavior == MouseDevice.Behavior.Wrapped ? this.mouseState.X - this.middleX : this.mouseState.X - this.previousMouseState.X;
    }

    public int GetTranslationY()
    {
      return this.behavior == MouseDevice.Behavior.Wrapped ? this.mouseState.Y - this.middleY : this.mouseState.Y - this.previousMouseState.Y;
    }

    public MouseDevice.Behavior CursorBehavior
    {
      get => this.behavior;
      set
      {
        this.behavior = value;
        if (value != MouseDevice.Behavior.Wrapped)
          return;
        this.UpdateDevice();
      }
    }

    public Point Position => new Point(this.mouseState.X, this.mouseState.Y);

    public enum Behavior
    {
      Wrapped,
      Free,
    }
  }
}
