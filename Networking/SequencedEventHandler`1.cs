// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.SequencedEventHandler`1
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Lidgren.Network;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Networking
{
  public abstract class SequencedEventHandler<Y> : EventHandler<NetConnection, Y> where Y : INetwork
  {
    private List<SequencedEventHandler<Y>.SequenceFunction> sequenceFunctions;
    private int currentState;

    public SequencedEventHandler(NetConnection key, Y networkInterface)
      : base(key, networkInterface)
    {
      this.sequenceFunctions = new List<SequencedEventHandler<Y>.SequenceFunction>();
      this.currentState = 0;
    }

    public SequencedEventHandler<Y> AddNewSequenceFunction(
      string comment,
      NetIncomingMessageType expectedMessages,
      Action<NetIncomingMessage> sequenceFunction)
    {
      if (sequenceFunction != null)
        this.sequenceFunctions.Add(new SequencedEventHandler<Y>.SequenceFunction()
        {
          ExceptedMessages = expectedMessages,
          Function = sequenceFunction
        });
      return this;
    }

    public override sealed void Handle(NetIncomingMessage message)
    {
      if (this.currentState < this.sequenceFunctions.Count)
      {
        message.Position = 0L;
        SequencedEventHandler<Y>.SequenceFunction sequenceFunction = this.sequenceFunctions[this.currentState];
        if (sequenceFunction.ExceptedMessages == message.MessageType)
          sequenceFunction.Function(message);
      }
      message.Position = 0L;
      this.HandleMessageOutsideOfSequence(message);
    }

    protected virtual void HandleMessageOutsideOfSequence(NetIncomingMessage message)
    {
    }

    protected void MoveToNextState()
    {
      ++this.currentState;
      if (this.currentState < this.sequenceFunctions.Count)
        return;
      this.Unregister();
    }

    private class SequenceFunction
    {
      public Action<NetIncomingMessage> Function;
      public NetIncomingMessageType ExceptedMessages;
    }
  }
}
