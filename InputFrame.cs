// Decompiled with JetBrains decompiler
// Type: CornerSpace.InputFrame
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace
{
  public class InputFrame
  {
    private static ControlsManager.SpecialKey[] buttons = new ControlsManager.SpecialKey[15]
    {
      ControlsManager.SpecialKey.Use,
      ControlsManager.SpecialKey.Edit,
      ControlsManager.SpecialKey.Forward,
      ControlsManager.SpecialKey.Backward,
      ControlsManager.SpecialKey.StrafeLeft,
      ControlsManager.SpecialKey.StrafeRight,
      ControlsManager.SpecialKey.Boost,
      ControlsManager.SpecialKey.Up,
      ControlsManager.SpecialKey.Down,
      ControlsManager.SpecialKey.OrientateBlock,
      ControlsManager.SpecialKey.Inventory,
      ControlsManager.SpecialKey.BeaconTool,
      ControlsManager.SpecialKey.RollLeft,
      ControlsManager.SpecialKey.RollRight,
      ControlsManager.SpecialKey.Flashlight
    };
    private bool[] inputValues;
    private bool[] previousInputValues;
    private bool leftDown;
    private bool middleDown;
    private bool rightDown;
    private bool previousLeftDown;
    private bool previousMiddleDown;
    private bool previousRightDown;
    private int scroll;
    private int previousScroll;
    private float translationX;
    private float translationY;

    public InputFrame()
    {
      this.inputValues = new bool[256];
      this.previousInputValues = new bool[256];
    }

    public void Clear() => this.StorePreviousState();

    public bool KeyClicked(Keys key)
    {
      return this.inputValues[(int) (byte) key] && !this.previousInputValues[(int) (byte) key];
    }

    public bool KeyClicked(ControlsManager.SpecialKey key)
    {
      return this.inputValues[(int) (byte) Engine.SettingsManager.ControlsManager.GetSpecialKey(key)] && !this.previousInputValues[(int) (byte) Engine.SettingsManager.ControlsManager.GetSpecialKey(key)];
    }

    public bool KeyDown(ControlsManager.SpecialKey key)
    {
      return this.inputValues[(int) (byte) Engine.SettingsManager.ControlsManager.GetSpecialKey(key)];
    }

    public bool KeyDown(Keys key) => key != Keys.None && this.inputValues[(int) (byte) key];

    public void SetUp(
      KeyboardState keyboard,
      MouseState mouse,
      float translationX,
      float translationY)
    {
      this.StorePreviousState();
      foreach (byte pressedKey in keyboard.GetPressedKeys())
        this.inputValues[(int) pressedKey] = true;
      this.leftDown = mouse.LeftButton == ButtonState.Pressed;
      this.middleDown = mouse.MiddleButton == ButtonState.Pressed;
      this.rightDown = mouse.RightButton == ButtonState.Pressed;
      this.scroll = mouse.ScrollWheelValue;
      this.translationX = translationX;
      this.translationY = translationY;
    }

    public void SetUp(ref StateFrame frame)
    {
      this.StorePreviousState();
      this.leftDown = frame.MouseLeft;
      this.middleDown = frame.MouseMiddle;
      this.rightDown = frame.MouseRight;
      ControlsManager controlsManager = Engine.SettingsManager.ControlsManager;
      this.inputValues[(int) (byte) controlsManager.GetSpecialKey(ControlsManager.SpecialKey.OrientateBlock)] = frame.Rotate;
      this.inputValues[(int) (byte) controlsManager.GetSpecialKey(ControlsManager.SpecialKey.Inventory)] = frame.CharacterScreen;
    }

    public void SetUp(ControlBlock block, short buttonStates)
    {
      if (block == null)
        return;
      for (int index = 0; index < Math.Min(16, block.ColorKeyGroups.Count); ++index)
      {
        Keys key = block.ColorKeyGroups[index].Key;
        if ((1 << index & (int) buttonStates) != 0)
          this.inputValues[(int) (byte) key] = true;
      }
    }

    public bool LeftClick => !this.leftDown && this.previousLeftDown;

    public bool MiddleClick => !this.middleDown && this.previousMiddleDown;

    public bool RightClick => !this.rightDown && this.previousRightDown;

    public bool LeftDown => this.leftDown;

    public bool MiddleDown => this.middleDown;

    public bool RightDown => this.rightDown;

    public int Scroll => this.scroll - this.previousScroll;

    public float TranslationX => this.translationX;

    public float TranslationY => this.translationY;

    private void StorePreviousState()
    {
      Array.Copy((Array) this.inputValues, (Array) this.previousInputValues, this.inputValues.Length);
      Array.Clear((Array) this.inputValues, 0, this.inputValues.Length);
      this.previousLeftDown = this.leftDown;
      this.previousMiddleDown = this.middleDown;
      this.previousRightDown = this.rightDown;
      this.previousScroll = this.scroll;
    }
  }
}
