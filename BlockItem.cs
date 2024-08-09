// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockItem
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
  public abstract class BlockItem : PlaceableItem
  {
    private Effect blockEffect;
    private Block blockToPlace;
    private Effect selectionEffect;
    private VertexDeclaration vertexDeclarationBlock;
    private VertexDeclaration vertexDeclarationSelector;
    private VertexPositionTexture[] blockVertices;
    private short[] blockIndices;
    private short[] selectionIndices;
    private VertexPositionColorTexture[] selectionVertices;
    private static Effect lineEffect;
    private static short[] lineIndices;
    private static VertexPositionColor[] lineVertices;
    private static VertexPositionColor[] arrowVertices;
    private static short[] arrowIndices;
    public static Vector3 iii;

    public BlockItem()
    {
      this.blockEffect = Engine.ShaderPool[3];
      this.selectionEffect = Engine.ShaderPool[2];
      this.vertexDeclarationBlock = Engine.VertexDeclarationPool[2];
      this.vertexDeclarationSelector = Engine.VertexDeclarationPool[3];
      this.InitializeLineStructure();
      this.InitializeArrowStructure();
    }

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      if (this.blockToPlace != null)
      {
        int num = input.LeftDown ? 1 : 0;
        if (input.LeftClick && Item.entityManager.AddBlock(this.blockToPlace, this.pickedWorldPosition.Value, owner) != null)
        {
          this.InitializeNewBlock(this.blockToPlace.Orientation);
          return Item.UsageResult.Consumed;
        }
        if (input.MiddleClick || input.KeyClicked(ControlsManager.SpecialKey.OrientateBlock))
        {
          Vector3 lookatVector = owner.Astronaut.UsedCamera.GetLookatVector();
          if (this.pickedEntity != null)
            this.blockToPlace.Rotate(this.pickedEntityNormal ?? -lookatVector);
          else
            this.blockToPlace.Rotate(-lookatVector);
          Primitive.CreateSelectorBox((Vector3) this.blockToPlace.Size, out this.selectionVertices, out this.selectionIndices);
          BlockBuilder.Factory.ConstructBlock(this.blockToPlace, Vector3.Zero, out this.blockVertices, out this.blockIndices);
          this.itemSize = (Vector3) this.blockToPlace.Size;
        }
      }
      return Item.UsageResult.None;
    }

    public abstract Block CreateBlock();

    public override void RenderFirstPerson(IRenderCamera camera)
    {
      this.RenderBlock(camera);
      this.RenderSelectionBox(camera);
      this.RenderEntityToBlockLine(camera);
      this.RenderUpArrow(camera);
    }

    public override void Update()
    {
      base.Update();
      if (this.blockToPlace != null)
        return;
      this.InitializeNewBlock((byte) 0);
    }

    private Matrix EvaluateLineMatrix(Vector3 blockPosition, Vector3 entityPosition)
    {
      float xScale = (blockPosition - entityPosition).Length();
      if ((double) xScale <= 0.0)
        return Matrix.Identity;
      Vector3 vector2 = Vector3.Normalize(entityPosition - blockPosition);
      Vector3 unitX = Vector3.UnitX;
      Vector3 zero = Vector3.Zero;
      if (vector2 != unitX)
      {
        Vector3 axis = Vector3.Cross(unitX, vector2);
        if (axis != Vector3.Zero)
        {
          axis.Normalize();
          float angle = (float) Math.Acos((double) Vector3.Dot(unitX, vector2));
          return Matrix.CreateScale(xScale, 0.1f, 0.1f) * Matrix.CreateFromAxisAngle(axis, angle) * Matrix.CreateTranslation(blockPosition);
        }
      }
      return Matrix.CreateScale(xScale, 0.1f, 0.1f) * Matrix.CreateTranslation(blockPosition);
    }

    private Vector3 EvaluateRenderPositionForBlock(IRenderCamera camera)
    {
      if (this.blockToPlace == null)
        return Vector3.Zero;
      Byte3 size = this.blockToPlace.Size;
      Vector3 zero = Vector3.Zero;
      Vector3 vector3 = this.pickedEntity != null ? this.pickedEntityPosition.Value : camera.GetPositionRelativeToCamera(this.pickedWorldPosition.Value);
      Vector3 position = (float) ((int) size.X % 2 - 1) * 0.5f * Vector3.UnitX + (float) ((int) size.Y % 2 - 1) * 0.5f * Vector3.UnitY + (float) ((int) size.Z % 2 - 1) * 0.5f * Vector3.UnitZ + vector3;
      BlockItem.iii = position;
      if (this.pickedEntity != null)
        position = camera.GetPositionRelativeToCamera(this.pickedEntity.EntityCoordsToWorldCoords(position));
      return position;
    }

    private void InitializeArrowStructure()
    {
      if (BlockItem.arrowIndices != null && BlockItem.arrowVertices != null)
        return;
      VertexPositionColor[] vertices;
      short[] indices;
      Primitive.CreateUnitCone(8, out vertices, out indices);
      Matrix matrix = Matrix.CreateScale(0.5f) * Matrix.CreateRotationZ(1.57079637f) * Matrix.CreateTranslation(1.5f, 0.0f, 0.0f);
      for (int index = 0; index < vertices.Length; ++index)
        vertices[index].Position = Vector3.Transform(vertices[index].Position, matrix);
      List<VertexPositionColor> collection1 = new List<VertexPositionColor>();
      List<short> collection2 = new List<short>();
      collection1.Add(new VertexPositionColor(new Vector3(0.0f, -0.1f, -0.1f), Color.Red));
      collection1.Add(new VertexPositionColor(new Vector3(0.0f, -0.1f, 0.1f), Color.Red));
      collection1.Add(new VertexPositionColor(new Vector3(0.0f, 0.1f, 0.1f), Color.Red));
      collection1.Add(new VertexPositionColor(new Vector3(0.0f, 0.1f, -0.1f), Color.Red));
      collection1.Add(new VertexPositionColor(new Vector3(1f, -0.1f, -0.1f), Color.Red));
      collection1.Add(new VertexPositionColor(new Vector3(1f, -0.1f, 0.1f), Color.Red));
      collection1.Add(new VertexPositionColor(new Vector3(1f, 0.1f, 0.1f), Color.Red));
      collection1.Add(new VertexPositionColor(new Vector3(1f, 0.1f, -0.1f), Color.Red));
      collection2.Add((short) 9);
      collection2.Add((short) 13);
      collection2.Add((short) 14);
      collection2.Add((short) 14);
      collection2.Add((short) 10);
      collection2.Add((short) 9);
      collection2.Add((short) 10);
      collection2.Add((short) 14);
      collection2.Add((short) 15);
      collection2.Add((short) 15);
      collection2.Add((short) 11);
      collection2.Add((short) 10);
      collection2.Add((short) 11);
      collection2.Add((short) 15);
      collection2.Add((short) 16);
      collection2.Add((short) 16);
      collection2.Add((short) 12);
      collection2.Add((short) 11);
      collection2.Add((short) 12);
      collection2.Add((short) 16);
      collection2.Add((short) 13);
      collection2.Add((short) 13);
      collection2.Add((short) 9);
      collection2.Add((short) 12);
      List<VertexPositionColor> vertexPositionColorList = new List<VertexPositionColor>();
      List<short> shortList = new List<short>();
      vertexPositionColorList.AddRange((IEnumerable<VertexPositionColor>) vertices);
      vertexPositionColorList.AddRange((IEnumerable<VertexPositionColor>) collection1);
      shortList.AddRange((IEnumerable<short>) indices);
      shortList.AddRange((IEnumerable<short>) collection2);
      BlockItem.arrowVertices = vertexPositionColorList.ToArray();
      BlockItem.arrowIndices = shortList.ToArray();
    }

    private void InitializeLineStructure()
    {
      if (BlockItem.lineEffect == null)
        BlockItem.lineEffect = Engine.ShaderPool[10];
      if (BlockItem.lineVertices != null && BlockItem.lineIndices != null)
        return;
      List<VertexPositionColor> vertexPositionColorList = new List<VertexPositionColor>();
      List<short> shortList = new List<short>();
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(0.0f, -0.5f, -0.5f), Color.Red));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(0.0f, -0.5f, 0.5f), Color.Red));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(0.0f, 0.5f, 0.5f), Color.Red));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(0.0f, 0.5f, -0.5f), Color.Red));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, -0.5f, -0.5f), Color.Red));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, -0.5f, 0.5f), Color.Red));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, 0.5f, 0.5f), Color.Red));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, 0.5f, -0.5f), Color.Red));
      shortList.Add((short) 0);
      shortList.Add((short) 4);
      shortList.Add((short) 5);
      shortList.Add((short) 5);
      shortList.Add((short) 1);
      shortList.Add((short) 0);
      shortList.Add((short) 1);
      shortList.Add((short) 5);
      shortList.Add((short) 6);
      shortList.Add((short) 6);
      shortList.Add((short) 2);
      shortList.Add((short) 1);
      shortList.Add((short) 2);
      shortList.Add((short) 6);
      shortList.Add((short) 7);
      shortList.Add((short) 7);
      shortList.Add((short) 3);
      shortList.Add((short) 2);
      shortList.Add((short) 3);
      shortList.Add((short) 7);
      shortList.Add((short) 4);
      shortList.Add((short) 4);
      shortList.Add((short) 0);
      shortList.Add((short) 3);
      BlockItem.lineVertices = vertexPositionColorList.ToArray();
      BlockItem.lineIndices = shortList.ToArray();
    }

    private void InitializeNewBlock(byte initialOrientation)
    {
      this.blockToPlace = this.CreateBlock();
      this.blockToPlace.Orientation = initialOrientation;
      this.SizeOfItem = (Vector3) this.blockToPlace.Size;
      Primitive.CreateSelectorBox((Vector3) this.blockToPlace.Size, out this.selectionVertices, out this.selectionIndices);
      BlockBuilder.Factory.ConstructBlock(this.blockToPlace, Vector3.Zero, out this.blockVertices, out this.blockIndices);
    }

    private void RenderBlock(IRenderCamera camera)
    {
      if (!this.pickedWorldPosition.HasValue)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      Vector3 size1 = (Vector3) this.blockToPlace.Size;
      Byte3 size2 = this.blockToPlace.Size;
      Vector3 modelPlacement = this.blockToPlace.GetBlockType().ModelPlacement;
      Vector3 positionForBlock = this.EvaluateRenderPositionForBlock(camera);
      Quaternion quaternion = this.pickedEntity != null ? this.pickedEntity.Orientation : Quaternion.Identity;
      this.blockEffect.Parameters["World"].SetValue(Matrix.CreateScale(0.99f) * Matrix.CreateTranslation(modelPlacement) * this.blockToPlace.OrientationMatrix * Matrix.CreateFromQuaternion(quaternion) * Matrix.CreateTranslation(positionForBlock));
      this.blockEffect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.blockEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      Engine.GraphicsDevice.VertexDeclaration = this.vertexDeclarationBlock;
      graphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
      graphicsDevice.RenderState.DepthBufferEnable = true;
      graphicsDevice.RenderState.DepthBufferWriteEnable = true;
      graphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      graphicsDevice.RenderState.AlphaBlendEnable = false;
      this.blockEffect.Begin();
      foreach (EffectPass pass in this.blockEffect.CurrentTechnique.Passes)
      {
        pass.Begin();
        graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, this.blockVertices, 0, this.blockVertices.Length, this.blockIndices, 0, this.blockIndices.Length / 3);
        pass.End();
      }
      this.blockEffect.End();
    }

    private void RenderEntityToBlockLine(IRenderCamera camera)
    {
      if (this.pickedEntity == null || BlockItem.lineEffect == null || BlockItem.lineVertices == null || BlockItem.lineIndices == null)
        return;
      Position3 position3 = this.pickedEntity == null ? this.pickedWorldPosition.Value : this.pickedEntity.EntityCoordsToWorldCoords(this.pickedEntityPosition.Value);
      Position3 position = this.pickedEntity.Position;
      float? squaredToClosestBlock = this.pickedEntity.GetDistanceSquaredToClosestBlock(position3);
      if (((double) squaredToClosestBlock.GetValueOrDefault() > 256.0 ? 0 : (squaredToClosestBlock.HasValue ? 1 : 0)) == 0)
        return;
      Matrix lineMatrix = this.EvaluateLineMatrix(camera.GetPositionRelativeToCamera(position3), camera.GetPositionRelativeToCamera(position));
      BlockItem.lineEffect.Parameters["World"].SetValue(lineMatrix);
      BlockItem.lineEffect.Parameters["View"].SetValue(camera.ViewMatrix);
      BlockItem.lineEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      Engine.GraphicsDevice.RenderState.CullMode = CullMode.None;
      Engine.GraphicsDevice.RenderState.DepthBufferEnable = true;
      Engine.GraphicsDevice.VertexDeclaration = Engine.VertexDeclarationPool[1];
      BlockItem.lineEffect.Begin();
      BlockItem.lineEffect.CurrentTechnique.Passes[0].Begin();
      Engine.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, BlockItem.lineVertices, 0, BlockItem.lineVertices.Length, BlockItem.lineIndices, 0, BlockItem.lineIndices.Length / 3);
      BlockItem.lineEffect.CurrentTechnique.Passes[0].End();
      BlockItem.lineEffect.End();
    }

    private void RenderSelectionBox(IRenderCamera camera)
    {
      if (!this.pickedWorldPosition.HasValue)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      Vector3 positionForBlock = this.EvaluateRenderPositionForBlock(camera);
      Quaternion quaternion = this.pickedEntity != null ? this.pickedEntity.Orientation : Quaternion.Identity;
      this.selectionEffect.Parameters["World"].SetValue(Matrix.CreateScale(1.01f) * Matrix.CreateFromQuaternion(quaternion) * Matrix.CreateTranslation(positionForBlock));
      this.selectionEffect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.selectionEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      this.selectionEffect.Parameters["SelectorColor"].SetValue(Vector4.One);
      Engine.GraphicsDevice.VertexDeclaration = this.vertexDeclarationSelector;
      graphicsDevice.RenderState.CullMode = CullMode.None;
      graphicsDevice.RenderState.DepthBufferEnable = true;
      graphicsDevice.RenderState.DepthBufferWriteEnable = false;
      graphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      graphicsDevice.RenderState.AlphaBlendEnable = false;
      this.selectionEffect.Begin();
      foreach (EffectPass pass in this.selectionEffect.CurrentTechnique.Passes)
      {
        pass.Begin();
        graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.selectionVertices, 0, this.selectionVertices.Length, this.selectionIndices, 0, this.selectionIndices.Length / 3);
        pass.End();
      }
      this.selectionEffect.End();
    }

    private void RenderUpArrow(IRenderCamera camera)
    {
      if (BlockItem.arrowVertices == null || BlockItem.arrowIndices == null || camera == null || BlockItem.lineEffect == null)
        return;
      Vector3 up = Vector3.Up;
      if (this.pickedEntity == null)
        return;
      Vector3 worldNormal = this.pickedEntity.EntityNormalToWorldNormal(up);
      Position3 worldCoords = this.pickedEntity.EntityCoordsToWorldCoords(this.pickedEntityPosition.Value);
      Matrix rotationZ = Matrix.CreateRotationZ(1.57079637f);
      if (this.blockToPlace != null)
        rotationZ *= Matrix.CreateTranslation(0.0f, (float) this.blockToPlace.ModelSize.Y - 1f, 0.0f);
      Vector3 axis = Vector3.Cross(Vector3.Up, worldNormal);
      if (axis != Vector3.Zero)
      {
        axis.Normalize();
        float angle = (float) Math.Acos((double) Vector3.Dot(worldNormal, Vector3.Up));
        rotationZ *= Matrix.CreateFromAxisAngle(axis, angle);
      }
      Matrix matrix = rotationZ * Matrix.CreateTranslation(camera.GetPositionRelativeToCamera(worldCoords));
      BlockItem.lineEffect.Parameters["World"].SetValue(matrix);
      BlockItem.lineEffect.Parameters["View"].SetValue(camera.ViewMatrix);
      BlockItem.lineEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      Engine.GraphicsDevice.RenderState.CullMode = CullMode.None;
      Engine.GraphicsDevice.RenderState.DepthBufferEnable = true;
      Engine.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
      Engine.GraphicsDevice.VertexDeclaration = Engine.VertexDeclarationPool[1];
      BlockItem.lineEffect.Begin();
      BlockItem.lineEffect.CurrentTechnique.Passes[0].Begin();
      Engine.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, BlockItem.arrowVertices, 0, BlockItem.arrowVertices.Length, BlockItem.arrowIndices, 0, BlockItem.arrowIndices.Length / 3);
      BlockItem.lineEffect.CurrentTechnique.Passes[0].End();
      BlockItem.lineEffect.End();
    }
  }
}
