// Decompiled with JetBrains decompiler
// Type: CornerSpace.EntityTransferHelper
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace
{
  public class EntityTransferHelper : IDisposable
  {
    private EntityCompressor compressor;
    private List<EntityTransferHelper.PlayerEntityQueue> playersAndEntitiesToTransfer;
    private NetworkServerManager server;

    public EntityTransferHelper(NetworkServerManager server)
    {
      this.server = server != null ? server : throw new ArgumentNullException("NetworkServerManager server");
      this.compressor = new EntityCompressor();
      this.playersAndEntitiesToTransfer = new List<EntityTransferHelper.PlayerEntityQueue>();
    }

    public void Dispose() => this.compressor.Dispose();

    public void EnqueueEntityForTransfer(SpaceEntity entity, NetworkPlayer targetClient)
    {
      if (entity == null || targetClient == null || this.IsEntityQueuedForPlayer(targetClient, entity))
        return;
      float priority = (entity.Position - targetClient.Astronaut.Position).LengthSquared();
      if (!this.compressor.IsEntityQueued(entity))
        this.compressor.EnqueueWorkItem(entity, priority);
      this.EnqueueEntityForPlayer(targetClient, entity);
    }

    public ServerEntityTransactionHandler GetEntitySenderForQueuedEntity()
    {
      PackedEntity packedEntity = this.compressor.PeekFinishedWorkItem();
      if (packedEntity != null)
      {
        EntityTransferHelper.PlayerEntityQueue[] array = this.playersAndEntitiesToTransfer.Where<EntityTransferHelper.PlayerEntityQueue>((Func<EntityTransferHelper.PlayerEntityQueue, bool>) (q => q.EntityBeingTransferred == null && q.EntityQueue.Contains(packedEntity.SpaceEntity))).ToArray<EntityTransferHelper.PlayerEntityQueue>();
        if (array.Length > 0)
        {
          EntityTransferHelper.PlayerEntityQueue target = array[0];
          target.EntityBeingTransferred = packedEntity.SpaceEntity;
          target.EntityQueue.Remove(packedEntity.SpaceEntity);
          ServerToClientConnection connectionOfId = this.server.GetConnectionOfId(target.Client.Id);
          if (connectionOfId != null)
            return new ServerEntityTransactionHandler(connectionOfId.Connection, (NetworkServer) this.server, packedEntity, target.Client, (Action<bool>) (sent => this.MarkPlayerReadyForNextTransaction(target.Client)));
        }
        if (this.playersAndEntitiesToTransfer.FindAll((Predicate<EntityTransferHelper.PlayerEntityQueue>) (p => p.EntityQueue.Contains(packedEntity.SpaceEntity))).Count == 0)
          this.compressor.GetFinishedWorkItem();
      }
      return (ServerEntityTransactionHandler) null;
    }

    public void PlayerDisconnected(int connectionId)
    {
      EntityTransferHelper.PlayerEntityQueue playerEntityQueue = this.playersAndEntitiesToTransfer.Find((Predicate<EntityTransferHelper.PlayerEntityQueue>) (p => p.Client.Id == connectionId));
      if (playerEntityQueue == null)
        return;
      this.playersAndEntitiesToTransfer.Remove(playerEntityQueue);
    }

    private void EnqueueEntityForPlayer(NetworkPlayer client, SpaceEntity entity)
    {
      if (client == null || entity == null)
        return;
      EntityTransferHelper.PlayerEntityQueue playerEntityQueue = this.playersAndEntitiesToTransfer.Find((Predicate<EntityTransferHelper.PlayerEntityQueue>) (c => c.Client == client));
      if (playerEntityQueue != null)
        playerEntityQueue.EntityQueue.Add(entity);
      else
        this.playersAndEntitiesToTransfer.Add(new EntityTransferHelper.PlayerEntityQueue()
        {
          Client = client,
          EntityQueue = {
            entity
          }
        });
    }

    private bool IsEntityQueuedForPlayer(NetworkPlayer client, SpaceEntity entity)
    {
      if (client != null && entity != null)
      {
        EntityTransferHelper.PlayerEntityQueue playerEntityQueue = this.playersAndEntitiesToTransfer.Find((Predicate<EntityTransferHelper.PlayerEntityQueue>) (c => c.Client == client));
        if (playerEntityQueue != null)
          return playerEntityQueue.IsEntityQueued(entity);
      }
      return false;
    }

    private void MarkPlayerReadyForNextTransaction(NetworkPlayer player)
    {
      if (player == null)
        return;
      EntityTransferHelper.PlayerEntityQueue playerEntityQueue = this.playersAndEntitiesToTransfer.Find((Predicate<EntityTransferHelper.PlayerEntityQueue>) (c => c.Client == player));
      if (playerEntityQueue == null)
        return;
      playerEntityQueue.EntityBeingTransferred = (SpaceEntity) null;
    }

    private class PlayerEntityQueue
    {
      public NetworkPlayer Client;
      public List<SpaceEntity> EntityQueue = new List<SpaceEntity>();
      public SpaceEntity EntityBeingTransferred;

      public bool IsEntityQueued(SpaceEntity entity)
      {
        return this.EntityBeingTransferred == entity || this.EntityQueue.Contains(entity);
      }
    }
  }
}
