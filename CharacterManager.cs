// Decompiled with JetBrains decompiler
// Type: CornerSpace.CharacterManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Enemies;
using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace
{
  public class CharacterManager : IDisposable, IAICharacterManagement
  {
    private const float cDistanceToEntityForParasiteSpawning = 16f;
    private const int cMaxNumberOfFlyersNearPlayer = 2;
    private const float cMaxRenderDistance = 64f;
    private const float cParasiteMaxSpawnDistance = 28f;
    private const float cParasiteMinDistance = 8f;
    private const float cParasiteMinDistToPlayer = 16f;
    private const float cParasiteSpawnDistance = 16f;
    private const int cDyingTimeMS = 200;
    private const float cSmallFlyerMinDistance = 8f;
    private const float cSmallFlyerMinSpawnDistance = 32f;
    private const float cSmallFlyerMaxSpawnDistance = 64f;
    private const int cSpawnCycleMS = 1000;
    private const float cSpawnRadius = 64f;
    private const int cSpawnTime = 3000;
    private readonly List<KeyValuePair<CharacterManager.CreateParasiteDelegate, Action<ParasiteCharacter>>> parasiteEnemies = new List<KeyValuePair<CharacterManager.CreateParasiteDelegate, Action<ParasiteCharacter>>>();
    private readonly List<CharacterManager.CreateSmallFlyerDelegate> smallFlyerEnemies = new List<CharacterManager.CreateSmallFlyerDelegate>();
    private Queue<ParasiteCharacter> mineralParasiteQueue = new Queue<ParasiteCharacter>();
    private Queue<ParasiteCharacter> eggParasiteQueue = new Queue<ParasiteCharacter>();
    private readonly List<CharacterManager.CharacterContainer> characters;
    private readonly List<AICharacter> charactersToAdd;
    private readonly List<AICharacter> charactersToRemove;
    private readonly List<CharacterManager.DyingCharacter> dyingCharacters;
    protected readonly SpaceEntityManager entityManager;
    private readonly EnvironmentManager environmentManager;
    private readonly ProjectileManager projectileManager;
    protected readonly List<Player> players;
    private readonly List<ParasiteCharacter> spawnedParasites;
    private LocalPlayer localPlayer;
    private Random rng;
    private int spawnCycleCounter;
    private bool spawnEnemies;

    public CharacterManager(
      SpaceEntityManager entityManager,
      EnvironmentManager environmentManager,
      ProjectileManager projectileManager)
    {
      if (entityManager == null || environmentManager == null || projectileManager == null)
        throw new ArgumentNullException();
      this.characters = new List<CharacterManager.CharacterContainer>();
      this.charactersToAdd = new List<AICharacter>();
      this.charactersToRemove = new List<AICharacter>();
      this.dyingCharacters = new List<CharacterManager.DyingCharacter>();
      this.entityManager = entityManager;
      this.environmentManager = environmentManager;
      this.projectileManager = projectileManager;
      this.players = new List<Player>();
      this.spawnCycleCounter = 0;
      this.spawnedParasites = new List<ParasiteCharacter>();
      this.spawnEnemies = true;
      this.rng = new Random();
      this.projectileManager.CharacterManager = this;
      this.entityManager.CharacterManager = this;
      this.InitializeEnemyTypes();
    }

    public bool AddCharacter(AICharacter character)
    {
      if (character == null || this.characters.Find((Predicate<CharacterManager.CharacterContainer>) (co => co.Character == character)) != null)
        return false;
      CharacterManager.CharacterContainer characterContainer = new CharacterManager.CharacterContainer(character);
      characterContainer.CycleCounter = this.characters.Count * 397 % character.AIUpdateCycle;
      character.ManagerInterface = (IAICharacterManagement) this;
      this.characters.Add(characterContainer);
      return true;
    }

    public void AddCharacterSafe(AICharacter character)
    {
      if (character == null)
        return;
      this.charactersToAdd.Add(character);
    }

    public virtual void AddPlayer(Player player)
    {
      if (player == null || this.players.Contains(player))
        return;
      this.players.Add(player);
      if (player is LocalPlayer)
        this.localPlayer = player as LocalPlayer;
      player.EnvironmentManager = this.environmentManager;
      player.Astronaut.DiedEvent += new Action<Character>(this.PlayerDied);
    }

    public virtual void BindToClient(NetworkClientManager client)
    {
      throw new ArgumentException("Cannot bind a client to singleplayer character manager!");
    }

    public virtual void BindToServer(NetworkServerManager server)
    {
      throw new ArgumentException("Cannot bind a server to singleplayer character manager!");
    }

    public void CheckPlayerEntityCollisions(Player player, bool checkBoarding)
    {
      Astronaut astronaut = player.Astronaut;
      List<SpaceEntity> closeEntities = this.entityManager.GetCloseEntities(astronaut.Position, 3f);
      foreach (SpaceEntity entity in closeEntities)
        CollisionManager.Instance.CheckCharacterEntityCollision((Character) astronaut, entity);
      if (!checkBoarding)
        return;
      this.CheckAndBoardCharacter((Character) player.Astronaut, (IEnumerable<SpaceEntity>) closeEntities);
    }

    public virtual void Dispose()
    {
      this.characters.ForEach((Action<CharacterManager.CharacterContainer>) (e => e.Character.Dispose()));
      this.characters.Clear();
      while (this.mineralParasiteQueue.Count > 0)
        this.mineralParasiteQueue.Dequeue().Dispose();
      foreach (Player player in this.players)
        player.Dispose();
    }

    public Player GetClosestPlayer(Position3 position)
    {
      float num1 = float.MaxValue;
      Player closestPlayer = (Player) null;
      foreach (Player player in this.players)
      {
        float num2 = (position - player.Astronaut.Position).LengthSquared();
        if ((double) num2 < (double) num1)
        {
          num1 = num2;
          closestPlayer = player;
        }
      }
      return closestPlayer;
    }

    public void RemoveCharacter(AICharacter character)
    {
      if (character == null)
        return;
      for (int index = 0; index < this.characters.Count; ++index)
      {
        if (this.characters[index].Character == character)
        {
          CharacterManager.CharacterContainer character1 = this.characters[index];
          this.characters.RemoveAt(index);
          character.BoardedEntity = (SpaceEntity) null;
          character.ManagerInterface = (IAICharacterManagement) null;
          this.dyingCharacters.Add(new CharacterManager.DyingCharacter((Character) character));
          break;
        }
      }
    }

    public void RemoveCharacterSafe(AICharacter character)
    {
      if (character == null)
        return;
      this.charactersToRemove.Add(character);
    }

    public virtual void RemovePlayer(Player player)
    {
      if (player == null)
        return;
      this.players.Remove(player);
      if (this.localPlayer == player)
        this.localPlayer = (LocalPlayer) null;
      player.Astronaut.DiedEvent -= new Action<Character>(this.PlayerDied);
      player.Dispose();
    }

    public void Render(IRenderCamera camera, ILightingInterface lightingInterface)
    {
      if (camera == null)
        return;
      this.RenderLocalPlayerThirdPerson(camera, lightingInterface);
      foreach (Player player in this.players)
      {
        if (player != this.localPlayer)
          player.RenderThirdPerson(camera, lightingInterface);
      }
      for (int index = 0; index < this.characters.Count; ++index)
      {
        AICharacter character = this.characters[index].Character;
        if ((double) (character.Position - camera.Position).LengthSquared() <= 4096.0)
          character.RenderThirdPerson(camera, lightingInterface);
      }
      foreach (CharacterManager.DyingCharacter dyingCharacter in this.dyingCharacters)
      {
        if ((double) (dyingCharacter.Character.Position - camera.Position).LengthSquared() <= 4096.0)
          dyingCharacter.Character.RenderThirdPerson(camera, lightingInterface);
      }
    }

    public bool SpaceForBlock(Position3 position, Block block)
    {
      foreach (Player player in this.players)
      {
        Astronaut astronaut = player.Astronaut;
        SpaceEntity boardedEntity = astronaut.BoardedEntity;
        Byte3 modelSize = block.ModelSize;
        if (boardedEntity != null)
        {
          BoundingSphere boundingSphere = new BoundingSphere(block.ModelPlacement, ((Vector3) modelSize * 0.5f).Length());
          boundingSphere.Center = Vector3.Transform(boundingSphere.Center, boardedEntity.Orientation);
          if ((double) (astronaut.BoundingSphere.Center - (position + boundingSphere.Center)).Length() <= (double) boundingSphere.Radius + (double) astronaut.BoundingSphere.Radius * 0.75)
            return false;
        }
      }
      return true;
    }

    public virtual void Update()
    {
      int deltaTimeMs = (int) Engine.FrameCounter.DeltaTimeMS;
      foreach (Player player in this.players)
        player.Update(Engine.FrameCounter.DeltaTime, this, this.entityManager);
      LocalPlayer localPlayer = this.LocalPlayer;
      for (int index = this.characters.Count - 1; index >= 0; --index)
      {
        AICharacter character1 = this.characters[index].Character;
        if ((double) (character1.Position - localPlayer.Astronaut.Position).Length() <= (double) Math.Min(character1.MaxUpkeepDistance, 128f))
        {
          CharacterManager.CharacterContainer character2 = this.characters[index];
          character2.CycleCounter += deltaTimeMs;
          if (character2.CycleCounter >= character2.Character.AIUpdateCycle)
          {
            character1.AICycle(this.entityManager);
            character2.CycleCounter %= character2.Character.AIUpdateCycle;
          }
          else
            character1.Update(Engine.FrameCounter.DeltaTime);
          if (character2.Character.Health <= (short) 0)
          {
            this.RemoveCharacter(character1);
          }
          else
          {
            bool flag1 = false;
            bool flag2 = false;
            if (character1.CollisionCheckRequired())
            {
              foreach (SpaceEntity closeEntity in this.entityManager.GetCloseEntities(character1.Position, 5f))
                flag1 |= CollisionManager.Instance.CheckCharacterEntityCollision((Character) character1, closeEntity);
            }
            if (flag1)
              character1.CollidedWithEntity();
            foreach (Player player in this.players)
            {
              if (CollisionManager.Instance.CheckAstronautCharacterCollision(player.Astronaut, character1))
              {
                int num = flag2 ? 1 : 0;
                flag2 = true;
              }
            }
            if (flag2)
              character1.CollidedWithPlayer();
          }
        }
        else
          this.RemoveCharacter(this.characters[index].Character);
      }
      this.RemoveListedCharacters();
      this.AddListedCharacters();
      this.UpdateDyingCharacters();
      if (localPlayer == null || !this.spawnEnemies)
        return;
      this.SpawnEnemies((Player) localPlayer);
    }

    public virtual void UpdateInput(InputFrame input)
    {
      if (this.localPlayer == null)
        return;
      this.localPlayer.UpdateInput(input);
    }

    public List<CharacterManager.CharacterContainer> Characters => this.characters;

    public int CharactersInSpace => this.characters.Count;

    public EnvironmentManager EnvironmentManager => this.environmentManager;

    public LocalPlayer LocalPlayer => this.localPlayer;

    public List<Player> Players => this.players;

    public ProjectileManager ProjectileManager => this.projectileManager;

    public virtual bool EnemiesEnabled
    {
      set => this.spawnEnemies = value;
    }

    private void AddListedCharacters()
    {
      foreach (AICharacter character in this.charactersToAdd)
      {
        if (!this.AddCharacter(character))
          character.Dispose();
      }
      this.charactersToAdd.Clear();
    }

    private void CheckAndBoardCharacter(Character character, IEnumerable<SpaceEntity> closeEntities)
    {
      if (character == null)
        return;
      CollisionData collisionData = new CollisionData();
      BoundingSphereI boundingSphere = character.BoundingSphere;
      if (character.BoardedEntity != null)
      {
        boundingSphere.Radius *= 5f;
        if (CollisionManager.Instance.GetCollision(character.BoardedEntity, boundingSphere, character.Speed, ref collisionData, false))
          return;
        character.BoardedEntity = (SpaceEntity) null;
      }
      else
      {
        if (closeEntities == null)
          return;
        boundingSphere.Radius *= 2.5f;
        foreach (SpaceEntity closeEntity in closeEntities)
        {
          if (CollisionManager.Instance.GetCollision(closeEntity, boundingSphere, character.Speed, ref collisionData, false))
            character.BoardedEntity = closeEntity;
        }
      }
    }

    private bool CheckCollisionWithPlayer(AICharacter character, Player player)
    {
      if (player != null && character != null && player.Astronaut != null)
      {
        float num1 = (player.Astronaut.Position - character.Position).Length();
        if ((double) num1 <= (double) character.BoundingSphere.Radius + (double) player.Astronaut.BoundingSphere.Radius)
        {
          player.Astronaut.CollisionWithCharacter(character);
          float num2 = player.Astronaut.BoundingSphere.Radius + character.BoundingSphere.Radius - num1;
          Vector3 vector3 = player.Astronaut.Position - character.Position;
          if (vector3 != Vector3.Zero)
          {
            vector3.Normalize();
            Astronaut astronaut = player.Astronaut;
            astronaut.Position = astronaut.Position + vector3 * num2;
          }
          return true;
        }
      }
      return false;
    }

    private void SpawnEnemies(Player player)
    {
      this.spawnCycleCounter += (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.spawnCycleCounter >= 1000)
      {
        Astronaut astronaut = player.Astronaut;
        bool flag = false;
        SpaceEntity[] array = this.entityManager.GetCloseEntities(astronaut.Position, 5f).Where<SpaceEntity>((Func<SpaceEntity, bool>) (e => e.ParasiteHostileCount < (int) ((double) ParasiteCharacter.MaxInstancesPerEntitySector * (double) e.BlockSectorManager.SectorList.Count))).ToArray<SpaceEntity>();
        if (array.Length > 0)
        {
          foreach (SpaceEntity spaceEntity in array)
          {
            Position3? positionOfClosestBlock = spaceEntity.GetPositionOfClosestBlock(player.Astronaut.Position);
            if (positionOfClosestBlock.HasValue && (double) (positionOfClosestBlock.Value - player.Astronaut.Position).Length() <= 16.0)
            {
              this.SpawnParasites(astronaut, array);
              flag = true;
              break;
            }
          }
          if (!flag)
            this.SpawnSmallFlyers(astronaut);
        }
        else
          this.SpawnSmallFlyers(astronaut);
      }
      this.spawnCycleCounter %= 1000;
    }

    private bool CreateParasiteInstances(
      CharacterManager.CreateParasiteDelegate parasiteDelegate,
      List<ParasiteCharacter> resultParasites)
    {
      if (parasiteDelegate == null || resultParasites == null)
        return false;
      ParasiteCharacter parasiteCharacter = (ParasiteCharacter) null;
      this.spawnedParasites.Clear();
      try
      {
        parasiteCharacter = parasiteDelegate();
        if (parasiteCharacter != null)
        {
          this.spawnedParasites.Add(parasiteCharacter);
          int num = parasiteCharacter.MaxHostileCountPerSpawnCycle - 1;
          for (int index = 0; index < num; ++index)
          {
            parasiteCharacter = parasiteDelegate();
            if (parasiteCharacter != null)
              this.spawnedParasites.Add(parasiteCharacter);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to spawn parasites: " + ex.Message);
        this.spawnedParasites.ForEach((Action<ParasiteCharacter>) (p => p.Dispose()));
        parasiteCharacter?.Dispose();
        return false;
      }
    }

    private Position3 evaluateRandomSpawnPosition(Astronaut astronaut, SpaceEntity closestEntity)
    {
      if (astronaut == null || closestEntity == null)
        return Position3.Zero;
      Position3 position = closestEntity.Position;
      double radius = (double) closestEntity.BoundingSphere.Radius;
      Position3 randomSpawnPosition = new Vector3((float) this.rng.NextDouble() - 0.5f, (float) this.rng.NextDouble() - 0.5f, (float) this.rng.NextDouble() - 0.5f) * 28f + astronaut.Position;
      Vector3 vector3 = randomSpawnPosition - astronaut.Position;
      if ((double) vector3.Length() > 16.0)
        return randomSpawnPosition;
      if (vector3 != Vector3.Zero)
        vector3.Normalize();
      else
        vector3 = Vector3.Up;
      return astronaut.Position + vector3 * 16f;
    }

    private void InitializeEnemyTypes()
    {
      KeyValuePair<CharacterManager.CreateParasiteDelegate, Action<ParasiteCharacter>> keyValuePair1 = new KeyValuePair<CharacterManager.CreateParasiteDelegate, Action<ParasiteCharacter>>((CharacterManager.CreateParasiteDelegate) (() => this.mineralParasiteQueue.Count > 0 ? this.mineralParasiteQueue.Dequeue() : (ParasiteCharacter) new MineralParasite()), (Action<ParasiteCharacter>) (parasite =>
      {
        if (parasite == null)
          return;
        this.mineralParasiteQueue.Enqueue(parasite);
      }));
      KeyValuePair<CharacterManager.CreateParasiteDelegate, Action<ParasiteCharacter>> keyValuePair2 = new KeyValuePair<CharacterManager.CreateParasiteDelegate, Action<ParasiteCharacter>>((CharacterManager.CreateParasiteDelegate) (() => this.eggParasiteQueue.Count > 0 ? this.eggParasiteQueue.Dequeue() : (ParasiteCharacter) new SwarmEggParasite()), (Action<ParasiteCharacter>) (parasite =>
      {
        if (parasite == null)
          return;
        this.eggParasiteQueue.Enqueue(parasite);
      }));
      CharacterManager.CreateSmallFlyerDelegate smallFlyerDelegate = (CharacterManager.CreateSmallFlyerDelegate) (() => (SmallFlyerCharacter) new ScavengerFlyer());
      this.parasiteEnemies.Add(keyValuePair1);
      this.parasiteEnemies.Add(keyValuePair2);
    }

    private void PlayerDied(Character astronaut)
    {
      if (astronaut == null)
        return;
      Player player = this.players.Find((Predicate<Player>) (p => p.Astronaut == astronaut));
      if (player == null)
        return;
      this.ShowDiedDialog(player);
      this.dyingCharacters.Add(new CharacterManager.DyingCharacter(astronaut));
    }

    private void RemoveListedCharacters()
    {
      foreach (AICharacter character in this.charactersToRemove)
      {
        this.RemoveCharacter(character);
        character.Dispose();
      }
      this.charactersToRemove.Clear();
    }

    private void RenderLocalPlayerThirdPerson(
      IRenderCamera camera,
      ILightingInterface lightingInterface)
    {
      if (this.localPlayer == null || this.localPlayer.Astronaut.UsedCamera == this.localPlayer.Astronaut)
        return;
      this.localPlayer.RenderThirdPerson(camera, lightingInterface);
    }

    protected virtual void ShowDiedDialog(Player player)
    {
      LocalPlayer localPlayer = player as LocalPlayer;
      if (localPlayer == null)
        return;
      Engine.LoadNewScreen((GameScreen) new MessageScreen("You have died", "Oh noes!", true, "Respawn", (System.Action) (() => this.SpawnPlayer((Player) localPlayer))));
    }

    private void SpawnSmallFlyers(Astronaut astronaut)
    {
      if (this.smallFlyerEnemies.Count <= 0 || this.characters.Count<CharacterManager.CharacterContainer>((Func<CharacterManager.CharacterContainer, bool>) (c => (double) (c.Character.Position - astronaut.Position).Length() <= 64.0)) > 2)
        return;
      Position3? nullable = new Position3?();
      int num1 = 0;
      SmallFlyerCharacter character = this.smallFlyerEnemies[this.rng.Next(0, this.smallFlyerEnemies.Count)]();
      for (; !nullable.HasValue && num1 <= 100; ++num1)
      {
        Vector3 vector3 = new Vector3((float) (this.rng.NextDouble() - 0.5) * 2f, (float) (this.rng.NextDouble() - 0.5) * 2f, (float) (this.rng.NextDouble() - 0.5) * 2f);
        if (vector3 != Vector3.Zero)
        {
          vector3.Normalize();
          float num2 = (float) (this.rng.NextDouble() * 32.0 + 32.0);
          Position3 worldPosition = astronaut.Position + vector3 * num2;
          nullable = character.CheckPositionForSpawning(worldPosition, this.entityManager.GetClosestEntity(astronaut.Position, 0.0f));
        }
      }
      if (nullable.HasValue)
      {
        character.Spawn(nullable.Value);
        if (this.AddCharacter((AICharacter) character))
          return;
        character.Dispose();
      }
      else
        character.Dispose();
    }

    private void SpawnParasites(Astronaut astronaut, SpaceEntity[] closestEntities)
    {
      if (this.parasiteEnemies.Count <= 0)
        return;
      KeyValuePair<CharacterManager.CreateParasiteDelegate, Action<ParasiteCharacter>> parasiteEnemy = this.parasiteEnemies[this.rng.Next(0, this.parasiteEnemies.Count)];
      CharacterManager.CreateParasiteDelegate key = parasiteEnemy.Key;
      Action<ParasiteCharacter> action = parasiteEnemy.Value;
      if (!this.CreateParasiteInstances(key, this.spawnedParasites))
        return;
      foreach (ParasiteCharacter spawnedParasite in this.spawnedParasites)
      {
        SpaceEntity closestEntity = closestEntities[this.rng.Next(0, closestEntities.Length)];
        if (closestEntity != null)
        {
          Position3 spawnPosition = this.evaluateRandomSpawnPosition(astronaut, closestEntity);
          if (this.characters.Find((Predicate<CharacterManager.CharacterContainer>) (c => (double) (c.Character.Position - spawnPosition).LengthSquared() < 64.0)) == null)
          {
            Position3? nullable = spawnedParasite.CheckPositionForSpawning(spawnPosition, closestEntity);
            if (nullable.HasValue)
            {
              spawnedParasite.SpawnAt(nullable.Value, this.entityManager);
              spawnedParasite.BoardedEntity = closestEntity;
              if (!this.AddCharacter((AICharacter) spawnedParasite))
                action(spawnedParasite);
            }
            else
              action(spawnedParasite);
          }
          else
            action(spawnedParasite);
        }
        else
          action(spawnedParasite);
      }
      this.spawnedParasites.Clear();
    }

    protected virtual void SpawnPlayer(Player player)
    {
      if (player == null)
        return;
      bool flag = false;
      Random random = new Random();
      player.Astronaut.ModificationMatrix = Matrix.Identity;
      while (!flag)
      {
        Vector3 vector3 = new Vector3((float) random.NextDouble(), (float) random.NextDouble(), (float) random.NextDouble());
        if (vector3 != Vector3.Zero)
        {
          vector3.Normalize();
          int num = random.Next(64, 96);
          Position3 position3 = player.Astronaut.Position + vector3 * (float) num;
          if (this.entityManager.PickBlock(position3) == null)
          {
            Position3 position = player.Astronaut.Position;
            player.Astronaut.Spawn(position3);
            this.environmentManager.AddSpawnParticles(position3);
            player.Astronaut.LookAt(position - position3);
            flag = true;
          }
        }
      }
    }

    private void UpdateDyingCharacters()
    {
      for (int index = this.dyingCharacters.Count - 1; index >= 0; --index)
      {
        CharacterManager.DyingCharacter dyingCharacter = this.dyingCharacters[index];
        dyingCharacter.Character.ModificationMatrix = Matrix.CreateScale((float) dyingCharacter.Counter / 200f);
        dyingCharacter.Counter -= (int) Engine.FrameCounter.DeltaTimeMS;
        if (dyingCharacter.Counter <= 0)
        {
          this.dyingCharacters.RemoveAt(index);
          this.EnvironmentManager.AddExplosion(dyingCharacter.Character.Position, dyingCharacter.Character.BloodColor);
          this.EnvironmentManager.AddExplosion(dyingCharacter.Character.Position, dyingCharacter.Character.BloodColor);
        }
      }
    }

    private delegate ParasiteCharacter CreateParasiteDelegate();

    private delegate SmallFlyerCharacter CreateSmallFlyerDelegate();

    public class CharacterContainer
    {
      private AICharacter character;
      private int cycleCounter;

      public CharacterContainer(AICharacter character)
      {
        this.character = character;
        this.cycleCounter = 0;
      }

      public AICharacter Character
      {
        get => this.character;
        set
        {
          this.character = value;
          this.cycleCounter = 0;
        }
      }

      public int CycleCounter
      {
        get => this.cycleCounter;
        set => this.cycleCounter = value;
      }
    }

    private class DyingCharacter
    {
      public Character Character;
      public int Counter;

      public DyingCharacter(Character character)
      {
        this.Character = character;
        this.Counter = 200;
      }
    }
  }
}
