// Decompiled with JetBrains decompiler
// Type: CornerSpace.WorldView
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class WorldView
  {
    private List<int> entities;

    public WorldView() => this.entities = new List<int>();

    public void Add(int entityId)
    {
      if (this.entities.Contains(entityId))
        return;
      this.entities.Add(entityId);
    }

    public void Clear() => this.entities.Clear();

    public bool Contains(int entityId) => this.entities.Contains(entityId);

    public void Remove(int entityId) => this.entities.Remove(entityId);

    public IList<int> Entities => (IList<int>) this.entities;
  }
}
