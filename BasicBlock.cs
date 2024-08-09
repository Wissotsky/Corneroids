// Decompiled with JetBrains decompiler
// Type: CornerSpace.BasicBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class BasicBlock : Block
  {
    public BasicBlock(BasicBlockType creationParameters)
      : base((BlockType) creationParameters)
    {
    }

    public override BlockType GetBlockType()
    {
      return Engine.LoadedWorld.Itemset.BlockTypes[(int) this.blockType];
    }

    public override BlockTextureCoordinates GetTextureCoordinates()
    {
      return ((BasicBlockType) this.GetBlockType()).BlockTextures;
    }
  }
}
