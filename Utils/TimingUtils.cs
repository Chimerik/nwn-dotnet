using System;
using System.Collections.Generic;
using NWN.Core;

namespace NWN
{
  public static class TimingUtils
  {
    public static Action Debounce(Action action, float delay)
    {
      var cancelDelay = new Dictionary<int, bool>();
      int nextId = 0;
      return () =>
      {
        if (cancelDelay.Count != 0)
        {
          cancelDelay[nextId - 1] = true;
        }
        cancelDelay.Add(nextId, false);

        // Copy id before incrementation for use in the DelayCommand
        var currentId = nextId;

        NWScript.DelayCommand(delay, () => {
          if (!cancelDelay[currentId])
          {
            action();
          }
          cancelDelay.Remove(currentId);
        });

        nextId++;
      };
    }
  }
}
