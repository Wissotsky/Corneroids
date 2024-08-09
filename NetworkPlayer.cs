// Decompiled with JetBrains decompiler
// Type: CornerSpace.NetworkPlayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public abstract class NetworkPlayer : Player
  {
    private const long cInterpolationDelay = 100;
    protected InputFrame inputFrame;
    private WorldView view;

    public NetworkPlayer()
    {
      this.inputFrame = new InputFrame();
      this.view = new WorldView();
    }

    public WorldView WorldView => this.view;

    protected override void InputStep(ref StateFrame frame)
    {
      Astronaut astronaut = this.Astronaut;
      if (frame.Use && !this.PreviousState.Use)
      {
        if (astronaut.MountedTo == null)
        {
          if (this.PickedBlock != null && this.PickedBlock.CanBeUsed)
          {
            this.PickedBlock.UseButtonClicked((Player) this);
            this.OnUse(this.PickedBlock);
          }
        }
        else if (astronaut.MountedTo != null)
          astronaut.MountedTo.UnMount();
      }
      this.UpdateMountedBlock(astronaut, frame.ControlButtonsStates);
      base.InputStep(ref frame);
    }

    protected void UpdateActiveItem(int itemId, ref StateFrame state)
    {
      if (Engine.LoadedWorld == null)
        return;
      Astronaut astronaut = this.Astronaut;
      Item activeItem = astronaut.ActiveItem;
      int num = activeItem != (Item) null ? activeItem.ItemId : -1;
      if (itemId != num)
      {
        Item obj = Engine.LoadedWorld.Itemset.GetItem(itemId);
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

    protected void UpdateMountedBlock(Astronaut astronaut, short controlButtonStates)
    {
      if (astronaut == null || astronaut.MountedTo == null)
        return;
      astronaut.MountedTo.ActiveButtons = (short) 0;
      this.inputFrame.SetUp(astronaut.MountedTo as ControlBlock, controlButtonStates);
      astronaut.MountedTo.ActiveButtons = controlButtonStates;
      astronaut.MountedTo.Update(this.inputFrame, astronaut);
    }

    public enum Playing
    {
      Server,
      Client,
    }
  }
}
