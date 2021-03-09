using NWN.API;
using NWN.Core;
using NWN.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using static NWN.Systems.LootSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    public static void AreaSpawner(NwArea area)
    {
      if (area.GetLocalVariable<int>("_NO_SPAWN_ALLOWED").HasValue)
        return;

      //Log.Info($"Handling spawns for area {area.Name}");

      foreach (NwWaypoint wp in area.FindObjectsOfTypeInArea<NwWaypoint>().Where(a => a.Tag == "creature_spawn"))
        HandleSpawnWaypoint(wp);

      area.GetLocalVariable<int>("_NO_SPAWN_ALLOWED").Value = 1;

      Task spawnAllowed = NwTask.Run(async () =>
      {
        //Log.Info($"Spawns blocked for the next 10 minutes");
        await NwTask.Delay(TimeSpan.FromMinutes(10));
        area.GetLocalVariable<int>("_NO_SPAWN_ALLOWED").Delete();
        return true;
      });
    }
  }
}
