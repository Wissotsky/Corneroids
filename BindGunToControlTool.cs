// Decompiled with JetBrains decompiler
// Type: CornerSpace.BindGunToControlTool
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;

#nullable disable
namespace CornerSpace
{
  public class BindGunToControlTool : BindTriggerToControlTool
  {
    private readonly string cHelpText = "Choose cannon blocks you wish to control!" + Environment.NewLine + Environment.NewLine + "Press Edit again to assign keys!";

    public BindGunToControlTool(GunControlBlock block, Player user)
      : base((ControlBlock) block)
    {
      this.ItemId = -2;
    }

    public BindGunToControlTool(ControlBlock block, Player user)
      : base(block)
    {
      this.ItemId = -2;
    }

    public override void DrawHelpTexts() => this.DrawHelpString(this.cHelpText);

    protected override bool IsPickedBlockValid(Block block) => block is GunBlock;
  }
}
