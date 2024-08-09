// Decompiled with JetBrains decompiler
// Type: CornerSpace.OctTreeLeafNode`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class OctTreeLeafNode<T> : OctTreeNode<T>
  {
    public T Data;

    public override Vector3 GetClosestNode(Vector3 position, Vector3 nodePosition, int nodeSize)
    {
      return nodePosition;
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
      if (otherNode is OctTreeLeafNode<T>)
        cont.CollidedNodes.Add(new OctTreeCollisionPoint<T>()
        {
          NodeOne = (OctTreeNode<T>) this,
          NodeTwo = otherNode,
          NodeOnePosition = thisPosition,
          NodeTwoPosition = otherPosition
        });
      else
        otherNode.GetCollidingLeafNodes((OctTreeNode<T>) this, otherPosition, thisPosition, otherNodeSize, thisNodeSize, otherAxises, thisAxises, ref cont);
    }

    public override T GetLeaf(ref Vector3 positionInTreeCoordinates, int nodeSize) => this.Data;

    public override OctTreeNode<T> GetLeafNode(ref Vector3 positionInTreeCoordinates, int nodeSize)
    {
      return (OctTreeNode<T>) this;
    }

    public override T RemoveLeafNode(ref Vector3 positionInTreeCoordinates, int nodeSize)
    {
      return this.Data;
    }
  }
}
