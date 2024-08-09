// Decompiled with JetBrains decompiler
// Type: CornerSpace.LightSource
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public abstract class LightSource
  {
    private bool enabled;
    private Color color;
    private Position3 position;
    private Vector3 positionInEntitySpace;
    private float radius;

    public LightSource(Color color)
    {
      this.enabled = true;
      this.color = color;
      this.position = Position3.Zero;
      this.radius = 0.0f;
    }

    public abstract void ApplyShaderVariables();

    public virtual LightSource Copy() => this.MemberwiseClone() as LightSource;

    public Color Color
    {
      get => this.color;
      set => this.color = value;
    }

    public abstract Effect Effect { get; }

    public bool Enabled
    {
      get => this.enabled;
      set => this.enabled = value;
    }

    public abstract ModelStructure<VertexPositionColor> Model { get; }

    public Position3 Position
    {
      get => this.position;
      set => this.position = value;
    }

    public Vector3 PositionInEntitySpace
    {
      get => this.positionInEntitySpace;
      set => this.positionInEntitySpace = value;
    }

    public float Radius
    {
      get => this.radius;
      set => this.radius = value;
    }

    public abstract Matrix TransformMatrix { get; }
  }
}
