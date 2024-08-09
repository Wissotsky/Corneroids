// Decompiled with JetBrains decompiler
// Type: CornerSpace.DoorBlockType
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
  public class DoorBlockType : BasicBlockType
  {
    private Vector3 openingDirection;
    private float openingDistance;
    private float openingSpeed;
    private int timeToRemainOpen;
    private Byte3 trueSize;

    public DoorBlockType()
    {
      this.openingDirection = Vector3.Zero;
      this.openingDistance = 0.0f;
      this.openingSpeed = 0.0f;
      this.timeToRemainOpen = 0;
    }

    public override Block CreateBlock() => (Block) new DoorBlock(this);

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Distance: " + (object) this.openingDistance);
      lines.Add("Speed: " + (object) this.openingSpeed);
      lines.Add("Time: " + ((float) this.timeToRemainOpen / 1000f).ToString() + " s");
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      element.Element((XName) "door");
      this.openingDistance = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 0.0f, "door", "distance");
      this.openingSpeed = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 0.0f, "door", "speed");
      this.timeToRemainOpen = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "door", "time");
      this.openingDirection = new Vector3(instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "x", 0.0f, "door", "direction"), instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "y", 1f, "door", "direction"), instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "z", 0.0f, "door", "direction"));
      if (this.openingDirection == Vector3.Zero)
        this.openingDirection = Vector3.Up;
      else
        this.openingDirection.Normalize();
      this.timeToRemainOpen = Math.Max(this.timeToRemainOpen, 0);
    }

    public Vector3 OpeningDirection
    {
      get => this.openingDirection;
      set => this.openingDirection = value;
    }

    public float OpeningDistance
    {
      get => this.openingDistance;
      set => this.openingDistance = value;
    }

    public float OpeningSpeed
    {
      get => this.openingSpeed;
      set => this.openingSpeed = value;
    }

    public int TimeToRemainOpen
    {
      get => this.timeToRemainOpen;
      set => this.timeToRemainOpen = value;
    }

    public Byte3 TrueSize => this.trueSize;
  }
}
