// Decompiled with JetBrains decompiler
// Type: CornerSpace.ILightChunk
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public interface ILightChunk
  {
    List<LightSource> LightSources { get; }

    SpaceEntity Owner { get; }

    Vector3 PositionInShipCoordinates { get; }

    Matrix TransformMatrix { get; }
  }
}
