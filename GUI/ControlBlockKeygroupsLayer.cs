// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ControlBlockKeygroupsLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.GUI
{
  public class ControlBlockKeygroupsLayer : CaptionWindowLayer
  {
    private const int windowWidth = 460;
    private const int windowHeight = 400;
    private List<ColorKeyGroupLayer> colorKeyLayers;
    private ControlBlock controlBlock;

    public ControlBlockKeygroupsLayer(ControlBlock controlBlock)
      : base(new Rectangle(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 230, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / 2 - 200, 460, 400), "Control block")
    {
      this.controlBlock = controlBlock;
      this.colorKeyLayers = new List<ColorKeyGroupLayer>();
      for (int index = 0; index < controlBlock.ColorKeyGroups.Count; ++index)
      {
        ColorKeyGroupLayer colorKeyGroupLayer = new ColorKeyGroupLayer(new Rectangle(35 + (int) ((double) index / 5.0) * 210 + this.Location.X, 60 + index % 5 * 50 + this.Location.Y, 150, 30), controlBlock.ColorKeyGroups[index].Color.ToString(), controlBlock.ColorKeyGroups[index].Color);
        colorKeyGroupLayer.Key = controlBlock.ColorKeyGroups[index].Key;
        this.colorKeyLayers.Add(colorKeyGroupLayer);
        this.layers.Add((Layer) colorKeyGroupLayer);
      }
    }

    public override void Render()
    {
      base.Render();
      foreach (Layer colorKeyLayer in this.colorKeyLayers)
        colorKeyLayer.Render();
    }

    public void UpdateLayerColors()
    {
      using (List<ColorKeyGroup>.Enumerator enumerator = this.controlBlock.ColorKeyGroups.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          ColorKeyGroup colorKey = enumerator.Current;
          ColorKeyGroupLayer colorKeyGroupLayer = this.colorKeyLayers.Find((Predicate<ColorKeyGroupLayer>) (l => l.Color == colorKey.Color));
          if (colorKeyGroupLayer != null)
            colorKeyGroupLayer.Key = colorKey.Key;
        }
      }
    }
  }
}
