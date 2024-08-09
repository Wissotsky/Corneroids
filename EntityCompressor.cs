// Decompiled with JetBrains decompiler
// Type: CornerSpace.EntityCompressor
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace CornerSpace
{
  public class EntityCompressor : IDisposable
  {
    private volatile bool disposed;
    private object workLock;
    private BlockSectorCompressor compressor;
    private Queue<PackedEntity> finishedWork;
    private List<EntityCompressor.WorkItem> workQueue;

    public EntityCompressor()
    {
      this.disposed = false;
      this.workLock = new object();
      this.compressor = new BlockSectorCompressor();
      this.finishedWork = new Queue<PackedEntity>();
      this.workQueue = new List<EntityCompressor.WorkItem>();
      ThreadPool.QueueUserWorkItem((WaitCallback) delegate
      {
        this.WorkerFunction();
      });
    }

    public void Dispose()
    {
      this.disposed = true;
      lock (this.workLock)
        Monitor.Pulse(this.workLock);
    }

    public void EnqueueWorkItem(SpaceEntity entity, float priority)
    {
      if (entity == null)
        return;
      lock (this.workLock)
      {
        if (this.workQueue.Find((Predicate<EntityCompressor.WorkItem>) (i => i.Entity == entity)) != null)
          return;
        this.workQueue.Add(new EntityCompressor.WorkItem()
        {
          Entity = entity,
          priority = priority
        });
        SimpleMath.InsertionSort<EntityCompressor.WorkItem>(this.workQueue, (Comparison<EntityCompressor.WorkItem>) ((i1, i2) => (int) ((double) i1.priority - (double) i2.priority)));
        Monitor.Pulse(this.workLock);
      }
    }

    public PackedEntity GetFinishedWorkItem()
    {
      lock (this.workLock)
        return this.finishedWork.Dequeue();
    }

    public bool IsEntityQueued(SpaceEntity entity)
    {
      if (entity == null)
        return false;
      lock (this.workLock)
        return this.workQueue.Find((Predicate<EntityCompressor.WorkItem>) (i => i.Entity == entity)) != null;
    }

    public PackedEntity PeekFinishedWorkItem()
    {
      lock (this.workLock)
        return this.finishedWork.Count > 0 ? this.finishedWork.Peek() : (PackedEntity) null;
    }

    private void WorkerFunction()
    {
      try
      {
        SpaceEntity spaceEntity = (SpaceEntity) null;
        while (!this.disposed)
        {
          lock (this.workLock)
          {
            while (this.workQueue.Count == 0 && !this.disposed)
              Monitor.Wait(this.workLock);
            if (this.disposed)
              break;
            if (this.workQueue.Count > 0)
            {
              spaceEntity = this.workQueue[0].Entity;
              this.workQueue.RemoveAt(0);
            }
            else
              continue;
          }
          if (spaceEntity != null)
          {
            PackedEntity packedEntity = new PackedEntity()
            {
              SpaceEntity = spaceEntity
            };
            for (int index = spaceEntity.BlockSectorManager.SectorList.Count - 1; index >= 0; --index)
            {
              BlockSector sector = spaceEntity.BlockSectorManager.SectorList[index];
              PackedBlockSector result = new PackedBlockSector();
              if (this.compressor.CompressBlockSector(sector, ref result))
                packedEntity.PackedSectors.Add(result);
            }
            lock (this.workLock)
            {
              if (!spaceEntity.IsDisposed)
                this.finishedWork.Enqueue(packedEntity);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Entity compression crashed: " + ex.Message);
      }
    }

    private class WorkItem
    {
      public SpaceEntity Entity;
      public float priority;
    }
  }
}
