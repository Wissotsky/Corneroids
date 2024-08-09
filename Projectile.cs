// Decompiled with JetBrains decompiler
// Type: CornerSpace.Projectile
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;

#nullable disable
namespace CornerSpace
{
  public class Projectile
  {
    public byte DamageLeft;
    public Vector3 Direction;
    public int Lifetime;
    public Position3 Position;
    public PhysicalObject ShotBy;
    public Vector3 Speed;
    public ProjectileType ProjectileType;
    public PointLight PointLight;

    public Projectile(ProjectileType type, Position3 position, Vector3 direction)
    {
      this.DamageLeft = type.Damage;
      this.Direction = direction;
      this.Lifetime = type.Lifetime;
      this.ProjectileType = type;
      this.Position = position;
      this.Speed = direction * type.Speed;
      this.PointLight = new PointLight(type.Color, 3f);
    }

    public void Update()
    {
      this.Position += this.Speed * Engine.FrameCounter.DeltaTime;
      if (this.PointLight != null)
        this.PointLight.Position = this.Position;
      this.Lifetime -= (int) Engine.FrameCounter.DeltaTimeMS;
    }
  }
}
