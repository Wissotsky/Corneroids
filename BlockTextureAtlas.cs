// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockTextureAtlas
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class BlockTextureAtlas : TextureAtlas
  {
    private Texture2D lightValuesTexture;

    public BlockTextureAtlas()
    {
    }

    public BlockTextureAtlas(
      Texture2D blockTexture,
      Texture2D lightTexture,
      int textureUnitSizePixels)
      : base(blockTexture, textureUnitSizePixels)
    {
      this.lightValuesTexture = lightTexture;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.lightValuesTexture == null)
        return;
      this.lightValuesTexture.Dispose();
    }

    public override bool ParseFromXml(XElement atlasElement)
    {
      if (!base.ParseFromXml(atlasElement))
        return false;
      try
      {
        string str = atlasElement.Element((XName) "lightTexture").Value.ToString();
        this.lightValuesTexture = Engine.LoadTexture(Engine.ContentManager.RootDirectory + "/" + str, false);
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load light block texture atlas. " + ex.Message);
        return true;
      }
    }

    public Texture2D LightValuesTexture => this.lightValuesTexture;

    protected override bool GenerateMipmaps => true;
  }
}
