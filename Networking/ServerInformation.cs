// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerInformation
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Net;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerInformation
  {
    public IPEndPoint Address;
    public string Name;
    public int MaxPlayers;
    public bool PasswordRequired;
    public int PlayerCount;
    public string PlayerToken;
    public int Version;
  }
}
