// Decompiled with JetBrains decompiler
// Type: CornerSpace.BindTriggerToControlTool
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class BindTriggerToControlTool : ToolItem
  {
    private readonly string cHelpText = "Choose blocks you wish to control!" + Environment.NewLine + Environment.NewLine + "Press Edit again to assign keys!";
    private ControlBlock controlBlock;
    private System.Action dataChanged;
    private Effect effect;
    private LinkedList<Color> availableColors;
    private VertexDeclaration vertexDeclaration;
    private VertexPositionColorTexture[] vertices;
    private short[] indices;

    public BindTriggerToControlTool(ControlBlock controlBlock)
    {
      this.controlBlock = controlBlock != null ? controlBlock : throw new ArgumentNullException();
      this.availableColors = new LinkedList<Color>();
      this.effect = Engine.ShaderPool[4];
      this.vertexDeclaration = Engine.VertexDeclarationPool[3];
      this.ItemId = -1;
      this.UseSelectionBox = true;
      this.SelectorColor = Color.White;
      this.CreateColorList();
      this.CreateVertexAndIndexStructure();
      this.dataChanged = (System.Action) (() => this.CreateVertexAndIndexStructure());
      this.controlBlock.DataChanged += this.dataChanged;
      this.effect.Parameters[nameof (SelectorTexture)].SetValue((Texture) Engine.ContentManager.Load<Texture2D>("Textures/Sprites/squareCorners"));
      this.Crosshair = new SpriteImage(Engine.ContentManager.Load<Texture2D>("Textures/Sprites/Crosshairs/bindBlocks"));
    }

    public override void Close()
    {
      base.Close();
      this.controlBlock.DataChanged -= this.dataChanged;
    }

    public override Item Copy()
    {
      BindTriggerToControlTool triggerToControlTool = base.Copy() as BindTriggerToControlTool;
      triggerToControlTool.dataChanged = (System.Action) (() => this.CreateVertexAndIndexStructure());
      return (Item) triggerToControlTool;
    }

    public override void DrawHelpTexts() => this.DrawHelpString(this.cHelpText);

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      if (input.LeftClick && this.pickedBlockCell != null && this.controlBlock.OwnerSector.Owner == this.pickedEntity && this.pickedBlockCell.Block is TriggerBlock)
      {
        TriggerBlock block = this.pickedBlockCell.Block as TriggerBlock;
        LinkedList<Color> linkedList = new LinkedList<Color>();
        foreach (ColorKeyGroup colorKeyGroup in this.controlBlock.ColorKeyGroups)
          linkedList.AddLast(colorKeyGroup.Color);
        if (!this.controlBlock.IsBlockBound(block))
        {
          this.controlBlock.BindBlock(block, linkedList.First.Value, true);
        }
        else
        {
          Color? blockColor = this.controlBlock.GetBlockColor(block);
          if (blockColor.HasValue)
          {
            LinkedListNode<Color> linkedListNode = linkedList.Find(blockColor.Value);
            if (linkedListNode != null)
            {
              if (linkedListNode == linkedList.Last)
                this.controlBlock.UnbindBlock(block, true);
              else
                this.controlBlock.RebindBlock(block, linkedListNode.Next.Value, true);
            }
          }
        }
      }
      return Item.UsageResult.None;
    }

    public override void Update(Astronaut owner)
    {
      base.Update(owner);
      if (this.pickedBlockCell == null || this.IsPickedBlockValid(this.pickedBlockCell.Block))
        return;
      this.pickedBlockCell = (BlockCell) null;
    }

    public override void RenderFirstPerson(IRenderCamera camera)
    {
      base.RenderFirstPerson(camera);
      SpaceEntity owner = this.controlBlock.OwnerSector.Owner;
      if (owner == null || this.vertices.Length <= 0 || this.indices.Length <= 0)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      this.effect.Parameters["World"].SetValue(owner.TransformMatrix * Matrix.CreateTranslation(camera.GetPositionRelativeToCamera(owner.Position)));
      this.effect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      graphicsDevice.BlendState = BlendState.Opaque;
      graphicsDevice.DepthStencilState = DepthStencilState.Default;
      graphicsDevice.RasterizerState = RasterizerState.CullNone;
      this.effect.CurrentTechnique.Passes[0].Apply();
      graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.vertices, 0, this.vertices.Length, this.indices, 0, this.indices.Length / 3);
    }

    public ControlBlock ControlBlock => this.controlBlock;

    public Texture2D SelectorTexture
    {
      set
      {
        if (value == null || this.effect == null)
          return;
        this.effect.Parameters[nameof (SelectorTexture)].SetValue((Texture) value);
      }
    }

    private void AddBlockToRenderArray(Block block, Vector3 positionInShipSpace)
    {
    }

    protected void CreateColorList()
    {
      this.availableColors.Clear();
      foreach (ColorKeyGroup colorKeyGroup in this.controlBlock.ColorKeyGroups)
        this.availableColors.AddLast(colorKeyGroup.Color);
    }

    protected void CreateColorList(Color[] colors)
    {
      this.availableColors.Clear();
      foreach (Color color in colors)
        this.availableColors.AddLast(color);
    }

    protected override SpriteImage CreateCrosshair()
    {
      return new SpriteImage(Engine.ContentManager.Load<Texture2D>("Textures/Sprites/Crosshairs/bindBlocks"));
    }

    protected void CreateVertexAndIndexStructure()
    {
      int destinationIndex1 = 0;
      int destinationIndex2 = 0;
      this.vertices = new VertexPositionColorTexture[24 * (this.controlBlock.BlockCount + 1)];
      this.indices = new short[36 * (this.controlBlock.BlockCount + 1)];
      VertexPositionColorTexture[] vertices;
      short[] indices;
      foreach (Color key in this.controlBlock.ControlledBlocks.Keys)
      {
        foreach (TriggerBlock triggerBlock in this.controlBlock.ControlledBlocks[key])
        {
          if (this.IsPickedBlockValid((Block) triggerBlock))
          {
            Primitive.CreateSelectorBox((Vector3) triggerBlock.Size, out vertices, out indices);
            for (int index = 0; index < vertices.Length; ++index)
            {
              vertices[index].Position = vertices[index].Position * 1.03f + triggerBlock.PositionInShipSpace;
              vertices[index].Color = key;
            }
            for (int index = 0; index < indices.Length; ++index)
              indices[index] += (short) destinationIndex1;
            Array.Copy((Array) vertices, 0, (Array) this.vertices, destinationIndex1, vertices.Length);
            Array.Copy((Array) indices, 0, (Array) this.indices, destinationIndex2, indices.Length);
            destinationIndex1 += vertices.Length;
            destinationIndex2 += indices.Length;
          }
        }
      }
      Primitive.CreateSelectorBox((Vector3) this.controlBlock.Size, out vertices, out indices);
      for (int index = 0; index < vertices.Length; ++index)
      {
        vertices[index].Position = vertices[index].Position * 1.03f + this.controlBlock.PositionInEntitySpace;
        vertices[index].Color = Color.Blue;
      }
      for (int index = 0; index < indices.Length; ++index)
        indices[index] += (short) destinationIndex1;
      Array.Copy((Array) vertices, 0, (Array) this.vertices, destinationIndex1, vertices.Length);
      Array.Copy((Array) indices, 0, (Array) this.indices, destinationIndex2, indices.Length);
    }

    protected virtual bool IsPickedBlockValid(Block block)
    {
      return block is TriggerBlock && !(block is CameraBlock);
    }
  }
}
