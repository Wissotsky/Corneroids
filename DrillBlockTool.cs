// Decompiled with JetBrains decompiler
// Type: CornerSpace.DrillBlockTool
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class DrillBlockTool : ToolItem
  {
    private const int cTimeBetweenSparksMS = 16;
    private int alarmLightTime;
    private float vibrationValue;
    private int power;
    private int sparkTimerMS;
    private Block targetedBlock;
    private int timeDrilled;

    public DrillBlockTool()
    {
      this.UseSelectionBox = false;
      this.SelectorColor = new Color(byte.MaxValue, (byte) 0, (byte) 0);
      this.vibrationValue = 0.0f;
      Item.random = new Random();
      this.timeDrilled = 0;
    }

    public override void GetStatistics(List<string> lines, bool nameOnly)
    {
      base.GetStatistics(lines, nameOnly);
      lines.Add("Drill power: " + (object) this.power);
    }

    public override Item.UsageResult UpdateInput(InputFrame input, Player owner, float powerToUse)
    {
      if (input.LeftDown)
      {
        if (this.pickedWorldPosition.HasValue)
        {
          BlockCell blockCell = Item.entityManager.PickBlock(this.pickedWorldPosition.Value);
          if (blockCell != null)
          {
            this.LightOn = true;
            this.Light.Color = this.Color;
            this.LightOffset = this.pickedWorldPosition.Value - owner.Astronaut.Position - owner.Astronaut.GetLookatVector() * 0.2f;
            Color color = Color.Gray;
            if (blockCell.Block != this.targetedBlock)
            {
              this.targetedBlock = blockCell.Block;
              this.timeDrilled = 0;
            }
            else
            {
              this.timeDrilled += (int) Engine.FrameCounter.DeltaTimeMS;
              int power = this.power;
              int powerToDrill = (int) this.targetedBlock.GetBlockType().PowerToDrill;
              if (power >= powerToDrill)
              {
                if (this.timeDrilled >= Math.Max(Math.Min(800 - (power - powerToDrill) * 100, 800), 300))
                {
                  Item.entityManager.DrillBlock(this.pickedWorldPosition.Value, this.power, owner.Id);
                  this.targetedBlock = (Block) null;
                  this.timeDrilled = 0;
                }
              }
              else
                color = Color.Yellow;
            }
            if (Item.environmentManager != null)
            {
              this.sparkTimerMS += (int) Engine.FrameCounter.DeltaTimeMS;
              if (this.sparkTimerMS >= 16)
              {
                Item.environmentManager.AddSpark(this.pickedWorldPosition.Value - owner.Astronaut.GetLookatVector() * 0.2f, -owner.Astronaut.GetLookatVector(), owner.Astronaut.GetUpVector(), 1.57079637f, color);
                this.sparkTimerMS %= 16;
              }
            }
          }
        }
        else
        {
          this.vibrationValue = 0.0f;
          this.LightOn = false;
          this.targetedBlock = (Block) null;
          this.timeDrilled = 0;
        }
        this.vibrationValue = (float) Item.random.NextDouble() * 0.005f;
        this.LightRadiusOffset = (float) ((Item.random.NextDouble() - 0.5) * 0.30000001192092896);
      }
      else
      {
        this.vibrationValue = 0.0f;
        this.LightOn = false;
        this.targetedBlock = (Block) null;
        this.timeDrilled = 0;
      }
      this.ModelOffset = Vector3.One * this.vibrationValue;
      return Item.UsageResult.None;
    }

    public override void LoadFromXml(XElement element)
    {
      base.LoadFromXml(element);
      XmlReader instance = XmlReader.Instance;
      this.power = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "power");
      this.power = Math.Max(0, this.power);
    }

    public int DrillPower
    {
      get => this.power;
      set => this.power = value;
    }
  }
}
