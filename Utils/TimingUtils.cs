using System;
using System.Collections.Generic;
using NWN.Core;

namespace NWN
{
  public static class TimingUtils
  {
    public static Action<T1> Debounce<T1>(Action<T1> action, float delay)
    {
      var cancelDelay = new Dictionary<int, bool>();
      int nextId = 0;
      return (T1) =>
      {
        if (cancelDelay.Count != 0)
        {
          cancelDelay[nextId - 1] = true;
        }
        cancelDelay.Add(nextId, false);

        // Copy id before incrementation for use in the DelayCommand
        var currentId = nextId;

        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(delay, () => {
          if (!cancelDelay[currentId])
          {
            action(T1);
          }
          cancelDelay.Remove(currentId);
        }));

        nextId++;
      };
    }
  }
}
