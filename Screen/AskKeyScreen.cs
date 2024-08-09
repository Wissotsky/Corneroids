// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.AskKeyScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class AskKeyScreen : MenuScreen
  {
    private AskKeyLayer layer;
    private Keys[] pressedKeys;
    private Action<Keys> resultFunction;
    private IKeyboardInterface keyboard;

    public AskKeyScreen(Action<Keys> result)
      : base(GameScreen.Type.Popup, false, false, false)
    {
      this.resultFunction = result;
    }

    public override void UpdateInput()
    {
      this.pressedKeys = this.keyboard.GetPressedKeys();
      if (this.pressedKeys == null || this.pressedKeys.Length <= 0)
        return;
      for (int index = 0; index < this.pressedKeys.Length; ++index)
      {
        if (this.pressedKeys[index] == Keys.Escape)
        {
          this.CloseScreen();
          return;
        }
      }
      for (int index = 0; index < this.pressedKeys.Length; ++index)
      {
        if (this.pressedKeys[index] != Keys.None)
        {
          Keys pressedKey = this.pressedKeys[index];
          if (this.resultFunction != null)
            this.resultFunction(pressedKey);
          this.CloseScreen();
        }
      }
    }

    public override void Load()
    {
      this.layer = new AskKeyLayer();
      this.keyboard = (IKeyboardInterface) this.InputManager.GetKeyboardListener();
    }

    public override void Render() => this.layer.Render();
  }
}
