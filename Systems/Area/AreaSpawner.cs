using NWN.API;
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
    private static void AreaSpawner(NwArea area)
    {
      if (area.GetLocalVariable<int>("_NO_SPAWN_ALLOWED").HasValue)
        return;

      foreach (NwWaypoint wp in area.FindObjectsOfTypeInArea<NwWaypoint>().Where(a => a.Tag == "creature_spawn"))
        HandleSpawnWaypoint(wp);

      foreach (NwPlaceable chest in area.FindObjectsOfTypeInArea<NwPlaceable>().Where(c => c.GetLocalVariable<string>("_LOOT_REFERENCE").HasValue))
      {
        Utils.DestroyInventory(chest); 

        if (lootablesDic.TryGetValue(chest.Tag, out Lootable.Config lootableConfig))
        {
          Task generateLoot = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.1));
            await area.AddActionToQueue(() => lootableConfig.GenerateLoot(chest));
            return true;
          });
        }
        else
          NWN.Utils.LogMessageToDMs($"AREA - {area.Name} - Unregistered container tag=\"{chest.Tag}\", name : {chest.Name}");
      }

      area.GetLocalVariable<int>("_NO_SPAWN_ALLOWED").Value = 1;

      Task spawnAllowed = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromMinutes(10));
        area.GetLocalVariable<int>("_NO_SPAWN_ALLOWED").Delete();
        return true;
      });
    }
  }
}
