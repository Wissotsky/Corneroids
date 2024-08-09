// Decompiled with JetBrains decompiler
// Type: CornerSpace.Blueprint
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class Blueprint
  {
    private Dictionary<Item, int> ingredients;
    private string name;
    private ItemSlot result;
    private Blueprint.Usage usage;

    public Blueprint(string name, Blueprint.Usage usage)
    {
      this.ingredients = new Dictionary<Item, int>();
      this.name = name ?? string.Empty;
      this.result = (ItemSlot) null;
      this.usage = usage;
    }

    public void AddNewIngredient(Item item, int quantity)
    {
      if (this.ingredients.ContainsKey(item))
      {
        Dictionary<Item, int> ingredients;
        Item key;
        (ingredients = this.ingredients)[key = item] = ingredients[key] + quantity;
      }
      else
        this.ingredients.Add(item, quantity);
    }

    public int GetResultMultiplier(ItemSlot[] providedMaterials)
    {
      int num = 0;
      int val1 = int.MaxValue;
      using (Dictionary<Item, int>.Enumerator enumerator = this.ingredients.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<Item, int> ingredient = enumerator.Current;
          ItemSlot[] all = Array.FindAll<ItemSlot>(providedMaterials, (Predicate<ItemSlot>) (m => m.Item == ingredient.Key));
          int foundCount = 0;
          Array.ForEach<ItemSlot>(all, (Action<ItemSlot>) (i => foundCount += i.Count));
          int val2 = foundCount / ingredient.Value;
          if (val2 == 0)
            return 0;
          if (val2 >= 1)
          {
            val1 = Math.Min(val1, val2);
            ++num;
          }
        }
      }
      return num == this.ingredients.Count ? val1 : 0;
    }

    public void SetResultItem(Item item, int quantity)
    {
      this.result = new ItemSlot(item, quantity);
    }

    public Dictionary<Item, int> Ingredients => this.ingredients;

    public string Name => this.name;

    public ItemSlot Result => this.result;

    public Blueprint.Usage UsageType => this.usage;

    public enum Usage
    {
      Craft,
      Extractor,
      Smelter,
    }
  }
}
