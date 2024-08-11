// Decompiled with JetBrains decompiler
// Type: CornerSpace.LightingManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class LightingManager : IDisposable, ILightingInterface
  {
    private const float cMaxLightRenderDistance = 100f;
    private bool lightingEnabled;
    public RenderTarget2D colorRenderTarget;
    public RenderTarget2D depthRenderTarget;
    public RenderTarget2D normalRenderTarget;
    public RenderTarget2D lightRenderTarget;
    public RenderTarget2D ssaoRenderTarget;
    private ModelStructure<VertexPositionTexture> screenModel;
    private bool beginLighting;
    private bool initializationSuccess;
    private Stack<ILightChunk> lightChunks;
    private HashSet<LightSource> lights;
    private SettingsManager.Quality ssao;
    private Texture2D ssaoRandomTexture;
    private VertexDeclaration vertexDeclarationPositionColor;
    private VertexDeclaration vertexDeclarationPositionTexture;
    private Effect combineShader;
    private Effect ssaoShader;
    private Effect ssaoCombineShader;

    public LightingManager()
    {
      this.lightChunks = new Stack<ILightChunk>();
      this.lightingEnabled = true;
      this.lights = new HashSet<LightSource>();
      this.initializationSuccess = false;
      this.ssao = SettingsManager.Quality.Off;
      this.beginLighting = false;
      this.vertexDeclarationPositionColor = Engine.VertexDeclarationPool[1];
      this.vertexDeclarationPositionTexture = Engine.VertexDeclarationPool[2];
      Engine.SettingsManager.SSAOChangedEvent += new Action<SettingsManager.Quality>(this.SSAOChanged);
      Engine.SettingsManager.CameraAttributesChanged += new System.Action(this.GraphicsChanged);
    }

    public bool DrawLight(LightSource light) => light != null && this.lights.Add(light);
    public bool Begin()
    {
      if (!this.initializationSuccess)
        throw new Exception("Cannot begin lighting phase: manager must be initialized first");
      if (this.beginLighting)
        throw new Exception("Begin() called twice in a row");
      try
      {
        this.ClearTexturesFromGPUregisters();
        Engine.GraphicsDevice.SetRenderTargets(this.colorRenderTarget, this.normalRenderTarget, this.depthRenderTarget);
        this.ClearRenderTargets();
        this.lightChunks.Clear();
        this.SetStencilBufferToWriteState();
        this.beginLighting = true;
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to begin lighting phase: " + ex.Message);
        return false;
      }
    }

    public void Dispose()
    {
      this.DisposeRendertargets();
      Engine.SettingsManager.CameraAttributesChanged -= new System.Action(this.GraphicsChanged);
      Engine.SettingsManager.SSAOChangedEvent -= new Action<SettingsManager.Quality>(this.SSAOChanged);
    }

    public void DrawLightChunk(ILightChunk chunk)
    {
      if (chunk.LightSources.Count <= 0)
        return;
      this.lightChunks.Push(chunk);
    }

    public void End(IRenderCamera camera)
    {
      if (!this.beginLighting)
        throw new Exception("Begin() must be called before End()");
      try
      {
        this.DisableStencilBufferWriting();
        Engine.GraphicsDevice.SetRenderTargets(this.lightRenderTarget,null,null);
        Engine.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1f, 0);
        this.SetGBufferTexturesToGPUregisters();
        if (Engine.SettingsManager.Lighting)
        {
          this.RenderLights(camera);
          this.RenderLightChunks(camera);
        }
        if (this.ssao == SettingsManager.Quality.Off)
        {
          Engine.GraphicsDevice.SetRenderTarget(null);
          Engine.GraphicsDevice.Textures[6] = (Texture) this.lightRenderTarget;
          this.RenderResultToScene();
        }
        else if (this.ssaoRenderTarget != null)
        {
          Engine.GraphicsDevice.SetRenderTarget(this.ssaoRenderTarget);
          Engine.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1f, 0);
          Engine.GraphicsDevice.Textures[6] = (Texture) this.lightRenderTarget;
          this.RenderSSAO(camera);
          Engine.GraphicsDevice.SetRenderTarget(null);
          this.RenderResultWithSSAOToScene();
        }
        this.beginLighting = false;
        this.lights.Clear();
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to end the lighting phase: " + ex.Message);
      }
    }

    public bool Initialize()
    {
      try
      {
        this.DisposeRendertargets();
        int backBufferWidth = Engine.GraphicsDevice.PresentationParameters.BackBufferWidth;
        int backBufferHeight = Engine.GraphicsDevice.PresentationParameters.BackBufferHeight;
        this.lightRenderTarget = new RenderTarget2D(Engine.GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
        this.colorRenderTarget = new RenderTarget2D(Engine.GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
        this.depthRenderTarget = new RenderTarget2D(Engine.GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Single, DepthFormat.None);
        this.normalRenderTarget = new RenderTarget2D(Engine.GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
        this.ssaoRandomTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/normalVectors");
        this.combineShader = Engine.ShaderPool[14];
        this.ssaoShader = Engine.ShaderPool[15];
        this.ssaoCombineShader = Engine.ShaderPool[16];
        this.ssaoShader.Parameters["RandomTexture"].SetValue((Texture2D)this.ssaoRandomTexture);
        this.screenModel = new ModelStructure<VertexPositionTexture>(new VertexPositionTexture[4], new short[6]);
        this.screenModel.Vertices[0] = new VertexPositionTexture(new Vector3(-1f, 1f, 0.0f), new Vector2(0.0f, 0.0f));
        this.screenModel.Vertices[1] = new VertexPositionTexture(new Vector3(1f, 1f, 0.0f), new Vector2(1f, 0.0f));
        this.screenModel.Vertices[2] = new VertexPositionTexture(new Vector3(1f, -1f, 0.0f), new Vector2(1f, 1f));
        this.screenModel.Vertices[3] = new VertexPositionTexture(new Vector3(-1f, -1f, 0.0f), new Vector2(0.0f, 1f));
        this.screenModel.Indices[0] = 0;
        this.screenModel.Indices[1] = 1;
        this.screenModel.Indices[2] = 2;
        this.screenModel.Indices[3] = 2;
        this.screenModel.Indices[4] = 3;
        this.screenModel.Indices[5] = 0;
        this.initializationSuccess = true;
        return true;
      }
      catch (Exception ex)
      {
        this.initializationSuccess = false;
        this.Dispose();
        Engine.Console.WriteErrorLine("Failed to initialize lighting manager: " + ex.Message);
        return false;
      }
    }

    public bool RemoveLight(LightSource light) => false;

    public bool Enabled
    {
      get => this.lightingEnabled;
      set => this.lightingEnabled = value;
    }

    public SettingsManager.Quality ScreenSpaceAmbientOcclusion
    {
      get => this.ssao;
      set
      {
        this.ssao = value;
        if (value == SettingsManager.Quality.Off)
          return;
        int num = 4 >> (int) (value - 1 & (SettingsManager.Quality) 31);
        int width = Engine.GraphicsDevice.PresentationParameters.BackBufferWidth / num;
        int height = Engine.GraphicsDevice.PresentationParameters.BackBufferHeight / num;
        if (this.ssaoRenderTarget != null)
        {
          if (this.ssaoRenderTarget.Width == width && this.ssaoRenderTarget.Height == height)
            return;
          this.ssaoRenderTarget.Dispose();
        }
        try
        {
            this.ssaoRenderTarget = new RenderTarget2D(Engine.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.DiscardContents);
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to enable SSAO: " + ex.Message);
          this.ssao = SettingsManager.Quality.Off;
        }
      }
    }

    private void ClearRenderTargets()
    {
      Engine.GraphicsDevice.Clear(ClearOptions.Target, new Color((byte) 0, (byte) 0, (byte) 0, (byte) 1), 0.0f, 0);
    }

    private void ClearTexturesFromGPUregisters()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      graphicsDevice.Textures[3] = (Texture) null;
      graphicsDevice.Textures[5] = (Texture) null;
      graphicsDevice.Textures[4] = (Texture) null;
      graphicsDevice.Textures[6] = (Texture) null;
    }

    private void DisableStencilBufferWriting()
    {
        GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
        if (graphicsDevice == null)
            return;

        DepthStencilState depthStencilState = new DepthStencilState
        {
            StencilEnable = false,
            StencilFunction = CompareFunction.Never,
            StencilPass = StencilOperation.Replace,
            ReferenceStencil = 0
        };

        graphicsDevice.DepthStencilState = depthStencilState;
    }

        private void DisposeRendertargets()
    {
      if (this.colorRenderTarget != null && !this.colorRenderTarget.IsDisposed)
        this.colorRenderTarget.Dispose();
      if (this.depthRenderTarget != null && !this.depthRenderTarget.IsDisposed)
        this.depthRenderTarget.Dispose();
      if (this.normalRenderTarget != null && !this.normalRenderTarget.IsDisposed)
        this.normalRenderTarget.Dispose();
      if (this.lightRenderTarget == null || this.lightRenderTarget.IsDisposed)
        return;
      this.lightRenderTarget.Dispose();
    }

    private void PrepareStencilBufferForSSAO()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      graphicsDevice.DepthStencilState = new DepthStencilState
      {
        StencilEnable = true,
        StencilFunction = CompareFunction.Equal,
        StencilPass = StencilOperation.Keep,
        ReferenceStencil = 1
      };
    }

    private void RenderBillboardLights(IRenderCamera camera)
    {
    }

    private void RenderLightChunks(IRenderCamera camera)
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      Vector2 vector2 = new Vector2(0.5f / (float)graphicsDevice.PresentationParameters.BackBufferWidth, 0.5f / (float)graphicsDevice.PresentationParameters.BackBufferHeight);
      //graphicsDevice.VertexDeclaration = this.vertexDeclarationPositionColor;
      this.SetRenderstatesForLightPhase();
      Matrix matrix1 = Matrix.Invert(camera.ViewMatrix * camera.ProjectionMatrix);
      while (this.lightChunks.Count > 0)
      {
        ILightChunk lightChunk = this.lightChunks.Pop();
        if (lightChunk.LightSources.Count != 0)
        {
          foreach (LightSource lightSource in lightChunk.LightSources)
          {
            if (lightSource.Enabled)
            {
              Effect effect = lightSource.Effect;
              Matrix transformMatrix = lightSource.TransformMatrix;
              Position3 position = Vector3.Transform(lightSource.PositionInEntitySpace, lightChunk.Owner.TransformMatrix) + lightChunk.Owner.Position;
              if ((double)camera.GetPositionRelativeToCamera(position).LengthSquared() <= 10000.0)
              {
                Matrix matrix2 = transformMatrix * Matrix.CreateTranslation(camera.GetPositionRelativeToCamera(position));
                effect.Parameters["HalfPixel"].SetValue(vector2);
                effect.Parameters["LightWorld"].SetValue(Matrix.Identity);
                effect.Parameters["Position"].SetValue(camera.GetPositionRelativeToCamera(position));
                effect.Parameters["World"].SetValue(matrix2);
                effect.Parameters["View"].SetValue(camera.ViewMatrix);
                effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                effect.Parameters["InverseTransform"].SetValue(matrix1);
                lightSource.ApplyShaderVariables();
                effect.CurrentTechnique.Passes[0].Apply();
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, lightSource.Model.Vertices, 0, lightSource.Model.Vertices.Length, lightSource.Model.Indices, 0, lightSource.Model.Indices.Length / 3);
              }
            }
          }
        }
      }
    }

    private void RenderLights(IRenderCamera camera)
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      Vector2 vector2 = new Vector2(0.5f / (float)graphicsDevice.PresentationParameters.BackBufferWidth, 0.5f / (float)graphicsDevice.PresentationParameters.BackBufferHeight);
      //graphicsDevice.VertexDeclaration = this.vertexDeclarationPositionColor;
      this.SetRenderstatesForLightPhase();
      Matrix matrix1 = Matrix.Invert(camera.ViewMatrix * camera.ProjectionMatrix);
      foreach (LightSource light in this.lights)
      {
        if (light.Enabled && (double)camera.GetPositionRelativeToCamera(light.Position).LengthSquared() <= 10000.0)
        {
          Effect effect = light.Effect;
          Matrix matrix2 = light.TransformMatrix * Matrix.CreateTranslation(camera.GetPositionRelativeToCamera(light.Position));
          effect.Parameters["HalfPixel"].SetValue(vector2);
          effect.Parameters["LightWorld"].SetValue(Matrix.Identity);
          effect.Parameters["Position"].SetValue(camera.GetPositionRelativeToCamera(light.Position));
          effect.Parameters["World"].SetValue(matrix2);
          effect.Parameters["View"].SetValue(camera.ViewMatrix);
          effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
          effect.Parameters["InverseTransform"].SetValue(matrix1);
          light.ApplyShaderVariables();
          effect.CurrentTechnique.Passes[0].Apply();
          graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, light.Model.Vertices, 0, light.Model.Vertices.Length, light.Model.Indices, 0, light.Model.Indices.Length / 3);
        }
      }
    }

    private void RenderResultToScene()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      //graphicsDevice.VertexDeclaration = this.vertexDeclarationPositionTexture;
      graphicsDevice.RasterizerState = RasterizerState.CullNone;
      graphicsDevice.DepthStencilState = DepthStencilState.None;
      graphicsDevice.BlendState = BlendState.Opaque;
      this.combineShader.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, this.screenModel.Vertices, 0, 4, this.screenModel.Indices, 0, 2);
    }

    private void RenderResultWithSSAOToScene()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      //graphicsDevice.VertexDeclaration = this.vertexDeclarationPositionTexture;
      this.ssaoCombineShader.Parameters["SSAOTexture"].SetValue((Texture2D) this.ssaoRenderTarget);
      graphicsDevice.RasterizerState = RasterizerState.CullNone;
      graphicsDevice.DepthStencilState = DepthStencilState.None;
      graphicsDevice.BlendState = BlendState.Opaque;
      this.ssaoCombineShader.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, this.screenModel.Vertices, 0, 4, this.screenModel.Indices, 0, 2);
    }

    private void RenderSSAO(IRenderCamera camera)
    {
      try
      {
      this.DisableStencilBufferWriting();
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      this.ssaoShader.Parameters["InverseTransform"].SetValue(Matrix.Invert(camera.ViewMatrix * camera.ProjectionMatrix));
      this.ssaoShader.Parameters["RandomTexture"].SetValue((Texture2D) this.ssaoRandomTexture);
      //graphicsDevice.VertexDeclaration = this.vertexDeclarationPositionTexture;
      graphicsDevice.RasterizerState = RasterizerState.CullNone;
      graphicsDevice.DepthStencilState = DepthStencilState.None;
      graphicsDevice.BlendState = BlendState.Opaque;
      this.ssaoShader.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, this.screenModel.Vertices, 0, 4, this.screenModel.Indices, 0, 2);
      this.DisableStencilBufferWriting();
      }
      catch (Exception ex)
      {
      Engine.Console.WriteErrorLine("SSAO failed! " + ex.Message);
      this.ScreenSpaceAmbientOcclusion = SettingsManager.Quality.Off;
      }
    }

    private void SetGBufferTexturesToGPUregisters()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      graphicsDevice.Textures[3] = (Texture) this.colorRenderTarget;
      graphicsDevice.Textures[5] = (Texture) this.depthRenderTarget;
      graphicsDevice.Textures[4] = (Texture) this.normalRenderTarget;
    }

    private void SetRenderstatesForLightPhase()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      graphicsDevice.DepthStencilState = DepthStencilState.Default;
      graphicsDevice.BlendState = BlendState.Additive;
      graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
    }

    private void SetStencilBufferToWriteState()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      if (graphicsDevice == null)
      return;
      graphicsDevice.DepthStencilState = new DepthStencilState()
      {
      StencilEnable = true,
      StencilFunction = CompareFunction.Always,
      StencilPass = StencilOperation.Replace,
      ReferenceStencil = 1
      };
    }

    private void GraphicsChanged() => this.Initialize();

    private void SSAOChanged(SettingsManager.Quality quality)
    {
      this.ScreenSpaceAmbientOcclusion = quality;
    }

    public enum SSAO
    {
      Off = 0,
      x8 = 8,
      x16 = 16, // 0x00000010
    }
  }
}
