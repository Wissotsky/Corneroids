// Decompiled with JetBrains decompiler
// Type: CornerSpace.ConsumableItem
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class ConsumableItem : ItemWithModel
  {
    private const int cCoolDownMS = 1000;
    private int armorModification;
    private Rectangle buffSprite;
    private int cooldownTimerMS;
    private int effectDurationMS;
    private short healthModification;
    private short powerModification;
    private ConsumableItem.Type type;
    private ConsumableItem.State state;

    public ConsumableItem()
    {
      this.cooldownTimerMS = 0;
      this.state = ConsumableItem.State.Ready;
    }

    public Buff CreateBuff()
    {
      return new Buff((object) this, (Action<Character>) (character =>
      {
        if (this.type == ConsumableItem.Type.PowerUp)
        {
          character.Suit.ArmorRating += new ArmorRating((uint) Math.Max(this.armorModification, 0));
          character.InitialMaximumHealth += this.healthModification;
          character.Suit.PowerRechargeRate += (int) this.powerModification;
        }
        else
        {
          if (this.type != ConsumableItem.Type.Restore)
            return;
          character.Health += this.healthModification;
          character.Power += (float) this.powerModification;
        }
      }), (Action<Character>) (character =>
      {
        if (this.type != ConsumableItem.Type.PowerUp)
          return;
        character.Suit.ArmorRating -= new ArmorRating((uint) Math.Max(this.armorModification, 0));
        character.InitialMaximumHealth -= this.healthModification;
        character.Suit.PowerRechargeRate -= (int) this.powerModification;
      }), this.effectDurationMS, this.SpriteCoordsRect);
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      this.type = (ConsumableItem.Type) Convert.ToInt32(element.Element((XName) "type").Value, (IFormatProvider) CultureInfo.InvariantCulture);
      this.armorModification = Convert.ToInt32(element.Element((XName) "armor").Value, (IFormatProvider) CultureInfo.InvariantCulture);
      this.healthModification = Convert.ToInt16(element.Element((XName) "health").Value, (IFormatProvider) CultureInfo.InvariantCulture);
      this.powerModification = Convert.ToInt16(element.Element((XName) "power").Value, (IFormatProvider) CultureInfo.InvariantCulture);
      this.effectDurationMS = Convert.ToInt32(element.Element((XName) "duration").Value, (IFormatProvider) CultureInfo.InvariantCulture);
      XmlReader instance = XmlReader.Instance;
      this.buffSprite = new Rectangle(instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "x", 0, "buffSpriteCoords"), instance.ReadAttributeValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), "y", 0, "buffSpriteCoords"), 1, 1);
      this.effectDurationMS = Math.Max(this.effectDurationMS, 0);
    }

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      if (!input.LeftClick || this.cooldownTimerMS != 0)
        return Item.UsageResult.None;
      Buff buff = this.CreateBuff();
      owner.Astronaut.ApplyBuff(buff);
      this.cooldownTimerMS = 1000;
      return Item.UsageResult.Consumed;
    }

    public override void Update()
    {
      this.cooldownTimerMS = Math.Max(this.cooldownTimerMS - (int) Engine.FrameCounter.DeltaTimeMS, 0);
    }

    public virtual Rectangle BuffSpriteCoords
    {
      get
      {
        World loadedWorld = Engine.LoadedWorld;
        if (loadedWorld != null)
        {
          SpriteTextureAtlas spriteTextureAtlas = loadedWorld.SpriteTextureAtlas;
          if (spriteTextureAtlas != null)
          {
            int textureUnitSizePixels = spriteTextureAtlas.TextureUnitSizePixels;
            return new Rectangle(this.buffSprite.X * textureUnitSizePixels, this.buffSprite.Y * textureUnitSizePixels, this.buffSprite.Width * textureUnitSizePixels, this.buffSprite.Height * textureUnitSizePixels);
          }
        }
        return this.buffSprite;
      }
    }

    public enum Type
    {
      PowerUp,
      Restore,
    }

    private enum State
    {
      OnCooldown,
      Ready,
    }
  }
}
