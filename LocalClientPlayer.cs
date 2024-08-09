// Decompiled with JetBrains decompiler
// Type: CornerSpace.LocalClientPlayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class LocalClientPlayer : LocalPlayer
  {
    private StateFrameBuffer inputHistory;
    private bool inputUpdated;

    public LocalClientPlayer()
    {
      this.inputHistory = new StateFrameBuffer(64);
      this.inputUpdated = false;
    }

    public override void Update(
      float deltaTime,
      CharacterManager characterManager,
      SpaceEntityManager entityManager)
    {
      this.Astronaut.MaximumSpeed = 4f;
      this.CorrectPlayerPosition(deltaTime, characterManager, entityManager);
      this.PredictUpdate(characterManager);
      this.UpdateDamageTakenMarker();
      if (!Engine.SettingsManager.ViewBobbing)
        return;
      this.UpdateCameraShaking();
    }

    public override void UpdateInput(InputFrame input)
    {
      ControlsManager controlsManager = Engine.SettingsManager.ControlsManager;
      Astronaut astronaut = this.Astronaut;
      this.Astronaut.UsedCamera.Rotate((float) (-(double) input.TranslationX * (double) controlsManager.MouseSensitivity * 0.0099999997764825821), (float) (-(double) input.TranslationY * (double) controlsManager.MouseSensitivity * 0.0099999997764825821));
      this.SaveInputAsLatestState(input);
      if (this.Astronaut.MountedTo != null)
      {
        if (this.Astronaut.MountedTo is ControlBlock mountedTo)
          this.LatestStateFrame.SetUpControlButtons(mountedTo, input);
      }
      else if (input.RightClick)
      {
        astronaut.ActiveItem = (Item) null;
        this.Toolbar.SelectedIndex = (sbyte) -1;
      }
      if (input.KeyClicked(ControlsManager.SpecialKey.Inventory))
        this.OpenCharacterScreen();
      if (input.KeyClicked(ControlsManager.SpecialKey.Edit) && astronaut.MountedTo == null && this.PickedBlock != null)
        this.PickedBlock.EditButtonClicked((Player) this);
      if (input.KeyClicked(ControlsManager.SpecialKey.Use) && astronaut.MountedTo == null && this.PickedBlock != null && !(this.PickedBlock is ControlBlock) && this.PickedBlock.CanBeUsed)
        this.PickedBlock.UseButtonClicked((Player) this);
      this.UpdateToolbarItem(input);
      this.SetItemSynchronizationValue(this.LatestStateFrame.Tick);
      this.UpdateActiveItem(input);
    }

    private void CorrectPlayerPosition(
      float deltaTime,
      CharacterManager characterManager,
      SpaceEntityManager entityManager)
    {
      Astronaut astronaut = this.Astronaut;
      if (!astronaut.StateBuffer.HasUnhandledStates())
        return;
      StateFrame latestState = astronaut.StateBuffer.GetLatestState();
      this.SetAstronautStateFromState(ref latestState, entityManager);
      astronaut.SetHealthButDontKill(latestState.Health);
      astronaut.Power = latestState.Power;
      this.inputHistory.RewindToTime(latestState.Tick);
      while (this.inputHistory.GetOldestUnhandledState(ref latestState))
      {
        this.SetAstronautOrientation(ref latestState, entityManager);
        this.InputStep(ref latestState);
        this.Astronaut.UpdatePhysics(latestState.SimulationTime);
        characterManager.CheckPlayerEntityCollisions((Player) this, false);
      }
    }

    private void CreateDummyState()
    {
      Astronaut astronaut = this.Astronaut;
      StateFrame newState = new StateFrame()
      {
        Orientation = astronaut.Orientation,
        ActiveToolbarIndex = this.Toolbar.SelectedIndex,
        ActiveItemId = astronaut.ActiveItem != (Item) null ? astronaut.ActiveItem.ItemId : -1,
        Position = astronaut.Position,
        SimulationTime = Engine.FrameCounter.DeltaTime,
        Tick = Engine.FrameCounter.Ticks,
        Power = astronaut.Power
      };
      this.LatestStateFrame = newState;
      this.inputHistory.AddState(newState);
    }

    protected override ItemSlot PopItemFromToolbar()
    {
      return this.Toolbar.Items.PopItem((int) this.Toolbar.SelectedIndex, 0, 1, false);
    }

    private void PredictUpdate(CharacterManager characterManager)
    {
      Astronaut astronaut = this.Astronaut;
      if (!this.inputUpdated)
        this.CreateDummyState();
      this.inputUpdated = false;
      StateFrame latestStateFrame = this.LatestStateFrame;
      this.InputStep(ref latestStateFrame);
      astronaut.Update(latestStateFrame.SimulationTime);
      this.UpdateHealthAndPowerBars();
      characterManager.CheckPlayerEntityCollisions((Player) this, false);
    }

    private void SaveInputAsLatestState(InputFrame frame)
    {
      Astronaut astronaut = this.Astronaut;
      StateFrame newState = new StateFrame(frame)
      {
        BoardedEntityId = astronaut.BoardedEntity != null ? astronaut.BoardedEntity.Id : -1,
        Orientation = astronaut.UsedCamera.Orientation,
        ActiveToolbarIndex = this.Toolbar.SelectedIndex,
        ActiveItemId = astronaut.ActiveItem != (Item) null ? astronaut.ActiveItem.ItemId : -1,
        Position = astronaut.Position,
        PositionInEntitySpace = astronaut.PositionOnBoardedEntity,
        Power = astronaut.Power,
        SimulationTime = Engine.FrameCounter.DeltaTime,
        Tick = Engine.FrameCounter.Ticks,
        Speed = astronaut.Speed,
        SpeedInEntitySpace = astronaut.SpeedOnBoardedEntity
      };
      this.LatestStateFrame = newState;
      this.inputHistory.AddState(newState);
      this.inputUpdated = true;
    }

    private void SetAstronautStateFromState(ref StateFrame state, SpaceEntityManager entityManager)
    {
      Astronaut astronaut = this.Astronaut;
      this.SetPlayerBoardedState(ref state, entityManager);
      this.SetAstronautOrientation(ref state, entityManager);
      this.SetAstronautPosition(ref state, entityManager);
    }
  }
}
