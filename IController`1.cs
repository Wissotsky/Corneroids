// Decompiled with JetBrains decompiler
// Type: CornerSpace.IController`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public interface IController<T>
  {
    T Controller { get; }

    IMountable<T> MountedTo { get; set; }

    Quaternion Orientation { get; }
  }
}
