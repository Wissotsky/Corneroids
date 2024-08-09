// Decompiled with JetBrains decompiler
// Type: CornerSpace.TextureAtlas
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public abstract class TextureAtlas : IDisposable
  {
    protected Color[] colorTable;
    protected string key;
    protected string name;
    protected Texture2D texture;
    protected float textureUnitSize;
    protected int textureUnitSizePixels;
    private Dictionary<string, Rectangle> definedSpriteCoors = new Dictionary<string, Rectangle>();

    public TextureAtlas()
    {
    }

    public TextureAtlas(Texture2D texture, int textureUnitSizePixels)
    {
      if (texture == null || textureUnitSizePixels <= 0)
        throw new ArgumentException();
      this.texture = this.ValidateTextureSize(texture, textureUnitSizePixels) ? texture : throw new ArgumentException();
      this.textureUnitSizePixels = textureUnitSizePixels;
      this.textureUnitSize = (float) this.textureUnitSizePixels / (float) texture.Width;
    }

    public virtual void Dispose()
    {
      if (this.texture == null || this.texture.IsDisposed)
        return;
      this.texture.Dispose();
    }

    public Rectangle GetSpriteCoordinates(string type)
    {
      return this.definedSpriteCoors.ContainsKey(type) ? this.definedSpriteCoors[type] : new Rectangle(0, 0, 32, 32);
    }

    public Vector4 GetSpriteCoordinatesUV(string type)
    {
      if (!this.definedSpriteCoors.ContainsKey(type))
        return new Vector4(0.0f, 0.0f, 1f, 1f) * this.textureUnitSize;
      Rectangle definedSpriteCoor = this.definedSpriteCoors[type];
      return new Vector4((float) (definedSpriteCoor.X / 32) * this.textureUnitSize, (float) (definedSpriteCoor.Y / 32) * this.textureUnitSize, (float) (definedSpriteCoor.Width / 32) * this.textureUnitSize, (float) (definedSpriteCoor.Height / 32) * this.textureUnitSize);
    }

    public virtual bool ParseFromXml(XElement atlasElement)
    {
      try
      {
        XmlReader instance = XmlReader.Instance;
        this.name = instance.ReadElementValue<string>(atlasElement, new XmlReader.ConvertValue<string>(instance.ReadString), "noname", "name");
        this.key = instance.ReadElementValue<string>(atlasElement, new XmlReader.ConvertValue<string>(instance.ReadString), "key", "key");
        XElement xelement1 = atlasElement.Element((XName) "texture");
        XElement xelement2 = atlasElement.Element((XName) "blockSize");
        string path2 = xelement1.Value.ToString();
        this.texture = Engine.LoadTexture(Path.Combine(Engine.ContentManager.RootDirectory, path2), this.GenerateMipmaps);
        this.textureUnitSizePixels = Convert.ToInt32(xelement2.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        if (this.texture == null)
          throw new Exception("Failed to load texture " + path2);
        if (this.ValidateTextureUnitSize(this.textureUnitSizePixels) && this.ValidateTextureSize(this.texture, this.textureUnitSizePixels))
        {
          this.textureUnitSize = (float) this.textureUnitSizePixels / (float) this.texture.Width;
          this.ParseSprites(atlasElement);
        }
        return true;
      }
      catch (Exception ex)
      {
        this.Dispose();
        Engine.Console.WriteErrorLine("Failed to load a block texture atlas: " + ex.Message);
        return false;
      }
    }

    public string Key => this.key;

    public string Name => this.name;

    public Texture2D Texture => this.texture;

    public float TextureUnitSize => this.textureUnitSize;

    public int TextureUnitSizePixels => this.textureUnitSizePixels;

    protected abstract bool GenerateMipmaps { get; }

    private void ParseSprites(XElement element)
    {
      if (element == null)
        return;
      foreach (XElement element1 in element.Elements((XName) "sprite"))
      {
        try
        {
          string key = element1.Attribute((XName) "type").Value;
          Rectangle rectangle = new Rectangle(Convert.ToInt32(element1.Attribute((XName) "x").Value, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(element1.Attribute((XName) "y").Value, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(element1.Attribute((XName) "w").Value, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(element1.Attribute((XName) "h").Value, (IFormatProvider) CultureInfo.InvariantCulture));
          if (!this.definedSpriteCoors.ContainsKey(key))
            this.definedSpriteCoors.Add(key, rectangle);
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to parse coordinates for a sprite: " + ex.Message);
        }
      }
    }

    private bool ValidateTextureSize(Texture2D texture, int unitSize)
    {
      if (texture == null || unitSize <= 0)
        return false;
      int width = texture.Width;
      int height = texture.Height;
      return width == height && width >= unitSize && height >= unitSize && SimpleMath.IsPowerOfTwo(width) && SimpleMath.IsPowerOfTwo(height) && width % unitSize == 0 && height % unitSize == 0;
    }

    private bool ValidateTextureUnitSize(int unitSize) => unitSize > 0;
  }
}
