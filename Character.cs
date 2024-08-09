// Decompiled with JetBrains decompiler
// Type: CornerSpace.Character
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public abstract class Character : Camera
  {
    private const int cHealthRegenIntervalMS = 2000;
    private const float cSpeedReduction = 6f;
    private const float cSpeedRedcutionOverSpeed = 8f;
    private const float width = 0.5f;
    private const float height = 1f;
    private const float depth = 0.5f;
    private bool alive;
    private ArmorRating initialArmorRating;
    private Color bloodColor;
    private SpaceEntity boardedEntity;
    private List<Buff> buffs;
    private float collisionDamageSpeedTreshold;
    private float collisionDamagePerSpeedUnit;
    private bool forceAppliedThisFrame;
    private short health;
    private int healthRegenTimer;
    private short initialMaximumHealth;
    private float initialMaximumPower;
    private float maxSpeed;
    private Model model;
    private Matrix modificationMatrix;
    private Vector3 positionInBoardedEntitySpace;
    private float power;
    private float powerPreUpdate;
    private int initialPowerRechargeRate;
    private Vector3 speedInBoardedEntitySpace;
    private Suit suit;
    private Effect effect;
    private Matrix initializationMatrix;
    private float offset;
    private float radius;
    private Texture2D texture;

    public Character(string modelPath, string texturePath, float radius)
    {
      this.alive = true;
      this.bloodColor = Color.Red;
      this.buffs = new List<Buff>();
      this.maxSpeed = 4f;
      this.forceAppliedThisFrame = false;
      this.Health = this.initialMaximumHealth = (short) 100;
      this.initialMaximumPower = this.Power = 100f;
      this.initialArmorRating = new ArmorRating(0U);
      this.initialPowerRechargeRate = 5;
      this.modificationMatrix = Matrix.Identity;
      this.initializationMatrix = Matrix.Identity;
      this.collisionDamageSpeedTreshold = 100f;
      this.suit = new Suit();
      this.radius = MathHelper.Clamp(radius, 0.1f, 10f);
      this.BoundingRadius = radius;
      this.BoundingSphere = new BoundingSphereI(Position3.Zero, radius);
      if (string.IsNullOrEmpty(modelPath))
        return;
      try
      {
        this.effect = Engine.ContentManager.Load<Effect>("Shaders/CharacterShader");
        this.model = Engine.ContentManager.Load<Model>(modelPath);
        if (this.effect != null)
        {
          foreach (ModelMesh mesh in this.model.Meshes)
          {
            foreach (ModelMeshPart meshPart in mesh.MeshParts)
              meshPart.Effect = this.effect;
          }
        }
        if (string.IsNullOrEmpty(texturePath))
          return;
        this.texture = Engine.ContentManager.Load<Texture2D>(texturePath);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load a model: " + ex.Message);
        this.model = (Model) null;
      }
    }

    public void ApplyBuff(Buff buff)
    {
      if (buff == null)
        return;
      Buff buff1 = this.buffs.Find((Predicate<Buff>) (b => b.Id == buff.Id));
      if (buff1 != null)
        this.RemoveBuff(buff1);
      this.buffs.Add(buff);
      try
      {
        if (buff.ApplyDelegate == null)
          return;
        buff.ApplyDelegate(this);
      }
      catch
      {
      }
    }

    public override void ApplyForce(Vector3 point, Vector3 force, float deltaTime)
    {
      base.ApplyForce(point, force, deltaTime);
      if ((double) this.Mass <= 0.0)
        return;
      if (this.boardedEntity == null)
      {
        Character character = this;
        character.Speed = character.Speed + force / this.Mass * deltaTime;
      }
      else
        this.SpeedOnBoardedEntity += this.boardedEntity.WorldNormalToEntityNormal(force) / this.Mass * deltaTime;
      Character character1 = this;
      int num = character1.forceAppliedThisFrame ? 1 : 0;
      character1.forceAppliedThisFrame = true;
    }

    public virtual void DamageTaken(Position3 position)
    {
    }

    public override void Dispose()
    {
      base.Dispose();
      this.DiedEvent = (Action<Character>) null;
    }

    public void Kill() => this.Died();

    public virtual void ReduceHealth(int value)
    {
      this.Health -= (short) (this.initialArmorRating + this.suit.ArmorRating).ReducedValue(value);
    }

    public void Regen()
    {
      if (!this.alive)
        return;
      this.healthRegenTimer += (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.healthRegenTimer >= 2000)
      {
        if ((int) this.Health < (int) this.MaximumHealth)
          ++this.Health;
        this.healthRegenTimer = 0;
      }
      this.power += this.PowerRechargeRate * Engine.FrameCounter.DeltaTime;
      this.power = Math.Min(this.power, this.MaximumPower);
    }

    public virtual void RenderThirdPerson(
      IRenderCamera camera,
      ILightingInterface lightingInterface)
    {
      if (this.model == null || camera == null || camera == this)
        return;
      Engine.GraphicsDevice.RenderState.DepthBufferEnable = true;
      Engine.GraphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      Engine.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
      Engine.GraphicsDevice.RenderState.AlphaBlendEnable = false;
      this.effect.Parameters["Texture"].SetValue((Texture) this.texture);
      Matrix modelMatrix = this.CreateModelMatrix(camera);
      foreach (ModelMesh mesh in this.model.Meshes)
      {
        Microsoft.Xna.Framework.BoundingSphere sphere = new Microsoft.Xna.Framework.BoundingSphere(camera.GetPositionRelativeToCamera(this.Position), this.radius * 2f);
        if (camera.ViewFrustum.Contains(sphere) != ContainmentType.Disjoint)
        {
          foreach (Effect effect in mesh.Effects)
          {
            effect.Parameters["World"].SetValue(modelMatrix);
            effect.Parameters["View"].SetValue(camera.ViewMatrix);
            effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
          }
          mesh.Draw();
        }
      }
    }

    public void SetHealthButDontKill(short newHealth)
    {
      this.health = Math.Min(Math.Max((short) 0, newHealth), this.MaximumHealth);
    }

    public virtual void Spawn(Position3 position)
    {
      this.Health = this.InitialMaximumHealth;
      this.Power = this.InitialMaximumPower;
      this.BoardedEntity = (SpaceEntity) null;
      base.Position = position;
      this.alive = true;
      if (this.Spawned == null)
        return;
      this.Spawned(this);
    }

    public override void Update(float deltaTime)
    {
      if (this.boardedEntity != null)
        this.CoordinateSpace = this.boardedEntity.Orientation;
      this.UpdatePhysics(deltaTime);
      base.Update(deltaTime);
      this.UpdateBuffs();
      this.offset = MathHelper.WrapAngle(this.offset + 0.7853982f * deltaTime);
      this.Regen();
    }

    public override void UpdatePhysics(float deltaTime)
    {
      base.UpdatePhysics(deltaTime);
      if (this.boardedEntity == null)
      {
        Character character = this;
        character.Position = character.Position + this.Speed * deltaTime;
      }
      else
      {
        this.positionInBoardedEntitySpace += this.speedInBoardedEntitySpace * deltaTime;
        base.Position = this.boardedEntity.EntityCoordsToWorldCoords(this.positionInBoardedEntitySpace);
        base.Speed = this.boardedEntity.EntityNormalToWorldNormal(this.speedInBoardedEntitySpace);
      }
      float num1 = this.Speed.Length();
      if ((double) num1 > (double) this.maxSpeed)
      {
        float num2 = 8f * deltaTime;
        Character character = this;
        character.Speed = character.Speed - Vector3.Normalize(this.Speed) * num2;
      }
      if (!this.forceAppliedThisFrame)
      {
        if ((double) num1 > 6.0 * (double) deltaTime)
        {
          float num3 = 6f * deltaTime;
          Character character = this;
          character.Speed = character.Speed - Vector3.Normalize(this.Speed) * num3;
        }
        else
          this.Speed = Vector3.Zero;
      }
      this.forceAppliedThisFrame = false;
    }

    public void UpdatePositionAndOrientationToSpace()
    {
      if (this.boardedEntity == null)
        return;
      this.position = this.boardedEntity.EntityCoordsToWorldCoords(this.positionInBoardedEntitySpace);
      this.CoordinateSpace = this.boardedEntity.Orientation;
    }

    public event Action<Character> DiedEvent;

    public event Action<Character> Spawned;

    public bool Alive => this.alive;

    public ArmorRating ArmorRating => this.initialArmorRating + this.suit.ArmorRating;

    public Color BloodColor
    {
      get => this.bloodColor;
      protected set => this.bloodColor = value;
    }

    public virtual SpaceEntity BoardedEntity
    {
      get => this.boardedEntity;
      set
      {
        if (value != null && this.boardedEntity == value)
          return;
        if (this.boardedEntity != null)
          this.boardedEntity.DisposedEvent -= new System.Action(this.BoardedEntityDisposed);
        if (value != null && value != this.boardedEntity)
        {
          this.PositionOnBoardedEntity = value.WorldCoordsToEntityCoords(this.Position);
          this.Orientation = Quaternion.Concatenate(this.Orientation, Quaternion.Conjugate(value.Orientation));
          this.CoordinateSpace = value.Orientation;
          value.DisposedEvent += new System.Action(this.BoardedEntityDisposed);
          this.SpeedOnBoardedEntity = value.WorldNormalToEntityNormal(this.Speed);
        }
        else if (this.boardedEntity != null)
        {
          this.CoordinateSpace = Quaternion.Identity;
          this.Orientation = Quaternion.Concatenate(this.Orientation, this.boardedEntity.Orientation);
          Character character = this;
          character.Speed = character.Speed + this.boardedEntity.GetSpeedAt(this.Position);
        }
        this.boardedEntity = value;
      }
    }

    public List<Buff> Buffs => this.buffs;

    public float CollisionDamagePerSpeedUnit
    {
      get => this.collisionDamagePerSpeedUnit;
      protected set => this.collisionDamagePerSpeedUnit = value;
    }

    public float CollisionDamageSpeedTreshold
    {
      get => this.collisionDamageSpeedTreshold;
      protected set => this.collisionDamageSpeedTreshold = value;
    }

    public virtual short Health
    {
      get => this.health;
      set
      {
        short health = this.health;
        this.SetHealthButDontKill(value);
        if (this.health != (short) 0 || health <= (short) 0)
          return;
        this.Kill();
      }
    }

    protected Matrix InitializationMatrix
    {
      get => this.initializationMatrix;
      set => this.initializationMatrix = value;
    }

    public short InitialMaximumHealth
    {
      get => this.initialMaximumHealth;
      set => this.initialMaximumHealth = value;
    }

    public float InitialMaximumPower
    {
      get => this.initialMaximumPower;
      set => this.initialMaximumPower = value;
    }

    public short MaximumHealth
    {
      get
      {
        return (short) ((int) this.initialMaximumHealth + (this.suit != null ? this.suit.HealthRating : 0));
      }
    }

    public float MaximumPower => this.initialMaximumPower;

    public float MaximumSpeed
    {
      get => this.maxSpeed;
      set => this.maxSpeed = value;
    }

    public Matrix ModificationMatrix
    {
      get => this.modificationMatrix;
      set => this.modificationMatrix = value;
    }

    public override Position3 Position
    {
      set
      {
        base.Position = value;
        if (this.boardedEntity == null)
          return;
        this.positionInBoardedEntitySpace = this.boardedEntity.WorldCoordsToEntityCoords(value);
      }
    }

    public Vector3 PositionOnBoardedEntity
    {
      get => this.positionInBoardedEntitySpace;
      set
      {
        this.positionInBoardedEntitySpace = value;
        if (this.boardedEntity == null)
          return;
        base.Position = this.boardedEntity.EntityCoordsToWorldCoords(value);
      }
    }

    public float PowerPreUpdate
    {
      get => this.powerPreUpdate;
      set => this.powerPreUpdate = value;
    }

    public float PowerRechargeRate
    {
      get => (float) (this.initialPowerRechargeRate + this.suit.PowerRechargeRate);
    }

    public float Power
    {
      get => this.power;
      set => this.power = Math.Max(0.0f, value);
    }

    public override Vector3 Speed
    {
      get => base.Speed;
      set
      {
        base.Speed = value;
        if (this.boardedEntity == null)
          return;
        this.speedInBoardedEntitySpace = this.boardedEntity.WorldNormalToEntityNormal(value);
      }
    }

    public Vector3 SpeedOnBoardedEntity
    {
      get => this.speedInBoardedEntitySpace;
      set
      {
        this.speedInBoardedEntitySpace = value;
        if (this.boardedEntity == null)
          return;
        this.speed = this.boardedEntity.EntityNormalToWorldNormal(value);
      }
    }

    public Suit Suit => this.suit;

    private void BoardedEntityDisposed() => this.BoardedEntity = (SpaceEntity) null;

    protected Matrix CreateModelMatrix(IRenderCamera camera)
    {
      return camera != null ? this.initializationMatrix * this.modificationMatrix * Matrix.CreateFromQuaternion(this.Orientation) * Matrix.CreateFromQuaternion(this.CoordinateSpace) * Matrix.CreateTranslation(camera.GetPositionRelativeToCamera(this.Position)) : Matrix.Identity;
    }

    protected virtual void Died()
    {
      if (!this.alive)
        return;
      this.alive = false;
      this.health = (short) 0;
      this.buffs.Clear();
      if (this.DiedEvent == null)
        return;
      this.DiedEvent(this);
    }

    private void RemoveBuff(Buff buff)
    {
      if (buff == null)
        return;
      try
      {
        if (buff.RemoveDelegate != null)
          buff.RemoveDelegate(this);
      }
      catch
      {
      }
      this.buffs.Remove(buff);
    }

    private void UpdateBuffs()
    {
      int deltaTimeMs = (int) Engine.FrameCounter.DeltaTimeMS;
      for (int index = this.buffs.Count - 1; index >= 0; --index)
      {
        Buff buff = this.buffs[index];
        if (buff.DurationMS > 0)
          buff.DurationMS -= deltaTimeMs;
        else
          this.RemoveBuff(buff);
      }
    }

    private enum State
    {
      Flying,
      Boarded,
      Boarding,
    }
  }
}
