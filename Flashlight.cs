// Decompiled with JetBrains decompiler
// Type: CornerSpace.Flashlight
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace CornerSpace
{
  public class Flashlight : SpotLight
  {
    private int batteryCapacity;
    private int batteryRechargeRate;

    public Flashlight(Color color, float length, float radius, Vector3 direction)
      : base(color, length, radius, direction)
    {
    }

    public int BatteryCapacity
    {
      get => this.batteryCapacity;
      set => this.batteryCapacity = value;
    }

    public int BatteryRechargeRate
    {
      get => this.batteryRechargeRate;
      set => this.batteryRechargeRate = value;
    }
  }
}
