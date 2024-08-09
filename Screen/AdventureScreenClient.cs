// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.AdventureScreenClient
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Screen
{
  public class AdventureScreenClient : ClientSpaceScreen
  {
    private bool showScoreboard;

    public AdventureScreenClient(NetworkClientManager client, LocalPlayer player)
      : base(client, player)
    {
    }

    public override void Load()
    {
      base.Load();
      this.ChatWindow.AddChatMessage("SERVER", "Welcome to " + this.Client.ServerName + "!", Color.Orange);
    }

    public override void Render()
    {
      LocalPlayer localPlayer = this.CharacterManager.LocalPlayer;
      this.LightingManager.Begin();
      this.EntityManager.Render((IRenderCamera) localPlayer.Astronaut.UsedCamera, this.LightingManager);
      this.CharacterManager.Render((IRenderCamera) localPlayer.Astronaut.UsedCamera, (ILightingInterface) this.LightingManager);
      foreach (Player player in this.CharacterManager.Players)
        player.RenderLights((ILightingInterface) this.LightingManager);
      localPlayer.RenderFirstPersonWithLighting();
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
      localPlayer.RenderFirstPerson();
      localPlayer.RenderGUI();
      if (this.ChatWindow != null)
        this.ChatWindow.Render();
      if (this.showScoreboard)
        this.Scoreboard.Render();
      if (!Engine.DebugMode)
        return;
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      Engine.SpriteBatch.DrawString(Engine.Font, "local orientation: " + (object) localPlayer.Astronaut.Orientation, new Vector2(10f, 300f), Color.White);
      Engine.SpriteBatch.DrawString(Engine.Font, "local world orientation: " + (object) localPlayer.Astronaut.WorldOrientation, new Vector2(10f, 320f), Color.White);
      Engine.SpriteBatch.DrawString(Engine.Font, "local speed: " + (object) localPlayer.Astronaut.Speed.Length(), new Vector2(10f, 340f), Color.White);
      for (int index = 0; index < this.CharacterManager.Players.Count; ++index)
      {
        if (this.CharacterManager.Players[index] is NetworkPlayerOnClient)
          Engine.SpriteBatch.DrawString(Engine.Font, "sync value: " + (object) this.CharacterManager.Players[index].SynchronizationValue.StoredValue, new Vector2(10f, 360f), Color.White);
      }
      spriteBatch.End();
    }

    public override void Update()
    {
      this.ProjectileManager.Update();
      base.Update();
      this.EntityManager.Update((IEnumerable<Player>) this.CharacterManager.Players);
      this.CharacterManager.Update();
      SpaceScreen.EnvironmentManager.Update();
      SpaceScreen.EnvironmentManager.TryPickItemsToPlayers((IEnumerable<Player>) this.CharacterManager.Players);
      foreach (Player player in this.CharacterManager.Players)
        player.PickedBlock = this.PickBlockForPlayer(player);
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      LocalPlayer localPlayer = this.CharacterManager.LocalPlayer;
      if (this.Keyboard.KeyPressed(Keys.Escape))
        Engine.LoadNewScreen((GameScreen) new GameMenuClient());
      if (!this.ReadingInput)
        this.CharacterManager.UpdateInput(this.InputFrame);
      if (this.Keyboard.KeyPressed(Keys.F5))
        Engine.LoadNewScreen((GameScreen) new ItemListScreen((Player) localPlayer));
      this.showScoreboard = this.InputFrame.KeyDown(Keys.Tab);
    }
  }
}
