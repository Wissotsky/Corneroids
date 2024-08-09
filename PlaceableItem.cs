// Decompiled with JetBrains decompiler
// Type: CornerSpace.PlaceableItem
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public abstract class PlaceableItem : Item
  {
    protected Vector3 itemSize;
    protected SpaceEntity pickedEntity;
    protected Vector3? pickedEntityNormal;
    protected Vector3? pickedEntityPosition;
    protected Position3? pickedWorldPosition;
    private Position3? previouslyPickedWorldPosition;
    private Stack<SpaceEntity> pickStack;

    public PlaceableItem()
    {
      this.itemSize = Vector3.One;
      this.pickStack = new Stack<SpaceEntity>();
    }

    public override void Update(Astronaut owner) => this.PickPlaceForItem((ICameraInterface) owner);

    public Vector3 SizeOfItem
    {
      set => this.itemSize = value;
    }

    private Vector3 GetPickedNormal(Vector3int blockPosition, Vector3int previousPosition)
    {
      if (blockPosition.X < previousPosition.X)
        return Vector3.UnitX;
      if (blockPosition.X > previousPosition.X)
        return -Vector3.UnitX;
      if (blockPosition.Y < previousPosition.Y)
        return Vector3.UnitY;
      if (blockPosition.Y > previousPosition.Y)
        return -Vector3.UnitY;
      if (blockPosition.Z < previousPosition.Z)
        return Vector3.UnitZ;
      return blockPosition.Z > previousPosition.Z ? -Vector3.UnitZ : Vector3.Zero;
    }

    private void PickPlaceForItem(ICameraInterface camera)
    {
      float num1 = 0.0f;
      Vector3 lookatVector = camera.GetLookatVector();
      float num2 = (float) Math.Sqrt(0.25 * (double) this.itemSize.X * (double) this.itemSize.X + 0.25 * (double) this.itemSize.Y * (double) this.itemSize.Y + 0.25 * (double) this.itemSize.Z * (double) this.itemSize.Z) + 2f;
      this.previouslyPickedWorldPosition = this.pickedWorldPosition;
      this.pickedEntityPosition = new Vector3?();
      this.pickedWorldPosition = new Position3?();
      this.pickedEntityNormal = new Vector3?();
      Position3 position = Position3.Zero;
      Vector3 zero = Vector3.Zero;
      for (; (double) num1 < (double) num2; num1 += 0.1f)
      {
        position = camera.Position + lookatVector * num1;
        SpaceEntity closestEntity = Item.entityManager.GetClosestEntity(position, 5f);
        this.pickStack.Push(closestEntity);
        if (closestEntity != null && closestEntity.GetBlockCell(position) != null)
        {
          position -= lookatVector * 0.1f;
          break;
        }
      }
      this.pickedWorldPosition = new Position3?(position);
      this.pickedEntity = this.pickStack.Pop();
      if (this.pickedEntity != null && this.pickedWorldPosition.HasValue)
      {
        this.pickedEntityPosition = new Vector3?(this.pickedEntity.WorldCoordsToEntityCoords(this.pickedWorldPosition.Value));
        Vector3 pickedNormal = this.GetPickedNormal(new Vector3int(this.pickedEntity.WorldCoordsToEntityCoords(this.pickedWorldPosition.Value + lookatVector * 0.1f)), new Vector3int(this.pickedEntityPosition.Value));
        this.pickedEntityPosition = new Vector3?(new Vector3((float) Math.Floor((double) this.pickedEntityPosition.Value.X), (float) Math.Floor((double) this.pickedEntityPosition.Value.Y), (float) Math.Floor((double) this.pickedEntityPosition.Value.Z)) + Vector3.One * 0.5f);
        if (pickedNormal != Vector3.Zero)
          this.pickedEntityNormal = new Vector3?(pickedNormal);
        else
          this.pickedEntityNormal = new Vector3?(-lookatVector);
      }
      else
      {
        if (!this.pickedWorldPosition.HasValue)
          return;
        this.pickedWorldPosition = new Position3?(Position3.Floor(this.pickedWorldPosition.Value) + Vector3.One * 0.5f);
      }
    }
  }
}
