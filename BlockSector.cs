// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockSector
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
  public class BlockSector : IDisposable, ILightChunk
  {
    private const float cRenderQualityDistanceMargin = 8f;
    private const float transitionDistance = 20f;
    private const int billboardLevels = 6;
    private object blockListLock = new object();
    private object PartitionTreeLock = new object();
    private List<PowerNetwork> foundPowerNetworks;
    private List<Vector3> foundPowerBlocks;
    private HashSet<PowerBlock> visitedPowerBlocks;
    private int blockCount;
    private Vector3 centerOfMass;
    private BlockSector.ChangeUpdateState changeUpdateStateFunction;
    private List<ContainerBlock> containerBlocks;
    private List<IDynamicBlock> blocksRequiringUpdate;
    private Vector3 inertiaTensor;
    private List<LightSource> lights;
    private byte[] lookupTable;
    private float mass;
    private BlockOctTree blockOctTree;
    private SpaceEntity ownerEntity;
    private Vector3 positionInShipCoordinates;
    private int previousQuality;
    private Matrix transformMatrix;
    private List<ControlBlock> controlBlocks;
    private StaticBlockRenderManager blockRenderManager;
    private DynamicBlockRenderManager dynamicBlockRenderManager;
    private ThrusterRenderManager thrusterRenderManager;
    private static GraphicBuffersManager bufferManager;
    private static int size = 16;
    private static Vector3[] adjacentDirections = new Vector3[6]
    {
      Vector3.Up,
      Vector3.Down,
      Vector3.Backward,
      Vector3.Forward,
      Vector3.Left,
      Vector3.Right
    };

    public BlockSector(SpaceEntity owner)
    {
      this.blockCount = 0;
      this.blockOctTree = new BlockOctTree(1, Vector3.Zero, Vector3.One * (float) BlockSector.Size);
      this.blocksRequiringUpdate = new List<IDynamicBlock>();
      this.inertiaTensor = Vector3.Zero;
      this.lights = new List<LightSource>();
      this.mass = 0.0f;
      this.centerOfMass = Vector3.Zero;
      this.positionInShipCoordinates = Vector3.One * 0.5f * (float) BlockSector.size;
      this.containerBlocks = new List<ContainerBlock>();
      this.controlBlocks = new List<ControlBlock>();
      this.ownerEntity = owner;
      this.previousQuality = 4;
      this.lookupTable = new byte[512];
      this.foundPowerNetworks = new List<PowerNetwork>();
      this.foundPowerBlocks = new List<Vector3>();
      this.visitedPowerBlocks = new HashSet<PowerBlock>();
      this.InitializeManagers();
      this.InitializeDelegateFunctions();
    }

    public bool AddBlock(Block block, Vector3 blockPosition)
    {
      Byte3 size = block.Size;
      ++this.blockCount;
      this.AddMass(block, blockPosition + block.ModelPlacement, this.blockCount);
      this.UpdateInertiaTensor();
      block.Added(this.ownerEntity, this, blockPosition);
      if (block is IDynamicBlock)
        ((IDynamicBlock) block).SetUpdateInterface(this.changeUpdateStateFunction);
      if (block.HasDynamicBehavior)
        this.dynamicBlockRenderManager.AddBlock(block, blockPosition);
      else
        this.blockRenderManager.ForceUpdate();
      this.thrusterRenderManager.AddBlock(block as EngineBlock);
      if (block is LightBlock)
        this.lights.Add(((LightBlock) block).LightSource);
      return true;
    }

    public bool AddBlockCell(Vector3 positionInEntitySpace, Block cell)
    {
      int index = 0;
      int bit = 0;
      if (cell != null)
      {
        if (this.blockOctTree.AddLeafIfFits(cell, positionInEntitySpace) && this.GetLookupTableAddress(positionInEntitySpace, ref index, ref bit))
          this.SetLookupBit(index, bit);
      }
      else
      {
        this.blockOctTree.RemoveLeaf(positionInEntitySpace);
        if (this.GetLookupTableAddress(positionInEntitySpace, ref index, ref bit))
          this.UnsetLookupBit(index, bit);
      }
      return true;
    }

    public void ConstructBlock(Vector3 positionEntityCoords)
    {
      this.blockOctTree.ConstructBlock(positionEntityCoords);
      this.blockRenderManager.ForceUpdate();
    }

    public void Dispose()
    {
      this.DisposeManagers();
      this.controlBlocks.ForEach((Action<ControlBlock>) (b => b.UnMount()));
    }

    public void ForceConstruct() => this.blockOctTree.ReconstructBlocks();

    public BlockCell GetBlockCell(Vector3 position)
    {
      return this.blockOctTree.GetLeafNode(position) as BlockCell;
    }

    public static Vector3 GetBlockPositionInBlockCoordinates(Vector3 position)
    {
      return new Vector3((float) (int) Math.Floor((double) position.X), (float) (int) Math.Floor((double) position.Y), (float) (int) Math.Floor((double) position.Z)) + Vector3.One * 0.5f;
    }

    public float GetMomentOfInertia(Vector3 rotationAxis, Vector3 centerOfMass)
    {
      if (rotationAxis == Vector3.Zero)
        return 0.0f;
      Vector3 vector2 = Vector3.Normalize(rotationAxis);
      Vector3 vector1 = this.centerOfMass - centerOfMass;
      float num = (Vector3.Dot(vector1, vector2) * vector2 - vector1).Length();
      return (float) (0.40000000596046448 * (double) this.mass * 0.25 * (double) (BlockSector.Size * BlockSector.Size) + (double) this.mass * (double) num * (double) num);
    }

    public bool IsBlocked(Vector3 positionInEntitySpace)
    {
      int index = 0;
      int bit = 0;
      return this.GetLookupTableAddress(positionInEntitySpace, ref index, ref bit) && ((int) this.lookupTable[index] & (int) (byte) (128 >> bit)) != 0;
    }

    public static void SetRenderstates()
    {
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      if (graphicsDevice == null)
        return;
      graphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
      graphicsDevice.RenderState.DepthBufferEnable = true;
      graphicsDevice.RenderState.DepthBufferWriteEnable = true;
      graphicsDevice.RenderState.AlphaBlendEnable = false;
      graphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
    }

    public void Render(IRenderCamera camera, ref Matrix sectorTransformMatrix, float distance)
    {
      this.transformMatrix = sectorTransformMatrix;
      Matrix transformMatrix = Matrix.CreateTranslation(-Vector3.One * (float) BlockSector.Size * 0.5f) * Matrix.CreateTranslation(this.positionInShipCoordinates) * sectorTransformMatrix;
      this.blockRenderManager.SectorMatrix = transformMatrix;
      this.dynamicBlockRenderManager.SectorMatrix = sectorTransformMatrix;
      byte renderQuality = (byte) this.EvaluateRenderQuality(distance);
      this.blockRenderManager.Render(camera, renderQuality);
      this.dynamicBlockRenderManager.Render(camera, renderQuality);
      if ((double) distance > 128.0)
        return;
      this.thrusterRenderManager.RenderFullQuality(camera, ref transformMatrix);
    }

    public bool RemoveBlock(Block block, Vector3 entityPosition)
    {
      this.ConvertToBlocks(entityPosition);
      Byte3 size = block.Size;
      --this.blockCount;
      this.RemoveMass(block, entityPosition + block.ModelPlacement, this.blockCount);
      block.Removed(this.ownerEntity, this, entityPosition);
      if (block is IDynamicBlock)
        this.blocksRequiringUpdate.Remove(block as IDynamicBlock);
      if (block.HasDynamicBehavior)
        this.dynamicBlockRenderManager.RemoveBlock(block);
      else
        this.blockRenderManager.ForceUpdate();
      this.UpdateInertiaTensor();
      if (block is EngineBlock)
        this.thrusterRenderManager.RemoveBlock(block as EngineBlock);
      if (block is LightBlock)
        this.lights.Remove(((LightBlock) block).LightSource);
      return true;
    }

    public bool SectorEmpty() => this.blockOctTree.IsEmpty();

    public void UpdateBlocks()
    {
      for (int index = 0; index < this.blocksRequiringUpdate.Count; ++index)
        this.blocksRequiringUpdate[index].Update();
    }

    public void UpdatePowerBlocks()
    {
      this.blockOctTree.UpdatePowerBlocks();
      this.blockRenderManager.ForceUpdate();
      this.dynamicBlockRenderManager.ReconstructPowerBlocks();
    }

    public int BlockCount => this.blockCount;

    public virtual Vector3 CenterOfMass
    {
      get => this.centerOfMass;
      set => this.centerOfMass = value;
    }

    public List<ContainerBlock> ContainerBlocks => this.containerBlocks;

    public List<ControlBlock> ControlBlocks => this.controlBlocks;

    public virtual bool Dummy => false;

    public static GraphicBuffersManager GraphicBufferManager
    {
      get
      {
        if (BlockSector.bufferManager == null)
          BlockSector.bufferManager = new GraphicBuffersManager();
        return BlockSector.bufferManager;
      }
    }

    public bool HasDrawableGeometry => this.blockRenderManager.HasGeometry;

    public Vector3 InertiaTensor => this.inertiaTensor;

    public List<LightSource> LightSources => this.lights;

    public virtual float Mass
    {
      get => this.mass;
      set => this.mass = value;
    }

    public SpaceEntity Owner => this.ownerEntity;

    public BlockOctTree OctTree => this.blockOctTree;

    public Vector3 PositionInShipCoordinates
    {
      get => this.positionInShipCoordinates;
      set
      {
        Vector3 inShipCoordinates = this.positionInShipCoordinates;
        this.positionInShipCoordinates = value;
        Vector3 vector3 = value - inShipCoordinates;
        this.blockRenderManager.SectorPosition = value;
        this.dynamicBlockRenderManager.SectorPosition = value;
        this.blockOctTree.Bounds = new BoundingBox(value - Vector3.One * (float) BlockSector.Size / 2f, value + Vector3.One * (float) BlockSector.size / 2f);
      }
    }

    public static int Size => BlockSector.size;

    public Matrix TransformMatrix => this.transformMatrix;

    public virtual bool UnderConstruction
    {
      get => false;
      set
      {
      }
    }

    private void AddMass(Block block, Vector3 positionInShipSpace, int newBlockCount)
    {
      if (newBlockCount == 1)
      {
        this.centerOfMass = positionInShipSpace;
        this.mass = block.Mass;
      }
      else
      {
        if (newBlockCount <= 1)
          return;
        Vector3 vector3 = this.centerOfMass * this.mass;
        this.mass += block.Mass;
        this.centerOfMass = (vector3 + positionInShipSpace * block.Mass) / this.mass;
      }
    }

    private Vector3 ConvertPositionToSector(Vector3 positionInShipSpace)
    {
      Vector3 vector3 = new Vector3((float) ((int) positionInShipSpace.X % BlockSector.size), (float) ((int) positionInShipSpace.Y % BlockSector.size), (float) ((int) positionInShipSpace.Z % BlockSector.size)) + Vector3.One * 0.5f;
      return new Vector3((double) vector3.X < 0.0 ? (float) BlockSector.size + vector3.X : vector3.X, (double) vector3.Y < 0.0 ? (float) BlockSector.size + vector3.Y : vector3.Y, (double) vector3.Z < 0.0 ? (float) BlockSector.size + vector3.Z : vector3.Z);
    }

    private Vector3int ConvertToBlocks(Vector3 position)
    {
      Vector3 vector3 = -(this.positionInShipCoordinates - Vector3.One * (float) BlockSector.Size * 0.5f) + position;
      return new Vector3int((int) Math.Floor((double) vector3.X), (int) Math.Floor((double) vector3.Y), (int) Math.Floor((double) vector3.Z));
    }

    private void DisposeManagers()
    {
      this.dynamicBlockRenderManager.Dispose();
      this.blockRenderManager.Dispose();
      this.thrusterRenderManager.Dispose();
      this.dynamicBlockRenderManager = (DynamicBlockRenderManager) null;
      this.blockRenderManager = (StaticBlockRenderManager) null;
      this.thrusterRenderManager = (ThrusterRenderManager) null;
    }

    private int EvaluateRenderQuality(float sectorDistance)
    {
      int currentViewDistance = Engine.SettingsManager.CurrentViewDistance;
      int renderQuality = (int) ((double) sectorDistance / (double) currentViewDistance);
      if (renderQuality == this.previousQuality || (double) sectorDistance >= (double) (this.previousQuality * currentViewDistance) - 8.0 && (double) sectorDistance <= (double) ((this.previousQuality + 1) * currentViewDistance) + 8.0)
        return this.previousQuality;
      this.previousQuality = renderQuality;
      this.blockRenderManager.ForceUpdate();
      return renderQuality;
    }

    private bool GetLookupTableAddress(Vector3 positionInEntitySpace, ref int index, ref int bit)
    {
      Vector3 vector3 = Vector3.One * 8f - this.positionInShipCoordinates + positionInEntitySpace;
      int x = (int) vector3.X;
      int y = (int) vector3.Y;
      int num = (int) vector3.Z * 256 + y * 16 + x;
      index = num / 8;
      bit = num % 8;
      return index >= 0 && index < 512;
    }

    private void InitializeDelegateFunctions()
    {
      this.changeUpdateStateFunction = (BlockSector.ChangeUpdateState) ((sender, state) =>
      {
        if (state)
        {
          if (this.blocksRequiringUpdate.Contains(sender))
            return;
          this.blocksRequiringUpdate.Add(sender);
        }
        else
          this.blocksRequiringUpdate.Remove(sender);
      });
    }

    private void InitializeManagers()
    {
      this.dynamicBlockRenderManager = new DynamicBlockRenderManager();
      this.blockRenderManager = new StaticBlockRenderManager();
      this.thrusterRenderManager = new ThrusterRenderManager(this.ownerEntity);
      this.blockRenderManager.SetVertexStructureTree(this.blockOctTree);
      this.dynamicBlockRenderManager.SetVertexStructureTree(this.blockOctTree);
    }

    private void RemoveMass(Block block, Vector3 positionInShipSpace, int newBlockCount)
    {
      if (newBlockCount == 0)
      {
        this.mass = 0.0f;
        this.centerOfMass = Vector3.Zero;
      }
      else
      {
        if (newBlockCount <= 0)
          return;
        this.centerOfMass -= block.Mass * positionInShipSpace / this.mass;
        this.centerOfMass *= this.mass / (this.mass - block.Mass);
        this.mass -= block.Mass;
      }
    }

    private void SetLookupBit(int index, int bit)
    {
      if (index < 0 || index >= 512)
        return;
      this.lookupTable[index] |= (byte) (128 >> bit);
    }

    private void UnsetLookupBit(int index, int bit)
    {
      if (index < 0 || index >= 512)
        return;
      this.lookupTable[index] &= (byte) ~(128 >> bit);
    }

    private void UpdateInertiaTensor()
    {
      this.inertiaTensor = (float) (0.40000000596046448 * (double) this.mass * 0.25) * (float) (BlockSector.Size * BlockSector.Size) * Vector3.One;
    }

    public delegate void ChangeUpdateState(IDynamicBlock sender, bool needUpdate);

    private enum RenderQuality
    {
      FullQuality = 60, // 0x0000003C
      BillboardQuality = 220, // 0x000000DC
      SpriteQuality = 400, // 0x00000190
      None = 1500, // 0x000005DC
    }
  }
}
