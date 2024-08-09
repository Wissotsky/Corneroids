// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.RotateBlockScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Screen
{
  public class RotateBlockScreen : MenuScreen
  {
    private Block block;
    private RotateBlockLayer blockLayer;
    private Vector3 blockPosition;
    private Matrix? mouseOnButton;
    private SpaceEntity ownerEntity;
    private Action<Vector3int> resultRotation;
    private Effect rotationEffect;
    private Texture2D rotationTexture;
    private Astronaut user;
    private VertexDeclaration vertexDeclaration;
    private short[] rotationIndices;
    private VertexPositionTexture[] rotationVertices;

    public RotateBlockScreen(
      Astronaut user,
      Block block,
      Vector3 worldPosition,
      SpaceEntity ownerEntity,
      Action<Vector3int> resultRotation)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.block = block;
      this.resultRotation = resultRotation;
      this.blockPosition = worldPosition;
      this.ownerEntity = ownerEntity;
      this.user = user;
      this.rotationEffect = Engine.ShaderPool[9];
      this.rotationTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/rotation");
      this.vertexDeclaration = Engine.VertexDeclarationPool[2];
      if (this.rotationEffect != null && this.rotationTexture != null)
        this.rotationEffect.Parameters["ArrowTexture"].SetValue((Texture) this.rotationTexture);
      this.InitializeArrowStructure();
    }

    public override void Load() => this.blockLayer = new RotateBlockLayer();

    public override void Render()
    {
      if (this.block != null && this.user != null && this.mouseOnButton.HasValue && this.rotationEffect != null)
      {
        this.rotationEffect.Parameters["World"].SetValue(this.mouseOnButton.Value * (this.ownerEntity != null ? Matrix.CreateFromQuaternion(this.ownerEntity.Orientation) : Matrix.Identity) * Matrix.CreateTranslation(this.blockPosition));
        this.rotationEffect.Parameters["View"].SetValue(this.user.UsedCamera.ViewMatrix);
        this.rotationEffect.Parameters["Projection"].SetValue(this.user.UsedCamera.ProjectionMatrix);
        Engine.GraphicsDevice.VertexDeclaration = this.vertexDeclaration;
        Engine.GraphicsDevice.RenderState.CullMode = CullMode.None;
        Engine.GraphicsDevice.RenderState.AlphaBlendEnable = true;
        Engine.GraphicsDevice.RenderState.DepthBufferEnable = true;
        this.rotationEffect.Begin();
        this.rotationEffect.CurrentTechnique.Passes[0].Begin();
        Engine.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, this.rotationVertices, 0, this.rotationVertices.Length, this.rotationIndices, 0, this.rotationIndices.Length / 3);
        this.rotationEffect.CurrentTechnique.Passes[0].End();
        this.rotationEffect.End();
      }
      this.blockLayer.Render();
    }

    public override void Update()
    {
      Layer clickedLayer = this.blockLayer.GetClickedLayer(this.Mouse.Position);
      this.mouseOnButton = new Matrix?();
      if (clickedLayer == null)
        return;
      if (clickedLayer.Name == "yaw")
        this.mouseOnButton = new Matrix?(Matrix.Identity);
      else if (clickedLayer.Name == "pitch")
      {
        this.mouseOnButton = new Matrix?(Matrix.CreateRotationZ(-1.57079637f));
      }
      else
      {
        if (!(clickedLayer.Name == "roll"))
          return;
        this.mouseOnButton = new Matrix?(Matrix.CreateRotationX(1.57079637f));
      }
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      if (this.Mouse.LeftClick())
      {
        Layer clickedLayer = this.blockLayer.GetClickedLayer(this.Mouse.Position);
        if (clickedLayer != null)
        {
          Vector3int vector3int = new Vector3int(0, 0, 0);
          if (clickedLayer.Name == "pitch")
            vector3int = new Vector3int(1, 0, 0);
          else if (clickedLayer.Name == "yaw")
            vector3int = new Vector3int(0, 1, 0);
          else if (clickedLayer.Name == "roll")
            vector3int = new Vector3int(0, 0, 1);
          Vector3 vector3 = -1f * this.user.UsedCamera.GetLookatVector();
          if (this.ownerEntity != null)
            vector3 = Vector3.TransformNormal(vector3, this.ownerEntity.InverseTransformMatrix);
          this.block.Rotate(vector3);
          if (this.resultRotation != null)
            this.resultRotation(vector3int);
        }
      }
      if (!this.Mouse.MiddleClick() && !this.Mouse.RightClick())
        return;
      this.CloseScreen();
    }

    private void InitializeArrowStructure()
    {
      VertexPositionTexture[] vertexPositionTextureArray = new VertexPositionTexture[4];
      List<short> shortList = new List<short>();
      shortList.Add((short) 0);
      shortList.Add((short) 1);
      shortList.Add((short) 2);
      shortList.Add((short) 2);
      shortList.Add((short) 3);
      shortList.Add((short) 0);
      vertexPositionTextureArray[0] = new VertexPositionTexture(new Vector3(-1f, 0.0f, -1f), Vector2.Zero);
      vertexPositionTextureArray[1] = new VertexPositionTexture(new Vector3(1f, 0.0f, -1f), Vector2.UnitX);
      vertexPositionTextureArray[2] = new VertexPositionTexture(new Vector3(1f, 0.0f, 1f), Vector2.One);
      vertexPositionTextureArray[3] = new VertexPositionTexture(new Vector3(-1f, 0.0f, 1f), Vector2.UnitY);
      this.rotationIndices = shortList.ToArray();
      this.rotationVertices = vertexPositionTextureArray;
    }
  }
}
