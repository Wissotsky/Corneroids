// Decompiled with JetBrains decompiler
// Type: CornerSpace.Utility.SimpleMath
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Utility
{
  public class SimpleMath
  {
    private SimpleMath()
    {
    }

    public static int FastFloor(float value)
    {
      return (double) value < 0.0 ? (int) value - 1 : (int) value;
    }

    public static float FastMax(float v1, float v2) => (double) v1 < (double) v2 ? v2 : v1;

    public static int FastMax(int v1, int v2) => v1 < v2 ? v2 : v1;

    public static float FastMin(float v1, float v2) => (double) v1 > (double) v2 ? v2 : v1;

    public static int FastMin(int v1, int v2) => v1 > v2 ? v2 : v1;

    public static void InsertionSort<T>(List<T> listToSort, Comparison<T> comparison)
    {
      if (listToSort == null || comparison == null)
        return;
      for (int index1 = 1; index1 < listToSort.Count; ++index1)
      {
        T y = listToSort[index1];
        int index2;
        for (index2 = index1; index2 > 0 && comparison(listToSort[index2 - 1], y) > 0; --index2)
          listToSort[index2] = listToSort[index2 - 1];
        listToSort[index2] = y;
      }
    }

    public static bool IsPowerOfTwo(int value) => (value & value - 1) == 0;

    public static float RoundTwoDecimals(float value)
    {
      return (float) (int) ((double) value * 100.0) / 100f;
    }

    public static Vector3 VectorProjection(Vector3 vector, Vector3 normal)
    {
      return Vector3.Dot(vector, normal) * normal;
    }
  }
}
