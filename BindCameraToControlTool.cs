// Decompiled with JetBrains decompiler
// Type: CornerSpace.BindCameraToControlTool
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public class BindCameraToControlTool : BindTriggerToControlTool
  {
    private readonly string cHelpText = "Choose a camera block you wish to bind to the console!";
    private ControlBlock controlBlock;
    private Player user;

    public BindCameraToControlTool(ControlBlock controlBlock, Player user)
      : base(controlBlock)
    {
      this.controlBlock = controlBlock;
      this.user = user;
      this.UseSelectionBox = true;
      this.CreateColorList(new Color[1]{ Color.Magenta });
      this.SelectorTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/camera");
      this.Crosshair = new SpriteImage(Engine.ContentManager.Load<Texture2D>("Textures/Sprites/Crosshairs/bindCamera"));
    }

    public override void DrawHelpTexts() => this.DrawHelpString(this.cHelpText);

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      if (input.LeftClick && this.pickedBlockCell != null && this.pickedBlockCell.Block is CameraBlock block)
      {
        if (this.controlBlock.CameraBlock == block)
        {
          this.controlBlock.UnbindCamera(true);
        }
        else
        {
          this.controlBlock.UnbindCamera(true);
          this.controlBlock.BindCamera(block, true);
        }
        this.CreateVertexAndIndexStructure();
      }
      return Item.UsageResult.None;
    }

    protected override SpriteImage CreateCrosshair()
    {
      return new SpriteImage(Engine.ContentManager.Load<Texture2D>("Textures/Sprites/Crosshairs/bindCamera"));
    }

    protected override bool IsPickedBlockValid(Block block) => block is CameraBlock;
  }
}
