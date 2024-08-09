// Decompiled with JetBrains decompiler
// Type: CornerSpace.InputManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class InputManager : IInput
  {
    private List<InputDevice> inputDevices;
    private InputReader inputReader;
    private InputFrame inputFrame;

    public InputManager(GameWindow window)
    {
      this.inputDevices = new List<InputDevice>();
      this.inputReader = new InputReader(window);
      this.inputFrame = new InputFrame();
    }

    public MouseDevice GetMouseListener()
    {
      foreach (InputDevice inputDevice in this.inputDevices)
      {
        if (inputDevice is MouseDevice)
          return (MouseDevice) inputDevice;
      }
      MouseDevice mouseListener = new MouseDevice();
      this.inputDevices.Add((InputDevice) mouseListener);
      return mouseListener;
    }

    public KeyboardDevice GetKeyboardListener()
    {
      foreach (InputDevice inputDevice in this.inputDevices)
      {
        if (inputDevice is KeyboardDevice)
          return (KeyboardDevice) inputDevice;
      }
      KeyboardDevice keyboardListener = new KeyboardDevice();
      this.inputDevices.Add((InputDevice) keyboardListener);
      return keyboardListener;
    }

    public InputReader InputReader => this.inputReader;

    public void Update()
    {
      foreach (InputDevice inputDevice in this.inputDevices)
        inputDevice.UpdateDevice();
      MouseDevice mouseListener = this.GetMouseListener();
      this.inputFrame.SetUp(Keyboard.GetState(), Mouse.GetState(), (float) mouseListener.GetTranslationX(), (float) mouseListener.GetTranslationY());
    }

    public InputFrame InputFrame => this.inputFrame;
  }
}
