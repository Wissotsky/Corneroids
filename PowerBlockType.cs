// Decompiled with JetBrains decompiler
// Type: CornerSpace.PowerBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class PowerBlockType : BlockType
  {
    private int power;
    private BlockTextureCoordinates activeTexturePosition;
    private BlockTextureCoordinates deactiveTexturePosition;

    public PowerBlockType()
    {
      this.activeTexturePosition = new BlockTextureCoordinates(Rectangle.Empty, Rectangle.Empty, Rectangle.Empty);
      this.deactiveTexturePosition = new BlockTextureCoordinates(Rectangle.Empty, Rectangle.Empty, Rectangle.Empty);
    }

    public override Block CreateBlock() => (Block) new PowerBlock(this);

    public override BlockTextureCoordinates GetTextureCoordinates() => this.activeTexturePosition;

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Power: " + (object) this.power);
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.power = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "power");
      XElement textEle1 = element.Element((XName) "activeTextures");
      XElement textEle2 = element.Element((XName) "deactiveTextures");
      this.activeTexturePosition = this.ParseTextureCoordinates(textEle1);
      this.deactiveTexturePosition = this.ParseTextureCoordinates(textEle2);
    }

    public BlockTextureCoordinates ActiveBlockTextures
    {
      get => this.activeTexturePosition;
      set => this.activeTexturePosition = value;
    }

    public BlockTextureCoordinates DeactiveTexturePosition
    {
      get => this.deactiveTexturePosition;
      set => this.deactiveTexturePosition = value;
    }

    public int Power
    {
      get => this.power;
      set => this.power = value;
    }
  }
}
