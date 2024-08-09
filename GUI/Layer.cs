// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.Layer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.GUI
{
  public abstract class Layer
  {
    protected List<Layer> layers;
    private Rectangle positionAndSize;
    private string name;
    private object tag;

    public Layer(Rectangle positionAndSize)
    {
      this.layers = new List<Layer>();
      this.positionAndSize = positionAndSize;
      this.positionAndSize.Width = Math.Max(this.positionAndSize.Width, 1);
      this.positionAndSize.Height = Math.Max(this.positionAndSize.Height, 1);
    }

    public virtual bool Contains(Point location) => this.positionAndSize.Contains(location);

    public static Rectangle EvaluateMiddlePosition(int width, int height)
    {
      Point point = new Point(Engine.GraphicsDevice.PresentationParameters.BackBufferWidth, Engine.GraphicsDevice.PresentationParameters.BackBufferHeight);
      return new Rectangle(point.X / 2 - width / 2, point.Y / 2 - height / 2, width, height);
    }

    public Layer GetClickedLayer(Point mousePosition, params System.Type[] acceptLayers)
    {
      if (this.positionAndSize.Contains(mousePosition))
      {
        foreach (Layer layer in this.layers)
        {
          Layer clickedLayer = layer.GetClickedLayer(mousePosition, acceptLayers);
          if (clickedLayer != null)
            return clickedLayer;
        }
        if (acceptLayers.Length == 0 || Array.Find<System.Type>(acceptLayers, (Predicate<System.Type>) (t => t == this.GetType())) != null)
          return this;
      }
      return (Layer) null;
    }

    public Layer GetLayer(string name)
    {
      foreach (Layer layer in this.layers)
      {
        if (layer.Name == name)
          return layer;
      }
      return (Layer) null;
    }

    public abstract void Render();

    public virtual Point Location
    {
      get => new Point(this.positionAndSize.X, this.positionAndSize.Y);
      set
      {
        Point point = new Point(value.X - this.Location.X, value.Y - this.Location.Y);
        this.positionAndSize = new Rectangle(value.X, value.Y, this.positionAndSize.Width, this.positionAndSize.Height);
        foreach (Layer layer in this.layers)
          layer.Location = new Point(layer.Location.X + point.X, layer.Location.Y + point.Y);
      }
    }

    public virtual Rectangle PositionAndSize
    {
      get => this.positionAndSize;
      set => this.positionAndSize = value;
    }

    public virtual Vector2 Position
    {
      get => new Vector2((float) this.positionAndSize.X, (float) this.positionAndSize.Y);
      set
      {
        this.positionAndSize = new Rectangle((int) value.X, (int) value.Y, this.positionAndSize.Width, this.positionAndSize.Height);
      }
    }

    public virtual Vector2 Size
    {
      get => new Vector2((float) this.positionAndSize.Width, (float) this.positionAndSize.Height);
      set
      {
        this.positionAndSize = new Rectangle(this.positionAndSize.X, this.positionAndSize.Y, (int) value.X, (int) value.Y);
      }
    }

    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    public object Tag
    {
      get => this.tag;
      set => this.tag = value;
    }

    private bool PointInLayer(Point point)
    {
      return point.X >= 0 && point.Y >= 0 && point.X < this.positionAndSize.Width && point.Y < this.positionAndSize.Height;
    }
  }
}
