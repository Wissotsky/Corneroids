// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.AdventureScreenSP
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace.Screen
{
  public class AdventureScreenSP : SpaceScreen
  {
    private const string buildingStateName = "buildingState";
    private const string flyingStateName = "flyingState";
    private readonly Vector3 cSpawnPosition = new Vector3(160f, 210f, 64f);
    private readonly Quaternion cSpawnOrientation = Quaternion.Normalize(new Quaternion(0.0f, 0.85f, 0.5f, 0.0f));
    private BuilderManagerSP builderManager;
    private bool drawGUI;
    private long previousTicks;

    public AdventureScreenSP()
      : base(false)
    {
      this.drawGUI = true;
    }

    public override void Dispose() => base.Dispose();

    public override void Load()
    {
      base.Load();
      this.LoadLocalPlayer();
      this.builderManager = new BuilderManagerSP(this.EntityManager);
      this.LoadBeacons();
      Engine.LoadNewScreen((GameScreen) new SingleplayerTutorialScreen());
    }

    public override void Render()
    {
      LocalPlayer localPlayer = this.CharacterManager.LocalPlayer;
      this.LightingManager.Begin();
      this.EntityManager.Render((IRenderCamera) localPlayer.Astronaut.UsedCamera, this.LightingManager);
      localPlayer.RenderFirstPersonWithLighting();
      localPlayer.RenderLights((ILightingInterface) this.LightingManager);
      this.CharacterManager.Render((IRenderCamera) localPlayer.Astronaut.UsedCamera, (ILightingInterface) this.LightingManager);
      this.LightingManager.End((IRenderCamera) localPlayer.Astronaut.UsedCamera);
      Engine.LoadedWorld.RenderBackground((IRenderCamera) localPlayer.Astronaut.UsedCamera);
      this.ProjectileManager.Render((IRenderCamera) localPlayer.Astronaut.UsedCamera);
      SpaceScreen.EnvironmentManager.Render((IRenderCamera) localPlayer.Astronaut.UsedCamera);
      this.SpaceDustManager.Render((IRenderCamera) localPlayer.Astronaut.UsedCamera);
      if (Engine.SettingsManager.DrawTargetSquares)
      {
        SpaceScreen.EnvironmentManager.RenderMarkers<SpaceEntity>((IEnumerable<SpaceEntity>) this.EntityManager.Entities, (IRenderCamera) localPlayer.Astronaut.UsedCamera, new Color((byte) 20, (byte) 110, (byte) 10, (byte) 125));
        SpaceScreen.EnvironmentManager.RenderMarkers<Player>((IEnumerable<Player>) this.CharacterManager.Players, (IRenderCamera) localPlayer.Astronaut.UsedCamera, Color.LightBlue);
      }
      SpaceScreen.EnvironmentManager.RenderMarkers<BeaconObject>((IEnumerable<BeaconObject>) this.EntityManager.Beacons, (IRenderCamera) localPlayer.Astronaut.UsedCamera, Color.Orange);
      localPlayer.RenderFirstPerson();
      if (this.drawGUI)
        localPlayer.RenderGUI();
      if (!Engine.DebugMode || !this.drawGUI)
        return;
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Entities in space: " + this.EntityManager.EntityCount.ToString(), Vector2.One * 10f, Color.White);
      spriteBatch.DrawString(Engine.Font, "Astronaut position: " + localPlayer.Astronaut.Position.ToString(), new Vector2(10f, 30f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Memory allocated: " + (GC.GetTotalMemory(false) / 1000000L).ToString() + " megabytes", new Vector2(10f, 50f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Delta time: " + (object) Engine.FrameCounter.DeltaTime, new Vector2(10f, 70f), Color.White);
      spriteBatch.DrawString(Engine.Font, "AI characters in space: " + (object) this.CharacterManager.CharactersInSpace, new Vector2(10f, 90f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Up-vector: " + localPlayer.Astronaut.GetUpVector().ToString(), new Vector2(10f, 110f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Delta ticks: " + (Engine.FrameCounter.Ticks - this.previousTicks).ToString(), new Vector2(10f, 130f), Color.White);
      this.previousTicks = Engine.FrameCounter.Ticks;
      if (localPlayer.Astronaut.BoardedEntity != null)
      {
        spriteBatch.DrawString(Engine.Font, "Entity rotation: " + (object) localPlayer.Astronaut.BoardedEntity.Rotation.Length(), new Vector2(10f, 150f), Color.White);
        spriteBatch.DrawString(Engine.Font, "Entity speed: " + (object) localPlayer.Astronaut.BoardedEntity.Speed.Length(), new Vector2(10f, 170f), Color.White);
        spriteBatch.DrawString(Engine.Font, "Entity block count: " + (object) localPlayer.Astronaut.BoardedEntity.BlockCount, new Vector2(10f, 190f), Color.White);
        spriteBatch.DrawString(Engine.Font, "Entity id: " + (object) localPlayer.Astronaut.BoardedEntity.Id, new Vector2(10f, 210f), Color.White);
      }
      spriteBatch.DrawString(Engine.Font, "Number of created vertexbuffers: " + (object) BlockSector.GraphicBufferManager.NumberOfCreatedVertexBuffers, new Vector2(10f, 230f), Color.White);
      spriteBatch.DrawString(Engine.Font, "Number of reused vertexbuffers: " + (object) BlockSector.GraphicBufferManager.NumberOfReusedVertexBuffers, new Vector2(10f, 250f), Color.White);
      spriteBatch.End();
    }

    public void Save()
    {
      LocalPlayer player = this.CharacterManager.LocalPlayer;
      IEnumerable<SpaceEntity> source = this.EntityManager.GetCloseEntities(player.Astronaut.Position, 8f).Where<SpaceEntity>((Func<SpaceEntity, bool>) (e =>
      {
        float? squaredToClosestBlock = e.GetDistanceSquaredToClosestBlock(player.Astronaut.Position);
        return (double) squaredToClosestBlock.GetValueOrDefault() <= 1024.0 && squaredToClosestBlock.HasValue;
      }));
      Engine.LoadedWorld.SavePlayer((Player) player, source.ToArray<SpaceEntity>());
      this.EntityManager.Save();
    }

    public override void Update()
    {
      LocalPlayer localPlayer = this.CharacterManager.LocalPlayer;
      base.Update();
      this.EntityManager.Update((IEnumerable<Player>) this.CharacterManager.Players);
      this.ProjectileManager.Update();
      this.CharacterManager.Update();
      SpaceScreen.EnvironmentManager.Update();
      SpaceScreen.EnvironmentManager.TryPickItemsToPlayers((IEnumerable<Player>) this.CharacterManager.Players);
      localPlayer.PickedBlock = this.PickBlockForPlayer((Player) localPlayer);
    }

    public override void UpdateInput()
    {
      LocalPlayer localPlayer = this.CharacterManager.LocalPlayer;
      if (this.Keyboard.KeyPressed(Keys.Escape))
        Engine.LoadNewScreen((GameScreen) new GameMenuSingleplayerScreen(this));
      if (this.Keyboard.KeyPressed(Keys.F1))
        Engine.LoadNewScreen((GameScreen) new SingleplayerTutorialScreen());
      if (this.Keyboard.KeyPressed(Keys.F6))
        this.drawGUI = !this.drawGUI;
      this.CharacterManager.UpdateInput(this.InputFrame);
      if (!Engine.DebugMode)
        return;
      if (this.Keyboard.KeyPressed(Keys.F5))
        Engine.LoadNewScreen((GameScreen) new ItemListScreen((Player) localPlayer));
      if (!this.Keyboard.KeyPressed(Keys.K))
        return;
      localPlayer.Astronaut.Kill();
    }

    private void LoadBeacons()
    {
      if (Engine.LoadedWorld == null)
        return;
      List<BeaconObject> beaconObjectList = Engine.LoadedWorld.LoadBeacons();
      if (beaconObjectList == null)
        return;
      foreach (BeaconObject beacon in beaconObjectList)
        this.EntityManager.AddBeacon(beacon, false);
    }

    private void LoadLocalPlayer()
    {
      if (Engine.LoadedWorld == null)
        return;
      LoadedPlayerContainer<LocalPlayer> loadedPlayerContainer = Engine.LoadedWorld.LoadLocalPlayer();
      LocalPlayer newPlayer = loadedPlayerContainer != null ? loadedPlayerContainer.Player : throw new Exception("Failed to load the player.");
      newPlayer.EnvironmentManager = SpaceScreen.EnvironmentManager;
      this.CharacterManager.AddPlayer((Player) newPlayer);
      if (loadedPlayerContainer.CreationSource == LoadedPlayerContainer<LocalPlayer>.Source.Database)
      {
        int boardedEntityId = loadedPlayerContainer.BoardedEntityId;
        int[] boundEntityIds = loadedPlayerContainer.BoundEntityIds;
        if (boardedEntityId == -1 && boundEntityIds.Length <= 0)
          return;
        List<int> entitiesToLoad = new List<int>();
        if (boardedEntityId != -1)
          entitiesToLoad.Add(boardedEntityId);
        if (boundEntityIds.Length > 0)
          entitiesToLoad.AddRange((IEnumerable<int>) boundEntityIds);
        entitiesToLoad = ((IEnumerable<int>) boundEntityIds).Distinct<int>().ToList<int>();
        MessageScreen loadingScreen = new MessageScreen("Loading", "Please wait", false);
        Engine.LoadNewScreen((GameScreen) loadingScreen);
        Action<SpaceEntity> entityListener = (Action<SpaceEntity>) null;
        entityListener = (Action<SpaceEntity>) (entity =>
        {
          if (entity == null)
            return;
          entitiesToLoad.Remove(entity.Id);
          if (entitiesToLoad.Count != 0)
            return;
          loadingScreen.CloseScreen();
          this.EntityManager.EntityAddedEvent -= entityListener;
        });
        this.EntityManager.EntityAddedEvent += entityListener;
      }
      else
      {
        if (loadedPlayerContainer.CreationSource != LoadedPlayerContainer<LocalPlayer>.Source.New)
          return;
        newPlayer.Astronaut.Orientation = Quaternion.Identity;
        foreach (Item defaultItem in Engine.LoadedWorld.Itemset.DefaultItems)
          newPlayer.Inventory.AddItems(defaultItem, 1);
        AsteroidCreationParameters creationParameters = new AsteroidCreationParameters();
        creationParameters.PositionBounds = new BoundingBoxI(Position3.Zero, new Position3(Vector3.One * (float) Engine.LoadedWorld.SectorSize));
        creationParameters.PreferredSize = Vector3.One * 64f;
        creationParameters.Smoothness = (byte) 5;
        creationParameters.Seed = 171087;
        AsteroidCreationParameters parameters = creationParameters;
        MessageScreen loadingScreen = new MessageScreen("Loading", "Please wait", false);
        Engine.LoadNewScreen((GameScreen) loadingScreen);
        SpaceEntityFactory.Instance.CreateEntityAsync(new SpaceEntityFactory.CreateEntityFunction(SpaceEntityFactory.Instance.CreateAsteroid), (EntityCreationParameters) parameters, (Action<SpaceEntity>) (entity =>
        {
          if (entity != null)
          {
            this.EntityManager.AddAsyncConstructedEntity(entity);
            newPlayer.Astronaut.Position = entity.Position + Vector3.Up * 24f;
          }
          loadingScreen.CloseScreen();
        }));
      }
    }
  }
}
