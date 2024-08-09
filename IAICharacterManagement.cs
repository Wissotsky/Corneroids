// Decompiled with JetBrains decompiler
// Type: CornerSpace.IAICharacterManagement
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public interface IAICharacterManagement
  {
    void AddCharacterSafe(AICharacter enemy);

    EnvironmentManager EnvironmentManager { get; }

    Player GetClosestPlayer(Position3 position);

    void RemoveCharacterSafe(AICharacter enemy);

    ProjectileManager ProjectileManager { get; }
  }
}
