// Decompiled with JetBrains decompiler
// Type: CornerSpace.ModelStructure`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class ModelStructure<T>
  {
    public T[] Vertices;
    public short[] Indices;

    public ModelStructure(T[] v, short[] i)
    {
      this.Vertices = v;
      this.Indices = i;
    }
  }
}
