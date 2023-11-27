using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NWN.Core.NWNX;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
          InitializeCreatureEvents(creature);
          InitializeGenericVariables(creature, spawnPoint);
          HandleSpawnSpecificBehaviour(creature, spawnPoint);
          InitializeCreatureStats(creature);
          //creature.ApplyEffect(EffectDuration.Instant, Effect.Death(false, false));
          //creature.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneParalyze());
        }
        else
          Utils.LogMessageToDMs($"SPAWN SYSTEM - Area {area.Name} - Could not spawn {spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value}");
      }
    }
    private static async void DelayVisualTransform(NwCreature creature, float scale, Vector3 translation, Vector3 rotation, int appearance)
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
    private async void InitializeCreatureEvents(NwCreature creature)
    {
      creature.OnHeartbeat += CheckIfNoPlayerAround;
      creature.OnHeartbeat += CreatureUtils.OnHeartbeatRefreshActions;
      creature.OnDeath += CreatureUtils.MakeInventoryUndroppable;
      creature.OnDeath += CreatureUtils.OnMobDeathResetSpawn;

      if (creature.Race.RacialType == RacialType.HalfOrc)
        creature.OnDamaged += CreatureUtils.HandleImplacableEndurance;

      if (creature.KnowsFeat(Feat.RapidReload))
        creature.OnCreatureAttack += CreatureUtils.OnAttackCrossbowMaster;

      if (creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.CogneurLourd)))
        creature.OnCreatureAttack += CreatureUtils.OnAttackCogneurLourd;

      if (creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TueurDeMage)))
        creature.OnCreatureAttack += CreatureUtils.OnAttackTueurDeMage;

      var creatureLoop = scheduler.ScheduleRepeating(() => CreatureUtils.CreatureHealthRegenLoop(creature), TimeSpan.FromSeconds(1));

      await NwTask.WaitUntil(() => creature == null || !creature.IsValid);
      creatureLoop.Dispose();
    }
    
    private static void InitializeGenericVariables(NwCreature creature, NwWaypoint spawnPoint) 
    {
      creature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = spawnPoint.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value;
      creature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value = spawnPoint;
      creature.GetObjectVariable<LocalVariableFloat>("_PERSONNAL_SPACE").Value = CreaturePlugin.GetPersonalSpace(creature);
      creature.GetObjectVariable<LocalVariableFloat>("_HEIGHT").Value = CreaturePlugin.GetHeight(creature);
      creature.GetObjectVariable<LocalVariableFloat>("_HIT_DISTANCE").Value = CreaturePlugin.GetHitDistance(creature);
      creature.GetObjectVariable<LocalVariableFloat>("_CREATURE_PERSONNAL_SPACE").Value = CreaturePlugin.GetCreaturePersonalSpace(creature);

      if (spawnPoint.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").HasValue)
        creature.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value = spawnPoint.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value;

      DelayVisualTransform(creature, spawnPoint.GetObjectVariable<LocalVariableFloat>("_CREATURE_SCALE").HasValue ? spawnPoint.GetObjectVariable<LocalVariableFloat>("_CREATURE_SCALE").Value : 1,
        spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_TRANSLATION").HasValue ? spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_TRANSLATION").Value.Position : Vector3.Zero,
        spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_ROTATION").HasValue ? spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_ROTATION").Value.Position : Vector3.Zero,
        spawnPoint.GetObjectVariable<LocalVariableInt>("_CREATURE_APPEARANCE").Value);
    }
    private static void InitializeCreatureStats(NwCreature creature)
    {
      // HP - Min damage - Max damage - Crit chance - AC - Nb attacks
      if (creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").HasValue)
        return;

      if(Config.creatureStats.TryGetValue(creature.Tag, out CreatureStats stats))
      {
        creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = stats.minDamage;
        creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = stats.maxDamage;
        creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = stats.critChance;
        creature.MaxHP = stats.HP;
        creature.HP = creature.MaxHP;
        CreaturePlugin.SetBaseAC(creature, stats.AC);
        creature.BaseAttackCount = stats.nbAttacks;
      }
    }
  }
}
