// Decompiled with JetBrains decompiler
// Type: CornerSpace.LoadedPlayerContainer`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class LoadedPlayerContainer<T> where T : CornerSpace.Player, new()
  {
    public int BoardedEntityId;
    public int[] BoundEntityIds;
    public T Player;
    public LoadedPlayerContainer<T>.Source CreationSource;

    public LoadedPlayerContainer()
    {
      this.BoardedEntityId = -1;
      this.BoundEntityIds = new int[0];
      this.Player = default (T);
    }

    public enum Source
    {
      Database,
      New,
    }
  }
}
