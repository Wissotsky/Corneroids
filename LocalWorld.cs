// Decompiled with JetBrains decompiler
// Type: CornerSpace.LocalWorld
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class LocalWorld : World
  {
    private const byte randomDataTresholdValue = 200;
    private KeyValuePair<int, LocalWorld.GenerateEntity>[] creationFunctionsAndPropabilities = new KeyValuePair<int, LocalWorld.GenerateEntity>[5]
    {
      new KeyValuePair<int, LocalWorld.GenerateEntity>(500, new LocalWorld.GenerateEntity(LocalWorld.GenCreateSingleAsteroid)),
      new KeyValuePair<int, LocalWorld.GenerateEntity>(200, new LocalWorld.GenerateEntity(LocalWorld.GenCreateMultipleAsteroids)),
      new KeyValuePair<int, LocalWorld.GenerateEntity>(0, new LocalWorld.GenerateEntity(LocalWorld.GenCreateAsteroidBelt)),
      new KeyValuePair<int, LocalWorld.GenerateEntity>(300, new LocalWorld.GenerateEntity(LocalWorld.GenCreateRichAsteroid)),
      new KeyValuePair<int, LocalWorld.GenerateEntity>(0, new LocalWorld.GenerateEntity(LocalWorld.GenCreateAlienHullPiece))
    };
    private readonly string databasePath;
    private readonly string key;
    private readonly int seed;
    private PlayerCache<NetworkPlayerOnServer> playerCache;
    private WorldDataStorageInterface storage;
    private IdGen idGenerator;
    private NoiseGenerator noiseGenerator;

    public LocalWorld(
      string name,
      int seed,
      string key,
      string databasePath,
      DateTime creationTime,
      Itemset itemset,
      BlockTextureAtlas blockTextureAtlas,
      SpriteTextureAtlas spriteTextureAtlas)
      : base(name, creationTime, itemset, blockTextureAtlas, spriteTextureAtlas)
    {
      this.databasePath = databasePath;
      this.seed = seed;
      this.key = key;
      this.noiseGenerator = new NoiseGenerator(seed);
      this.playerCache = new PlayerCache<NetworkPlayerOnServer>();
    }

    public override void AddBeacon(BeaconObject beacon)
    {
      if (beacon == null || this.storage == null)
        return;
      this.storage.StoreBeacon(beacon);
    }

    public override string CreateNewPlayerToken() => World.CreateToken(20);

    public override void CreateStorage()
    {
      this.token = World.CreateToken(20);
      WorldIOManager.Instance.RequestDataStorageCreationForWorld((World) this);
    }

    public override void Dispose()
    {
      if (this.storage != null)
        this.storage.Dispose();
      if (this.spaceModel == null)
        return;
      this.spaceModel.Dispose();
    }

    public override int GetId() => this.idGenerator.GetNewId();

    public override NetworkPlayerOnServer GetPlayerWithToken(string token)
    {
      return this.playerCache.GetPlayer(token);
    }

    public override Vector3int GetSectorCoords(Position3 worldPosition)
    {
      Vector3 vector3 = new Position3(worldPosition.X / (long) this.sectorSize, worldPosition.Y / (long) this.sectorSize, worldPosition.Z / (long) this.sectorSize) - Position3.Zero;
      return new Vector3int((int) Math.Floor((double) vector3.X), (int) Math.Floor((double) vector3.Y), (int) Math.Floor((double) vector3.Z));
    }

    public override bool Load()
    {
      this.storage = WorldIOManager.Instance.LoadDataStorageForWorld((World) this);
      if (this.storage != null)
      {
        this.idGenerator = new IdGen(this.storage.GetNextFreeId());
        this.token = this.storage.GetToken();
        this.LoadClientPlayersToCache();
      }
      return this.storage != null;
    }

    public override List<BeaconObject> LoadBeacons()
    {
      return this.storage != null ? this.storage.LoadBeacons() : (List<BeaconObject>) null;
    }

    public override LoadedPlayerContainer<LocalPlayer> LoadLocalPlayer()
    {
      if (this.storage == null)
        return (LoadedPlayerContainer<LocalPlayer>) null;
      LoadedPlayerContainer<LocalPlayer> loadedPlayerContainer = this.storage.LoadPlayer<LocalPlayer>(0);
      if (loadedPlayerContainer.Player != null)
        return loadedPlayerContainer;
      LocalPlayer localPlayer = new LocalPlayer();
      return new LoadedPlayerContainer<LocalPlayer>()
      {
        Player = localPlayer,
        CreationSource = LoadedPlayerContainer<LocalPlayer>.Source.New
      };
    }

    public override void LoadSpaceEntities(
      Vector3int lowerBounds,
      Vector3int upperBounds,
      Position3 playerPosition,
      Action<SpaceEntity> entityLoaded)
    {
      if (this.storage == null || entityLoaded == null)
        return;
      this.storage.LoadEntities(lowerBounds, upperBounds, playerPosition, entityLoaded);
      for (int z = lowerBounds.Z; z <= upperBounds.Z; ++z)
      {
        for (int y = lowerBounds.Y; y <= upperBounds.Y; ++y)
        {
          for (int x = lowerBounds.X; x <= upperBounds.X; ++x)
          {
            Vector3int sectorPosition = new Vector3int(x, y, z);
            if (this.SectorContainsProceduralData(sectorPosition))
              this.storage.SectorDataCreated(sectorPosition, (Action<World.SectorState>) (created =>
              {
                if (created != World.SectorState.not_processed)
                  return;
                this.GenerateEntitiesA((Action<SpaceEntity>) (newEntity =>
                {
                  this.storage.MarkSector(sectorPosition, World.SectorState.entities_generated);
                  entityLoaded(newEntity);
                }), sectorPosition);
                this.storage.MarkSector(sectorPosition, World.SectorState.generating_entities);
              }));
          }
        }
      }
    }

    public override bool PlayerWithTokenExists(string token)
    {
      return this.playerCache.PlayerStored(token);
    }

    public override bool PollIsDatabaseWorkLeft()
    {
      return this.storage != null && this.storage.PollWorkToBeDone();
    }

    public override void RemoveBeacon(BeaconObject beacon)
    {
      if (beacon == null || this.storage == null)
        return;
      this.storage.RemoveBeacon(beacon);
    }

    public override void RemoveEntity(int entityId) => this.storage.RemoveEntity(entityId);

    public override void Save()
    {
      if (this.storage == null)
        return;
      this.storage.Save();
    }

    public override void SavePlayer(Player player, SpaceEntity[] closeEntities)
    {
      if (player == null || this.storage == null)
        return;
      if (player is NetworkPlayerOnServer player1)
      {
        player1.WorldView.Clear();
        player1.Astronaut.StateBuffer.Clear();
        this.playerCache.AddPlayer(player1);
      }
      this.storage.StorePlayer(player, closeEntities);
    }

    public override void StoreEntity(SpaceEntity entity)
    {
      if (entity == null)
        throw new ArgumentNullException();
      if (this.storage == null)
        return;
      this.storage.StoreEntity(entity, this.GetSectorCoords(entity.Position));
    }

    public override string DatabasePath => this.databasePath;

    public override string Key => this.key;

    public override int Seed => this.seed;

    private void GenerateEntitiesA(Action<SpaceEntity> result, Vector3int position)
    {
      if (result == null)
        return;
      BoundingBoxI bounds = new BoundingBoxI(new Position3((Vector3) position * (float) this.sectorSize), new Position3(((Vector3) position + Vector3.One) * (float) this.sectorSize));
      Vector3 vector3_1 = bounds.Max - bounds.Min;
      Random random = new Random(this.noiseGenerator.PRNG(ref position));
      LocalWorld.GenerateEntity generationDelegate = this.GetGenerationDelegate(random.Next(1000));
      if (generationDelegate == null)
        return;
      Vector3 vector3_2 = generationDelegate(random.Next(), bounds, (Action<SpaceEntity>) (e =>
      {
        if (e == null)
          return;
        result(e);
      }));
    }

    private Vector3[] EvaluateRandomPositionsForEntities(
      int entityCount,
      int seed,
      BoundingBox bounds)
    {
      Vector3[] positionsForEntities = new Vector3[entityCount];
      Random random = new Random(seed);
      Vector3 min = bounds.Min;
      Vector3 vector3_1 = bounds.Max - bounds.Min;
      for (int index = 0; index < positionsForEntities.Length; ++index)
        positionsForEntities[index] = min + vector3_1 * new Vector3((float) random.NextDouble(), (float) random.NextDouble(), (float) random.NextDouble());
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < positionsForEntities.Length; ++index2)
        {
          for (int index3 = 0; index3 < positionsForEntities.Length; ++index3)
          {
            if (index2 != index3)
            {
              float num1 = (positionsForEntities[index2] - positionsForEntities[index3]).Length();
              if ((double) num1 < 128.0)
              {
                float num2 = (float) ((128.0 - (double) num1) * 0.5);
                Vector3 vector3_2 = Vector3.Normalize(positionsForEntities[index3] - positionsForEntities[index2]);
                positionsForEntities[index3] += vector3_2 * num2;
                positionsForEntities[index2] -= vector3_2 * num2;
                double num3 = (double) (positionsForEntities[index2] - positionsForEntities[index3]).Length();
                positionsForEntities[index3].X = Math.Min(Math.Max(positionsForEntities[index3].X, bounds.Min.X), bounds.Max.X);
                positionsForEntities[index3].Y = Math.Min(Math.Max(positionsForEntities[index3].Y, bounds.Min.Y), bounds.Max.Y);
                positionsForEntities[index3].Z = Math.Min(Math.Max(positionsForEntities[index3].Z, bounds.Min.Z), bounds.Max.Z);
                positionsForEntities[index2].X = Math.Min(Math.Max(positionsForEntities[index2].X, bounds.Min.X), bounds.Max.X);
                positionsForEntities[index2].Y = Math.Min(Math.Max(positionsForEntities[index2].Y, bounds.Min.Y), bounds.Max.Y);
                positionsForEntities[index2].Z = Math.Min(Math.Max(positionsForEntities[index2].Z, bounds.Min.Z), bounds.Max.Z);
              }
            }
          }
        }
      }
      return positionsForEntities;
    }

    private LocalWorld.GenerateEntity GetGenerationDelegate(int probability)
    {
      int num = 0;
      for (int index = 0; index < this.creationFunctionsAndPropabilities.Length; ++index)
      {
        num += this.creationFunctionsAndPropabilities[index].Key;
        if (num >= probability)
          return this.creationFunctionsAndPropabilities[index].Value;
      }
      return (LocalWorld.GenerateEntity) null;
    }

    private void LoadClientPlayersToCache()
    {
      foreach (LoadedPlayerContainer<NetworkPlayerOnServer> loadClientPlayer in this.storage.LoadClientPlayers())
        this.playerCache.AddPlayer(loadClientPlayer.Player);
    }

    private bool SectorContainsProceduralData(Vector3int position)
    {
      return (position.X != 0 || position.Y != 0 || position.Z != 0) && this.noiseGenerator.PerlinNoise3D((float) position.X, (float) position.Y, (float) position.Z) > (byte) 200;
    }

    private static Vector3 GenCreateAlienHullPiece(
      int seed,
      BoundingBoxI bounds,
      Action<SpaceEntity> result)
    {
      EntityCreationParameters parameters = new EntityCreationParameters()
      {
        PositionBounds = bounds,
        Seed = seed
      };
      SpaceEntityFactory instance = SpaceEntityFactory.Instance;
      instance.CreateEntityAsync(new SpaceEntityFactory.CreateEntityFunction(instance.CreateAlienHullPiece), parameters, result);
      return Vector3.Zero;
    }

    private static Vector3 GenCreateRichAsteroid(
      int seed,
      BoundingBoxI bounds,
      Action<SpaceEntity> result)
    {
      Random random = new Random(seed);
      AsteroidCreationParameters creationParameters = new AsteroidCreationParameters();
      creationParameters.PositionBounds = bounds;
      creationParameters.PreferredSize = Vector3.One * (float) random.Next(64, 128);
      creationParameters.Seed = seed;
      creationParameters.Smoothness = (byte) 6;
      AsteroidCreationParameters parameters = creationParameters;
      SpaceEntityFactory instance = SpaceEntityFactory.Instance;
      instance.CreateEntityAsync(new SpaceEntityFactory.CreateEntityFunction(instance.CreateRichAsteroid), (EntityCreationParameters) parameters, result);
      return parameters.PreferredSize;
    }

    private static Vector3 GenCreateSingleAsteroid(
      int seed,
      BoundingBoxI bounds,
      Action<SpaceEntity> result)
    {
      AsteroidCreationParameters creationParameters = new AsteroidCreationParameters();
      creationParameters.PositionBounds = bounds;
      AsteroidCreationParameters parameters = creationParameters;
      parameters.RandomizeParameters(seed);
      SpaceEntityFactory instance = SpaceEntityFactory.Instance;
      instance.CreateEntityAsync(new SpaceEntityFactory.CreateEntityFunction(instance.CreateAsteroid), (EntityCreationParameters) parameters, result);
      return parameters.PreferredSize;
    }

    private static Vector3 GenCreateMultipleAsteroids(
      int seed,
      BoundingBoxI bounds,
      Action<SpaceEntity> result)
    {
      AsteroidCreationParameters creationParameters = new AsteroidCreationParameters();
      creationParameters.PositionBounds = bounds;
      AsteroidCreationParameters parameters = creationParameters;
      parameters.RandomizeParameters(seed);
      SpaceEntityFactory instance = SpaceEntityFactory.Instance;
      instance.CreateEntityAsync(new SpaceEntityFactory.CreateEntityFunction(instance.CreateMultipleAsteroids), (EntityCreationParameters) parameters, result);
      return parameters.PreferredSize;
    }

    private static Vector3 GenCreateAsteroidBelt(
      int seed,
      BoundingBoxI bounds,
      Action<SpaceEntity> result)
    {
      AsteroidBeltCreationParameters creationParameters = new AsteroidBeltCreationParameters();
      creationParameters.PositionBounds = bounds;
      creationParameters.PreferredSize = Vector3.One * 16f;
      creationParameters.Smoothness = (byte) 3;
      creationParameters.Seed = seed;
      creationParameters.NumberOfAsteroids = 32;
      creationParameters.InnerRadius = 32f;
      creationParameters.OuterRadius = 128f;
      AsteroidBeltCreationParameters parameters = creationParameters;
      SpaceEntityFactory instance = SpaceEntityFactory.Instance;
      instance.CreateEntityAsync(new SpaceEntityFactory.CreateEntityFunction(instance.CreateAsteroidBelt), (EntityCreationParameters) parameters, result);
      return parameters.PreferredSize;
    }

    public delegate Vector3 GenerateEntity(
      int seed,
      BoundingBoxI bounds,
      Action<SpaceEntity> result);
  }
}
