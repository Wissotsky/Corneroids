// Decompiled with JetBrains decompiler
// Type: CornerSpace.ICameraInterface
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public interface ICameraInterface
  {
    Vector3 GetLookatVector();

    void LookAt(Vector3 direction);

    void LookAtPoint(Position3 point);

    void Move(float amount);

    void Rotate(float yaw, float pitch);

    Position3 Position { get; }
  }
}
