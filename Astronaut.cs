// Decompiled with JetBrains decompiler
// Type: CornerSpace.Astronaut
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace
{
  public class Astronaut : Character, IController<Astronaut>
  {
    private const int cHealthRegenIntervalMS = 2000;
    private const int cHearthBeatSequenceMS = 1000;
    private const float cPushbackSpeed = 5f;
    private const int cPushbackTimerMS = 700;
    private Item activeItem;
    private Flashlight flashlight;
    private int hearthBeatCounterMS;
    private IMountable<Astronaut> mountedTo;
    private Player owner;
    private int pushbackCounter;
    private bool reorientating;

    public Astronaut(Player owner)
      : base("Models/astronaut_final", "Textures/astronaut", 0.49f)
    {
      if (owner == null)
        throw new ArgumentNullException();
      this.flashlight = new Flashlight(Color.White, 15f, 15f, Vector3.Up);
      this.pushbackCounter = 0;
      this.hearthBeatCounterMS = 0;
      this.CollisionDamageSpeedTreshold = 8f;
      this.CollisionDamagePerSpeedUnit = 7f;
      this.owner = owner;
      Engine.SettingsManager.CameraStyleChanged += new Action<bool>(this.CameraStyleChanged);
    }

    public virtual void CollisionWithCharacter(AICharacter character)
    {
      if (!character.InflictsDamageOnTouch || this.pushbackCounter != 0)
        return;
      this.Health -= (short) (int) (short) this.ArmorRating.ReducedValue(character.InflictedDamage);
      this.DamageTaken(character.Position);
      Vector3 vector3 = this.Position - character.Position;
      if (vector3 != Vector3.Zero)
      {
        vector3.Normalize();
        Astronaut astronaut = this;
        astronaut.Speed = astronaut.Speed + vector3 * 5f;
      }
      Astronaut astronaut1 = this;
      // ISSUE: explicit non-virtual call
      astronaut1.Orientation = __nonvirtual (astronaut1.Orientation) * Quaternion.CreateFromYawPitchRoll(0.2f, 0.2f, 0.0f);
      this.pushbackCounter = 700;
    }

    public override void DamageTaken(Position3 position)
    {
      if (this.owner == null)
        return;
      this.owner.DamageTaken(position);
    }

    public override void Dispose()
    {
      base.Dispose();
      this.UnMount();
      Engine.SettingsManager.CameraStyleChanged -= new Action<bool>(this.CameraStyleChanged);
    }

    public virtual void RenderFirstPerson()
    {
      if (!(this.activeItem != (Item) null))
        return;
      this.activeItem.RenderFirstPerson((IRenderCamera) this);
    }

    public virtual void RenderFirstPersonWithLighting()
    {
      if (!(this.activeItem != (Item) null))
        return;
      this.activeItem.RenderFirstPersonWithLighting((IRenderCamera) this);
    }

    public override void RenderThirdPerson(
      IRenderCamera camera,
      ILightingInterface lightingInterface)
    {
      base.RenderThirdPerson(camera, lightingInterface);
      if (!(this.activeItem != (Item) null))
        return;
      this.activeItem.RenderThirdPersonWithLighting(camera, this.CreateModelMatrix(camera));
    }

    public void UnMount()
    {
      if (this.MountedTo == null)
        return;
      this.MountedTo.UnMount();
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (this.Alive)
      {
        this.pushbackCounter = (int) Math.Max((long) this.pushbackCounter - Engine.FrameCounter.DeltaTimeMS, 0L);
        this.UpdateItems();
        if (!this.reorientating || this.BoardedEntity == null)
          return;
        this.reorientating = false;
      }
      else
        this.ApplyHearthBeatEffect();
    }

    public virtual void UpdateItems()
    {
      if (this.flashlight != null)
      {
        this.flashlight.Position = this.Position + this.GetStrafeVector() * 0.25f;
        this.flashlight.Direction = this.GetLookatVector();
      }
      if (!(this.activeItem != (Item) null))
        return;
      this.activeItem.Update();
      this.activeItem.Update(this);
    }

    public event Action<ControlBlock, Astronaut> MountedToControlBlock;

    public event Action<ControlBlock, Astronaut> UnMountedFromControlBlock;

    public Item ActiveItem
    {
      get => this.activeItem;
      set
      {
        ToolItem activeItem = this.activeItem as ToolItem;
        if ((Item) activeItem != (Item) null && (Item) activeItem != value)
          activeItem.Close();
        if (value is ToolItem)
          this.activeItem = value;
        else if (value == (Item) null)
          this.activeItem = (Item) null;
        else
          this.activeItem = value.Copy();
      }
    }

    public override SpaceEntity BoardedEntity
    {
      set
      {
        base.BoardedEntity = value;
        this.CameraMode = Camera.Mode.Space;
        if (value == null || !Engine.SettingsManager.FixedCameraWhenAboard)
          return;
        this.ChangeCameraModeToFixed();
      }
    }

    public Astronaut Controller => this;

    public Flashlight Flashlight
    {
      get => this.flashlight;
      set => this.flashlight = value;
    }

    public override short Health
    {
      set
      {
        base.Health = value;
        if (this.Alive)
          return;
        this.ActiveItem = (Item) null;
      }
    }

    public IMountable<Astronaut> MountedTo
    {
      get => this.mountedTo;
      set
      {
        IMountable<Astronaut> mountedTo = this.mountedTo;
        this.mountedTo = value;
        if (mountedTo == value)
          return;
        if (mountedTo != null && this.UnMountedFromControlBlock != null && mountedTo is ControlBlock controlBlock1)
          this.UnMountedFromControlBlock(controlBlock1, this);
        if (value == null)
          return;
        if (this.MountedToControlBlock != null && value is ControlBlock controlBlock2)
          this.MountedToControlBlock(controlBlock2, this);
        this.ActiveItem = (Item) null;
      }
    }

    public Camera UsedCamera
    {
      get
      {
        return this.mountedTo != null && this.mountedTo.CameraBlock != null && this.mountedTo.CameraBlock.Activated ? this.mountedTo.CameraBlock.Camera : (Camera) this;
      }
    }

    private void ApplyHearthBeatEffect()
    {
      this.hearthBeatCounterMS += (int) Engine.FrameCounter.DeltaTimeMS;
      if ((160 & (int) (byte) (128U >> (int) ((double) this.hearthBeatCounterMS / 1000.0 * 8.0))) != 0)
        this.SetAttributes(Engine.SettingsManager.AspectRatio * 1.01f, Engine.SettingsManager.CameraFoV * 1.01f, 0.1f, 800f);
      else
        this.SetAttributes(Engine.SettingsManager.AspectRatio, Engine.SettingsManager.CameraFoV, 0.1f, 800f);
      this.hearthBeatCounterMS %= 1000;
    }

    private void CameraStyleChanged(bool style)
    {
      if (style)
      {
        if (this.BoardedEntity == null)
          return;
        this.ChangeCameraModeToFixed();
      }
      else
        this.CameraMode = Camera.Mode.Space;
    }

    private void ChangeCameraModeToFixed()
    {
      this.CameraMode = Camera.Mode.Fixed_plane;
      this.CoordinateSpace = Quaternion.Concatenate(Quaternion.Conjugate(this.BoardedEntity.Orientation), this.Orientation);
      this.ResetDirection();
      this.reorientating = true;
    }

    protected override void Died()
    {
      base.Died();
      if (this.MountedTo != null)
        this.MountedTo.UnMount();
      this.MountedTo = (IMountable<Astronaut>) null;
    }
  }
}
