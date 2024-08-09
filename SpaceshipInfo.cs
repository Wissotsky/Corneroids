// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceshipInfo
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class SpaceshipInfo
  {
    private string name;
    private string author;
    private string path;

    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    public string Author
    {
      get => this.author;
      set => this.author = value;
    }

    public string Path
    {
      get => this.path;
      set => this.path = value;
    }
  }
}
