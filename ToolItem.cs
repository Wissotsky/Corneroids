// Decompiled with JetBrains decompiler
// Type: CornerSpace.ToolItem
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
  public abstract class ToolItem : LightingItem
  {
    protected BlockCell pickedBlockCell;
    protected SpaceEntity pickedEntity;
    protected Vector3? pickedCellPosition;
    protected Vector3? pickedBlockMiddlePosition;
    protected Position3? pickedWorldPosition;
    private Effect effect;
    private bool useSelectionBox;
    private VertexDeclaration vertexDeclaration;
    private SelectorBox<VertexPositionColorTexture> selectorBox;

    public ToolItem()
    {
      this.effect = Engine.ShaderPool[2];
      this.vertexDeclaration = Engine.VertexDeclarationPool[3];
      this.selectorBox = new SelectorBox<VertexPositionColorTexture>()
      {
        Color = Color.White
      };
      this.ItemId = -1;
      this.effect.Parameters["SelectorTexture"].SetValue((Texture) Engine.ContentManager.Load<Texture2D>("Textures/Sprites/squareCorners"));
    }

    public virtual void Close()
    {
    }

    public override void Update(Astronaut owner)
    {
      base.Update(owner);
      this.PickBlock((ICameraInterface) owner);
      if (this.pickedBlockCell == null || !((Vector3) this.pickedBlockCell.Block.Size != this.selectorBox.Size))
        return;
      this.selectorBox.Block = this.pickedBlockCell.Block;
      this.selectorBox.Size = (Vector3) this.pickedBlockCell.Block.Size;
      VertexPositionColorTexture[] vertices = this.selectorBox.Vertices;
      short[] indices = this.selectorBox.Indices;
      Primitive.CreateSelectorBox((Vector3) this.pickedBlockCell.Block.Size, out vertices, out indices);
      this.selectorBox.Vertices = vertices;
      this.selectorBox.Indices = indices;
    }

    public override void RenderFirstPerson(IRenderCamera camera)
    {
      base.RenderFirstPerson(camera);
      if (!this.useSelectionBox)
        return;
      this.RenderSelectionBox(camera);
    }

    protected bool UseSelectionBox
    {
      set => this.useSelectionBox = value;
    }

    public Color SelectorColor
    {
      set
      {
        if (this.selectorBox == null)
          return;
        this.selectorBox.Color = value;
      }
    }

    private void PickBlock(ICameraInterface camera)
    {
      Vector3 lookatVector = camera.GetLookatVector();
      float num = 0.0f;
      this.pickedBlockCell = (BlockCell) null;
      this.pickedWorldPosition = new Position3?();
      this.pickedCellPosition = new Vector3?();
      this.pickedEntity = (SpaceEntity) null;
      Position3 zero = Position3.Zero;
      for (; (double) num < 3.0; num += 0.1f)
      {
        Position3 position = camera.Position + lookatVector * num;
        foreach (SpaceEntity closeEntity in Item.entityManager.GetCloseEntities(position, 5f))
        {
          BlockCell blockCell = closeEntity.GetBlockCell(position);
          this.pickedEntity = closeEntity;
          if (blockCell != null)
          {
            this.pickedBlockCell = blockCell;
            Byte3 size = this.pickedBlockCell.Block.Size;
            Vector3 entityCoords = closeEntity.WorldCoordsToEntityCoords(position);
            Vector3 vector3 = Vector3.One * 0.5f + new Vector3((float) Math.Floor((double) entityCoords.X), (float) Math.Floor((double) entityCoords.Y), (float) Math.Floor((double) entityCoords.Z));
            this.pickedBlockMiddlePosition = new Vector3?(vector3 - new Vector3((float) this.pickedBlockCell.X, (float) this.pickedBlockCell.Y, (float) this.pickedBlockCell.Z) - Vector3.One * 0.5f + (Vector3) size * 0.5f);
            this.pickedCellPosition = new Vector3?(vector3);
            this.pickedWorldPosition = new Position3?(position);
            this.pickedEntity = closeEntity;
            return;
          }
        }
      }
    }

    private void RenderSelectionBox(IRenderCamera camera)
    {
      if (this.pickedBlockCell == null)
        return;
      VertexPositionColorTexture[] vertices = this.selectorBox.Vertices;
      short[] indices = this.selectorBox.Indices;
      if (vertices == null && indices == null || !this.pickedBlockMiddlePosition.HasValue || this.pickedBlockCell == null)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      this.effect.Parameters["World"].SetValue(Matrix.CreateScale(1.01f) * Matrix.CreateTranslation(this.pickedBlockMiddlePosition.Value) * this.pickedEntity.TransformMatrix * Matrix.CreateTranslation(camera.GetPositionRelativeToCamera(this.pickedEntity.Position)));
      this.effect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      this.effect.Parameters["SelectorColor"].SetValue(this.selectorBox.Color.ToVector4());
      Engine.GraphicsDevice.VertexDeclaration = this.vertexDeclaration;
      graphicsDevice.RenderState.CullMode = CullMode.None;
      graphicsDevice.RenderState.DepthBufferEnable = true;
      graphicsDevice.RenderState.DepthBufferWriteEnable = false;
      graphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      graphicsDevice.RenderState.AlphaBlendEnable = false;
      this.effect.Begin();
      foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
      {
        pass.Begin();
        graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
        pass.End();
      }
      this.effect.End();
    }

    public enum SpecialToolIds
    {
      BindCameraToControl = -3, // 0xFFFFFFFD
      BindGunToControl = -2, // 0xFFFFFFFE
      BindTriggerToConsole = -1, // 0xFFFFFFFF
    }
  }
}
