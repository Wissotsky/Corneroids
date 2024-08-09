// Decompiled with JetBrains decompiler
// Type: CornerSpace.Player
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class Player : IDisposable, IRemarkable
  {
    public const int LocalPlayerId = 0;
    protected const float cBoostPowerDepleteRate = 20f;
    protected const int cDamageTakenCounter = 400;
    protected const int cDefaultMaxHealth = 100;
    protected const int cDefaultMaxPower = 100;
    protected const float cMaxBoostSpeed = 8f;
    protected const float cMaxShakeAmount = 0.02f;
    protected const float cMaxTravelSpeed = 4f;
    protected const float throttleSpeed = 5f;
    protected const float boostForce = 10f;
    protected const float rollSpeed = 2f;
    protected const float verticalThrottleSpeed = 5f;
    private EnvironmentManager environmentManager;
    private SynchronizationValue synchronizationValue;
    private Astronaut astronaut;
    private int id;
    private Inventory inventory;
    private long latestReceivedMessageTick;
    private string name;
    private Block pickedBlock;
    private Random random;
    private StateFrame previousState;
    private bool stateChanged;
    private string token;
    private ItemToolbarLayer toolbar;
    public StateFrame LatestStateFrame;

    public Player()
    {
      this.astronaut = new Astronaut(this);
      this.synchronizationValue = new SynchronizationValue();
      this.toolbar = new ItemToolbarLayer();
      this.inventory = new Inventory(6U, 5U, this);
      this.random = new Random();
      this.name = "You!";
      this.SubscribeToEvents();
    }

    public virtual bool AddItems(Item item, int count)
    {
      int num = this.toolbar.AddItems(item, count);
      if (num >= count)
        return true;
      int count1 = count - num;
      if (!this.inventory.SpaceLeftForItem(item, count1))
        return false;
      this.inventory.AddItems(item, count1);
      return true;
    }

    public virtual bool CanSeeEntity(SpaceEntity entity, float squaredDistanceLimit)
    {
      return (double) (entity.Position - this.astronaut.Position).LengthSquared() <= (double) squaredDistanceLimit;
    }

    public virtual void DamageTaken(Position3 worldPosition)
    {
    }

    public virtual void Dispose() => this.astronaut.Dispose();

    public string GetDescriptionTag() => this.name;

    public virtual bool IsSpaceForItem(Item item, uint count)
    {
      bool flag1 = this.inventory.SpaceLeftForItem(item, (int) count);
      bool flag2 = this.toolbar.SpaceLeftForItem(item, (int) count);
      return flag1 || flag2;
    }

    public void RenderFirstPerson() => this.astronaut.RenderFirstPerson();

    public void RenderFirstPersonWithLighting() => this.astronaut.RenderFirstPersonWithLighting();

    public virtual void RenderGUI()
    {
    }

    public virtual void RenderLights(ILightingInterface lightingInterface)
    {
      lightingInterface.DrawLight((LightSource) this.astronaut.Flashlight);
      if (!(this.astronaut.ActiveItem != (Item) null))
        return;
      this.astronaut.ActiveItem.RenderLights();
    }

    public void RenderThirdPerson(IRenderCamera camera, ILightingInterface lightingInterface)
    {
      this.astronaut.RenderThirdPerson(camera, lightingInterface);
    }

    public virtual void Update(
      float deltaTime,
      CharacterManager characterManager,
      SpaceEntityManager entityManager)
    {
      this.astronaut.Update(deltaTime);
    }

    public virtual void UpdateInput(InputFrame input)
    {
    }

    public event Action<Player, Block> BlockEdited;

    public event Action<Player, Block> BlockUsed;

    public event Action<Player, CharacterScreen> CharacterScreenOpened;

    public event Action<Player, Player.ItemModifiedArgs> ItemAddedToInventory;

    public event Action<Player, Player.ItemModifiedArgs> ItemRemovedFromInventory;

    public Astronaut Astronaut
    {
      get => this.astronaut;
      set => this.astronaut = value;
    }

    public EnvironmentManager EnvironmentManager
    {
      get => this.environmentManager;
      set => this.environmentManager = value;
    }

    public int Id
    {
      get => this.id;
      set => this.id = value;
    }

    public Inventory Inventory => this.inventory;

    public long LatestMessageTicks
    {
      get => this.latestReceivedMessageTick;
      set => this.latestReceivedMessageTick = value;
    }

    public string Name
    {
      get => this.name;
      set => this.name = value ?? "Noname";
    }

    public Block PickedBlock
    {
      get => this.pickedBlock;
      set => this.pickedBlock = value;
    }

    public Position3 Position => this.astronaut.Position;

    protected StateFrame PreviousState
    {
      get => this.previousState;
      set => this.previousState = value;
    }

    protected Random Random => this.random;

    public SynchronizationValue SynchronizationValue => this.synchronizationValue;

    public string Token
    {
      get => this.token;
      set => this.token = value;
    }

    public ItemToolbarLayer Toolbar => this.toolbar;

    protected void ActivateBoost(float deltaTime)
    {
      float num = 20f * deltaTime;
      if ((double) this.astronaut.Power < (double) num)
        return;
      this.astronaut.ApplyForce(Vector3.Zero, this.astronaut.GetLookatVector() * 10f, deltaTime);
      this.Astronaut.MaximumSpeed = 8f;
      this.astronaut.Power -= num;
    }

    protected virtual void InputStep(ref StateFrame frame)
    {
      float simulationTime = frame.SimulationTime;
      this.astronaut.GetLookatVector();
      if (this.astronaut.Alive)
      {
        if (this.astronaut.MountedTo == null)
        {
          if (frame.Boost)
            this.ActivateBoost(simulationTime);
          else if (frame.Forward)
            this.astronaut.ApplyForce(Vector3.Zero, this.astronaut.GetLookatVector() * 5f, simulationTime);
          if (frame.Backward)
            this.astronaut.ApplyForce(Vector3.Zero, -this.astronaut.GetLookatVector() * 5f, simulationTime);
          if (frame.StrafeLeft)
            this.astronaut.ApplyForce(Vector3.Zero, this.astronaut.GetStrafeVector() * -5f, simulationTime);
          if (frame.StrafeRight)
            this.astronaut.ApplyForce(Vector3.Zero, this.astronaut.GetStrafeVector() * 5f, simulationTime);
          Vector3 vector3 = this.Astronaut.CameraMode == Camera.Mode.Space ? this.Astronaut.GetUpVector() : Vector3.Transform(Vector3.Up, this.Astronaut.CoordinateSpace);
          if (frame.Up)
            this.astronaut.ApplyForce(Vector3.Zero, vector3 * 5f, simulationTime);
          else if (frame.Down)
            this.astronaut.ApplyForce(Vector3.Zero, -vector3 * 5f, simulationTime);
        }
        if (frame.RollLeft)
          this.astronaut.Roll(2f * simulationTime);
        else if (frame.RollRight)
          this.astronaut.Roll(-2f * simulationTime);
      }
      this.previousState = frame;
    }

    protected virtual ItemSlot PopItemFromToolbar()
    {
      return this.toolbar.PopItems((int) this.toolbar.SelectedIndex, 1);
    }

    protected void OnCharacterScreenOpened(CharacterScreen screen)
    {
      if (this.CharacterScreenOpened == null)
        return;
      this.CharacterScreenOpened(this, screen);
    }

    protected void OnEdit(Block block)
    {
      if (this.BlockEdited == null || block == null)
        return;
      this.BlockEdited(this, block);
    }

    protected void OnUse(Block block)
    {
      if (this.BlockUsed == null || block == null)
        return;
      this.BlockUsed(this, block);
    }

    protected void SetAstronautOrientation(ref StateFrame frame, SpaceEntityManager entityManager)
    {
      Astronaut astronaut = this.Astronaut;
      if (astronaut.BoardedEntity != null && frame.BoardedEntityId == -1)
        astronaut.UsedCamera.Orientation = Quaternion.Concatenate(frame.Orientation, Quaternion.Conjugate(astronaut.BoardedEntity.Orientation));
      else if (astronaut.BoardedEntity == null && frame.BoardedEntityId != -1)
      {
        SpaceEntity entity = entityManager.GetEntity(frame.BoardedEntityId);
        if (entity == null)
          return;
        astronaut.UsedCamera.Orientation = Quaternion.Concatenate(frame.Orientation, entity.Orientation);
      }
      else
        astronaut.UsedCamera.Orientation = frame.Orientation;
    }

    protected virtual void SetAstronautPosition(
      ref StateFrame frame,
      SpaceEntityManager entityManager)
    {
      Astronaut astronaut = this.Astronaut;
      if (frame.BoardedEntityId != -1 && astronaut.BoardedEntity == null)
      {
        SpaceEntity entity = entityManager.GetEntity(frame.BoardedEntityId);
        if (entity == null)
          return;
        astronaut.Position = entity.EntityCoordsToWorldCoords(frame.PositionInEntitySpace);
        astronaut.Speed = entity.EntityNormalToWorldNormal(frame.SpeedInEntitySpace);
      }
      else if (frame.BoardedEntityId == -1)
      {
        astronaut.Position = frame.Position;
        astronaut.Speed = frame.Speed;
      }
      else
      {
        astronaut.PositionOnBoardedEntity = frame.PositionInEntitySpace;
        astronaut.SpeedOnBoardedEntity = frame.SpeedInEntitySpace;
      }
    }

    protected virtual void SetPlayerBoardedState(
      ref StateFrame frame,
      SpaceEntityManager entityManager)
    {
      Astronaut astronaut = this.Astronaut;
      if (frame.BoardedEntityId == -1)
      {
        astronaut.BoardedEntity = (SpaceEntity) null;
      }
      else
      {
        SpaceEntity entity = entityManager.GetEntity(frame.BoardedEntityId);
        if (entity == null || astronaut.BoardedEntity == entity)
          return;
        astronaut.BoardedEntity = entity;
      }
    }

    protected void SetItemSynchronizationValue(long ticks)
    {
      if (!(this.astronaut.ActiveItem != (Item) null))
        return;
      this.astronaut.ActiveItem.SynchronizationValue = (byte) ((ulong) ticks % 256UL);
    }

    private void SubscribeToEvents()
    {
      this.inventory.ItemsAdded += (Action<ItemSlot, int, int>) ((itemSlot, x, y) =>
      {
        if (this.ItemAddedToInventory == null)
          return;
        this.ItemAddedToInventory(this, new Player.ItemModifiedArgs()
        {
          ItemSlot = itemSlot,
          InventoryType = Inventory.Type.Inventory,
          X = x,
          Y = y
        });
      });
      this.inventory.ItemsRemoved += (Action<ItemSlot, int, int>) ((itemSlot, x, y) =>
      {
        if (this.ItemRemovedFromInventory == null)
          return;
        this.ItemRemovedFromInventory(this, new Player.ItemModifiedArgs()
        {
          ItemSlot = itemSlot,
          InventoryType = Inventory.Type.Inventory,
          X = x,
          Y = y
        });
      });
      this.toolbar.Items.ItemsAdded += (Action<ItemSlot, int, int>) ((itemSlot, x, y) =>
      {
        if (this.ItemAddedToInventory == null)
          return;
        this.ItemAddedToInventory(this, new Player.ItemModifiedArgs()
        {
          ItemSlot = itemSlot,
          InventoryType = Inventory.Type.Toolbar,
          X = x,
          Y = y
        });
      });
      this.toolbar.Items.ItemsRemoved += (Action<ItemSlot, int, int>) ((itemSlot, x, y) =>
      {
        if (this.ItemRemovedFromInventory == null)
          return;
        this.ItemRemovedFromInventory(this, new Player.ItemModifiedArgs()
        {
          ItemSlot = itemSlot,
          InventoryType = Inventory.Type.Toolbar,
          X = x,
          Y = y
        });
      });
    }

    protected void UpdateActiveItem(InputFrame input)
    {
      Item activeItem = this.astronaut.ActiveItem;
      if (!(activeItem != (Item) null))
        return;
      Item.UsageResult usageResult = activeItem.UpdateInput(input, this, this.astronaut.Power);
      if ((usageResult & Item.UsageResult.Consumed) > Item.UsageResult.None)
      {
        this.PopItemFromToolbar();
        if (this.toolbar.PeekItem((int) this.toolbar.SelectedIndex) == null)
          this.astronaut.ActiveItem = (Item) null;
      }
      if ((usageResult & Item.UsageResult.Power_used) <= Item.UsageResult.None)
        return;
      this.astronaut.Power -= (float) activeItem.PowerUsage;
    }

    protected void UpdateInteractionInput(ref StateFrame frame)
    {
      if (!frame.Edit || this.astronaut.MountedTo != null || this.pickedBlock == null)
        return;
      this.pickedBlock.EditButtonClicked(this);
    }

    protected void UpdateItemBasedOnToolbarIndex()
    {
      Item activeItem = this.astronaut.ActiveItem;
      Item obj = (Item) null;
      sbyte selectedIndex = this.toolbar.SelectedIndex;
      if (selectedIndex < (sbyte) 0 || selectedIndex > (sbyte) 7)
        return;
      ItemSlot itemSlot = this.toolbar.PeekItem((int) selectedIndex);
      if (itemSlot != null)
        obj = itemSlot.Item;
      if (!(activeItem != obj))
        return;
      if (activeItem == (Item) null && obj != (Item) null)
        this.astronaut.ActiveItem = obj;
      else if (activeItem != (Item) null && obj == (Item) null)
      {
        this.astronaut.ActiveItem = (Item) null;
      }
      else
      {
        if (!(activeItem != (Item) null) || !(obj != (Item) null) || activeItem.ItemId == obj.ItemId)
          return;
        this.astronaut.ActiveItem = obj;
      }
    }

    protected void UpdateToolbarItem(InputFrame input)
    {
      sbyte clickedIndex = this.toolbar.GetClickedIndex(input);
      if (clickedIndex >= (sbyte) 0 || input.Scroll != 0)
      {
        if (clickedIndex >= (sbyte) 0)
        {
          this.toolbar.SelectedIndex = clickedIndex;
        }
        else
        {
          this.toolbar.SelectedIndex += input.Scroll > 0 ? (sbyte) -1 : (sbyte) 1;
          this.toolbar.SelectedIndex = Math.Min(Math.Max((sbyte) 0, this.toolbar.SelectedIndex), (sbyte) 7);
        }
      }
      this.UpdateItemBasedOnToolbarIndex();
    }

    public class ItemModifiedArgs : EventArgs
    {
      public ItemSlot ItemSlot;
      public int X;
      public int Y;
      public Inventory.Type InventoryType;
    }
  }
}
