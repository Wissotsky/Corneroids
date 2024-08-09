// Decompiled with JetBrains decompiler
// Type: CornerSpace.DoorBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class DoorBlock : BasicBlock, IDynamicBlock
  {
    private BlockSector.ChangeUpdateState updateState;
    private Vector3 openingDirectionInShipSpace;
    private float doorPosition;
    private DoorBlock.State doorState;
    private int timeUntilClose;
    private Matrix transformMatrix;

    public DoorBlock(DoorBlockType creationParameters)
      : base((BasicBlockType) creationParameters)
    {
      this.openingDirectionInShipSpace = Vector3.Normalize(creationParameters.OpeningDirection);
      this.timeUntilClose = 0;
      this.transformMatrix = Matrix.Identity;
      this.doorPosition = 0.0f;
      this.doorState = DoorBlock.State.close;
    }

    public override void CollidingWithObject(Vector3 collisionNormal)
    {
      switch ((int) Math.Round((double) Vector3.Dot(collisionNormal, this.openingDirectionInShipSpace)))
      {
        case -1:
          if (this.doorState != DoorBlock.State.opening)
            break;
          this.doorState = DoorBlock.State.closing;
          break;
        case 1:
          if (this.doorState != DoorBlock.State.closing)
            break;
          this.doorState = DoorBlock.State.opening;
          break;
      }
    }

    public override void UseButtonClicked(Player player)
    {
      this.doorState = DoorBlock.State.opening;
      this.updateState((IDynamicBlock) this, true);
    }

    public override void Rotate(Vector3 rotation)
    {
      base.Rotate(rotation);
      this.openingDirectionInShipSpace = Vector3.Transform(((DoorBlockType) this.GetBlockType()).OpeningDirection, this.OrientationMatrix);
    }

    public void SetUpdateInterface(BlockSector.ChangeUpdateState updateStateFunction)
    {
      this.updateState = updateStateFunction;
    }

    public void Update()
    {
      DoorBlockType blockType = this.GetBlockType() as DoorBlockType;
      switch (this.doorState)
      {
        case DoorBlock.State.open:
          this.timeUntilClose -= (int) ((double) Engine.FrameCounter.DeltaTime * 1000.0);
          if (this.timeUntilClose > 0)
            break;
          this.timeUntilClose = 0;
          this.doorState = DoorBlock.State.closing;
          break;
        case DoorBlock.State.opening:
          this.doorPosition += blockType.OpeningSpeed * Engine.FrameCounter.DeltaTime;
          if ((double) this.doorPosition >= (double) blockType.OpeningDistance)
          {
            this.doorPosition = blockType.OpeningDistance;
            this.doorState = DoorBlock.State.open;
            this.timeUntilClose = blockType.TimeToRemainOpen;
          }
          this.transformMatrix = Matrix.CreateTranslation(this.openingDirectionInShipSpace * this.doorPosition);
          break;
        case DoorBlock.State.closing:
          this.doorPosition -= blockType.OpeningSpeed * Engine.FrameCounter.DeltaTime;
          if ((double) this.doorPosition <= 0.0)
          {
            this.doorPosition = 0.0f;
            this.doorState = DoorBlock.State.close;
            this.updateState((IDynamicBlock) this, false);
          }
          this.transformMatrix = Matrix.CreateTranslation(this.openingDirectionInShipSpace * this.doorPosition);
          break;
      }
    }

    public override bool CanBeUsed => true;

    public bool DynamicMovement => true;

    public override Vector3 ModelPlacement
    {
      get
      {
        return Vector3.Transform(this.GetBlockType().ModelPlacement, this.OrientationQuaternion) + this.openingDirectionInShipSpace * this.doorPosition;
      }
    }

    public override bool HasDynamicBehavior => true;

    public override byte Orientation
    {
      get => base.Orientation;
      set
      {
        base.Orientation = value;
        this.openingDirectionInShipSpace = Vector3.Transform(((DoorBlockType) this.GetBlockType()).OpeningDirection, this.OrientationMatrix);
      }
    }

    public Matrix TransformMatrix => this.transformMatrix;

    public override bool Transparent => true;

    public override byte Visibility => 63;

    private enum State : byte
    {
      open,
      close,
      opening,
      closing,
    }
  }
}
