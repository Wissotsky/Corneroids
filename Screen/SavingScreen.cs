// Decompiled with JetBrains decompiler
// Type: CornerSpace.Screen.SavingScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;

#nullable disable
namespace CornerSpace.Screen
{
  public class SavingScreen : MessageScreen
  {
    private SavingScreen.PollFunction pollfunction;
    private System.Action savingDone;

    public SavingScreen(System.Action savingDoneDelegate, SavingScreen.PollFunction poll)
      : base("Please wait", "Saving", false)
    {
      this.pollfunction = poll != null ? poll : throw new ArgumentNullException();
      this.savingDone = savingDoneDelegate;
    }

    public override void Update()
    {
      base.Update();
      if (this.pollfunction())
        return;
      if (this.savingDone != null)
        this.savingDone();
      this.CloseScreen();
    }

    public delegate bool PollFunction();
  }
}
