// Decompiled with JetBrains decompiler
// Type: CornerSpace.TorchItem
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class TorchItem : LightingItem
  {
    private const float cTossSpeed = 7f;
    private int burningTimeTotalMS;

    public TorchItem() => this.LightOn = true;

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Torch time: " + ((float) this.burningTimeTotalMS / 1000f).ToString() + " s");
    }

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      if (!input.LeftClick || Item.environmentManager == null || (double) powerToUse < (double) this.PowerUsage)
        return Item.UsageResult.None;
      TorchItem torch = this.Copy();
      Position3 position = owner.Astronaut.Position;
      Vector3 speed = owner.Astronaut.GetLookatVector() * 7f;
      Item.environmentManager.TossTorch(torch, position, speed);
      return Item.UsageResult.Consumed | Item.UsageResult.Power_used;
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.burningTimeTotalMS = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 10000, "burningTime");
      this.burningTimeTotalMS = Math.Max(this.burningTimeTotalMS, 0);
    }

    public int BurningTime => this.burningTimeTotalMS;

    private TorchItem Copy()
    {
      TorchItem torchItem = this.MemberwiseClone() as TorchItem;
      torchItem.Light = (LightSource) new PointLight(this.Light.Color, this.Light.Radius);
      return torchItem;
    }
  }
}
