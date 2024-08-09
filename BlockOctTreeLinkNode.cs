// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockOctTreeLinkNode
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class BlockOctTreeLinkNode : OctTreeLinkNode<Block>, IBlockOctTreeNode
  {
    private const byte NOBLOCK = 255;
    public static BlockVertex[] UnitBlockVertices;
    public static short[] UnitBlockIndices;

    public void CreateBlock(
      int nodeSize,
      ref Vector3 positionInTreeCoordinates,
      Byte3 sectorPosition)
    {
      int childIndex = this.GetChildIndex(nodeSize, ref positionInTreeCoordinates);
      int nodeSize1 = nodeSize / 2;
      if (this.ChildNodes[childIndex] == null)
        return;
      ((IBlockOctTreeNode) this.ChildNodes[childIndex]).CreateBlock(nodeSize1, ref positionInTreeCoordinates, sectorPosition);
    }

    public void CreateBlocksOfType(int nodeSize, Byte3 sectorZeroPosition, System.Type type)
    {
      int nodeSize1 = nodeSize / 2;
      for (int index = 0; index < this.ChildNodes.Length; ++index)
      {
        if (this.ChildNodes[index] != null)
        {
          Byte3 sectorPosition = sectorZeroPosition + new Byte3(index % 2 == 0 ? (byte) 0 : (byte) nodeSize1, index / 2 % 2 == 0 ? (byte) 0 : (byte) nodeSize1, index / 4 == 0 ? (byte) 0 : (byte) nodeSize1);
          ((IBlockOctTreeNode) this.ChildNodes[index]).CreateBlocksOfType(nodeSize1, sectorPosition, type);
        }
      }
    }

    public void CreateVerticesAndIndices(Byte3 positionInSectorCoordinates)
    {
    }

    public int GetNumberOfVertices(int thisNodeSize, int blockSize)
    {
      int numberOfVertices = 0;
      if (thisNodeSize <= blockSize)
        return this.GetNumberOfVertices(thisNodeSize, 1) <= 0 ? 0 : 24;
      for (int index = 0; index < this.ChildNodes.Length; ++index)
      {
        if (this.ChildNodes[index] != null)
          numberOfVertices += ((IBlockOctTreeNode) this.ChildNodes[index]).GetNumberOfVertices(thisNodeSize >> 1, blockSize);
      }
      return numberOfVertices;
    }

    public void GetVerticesAndIndices(
      BlockVertex[] vertices,
      ref int vertexOffset,
      short[] indices,
      ref int indexOffset,
      byte levels,
      int nodeSize,
      Byte3 position)
    {
      if (levels == (byte) 0)
      {
        if (this.GetNumberOfVertices(nodeSize, 1) <= 0 || vertices.Length < vertexOffset + BlockOctTreeLinkNode.UnitBlockVertices.Length || indices.Length < indexOffset + BlockOctTreeLinkNode.UnitBlockIndices.Length)
          return;
        Array.Copy((Array) BlockOctTreeLinkNode.UnitBlockVertices, 0, (Array) vertices, vertexOffset, BlockOctTreeLinkNode.UnitBlockVertices.Length);
        for (int index = 0; index < BlockOctTreeLinkNode.UnitBlockVertices.Length; ++index)
          vertices[vertexOffset + index].Position += 8 * position;
        for (int index = 0; index < BlockOctTreeLinkNode.UnitBlockIndices.Length; ++index)
          indices[indexOffset + index] = (short) ((int) BlockOctTreeLinkNode.UnitBlockIndices[index] + vertexOffset);
        vertexOffset += BlockOctTreeLinkNode.UnitBlockVertices.Length;
        indexOffset += BlockOctTreeLinkNode.UnitBlockIndices.Length;
      }
      else
      {
        int nodeSize1 = nodeSize / 2;
        byte depth = (byte) ((uint) levels - 1U);
        for (int index = 0; index < this.ChildNodes.Length; ++index)
        {
          if (this.ChildNodes[index] != null)
          {
            Byte3 position1 = position + new Byte3((byte) ((index % 2 == 0 ? 0 : 1) * nodeSize1), (byte) ((index / 2 % 2 == 0 ? 0 : 1) * nodeSize1), (byte) ((index / 4 == 0 ? 0 : 1) * nodeSize1));
            ((IBlockOctTreeNode) this.ChildNodes[index]).GetVerticesAndIndices(vertices, ref vertexOffset, indices, ref indexOffset, depth, nodeSize1, position1);
          }
        }
      }
    }

    public bool SetVisibility(
      ref Vector3 positionInTreeCoordinates,
      int nodeSize,
      byte visible,
      byte nonvisible)
    {
      int childIndex = this.GetChildIndex(nodeSize, ref positionInTreeCoordinates);
      return this.ChildNodes[childIndex] != null && ((IBlockOctTreeNode) this.ChildNodes[childIndex]).SetVisibility(ref positionInTreeCoordinates, nodeSize / 2, visible, nonvisible);
    }

    protected override OctTreeNode<Block> CreateNewLeafNode(Block data)
    {
      BlockCell newLeafNode = new BlockCell();
      newLeafNode.Data = data;
      return (OctTreeNode<Block>) newLeafNode;
    }

    protected override OctTreeNode<Block> CreateNewLinkNode()
    {
      return (OctTreeNode<Block>) new BlockOctTreeLinkNode();
    }

    private enum BitMask : byte
    {
      VISIBILITY = 63, // 0x3F
      UNIFORM = 64, // 0x40
    }
  }
}
