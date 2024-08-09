// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockBuilder
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class BlockBuilder
  {
    public const byte cVertexUnitSize = 8;
    private const byte XMINUS = 0;
    private const byte XPLUS = 4;
    private const byte YMINUS = 8;
    private const byte YPLUS = 12;
    private const byte ZMINUS = 16;
    private const byte ZPLUS = 20;
    private static BlockVertex[] premadeBlockVertices;
    private static DynamicBlockVertex[] premadeDynamicBlockVertices;
    private static VertexPositionTexture[] premadePositionTextures;
    private static short[] premadeIndices;
    private static BlockBuilder instance;

    private BlockBuilder() => this.InitializePremadeVariables();

    public void ConstructBlock(out BlockVertex[] vertices, out short[] indices, Byte3 size)
    {
      vertices = new BlockVertex[24];
      indices = new short[36];
      Array.Copy((Array) BlockBuilder.premadeBlockVertices, (Array) vertices, vertices.Length);
      Array.Copy((Array) BlockBuilder.premadeIndices, (Array) indices, indices.Length);
      Byte3 scale = new Byte3((byte) ((uint) size.X * 8U), (byte) ((uint) size.Y * 8U), (byte) ((uint) size.Z * 8U));
      for (int index = 0; index < vertices.Length; ++index)
        vertices[index].ScalePosition(scale);
    }

    public void ConstructBlock(
      Block block,
      Byte3 minPosition,
      out BlockVertex[] vertices,
      out short[] indices)
    {
      byte orientation = block.Orientation;
      byte visibility = block.Visibility;
      Byte3 modelSize = block.ModelSize;
      Byte4 wallCoordinates = block.GetTextureCoordinates().WallCoordinates;
      Byte4 bottomCoordinates = block.GetTextureCoordinates().BottomCoordinates;
      Byte4 topCoordinates = block.GetTextureCoordinates().TopCoordinates;
      byte num1 = (byte) ((uint) orientation & 3U);
      byte num2 = (byte) ((int) orientation >> 2 & 3);
      byte num3 = (byte) ((int) orientation >> 4 & 3);
      BlockBuilder.BlockFace f2_1 = new BlockBuilder.BlockFace(new Byte2(wallCoordinates.X, wallCoordinates.Y), new Byte2((byte) ((uint) wallCoordinates.X + (uint) wallCoordinates.Z), wallCoordinates.Y), new Byte2((byte) ((uint) wallCoordinates.X + (uint) wallCoordinates.Z), (byte) ((uint) wallCoordinates.Y + (uint) wallCoordinates.W)), new Byte2(wallCoordinates.X, (byte) ((uint) wallCoordinates.Y + (uint) wallCoordinates.W)));
      BlockBuilder.BlockFace blockFace1 = f2_1;
      BlockBuilder.BlockFace blockFace2 = f2_1;
      BlockBuilder.BlockFace f2_2 = f2_1;
      BlockBuilder.BlockFace uv1 = new BlockBuilder.BlockFace(new Byte2(bottomCoordinates.X, bottomCoordinates.Y), new Byte2((byte) ((uint) bottomCoordinates.X + (uint) bottomCoordinates.Z), bottomCoordinates.Y), new Byte2((byte) ((uint) bottomCoordinates.X + (uint) bottomCoordinates.Z), (byte) ((uint) bottomCoordinates.Y + (uint) bottomCoordinates.W)), new Byte2(bottomCoordinates.X, (byte) ((uint) bottomCoordinates.Y + (uint) bottomCoordinates.W)));
      BlockBuilder.BlockFace uv2 = new BlockBuilder.BlockFace(new Byte2(topCoordinates.X, topCoordinates.Y), new Byte2((byte) ((uint) topCoordinates.X + (uint) topCoordinates.Z), topCoordinates.Y), new Byte2((byte) ((uint) topCoordinates.X + (uint) topCoordinates.Z), (byte) ((uint) topCoordinates.Y + (uint) topCoordinates.W)), new Byte2(topCoordinates.X, (byte) ((uint) topCoordinates.Y + (uint) topCoordinates.W)));
      BlockBuilder.BlockFace blockFace3;
      for (byte index = 0; (int) index < (int) num3; ++index)
      {
        blockFace2.RotateCW();
        f2_2.RotateCCW();
        BlockBuilder.BlockFace f1_1 = uv2;
        uv2 = this.SwapF1toF2(blockFace1, uv2, (byte) 1);
        BlockBuilder.BlockFace f1_2 = f2_1;
        f2_1 = this.SwapF1toF2(f1_1, f2_1, (byte) 1);
        BlockBuilder.BlockFace f1_3 = uv1;
        uv1 = this.SwapF1toF2(f1_2, uv1, (byte) 1);
        blockFace3 = blockFace1;
        blockFace1 = this.SwapF1toF2(f1_3, blockFace1, (byte) 1);
      }
      for (byte index = 0; (int) index < (int) num2; ++index)
      {
        f2_1.RotateCW();
        blockFace1.RotateCCW();
        BlockBuilder.BlockFace f1_4 = uv2;
        uv2 = this.SwapF1toF2(blockFace2, uv2, (byte) 0);
        BlockBuilder.BlockFace f1_5 = f2_2;
        f2_2 = this.SwapF1toF2(f1_4, f2_2, (byte) 2);
        BlockBuilder.BlockFace f1_6 = uv1;
        uv1 = this.SwapF1toF2(f1_5, uv1, (byte) 2);
        blockFace3 = blockFace2;
        blockFace2 = this.SwapF1toF2(f1_6, blockFace2, (byte) 0);
      }
      for (byte index = 0; (int) index < (int) num1; ++index)
      {
        uv1.RotateCW();
        uv2.RotateCCW();
        this.SwapFaces(ref f2_1, ref blockFace2, ref blockFace1, ref f2_2);
      }
      byte num4 = this.NumberOfVisibleFaces(visibility);
      vertices = new BlockVertex[(int) num4 * 4];
      indices = new short[(int) num4 * 6];
      byte num5 = 0;
      if (((int) visibility & 1) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 0, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref f2_1);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 2) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 4, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref blockFace1);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 4) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 8, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref uv1);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 8) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 12, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref uv2);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 16) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 16, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref blockFace2);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 32) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 20, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref f2_2);
        BlockVertex[] vertices1 = vertices;
        int num6 = (int) num5;
        byte num7 = (byte) (num6 + 1);
        int vIndex = (int) (byte) (4 * num6);
        Byte3 scale = modelSize;
        this.ScaleFace(vertices1, (byte) vIndex, scale);
      }
      minPosition = new Byte3((byte) ((uint) minPosition.X * 8U), (byte) ((uint) minPosition.Y * 8U), (byte) ((uint) minPosition.Z * 8U));
      Byte3 scale1 = new Byte3((byte) 8, (byte) 8, (byte) 8);
      for (int index = 0; index < vertices.Length; ++index)
      {
        vertices[index].ScalePosition(scale1);
        vertices[index].Position += minPosition;
      }
      Array.Copy((Array) BlockBuilder.premadeIndices, (Array) indices, indices.Length);
    }

    public void ConstructBlock(
      Block block,
      Byte3 minPosition,
      Byte3 sizeModifier,
      Byte3 positionModifier,
      out BlockVertex[] vertices,
      out short[] indices)
    {
      byte orientation = block.Orientation;
      byte visibility = block.Visibility;
      Byte3 modelSize = block.ModelSize;
      Byte4 wallCoordinates = block.GetTextureCoordinates().WallCoordinates;
      Byte4 bottomCoordinates = block.GetTextureCoordinates().BottomCoordinates;
      Byte4 topCoordinates = block.GetTextureCoordinates().TopCoordinates;
      byte num1 = (byte) ((uint) orientation & 3U);
      byte num2 = (byte) ((int) orientation >> 2 & 3);
      byte num3 = (byte) ((int) orientation >> 4 & 3);
      BlockBuilder.BlockFace f2_1 = new BlockBuilder.BlockFace(new Byte2(wallCoordinates.X, wallCoordinates.Y), new Byte2((byte) ((uint) wallCoordinates.X + (uint) wallCoordinates.Z), wallCoordinates.Y), new Byte2((byte) ((uint) wallCoordinates.X + (uint) wallCoordinates.Z), (byte) ((uint) wallCoordinates.Y + (uint) wallCoordinates.W)), new Byte2(wallCoordinates.X, (byte) ((uint) wallCoordinates.Y + (uint) wallCoordinates.W)));
      BlockBuilder.BlockFace blockFace1 = f2_1;
      BlockBuilder.BlockFace blockFace2 = f2_1;
      BlockBuilder.BlockFace f2_2 = f2_1;
      BlockBuilder.BlockFace uv1 = new BlockBuilder.BlockFace(new Byte2(bottomCoordinates.X, bottomCoordinates.Y), new Byte2((byte) ((uint) bottomCoordinates.X + (uint) bottomCoordinates.Z), bottomCoordinates.Y), new Byte2((byte) ((uint) bottomCoordinates.X + (uint) bottomCoordinates.Z), (byte) ((uint) bottomCoordinates.Y + (uint) bottomCoordinates.W)), new Byte2(bottomCoordinates.X, (byte) ((uint) bottomCoordinates.Y + (uint) bottomCoordinates.W)));
      BlockBuilder.BlockFace uv2 = new BlockBuilder.BlockFace(new Byte2(topCoordinates.X, topCoordinates.Y), new Byte2((byte) ((uint) topCoordinates.X + (uint) topCoordinates.Z), topCoordinates.Y), new Byte2((byte) ((uint) topCoordinates.X + (uint) topCoordinates.Z), (byte) ((uint) topCoordinates.Y + (uint) topCoordinates.W)), new Byte2(topCoordinates.X, (byte) ((uint) topCoordinates.Y + (uint) topCoordinates.W)));
      BlockBuilder.BlockFace blockFace3;
      for (byte index = 0; (int) index < (int) num3; ++index)
      {
        blockFace2.RotateCW();
        f2_2.RotateCCW();
        BlockBuilder.BlockFace f1_1 = uv2;
        uv2 = this.SwapF1toF2(blockFace1, uv2, (byte) 1);
        BlockBuilder.BlockFace f1_2 = f2_1;
        f2_1 = this.SwapF1toF2(f1_1, f2_1, (byte) 1);
        BlockBuilder.BlockFace f1_3 = uv1;
        uv1 = this.SwapF1toF2(f1_2, uv1, (byte) 1);
        blockFace3 = blockFace1;
        blockFace1 = this.SwapF1toF2(f1_3, blockFace1, (byte) 1);
      }
      for (byte index = 0; (int) index < (int) num2; ++index)
      {
        f2_1.RotateCW();
        blockFace1.RotateCCW();
        BlockBuilder.BlockFace f1_4 = uv2;
        uv2 = this.SwapF1toF2(blockFace2, uv2, (byte) 0);
        BlockBuilder.BlockFace f1_5 = f2_2;
        f2_2 = this.SwapF1toF2(f1_4, f2_2, (byte) 2);
        BlockBuilder.BlockFace f1_6 = uv1;
        uv1 = this.SwapF1toF2(f1_5, uv1, (byte) 2);
        blockFace3 = blockFace2;
        blockFace2 = this.SwapF1toF2(f1_6, blockFace2, (byte) 0);
      }
      for (byte index = 0; (int) index < (int) num1; ++index)
      {
        uv1.RotateCW();
        uv2.RotateCCW();
        this.SwapFaces(ref f2_1, ref blockFace2, ref blockFace1, ref f2_2);
      }
      byte num4 = this.NumberOfVisibleFaces(visibility);
      vertices = new BlockVertex[(int) num4 * 4];
      indices = new short[(int) num4 * 6];
      byte num5 = 0;
      if (((int) visibility & 1) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 0, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref f2_1);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 2) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 4, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref blockFace1);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 4) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 8, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref uv1);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 8) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 12, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref uv2);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 16) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 16, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref blockFace2);
        this.ScaleFace(vertices, (byte) (4 * (int) num5++), modelSize);
      }
      if (((int) visibility & 32) > 0)
      {
        Array.Copy((Array) BlockBuilder.premadeBlockVertices, 20, (Array) vertices, 4 * (int) num5, 4);
        this.SetTextureToFace(vertices, (byte) (4U * (uint) num5), ref f2_2);
        BlockVertex[] vertices1 = vertices;
        int num6 = (int) num5;
        byte num7 = (byte) (num6 + 1);
        int vIndex = (int) (byte) (4 * num6);
        Byte3 scale = modelSize;
        this.ScaleFace(vertices1, (byte) vIndex, scale);
      }
      minPosition = new Byte3((byte) ((int) minPosition.X * 8 + (int) positionModifier.X - (int) sizeModifier.X / 2), (byte) ((int) minPosition.Y * 8 + (int) positionModifier.Y - (int) sizeModifier.Y / 2), (byte) ((int) minPosition.Z * 8 + (int) positionModifier.Z - (int) sizeModifier.Z / 2));
      for (int index = 0; index < vertices.Length; ++index)
      {
        vertices[index].ScalePosition(sizeModifier);
        vertices[index].Position += minPosition;
      }
      Array.Copy((Array) BlockBuilder.premadeIndices, (Array) indices, indices.Length);
    }

    public void ConstructBlock(
      Block block,
      Vector3 position,
      out VertexPositionTexture[] vertices,
      out short[] indices)
    {
      vertices = new VertexPositionTexture[24];
      indices = new short[36];
      Vector4 wallCoordinatesUv = block.GetTextureCoordinates().WallCoordinatesUV;
      Vector4 bottomCoordinatesUv = block.GetTextureCoordinates().BottomCoordinatesUV;
      Vector4 topCoordinatesUv = block.GetTextureCoordinates().TopCoordinatesUV;
      Array.Copy((Array) BlockBuilder.premadePositionTextures, (Array) vertices, 24);
      Array.Copy((Array) BlockBuilder.premadeIndices, (Array) indices, 36);
      this.SetTextureToFace(vertices, (byte) 0, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 4, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 16, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 20, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 8, ref bottomCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 12, ref topCoordinatesUv);
      Matrix scale = Matrix.CreateScale((Vector3) block.GetBlockType().ModelSize);
      for (int index = 0; index < vertices.Length; ++index)
        vertices[index].Position = Vector3.Transform(vertices[index].Position, scale);
      if (!(position != Vector3.Zero))
        return;
      for (int index = 0; index < vertices.Length; ++index)
        vertices[index].Position += position;
    }

    public void ConstructBlock(
      Block block,
      Vector3 position,
      byte index,
      out DynamicBlockVertex[] vertices,
      out short[] indices)
    {
      vertices = new DynamicBlockVertex[24];
      indices = new short[36];
      Vector4 wallCoordinatesUv = block.GetTextureCoordinates().WallCoordinatesUV;
      Vector4 bottomCoordinatesUv = block.GetTextureCoordinates().BottomCoordinatesUV;
      Vector4 topCoordinatesUv = block.GetTextureCoordinates().TopCoordinatesUV;
      Vector3 modelPlacement = block.GetBlockType().ModelPlacement;
      Array.Copy((Array) BlockBuilder.premadeDynamicBlockVertices, (Array) vertices, 24);
      Array.Copy((Array) BlockBuilder.premadeIndices, (Array) indices, 36);
      this.SetTextureToFace(vertices, (byte) 0, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 4, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 16, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 20, ref wallCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 8, ref bottomCoordinatesUv);
      this.SetTextureToFace(vertices, (byte) 12, ref topCoordinatesUv);
      Matrix matrix = Matrix.CreateScale((Vector3) block.GetBlockType().ModelSize) * Matrix.CreateTranslation(modelPlacement) * block.OrientationMatrix;
      for (int index1 = 0; index1 < vertices.Length; ++index1)
      {
        vertices[index1].Position = Vector3.Transform(vertices[index1].Position, matrix);
        vertices[index1].Normal = Vector3.TransformNormal(vertices[index1].Normal, matrix);
      }
      for (int index2 = 0; index2 < vertices.Length; ++index2)
      {
        vertices[index2].Index = index;
        vertices[index2].Position += position;
      }
    }

    public static BlockBuilder Factory
    {
      get
      {
        if (BlockBuilder.instance == null)
          BlockBuilder.instance = new BlockBuilder();
        return BlockBuilder.instance;
      }
    }

    private void InitializePremadeVariables()
    {
      Vector3 vector3_1 = new Vector3(-0.5f, -0.5f, -0.5f);
      Vector3 vector3_2 = new Vector3(0.5f, -0.5f, -0.5f);
      Vector3 vector3_3 = new Vector3(0.5f, -0.5f, 0.5f);
      Vector3 vector3_4 = new Vector3(-0.5f, -0.5f, 0.5f);
      Vector3 vector3_5 = new Vector3(-0.5f, 0.5f, -0.5f);
      Vector3 vector3_6 = new Vector3(0.5f, 0.5f, -0.5f);
      Vector3 vector3_7 = new Vector3(0.5f, 0.5f, 0.5f);
      Vector3 vector3_8 = new Vector3(-0.5f, 0.5f, 0.5f);
      if (BlockBuilder.premadePositionTextures == null)
      {
        BlockBuilder.premadePositionTextures = new VertexPositionTexture[24];
        BlockBuilder.premadePositionTextures[0] = new VertexPositionTexture(vector3_5, Vector2.Zero);
        BlockBuilder.premadePositionTextures[1] = new VertexPositionTexture(vector3_8, Vector2.Zero);
        BlockBuilder.premadePositionTextures[2] = new VertexPositionTexture(vector3_4, Vector2.Zero);
        BlockBuilder.premadePositionTextures[3] = new VertexPositionTexture(vector3_1, Vector2.Zero);
        BlockBuilder.premadePositionTextures[4] = new VertexPositionTexture(vector3_7, Vector2.Zero);
        BlockBuilder.premadePositionTextures[5] = new VertexPositionTexture(vector3_6, Vector2.Zero);
        BlockBuilder.premadePositionTextures[6] = new VertexPositionTexture(vector3_2, Vector2.Zero);
        BlockBuilder.premadePositionTextures[7] = new VertexPositionTexture(vector3_3, Vector2.Zero);
        BlockBuilder.premadePositionTextures[8] = new VertexPositionTexture(vector3_4, Vector2.Zero);
        BlockBuilder.premadePositionTextures[9] = new VertexPositionTexture(vector3_3, Vector2.Zero);
        BlockBuilder.premadePositionTextures[10] = new VertexPositionTexture(vector3_2, Vector2.Zero);
        BlockBuilder.premadePositionTextures[11] = new VertexPositionTexture(vector3_1, Vector2.Zero);
        BlockBuilder.premadePositionTextures[12] = new VertexPositionTexture(vector3_7, Vector2.Zero);
        BlockBuilder.premadePositionTextures[13] = new VertexPositionTexture(vector3_8, Vector2.Zero);
        BlockBuilder.premadePositionTextures[14] = new VertexPositionTexture(vector3_5, Vector2.Zero);
        BlockBuilder.premadePositionTextures[15] = new VertexPositionTexture(vector3_6, Vector2.Zero);
        BlockBuilder.premadePositionTextures[16] = new VertexPositionTexture(vector3_6, Vector2.Zero);
        BlockBuilder.premadePositionTextures[17] = new VertexPositionTexture(vector3_5, Vector2.Zero);
        BlockBuilder.premadePositionTextures[18] = new VertexPositionTexture(vector3_1, Vector2.Zero);
        BlockBuilder.premadePositionTextures[19] = new VertexPositionTexture(vector3_2, Vector2.Zero);
        BlockBuilder.premadePositionTextures[20] = new VertexPositionTexture(vector3_8, Vector2.Zero);
        BlockBuilder.premadePositionTextures[21] = new VertexPositionTexture(vector3_7, Vector2.Zero);
        BlockBuilder.premadePositionTextures[22] = new VertexPositionTexture(vector3_3, Vector2.Zero);
        BlockBuilder.premadePositionTextures[23] = new VertexPositionTexture(vector3_4, Vector2.Zero);
      }
      if (BlockBuilder.premadeDynamicBlockVertices == null)
      {
        BlockBuilder.premadeDynamicBlockVertices = new DynamicBlockVertex[24];
        BlockBuilder.premadeDynamicBlockVertices[0] = new DynamicBlockVertex(vector3_5, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[1] = new DynamicBlockVertex(vector3_8, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[2] = new DynamicBlockVertex(vector3_4, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[3] = new DynamicBlockVertex(vector3_1, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[4] = new DynamicBlockVertex(vector3_7, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[5] = new DynamicBlockVertex(vector3_6, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[6] = new DynamicBlockVertex(vector3_2, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[7] = new DynamicBlockVertex(vector3_3, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[8] = new DynamicBlockVertex(vector3_4, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[9] = new DynamicBlockVertex(vector3_3, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[10] = new DynamicBlockVertex(vector3_2, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[11] = new DynamicBlockVertex(vector3_1, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[12] = new DynamicBlockVertex(vector3_5, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[13] = new DynamicBlockVertex(vector3_6, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[14] = new DynamicBlockVertex(vector3_7, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[15] = new DynamicBlockVertex(vector3_8, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[16] = new DynamicBlockVertex(vector3_6, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[17] = new DynamicBlockVertex(vector3_5, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[18] = new DynamicBlockVertex(vector3_1, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[19] = new DynamicBlockVertex(vector3_2, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[20] = new DynamicBlockVertex(vector3_8, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[21] = new DynamicBlockVertex(vector3_7, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[22] = new DynamicBlockVertex(vector3_3, Vector2.Zero, (byte) 0);
        BlockBuilder.premadeDynamicBlockVertices[23] = new DynamicBlockVertex(vector3_4, Vector2.Zero, (byte) 0);
        for (int index = 0; index < 4; ++index)
          BlockBuilder.premadeDynamicBlockVertices[index].Normal = -Vector3.UnitX;
        for (int index = 4; index < 8; ++index)
          BlockBuilder.premadeDynamicBlockVertices[index].Normal = Vector3.UnitX;
        for (int index = 8; index < 12; ++index)
          BlockBuilder.premadeDynamicBlockVertices[index].Normal = -Vector3.UnitY;
        for (int index = 12; index < 16; ++index)
          BlockBuilder.premadeDynamicBlockVertices[index].Normal = Vector3.UnitY;
        for (int index = 16; index < 20; ++index)
          BlockBuilder.premadeDynamicBlockVertices[index].Normal = -Vector3.UnitZ;
        for (int index = 20; index < 24; ++index)
          BlockBuilder.premadeDynamicBlockVertices[index].Normal = Vector3.UnitZ;
      }
      if (BlockBuilder.premadeBlockVertices == null)
      {
        BlockBuilder.premadeBlockVertices = new BlockVertex[24];
        BlockBuilder.premadeBlockVertices[0] = new BlockVertex((byte) 0, (byte) 1, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[1] = new BlockVertex((byte) 0, (byte) 1, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[2] = new BlockVertex((byte) 0, (byte) 0, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[3] = new BlockVertex((byte) 0, (byte) 0, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[4] = new BlockVertex((byte) 1, (byte) 1, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[5] = new BlockVertex((byte) 1, (byte) 1, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[6] = new BlockVertex((byte) 1, (byte) 0, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[7] = new BlockVertex((byte) 1, (byte) 0, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[8] = new BlockVertex((byte) 1, (byte) 0, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[9] = new BlockVertex((byte) 0, (byte) 0, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[10] = new BlockVertex((byte) 0, (byte) 0, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[11] = new BlockVertex((byte) 1, (byte) 0, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[12] = new BlockVertex((byte) 1, (byte) 1, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[13] = new BlockVertex((byte) 0, (byte) 1, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[14] = new BlockVertex((byte) 0, (byte) 1, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[15] = new BlockVertex((byte) 1, (byte) 1, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[16] = new BlockVertex((byte) 1, (byte) 1, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[17] = new BlockVertex((byte) 0, (byte) 1, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[18] = new BlockVertex((byte) 0, (byte) 0, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[19] = new BlockVertex((byte) 1, (byte) 0, (byte) 0, (byte) 0);
        BlockBuilder.premadeBlockVertices[20] = new BlockVertex((byte) 0, (byte) 1, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[21] = new BlockVertex((byte) 1, (byte) 1, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[22] = new BlockVertex((byte) 1, (byte) 0, (byte) 1, (byte) 0);
        BlockBuilder.premadeBlockVertices[23] = new BlockVertex((byte) 0, (byte) 0, (byte) 1, (byte) 0);
        for (int index = 0; index < 4; ++index)
          BlockBuilder.premadeBlockVertices[index].SetNormal(-Vector3.UnitX);
        for (int index = 4; index < 8; ++index)
          BlockBuilder.premadeBlockVertices[index].SetNormal(Vector3.UnitX);
        for (int index = 8; index < 12; ++index)
          BlockBuilder.premadeBlockVertices[index].SetNormal(-Vector3.UnitY);
        for (int index = 12; index < 16; ++index)
          BlockBuilder.premadeBlockVertices[index].SetNormal(Vector3.UnitY);
        for (int index = 16; index < 20; ++index)
          BlockBuilder.premadeBlockVertices[index].SetNormal(-Vector3.UnitZ);
        for (int index = 20; index < 24; ++index)
          BlockBuilder.premadeBlockVertices[index].SetNormal(Vector3.UnitZ);
      }
      if (BlockBuilder.premadeIndices != null)
        return;
      BlockBuilder.premadeIndices = new List<short>()
      {
        (short) 3,
        (short) 2,
        (short) 1,
        (short) 1,
        (short) 0,
        (short) 3,
        (short) 7,
        (short) 6,
        (short) 5,
        (short) 5,
        (short) 4,
        (short) 7,
        (short) 11,
        (short) 10,
        (short) 9,
        (short) 9,
        (short) 8,
        (short) 11,
        (short) 15,
        (short) 14,
        (short) 13,
        (short) 13,
        (short) 12,
        (short) 15,
        (short) 19,
        (short) 18,
        (short) 17,
        (short) 17,
        (short) 16,
        (short) 19,
        (short) 23,
        (short) 22,
        (short) 21,
        (short) 21,
        (short) 20,
        (short) 23
      }.ToArray();
    }

    private void ScaleFace(BlockVertex[] vertices, byte vIndex, Byte3 scale)
    {
      for (byte index = 0; index < (byte) 4; ++index)
        vertices[(int) vIndex + (int) index].ScalePosition(scale);
    }

    private void SetTextureToFace(VertexPositionTexture[] vertices, byte vIndex, ref Vector4 uv)
    {
      vertices[(int) vIndex].TextureCoordinate = new Vector2(uv.X, uv.Y);
      vertices[(int) vIndex + 1].TextureCoordinate = new Vector2(uv.X + uv.Z, uv.Y);
      vertices[(int) vIndex + 2].TextureCoordinate = new Vector2(uv.X + uv.Z, uv.Y + uv.W);
      vertices[(int) vIndex + 3].TextureCoordinate = new Vector2(uv.X, uv.Y + uv.W);
    }

    private void SetTextureToFace(
      BlockVertex[] vertices,
      byte vIndex,
      ref BlockBuilder.BlockFace uv)
    {
      vertices[(int) vIndex].SetTextureCoordinates(uv.uv00.X, uv.uv00.Y);
      vertices[(int) vIndex + 1].SetTextureCoordinates(uv.uv10.X, uv.uv10.Y);
      vertices[(int) vIndex + 2].SetTextureCoordinates(uv.uv11.X, uv.uv11.Y);
      vertices[(int) vIndex + 3].SetTextureCoordinates(uv.uv01.X, uv.uv01.Y);
    }

    private void SetTextureToFace(DynamicBlockVertex[] vertices, byte vIndex, ref Vector4 uv)
    {
      vertices[(int) vIndex].UV = new Vector2(uv.X, uv.Y);
      vertices[(int) vIndex + 1].UV = new Vector2(uv.X + uv.Z, uv.Y);
      vertices[(int) vIndex + 2].UV = new Vector2(uv.X + uv.Z, uv.Y + uv.W);
      vertices[(int) vIndex + 3].UV = new Vector2(uv.X, uv.Y + uv.W);
    }

    private void SwapFaces(BlockBuilder.BlockFace[] faces, byte b1, byte b2, byte b3, byte b4)
    {
      BlockBuilder.BlockFace face = faces[(int) b1];
      faces[(int) b1] = faces[(int) b2];
      faces[(int) b2] = faces[(int) b3];
      faces[(int) b3] = faces[(int) b4];
      faces[(int) b4] = face;
    }

    private void SwapFaces(
      ref BlockBuilder.BlockFace f1,
      ref BlockBuilder.BlockFace f2,
      ref BlockBuilder.BlockFace f3,
      ref BlockBuilder.BlockFace f4)
    {
      BlockBuilder.BlockFace blockFace = f1;
      f1 = f2;
      f2 = f3;
      f3 = f4;
      f4 = blockFace;
    }

    private byte NumberOfVisibleFaces(byte visibility)
    {
      int num1 = 1;
      byte num2 = 0;
      for (int index = 0; index < 6; ++index)
      {
        if (((int) visibility & num1 << index) > 0)
          ++num2;
      }
      return num2;
    }

    private BlockBuilder.BlockFace SwapF1toF2(
      BlockBuilder.BlockFace f1,
      BlockBuilder.BlockFace f2,
      byte map)
    {
      switch (map)
      {
        case 0:
          f2.uv00 = f1.uv00;
          f2.uv10 = f1.uv10;
          f2.uv11 = f1.uv11;
          f2.uv01 = f1.uv01;
          break;
        case 1:
          f2.uv00 = f1.uv01;
          f2.uv10 = f1.uv00;
          f2.uv11 = f1.uv10;
          f2.uv01 = f1.uv11;
          break;
        case 2:
          f2.uv00 = f1.uv11;
          f2.uv10 = f1.uv01;
          f2.uv11 = f1.uv00;
          f2.uv01 = f1.uv10;
          break;
        case 3:
          f2.uv00 = f1.uv10;
          f2.uv10 = f1.uv11;
          f2.uv11 = f1.uv01;
          f2.uv01 = f1.uv00;
          break;
      }
      return f2;
    }

    public struct BlockFace
    {
      public Byte2 uv00;
      public Byte2 uv10;
      public Byte2 uv11;
      public Byte2 uv01;

      public void RotateCW()
      {
        Byte2 uv01 = this.uv01;
        this.uv01 = this.uv11;
        this.uv11 = this.uv10;
        this.uv10 = this.uv00;
        this.uv00 = uv01;
      }

      public void RotateCCW()
      {
        Byte2 uv00 = this.uv00;
        this.uv00 = this.uv10;
        this.uv10 = this.uv11;
        this.uv11 = this.uv01;
        this.uv01 = uv00;
      }

      public BlockFace(Byte2 uv00, Byte2 uv10, Byte2 uv11, Byte2 uv01)
      {
        this.uv00 = uv00;
        this.uv10 = uv10;
        this.uv11 = uv11;
        this.uv01 = uv01;
      }
    }
  }
}
