// Decompiled with JetBrains decompiler
// Type: CornerSpace.WeaponItem
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class WeaponItem : LightingItem
  {
    private const float cPushbackReduction = 0.75f;
    private const int cSpreadRandomizerSeed = 24092004;
    private Position3 gunPosition;
    private Vector3 pointingDirection;
    private Quaternion orientation;
    private int projectileId;
    private float pushback;
    private float pushbackDistance;
    private byte randomVectorPosition;
    private int reloadTime;
    private int reloadLeft;
    private float spread;
    private Vector3[] spreadVectors;

    public WeaponItem()
    {
      this.spreadVectors = new Vector3[256];
      this.randomVectorPosition = (byte) 0;
      this.randomVectorPosition = (byte) 0;
      this.InitializeSpreadVectors();
    }

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Reload time: " + ((float) this.reloadTime / 1000f).ToString() + " s");
      ProjectileType projectileType = Engine.LoadedWorld.Itemset.ProjectileTypes[this.projectileId];
      if (projectileType == null)
        return;
      lines.Add("Damage: " + (object) projectileType.Damage);
      lines.Add("Speed: " + (object) projectileType.Speed + " units/s");
    }

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      this.LightOn = false;
      if (!input.LeftDown || this.reloadLeft != 0 || (double) powerToUse < (double) this.PowerUsage)
        return Item.UsageResult.None;
      ProjectileType projectileType = Engine.LoadedWorld.Itemset.ProjectileTypes[this.projectileId];
      if (projectileType != null)
      {
        Position3 position = this.gunPosition + this.pointingDirection;
        Vector3 pointingDirection = this.pointingDirection;
        if ((double) this.spread > 0.0)
        {
          Vector3 spreadVector = this.spreadVectors[(int) this.randomVectorPosition];
          owner.SynchronizationValue.Clear();
          owner.SynchronizationValue.StoreValue(this.randomVectorPosition);
          pointingDirection += spreadVector * this.spread;
          this.randomVectorPosition = (byte) (((int) this.randomVectorPosition + 1) % 256);
        }
        Projectile projectile = new Projectile(projectileType, position, pointingDirection);
        projectile.ShotBy = (PhysicalObject) owner.Astronaut;
        if (Item.projectileManager != null)
          Item.projectileManager.AddProjectile(projectile);
      }
      this.reloadLeft = this.reloadTime;
      this.LightOn = true;
      this.pushbackDistance = this.pushback;
      this.ModelOffset = Vector3.UnitZ * this.pushback;
      return Item.UsageResult.Power_used;
    }

    public override void Update(Astronaut astronaut)
    {
      base.Update(astronaut);
      this.pointingDirection = astronaut.GetLookatVector();
      this.orientation = astronaut.Orientation;
      this.gunPosition = astronaut.Position + astronaut.GetStrafeVector() * 0.3f - astronaut.GetUpVector() * 0.2f;
      this.Light.Position = this.gunPosition - astronaut.GetStrafeVector() * 0.5f;
      this.reloadLeft -= (int) Engine.FrameCounter.DeltaTimeMS;
      if (this.reloadLeft < 0)
        this.reloadLeft = 0;
      if ((double) this.pushbackDistance <= 0.0)
        return;
      this.pushbackDistance -= 0.75f * Engine.FrameCounter.DeltaTime;
      this.ModelOffset = Vector3.UnitZ * this.pushbackDistance;
      if ((double) this.pushbackDistance > 0.0)
        return;
      this.pushbackDistance = 0.0f;
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.projectileId = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "projectileId");
      this.reloadTime = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 1000, "reloadTime");
      this.pushback = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 0.0f, "pushback");
      this.spread = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 0.0f, "spread");
      this.reloadTime = Math.Max(this.reloadTime, 50);
      this.pushback = MathHelper.Clamp(this.pushback, 0.0f, 1f);
      this.spread = MathHelper.Clamp(this.spread, 0.0f, 0.5f);
    }

    public override byte SynchronizationValue
    {
      set => this.randomVectorPosition = value;
    }

    private void InitializeSpreadVectors()
    {
      Random random = new Random(24092004);
      for (int index = 0; index < this.spreadVectors.Length; ++index)
      {
        Vector3 vector3 = new Vector3((float) random.NextDouble() - 0.5f, (float) random.NextDouble() - 0.5f, (float) random.NextDouble() - 0.5f);
        if (vector3 != Vector3.Zero)
        {
          vector3.Normalize();
          this.spreadVectors[index] = vector3;
        }
      }
    }
  }
}
