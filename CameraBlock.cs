// Decompiled with JetBrains decompiler
// Type: CornerSpace.CameraBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class CameraBlock : TriggerBlock
  {
    private Camera camera;

    public CameraBlock(PowerBlockType creationParameters)
      : base(creationParameters)
    {
      this.camera = new Camera()
      {
        CameraMode = Camera.Mode.Fixed_plane
      };
    }

    public override void Removed(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
      if (this.controller != null)
        this.controller.UnbindCamera(true);
      if (this.camera != null)
        this.camera.Dispose();
      base.Removed(entity, sector, positionInEntitySpace);
    }

    public void UpdateCameraPosition()
    {
      if (this.camera == null || this.ownerSector == null)
        return;
      this.camera.Position = this.ownerSector.Owner.EntityCoordsToWorldCoords(this.positionInShipSpace);
      if (this.ownerSector.Owner != null)
        this.camera.CoordinateSpace = Quaternion.Concatenate(this.OrientationQuaternion, this.ownerSector.Owner.Orientation);
      this.camera.UpdateMatrices();
    }

    public Camera Camera => this.camera;
  }
}
