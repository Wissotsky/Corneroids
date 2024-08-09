// Decompiled with JetBrains decompiler
// Type: CornerSpace.BeaconTool
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Screen;
using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class BeaconTool : ToolItem
  {
    private const float cMaxPickDistance = 6f;
    private KeyValuePair<BeaconObject, float> pickedBeacon;
    private VertexPositionColorTexture[] boxVertices;
    private short[] boxIndices;
    private Effect effect;
    private VertexDeclaration vertexDeclaration;

    public BeaconTool()
    {
      Primitive.CreateSelectorBox(Vector3.One, out this.boxVertices, out this.boxIndices);
      this.effect = Engine.ShaderPool[2];
      this.vertexDeclaration = Engine.VertexDeclarationPool[3];
      this.effect.Parameters["SelectorTexture"].SetValue((Texture) Engine.ContentManager.Load<Texture2D>("Textures/Sprites/squareCorners"));
    }

    public override void DrawHelpTexts()
    {
      if (this.pickedBeacon.Key == null)
        this.DrawHelpString("Click to create a new beacon!");
      else
        this.DrawHelpString("Click to remove the beacon!");
    }

    public override void RenderFirstPerson(IRenderCamera camera)
    {
      if (this.pickedBeacon.Key == null || this.boxVertices == null && this.boxIndices == null)
        return;
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      this.effect.Parameters["World"].SetValue(Matrix.CreateTranslation(camera.GetPositionRelativeToCamera(this.pickedBeacon.Key.Position)));
      this.effect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      this.effect.Parameters["SelectorColor"].SetValue(Color.Red.ToVector4());
      Engine.GraphicsDevice.VertexDeclaration = this.vertexDeclaration;
      graphicsDevice.RenderState.CullMode = CullMode.None;
      graphicsDevice.RenderState.DepthBufferEnable = true;
      graphicsDevice.RenderState.DepthBufferWriteEnable = false;
      graphicsDevice.RenderState.DepthBufferFunction = CompareFunction.LessEqual;
      graphicsDevice.RenderState.AlphaBlendEnable = false;
      this.effect.Begin();
      foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
      {
        pass.Begin();
        graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, this.boxVertices, 0, this.boxVertices.Length, this.boxIndices, 0, this.boxIndices.Length / 3);
        pass.End();
      }
      this.effect.End();
    }

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      if (input.LeftClick)
      {
        if (this.pickedBeacon.Key != null)
          Item.entityManager.RemoveBeacon(this.pickedBeacon.Key);
        else
          Engine.LoadNewScreen((GameScreen) new BeaconScreen((Action<string>) (name =>
          {
            if (string.IsNullOrEmpty(name))
              return;
            BeaconObject beacon = new BeaconObject()
            {
              Position = owner.Astronaut.Position + owner.Astronaut.GetLookatVector() * 2f,
              Message = name
            };
            Item.entityManager.AddBeacon(beacon, true);
          })));
      }
      return Item.UsageResult.None;
    }

    public override void Update(Astronaut owner)
    {
      HashSet<BeaconObject> beacons = Item.entityManager.Beacons;
      this.pickedBeacon = new KeyValuePair<BeaconObject, float>((BeaconObject) null, float.MaxValue);
      foreach (BeaconObject key in beacons)
      {
        float num = (key.Position - owner.Position).Length();
        if ((double) num <= 6.0 && (double) num < (double) this.pickedBeacon.Value)
          this.pickedBeacon = new KeyValuePair<BeaconObject, float>(key, num);
      }
    }
  }
}
