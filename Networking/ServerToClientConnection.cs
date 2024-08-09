// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerToClientConnection
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerToClientConnection
  {
    private NetConnection connection;
    private int id;
    private string name;
    private string token;

    public ServerToClientConnection(NetConnection connection, int id)
    {
      this.connection = connection;
      this.id = id;
    }

    public NetConnection Connection => this.connection;

    public string ClientName
    {
      get => this.name;
      set => this.name = value;
    }

    public int Id => this.id;

    public string Token
    {
      get => this.token;
      set => this.token = value ?? string.Empty;
    }
  }
}
