// Decompiled with JetBrains decompiler
// Type: CornerSpace.ContainerBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class ContainerBlock : BasicBlock
  {
    private Inventory inventory;
    private SpaceEntity ownerEntity;
    private Vector3 positionInEntitySpace;

    public ContainerBlock(ContainerBlockType parameters)
      : base((BasicBlockType) parameters)
    {
      this.inventory = new Inventory((uint) parameters.Width, (uint) parameters.Height, (Player) null);
    }

    public override void Added(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
      sector.ContainerBlocks.Add(this);
      this.positionInEntitySpace = positionInEntitySpace;
      this.ownerEntity = entity;
    }

    public override void Removed(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
      sector.ContainerBlocks.Remove(this);
      this.ThrowItemsInside();
    }

    public override bool CanBeUsed => true;

    public Inventory Inventory => this.inventory;

    public SpaceEntity OwnerEntity => this.ownerEntity;

    public Vector3 PositionInEntitySpace => this.positionInEntitySpace;

    public override void UseButtonClicked(Player player)
    {
      if (!(player is LocalPlayer))
        return;
      Engine.LoadNewScreen((GameScreen) new ContainerBlockScreen(player as LocalPlayer, this));
    }

    private void ThrowItemsInside()
    {
      EnvironmentManager environmentManager = SpaceScreen.EnvironmentManager;
      if (environmentManager == null)
        return;
      Position3 worldCoords = this.ownerEntity.EntityCoordsToWorldCoords(this.positionInEntitySpace);
      Random random = new Random();
      foreach (ItemSlot itemSlot in this.inventory)
      {
        Vector3 randomUnitDirection = this.GetRandomUnitDirection(random);
        float num = (float) (random.NextDouble() * 3.0 + 1.0);
        environmentManager.AddFloatingItem(new ItemSlot(itemSlot.Item, itemSlot.Count), worldCoords, num * randomUnitDirection, (ushort) 1000);
      }
    }

    private Vector3 GetRandomUnitDirection(Random random)
    {
      if (random == null)
        return Vector3.Zero;
      Vector3 vector3 = new Vector3((float) random.NextDouble() - 0.5f, (float) random.NextDouble() - 0.5f, (float) random.NextDouble() - 0.5f);
      return !(vector3 != Vector3.Zero) ? Vector3.Zero : Vector3.Normalize(vector3);
    }
  }
}
