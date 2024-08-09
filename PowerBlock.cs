// Decompiled with JetBrains decompiler
// Type: CornerSpace.PowerBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;

#nullable disable
namespace CornerSpace
{
  public class PowerBlock : Block
  {
    protected bool activated;
    protected PowerNetwork network;

    public PowerBlock(PowerBlockType creationParameters)
      : base((BlockType) creationParameters)
    {
      this.activated = false;
    }

    public override void EditButtonClicked(Player player)
    {
      Engine.LoadNewScreen((GameScreen) new PowerNetworkInformationScreen(this));
    }

    public virtual void Activate() => this.activated = true;

    public virtual void Deactivate() => this.activated = false;

    public override BlockTextureCoordinates GetTextureCoordinates()
    {
      return this.activated ? ((PowerBlockType) this.GetBlockType()).ActiveBlockTextures : ((PowerBlockType) this.GetBlockType()).DeactiveTexturePosition;
    }

    public override bool CanBeEdited => true;

    public bool Activated => this.activated;

    public int Power => ((PowerBlockType) this.GetBlockType()).Power;

    public PowerNetwork Network
    {
      get => this.network;
      set => this.network = value;
    }
  }
}
