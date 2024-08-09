// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public abstract class BlockType : BlockItem
  {
    private bool transparent;
    private byte blockId;
    private Byte3 modelSize;
    private Vector3 modelPlacement;
    private byte powerToDrill;
    private Byte3 size = new Byte3((byte) 1, (byte) 1, (byte) 1);
    private byte maximumHealth = 10;
    private float mass = 1f;

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      if (nameOnly)
        return;
      lines.Add("mass: " + (object) this.mass);
      lines.Add("health: " + (object) this.maximumHealth);
      lines.Add("size: " + (object) this.modelSize.X + "x" + (object) this.modelSize.Y + "x" + (object) this.modelSize.Z);
    }

    public abstract BlockTextureCoordinates GetTextureCoordinates();

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.blockId = instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "blockId", (byte) 0, (string[]) null);
      this.mass = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 1f, "mass");
      this.maximumHealth = instance.ReadElementValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 1, "health");
      this.powerToDrill = instance.ReadElementValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 1, "powerToDrill");
      element.Element((XName) "dimensions");
      this.size = new Byte3(instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "w", (byte) 1, "dimensions", "size"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "h", (byte) 1, "dimensions", "size"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "d", (byte) 1, "dimensions", "size"));
      this.modelPlacement = new Vector3(instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "x", 0.0f, "dimensions", "modelPlacement"), instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "y", 0.0f, "dimensions", "modelPlacement"), instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "z", 0.0f, "dimensions", "modelPlacement"));
      this.transparent = this.modelPlacement != Vector3.Zero;
      this.mass = MathHelper.Clamp(this.mass, 0.1f, 100f);
      this.maximumHealth = Math.Max(this.maximumHealth, (byte) 1);
      this.powerToDrill = Math.Max(this.powerToDrill, (byte) 0);
      this.size = new Byte3(Math.Max(this.size.X, (byte) 1), Math.Max(this.size.Y, (byte) 1), Math.Max(this.size.Z, (byte) 1));
      this.modelSize = new Byte3(instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "w", (byte) 1, "dimensions", "modelSize"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "h", (byte) 1, "dimensions", "modelSize"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "d", (byte) 1, "dimensions", "modelSize"));
      if (!((Vector3) this.modelSize == Vector3.Zero))
        return;
      this.modelSize = this.size;
    }

    public void SetSpriteCoordsInBlockAtlas(Rectangle rect, Vector4 uv)
    {
      this.SpriteCoordsRect = rect;
      this.SpriteCoordsUV = uv;
    }

    public bool Transparent
    {
      get => this.transparent;
      set => this.transparent = value;
    }

    public byte BlockId
    {
      get => this.blockId;
      set => this.blockId = value;
    }

    public byte MaximumHealth
    {
      get => this.maximumHealth;
      set => this.maximumHealth = value;
    }

    public Vector3 ModelPlacement => this.modelPlacement;

    public Byte3 ModelSize => this.modelSize;

    public float Mass
    {
      get => this.mass;
      set => this.mass = value;
    }

    public byte PowerToDrill => this.powerToDrill;

    public Byte3 Size => this.size;

    public override Texture2D SpriteAtlasTexture
    {
      get
      {
        return Engine.LoadedWorld != null && Engine.LoadedWorld.BlockSpriteTextureAtlas != null ? Engine.LoadedWorld.BlockSpriteTextureAtlas.Texture : (Texture2D) null;
      }
    }

    public override Rectangle SpriteCoordsRect => this.spriteCoordsRect;

    public override Vector4 SpriteCoordsUV => this.spriteCoordsUV;

    protected BlockTextureCoordinates ParseTextureCoordinates(XElement textEle)
    {
      XmlReader instance = XmlReader.Instance;
      Rectangle walls = new Rectangle(instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "x", 0, "walls"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "y", 0, "walls"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "w", 0, "walls"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "h", 0, "walls"));
      Rectangle top = new Rectangle(instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "x", 0, "top"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "y", 0, "top"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "w", 0, "top"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "h", 0, "top"));
      Rectangle bottom = new Rectangle(instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "x", 0, "bottom"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "y", 0, "bottom"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "w", 0, "bottom"), instance.ReadAttributeValue<int>(textEle, new XmlReader.ConvertValue<int>(instance.ReadInt), "h", 0, "bottom"));
      return new BlockTextureCoordinates(walls, bottom, top);
    }
  }
}
