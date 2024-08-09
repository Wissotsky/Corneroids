// Decompiled with JetBrains decompiler
// Type: CornerSpace.KeyBinding
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class KeyBinding : IDisposable
  {
    private ControlKey key;
    private HashSet<TriggerBlock> connectedTriggerBlocks;

    public KeyBinding(ControlKey key)
    {
      this.key = key;
      this.connectedTriggerBlocks = new HashSet<TriggerBlock>();
    }

    public void Activate(IController<Astronaut> controller)
    {
      if (this.key.Pressed())
      {
        foreach (TriggerBlock connectedTriggerBlock in this.connectedTriggerBlocks)
          connectedTriggerBlock.Trigger();
      }
      else
      {
        foreach (TriggerBlock connectedTriggerBlock in this.connectedTriggerBlocks)
          connectedTriggerBlock.UnTrigger();
      }
    }

    public bool BindBlock(TriggerBlock block) => this.connectedTriggerBlocks.Add(block);

    public void Dispose() => this.connectedTriggerBlocks.Clear();

    public bool IsBlockBound(TriggerBlock block) => this.connectedTriggerBlocks.Contains(block);

    public bool UnbindBlock(TriggerBlock block) => this.connectedTriggerBlocks.Remove(block);

    public ControlKey ControlKey => this.key;
  }
}
