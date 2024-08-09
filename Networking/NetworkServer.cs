// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.NetworkServer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

#nullable disable
namespace CornerSpace.Networking
{
  public class NetworkServer : NetworkPeer, INetworkServer, INetwork
  {
    private const int cRate = 20;
    private NetworkDispatcher<NetConnection, NetworkServer> dispatcher;
    protected List<ServerToClientConnection> connections;
    private NetServer server;
    private IdGen idGen;
    private int maxConnectionCount;
    private string ownerName;
    private string password;
    private string serverName;
    private string token;

    public NetworkServer(
      int serverGameVersion,
      int maxConnections,
      int portNumber,
      string name,
      string token)
      : base(serverGameVersion, portNumber, 20)
    {
      this.connections = new List<ServerToClientConnection>();
      this.idGen = new IdGen(1);
      this.maxConnectionCount = maxConnections;
      this.ServerName = name;
      this.token = token;
      this.dispatcher = new NetworkDispatcher<NetConnection, NetworkServer>();
      NetPeerConfiguration config = new NetPeerConfiguration("Corneroids");
      config.ConnectionTimeout = 20f;
      config.Port = portNumber;
      config.UseMessageRecycling = true;
      config.MaximumConnections = maxConnections;
      config.SetMessageTypeEnabled(NetIncomingMessageType.ConnectionApproval, true);
      config.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
      config.ReceiveBufferSize = 3000000;
      config.SendBufferSize = 3000000;
      config.SetMessageTypeEnabled(NetIncomingMessageType.DiscoveryResponse, false);
      config.SetMessageTypeEnabled(NetIncomingMessageType.DiscoveryRequest, true);
      this.server = new NetServer(config);
    }

    public virtual bool ClientNameAvailable(string name) => true;

    public NetOutgoingMessage CreateMessage() => this.server.CreateMessage();

    public override NetOutgoingMessage CreateMessageTemplate(NetworkPeer.MessageID messageType)
    {
      NetOutgoingMessage message = this.server.CreateMessage();
      message.Write((byte) messageType);
      return message;
    }

    public override void Disconnect() => this.server.Shutdown("Server shutdown");

    public void DisconnectUser(NetConnection client, string reason)
    {
      if (client == null)
        return;
      client.Disconnect(reason ?? "no reason");
      this.UserDisconnected(client, reason);
    }

    public ServerToClientConnection GetConnection(NetConnection connection)
    {
      return this.connections.Find((Predicate<ServerToClientConnection>) (c => c.Connection == connection));
    }

    public ServerToClientConnection GetConnectionOfId(int id)
    {
      return this.connections.Find((Predicate<ServerToClientConnection>) (c => c.Id == id));
    }

    public ServerToClientConnection GetConnectionOfPlayer(Player player)
    {
      return player != null ? this.connections.Find((Predicate<ServerToClientConnection>) (c => c.Id == player.Id)) : (ServerToClientConnection) null;
    }

