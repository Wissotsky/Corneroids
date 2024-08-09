// Decompiled with JetBrains decompiler
// Type: CornerSpace.ControlBlockType
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
  public class ControlBlockType : PowerBlockType
  {
    private short numberOfKeys;

    public override Block CreateBlock() => (Block) new ControlBlock(this);

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Buttons: " + (object) this.numberOfKeys);
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.NumberOfKeys = instance.ReadElementValue<short>(element, new XmlReader.ConvertValue<short>(instance.ReadShort), (short) 0, "buttons");
      this.numberOfKeys = Math.Min(this.numberOfKeys, (short) 16);
      this.numberOfKeys = Math.Max((short) 1, this.numberOfKeys);
    }

    public short NumberOfKeys
    {
      get => this.numberOfKeys;
      set => this.numberOfKeys = Math.Max(value, (short) 1);
    }
  }
}
