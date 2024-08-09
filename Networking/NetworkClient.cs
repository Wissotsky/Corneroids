// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.NetworkClient
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public class NetworkClient : NetworkPeer, INetworkClient, INetwork
  {
    private const int cConnectionRetryCount = 3;
    private const int cRate = 40;
    private const int cTimeBetweenConnectionAttempts = 5000;
    protected NetConnection connection;
    protected NetworkDispatcher<NetConnection, NetworkClient> dispatcher;
    private NetClient client;
    private int clientId;
    private float clientLag;
    private string clientToken;
    private string name;
    private string password;
    private string serverName;

    public NetworkClient(int clientGameVersion, int port, string clientName)
      : base(clientGameVersion, port, 40)
    {
      this.ClientName = clientName;
      NetPeerConfiguration config = new NetPeerConfiguration("Corneroids");
      this.dispatcher = new NetworkDispatcher<NetConnection, NetworkClient>();
      config.ConnectionTimeout = 30f;
      config.Port = port;
      config.UseMessageRecycling = true;
      config.MaximumConnections = 16;
      config.ReceiveBufferSize = 1000000;
      config.SendBufferSize = 1000000;
      config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
      config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
      config.SetMessageTypeEnabled(NetIncomingMessageType.DiscoveryResponse, true);
      config.SetMessageTypeEnabled(NetIncomingMessageType.DiscoveryRequest, false);
      this.client = new NetClient(config);
    }

    public virtual bool ConnectTo(string address, string password, string token)
    {
      this.clientToken = token;
      this.password = password;
      if (this.connection != null)
      {
        this.connection.Disconnect("Disconnected by the user");
        this.connection = (NetConnection) null;
      }
      if (address != null)
      {
        try
        {
          this.connection = this.client.Connect(address, this.Port);
          this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientConnectionInitializerHandler(this, this.connection));
          this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientDisconnectListener(this.connection, this));
          this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkClient>) new ClientOtherClientsHandler(this));
          return true;
        }
        catch
        {
          this.Console.WriteErrorLine("Failed to connect to the given address. Server data is invalid.");
        }
      }
      return false;
    }

    public void Connected(int id)
    {
      this.clientId = id;
      if (this.ConnectedEvent == null)
        return;
      this.ConnectedEvent(id);
    }

    public NetOutgoingMessage CreateMessage() => this.client.CreateMessage();

    public override NetOutgoingMessage CreateMessageTemplate(NetworkPeer.MessageID messageType)
    {
      NetOutgoingMessage message = this.client.CreateMessage();
      message.Write((byte) messageType);
      return message;
    }

    public override void Disconnect()
    {
      this.client.Shutdown("Disconnected by the user");
      if (this.connection == null || this.DisconnectedEvent == null)
        return;
      this.DisconnectedEvent("Disconnected by the user");
    }

    public void Disconnect(string message) => this.DisconnectClient(message);

    public void DiscoverLanServers()
    {
      if (this.client.Status != NetPeerStatus.Running)
        return;
      this.client.DiscoverLocalPeers(this.Port);
    }

    public void DiscoverRemoteServer(string ip, int port)
    {
      if (string.IsNullOrEmpty(ip))
        return;
      this.client.DiscoverKnownPeer(ip, port);
    }

    public void OtherClientConnected(int id, string name)
    {
      if (this.OtherClientConnectedEvent == null)
        return;
      this.OtherClientConnectedEvent(id, name);
    }

    public void OtherClientDisconnected(int id, string reason)
    {
      if (this.OtherClientDisconnectedEvent == null)
        return;
      this.OtherClientDisconnectedEvent(id, reason);
    }

    public void RegisterNewEventHandler(
      EventHandler<NetConnection, NetworkClient> eventHandler)
    {
      if (eventHandler == null)
        return;
      this.dispatcher.RegisterEventHandler(eventHandler);
    }

    public override bool Prepare()
    {
      try
      {
        this.client.Start();
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public override void Update(int elapsedMilliSeconds)
    {
      if (this.client.Status != NetPeerStatus.Running)
      {
        this.Console.WriteErrorLine("Client update failed: client is not running!");
      }
      else
      {
        base.Update(elapsedMilliSeconds);
        NetIncomingMessage netIncomingMessage;
        while ((netIncomingMessage = this.client.ReadMessage()) != null)
        {
          try
          {
            NetIncomingMessageType messageType = netIncomingMessage.MessageType;
            NetConnection senderConnection = netIncomingMessage.SenderConnection;
            switch (messageType)
            {
              case NetIncomingMessageType.Error:
                Engine.Console.WriteErrorLine("Lidgren network: Error");
                break;
              case NetIncomingMessageType.DiscoveryResponse:
                this.ParseDiscoveryResponse(netIncomingMessage);
                break;
              case NetIncomingMessageType.WarningMessage:
                Engine.Console.WriteErrorLine("Lidgren warning: " + netIncomingMessage.ReadString());
                break;
              case NetIncomingMessageType.ErrorMessage:
                Engine.Console.WriteErrorLine(netIncomingMessage.ReadString());
                break;
              default:
                this.dispatcher.HandleIncomingMessage(netIncomingMessage);
                break;
            }
            if (senderConnection != null)
              this.clientLag = senderConnection.AverageRoundtripTime;
          }
          finally
          {
            this.client.Recycle(netIncomingMessage);
          }
        }
        this.dispatcher.Update();
      }
    }

    public event Action<int> ConnectedEvent;

    public event Action<string> DisconnectedEvent;

    public event Action<int, string> OtherClientConnectedEvent;

    public event Action<int, string> OtherClientDisconnectedEvent;

    public event Action<ServerInformation> ServerFoundEvent;

    public int ClientId => this.clientId;

    public float ClientLag => this.clientLag;

    public string ClientName
    {
      get => this.name;
      set
      {
        this.name = !string.IsNullOrEmpty(value) ? value : throw new ArgumentException("Name must be set.");
      }
    }

    public string Password => this.password;

    public string ServerName
    {
      get => this.serverName;
      set => this.serverName = value;
    }

    public string Token => this.clientToken;

    private void DisconnectClient(string reason)
    {
      if (this.connection == null)
        return;
      this.client.Disconnect(reason ?? "Unknown reason");
      this.connection = (NetConnection) null;
      if (this.DisconnectedEvent == null)
        return;
      this.DisconnectedEvent(reason);
    }

    protected override void DispatchMessages(Queue<MessageContainer> messageQueue)
    {
      if (this.connection == null)
        return;
      while (messageQueue.Count > 0)
      {
        MessageContainer messageContainer = messageQueue.Dequeue();
        int num = (int) this.connection.SendMessage(messageContainer.Message, messageContainer.SendMethod, messageContainer.SequenceChannel);
      }
    }

    private void ParseDiscoveryResponse(NetIncomingMessage message)
    {
      if (message == null || this.ServerFoundEvent == null)
        return;
      this.ServerFoundEvent(new ServerInformation()
      {
        Name = message.ReadString(),
        Version = message.ReadInt32(),
        PlayerCount = message.ReadInt32(),
        MaxPlayers = message.ReadInt32(),
        PlayerToken = message.ReadString(),
        PasswordRequired = message.ReadBoolean()
      });
    }
  }
}
