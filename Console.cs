// Decompiled with JetBrains decompiler
// Type: CornerSpace.Console
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace CornerSpace
{
  public class Console
  {
    private const int maxLines = 300;
    private Queue<string> errorLines;
    private readonly float consoleVersion;
    private Queue<string> networkLogLines;

    public Console()
    {
      this.errorLines = new Queue<string>();
      this.networkLogLines = new Queue<string>();
      this.consoleVersion = 1f;
    }

    public void LogNetworkTraffic(string line)
    {
      if (line == null)
        return;
      lock (this.networkLogLines)
      {
        if (this.networkLogLines.Count >= 300)
          this.networkLogLines.Dequeue();
        this.networkLogLines.Enqueue(line);
      }
    }

    public void SaveToFile(string folderPath)
    {
      lock (this.errorLines)
      {
        try
        {
          using (TextWriter textWriter = (TextWriter) new StreamWriter(Path.Combine(folderPath, "errorlog.txt")))
          {
            textWriter.WriteLine("Corneroids error log (v." + this.consoleVersion.ToString() + ")");
            textWriter.WriteLine("Timestamp: " + (object) DateTime.Now);
            textWriter.WriteLine("---------------------------------------");
            foreach (string errorLine in this.errorLines)
              textWriter.WriteLine(errorLine);
          }
        }
        catch
        {
        }
      }
      lock (this.networkLogLines)
      {
        try
        {
          using (TextWriter textWriter = (TextWriter) new StreamWriter(Path.Combine(folderPath, "networklog.txt")))
          {
            textWriter.WriteLine("Corneroids network traffic log (v." + this.consoleVersion.ToString() + ")");
            textWriter.WriteLine("Timestamp: " + (object) DateTime.Now);
            textWriter.WriteLine("---------------------------------------");
            foreach (string networkLogLine in this.networkLogLines)
              textWriter.WriteLine(networkLogLine);
          }
        }
        catch
        {
        }
      }
    }

    public void WriteErrorLine(string line)
    {
      if (line == null)
        return;
      lock (this.errorLines)
      {
        if (this.errorLines.Count >= 300)
          this.errorLines.Dequeue();
        this.errorLines.Enqueue(DateTime.Now.ToString() + " " + line);
      }
    }

    public delegate void Command(string[] parameters, List<string> result);
  }
}
