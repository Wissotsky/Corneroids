// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.EditLightBlockScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace.Screen
{
  public class EditLightBlockScreen : MenuScreen
  {
    private ButtonLayer buttonOk;
    private ButtonLayer buttonCancel;
    private LightBlock lightBlock;
    private EditLightBlockLayer lightBlockLayer;
    private int sequenceValue;

    public EditLightBlockScreen(LightBlock block)
      : base(GameScreen.Type.Popup, false, true, false)
    {
      this.lightBlock = block;
      this.sequenceValue = 0;
    }

    public override void Load()
    {
      this.lightBlockLayer = new EditLightBlockLayer();
      this.buttonOk = new ButtonLayer(new Rectangle(this.lightBlockLayer.Location.X + (int) this.lightBlockLayer.Size.X / 2 - 80 - 10, this.lightBlockLayer.Location.Y + (int) this.lightBlockLayer.Size.Y - 50, 80, 30), "Ok");
      this.buttonCancel = new ButtonLayer(new Rectangle(this.lightBlockLayer.Location.X + (int) this.lightBlockLayer.Size.X / 2 + 10, this.lightBlockLayer.Location.Y + (int) this.lightBlockLayer.Size.Y - 50, 80, 30), "Cancel");
      this.lightBlockLayer.PulseSequence = this.lightBlock.LightSequence;
    }

    public override void Render()
    {
      base.Render();
      this.lightBlockLayer.Render();
      this.buttonOk.Render();
      this.buttonCancel.Render();
    }

    public override void Update()
    {
      this.sequenceValue = (this.sequenceValue + (int) Engine.FrameCounter.DeltaTimeMS) % 2000;
      this.lightBlockLayer.SampleTime = this.sequenceValue;
    }

    public override void UpdateInput()
    {
      base.UpdateInput();
      this.buttonCancel.UpdateInput(this.Mouse);
      this.buttonOk.UpdateInput(this.Mouse);
      if (!this.Mouse.LeftClick())
        return;
      if (this.buttonOk.Contains(this.Mouse.Position))
      {
        this.lightBlock.LightSequence = this.lightBlockLayer.PulseSequence;
        this.CloseScreen();
      }
      else if (this.buttonCancel.Contains(this.Mouse.Position))
      {
        this.CloseScreen();
      }
      else
      {
        Layer clickedLayer = this.lightBlockLayer.GetClickedLayer(this.Mouse.Position);
        if (clickedLayer == null || !(clickedLayer.Name == "sequenceLayer"))
          return;
        this.lightBlockLayer.PulseSequence ^= (byte) (128U >> (int) (byte) ((double) (this.Mouse.Position.X - clickedLayer.Location.X) / ((double) clickedLayer.Size.X / 8.0)));
      }
    }
  }
}
