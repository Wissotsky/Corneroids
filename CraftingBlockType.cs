// Decompiled with JetBrains decompiler
// Type: CornerSpace.CraftingBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class CraftingBlockType : BasicBlockType
  {
    private CraftingBlockType.Type type;

    public override Block CreateBlock() => (Block) new CraftingBlock(this);

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      switch (instance.ReadAttributeValue<string>(element, new XmlReader.ConvertValue<string>(instance.ReadString), "type", string.Empty, (string[]) null))
      {
        case "craftingtable":
          this.type = CraftingBlockType.Type.CraftingTable;
          break;
        case "smelter":
          this.type = CraftingBlockType.Type.Smelter;
          break;
        case "extractor":
          this.type = CraftingBlockType.Type.Extractor;
          break;
        default:
          this.type = CraftingBlockType.Type.CraftingTable;
          break;
      }
    }

    public CraftingBlockType.Type UsageType => this.type;

    public enum Type
    {
      CraftingTable,
      Smelter,
      Extractor,
    }
  }
}
