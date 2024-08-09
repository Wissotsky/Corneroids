// Decompiled with JetBrains decompiler
// Type: CornerSpace.ControlsManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class ControlsManager
  {
    private Dictionary<ControlsManager.SpecialKey, string> keyNameMappings = new Dictionary<ControlsManager.SpecialKey, string>()
    {
      {
        ControlsManager.SpecialKey.BeaconTool,
        "Beacon tool"
      },
      {
        ControlsManager.SpecialKey.OrientateBlock,
        "Orientate block"
      },
      {
        ControlsManager.SpecialKey.StrafeLeft,
        "Strafe left"
      },
      {
        ControlsManager.SpecialKey.StrafeRight,
        "Strafe right"
      },
      {
        ControlsManager.SpecialKey.RollLeft,
        "Roll left"
      },
      {
        ControlsManager.SpecialKey.RollRight,
        "Roll right"
      }
    };
    private Dictionary<Keys, string> keysStringPresentation;
    private float mouseSensitivity;
    private Keys[] specialKeys;

    public ControlsManager()
    {
      this.keysStringPresentation = new Dictionary<Keys, string>();
      this.specialKeys = new Keys[15];
      this.mouseSensitivity = 1f;
      this.InitializeStringPresentations();
    }

    public string ControlStringPresentation(ControlsManager.SpecialKey key)
    {
      return this.keyNameMappings.ContainsKey(key) ? this.keyNameMappings[key] : key.ToString();
    }

    public Keys GetSpecialKey(ControlsManager.SpecialKey key)
    {
      int index = (int) key;
      return index >= 0 && index < this.specialKeys.Length ? this.specialKeys[index] : Keys.None;
    }

    public string KeyStringPresentation(Keys key)
    {
      return this.keysStringPresentation.ContainsKey(key) ? this.keysStringPresentation[key] : key.ToString();
    }

    public void ParseFromXML(XElement controlsEle)
    {
      if (controlsEle == null)
        return;
      try
      {
        XmlReader instance = XmlReader.Instance;
        controlsEle.Element((XName) "specialKeys");
        this.mouseSensitivity = instance.ReadElementValue<float>(controlsEle, new XmlReader.ConvertValue<float>(instance.ReadFloat), 0.5f, "sensitivity");
        this.SetSpecialKey(ControlsManager.SpecialKey.Use, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 69, "specialKeys", "use"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Edit, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 70, "specialKeys", "edit"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Inventory, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 67, "specialKeys", "inventory"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Forward, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 87, "specialKeys", "forward"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Backward, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 83, "specialKeys", "backward"));
        this.SetSpecialKey(ControlsManager.SpecialKey.StrafeLeft, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 65, "specialKeys", "left"));
        this.SetSpecialKey(ControlsManager.SpecialKey.StrafeRight, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 68, "specialKeys", "right"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Boost, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 160, "specialKeys", "boost"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Up, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 32, "specialKeys", "up"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Down, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 162, "specialKeys", "down"));
        this.SetSpecialKey(ControlsManager.SpecialKey.OrientateBlock, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 82, "specialKeys", "orientate"));
        this.SetSpecialKey(ControlsManager.SpecialKey.BeaconTool, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 66, "specialKeys", "beaconTool"));
        this.SetSpecialKey(ControlsManager.SpecialKey.RollLeft, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 90, "specialKeys", "rollLeft"));
        this.SetSpecialKey(ControlsManager.SpecialKey.RollRight, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 88, "specialKeys", "rollRight"));
        this.SetSpecialKey(ControlsManager.SpecialKey.Flashlight, (Keys) instance.ReadElementValue<int>(controlsEle, new XmlReader.ConvertValue<int>(instance.ReadInt), 84, "specialKeys", "flashlight"));
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to parse controls: " + ex.Message);
      }
    }

    public void SaveToXML(XElement controlsEle)
    {
      try
      {
        if (controlsEle == null)
          return;
        XElement xelement1 = controlsEle.Element((XName) "sensitivity");
        XElement xelement2 = controlsEle.Element((XName) "specialKeys");
        xelement1.Value = this.mouseSensitivity.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "edit").Value = ((int) this.specialKeys[1]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "use").Value = ((int) this.specialKeys[0]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "inventory").Value = ((int) this.specialKeys[10]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "forward").Value = ((int) this.specialKeys[2]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "backward").Value = ((int) this.specialKeys[3]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "left").Value = ((int) this.specialKeys[4]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "right").Value = ((int) this.specialKeys[5]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "boost").Value = ((int) this.specialKeys[6]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "up").Value = ((int) this.specialKeys[7]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "down").Value = ((int) this.specialKeys[8]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "orientate").Value = ((int) this.specialKeys[9]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "beaconTool").Value = ((int) this.specialKeys[11]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "rollLeft").Value = ((int) this.specialKeys[12]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "rollRight").Value = ((int) this.specialKeys[13]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xelement2.Element((XName) "flashlight").Value = ((int) this.specialKeys[14]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to save controls: " + ex.Message);
      }
    }

    public void SetSpecialKey(ControlsManager.SpecialKey key, Keys value)
    {
      int index1 = (int) key;
      for (int index2 = 0; index2 < this.specialKeys.Length; ++index2)
      {
        if (this.specialKeys[index2] == value)
          this.specialKeys[index2] = Keys.None;
      }
      if (index1 < 0 || index1 >= this.specialKeys.Length)
        return;
      this.specialKeys[index1] = value;
    }

    public Keys[] AllSpecialKeys => this.specialKeys;

    public float MouseSensitivity
    {
      get => this.mouseSensitivity;
      set => this.mouseSensitivity = (float) (int) ((double) value * 100.0) / 100f;
    }

    private void InitializeStringPresentations()
    {
    }

    public enum SpecialKey : byte
    {
      Use,
      Edit,
      Forward,
      Backward,
      StrafeLeft,
      StrafeRight,
      Boost,
      Up,
      Down,
      OrientateBlock,
      Inventory,
      BeaconTool,
      RollLeft,
      RollRight,
      Flashlight,
    }
  }
}
