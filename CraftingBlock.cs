// Decompiled with JetBrains decompiler
// Type: CornerSpace.CraftingBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class CraftingBlock : BasicBlock
  {
    public CraftingBlock(CraftingBlockType creationParameters)
      : base((BasicBlockType) creationParameters)
    {
    }

    public override void UseButtonClicked(Player player)
    {
      if (!(player is LocalPlayer))
        return;
      List<Blueprint> blueprintsPool = (List<Blueprint>) null;
      string caption = (string) null;
      Itemset itemset = Engine.LoadedWorld.Itemset;
      CraftingBlockType blockType = this.GetBlockType() as CraftingBlockType;
      if (!((Item) blockType != (Item) null))
        return;
      if (blockType.UsageType == CraftingBlockType.Type.CraftingTable)
      {
        List<Blueprint> blueprintsOfType = itemset.Crafting.GetBlueprintsOfType(Blueprint.Usage.Craft);
        Engine.LoadNewScreen((GameScreen) new CraftingScreen(player.Inventory, blueprintsOfType));
      }
      else
      {
        if (blockType.UsageType == CraftingBlockType.Type.Smelter)
        {
          blueprintsPool = itemset.Crafting.GetBlueprintsOfType(Blueprint.Usage.Smelter);
          caption = "Ore furnace";
        }
        else if (blockType.UsageType == CraftingBlockType.Type.Extractor)
        {
          blueprintsPool = itemset.Crafting.GetBlueprintsOfType(Blueprint.Usage.Extractor);
          caption = "Mineral extractor";
        }
        Engine.LoadNewScreen((GameScreen) new BlueprintsScreen(player.Inventory, blueprintsPool, caption));
      }
    }

    public override bool CanBeUsed => true;
  }
}
