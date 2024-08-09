// Decompiled with JetBrains decompiler
// Type: CornerSpace.GunBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class GunBlock : TriggerBlock, IDynamicBlock
  {
    private BlockSector.ChangeUpdateState updateState;
    private GunBlockType gunBlockType;
    private Quaternion userOrientation;
    private short readyToFire;
    public static ProjectileManager ProjectileManager;
    private static Quaternion initializationQuaternion = Quaternion.CreateFromYawPitchRoll(0.0f, -1.57079637f, 0.0f);

    public GunBlock(GunBlockType creationParameters)
      : base((PowerBlockType) creationParameters)
    {
      this.gunBlockType = creationParameters;
      this.readyToFire = (short) 0;
      this.userOrientation = Quaternion.Identity;
    }

    public void SetUpdateInterface(BlockSector.ChangeUpdateState updateStateFunction)
    {
      this.updateState = updateStateFunction;
    }

    public override void Trigger()
    {
      if (!this.activated || this.readyToFire > (short) 0)
        return;
      Quaternion orientationQuaternion = this.OrientationQuaternion;
      SpaceEntity owner = this.Owner.Owner;
      Vector3 normal = Vector3.Transform(Vector3.Transform(Vector3.Up, this.OrientationMatrix), this.userOrientation);
      ProjectileType projectileType = Engine.LoadedWorld.Itemset.ProjectileTypes[(int) this.gunBlockType.ProjectileId];
      if (projectileType != null)
      {
        Vector3 vector3 = Vector3.Transform(this.positionInShipSpace + normal * this.gunBlockType.BarrelLength, owner.TransformMatrix);
        Vector3 direction = Vector3.TransformNormal(normal, owner.TransformMatrix);
        Projectile projectile = new Projectile(projectileType, owner.Position + vector3, direction);
        projectile.ShotBy = (PhysicalObject) owner;
        if (GunBlock.ProjectileManager != null)
          GunBlock.ProjectileManager.AddProjectile(projectile);
      }
      this.readyToFire = this.gunBlockType.ReloadTime;
      this.updateState((IDynamicBlock) this, true);
    }

    public void Update()
    {
      if (this.readyToFire > (short) 0)
      {
        this.readyToFire -= (short) Engine.FrameCounter.DeltaTimeMS;
      }
      else
      {
        this.readyToFire = (short) 0;
        this.updateState((IDynamicBlock) this, false);
      }
    }

    public override void UserUpdate(Camera userCamera)
    {
      this.userOrientation = Quaternion.Concatenate(Quaternion.Concatenate(Quaternion.Conjugate(this.OrientationQuaternion), GunBlock.initializationQuaternion), Quaternion.Concatenate(Quaternion.Concatenate(userCamera.Orientation, userCamera.CoordinateSpace), Quaternion.Conjugate(this.ownerSector.Owner.Orientation)));
    }

    public override bool HasDynamicBehavior => true;

    public Matrix TransformMatrix
    {
      get
      {
        return this.userOrientation == Quaternion.Identity ? Matrix.Identity : Matrix.CreateTranslation(-this.positionInShipSpace) * Matrix.CreateFromQuaternion(this.userOrientation) * Matrix.CreateTranslation(this.positionInShipSpace);
      }
    }

    public override Vector3 PositionInShipSpace
    {
      get => base.PositionInShipSpace;
      set => base.PositionInShipSpace = value;
    }
  }
}
