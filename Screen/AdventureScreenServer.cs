// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.AdventureScreenServer
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
  public class AdventureScreenServer : ServerSpaceScreen
  {
    private bool showScoreboard;

    public AdventureScreenServer(
      int maxPlayers,
      int portNumber,
      string serverName,
      string password)
      : base(maxPlayers, portNumber, serverName, password)
    {
    }

    public override void Load()
    {
      base.Load();
      LocalPlayer player = Engine.LoadedWorld.LoadLocalPlayer().Player;
      player.Name = Engine.StoredValues.RetrieveValue<string>("playerName") ?? "noname";
      this.CharacterManager.AddPlayer((Player) player);
      this.ServerManager.SetLocalPlayer(player);
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
      spriteBatch.DrawString(Engine.Font, BlockItem.iii.ToString(), new Vector2(10f, 420f), Color.White);
      if (this.CharacterManager.Players.Count >= 2)
      {
        Player player = this.CharacterManager.Players[1];
        Engine.SpriteBatch.DrawString(Engine.Font, "client camera orientation: " + player.Astronaut.UsedCamera.Orientation.ToString(), new Vector2(10f, 400f), Color.White);
      }
      for (int index = 0; index < this.CharacterManager.Players.Count; ++index)
      {
        int num = this.CharacterManager.Players[index].Astronaut.ActiveItem != (Item) null ? 1 : 0;
      }
      spriteBatch.End();
    }

    public void SaveAndExit()
    {
      foreach (Player player in this.CharacterManager.Players)
        Engine.LoadedWorld.SavePlayer(player, (SpaceEntity[]) null);
      this.ServerManager.Disconnect();
      this.EntityManager.Save();
    }

    public override void Update()
    {
      base.Update();
      this.EntityManager.Update((IEnumerable<Player>) this.CharacterManager.Players);
      this.ProjectileManager.Update();
      this.CharacterManager.Update();
      SpaceScreen.EnvironmentManager.Update();
      SpaceScreen.EnvironmentManager.TryPickItemsToPlayers((IEnumerable<Player>) this.CharacterManager.Players);
      foreach (Player player in this.CharacterManager.Players)
        player.PickedBlock = this.PickBlockForPlayer(player);
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (!this.ReadingInput)
        this.CharacterManager.UpdateInput(this.InputFrame);
      if (this.Keyboard.KeyPressed(Keys.Escape))
        Engine.LoadNewScreen((GameScreen) new GameMenuServer(this));
      if (this.Keyboard.KeyPressed(Keys.F5))
        Engine.LoadNewScreen((GameScreen) new ItemListScreen((Player) this.CharacterManager.LocalPlayer));
      this.showScoreboard = this.InputFrame.KeyDown(Keys.Tab);
    }
  }
}
