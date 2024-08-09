// Decompiled with JetBrains decompiler
// Type: CornerSpace.PowerNetwork
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace
{
  public class PowerNetwork
  {
    private Dictionary<BlockSector, int> networkInSectors;
    private HashSet<PowerBlock> powerBlocks;
    private int powerLevel;
    private bool updateRequired;

    public PowerNetwork()
    {
      this.powerBlocks = new HashSet<PowerBlock>();
      this.networkInSectors = new Dictionary<BlockSector, int>();
    }

    public bool AddBlock(PowerBlock block, BlockSector sector)
    {
      if (!this.powerBlocks.Add(block))
        return false;
      block.Network = this;
      this.powerLevel += block.Power;
      this.UpdateNetworkStructure();
      if (this.networkInSectors.ContainsKey(sector))
      {
        Dictionary<BlockSector, int> networkInSectors;
        BlockSector key;
        (networkInSectors = this.networkInSectors)[key = sector] = networkInSectors[key] + 1;
      }
      else
        this.networkInSectors.Add(sector, 1);
      return true;
    }

    public void Disconnect()
    {
      foreach (PowerBlock powerBlock in this.powerBlocks)
      {
        powerBlock.Deactivate();
        powerBlock.Network = (PowerNetwork) null;
      }
      this.powerLevel = 0;
      this.powerBlocks.Clear();
    }

    public void MergeWith(PowerNetwork network)
    {
      if (network == this || network == null)
        return;
      this.powerBlocks.UnionWith((IEnumerable<PowerBlock>) network.PowerBlocks);
      this.powerLevel += network.PowerLevel;
      foreach (PowerBlock powerBlock in network.PowerBlocks)
        powerBlock.Network = this;
      foreach (BlockSector key1 in network.networkInSectors.Keys)
      {
        if (this.networkInSectors.ContainsKey(key1))
        {
          Dictionary<BlockSector, int> networkInSectors;
          BlockSector key2;
          (networkInSectors = this.networkInSectors)[key2 = key1] = networkInSectors[key2] + 1;
        }
        else
          this.networkInSectors.Add(key1, 1);
      }
      this.UpdateNetworkStructure();
    }

    public void RemoveBlock(PowerBlock block, BlockSector sector)
    {
      if (!this.powerBlocks.Remove(block))
        return;
      this.powerLevel -= block.Power;
      block.Network = (PowerNetwork) null;
      this.UpdateNetworkStructure();
      if (!this.networkInSectors.ContainsKey(sector))
        return;
      Dictionary<BlockSector, int> networkInSectors;
      BlockSector key;
      (networkInSectors = this.networkInSectors)[key = sector] = networkInSectors[key] - 1;
      if (this.networkInSectors[sector] > 0)
        return;
      this.networkInSectors.Remove(sector);
    }

    public void UpdateBlockSectors()
    {
      foreach (BlockSector blockSector in this.BlockSectors)
        blockSector.UpdatePowerBlocks();
    }

    public void UpdateNetworkStructure()
    {
      if (this.powerLevel >= 0)
      {
        foreach (PowerBlock powerBlock in this.powerBlocks)
          powerBlock.Activate();
      }
      else
      {
        foreach (PowerBlock powerBlock in this.powerBlocks)
          powerBlock.Deactivate();
      }
    }

    public BlockSector[] BlockSectors => this.networkInSectors.Keys.ToArray<BlockSector>();

    public HashSet<PowerBlock> PowerBlocks => this.powerBlocks;

    public int PowerLevel => this.powerLevel;
  }
}
