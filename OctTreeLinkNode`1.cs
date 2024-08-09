// Decompiled with JetBrains decompiler
// Type: CornerSpace.OctTreeLinkNode`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class OctTreeLinkNode<T> : OctTreeNode<T>
  {
    public OctTreeNode<T>[] ChildNodes;

    public OctTreeLinkNode() => this.ChildNodes = new OctTreeNode<T>[8];

    public override T AddLeaf(
      T newLeaf,
      int nodeSize,
      int leafSize,
      ref Vector3 positionInTreeCoordinates)
    {
      int childIndex = this.GetChildIndex(nodeSize, ref positionInTreeCoordinates);
      int nodeSize1 = nodeSize / 2;
      if (this.ChildNodes[childIndex] != null)
        return this.ChildNodes[childIndex].AddLeaf(newLeaf, nodeSize1, leafSize, ref positionInTreeCoordinates);
      if (nodeSize1 == leafSize)
      {
        this.ChildNodes[childIndex] = this.CreateNewLeafNode(newLeaf);
        return newLeaf;
      }
      if (nodeSize1 <= leafSize)
        return default (T);
      this.ChildNodes[childIndex] = this.CreateNewLinkNode();
      return this.ChildNodes[childIndex].AddLeaf(newLeaf, nodeSize1, leafSize, ref positionInTreeCoordinates);
    }

    public override void AddNode(
      OctTreeNode<T> node,
      int sizeOfNodeToAdd,
      int nodeSize,
      ref Vector3 positionInTreeCoordinates)
    {
      int childIndex = this.GetChildIndex(nodeSize, ref positionInTreeCoordinates);
      int nodeSize1 = nodeSize / 2;
      if (nodeSize1 == sizeOfNodeToAdd)
      {
        this.ChildNodes[childIndex] = node;
      }
      else
      {
        this.ChildNodes[childIndex] = (OctTreeNode<T>) new OctTreeLinkNode<T>();
        this.ChildNodes[childIndex].AddNode(node, sizeOfNodeToAdd, nodeSize1, ref positionInTreeCoordinates);
      }
    }

    public override Vector3 GetClosestNode(Vector3 position, Vector3 nodePosition, int nodeSize)
    {
      bool flag = false;
      Vector3 nodePosition1 = Vector3.Zero;
      float num1 = float.MaxValue;
      int index = -1;
      for (int indexOfPosition = 0; indexOfPosition < this.ChildNodes.Length; ++indexOfPosition)
      {
        if (this.ChildNodes[indexOfPosition] != null)
        {
          int num2 = nodeSize / 2 / 2;
          int num3 = flag ? 1 : 0;
          flag = true;
          Vector3 vector3 = this.GetChildDirection(indexOfPosition) * (float) num2 + nodePosition;
          float num4 = Vector3.DistanceSquared(position, vector3);
          if ((double) num4 < (double) num1)
          {
            index = indexOfPosition;
            num1 = num4;
            nodePosition1 = vector3;
          }
        }
      }
      return !flag ? nodePosition : this.ChildNodes[index].GetClosestNode(position, nodePosition1, nodeSize / 2);
    }

    public override void GetCollidingLeafNodes(
      OctTreeNode<T> otherNode,
      Position3 thisPosition,
      Position3 otherPosition,
      int thisNodeSize,
      int otherNodeSize,
      Vector3[] thisAxises,
      Vector3[] otherAxises,
      ref OctTreeCollisonContainer<T> cont)
    {
      if (!this.NodesCollide(ref thisPosition, ref otherPosition, thisNodeSize, otherNodeSize))
        return;
      int otherNodeSize1 = thisNodeSize / 2;
      for (int indexOfPosition = 0; indexOfPosition < 8; ++indexOfPosition)
      {
        if (this.ChildNodes[indexOfPosition] != null)
        {
          Vector3 childDirection = this.GetChildDirection(indexOfPosition);
          Position3 otherPosition1 = thisPosition + (childDirection.X * thisAxises[0] + childDirection.Y * thisAxises[1] + childDirection.Z * thisAxises[2]) * (float) otherNodeSize1 * 0.5f;
          otherNode.GetCollidingLeafNodes(this.ChildNodes[indexOfPosition], otherPosition, otherPosition1, otherNodeSize, otherNodeSize1, otherAxises, thisAxises, ref cont);
        }
      }
    }

    public override T GetLeaf(ref Vector3 positionInTreeCoordinates, int nodeSize)
    {
      int childIndex = this.GetChildIndex(nodeSize, ref positionInTreeCoordinates);
      return this.ChildNodes[childIndex] != null ? this.ChildNodes[childIndex].GetLeaf(ref positionInTreeCoordinates, nodeSize / 2) : default (T);
    }

    public override OctTreeNode<T> GetLeafNode(ref Vector3 positionInTreeCoordinates, int nodeSize)
    {
      int childIndex = this.GetChildIndex(nodeSize, ref positionInTreeCoordinates);
      return this.ChildNodes[childIndex] != null ? this.ChildNodes[childIndex].GetLeafNode(ref positionInTreeCoordinates, nodeSize / 2) : (OctTreeNode<T>) this;
    }

    public override bool IsEmpty()
    {
      for (int index = 0; index < this.ChildNodes.Length; ++index)
      {
        if (this.ChildNodes[index] != null)
          return false;
      }
      return true;
    }

    public override OctTreeNode<T> Optimizable()
    {
      if (this.NumberOfChildNodes == (byte) 1)
      {
        for (int index = 0; index < this.ChildNodes.Length; ++index)
        {
          if (this.ChildNodes[index] != null)
            return this.ChildNodes[index].Optimizable();
        }
      }
      return (OctTreeNode<T>) this;
    }

    public override T RemoveLeafNode(ref Vector3 positionInTreeCoordinates, int nodeSize)
    {
      int childIndex = this.GetChildIndex(nodeSize, ref positionInTreeCoordinates);
      if (this.ChildNodes[childIndex] != null)
      {
        if (this.ChildNodes[childIndex] is OctTreeLeafNode<T>)
        {
          T leaf = this.ChildNodes[childIndex].GetLeaf(ref positionInTreeCoordinates, nodeSize);
          this.ChildNodes[childIndex] = (OctTreeNode<T>) null;
          return leaf;
        }
        T obj = this.ChildNodes[childIndex].RemoveLeafNode(ref positionInTreeCoordinates, nodeSize / 2);
        if ((object) obj != null)
        {
          if (this.ChildNodes[childIndex].IsEmpty())
            this.ChildNodes[childIndex] = (OctTreeNode<T>) null;
          return obj;
        }
      }
      return default (T);
    }

    public override byte NumberOfChildNodes
    {
      get
      {
        byte numberOfChildNodes = 0;
        for (int index = 0; index < this.ChildNodes.Length; ++index)
          numberOfChildNodes += this.ChildNodes[index] != null ? (byte) 1 : (byte) 0;
        return numberOfChildNodes;
      }
    }
  }
}
