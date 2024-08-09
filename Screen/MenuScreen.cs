// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.MenuScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace.Screen
{
  public abstract class MenuScreen : GameScreen
  {
    private bool escToExit;

    public MenuScreen(
      GameScreen.Type type,
      bool pausable,
      bool escToExit,
      bool listenResolutionChanges)
      : base(type, pausable, MouseDevice.Behavior.Free)
    {
      this.escToExit = escToExit;
      if (!listenResolutionChanges)
        return;
      Engine.SettingsManager.ResolutionChangedEvent += new Action<Point>(this.ResolutionChanged);
    }

    public override void Dispose()
    {
      base.Dispose();
      Engine.SettingsManager.ResolutionChangedEvent -= new Action<Point>(this.ResolutionChanged);
    }

    public override void UpdateInput()
    {
      if (!this.escToExit || !this.Keyboard.KeyPressed(Keys.Escape))
        return;
      this.CloseScreen();
    }

    protected virtual void ResolutionChanged(Point resolution)
    {
    }
  }
}
