// Decompiled with JetBrains decompiler
// Type: CornerSpace.AsteroidBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class AsteroidBlockType : BasicBlockType
  {
    private AsteroidBlockType.Type type;

    public override Block CreateBlock() => (Block) new AsteroidBlock(this);

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      switch (instance.ReadElementValue<string>(element, new XmlReader.ConvertValue<string>(instance.ReadString), "light", "asteroidType").ToLower())
      {
        case "light":
          this.type = AsteroidBlockType.Type.Light;
          break;
        case "medium":
          this.type = AsteroidBlockType.Type.Medium;
          break;
        case "heavy":
          this.type = AsteroidBlockType.Type.Heavy;
          break;
        default:
          this.type = AsteroidBlockType.Type.Light;
          break;
      }
    }

    public AsteroidBlockType.Type AsteroidType => this.type;

    public enum Type : byte
    {
      Light,
      Medium,
      Heavy,
    }
  }
}
