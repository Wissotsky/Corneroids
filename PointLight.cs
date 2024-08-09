// Decompiled with JetBrains decompiler
// Type: CornerSpace.PointLight
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public class PointLight : LightSource
  {
    private static Effect effect;
    private static ModelStructure<VertexPositionColor> model;

    public PointLight(Color color, float radius)
      : base(color)
    {
      this.Radius = radius;
      if (PointLight.model == null)
      {
        VertexPositionColor[] vertices;
        short[] indices;
        Primitive.CreateUnitBox(out vertices, out indices, Color.White);
        PointLight.model = new ModelStructure<VertexPositionColor>(vertices, indices);
      }
      if (PointLight.effect != null)
        return;
      PointLight.effect = Engine.ContentManager.Load<Effect>("Shaders\\LightShaders\\PointLightShader");
    }

    public override void ApplyShaderVariables()
    {
      PointLight.effect.Parameters["Color"].SetValue(this.Color.ToVector4());
      PointLight.effect.Parameters["Radius"].SetValue(this.Radius);
    }

    public override Effect Effect => PointLight.effect;

    public override ModelStructure<VertexPositionColor> Model => PointLight.model;

    public override Matrix TransformMatrix => Matrix.CreateScale(this.Radius);
  }
}
