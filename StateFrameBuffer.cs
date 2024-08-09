// Decompiled with JetBrains decompiler
// Type: CornerSpace.StateFrameBuffer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public class StateFrameBuffer
  {
    private const int cMaxStatesCount = 256;
    private StateFrame[] frames;
    private int newestFrameIndex;
    private int unhandledStates;

    public StateFrameBuffer(int size)
    {
      size = size > 0 ? Math.Min(size, 256) : throw new ArgumentException();
      this.frames = new StateFrame[size];
      this.newestFrameIndex = 0;
      this.unhandledStates = 0;
      this.Clear();
    }

    public void AddState(StateFrame newState)
    {
      int newestFrameIndex = this.newestFrameIndex;
      if (newState.Tick <= this.frames[newestFrameIndex].Tick)
        return;
      this.newestFrameIndex = (this.newestFrameIndex + 1) % this.frames.Length;
      ++this.unhandledStates;
      this.frames[this.newestFrameIndex] = newState;
    }

    public void Clear()
    {
      for (int index = 0; index < this.frames.Length; ++index)
        this.frames[index].Initialize();
    }

    public StateFrame EvaluateInterpolationState(long ticks)
    {
      for (int index1 = 0; index1 < this.frames.Length; ++index1)
      {
        int index2 = this.newestFrameIndex - index1;
        while (index2 < 0)
          index2 += this.frames.Length;
        if (this.frames[index2].Tick == ticks)
          return this.frames[index2];
        if (this.frames[index2].Tick < ticks)
        {
          StateFrame frame1 = this.frames[index2];
          StateFrame frame2 = this.frames[index2 + 1 >= this.frames.Length ? index2 + 1 - this.frames.Length : index2 + 1];
          float amount = MathHelper.Clamp((float) ((ticks - frame1.Tick) / (frame2.Tick - frame1.Tick)), 0.0f, 1f);
          return new StateFrame()
          {
            Position = frame1.Position + (frame2.Position - frame1.Position) * amount,
            Orientation = Quaternion.Lerp(frame1.Orientation, frame2.Orientation, amount)
          };
        }
      }
      return new StateFrame();
    }

    public StateFrame GetLatestState()
    {
      StateFrame frame = this.frames[this.newestFrameIndex];
      this.unhandledStates = 0;
      return frame;
    }

    public void GetInterpolatedValue(
      ref StateFrame beginState,
      ref StateFrame endState,
      ref float fraction,
      long ticks)
    {
      for (int index1 = 0; index1 < this.frames.Length; ++index1)
      {
        int index2 = this.newestFrameIndex - index1;
        while (index2 < 0)
          index2 += this.frames.Length;
        if (this.frames[index2].Tick > ticks)
        {
          int index3 = index2 - 1;
          if (index3 < 0)
            index3 += this.frames.Length;
          beginState = this.frames[index3];
          int index4 = (index3 + 1) % this.frames.Length;
          endState = this.frames[index4];
          if (endState.Tick <= beginState.Tick)
            break;
          fraction = (float) (ticks - beginState.Tick) / (float) (endState.Tick - beginState.Tick);
          break;
        }
      }
    }

    public bool GetOldestUnhandledState(ref StateFrame state)
    {
      if (this.unhandledStates <= 0)
        return false;
      int index = this.newestFrameIndex + 1 - this.unhandledStates;
      while (index < 0)
        index += this.frames.Length;
      state = this.frames[index];
      --this.unhandledStates;
      return true;
    }

    public bool HasUnhandledStates() => this.UnhandledStatesCount() > 0;

    public void InitializationState(StateFrame initializationFrame)
    {
      for (int index = 0; index < this.frames.Length; ++index)
        this.frames[index] = initializationFrame;
    }

    public StateFrame PeekHistoryState(int count)
    {
      count = Math.Abs(count);
      int index = this.newestFrameIndex - count;
      while (index < 0)
        index += this.frames.Length;
      return this.frames[index];
    }

    public StateFrame PeekLatestState() => this.frames[this.newestFrameIndex];

    public void RewindToTime(long ticks)
    {
      for (int index1 = 0; index1 < this.frames.Length; ++index1)
      {
        int index2 = this.newestFrameIndex - this.unhandledStates;
        while (index2 < 0)
          index2 += this.frames.Length;
        if (this.frames[index2].Tick <= ticks)
          break;
        ++this.unhandledStates;
      }
    }

    public int UnhandledStatesCount() => this.unhandledStates;
  }
}
