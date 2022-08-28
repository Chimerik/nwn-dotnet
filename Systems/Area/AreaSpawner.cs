using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NWN.Core.NWNX;

using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AreaSystem))]
  partial class AreaSystem
  {
    private void CheckSpawns(NwArea area)
    {
      IEnumerable<NwPlayer> playersInArea = NwModule.Instance.Players.Where(p => p.ControlledCreature != null && p.ControlledCreature.Area == area);

      foreach (NwWaypoint spawnPoint in area.FindObjectsOfTypeInArea<NwWaypoint>())
      {
        if (spawnPoint.Tag != "creature_spawn" || spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").HasValue || !playersInArea.Any(p => p.ControlledCreature.DistanceSquared(spawnPoint) < 2026))
          continue;

        spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Value = true;

        if (CreatureUtils.creatureSpawnDictionary.ContainsKey(spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value))
        {
          NwCreature creature = CreatureUtils.creatureSpawnDictionary[spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value].Clone(spawnPoint.Location);

          creature.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneParalyze());
          creature.OnHeartbeat += CheckIfNoPlayerAround;
          creature.OnDeath += CreatureUtils.OnMobDeathResetSpawn; ;
          creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value = spawnPoint;
          creature.GetObjectVariable<LocalVariableFloat>("_PERSONNAL_SPACE").Value = CreaturePlugin.GetPersonalSpace(creature);
          creature.GetObjectVariable<LocalVariableFloat>("_HEIGHT").Value = CreaturePlugin.GetHeight(creature);
          creature.GetObjectVariable<LocalVariableFloat>("_HIT_DISTANCE").Value = CreaturePlugin.GetHitDistance(creature);
          creature.GetObjectVariable<LocalVariableFloat>("_CREATURE_PERSONNAL_SPACE").Value = CreaturePlugin.GetCreaturePersonalSpace(creature);
          HandleSpawnSpecificBehaviour(creature);
        }
        else
          Utils.LogMessageToDMs($"SPAWN SYSTEM - Area {area.Name} - Could not spawn {spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value}");
      }
    }

    private void CheckIfNoPlayerAround(CreatureEvents.OnHeartbeat onHB)
    {
      //Log.Info("start check if no one around");
      if (NwModule.Instance.Players.Any(p => p.ControlledCreature != null && p.ControlledCreature.Area == onHB.Creature.Area && p.ControlledCreature.DistanceSquared(onHB.Creature) < 2026))
      {
        //Log.Info("end check if no one around");
        return;
      }

      onHB.Creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
      onHB.Creature.OnDeath -= LootSystem.HandleLoot;
      onHB.Creature.OnDeath -= CreatureUtils.OnMobDeathResetSpawn;
      onHB.Creature.Destroy();

      //Log.Info("end check if no one around");
    }
  }
}
