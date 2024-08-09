// Decompiled with JetBrains decompiler
// Type: CornerSpace.TimedEventManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class TimedEventManager
  {
    private List<TimedEventManager.TimedEvent> events;

    public TimedEventManager() => this.events = new List<TimedEventManager.TimedEvent>();

    public void ExecuteOnce(int timeDelay, System.Action eventFunction)
    {
      this.ExecuteMultipleTimes(timeDelay, 1, eventFunction);
    }

    public void ExecuteMultipleTimes(int timeDelay, int executionCount, System.Action eventFunction)
    {
      if (eventFunction == null)
        return;
      this.events.Add(new TimedEventManager.TimedEvent()
      {
        EventFunction = eventFunction,
        ExecutionCount = executionCount,
        ExecutionInterval = timeDelay,
        TimeUntilExecution = timeDelay
      });
    }

    public void Update(int deltaTime)
    {
      for (int index = this.events.Count - 1; index >= 0; --index)
      {
        TimedEventManager.TimedEvent timedEvent = this.events[index];
        timedEvent.TimeUntilExecution -= deltaTime;
        if (timedEvent.TimeUntilExecution <= 0)
        {
          try
          {
            timedEvent.EventFunction();
          }
          catch (Exception ex)
          {
            Engine.Console.WriteErrorLine("Exeption thrown in timed event function: " + ex.Message);
            this.events.RemoveAt(index);
            continue;
          }
          --timedEvent.ExecutionCount;
          timedEvent.TimeUntilExecution = timedEvent.ExecutionInterval;
          if (timedEvent.ExecutionCount <= 0)
            this.events.RemoveAt(index);
        }
      }
    }

    private class TimedEvent
    {
      public System.Action EventFunction;
      public int TimeUntilExecution;
      public int ExecutionInterval;
      public int ExecutionCount;
    }
  }
}
