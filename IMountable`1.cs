// Decompiled with JetBrains decompiler
// Type: CornerSpace.IMountable`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public interface IMountable<T>
  {
    SpriteImage Crosshair { get; }

    bool Mount(IController<T> controller);

    void UnMount();

    void Update(InputFrame input, T updater);

    CameraBlock CameraBlock { get; }

    Vector3 Speed { get; }

    short ActiveButtons { get; set; }
  }
}
