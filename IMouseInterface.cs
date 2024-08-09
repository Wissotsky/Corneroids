// Decompiled with JetBrains decompiler
// Type: CornerSpace.IMouseInterface
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public interface IMouseInterface
  {
    bool LeftClick();

    bool LeftDown();

    bool RightClick();

    bool RightDown();

    bool MiddleClick();

    bool MiddleDown();

    void RestoreState();

    int Scroll();

    void StoreState();

    int GetTranslationX();

    int GetTranslationY();

    bool WrapPosition { get; set; }

    Point Position { get; }
  }
}
