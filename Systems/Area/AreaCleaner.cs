using Anvil.API;
using NWN.Core;
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
    private static void ResetAreaSpawns(NwArea area, bool playerInArea)
    {
      Log.Info($"{area.Name} resetting spawns");
      foreach (NwAreaOfEffect aoe in area.FindObjectsOfTypeInArea<NwAreaOfEffect>().Where(a => a.Tag == "creature_exit_range_aoe"))
      {
        NwCreature mob = aoe.GetObjectVariable<LocalVariableObject<NwCreature>>("creature").Value;
        NwWaypoint spawnPoint = mob.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value;

        if (mob.IsValid)
        {
          mob.OnDeath -= LootSystem.HandleLoot;
          mob.OnDeath -= OnMobDeathResetSpawn;

          if (mob.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").HasValue)
            mob.GetObjectVariable<LocalVariableObject<NwAreaOfEffect>>("reset_aoe").Value.Destroy();

          mob.Destroy();
          Log.Info($"{mob.Name} destroyed");
        }

        if(spawnPoint.IsValid)
          spawnPoint.GetObjectVariable<LocalVariableBool>("active").Delete();

        if (!playerInArea)
          continue;

        Effect spawnEffect = Effect.AreaOfEffect(198, "enterSpawn", "b", "c");
        spawnEffect.SubType = EffectSubType.Supernatural;
        spawnEffect.Tag = "creature_spawn_aoe";
        spawnPoint.Location.ApplyEffect(EffectDuration.Permanent, spawnEffect);

        NwAreaOfEffect spawnAoE = (NwAreaOfEffect)NwModule.Instance.GetLastCreatedObjects().FirstOrDefault(aoe => aoe is NwAreaOfEffect);
        spawnAoE.GetObjectVariable<LocalVariableString>("creature").Value = spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value;
        spawnAoE.GetObjectVariable<LocalVariableObject<NwWaypoint>>("spawn_wp").Value = spawnPoint;
        spawnAoE.Tag = "creature_spawn_aoe";

        Log.Info($"{mob.Name} spawn AoE restored");
      }
    }
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
      await NwTask.WaitUntil(() => !area.FindObjectsOfTypeInArea<NwCreature>().Any(p => p.ControllingPlayer != null));
      await NwTask.WaitUntil(() => !NwModule.Instance.Players.Any(p => p.ControlledCreature.Location.Area == null));
      Log.Info($"Destroyed area {area.Name}");
      area.Destroy();
    }
  }
}
