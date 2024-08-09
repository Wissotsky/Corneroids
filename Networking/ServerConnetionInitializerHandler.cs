// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerConnetionInitializerHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerConnetionInitializerHandler : SequencedEventHandler<NetworkServer>
  {
    private IPlayerStorage storage;

    public ServerConnetionInitializerHandler(
      NetworkServer server,
      NetConnection connection,
      IPlayerStorage storage)
      : base(connection, server)
    {
      this.storage = storage;
      this.AddNewSequenceFunction("Odotetaan yhteyden muodostumista ja lähetetään servun tiedot vastauksessa", NetIncomingMessageType.StatusChanged, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 5)
          return;
        NetOutgoingMessage message1 = this.networkInterface.CreateMessage();
        message1.Write((byte) 5);
        message1.Write(this.networkInterface.ServerName);
        message1.Write(this.networkInterface.Token ?? string.Empty);
        message1.Write(!string.IsNullOrEmpty(this.networkInterface.Password));
        int num = (int) this.Key.SendMessage(message1, NetDeliveryMethod.ReliableOrdered, 0);
        this.MoveToNextState();
      })).AddNewSequenceFunction("Odotetaan clientin tietoja sekä salasanaa", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 6)
          return;
        string name = message.ReadString();
        int num1 = message.ReadInt32();
        string str = message.ReadString();
        string token = message.ReadString();
        if (num1 != this.networkInterface.Version)
          this.Key.Disconnect("Version mismatch");
        else if (!this.networkInterface.ClientNameAvailable(name))
          this.Key.Disconnect("Invalid or reserved user name");
        else if (this.networkInterface.ConnectionCount >= this.networkInterface.MaxConnections - 1)
          this.Key.Disconnect("Server is full");
        else if (!string.IsNullOrEmpty(this.networkInterface.Password) && this.networkInterface.Password != str)
        {
          this.Key.Disconnect("Invalid password");
        }
        else
        {
          string ackToken = this.GetAckToken(token);
          ServerToClientConnection clientConnection = this.networkInterface.UserConnected(this.Key, name, ackToken);
          if (clientConnection != null)
          {
            NetOutgoingMessage message2 = this.networkInterface.CreateMessage();
            message2.Write((byte) 7);
            message2.Write(clientConnection.Id);
            message2.Write(ackToken);
            int num2 = (int) this.Key.SendMessage(message2, NetDeliveryMethod.ReliableOrdered, 0);
          }
          else
            this.Key.Disconnect("Server error");
          this.Unregister();
        }
      }));
    }

    public override void ErrorCaught(Exception e)
    {
      this.Key.Disconnect("Connection initialization failed");
    }

    private string GetAckToken(string token)
    {
      return this.storage.PlayerWithTokenExists(token) ? token : this.storage.CreateNewPlayerToken();
    }
  }
}
