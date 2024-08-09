// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerPlayerUpdateListener
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerPlayerUpdateListener : EventHandler<NetConnection, NetworkServer>
  {
    private NetworkServerManager server;
    private StateFrame[] stateFrames;

    public ServerPlayerUpdateListener(NetworkServerManager server)
      : base((NetConnection) null, (NetworkServer) server)
    {
      this.server = server;
      this.stateFrames = new StateFrame[128];
    }

    public override void ErrorCaught(Exception e)
    {
    }

    public override void Handle(NetIncomingMessage message)
    {
      if (message.MessageType != NetIncomingMessageType.Data)
        return;
      if (message.ReadByte() != (byte) 16)
        return;
      try
      {
        int id = message.ReadInt32();
        int num1 = message.ReadInt32();
        float x = message.ReadFloat();
        float y = message.ReadFloat();
        float z = message.ReadFloat();
        float w = message.ReadFloat();
        sbyte num2 = message.ReadSByte();
        Quaternion quaternion = new Quaternion(x, y, z, w);
        StateFrame state = new StateFrame()
        {
          ActiveToolbarIndex = num2,
          BoardedEntityId = num1,
          ButtonsStates = message.ReadInt32(),
          ControlButtonsStates = message.ReadInt16(),
          Orientation = quaternion,
          SimulationTime = message.ReadFloat(),
          Tick = message.ReadInt64()
        };
        this.server.SignalPlayerStateReceived(id, state);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Illegal player input received! " + ex.Message);
      }
    }
  }
}
