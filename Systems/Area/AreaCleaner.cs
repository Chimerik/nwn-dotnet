﻿using Anvil.API;
using Anvil.Services;
using System;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private void CleanArea(NwArea area)
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
        if(item.Possessor == null)
        {
          Log.Info($"destroying item {item.Name}");
          item.Destroy();
        }
      }
    }
    private ScheduledTask areaDestroyerScheduler;
    private ScheduledTask areaCleanerCheckScheduler;
    private ScheduledTask areaCleanerScheduler;
    private void AreaCleaner(NwArea area)
    {
      Log.Info($"Initiating cleaning for area {area.Name}");

      areaCleanerCheckScheduler = scheduler.ScheduleRepeating(() =>
      {
        Log.Info("areaCleanerCheckScheduler on");
        if (area.PlayerCount > 1)
        {
          areaCleanerScheduler.Dispose();
          areaCleanerCheckScheduler.Dispose();
          Log.Info($"Canceling cleaning for area {area.Name}");
        }
        Log.Info("areaCleanerCheckScheduler off");
      } , TimeSpan.FromSeconds(10));

      Log.Info("areaCleanerCheckScheduler OK");

      areaCleanerScheduler = scheduler.Schedule(() =>
      {
        Log.Info($"Cleaning area {area.Name}");
        areaCleanerCheckScheduler.Dispose();

        foreach (NwPlaceable bodyBag in area.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "BodyBag"))
        {
          Utils.DestroyInventory(bodyBag);
          Log.Info($"destroying body bag {bodyBag.Name}");
          bodyBag.Destroy();
        }

        foreach (NwItem item in area.FindObjectsOfTypeInArea<NwItem>().Where(i => i.Possessor == null))
        {
          Log.Info($"destroying item {item.Name}");
          item.Destroy();
        }
      } , TimeSpan.FromMinutes(25));

      Log.Info("areaCleanerScheduler OK");
    }
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
