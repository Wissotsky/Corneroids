// Decompiled with JetBrains decompiler
// Type: CornerSpace.NodePair`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public struct NodePair<T>
  {
    public T Node1;
    public T Node2;
    public Position3 Node1Position;
    public Position3 Node2Position;

    public NodePair(T node1, T node2, Position3 n1Pos, Position3 n2Pos)
    {
      this.Node1 = node1;
      this.Node2 = node2;
      this.Node1Position = n1Pos;
      this.Node2Position = n2Pos;
    }
  }
}
