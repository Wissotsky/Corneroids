// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.ConnectingScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class ConnectingScreen : GameScreen
  {
    private const int cTimeOutTime = 30000;
    private NetworkClientManager client;
    private string address;
    private string clientName;
    private string clientToken;
    private string password;
    private int port;
    private WindowLayer backgroundLayer;
    private ButtonLayer buttonLayer;
    private bool disposed;
    private string statusText;
    private int timeOutTimer;
    private LocalPlayer receivedPlayer;
    private Action<LocalClientPlayer> connectedEvent;
    private Action<string> disconnectedEvent;
    private Action<Itemset, SpriteTextureAtlas, BlockTextureAtlas, PlayerSerializedData> worldDataReceivedEvent;

    public ConnectingScreen(string address, int port, string clientName)
      : base(GameScreen.Type.Fullscreen, false, MouseDevice.Behavior.Free)
    {
      this.address = address;
      this.clientName = clientName;
      this.port = port;
      this.disposed = false;
      if (string.IsNullOrEmpty(this.address) || string.IsNullOrEmpty(this.clientName) || this.port <= 0)
        throw new ArgumentException();
    }

    protected override void Closing()
    {
      base.Closing();
      this.disposed = true;
      if (this.client == null)
        return;
      this.client.Disconnect();
    }

    public override void Dispose()
    {
      base.Dispose();
      this.disposed = true;
      if (this.client == null)
        return;
      this.client.Disconnect();
    }

    public override void Load()
    {
      base.Load();
      try
      {
        this.backgroundLayer = new WindowLayer(Layer.EvaluateMiddlePosition(200, 100));
        this.buttonLayer = new ButtonLayer(new Rectangle(this.backgroundLayer.Location.X + (int) this.backgroundLayer.Size.X / 2 - 60, this.backgroundLayer.Location.Y + (int) this.backgroundLayer.Size.Y - 40, 120, 30), "Cancel");
        this.client = new NetworkClientManager(this.port, this.clientName, Engine.GameVersion);
        this.client.Prepare();
        this.SubscribeToEvents();
        this.statusText = "Connecting";
        Action<ServerInformation> infoRecievedDelegate = (Action<ServerInformation>) null;
        infoRecievedDelegate = (Action<ServerInformation>) (serverInfo =>
        {
          if (serverInfo != null)
          {
            if (serverInfo.PasswordRequired)
              Engine.LoadNewScreen((GameScreen) new PasswordScreen((Action<string>) (pw =>
              {
                if (!string.IsNullOrEmpty(pw))
                  this.client.ConnectTo(this.address, pw, this.clientToken);
                else
                  this.CloseScreen();
              })));
            else
              this.client.ConnectTo(this.address, string.Empty, this.clientToken);
          }
          this.client.ServerFoundEvent -= infoRecievedDelegate;
        });
        this.client.ServerFoundEvent += infoRecievedDelegate;
        this.client.DiscoverRemoteServer(this.address, this.port);
      }
      catch (Exception ex)
      {
        Engine.ExitToMainMenu();
        Engine.MessageBoxShow("Failed to connect. " + ex.Message);
        this.disposed = true;
      }
    }

    public override void Render()
    {
      if (this.disposed)
        return;
      base.Render();
      this.backgroundLayer.Render();
      this.buttonLayer.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      if (string.IsNullOrEmpty(this.statusText))
        return;
      Vector2 position = this.backgroundLayer.Position + new Vector2((float) ((double) this.backgroundLayer.Size.X * 0.5 - (double) Engine.Font.MeasureString(this.statusText).X * 0.5), 10f);
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Connecting...", position, Color.White);
      spriteBatch.End();
    }

    public override void Update()
    {
      if (this.disposed)
        return;
      base.Update();
      if (this.client != null)
        this.client.Update((int) Engine.FrameCounter.DeltaTimeMS);
      this.timeOutTimer += (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.timeOutTimer <= 30000)
        return;
      Engine.ExitToMainMenu();
      Engine.MessageBoxShow("Connection timed out!");
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonLayer.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick() || !this.buttonLayer.Contains(this.Mouse.Position))
        return;
      this.CloseScreen();
    }

    private void LoadPlayerData(Player player, PlayerSerializedData playerData)
    {
      try
      {
        player.Inventory.Extract(playerData.InventoryItems);
        player.Toolbar.Items.Extract(playerData.ToolbarItems);
        player.Astronaut.Suit.Extract(playerData.SuitItems);
        player.Astronaut.Position = playerData.Position;
        player.Astronaut.Orientation = playerData.Orientation;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to set player data from recieved bytes: " + ex.Message);
      }
    }

    private void SubscribeToEvents()
    {
      if (this.client == null)
        return;
      this.connectedEvent = (Action<LocalClientPlayer>) (player =>
      {
        if (player == null)
          return;
        if (!this.disposed)
          this.receivedPlayer = (LocalPlayer) player;
        else
          player.Dispose();
      });
      this.disconnectedEvent = (Action<string>) (reason =>
      {
        if (this.disposed)
          return;
        this.CloseScreen();
        Engine.MessageBoxShow("Disconnected from the server: " + reason);
      });
      this.worldDataReceivedEvent = (Action<Itemset, SpriteTextureAtlas, BlockTextureAtlas, PlayerSerializedData>) ((itemset, spriteAtlas, blockAtlas, playerData) =>
      {
        if (itemset != null && spriteAtlas != null && blockAtlas != null)
        {
          if (!this.disposed)
          {
            try
            {
              World newClientWorld = Engine.CreateNewClientWorld(itemset, spriteAtlas, blockAtlas);
              if (newClientWorld == null || this.receivedPlayer == null)
                throw new Exception("ClientWorld returned null or player data is missing.");
              if (!Engine.PlayWorld(newClientWorld))
                throw new Exception("PlayWorld returned false");
              this.LoadPlayerData((Player) this.receivedPlayer, playerData);
              Engine.LoadNewScreen((GameScreen) new AdventureScreenClient(this.client, this.receivedPlayer));
              this.client.ConnectedToSeverEvent -= this.connectedEvent;
              this.client.DisconnectedEvent -= this.disconnectedEvent;
              this.client.WorldDataReceivedEvent -= this.worldDataReceivedEvent;
              this.client = (NetworkClientManager) null;
              return;
            }
            catch (Exception ex)
            {
              Engine.Console.WriteErrorLine("Failed to create a client world: " + ex.Message);
              Engine.MessageBoxShow("Failed to initialize game data. " + ex.Message);
              spriteAtlas?.Dispose();
              blockAtlas?.Dispose();
              return;
            }
            finally
            {
              this.CloseScreen();
            }
          }
        }
        spriteAtlas?.Dispose();
        blockAtlas?.Dispose();
        if (this.disposed)
          return;
        this.CloseScreen();
        Engine.MessageBoxShow("Invalid itemset or textures received from the server");
      });
      this.client.ConnectedToSeverEvent += this.connectedEvent;
      this.client.DisconnectedEvent += this.disconnectedEvent;
      this.client.WorldDataReceivedEvent += this.worldDataReceivedEvent;
    }
  }
}
