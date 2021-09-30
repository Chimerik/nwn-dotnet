using Anvil.API;
using Anvil.Services;
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

      foreach (NwAreaOfEffect spawnAOE in area.FindObjectsOfTypeInArea<NwAreaOfEffect>().Where(a => a.Tag == "creature_spawn_aoe" || a.Tag == "creature_reset_spawn_aoe"))
        spawnAOE.Destroy();

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
    }
    public async static void AreaDestroyer(NwArea area)
    {
      await NwTask.WaitUntil(() => !NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == area));
      await NwTask.WaitUntil(() => !NwModule.Instance.Players.Any(p => p.ControlledCreature.Area == null));
      Log.Info($"Destroyed area {area.Name}");
      area.Destroy();
    }
  }
}
