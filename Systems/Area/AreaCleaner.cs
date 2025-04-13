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
        if(bodyBag.IsBodyBag)
        {
          Utils.DestroyInventory(bodyBag);
          LogUtils.LogMessage($"destroying body bag {bodyBag.Name}", LogUtils.LogType.AreaManagement);
          bodyBag.Destroy();
        }
      }

      foreach (NwItem item in area.FindObjectsOfTypeInArea<NwItem>())
      {
        if (item.RootPossessor == null && item.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").HasNothing)
        {
          LogUtils.LogMessage($"destroying item {item.Name}", LogUtils.LogType.AreaManagement);
          item.Destroy();
        }
      }
    }
    //private ScheduledTask areaDestroyerScheduler;
    public static async void AreaDestroyer(NwArea area)
    {
      if (area is null)
        return;

      if (area.PlayerCount < 1 && !NwModule.Instance.Players.Any(p => p?.ControlledCreature?.Area is null))
      {
        LogUtils.LogMessage($"Destroyed area {area.Name}", LogUtils.LogType.AreaManagement);
        area.Destroy();
      }
      else
      {
        await NwTask.Delay(TimeSpan.FromSeconds(6));
        AreaDestroyer(area);
      }

        /*areaDestroyerScheduler = scheduler.ScheduleRepeating(() =>
      {
        if (!NwModule.Instance.Players.Any(p => p.ControlledCreature == null && p.ControlledCreature.Area == null) && area.PlayerCount < 1)
        {
          LogUtils.LogMessage($"Destroyed area {area.Name}", LogUtils.LogType.AreaManagement);
          area.Destroy();
          areaDestroyerScheduler.Dispose();
        }
      }
        , TimeSpan.FromSeconds(6));*/
    }
  }
}
