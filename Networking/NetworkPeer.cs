// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.NetworkPeer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public abstract class NetworkPeer
  {
    protected const string cAppIdentifier = "Corneroids";
    protected const int cMaxMessageLength = 64;
    protected const int cMaxMessageQueueLength = 3;
    private CornerSpace.Console console;
    private bool isMessageCycle;
    private int port;
    private int version;
    private int rate;
    private int timer;
    private int timeUntilNextMessageCycle;
    private NetworkPeer.Type type;
    private readonly Queue<MessageContainer> messageQueue;

    public NetworkPeer(int requiredGameVersion, int portNumber, int rate)
    {
      this.Port = portNumber;
      this.version = requiredGameVersion;
      this.Rate = rate;
      this.console = new CornerSpace.Console();
      this.messageQueue = new Queue<MessageContainer>();
    }

    public abstract void Disconnect();

    public bool IsMessageCycle() => this.isMessageCycle;

    public abstract bool Prepare();

    public bool SendDataMessage(NetOutgoingMessage message, NetConnection receiver)
    {
      if (message == null)
        return false;
      this.messageQueue.Enqueue(new MessageContainer()
      {
        SendMethod = NetDeliveryMethod.Unreliable,
        Message = message,
        Receiver = receiver
      });
      return true;
    }

    public bool SendOrderedDataMessage(
      NetOutgoingMessage message,
      NetConnection receiver,
      int sequenceChannel)
    {
      if (message == null)
        return false;
      this.messageQueue.Enqueue(new MessageContainer()
      {
        SendMethod = NetDeliveryMethod.ReliableOrdered,
        Message = message,
        Receiver = receiver,
        SequenceChannel = sequenceChannel
      });
      return true;
    }

    public virtual void Update(int elapsedMilliSeconds)
    {
      int num = 1000 / this.rate;
      this.timer += elapsedMilliSeconds;
      if (this.timer >= num)
      {
        this.DispatchMessages(this.messageQueue);
        this.timer = 0;
        this.isMessageCycle = true;
      }
      else
        this.isMessageCycle = false;
      this.timeUntilNextMessageCycle = num - this.timer;
    }

    protected CornerSpace.Console Console => this.console;

    public int Port
    {
      get => this.port;
      private set => this.port = Math.Max(0, Math.Min(value, (int) ushort.MaxValue));
    }

    public int Rate
    {
      get => this.rate;
      set => this.rate = Math.Max(1, Math.Min(value, 60));
    }

    public int TimeUntilNextMessageCycle => this.timeUntilNextMessageCycle;

    public int Version => this.version;

    public NetOutgoingMessage CreateEventMessageTemplate(NetworkPeer.GameEventId eventType)
    {
      NetOutgoingMessage messageTemplate = this.CreateMessageTemplate(NetworkPeer.MessageID.GameEvent);
      messageTemplate.Write((byte) eventType);
      return messageTemplate;
    }

    public abstract NetOutgoingMessage CreateMessageTemplate(NetworkPeer.MessageID messageType);

    protected abstract void DispatchMessages(Queue<MessageContainer> messageQueue);

    protected string ParseChatMessage(string message)
    {
      if (!string.IsNullOrEmpty(message))
      {
        message = message.Trim();
        if (message.Length > 0)
        {
          if (message.Length > 64)
            message = message.Substring(0, 64);
          return message;
        }
      }
      return (string) null;
    }

    private void ResetTimer()
    {
      if (this.timer < 1000 / this.rate)
        return;
      this.timer = 0;
    }

    private void IncrementTimer(int value) => this.timer += value;

    public enum MessageID : byte
    {
      ServerInfoRequest = 0,
      ServerInfoResponse = 1,
      ClientDataRequest = 2,
      ClientDataResponse = 3,
      ClientPlayersDataRequest = 4,
      ServerInfo = 5,
      ClientInfo = 6,
      ClientData = 7,
      WorldData = 10, // 0x0A
      WorldDataResponse = 11, // 0x0B
      NewClientConnected = 12, // 0x0C
      ClientDisconnected = 13, // 0x0D
      ChatMessage = 14, // 0x0E
      ServerToPlayerStateUpdate = 15, // 0x0F
      PlayerToServerStateUpdate = 16, // 0x10
      CreateNewEntity = 17, // 0x11
      RemoveEntity = 18, // 0x12
      AddNewBlock = 19, // 0x13
      RemoveBlock = 20, // 0x14
      EntityTransferRequest = 21, // 0x15
      EntityTransferResponse = 22, // 0x16
      EntityTransferData = 23, // 0x17
      EntityTransferAck = 24, // 0x18
      EntityTransferFinalization = 25, // 0x19
      EntityTransferFinalizationAck = 26, // 0x1A
      EntityTransferBlockSector = 27, // 0x1B
      EntityTransferBlockControls = 28, // 0x1C
      ControlKeyChanged = 29, // 0x1D
      ControlNewBind = 30, // 0x1E
      ControlUnbind = 31, // 0x1F
      EntityStateUpdate = 32, // 0x20
      PlayerMounted = 33, // 0x21
      CreateProjectile = 34, // 0x22
      ClientSynchronizationState = 35, // 0x23
      GameEvent = 36, // 0x24
      ControlKeyChangeRequest = 37, // 0x25
      ControlBindRequest = 38, // 0x26
      ControlUnbindRequest = 39, // 0x27
      ServerMessage = 100, // 0x64
    }

    public enum GameEventId : byte
    {
      KillPlayer = 0,
      AddItem = 1,
      SpawnPlayer = 2,
      TossItem = 3,
      PickItem = 4,
      SetItem = 5,
      AckItemRequest = 6,
      OpenContainer = 7,
      EndSynchronization = 8,
      InventoryData = 9,
      SynchronizeInventories = 10, // 0x0A
      RequestSpawn = 102, // 0x66
      RequestTossItem = 103, // 0x67
      RequestItemOperation = 104, // 0x68
    }

    public enum EntityTransactionID : byte
    {
      BlockSector,
      EntityData,
    }

    public enum Type : byte
    {
      LAN,
      Internet,
    }

    public enum MessageSequenceChannels : byte
    {
      InitializationData,
      HandlerRequests,
      WorldData,
      ClientConnectionUpdate,
      BlockData,
      ChatMessages,
      EntityTransfer,
      ControlBlockData,
      GameEvent,
      ItemEvent,
    }
  }
}
