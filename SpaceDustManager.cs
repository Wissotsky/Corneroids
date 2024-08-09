// Decompiled with JetBrains decompiler
// Type: CornerSpace.SpaceDustManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace
{
  public class SpaceDustManager : IDisposable
  {
    private readonly Vector3 cCoveredArea;
    private readonly Vector2 cDustSize;
    private readonly Vector4 cDustUV;
    private BillboardBatch billboardBatch;
    private Position3[] dustPositions;
    private Texture2D dustTexture;

    public SpaceDustManager()
    {
      this.cCoveredArea = Vector3.One * 256f;
      this.cDustSize = Vector2.One * 0.5f;
      this.cDustUV = new Vector4(0.0f, 0.0f, 1f, 1f);
      this.dustTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/dust");
    }

    public void Dispose()
    {
      if (this.billboardBatch == null)
        return;
      this.billboardBatch.Dispose();
    }

    public void InitializeDust(int particleCount)
    {
      particleCount = Math.Min(512, Math.Max(0, particleCount));
      if (this.billboardBatch == null)
        this.billboardBatch = new BillboardBatch(particleCount);
      else if (this.billboardBatch.MaxBillboardsPerBatch < particleCount)
      {
        this.billboardBatch.Dispose();
        this.billboardBatch = new BillboardBatch(particleCount);
      }
      if (particleCount <= 0)
        return;
      this.dustPositions = new Position3[particleCount];
      Random random = new Random();
      for (int index = 0; index < particleCount; ++index)
        this.dustPositions[index] = new Position3(new Vector3((float) random.NextDouble() * this.cCoveredArea.X, (float) random.NextDouble() * this.cCoveredArea.Y, (float) random.NextDouble() * this.cCoveredArea.Z));
    }

    public void Render(IRenderCamera camera)
    {
      if (this.dustPositions == null || this.dustTexture == null || camera == null || this.billboardBatch == null)
        return;
      Color color = new Color(0.4f, 0.4f, 0.4f, 1f);
      this.billboardBatch.Begin();
      for (int index = 0; index < this.dustPositions.Length; ++index)
      {
        if ((double) (this.dustPositions[index] - camera.Position).LengthSquared() > 256.0)
          this.billboardBatch.DrawBillboard(this.dustTexture, camera.GetPositionRelativeToCamera(this.dustPositions[index]), this.cDustUV, this.cDustSize, color);
      }
      this.billboardBatch.End(camera);
    }

    public void Update(Position3 position)
    {
      if (this.dustPositions == null || Engine.FrameCounter.FrameNumber % 15 != 0)
        return;
      Position3 position3_1 = new Position3(this.cCoveredArea * 0.5f);
      Position3 position3_2 = new Position3(position - position3_1);
      Position3 position3_3 = position + position3_1;
      for (int index = 0; index < this.dustPositions.Length; ++index)
      {
        if (this.dustPositions[index].X < position3_2.X)
          this.dustPositions[index].X = position3_3.X - (position3_2.X - this.dustPositions[index].X) % (2L * position3_1.X);
        else if (this.dustPositions[index].X > position3_3.X)
          this.dustPositions[index].X = position3_2.X + (this.dustPositions[index].X - position3_3.X) % (2L * position3_1.X);
        if (this.dustPositions[index].Y < position3_2.Y)
          this.dustPositions[index].Y = position3_3.Y - (position3_2.Y - this.dustPositions[index].Y) % (2L * position3_1.Y);
        else if (this.dustPositions[index].Y > position3_3.Y)
          this.dustPositions[index].Y = position3_2.Y + (this.dustPositions[index].Y - position3_3.Y) % (2L * position3_1.Y);
        if (this.dustPositions[index].Z < position3_2.Z)
          this.dustPositions[index].Z = position3_3.Z - (position3_2.Z - this.dustPositions[index].Z) % (2L * position3_1.Z);
        else if (this.dustPositions[index].Z > position3_3.Z)
          this.dustPositions[index].Z = position3_2.Z + (this.dustPositions[index].Z - position3_3.Z) % (2L * position3_1.Z);
      }
    }
  }
}
