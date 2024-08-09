// Decompiled with JetBrains decompiler
// Type: CornerSpace.LightBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class LightBlock : TriggerBlock, IDynamicBlock
  {
    private short elapsedTime;
    private bool enabled;
    private byte lightSequence;
    private LightSource lightSource;
    protected BlockSector.ChangeUpdateState updateState;

    public LightBlock(LightBlockType creationParameters)
      : base((PowerBlockType) creationParameters)
    {
      this.lightSequence = byte.MaxValue;
      this.elapsedTime = (short) 0;
      this.enabled = true;
    }

    public override void CreateBlock(
      Byte3 position,
      out BlockVertex[] vertices,
      out short[] indices)
    {
      Byte3 sizeModifier = this.RotateSizeVector(new Byte3((byte) 2, (byte) 4, (byte) 2));
      Vector3 vector3 = Vector3.Transform(new Vector3(0.0f, -2f, 0.0f), this.OrientationQuaternion);
      vector3 = new Vector3((float) (int) Math.Round((double) vector3.X), (float) (int) Math.Round((double) vector3.Y), (float) (int) Math.Round((double) vector3.Z));
      Byte3 positionModifier = (Byte3) vector3 + new Byte3((byte) 4, (byte) 4, (byte) 4);
      BlockBuilder.Factory.ConstructBlock((Block) this, position, sizeModifier, positionModifier, out vertices, out indices);
    }

    public override void EditButtonClicked(Player player)
    {
      Engine.LoadNewScreen((GameScreen) new EditLightBlockScreen(this));
    }

    public override void Trigger()
    {
      if (!this.triggered && this.lightSource != null)
      {
        this.lightSource.Enabled = this.enabled = !this.enabled;
        if (this.lightSequence != byte.MaxValue)
          this.updateState((IDynamicBlock) this, this.enabled);
      }
      base.Trigger();
    }

    public void SetUpdateInterface(BlockSector.ChangeUpdateState updateFunction)
    {
      this.updateState = updateFunction;
    }

    public override void UnTrigger() => base.UnTrigger();

    public virtual void Update()
    {
      this.elapsedTime = (short) (((int) this.elapsedTime + (int) Engine.FrameCounter.DeltaTimeMS) % 2000);
      byte num = (byte) (128U >> (int) (byte) ((uint) this.elapsedTime / 250U));
      this.lightSource.Enabled = this.activated && this.enabled && ((int) num & (int) this.lightSequence) != 0;
    }

    public override bool HasCollision => false;

    public override bool HasDynamicBehavior => false;

    public byte LightSequence
    {
      get => this.lightSequence;
      set
      {
        this.lightSequence = value;
        if (this.lightSequence == byte.MaxValue)
          this.updateState((IDynamicBlock) this, false);
        else
          this.updateState((IDynamicBlock) this, true);
      }
    }

    public LightSource LightSource
    {
      get => this.lightSource;
      set => this.lightSource = value;
    }

    public override Vector3 PositionInShipSpace
    {
      get => base.PositionInShipSpace;
      set
      {
        base.PositionInShipSpace = value;
        if (this.lightSource == null)
          return;
        this.lightSource.PositionInEntitySpace = value;
      }
    }

    public virtual Matrix TransformMatrix => Matrix.Identity;

    public override bool Transparent => true;
  }
}
