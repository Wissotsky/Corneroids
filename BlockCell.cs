// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockCell
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class BlockCell : OctTreeLeafNode<Block>, IBlockOctTreeNode
  {
    public BlockVertex[] Vertices;
    public short[] Indices;
    private ushort offsetData;

    public override void Clear()
    {
      this.Indices = (short[]) null;
      this.Vertices = (BlockVertex[]) null;
    }

    public void CreateBlock(
      int nodeSize,
      ref Vector3 positionInTreeCoordinates,
      Byte3 sectorPosition)
    {
      this.CreateVerticesAndIndices(sectorPosition);
    }

    public void CreateBlocksOfType(int nodeSize, Byte3 sectorPosition, System.Type type)
    {
      BlockType blockType = this.Block.GetBlockType();
      if (this.X != (byte) 0 || this.Y != (byte) 0 || this.Z != (byte) 0 || !type.IsInstanceOfType((object) blockType))
        return;
      this.CreateVerticesAndIndices(sectorPosition);
    }

    public void CreateVerticesAndIndices(Byte3 positionInSectorCoordinates)
    {
      if (this.Data.HasDynamicBehavior || this.X > (byte) 0 || this.Y > (byte) 0 || this.Z > (byte) 0)
        return;
      Byte3 position = positionInSectorCoordinates - new Byte3(this.X, this.Y, this.Z);
      if (this.Data.ModelPlacement != Vector3.Zero)
        position = (Byte3) ((Vector3) position + 0.5f * (Vector3) this.Data.Size + this.Data.ModelPlacement - 0.5f * (Vector3) this.Data.ModelSize + 0.5f * Vector3.One);
      this.Data.CreateBlock(position, out this.Vertices, out this.Indices);
    }

    public int GetNumberOfVertices(int thisNodeSize, int blockSize)
    {
      return this.Vertices == null ? 0 : this.Vertices.Length;
    }

    public void GetVerticesAndIndices(
      BlockVertex[] vertexArray,
      ref int vertexOffset,
      short[] indexArray,
      ref int indexOffset,
      byte levels,
      int nodeSize,
      Byte3 position)
    {
      if (this.Vertices == null || this.Indices == null || vertexArray.Length < vertexOffset + this.Vertices.Length || indexArray.Length < indexOffset + this.Indices.Length)
        return;
      Array.Copy((Array) this.Vertices, 0, (Array) vertexArray, vertexOffset, this.Vertices.Length);
      for (int index = 0; index < this.Indices.Length; ++index)
        indexArray[indexOffset + index] = (short) ((int) this.Indices[index] + vertexOffset);
      vertexOffset += this.Vertices.Length;
      indexOffset += this.Indices.Length;
    }

    public bool SetVisibility(
      ref Vector3 positionInTreeCoordinates,
      int nodeSize,
      byte visible,
      byte nonvisible)
    {
        byte visibility = this.Data.Visibility;
        this.Data.Visibility = visible != (byte)0 ? (byte)(this.Data.Visibility | visible) : (byte)(this.Data.Visibility & ~nonvisible);
        return visibility != this.Data.Visibility;
    }

    public Block Block => this.Data;

    public byte X
    {
      get => (byte) ((uint) this.offsetData & 31U);
      set
      {
        this.offsetData &= (ushort) 65504;
        this.offsetData |= (ushort) ((uint) value & 31U);
      }
    }

    public byte Y
    {
      get => (byte) (((int) this.offsetData & 992) >> 5);
      set
      {
        this.offsetData &= (ushort) 64543;
        this.offsetData |= (ushort) (((int) value & 31) << 5);
      }
    }

    public byte Z
    {
      get => (byte) (((int) this.offsetData & 31744) >> 10);
      set
      {
        this.offsetData &= (ushort) 33791;
        this.offsetData |= (ushort) (((int) value & 31) << 10);
      }
    }

    private enum BitMask : byte
    {
      VISIBILITY = 63, // 0x3F
      UNIFORM = 64, // 0x40
    }
  }
}
