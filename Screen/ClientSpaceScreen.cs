// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ClientSpaceScreen
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
  public class ClientSpaceScreen : SpaceScreen
  {
    private ChatWindowLayer chatLayer;
    private NetworkClientManager client;
    private LocalPlayer player;
    private bool readingInput;
    private ScoreboardLayer scoreboard;

    public ClientSpaceScreen(NetworkClientManager client, LocalPlayer player)
      : base(false)
    {
      this.client = client != null && player != null ? client : throw new ArgumentNullException();
      this.player = player;
    }

    public override void Dispose()
    {
      base.Dispose();
      this.client.Dispose();
    }

    public override void Load()
    {
      base.Load();
      this.SubscribeToEvents();
      this.CharacterManager.BindToClient(this.client);
      this.EntityManager.BindToClient(this.client);
      this.ProjectileManager.BindToClient(this.Client);
      SpaceScreen.EnvironmentManager.BindToClient(this.client);
      Engine.SettingsManager.ResolutionChangedEvent += new Action<Point>(this.PositionChatLayer);
      this.chatLayer = new ChatWindowLayer(new Rectangle(0, 0, 300, 200), 10);
      this.scoreboard = new ScoreboardLayer((IPlayerList) this.client);
      this.PositionChatLayer(Engine.SettingsManager.Resolution);
      foreach (Player player in this.client.Players)
        this.CharacterManager.AddPlayer(player);
    }

    public override void Update()
    {
      base.Update();
      if (this.client == null)
        return;
      this.client.Update((int) Engine.FrameCounter.DeltaTimeMS);
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
          this.client.Chat(message);
      }
      this.readingInput = !this.readingInput;
    }

    protected ChatWindowLayer ChatWindow => this.chatLayer;

    protected NetworkClientManager Client => this.client;

    protected LocalPlayer LocalPlayer => this.player;

    protected bool ReadingInput => this.readingInput;

    protected ScoreboardLayer Scoreboard => this.scoreboard;

    protected override void CreateManagers()
    {
      this.EntityManager = (SpaceEntityManager) new SpaceEntityManagerClient(Engine.LoadedWorld);
      if (SpaceScreen.EnvironmentManager != null)
        SpaceScreen.EnvironmentManager.Dispose();
      SpaceScreen.EnvironmentManager = (EnvironmentManager) new EnvironmentManagerClient(this.EntityManager, this.LightingManager);
      this.ProjectileManager = (ProjectileManager) new ProjectileManagerClient(this.EntityManager, SpaceScreen.EnvironmentManager, this.LightingManager);
      this.CharacterManager = (CharacterManager) new CharacterManagerClient(this.EntityManager, SpaceScreen.EnvironmentManager, this.ProjectileManager);
    }

    private void PositionChatLayer(Point resolution)
    {
    }

    private void SubscribeToEvents()
    {
      if (this.client == null)
        return;
      this.client.ChatMessageReceivedEvent += (Action<Player, string>) ((sender, message) =>
      {
        if (!(sender != null & !string.IsNullOrEmpty(message)))
          return;
        this.chatLayer.AddChatMessage(sender.Name, message, Color.White);
      });
      this.client.DisconnectedEvent += (Action<string>) (reason =>
      {
        Engine.ExitToMainMenu();
        Engine.MessageBoxShow("Disconnected from the server: " + reason);
      });
      this.client.PlayerConnected += (Action<NetworkPlayer>) (newPlayer =>
      {
        if (newPlayer != null)
        {
          this.CharacterManager.AddPlayer((Player) newPlayer);
          newPlayer.EnvironmentManager = SpaceScreen.EnvironmentManager;
        }
        this.chatLayer.AddChatMessage("SERVER", "Player " + this.player.Name + " connected!", Color.Green);
      });
      this.client.PlayerDisconnected += (Action<Player, string>) ((player, reason) =>
      {
        if (player != null)
        {
          this.CharacterManager.RemovePlayer(player);
          player.Dispose();
        }
        this.chatLayer.AddChatMessage("SERVER", "Player " + player.Name + " disconnected. (" + (reason ?? "unknown reason") + ")", Color.Red);
      });
    }
  }
}
