﻿// Decompiled with JetBrains decompiler
// Type: CornerSpace.OctTreeCollisonContainer`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public struct OctTreeCollisonContainer<T>
  {
    public int TargetNodeSize;
    public List<OctTreeCollisionPoint<T>> CollidedNodes;
  }
}
