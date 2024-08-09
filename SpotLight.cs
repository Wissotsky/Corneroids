// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpotLight
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace
{
  public class SpotLight : LightSource
  {
    private static Effect effect;
    private static ModelStructure<VertexPositionColor> model;
    private Vector3 direction;
    private float length;

    public SpotLight(Color color, float length, float radius, Vector3 direction)
      : base(color)
    {
      this.length = length;
      this.direction = direction == Vector3.Zero ? Vector3.Up : Vector3.Normalize(direction);
      this.Radius = radius;
      if (SpotLight.model == null)
      {
        VertexPositionColor[] vertices;
        short[] indices;
        Primitive.CreateUnitCone(9, out vertices, out indices);
        SpotLight.model = new ModelStructure<VertexPositionColor>(vertices, indices);
      }
      if (SpotLight.effect != null)
        return;
      SpotLight.effect = Engine.ContentManager.Load<Effect>("Shaders\\LightShaders\\SpotLightShader");
    }

    public override void ApplyShaderVariables()
    {
      SpotLight.effect.Parameters["Direction"].SetValue(this.direction);
      SpotLight.effect.Parameters["Color"].SetValue(this.Color.ToVector4());
      SpotLight.effect.Parameters["Radius"].SetValue(this.length);
      SpotLight.effect.Parameters["ConeCosine"].SetValue(this.length / (float) Math.Sqrt((double) this.length * (double) this.length + 0.25 * (double) this.Radius * (double) this.Radius));
    }

    public float Length
    {
      get => this.length;
      set => this.length = value;
    }

    public Vector3 Direction
    {
      get => this.direction;
      set
      {
        if (value == Vector3.Zero)
          this.direction = Vector3.Up;
        else
          this.direction = Vector3.Normalize(value);
      }
    }

    public override Effect Effect => SpotLight.effect;

    public override ModelStructure<VertexPositionColor> Model => SpotLight.model;

    public override Matrix TransformMatrix
    {
      get
      {
        Vector3 vector3 = Vector3.Cross(Vector3.Up, this.direction);
        Matrix matrix = Matrix.Identity;
        if (vector3 != Vector3.Zero)
          matrix = Matrix.CreateFromAxisAngle(Vector3.Normalize(vector3), (float) Math.Acos((double) Vector3.Dot(this.direction, Vector3.Up)));
        return Matrix.CreateScale(this.Radius, this.length, this.Radius) * matrix;
      }
    }
  }
}
