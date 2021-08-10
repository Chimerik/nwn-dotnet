using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anvil.API;

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

        Task debounce = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(delay));

          if (!cancelDelay[currentId])
          {
            action(T1);
          }
          cancelDelay.Remove(currentId);
        });

        nextId++;
      };
    }
  }
}
