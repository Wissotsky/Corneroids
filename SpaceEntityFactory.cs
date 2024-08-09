// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceEntityFactory
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace CornerSpace
{
  public class SpaceEntityFactory
  {
    private object workerLock = new object();
    private static SpaceEntityFactory instance;
    private bool workerWorkingA;
    private Queue<SpaceEntityFactory.AsyncEntityContainer> entityQueueA;

    private SpaceEntityFactory()
    {
      this.entityQueueA = new Queue<SpaceEntityFactory.AsyncEntityContainer>();
      this.workerWorkingA = false;
    }

    public void CreateAlienHullPiece(
      EntityCreationParameters parameters,
      Action<SpaceEntity> result)
    {
      if (parameters == null || result == null)
        return;
      SpaceEntity entity = (SpaceEntity) null;
      try
      {
        entity = new SpaceEntity();
        Random random = new Random(parameters.Seed);
        BlockType blockType = Engine.LoadedWorld.Itemset.BlockTypes[0];
        Vector3 vector3 = new Vector3(64f, 32f, 1f);
        for (int index1 = 0; (double) index1 < (double) vector3.Z; ++index1)
        {
          for (int index2 = 0; (double) index2 < (double) vector3.Y; ++index2)
          {
            if (0.0 < (double) vector3.X)
              throw new NotImplementedException();
          }
        }
        entity.ForceConstruct();
        entity.UpdatePhysicalProperties(true);
        this.EvaluateRandomPositionAndOrientation(random, entity, parameters.PositionBounds, Vector3.One * 64f);
        result(entity);
      }
      catch (Exception ex)
      {
        entity?.Dispose();
        Engine.Console.WriteErrorLine("Failed to create an alien entity: " + ex.Message);
      }
    }

    public void CreateMultipleAsteroids(
      EntityCreationParameters parameters,
      Action<SpaceEntity> result)
    {
      if (parameters == null || result == null)
        return;
      Random random = new Random(parameters.Seed);
      int num = random.Next(1, 4);
      List<SpaceEntity> createdEntities = new List<SpaceEntity>();
      for (int index = 0; index < num; ++index)
      {
        AsteroidCreationParameters creationParameters = new AsteroidCreationParameters();
        creationParameters.PositionBounds = parameters.PositionBounds;
        AsteroidCreationParameters parameters1 = creationParameters;
        parameters1.RandomizeParameters(random.Next());
        this.CreateAsteroid((EntityCreationParameters) parameters1, (Action<SpaceEntity>) (entity =>
        {
          if (entity == null)
            return;
          createdEntities.Add(entity);
        }));
      }
      BoundingBoxI region = new BoundingBoxI(parameters.PositionBounds.Min + Vector3.One * 16f, parameters.PositionBounds.Max - Vector3.One * 16f);
      this.SetPositionsForEntities(createdEntities, region, new int?(random.Next()));
      createdEntities.ForEach((Action<SpaceEntity>) (e => result(e)));
    }

    public void CreateAsteroidBelt(EntityCreationParameters parameters, Action<SpaceEntity> result)
    {
      if (parameters == null || !(parameters is AsteroidBeltCreationParameters))
        return;
      AsteroidBeltCreationParameters creationParameters = parameters as AsteroidBeltCreationParameters;
      int numberOfAsteroids = creationParameters.NumberOfAsteroids;
      Vector3 preferredSize = creationParameters.PreferredSize;
      float outerRadius = creationParameters.OuterRadius;
      float innerRadius = creationParameters.InnerRadius;
      BoundingBoxI positionBounds = parameters.PositionBounds;
      int seed = creationParameters.Seed;
      Position3 sectorPosition = positionBounds.Min + (positionBounds.Max - positionBounds.Min) * 0.5f;
      Random random = new Random();
      Matrix orientationMatrix = Matrix.CreateFromYawPitchRoll((float) random.NextDouble() * 6.28318548f, (float) random.NextDouble() * 6.28318548f, (float) random.NextDouble() * 6.28318548f);
      List<SpaceEntity> createdEntities = new List<SpaceEntity>();
      try
      {
        for (int index = 0; index < numberOfAsteroids; ++index)
        {
          parameters.Seed = random.Next();
          this.CreateAsteroid(parameters, (Action<SpaceEntity>) (newEntity =>
          {
            if (newEntity == null)
              return;
            double num = random.NextDouble() * Math.PI * 2.0;
            Vector3 result1 = ((float) random.NextDouble() * (outerRadius - innerRadius) + innerRadius) * new Vector3((float) Math.Cos(num), 0.0f, (float) Math.Sin(num)) + Vector3.Up * (float) random.NextDouble() * 16f;
            Vector3.Transform(ref result1, ref orientationMatrix, out result1);
            Position3 position3 = result1 + sectorPosition;
            newEntity.Position = position3;
            createdEntities.Add(newEntity);
          }));
        }
        BoundingBoxI region = new BoundingBoxI(parameters.PositionBounds.Min + Vector3.One * 16f, parameters.PositionBounds.Max - Vector3.One * 16f);
        this.SetPositionsForEntities(createdEntities, region, new int?());
        createdEntities.ForEach((Action<SpaceEntity>) (e => result(e)));
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create an asteroid belt: " + ex.Message);
      }
    }

    public void CreateEntityAsync(
      SpaceEntityFactory.CreateEntityFunction function,
      EntityCreationParameters parameters,
      Action<SpaceEntity> resultEntity)
    {
      if (function == null || parameters == null || resultEntity == null)
        throw new ArgumentNullException();
      lock (this.entityQueueA)
        this.entityQueueA.Enqueue(new SpaceEntityFactory.AsyncEntityContainer(function, parameters, resultEntity));
      this.StartWorking();
    }

    public void CreateAsteroid(EntityCreationParameters parameters, Action<SpaceEntity> result)
    {
      if (parameters == null || !(parameters is AsteroidCreationParameters))
        return;
      AsteroidCreationParameters creationParameters = parameters as AsteroidCreationParameters;
      int seed = creationParameters.Seed;
      Vector3 preferredSize = creationParameters.PreferredSize;
      byte smoothness = creationParameters.Smoothness;
      float num1 = (0.5f * preferredSize).Length();
      BoundingBoxI positionBounds = parameters.PositionBounds;
      SpaceEntity spaceEntity = (SpaceEntity) null;
      try
      {
        Itemset itemset = Engine.LoadedWorld.Itemset;
        spaceEntity = new SpaceEntity();
        Random random = new Random(seed);
        Vector3[] vector3Array = new Vector3[7]
        {
          new Vector3(-1f, 0.0f, 0.0f),
          new Vector3(-1f, 0.0f, -1f),
          new Vector3(0.0f, 0.0f, -1f),
          new Vector3(0.0f, -1f, 0.0f),
          new Vector3(-1f, -1f, 0.0f),
          new Vector3(-1f, -1f, -1f),
          new Vector3(0.0f, -1f, -1f)
        };
        NoiseGenerator noiseGenerator1 = new NoiseGenerator(random.Next());
        noiseGenerator1.Masks.SphericalFallofDistance = num1 * 0.8f;
        noiseGenerator1.Masks.SphericalGradient = false;
        NoiseGenerator noiseGenerator2 = new NoiseGenerator(random.Next());
        List<NoiseGenerator>[] noiseGeneratorListArray = this.InitializeAsteroidDepositGenerators(itemset, seed);
        if (noiseGeneratorListArray == null)
          return;
        NoiseGenerator.Mask mask = new NoiseGenerator.Mask(noiseGenerator1.Masks.SphericalBurnMaskFunction);
        int lightAsteroidBlocks = itemset.NumberOfLightAsteroidBlocks;
        int mediumAsteroidBlocks = itemset.NumberOfMediumAsteroidBlocks;
        int heavyAsteroidBlocks = itemset.NumberOfHeavyAsteroidBlocks;
        Vector3 vector3_1 = preferredSize * 0.5f;
        for (int z = 0; (double) z < (double) preferredSize.Z; ++z)
        {
          for (int y = 0; (double) y < (double) preferredSize.Y; ++y)
          {
            for (int x = 0; (double) x < (double) preferredSize.X; ++x)
            {
              Vector3 vector3_2 = new Vector3((float) x, (float) y, (float) z);
              Position3 position = new Position3(vector3_2);
              if (noiseGenerator1.PerlinNoise3DWithMask(vector3_2, ref preferredSize, (int) smoothness, mask) > (byte) 80 && noiseGenerator2.PerlinNoise3D((float) x, (float) y, (float) z, 3) > (byte) 70)
              {
                double num2 = (double) Vector3.DistanceSquared(vector3_2, vector3_1) / ((double) num1 * (double) num1);
                AsteroidBlockType.Type asteroidType = AsteroidBlockType.Type.Light;
                if (noiseGenerator1.PerlinNoise3D((float) x, (float) y, (float) z, 2) > (byte) 138)
                {
                  MineralBlockType mineralBlockType = (MineralBlockType) null;
                  for (int index = 0; index < vector3Array.Length && (Item) mineralBlockType == (Item) null; ++index)
                  {
                    BlockCell cellEntityCoords = spaceEntity.GetBlockCellEntityCoords(vector3_2 + vector3Array[index]);
                    if (cellEntityCoords != null)
                      mineralBlockType = cellEntityCoords.Block.GetBlockType() as MineralBlockType;
                  }
                  if ((Item) mineralBlockType != (Item) null)
                    spaceEntity.AddBlockFast(mineralBlockType.CreateBlock(), position);
                  else
                    spaceEntity.AddBlockFast(itemset.GetMineralByProbability((int) asteroidType, random.Next(0, 1000)), position);
                }
                else
                {
                  byte num3 = 0;
                  int number = 0;
                  if (asteroidType == AsteroidBlockType.Type.Light)
                  {
                    for (int index = 0; index < lightAsteroidBlocks; ++index)
                    {
                      byte num4 = noiseGeneratorListArray[0][index].PerlinNoise3D((float) x, (float) y, (float) z, 3);
                      if ((int) num4 > (int) num3)
                      {
                        num3 = num4;
                        number = index;
                      }
                    }
                    spaceEntity.AddBlockFast(itemset.GetAsteroidBlock(AsteroidBlockType.Type.Light, number), position);
                  }
                }
              }
            }
          }
        }
        spaceEntity.ForceConstruct();
        spaceEntity.UpdatePhysicalProperties(true);
        Vector3 vector3_3 = positionBounds.Max - positionBounds.Min - preferredSize;
        spaceEntity.Position = positionBounds.Min + (positionBounds.Max - positionBounds.Min) * 0.5f + new Vector3((float) (random.NextDouble() - 0.5) * vector3_3.X, (float) (random.NextDouble() - 0.5) * vector3_3.Y, (float) (random.NextDouble() - 0.5) * vector3_3.Z);
        result(spaceEntity);
      }
      catch (Exception ex)
      {
        spaceEntity?.Dispose();
        Engine.Console.WriteErrorLine("Failed to create a huge asteroid: " + ex.Message);
      }
    }

    public void CreateRichAsteroid(EntityCreationParameters parameters, Action<SpaceEntity> result)
    {
      if (parameters == null || !(parameters is AsteroidCreationParameters))
        return;
      AsteroidCreationParameters creationParameters = parameters as AsteroidCreationParameters;
      int seed = creationParameters.Seed;
      Vector3 preferredSize = creationParameters.PreferredSize;
      byte smoothness = creationParameters.Smoothness;
      float num1 = (0.5f * preferredSize).Length();
      BoundingBoxI positionBounds = parameters.PositionBounds;
      SpaceEntity spaceEntity = (SpaceEntity) null;
      try
      {
        Itemset itemset = Engine.LoadedWorld.Itemset;
        spaceEntity = new SpaceEntity();
        Random random = new Random(seed);
        Vector3[] vector3Array = new Vector3[7]
        {
          new Vector3(-1f, 0.0f, 0.0f),
          new Vector3(-1f, 0.0f, -1f),
          new Vector3(0.0f, 0.0f, -1f),
          new Vector3(0.0f, -1f, 0.0f),
          new Vector3(-1f, -1f, 0.0f),
          new Vector3(-1f, -1f, -1f),
          new Vector3(0.0f, -1f, -1f)
        };
        NoiseGenerator noiseGenerator1 = new NoiseGenerator(random.Next());
        noiseGenerator1.Masks.SphericalFallofDistance = num1 * 0.8f;
        noiseGenerator1.Masks.SphericalGradient = false;
        NoiseGenerator noiseGenerator2 = new NoiseGenerator(random.Next());
        List<NoiseGenerator>[] noiseGeneratorListArray = this.InitializeAsteroidDepositGenerators(itemset, seed);
        if (noiseGeneratorListArray == null)
          return;
        NoiseGenerator.Mask mask = new NoiseGenerator.Mask(noiseGenerator1.Masks.SphericalBurnMaskFunction);
        int lightAsteroidBlocks = itemset.NumberOfLightAsteroidBlocks;
        int mediumAsteroidBlocks = itemset.NumberOfMediumAsteroidBlocks;
        int heavyAsteroidBlocks = itemset.NumberOfHeavyAsteroidBlocks;
        Vector3 vector3_1 = preferredSize * 0.5f;
        for (int z = 0; (double) z < (double) preferredSize.Z; ++z)
        {
          for (int y = 0; (double) y < (double) preferredSize.Y; ++y)
          {
            for (int x = 0; (double) x < (double) preferredSize.X; ++x)
            {
              Vector3 vector3_2 = new Vector3((float) x, (float) y, (float) z);
              Position3 position = new Position3(vector3_2);
              if (noiseGenerator1.PerlinNoise3DWithMask(vector3_2, ref preferredSize, (int) smoothness, mask) > (byte) 80 && noiseGenerator2.PerlinNoise3D((float) x, (float) y, (float) z, 3) > (byte) 70)
              {
                float num2 = Vector3.DistanceSquared(vector3_2, vector3_1) / (num1 * num1);
                AsteroidBlockType.Type asteroidType = (double) num2 <= 1.0 / 16.0 ? ((double) num2 <= 0.0099999997764825821 ? AsteroidBlockType.Type.Heavy : AsteroidBlockType.Type.Medium) : AsteroidBlockType.Type.Light;
                if (noiseGenerator1.PerlinNoise3D((float) x, (float) y, (float) z, 2) > (byte) 138)
                {
                  MineralBlockType mineralBlockType = (MineralBlockType) null;
                  for (int index = 0; index < vector3Array.Length && (Item) mineralBlockType == (Item) null; ++index)
                  {
                    BlockCell cellEntityCoords = spaceEntity.GetBlockCellEntityCoords(vector3_2 + vector3Array[index]);
                    if (cellEntityCoords != null)
                      mineralBlockType = cellEntityCoords.Block.GetBlockType() as MineralBlockType;
                  }
                  if ((Item) mineralBlockType != (Item) null)
                    spaceEntity.AddBlockFast(mineralBlockType.CreateBlock(), position);
                  else
                    spaceEntity.AddBlockFast(itemset.GetMineralByProbability((int) asteroidType, random.Next(0, 1000)), position);
                }
                else
                {
                  byte num3 = 0;
                  int number = 0;
                  switch (asteroidType)
                  {
                    case AsteroidBlockType.Type.Light:
                      for (int index = 0; index < lightAsteroidBlocks; ++index)
                      {
                        byte num4 = noiseGeneratorListArray[0][index].PerlinNoise3D((float) x, (float) y, (float) z, 3);
                        if ((int) num4 > (int) num3)
                        {
                          num3 = num4;
                          number = index;
                        }
                      }
                      spaceEntity.AddBlockFast(itemset.GetAsteroidBlock(AsteroidBlockType.Type.Light, number), position);
                      continue;
                    case AsteroidBlockType.Type.Medium:
                      for (int index = 0; index < mediumAsteroidBlocks; ++index)
                      {
                        byte num5 = noiseGeneratorListArray[1][index].PerlinNoise3D((float) x, (float) y, (float) z, 3);
                        if ((int) num5 > (int) num3)
                        {
                          num3 = num5;
                          number = index;
                        }
                      }
                      spaceEntity.AddBlockFast(itemset.GetAsteroidBlock(AsteroidBlockType.Type.Medium, number), position);
                      continue;
                    default:
                      for (int index = 0; index < heavyAsteroidBlocks; ++index)
                      {
                        byte num6 = noiseGeneratorListArray[2][index].PerlinNoise3D((float) x, (float) y, (float) z, 3);
                        if ((int) num6 > (int) num3)
                        {
                          num3 = num6;
                          number = index;
                        }
                      }
                      spaceEntity.AddBlockFast(itemset.GetAsteroidBlock(AsteroidBlockType.Type.Heavy, number), position);
                      continue;
                  }
                }
              }
            }
          }
        }
        spaceEntity.ForceConstruct();
        spaceEntity.UpdatePhysicalProperties(true);
        Vector3 vector3_3 = positionBounds.Max - positionBounds.Min - preferredSize;
        spaceEntity.Position = positionBounds.Min + (positionBounds.Max - positionBounds.Min) * 0.5f + new Vector3(((float) random.NextDouble() - 0.5f) * vector3_3.X, ((float) random.NextDouble() - 0.5f) * vector3_3.Y, ((float) random.NextDouble() - 0.5f) * vector3_3.Z);
        result(spaceEntity);
      }
      catch (Exception ex)
      {
        spaceEntity?.Dispose();
        Engine.Console.WriteErrorLine("Failed to create a huge asteroid: " + ex.Message);
      }
    }

    public static SpaceEntityFactory Instance
    {
      get
      {
        if (SpaceEntityFactory.instance == null)
          SpaceEntityFactory.instance = new SpaceEntityFactory();
        return SpaceEntityFactory.instance;
      }
    }

    public bool Working
    {
      get
      {
        lock (this.workerLock)
          return this.workerWorkingA;
      }
    }

    private void EvaluateRandomPositionAndOrientation(
      Random random,
      SpaceEntity entity,
      BoundingBoxI bounds,
      Vector3 marginals)
    {
      if (random == null || entity == null)
        return;
      Vector3 vector3 = bounds.Max - bounds.Min - marginals;
      entity.Position = bounds.Min + 0.5f * vector3 + new Vector3((float) (random.NextDouble() - 0.5) * vector3.X, (float) (random.NextDouble() - 0.5) * vector3.Y, (float) (random.NextDouble() - 0.5) * vector3.Z);
      entity.Orientation = Quaternion.CreateFromYawPitchRoll((float) random.NextDouble() * 6.28318548f, (float) random.NextDouble() * 6.28318548f, (float) random.NextDouble() * 6.28318548f);
    }

    private List<NoiseGenerator>[] InitializeAsteroidDepositGenerators(
      Itemset itemset,
      int baseSeed)
    {
      if (itemset == null)
        return (List<NoiseGenerator>[]) null;
      Random random = new Random(baseSeed);
      List<NoiseGenerator>[] noiseGeneratorListArray = new List<NoiseGenerator>[3]
      {
        new List<NoiseGenerator>(),
        new List<NoiseGenerator>(),
        new List<NoiseGenerator>()
      };
      for (int index = 0; index < itemset.NumberOfLightAsteroidBlocks; ++index)
        noiseGeneratorListArray[0].Add(new NoiseGenerator(random.Next()));
      for (int index = 0; index < itemset.NumberOfMediumAsteroidBlocks; ++index)
        noiseGeneratorListArray[1].Add(new NoiseGenerator(random.Next()));
      for (int index = 0; index < itemset.NumberOfHeavyAsteroidBlocks; ++index)
        noiseGeneratorListArray[2].Add(new NoiseGenerator(random.Next()));
      return noiseGeneratorListArray;
    }

    private bool IsWork()
    {
      lock (this.entityQueueA)
        return this.entityQueueA.Count > 0;
    }

    private void SetPositionsForEntities(
      List<SpaceEntity> entities,
      BoundingBoxI region,
      int? seed)
    {
      if (entities == null)
        return;
      Vector3 vector3_1 = region.Max - region.Min;
      if (seed.HasValue)
      {
        Random random = new Random(seed.Value);
        foreach (PhysicalObject entity in entities)
          entity.Position = region.Min + new Position3(new Vector3(vector3_1.X * (float) random.NextDouble(), vector3_1.Y * (float) random.NextDouble(), vector3_1.Z * (float) random.NextDouble()));
      }
      for (int index1 = 0; index1 < 5; ++index1)
      {
        for (int index2 = 0; index2 < entities.Count; ++index2)
        {
          for (int index3 = 0; index3 < entities.Count; ++index3)
          {
            if (index2 != index3)
            {
              Position3 position1 = entities[index2].Position;
              Position3 position2 = entities[index3].Position;
              float num1 = (position2 - position1).Length();
              float num2 = (float) ((double) entities[index2].BoundingSphere.Radius + (double) entities[index3].BoundingSphere.Radius + 16.0);
              if ((double) num1 < (double) num2)
              {
                float num3 = (float) (((double) num2 - (double) num1) * 0.5);
                Vector3 vector3_2 = Vector3.Normalize(position2 - position1);
                SpaceEntity entity1 = entities[index3];
                entity1.Position = entity1.Position + vector3_2 * num3;
                SpaceEntity entity2 = entities[index2];
                entity2.Position = entity2.Position - vector3_2 * num3;
                double num4 = (double) (entities[index2].Position - entities[index3].Position).Length();
                position2.X = Math.Min(Math.Max(position2.X, region.Min.X), region.Max.X);
                position2.Y = Math.Min(Math.Max(position2.Y, region.Min.Y), region.Max.Y);
                position2.Z = Math.Min(Math.Max(position2.Z, region.Min.Z), region.Max.Z);
                position1.X = Math.Min(Math.Max(position1.X, region.Min.X), region.Max.X);
                position1.Y = Math.Min(Math.Max(position1.Y, region.Min.Y), region.Max.Y);
                position1.Z = Math.Min(Math.Max(position1.Z, region.Min.Z), region.Max.Z);
                entities[index2].Position = position1;
                entities[index3].Position = position2;
              }
            }
          }
        }
      }
    }

    private void StartWorking()
    {
      lock (this.workerLock)
      {
        if (this.workerWorkingA)
          return;
        this.workerWorkingA = true;
        ThreadPool.QueueUserWorkItem((WaitCallback) delegate
        {
          SpaceEntityFactory.AsyncEntityContainer asyncEntityContainer = (SpaceEntityFactory.AsyncEntityContainer) null;
          while (this.IsWork())
          {
            lock (this.entityQueueA)
              asyncEntityContainer = this.entityQueueA.Dequeue();
            asyncEntityContainer.Function(asyncEntityContainer.Parameters, asyncEntityContainer.Result);
          }
          lock (this.workerLock)
            this.workerWorkingA = false;
        });
      }
    }

    public class AsyncEntityContainer
    {
      public SpaceEntityFactory.CreateEntityFunction Function;
      public EntityCreationParameters Parameters;
      public Action<SpaceEntity> Result;

      public AsyncEntityContainer(
        SpaceEntityFactory.CreateEntityFunction function,
        EntityCreationParameters parameters,
        Action<SpaceEntity> result)
      {
        this.Function = function;
        this.Parameters = parameters;
        this.Result = result;
      }
    }

    public delegate void CreateEntityFunction(
      EntityCreationParameters parameters,
      Action<SpaceEntity> result);
  }
}
