// Decompiled with JetBrains decompiler
// Type: CornerSpace.MineralBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class MineralBlockType : AsteroidBlockType
  {
    private short rarity;

    public override Block CreateBlock() => (Block) new MineralBlock(this);

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.rarity = instance.ReadElementValue<short>(element, new XmlReader.ConvertValue<short>(instance.ReadShort), (short) 1, "rarity");
      this.rarity = (short) MathHelper.Clamp((float) this.rarity, 0.0f, 999f);
    }

    public short Rarity => this.rarity;
  }
}
