using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NWN.Core.NWNX;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Transactions;

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

          //creature.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneParalyze());
          creature.OnHeartbeat += CheckIfNoPlayerAround;
          creature.OnDeath += CreatureUtils.MakeInventoryUndroppable;
          creature.OnDeath += CreatureUtils.OnMobDeathResetSpawn;
          creature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = spawnPoint.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value;
          creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value = spawnPoint;
          creature.GetObjectVariable<LocalVariableFloat>("_PERSONNAL_SPACE").Value = CreaturePlugin.GetPersonalSpace(creature);
          creature.GetObjectVariable<LocalVariableFloat>("_HEIGHT").Value = CreaturePlugin.GetHeight(creature);
          creature.GetObjectVariable<LocalVariableFloat>("_HIT_DISTANCE").Value = CreaturePlugin.GetHitDistance(creature);
          creature.GetObjectVariable<LocalVariableFloat>("_CREATURE_PERSONNAL_SPACE").Value = CreaturePlugin.GetCreaturePersonalSpace(creature);

          if (spawnPoint.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").HasValue)
            creature.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value = spawnPoint.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value;

          HandleSpawnSpecificBehaviour(creature, spawnPoint);

          DelayVisualTransform(creature, spawnPoint.GetObjectVariable<LocalVariableFloat>("_CREATURE_SCALE").HasValue ? spawnPoint.GetObjectVariable<LocalVariableFloat>("_CREATURE_SCALE").Value : 1,
            spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_TRANSLATION").HasValue ? spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_TRANSLATION").Value.Position : Vector3.Zero,
            spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_ROTATION").HasValue ? spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_ROTATION").Value.Position : Vector3.Zero,
            spawnPoint.GetObjectVariable<LocalVariableInt>("_CREATURE_APPEARANCE").Value);
        }
        else
          Utils.LogMessageToDMs($"SPAWN SYSTEM - Area {area.Name} - Could not spawn {spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value}");
      }
    }
    private async void DelayVisualTransform(NwCreature creature, float scale, Vector3 translation, Vector3 rotation, int appearance)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));
      creature.Appearance = NwGameTables.AppearanceTable.GetRow(appearance);
      creature.VisualTransform.Scale = scale;
      creature.VisualTransform.Translation = translation;
      creature.VisualTransform.Rotation = rotation;
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
