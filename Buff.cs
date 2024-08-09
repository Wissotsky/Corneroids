// Decompiled with JetBrains decompiler
// Type: CornerSpace.Buff
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class Buff
  {
    private Action<Character> applyDelegate;
    private Action<Character> removeDelegate;
    private int durationMS;
    private Rectangle buffRectangle;
    private object id;

    public Buff(
      object id,
      Action<Character> apply,
      Action<Character> remove,
      int durationMS,
      Rectangle buffSprite)
    {
      this.durationMS = durationMS;
      this.applyDelegate = apply;
      this.removeDelegate = remove;
      this.buffRectangle = buffSprite;
      this.id = id;
    }

    public Action<Character> ApplyDelegate => this.applyDelegate;

    public Rectangle BuffSpriteCoords => this.buffRectangle;

    public object Id => this.id;

    public int DurationMS
    {
      get => this.durationMS;
      set => this.durationMS = value;
    }

    public Action<Character> RemoveDelegate => this.removeDelegate;
  }
}
