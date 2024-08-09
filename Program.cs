// Decompiled with JetBrains decompiler
// Type: CornerSpace.Program
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  internal static class Program
  {
    private static void Main(string[] args)
    {
      using (Engine engine = new Engine(Program.ParseLaunchParameters(args)))
        engine.Run();
    }

    private static Engine.LaunchParameters ParseLaunchParameters(string[] args)
    {
      Engine.LaunchParameters launchParameters = Engine.LaunchParameters.Normal;
      if (args != null)
      {
        for (int index = 0; index < args.Length; ++index)
        {
          if (!string.IsNullOrEmpty(args[index]))
          {
            switch (args[index].ToLower())
            {
              case "debug":
                launchParameters |= Engine.LaunchParameters.Debug;
                continue;
              case "fasttravel":
                launchParameters |= Engine.LaunchParameters.FastTravel;
                continue;
              default:
                continue;
            }
          }
        }
      }
      return launchParameters;
    }
  }
}
