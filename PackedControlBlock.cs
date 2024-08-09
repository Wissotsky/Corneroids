// Decompiled with JetBrains decompiler
// Type: CornerSpace.PackedControlBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class PackedControlBlock
  {
    public List<ColorKeyGroup> ColorsAndKeys;
    public List<KeyValuePair<ColorKeyGroup, Vector3>> BoundBlocks;
    public Vector3? BoundCamera;
    public Vector3 PositionInEntitySpace;

    public PackedControlBlock()
    {
      this.ColorsAndKeys = new List<ColorKeyGroup>();
      this.BoundBlocks = new List<KeyValuePair<ColorKeyGroup, Vector3>>();
    }

    public void BindBlock(Vector3 position, Color color)
    {
      ColorKeyGroup key = this.ColorsAndKeys.Find((Predicate<ColorKeyGroup>) (g => g.Color == color));
      if (key == null)
        return;
      this.BoundBlocks.Add(new KeyValuePair<ColorKeyGroup, Vector3>(key, position));
    }

    public void BindKeyToColor(Color color, Keys key)
    {
      ColorKeyGroup colorKeyGroup = this.ColorsAndKeys.Find((Predicate<ColorKeyGroup>) (g => g.Color == color));
      if (colorKeyGroup != null)
        colorKeyGroup.Key = key;
      else
        this.ColorsAndKeys.Add(new ColorKeyGroup(key, color));
    }
  }
}
