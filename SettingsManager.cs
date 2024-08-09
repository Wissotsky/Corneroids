// Decompiled with JetBrains decompiler
// Type: CornerSpace.SettingsManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class SettingsManager : IDisposable
  {
    public const float cNearPlaneDistance = 0.1f;
    public const float cFarPlaneDistance = 800f;
    public readonly Point cMinSupportedResolution = new Point(800, 700);
    private Color backgroundColor;
    private SettingsManager.ChangedVariables changedVariables;
    private ControlsManager controlsManager;
    private bool fixedCameraWhenAboard;
    private GraphicsDeviceManager graphicsDeviceManager;
    private XDocument settingsXml;
    private float cameraAspectRatio;
    private float cameraFoV;
    private bool drawTargetSquares;
    private int gamma;
    private bool lighting;
    private bool showHelpTexts;
    private SettingsManager.Quality ssao;
    private bool viewBobbing;
    private int viewDistance;
    private readonly string userDataFolder;

    public SettingsManager(GraphicsDeviceManager gManager, string userDataFolder)
    {
      if (gManager == null)
        throw new ArgumentNullException();
      try
      {
        this.graphicsDeviceManager = gManager;
        this.backgroundColor = Color.Black;
        this.controlsManager = new ControlsManager();
        this.cameraFoV = 1.57079637f;
        this.cameraAspectRatio = 1.77777779f;
        this.changedVariables = SettingsManager.ChangedVariables.None;
        this.viewDistance = 50;
        this.viewBobbing = true;
        this.userDataFolder = userDataFolder;
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to create new settings manager: " + ex.Message);
      }
    }

    public void ApplyChanges()
    {
      try
      {
        this.graphicsDeviceManager.ApplyChanges();
        this.UpdateCameraVariables();
        this.LaunchEvents(this.changedVariables);
        this.changedVariables = SettingsManager.ChangedVariables.None;
      }
      catch (Exception ex)
      {
        throw new Exception("Cannot apply changes. Some of the given values are not valid: " + ex.Message);
      }
    }

    public static XDocument CreateDefaultSettingsDoc()
    {
      return new XDocument(new XDeclaration("1.0", (string) null, "true"), new object[1]
      {
        (object) new XElement((XName) "settings", new object[3]
        {
          (object) new XElement((XName) "controls", new object[2]
          {
            (object) new XElement((XName) "sensitivity"),
            (object) new XElement((XName) "specialKeys", new object[15]
            {
              (object) new XElement((XName) "edit"),
              (object) new XElement((XName) "use"),
              (object) new XElement((XName) "forward"),
              (object) new XElement((XName) "backward"),
              (object) new XElement((XName) "left"),
              (object) new XElement((XName) "right"),
              (object) new XElement((XName) "flashlight"),
              (object) new XElement((XName) "inventory"),
              (object) new XElement((XName) "up"),
              (object) new XElement((XName) "down"),
              (object) new XElement((XName) "boost"),
              (object) new XElement((XName) "orientate"),
              (object) new XElement((XName) "beaconTool"),
              (object) new XElement((XName) "rollLeft"),
              (object) new XElement((XName) "rollRight")
            })
          }),
          (object) new XElement((XName) "gameplay", new object[5]
          {
            (object) new XElement((XName) "drawTargets"),
            (object) new XElement((XName) "fixedCamera"),
            (object) new XElement((XName) "gamma"),
            (object) new XElement((XName) "showHelp"),
            (object) new XElement((XName) "viewBobbing")
          }),
          (object) new XElement((XName) "graphics", new object[7]
          {
            (object) new XElement((XName) "fov"),
            (object) new XElement((XName) "fullscreen"),
            (object) new XElement((XName) "lighting"),
            (object) new XElement((XName) "resolution", new object[2]
            {
              (object) new XElement((XName) "w"),
              (object) new XElement((XName) "h")
            }),
            (object) new XElement((XName) "viewDistance"),
            (object) new XElement((XName) "ssao"),
            (object) new XElement((XName) "vsync")
          })
        })
      });
    }

    public void Dispose()
    {
    }

    public void FinalizeFrame()
    {
    }

    public bool FixedCameraWhenAboard => this.fixedCameraWhenAboard;

    public List<DisplayMode> GetSupportedResolutions()
    {
      List<DisplayMode> supportedResolutions = new List<DisplayMode>();
      try
      {
        foreach (DisplayMode supportedDisplayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
        {
          if (supportedDisplayMode.Width >= this.cMinSupportedResolution.X && supportedDisplayMode.Height >= this.cMinSupportedResolution.Y)
            supportedResolutions.Add(supportedDisplayMode);
        }
      }
      catch
      {
      }
      return supportedResolutions;
    }

    public void InitializeFrame()
    {
      Engine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, this.backgroundColor, 1f, 0);
    }

    public void ParseFromXml(XDocument document)
    {
      if (document == null)
        return;
      try
      {
        XmlReader instance = XmlReader.Instance;
        XElement root1 = document.Root;
        XElement root2 = document.Root.Element((XName) "gameplay");
        document.Root.Element((XName) "graphics").Element((XName) "resolution");
        KeyValuePair<int, int> keyValuePair = this.ValidateResolution(instance.ReadElementValue<int>(root1, new XmlReader.ConvertValue<int>(instance.ReadInt), 1024, "graphics", "resolution", "w"), instance.ReadElementValue<int>(root1, new XmlReader.ConvertValue<int>(instance.ReadInt), 768, "graphics", "resolution", "h"));
        this.SetResolution(keyValuePair.Key, keyValuePair.Value);
        this.SetFieldOfView((float) ((double) instance.ReadElementValue<int>(root1, new XmlReader.ConvertValue<int>(instance.ReadInt), 90, "graphics", "fow") * 3.1415927410125732 / 180.0));
        this.SetFullScreen(instance.ReadElementValue<byte>(root1, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 0, "graphics", "fullscreen") == (byte) 1);
        this.SetVSync(instance.ReadElementValue<byte>(root1, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 1, "graphics", "vsync") == (byte) 1);
        this.SetViewDistance(instance.ReadElementValue<int>(root1, new XmlReader.ConvertValue<int>(instance.ReadInt), 100, "graphics", "viewDistance"));
        this.SSAO = (SettingsManager.Quality) Math.Min(2, Math.Max(0, instance.ReadElementValue<int>(root1, new XmlReader.ConvertValue<int>(instance.ReadInt), 2, "graphics", "ssao")));
        this.Lighting = instance.ReadElementValue<int>(root1, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "graphics", "lighting") == 1;
        this.controlsManager.ParseFromXML(document.Root.Element((XName) "controls"));
        this.drawTargetSquares = instance.ReadElementValue<int>(root2, new XmlReader.ConvertValue<int>(instance.ReadInt), 1, "drawTargets") == 1;
        this.viewBobbing = instance.ReadElementValue<int>(root2, new XmlReader.ConvertValue<int>(instance.ReadInt), 1, "viewBobbing") == 1;
        this.SetGamma(instance.ReadElementValue<int>(root2, new XmlReader.ConvertValue<int>(instance.ReadInt), 50, "gamma"));
        this.SetCameraFixedWhenAboard(instance.ReadElementValue<int>(root2, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "fixedCamera") == 1);
        this.ShowHelpTexts = instance.ReadElementValue<int>(root2, new XmlReader.ConvertValue<int>(instance.ReadInt), 1, "showHelp") == 1;
        this.settingsXml = document;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to parse settings data: " + ex.Message);
        this.SetDefaultSettings();
      }
    }

    public void SaveSettings(string path)
    {
      try
      {
        if (string.IsNullOrEmpty(path) || this.settingsXml == null)
          return;
        XElement root = this.settingsXml.Root;
        XElement xelement1 = root.Element((XName) "gameplay");
        XElement xelement2 = root.Element((XName) "graphics");
        XElement xelement3 = xelement2.Element((XName) "resolution");
        XElement xelement4 = xelement2.Element((XName) "viewDistance");
        xelement2.Element((XName) "vsync");
        xelement1.Element((XName) "drawTargets").Value = this.DrawTargetSquares ? "1" : "0";
        xelement1.Element((XName) "gamma").Value = this.Gamma.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement1.Element((XName) "viewBobbing").Value = this.viewBobbing ? "1" : "0";
        xelement1.Element((XName) "fixedCamera").Value = this.fixedCameraWhenAboard ? "1" : "0";
        xelement1.Element((XName) "showHelp").Value = this.ShowHelpTexts ? "1" : "0";
        xelement2.Element((XName) "fullscreen").Value = (this.Fullscreen ? 1 : 0).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "vsync").Value = (this.VSync ? 1 : 0).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "fov").Value = MathHelper.ToDegrees(this.cameraFoV).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "ssao").Value = ((int) this.ssao).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "lighting").Value = (this.Lighting ? 1 : 0).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement3.Element((XName) "w").Value = this.Resolution.X.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement3.Element((XName) "h").Value = this.Resolution.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement4.Value = this.viewDistance.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.controlsManager.SaveToXML(root.Element((XName) "controls"));
        this.settingsXml.Save(path);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to save settings file: " + ex.Message);
      }
    }

    public void SetCameraFixedWhenAboard(bool value)
    {
      this.fixedCameraWhenAboard = value;
      this.changedVariables |= SettingsManager.ChangedVariables.CameraStyle;
    }

    public void SetFieldOfView(float fovRadians)
    {
      this.cameraFoV = fovRadians;
      this.changedVariables |= SettingsManager.ChangedVariables.Fov;
    }

    public void SetFullScreen(bool fullscreen)
    {
      this.graphicsDeviceManager.IsFullScreen = fullscreen;
      this.changedVariables |= SettingsManager.ChangedVariables.FullScreenSwap;
    }

    public void SetGamma(int newValue)
    {
      this.gamma = Math.Max(0, Math.Min(newValue, 50));
      this.changedVariables |= SettingsManager.ChangedVariables.Gamma;
    }

    public void SetResolution(int width, int height)
    {
      this.graphicsDeviceManager.PreferredBackBufferWidth = width;
      this.graphicsDeviceManager.PreferredBackBufferHeight = height;
      this.changedVariables |= SettingsManager.ChangedVariables.Resolution;
    }

    public void SetViewDistance(int distance)
    {
      this.viewDistance = distance;
      this.changedVariables |= SettingsManager.ChangedVariables.ViewDistance;
    }

    public void SetVSync(bool value)
    {
      this.graphicsDeviceManager.SynchronizeWithVerticalRetrace = value;
      this.changedVariables |= SettingsManager.ChangedVariables.VSync;
    }

    public float AspectRatio => this.cameraAspectRatio;

    public float CameraFoV => this.cameraFoV;

    public ControlsManager ControlsManager => this.controlsManager;

    public int CurrentViewDistance => this.viewDistance;

    public bool DrawTargetSquares
    {
      get => this.drawTargetSquares;
      set => this.drawTargetSquares = value;
    }

    public bool Fullscreen => this.graphicsDeviceManager.IsFullScreen;

    public int Gamma => this.gamma;

    public bool Lighting
    {
      get => this.lighting;
      set => this.lighting = value;
    }

    public Point Resolution
    {
      get
      {
        return new Point(this.graphicsDeviceManager.PreferredBackBufferWidth, this.graphicsDeviceManager.PreferredBackBufferHeight);
      }
    }

    public bool ShowHelpTexts
    {
      get => this.showHelpTexts;
      set => this.showHelpTexts = value;
    }

    public SettingsManager.Quality SSAO
    {
      get => this.ssao;
      set
      {
        this.ssao = value;
        this.changedVariables |= SettingsManager.ChangedVariables.SSAO;
      }
    }

    public string UserDataFolder => this.userDataFolder;

    public bool ViewBobbing
    {
      get => this.viewBobbing;
      set => this.viewBobbing = value;
    }

    public bool VSync => this.graphicsDeviceManager.SynchronizeWithVerticalRetrace;

    public event System.Action CameraAttributesChanged;

    public event Action<bool> CameraStyleChanged;

    public event Action<bool> FullscreenSwitchEvent;

    public event Action<int> GammaChangedEvent;

    public event Action<Point> ResolutionChangedEvent;

    public event Action<SettingsManager.Quality> SSAOChangedEvent;

    private void LaunchEvents(SettingsManager.ChangedVariables variables)
    {
      if ((variables & SettingsManager.ChangedVariables.Resolution) > SettingsManager.ChangedVariables.None && this.ResolutionChangedEvent != null)
        this.ResolutionChangedEvent(this.Resolution);
      if ((variables & SettingsManager.ChangedVariables.FullScreenSwap) > SettingsManager.ChangedVariables.None && this.FullscreenSwitchEvent != null)
        this.FullscreenSwitchEvent(this.Fullscreen);
      if ((variables & (SettingsManager.ChangedVariables.Resolution | SettingsManager.ChangedVariables.ViewDistance | SettingsManager.ChangedVariables.Fov)) > SettingsManager.ChangedVariables.None && this.CameraAttributesChanged != null)
        this.CameraAttributesChanged();
      if ((variables & SettingsManager.ChangedVariables.Gamma) > SettingsManager.ChangedVariables.None && this.GammaChangedEvent != null)
        this.GammaChangedEvent(this.Gamma);
      if ((variables & SettingsManager.ChangedVariables.CameraStyle) > SettingsManager.ChangedVariables.None && this.CameraStyleChanged != null)
        this.CameraStyleChanged(this.fixedCameraWhenAboard);
      if ((variables & SettingsManager.ChangedVariables.SSAO) <= SettingsManager.ChangedVariables.None || this.SSAOChangedEvent == null)
        return;
      this.SSAOChangedEvent(this.ssao);
    }

    private void SetDefaultSettings()
    {
    }

    private void UpdateCameraVariables()
    {
      this.cameraAspectRatio = (float) this.graphicsDeviceManager.PreferredBackBufferWidth / (float) this.graphicsDeviceManager.PreferredBackBufferHeight;
    }

    private KeyValuePair<int, int> ValidateResolution(int width, int height)
    {
      foreach (DisplayMode supportedResolution in this.GetSupportedResolutions())
      {
        if (supportedResolution.Width == width && supportedResolution.Height == height)
          return new KeyValuePair<int, int>(width, height);
      }
      return new KeyValuePair<int, int>(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
    }

    private enum ChangedVariables
    {
      None = 0,
      Resolution = 1,
      FullScreenSwap = 2,
      ViewDistance = 4,
      VSync = 8,
      Fov = 16, // 0x00000010
      Gamma = 32, // 0x00000020
      CameraStyle = 64, // 0x00000040
      SSAO = 128, // 0x00000080
    }

    public enum Quality
    {
      Off,
      Low,
      High,
    }
  }
}
