// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.SpaceScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Screen
{
  public abstract class SpaceScreen : GameScreen
  {
    private CharacterManager characterManager;
    private SpaceEntityManager entityManager;
    private LightingManager lightingManager;
    private ProjectileManager projectileManager;
    private SpaceDustManager spaceDustManager;
    private static EnvironmentManager environmentManager;

    public SpaceScreen(bool pausable)
      : base(GameScreen.Type.Fullscreen, pausable, MouseDevice.Behavior.Wrapped)
    {
    }

    public override void Dispose()
    {
      base.Dispose();
      this.lightingManager.Dispose();
      this.characterManager.Dispose();
      this.entityManager.Dispose();
      SpaceScreen.environmentManager.Dispose();
      this.projectileManager.Dispose();
      this.spaceDustManager.Dispose();
    }

    public override void Load()
    {
      if (Engine.LoadedWorld == null)
        throw new Exception("A world must be loaded before entering the game.");
      this.lightingManager = new LightingManager();
      this.spaceDustManager = new SpaceDustManager();
      this.CreateManagers();
      this.lightingManager.ScreenSpaceAmbientOcclusion = Engine.SettingsManager.SSAO;
      this.lightingManager.Initialize();
      Item.EntityManager = this.entityManager;
      Item.LightingManager = this.lightingManager;
      Item.ProjectileManager = this.projectileManager;
      Item.EnvironmentManager = SpaceScreen.environmentManager;
      GunBlock.ProjectileManager = this.projectileManager;
      ThrusterRenderManager.EnvironmentManager = SpaceScreen.environmentManager;
      this.spaceDustManager.InitializeDust(256);
    }

    public override void Update()
    {
      LocalPlayer localPlayer = this.characterManager.LocalPlayer;
      if (localPlayer == null)
        return;
      this.SpaceDustManager.Update(localPlayer.Astronaut.UsedCamera.Position);
    }

    protected CharacterManager CharacterManager
    {
      get => this.characterManager;
      set => this.characterManager = value;
    }

    protected CollisionManager CollisionManager => CollisionManager.Instance;

    protected SpaceEntityManager EntityManager
    {
      get => this.entityManager;
      set => this.entityManager = value;
    }

    public static EnvironmentManager EnvironmentManager
    {
      get => SpaceScreen.environmentManager;
      set => SpaceScreen.environmentManager = value;
    }

    protected LightingManager LightingManager => this.lightingManager;

    protected ProjectileManager ProjectileManager
    {
      get => this.projectileManager;
      set => this.projectileManager = value;
    }

    protected SpaceDustManager SpaceDustManager => this.spaceDustManager;

    protected virtual void CreateManagers()
    {
      this.entityManager = new SpaceEntityManager(Engine.LoadedWorld);
      if (SpaceScreen.environmentManager != null)
        SpaceScreen.environmentManager.Dispose();
      SpaceScreen.environmentManager = new EnvironmentManager(this.entityManager, this.lightingManager);
      this.projectileManager = (ProjectileManager) new ProjectileManagerSP(this.entityManager, SpaceScreen.environmentManager, this.lightingManager);
      this.characterManager = new CharacterManager(this.entityManager, SpaceScreen.environmentManager, this.projectileManager);
    }

    protected Block PickBlockForPlayer(Player player)
    {
      if (player != null)
      {
        Position3 position = player.Astronaut.Position + player.Astronaut.GetLookatVector() * 0.1f;
        Vector3 lookatVector = player.Astronaut.GetLookatVector();
        List<SpaceEntity> closeEntities = this.EntityManager.GetCloseEntities(position, 8f);
        for (int index1 = 0; index1 < 16; ++index1)
        {
          position += lookatVector * 0.1f;
          for (int index2 = 0; index2 < closeEntities.Count; ++index2)
          {
            BlockCell blockCell = closeEntities[index2].GetBlockCell(position);
            if (blockCell != null)
              return blockCell.Block;
          }
        }
      }
      return (Block) null;
    }
  }
}
