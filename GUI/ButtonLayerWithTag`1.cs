// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ButtonLayerWithTag`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace.GUI
{
  public class ButtonLayerWithTag<T> : ButtonLayer
  {
    private T tag;

    public ButtonLayerWithTag(Rectangle positionAndSize, string text, T tag)
      : base(positionAndSize, text)
    {
      this.tag = tag;
    }

    public T Tag
    {
      get => this.tag;
      set => this.tag = value;
    }
  }
}
