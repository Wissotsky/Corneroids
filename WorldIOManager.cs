// Decompiled with JetBrains decompiler
// Type: CornerSpace.WorldIOManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace CornerSpace
{
  public class WorldIOManager
  {
    private const string cPremadeEntityFolderPath = "\\Entities\\";
    private object queueLock = new object();
    private Queue<System.Action> ioQueueAsync;
    private static WorldIOManager instance;
    private StringBuilder stringBuilder;

    private WorldIOManager()
    {
      this.ioQueueAsync = new Queue<System.Action>();
      this.stringBuilder = new StringBuilder();
    }

    public static DataTable ExecuteQuery(
      SQLiteConnection connection,
      string query,
      SQLiteParameter[] parameters)
    {
      try
      {
        if (connection.State == ConnectionState.Closed)
          connection.Open();
        using (SQLiteDataAdapter SQLiteDataAdapter = new SQLiteDataAdapter())
        {
          using (SQLiteCommand SQLiteCommand = new SQLiteCommand(connection))
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

    public string GetDatabasePathForWorld(string worldName)
    {
      try
      {
        string worldsFolderPath = this.GetWorldsFolderPath();
        if (!Directory.Exists(worldsFolderPath))
          Directory.CreateDirectory(worldsFolderPath);
        string path2 = Regex.Replace(worldName, new string(Path.GetInvalidFileNameChars()), "") + ".wrl";
        return Path.Combine(worldsFolderPath, path2);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a directory for worlds: " + ex.Message);
        return (string) null;
      }
    }

    public WorldInfo[] GetSavedWorlds(float version)
    {
      string[] worldFileCandidates = this.GetWorldFileCandidates();
      List<WorldInfo> worldInfoList = new List<WorldInfo>();
      foreach (string worldPath in worldFileCandidates)
      {
        WorldInfo worldData = this.GetWorldData(worldPath);
        if (worldData != null && (double) worldData.Version == (double) version)
          worldInfoList.Add(worldData);
      }
      return worldInfoList.ToArray();
    }

    public WorldDataStorageInterface LoadDataStorageForWorld(World world)
    {
      try
      {
        string str1 = world != null ? this.GetDatabasePathForWorld(world.Name) : throw new ArgumentNullException();
        if (!File.Exists(str1))
          throw new Exception("Database for a world was not found (name=" + world.Name + ")");
        string str2 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.Copy(str1, str2);
        SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + str2);
        this.InitializeLoadedDatabase(dbConnection);
        return new WorldDataStorageInterface(world, dbConnection, str1, str2);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load a database: " + ex.Message);
        return (WorldDataStorageInterface) null;
      }
    }

    public void RequestDataStorageCreationForWorld(World world)
    {
      try
      {
        string str = world != null ? world.DatabasePath : throw new ArgumentNullException();
        if (string.IsNullOrEmpty(str))
          throw new Exception("Failed to get a path for the storage file.");
        if (File.Exists(str))
          throw new Exception("World name is invalid, empty or already in use.");
        SQLiteConnection.CreateFile(str);
        if (!File.Exists(str))
          throw new Exception("Failed to create the storage file.");
        using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + str))
        {
          if (!this.InitializeDatabase(dbConnection, world))
          {
            File.Delete(str);
            throw new Exception("Storage initialization failed.");
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to initialize storage file: " + ex.Message);
        throw ex;
      }
    }

    public bool RequestDataStorageDeletion(World world)
    {
      if (world != null)
      {
        try
        {
          string databasePath = world.DatabasePath;
          if (string.IsNullOrEmpty(databasePath))
            return false;
          if (File.Exists(databasePath))
            File.Delete(databasePath);
          return true;
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to remove a database file: " + ex.Message);
        }
      }
      return false;
    }

    public void RequestSaveWorldFile(WorldDataStorageInterface dataInterface)
    {
      try
      {
        string temporaryFileLocation = dataInterface.TemporaryFileLocation;
        string originalFileLocation = dataInterface.OriginalFileLocation;
        string path2 = Path.GetFileNameWithoutExtension(originalFileLocation) + "_" + Path.GetRandomFileName();
        string str = Path.Combine(Path.GetDirectoryName(originalFileLocation), path2);
        if (File.Exists(originalFileLocation))
          File.Move(originalFileLocation, str);
        File.Copy(temporaryFileLocation, originalFileLocation);
        if (!File.Exists(str))
          return;
        File.Delete(str);
      }
      catch
      {
        throw new Exception("Backup restored.");
      }
    }

    public static WorldIOManager Instance
    {
      get
      {
        if (WorldIOManager.instance == null)
          WorldIOManager.instance = new WorldIOManager();
        return WorldIOManager.instance;
      }
    }

    private WorldInfo GetWorldData(string worldPath)
    {
      if (File.Exists(worldPath))
      {
        SQLiteConnection connection = (SQLiteConnection) null;
        try
        {
          connection = new SQLiteConnection("Data Source=" + worldPath);
          if (connection != null)
          {
            using (DataTable dataTable = WorldIOManager.ExecuteQuery(connection, "SELECT name, version, seed, itemsetKey, timestamp FROM worldinfo", (SQLiteParameter[]) null))
            {
              if (dataTable != null)
              {
                DataRow row = dataTable.Rows[0];
                string str1 = (string) row[0];
                float num1 = (float) (double) row[1];
                int num2 = (int) row[2];
                string str2 = (string) row[3];
                DateTime dateTime = DateTime.FromBinary((long) row[4]);
                return new WorldInfo()
                {
                  Name = str1,
                  Version = num1,
                  Seed = num2,
                  ItemsetKey = str2,
                  Path = worldPath,
                  TimesStamp = dateTime
                };
              }
            }
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to get world data: " + ex.Message);
        }
        finally
        {
          if (connection != null)
          {
            connection.Close();
            connection.Dispose();
          }
        }
      }
      return (WorldInfo) null;
    }

    private string[] GetWorldFileCandidates()
    {
      string worldsFolderPath = this.GetWorldsFolderPath();
      if (!string.IsNullOrEmpty(worldsFolderPath))
      {
        try
        {
          if (!Directory.Exists(worldsFolderPath))
            Directory.CreateDirectory(worldsFolderPath);
          return Directory.GetFiles(worldsFolderPath, "*.wrl");
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to create a folder to worlds: " + ex.Message);
        }
      }
      return new string[0];
    }

    private string GetWorldsFolderPath()
    {
      return Path.Combine(Engine.SettingsManager.UserDataFolder, "Worlds");
    }

    private bool InitializeDatabase(SQLiteConnection dbConnection, World world)
    {
      try
      {
        dbConnection.Open();
        string databaseCreationQuery = DatabaseStructure.Instance.DatabaseCreationQuery;
        using (SQLiteCommand SQLiteCommand = new SQLiteCommand(dbConnection))
        {
          SQLiteCommand.CommandText = databaseCreationQuery;
          SQLiteCommand.ExecuteNonQuery();
          string str1 = Engine.WorldVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          string str2 = "INSERT INTO worldinfo VALUES ('" + world.Name + "'," + str1 + "," + (object) world.Seed + ",'" + world.Key + "'," + (object) DateTime.Now.ToBinary() + ",'" + world.Token + "');";
          SQLiteCommand.CommandText = str2;
          SQLiteCommand.ExecuteNonQuery();
          return true;
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Database initialization failed: " + ex.Message);
        return false;
      }
      finally
      {
        dbConnection.Close();
      }
    }

    private void InitializeLoadedDatabase(SQLiteConnection dbConnection)
    {
      try
      {
        dbConnection.Open();
        string str1 = "UPDATE spacesectors SET state = 0 WHERE state = 1;";
        string str2 = "UPDATE entities SET loaded = 0;";
        using (SQLiteCommand SQLiteCommand = new SQLiteCommand(dbConnection))
        {
          SQLiteCommand.CommandText = str1;
          SQLiteCommand.ExecuteNonQuery();
          SQLiteCommand.CommandText = str2;
          SQLiteCommand.ExecuteNonQuery();
        }
      }
      catch
      {
        throw new Exception("Failed to initialize database");
      }
      finally
      {
        dbConnection.Close();
      }
    }
  }
}
