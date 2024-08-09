// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientPlayerUpdateListener
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientPlayerUpdateListener : EventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;

    public ClientPlayerUpdateListener(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client)
    {
      this.client = client;
    }

    public override void ErrorCaught(Exception e)
    {
    }

    public override void Handle(NetIncomingMessage message)
    {
      if (message.MessageType != NetIncomingMessageType.Data)
        return;
      if (message.ReadByte() != (byte) 15)
        return;
      try
      {
        int id = message.ReadInt32();
        long num1 = message.ReadInt64();
        int num2 = message.ReadInt32();
        int num3 = message.ReadInt32();
        uint num4 = message.ReadUInt32();
        short num5 = message.ReadInt16();
        float num6 = message.ReadFloat();
        int num7 = message.ReadInt32();
        Position3 position3 = new Position3();
        Vector3 vector3_1 = Vector3.Zero;
        Vector3 vector3_2 = Vector3.Zero;
        Vector3 vector3_3 = Vector3.Zero;
        if (num7 == -1)
        {
          position3 = new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64());
          vector3_2 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
        }
        else
        {
          vector3_1 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
          vector3_3 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
        }
        Quaternion quaternion = new Quaternion(message.ReadFloat(), message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
        StateFrame frame = new StateFrame()
        {
          ActiveItemId = num2,
          BoardedEntityId = num7,
          ButtonsStates = num3,
          Health = num5,
          Position = position3,
          PositionInEntitySpace = vector3_1,
          Orientation = quaternion,
          Power = num6,
          Tick = num1,
          Speed = vector3_2,
          SpeedInEntitySpace = vector3_3,
          SynchronizationValue = num4
        };
        this.client.SignalPlayerStateReceived(id, frame);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Illegal state update received! " + ex.Message);
      }
    }
  }
}
