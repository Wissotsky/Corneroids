// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ISynchronizable
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace.Networking
{
  public interface ISynchronizable : IByteSerializable
  {
    event System.Action DataChanged;
  }
}
