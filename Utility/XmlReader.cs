// Decompiled with JetBrains decompiler
// Type: CornerSpace.Utility.XmlReader
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Globalization;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace.Utility
{
  public class XmlReader
  {
    private static XmlReader instance;

    private XmlReader()
    {
    }

    public bool ReadByte(string elementValue, out byte result)
    {
      try
      {
        result = Convert.ToByte(elementValue, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch
      {
        result = (byte) 0;
        return false;
      }
    }

    public bool ReadFloat(string elementValue, out float result)
    {
      try
      {
        result = Convert.ToSingle(elementValue, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch
      {
        result = 0.0f;
        return false;
      }
    }

    public bool ReadInt(string elementValue, out int result)
    {
      try
      {
        result = Convert.ToInt32(elementValue, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch
      {
        result = 0;
        return false;
      }
    }

    public bool ReadShort(string elementValue, out short result)
    {
      try
      {
        result = Convert.ToInt16(elementValue, (IFormatProvider) CultureInfo.InvariantCulture);
        return true;
      }
      catch
      {
        result = (short) 0;
        return false;
      }
    }

    public bool ReadString(string elementValue, out string result)
    {
      try
      {
        result = elementValue;
        return true;
      }
      catch
      {
        result = (string) null;
        return false;
      }
    }

    public T ReadAttributeValue<T>(
      XElement root,
      XmlReader.ConvertValue<T> convertFunction,
      string attributeName,
      T ifNull,
      params string[] structure)
    {
      if (root == null || convertFunction == null || string.IsNullOrEmpty(attributeName))
        return ifNull;
      XElement xelement = this.ReadElement(root, structure);
      if (xelement != null)
      {
        XAttribute xattribute = xelement.Attribute((XName) attributeName);
        T result;
        if (xattribute != null && !string.IsNullOrEmpty(xattribute.Value) && convertFunction(xattribute.Value, out result))
          return result;
      }
      return ifNull;
    }

    public T ReadElementValue<T>(
      XElement root,
      XmlReader.ConvertValue<T> convertFunction,
      T ifNull,
      params string[] structure)
    {
      if (root == null || convertFunction == null)
        return ifNull;
      XElement xelement = this.ReadElement(root, structure);
      T result;
      return xelement != null && !string.IsNullOrEmpty(xelement.Value) && convertFunction(xelement.Value, out result) ? result : ifNull;
    }

    public static XmlReader Instance
    {
      get
      {
        if (XmlReader.instance == null)
          XmlReader.instance = new XmlReader();
        return XmlReader.instance;
      }
    }

    private XElement ReadElement(XElement element, params string[] structure)
    {
      if (element == null)
        return (XElement) null;
      if (structure == null || structure.Length <= 0)
        return element;
      XElement xelement = element;
      foreach (string name in structure)
      {
        if (name != null)
        {
          xelement = xelement.Element((XName) name);
          if (xelement != null && name == structure[structure.Length - 1])
            return xelement;
        }
      }
      return (XElement) null;
    }

    public delegate bool ConvertValue<T>(string elementValue, out T result);
  }
}
