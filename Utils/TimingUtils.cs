using System;
using System.Collections.Generic;
using NWN.Core;

namespace NWN
{
  public static class TimingUtils
  {
    public static Action Debounce(Action action, float delay)
    {
      var cancelDelay = new List<bool>();
      return () =>
      {
        if (cancelDelay.Count != 0)
        {
          cancelDelay[cancelDelay.Count - 1] = true;
        }
        cancelDelay.Add(false);
        var currentIndex = cancelDelay.Count - 1;

        NWScript.DelayCommand(delay, () => { 
          if (!cancelDelay[currentIndex])
          {
            action();
          }
        });
      };
    }
  }
}
