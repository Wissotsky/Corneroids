// Decompiled with JetBrains decompiler
// Type: CornerSpace.EngineBlockType
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
  public class EngineBlockType : PowerBlockType
  {
    private float throttleForce;

    public override Block CreateBlock() => (Block) new EngineBlock(this);

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Force: " + (object) this.throttleForce);
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      element.Element((XName) "thruster");
      this.throttleForce = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 0.0f, "thruster", "force");
      this.throttleForce = Math.Max(this.throttleForce, 0.0f);
    }

    public float ThrottleForce
    {
      get => this.throttleForce;
      set => this.throttleForce = value;
    }
  }
}
