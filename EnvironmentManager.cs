// Decompiled with JetBrains decompiler
// Type: CornerSpace.EnvironmentManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace
{
  public class EnvironmentManager : IDisposable
  {
    protected const float cBloodDefaultSize = 0.12f;
    protected const int cBillboardsPerBrokenBlockSide = 4;
    protected const float cGeneralRenderDistance = 96f;
    protected const int cItemLifetime = 30000;
    protected const float cItemDefaultSize = 0.2f;
    protected const float cItemPickDistance = 1.5f;
    protected const float cItemPickSpeed = 5f;
    protected const ushort cItemPickUpDelay = 2000;
    protected const float cItemPulseSize = 0.025f;
    protected const float cItemRenderDistance = 32f;
    private const float cSparkDefaultSize = 0.05f;
    protected const float cTorchSize = 0.2f;
    protected const float cTossSpeed = 3f;
    private readonly Color cTargetTextureColor = new Color((byte) 20, (byte) 110, (byte) 10, byte.MaxValue);
    private BillboardBatch billboardBatch;
    protected List<EnvironmentManager.FloatingItem<ItemSlot>> floatingItems;
    private List<EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard>> generalItems;
    private IdGen idGenerator;
    private List<EnvironmentManager.FloatingItem<TorchItem>> torchItems;
    private Random random;
    private Texture2D spriteAtlas;
    private Texture2D blankTexture;
    private Texture2D targetTexture;
    private Vector3[] brokenBlockVertices;
    private LightingManager lightingManager;
    private SpaceEntityManager entityManager;

    public EnvironmentManager(SpaceEntityManager entityManager, LightingManager lightingManager)
    {
      this.billboardBatch = new BillboardBatch(1000);
      this.brokenBlockVertices = new Vector3[96];
      this.floatingItems = new List<EnvironmentManager.FloatingItem<ItemSlot>>();
      this.generalItems = new List<EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard>>();
      this.torchItems = new List<EnvironmentManager.FloatingItem<TorchItem>>();
      this.entityManager = entityManager;
      this.idGenerator = new IdGen(0);
      this.lightingManager = lightingManager;
      this.random = new Random();
      entityManager.EnvironmentManager = this;
      this.spriteAtlas = Engine.LoadedWorld.SpriteTextureAtlas.Texture;
      this.blankTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
      this.targetTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/target");
    }

    public void AddBlood(Position3 position)
    {
      for (int index = 0; index < 16; ++index)
      {
        float num1 = (float) (1.0 * (this.random.NextDouble() * 0.25 + 1.0));
        int num2 = 400 + this.random.Next(0, 300);
        Color red = Color.Red;
        this.generalItems.Add(new EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard>()
        {
          Color = red,
          Sprite = new EnvironmentManager.GenericBillboard(new Vector4(0.0f, 0.0f, 1f, 1f)),
          Position = position,
          LifetimeLeft = num2,
          Speed = num1 * (float) this.random.NextDouble() * new Vector3((float) ((this.random.NextDouble() - 0.5) * 2.0), (float) ((this.random.NextDouble() - 0.5) * 2.0), (float) ((this.random.NextDouble() - 0.5) * 2.0)),
          Size = 0.12f * Vector2.One,
          Texture = this.blankTexture
        });
      }
    }

    public virtual EnvironmentManager.FloatingItem<ItemSlot> AddFloatingItem(
      ItemSlot item,
      Position3 position,
      Vector3 speed,
      ushort pickupDelay)
    {
      return item == null || this.spriteAtlas == null ? (EnvironmentManager.FloatingItem<ItemSlot>) null : this.AddFloatingItem(item, position, speed, this.idGenerator.GetNewId(), pickupDelay);
    }

    public void AddSpark(
      Position3 position,
      Vector3 normal,
      Vector3 up,
      float maxAngle,
      Color color)
    {
      Vector3 vector3 = Vector3.Zero;
      if (normal != up && normal != Vector3.Zero && up != Vector3.Zero)
      {
        normal.Normalize();
        up.Normalize();
        this.random.NextDouble();
        float angle = (float) this.random.NextDouble() * 6.28318548f;
        Vector3.Normalize(Vector3.Cross(normal, up));
        Matrix fromAxisAngle = Matrix.CreateFromAxisAngle(normal, angle);
        vector3 = Vector3.Transform(up, fromAxisAngle) * 2.5f;
      }
      this.generalItems.Add(new EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard>()
      {
        Color = color,
        Sprite = new EnvironmentManager.GenericBillboard(new Vector4(0.0f, 0.0f, 1f, 1f)),
        Position = position,
        LifetimeLeft = 200,
        Speed = vector3,
        Size = 0.05f * Vector2.One,
        Texture = this.blankTexture
      });
    }

    public void AddExplosion(Position3 worldPosition, ProjectileType projectileType)
    {
      if (projectileType == null)
        return;
      this.AddExplosion(worldPosition, projectileType.Color);
    }

    public void AddExplosion(Position3 worldPosition, Color color)
    {
      for (int index = 0; index < 24; ++index)
      {
        float num1 = (float) (5.0 * (this.random.NextDouble() * 0.5 + 1.0));
        int num2 = 400 + this.random.Next(0, 200);
        this.generalItems.Add(new EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard>()
        {
          Color = color,
          Sprite = new EnvironmentManager.GenericBillboard(new Vector4(0.0f, 0.0f, 1f, 1f)),
          Position = worldPosition,
          LifetimeLeft = num2,
          Speed = num1 * (float) this.random.NextDouble() * new Vector3((float) ((this.random.NextDouble() - 0.5) * 2.0), (float) ((this.random.NextDouble() - 0.5) * 2.0), (float) ((this.random.NextDouble() - 0.5) * 2.0)),
          Size = 0.05f * Vector2.One,
          Texture = this.blankTexture
        });
      }
    }

    public void AddSpawnParticles(Position3 worldPosition)
    {
      this.AddExplosion(worldPosition, Color.LightBlue);
      this.AddExplosion(worldPosition, Color.LightBlue);
    }

    public void AddThrusterFlame(
      Position3 worldPosition,
      Vector3 speed,
      Color color,
      int lifetime,
      float size)
    {
      this.generalItems.Add(new EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard>()
      {
        Color = color,
        Sprite = new EnvironmentManager.GenericBillboard(new Vector4(0.0f, 0.0f, 1f, 1f)),
        Position = worldPosition,
        LifetimeLeft = lifetime,
        Speed = speed,
        Size = size * Vector2.One,
        Texture = this.blankTexture
      });
    }

    public virtual void BindToClient(NetworkClientManager client)
    {
      throw new ArgumentException("Cannot bind a client to singleplayer manager!");
    }

    public virtual void BindToServer(NetworkServerManager server)
    {
      throw new ArgumentException("Cannot bind a server to singleplayer manager!");
    }

    public void BreakBlock(Position3 worldPosition, Block block, Quaternion orientation)
    {
      if (block == null)
        return;
      Byte3 modelSize = block.ModelSize;
      Vector3 modelPlacement = block.ModelPlacement;
      Texture2D texture = Engine.LoadedWorld.BlockTextureAtlas.Texture;
      Color itemColor = block.GetBlockType().ItemColor;
      Vector3 vector3_1 = new Vector3(0.25f * (float) modelSize.X, 0.25f * (float) modelSize.Y, 0.25f * (float) modelSize.Z);
      Vector3 vector3_2 = 0.5f * vector3_1;
      int num1 = 0;
      Vector3 vector3_3 = -0.5f * (Vector3) modelSize + vector3_2;
      for (int index1 = 0; index1 < 4; ++index1)
      {
        for (int index2 = 0; index2 < 4; ++index2)
        {
          for (int index3 = 0; index3 < 4; ++index3)
          {
            if (index3 == 0 || index3 == 3 || index2 == 0 || index2 == 3 || index1 == 0 || index1 == 3)
              this.brokenBlockVertices[num1++] = vector3_3 + new Vector3((float) index3 * vector3_1.X, (float) index2 * vector3_1.Y, (float) index1 * vector3_1.Z);
          }
        }
      }
      Matrix matrix = Matrix.CreateTranslation(block.ModelPlacement) * Matrix.CreateFromQuaternion(orientation);
      Vector3.Transform(this.brokenBlockVertices, ref matrix, this.brokenBlockVertices);
      for (int index = 0; index < num1; ++index)
      {
        Vector3 brokenBlockVertex = this.brokenBlockVertices[index];
        if ((double) brokenBlockVertex.LengthSquared() > 0.0)
        {
          Vector3 vector3_4 = Vector3.Normalize(brokenBlockVertex) * (float) (0.25 + this.random.NextDouble() * 0.5);
          int num2 = 200 + this.random.Next(0, 500);
          float num3 = (float) (0.800000011920929 + this.random.NextDouble() * 0.40000000596046448);
          this.generalItems.Add(new EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard>()
          {
            Color = new Color((byte) ((double) itemColor.R * (double) num3), (byte) ((double) itemColor.G * (double) num3), (byte) ((double) itemColor.B * (double) num3)),
            Sprite = new EnvironmentManager.GenericBillboard(new Vector4(0.0f, 0.0f, 1f, 1f)),
            Position = worldPosition + brokenBlockVertex,
            LifetimeLeft = num2,
            Speed = vector3_4,
            Size = 0.25f * Vector2.One,
            Texture = this.blankTexture
          });
        }
      }
    }

    public void Dispose() => this.billboardBatch.Dispose();

    public void Render(IRenderCamera camera)
    {
      this.billboardBatch.Begin();
      foreach (EnvironmentManager.FloatingItem<ItemSlot> floatingItem in this.floatingItems)
      {
        if ((double) (floatingItem.Position - camera.Position).LengthSquared() <= 1024.0)
        {
          Vector2 size = floatingItem.Size + new Vector2((float) Math.Cos((double) floatingItem.PulseAngle), (float) Math.Cos((double) floatingItem.PulseAngle)) * 0.025f;
          this.billboardBatch.DrawBillboard(floatingItem.Texture, camera.GetPositionRelativeToCamera(floatingItem.Position), floatingItem.Sprite.SpriteCoordsUV, size, floatingItem.Color);
        }
      }
      foreach (EnvironmentManager.FloatingItem<TorchItem> torchItem in this.torchItems)
      {
        if ((double) (torchItem.Position - camera.Position).LengthSquared() <= 1024.0)
          this.billboardBatch.DrawBillboard(torchItem.Texture, camera.GetPositionRelativeToCamera(torchItem.Position), torchItem.Sprite.SpriteCoordsUV, Vector2.One * 0.2f, Color.White);
      }
      foreach (EnvironmentManager.FloatingItem<EnvironmentManager.GenericBillboard> generalItem in this.generalItems)
      {
        if ((double) (generalItem.Position - camera.Position).LengthSquared() <= 9216.0)
          this.billboardBatch.DrawBillboard(generalItem.Texture, camera.GetPositionRelativeToCamera(generalItem.Position), generalItem.Sprite.SpriteCoordsUV, generalItem.Size, generalItem.Color);
      }
      this.billboardBatch.End(camera);
    }

    public void RenderMarkers<T>(IEnumerable<T> markers, IRenderCamera camera, Color color) where T : IRemarkable
    {
      if (markers == null || camera == null)
        return;
      Vector2 vector2 = 0.5f * new Vector2((float) -this.targetTexture.Width, (float) -this.targetTexture.Height);
      Engine.SpriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend);
      Engine.SetPointSamplerStateForSpritebatch();
      foreach (T marker in markers)
      {
        Vector3 vector3 = marker.Position - camera.Position;
        if (vector3 != Vector3.Zero)
          vector3.Normalize();
        Vector2? positionInScreenSpace = camera.GetPositionInScreenSpace(camera.Position + vector3);
        if (positionInScreenSpace.HasValue)
        {
          Vector2 position = positionInScreenSpace.Value + vector2;
          Engine.SpriteBatch.Draw(this.targetTexture, position, color);
          int num = (int) camera.GetPositionRelativeToCamera(marker.Position).Length() * 5;
          string descriptionTag = marker.GetDescriptionTag();
          Engine.SpriteBatch.DrawString(Engine.Font, num.ToString() + " m", position + new Vector2(70f, 8f), color);
          if (!string.IsNullOrEmpty(descriptionTag))
            Engine.SpriteBatch.DrawString(Engine.Font, descriptionTag, position + new Vector2(70f, 34f), color);
        }
      }
      Engine.SpriteBatch.End();
    }

    public virtual bool TossItems(Character tosser, ItemSlot item)
    {
      if (tosser == null || item == null)
        return false;
      Position3 position = tosser.Position;
      Vector3 speed = tosser.GetLookatVector() * 3f;
      return this.AddFloatingItem(item, position, speed, (ushort) 2000) != null;
    }

    public void TossTorch(TorchItem torch, Position3 position, Vector3 speed)
    {
      if (!((Item) torch != (Item) null))
        return;
      this.torchItems.Add(new EnvironmentManager.FloatingItem<TorchItem>()
      {
        LifetimeLeft = torch.BurningTime,
        Position = position,
        Speed = speed,
        Sprite = torch,
        Texture = this.spriteAtlas
      });
    }

    public virtual void TryPickItemsToPlayers(IEnumerable<Player> players)
    {
      for (int index = this.floatingItems.Count - 1; index >= 0; --index)
      {
        EnvironmentManager.FloatingItem<ItemSlot> floatingItem = this.floatingItems[index];
        if (floatingItem.PickupTimer <= 0)
        {
          if (floatingItem.IsBeingPickedBy == null)
          {
            Player player = players.Where<Player>((Func<Player, bool>) (p => p.IsSpaceForItem(floatingItem.Sprite.Item, (uint) floatingItem.Sprite.Count))).OrderBy<Player, float>((Func<Player, float>) (p => (p.Astronaut.Position - floatingItem.Position).LengthSquared())).FirstOrDefault<Player>();
            if (player != null && (double) (player.Astronaut.Position - floatingItem.Position).LengthSquared() <= 2.25)
              this.ItemIsBeingPickedBy(player, floatingItem);
          }
          else
          {
            Player isBeingPickedBy = floatingItem.IsBeingPickedBy;
            if (isBeingPickedBy.IsSpaceForItem(floatingItem.Sprite.Item, (uint) floatingItem.Sprite.Count))
            {
              Position3 position3 = isBeingPickedBy.Position - isBeingPickedBy.Astronaut.GetUpVector() * 0.5f;
              double num1 = (double) (floatingItem.Position - isBeingPickedBy.Position).LengthSquared();
              float num2 = (floatingItem.Position - position3).LengthSquared();
              Vector3 vector3 = position3 - floatingItem.Position;
              if ((double) num2 <= 1.0 / 16.0)
              {
                if (this.ItemsPickedByPlayer(isBeingPickedBy, floatingItem))
                  this.RemoveFloatingItem(floatingItem);
              }
              else
                floatingItem.Position += vector3 * Engine.FrameCounter.DeltaTime * 5f;
            }
            else
              floatingItem.IsBeingPickedBy = (Player) null;
          }
        }
      }
    }

    public void Update()
    {
      this.UpdateFloatingItems();
      this.UpdateTorchItems();
      this.UpdateGeneralItems();
    }

    protected EnvironmentManager.FloatingItem<ItemSlot> AddFloatingItem(
      ItemSlot item,
      Position3 position,
      Vector3 speed,
      int environmentId,
      ushort pickupDelay)
    {
      EnvironmentManager.FloatingItem<ItemSlot> floatingItem = new EnvironmentManager.FloatingItem<ItemSlot>()
      {
        Color = Color.White,
        Id = environmentId,
        Sprite = item,
        Position = position,
        LifetimeLeft = 30000,
        PickupTimer = (int) pickupDelay,
        Speed = speed,
        Size = 0.2f * Vector2.One,
        Texture = item.Item.SpriteAtlasTexture
      };
      this.floatingItems.Add(floatingItem);
      return floatingItem;
    }

    private void CheckAndHandleItemCollisions<T>(EnvironmentManager.FloatingItem<T> item) where T : IBillboard
    {
      BoundingSphereI boundingSphere = new BoundingSphereI(item.Position, 0.2f);
      CollisionData collisionData = new CollisionData();
      foreach (SpaceEntity closeEntity in this.entityManager.GetCloseEntities(item.Position, 5f))
      {
        if (CollisionManager.Instance.GetCollision(closeEntity, boundingSphere, item.Speed, ref collisionData, false))
        {
          Vector3 worldNormal = closeEntity.EntityNormalToWorldNormal(collisionData.CollisionNormal);
          Vector3 vector3 = worldNormal * collisionData.CollisionDepth;
          item.Position += vector3;
          if (item.Speed != Vector3.Zero)
            item.Speed -= SimpleMath.FastMin(Vector3.Dot(item.Speed, worldNormal), 0.0f) * worldNormal;
        }
      }
    }

    protected virtual void ItemIsBeingPickedBy(
      Player player,
      EnvironmentManager.FloatingItem<ItemSlot> item)
    {
      if (player == null || item == null)
        return;
      item.IsBeingPickedBy = player;
    }

    protected virtual bool ItemsPickedByPlayer(
      Player player,
      EnvironmentManager.FloatingItem<ItemSlot> item)
    {
      return player != null && player.AddItems(item.Sprite.Item, item.Sprite.Count);
    }

    protected void RemoveFloatingItem(EnvironmentManager.FloatingItem<ItemSlot> item)
    {
      if (item == null)
        return;
      this.idGenerator.ReleaseID(item.Id);
      this.floatingItems.Remove(item);
    }

    protected void RemoveFloatingItem(int environmentId)
    {
      this.RemoveFloatingItem(this.floatingItems.Find((Predicate<EnvironmentManager.FloatingItem<ItemSlot>>) (i => i.Id == environmentId)));
    }

    private void UpdateFloatingItems()
    {
      float num = 3.14159274f;
      for (int index = this.floatingItems.Count - 1; index >= 0; --index)
      {
        this.floatingItems[index].PulseAngle = MathHelper.WrapAngle(this.floatingItems[index].PulseAngle + Engine.FrameCounter.DeltaTime * num);
        this.floatingItems[index].Position += this.floatingItems[index].Speed * Engine.FrameCounter.DeltaTime;
        this.CheckAndHandleItemCollisions<ItemSlot>(this.floatingItems[index]);
        this.floatingItems[index].Speed *= 0.98f;
        this.floatingItems[index].LifetimeLeft -= (int) Engine.FrameCounter.DeltaTimeMS;
        this.floatingItems[index].PickupTimer = Math.Max(this.floatingItems[index].PickupTimer - (int) Engine.FrameCounter.DeltaTimeMS, 0);
        if (this.floatingItems[index].LifetimeLeft <= 0)
          this.RemoveFloatingItem(this.floatingItems[index]);
      }
    }

    private void UpdateGeneralItems()
    {
      for (int index = this.generalItems.Count - 1; index >= 0; --index)
      {
        this.generalItems[index].Position += this.generalItems[index].Speed * Engine.FrameCounter.DeltaTime;
        this.generalItems[index].LifetimeLeft -= (int) Engine.FrameCounter.DeltaTimeMS;
        if (this.generalItems[index].LifetimeLeft <= 0)
          this.generalItems.RemoveAt(index);
      }
    }

    private void UpdateTorchItems()
    {
      for (int index = this.torchItems.Count - 1; index >= 0; --index)
      {
        EnvironmentManager.FloatingItem<TorchItem> torchItem = this.torchItems[index];
        torchItem.Position += torchItem.Speed * Engine.FrameCounter.DeltaTime;
        torchItem.Sprite.Light.Position = torchItem.Position;
        this.CheckAndHandleItemCollisions<TorchItem>(torchItem);
        if (torchItem.LifetimeLeft >= 1)
          torchItem.Sprite.Light.Radius = (float) Math.Log((double) torchItem.LifetimeLeft, Math.E) * (float) (1.0 - this.random.NextDouble() * 0.10000000149011612);
        Vector3 speed = this.torchItems[index].Speed;
        if (speed != Vector3.Zero)
        {
          Vector3 vector3 = Vector3.Normalize(speed);
          this.torchItems[index].Speed -= 5f * vector3 * Engine.FrameCounter.DeltaTime;
        }
        this.lightingManager.DrawLight(torchItem.Sprite.Light);
        torchItem.LifetimeLeft -= (int) Engine.FrameCounter.DeltaTimeMS;
        if (torchItem.LifetimeLeft <= 0)
          this.torchItems.RemoveAt(index);
      }
    }

    public class FloatingItem<T> where T : IBillboard
    {
      public Color Color;
      public int Id;
      public T Sprite;
      public Player IsBeingPickedBy;
      public int LifetimeLeft;
      public Position3 Position;
      public int PickupTimer;
      public float PulseAngle;
      public Vector3 Speed;
      public Vector2 Size;
      public Texture2D Texture;
    }

    public class GenericBillboard : IBillboard
    {
      private Vector4 spriteCoords;

      public GenericBillboard(Vector4 coords) => this.spriteCoords = coords;

      public Vector4 SpriteCoordsUV
      {
        get => this.spriteCoords;
        set => this.spriteCoords = value;
      }
    }
  }
}
