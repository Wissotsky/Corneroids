// Decompiled with JetBrains decompiler
// Type: CornerSpace.GameScreens.Gui.TimedNotificationScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;

#nullable disable
namespace CornerSpace.GameScreens.Gui
{
  public class TimedNotificationScreen : GameScreen
  {
    private CaptionWindowLayer bgLayer;
    private System.Action done;

    public TimedNotificationScreen(string message, int time, System.Action done)
      : base(GameScreen.Type.Popup, false, MouseDevice.Behavior.Wrapped)
    {
    }
  }
}
