// Decompiled with JetBrains decompiler
// Type: CornerSpace.PointLightBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class PointLightBlockType : LightBlockType
  {
    public override Block CreateBlock() => (Block) new PointLightBlock(this);

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      this.Radius = Convert.ToSingle(element.Element((XName) "light").Element((XName) "radius").Value, (IFormatProvider) CultureInfo.InvariantCulture);
      this.Radius = MathHelper.Clamp(this.Radius, 0.0f, 32f);
    }
  }
}
