using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using System;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private ScheduledTask spawnScheduler;
    private void CreateSpawnChecker(NwArea area)
    {
      spawnScheduler = ModuleSystem.scheduler.ScheduleRepeating(() =>
      {
        foreach (NwWaypoint spawnPoint in area.FindObjectsOfTypeInArea<NwWaypoint>().Where(wp => wp.Tag == "creature_spawn" && wp.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").HasNothing))
        {
          if (NwModule.Instance.Players.Any(p => p.ControlledCreature != null && p.ControlledCreature.Area == area && p.ControlledCreature.DistanceSquared(spawnPoint) < 2026))
          {
            spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Value = true;

            NwCreature creature = NwCreature.Deserialize(spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value.ToByteArray());
            if (creature != null)
            {
              Log.Info($"spawned : {creature.Name}");
              creature.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneParalyze());
              creature.Location = spawnPoint.Location;
              creature.OnHeartbeat += CheckIfNoPlayerAround;
              creature.OnDeath += OnMobDeathResetSpawn;
              creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value = spawnPoint;
              HandleSpawnSpecificBehaviour(creature);
            }
          }
        }

        if(!NwModule.Instance.Players.Any(p => p.ControlledCreature != null && p.ControlledCreature.Area == area))
          spawnScheduler.Dispose();
      }
        , TimeSpan.FromSeconds(1));      
    }

    private void CheckIfNoPlayerAround(CreatureEvents.OnHeartbeat onHB)
    {
      Log.Info("start check if no one around");
      if (NwModule.Instance.Players.Any(p => p.ControlledCreature != null && p.ControlledCreature.Area == onHB.Creature.Area && p.ControlledCreature.DistanceSquared(onHB.Creature) < 2026))
      {
        Log.Info("end check if no one around");
        return;
      }

      onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
      onHB.Creature.OnDeath -= LootSystem.HandleLoot;
      onHB.Creature.OnDeath -= OnMobDeathResetSpawn;
      onHB.Creature.Destroy();

      Log.Info("end check if no one around");
    }

    private void OnMobDeathResetSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint spawnPoint = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      ModuleSystem.scheduler.Schedule(() => { spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete(); }
      , TimeSpan.FromMinutes(10));
    }
  }
}
