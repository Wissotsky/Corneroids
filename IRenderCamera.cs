// Decompiled with JetBrains decompiler
// Type: CornerSpace.IRenderCamera
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public interface IRenderCamera
  {
    Vector3 GetLookatVector();

    Vector2? GetPositionInScreenSpace(Position3 worldPosition);

    Vector3 GetPositionRelativeToCamera(Position3 position);

    BoundingFrustum ViewFrustum { get; }

    Matrix ViewMatrix { get; }

    Quaternion Orientation { get; }

    Matrix ProjectionMatrix { get; }

    Position3 Position { get; }

    Vector3 GetUpVector();
  }
}
