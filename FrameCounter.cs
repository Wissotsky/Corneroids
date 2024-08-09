// Decompiled with JetBrains decompiler
// Type: CornerSpace.FrameCounter
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System.Diagnostics;

#nullable disable
namespace CornerSpace
{
  public class FrameCounter : IElapsedTime
  {
    private const int cDeltaTimeBufferSize = 5;
    private float deltaTime;
    private long deltaTimeMS;
    private float[] deltaTimeBuffer;
    private long[] deltaTimeMSBuffer;
    private long ticks;
    private int? limitFrames;
    private int frameNumber;
    private long elapsedSecondsTotal;
    private Stopwatch stopwatch;

    public FrameCounter()
    {
      this.limitFrames = new int?();
      this.frameNumber = 0;
      this.deltaTime = 0.0f;
      this.stopwatch = new Stopwatch();
      this.ticks = 0L;
      this.deltaTimeBuffer = new float[5];
      this.deltaTimeMSBuffer = new long[5];
    }

    public void Execute()
    {
      this.frameNumber = (this.frameNumber + 1) % 60;
      if (this.stopwatch.IsRunning)
        this.stopwatch.Stop();
      this.deltaTimeMS = this.stopwatch.ElapsedMilliseconds;
      this.deltaTime = (float) this.deltaTimeMS / 1000f;
      this.ticks += this.deltaTimeMS;
      this.stopwatch.Reset();
      this.stopwatch.Start();
    }

    public void Initialize() => this.elapsedSecondsTotal = 0L;

    public int FrameNumber => this.frameNumber;

    public int? LimitFrameRate
    {
      get => this.limitFrames;
      set => this.limitFrames = value;
    }

    public float DeltaTime => this.deltaTime;

    public long DeltaTimeMS => this.deltaTimeMS;

    public long Ticks => this.ticks;
  }
}
