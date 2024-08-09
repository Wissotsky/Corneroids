// Decompiled with JetBrains decompiler
// Type: CornerSpace.EntityExtractor
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace CornerSpace
{
  public class EntityExtractor : IDisposable
  {
    private Queue<PackedEntityContainer> entitiesToExtract;
    private Queue<SpaceEntity> finishedEntities;
    private volatile bool isDisposed;
    private object workLock;
    private BlockSectorCompressor entityCompressor;

    public EntityExtractor()
    {
      this.entitiesToExtract = new Queue<PackedEntityContainer>();
      this.finishedEntities = new Queue<SpaceEntity>();
      this.workLock = new object();
      this.entityCompressor = new BlockSectorCompressor();
      ThreadPool.QueueUserWorkItem((WaitCallback) delegate
      {
        this.WorkerFunction();
      });
    }

    public void Dispose()
    {
      this.isDisposed = true;
      lock (this.workLock)
        Monitor.Pulse(this.workLock);
    }

    public void ExtractEntity(PackedEntityContainer receivedEntity)
    {
      lock (this.workLock)
      {
        this.entitiesToExtract.Enqueue(receivedEntity);
        Monitor.Pulse(this.workLock);
      }
    }

    public SpaceEntity FinishedEntity()
    {
      lock (this.workLock)
        return this.finishedEntities.Count > 0 ? this.finishedEntities.Dequeue() : (SpaceEntity) null;
    }

    private void WorkerFunction()
    {
      try
      {
        PackedEntityContainer packedEntityContainer = new PackedEntityContainer();
        while (!this.isDisposed)
        {
          lock (this.workLock)
          {
            while (this.entitiesToExtract.Count == 0 && !this.isDisposed)
              Monitor.Wait(this.workLock);
            if (this.isDisposed)
              break;
            packedEntityContainer = this.entitiesToExtract.Dequeue();
          }
          SpaceEntity entity = new SpaceEntity()
          {
            Id = packedEntityContainer.Id
          };
          for (int index = 0; index < packedEntityContainer.BlockSectors.Length; ++index)
            this.entityCompressor.ExtractBlockSectorToEntity(ref packedEntityContainer.BlockSectors[index], entity);
          foreach (PackedControlBlock controlBlock in packedEntityContainer.ControlBlocks)
          {
            BlockCell cellEntityCoords = entity.GetBlockCellEntityCoords(controlBlock.PositionInEntitySpace);
            if (cellEntityCoords != null && cellEntityCoords.Block is ControlBlock block)
            {
              using (List<ColorKeyGroup>.Enumerator enumerator = controlBlock.ColorsAndKeys.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  ColorKeyGroup sentKeyColorGroup = enumerator.Current;
                  ColorKeyGroup colorKeyGroup = block.ColorKeyGroups.Find((Predicate<ColorKeyGroup>) (g => g.Color == sentKeyColorGroup.Color));
                  if (colorKeyGroup != null)
                    colorKeyGroup.Key = sentKeyColorGroup.Key;
                }
              }
              foreach (KeyValuePair<ColorKeyGroup, Vector3> boundBlock in controlBlock.BoundBlocks)
              {
                TriggerBlock blockOfType = entity.GetBlockOfType<TriggerBlock>(boundBlock.Value);
                if (blockOfType != null)
                  block.BindBlock(blockOfType, boundBlock.Key.Color, false);
              }
              if (controlBlock.BoundCamera.HasValue)
              {
                CameraBlock blockOfType = entity.GetBlockOfType<CameraBlock>(controlBlock.BoundCamera.Value);
                if (blockOfType != null)
                  block.BindCamera(blockOfType, false);
              }
            }
          }
          entity.ForceConstruct();
          entity.UpdatePhysicalProperties(false);
          entity.Position = packedEntityContainer.EntityPosition;
          entity.Orientation = packedEntityContainer.EntityOrientation;
          lock (this.workLock)
            this.finishedEntities.Enqueue(entity);
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Entity extractor crashed: " + ex.Message);
      }
    }
  }
}
