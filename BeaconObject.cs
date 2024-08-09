// Decompiled with JetBrains decompiler
// Type: CornerSpace.BeaconObject
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class BeaconObject : PhysicalObject, IRemarkable
  {
    private const int cMessageMaxLength = 20;
    private string message;

    public string GetDescriptionTag() => this.message;

    public string Message
    {
      get => this.message;
      set
      {
        if (!string.IsNullOrEmpty(value) && value.Length > 20)
          this.message = value.Substring(0, 20);
        else
          this.message = value;
      }
    }
  }
}
