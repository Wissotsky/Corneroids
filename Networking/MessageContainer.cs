// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.MessageContainer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;

#nullable disable
namespace CornerSpace.Networking
{
  public struct MessageContainer
  {
    public NetOutgoingMessage Message;
    public NetConnection Receiver;
    public NetDeliveryMethod SendMethod;
    public int SequenceChannel;
  }
}
