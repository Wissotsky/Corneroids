// Decompiled with JetBrains decompiler
// Type: CornerSpace.LightingItem
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class LightingItem : ItemWithModel
  {
    private Color color;
    private LightSource light;
    private Vector3 lightOffset;
    private bool lightOn;
    private float radius;
    private float radiusOffset;

    public LightingItem()
    {
      this.light = (LightSource) new PointLight(Color.White, 0.0f);
      this.lightOffset = Vector3.Zero;
      this.lightOn = false;
    }

    public override Item Copy()
    {
      LightingItem lightingItem = base.Copy() as LightingItem;
      lightingItem.light = this.light.Copy();
      return (Item) lightingItem;
    }

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Color radius: " + this.radius.ToString());
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      if (element.Element((XName) "light") == null)
        return;
      string str = instance.ReadAttributeValue<string>(element, new XmlReader.ConvertValue<string>(instance.ReadString), "type", "point", "light");
      this.radius = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 1f, "light", "radius");
      this.color = new Color(instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "r", (byte) 0, "light", "color"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "g", (byte) 0, "light", "color"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "b", (byte) 0, "light", "color"));
      switch (str)
      {
        case "point":
          this.light = (LightSource) new PointLight(this.color, this.radius);
          break;
        case "spot":
          this.light = (LightSource) new SpotLight(this.color, instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 1f, "light", "length"), this.radius, Vector3.Up);
          break;
      }
    }

    public override void RenderLights()
    {
      base.RenderLights();
      if (!this.lightOn || Item.lightingManager == null || this.light == null)
        return;
      Item.lightingManager.DrawLight(this.light);
    }

    public override void Update(Astronaut owner)
    {
      base.Update(owner);
      if (this.light == null)
        return;
      this.light.Position = owner.Position + this.lightOffset;
      this.light.Radius = this.radius + this.radiusOffset;
    }

    public Color Color
    {
      get => this.color;
      set => this.color = value;
    }

    public LightSource Light
    {
      get => this.light;
      set
      {
        this.light = value;
        if (this.light == null)
          return;
        this.radius = this.light.Radius;
      }
    }

    public Vector3 LightOffset
    {
      set => this.lightOffset = value;
    }

    public bool LightOn
    {
      get => this.lightOn;
      set => this.lightOn = value;
    }

    public float LightRadiusOffset
    {
      set => this.radiusOffset = value;
    }
  }
}
