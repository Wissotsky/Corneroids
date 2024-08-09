// Decompiled with JetBrains decompiler
// Type: CornerSpace.StoredValues
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class StoredValues
  {
    private Dictionary<string, string> values;

    public StoredValues() => this.values = new Dictionary<string, string>();

    public void LoadFromXml(XDocument document)
    {
      if (document == null)
        return;
      foreach (XElement element in document.Root.Elements())
      {
        try
        {
          string key = element.Name.ToString();
          string str = element.Value;
          if (!string.IsNullOrEmpty(key))
          {
            if (!string.IsNullOrEmpty(str))
            {
              if (!this.values.ContainsKey(key))
                this.values.Add(key, str);
            }
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to load stored value: " + ex.Message);
        }
      }
    }

    public T RetrieveValue<T>(string name)
    {
      if (!string.IsNullOrEmpty(name))
      {
        try
        {
          if (this.values.ContainsKey(name))
            return (T) this.values[name];
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to retrieve value for name " + name + ": " + ex.Message);
        }
      }
      return default (T);
    }

    public void SaveToFile(string file)
    {
      if (string.IsNullOrEmpty(file))
        return;
      try
      {
        XDocument xdocument = new XDocument();
        XElement content = new XElement((XName) "values");
        xdocument.Add((object) content);
        foreach (KeyValuePair<string, string> keyValuePair in this.values)
        {
          try
          {
            content.Add((object) new XElement((XName) keyValuePair.Key, (object) new XText(keyValuePair.Value)));
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Failed to store custom value field: " + ex.Message);
          }
        }
        xdocument.Save(file);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to store custom values: " + ex.Message);
      }
    }

    public void StoreValue<T>(string name, T value)
    {
      if (string.IsNullOrEmpty(name))
        return;
      this.values.Remove(name);
      try
      {
        this.values.Add(name, value.ToString());
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to store a value: " + ex.Message);
      }
    }
  }
}
