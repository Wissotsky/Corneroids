// Decompiled with JetBrains decompiler
// Type: CornerSpace.ControlBlocksStates
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class ControlBlocksStates
  {
    private List<KeyValuePair<Vector3, short>> blocksAndValues = new List<KeyValuePair<Vector3, short>>();

    public void AddBlock(Vector3 position, short buttonValues)
    {
      if (buttonValues == (short) 0)
        return;
      this.blocksAndValues.Add(new KeyValuePair<Vector3, short>(position, buttonValues));
    }

    public void Clear() => this.blocksAndValues.Clear();

    public IEnumerable<KeyValuePair<Vector3, short>> ControlBlocks
    {
      get => (IEnumerable<KeyValuePair<Vector3, short>>) this.blocksAndValues;
    }
  }
}
