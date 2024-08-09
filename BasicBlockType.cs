// Decompiled with JetBrains decompiler
// Type: CornerSpace.BasicBlockType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class BasicBlockType : BlockType
  {
    private BlockTextureCoordinates blockTextures;

    public override Block CreateBlock() => (Block) new BasicBlock(this);

    public override BlockTextureCoordinates GetTextureCoordinates() => this.blockTextures;

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      this.blockTextures = this.ParseTextureCoordinates(element.Element((XName) "activeTextures"));
    }

    public BlockTextureCoordinates BlockTextures
    {
      get => this.blockTextures;
      set => this.blockTextures = value;
    }
  }
}
