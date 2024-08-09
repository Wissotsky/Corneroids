// Decompiled with JetBrains decompiler
// Type: CornerSpace.World
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace CornerSpace
{
  public abstract class World : IDisposable, IPlayerStorage
  {
    protected BlockTextureAtlas blockTextureAtlas;
    protected BlockSpriteTextureAtlas blockSpriteTextureAtlas;
    protected DateTime creationTime;
    protected Itemset itemset;
    protected string name;
    protected readonly int sectorSize;
    protected SpaceModel spaceModel;
    protected SpriteTextureAtlas spriteTextureAtlas;
    protected string token;

    public World(
      string name,
      DateTime creationTime,
      Itemset itemset,
      BlockTextureAtlas blockTextureAtlas,
      SpriteTextureAtlas spriteTextureAtlas)
    {
      this.name = name;
      this.creationTime = creationTime;
      this.itemset = itemset;
      this.blockTextureAtlas = blockTextureAtlas;
      this.spriteTextureAtlas = spriteTextureAtlas;
      this.sectorSize = 256;
    }

    public abstract void AddBeacon(BeaconObject beacon);

    public abstract string CreateNewPlayerToken();

    public abstract void CreateStorage();

    public static string CreateToken(int length)
    {
      length = Math.Max(length, 1);
      try
      {
        string str = Md5.Hasher.Hash(DateTime.Now.ToString() + Path.GetRandomFileName());
        return str.Length > length ? "a" + str.Substring(0, length - 1) : str;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a token for a world! " + ex.Message);
        return "-";
      }
    }

    public virtual void Dispose()
    {
      if (this.blockSpriteTextureAtlas == null)
        return;
      this.blockSpriteTextureAtlas.Dispose();
    }

    public abstract int GetId();

    public abstract NetworkPlayerOnServer GetPlayerWithToken(string token);

    public abstract Vector3int GetSectorCoords(Position3 worldPosition);

    public abstract bool Load();

    public abstract List<BeaconObject> LoadBeacons();

    public abstract LoadedPlayerContainer<LocalPlayer> LoadLocalPlayer();

    public abstract void LoadSpaceEntities(
      Vector3int lowerBounds,
      Vector3int upperBounds,
      Position3 playerPosition,
      Action<SpaceEntity> entityLoaded);

    public abstract bool PlayerWithTokenExists(string token);

    public abstract bool PollIsDatabaseWorkLeft();

    public void RenderBackground(IRenderCamera camera)
    {
      if (this.spaceModel == null)
        return;
      this.spaceModel.Render(camera);
    }

    public abstract void RemoveBeacon(BeaconObject beacon);

    public abstract void RemoveEntity(int entityId);

    public abstract void Save();

    public abstract void SavePlayer(Player player, SpaceEntity[] closeEntities);

    public abstract void StoreEntity(SpaceEntity entity);

    public DateTime CreationTime => this.creationTime;

    public Itemset Itemset => this.itemset;

    public BlockTextureAtlas BlockTextureAtlas => this.blockTextureAtlas;

    public BlockSpriteTextureAtlas BlockSpriteTextureAtlas
    {
      get => this.blockSpriteTextureAtlas;
      set => this.blockSpriteTextureAtlas = value;
    }

    public abstract string DatabasePath { get; }

    public abstract string Key { get; }

    public int SectorSize => this.sectorSize;

    public abstract int Seed { get; }

    public SpaceModel SpaceModel
    {
      set => this.spaceModel = value;
    }

    public SpriteTextureAtlas SpriteTextureAtlas => this.spriteTextureAtlas;

    public string Name => this.name;

    public string Token
    {
      get => this.token;
      protected set => this.token = value;
    }

    public enum SectorState
    {
      not_processed,
      generating_entities,
      entities_generated,
    }
  }
}
