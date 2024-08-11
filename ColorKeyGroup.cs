// Decompiled with JetBrains decompiler
// Type: CornerSpace.ColorKeyGroup
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace CornerSpace
{
  public class ColorKeyGroup
  {
    public Keys Key;
    public Color Color;

    public ColorKeyGroup(Keys key, Color color)
    {
      this.Key = key;
      this.Color = color;
    }
  }
}
