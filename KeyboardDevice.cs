// Decompiled with JetBrains decompiler
// Type: CornerSpace.KeyboardDevice
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class KeyboardDevice : InputDevice, IKeyboardInterface
  {
    private static Dictionary<KeyboardDevice.Action, Keys> keyBinds = new Dictionary<KeyboardDevice.Action, Keys>();
    private KeyboardState keyboardState;
    private KeyboardState previousKeyboardState;

    public override void UpdateDevice()
    {
      this.previousKeyboardState = this.keyboardState;
      this.keyboardState = Keyboard.GetState();
    }

    public Keys[] GetPressedKeys() => this.keyboardState.GetPressedKeys();

    public bool KeyDown(Keys key) => this.keyboardState.IsKeyDown(key);

    public bool KeyPressed(Keys key)
    {
      return this.previousKeyboardState.IsKeyUp(key) && this.keyboardState.IsKeyDown(key);
    }

    public bool KeyReleased(Keys key)
    {
      return this.previousKeyboardState.IsKeyDown(key) && this.keyboardState.IsKeyUp(key);
    }

    public static Dictionary<KeyboardDevice.Action, Keys> KeyBinds => KeyboardDevice.keyBinds;

    public enum Action
    {
      Edit,
      Use,
    }
  }
}
