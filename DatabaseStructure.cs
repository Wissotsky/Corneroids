// Decompiled with JetBrains decompiler
// Type: CornerSpace.DatabaseStructure
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class DatabaseStructure
  {
    private string[][] structure;
    private static DatabaseStructure instance;

    private DatabaseStructure() => this.InitializeStructure();

    public static DatabaseStructure Instance
    {
      get
      {
        if (DatabaseStructure.instance == null)
          DatabaseStructure.instance = new DatabaseStructure();
        return DatabaseStructure.instance;
      }
    }

    public string DatabaseCreationQuery
    {
      get
      {
        string empty = string.Empty;
        for (int index1 = 0; index1 < this.structure.Length; ++index1)
        {
          if (this.structure[index1].Length > 1)
          {
            string str1 = "CREATE TABLE " + this.structure[index1][0] + " (";
            for (int index2 = 1; index2 < this.structure[index1].Length; ++index2)
            {
              str1 += this.structure[index1][index2];
              if (index2 < this.structure[index1].Length - 1)
                str1 += ", ";
            }
            string str2 = str1 + ");";
            empty += str2;
          }
        }
        return empty;
      }
    }

    private void InitializeStructure()
    {
      this.structure = new string[10][];
      this.structure[0] = new string[7]
      {
        "worldinfo",
        "name VARCHAR(50)",
        "version FLOAT",
        "seed INT",
        "itemsetKey VARCHAR(50)",
        "timestamp LONG",
        "token VARCHAR(20)"
      };
      this.structure[1] = new string[13]
      {
        "players",
        "id INT PRIMARY KEY",
        "name VARCHAR(50)",
        "posx LONG",
        "posy LONG",
        "posz LONG",
        "orx FLOAT",
        "ory FLOAT",
        "orz FLOAT",
        "orw FLOAT",
        "boardedEntity INT",
        "boundEntities BLOB",
        "token VARCHAR(20)"
      };
      this.structure[2] = new string[13]
      {
        "entities",
        "id INT PRIMARY KEY",
        "loaded BOOLEAN",
        "posx LONG",
        "posy LONG",
        "posz LONG",
        "orx FLOAT",
        "ory FLOAT",
        "orz FLOAT",
        "orw FLOAT",
        "sectorx INT",
        "sectory INT",
        "sectorz INT"
      };
      this.structure[3] = new string[9]
      {
        "blocksectors",
        "ownerId INT",
        "posx INT",
        "posy INT",
        "posz INT",
        "formation BLOB",
        "blockIds BLOB",
        "orientations BLOB",
        "PRIMARY KEY (ownerId, posx, posy, posz)"
      };
      this.structure[4] = new string[8]
      {
        "controlledblocks",
        "id INT",
        "boundTo INT",
        "bindingColor INT",
        "ownerEntityId INT",
        "posx FLOAT",
        "posy FLOAT",
        "posz FLOAT"
      };
      this.structure[5] = new string[7]
      {
        "controlblocks",
        "id INT",
        "bindingId INT",
        "ownerEntityId INT",
        "posx FLOAT",
        "posy FLOAT",
        "posz FLOAT"
      };
      this.structure[6] = new string[5]
      {
        "keyColorMappings",
        "ownerEntityId INT",
        "blockId INT",
        "color INT",
        "key INT"
      };
      this.structure[7] = new string[6]
      {
        "spacesectors",
        "sectorx INT",
        "sectory INT",
        "sectorz INT",
        "state INT",
        "PRIMARY KEY (sectorx, sectory, sectorz)"
      };
      this.structure[8] = new string[8]
      {
        "inventoryItems",
        "id INT",
        "type INT",
        "posx FLOAT",
        "posy FLOAT",
        "posz FLOAT",
        "items BLOB",
        "PRIMARY KEY (id, posx, posy, posz)"
      };
      this.structure[9] = new string[6]
      {
        "beacons",
        "posx LONG",
        "posy LONG",
        "posz LONG",
        "message VARCHAR(20)",
        "PRIMARY KEY (posx, posy, posz)"
      };
    }

    public enum InventoryId : byte
    {
      Inventory,
      Chest,
      Toolbar,
      Suit,
    }
  }
}
