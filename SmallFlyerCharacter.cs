// Decompiled with JetBrains decompiler
// Type: CornerSpace.SmallFlyerCharacter
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public abstract class SmallFlyerCharacter : HostileCharacter
  {
    private const float cMaxUpkeepDistance = 64f;

    public SmallFlyerCharacter(string modelPath, string texturePath, float radius)
      : base(modelPath, texturePath, radius)
    {
    }

    public override sealed Position3? CheckPositionForSpawning(
      Position3 worldPosition,
      SpaceEntity closestEntity)
    {
      if (closestEntity == null)
        return new Position3?(worldPosition);
      Position3? positionOfClosestBlock = closestEntity.GetPositionOfClosestBlock(worldPosition);
      if (!positionOfClosestBlock.HasValue)
        return new Position3?(worldPosition);
      return (double) (positionOfClosestBlock.Value - worldPosition).LengthSquared() >= 256.0 ? new Position3?(worldPosition) : new Position3?();
    }

    public override void Spawn(Position3 position) => this.Position = position;

    public override float MaxUpkeepDistance => 64f;
  }
}
