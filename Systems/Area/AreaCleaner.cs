using NWN.API;
using NWN.Core;
using NWN.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private async static void AreaCleaner(NwArea area)
    {
      Log.Info($"Initiating cleaning for area {area.Name}");

      Task playerReturned = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => area.FindObjectsOfTypeInArea<NwPlayer>().Count(p => !p.IsDM && !p.IsDMPossessed && !p.IsPlayerDM) > 0);
        return true;
      });

      Task activateCleaner = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromMinutes(25));
        return true;
      });

      await NwTask.WhenAny(playerReturned, activateCleaner);

      if (playerReturned.IsCompletedSuccessfully)
      {
        Log.Info($"Canceling cleaning for area {area.Name}");
        return;
      }

      Log.Info($"Cleaning area {area.Name}");

      foreach (NwCreature creature in area.FindObjectsOfTypeInArea<NwCreature>().Where(c => c.Tag != "Statuereptilienne" && c.Tag != "Statuereptilienne2" && !c.IsDMPossessed && c.Tag != "pccorpse" && !(c is NwPlayer)))
      {
        if (creature.GetLocalVariable<API.Location>("_SPAWN_LOCATION").HasValue)
        {
          Task createWP = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.1));

            NwWaypoint waypoint = NwWaypoint.Create(creature.GetLocalVariable<string>("_WAYPOINT_TEMPLATE"), creature.GetLocalVariable<API.Location>("_SPAWN_LOCATION").Value);
            waypoint.GetLocalVariable<string>("_CREATURE_TEMPLATE").Value = creature.ResRef;

            return true;
          });

        }
        else if (creature.Tag == "mineable_animal")
        {
          Task createWP = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            NwWaypoint.Create("animal_spawn_wp", creature.Location);
            return true;
          });
        }

        Log.Info($"destroying creature {creature.Name}");
        NWN.Utils.DestroyInventory(creature);
        creature.Destroy(0.2f);
      }

      foreach (NwGameObject bodyBag in area.FindObjectsOfTypeInArea<NwGameObject>().Where(o => o.Tag == "BodyBag"))
      {
        NWN.Utils.DestroyInventory(bodyBag);
        Log.Info($"destroying body bag {bodyBag.Name}");
        bodyBag.Destroy();
      }

      foreach (NwItem item in area.FindObjectsOfTypeInArea<NwItem>())
      {
        Log.Info($"destroying item {item.Name}");
        item.Destroy();
      }

      foreach (NwStore store in area.FindObjectsOfTypeInArea<NwStore>())
      {
        Log.Info($"destroying store {store.Name}");
        store.Destroy();
      }
    }
    public async static void AreaDestroyer(NwArea area)
    {
      await NwTask.WaitUntil(() => area.FindObjectsOfTypeInArea<NwPlayer>().Count() == 0);
      await NwTask.WaitUntil(() => NwModule.FindObjectsOfType<NwPlayer>().Where(p => p.Area is null).Count() == 0);
      area.Destroy();
    }
  }
}
