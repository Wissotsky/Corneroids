// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceModel
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class SpaceModel : IDisposable
  {
    private const int cStarCount = 1024;
    private Effect effect;
    private IndexBuffer modelIB;
    private VertexBuffer modelVB;
    private int numberOfIndices;
    private int numberOfVertices;
    private VertexDeclaration vertexDeclaration;

    public SpaceModel()
    {
      this.effect = Engine.ShaderPool[13];
      this.vertexDeclaration = Engine.VertexDeclarationPool[1];
      this.CreateSpaceModel(1024);
    }

    public void Dispose()
    {
      if (this.modelVB != null)
        this.modelVB.Dispose();
      if (this.modelIB == null)
        return;
      this.modelIB.Dispose();
    }

    public void Render(IRenderCamera camera)
    {
      if (camera == null || this.modelVB == null || this.modelIB == null || this.effect == null || this.vertexDeclaration == null)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      Engine.GraphicsDevice.RenderState.DepthBufferEnable = false;
      Engine.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
      Engine.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
      Engine.GraphicsDevice.RenderState.StencilEnable = true;
      Engine.GraphicsDevice.RenderState.StencilFunction = CompareFunction.Equal;
      Engine.GraphicsDevice.RenderState.StencilPass = StencilOperation.Keep;
      Engine.GraphicsDevice.RenderState.ReferenceStencil = 0;
      graphicsDevice.VertexDeclaration = this.vertexDeclaration;
      graphicsDevice.Vertices[0].SetSource(this.modelVB, 0, VertexPositionColor.SizeInBytes);
      graphicsDevice.Indices = this.modelIB;
      this.effect.Parameters["World"].SetValue(Matrix.Identity);
      this.effect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      this.effect.Begin();
      this.effect.CurrentTechnique.Passes[0].Begin();
      graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.numberOfVertices, 0, this.numberOfIndices / 3);
      this.effect.CurrentTechnique.Passes[0].End();
      this.effect.End();
      Engine.GraphicsDevice.RenderState.StencilEnable = false;
    }

    private Vector3 ChooseRandomSpherePoint(Random rng)
    {
      if (rng == null)
        return Vector3.Zero;
      float num1 = 1f;
      float num2;
      for (num2 = 1f; (double) num1 * (double) num1 + (double) num2 * (double) num2 >= 1.0; num2 = (float) (rng.NextDouble() - 0.5) * 2f)
        num1 = (float) (rng.NextDouble() - 0.5) * 2f;
      return Vector3.Normalize(new Vector3(2f * num1 * (float) Math.Sqrt(1.0 - (double) num1 * (double) num1 - (double) num2 * (double) num2), 2f * num2 * (float) Math.Sqrt(1.0 - (double) num1 * (double) num1 - (double) num2 * (double) num2), (float) (1.0 - 2.0 * ((double) num1 * (double) num1 + (double) num2 * (double) num2))));
    }

    private void CreateSpaceModel(int starCount)
    {
      List<VertexPositionColor> vertexPositionColorList = new List<VertexPositionColor>();
      List<short> shortList = new List<short>();
      Random rng = new Random();
      VertexPositionColor[] vertices;
      short[] indices;
      Primitive.CreateUnitBox(out vertices, out indices, Color.Black);
      vertexPositionColorList.AddRange((IEnumerable<VertexPositionColor>) vertices);
      shortList.AddRange((IEnumerable<short>) indices);
      for (int index = 0; index < starCount; ++index)
      {
        Vector3 vector2 = this.ChooseRandomSpherePoint(rng);
        Matrix identity = Matrix.Identity;
        Matrix matrix1 = !(vector2 == Vector3.Up) ? (!(vector2 == Vector3.Down) ? Matrix.CreateFromAxisAngle(Vector3.Normalize(Vector3.Cross(-Vector3.UnitZ, vector2)), (float) Math.Acos((double) Vector3.Dot(-Vector3.UnitZ, vector2))) : Matrix.CreateRotationZ(4.712389f)) : Matrix.CreateRotationZ(1.57079637f);
        byte num1 = (byte) (50 + rng.Next(0, 150));
        float num2 = (float) (rng.NextDouble() * 0.699999988079071 + 0.30000001192092896);
        Color color = new Color(num1, num1, num1, byte.MaxValue);
        Matrix matrix2 = Matrix.CreateScale(num2, num2, 1f) * Matrix.CreateTranslation(0.0f, 0.0f, -150f) * Matrix.CreateRotationZ((float) rng.NextDouble() * 6.28318548f) * matrix1;
        int count1 = vertexPositionColorList.Count;
        int count2 = shortList.Count;
        vertexPositionColorList.Add(new VertexPositionColor(Vector3.Transform(new Vector3(-0.5f, 0.5f, -1f), matrix2), color));
        vertexPositionColorList.Add(new VertexPositionColor(Vector3.Transform(new Vector3(0.5f, 0.5f, -1f), matrix2), color));
        vertexPositionColorList.Add(new VertexPositionColor(Vector3.Transform(new Vector3(0.5f, -0.5f, -1f), matrix2), color));
        vertexPositionColorList.Add(new VertexPositionColor(Vector3.Transform(new Vector3(-0.5f, -0.5f, -1f), matrix2), color));
        shortList.Add((short) count1);
        shortList.Add((short) (count1 + 1));
        shortList.Add((short) (count1 + 2));
        shortList.Add((short) (count1 + 2));
        shortList.Add((short) (count1 + 3));
        shortList.Add((short) count1);
      }
      if (vertexPositionColorList.Count <= 0)
        return;
      if (shortList.Count <= 0)
        return;
      try
      {
        this.modelVB = new VertexBuffer(Engine.GraphicsDevice, VertexPositionColor.SizeInBytes * vertexPositionColorList.Count, BufferUsage.WriteOnly);
        this.modelIB = new IndexBuffer(Engine.GraphicsDevice, 2 * shortList.Count, BufferUsage.WriteOnly, IndexElementSize.SixteenBits);
        this.modelVB.SetData<VertexPositionColor>(vertexPositionColorList.ToArray());
        this.modelIB.SetData<short>(shortList.ToArray());
        this.numberOfVertices = vertexPositionColorList.Count;
        this.numberOfIndices = shortList.Count;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to create a space dome: " + ex.Message);
        if (this.modelIB != null)
        {
          this.modelIB.Dispose();
          this.modelIB = (IndexBuffer) null;
        }
        if (this.modelVB == null)
          return;
        this.modelVB.Dispose();
        this.modelVB = (VertexBuffer) null;
      }
    }
  }
}
