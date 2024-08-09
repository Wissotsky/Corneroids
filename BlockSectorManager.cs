// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockSectorManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace
{
  public class BlockSectorManager : IDisposable
  {
    private const int maxNumberOfSectorsInReserve = 50;
    private static Vector3[] adjacentDirections = new Vector3[6]
    {
      Vector3.Up,
      Vector3.Down,
      Vector3.Left,
      Vector3.Right,
      Vector3.Forward,
      Vector3.Backward
    };
    private static Queue<BlockSector> createdSectorsAsync = new Queue<BlockSector>();
    private HashSet<BlockSector> foundSectors = new HashSet<BlockSector>();
    private Dictionary<Block, Vector3> adjacentBlocks = new Dictionary<Block, Vector3>();
    private Dictionary<PowerBlock, Vector3> adjacentPowerBlocks = new Dictionary<PowerBlock, Vector3>();
    private HashSet<PowerNetwork> foundPowerNetworks = new HashSet<PowerNetwork>();
    private SpaceEntity ownerEntity;
    private PowerGridSolver powerGridSolver;
    private List<PowerNetwork> powerNetworks;
    private CornerSpace.OctTree<OctTreeNode<BlockSector>, BlockSector> sectorOctTree;
    private List<BlockSector> sectorsInList;

    public BlockSectorManager(SpaceEntity owner)
    {
      this.ownerEntity = owner != null ? owner : throw new ArgumentNullException();
      this.powerNetworks = new List<PowerNetwork>();
      this.sectorOctTree = new CornerSpace.OctTree<OctTreeNode<BlockSector>, BlockSector>(BlockSector.Size);
      this.sectorsInList = new List<BlockSector>();
      this.powerGridSolver = new PowerGridSolver(this, 32U);
    }

    public bool AddBlock(Block block, Vector3 positionInEntitySpace, bool construct)
    {
      Vector3 positionForFirstCell = this.GetPositionForFirstCell(block, positionInEntitySpace);
      Vector3 positionForBlock = this.GetPositionForBlock(block, positionInEntitySpace);
      if (!this.CheckBlockFits(block, positionForFirstCell))
        return false;
      BlockSector sector = this.AddBlockCells(positionForFirstCell, block);
      sector.AddBlock(block, positionForBlock);
      if (block is PowerBlock)
        this.AddPowerBlock(block as PowerBlock, positionForFirstCell, sector);
      if (construct)
        this.ConstructNewBlockAfterAdd(positionForBlock, true);
      return true;
    }

    public bool CheckBlockFits(Block block, Vector3 positionInEntityCoordinates)
    {
      Byte3 size = block.Size;
      for (int index1 = 0; index1 < (int) size.Z; ++index1)
      {
        for (int index2 = 0; index2 < (int) size.Y; ++index2)
        {
          for (int index3 = 0; index3 < (int) size.X; ++index3)
          {
            Vector3 positionInEntitySpace = positionInEntityCoordinates + Vector3.UnitX * (float) index3 + Vector3.UnitY * (float) index2 + Vector3.UnitZ * (float) index1;
            BlockSector sector = this.GetSector(positionInEntitySpace);
            if (sector != null && sector.IsBlocked(positionInEntitySpace))
              return false;
          }
        }
      }
      return true;
    }

    public void Dispose()
    {
      foreach (BlockSector sectorsIn in this.sectorsInList)
        sectorsIn.Dispose();
    }

    public void ForceConstruct()
    {
      foreach (BlockSector sectorsIn in this.sectorsInList)
      {
        Vector3 vector3_1 = sectorsIn.PositionInShipCoordinates - Vector3.One * 7.5f;
        for (byte z = 0; z < (byte) 16; ++z)
        {
          for (byte y = 0; y < (byte) 16; ++y)
          {
            for (byte x = 0; x < (byte) 16; ++x)
            {
              Vector3 vector3_2 = vector3_1 + new Vector3((float) x, (float) y, (float) z);
              BlockCell block = this.GetBlock(vector3_2);
              if (block != null && block.X == (byte) 0 && block.Y == (byte) 0 && block.Z == (byte) 0)
                block.Block.Visibility = block.Block.Transparent ? (byte) 63 : this.CheckBlockVisibility(vector3_2, block.Block.Size, (Dictionary<Block, Vector3>) null, true);
            }
          }
        }
        sectorsIn.ForceConstruct();
      }
    }

    public BlockCell GetBlock(Vector3 positionInEntityCoordinates)
    {
      return this.GetSector(positionInEntityCoordinates)?.GetBlockCell(positionInEntityCoordinates);
    }

    public BoundingBox GetBounds() => this.sectorOctTree.Bounds;

    public Vector3? GetPositionOfClosestBlock(Vector3 positionInEntitySpace)
    {
      Vector3? closestNode = this.sectorOctTree.GetClosestNode(positionInEntitySpace);
      if (closestNode.HasValue)
      {
        BlockSector leaf = this.sectorOctTree.GetLeaf(closestNode.Value);
        if (leaf != null)
          return leaf.OctTree.GetClosestNode(positionInEntitySpace);
      }
      return new Vector3?();
    }

    public BlockSector GetSector(Vector3 positionInEntitySpace)
    {
      return this.sectorOctTree.GetLeaf(positionInEntitySpace);
    }

    public bool IsBlocked(Vector3 positionInEntitySpace)
    {
      BlockSector sector = this.GetSector(positionInEntitySpace);
      return sector != null && sector.IsBlocked(positionInEntitySpace);
    }

    public bool RemoveBlock(Vector3 position)
    {
      BlockCell block = this.GetBlock(position);
      if (block == null)
        return false;
      Vector3 vector3 = this.GetBlockPositionInBlockCoordinates(position) - new Vector3((float) block.X, (float) block.Y, (float) block.Z);
      Vector3 entityPosition = vector3 - Vector3.One * 0.5f + 0.5f * (Vector3) block.Block.Size;
      BlockSector sector = this.GetSector(vector3);
      sector.RemoveBlock(block.Block, entityPosition);
      this.RemoveBlockCells(vector3, block.Block);
      this.ConstructBlocksAfterRemove(vector3, block.Block.Size);
      if (block.Block is PowerBlock)
        this.RemovePowerBlock(block.Block as PowerBlock, vector3, sector);
      return true;
    }

    public bool RemoveSector(BlockSector sector)
    {
      if (sector == null || this.sectorOctTree.RemoveLeaf(sector.PositionInShipCoordinates) != sector)
        return false;
      this.SectorList.Remove(sector);
      return true;
    }

    public void UpdateBlocks()
    {
      foreach (BlockSector sector in this.SectorList)
        sector.UpdateBlocks();
    }

    public CornerSpace.OctTree<OctTreeNode<BlockSector>, BlockSector> OctTree => this.sectorOctTree;

    public List<BlockSector> SectorList => this.sectorsInList;

    private void AddPowerBlock(PowerBlock block, Vector3 firstCellPosition, BlockSector sector)
    {
      if (block == null)
        return;
      this.GetAdjacentPowerNetworks(block, firstCellPosition, this.foundPowerNetworks);
      PowerNetwork powerNetwork;
      if (this.foundPowerNetworks.Count == 0)
      {
        powerNetwork = new PowerNetwork();
        this.powerNetworks.Add(powerNetwork);
      }
      else
      {
        PowerNetwork[] array = this.foundPowerNetworks.ToArray<PowerNetwork>();
        powerNetwork = array[0];
        for (int index = 1; index < array.Length; ++index)
        {
          powerNetwork.MergeWith(array[index]);
          this.powerNetworks.Remove(array[index]);
        }
      }
      powerNetwork.AddBlock(block, sector);
      powerNetwork.UpdateNetworkStructure();
      powerNetwork.UpdateBlockSectors();
    }

    private BlockSector AddBlockCells(Vector3 positionInEntitySpace, Block block)
    {
      Byte3 size = block.Size;
      for (byte index1 = 0; (int) index1 < (int) size.Z; ++index1)
      {
        for (byte index2 = 0; (int) index2 < (int) size.Y; ++index2)
        {
          for (byte index3 = 0; (int) index3 < (int) size.X; ++index3)
          {
            Vector3 vector3 = positionInEntitySpace + Vector3.UnitX * (float) index3 + Vector3.UnitY * (float) index2 + Vector3.UnitZ * (float) index1;
            BlockSector blockSector = this.GetSector(vector3) ?? this.CreateNewSector(vector3);
            blockSector.AddBlockCell(vector3, block);
            BlockCell blockCell = blockSector.GetBlockCell(vector3);
            if (blockCell != null)
            {
              blockCell.X = index3;
              blockCell.Y = index2;
              blockCell.Z = index1;
            }
          }
        }
      }
      return this.GetSector(positionInEntitySpace);
    }

    private byte CheckBlockVisibility(
      Vector3 firstCellPosition,
      Byte3 blockSize,
      Dictionary<Block, Vector3> adjacentBlocks,
      bool skipTransparent)
    {
      byte visibility = 0;
      for (byte z = 0; (int) z < (int) blockSize.Z; ++z)
      {
        for (byte y = 0; (int) y < (int) blockSize.Y; ++y)
        {
          for (byte x = 0; (int) x < (int) blockSize.X; ++x)
          {
            Vector3 vector3 = firstCellPosition + new Vector3((float) x, (float) y, (float) z);
            if (x == (byte) 0)
              visibility = this.GetAdjacentBlock(vector3 + Vector3.Left, visibility, (byte) 1, adjacentBlocks, skipTransparent);
            if ((int) x == (int) blockSize.X - 1)
              visibility = this.GetAdjacentBlock(vector3 + Vector3.Right, visibility, (byte) 2, adjacentBlocks, skipTransparent);
            if (y == (byte) 0)
              visibility = this.GetAdjacentBlock(vector3 + Vector3.Down, visibility, (byte) 4, adjacentBlocks, skipTransparent);
            if ((int) y == (int) blockSize.Y - 1)
              visibility = this.GetAdjacentBlock(vector3 + Vector3.Up, visibility, (byte) 8, adjacentBlocks, skipTransparent);
            if (z == (byte) 0)
              visibility = this.GetAdjacentBlock(vector3 + Vector3.Forward, visibility, (byte) 16, adjacentBlocks, skipTransparent);
            if ((int) z == (int) blockSize.Z - 1)
              visibility = this.GetAdjacentBlock(vector3 + Vector3.Backward, visibility, (byte) 32, adjacentBlocks, skipTransparent);
          }
        }
      }
      return visibility;
    }

    public void ConstructBlocksAfterRemove(Vector3 firstCellPosition, Byte3 removedBlockSize)
    {
      Dictionary<Block, Vector3> adjacentBlocks = new Dictionary<Block, Vector3>();
      int num = (int) this.CheckBlockVisibility(firstCellPosition, removedBlockSize, adjacentBlocks, true);
      foreach (Block key in adjacentBlocks.Keys)
        this.ConstructNewBlockAfterAdd(adjacentBlocks[key], false);
    }

    public void ConstructNewBlockAfterAdd(Vector3 position, bool updateAdjacent)
    {
      position = 0.5f * Vector3.One + new Vector3((float) (int) Math.Floor((double) position.X), (float) (int) Math.Floor((double) position.Y), (float) (int) Math.Floor((double) position.Z));
      BlockCell block1 = this.GetBlock(position);
      if (block1 == null)
        return;
      Block block2 = block1.Block;
      Vector3 vector3 = position - new Vector3((float) block1.X, (float) block1.Y, (float) block1.Z);
      Byte3 size = block2.Size;
      Dictionary<Block, Vector3> adjacentBlocks = updateAdjacent ? new Dictionary<Block, Vector3>() : (Dictionary<Block, Vector3>) null;
      byte num = block2.Transparent ? (byte) 63 : this.CheckBlockVisibility(vector3, size, adjacentBlocks, true);
      block2.Visibility = num;
      this.GetSector(vector3).ConstructBlock(vector3);
      if (!updateAdjacent)
        return;
      foreach (Block key in adjacentBlocks.Keys)
        this.ConstructNewBlockAfterAdd(adjacentBlocks[key], false);
    }

    private BlockSector CreateNewSector(Vector3 positionInEntityCoordinates)
    {
      BlockSector sector = this.GetSector(positionInEntityCoordinates);
      if (sector != null)
        return sector;
      BlockSector leafData = new BlockSector(this.ownerEntity);
      Vector3 position = (float) BlockSector.Size * this.GetSectorPosition(positionInEntityCoordinates) + (float) (BlockSector.Size / 2) * Vector3.One;
      leafData.PositionInShipCoordinates = position;
      this.sectorsInList.Add(leafData);
      this.sectorOctTree.AddLeaf(leafData, position);
      return leafData;
    }

    private byte GetAdjacentBlock(
      Vector3 position,
      byte visibility,
      byte bitMask,
      Dictionary<Block, Vector3> adjacentBlocks,
      bool skipTransparent)
    {
      BlockCell block = this.GetBlock(position);
      if (block == null)
        visibility |= bitMask;
      else if (block != null && block.Block.Transparent && skipTransparent)
        visibility |= bitMask;
      else if (adjacentBlocks != null && !adjacentBlocks.ContainsKey(block.Block))
        adjacentBlocks.Add(block.Block, position);
      return visibility;
    }

    private void GetAdjacentPowerBlocks(
      Vector3 firstCellPosition,
      Byte3 size,
      Dictionary<PowerBlock, Vector3> powerBlocks)
    {
      if (powerBlocks == null)
        return;
      this.adjacentBlocks.Clear();
      powerBlocks.Clear();
      int num = (int) this.CheckBlockVisibility(firstCellPosition, size, this.adjacentBlocks, false);
      foreach (Block key in this.adjacentBlocks.Keys)
      {
        if (key is PowerBlock)
          powerBlocks.Add(key as PowerBlock, this.adjacentBlocks[key]);
      }
    }

    private void GetAdjacentPowerNetworks(
      PowerBlock block,
      Vector3 firstCellPosition,
      HashSet<PowerNetwork> foundNetworks)
    {
      if (block == null || foundNetworks == null)
        return;
      foundNetworks.Clear();
      this.adjacentBlocks.Clear();
      int num = (int) this.CheckBlockVisibility(firstCellPosition, block.Size, this.adjacentBlocks, false);
      foreach (Block key in this.adjacentBlocks.Keys)
      {
        if (key is PowerBlock powerBlock)
          foundNetworks.Add(powerBlock.Network);
      }
    }

    private Vector3 GetPositionForBlock(Block block, Vector3 middlePosition)
    {
      Vector3 positionForFirstCell = this.GetPositionForFirstCell(block, middlePosition);
      Vector3 vector3 = positionForFirstCell + ((Vector3) block.Size - Vector3.One);
      return 0.5f * (positionForFirstCell + vector3);
    }

    private Vector3 GetBlockPositionInBlockCoordinates(Vector3 position)
    {
      return new Vector3((float) (int) Math.Floor((double) position.X), (float) (int) Math.Floor((double) position.Y), (float) (int) Math.Floor((double) position.Z)) + Vector3.One * 0.5f;
    }

    private Vector3 GetPositionForFirstCell(Block block, Vector3 position)
    {
      Vector3 blockCoordinates = this.GetBlockPositionInBlockCoordinates(position);
      Byte3 size = block.Size;
      blockCoordinates.X -= (float) (((int) size.X - (int) size.X % 2) / 2);
      blockCoordinates.Y -= (float) (((int) size.Y - (int) size.Y % 2) / 2);
      blockCoordinates.Z -= (float) (((int) size.Z - (int) size.Z % 2) / 2);
      return blockCoordinates;
    }

    private Vector3 GetSectorPosition(Vector3 positionInEntityCoordinates)
    {
      positionInEntityCoordinates.X /= (float) BlockSector.Size;
      positionInEntityCoordinates.Y /= (float) BlockSector.Size;
      positionInEntityCoordinates.Z /= (float) BlockSector.Size;
      return new Vector3((float) (int) Math.Floor((double) positionInEntityCoordinates.X), (float) (int) Math.Floor((double) positionInEntityCoordinates.Y), (float) (int) Math.Floor((double) positionInEntityCoordinates.Z));
    }

    private void RemoveBlockCells(Vector3 positionInEntitySpace, Block block)
    {
      Byte3 size = block.Size;
      this.foundSectors.Clear();
      for (int index1 = 0; index1 < (int) size.Z; ++index1)
      {
        for (int index2 = 0; index2 < (int) size.Y; ++index2)
        {
          for (int index3 = 0; index3 < (int) size.X; ++index3)
          {
            Vector3 positionInEntitySpace1 = positionInEntitySpace + Vector3.UnitX * (float) index3 + Vector3.UnitY * (float) index2 + Vector3.UnitZ * (float) index1;
            BlockSector sector = this.GetSector(positionInEntitySpace1);
            sector.AddBlockCell(positionInEntitySpace1, (Block) null);
            if (sector.SectorEmpty())
            {
              this.RemoveSector(sector);
              sector.Dispose();
            }
          }
        }
      }
    }

    private void RemovePowerBlock(PowerBlock block, Vector3 firstCellPosition, BlockSector sector)
    {
      if (block == null)
        return;
      PowerNetwork network = block.Network;
      if (network == null)
        return;
      network.RemoveBlock(block, sector);
      this.GetAdjacentPowerBlocks(firstCellPosition, block.Size, this.adjacentPowerBlocks);
      if (this.adjacentPowerBlocks.Count == 0)
        this.powerNetworks.Remove(network);
      else if (this.adjacentPowerBlocks.Count == 1)
      {
        network.RemoveBlock(block, sector);
        network.UpdateBlockSectors();
      }
      else
      {
        Dictionary<PowerBlock, Vector3> powerBlocks = new Dictionary<PowerBlock, Vector3>();
        Dictionary<PowerBlock, Vector3> dictionary = new Dictionary<PowerBlock, Vector3>();
        HashSet<PowerBlock> powerBlockSet = new HashSet<PowerBlock>();
        Stack<Vector3> vector3Stack = new Stack<Vector3>();
        this.foundPowerNetworks.Clear();
        foreach (KeyValuePair<PowerBlock, Vector3> adjacentPowerBlock in this.adjacentPowerBlocks)
        {
          PowerBlock key = adjacentPowerBlock.Key;
          Vector3 vector3_1 = adjacentPowerBlock.Value;
          dictionary.Clear();
          if (!powerBlockSet.Contains(key))
          {
            vector3Stack.Push(vector3_1);
            while (vector3Stack.Count > 0)
            {
              Vector3 positionInEntityCoordinates = vector3Stack.Pop();
              BlockCell block1 = this.GetBlock(positionInEntityCoordinates);
              if (block1 != null && block1.Block is PowerBlock block2 && !powerBlockSet.Contains(block2))
              {
                powerBlockSet.Add(block2);
                dictionary.Add(block2, positionInEntityCoordinates);
                powerBlocks.Clear();
                this.GetAdjacentPowerBlocks(positionInEntityCoordinates - new Vector3((float) block1.X, (float) block1.Y, (float) block1.Z), block2.Size, powerBlocks);
                foreach (Vector3 vector3_2 in powerBlocks.Values)
                  vector3Stack.Push(vector3_2);
              }
            }
          }
          if (dictionary.Keys.Count > 0)
          {
            PowerNetwork powerNetwork = new PowerNetwork();
            this.powerNetworks.Add(powerNetwork);
            foreach (KeyValuePair<PowerBlock, Vector3> keyValuePair in dictionary)
            {
              BlockSector sector1 = this.GetSector(keyValuePair.Value);
              this.foundSectors.Add(sector1);
              powerNetwork.AddBlock(keyValuePair.Key, sector1);
            }
          }
        }
        this.powerNetworks.Remove(network);
        foreach (BlockSector foundSector in this.foundSectors)
          foundSector.UpdatePowerBlocks();
      }
    }

    public enum AddParameters
    {
      None,
      Normal,
      Fast,
    }
  }
}
