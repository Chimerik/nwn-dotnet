using NWN.API;
using NWN.Core;
using NWN.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private async static void AreaCleaner(NwArea area)
    {
      Log.Info($"Initiating cleaning for area {area.Name}");

      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task playerReturned = NwTask.WaitUntil(() => area.FindObjectsOfTypeInArea<NwCreature>().Any(p => p.IsPlayerControlled), tokenSource.Token);
      Task activateCleaner = NwTask.Delay(TimeSpan.FromMinutes(25), tokenSource.Token);
      await NwTask.WhenAny(playerReturned, activateCleaner);
      tokenSource.Cancel();

      if (playerReturned.IsCompletedSuccessfully)
      {
        Log.Info($"Canceling cleaning for area {area.Name}");
        return;
      }

      Log.Info($"Cleaning area {area.Name}");

      foreach (NwCreature creature in area.FindObjectsOfTypeInArea<NwCreature>().Where(c => c.Tag != "Statuereptilienne" && c.Tag != "Statuereptilienne2" && c.Tag != "statue_tiamat" && !c.IsDMPossessed && c.Tag != "pccorpse" && !c.IsPlayerControlled && !c.IsLoginPlayerCharacter))
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

        Utils.DestroyInventory(creature);
        creature.Destroy(0.2f);
      }

      foreach (NwGameObject bodyBag in area.FindObjectsOfTypeInArea<NwGameObject>().Where(o => o.Tag == "BodyBag"))
      {
        NWN.Utils.DestroyInventory(bodyBag);
        Log.Info($"destroying body bag {bodyBag.Name}");
        bodyBag.Destroy();
      }

      foreach (NwItem item in area.FindObjectsOfTypeInArea<NwItem>().Where(i => i.Possessor == null))
      {
        Log.Info($"destroying item {item.Name}");
        item.Destroy();
      }

      /*foreach (NwStore store in area.FindObjectsOfTypeInArea<NwStore>())
      {
        Log.Info($"destroying store {store.Name}");
        store.Destroy();
      }*/
    }
    public async static void AreaDestroyer(NwArea area)
    {
      await NwTask.WaitUntil(() => !area.FindObjectsOfTypeInArea<NwCreature>().Any(p => p.ControllingPlayer != null));
      await NwTask.WaitUntil(() => !NwModule.Instance.Players.Any(p => p.ControlledCreature.Location.Area == null));
      Log.Info($"Destroyed area {area.Name}");
      area.Destroy();
    }
  }
}
