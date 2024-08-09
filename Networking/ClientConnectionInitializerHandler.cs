// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientConnectionInitializerHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientConnectionInitializerHandler : SequencedEventHandler<NetworkClient>
  {
    private string serverToken;
    private string responseToken;

    public ClientConnectionInitializerHandler(NetworkClient client, NetConnection connection)
      : base(connection, client)
    {
      this.AddNewSequenceFunction("Jäädään odottamaan palvelimelta tulevaa dataa", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 5)
          return;
        string str = message.ReadString();
        this.serverToken = message.ReadString();
        bool flag = message.ReadBoolean();
        this.networkInterface.ServerName = str;
        this.responseToken = this.GetResponseToken(this.serverToken);
        NetOutgoingMessage message1 = this.networkInterface.CreateMessage();
        message1.Write((byte) 6);
        message1.Write(this.networkInterface.ClientName ?? "Noname");
        message1.Write(this.networkInterface.Version);
        message1.Write(flag ? this.networkInterface.Password ?? string.Empty : string.Empty);
        message1.Write(this.responseToken);
        Engine.Console.LogNetworkTraffic("Server info received!");
        Engine.Console.LogNetworkTraffic("Client info sent to server with token " + this.responseToken);
        int num = (int) this.Key.SendMessage(message1, NetDeliveryMethod.ReliableOrdered, 0);
        this.MoveToNextState();
      })).AddNewSequenceFunction("Odotetaan servun hyväksyntää sekä pelaajan id:tä ja kuitataan yhteys avatuksi", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 7)
          return;
        int id = message.ReadInt32();
        string str = message.ReadString();
        this.networkInterface.Connected(id);
        Engine.Console.LogNetworkTraffic("Server acked with token " + str + " and connection is initialized!");
        if (str != this.responseToken)
          Engine.StoredValues.StoreValue<string>(this.serverToken, str);
        this.Unregister();
      }));
    }

    public override void ErrorCaught(Exception e) => this.networkInterface.Disconnect();

    private string GetResponseToken(string token)
    {
      return token == null || token.Length == 0 ? string.Empty : Engine.StoredValues.RetrieveValue<string>(token) ?? string.Empty;
    }
  }
}
