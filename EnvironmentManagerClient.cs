// Decompiled with JetBrains decompiler
// Type: CornerSpace.EnvironmentManagerClient
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class EnvironmentManagerClient : EnvironmentManager
  {
    private NetworkClientManager client;

    public EnvironmentManagerClient(
      SpaceEntityManager entityManager,
      LightingManager lightingManager)
      : base(entityManager, lightingManager)
    {
    }

    public override EnvironmentManager.FloatingItem<ItemSlot> AddFloatingItem(
      ItemSlot item,
      Position3 position,
      Vector3 speed,
      ushort pickupDelay)
    {
      return (EnvironmentManager.FloatingItem<ItemSlot>) null;
    }

    public override void BindToClient(NetworkClientManager client)
    {
      this.client = client;
      this.client.NewItemSpawned += (Action<ItemSlot, Position3, Vector3, int>) ((item, position, speed, envId) =>
      {
        if (item == null)
          return;
        this.AddFloatingItem(item, position, speed, envId, ushort.MaxValue);
      });
      this.client.ItemPickedUp += (Action<Player, ItemSlot, int>) ((player, item, envId) =>
      {
        if (player == null || item == null)
          return;
        EnvironmentManager.FloatingItem<ItemSlot> floatingItem = this.floatingItems.Find((Predicate<EnvironmentManager.FloatingItem<ItemSlot>>) (i => i.Id == envId));
        if (floatingItem == null)
          return;
        this.floatingItems.Remove(floatingItem);
      });
    }

    public override bool TossItems(Character tosser, ItemSlot item)
    {
      if (tosser != null)
        this.client.RequestTossItem(item, tosser.Position, tosser.GetLookatVector() * 3f);
      return true;
    }

    protected override bool ItemsPickedByPlayer(
      Player player,
      EnvironmentManager.FloatingItem<ItemSlot> item)
    {
      return false;
    }
  }
}
