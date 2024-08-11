// Decompiled with JetBrains decompiler
// Type: CornerSpace.LandscapeModel
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace
{
  public class LandscapeModel
  {
    private Model model;
    private Quaternion orientation;

    public LandscapeModel(Model model)
    {
      this.model = model;
      this.orientation = Quaternion.Identity;
    }

    public void Render(IRenderCamera camera)
    {
        GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.BlendState = BlendState.Opaque;

        foreach (ModelMesh mesh in this.Model.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                // Set effect parameters here
                throw new NotImplementedException();
            }
            mesh.Draw();
        }
    }

        protected Model Model
    {
      get => this.model;
      set => this.model = value;
    }

    public Quaternion Orientation
    {
      get => this.orientation;
      set => this.orientation = value;
    }
  }
}
