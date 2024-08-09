// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpriteTextureAtlas
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public class SpriteTextureAtlas : TextureAtlas
  {
    public const string tDefaultDrill = "defaultDrill";

    public SpriteTextureAtlas()
    {
    }

    public SpriteTextureAtlas(Texture2D texture, int textureUnitSizePixels)
      : base(texture, textureUnitSizePixels)
    {
    }

    protected override bool GenerateMipmaps => false;
  }
}
