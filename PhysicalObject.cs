// Decompiled with JetBrains decompiler
// Type: CornerSpace.PhysicalObject
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class PhysicalObject : IDisposable
  {
    private const float cMaxRotationSpeed = 1.5f;
    private readonly float cMaxSpeed = 20f;
    private BoundingSphereI boundingSphere;
    private float boundingRadius;
    protected long elapsedInterpolationTicks;
    private Vector3 inertiaTensor;
    private float mass;
    private Quaternion orientation;
    protected Position3 position;
    private Vector3 rotation;
    protected Vector3 speed;
    private StateFrameBuffer stateBuffer;
    protected Matrix inverseTransformMatrix;
    protected Matrix transformMatrix;

    public PhysicalObject()
    {
      this.elapsedInterpolationTicks = 0L;
      this.orientation = Quaternion.Identity;
      this.inertiaTensor = Vector3.Zero;
      this.mass = 1f;
      this.position = Position3.Zero;
      this.rotation = Vector3.Zero;
      this.speed = Vector3.Zero;
      this.inverseTransformMatrix = Matrix.Identity;
      this.transformMatrix = Matrix.Identity;
      this.stateBuffer = new StateFrameBuffer(64);
      if (!Engine.FastTravel)
        return;
      this.cMaxSpeed = 100f;
    }

    public virtual void ApplyForce(Vector3 point, Vector3 force, float deltaTime)
    {
    }

    public virtual void Dispose()
    {
    }

    public virtual bool GetCollision(PhysicalObject physicalObject, ref CollisionData collisionData)
    {
      throw new NotImplementedException();
    }

    public virtual Vector3 GetSpeedAt(Position3 position) => this.speed;

    public virtual void InterpolatePosition()
    {
      this.elapsedInterpolationTicks += Engine.FrameCounter.DeltaTimeMS;
      StateFrame stateFrame1 = this.stateBuffer.PeekLatestState();
      StateFrame stateFrame2 = this.stateBuffer.PeekHistoryState(1);
      int num = (int) (stateFrame1.Tick - stateFrame2.Tick);
      if (num <= 0)
        return;
      Vector3 vector3 = stateFrame1.Position - stateFrame2.Position;
      float amount = MathHelper.Clamp((float) this.elapsedInterpolationTicks / (float) num, 0.0f, 1f);
      this.Position = stateFrame2.Position + vector3 * amount;
      this.Orientation = Quaternion.Lerp(stateFrame2.Orientation, stateFrame1.Orientation, amount);
    }

    public virtual bool UpdateBasedOnLatestState()
    {
      if (!this.stateBuffer.HasUnhandledStates())
        return false;
      this.stateBuffer.GetLatestState();
      StateFrame stateFrame = this.stateBuffer.PeekHistoryState(1);
      this.Position = stateFrame.Position;
      this.Orientation = stateFrame.Orientation;
      this.elapsedInterpolationTicks = 0L;
      return true;
    }

    public virtual void UpdatePhysics(float deltaTime)
    {
      if ((double) this.speed.Length() > (double) this.cMaxSpeed)
        this.speed = Vector3.Normalize(this.speed) * this.cMaxSpeed;
      if ((double) this.rotation.Length() > 1.5)
        this.rotation = Vector3.Normalize(this.rotation) * 1.5f;
      this.orientation.Normalize();
    }

    public virtual BoundingSphereI BoundingSphere
    {
      get => this.boundingSphere;
      protected set => this.boundingSphere = value;
    }

    public float BoundingRadius
    {
      get => this.boundingRadius;
      set => this.boundingRadius = value;
    }

    public Vector3 InertiaTensor
    {
      get => this.inertiaTensor;
      set => this.inertiaTensor = value;
    }

    public Quaternion Orientation
    {
      get => this.orientation;
      set => this.orientation = value;
    }

    public Vector3 Rotation
    {
      get => this.rotation;
      set => this.rotation = value;
    }

    public Quaternion RotationQuaternion
    {
      get
      {
        return this.rotation == Vector3.Zero ? Quaternion.Identity : Quaternion.CreateFromAxisAngle(Vector3.Normalize(this.Rotation), this.Rotation.Length() * Engine.FrameCounter.DeltaTime);
      }
    }

    public virtual Position3 Position
    {
      get => this.position;
      set
      {
        this.position = value;
        this.boundingSphere.Center = value;
      }
    }

    public virtual Vector3 Speed
    {
      get => this.speed;
      set => this.speed = value;
    }

    public StateFrameBuffer StateBuffer => this.stateBuffer;

    public Matrix InverseTransformMatrix => this.inverseTransformMatrix;

    public Matrix TransformMatrix => this.transformMatrix;

    public float Mass
    {
      get => this.mass;
      set => this.mass = value;
    }
  }
}
