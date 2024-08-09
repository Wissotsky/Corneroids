// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockOctTree
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class BlockOctTree : OctTree<OctTreeNode<Block>, Block>
  {
    private static KeyValuePair<Vector3, byte>[] adjacentDirections = new KeyValuePair<Vector3, byte>[6]
    {
      new KeyValuePair<Vector3, byte>(Vector3.Up, (byte) 4),
      new KeyValuePair<Vector3, byte>(Vector3.Down, (byte) 8),
      new KeyValuePair<Vector3, byte>(Vector3.Left, (byte) 2),
      new KeyValuePair<Vector3, byte>(Vector3.Right, (byte) 1),
      new KeyValuePair<Vector3, byte>(Vector3.Backward, (byte) 16),
      new KeyValuePair<Vector3, byte>(Vector3.Forward, (byte) 32)
    };
    private IBlockOctTreeNode rootNode;

    public BlockOctTree(int leafSize, Vector3 min, Vector3 max)
      : base(leafSize, min, max)
    {
    }

    public void ConstructBlock(Vector3 position)
    {
      if (this.Bounds.Contains(position) != ContainmentType.Contains)
        return;
      int rootNodeSize = this.GetRootNodeSize();
      Vector3 octTreeCoordinates = this.ToOctTreeCoordinates(position);
      Byte3 sectorPosition = new Byte3((byte) ((double) octTreeCoordinates.X % (double) BlockSector.Size), (byte) ((double) octTreeCoordinates.Y % (double) BlockSector.Size), (byte) ((double) octTreeCoordinates.Z % (double) BlockSector.Size));
      this.rootNode.CreateBlock(rootNodeSize, ref octTreeCoordinates, sectorPosition);
    }

    public int GetNumberOfVertices(byte quality)
    {
      return this.rootNode != null ? this.rootNode.GetNumberOfVertices(this.GetRootNodeSize(), 1 << (int) quality) : 0;
    }

    public void GetReducedVerticesAndIndices(
      BlockVertex[] vertexArray,
      ref int vertexOffset,
      short[] indexArray,
      ref int indexOffset,
      byte quality)
    {
      if (this.rootNode == null)
        return;
      quality = quality > (byte) 4 ? (byte) 4 : quality;
      Byte3 position = new Byte3((byte) 0, (byte) 0, (byte) 0);
      byte num = (byte) (1U << (int) quality);
      BlockBuilder.Factory.ConstructBlock(out BlockOctTreeLinkNode.UnitBlockVertices, out BlockOctTreeLinkNode.UnitBlockIndices, new Byte3(num, num, num));
      int depth = SimpleMath.FastMax(SimpleMath.FastMin(4 - (int) quality, 4), 0);
      this.rootNode.GetVerticesAndIndices(vertexArray, ref vertexOffset, indexArray, ref indexOffset, (byte) depth, this.GetRootNodeSize(), position);
    }

    public bool IsBlocked(Vector3 position)
    {
      Block leaf = this.GetLeaf(position);
      return leaf != null && !leaf.Transparent;
    }

    public void ReconstructBlocks()
    {
      if (this.rootNode == null)
        return;
      Vector3 vector3 = this.Bounds.Min + Vector3.One * 0.5f;
      Vector3int vector3int = (Vector3int) (this.Bounds.Max - this.Bounds.Min);
      for (int z = 0; z < vector3int.Z; ++z)
      {
        for (int y = 0; y < vector3int.Y; ++y)
        {
          for (int x = 0; x < vector3int.X; ++x)
          {
            Vector3 position = vector3 + new Vector3((float) x, (float) y, (float) z);
            if (this.GetLeafNode(position) is BlockCell leafNode && leafNode.X == (byte) 0 && leafNode.Y == (byte) 0 && leafNode.Z == (byte) 0)
              this.ConstructBlock(position);
          }
        }
      }
    }

    public void UpdatePowerBlocks()
    {
      if (this.rootNode == null)
        return;
      Byte3 sectorPosition = new Byte3((byte) 0, (byte) 0, (byte) 0);
      this.rootNode.CreateBlocksOfType(this.GetRootNodeSize(), sectorPosition, typeof (PowerBlockType));
    }

    protected override OctTreeNode<Block> CreateRootNode(Block data)
    {
      BlockOctTreeLinkNode rootNode = new BlockOctTreeLinkNode();
      this.rootNode = (IBlockOctTreeNode) rootNode;
      return (OctTreeNode<Block>) rootNode;
    }

    public enum BlockSize : byte
    {
      x16,
      x8,
      x4,
      x2,
      x1,
    }
  }
}
