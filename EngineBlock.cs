// Decompiled with JetBrains decompiler
// Type: CornerSpace.EngineBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class EngineBlock : TriggerBlock
  {
    protected Vector3 throttleDirection;
    protected float throttleForce;

    public EngineBlock(EngineBlockType creationParameters)
      : base((PowerBlockType) creationParameters)
    {
      this.throttleForce = creationParameters.ThrottleForce;
      this.throttleDirection = Vector3.Down;
    }

    public override void Rotate(Vector3 rotation)
    {
      base.Rotate(rotation);
      this.throttleDirection = Vector3.Transform(Vector3.Down, this.OrientationMatrix);
    }

    public override void Trigger()
    {
      if (!this.activated)
        return;
      base.Trigger();
      if (this.ownerSector == null || this.ownerSector.Owner == null)
        return;
      this.ownerSector.Owner.ApplyForce(this.positionInShipSpace, this.throttleDirection * this.throttleForce, Engine.FrameCounter.DeltaTime);
    }

    public override byte Orientation
    {
      get => base.Orientation;
      set
      {
        base.Orientation = value;
        this.throttleDirection = Vector3.Transform(Vector3.Down, this.OrientationMatrix);
      }
    }
  }
}
