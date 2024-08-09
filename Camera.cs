// Decompiled with JetBrains decompiler
// Type: CornerSpace.Camera
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class Camera : PhysicalObject, IRenderCamera, ICameraInterface
  {
    private BoundingFrustum boundingFrustum;
    private Camera.Mode mode;
    private Matrix projectionMatrix;
    private Matrix viewMatrix;
    private Matrix viewMatrixModification;
    private float yaw;
    private float pitch;
    private Quaternion coordinateSpace;

    public Camera(float aspectRatio, float fov, float nearPlane, float farPlane)
    {
      this.UpdateViewMatrix();
      this.SetAttributes(aspectRatio, fov, nearPlane, farPlane);
      this.coordinateSpace = Quaternion.Identity;
      this.mode = Camera.Mode.Space;
      this.viewMatrixModification = Matrix.Identity;
      this.boundingFrustum = new BoundingFrustum(this.viewMatrix * this.projectionMatrix);
    }

    public Camera()
    {
      SettingsManager settingsManager = Engine.SettingsManager;
      this.SetAttributes(settingsManager.AspectRatio, settingsManager.CameraFoV, 0.1f, 800f);
      this.UpdateViewMatrix();
      this.coordinateSpace = Quaternion.Identity;
      this.mode = Camera.Mode.Space;
      this.viewMatrixModification = Matrix.Identity;
      this.boundingFrustum = new BoundingFrustum(this.viewMatrix * this.projectionMatrix);
      Engine.SettingsManager.CameraAttributesChanged += new System.Action(this.CameraAttributesChanged);
    }

    public override void Dispose()
    {
      Engine.SettingsManager.CameraAttributesChanged -= new System.Action(this.CameraAttributesChanged);
    }

    public Vector3 GetLookatVector() => Vector3.Transform(-Vector3.UnitZ, this.WorldOrientation);

    public Vector2? GetPositionInScreenSpace(Position3 worldPosition)
    {
      Vector3 relativeToCamera = this.GetPositionRelativeToCamera(worldPosition);
      Vector3 vector3 = Engine.GraphicsDevice.Viewport.Project(relativeToCamera, this.projectionMatrix, this.viewMatrix, Matrix.Identity);
      return (double) vector3.Z < 0.0 || (double) vector3.Z > 1.0 ? new Vector2?() : new Vector2?(new Vector2(vector3.X, vector3.Y));
    }

    public Vector3 GetPositionRelativeToCamera(Position3 position) => position - this.Position;

    public Vector3 GetStrafeVector()
    {
      return Vector3.Transform(Vector3.UnitX, Quaternion.Concatenate(this.Orientation, this.coordinateSpace));
    }

    public Vector3 GetUpVector() => Vector3.Cross(this.GetStrafeVector(), this.GetLookatVector());

    public void LookAt(Vector3 direction)
    {
      if (direction == Vector3.Zero)
        return;
      direction = Vector3.Normalize(direction);
      direction = Vector3.Transform(direction, Quaternion.Conjugate(this.CoordinateSpace));
      this.yaw = (float) Math.Atan2(-(double) direction.X, -(double) direction.Z);
      this.pitch = (float) Math.Asin((double) direction.Y);
      this.Orientation = Quaternion.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f);
      this.UpdateMatrices();
    }

    public void LookAtPoint(Position3 point) => this.LookAt(point - this.Position);

    public void Move(float amount)
    {
      Camera camera = this;
      camera.Position = camera.Position + this.GetLookatVector() * amount;
      this.UpdateMatrices();
    }

    public void ResetDirection()
    {
      this.yaw = this.pitch = 0.0f;
      this.UpdateQuaternions(this.yaw, this.pitch, 0.0f);
      this.UpdateMatrices();
    }

    public void Roll(float amount) => this.UpdateQuaternions(0.0f, 0.0f, amount);

    public void Rotate(float yaw, float pitch)
    {
      this.pitch += pitch;
      this.yaw += yaw;
      this.pitch = Math.Max(Math.Min(this.pitch, 1.57079637f), -1.57079637f);
      this.yaw = MathHelper.WrapAngle(this.yaw);
      this.UpdateQuaternions(yaw, pitch, 0.0f);
      this.UpdateMatrices();
    }

    public void SetAttributes(float aspectRatio, float fov, float nearPlane, float farPlane)
    {
      this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane);
    }

    public virtual void Update(float deltaTime)
    {
      this.coordinateSpace.Normalize();
      this.UpdateCamera();
    }

    protected void UpdateCamera() => this.UpdateMatrices();

    public BoundingFrustum ViewFrustum => this.boundingFrustum;

    public Matrix ProjectionMatrix
    {
      get => this.projectionMatrix;
      set => this.projectionMatrix = value;
    }

    public Matrix ViewMatrix => this.viewMatrix;

    public Matrix ViewMatrixModification
    {
      set => this.viewMatrixModification = value;
    }

    public override Position3 Position
    {
      get => base.Position;
      set
      {
        base.Position = value;
        this.UpdateViewMatrix();
        this.UpdateBoundingFrustum(this.boundingFrustum, ref this.viewMatrix, ref this.projectionMatrix);
      }
    }

    public Quaternion CoordinateSpace
    {
      get => this.coordinateSpace;
      set => this.coordinateSpace = value;
    }

    public Camera.Mode CameraMode
    {
      get => this.mode;
      set => this.mode = value;
    }

    public Quaternion WorldOrientation
    {
      get => Quaternion.Concatenate(this.Orientation, this.coordinateSpace);
    }

    private void UpdateBoundingFrustum(
      BoundingFrustum frustum,
      ref Matrix view,
      ref Matrix projection)
    {
      frustum.Matrix = view * projection;
    }

    public void UpdateMatrices()
    {
      this.UpdateViewMatrix();
      this.UpdateBoundingFrustum(this.boundingFrustum, ref this.viewMatrix, ref this.projectionMatrix);
    }

    private void UpdateQuaternions(float deltaYaw, float deltaPitch, float deltaRoll)
    {
      switch (this.mode)
      {
        case Camera.Mode.Fixed_plane:
          this.Orientation = Quaternion.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f);
          break;
        case Camera.Mode.Space:
          Camera camera = this;
          // ISSUE: explicit non-virtual call
          camera.Orientation = __nonvirtual (camera.Orientation) * Quaternion.CreateFromYawPitchRoll(deltaYaw, deltaPitch, deltaRoll);
          break;
      }
    }

    private void UpdateViewMatrix()
    {
      this.viewMatrix = Matrix.CreateFromQuaternion(Quaternion.Conjugate(this.coordinateSpace)) * Matrix.CreateFromQuaternion(Quaternion.Conjugate(this.Orientation)) * this.viewMatrixModification;
    }

    private void CameraAttributesChanged()
    {
      SettingsManager settingsManager = Engine.SettingsManager;
      this.SetAttributes(settingsManager.AspectRatio, settingsManager.CameraFoV, 0.1f, 800f);
      this.boundingFrustum = new BoundingFrustum(this.viewMatrix * this.projectionMatrix);
    }

    public enum Mode
    {
      Fixed_plane,
      Space,
    }
  }
}
