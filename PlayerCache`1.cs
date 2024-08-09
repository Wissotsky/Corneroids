// Decompiled with JetBrains decompiler
// Type: CornerSpace.PlayerCache`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class PlayerCache<T> where T : Player
  {
    private List<T> players;

    public PlayerCache() => this.players = new List<T>();

    public void AddPlayer(T player)
    {
      if ((object) player == null || this.players.Contains(player))
        return;
      this.players.Add(player);
    }

    public T GetPlayer(string token)
    {
      return !string.IsNullOrEmpty(token) ? this.players.Find((Predicate<T>) (p => p.Token == token)) : default (T);
    }

    public bool PlayerStored(string token) => (object) this.GetPlayer(token) != null;
  }
}
