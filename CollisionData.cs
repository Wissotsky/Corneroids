// Decompiled with JetBrains decompiler
// Type: CornerSpace.CollisionData
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public struct CollisionData
  {
    private Vector3 collisionNormal;
    private float collisionDepth;
    private float collisionPressure;

    public Vector3 CollisionNormal
    {
      get => this.collisionNormal;
      set => this.collisionNormal = value;
    }

    public float CollisionDepth
    {
      get => this.collisionDepth;
      set => this.collisionDepth = value;
    }

    public float CollisionPressure
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
  }
}
