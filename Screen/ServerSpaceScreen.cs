// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ServerSpaceScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public abstract class ServerSpaceScreen : SpaceScreen
  {
    private NetworkServerManager server;
    private ChatWindowLayer chatLayer;
    private int maxPlayers;
    private string name;
    private string password;
    private int port;
    private bool readingInput;
    private ScoreboardLayer scoreboard;
    private NetworkPeer.Type type;

    public ServerSpaceScreen(int maxPlayers, int portNumber, string serverName, string password)
      : base(false)
    {
      this.maxPlayers = maxPlayers;
      this.port = portNumber;
      this.password = password;
      this.name = serverName;
      this.readingInput = false;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.server != null)
        this.server.Dispose();
      Engine.SettingsManager.ResolutionChangedEvent -= new Action<Point>(this.PositionChatLayer);
    }

    public override void Load()
    {
      base.Load();
      this.server = new NetworkServerManager(Engine.LoadedWorld, this.maxPlayers, this.port, this.name, this.password);
      if (!this.server.Prepare())
      {
        Engine.MessageBoxShow("Failed to start a server.");
        this.CloseScreen();
      }
      else
      {
        this.SubscribeToEvents();
        this.CharacterManager.BindToServer(this.server);
        this.EntityManager.BindToServer(this.server);
        this.ProjectileManager.BindToServer(this.server);
        SpaceScreen.EnvironmentManager.BindToServer(this.server);
        Engine.SettingsManager.ResolutionChangedEvent += new Action<Point>(this.PositionChatLayer);
        this.chatLayer = new ChatWindowLayer(new Rectangle(0, 0, 300, 200), 10);
        this.scoreboard = new ScoreboardLayer((IPlayerList) this.server);
        this.PositionChatLayer(Engine.SettingsManager.Resolution);
      }
    }

    public override void Update()
    {
      base.Update();
      if (this.server == null)
        return;
      this.server.Update((int) Engine.FrameCounter.DeltaTimeMS);
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.chatLayer.Update(this.InputManager);
      if (!this.Keyboard.KeyPressed(Keys.Enter) || this.CharacterManager.LocalPlayer == null)
        return;
      if (!this.readingInput)
      {
        this.chatLayer.BeginListeningInput(this.InputManager);
      }
      else
      {
        string message = this.chatLayer.EndListeningInput(this.InputManager, this.CharacterManager.LocalPlayer.Name);
        if (!string.IsNullOrEmpty(message))
          this.server.Chat(message);
      }
      this.readingInput = !this.readingInput;
    }

    public ChatWindowLayer ChatWindow => this.chatLayer;

    public NetworkServerManager ServerManager => this.server;

    protected bool ReadingInput => this.readingInput;

    protected ScoreboardLayer Scoreboard => this.scoreboard;

    public event Action<Player, string> PlayerDisconnectedEvent;

    public event Action<Player> PlayerJoinedEvent;

    protected override void CreateManagers()
    {
      this.EntityManager = (SpaceEntityManager) new SpaceEntityManagerServer(Engine.LoadedWorld);
      if (SpaceScreen.EnvironmentManager != null)
        SpaceScreen.EnvironmentManager.Dispose();
      SpaceScreen.EnvironmentManager = (EnvironmentManager) new EnvironmentManagerServer(this.EntityManager, this.LightingManager);
      this.ProjectileManager = (ProjectileManager) new ProjectileManagerServer(this.EntityManager, SpaceScreen.EnvironmentManager, this.LightingManager);
      this.CharacterManager = (CharacterManager) new CharacterManagerServer(this.EntityManager, SpaceScreen.EnvironmentManager, this.ProjectileManager);
    }

    private void PositionChatLayer(Point resolution)
    {
    }

    private void SubscribeToEvents()
    {
      if (this.server == null)
        return;
      this.server.ChatMessageReceivedEvent += (Action<Player, string>) ((sender, message) =>
      {
        if (!(sender != null & !string.IsNullOrEmpty(message)))
          return;
        this.chatLayer.AddChatMessage(sender.Name, message, Color.White);
      });
      this.server.PlayerConnected += (Action<NetworkPlayer>) (player =>
      {
        if (player == null)
          return;
        this.CharacterManager.AddPlayer((Player) player);
        if (this.PlayerJoinedEvent != null)
          this.PlayerJoinedEvent((Player) player);
        this.chatLayer.AddChatMessage("SERVER: ", player.Name + " connected!", Color.Green);
      });
      this.server.PlayerDisconnected += (Action<Player, string>) ((player, reason) =>
      {
        if (player == null)
          return;
        this.CharacterManager.RemovePlayer(player);
        Engine.LoadedWorld.SavePlayer(player, (SpaceEntity[]) null);
        player.Dispose();
        if (this.PlayerDisconnectedEvent != null)
          this.PlayerDisconnectedEvent(player, reason);
        this.chatLayer.AddChatMessage("SERVER: ", player.Name + " disconnected (" + reason + ")", Color.Red);
      });
    }
  }
}
