// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientBlockDataHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientBlockDataHandler : DataEventHandler<NetConnection, NetworkClient>
  {
    private NetworkClientManager client;
    private Action<int, byte, byte, Vector3> blockAdded;
    private Action<int, Vector3, bool> blockRemoved;

    public ClientBlockDataHandler(NetworkClientManager client)
      : base((NetConnection) null, (NetworkClient) client, NetworkPeer.MessageID.AddNewBlock, NetworkPeer.MessageID.RemoveBlock)
    {
      this.client = client;
      this.blockAdded = (Action<int, byte, byte, Vector3>) ((entityId, blockId, orientation, position) => this.client.SignalNewBlockAdded(entityId, blockId, orientation, position));
      this.blockRemoved = (Action<int, Vector3, bool>) ((entityId, position, destroy) => this.client.SignalRemoveBlock(entityId, position, destroy));
    }

    public override void ErrorCaught(Exception e)
    {
      Engine.Console.WriteErrorLine("Error in block data handler: " + e.Message);
    }

    public static void ParseAddBlockFromMessage(
      NetIncomingMessage message,
      Action<int, byte, byte, Vector3> resultDelegate)
    {
      if (message == null)
        return;
      message.ReadInt32();
      int num1 = message.ReadInt32();
      byte num2 = message.ReadByte();
      byte num3 = message.ReadByte();
      Vector3 vector3 = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
      resultDelegate(num1, num2, num3, vector3);
    }

    public static void ParseRemoveBlockFromMessage(
      NetIncomingMessage message,
      Action<int, Vector3, bool> resultDelegate)
    {
      if (message == null)
        return;
      message.ReadInt32();
      int num = message.ReadInt32();
      float x = message.ReadFloat();
      float y = message.ReadFloat();
      float z = message.ReadFloat();
      bool flag = message.ReadBoolean();
      resultDelegate(num, new Vector3(x, y, z), flag);
    }

    protected override void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType)
    {
      if (dataType == NetworkPeer.MessageID.AddNewBlock)
      {
        ClientBlockDataHandler.ParseAddBlockFromMessage(message, this.blockAdded);
      }
      else
      {
        if (dataType != NetworkPeer.MessageID.RemoveBlock)
          return;
        ClientBlockDataHandler.ParseRemoveBlockFromMessage(message, this.blockRemoved);
      }
    }
  }
}
