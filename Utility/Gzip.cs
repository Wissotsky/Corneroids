// Decompiled with JetBrains decompiler
// Type: CornerSpace.Utility.Gzip
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.IO;
using System.IO.Compression;

#nullable disable
namespace CornerSpace.Utility
{
  public class Gzip
  {
    private static Gzip instance;

    public byte[] Compress(byte[] bytesToCompress)
    {
      if (bytesToCompress == null || bytesToCompress.Length == 0)
        return (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Compress))
          gzipStream.Write(bytesToCompress, 0, bytesToCompress.Length);
        return memoryStream.ToArray();
      }
    }

    public byte[] Decompress(byte[] bytesToDecompress, int unzippedByteCount)
    {
      if (bytesToDecompress == null || bytesToDecompress.Length == 0 || unzippedByteCount <= 0)
        return (byte[]) null;
      byte[] buffer = new byte[unzippedByteCount];
      using (MemoryStream memoryStream = new MemoryStream(bytesToDecompress))
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Decompress))
          gzipStream.Read(buffer, 0, unzippedByteCount);
      }
      return buffer;
    }

    public static Gzip Instance
    {
      get
      {
        if (Gzip.instance == null)
          Gzip.instance = new Gzip();
        return Gzip.instance;
      }
    }
  }
}
