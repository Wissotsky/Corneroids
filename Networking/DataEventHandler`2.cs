// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.DataEventHandler`2
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;

#nullable disable
namespace CornerSpace.Networking
{
  public abstract class DataEventHandler<T, Y> : EventHandler<T, Y>
    where T : class
    where Y : INetwork
  {
    private NetworkPeer.MessageID[] expectedData;

    public DataEventHandler(T key, Y netInterface, params NetworkPeer.MessageID[] expectedData)
      : base(key, netInterface)
    {
      this.expectedData = expectedData;
    }

    public override sealed void Handle(NetIncomingMessage message)
    {
      if (message.MessageType != NetIncomingMessageType.Data)
        return;
      NetworkPeer.MessageID messageId = (NetworkPeer.MessageID) message.ReadByte();
      for (int index = 0; index < this.expectedData.Length; ++index)
      {
        if (this.expectedData[index] == messageId)
          this.HandleDataMessage(message, this.expectedData[index]);
      }
    }

    protected abstract void HandleDataMessage(
      NetIncomingMessage message,
      NetworkPeer.MessageID dataType);
  }
}
