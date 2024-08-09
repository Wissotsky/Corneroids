// Decompiled with JetBrains decompiler
// Type: CornerSpace.ScreenBuffer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class ScreenBuffer
  {
    private byte[,] bufferArray;

    public ScreenBuffer() => this.bufferArray = new byte[32, 20];

    public byte AddItem(Vector2 screenPosition)
    {
      Point resolution = Engine.SettingsManager.Resolution;
      int index1 = (int) ((double) screenPosition.X / (double) resolution.X * (double) this.bufferArray.GetLength(0));
      int index2 = (int) ((double) screenPosition.Y / (double) resolution.Y * (double) this.bufferArray.GetLength(1));
      return index1 < 0 || index1 >= this.bufferArray.GetLength(0) || index2 < 0 || index2 >= this.bufferArray.GetLength(1) ? byte.MaxValue : this.bufferArray[index1, index2]++;
    }

    public void Clear() => Array.Clear((Array) this.bufferArray, 0, this.bufferArray.Length);
  }
}
