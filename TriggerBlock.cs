// Decompiled with JetBrains decompiler
// Type: CornerSpace.TriggerBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public abstract class TriggerBlock : PowerBlock
  {
    protected ControlBlock controller;
    protected BlockSector ownerSector;
    protected Vector3 positionInShipSpace;
    protected bool triggered;

    public TriggerBlock(PowerBlockType creationParameters)
      : base(creationParameters)
    {
    }

    public override void Added(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
      this.PositionInShipSpace = positionInEntitySpace;
      this.ownerSector = sector;
    }

    public override void Removed(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
      this.Unbind();
    }

    public virtual void Trigger() => this.triggered = true;

    public virtual void Unbind()
    {
      if (this.controller == null)
        return;
      this.controller.UnbindBlock(this, true);
      this.controller = (ControlBlock) null;
    }

    public virtual void UnTrigger() => this.triggered = false;

    public virtual void UserUpdate(Camera userCamera)
    {
    }

    public ControlBlock Controller
    {
      get => this.controller;
      set => this.controller = value;
    }

    public BlockSector Owner
    {
      get => this.ownerSector;
      set => this.ownerSector = value;
    }

    public virtual Vector3 PositionInShipSpace
    {
      get => this.positionInShipSpace;
      set => this.positionInShipSpace = value;
    }

    public bool Triggered => this.triggered;
  }
}
