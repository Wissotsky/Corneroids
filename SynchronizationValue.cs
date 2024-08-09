// Decompiled with JetBrains decompiler
// Type: CornerSpace.SynchronizationValue
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class SynchronizationValue
  {
    private byte index;
    private uint value;

    public byte GetByte(byte index)
    {
      return (byte) (this.value >> (int) index % 4 * 8 & (uint) byte.MaxValue);
    }

    public SynchronizationValue() => this.index = (byte) 0;

    public SynchronizationValue(uint value)
    {
      this.index = (byte) 0;
      this.value = value;
    }

    public void Clear()
    {
      this.index = (byte) 0;
      this.value = 0U;
    }

    public void StoreValue(byte value)
    {
      this.index %= (byte) 4;
      uint num = (uint) value;
      this.value &= (uint) ~((int) byte.MaxValue << 8 * (int) this.index);
      this.value |= num << 8 * (int) this.index;
      ++this.index;
    }

    public uint StoredValue
    {
      get => this.value;
      set => this.value = value;
    }
  }
}
