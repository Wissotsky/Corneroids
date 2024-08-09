// Decompiled with JetBrains decompiler
// Type: CornerSpace.Utility.Md5
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Security.Cryptography;
using System.Text;

#nullable disable
namespace CornerSpace.Utility
{
  public class Md5
  {
    private static Md5 instance;

    private Md5()
    {
    }

    public string Hash(byte[] data)
    {
      if (data == null || data.Length == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      using (MD5 md5 = MD5.Create())
      {
        foreach (byte num in md5.ComputeHash(data))
          stringBuilder.Append(num.ToString("x2"));
      }
      return stringBuilder.ToString();
    }

    public string Hash(string data) => this.Hash(Encoding.Default.GetBytes(data));

    public static Md5 Hasher
    {
      get
      {
        if (Md5.instance == null)
          Md5.instance = new Md5();
        return Md5.instance;
      }
    }
  }
}
