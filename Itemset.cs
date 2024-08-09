// Decompiled with JetBrains decompiler
// Type: CornerSpace.Itemset
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class Itemset
  {
    private static readonly Dictionary<string, System.Type> blockTypeCollection = new Dictionary<string, System.Type>()
    {
      {
        "armor",
        typeof (BasicBlockType)
      },
      {
        "asteroid",
        typeof (AsteroidBlockType)
      },
      {
        "console",
        typeof (ControlBlockType)
      },
      {
        "door",
        typeof (DoorBlockType)
      },
      {
        "thruster",
        typeof (EngineBlockType)
      },
      {
        "mineral",
        typeof (MineralBlockType)
      },
      {
        "pointLight",
        typeof (PointLightBlockType)
      },
      {
        "power",
        typeof (PowerBlockType)
      },
      {
        "gun",
        typeof (GunBlockType)
      }
    };
    private string name;
    private string author;
    private string key;
    private float version;
    private XDocument document;
    private string blockTextureKey;
    private string blockTextureName;
    private string spriteTextureKey;
    private string spriteTextureName;
    private CraftingManager craftingManager;
    private Random random;
    private int sumOfMineralRarities;
    private List<AsteroidBlockType>[] asteroidBlockTypes;
    private BlockType[] blockTypes;
    private Item[] items;
    private List<MineralBlockType>[] mineralBlockTypes;
    private CraftingBlockType defaultCraftingTableBlock;
    private DrillBlockTool defaultDrill;
    private CraftingBlockType defaultSmelterBlock;
    private List<Item> defaultItems;
    private ProjectileType[] projectileTypes;

    public Itemset()
    {
      this.craftingManager = new CraftingManager();
      this.random = new Random();
      this.blockTypes = new BlockType[(int) byte.MaxValue];
      this.items = new Item[1024];
      this.projectileTypes = new ProjectileType[(int) byte.MaxValue];
      this.defaultItems = new List<Item>();
      this.mineralBlockTypes = new List<MineralBlockType>[3];
      this.mineralBlockTypes[0] = new List<MineralBlockType>();
      this.mineralBlockTypes[1] = new List<MineralBlockType>();
      this.mineralBlockTypes[2] = new List<MineralBlockType>();
      this.asteroidBlockTypes = new List<AsteroidBlockType>[3];
      this.asteroidBlockTypes[0] = new List<AsteroidBlockType>();
      this.asteroidBlockTypes[1] = new List<AsteroidBlockType>();
      this.asteroidBlockTypes[2] = new List<AsteroidBlockType>();
    }

    public Block GetBlockByID(int id)
    {
      if (id < 0 || id >= this.blockTypes.Length)
        return (Block) null;
      return !((Item) this.blockTypes[id] != (Item) null) ? (Block) null : this.blockTypes[id].CreateBlock();
    }

    public Block GetAsteroidBlock(AsteroidBlockType.Type asteroidType, int number)
    {
      int index = (int) asteroidType;
      return index >= 0 && index < this.asteroidBlockTypes.Length && number >= 0 && number < this.asteroidBlockTypes[index].Count ? this.asteroidBlockTypes[index][number].CreateBlock() : (Block) null;
    }

    public Item GetItem(int id)
    {
      if (id >= 0 && id < this.items.Length)
      {
        Item obj = this.items[id];
        if (obj != (Item) null)
          return obj.Copy();
      }
      return (Item) null;
    }

    public Block GetMineralByProbability(int asteroidType, int probability)
    {
      int num = 0;
      probability %= this.sumOfMineralRarities;
      if (asteroidType >= 0 && asteroidType < this.mineralBlockTypes.Length)
      {
        for (int index = 0; index < this.mineralBlockTypes[asteroidType].Count; ++index)
        {
          num += (int) this.mineralBlockTypes[asteroidType][index].Rarity;
          if (probability <= num)
            return this.mineralBlockTypes[asteroidType][index].CreateBlock();
        }
      }
      return (Block) null;
    }

    public ProjectileType GetProjectileType(byte id)
    {
      return id >= (byte) 0 && (int) id < this.projectileTypes.Length ? this.projectileTypes[(int) id] : (ProjectileType) null;
    }

    public bool ParseFromXml(XDocument blocksetDoc)
    {
      try
      {
        XmlReader instance = XmlReader.Instance;
        XElement xelement1 = blocksetDoc.Element((XName) "itemset");
        this.name = instance.ReadElementValue<string>(xelement1, new XmlReader.ConvertValue<string>(instance.ReadString), "noname", "name");
        this.author = instance.ReadElementValue<string>(xelement1, new XmlReader.ConvertValue<string>(instance.ReadString), "noname", "author");
        this.key = instance.ReadElementValue<string>(xelement1, new XmlReader.ConvertValue<string>(instance.ReadString), string.Empty, "key");
        this.version = instance.ReadAttributeValue<float>(xelement1, new XmlReader.ConvertValue<float>(instance.ReadFloat), "version", 1f, (string[]) null);
        this.ParseTextures(xelement1.Element((XName) "textures"));
        this.ParseProjectiles(xelement1);
        this.ParseBlocks(xelement1);
        if (xelement1.Element((XName) "items") != null)
        {
          XElement xelement2 = xelement1.Element((XName) "items");
          this.ParseItems<ConsumableItem>(xelement2.Element((XName) "consumables"), "consumable");
          this.ParseItems<InventoryItem>(xelement2.Element((XName) "ingredients"), "ingredient");
          this.ParseItems<DrillBlockTool>(xelement2.Element((XName) "drills"), "drill");
          this.ParseItems<TorchItem>(xelement2.Element((XName) "torches"), "torch");
          this.ParseItems<WeaponItem>(xelement2.Element((XName) "weapons"), "weapon");
          this.ParseItems<WearableItem>(xelement2.Element((XName) "wearables"), "wearable");
        }
        if (xelement1.Element((XName) "crafting") != null)
          this.ParseBlueprints(xelement1.Element((XName) "crafting"));
        this.ParseDefaultItems(xelement1.Element((XName) "defaultItems"));
        this.document = blocksetDoc;
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Blockset: " + ex.Message);
        return false;
      }
    }

    public override string ToString()
    {
      return (this.name ?? "noname") + " by " + (this.author ?? "anonymous");
    }

    public string ItemsetName => this.name;

    public string BlockTextureKey => this.blockTextureKey;

    public string BlockTextureName => this.blockTextureName;

    public CraftingBlockType DefaultCraftingTableBlock => this.defaultCraftingTableBlock;

    public DrillBlockTool DefaultDrill => this.defaultDrill;

    public CraftingBlockType DefaultSmelterBlock => this.defaultSmelterBlock;

    public List<Item> DefaultItems => this.defaultItems;

    public XDocument ItemsetXML => this.document;

    public int NumberOfHeavyAsteroidBlocks => this.asteroidBlockTypes[2].Count;

    public int NumberOfLightAsteroidBlocks => this.asteroidBlockTypes[0].Count;

    public int NumberOfMediumAsteroidBlocks => this.asteroidBlockTypes[1].Count;

    public string SpriteTextureKey => this.spriteTextureKey;

    public string SpriteTextureName => this.spriteTextureName;

    public float Version => this.version;

    public CraftingManager Crafting => this.craftingManager;

    public BlockType[] BlockTypes => this.blockTypes;

    public Item[] Items => this.items;

    public string Key => this.key;

    public ProjectileType[] ProjectileTypes => this.projectileTypes;

    private int GetFreeItemId()
    {
      return Array.FindIndex<Item>(this.items, (Predicate<Item>) (i => i == (Item) null));
    }

    private void ParseBlocks(XElement documentRoot)
    {
      if (documentRoot == null)
        return;
      if (documentRoot.Element((XName) "blocks") == null)
        throw new Exception("Itemset contains no blocks-element");
      XmlReader instance1 = XmlReader.Instance;
      Dictionary<string, System.Type> dictionary = new Dictionary<string, System.Type>()
      {
        {
          "armors",
          typeof (BasicBlockType)
        },
        {
          "asteroids",
          typeof (AsteroidBlockType)
        },
        {
          "cameras",
          typeof (CameraBlockType)
        },
        {
          "consoles",
          typeof (ControlBlockType)
        },
        {
          "crafting",
          typeof (CraftingBlockType)
        },
        {
          "doors",
          typeof (DoorBlockType)
        },
        {
          "gunConsoles",
          typeof (GunControlBlockType)
        },
        {
          "guns",
          typeof (GunBlockType)
        },
        {
          "minerals",
          typeof (MineralBlockType)
        },
        {
          "pointLights",
          typeof (PointLightBlockType)
        },
        {
          "power",
          typeof (PowerBlockType)
        },
        {
          "thrusters",
          typeof (EngineBlockType)
        },
        {
          "containers",
          typeof (ContainerBlockType)
        },
        {
          "windows",
          typeof (WindowBlockType)
        }
      };
      foreach (XElement element1 in documentRoot.Element((XName) "blocks").Elements())
      {
        try
        {
          System.Type type = dictionary.ContainsKey(element1.Name.ToString()) ? dictionary[element1.Name.ToString()] : (System.Type) null;
          if (type == null)
            throw new Exception("Unknown block type: " + element1.Value);
          foreach (XElement element2 in element1.Elements())
          {
            try
            {
              byte index1 = instance1.ReadAttributeValue<byte>(element2, new XmlReader.ConvertValue<byte>(instance1.ReadByte), "blockId", (byte) 0, (string[]) null);
              int index2 = instance1.ReadAttributeValue<int>(element2, new XmlReader.ConvertValue<int>(instance1.ReadInt), "itemId", 0, (string[]) null);
              if (index1 >= (byte) 0)
              {
                if ((int) index1 < this.blockTypes.Length)
                {
                  if (index2 >= 0)
                  {
                    if (index2 < this.items.Length)
                    {
                      if ((Item) this.blockTypes[(int) index1] != (Item) null || this.items[index2] != (Item) null)
                        throw new Exception("Duplicate itemId and/or blockId found.");
                      BlockType instance2 = Activator.CreateInstance(type) as BlockType;
                      instance2.LoadFromXml(element2);
                      this.blockTypes[(int) index1] = instance2;
                      this.items[index2] = (Item) instance2;
                      this.SetAsteroidBlockType(instance2 as AsteroidBlockType);
                      this.SetMineralBlockType(instance2 as MineralBlockType);
                    }
                  }
                }
              }
            }
            catch (Exception ex)
            {
              Engine.Console.WriteErrorLine("Failed to parse a block: " + ex.Message);
            }
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine(ex.Message);
        }
      }
    }

    private void ParseBlueprints(XElement craftingRoot)
    {
      if (craftingRoot == null)
        return;
      XmlReader instance = XmlReader.Instance;
      if (craftingRoot.Element((XName) "blueprints") == null)
        throw new Exception("Document contains no crafting-element!");
      foreach (XElement element1 in craftingRoot.Element((XName) "blueprints").Elements())
      {
        if (!(element1.Name != (XName) "blueprint"))
        {
          try
          {
            string name = instance.ReadAttributeValue<string>(element1, new XmlReader.ConvertValue<string>(instance.ReadString), "name", "noname", (string[]) null);
            List<KeyValuePair<Item, int>> keyValuePairList = new List<KeyValuePair<Item, int>>();
            KeyValuePair<Item, int> keyValuePair1 = new KeyValuePair<Item, int>((Item) null, 0);
            foreach (XElement element2 in element1.Element((XName) "ingredients").Elements())
            {
              int num = (int) instance.ReadAttributeValue<byte>(element2, new XmlReader.ConvertValue<byte>(instance.ReadByte), "quantity", (byte) 0, (string[]) null);
              int index = instance.ReadAttributeValue<int>(element2, new XmlReader.ConvertValue<int>(instance.ReadInt), "itemId", 0, (string[]) null);
              if (index < 0 || index >= this.items.Length || num <= 0)
                throw new Exception("Invalid blueprint itemId or quantity");
              Item key = this.items[index];
              if (key != (Item) null)
                keyValuePairList.Add(new KeyValuePair<Item, int>(key, num));
            }
            int index1 = instance.ReadAttributeValue<int>(element1, new XmlReader.ConvertValue<int>(instance.ReadInt), "itemId", 0, "result");
            int num1 = instance.ReadAttributeValue<int>(element1, new XmlReader.ConvertValue<int>(instance.ReadInt), "quantity", 0, "result");
            if (index1 < 0 || index1 >= this.items.Length || num1 <= 0)
              throw new Exception("Invalid blueprint itemId or quantity");
            Item key1 = this.items[index1];
            if (key1 != (Item) null)
              keyValuePair1 = new KeyValuePair<Item, int>(key1, num1);
            int num2 = instance.ReadAttributeValue<int>(element1, new XmlReader.ConvertValue<int>(instance.ReadInt), "usage", 0, (string[]) null);
            Blueprint.Usage usage = Blueprint.Usage.Craft;
            switch (num2)
            {
              case 1:
                usage = Blueprint.Usage.Extractor;
                break;
              case 2:
                usage = Blueprint.Usage.Smelter;
                break;
            }
            Blueprint blueprint = new Blueprint(name, usage);
            foreach (KeyValuePair<Item, int> keyValuePair2 in keyValuePairList)
              blueprint.AddNewIngredient(keyValuePair2.Key, keyValuePair2.Value);
            blueprint.SetResultItem(keyValuePair1.Key, keyValuePair1.Value);
            this.craftingManager.AddNewBlueprint(blueprint);
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Failed to parse a blueprint: " + ex.Message);
          }
        }
      }
    }

    private void ParseDefaultItems(XElement defaultItemsEle)
    {
      if (defaultItemsEle == null)
        return;
      XmlReader instance = XmlReader.Instance;
      foreach (XElement element in defaultItemsEle.Elements())
      {
        if (element.Name == (XName) "item")
        {
          int itemId = instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "itemId", -1, (string[]) null);
          Item obj = Array.Find<Item>(this.items, (Predicate<Item>) (i => i != (Item) null && i.ItemId == itemId));
          if (obj != (Item) null)
          {
            switch (obj)
            {
              case DrillBlockTool _:
                this.defaultDrill = this.defaultDrill ?? obj as DrillBlockTool;
                continue;
              case CraftingBlockType _:
                CraftingBlockType craftingBlockType = obj as CraftingBlockType;
                if ((Item) craftingBlockType != (Item) null)
                {
                  if (craftingBlockType.UsageType != CraftingBlockType.Type.Extractor)
                  {
                    if (craftingBlockType.UsageType == CraftingBlockType.Type.CraftingTable)
                    {
                      this.defaultCraftingTableBlock = craftingBlockType;
                      continue;
                    }
                    if (craftingBlockType.UsageType == CraftingBlockType.Type.Smelter)
                    {
                      this.defaultSmelterBlock = craftingBlockType;
                      continue;
                    }
                    continue;
                  }
                  this.defaultItems.Add(obj);
                  continue;
                }
                continue;
              default:
                this.defaultItems.Add(obj);
                continue;
            }
          }
        }
      }
    }

    private void ParseConsumables(XElement consumablesRoot)
    {
      if (consumablesRoot == null)
        return;
      XmlReader instance = XmlReader.Instance;
      foreach (XElement element in consumablesRoot.Elements())
      {
        if (!(element.Name != (XName) "consumable"))
        {
          try
          {
            int index = instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "itemId", 0, (string[]) null);
            ConsumableItem consumableItem = new ConsumableItem();
            consumableItem.LoadFromXml(element);
            if (index >= 0)
            {
              if (index < this.items.Length)
              {
                if (this.items[index] != (Item) null)
                  throw new Exception("Duplicate item id found");
                this.items[index] = (Item) consumableItem;
              }
            }
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Failed to parse an ingredient: " + ex.Message);
          }
        }
      }
    }

    private void ParseItems<T>(XElement root, string elementName) where T : Item, new()
    {
      if (root == null || string.IsNullOrEmpty(elementName))
        return;
      XmlReader instance = XmlReader.Instance;
      foreach (XElement element in root.Elements((XName) elementName))
      {
        try
        {
          int index = instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "itemId", 0, (string[]) null);
          T obj = new T();
          obj.LoadFromXml(element);
          if (index >= 0)
          {
            if (index < this.items.Length)
            {
              if (this.items[index] != (Item) null)
                throw new Exception("Duplicate item id found");
              this.items[index] = (Item) obj;
            }
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to parse an item of type " + elementName + ": " + ex.Message);
        }
      }
    }

    private bool ParseProjectiles(XElement documentRoot)
    {
      if (documentRoot == null || documentRoot.Element((XName) "projectiles") == null)
        return false;
      foreach (XElement element in documentRoot.Element((XName) "projectiles").Elements())
      {
        try
        {
          XmlReader instance = XmlReader.Instance;
          short index = instance.ReadAttributeValue<short>(element, new XmlReader.ConvertValue<short>(instance.ReadShort), "projectileId", (short) -1, (string[]) null);
          if (index >= (short) 0)
          {
            if ((int) index < this.projectileTypes.Length)
            {
              if (this.projectileTypes[(int) index] != null)
                throw new Exception("Duplicate projectile id found.");
              ProjectileType projectileType = new ProjectileType();
              projectileType.LoadFromXml(element);
              this.projectileTypes[(int) index] = projectileType;
            }
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to parse a projectile: " + ex.Message);
        }
      }
      return true;
    }

    private void ParseTextures(XElement textureElement)
    {
      if (textureElement == null)
        throw new Exception("textures - element was not found.");
      XmlReader instance = XmlReader.Instance;
      textureElement.Element((XName) "blockTexture");
      this.blockTextureName = instance.ReadElementValue<string>(textureElement, new XmlReader.ConvertValue<string>(instance.ReadString), (string) null, "blockTexture", "name");
      this.blockTextureKey = instance.ReadElementValue<string>(textureElement, new XmlReader.ConvertValue<string>(instance.ReadString), (string) null, "blockTexture", "key");
      textureElement.Element((XName) "spriteTexture");
      this.spriteTextureName = instance.ReadElementValue<string>(textureElement, new XmlReader.ConvertValue<string>(instance.ReadString), (string) null, "spriteTexture", "name");
      this.spriteTextureKey = instance.ReadElementValue<string>(textureElement, new XmlReader.ConvertValue<string>(instance.ReadString), (string) null, "spriteTexture", "key");
    }

    private void SetAsteroidBlockType(AsteroidBlockType asteroid)
    {
      if (!((Item) asteroid != (Item) null) || asteroid is MineralBlockType)
        return;
      byte asteroidType = (byte) asteroid.AsteroidType;
      if (asteroidType < (byte) 0 || (int) asteroidType >= this.asteroidBlockTypes.Length)
        return;
      this.asteroidBlockTypes[(int) asteroidType].Add(asteroid);
    }

    private void SetMineralBlockType(MineralBlockType mineral)
    {
      if (!((Item) mineral != (Item) null))
        return;
      int asteroidType = (int) mineral.AsteroidType;
      if (asteroidType < 0 || asteroidType >= this.asteroidBlockTypes.Length)
        return;
      this.mineralBlockTypes[asteroidType].Add(mineral);
      this.sumOfMineralRarities += (int) mineral.Rarity;
    }

    public void PrintAvailableBlockIds(string[] parameters, List<string> result)
    {
      if (parameters.Length == 1)
      {
        byte result1;
        if (byte.TryParse(parameters[0], out result1))
        {
          for (int index = (int) result1; index < (int) result1 + 19; ++index)
          {
            if (index >= 0 && index < (int) byte.MaxValue)
              result.Add(index.ToString() + " = " + ((Item) this.blockTypes[index] != (Item) null ? this.blockTypes[index].Name : "none"));
          }
        }
        else
          result.Add("Parameter has to be an index [0, 255]");
      }
      else
        result.Add("Expected 1 parameter, received " + (object) parameters.Length);
    }

    public void PrintAvailableProjectiles(string[] parameters, List<string> result)
    {
      if (parameters.Length == 1)
      {
        byte result1;
        if (byte.TryParse(parameters[0], out result1))
        {
          for (int index = (int) result1; index < (int) result1 + 19; ++index)
          {
            if (index >= 0 && index < (int) byte.MaxValue)
              result.Add(index.ToString() + " = " + (this.projectileTypes[index] != null ? this.projectileTypes[index].ToString() : "none"));
          }
        }
        else
          result.Add("Parameter has to be an index [0, 255]");
      }
      else
        result.Add("Expected 1 parameter, received " + (object) parameters.Length);
    }
  }
}
