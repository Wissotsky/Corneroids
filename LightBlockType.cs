// Decompiled with JetBrains decompiler
// Type: CornerSpace.LightBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public abstract class LightBlockType : PowerBlockType
  {
    private Color lightColor;
    private float radius;

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Radius: " + (object) this.radius + " units");
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XElement xelement = element.Element((XName) "light").Element((XName) "color");
      this.lightColor = new Color(Convert.ToByte(xelement.Attribute((XName) "r").Value, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToByte(xelement.Attribute((XName) "g").Value, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToByte(xelement.Attribute((XName) "b").Value, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public Color LightColor
    {
      get => this.lightColor;
      set => this.lightColor = value;
    }

    public float Radius
    {
      get => this.radius;
      set => this.radius = value;
    }
  }
}
