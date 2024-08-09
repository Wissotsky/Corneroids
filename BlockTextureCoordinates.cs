// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockTextureCoordinates
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class BlockTextureCoordinates
  {
    private Byte4 topCoords;
    private Byte4 sideCoords;
    private Byte4 bottomCoords;
    private Vector4 topCoordsUV;
    private Vector4 sideCoordsUV;
    private Vector4 bottomCoordsUV;

    public BlockTextureCoordinates(Rectangle walls, Rectangle bottom, Rectangle top)
    {
      this.topCoords = new Byte4()
      {
        X = (byte) top.X,
        Y = (byte) top.Y,
        Z = (byte) top.Width,
        W = (byte) top.Height
      };
      this.sideCoords = new Byte4()
      {
        X = (byte) walls.X,
        Y = (byte) walls.Y,
        Z = (byte) walls.Width,
        W = (byte) walls.Height
      };
      this.bottomCoords = new Byte4()
      {
        X = (byte) bottom.X,
        Y = (byte) bottom.Y,
        Z = (byte) bottom.Width,
        W = (byte) bottom.Height
      };
      this.topCoordsUV = new Vector4((float) top.X, (float) top.Y, (float) top.Width, (float) top.Height) / 16f;
      this.sideCoordsUV = new Vector4((float) walls.X, (float) walls.Y, (float) walls.Width, (float) walls.Height) / 16f;
      this.bottomCoordsUV = new Vector4((float) bottom.X, (float) bottom.Y, (float) bottom.Width, (float) bottom.Height) / 16f;
    }

    public Byte4 TopCoordinates => this.topCoords;

    public Byte4 WallCoordinates => this.sideCoords;

    public Byte4 BottomCoordinates => this.bottomCoords;

    public Vector4 TopCoordinatesUV => this.topCoordsUV;

    public Vector4 WallCoordinatesUV => this.sideCoordsUV;

    public Vector4 BottomCoordinatesUV => this.bottomCoordsUV;
  }
}
