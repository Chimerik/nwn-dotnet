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
    private void InitializeCreatureEvents(NwCreature creature)
    {
      creature.OnHeartbeat += CheckIfNoPlayerAround;
      creature.OnDeath += CreatureUtils.MakeInventoryUndroppable;
      creature.OnDeath += CreatureUtils.OnMobDeathResetSpawn;
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
      // HP - Min damage - Max damage - Crit chance - AC
      if (creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").HasValue)
        return;

      switch(creature.Tag)
      {
        case "Gobelinclaireur":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 1;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 0);
          creature.BaseAttackCount = 1;

          break;

        case "Gobelinchairacanon":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 2;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "Gobelinfrondeur":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 3;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 0);
          creature.BaseAttackCount = 1;

          break;

        case "Gobelinfourbe":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 2;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 0);
          creature.BaseAttackCount = 2;

          break;

        case "boss_gobelin":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 0);
          creature.BaseAttackCount = 1;

          break;

        case "Koboltfantassin":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 4;
          creature.MaxHP = 50;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "Koboltsournois":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 3;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 0);
          creature.BaseAttackCount = 1;

          break;

        case "boss_kobolt":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.MaxHP = 80;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "Cougar":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 1;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 0);
          creature.BaseAttackCount = 1;

          break;

        case "Leopard":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 50;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "Lynx":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 50;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "Scarabegant":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "NW_BTLFIRE02":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 50;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "NW_BTLSTINK":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 50;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "chauvesourisvampire":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 30;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 3;

          break;

        case "Banditdemiorc":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 9;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 120;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "Banditelfe":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 90;
          CreaturePlugin.SetBaseAC(creature, 0);
          creature.BaseAttackCount = 1;

          break;

        case "Bandithalfelin":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 90;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 2;

          break;

        case "boss_bandit":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 160;
          CreaturePlugin.SetBaseAC(creature, 21);
          creature.BaseAttackCount = 1;

          break;

        case "StatueCristalline":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 1;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 2;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "KuoToa":
        case "KuoToafouettard":
        case "KuoToasurveillant":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 2;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "boss_harpie":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 100;
          CreaturePlugin.SetBaseAC(creature, 21);
          creature.BaseAttackCount = 1;

          break;

        case "ourslonguegriffe":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 50;
          CreaturePlugin.SetBaseAC(creature, 15);
          creature.BaseAttackCount = 1;

          break;

        case "boss_gothra":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 17;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 200;
          CreaturePlugin.SetBaseAC(creature, 60);
          creature.BaseAttackCount = 1;

          break;

        case "MercenairedelOmniscienceranged":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 7);
          creature.BaseAttackCount = 1;

          break;

        case "MercenairedelOmniscience":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 2;
          creature.MaxHP = 80;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "HerautdelOmniscience":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 2;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "Moinedelomniscience":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 21);
          creature.BaseAttackCount = 3;

          break;

        case "RodeurdelOmniscience":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 9;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 15;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "PretredelOmniscience":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 11;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 100;
          CreaturePlugin.SetBaseAC(creature, 28);
          creature.BaseAttackCount = 1;

          break;

        case "PaladindelOmniscience":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 14;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 120;
          CreaturePlugin.SetBaseAC(creature, 40);
          creature.BaseAttackCount = 1;

          break;

        case "PrecheurdelOmniscience":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 7;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 100;
          CreaturePlugin.SetBaseAC(creature, 30);
          creature.BaseAttackCount = 1;

          break;

        case "boss_omni":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 10;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 18;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 220;
          CreaturePlugin.SetBaseAC(creature, 60);
          creature.BaseAttackCount = 2;

          break;

        case "Gobeloursvicieux":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 20;
          creature.MaxHP = 50;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 2;

          break;

        case "Gobeloursshaman":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 80;
          CreaturePlugin.SetBaseAC(creature, 18);
          creature.BaseAttackCount = 1;

          break;

        case "Gobeloursfurieux":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 12;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 18);
          creature.BaseAttackCount = 2;

          break;

        case "Gobelourscombattant":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 9;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 90;
          CreaturePlugin.SetBaseAC(creature, 24);
          creature.BaseAttackCount = 1;

          break;

        case "boss_gobelours":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 20;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 120;
          CreaturePlugin.SetBaseAC(creature, 30);
          creature.BaseAttackCount = 2;

          break;

        case "jaguar":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "panthere":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 15;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "oursalunettes":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 18;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 180;
          CreaturePlugin.SetBaseAC(creature, 40);
          creature.BaseAttackCount = 1;

          break;

        case "Fourmisoldat":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 40;
          CreaturePlugin.SetBaseAC(creature, 40);
          creature.BaseAttackCount = 1;

          break;

        case "Fourmigarderoyal":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 10;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 100;
          CreaturePlugin.SetBaseAC(creature, 40);
          creature.BaseAttackCount = 2;

          break;

        case "boss_ant":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 8;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 15;
          creature.MaxHP = 200;
          CreaturePlugin.SetBaseAC(creature, 40);
          creature.BaseAttackCount = 1;

          break;

        case "gnollwarrior":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 80;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "gnollranger":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 2;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 70;
          CreaturePlugin.SetBaseAC(creature, 10);
          creature.BaseAttackCount = 1;

          break;

        case "Gnollshaman":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 3;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 5;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 100;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 1;

          break;

        case "boss_gnoll":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 9;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 160;
          CreaturePlugin.SetBaseAC(creature, 14);
          creature.BaseAttackCount = 2;

          break;

        case "Orceclaireur":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 8;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 60;
          CreaturePlugin.SetBaseAC(creature, 21);
          creature.BaseAttackCount = 1;

          break;

        case "Orcchoc":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 6;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 14;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 120;
          CreaturePlugin.SetBaseAC(creature, 40);
          creature.BaseAttackCount = 1;

          break;

        case "Orctireur":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 8;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 10;
          creature.MaxHP = 80;
          CreaturePlugin.SetBaseAC(creature, 30);
          creature.BaseAttackCount = 2;

          break;

        case "Orcshaman":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 4;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 10;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 5;
          creature.MaxHP = 140;
          CreaturePlugin.SetBaseAC(creature, 30);
          creature.BaseAttackCount = 1;

          break;

        case "boss_orc":

          creature.GetObjectVariable<LocalVariableInt>("_MIN_CREATURE_DAMAGE").Value = 12;
          creature.GetObjectVariable<LocalVariableInt>("_MAX_CREATURE_DAMAGE").Value = 20;
          creature.GetObjectVariable<LocalVariableInt>("_ADD_CRIT_CHANCE").Value = 15;
          creature.MaxHP = 280;
          CreaturePlugin.SetBaseAC(creature, 60);
          creature.BaseAttackCount = 3;

          break;
      }
    }
  }
}
