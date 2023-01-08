using Anvil.API;
using Anvil.Services;

using System;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private static void CleanArea(NwArea area)
    {
      foreach (NwPlaceable bodyBag in area.FindObjectsOfTypeInArea<NwPlaceable>())
      {
        if (bodyBag.Tag == "BodyBag")
        {
          Utils.DestroyInventory(bodyBag);
          Log.Info($"destroying body bag {bodyBag.Name}");
          bodyBag.Destroy();
        }
      }

      foreach (NwItem item in area.FindObjectsOfTypeInArea<NwItem>())
      {
        if (item.Possessor == null)
        {
          Log.Info($"destroying item {item.Name}");
          item.Destroy();
        }
      }
    }
    private ScheduledTask areaDestroyerScheduler;
    public void AreaDestroyer(NwArea area)
    {
      areaDestroyerScheduler = scheduler.ScheduleRepeating(() =>
      {
        if (!NwModule.Instance.Players.Any(p => p.ControlledCreature == null && p.ControlledCreature.Area == null) && area.PlayerCount < 1)
        {
          Log.Info($"Destroyed area {area.Name}");
          area.Destroy();
          areaDestroyerScheduler.Dispose();
        }
      }
        , TimeSpan.FromSeconds(6));
    }
  }
}
