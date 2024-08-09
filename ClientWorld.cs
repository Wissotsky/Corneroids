// Decompiled with JetBrains decompiler
// Type: CornerSpace.ClientWorld
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class ClientWorld : World
  {
    public ClientWorld(
      string name,
      DateTime creationTime,
      Itemset itemset,
      SpriteTextureAtlas spriteTextureAtlas,
      BlockTextureAtlas blockTextureAtlas)
      : base(name, creationTime, itemset, blockTextureAtlas, spriteTextureAtlas)
    {
    }

    public override void AddBeacon(BeaconObject beacon)
    {
    }

    public override string CreateNewPlayerToken() => (string) null;

    public override void CreateStorage()
    {
    }

    public override int GetId() => 0;

    public override NetworkPlayerOnServer GetPlayerWithToken(string token)
    {
      return (NetworkPlayerOnServer) null;
    }

    public override Vector3int GetSectorCoords(Position3 worldPosition) => Vector3int.Zero;

    public override bool Load() => false;

    public override List<BeaconObject> LoadBeacons() => (List<BeaconObject>) null;

    public override string DatabasePath => (string) null;

    public override string Key => (string) null;

    public override LoadedPlayerContainer<LocalPlayer> LoadLocalPlayer()
    {
      return (LoadedPlayerContainer<LocalPlayer>) null;
    }

    public override void LoadSpaceEntities(
      Vector3int lowerBounds,
      Vector3int upperBounds,
      Position3 playerPosition,
      Action<SpaceEntity> entityLoaded)
    {
    }

    public override bool PlayerWithTokenExists(string token) => false;

    public override bool PollIsDatabaseWorkLeft() => false;

    public override void RemoveBeacon(BeaconObject beacon)
    {
    }

    public override void RemoveEntity(int entityId)
    {
    }

    public override void Save()
    {
    }

    public override void SavePlayer(Player player, SpaceEntity[] closeEntities)
    {
    }

    public override int Seed => 0;

    public override void StoreEntity(SpaceEntity entity)
    {
    }
  }
}
