// Decompiled with JetBrains decompiler
// Type: CornerSpace.EnvironmentManagerServer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class EnvironmentManagerServer : EnvironmentManager
  {
    private NetworkServerManager server;

    public EnvironmentManagerServer(
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
      EnvironmentManager.FloatingItem<ItemSlot> floatingItem = base.AddFloatingItem(item, position, speed, pickupDelay);
      if (floatingItem != null)
        this.server.SendAddFloatingItem(item, floatingItem.Id, position, speed);
      return floatingItem;
    }

    public override void BindToServer(NetworkServerManager server)
    {
      this.server = server;
      this.server.RequestTossItem += (Action<ItemSlot, Position3, Vector3>) ((item, position, speed) =>
      {
        if (item == null)
          return;
        this.AddFloatingItem(item, position, speed, (ushort) 2000);
      });
    }

    protected override bool ItemsPickedByPlayer(
      Player player,
      EnvironmentManager.FloatingItem<ItemSlot> item)
    {
      bool flag = base.ItemsPickedByPlayer(player, item);
      if (flag)
        this.server.SendItemPickedByPlayer(player, item.Sprite, item.Id);
      return flag;
    }
  }
}
