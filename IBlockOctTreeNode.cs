// Decompiled with JetBrains decompiler
// Type: CornerSpace.IBlockOctTreeNode
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public interface IBlockOctTreeNode
  {
    void CreateBlock(int nodeSize, ref Vector3 positionInTreeCoordinates, Byte3 sectorPosition);

    void CreateBlocksOfType(int nodeSize, Byte3 sectorPosition, System.Type type);

    void CreateVerticesAndIndices(Byte3 positionInSectorCoordinates);

    int GetNumberOfVertices(int thisNodeSize, int blockSize);

    void GetVerticesAndIndices(
      BlockVertex[] vertices,
      ref int vertexOffset,
      short[] indices,
      ref int indexOffset,
      byte depth,
      int nodeSize,
      Byte3 position);

    bool SetVisibility(
      ref Vector3 positionInTreeCoordinates,
      int nodeSize,
      byte visible,
      byte nonvisible);
  }
}
