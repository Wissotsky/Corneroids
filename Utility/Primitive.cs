// Decompiled with JetBrains decompiler
// Type: CornerSpace.Utility.Primitive
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.Utility
{
  public static class Primitive
  {
    public static void CreateBlockStructure(
      out VertexPositionTexture[] vertices,
      BlockTextureCoordinates tc)
    {
      vertices = new VertexPositionTexture[36];
      Primitive.CreateVertices(ref vertices);
      Primitive.SetTextureCoordinates(ref vertices, tc);
    }

    public static void CreateSelectorBox(
      Vector3 size,
      out VertexPositionColorTexture[] vertices,
      out short[] indices)
    {
      float x = size.X;
      float y = size.Y;
      float z = size.Z;
      Vector4 vector4 = new Vector4(0.0f, 0.0f, 1f, 1f);
      if ((double) x <= 0.0 || (double) y <= 0.0 || (double) z <= 0.0)
        throw new Exception("Invalid box dimensions");
      vertices = new VertexPositionColorTexture[24];
      indices = new short[36];
      Vector3 position1 = new Vector3((float) (-(double) x / 2.0), (float) (-(double) y / 2.0), (float) (-(double) z / 2.0));
      Vector3 position2 = new Vector3(x / 2f, (float) (-(double) y / 2.0), (float) (-(double) z / 2.0));
      Vector3 position3 = new Vector3(x / 2f, (float) (-(double) y / 2.0), z / 2f);
      Vector3 position4 = new Vector3((float) (-(double) x / 2.0), (float) (-(double) y / 2.0), z / 2f);
      Vector3 position5 = new Vector3((float) (-(double) x / 2.0), y / 2f, (float) (-(double) z / 2.0));
      Vector3 position6 = new Vector3(x / 2f, y / 2f, (float) (-(double) z / 2.0));
      Vector3 position7 = new Vector3(x / 2f, y / 2f, z / 2f);
      Vector3 position8 = new Vector3((float) (-(double) x / 2.0), y / 2f, z / 2f);
      vertices[0] = new VertexPositionColorTexture(position1, Color.White, new Vector2(vector4.X, vector4.Y));
      vertices[1] = new VertexPositionColorTexture(position2, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y));
      vertices[2] = new VertexPositionColorTexture(position6, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y + vector4.W));
      vertices[3] = new VertexPositionColorTexture(position5, Color.White, new Vector2(vector4.X, vector4.Y + vector4.Z));
      vertices[4] = new VertexPositionColorTexture(position2, Color.White, new Vector2(vector4.X, vector4.Y));
      vertices[5] = new VertexPositionColorTexture(position3, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y));
      vertices[6] = new VertexPositionColorTexture(position7, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y + vector4.W));
      vertices[7] = new VertexPositionColorTexture(position6, Color.White, new Vector2(vector4.X, vector4.Y + vector4.Z));
      vertices[8] = new VertexPositionColorTexture(position3, Color.White, new Vector2(vector4.X, vector4.Y));
      vertices[9] = new VertexPositionColorTexture(position4, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y));
      vertices[10] = new VertexPositionColorTexture(position8, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y + vector4.W));
      vertices[11] = new VertexPositionColorTexture(position7, Color.White, new Vector2(vector4.X, vector4.Y + vector4.Z));
      vertices[12] = new VertexPositionColorTexture(position4, Color.White, new Vector2(vector4.X, vector4.Y));
      vertices[13] = new VertexPositionColorTexture(position1, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y));
      vertices[14] = new VertexPositionColorTexture(position5, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y + vector4.W));
      vertices[15] = new VertexPositionColorTexture(position8, Color.White, new Vector2(vector4.X, vector4.Y + vector4.Z));
      vertices[16] = new VertexPositionColorTexture(position2, Color.White, new Vector2(vector4.X, vector4.Y));
      vertices[17] = new VertexPositionColorTexture(position1, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y));
      vertices[18] = new VertexPositionColorTexture(position4, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y + vector4.W));
      vertices[19] = new VertexPositionColorTexture(position3, Color.White, new Vector2(vector4.X, vector4.Y + vector4.Z));
      vertices[20] = new VertexPositionColorTexture(position7, Color.White, new Vector2(vector4.X, vector4.Y));
      vertices[21] = new VertexPositionColorTexture(position8, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y));
      vertices[22] = new VertexPositionColorTexture(position5, Color.White, new Vector2(vector4.X + vector4.Z, vector4.Y + vector4.W));
      vertices[23] = new VertexPositionColorTexture(position6, Color.White, new Vector2(vector4.X, vector4.Y + vector4.Z));
      indices[0] = (short) 0;
      indices[1] = (short) 2;
      indices[2] = (short) 3;
      indices[3] = (short) 0;
      indices[4] = (short) 1;
      indices[5] = (short) 2;
      indices[6] = (short) 4;
      indices[7] = (short) 5;
      indices[8] = (short) 6;
      indices[9] = (short) 4;
      indices[10] = (short) 6;
      indices[11] = (short) 7;
      indices[12] = (short) 8;
      indices[13] = (short) 9;
      indices[14] = (short) 10;
      indices[15] = (short) 8;
      indices[16] = (short) 10;
      indices[17] = (short) 11;
      indices[18] = (short) 12;
      indices[19] = (short) 13;
      indices[20] = (short) 14;
      indices[21] = (short) 12;
      indices[22] = (short) 14;
      indices[23] = (short) 15;
      indices[24] = (short) 16;
      indices[25] = (short) 17;
      indices[26] = (short) 18;
      indices[27] = (short) 16;
      indices[28] = (short) 18;
      indices[29] = (short) 19;
      indices[30] = (short) 20;
      indices[31] = (short) 21;
      indices[32] = (short) 22;
      indices[33] = (short) 20;
      indices[34] = (short) 22;
      indices[35] = (short) 23;
    }

    public static void CreateUnitBox(
      out VertexPositionColor[] vertices,
      out short[] indices,
      Color color)
    {
      List<VertexPositionColor> vertexPositionColorList = new List<VertexPositionColor>();
      List<short> shortList = new List<short>();
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(-1f, -1f, -1f), color));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, -1f, -1f), color));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, -1f, 1f), color));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(-1f, -1f, 1f), color));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(-1f, 1f, -1f), color));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, 1f, -1f), color));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(1f, 1f, 1f), color));
      vertexPositionColorList.Add(new VertexPositionColor(new Vector3(-1f, 1f, 1f), color));
      shortList.Add((short) 3);
      shortList.Add((short) 0);
      shortList.Add((short) 1);
      shortList.Add((short) 1);
      shortList.Add((short) 2);
      shortList.Add((short) 3);
      shortList.Add((short) 4);
      shortList.Add((short) 7);
      shortList.Add((short) 6);
      shortList.Add((short) 6);
      shortList.Add((short) 5);
      shortList.Add((short) 4);
      shortList.Add((short) 4);
      shortList.Add((short) 0);
      shortList.Add((short) 3);
      shortList.Add((short) 3);
      shortList.Add((short) 7);
      shortList.Add((short) 4);
      shortList.Add((short) 5);
      shortList.Add((short) 1);
      shortList.Add((short) 0);
      shortList.Add((short) 0);
      shortList.Add((short) 4);
      shortList.Add((short) 5);
      shortList.Add((short) 6);
      shortList.Add((short) 2);
      shortList.Add((short) 1);
      shortList.Add((short) 1);
      shortList.Add((short) 5);
      shortList.Add((short) 6);
      shortList.Add((short) 7);
      shortList.Add((short) 3);
      shortList.Add((short) 2);
      shortList.Add((short) 2);
      shortList.Add((short) 6);
      shortList.Add((short) 7);
      vertices = vertexPositionColorList.ToArray();
      indices = shortList.ToArray();
    }

    public static void CreateUnitBox(
      out VertexPositionNormalTexture[] vertices,
      out short[] indices)
    {
      vertices = new VertexPositionNormalTexture[24];
      indices = new short[36];
      Vector3 position1 = new Vector3(-0.5f, -0.5f, -0.5f);
      Vector3 position2 = new Vector3(0.5f, -0.5f, -0.5f);
      Vector3 position3 = new Vector3(0.5f, -0.5f, 0.5f);
      Vector3 position4 = new Vector3(-0.5f, -0.5f, 0.5f);
      Vector3 position5 = new Vector3(-0.5f, 0.5f, -0.5f);
      Vector3 position6 = new Vector3(0.5f, 0.5f, -0.5f);
      Vector3 position7 = new Vector3(0.5f, 0.5f, 0.5f);
      Vector3 position8 = new Vector3(-0.5f, 0.5f, 0.5f);
      vertices[0] = new VertexPositionNormalTexture(position1, -Vector3.UnitZ, Vector2.Zero);
      vertices[1] = new VertexPositionNormalTexture(position2, -Vector3.UnitZ, Vector2.Zero);
      vertices[2] = new VertexPositionNormalTexture(position6, -Vector3.UnitZ, Vector2.Zero);
      vertices[3] = new VertexPositionNormalTexture(position5, -Vector3.UnitZ, Vector2.Zero);
      vertices[4] = new VertexPositionNormalTexture(position2, Vector3.UnitX, Vector2.Zero);
      vertices[5] = new VertexPositionNormalTexture(position3, Vector3.UnitX, Vector2.Zero);
      vertices[6] = new VertexPositionNormalTexture(position7, Vector3.UnitX, Vector2.Zero);
      vertices[7] = new VertexPositionNormalTexture(position6, Vector3.UnitX, Vector2.Zero);
      vertices[8] = new VertexPositionNormalTexture(position3, Vector3.UnitZ, Vector2.Zero);
      vertices[9] = new VertexPositionNormalTexture(position4, Vector3.UnitZ, Vector2.Zero);
      vertices[10] = new VertexPositionNormalTexture(position8, Vector3.UnitZ, Vector2.Zero);
      vertices[11] = new VertexPositionNormalTexture(position7, Vector3.UnitZ, Vector2.Zero);
      vertices[12] = new VertexPositionNormalTexture(position4, -Vector3.UnitX, Vector2.Zero);
      vertices[13] = new VertexPositionNormalTexture(position1, -Vector3.UnitX, Vector2.Zero);
      vertices[14] = new VertexPositionNormalTexture(position5, -Vector3.UnitX, Vector2.Zero);
      vertices[15] = new VertexPositionNormalTexture(position8, -Vector3.UnitX, Vector2.Zero);
      vertices[16] = new VertexPositionNormalTexture(position2, -Vector3.UnitY, Vector2.Zero);
      vertices[17] = new VertexPositionNormalTexture(position1, -Vector3.UnitY, Vector2.Zero);
      vertices[18] = new VertexPositionNormalTexture(position4, -Vector3.UnitY, Vector2.Zero);
      vertices[19] = new VertexPositionNormalTexture(position3, -Vector3.UnitY, Vector2.Zero);
      vertices[20] = new VertexPositionNormalTexture(position7, Vector3.UnitY, Vector2.Zero);
      vertices[21] = new VertexPositionNormalTexture(position8, Vector3.UnitY, Vector2.Zero);
      vertices[22] = new VertexPositionNormalTexture(position5, Vector3.UnitY, Vector2.Zero);
      vertices[23] = new VertexPositionNormalTexture(position6, Vector3.UnitY, Vector2.Zero);
      indices[0] = (short) 0;
      indices[1] = (short) 2;
      indices[2] = (short) 3;
      indices[3] = (short) 0;
      indices[4] = (short) 1;
      indices[5] = (short) 2;
      indices[6] = (short) 4;
      indices[7] = (short) 5;
      indices[8] = (short) 6;
      indices[9] = (short) 4;
      indices[10] = (short) 6;
      indices[11] = (short) 7;
      indices[12] = (short) 8;
      indices[13] = (short) 9;
      indices[14] = (short) 10;
      indices[15] = (short) 8;
      indices[16] = (short) 10;
      indices[17] = (short) 11;
      indices[18] = (short) 12;
      indices[19] = (short) 13;
      indices[20] = (short) 14;
      indices[21] = (short) 12;
      indices[22] = (short) 14;
      indices[23] = (short) 15;
      indices[24] = (short) 16;
      indices[25] = (short) 17;
      indices[26] = (short) 18;
      indices[27] = (short) 16;
      indices[28] = (short) 18;
      indices[29] = (short) 19;
      indices[30] = (short) 20;
      indices[31] = (short) 21;
      indices[32] = (short) 22;
      indices[33] = (short) 20;
      indices[34] = (short) 22;
      indices[35] = (short) 23;
    }

    public static void CreateUnitCone(
      int complexity,
      out VertexPositionColor[] vertices,
      out short[] indices)
    {
      complexity = Math.Max(complexity, 3);
      List<VertexPositionColor> vertexPositionColorList = new List<VertexPositionColor>();
      List<short> shortList = new List<short>();
      vertexPositionColorList.Add(new VertexPositionColor(Vector3.Zero, Color.White));
      for (int index = 0; index < complexity; ++index)
      {
        float num = 6.28318548f / (float) complexity * (float) (index - 1);
        vertexPositionColorList.Add(new VertexPositionColor(new Vector3((float) Math.Cos((double) num) * 0.5f, 1f, (float) Math.Sin((double) num) * 0.5f), Color.White));
      }
      for (int index = 0; index < complexity; ++index)
      {
        shortList.Add((short) 0);
        shortList.Add((short) (index + 1));
        shortList.Add((short) (index + 2));
        if (index == complexity - 1)
        {
          shortList.RemoveAt(shortList.Count - 1);
          shortList.Add((short) 1);
        }
      }
      for (int index = complexity - 1; index > 1; --index)
      {
        shortList.Add((short) complexity);
        shortList.Add((short) index);
        shortList.Add((short) (index - 1));
      }
      vertices = vertexPositionColorList.ToArray();
      indices = shortList.ToArray();
    }

    private static void CreateVertices(ref VertexPositionTexture[] vertices)
    {
      Vector3 position1 = new Vector3(-0.5f, -0.5f, -0.5f);
      Vector3 position2 = new Vector3(0.5f, -0.5f, -0.5f);
      Vector3 position3 = new Vector3(0.5f, -0.5f, 0.5f);
      Vector3 position4 = new Vector3(-0.5f, -0.5f, 0.5f);
      Vector3 position5 = new Vector3(-0.5f, 0.5f, -0.5f);
      Vector3 position6 = new Vector3(0.5f, 0.5f, -0.5f);
      Vector3 position7 = new Vector3(0.5f, 0.5f, 0.5f);
      Vector3 position8 = new Vector3(-0.5f, 0.5f, 0.5f);
      vertices[0] = new VertexPositionTexture(position1, Vector2.Zero);
      vertices[1] = new VertexPositionTexture(position2, Vector2.Zero);
      vertices[2] = new VertexPositionTexture(position6, Vector2.Zero);
      vertices[3] = new VertexPositionTexture(position6, Vector2.Zero);
      vertices[4] = new VertexPositionTexture(position5, Vector2.Zero);
      vertices[5] = new VertexPositionTexture(position1, Vector2.Zero);
      vertices[6] = new VertexPositionTexture(position2, Vector2.Zero);
      vertices[7] = new VertexPositionTexture(position3, Vector2.Zero);
      vertices[8] = new VertexPositionTexture(position7, Vector2.Zero);
      vertices[9] = new VertexPositionTexture(position7, Vector2.Zero);
      vertices[10] = new VertexPositionTexture(position6, Vector2.Zero);
      vertices[11] = new VertexPositionTexture(position2, Vector2.Zero);
      vertices[12] = new VertexPositionTexture(position3, Vector2.Zero);
      vertices[13] = new VertexPositionTexture(position4, Vector2.Zero);
      vertices[14] = new VertexPositionTexture(position8, Vector2.Zero);
      vertices[15] = new VertexPositionTexture(position8, Vector2.Zero);
      vertices[16] = new VertexPositionTexture(position7, Vector2.Zero);
      vertices[17] = new VertexPositionTexture(position3, Vector2.Zero);
      vertices[18] = new VertexPositionTexture(position4, Vector2.Zero);
      vertices[19] = new VertexPositionTexture(position1, Vector2.Zero);
      vertices[20] = new VertexPositionTexture(position5, Vector2.Zero);
      vertices[21] = new VertexPositionTexture(position5, Vector2.Zero);
      vertices[22] = new VertexPositionTexture(position8, Vector2.Zero);
      vertices[23] = new VertexPositionTexture(position4, Vector2.Zero);
      vertices[24] = new VertexPositionTexture(position1, Vector2.Zero);
      vertices[25] = new VertexPositionTexture(position4, Vector2.Zero);
      vertices[26] = new VertexPositionTexture(position3, Vector2.Zero);
      vertices[27] = new VertexPositionTexture(position3, Vector2.Zero);
      vertices[28] = new VertexPositionTexture(position2, Vector2.Zero);
      vertices[29] = new VertexPositionTexture(position1, Vector2.Zero);
      vertices[30] = new VertexPositionTexture(position5, Vector2.Zero);
      vertices[31] = new VertexPositionTexture(position4, Vector2.Zero);
      vertices[32] = new VertexPositionTexture(position7, Vector2.Zero);
      vertices[33] = new VertexPositionTexture(position7, Vector2.Zero);
      vertices[34] = new VertexPositionTexture(position6, Vector2.Zero);
      vertices[35] = new VertexPositionTexture(position5, Vector2.Zero);
    }

    private static void SetTextureCoordinates(
      ref VertexPositionTexture[] vertices,
      BlockTextureCoordinates tc)
    {
      Byte4 topCoordinates = tc.TopCoordinates;
      Byte4 wallCoordinates = tc.WallCoordinates;
      Byte4 bottomCoordinates = tc.BottomCoordinates;
      float textureUnitSize = Engine.LoadedWorld.BlockTextureAtlas.TextureUnitSize;
      Vector2 vector2_1 = new Vector2((float) ((int) wallCoordinates.X % 16), (float) ((int) wallCoordinates.Y / 16)) * textureUnitSize;
      Vector2 vector2_2 = new Vector2((float) (((int) wallCoordinates.X + (int) wallCoordinates.Z) % 16), (float) ((int) wallCoordinates.Y / 16)) * textureUnitSize;
      Vector2 vector2_3 = new Vector2((float) ((int) wallCoordinates.X % 16), (float) (((int) wallCoordinates.Y + (int) wallCoordinates.W) / 16)) * textureUnitSize;
      Vector2 vector2_4 = new Vector2((float) (((int) wallCoordinates.X + (int) wallCoordinates.Z) % 16), (float) (((int) wallCoordinates.Y + (int) wallCoordinates.W) / 16)) * textureUnitSize;
      vertices[0].TextureCoordinate = vector2_4;
      vertices[1].TextureCoordinate = vector2_3;
      vertices[2].TextureCoordinate = vector2_1;
      vertices[3].TextureCoordinate = vector2_1;
      vertices[4].TextureCoordinate = vector2_2;
      vertices[5].TextureCoordinate = vector2_4;
    }
  }
}
