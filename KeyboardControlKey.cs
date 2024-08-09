// Decompiled with JetBrains decompiler
// Type: CornerSpace.KeyboardControlKey
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Input;

#nullable disable
namespace CornerSpace
{
  public class KeyboardControlKey : ControlKey
  {
    private bool pressed;
    private Keys key;

    public KeyboardControlKey(Keys key) => this.key = key;

    public override bool Pressed() => this.pressed;

    public override bool Equals(Keys key) => this.key == key;

    public override void UpdateKeyState(IKeyboardInterface keyboard)
    {
      this.pressed = keyboard.KeyDown(this.key);
    }
  }
}
