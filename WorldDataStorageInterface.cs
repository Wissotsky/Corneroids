// Decompiled with JetBrains decompiler
// Type: CornerSpace.WorldDataStorageInterface
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System.Data.SQLite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading;

#nullable disable
namespace CornerSpace
{
  public class WorldDataStorageInterface : IDisposable
  {
    private const int eIdi = 0;
    private const int eLoadedi = 1;
    private const int ePosxi = 2;
    private const int ePosyi = 3;
    private const int ePoszi = 4;
    private const int eOrixi = 5;
    private const int eOriyi = 6;
    private const int eOrizi = 7;
    private const int eOriwi = 8;
    private const int NOBINDING = -1;
    private const string queryLoadEntity = "SELECT * FROM entities WHERE id = @id;";
    private const string querySpaceSectorExist = "SELECT state FROM spacesectors WHERE sectorx = @x AND sectory = @y AND sectorz = @z;";
    private const string querySpaceSectorMarkCreated = "INSERT OR REPLACE INTO spacesectors VALUES (@x, @y, @z, @state);";
    private const string queryUpdateSpaceSector = "UPDATE spacesectors SET entityIds = @ids WHERE x = @x AND y = @y AND z = @z;";
    private const string queryWorldInfo = "SELECT * FROM worldinfo";
    private const string queryNextFreeId = "SELECT IFNULL(MAX(id),0) FROM entities;";
    private const string queryBeaconLoad = "SELECT * FROM beacons";
    private const string queryBeaconRemove = "DELETE FROM beacons WHERE posx = @x AND posy = @y AND posz = @z;";
    private const string queryBeaconStore = "INSERT INTO beacons VALUES (@x, @y, @z, @msg);";
    private const string queryBlockSectorLoadAll = "SELECT * FROM blocksectors WHERE ownerId = @id;";
    private const string queryBlockSectorRemove = "DELETE FROM blocksectors WHERE ownerId = @id";
    private const string queryBlockSectorStore = "INSERT OR REPLACE INTO blocksectors VALUES (@id, @x, @y, @z, @form, @ids, @ors);";
    private const string queryColorKeyMappingLoad = "SELECT * FROM keyColorMappings WHERE ownerEntityId = @ownerId;";
    private const string queryColorKeyMappingRemove = "DELETE FROM keyColorMappings WHERE ownerEntityId = @ownerId;";
    private const string queryColorKeyMappingStore = "INSERT INTO keyColorMappings VALUES (@ownerId, @blockId, @color, @key);";
    private const string queryControlBlocksLoad = "SELECT * FROM controlblocks WHERE ownerEntityId = @ownerId;";
    private const string queryControlBlocksRemove = "DELETE FROM controlblocks WHERE ownerEntityId = @ownerId;";
    private const string queryControlBlocksStore = "INSERT INTO controlblocks VALUES (@id, @bindId, @ownerId, @x, @y, @z);";
    private const string queryTriggerBlocksLoad = "SELECT * FROM controlledblocks WHERE ownerEntityId = @ownerId;";
    private const string queryTriggerBlocksRemove = "DELETE FROM controlledblocks WHERE ownerEntityId = @ownerId;";
    private const string queryTriggerBlocksStore = "INSERT INTO controlledblocks VALUES (@id, @bindId, @color, @ownerId, @x, @y, @z);";
    private const string queryEntityClearControls = "DELETE FROM controlblocks WHERE ownerEntityId = @id;";
    private const string queryEntityClearTriggers = "DELETE FROM controlledblocks WHERE ownerEntityId = @id;";
    private const string queryEntityLoadFromSectors = "SELECT * FROM entities WHERE sectorx >= @lx AND sectorx <= @ux AND sectory >= @ly AND sectory <= @uy AND sectorz >= @lz AND sectorz <= @uz AND loaded = 0;";
    private const string queryEntityMarkLoaded = "UPDATE entities SET loaded = 1 WHERE id = @id;";
    private const string queryEntityRemove = "DELETE FROM entities WHERE id = @id;";
    private const string queryEntityStore = "INSERT OR REPLACE INTO entities VALUES (@id, @loaded, @x, @y, @z, @ox, @oy, @oz, @ow, @sx, @sy, @sz);";
    private const string queryEntityUpdate = "UPDATE entities SET loaded = 0, posx = @x, posy = @y, posz = @z, orx = @ox, ory = @oy, orz = @oz, orw = @ow, sectorx = @sx, sectory = @sy, sectorz = @sz WHERE id = @id;";
    private const string queryInventoryLoad = "SELECT * FROM inventoryItems WHERE id = @id AND type = @type;";
    private const string queryInventoryRemove = "DELETE FROM inventoryItems WHERE id = @id AND type = @type;";
    private const string queryInventoryStore = "INSERT OR REPLACE INTO inventoryItems VALUES (@id, @type, @x, @y, @z, @items);";
    private const string queryPlayerLoad = "SELECT * FROM players WHERE id = @id;";
    private const string queryPlayerStore = "INSERT OR REPLACE INTO players VALUES (@id, @name, @x, @y, @z, @ox, @oy, @oz, @ow, @entityId, @boundEntities, @token);";
    private const string queryPlayerWearedItemsLoad = "SELECT * FROM wearedItems WHERE id = @id;";
    private const string queryPlayerWearedItemsStore = "INSERT OR REPLACE INTO wearedItems VALUES (@id, @head, @shoulders, @chest, @legs, @hands, @feet, @power);";
    private object blockArrayLock = new object();
    private object databaseLock = new object();
    private object workerLock = new object();
    private object workQueueLock = new object();
    private List<WorldDataStorageInterface.DatabaseAction> databaseActions = new List<WorldDataStorageInterface.DatabaseAction>();
    private Action<object[]> actionLoadSpaceEntities;
    private Action<object[]> actionMarkSectorCreated;
    private Action<object[]> actionRemoveBeacon;
    private Action<object[]> actionRemoveEntity;
    private Action<object[]> actionSpaceSectorExist;
    private Action<object[]> actionStoreBeacon;
    private Action<object[]> actionStoreEntity;
    private Action<object[]> actionStorePlayer;
    private System.Action workerAction;
    private bool workerRunningA;
    private BlockSectorCompressor compressor;
    private IdGen idGen;
    private readonly World tiedToWorld;
    private readonly SQLiteConnection dbConnection;
    private string originalFilePath;
    private Random random;
    private string tempFilePath;

    public WorldDataStorageInterface(
      World world,
      SQLiteConnection dbConnection,
      string originalFilePath,
      string tempFilePath)
    {
      this.tiedToWorld = world != null && dbConnection != null ? world : throw new ArgumentNullException();
      this.dbConnection = dbConnection;
      this.idGen = new IdGen(0);
      this.random = new Random();
      this.originalFilePath = originalFilePath;
      this.tempFilePath = tempFilePath;
      this.compressor = new BlockSectorCompressor();
      this.CreateDelegates();
    }

    public void Dispose()
    {
      this.dbConnection.Close();
      this.dbConnection.Dispose();
    }

