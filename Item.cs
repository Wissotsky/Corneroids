// Decompiled with JetBrains decompiler
// Type: CornerSpace.Item
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public abstract class Item : IBillboard
  {
    private const int cNameMaxLength = 30;
    private const int cDescriptionMaxLength = 50;
    private const float cPickMarginals = 0.15f;
    public const int NO_ID = -1;
    protected static SpaceEntityManager entityManager;
    protected static EnvironmentManager environmentManager;
    protected static LightingManager lightingManager;
    protected static ProjectileManager projectileManager;
    protected static Random random;
    private Color itemColor;
    private string description;
    private int id;
    private uint maxStackSize;
    private string name;
    private int powerUsage;
    private SpriteImage crosshair;
    protected Rectangle spriteCoordsRect;
    protected Vector4 spriteCoordsUV;

    public Item()
    {
      this.maxStackSize = 64U;
      if (Item.random == null)
        Item.random = new Random();
      this.crosshair = this.CreateCrosshair();
    }

    public virtual Item Copy() => this.MemberwiseClone() as Item;

    public virtual void DrawHelpTexts()
    {
    }

    public override bool Equals(object obj)
    {
      Item obj1 = obj as Item;
      return obj1 != (Item) null && this.id == obj1.id;
    }

    public virtual Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      return Item.UsageResult.None;
    }

    public virtual void GetStatistics(List<string> lines, bool nameOnly) => lines?.Add(this.name);

    public virtual void LoadFromXml(XElement element)
    {
      XmlReader instance = XmlReader.Instance;
      this.description = instance.ReadElementValue<string>(element, new XmlReader.ConvertValue<string>(instance.ReadString), "none", "description");
      this.name = instance.ReadElementValue<string>(element, new XmlReader.ConvertValue<string>(instance.ReadString), "none", "name");
      this.id = instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "itemId", 0, (string[]) null);
      this.maxStackSize = (uint) instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 64, "maxStackSize");
      this.powerUsage = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "powerUsage");
      this.spriteCoordsRect = new Rectangle(instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "x", 0, "sprite"), instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "y", 0, "sprite"), 1, 1);
      this.itemColor = new Color(instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "r", (byte) 0, "color"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "g", (byte) 0, "color"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "b", (byte) 0, "color"));
      this.spriteCoordsUV = new Vector4((float) this.spriteCoordsRect.X, (float) this.spriteCoordsRect.Y, 1f, 1f);
      if (this.description.Length > 50)
        this.description = this.description.Substring(0, 50);
      if (this.name.Length <= 30)
        return;
      this.name = this.name.Substring(0, 30);
    }

    public virtual void RenderFirstPerson(IRenderCamera camera)
    {
    }

    public virtual void RenderFirstPersonWithLighting(IRenderCamera camera)
    {
    }

    public virtual void RenderLights()
    {
    }

    public virtual void RenderThirdPersonWithLighting(IRenderCamera camera, Matrix holderMatrix)
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Update(Astronaut owner)
    {
    }

    public static bool operator ==(Item item1, Item item2)
    {
      if (!object.ReferenceEquals((object) item1, (object) null))
        return item1.Equals((object) item2);
      return object.ReferenceEquals((object) item2, (object) null) || item2.Equals((object) item1);
    }

    public static bool operator !=(Item item1, Item item2) => !(item1 == item2);

    public static SpaceEntityManager EntityManager
    {
      set => Item.entityManager = value;
    }

    public static EnvironmentManager EnvironmentManager
    {
      set => Item.environmentManager = value;
    }

    public static LightingManager LightingManager
    {
      set => Item.lightingManager = value;
    }

    public virtual ItemPreviewLayer PreviewLayer => (ItemPreviewLayer) null;

    public static ProjectileManager ProjectileManager
    {
      set => Item.projectileManager = value;
    }

    public SpriteImage Crosshair
    {
      get => this.crosshair;
      protected set => this.crosshair = value;
    }

    public virtual byte SynchronizationValue
    {
      set
      {
      }
    }

    public string Description
    {
      get => this.description;
      set => this.description = value;
    }

    public Color ItemColor => this.itemColor;

    public int ItemId
    {
      get => this.id;
      set => this.id = value;
    }

    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    public int PowerUsage => this.powerUsage;

    public virtual Texture2D SpriteAtlasTexture
    {
      get
      {
        return Engine.LoadedWorld != null && Engine.LoadedWorld.SpriteTextureAtlas != null ? Engine.LoadedWorld.SpriteTextureAtlas.Texture : (Texture2D) null;
      }
    }

    public virtual Rectangle SpriteCoordsRect
    {
      get
      {
        World loadedWorld = Engine.LoadedWorld;
        if (loadedWorld != null)
        {
          SpriteTextureAtlas spriteTextureAtlas = loadedWorld.SpriteTextureAtlas;
          if (spriteTextureAtlas != null)
          {
            int textureUnitSizePixels = spriteTextureAtlas.TextureUnitSizePixels;
            return new Rectangle(this.spriteCoordsRect.X * textureUnitSizePixels, this.spriteCoordsRect.Y * textureUnitSizePixels, this.spriteCoordsRect.Width * textureUnitSizePixels, this.spriteCoordsRect.Height * textureUnitSizePixels);
          }
        }
        return this.spriteCoordsRect;
      }
      protected set => this.spriteCoordsRect = value;
    }

    public virtual Vector4 SpriteCoordsUV
    {
      get
      {
        World loadedWorld = Engine.LoadedWorld;
        if (loadedWorld != null)
        {
          SpriteTextureAtlas spriteTextureAtlas = loadedWorld.SpriteTextureAtlas;
          if (spriteTextureAtlas != null)
          {
            float textureUnitSize = spriteTextureAtlas.TextureUnitSize;
            return new Vector4(this.spriteCoordsUV.X * textureUnitSize, this.spriteCoordsUV.Y * textureUnitSize, this.spriteCoordsUV.Z * textureUnitSize, this.spriteCoordsUV.W * textureUnitSize);
          }
        }
        return this.spriteCoordsUV;
      }
      protected set => this.spriteCoordsUV = value;
    }

    public uint MaxStackSize
    {
      get => this.maxStackSize;
      set => this.maxStackSize = value;
    }

    protected Vector3 ApplyPositionMarginals(Vector3 position, Vector3 previousPosition)
    {
      Vector3int vector3int1 = (Vector3int) position;
      Vector3int vector3int2 = (Vector3int) previousPosition;
      return (vector3int1.X != vector3int2.X || vector3int1.Y != vector3int2.Y || vector3int1.Z != vector3int2.Z) && ((double) position.X <= (double) previousPosition.X - 0.15000000596046448 || (double) position.X >= (double) previousPosition.X + 0.15000000596046448 || (double) position.Y <= (double) previousPosition.Y - 0.15000000596046448 || (double) position.Y >= (double) previousPosition.Y + 0.15000000596046448 || (double) position.Z <= (double) previousPosition.Z - 0.15000000596046448 || (double) position.Z >= (double) previousPosition.Z + 0.15000000596046448) ? position : previousPosition;
    }

    protected void DrawHelpString(string text)
    {
      if (string.IsNullOrEmpty(text))
        return;
      Engine.SpriteBatch.Begin();
      Vector2 vector2 = Engine.Font.MeasureString(text);
      Vector2 position = new Vector2((float) (Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - vector2.X / 2f, (float) (Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 + 50));
      Engine.SpriteBatch.DrawString(Engine.Font, text, position, Color.White);
      Engine.SpriteBatch.End();
    }

    protected virtual SpriteImage CreateCrosshair() => (SpriteImage) null;

    public enum UsageResult
    {
      None,
      Consumed,
      Power_used,
    }
  }
}
