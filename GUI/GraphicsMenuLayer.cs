// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.GraphicsMenuLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.GUI
{
  public class GraphicsMenuLayer : CaptionWindowLayer
  {
    private const int windowWidth = 600;
    private const int windowheight = 500;
    private const string sResolution = "Resolution:";
    private ComboboxLayer<GraphicsMenuLayer.SupportedResolution> resolutionTextbox;

    public GraphicsMenuLayer()
      : base(Layer.EvaluateMiddlePosition(600, 500), "Graphics")
    {
      this.layers.Add((Layer) new ButtonLayer(new Rectangle(this.Location.X + (int) this.Size.X / 2 - 110, this.Location.Y + (int) this.Size.Y - 50, 100, 30), "Ok"));
      this.layers.Add((Layer) new ButtonLayer(new Rectangle(this.Location.X + (int) this.Size.X / 2 + 10, this.Location.Y + (int) this.Size.Y - 50, 100, 30), "Cancel"));
      this.resolutionTextbox = new ComboboxLayer<GraphicsMenuLayer.SupportedResolution>(new Rectangle(this.Location.X + 175, this.Location.Y + 40, 400, 30), 30U);
      this.PopulateResolutionCombobox(this.resolutionTextbox);
      this.layers.Add((Layer) this.resolutionTextbox);
    }

    public override void Render()
    {
      base.Render();
      foreach (Layer layer in this.layers)
        layer.Render();
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      spriteBatch.DrawString(Engine.Font, "Resolution:", this.Position + new Vector2(30f, 40f), Color.White);
      spriteBatch.End();
    }

    private void PopulateResolutionCombobox(
      ComboboxLayer<GraphicsMenuLayer.SupportedResolution> combobox)
    {
      if (combobox == null)
        return;
      List<DisplayMode> supportedResolutions = Engine.SettingsManager.GetSupportedResolutions();
      List<GraphicsMenuLayer.SupportedResolution> supportedResolutionList = new List<GraphicsMenuLayer.SupportedResolution>();
      foreach (DisplayMode displayMode in supportedResolutions)
      {
        GraphicsMenuLayer.SupportedResolution supportedResolution1 = new GraphicsMenuLayer.SupportedResolution()
        {
          Width = displayMode.Width,
          Height = displayMode.Height
        };
        bool flag = false;
        foreach (GraphicsMenuLayer.SupportedResolution supportedResolution2 in supportedResolutionList)
          flag = ((flag ? 1 : 0) | (supportedResolution2.Width != supportedResolution1.Width ? 0 : (supportedResolution2.Height == supportedResolution1.Height ? 1 : 0))) != 0;
        if (!flag)
          supportedResolutionList.Add(supportedResolution1);
      }
      if (supportedResolutionList.Count > 2)
        supportedResolutionList.Sort((Comparison<GraphicsMenuLayer.SupportedResolution>) ((r1, r2) => r1.Width * r1.Height - r2.Width * r2.Height));
      combobox.ItemsToChoose = supportedResolutionList;
    }

    public class SupportedResolution
    {
      public int Width;
      public int Height;

      public override string ToString() => this.Width.ToString() + " x " + this.Height.ToString();
    }
  }
}