    public int GetNextFreeId()
    {
      try
      {
        using (DataTable dataTable = this.ExecuteQuery("SELECT IFNULL(MAX(id),0) FROM entities;", (SQLiteParameter[]) null))
        {
          if (dataTable != null)
          {
            if (dataTable.Rows.Count == 0)
              return 0;
            object obj = dataTable.Rows[0][0];
            return obj == null ? 0 : Convert.ToInt32(obj) + 1;
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to determine the next free entity ID: " + ex.Message);
      }
      return 0;
    }

    public string GetToken()
    {
      try
      {
        using (DataTable dataTable = this.ExecuteQuery("SELECT token FROM worldinfo", (SQLiteParameter[]) null))
        {
          if (dataTable != null)
          {
            if (dataTable.Rows.Count == 0)
              return string.Empty;
            object obj = dataTable.Rows[0][0];
            return obj == null ? string.Empty : Convert.ToString(obj);
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load world token from database: " + ex.Message);
      }
      return string.Empty;
    }

    public void LoadEntities(
      Vector3int sectorLowerBound,
      Vector3int sectorUpperBound,
      Position3 playerPosition,
      Action<SpaceEntity> resultEntity)
    {
      if (resultEntity == null)
        return;
      this.CreateAction(this.actionLoadSpaceEntities, WorldDataStorageInterface.ActionPriorities.LoadEntities, (object) new WorldDataStorageInterface.LoadSpaceEntityContainer(sectorLowerBound, sectorUpperBound, playerPosition, resultEntity), false);
    }

    public List<BeaconObject> LoadBeacons()
    {
      try
      {
        using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM beacons", (SQLiteParameter[]) null))
        {
          if (dataTable != null)
          {
            List<BeaconObject> beaconObjectList1 = new List<BeaconObject>();
            foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            {
              long int64_1 = Convert.ToInt64(row[0]);
              long int64_2 = Convert.ToInt64(row[1]);
              long int64_3 = Convert.ToInt64(row[2]);
              string str = Convert.ToString(row[3]);
              List<BeaconObject> beaconObjectList2 = beaconObjectList1;
              BeaconObject beaconObject1 = new BeaconObject();
              beaconObject1.Position = new Position3(int64_1, int64_2, int64_3);
              beaconObject1.Message = str;
              BeaconObject beaconObject2 = beaconObject1;
              beaconObjectList2.Add(beaconObject2);
            }
            return beaconObjectList1;
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load beacons: " + ex.Message);
      }
      return (List<BeaconObject>) null;
    }

    public List<LoadedPlayerContainer<NetworkPlayerOnServer>> LoadClientPlayers()
    {
      DataTable dataTable = this.ExecuteQuery("SELECT id FROM players", (SQLiteParameter[]) null);
      List<LoadedPlayerContainer<NetworkPlayerOnServer>> loadedPlayerContainerList = new List<LoadedPlayerContainer<NetworkPlayerOnServer>>();
      using (dataTable)
      {
        if (dataTable != null)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            try
            {
              int int32 = Convert.ToInt32(row[0]);
              if (int32 != 0)
              {
                LoadedPlayerContainer<NetworkPlayerOnServer> loadedPlayerContainer = this.LoadPlayer<NetworkPlayerOnServer>(int32);
                loadedPlayerContainerList.Add(loadedPlayerContainer);
              }
            }
            catch (Exception ex)
            {
              Engine.Console.WriteErrorLine("Failed to load client player: " + ex.Message);
            }
          }
        }
      }
      return loadedPlayerContainerList;
    }

    public LoadedPlayerContainer<T> LoadPlayer<T>(int id) where T : Player, new()
    {
      LoadedPlayerContainer<T> container = new LoadedPlayerContainer<T>();
      this.LoadPlayer<T>(container, id);
      if ((object) container.Player != null)
      {
        T player = container.Player;
        player.Astronaut.Suit.Extract(this.LoadPlayerWearedItems(container.Player.Id));
        player.Toolbar.Items.Extract(this.LoadToolbarItems(container.Player.Id));
        player.Inventory.Extract(this.LoadPlayerInventoryItems<T>(container.Player.Id));
      }
      return container;
    }

    public void MarkSector(Vector3int sectorPosition, World.SectorState state)
    {
      this.CreateAction(this.actionMarkSectorCreated, WorldDataStorageInterface.ActionPriorities.MarkSector, (object) new WorldDataStorageInterface.SpaceSectorContainer(sectorPosition, state), true);
    }

    public bool PollWorkToBeDone()
    {
      lock (this.workerLock)
        return this.workerRunningA;
    }

    public void SectorDataCreated(Vector3int sectorPosition, Action<World.SectorState> result)
    {
      if (result == null)
        return;
      this.CreateAction(this.actionSpaceSectorExist, WorldDataStorageInterface.ActionPriorities.SectorDataExists, (object) new WorldDataStorageInterface.SpaceSectorContainer(sectorPosition, result), false);
    }

    public void RemoveBeacon(BeaconObject beacon)
    {
      if (beacon == null)
        return;
      this.CreateAction(this.actionRemoveBeacon, WorldDataStorageInterface.ActionPriorities.RemoveBeacon, (object) new WorldDataStorageInterface.StoreBeaconContainer(beacon.Position, beacon.Message), true);
    }

    public void RemoveEntity(int entityId)
    {
      this.CreateAction(this.actionRemoveEntity, WorldDataStorageInterface.ActionPriorities.RemoveEntity, (object) entityId, true);
    }

    public void Save()
    {
      this.PollWorkToBeDone();
      WorldIOManager.Instance.RequestSaveWorldFile(this);
    }

    public void StoreBeacon(BeaconObject beacon)
    {
      if (beacon == null)
        return;
      this.CreateAction(this.actionStoreBeacon, WorldDataStorageInterface.ActionPriorities.StoreBeacon, (object) new WorldDataStorageInterface.StoreBeaconContainer(beacon.Position, beacon.Message), true);
    }

    public void StoreEntity(SpaceEntity entity, Vector3int sector)
    {
      if (entity == null)
        return;
      this.CreateAction(this.actionStoreEntity, WorldDataStorageInterface.ActionPriorities.StoreEntity, (object) new WorldDataStorageInterface.StoreEntityContainer(entity, sector), true);
    }

    public void StorePlayer(Player player, SpaceEntity[] closeEntities)
    {
      if (player == null)
        return;
      int[] numArray1 = closeEntities != null ? ((IEnumerable<SpaceEntity>) closeEntities).Select<SpaceEntity, int>((System.Func<SpaceEntity, int>) (entity => entity.Id)).ToArray<int>() : new int[0];
      byte[] numArray2 = player.Astronaut.Suit.Serialize();
      Quaternion quaternion = player.Astronaut.Orientation;
      if (player.Astronaut.BoardedEntity != null)
        quaternion = Quaternion.Concatenate(quaternion, player.Astronaut.CoordinateSpace);
      this.CreateAction(this.actionStorePlayer, WorldDataStorageInterface.ActionPriorities.StorePlayer, (object) new WorldDataStorageInterface.StorePlayerContainer()
      {
        Id = player.Id,
        Name = (string.IsNullOrEmpty(player.Name) ? "Noname" : player.Name),
        Position = player.Astronaut.Position,
        Orientation = quaternion,
        BoardedEntityId = (player.Astronaut.BoardedEntity != null ? player.Astronaut.BoardedEntity.Id : -1),
        CloseEntitiesIds = numArray1,
        WearedItemsIds = numArray2,
        ToolbarItemIds = player.Toolbar.Items.Serialize(),
        InventoryItems = player.Inventory.Serialize(),
        Token = player.Token
      }, false);
    }

    public string TemporaryFileLocation => this.tempFilePath;

    public string OriginalFileLocation => this.originalFilePath;

    private void CreateAction(
      Action<object[]> action,
      WorldDataStorageInterface.ActionPriorities priority,
      object data,
      bool groupable)
    {
      if (action == null)
        return;
      lock (this.workQueueLock)
      {
        WorldDataStorageInterface.DatabaseAction newAction = new WorldDataStorageInterface.DatabaseAction()
        {
          Action = action,
          CanBeGrouped = groupable,
          Data = data,
          Priority = (int) priority
        };
        int lastIndex1 = this.databaseActions.FindLastIndex((Predicate<WorldDataStorageInterface.DatabaseAction>) (a => a.Priority == newAction.Priority));
        if (lastIndex1 != -1)
        {
          this.databaseActions.Insert(lastIndex1 + 1, newAction);
        }
        else
        {
          int lastIndex2 = this.databaseActions.FindLastIndex((Predicate<WorldDataStorageInterface.DatabaseAction>) (a => a.Priority > newAction.Priority));
          if (lastIndex2 == -1)
            this.databaseActions.Insert(0, newAction);
          else
            this.databaseActions.Insert(lastIndex2 + 1, newAction);
        }
        this.StartWorkerIfStopped();
      }
    }

    private void CreateDelegates()
    {
      this.workerAction = new System.Action(this.WorkerFunction);
      this.actionMarkSectorCreated = new Action<object[]>(this.databaseMarkSectorsCreated);
      this.actionRemoveBeacon = new Action<object[]>(this.databaseRemoveBeacons);
      this.actionRemoveEntity = new Action<object[]>(this.databaseRemoveEntities);
      this.actionSpaceSectorExist = new Action<object[]>(this.databaseSpaceSectorExist);
      this.actionStoreBeacon = new Action<object[]>(this.databaseStoreBeacon);
      this.actionStoreEntity = new Action<object[]>(this.databaseStoreEntities);
      this.actionLoadSpaceEntities = new Action<object[]>(this.databaseLoadSpaceEntities);
      this.actionStorePlayer = new Action<object[]>(this.databaseStorePlayer);
    }

    private bool ExecuteNonQuery(string query, SQLiteParameter[] parameters)
    {
      lock (this.databaseLock)
      {
        SQLiteTransaction SQLiteTransaction = (SQLiteTransaction) null;
        try
        {
          this.dbConnection.Open();
          SQLiteTransaction = this.dbConnection.BeginTransaction();
          using (SQLiteCommand command = this.dbConnection.CreateCommand())
          {
            command.CommandText = query;
            if (parameters != null)
            {
              for (int index = 0; index < parameters.Length; ++index)
                command.Parameters.Add(parameters[index]);
            }
            command.ExecuteNonQuery();
          }
          SQLiteTransaction.Commit();
          return true;
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("NonQuery failed. Attempting a rollback: " + ex.Message);
          try
          {
            SQLiteTransaction.Rollback();
          }
          catch
          {
            Engine.Console.WriteErrorLine("Rollback failed: " + ex.Message);
          }
          return false;
        }
        finally
        {
          SQLiteTransaction?.Dispose();
          this.dbConnection.Close();
        }
      }
    }

    private DataTable ExecuteQuery(string query, SQLiteParameter[] parameters)
    {
      lock (this.databaseLock)
      {
        try
        {
          using (SQLiteDataAdapter SQLiteDataAdapter = new SQLiteDataAdapter())
          {
            using (SQLiteCommand SQLiteCommand = new SQLiteCommand(this.dbConnection))
            {
              DataTable dataTable = new DataTable();
              SQLiteCommand.CommandText = query;
              if (parameters != null)
              {
                for (int index = 0; index < parameters.Length; ++index)
                  SQLiteCommand.Parameters.Add(parameters[index]);
              }
              SQLiteDataAdapter.SelectCommand = SQLiteCommand;
              SQLiteDataAdapter.Fill(dataTable);
              return dataTable;
            }
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to execute database query: " + ex.Message);
          return (DataTable) null;
        }
      }
    }

    private WorldDataStorageInterface.DatabaseAction GetJobWithHighestPriority()
    {
      lock (this.workQueueLock)
      {
        if (this.databaseActions.Count <= 0)
          return (WorldDataStorageInterface.DatabaseAction) null;
        WorldDataStorageInterface.DatabaseAction databaseAction = this.databaseActions[0];
        this.databaseActions.RemoveAt(0);
        return databaseAction;
      }
    }

    private Color PackedIntToColor(int packedValue)
    {
      uint num = (uint) packedValue;
      return new Color((byte) (num >> 16 & (uint) byte.MaxValue), (byte) (num >> 8 & (uint) byte.MaxValue), (byte) (num & (uint) byte.MaxValue), (byte) (num >> 24 & (uint) byte.MaxValue));
    }

    private void StartWorkerIfStopped()
    {
      lock (this.workerLock)
      {
        if (this.workerRunningA)
          return;
        ThreadPool.QueueUserWorkItem((WaitCallback) delegate
        {
          this.workerAction();
        });
        this.workerRunningA = true;
      }
    }

    private void StoreContainerBlocks(
      WorldDataStorageInterface.StoreEntityContainer[] containersOfEntities)
    {
      if (containersOfEntities == null)
        return;
      SQLiteParameter[] SQLiteParameterArray1 = new SQLiteParameter[2]
      {
        new SQLiteParameter("id", DbType.Int32),
        null
      };
      SQLiteParameter[] SQLiteParameterArray2 = SQLiteParameterArray1;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter("type", DbType.Int32);
      SQLiteParameter1.Value = (object) 1;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray2[1] = SQLiteParameter2;
      SQLiteParameter[] parameters = SQLiteParameterArray1;
      foreach (WorldDataStorageInterface.StoreEntityContainer containersOfEntity in containersOfEntities)
      {
        SpaceEntity entity = containersOfEntity.Entity;
        parameters[0].Value = (object) entity.Id;
        this.ExecuteNonQuery("DELETE FROM inventoryItems WHERE id = @id AND type = @type;", parameters);
        foreach (BlockSector sector in entity.BlockSectorManager.SectorList)
        {
          foreach (ContainerBlock containerBlock in sector.ContainerBlocks)
          {
            Inventory inventory = containerBlock.Inventory;
            if (inventory.ItemCount > 0U)
              this.StoreInventoryItems(entity.Id, DatabaseStructure.InventoryId.Chest, inventory.Serialize(), containerBlock.PositionInEntitySpace);
          }
        }
      }
    }

    private void StoreTriggerAndControlBlocks(
      WorldDataStorageInterface.StoreEntityContainer[] entitiesToStore)
    {
      if (entitiesToStore == null)
        return;
      lock (this.databaseLock)
      {
        try
        {
          this.dbConnection.Open();
          using (SQLiteTransaction SQLiteTransaction = this.dbConnection.BeginTransaction())
          {
            using (SQLiteCommand command = this.dbConnection.CreateCommand())
            {
              SQLiteParameter parameter = new SQLiteParameter("ownerId", DbType.Int32);
              for (int index1 = 0; index1 < entitiesToStore.Length; ++index1)
              {
                SpaceEntity entity = entitiesToStore[index1].Entity;
                Vector3int sectorPosition = entitiesToStore[index1].SectorPosition;
                parameter.Value = (object) entity.Id;
                command.Parameters.Clear();
                command.Parameters.Add(parameter);
                command.CommandText = "DELETE FROM controlblocks WHERE ownerEntityId = @ownerId;";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM controlledblocks WHERE ownerEntityId = @ownerId;";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM keyColorMappings WHERE ownerEntityId = @ownerId;";
                command.ExecuteNonQuery();
                int num1 = 0;
                Dictionary<ControlBlock, int> dictionary = new Dictionary<ControlBlock, int>();
                SQLiteParameter[] values1 = new SQLiteParameter[7]
                {
                  new SQLiteParameter("id", DbType.Int32),
                  new SQLiteParameter("bindId", DbType.Int32),
                  new SQLiteParameter("ownerId", DbType.Int32),
                  new SQLiteParameter("x", DbType.Single),
                  new SQLiteParameter("y", DbType.Single),
                  new SQLiteParameter("z", DbType.Single),
                  new SQLiteParameter("color", DbType.Int32)
                };
                SQLiteParameter[] values2 = new SQLiteParameter[4]
                {
                  new SQLiteParameter("ownerId", DbType.Int32),
                  new SQLiteParameter("blockId", DbType.Int32),
                  new SQLiteParameter("color", DbType.Int32),
                  new SQLiteParameter("key", DbType.Int32)
                };
                int num2 = 0;
                foreach (BlockSector sector in entity.BlockSectorManager.SectorList)
                {
                  command.Parameters.Clear();
                  command.Parameters.AddRange(values1);
                  for (int index2 = 0; index2 < sector.ControlBlocks.Count; ++index2)
                  {
                    ControlBlock controlBlock = sector.ControlBlocks[index2];
                    dictionary.Add(controlBlock, num1);
                    command.CommandText = "INSERT INTO controlblocks VALUES (@id, @bindId, @ownerId, @x, @y, @z);";
                    values1[0].Value = (object) controlBlock.Id;
                    values1[1].Value = (object) num1;
                    values1[2].Value = (object) sector.Owner.Id;
                    values1[3].Value = (object) controlBlock.PositionInEntitySpace.X;
                    values1[4].Value = (object) controlBlock.PositionInEntitySpace.Y;
                    values1[5].Value = (object) controlBlock.PositionInEntitySpace.Z;
                    command.ExecuteNonQuery();
                    ++num1;
                  }
                  command.Parameters.Clear();
                  command.Parameters.AddRange(values2);
                  for (int index3 = 0; index3 < sector.ControlBlocks.Count; ++index3)
                  {
                    ControlBlock controlBlock = sector.ControlBlocks[index3];
                    command.CommandText = "INSERT INTO keyColorMappings VALUES (@ownerId, @blockId, @color, @key);";
                    values2[0].Value = (object) entity.Id;
                    values2[1].Value = (object) dictionary[controlBlock];
                    for (int index4 = 0; index4 < controlBlock.ColorKeyGroups.Count; ++index4)
                    {
                      if (controlBlock.ColorKeyGroups[index4].Key != Keys.None)
                      {
                        values2[2].Value = (object) (int) controlBlock.ColorKeyGroups[index4].Color.PackedValue;
                        values2[3].Value = (object) (int) controlBlock.ColorKeyGroups[index4].Key;
                        command.ExecuteNonQuery();
                      }
                    }
                  }
                }
                command.Parameters.Clear();
                command.Parameters.AddRange(values1);
                command.CommandText = "INSERT INTO controlledblocks VALUES (@id, @bindId, @color, @ownerId, @x, @y, @z);";
                foreach (ControlBlock key in dictionary.Keys)
                {
                  foreach (HashSet<TriggerBlock> triggerBlockSet in key.ControlledBlocks.Values)
                  {
                    foreach (TriggerBlock block in triggerBlockSet)
                    {
                      int num3 = -1;
                      if (block.Controller != null && dictionary.ContainsKey(block.Controller))
                      {
                        num3 = dictionary[block.Controller];
                        Color? blockColor = block.Controller.GetBlockColor(block);
                        if (blockColor.HasValue)
                          num2 = (int) blockColor.Value.PackedValue;
                      }
                      values1[0].Value = (object) block.Id;
                      values1[1].Value = (object) num3;
                      values1[2].Value = (object) block.Owner.Owner.Id;
                      values1[3].Value = (object) block.PositionInShipSpace.X;
                      values1[4].Value = (object) block.PositionInShipSpace.Y;
                      values1[5].Value = (object) block.PositionInShipSpace.Z;
                      values1[6].Value = (object) num2;
                      command.ExecuteNonQuery();
                    }
                  }
                }
              }
            }
            SQLiteTransaction.Commit();
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to store color/key mappings: " + ex.Message);
        }
        finally
        {
          this.dbConnection.Close();
        }
      }
    }

    private void WorkerFunction()
    {
      WorldDataStorageInterface.DatabaseAction job;
      while ((job = this.GetJobWithHighestPriority()) != null)
      {
        List<object> objectList = new List<object>();
        objectList.Add(job.Data);
        if (job.CanBeGrouped)
        {
          lock (this.workQueueLock)
          {
            foreach (WorldDataStorageInterface.DatabaseAction databaseAction in this.databaseActions.FindAll((Predicate<WorldDataStorageInterface.DatabaseAction>) (a => a.Action == job.Action)))
            {
              this.databaseActions.Remove(databaseAction);
              objectList.Add(databaseAction.Data);
            }
          }
        }
        object[] array = objectList.ToArray();
        job.Action(array);
      }
      lock (this.workerLock)
        this.workerRunningA = false;
    }

    private T CopyFirstWorkItem<T>(List<T> list)
    {
      T obj = default (T);
      lock (this.workQueueLock)
      {
        if (list.Count == 0)
          return default (T);
        obj = list[0];
        list.RemoveAt(0);
      }
      return obj;
    }

    private T[] CopyWorkList<T>(List<T> listToCopy)
    {
      T[] objArray = (T[]) null;
      lock (this.workQueueLock)
      {
        if (listToCopy.Count == 0)
          return (T[]) null;
        objArray = listToCopy.ToArray();
        listToCopy.Clear();
      }
      return objArray;
    }

    private void databaseMarkSectorsCreated(object[] inputParams)
    {
      WorldDataStorageInterface.SpaceSectorContainer[] array = inputParams.Cast<WorldDataStorageInterface.SpaceSectorContainer>().ToArray<WorldDataStorageInterface.SpaceSectorContainer>();
      if (array == null)
        return;
      lock (this.databaseLock)
      {
        SQLiteTransaction SQLiteTransaction = (SQLiteTransaction) null;
        try
        {
          this.dbConnection.Open();
          SQLiteTransaction = this.dbConnection.BeginTransaction();
          using (SQLiteCommand command = this.dbConnection.CreateCommand())
          {
            command.CommandText = "INSERT OR REPLACE INTO spacesectors VALUES (@x, @y, @z, @state);";
            SQLiteParameter[] values = new SQLiteParameter[4]
            {
              new SQLiteParameter("x", DbType.Int32),
              new SQLiteParameter("y", DbType.Int32),
              new SQLiteParameter("z", DbType.Int32),
              new SQLiteParameter("state", DbType.Int32)
            };
            command.Parameters.AddRange(values);
            for (int index = 0; index < array.Length; ++index)
            {
              WorldDataStorageInterface.SpaceSectorContainer spaceSectorContainer = array[index];
              Vector3int sectorPosition = spaceSectorContainer.SectorPosition;
              World.SectorState markedState = spaceSectorContainer.markedState;
              values[0].Value = (object) sectorPosition.X;
              values[1].Value = (object) sectorPosition.Y;
              values[2].Value = (object) sectorPosition.Z;
              values[3].Value = (object) (int) markedState;
              command.ExecuteNonQuery();
            }
          }
          SQLiteTransaction.Commit();
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("NonQuery failed. Attempting a rollback: " + ex.Message);
          try
          {
            SQLiteTransaction.Rollback();
          }
          catch
          {
            Engine.Console.WriteErrorLine("Rollback failed: " + ex.Message);
          }
        }
        finally
        {
          SQLiteTransaction?.Dispose();
          this.dbConnection.Close();
        }
      }
    }

    private void databaseLoadSpaceEntities(object[] inputParams)
    {
      WorldDataStorageInterface.LoadSpaceEntityContainer inputParam = (WorldDataStorageInterface.LoadSpaceEntityContainer) inputParams[0];
      Position3 playerPosition = inputParam.PlayerPosition;
      Vector3int lowerSpaceBounds = inputParam.LowerSpaceBounds;
      Vector3int upperSpaceBounds = inputParam.UpperSpaceBounds;
      Action<SpaceEntity> resultFunction = inputParam.ResultFunction;
      SQLiteParameter[] parameters1 = new SQLiteParameter[6];
      SQLiteParameter[] SQLiteParameterArray1 = parameters1;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter("lx", DbType.Int32);
      SQLiteParameter1.Value = (object) lowerSpaceBounds.X;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray1[0] = SQLiteParameter2;
      SQLiteParameter[] SQLiteParameterArray2 = parameters1;
      SQLiteParameter SQLiteParameter3 = new SQLiteParameter("ux", DbType.Int32);
      SQLiteParameter3.Value = (object) upperSpaceBounds.X;
      SQLiteParameter SQLiteParameter4 = SQLiteParameter3;
      SQLiteParameterArray2[1] = SQLiteParameter4;
      SQLiteParameter[] SQLiteParameterArray3 = parameters1;
      SQLiteParameter SQLiteParameter5 = new SQLiteParameter("ly", DbType.Int32);
      SQLiteParameter5.Value = (object) lowerSpaceBounds.Y;
      SQLiteParameter SQLiteParameter6 = SQLiteParameter5;
      SQLiteParameterArray3[2] = SQLiteParameter6;
      SQLiteParameter[] SQLiteParameterArray4 = parameters1;
      SQLiteParameter SQLiteParameter7 = new SQLiteParameter("uy", DbType.Int32);
      SQLiteParameter7.Value = (object) upperSpaceBounds.Y;
      SQLiteParameter SQLiteParameter8 = SQLiteParameter7;
      SQLiteParameterArray4[3] = SQLiteParameter8;
      SQLiteParameter[] SQLiteParameterArray5 = parameters1;
      SQLiteParameter SQLiteParameter9 = new SQLiteParameter("lz", DbType.Int32);
      SQLiteParameter9.Value = (object) lowerSpaceBounds.Z;
      SQLiteParameter SQLiteParameter10 = SQLiteParameter9;
      SQLiteParameterArray5[4] = SQLiteParameter10;
      SQLiteParameter[] SQLiteParameterArray6 = parameters1;
      SQLiteParameter SQLiteParameter11 = new SQLiteParameter("uz", DbType.Int32);
      SQLiteParameter11.Value = (object) upperSpaceBounds.Z;
      SQLiteParameter SQLiteParameter12 = SQLiteParameter11;
      SQLiteParameterArray6[5] = SQLiteParameter12;
      SQLiteParameter[] parameters2 = new SQLiteParameter[1]
      {
        new SQLiteParameter("id", DbType.Int32)
      };
      using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM entities WHERE sectorx >= @lx AND sectorx <= @ux AND sectory >= @ly AND sectory <= @uy AND sectorz >= @lz AND sectorz <= @uz AND loaded = 0;", parameters1))
      {
        if (dataTable == null)
          return;
        List<DataRow> list = dataTable.Rows.Cast<DataRow>().ToList<DataRow>();
        list.Sort((Comparison<DataRow>) ((e1, e2) =>
        {
          Position3 position3_1 = new Position3(Convert.ToInt64(e1[2]), Convert.ToInt64(e1[3]), Convert.ToInt64(e1[4]));
          Position3 position3_2 = new Position3(Convert.ToInt64(e2[2]), Convert.ToInt64(e2[3]), Convert.ToInt64(e2[4]));
          return (int) ((double) (position3_1 - playerPosition).LengthSquared() - (double) (position3_2 - playerPosition).LengthSquared());
        }));
        for (int index = 0; index < list.Count; ++index)
        {
          SpaceEntity entity = (SpaceEntity) null;
          try
          {
            int num = (int) list[index][0];
            parameters2[0].Value = (object) num;
            if (this.ExecuteNonQuery("UPDATE entities SET loaded = 1 WHERE id = @id;", parameters2))
            {
              entity = new SpaceEntity();
              entity.Id = num;
              this.LoadBlockSectors(entity);
              this.FetchControlAndTriggerBlocksToEntity(entity);
              this.FetchContainersToEntity(entity);
              entity.Orientation = new Quaternion((float) (double) list[index][5], (float) (double) list[index][6], (float) (double) list[index][7], (float) (double) list[index][8]);
              entity.Position = new Position3(Convert.ToInt64(list[index][2]), Convert.ToInt64(list[index][3]), Convert.ToInt64(list[index][4]));
              resultFunction(entity);
            }
          }
          catch (Exception ex)
          {
            entity?.Dispose();
            Engine.Console.WriteErrorLine("Failed to load an entity from the data source: " + ex.Message);
          }
        }
      }
    }

    private void databaseRemoveBeacons(object[] inputParams)
    {
      WorldDataStorageInterface.StoreBeaconContainer[] array = inputParams.Cast<WorldDataStorageInterface.StoreBeaconContainer>().ToArray<WorldDataStorageInterface.StoreBeaconContainer>();
      if (array == null)
        return;
      SQLiteParameter[] parameters = new SQLiteParameter[3]
      {
        new SQLiteParameter("x", DbType.Int64),
        new SQLiteParameter("y", DbType.Int64),
        new SQLiteParameter("z", DbType.Int64)
      };
      foreach (WorldDataStorageInterface.StoreBeaconContainer storeBeaconContainer in array)
      {
        parameters[0].Value = (object) storeBeaconContainer.Position.X;
        parameters[1].Value = (object) storeBeaconContainer.Position.Y;
        parameters[2].Value = (object) storeBeaconContainer.Position.Z;
        this.ExecuteNonQuery("DELETE FROM beacons WHERE posx = @x AND posy = @y AND posz = @z;", parameters);
      }
    }

    private void databaseRemoveEntities(object[] inputParams)
    {
      int[] array = inputParams.Cast<int>().ToArray<int>();
      if (array == null)
        return;
      SQLiteParameter[] parameters = new SQLiteParameter[2]
      {
        new SQLiteParameter("id", DbType.Int32),
        new SQLiteParameter("ownerId", DbType.Int32)
      };
      foreach (int num in array)
      {
        parameters[0].Value = (object) num;
        parameters[1].Value = (object) num;
        this.ExecuteNonQuery("DELETE FROM entities WHERE id = @id;", parameters);
        this.ExecuteNonQuery("DELETE FROM blocksectors WHERE ownerId = @id", parameters);
        this.ExecuteNonQuery("DELETE FROM controlblocks WHERE ownerEntityId = @ownerId;", parameters);
        this.ExecuteNonQuery("DELETE FROM keyColorMappings WHERE ownerEntityId = @ownerId;", parameters);
        this.ExecuteNonQuery("DELETE FROM controlledblocks WHERE ownerEntityId = @ownerId;", parameters);
      }
    }

    private void databaseSpaceSectorExist(object[] inputParams)
    {
      WorldDataStorageInterface.SpaceSectorContainer inputParam = (WorldDataStorageInterface.SpaceSectorContainer) inputParams[0];
      Vector3int sectorPosition = inputParam.SectorPosition;
      Action<World.SectorState> resultFunction = inputParam.ResultFunction;
      using (DataTable dataTable = this.ExecuteQuery("SELECT state FROM spacesectors WHERE sectorx = @x AND sectory = @y AND sectorz = @z;", new SQLiteParameter[3]
      {
        new SQLiteParameter("x", (object) sectorPosition.X),
        new SQLiteParameter("y", (object) sectorPosition.Y),
        new SQLiteParameter("z", (object) sectorPosition.Z)
      }))
      {
        if (dataTable == null)
          return;
        if (dataTable.Rows.Count == 0)
        {
          resultFunction(World.SectorState.not_processed);
        }
        else
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            World.SectorState sectorState = (World.SectorState) row[0];
            resultFunction(sectorState);
          }
        }
      }
    }

    private void databaseStoreBeacon(object[] inputParams)
    {
      WorldDataStorageInterface.StoreBeaconContainer[] array = inputParams.Cast<WorldDataStorageInterface.StoreBeaconContainer>().ToArray<WorldDataStorageInterface.StoreBeaconContainer>();
      if (array == null)
        return;
      lock (this.databaseLock)
      {
        SQLiteTransaction SQLiteTransaction = (SQLiteTransaction) null;
        try
        {
          this.dbConnection.Open();
          SQLiteTransaction = this.dbConnection.BeginTransaction();
          using (SQLiteCommand command = this.dbConnection.CreateCommand())
          {
            command.CommandText = "INSERT INTO beacons VALUES (@x, @y, @z, @msg);";
            SQLiteParameter[] values = new SQLiteParameter[4]
            {
              new SQLiteParameter("msg", DbType.String),
              new SQLiteParameter("x", DbType.Int64),
              new SQLiteParameter("y", DbType.Int64),
              new SQLiteParameter("z", DbType.Int64)
            };
            command.Parameters.AddRange(values);
            for (int index = 0; index < array.Length; ++index)
            {
              long x = array[index].Position.X;
              long y = array[index].Position.Y;
              long z = array[index].Position.Z;
              string message = array[index].Message;
              values[0].Value = (object) message;
              values[1].Value = (object) x;
              values[2].Value = (object) y;
              values[3].Value = (object) z;
              command.ExecuteNonQuery();
            }
            SQLiteTransaction.Commit();
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to store beacons: " + ex.Message);
        }
        finally
        {
          SQLiteTransaction?.Dispose();
          this.dbConnection.Close();
        }
      }
    }

    private void databaseStoreEntities(object[] inputParams)
    {
      WorldDataStorageInterface.StoreEntityContainer[] array = inputParams.Cast<WorldDataStorageInterface.StoreEntityContainer>().ToArray<WorldDataStorageInterface.StoreEntityContainer>();
      if (array == null)
        return;
      this.StoreEntities(array);
      this.StoreBlockSectors(array);
      this.StoreTriggerAndControlBlocks(array);
      this.StoreContainerBlocks(array);
    }

    private void databaseStorePlayer(object[] inputParams)
    {
      WorldDataStorageInterface.StorePlayerContainer inputParam = (WorldDataStorageInterface.StorePlayerContainer) inputParams[0];
      this.StorePlayer(inputParam);
      this.StoreInventoryItems(inputParam.Id, DatabaseStructure.InventoryId.Suit, inputParam.WearedItemsIds, Vector3.UnitX);
      this.StoreInventoryItems(inputParam.Id, DatabaseStructure.InventoryId.Inventory, inputParam.InventoryItems, Vector3.Zero);
      this.StoreInventoryItems(inputParam.Id, DatabaseStructure.InventoryId.Toolbar, inputParam.ToolbarItemIds, Vector3.One);
    }

    private void FetchContainersToEntity(SpaceEntity entity)
    {
      if (entity == null)
        return;
      KeyValuePair<byte[], Vector3> keyValuePair = this.LoadIventoryItems(entity.Id, DatabaseStructure.InventoryId.Chest);
      byte[] key = keyValuePair.Key;
      Vector3 positionInEntityCoordinates = keyValuePair.Value;
      BlockCell cellEntityCoords = entity.GetBlockCellEntityCoords(positionInEntityCoordinates);
      if (cellEntityCoords == null || !(cellEntityCoords.Block is ContainerBlock block) || Engine.LoadedWorld == null)
        return;
      block.Inventory.Extract(key);
    }

    private void FetchControlAndTriggerBlocksToEntity(SpaceEntity entity)
    {
      if (entity == null)
        return;
      int id = entity.Id;
      SQLiteParameter[] SQLiteParameterArray1 = new SQLiteParameter[1];
      SQLiteParameter[] SQLiteParameterArray2 = SQLiteParameterArray1;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter("ownerId", DbType.Int32);
      SQLiteParameter1.Value = (object) id;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray2[0] = SQLiteParameter2;
      SQLiteParameter[] parameters = SQLiteParameterArray1;
      try
      {
        Dictionary<int, ControlBlock> dictionary = new Dictionary<int, ControlBlock>();
        using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM controlblocks WHERE ownerEntityId = @ownerId;", parameters))
        {
          if (dataTable != null)
          {
            foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            {
              int num = (int) row[0];
              Vector3 positionInEntityCoordinates = new Vector3((float) (double) row[3], (float) (double) row[4], (float) (double) row[5]);
              int key = (int) row[1];
              BlockCell cellEntityCoords = entity.GetBlockCellEntityCoords(positionInEntityCoordinates);
              if (cellEntityCoords != null && cellEntityCoords.Block is ControlBlock)
                dictionary.Add(key, cellEntityCoords.Block as ControlBlock);
            }
          }
        }
        using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM keyColorMappings WHERE ownerEntityId = @ownerId;", parameters))
        {
          if (dataTable != null)
          {
            foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
            {
              int key1 = (int) row[1];
              int packedValue = (int) row[2];
              int num = (int) row[3];
              if (dictionary.ContainsKey(key1))
              {
                ControlBlock controlBlock = dictionary[key1];
                Color color = this.PackedIntToColor(packedValue);
                Keys key = (Keys) num;
                controlBlock.ColorKeyGroups.ForEach((Action<ColorKeyGroup>) (b =>
                {
                  if (!(b.Color == color))
                    return;
                  b.Key = key;
                }));
              }
            }
          }
        }
        using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM controlledblocks WHERE ownerEntityId = @ownerId;", parameters))
        {
          if (dataTable == null)
            return;
          foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          {
            int num = (int) row[0];
            Vector3 positionInEntityCoordinates = new Vector3((float) (double) row[4], (float) (double) row[5], (float) (double) row[6]);
            int key = (int) row[1];
            int packedValue = (int) row[2];
            BlockCell cellEntityCoords = entity.GetBlockCellEntityCoords(positionInEntityCoordinates);
            if (cellEntityCoords != null && cellEntityCoords.Block is TriggerBlock && key != -1 && dictionary.ContainsKey(key))
            {
              ControlBlock controlBlock = dictionary[key];
              if (controlBlock != null)
              {
                Color color = this.PackedIntToColor(packedValue);
                controlBlock.BindBlock(cellEntityCoords.Block as TriggerBlock, color, true);
                if (cellEntityCoords.Block is CameraBlock)
                  controlBlock.BindCamera(cellEntityCoords.Block as CameraBlock, false);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load control/trigger blocks: " + ex.Message);
      }
    }

    private void LoadBlockSectors(SpaceEntity entity)
    {
      if (entity == null)
        return;
      using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM blocksectors WHERE ownerId = @id;", new SQLiteParameter[1]
      {
        new SQLiteParameter("id", (object) entity.Id)
      }))
      {
        if (dataTable == null)
          return;
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        {
          try
          {
            Vector3int vector3int = new Vector3int((int) row[1], (int) row[2], (int) row[3]);
            byte[] numArray1 = (byte[]) row[4];
            byte[] numArray2 = (byte[]) row[5];
            byte[] numArray3 = (byte[]) row[6];
            PackedBlockSector sector = new PackedBlockSector()
            {
              SectorPosition = vector3int,
              CompressedFormations = numArray1,
              CompressedIds = numArray2,
              CompressedOrientations = numArray3
            };
            this.compressor.ExtractBlockSectorToEntity(ref sector, entity);
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Failed to load block sectors to an entity: " + ex.Message);
            throw ex;
          }
        }
        entity.ForceConstruct();
        entity.UpdatePhysicalProperties(false);
      }
    }

    private KeyValuePair<byte[], Vector3> LoadIventoryItems(
      int id,
      DatabaseStructure.InventoryId type)
    {
      SQLiteParameter[] parameters = new SQLiteParameter[2];
      SQLiteParameter[] SQLiteParameterArray1 = parameters;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter(nameof (id), DbType.Int32);
      SQLiteParameter1.Value = (object) id;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray1[0] = SQLiteParameter2;
      SQLiteParameter[] SQLiteParameterArray2 = parameters;
      SQLiteParameter SQLiteParameter3 = new SQLiteParameter(nameof (type), DbType.Int32);
      SQLiteParameter3.Value = (object) (int) type;
      SQLiteParameter SQLiteParameter4 = SQLiteParameter3;
      SQLiteParameterArray2[1] = SQLiteParameter4;
      using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM inventoryItems WHERE id = @id AND type = @type;", parameters))
      {
        if (dataTable != null)
        {
          try
          {
            if (dataTable.Rows.Count == 0)
              return new KeyValuePair<byte[], Vector3>();
            List<KeyValuePair<int, int>?> nullableList = new List<KeyValuePair<int, int>?>();
            IEnumerator enumerator = dataTable.Rows.GetEnumerator();
            try
            {
              if (enumerator.MoveNext())
              {
                DataRow current = (DataRow) enumerator.Current;
                nullableList.Clear();
                Vector3 vector3 = new Vector3((float) (double) current[2], (float) (double) current[3], (float) (double) current[4]);
                return new KeyValuePair<byte[], Vector3>((byte[]) current[5], vector3);
              }
            }
            finally
            {
              if (enumerator is IDisposable disposable)
                disposable.Dispose();
            }
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Failed to load inventory items: " + ex.Message);
          }
        }
      }
      return new KeyValuePair<byte[], Vector3>();
    }

    private void LoadPlayer<T>(LoadedPlayerContainer<T> container, int playerId) where T : Player, new()
    {
      if (container == null)
        return;
      SQLiteParameter[] parameters = new SQLiteParameter[1];
      SQLiteParameter[] SQLiteParameterArray = parameters;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter("id", DbType.Int32);
      SQLiteParameter1.Value = (object) playerId;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray[0] = SQLiteParameter2;
      using (DataTable dataTable = this.ExecuteQuery("SELECT * FROM players WHERE id = @id;", parameters))
      {
        try
        {
          if (dataTable == null || dataTable.Rows.Count == 0)
            return;
          DataRow row = dataTable.Rows[0];
          string str1 = (string) row[1];
          long int64_1 = Convert.ToInt64(row[2]);
          long int64_2 = Convert.ToInt64(row[3]);
          long int64_3 = Convert.ToInt64(row[4]);
          float x = (float) (double) row[5];
          float y = (float) (double) row[6];
          float z = (float) (double) row[7];
          float w = (float) (double) row[8];
          string str2 = Convert.ToString(row[11]);
          int num1 = (int) row[9];
          int[] numArray1 = (int[]) null;
          byte[] numArray2 = row[10] != DBNull.Value ? (byte[]) row[10] : (byte[]) null;
          if (numArray2 != null)
          {
            numArray1 = new int[numArray2.Length / 4];
            for (int index = 0; index < numArray1.Length; ++index)
            {
              int num2 = (((0 | (int) numArray2[index * 4 + 3]) << 8 | (int) numArray2[index * 4 + 2]) << 8 | (int) numArray2[index * 4 + 1]) << 8 | (int) numArray2[index * 4];
              numArray1[index] = num2;
            }
          }
          T obj = new T()
          {
            Name = str1,
            Token = str2,
            Id = playerId
          };
          obj.Astronaut.Position = new Position3(int64_1, int64_2, int64_3);
          obj.Astronaut.Orientation = new Quaternion(x, y, z, w);
          container.Player = obj;
          container.BoardedEntityId = num1;
          container.BoundEntityIds = numArray1 ?? new int[0];
          container.CreationSource = LoadedPlayerContainer<T>.Source.Database;
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to load the local player: " + ex.Message);
        }
      }
    }

    private byte[] LoadPlayerInventoryItems<T>(int playerId) where T : Player
    {
      return this.LoadIventoryItems(playerId, DatabaseStructure.InventoryId.Inventory).Key;
    }

    private byte[] LoadPlayerWearedItems(int playerId)
    {
      return this.LoadIventoryItems(playerId, DatabaseStructure.InventoryId.Suit).Key;
    }

    private byte[] LoadToolbarItems(int playerId)
    {
      return this.LoadIventoryItems(playerId, DatabaseStructure.InventoryId.Toolbar).Key;
    }

    private int ItemIdAndQuantityToInt(int itemId, int quantity)
    {
      return (0 | itemId & (int) ushort.MaxValue) << 16 | quantity & (int) ushort.MaxValue;
    }

    private void ItemIntToIdAndQuantity(int item, out int itemId, out int quantity)
    {
      itemId = -1;
      quantity = 0;
      itemId = item >> 16 & (int) ushort.MaxValue;
      quantity = item & (int) ushort.MaxValue;
    }

    private void StoreBlockSectors(
      WorldDataStorageInterface.StoreEntityContainer[] entitiesToStore)
    {
      if (entitiesToStore == null)
        return;
      lock (this.databaseLock)
      {
        SQLiteTransaction SQLiteTransaction = (SQLiteTransaction) null;
        try
        {
          SQLiteParameter parameter = new SQLiteParameter("id", DbType.Int32);
          this.dbConnection.Open();
          SQLiteTransaction = this.dbConnection.BeginTransaction();
          using (SQLiteCommand command = this.dbConnection.CreateCommand())
          {
            for (int index1 = 0; index1 < entitiesToStore.Length; ++index1)
            {
              SpaceEntity entity = entitiesToStore[index1].Entity;
              command.CommandText = "DELETE FROM blocksectors WHERE ownerId = @id";
              parameter.Value = (object) entity.Id;
              command.Parameters.Clear();
              command.Parameters.Add(parameter);
              command.ExecuteNonQuery();
              command.CommandText = "INSERT OR REPLACE INTO blocksectors VALUES (@id, @x, @y, @z, @form, @ids, @ors);";
              SQLiteParameter[] values = new SQLiteParameter[7]
              {
                new SQLiteParameter("id", DbType.Int32),
                new SQLiteParameter("x", DbType.Int32),
                new SQLiteParameter("y", DbType.Int32),
                new SQLiteParameter("z", DbType.Int32),
                new SQLiteParameter("form", DbType.Binary),
                new SQLiteParameter("ids", DbType.Binary),
                new SQLiteParameter("ors", DbType.Binary)
              };
              command.Parameters.AddRange(values);
              for (int index2 = 0; index2 < entity.BlockSectorManager.SectorList.Count; ++index2)
              {
                int id = entity.Id;
                BlockSector sector = entity.BlockSectorManager.SectorList[index2];
                PackedBlockSector result = new PackedBlockSector();
                if (this.compressor.CompressBlockSector(sector, ref result))
                {
                  values[0].Value = (object) id;
                  values[1].Value = (object) result.SectorPosition.X;
                  values[2].Value = (object) result.SectorPosition.Y;
                  values[3].Value = (object) result.SectorPosition.Z;
                  values[4].Value = (object) result.CompressedFormations;
                  values[5].Value = (object) result.CompressedIds;
                  values[6].Value = (object) result.CompressedOrientations;
                  command.ExecuteNonQuery();
                }
              }
            }
          }
          SQLiteTransaction.Commit();
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to store blocksectors: " + ex.Message);
        }
        finally
        {
          SQLiteTransaction?.Dispose();
          this.dbConnection.Close();
        }
      }
    }

    private void StoreEntities(
      WorldDataStorageInterface.StoreEntityContainer[] entitiesToStore)
    {
      lock (this.databaseLock)
      {
        SQLiteTransaction SQLiteTransaction = (SQLiteTransaction) null;
        try
        {
          this.dbConnection.Open();
          SQLiteTransaction = this.dbConnection.BeginTransaction();
          using (SQLiteCommand command = this.dbConnection.CreateCommand())
          {
            command.CommandText = "INSERT OR REPLACE INTO entities VALUES (@id, @loaded, @x, @y, @z, @ox, @oy, @oz, @ow, @sx, @sy, @sz);";
            SQLiteParameter[] values = new SQLiteParameter[12]
            {
              new SQLiteParameter("id", DbType.Int32),
              new SQLiteParameter("x", DbType.Int64),
              new SQLiteParameter("y", DbType.Int64),
              new SQLiteParameter("z", DbType.Int64),
              new SQLiteParameter("ox", DbType.Single),
              new SQLiteParameter("oy", DbType.Single),
              new SQLiteParameter("oz", DbType.Single),
              new SQLiteParameter("ow", DbType.Single),
              new SQLiteParameter("sx", DbType.Int32),
              new SQLiteParameter("sy", DbType.Int32),
              new SQLiteParameter("sz", DbType.Int32),
              new SQLiteParameter("loaded", DbType.Boolean)
            };
            command.Parameters.AddRange(values);
            for (int index = 0; index < entitiesToStore.Length; ++index)
            {
              SpaceEntity entity = entitiesToStore[index].Entity;
              int id = entity.Id;
              Position3 position = entity.Position;
              Quaternion orientation = entity.Orientation;
              Vector3int sectorPosition = entitiesToStore[index].SectorPosition;
              values[0].Value = (object) entity.Id;
              values[1].Value = (object) position.X;
              values[2].Value = (object) position.Y;
              values[3].Value = (object) position.Z;
              values[4].Value = (object) orientation.X;
              values[5].Value = (object) orientation.Y;
              values[6].Value = (object) orientation.Z;
              values[7].Value = (object) orientation.W;
              values[8].Value = (object) sectorPosition.X;
              values[9].Value = (object) sectorPosition.Y;
              values[10].Value = (object) sectorPosition.Z;
              values[11].Value = (object) false;
              command.ExecuteNonQuery();
            }
            SQLiteTransaction.Commit();
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to store entity: " + ex.Message);
        }
        finally
        {
          SQLiteTransaction?.Dispose();
          this.dbConnection.Close();
        }
      }
    }

    private void StorePlayer(
      WorldDataStorageInterface.StorePlayerContainer playerToStore)
    {
      int id = playerToStore.Id;
      string name = playerToStore.Name;
      Position3 position = playerToStore.Position;
      Quaternion orientation = playerToStore.Orientation;
      int boardedEntityId = playerToStore.BoardedEntityId;
      int[] closeEntitiesIds = playerToStore.CloseEntitiesIds;
      string token = playerToStore.Token;
      byte[] numArray = new byte[closeEntitiesIds.Length * 4];
      for (int index = 0; index < closeEntitiesIds.Length; ++index)
      {
        int num = closeEntitiesIds[index];
        numArray[index * 4] = (byte) (num & (int) byte.MaxValue);
        numArray[index * 4 + 1] = (byte) (num >> 8 & (int) byte.MaxValue);
        numArray[index * 4 + 2] = (byte) (num >> 16 & (int) byte.MaxValue);
        numArray[index * 4 + 3] = (byte) (num >> 24 & (int) byte.MaxValue);
      }
      SQLiteParameter[] parameters = new SQLiteParameter[12];
      SQLiteParameter[] SQLiteParameterArray1 = parameters;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter("id", DbType.Int32);
      SQLiteParameter1.Value = (object) id;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray1[0] = SQLiteParameter2;
      SQLiteParameter[] SQLiteParameterArray2 = parameters;
      SQLiteParameter SQLiteParameter3 = new SQLiteParameter("name", DbType.String);
      SQLiteParameter3.Value = (object) name;
      SQLiteParameter SQLiteParameter4 = SQLiteParameter3;
      SQLiteParameterArray2[1] = SQLiteParameter4;
      SQLiteParameter[] SQLiteParameterArray3 = parameters;
      SQLiteParameter SQLiteParameter5 = new SQLiteParameter("x", DbType.Int64);
      SQLiteParameter5.Value = (object) position.X;
      SQLiteParameter SQLiteParameter6 = SQLiteParameter5;
      SQLiteParameterArray3[2] = SQLiteParameter6;
      SQLiteParameter[] SQLiteParameterArray4 = parameters;
      SQLiteParameter SQLiteParameter7 = new SQLiteParameter("y", DbType.Int64);
      SQLiteParameter7.Value = (object) position.Y;
      SQLiteParameter SQLiteParameter8 = SQLiteParameter7;
      SQLiteParameterArray4[3] = SQLiteParameter8;
      SQLiteParameter[] SQLiteParameterArray5 = parameters;
      SQLiteParameter SQLiteParameter9 = new SQLiteParameter("z", DbType.Int64);
      SQLiteParameter9.Value = (object) position.Z;
      SQLiteParameter SQLiteParameter10 = SQLiteParameter9;
      SQLiteParameterArray5[4] = SQLiteParameter10;
      SQLiteParameter[] SQLiteParameterArray6 = parameters;
      SQLiteParameter SQLiteParameter11 = new SQLiteParameter("ox", DbType.Single);
      SQLiteParameter11.Value = (object) orientation.X;
      SQLiteParameter SQLiteParameter12 = SQLiteParameter11;
      SQLiteParameterArray6[5] = SQLiteParameter12;
      SQLiteParameter[] SQLiteParameterArray7 = parameters;
      SQLiteParameter SQLiteParameter13 = new SQLiteParameter("oy", DbType.Single);
      SQLiteParameter13.Value = (object) orientation.Y;
      SQLiteParameter SQLiteParameter14 = SQLiteParameter13;
      SQLiteParameterArray7[6] = SQLiteParameter14;
      SQLiteParameter[] SQLiteParameterArray8 = parameters;
      SQLiteParameter SQLiteParameter15 = new SQLiteParameter("oz", DbType.Single);
      SQLiteParameter15.Value = (object) orientation.Z;
      SQLiteParameter SQLiteParameter16 = SQLiteParameter15;
      SQLiteParameterArray8[7] = SQLiteParameter16;
      SQLiteParameter[] SQLiteParameterArray9 = parameters;
      SQLiteParameter SQLiteParameter17 = new SQLiteParameter("ow", DbType.Single);
      SQLiteParameter17.Value = (object) orientation.W;
      SQLiteParameter SQLiteParameter18 = SQLiteParameter17;
      SQLiteParameterArray9[8] = SQLiteParameter18;
      SQLiteParameter[] SQLiteParameterArray10 = parameters;
      SQLiteParameter SQLiteParameter19 = new SQLiteParameter("entityId", DbType.Int32);
      SQLiteParameter19.Value = (object) boardedEntityId;
      SQLiteParameter SQLiteParameter20 = SQLiteParameter19;
      SQLiteParameterArray10[9] = SQLiteParameter20;
      SQLiteParameter[] SQLiteParameterArray11 = parameters;
      SQLiteParameter SQLiteParameter21 = new SQLiteParameter("boundEntities", DbType.Binary);
      SQLiteParameter21.Value = (object) numArray;
      SQLiteParameter SQLiteParameter22 = SQLiteParameter21;
      SQLiteParameterArray11[10] = SQLiteParameter22;
      SQLiteParameter[] SQLiteParameterArray12 = parameters;
      SQLiteParameter SQLiteParameter23 = new SQLiteParameter("token", DbType.String);
      SQLiteParameter23.Value = (object) token;
      SQLiteParameter SQLiteParameter24 = SQLiteParameter23;
      SQLiteParameterArray12[11] = SQLiteParameter24;
      this.ExecuteNonQuery("INSERT OR REPLACE INTO players VALUES (@id, @name, @x, @y, @z, @ox, @oy, @oz, @ow, @entityId, @boundEntities, @token);", parameters);
    }

    private void StoreInventoryItems(
      int id,
      DatabaseStructure.InventoryId type,
      byte[] itemData,
      Vector3 position)
    {
      if (itemData == null)
        return;
      SQLiteParameter[] parameters = new SQLiteParameter[6];
      SQLiteParameter[] SQLiteParameterArray1 = parameters;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter(nameof (id), DbType.Int32);
      SQLiteParameter1.Value = (object) id;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray1[0] = SQLiteParameter2;
      SQLiteParameter[] SQLiteParameterArray2 = parameters;
      SQLiteParameter SQLiteParameter3 = new SQLiteParameter(nameof (type), DbType.Int32);
      SQLiteParameter3.Value = (object) (int) type;
      SQLiteParameter SQLiteParameter4 = SQLiteParameter3;
      SQLiteParameterArray2[1] = SQLiteParameter4;
      SQLiteParameter[] SQLiteParameterArray3 = parameters;
      SQLiteParameter SQLiteParameter5 = new SQLiteParameter("x", DbType.Single);
      SQLiteParameter5.Value = (object) position.X;
      SQLiteParameter SQLiteParameter6 = SQLiteParameter5;
      SQLiteParameterArray3[2] = SQLiteParameter6;
      SQLiteParameter[] SQLiteParameterArray4 = parameters;
      SQLiteParameter SQLiteParameter7 = new SQLiteParameter("y", DbType.Single);
      SQLiteParameter7.Value = (object) position.Y;
      SQLiteParameter SQLiteParameter8 = SQLiteParameter7;
      SQLiteParameterArray4[3] = SQLiteParameter8;
      SQLiteParameter[] SQLiteParameterArray5 = parameters;
      SQLiteParameter SQLiteParameter9 = new SQLiteParameter("z", DbType.Single);
      SQLiteParameter9.Value = (object) position.Z;
      SQLiteParameter SQLiteParameter10 = SQLiteParameter9;
      SQLiteParameterArray5[4] = SQLiteParameter10;
      SQLiteParameter[] SQLiteParameterArray6 = parameters;
      SQLiteParameter SQLiteParameter11 = new SQLiteParameter("items", DbType.Binary);
      SQLiteParameter11.Value = (object) itemData;
      SQLiteParameter SQLiteParameter12 = SQLiteParameter11;
      SQLiteParameterArray6[5] = SQLiteParameter12;
      this.ExecuteNonQuery("INSERT OR REPLACE INTO inventoryItems VALUES (@id, @type, @x, @y, @z, @items);", parameters);
    }

    private void StorePlayerWearedItems(int id, int?[] itemsToStore)
    {
      if (itemsToStore == null)
        return;
      int num1 = itemsToStore[0] ?? -1;
      int num2 = itemsToStore[1] ?? -1;
      int num3 = itemsToStore[2] ?? -1;
      int num4 = itemsToStore[3] ?? -1;
      int num5 = itemsToStore[4] ?? -1;
      int num6 = itemsToStore[5] ?? -1;
      int num7 = itemsToStore[6] ?? -1;
      SQLiteParameter[] parameters = new SQLiteParameter[8];
      SQLiteParameter[] SQLiteParameterArray1 = parameters;
      SQLiteParameter SQLiteParameter1 = new SQLiteParameter(nameof (id), DbType.Int32);
      SQLiteParameter1.Value = (object) id;
      SQLiteParameter SQLiteParameter2 = SQLiteParameter1;
      SQLiteParameterArray1[0] = SQLiteParameter2;
      SQLiteParameter[] SQLiteParameterArray2 = parameters;
      SQLiteParameter SQLiteParameter3 = new SQLiteParameter("head", DbType.Int32);
      SQLiteParameter3.Value = (object) num1;
      SQLiteParameter SQLiteParameter4 = SQLiteParameter3;
      SQLiteParameterArray2[1] = SQLiteParameter4;
      SQLiteParameter[] SQLiteParameterArray3 = parameters;
      SQLiteParameter SQLiteParameter5 = new SQLiteParameter("shoulders", DbType.Int32);
      SQLiteParameter5.Value = (object) num2;
      SQLiteParameter SQLiteParameter6 = SQLiteParameter5;
      SQLiteParameterArray3[2] = SQLiteParameter6;
      SQLiteParameter[] SQLiteParameterArray4 = parameters;
      SQLiteParameter SQLiteParameter7 = new SQLiteParameter("chest", DbType.Int32);
      SQLiteParameter7.Value = (object) num3;
      SQLiteParameter SQLiteParameter8 = SQLiteParameter7;
      SQLiteParameterArray4[3] = SQLiteParameter8;
      SQLiteParameter[] SQLiteParameterArray5 = parameters;
      SQLiteParameter SQLiteParameter9 = new SQLiteParameter("legs", DbType.Int32);
      SQLiteParameter9.Value = (object) num4;
      SQLiteParameter SQLiteParameter10 = SQLiteParameter9;
      SQLiteParameterArray5[4] = SQLiteParameter10;
      SQLiteParameter[] SQLiteParameterArray6 = parameters;
      SQLiteParameter SQLiteParameter11 = new SQLiteParameter("hands", DbType.Int32);
      SQLiteParameter11.Value = (object) num5;
      SQLiteParameter SQLiteParameter12 = SQLiteParameter11;
      SQLiteParameterArray6[5] = SQLiteParameter12;
      SQLiteParameter[] SQLiteParameterArray7 = parameters;
      SQLiteParameter SQLiteParameter13 = new SQLiteParameter("feet", DbType.Int32);
      SQLiteParameter13.Value = (object) num6;
      SQLiteParameter SQLiteParameter14 = SQLiteParameter13;
      SQLiteParameterArray7[6] = SQLiteParameter14;
      SQLiteParameter[] SQLiteParameterArray8 = parameters;
      SQLiteParameter SQLiteParameter15 = new SQLiteParameter("power", DbType.Int32);
      SQLiteParameter15.Value = (object) num7;
      SQLiteParameter SQLiteParameter16 = SQLiteParameter15;
      SQLiteParameterArray8[7] = SQLiteParameter16;
      this.ExecuteNonQuery("INSERT OR REPLACE INTO wearedItems VALUES (@id, @head, @shoulders, @chest, @legs, @hands, @feet, @power);", parameters);
    }

    public struct BlockSectorContainer
    {
      public int EntityId;
      public BlockSector Sector;

      public BlockSectorContainer(int id, BlockSector sector)
      {
        this.EntityId = id;
        this.Sector = sector;
      }
    }

    public class DatabaseAction
    {
      public Action<object[]> Action;
      public object Data;
      public bool CanBeGrouped;
      public int Priority;
    }

    public struct LoadBlockSectorContainer
    {
      public int EntityId;
      public Vector3int SectorPosition;
      public Action<BlockSector> ResultFunction;

      public LoadBlockSectorContainer(int id, Vector3int pos, Action<BlockSector> result)
      {
        this.EntityId = id;
        this.SectorPosition = pos;
        this.ResultFunction = result;
      }
    }

    public struct LoadSpaceEntityContainer
    {
      public Vector3int LowerSpaceBounds;
      public Vector3int UpperSpaceBounds;
      public Position3 PlayerPosition;
      public Action<SpaceEntity> ResultFunction;

      public LoadSpaceEntityContainer(
        Vector3int lb,
        Vector3int ub,
        Position3 playerPosition,
        Action<SpaceEntity> result)
      {
        this.LowerSpaceBounds = lb;
        this.UpperSpaceBounds = ub;
        this.PlayerPosition = playerPosition;
        this.ResultFunction = result;
      }
    }

    public struct SpaceSectorContainer
    {
      public Vector3int SectorPosition;
      public World.SectorState markedState;
      public Action<World.SectorState> ResultFunction;

      public SpaceSectorContainer(Vector3int position, Action<World.SectorState> result)
      {
        this.SectorPosition = position;
        this.markedState = World.SectorState.not_processed;
        this.ResultFunction = result;
      }

      public SpaceSectorContainer(Vector3int position, World.SectorState state)
      {
        this.SectorPosition = position;
        this.markedState = state;
        this.ResultFunction = (Action<World.SectorState>) null;
      }
    }

    public struct StoreBeaconContainer
    {
      public Position3 Position;
      public string Message;

      public StoreBeaconContainer(Position3 position, string message)
      {
        this.Position = position;
        this.Message = message;
      }
    }

    public struct StoreEntityContainer
    {
      public SpaceEntity Entity;
      public Vector3int SectorPosition;

      public StoreEntityContainer(SpaceEntity entity, Vector3int position)
      {
        this.Entity = entity;
        this.SectorPosition = position;
      }
    }

    public struct StorePlayerContainer
    {
      public int[] CloseEntitiesIds;
      public int Id;
      public string Name;
      public Position3 Position;
      public Quaternion Orientation;
      public string Token;
      public int BoardedEntityId;
      public byte[] WearedItemsIds;
      public byte[] InventoryItems;
      public byte[] ToolbarItemIds;
    }

    private enum ActionPriorities
    {
      StorePlayer = 4,
      LoadEntities = 5,
      SectorDataExists = 6,
      StoreEntity = 7,
      StoreBeacon = 8,
      RemoveEntity = 9,
      RemoveBeacon = 10, // 0x0000000A
      MarkSector = 11, // 0x0000000B
    }
  }
}
