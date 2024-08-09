// Decompiled with JetBrains decompiler
// Type: CornerSpace.ContainerBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class ContainerBlockType : BasicBlockType
  {
    private const byte maxWidth = 7;
    private const byte maxHeight = 5;
    private byte width;
    private byte height;

    public override Block CreateBlock() => (Block) new ContainerBlock(this);

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.width = instance.ReadElementValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 1, "container", "width");
      this.height = instance.ReadElementValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 1, "container", "height");
      this.width = Math.Min(Math.Max((byte) 1, this.width), (byte) 7);
      this.height = Math.Min(Math.Max((byte) 1, this.height), (byte) 5);
    }

    public byte Width => this.width;

    public byte Height => this.height;
  }
}
