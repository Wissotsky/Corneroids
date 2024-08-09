// Decompiled with JetBrains decompiler
// Type: CornerSpace.CraftingManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace CornerSpace
{
  public class CraftingManager
  {
    private List<Blueprint> blueprints;

    public CraftingManager() => this.blueprints = new List<Blueprint>();

    public void AddNewBlueprint(Blueprint blueprint)
    {
      if (this.blueprints.Contains(blueprint))
        return;
      this.blueprints.Add(blueprint);
    }

    public List<Blueprint> GetBlueprintResult(ItemSlot[] ingredients)
    {
      return this.blueprints.FindAll((Predicate<Blueprint>) (b => b != null && b.GetResultMultiplier(ingredients) > 0));
    }

    public List<Blueprint> GetBlueprintsOfType(Blueprint.Usage usage)
    {
      return this.blueprints.Where<Blueprint>((Func<Blueprint, bool>) (b => b.UsageType == usage)).ToList<Blueprint>();
    }

    public void RemoveBlueprint(Blueprint blueprint) => this.blueprints.Remove(blueprint);

    public List<Blueprint> Blueprints => this.blueprints;
  }
}
