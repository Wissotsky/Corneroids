// Decompiled with JetBrains decompiler
// Type: CornerSpace.DummyEntity
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class DummyEntity
  {
    private BoundingSphere entity;
    private string filePath;

    public DummyEntity(BoundingSphere positionAndSize, string filePath)
    {
      this.entity = positionAndSize;
      this.filePath = filePath;
    }

    public BoundingSphere Entity
    {
      get => this.entity;
      set => this.entity = value;
    }

    public string FilePath
    {
      get => this.filePath;
      set => this.filePath = value;
    }
  }
}
