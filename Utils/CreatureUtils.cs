﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using NWN.Systems;

namespace NWN
{
  public static class CreatureUtils
  {
    public static Dictionary<string, NwCreature> creatureSpawnDictionary = new();
    public static void OnMobPerception(CreatureEvents.OnPerception onPerception)
    {
      if (!onPerception.Creature.IsEnemy(onPerception.PerceivedCreature) || onPerception.Creature.IsInCombat)
        return;

      switch (onPerception.PerceptionEventType)
      {
        case PerceptionEventType.Seen:
        case PerceptionEventType.Heard:

          foreach (SpecialAbility ability in onPerception.Creature.SpecialAbilities)
            if (SpellUtils.IsSpellBuff(ability.Spell))
              _ = onPerception.Creature.ActionCastSpellAt(ability.Spell, onPerception.Creature, MetaMagic.Extend, true, 0, ProjectilePathType.Default, true);

          break;
      }
    }
    public static async void OnMobDeathResetSpawn(CreatureEvents.OnDeath onDeath)
    {
      NwWaypoint spawnPoint = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      await NwTask.Delay(TimeSpan.FromMinutes(10));
      spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
    }
    public static void ForceSlotReEquip(NwCreature creature, NwItem item, InventorySlot slot = InventorySlot.Chest)
    {
      creature.RunUnequip(item);

      Task waitUnequip = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.2));
        creature.RunEquip(item, slot);
      });
    }
    public static int GetCriticalMonsterDamage(int rowIndex)
    {
      var costTable = ItemProperty.MonsterDamage((IPMonsterDamage)rowIndex).CostTable;
      return costTable.GetInt(rowIndex, "NumDice").Value * costTable.GetInt(rowIndex, "Die").Value;
    }
    public static void MakeInventoryUndroppable(CreatureEvents.OnDeath onDeath)
    {
      ItemUtils.MakeCreatureInventoryUndroppable(onDeath.KilledCreature);
    }
    public static void HandleSpawnPointCreation(NwCreature creature)
    {
      NwWaypoint spawnPoint = NwWaypoint.Create("creature_spawn", creature.Location);

      if (creature.VisualTransform.Scale != 1 || creature.VisualTransform.Translation != Vector3.Zero || creature.VisualTransform.Rotation != Vector3.Zero)
      {
        spawnPoint.GetObjectVariable<LocalVariableFloat>("_CREATURE_SCALE").Value = creature.VisualTransform.Scale;
        spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_TRANSLATION").Value = Location.Create(creature.Area, creature.VisualTransform.Translation, 0);
        spawnPoint.GetObjectVariable<LocalVariableLocation>("_CREATURE_ROTATION").Value = Location.Create(creature.Area, creature.VisualTransform.Rotation, 0);
      }

      spawnPoint.GetObjectVariable<LocalVariableInt>("_CREATURE_APPEARANCE").Value = creature.Appearance.RowIndex;
      spawnPoint.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value = creature.GetObjectVariable<LocalVariableInt>("_SPAWN_ID").Value;
      spawnPoint.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value = creature.GetObjectVariable<LocalVariableString>("_SPAWN_TYPE").Value;
      spawnPoint.GetObjectVariable<LocalVariableInt>("animation").Value = creature.GetObjectVariable<LocalVariableInt>("animation").Value;

      if (creature.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").HasValue)
        spawnPoint.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value = creature.GetObjectVariable<LocalVariableString>("_SPAWNED_BY").Value;

      creatureSpawnDictionary.TryAdd(creature.Tag, NwCreature.Deserialize(creature.Serialize()));
      spawnPoint.GetObjectVariable<LocalVariableString>("creature").Value = creature.Tag;
      creature.Destroy();
    }
  }
}
