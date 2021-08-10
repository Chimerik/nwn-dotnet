using Anvil.API;
using NWN.Core;
using Anvil.Services;
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
      if (area.GetObjectVariable<LocalVariableInt>("_NO_SPAWN_ALLOWED").HasValue)
        return;

      foreach (NwWaypoint wp in area.FindObjectsOfTypeInArea<NwWaypoint>().Where(a => a.Tag == "creature_spawn"))
        HandleSpawnWaypoint(wp);

      area.GetObjectVariable<LocalVariableInt>("_NO_SPAWN_ALLOWED").Value = 1;

      Task spawnAllowed = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromMinutes(10));
        area.GetObjectVariable<LocalVariableInt>("_NO_SPAWN_ALLOWED").Delete();
      });
    }
  }
}
