// Decompiled with JetBrains decompiler
// Type: CornerSpace.GunBlockType
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
  public class GunBlockType : PowerBlockType
  {
    private byte projectileId;
    private short reloadTime;
    private float barrelLength;

    public override Block CreateBlock() => (Block) new GunBlock(this);

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Reload time: " + (object) (float) ((double) this.reloadTime / 1000.0) + " s");
      ProjectileType projectileType = Engine.LoadedWorld.Itemset.ProjectileTypes[(int) this.projectileId];
      if (projectileType == null)
        return;
      lines.Add("Damage: " + (object) projectileType.Damage);
      lines.Add("Speed: " + (object) projectileType.Speed + " units/s");
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.reloadTime = instance.ReadElementValue<short>(element, new XmlReader.ConvertValue<short>(instance.ReadShort), (short) 1000, "reloadTime");
      this.barrelLength = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 1f, "barrelLength");
      this.projectileId = instance.ReadElementValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 0, "projectileId");
      this.Transparent = true;
      this.reloadTime = Math.Max(this.reloadTime, (short) 50);
      this.barrelLength = Math.Max((float) this.Size.Y * 0.5f, this.barrelLength);
    }

    public float BarrelLength => this.barrelLength;

    public byte ProjectileId => this.projectileId;

    public short ReloadTime => this.reloadTime;
  }
}
