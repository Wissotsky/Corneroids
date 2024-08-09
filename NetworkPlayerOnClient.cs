// Decompiled with JetBrains decompiler
// Type: CornerSpace.NetworkPlayerOnClient
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class NetworkPlayerOnClient : NetworkPlayer
  {
    public override void Update(
      float deltaTime,
      CharacterManager characterManager,
      SpaceEntityManager entityManager)
    {
      Astronaut astronaut = this.Astronaut;
      astronaut.MaximumSpeed = 4f;
      astronaut.Regen();
      if (astronaut.BoardedEntity != null)
        astronaut.CoordinateSpace = astronaut.BoardedEntity.Orientation;
      if (astronaut.StateBuffer.HasUnhandledStates())
      {
        StateFrame latestState = astronaut.StateBuffer.GetLatestState();
        this.SynchronizationValue.StoredValue = latestState.SynchronizationValue;
        this.SetPlayerBoardedState(ref latestState, entityManager);
        this.SetAstronautPosition(ref latestState, entityManager);
        this.SetAstronautOrientation(ref latestState, entityManager);
        astronaut.SetHealthButDontKill(latestState.Health);
        astronaut.Power = latestState.Power;
        this.inputFrame.SetUp(ref latestState);
        astronaut.UpdateItems();
        this.SetItemSynchronizationValue((long) this.SynchronizationValue.GetByte((byte) 0));
        this.UpdateActiveItem(latestState.ActiveItemId, ref latestState);
        characterManager.CheckPlayerEntityCollisions((Player) this, false);
        this.CheckIfBlockUsed(ref latestState);
      }
      else
      {
        astronaut.UpdatePositionAndOrientationToSpace();
        StateFrame latestState = astronaut.StateBuffer.GetLatestState();
        astronaut.UpdateItems();
        this.UpdateActiveItem(latestState.ActiveItemId, ref latestState);
      }
    }

    private void CheckIfBlockUsed(ref StateFrame frame)
    {
      Astronaut astronaut = this.Astronaut;
      if (!frame.Use || this.PreviousState.Use || this.PickedBlock == null || this.PickedBlock is ControlBlock || !this.PickedBlock.CanBeUsed)
        return;
      this.PickedBlock.UseButtonClicked((Player) this);
      this.OnUse(this.PickedBlock);
    }
  }
}
