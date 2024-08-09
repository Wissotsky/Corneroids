// Decompiled with JetBrains decompiler
// Type: CornerSpace.WearableItem
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class WearableItem : InventoryItem
  {
    private WearableItem.Slot slot;
    private int armorRating;
    private int healthBonus;
    private int rechargeRate;

    public void AddedToSuit(ISuitRatings suit)
    {
      suit.HealthRating += this.healthBonus;
      suit.ArmorRating += new CornerSpace.ArmorRating((uint) this.armorRating);
      suit.PowerRechargeRate += this.rechargeRate;
    }

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Armor rating: " + (object) this.armorRating);
      lines.Add("Health bonus: " + (object) this.healthBonus);
      lines.Add("Recharge bonus: " + (object) this.rechargeRate);
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      int num = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "slot");
      this.armorRating = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "armorRating");
      this.healthBonus = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "healthRating");
      this.rechargeRate = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "powerRechargeRate");
      this.slot = (WearableItem.Slot) num;
    }

    public void RemovedFromSuit(ISuitRatings suit)
    {
      suit.HealthRating -= this.healthBonus;
      suit.ArmorRating -= new CornerSpace.ArmorRating((uint) this.armorRating);
      suit.PowerRechargeRate -= this.rechargeRate;
    }

    public int ArmorRating => this.armorRating;

    public int HealthBonus => this.healthBonus;

    public WearableItem.Slot ItemSlot
    {
      get => this.slot;
      set => this.slot = value;
    }

    public enum Slot
    {
      Head,
      Shoulders,
      Chest,
      Legs,
      Hands,
      Feet,
      PowerSource,
    }
  }
}