    public override bool Prepare()
    {
      try
      {
        this.server.Start();
        this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkServer>) new ServerDisconnectListener(this));
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to prepare the server: " + ex.Message);
        return false;
      }
    }

    public void RegisterNewEventHandler(
      EventHandler<NetConnection, NetworkServer> eventHandler)
    {
      if (eventHandler == null)
        return;
      this.dispatcher.RegisterEventHandler(eventHandler);
    }

    public override void Update(int elapsedMilliSeconds)
    {
      if (this.server.Status != NetPeerStatus.Running)
      {
        this.Console.WriteErrorLine("Server update failed: server is not running!");
      }
      else
      {
        base.Update(elapsedMilliSeconds);
        NetIncomingMessage netIncomingMessage;
        while ((netIncomingMessage = this.server.ReadMessage()) != null)
        {
          try
          {
            NetIncomingMessageType messageType = netIncomingMessage.MessageType;
            NetConnection senderConnection = netIncomingMessage.SenderConnection;
            switch (messageType)
            {
              case NetIncomingMessageType.Error:
                Engine.Console.WriteErrorLine("Lidgren error message: " + netIncomingMessage.ReadString());
                continue;
              case NetIncomingMessageType.ConnectionApproval:
                senderConnection.Approve();
                this.dispatcher.RegisterEventHandler((EventHandler<NetConnection, NetworkServer>) new ServerConnetionInitializerHandler(this, senderConnection, (IPlayerStorage) Engine.LoadedWorld));
                continue;
              case NetIncomingMessageType.DiscoveryRequest:
                this.SendDiscoveryResponse(netIncomingMessage.SenderEndPoint);
                continue;
              case NetIncomingMessageType.WarningMessage:
                Engine.Console.WriteErrorLine("Lidgren warning message: " + netIncomingMessage.ReadString());
                continue;
              default:
                this.dispatcher.HandleIncomingMessage(netIncomingMessage);
                continue;
            }
          }
          finally
          {
            this.server.Recycle(netIncomingMessage);
          }
        }
        this.dispatcher.Update();
      }
    }

    public ServerToClientConnection UserConnected(
      NetConnection connection,
      string name,
      string token)
    {
      if (connection == null || this.ConnectionToEndPoint(connection) != null)
        return (ServerToClientConnection) null;
      int newId = this.idGen.GetNewId();
      ServerToClientConnection newConnection = new ServerToClientConnection(connection, newId)
      {
        ClientName = name,
        Token = token
      };
      if (this.UserConnectedEvent != null)
        this.UserConnectedEvent(newConnection);
      this.connections.Add(newConnection);
      this.InformNewClientOfOtherClients(newConnection);
      this.InformOthersOfNewClient(newConnection);
      return newConnection;
    }

    public void UserDisconnected(NetConnection connection, string reason)
    {
      if (connection == null)
        return;
      ServerToClientConnection disconnectedClient = this.connections.Find((Predicate<ServerToClientConnection>) (c => c.Connection == connection));
      if (disconnectedClient == null)
        return;
      this.connections.Remove(disconnectedClient);
      this.idGen.ReleaseID(disconnectedClient.Id);
      if (this.UserDisconnectedEvent != null)
        this.UserDisconnectedEvent(reason ?? "Unknown reason", disconnectedClient.Id);
      this.dispatcher.ClientDisconnected(disconnectedClient.Id);
      this.dispatcher.UnregisterEventHandlersOf(connection);
      this.InformOthersOfDisconnectedClient(disconnectedClient, reason);
    }

    public event Action<ServerToClientConnection> UserConnectedEvent;

    public event Action<string, int> UserDisconnectedEvent;

    public int ConnectionCount => this.connections.Count;

    public int MaxConnections => this.maxConnectionCount;

    public string OwnerName
    {
      set => this.ownerName = value;
    }

    public string Password
    {
      get => this.password;
      set => this.password = value;
    }

    public string ServerName
    {
      get => this.serverName ?? "noname";
      private set
      {
        if (string.IsNullOrEmpty(value))
          this.serverName = "Corneroids server";
        else
          this.serverName = value;
      }
    }

    public string Token => this.token;

    private ServerToClientConnection ConnectionToEndPoint(NetConnection connection)
    {
      return connection != null ? this.connections.Find((Predicate<ServerToClientConnection>) (c => c.Connection == connection)) : (ServerToClientConnection) null;
    }

    private ServerToClientConnection ConnectionToEndPoint(IPEndPoint address)
    {
      if (address != null)
      {
        for (int index = 0; index < this.connections.Count; ++index)
        {
          if (this.connections[index].Connection != null && this.connections[index].Connection.RemoteEndPoint == address)
            return this.connections[index];
        }
      }
      return (ServerToClientConnection) null;
    }

    protected override void DispatchMessages(Queue<MessageContainer> messageQueue)
    {
      if (this.server == null || this.server.Status != NetPeerStatus.Running)
        return;
      while (messageQueue.Count > 0)
      {
        MessageContainer messageContainer = messageQueue.Dequeue();
        NetConnection receiver = messageContainer.Receiver;
        NetOutgoingMessage message = messageContainer.Message;
        NetDeliveryMethod sendMethod = messageContainer.SendMethod;
        if (receiver == null)
        {
          this.server.SendToAll(message, sendMethod);
        }
        else
        {
          int num = (int) receiver.SendMessage(message, sendMethod, messageContainer.SequenceChannel);
        }
      }
    }

    private void InformNewClientOfOtherClients(ServerToClientConnection newConnection)
    {
      if (newConnection == null)
        return;
      List<ServerToClientConnection> list = this.connections.Where<ServerToClientConnection>((Func<ServerToClientConnection, bool>) (c => c != newConnection && c != null)).ToList<ServerToClientConnection>();
      if (!string.IsNullOrEmpty(this.ownerName))
        list.Add(new ServerToClientConnection((NetConnection) null, 0)
        {
          ClientName = this.ownerName
        });
      foreach (ServerToClientConnection newConnection1 in list)
        this.SendInfoOfNewClientTo(newConnection1, newConnection);
    }

    private void InformOthersOfDisconnectedClient(
      ServerToClientConnection disconnectedClient,
      string reason)
    {
      if (disconnectedClient == null)
        return;
      try
      {
        List<ServerToClientConnection> list = this.connections.Where<ServerToClientConnection>((Func<ServerToClientConnection, bool>) (c => c != disconnectedClient && c != null)).ToList<ServerToClientConnection>();
        if (list.Count <= 0)
          return;
        foreach (ServerToClientConnection clientConnection in list)
        {
          NetOutgoingMessage message = this.server.CreateMessage();
          message.Write((byte) 13);
          message.Write(disconnectedClient.Id);
          message.Write(reason ?? "Unknown reason");
          int num = (int) clientConnection.Connection.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 3);
        }
      }
      catch (Exception ex)
      {
        this.Console.WriteErrorLine("Failed to deliver disconnectedClientMessage to others: " + ex.Message);
      }
    }

    private void InformOthersOfNewClient(ServerToClientConnection newConnection)
    {
      if (newConnection == null)
        return;
      foreach (ServerToClientConnection targetConnection in this.connections.Where<ServerToClientConnection>((Func<ServerToClientConnection, bool>) (c => c != newConnection && c != null)).ToList<ServerToClientConnection>())
        this.SendInfoOfNewClientTo(newConnection, targetConnection);
    }

    private void SendDiscoveryResponse(IPEndPoint target)
    {
      if (target == null)
        return;
      NetOutgoingMessage message = this.server.CreateMessage();
      message.Write(this.ServerName ?? string.Empty);
      message.Write(Engine.GameVersion);
      message.Write(this.connections.Count);
      message.Write(this.MaxConnections);
      message.Write("Reserved for token");
      message.Write(!string.IsNullOrEmpty(this.password));
      this.server.SendDiscoveryResponse(message, target);
    }

    private void SendInfoOfNewClientTo(
      ServerToClientConnection newConnection,
      ServerToClientConnection targetConnection)
    {
      if (newConnection == null)
        return;
      if (targetConnection == null)
        return;
      try
      {
        NetOutgoingMessage message = this.server.CreateMessage();
        message.Write((byte) 12);
        message.Write(newConnection.Id);
        message.Write(newConnection.ClientName ?? string.Empty);
        int num = (int) targetConnection.Connection.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 3);
      }
      catch (Exception ex)
      {
        this.Console.WriteErrorLine("Failed to inform the new client of other clients: " + ex.Message);
      }
    }
  }
}
