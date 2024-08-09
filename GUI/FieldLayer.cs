// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.FieldLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class FieldLayer : WindowLayer
  {
    public FieldLayer(Rectangle position)
      : base(position)
    {
      Color darkBorderColor = this.darkBorderColor;
      Color lightBorderColor = this.lightBorderColor;
      Color backgroundColor = this.backgroundColor;
      this.darkBorderColor = lightBorderColor;
      this.lightBorderColor = darkBorderColor;
      this.backgroundColor = new Color((byte) 170, (byte) 170, (byte) 170);
    }
  }
}
