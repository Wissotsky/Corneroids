// Decompiled with JetBrains decompiler
// Type: CornerSpace.GunControlBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class GunControlBlock : ControlBlock
  {
    public GunControlBlock(GunControlBlockType creationParameters)
      : base((ControlBlockType) creationParameters)
    {
    }

    public override void EditButtonClicked(Player player)
    {
      Astronaut astronaut = player.Astronaut;
      if (astronaut.ActiveItem != (Item) null && astronaut.ActiveItem is BindGunToControlTool && (astronaut.ActiveItem as BindGunToControlTool).ControlBlock == this)
      {
        Engine.LoadNewScreen((GameScreen) new ControlBlockKeygroupsScreen((ControlBlock) this, player));
      }
      else
      {
        astronaut.ActiveItem = (Item) new BindGunToControlTool(this, player);
        player.Toolbar.SelectedIndex = (sbyte) -1;
      }
    }

    public override void Update(InputFrame input, Astronaut updater)
    {
      if (updater == null)
        return;
      base.Update(input, updater);
      foreach (HashSet<TriggerBlock> triggerBlockSet in this.controlledTriggerBlocks.Values)
      {
        foreach (TriggerBlock triggerBlock in triggerBlockSet)
          triggerBlock.UserUpdate(updater.UsedCamera);
      }
    }
  }
}
