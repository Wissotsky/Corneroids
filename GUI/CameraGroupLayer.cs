// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.CameraGroupLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace.GUI
{
  public class CameraGroupLayer : Layer
  {
    private bool cameraSet;

    public CameraGroupLayer(Rectangle position)
      : base(position)
    {
    }

    public override void Render()
    {
      Engine.SpriteBatch.Begin();
      Engine.SpriteBatch.DrawString(Engine.Font, this.cameraSet ? "Camera set" : "Camera not set", this.Position, Color.White);
      Engine.SpriteBatch.End();
    }

    public bool CameraSet
    {
      get => this.cameraSet;
      set => this.cameraSet = value;
    }
  }
}
