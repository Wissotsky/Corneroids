// Decompiled with JetBrains decompiler
// Type: CornerSpace.BlockSectorCompressor
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.IO.Compression;

#nullable disable
namespace CornerSpace
{
  public class BlockSectorCompressor
  {
    private byte[] formations;
    private byte[] ids;
    private byte[] orientations;

    public BlockSectorCompressor()
    {
      this.formations = new byte[512];
      this.ids = new byte[4096];
      this.orientations = new byte[4096];
    }

    public bool CompressBlockSector(BlockSector sector, ref PackedBlockSector result)
    {
      lock (this)
      {
        if (sector == null)
          return false;
        try
        {
          Vector3int vector3int = new Vector3int((int) Math.Round((double) sector.PositionInShipCoordinates.X), (int) Math.Round((double) sector.PositionInShipCoordinates.Y), (int) Math.Round((double) sector.PositionInShipCoordinates.Z));
          using (MemoryStream memoryStream1 = new MemoryStream())
          {
            using (MemoryStream memoryStream2 = new MemoryStream())
            {
              using (MemoryStream memoryStream3 = new MemoryStream())
              {
                using (GZipStream gzipStream1 = new GZipStream((Stream) memoryStream1, CompressionMode.Compress))
                {
                  using (GZipStream gzipStream2 = new GZipStream((Stream) memoryStream2, CompressionMode.Compress))
                  {
                    using (GZipStream gzipStream3 = new GZipStream((Stream) memoryStream3, CompressionMode.Compress))
                    {
                      Vector3 vector3 = sector.PositionInShipCoordinates - Vector3.One * 7.5f;
                      int blockCount = sector.BlockCount;
                      int index1 = 0;
                      for (int index2 = 0; index2 < 512; ++index2)
                      {
                        byte num1 = 0;
                        for (int index3 = 0; index3 < 8; ++index3)
                        {
                          int num2 = index2 * 8 + index3;
                          BlockCell blockCell = sector.GetBlockCell(vector3 + new Vector3((float) (num2 % 16), (float) (num2 / 16 % 16), (float) (num2 / 16 / 16 % 16)));
                          bool flag = false;
                          if (blockCell != null && blockCell.X == (byte) 0 && blockCell.Y == (byte) 0 && blockCell.Z == (byte) 0)
                            flag = true;
                          num1 = (byte) ((int) num1 << 1 | (flag ? 1 : 0));
                          if (flag && blockCell.X == (byte) 0 && blockCell.Y == (byte) 0 && blockCell.Z == (byte) 0)
                          {
                            this.ids[index1] = blockCell.Block.Id;
                            this.orientations[index1] = blockCell.Block.Orientation;
                            ++index1;
                          }
                        }
                        this.formations[index2] = num1;
                      }
                      gzipStream1.Write(this.formations, 0, 512);
                      gzipStream2.Write(this.ids, 0, sector.BlockCount);
                      gzipStream3.Write(this.orientations, 0, sector.BlockCount);
                      gzipStream1.Dispose();
                      memoryStream1.Dispose();
                      gzipStream2.Dispose();
                      memoryStream2.Dispose();
                      gzipStream3.Dispose();
                      memoryStream3.Dispose();
                      byte[] array1 = memoryStream1.ToArray();
                      byte[] array2 = memoryStream2.ToArray();
                      byte[] array3 = memoryStream3.ToArray();
                      result = new PackedBlockSector()
                      {
                        SectorPosition = vector3int,
                        CompressedFormations = array1,
                        CompressedIds = array2,
                        CompressedOrientations = array3
                      };
                    }
                  }
                }
              }
            }
          }
          return true;
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Block sector compression failed: " + ex.Message);
          return false;
        }
      }
    }

    public void ExtractBlockSectorToEntity(ref PackedBlockSector sector, SpaceEntity entity)
    {
      lock (this)
      {
        try
        {
          int id1 = entity.Id;
          if (sector.CompressedFormations == null || sector.CompressedIds == null || sector.CompressedOrientations == null || entity == null)
            throw new ArgumentNullException("PackedBlockSector/entity");
          Vector3 vector3_1 = (Vector3) sector.SectorPosition - Vector3.One * 8f;
          using (MemoryStream memoryStream1 = new MemoryStream(sector.CompressedFormations))
          {
            using (MemoryStream memoryStream2 = new MemoryStream(sector.CompressedIds))
            {
              using (MemoryStream memoryStream3 = new MemoryStream(sector.CompressedOrientations))
              {
                using (GZipStream gzipStream1 = new GZipStream((Stream) memoryStream1, CompressionMode.Decompress))
                {
                  using (GZipStream gzipStream2 = new GZipStream((Stream) memoryStream2, CompressionMode.Decompress))
                  {
                    using (GZipStream gzipStream3 = new GZipStream((Stream) memoryStream3, CompressionMode.Decompress))
                    {
                      gzipStream1.Read(this.formations, 0, 512);
                      gzipStream2.Read(this.ids, 0, this.ids.Length);
                      gzipStream3.Read(this.orientations, 0, this.orientations.Length);
                    }
                  }
                }
              }
            }
          }
          int index1 = 0;
          for (int index2 = 0; index2 < this.formations.Length; ++index2)
          {
            byte formation = this.formations[index2];
            for (int index3 = 0; index3 < 8; ++index3)
            {
              if (((int) formation & 128 >> index3) != 0)
              {
                int num = index2 * 8 + index3;
                byte id2 = this.ids[index1];
                byte orientation = this.orientations[index1];
                Block blockById = Engine.LoadedWorld.Itemset.GetBlockByID((int) id2);
                if (blockById != null)
                {
                  Vector3 vector3_2 = new Vector3((float) (num % 16), (float) (num / 16 % 16), (float) (num / 256));
                  blockById.Orientation = orientation;
                  Vector3 size = (Vector3) blockById.Size;
                  vector3_2 += 0.5f * size;
                  entity.AddBlockFast(blockById, new Position3(vector3_1 + vector3_2));
                }
                ++index1;
              }
            }
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to extract block data: " + ex.Message);
        }
      }
    }
  }
}
