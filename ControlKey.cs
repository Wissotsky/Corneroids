// Decompiled with JetBrains decompiler
// Type: CornerSpace.ControlKey
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Input;

#nullable disable
namespace CornerSpace
{
  public abstract class ControlKey
  {
    public virtual bool Pressed() => false;

    public virtual float PressedRange() => 0.0f;

    public virtual bool Equals(Keys key) => false;

    public virtual void UpdateKeyState(IKeyboardInterface keyboard)
    {
    }
  }
}
