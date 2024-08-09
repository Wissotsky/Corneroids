// Decompiled with JetBrains decompiler
// Type: CornerSpace.OctTree`2
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class OctTree<T, Y> where T : OctTreeNode<Y>
  {
    private static List<OctTreeCollisionPoint<Y>> collidingNodes = new List<OctTreeCollisionPoint<Y>>();
    private static List<KeyValuePair<OctTreeNode<Y>, Position3>> collidingLeafNodes2 = new List<KeyValuePair<OctTreeNode<Y>, Position3>>();
    private BoundingBox bounds;
    private int leafSize;
    protected T rootNode;

    public OctTree(int leafSize) => this.leafSize = leafSize;

    public OctTree(int leafSize, Vector3 min, Vector3 max)
    {
      this.leafSize = leafSize;
      this.bounds = new BoundingBox(min, max);
      this.rootNode = this.CreateRootNode(default (Y));
    }

    public virtual void AddLeaf(Y leafData, Vector3 position)
    {
      if ((object) this.rootNode == null)
      {
        Vector3 nodeCoords = this.ConvertCoordsToNodeCoords(position);
        Vector3 max = nodeCoords + Vector3.One * (float) this.leafSize;
        this.bounds = new BoundingBox(nodeCoords, max);
        this.rootNode = this.CreateRootNode(leafData);
      }
      else
      {
        Vector3 positionInTreeCoordinates;
        while (this.bounds.Contains(position) == ContainmentType.Disjoint)
        {
          Vector3 vector3_1 = 0.5f * (this.bounds.Min + this.bounds.Max);
          Vector3 vector3_2 = vector3_1;
          float x = (this.bounds.Max - this.bounds.Min).X;
          vector3_2.X += (double) position.X >= (double) vector3_1.X ? 0.5f * x : -0.5f * x;
          vector3_2.Y += (double) position.Y >= (double) vector3_1.Y ? 0.5f * x : -0.5f * x;
          vector3_2.Z += (double) position.Z >= (double) vector3_1.Z ? 0.5f * x : -0.5f * x;
          T rootNode = this.rootNode;
          this.rootNode = this.CreateLinkNode();
          this.bounds = new BoundingBox(vector3_2 - Vector3.One * x, vector3_2 + Vector3.One * x);
          positionInTreeCoordinates = vector3_1 - this.bounds.Min;
          this.rootNode.AddNode((OctTreeNode<Y>) rootNode, (int) x, this.GetRootNodeSize(), ref positionInTreeCoordinates);
        }
        Vector3 vector3 = (this.bounds.Min + this.bounds.Max) / 2f;
        positionInTreeCoordinates = position - this.bounds.Min;
        this.rootNode.AddLeaf(leafData, this.GetRootNodeSize(), this.leafSize, ref positionInTreeCoordinates);
      }
    }

    public virtual bool AddLeafIfFits(Y leafData, Vector3 position)
    {
      if ((object) this.rootNode == null)
      {
        Vector3 nodeCoords = this.ConvertCoordsToNodeCoords(position);
        Vector3 max = nodeCoords + Vector3.One * (float) this.leafSize;
        this.bounds = new BoundingBox(nodeCoords, max);
        this.rootNode = this.CreateRootNode(leafData);
      }
      else
      {
        if (this.bounds.Contains(position) == ContainmentType.Disjoint)
          return false;
        Vector3 vector3 = (this.bounds.Min + this.bounds.Max) / 2f;
        Vector3 positionInTreeCoordinates = position - this.bounds.Min;
        this.rootNode.AddLeaf(leafData, this.GetRootNodeSize(), this.leafSize, ref positionInTreeCoordinates);
      }
      return true;
    }

    public Vector3? GetClosestNode(Vector3 positionInEntitySpace)
    {
      return (object) this.rootNode == null ? new Vector3?() : new Vector3?(this.rootNode.GetClosestNode(positionInEntitySpace, this.Position, this.GetRootNodeSize()));
    }

    public void GetCollidingLeafNodes(
      OctTree<T, Y> otherTree,
      Vector3[] thisAxises,
      Vector3[] otherAxises,
      Position3 thisPosition,
      Position3 otherPosition,
      List<OctTreeCollisionPoint<Y>> collidingLeaves,
      int minNodeSize)
    {
      OctTree<T, Y>.collidingNodes.Clear();
      OctTreeCollisonContainer<Y> cont = new OctTreeCollisonContainer<Y>()
      {
        TargetNodeSize = 16,
        CollidedNodes = collidingLeaves
      };
      this.rootNode.GetCollidingLeafNodes((OctTreeNode<Y>) otherTree.rootNode, thisPosition, otherPosition, this.GetRootNodeSize(), otherTree.GetRootNodeSize(), thisAxises, otherAxises, ref cont);
    }

    public Y GetLeaf(Vector3 position)
    {
      if ((object) this.rootNode == null || this.bounds.Contains(position) != ContainmentType.Contains)
        return default (Y);
      position = this.ToOctTreeCoordinates(position);
      return this.rootNode.GetLeaf(ref position, this.GetRootNodeSize());
    }

    public T GetLeafNode(Vector3 position)
    {
      if ((object) this.rootNode == null || this.bounds.Contains(position) != ContainmentType.Contains)
        return default (T);
      position -= this.bounds.Min;
      return this.rootNode.GetLeafNode(ref position, this.GetRootNodeSize()) as T;
    }

    public int GetRootNodeSize()
    {
      return (object) this.rootNode != null ? (int) (this.bounds.Max - this.bounds.Min).X : 0;
    }

    public bool IsEmpty() => (object) this.rootNode == null;

    public virtual Y RemoveLeaf(Vector3 position)
    {
      if (this.bounds.Contains(position) == ContainmentType.Disjoint)
        return default (Y);
      if ((object) this.rootNode == null)
        return default (Y);
      position -= this.bounds.Min;
      Y y = this.rootNode.RemoveLeafNode(ref position, this.GetRootNodeSize());
      if (this.rootNode.IsEmpty())
        this.rootNode = default (T);
      return y;
    }

    public BoundingBox Bounds
    {
      get => this.bounds;
      set => this.bounds = value;
    }

    public Vector3 Position => 0.5f * (this.bounds.Max + this.bounds.Min);

    public T RootNode => this.rootNode;

    protected Vector3 ConvertCoordsToNodeCoords(Vector3 coordinates)
    {
      return new Vector3((float) Math.Floor((double) coordinates.X / (double) this.leafSize), (float) Math.Floor((double) coordinates.Y / (double) this.leafSize), (float) Math.Floor((double) coordinates.Z / (double) this.leafSize)) * (float) this.leafSize;
    }

    protected virtual T CreateLinkNode() => new OctTreeLinkNode<Y>() as T;

    protected virtual T CreateRootNode(Y data)
    {
      return new OctTreeLeafNode<Y>() { Data = data } as T;
    }

    protected Vector3 ToOctTreeCoordinates(Vector3 position) => position - this.bounds.Min;
  }
}
