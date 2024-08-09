// Decompiled with JetBrains decompiler
// Type: CornerSpace.Utility.IdGen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Utility
{
  public class IdGen
  {
    private object stackLock = new object();
    private Stack<int> ids;

    public IdGen(int startValue)
    {
      this.ids = new Stack<int>();
      this.ids.Push(startValue);
    }

    public int GetNewId()
    {
      lock (this.stackLock)
      {
        int newId = this.ids.Pop();
        if (this.ids.Count == 0)
          this.ids.Push(newId + 1);
        return newId;
      }
    }

    public void ReleaseID(int id)
    {
      lock (this.stackLock)
        this.ids.Push(id);
    }
  }
}
