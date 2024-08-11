// Decompiled with JetBrains decompiler
// Type: CornerSpace.Engine
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class Engine : Game
  {
    private const string storedValuesFileName = "values.xml";
    private EventHandler deviceLostHandler;
    private string logPath;
    private GraphicsDeviceManager graphics;
    private InputManager inputManager;
    private Texture2D mouseCursor;
    private List<GameScreen> screensToDelete;
    private string userDataFolder;
    public static readonly int GameVersion = 12000;
    public static readonly float WorldVersion = 1.2f;
    private static Console console;
    private static bool debugMode;
    private static bool exitRequested;
    private static bool fastTravel;
    private static List<GameScreen> gameScreens;
    private static List<Itemset> loadedItemsets;
    private static List<BlockTextureAtlas> loadedBlockTextureAtlases;
    private static World loadedWorld;
    private static List<SpriteTextureAtlas> loadedSpriteTextureAtlases;
    private static List<World> loadedWorlds;
    private static FrameCounter frameCounter;
    private static SettingsManager settingsManager;
    private static Queue<GameScreen> screensToAdd;
    private static StoredValues storedvalues;
    private static TimedEventManager timedEventsManager;
    public static BasicEffect BasicEffect;
    public static ContentManager ContentManager;
    public static SpriteFont Font;
    public static GraphicsDevice GraphicsDevice;
    public static Effect[] ShaderPool;
    public static SpriteBatch SpriteBatch;
    public static VertexDeclaration[] VertexDeclarationPool;

    public Engine(Engine.LaunchParameters launchParams)
    {
      Engine.debugMode = (launchParams & Engine.LaunchParameters.Debug) > Engine.LaunchParameters.Normal;
      Engine.fastTravel = (launchParams & Engine.LaunchParameters.FastTravel) > Engine.LaunchParameters.Normal;
      this.Content.RootDirectory = "Content";
      Engine.frameCounter = new FrameCounter();
      this.graphics = new GraphicsDeviceManager((Game) this);
      Engine.ContentManager = this.Content;
      this.inputManager = new InputManager(this.Window);
      Engine.timedEventsManager = new TimedEventManager();
      Engine.gameScreens = new List<GameScreen>();
      Engine.loadedBlockTextureAtlases = new List<BlockTextureAtlas>();
      Engine.loadedSpriteTextureAtlases = new List<SpriteTextureAtlas>();
      Engine.screensToAdd = new Queue<GameScreen>();
      this.screensToDelete = new List<GameScreen>();
      Engine.loadedItemsets = new List<Itemset>();
      Engine.loadedWorlds = new List<World>();
      Engine.exitRequested = false;
      this.IsFixedTimeStep = true;
      this.TargetElapsedTime = TimeSpan.FromMilliseconds(8.0);
    }

    protected override void Initialize()
    {
      try
      {
      this.userDataFolder = this.PrepareUserDataFolder();
      this.logPath = this.userDataFolder;
      if (string.IsNullOrEmpty(this.userDataFolder))
        this.Exit();
      Engine.ContentManager = this.Content;
      Engine.GraphicsDevice = this.graphics.GraphicsDevice;
      Engine.console = new Console();
      Engine.settingsManager = this.PrepareSettings(this.graphics);
      Engine.storedvalues = this.LoadStoredValues();
      this.Window.Title = "Corneroids";
      if (this.SufficientPcCapabilities(Engine.GraphicsDevice))
      {
        this.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
        Engine.SpriteBatch = new SpriteBatch(this.graphics.GraphicsDevice);
        Engine.BasicEffect = new BasicEffect(this.graphics.GraphicsDevice);
        Engine.settingsManager.ApplyChanges();
        Engine.Font = Engine.ContentManager.Load<SpriteFont>("Textures\\Sprites\\fontImage");
        Engine.Font.DefaultCharacter = '?';
        this.mouseCursor = Engine.ContentManager.Load<Texture2D>("Textures\\mouseCursor");
        this.InitializeTextureRegisters();
        this.InitializeVertexDeclarations();
        this.InitializeShaders();
        this.LoadBlockTextureAtlases(this.Content.RootDirectory + "\\Textures\\Blocks\\");
        this.LoadSpriteTextureAtlases(this.Content.RootDirectory + "\\Textures\\Sprites\\");
        this.LoadBlocksets(this.Content.RootDirectory + "\\Blocksets\\");
        this.IsMouseVisible = false;
        Engine.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(this.GraphicsDeviceReseted);
        Engine.SettingsManager.GammaChangedEvent += new Action<int>(this.GammaChanged);
        this.GammaChanged(Engine.SettingsManager.Gamma);
        base.Initialize();
        this.AddNewScreen(new MainMenuScreen());
      }
      else
      {
        int num = (int) MessageBox.Show("Your graphics device does not meet the requirements to play the game :(");
        this.Exit();
      }
      }
      catch
      {
      int num = (int) MessageBox.Show("Failed to initialize the game.");
      this.Exit();
      }
    }

    protected override void LoadContent()
    {
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
      base.OnActivated(sender, args);
      this.inputManager.GetMouseListener().RestoreState();
    }

    protected override void OnDeactivated(object sender, EventArgs args)
    {
      base.OnDeactivated(sender, args);
      MouseDevice mouseListener = this.inputManager.GetMouseListener();
      mouseListener.StoreState();
      mouseListener.CursorBehavior = MouseDevice.Behavior.Free;
    }

    protected override void UnloadContent()
    {
      foreach (GameScreen gameScreen in Engine.gameScreens)
        gameScreen.Dispose();
      this.Content.Unload();
      foreach (TextureAtlas blockTextureAtlase in Engine.loadedBlockTextureAtlases)
        blockTextureAtlase.Dispose();
      foreach (TextureAtlas spriteTextureAtlase in Engine.loadedSpriteTextureAtlases)
        spriteTextureAtlase.Dispose();
      for (int index = 0; index < Engine.VertexDeclarationPool.Length; ++index)
      {
        if (Engine.VertexDeclarationPool[index] != null)
          Engine.VertexDeclarationPool[index].Dispose();
      }
      if (Engine.SpriteBatch != null)
        Engine.SpriteBatch.Dispose();
      if (Engine.BasicEffect != null)
        Engine.BasicEffect.Dispose();
      if (Engine.loadedWorld != null)
        Engine.loadedWorld.Dispose();
      base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
      try
      {
        Engine.frameCounter.Execute();
        bool flag = false;
        foreach (GameScreen gameScreen in Engine.gameScreens)
        {
          if (gameScreen.ExitScreen != null)
            this.screensToDelete.Add(gameScreen.ExitScreen);
        }
        this.DeleteScreens(this.screensToDelete);
        this.AddNewScreens(Engine.screensToAdd);
        this.inputManager.Update();
        if (Engine.gameScreens.Count == 0 || Engine.exitRequested)
        {
          this.Exit();
        }
        else
        {
          foreach (GameScreen gameScreen in Engine.gameScreens.Reverse<GameScreen>())
          {
            if (!flag)
            {
              if (this.IsActive)
                gameScreen.UpdateInput();
              gameScreen.Update();
              flag = true;
            }
            else
              gameScreen.Update();
          }
        }
        BlockSector.GraphicBufferManager.Update();
        Engine.timedEventsManager.Update((int) Engine.frameCounter.DeltaTimeMS);
        base.Update(gameTime);
      }
      catch (ContentLoadException ex)
      {
        int num = (int) MessageBox.Show("Failed to load content: " + ex.Message);
        Engine.Console.WriteErrorLine(ex.Message);
        this.Exit();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Unhandled error caught! More detailed info saved in errorlog.txt.");
        Engine.console.WriteErrorLine(ex.Message + Environment.NewLine + ex.StackTrace);
        this.Exit();
      }
    }

    protected override void Draw(GameTime gameTime)
    {
      try
      {
        if (GraphicsDevice.GraphicsDeviceStatus != GraphicsDeviceStatus.Normal)
          return;
        SettingsManager.InitializeFrame();
        for (int index = Math.Max(0, gameScreens.FindLastIndex((Predicate<GameScreen>) (s => s.ScreenType == GameScreen.Type.Fullscreen))); index < gameScreens.Count; ++index)
          gameScreens[index].Render();
        if (inputManager.GetMouseListener().CursorBehavior == MouseDevice.Behavior.Free)
          RenderMouseCursor();
        base.Draw(gameTime);
        GraphicsDevice.SetVertexBuffer(null);
        GraphicsDevice.Indices = null;
      }
      catch (Exception ex)
      {
        Console.WriteErrorLine("Fatal error during rendering phase: " + ex.StackTrace);
        try
        {
          if (LoadedWorld != null)
            LoadedWorld.Save();
          int num = (int)MessageBox.Show("Fatal error during rendering phase. Game world saved!");
        }
        catch
        {
          int num = (int)MessageBox.Show("Fatal error during rendering phase. Failed to save game world :(");
        }
        Exit();
      }
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
      base.OnExiting(sender, args);
      Engine.settingsManager.SaveSettings(this.GetFilePath("settings.xml"));
      Engine.storedvalues.SaveToFile(this.GetFilePath("values.xml"));
      Engine.Console.SaveToFile(this.logPath);
    }

    public static void ConfirmBoxShow(string message, Action<bool> result)
    {
      Engine.LoadNewScreen((GameScreen) new ConfirmScreen(result, message));
    }

    public static World CreateNewClientWorld(
      Itemset itemset,
      SpriteTextureAtlas spriteTextureAtlas,
      BlockTextureAtlas blockTextureAtlas)
    {
      try
      {
        return itemset == null || spriteTextureAtlas == null || blockTextureAtlas == null ? (World) null : (World) new ClientWorld("Client", DateTime.Now, itemset, spriteTextureAtlas, blockTextureAtlas);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a world for client: " + ex.Message);
      }
      return (World) null;
    }

    public static World CreateNewLocalWorld(
      string name,
      int seed,
      string itemsetKey,
      string databasePath,
      DateTime creationTime)
    {
      try
      {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(itemsetKey) || string.IsNullOrEmpty(databasePath))
          return (World) null;
        Itemset itemset = Engine.loadedItemsets.Find((Predicate<Itemset>) (i => i.Key == itemsetKey));
        if (itemset == null)
          throw new Exception("Itemset with key \"" + itemsetKey + "\" cannot be found");
        BlockTextureAtlas matchingTextureAtlas1 = Engine.GetMatchingTextureAtlas<BlockTextureAtlas>(Engine.loadedBlockTextureAtlases, itemset.BlockTextureKey);
        SpriteTextureAtlas matchingTextureAtlas2 = Engine.GetMatchingTextureAtlas<SpriteTextureAtlas>(Engine.loadedSpriteTextureAtlases, itemset.SpriteTextureKey);
        return matchingTextureAtlas1 == null || matchingTextureAtlas2 == null ? (World) null : (World) new LocalWorld(name, seed, itemsetKey, databasePath, creationTime, itemset, matchingTextureAtlas1, matchingTextureAtlas2);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a new world: " + ex.Message);
        return (World) null;
      }
    }

    public static World CreateNewLocalWorld(WorldInfo worldinfo)
    {
      return Engine.CreateNewLocalWorld(worldinfo.Name, worldinfo.Seed, worldinfo.ItemsetKey, worldinfo.Path, worldinfo.TimesStamp);
    }

    public static BlockTextureAtlas GetBlockTextureAtlas(string key)
    {
      return Engine.GetMatchingTextureAtlas<BlockTextureAtlas>(Engine.loadedBlockTextureAtlases, key);
    }

    public static SpriteTextureAtlas GetSpriteTextureAtlas(string key)
    {
      return Engine.GetMatchingTextureAtlas<SpriteTextureAtlas>(Engine.loadedSpriteTextureAtlases, key);
    }

    public static string EvaluateFileHash(string path)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        using (MD5 md5 = MD5.Create())
        {
          using (FileStream inputStream = new FileStream(path, FileMode.Open))
          {
            foreach (byte num in md5.ComputeHash((Stream) inputStream))
              stringBuilder.Append(num.ToString("x2"));
          }
        }
        return stringBuilder.ToString();
      }
      catch
      {
        return string.Empty;
      }
    }

    public static void ExitGame() => Engine.exitRequested = true;

    public static void ExitToMainMenu()
    {
      foreach (GameScreen gameScreen in Engine.gameScreens)
        gameScreen.CloseScreen();
      Engine.LoadNewScreen((GameScreen) new MainMenuScreen());
    }

    public static void LoadNewScreen(GameScreen screen) => Engine.screensToAdd.Enqueue(screen);

    public static Texture2D LoadTexture(string path, bool generateMipmaps)
    {
      //StreamReader streamReader = (StreamReader) null;
      try
      {
        //streamReader = new StreamReader(path);
        //return !generateMipmaps ? Texture2D.FromFile(Engine.GraphicsDevice, streamReader.BaseStream) : Texture2D.FromFile(Engine.GraphicsDevice, streamReader.BaseStream, TextureCreationParameters.Default);
        return !generateMipmaps ? Texture2D.FromFile(Engine.GraphicsDevice, path) : Texture2D.FromFile(Engine.GraphicsDevice, path);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load a texture: " + ex.Message);
        return (Texture2D) null;
      }
      //finally
      //{
      //  streamReader?.Close();
      //}
    }

    public static void MessageBoxShow(string message)
    {
      Engine.LoadNewScreen((GameScreen) new MessageScreen(message, "Attention", true));
    }

    public static bool PlayWorld(World world)
    {
      if (Engine.loadedWorld != null)
        Engine.loadedWorld.Dispose();
      if (world == null)
        return false;
      BlockSpriteTextureAtlas spriteTextureAtlas1 = (BlockSpriteTextureAtlas) null;
      try
      {
        BlockTextureAtlas blockTextureAtlas = world.BlockTextureAtlas;
        SpriteTextureAtlas spriteTextureAtlas2 = world.SpriteTextureAtlas;
        if (blockTextureAtlas != null)
        {
          Engine.GraphicsDevice.Textures[1] = (Texture) blockTextureAtlas.Texture;
          Engine.GraphicsDevice.Textures[2] = (Texture) blockTextureAtlas.LightValuesTexture;
        }
        if (spriteTextureAtlas2 != null)
          Engine.GraphicsDevice.Textures[7] = (Texture) spriteTextureAtlas2.Texture;
        spriteTextureAtlas1 = new BlockSpriteTextureAtlas(world.Itemset.BlockTypes, blockTextureAtlas);
        world.BlockSpriteTextureAtlas = spriteTextureAtlas1;
        world.SpaceModel = new SpaceModel();
        Engine.loadedWorld = world;
        if (Engine.WorldLoadedEvent != null)
          Engine.WorldLoadedEvent(world);
        return true;
      }
      catch
      {
        spriteTextureAtlas1?.Dispose();
        return false;
      }
    }
    public static void SetPointSamplerStateForSpritebatch()
    {
      Engine.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
    }

    public static Console Console => Engine.console;

    public static bool DebugMode => Engine.debugMode;

    public static bool FastTravel => Engine.fastTravel;

    public static FrameCounter FrameCounter => Engine.frameCounter;

    public static List<Itemset> LoadedItemsets => Engine.loadedItemsets;

    public static World LoadedWorld => Engine.loadedWorld;

    public static SettingsManager SettingsManager => Engine.settingsManager;

    public static StoredValues StoredValues => Engine.storedvalues;

    public static TimedEventManager TimedEvents => Engine.timedEventsManager;

    private void GraphicsDeviceReseted(object sender, EventArgs args)
    {
      Engine.Console.WriteErrorLine("GraphicsDevice reseted! Attempting to recover.");
      Engine.PlayWorld(Engine.LoadedWorld);
    }

    public static event Action<World> WorldLoadedEvent;

    private void AddNewScreen(GameScreen screen)
    {
      try
      {
        screen.Initialize((IInput) this.inputManager);
        if (Engine.gameScreens.Count > 0)
        {
          MouseDevice.Behavior mouseBehavior = screen.MouseBehavior;
          this.inputManager.GetMouseListener().CursorBehavior = mouseBehavior;
        }
        screen.Load();
        Engine.gameScreens.Add(screen);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load a new screen: " + ex.Message);
        try
        {
          screen.Dispose();
        }
        catch
        {
        }
      }
    }

    private void AddNewScreens(Queue<GameScreen> newScreens)
    {
      while (newScreens.Count > 0)
        this.AddNewScreen(newScreens.Dequeue());
    }

    private void CreateFolderStructure(string path)
    {
      if (Directory.Exists(path))
        return;
      Directory.CreateDirectory(path);
      Directory.CreateDirectory(Path.Combine(path, "Worlds"));
    }

    private void DeleteScreen(GameScreen screen)
    {
      try
      {
        foreach (GameScreen gameScreen in Engine.gameScreens)
        {
          if (gameScreen == screen)
            gameScreen.Dispose();
        }
        Engine.gameScreens.Remove(screen);
        this.SetMouseBehavior();
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to dispose a deleted screen: " + ex.Message);
      }
    }

    private void DeleteScreens(List<GameScreen> screensToDelete)
    {
      foreach (GameScreen screen in screensToDelete)
        this.DeleteScreen(screen);
      screensToDelete.Clear();
    }

    private void GammaChanged(int newGamma)
    {
      newGamma = Math.Max(0, Math.Min(newGamma, 100));
      float num = (float) newGamma / 100f;
      try
      {
        foreach (Effect effect in Engine.ShaderPool)
          effect?.Parameters["GammaValue"]?.SetValue(num);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to set gamma value to shaders: " + ex.Message);
      }
    }

    private static T GetMatchingTextureAtlas<T>(List<T> atlases, string key) where T : TextureAtlas
    {
      return !string.IsNullOrEmpty(key) && atlases != null ? atlases.Find((Predicate<T>) (a => a.Key == key)) : default (T);
    }

    private bool InitializeShaders()
    {
      try
      {
        Engine.ShaderPool = new Effect[20];
        Engine.ShaderPool[0] = Engine.ContentManager.Load<Effect>("Shaders/BlockShader");
        Engine.ShaderPool[1] = Engine.ContentManager.Load<Effect>("Shaders/DynamicBlockShader");
        Engine.ShaderPool[2] = Engine.ContentManager.Load<Effect>("Shaders/selectorEffect");
        Engine.ShaderPool[3] = Engine.ContentManager.Load<Effect>("Shaders/ToolBlockShader");
        Engine.ShaderPool[4] = Engine.ContentManager.Load<Effect>("Shaders/ToolControlGroupShader");
        Engine.ShaderPool[5] = Engine.ContentManager.Load<Effect>("Shaders/ProjectileShader");
        Engine.ShaderPool[6] = Engine.ContentManager.Load<Effect>("Shaders/BillboardShader");
        Engine.ShaderPool[7] = Engine.ContentManager.Load<Effect>("Shaders/ThrusterShader");
        Engine.ShaderPool[8] = Engine.ContentManager.Load<Effect>("Shaders/ItemShader");
        Engine.ShaderPool[9] = Engine.ContentManager.Load<Effect>("Shaders/RotateBlockShader");
        Engine.ShaderPool[10] = Engine.ContentManager.Load<Effect>("Shaders/EntityLineShader");
        Engine.ShaderPool[11] = Engine.ContentManager.Load<Effect>("Shaders/RadarShader");
        Engine.ShaderPool[12] = Engine.ContentManager.Load<Effect>("Shaders/BlockShaderReducedQuality");
        Engine.ShaderPool[13] = Engine.ContentManager.Load<Effect>("Shaders/SpaceModelShader");
        Engine.ShaderPool[14] = Engine.ContentManager.Load<Effect>("Shaders/DeferredCombineShader");
        Engine.ShaderPool[15] = Engine.ContentManager.Load<Effect>("Shaders/ssaoShader");
        Engine.ShaderPool[16] = Engine.ContentManager.Load<Effect>("Shaders/DeferredSSAOCombineShader");
        Engine.WorldLoadedEvent += new Action<World>(Engine.UpdateShadersVariables);
        Engine.UpdateHalfPixelToShaders(Engine.SettingsManager.Resolution);
        Engine.SettingsManager.ResolutionChangedEvent += new Action<Point>(Engine.UpdateHalfPixelToShaders);
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to initialize shaders: " + ex.Message);
        return false;
      }
    }

    private bool InitializeVertexDeclarations()
    {
      try
      {
      Engine.VertexDeclarationPool = new VertexDeclaration[20];
      Engine.VertexDeclarationPool[0] = new VertexDeclaration(BlockVertex.VertexDeclaration.GetVertexElements());
      Engine.VertexDeclarationPool[1] = new VertexDeclaration(VertexPositionColor.VertexDeclaration.GetVertexElements());
      Engine.VertexDeclarationPool[2] = new VertexDeclaration(VertexPositionTexture.VertexDeclaration.GetVertexElements());
      Engine.VertexDeclarationPool[3] = new VertexDeclaration(VertexPositionColorTexture.VertexDeclaration.GetVertexElements());
      Engine.VertexDeclarationPool[4] = new VertexDeclaration(DynamicBlockVertex.VertexDeclaration.GetVertexElements());
      Engine.VertexDeclarationPool[5] = new VertexDeclaration(ProjectileVertex.VertexDeclaration.GetVertexElements());
      Engine.VertexDeclarationPool[6] = new VertexDeclaration(BillboardVertex.VertexDeclaration.GetVertexElements());
      Engine.VertexDeclarationPool[7] = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());
      return true;
      }
      catch (Exception ex)
      {
      Engine.Console.WriteErrorLine("Failed to initialize vertex declarations: " + ex.Message);
      return false;
      }
    }

    private void InitializeTextureRegisters()
    {
      SamplerState samplerState1 = Engine.GraphicsDevice.SamplerStates[1];
      samplerState1.AddressU = TextureAddressMode.Clamp;
      samplerState1.AddressV = TextureAddressMode.Clamp;
      samplerState1.Filter = TextureFilter.Point;
      SamplerState samplerState2 = Engine.GraphicsDevice.SamplerStates[7];
      samplerState2.AddressU = TextureAddressMode.Clamp;
      samplerState2.AddressV = TextureAddressMode.Clamp;
      samplerState2.Filter = TextureFilter.Point;
    }

    private bool SufficientPcCapabilities(GraphicsDevice gc)
    {
      //int simultaneousRenderTargets = gc.GraphicsCapabilities.MaxSimultaneousRenderTargets;
      //GraphicsProfile graphicsProfile = gc.GraphicsProfile;
      //return simultaneousRenderTargets >= 3 && graphicsProfile >= GraphicsProfile.HiDef;
      // TODO: Temp patch always returns true
      return true;
    }

    private void LoadBlocksets(string directory)
    {
      Engine.loadedItemsets.Clear();
      try
      {
        foreach (string file in Directory.GetFiles(directory, "*.xml"))
        {
          XDocument blocksetDoc = XDocument.Load(file);
          Itemset newBlockset = new Itemset();
          if (newBlockset.ParseFromXml(blocksetDoc) && Engine.loadedItemsets.Find((Predicate<Itemset>) (b => b.Key == newBlockset.Key)) == null)
            Engine.loadedItemsets.Add(newBlockset);
        }
      }
      catch
      {
      }
    }

    private void LoadBlockTextureAtlases(string directory)
    {
      Engine.loadedBlockTextureAtlases = this.LoadTextureAtlases<BlockTextureAtlas>(directory);
    }

    private void LoadSpriteTextureAtlases(string directory)
    {
      Engine.loadedSpriteTextureAtlases = this.LoadTextureAtlases<SpriteTextureAtlas>(directory);
    }

    private List<T> LoadTextureAtlases<T>(string directory) where T : TextureAtlas, new()
    {
      List<T> objList = new List<T>();
      try
      {
        foreach (string file in Directory.GetFiles(directory, "*.xml"))
        {
          foreach (XElement element in XDocument.Load(file).Root.Elements((XName) "atlas"))
          {
            T obj = new T();
            if (obj.ParseFromXml(element))
              objList.Add(obj);
          }
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load any texture atlases: " + ex.Message);
      }
      return objList;
    }

    private StoredValues LoadStoredValues()
    {
      StoredValues storedValues = new StoredValues();
      string filePath = this.GetFilePath("values.xml");
      try
      {
        if (!File.Exists(filePath))
        {
          XDocument document = new XDocument();
          document.Add((object) new XElement((XName) "values"));
          storedValues.LoadFromXml(document);
        }
        else
        {
          XDocument document = XDocument.Load(filePath);
          storedValues.LoadFromXml(document);
        }
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to initialize file for stored values: " + ex.Message);
      }
      return storedValues;
    }

    private XDocument PrepareSettingsFile()
    {
      try
      {
        string filePath = this.GetFilePath("settings.xml");
        return File.Exists(filePath) ? XDocument.Load(filePath) : SettingsManager.CreateDefaultSettingsDoc();
      }
      catch
      {
        return SettingsManager.CreateDefaultSettingsDoc();
      }
    }

    private SettingsManager PrepareSettings(GraphicsDeviceManager graphics)
    {
      XDocument document = this.PrepareSettingsFile();
      if (document == null)
        return (SettingsManager) null;
      SettingsManager settingsManager = new SettingsManager(graphics, this.userDataFolder);
      settingsManager.ParseFromXml(document);
      return settingsManager;
    }

    private string PrepareUserDataFolder()
    {
      try
      {
        string path = this.UserFilePath();
        if (!Directory.Exists(path))
          this.CreateFolderStructure(path);
        return path;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a folder for save game data: " + ex.Message);
        return (string) null;
      }
    }

    private void RenderMouseCursor()
    {
      Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
      MouseDevice mouseListener = this.inputManager.GetMouseListener();
      Rectangle destinationRectangle = new Rectangle(mouseListener.Position.X, mouseListener.Position.Y, this.mouseCursor.Width * 2, this.mouseCursor.Height * 2);
      Engine.SpriteBatch.Draw(this.mouseCursor, destinationRectangle, Color.White);
      Engine.SpriteBatch.End();
    }

    private void SaveSettings()
    {
    }

    private void SetMouseBehavior()
    {
      if (Engine.gameScreens.Count <= 0)
        return;
      MouseDevice.Behavior mouseBehavior = Engine.gameScreens[Engine.gameScreens.Count - 1].MouseBehavior;
      if (mouseBehavior == MouseDevice.Behavior.Wrapped)
        Mouse.SetPosition(Engine.SettingsManager.cMinSupportedResolution.X / 2, Engine.SettingsManager.cMinSupportedResolution.Y / 2);
      this.inputManager.GetMouseListener().CursorBehavior = mouseBehavior;
    }

    private string GetFilePath(string fileName) => Path.Combine(this.UserFilePath(), fileName);

    private static void UpdateShadersVariables(World world)
    {
      if (world == null)
        return;
      try
      {
        float textureUnitSize = world.BlockTextureAtlas.TextureUnitSize;
        foreach (Effect effect in Engine.ShaderPool)
          effect?.Parameters["TextureUnitSize"]?.SetValue(textureUnitSize);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to update texture unit size to shaders: " + ex.Message);
      }
    }

    private static void UpdateHalfPixelToShaders(Point resolution)
    {
      try
      {
        Vector2 vector2 = new Vector2(0.5f / (float) resolution.X, 0.5f / (float) resolution.Y);
        foreach (Effect effect in Engine.ShaderPool)
          effect?.Parameters["HalfPixel"]?.SetValue(vector2);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to update texture unit size to shaders: " + ex.Message);
      }
    }

    private string UserFilePath()
    {
      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Corneroids");
    }

    public enum LaunchParameters
    {
      Normal,
      Debug,
      FastTravel,
    }

    public enum ShaderPoolEnum
    {
      Block_shader,
      Dynamic_block_shader,
      Pick_block_shader,
      Tool_block_shader,
      Bind_Tool_shader,
      Projectile_Shader,
      Billboard_shader,
      Thruster_shader,
      Item_shader,
      Rotate_block_shader,
      Entity_line_shader,
      Radar_shader,
      Block_shader_reduced_quality,
      SpaceModel_shader,
      DeferredCombine_shader,
      SSAO_shader,
      SSAO_combine_shader,
    }

    public enum ShaderTextureRegisters
    {
      Block_texture_atlas = 1,
      light_texture_atlas = 2,
      Color_rendertarget = 3,
      Normal_rendertarget = 4,
      Depth_rendertarget = 5,
      Light_rendertarget = 6,
      Sprite_texture_atlas = 7,
    }

    public enum VDpoolEnum
    {
      BlockVertex,
      VertexPositionColor,
      VertexPositionTexture,
      VertexPositionColorTexture,
      DynamicBlockVertex,
      ProjectileVertex,
      BillboardVertex,
      VertexPositionNormalTexture,
    }
  }
}
