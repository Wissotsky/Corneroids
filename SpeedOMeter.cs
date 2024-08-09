// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpeedOMeter
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
  public class SpeedOMeter
  {
    private VertexPositionColor[] arrowVertices;
    private short[] arrowIndices;
    private Camera camera;
    private Effect shader;
    private VertexDeclaration vertexDeclaration;

    public SpeedOMeter()
    {
      Primitive.CreateUnitCone(10, out this.arrowVertices, out this.arrowIndices);
      for (int index = 0; index < this.arrowVertices.Length; ++index)
        this.arrowVertices[index].Color = new Color((byte) 20, (byte) 110, (byte) 10, (byte) 125);
      this.camera = new Camera(1f, 1f, 0.1f, 100f);
      this.camera.Position = new Position3(new Vector3(0.0f, 1f, 1f));
      this.shader = Engine.ShaderPool[11];
      this.vertexDeclaration = Engine.VertexDeclarationPool[1];
    }

    public void Render(Rectangle screenPosition, Vector3 speed, Quaternion orientation)
    {
      Viewport viewport1 = new Viewport()
      {
        X = screenPosition.X,
        Y = screenPosition.Y,
        Width = screenPosition.Width,
        Height = screenPosition.Height,
        MinDepth = 0.0f,
        MaxDepth = 0.1f
      };
      this.camera.SetAttributes(viewport1.AspectRatio, 1.57079637f, 0.1f, 100f);
      this.camera.Update(Engine.FrameCounter.DeltaTime);
      float yScale = Math.Min(speed.Length(), 10f) * 0.25f;
      Matrix rotationX = Matrix.CreateRotationX(1.57079637f);
      if (speed != Vector3.Zero)
      {
        Vector3 vector2 = Vector3.Transform(Vector3.Normalize(speed), Quaternion.Conjugate(orientation));
        Vector3 axis = Vector3.Cross(Vector3.Forward, vector2);
        if (axis != Vector3.Zero)
        {
          axis.Normalize();
          float angle = (float) Math.Acos((double) Vector3.Dot(Vector3.Forward, vector2));
          rotationX *= Matrix.CreateFromAxisAngle(axis, angle);
        }
      }
      this.shader.Parameters["World"].SetValue(Matrix.CreateTranslation(0.0f, -0.5f, 0.0f) * Matrix.CreateScale(0.5f, yScale, 0.5f) * rotationX * Matrix.CreateTranslation(0.0f, 0.0f, -2f));
      this.shader.Parameters["View"].SetValue(this.camera.ViewMatrix);
      this.shader.Parameters["Projection"].SetValue(this.camera.ProjectionMatrix);
      Engine.GraphicsDevice.VertexDeclaration = this.vertexDeclaration;
      Engine.GraphicsDevice.RenderState.CullMode = CullMode.None;
      Engine.GraphicsDevice.RenderState.DepthBufferEnable = true;
      Engine.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
      Engine.GraphicsDevice.RenderState.AlphaBlendEnable = true;
      Viewport viewport2 = Engine.GraphicsDevice.Viewport;
      Engine.GraphicsDevice.Viewport = viewport1;
      this.shader.Begin();
      this.shader.CurrentTechnique.Passes[0].Begin();
      Engine.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.arrowVertices, 0, this.arrowVertices.Length, this.arrowIndices, 0, this.arrowIndices.Length / 3);
      this.shader.CurrentTechnique.Passes[0].End();
      this.shader.End();
      Engine.GraphicsDevice.Viewport = viewport2;
    }
  }
}
