// Decompiled with JetBrains decompiler
// Type: CornerSpace.NetworkPlayerOnServer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class NetworkPlayerOnServer : NetworkPlayer
  {
    private short latestControlButtonStates;

    public override bool CanSeeEntity(SpaceEntity entity, float squaredDistanceLimit)
    {
      return entity != null && this.WorldView.Contains(entity.Id);
    }

    public override void Update(
      float deltaTime,
      CharacterManager characterManager,
      SpaceEntityManager entityManager)
    {
      Astronaut astronaut = this.Astronaut;
      astronaut.MaximumSpeed = 4f;
      if (astronaut.StateBuffer.HasUnhandledStates())
      {
        StateFrame stateFrame = new StateFrame();
        long num = 0;
        while (astronaut.StateBuffer.GetOldestUnhandledState(ref stateFrame))
        {
          astronaut.PowerPreUpdate = astronaut.Power;
          this.inputFrame.SetUp(ref stateFrame);
          this.SetAstronautOrientation(ref stateFrame, entityManager);
          this.InputStep(ref stateFrame);
          this.SetItemSynchronizationValue(stateFrame.Tick);
          this.UpdateActiveItemFromToolbarIndex(stateFrame.ActiveToolbarIndex, ref stateFrame);
          astronaut.Update(stateFrame.SimulationTime);
          num = stateFrame.Tick;
          characterManager.CheckPlayerEntityCollisions((Player) this, true);
        }
        StateFrame latestState = astronaut.StateBuffer.GetLatestState();
        this.UpdateInteractionInput(ref latestState);
        this.LatestMessageTicks = num;
        this.latestControlButtonStates = latestState.ControlButtonsStates;
      }
      else
      {
        astronaut.UpdatePositionAndOrientationToSpace();
        this.UpdateMountedBlock(astronaut, this.latestControlButtonStates);
      }
    }

    private void UpdateActiveItemFromToolbarIndex(sbyte index, ref StateFrame frame)
    {
      if (Engine.LoadedWorld == null)
        return;
      Astronaut astronaut = this.Astronaut;
      Item activeItem = astronaut.ActiveItem;
      Item obj = (Item) null;
      this.Toolbar.SelectedIndex = index;
      ItemSlot itemSlot = this.Toolbar.PeekItem((int) index);
      if (itemSlot != null)
        obj = itemSlot.Item;
      if (activeItem != obj)
      {
        if (activeItem == (Item) null && obj != (Item) null)
          astronaut.ActiveItem = obj;
        else if (activeItem != (Item) null && obj == (Item) null)
          astronaut.ActiveItem = (Item) null;
        else if (activeItem != (Item) null && obj != (Item) null && activeItem.ItemId != obj.ItemId)
          astronaut.ActiveItem = obj;
      }
      if (!(activeItem != (Item) null))
        return;
      Item.UsageResult usageResult = activeItem.UpdateInput(this.inputFrame, (Player) this, astronaut.Power);
      if ((usageResult & Item.UsageResult.Consumed) > Item.UsageResult.None)
      {
        this.Toolbar.PopItems((int) this.Toolbar.SelectedIndex, 1);
        if (this.Toolbar.PeekItem((int) this.Toolbar.SelectedIndex) == null)
          astronaut.ActiveItem = (Item) null;
      }
      if ((usageResult & Item.UsageResult.Power_used) <= Item.UsageResult.None)
        return;
      astronaut.Power -= (float) activeItem.PowerUsage;
    }
  }
}
