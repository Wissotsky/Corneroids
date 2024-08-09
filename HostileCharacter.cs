// Decompiled with JetBrains decompiler
// Type: CornerSpace.HostileCharacter
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public abstract class HostileCharacter : AICharacter
  {
    private PhysicalObject target;

    public HostileCharacter(string modelPath, string texturePath, float radius)
      : base(modelPath, texturePath, radius)
    {
    }

    public abstract int MaxHostileCountPerSpawnCycle { get; }

    public PhysicalObject Target
    {
      get => this.target;
      set => this.target = value;
    }
  }
}
