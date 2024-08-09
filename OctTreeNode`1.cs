// Decompiled with JetBrains decompiler
// Type: CornerSpace.OctTreeNode`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class OctTreeNode<T>
  {
    public virtual T AddLeaf(
      T newLeaf,
      int nodeSize,
      int leafSize,
      ref Vector3 positionInTreeCoordinates)
    {
      return default (T);
    }

    public virtual void AddNode(
      OctTreeNode<T> node,
      int sizeOfNodeToAdd,
      int nodeSize,
      ref Vector3 positionInTreeCoordinates)
    {
    }

    public virtual void Clear()
    {
    }

    public abstract Vector3 GetClosestNode(Vector3 position, Vector3 nodePosition, int nodeSize);

    public virtual void GetCollidingLeafNodes(
      OctTreeNode<T> otherNode,
      Position3 thisPosition,
      Position3 otherPosition,
      int thisNodeSize,
      int otherNodeSize,
      Vector3[] thisAxises,
      Vector3[] otherAxises,
      ref OctTreeCollisonContainer<T> cont)
    {
    }

    public abstract T GetLeaf(ref Vector3 positionInTreeCoordinates, int nodeSize);

    public abstract OctTreeNode<T> GetLeafNode(ref Vector3 positionInTreeCoordinates, int nodeSize);

    public virtual bool IsEmpty() => true;

    public virtual OctTreeNode<T> Optimizable() => this;

    public abstract T RemoveLeafNode(ref Vector3 positionInTreeCoordinates, int nodeSize);

    public virtual byte NumberOfChildNodes => 0;

    public virtual bool Uniform
    {
      get => true;
      set
      {
      }
    }

    protected virtual OctTreeNode<T> CreateNewLeafNode(T data)
    {
      return (OctTreeNode<T>) new OctTreeLeafNode<T>()
      {
        Data = data
      };
    }

    protected virtual OctTreeNode<T> CreateNewLinkNode()
    {
      return (OctTreeNode<T>) new OctTreeLinkNode<T>();
    }

    protected int GetChildIndex(int nodeSize, ref Vector3 position)
    {
      int num1 = nodeSize / 2;
      int num2 = (double) position.Z < 0.0 ? ((double) Math.Abs(position.Z % (float) nodeSize) < (double) num1 ? 4 : 0) : ((double) position.Z % (double) nodeSize < (double) num1 ? 0 : 4);
      int num3 = (double) position.Y < 0.0 ? num2 + ((double) Math.Abs(position.Y % (float) nodeSize) < (double) num1 ? 2 : 0) : num2 + ((double) position.Y % (double) nodeSize < (double) num1 ? 0 : 2);
      return (double) position.X < 0.0 ? num3 + ((double) Math.Abs(position.X % (float) nodeSize) < (double) num1 ? 1 : 0) : num3 + ((double) position.X % (double) nodeSize < (double) num1 ? 0 : 1);
    }

    protected Vector3 GetChildDirection(int indexOfPosition)
    {
      return new Vector3(indexOfPosition % 2 == 0 ? -1f : 1f, indexOfPosition / 2 % 2 == 0 ? -1f : 1f, indexOfPosition / 4 == 0 ? -1f : 1f);
    }

    protected bool NodesCollide(
      ref Position3 node1Pos,
      ref Position3 node2Pos,
      int node1Size,
      int node2Size)
    {
      float num1 = 0.866025f * (float) node1Size;
      float num2 = 0.866025f * (float) node2Size;
      float num3 = (float) (((double) num1 + (double) num2) * ((double) num1 + (double) num2));
      return (double) (node1Pos - node2Pos).LengthSquared() <= (double) num3;
    }
  }
}
