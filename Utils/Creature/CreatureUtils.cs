using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;
using Ability = Anvil.API.Ability;
using DamageType = Anvil.API.DamageType;
using InventorySlot = Anvil.API.InventorySlot;
using ItemProperty = Anvil.API.ItemProperty;

namespace NWN
{
  public static partial class CreatureUtils
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
    /*public static async void OnMobDeathSoulReap(CreatureEvents.OnDeath onDeath)
    {
      foreach (NwPlayer player in NwModule.Instance.Players)
        if (player?.ControlledCreature?.Area == onDeath.KilledCreature?.Area
        && player?.ControlledCreature.DistanceSquared(onDeath.KilledCreature) < 600
        && PlayerSystem.Players.TryGetValue(player.LoginCreature, out PlayerSystem.Player reaper) && reaper.GetAttributeLevel(SkillSystem.Attribut.SoulReaping) > 0
          && reaper.endurance.regenerableMana > 0 && reaper.endurance.currentMana < reaper.endurance.maxMana
          && reaper.soulReapTriggers < (player.LoginCreature.GetAbilityScore(Ability.Intelligence, true) - 10) / 4)
        {
          int reaperLevel = reaper.GetAttributeLevel(SkillSystem.Attribut.SoulReaping);
          reaper.endurance.currentMana = reaperLevel + reaper.endurance.currentMana > reaper.endurance.maxMana ? reaper.endurance.maxMana : reaperLevel + reaper.endurance.currentMana;
          reaper.endurance.regenerableMana -= reaperLevel;

          reaper.soulReapTriggers += 1;

          await NwTask.Delay(TimeSpan.FromSeconds(15));
          reaper.soulReapTriggers -= 1;
        }
    }*/
    public static async void OnMobDeathResetSpawn(CreatureEvents.OnDeath onDeath)
    {
      ModuleSystem.Log.Info("On death triggered - OnMobDeathResetSpawn");

      NwWaypoint spawnPoint = onDeath.KilledCreature.GetObjectVariable<LocalVariableObject<NwWaypoint>>("_SPAWN").Value;
      await NwTask.Delay(TimeSpan.FromSeconds(10));
      //await NwTask.Delay(TimeSpan.FromMinutes(10));
      spawnPoint.GetObjectVariable<LocalVariableBool>("_SPAWN_COOLDOWN").Delete();
    }
    public static void CreatureHealthRegenLoop(NwCreature creature)
    {
      if (creature is null || !creature.IsValid)
        return;

      int maxHP = creature.MaxHP;
      int healthRegen = 0;

      foreach (var eff in creature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_BLEEDING")
          healthRegen -= 3;
        else if (eff.Tag == "CUSTOM_CONDITION_POISON")
          healthRegen -= 4;
        else if (eff.Tag == "CUSTOM_CONDITION_DISEASE")
          healthRegen -= 4;
        else if (eff.Tag == "CUSTOM_CONDITION_BURNING")
          healthRegen -= 7;
        else if (eff.Tag.StartsWith("CUSTOM_EFFECT_REGEN_"))
        {
          var split = eff.Tag.Split("_");
          healthRegen += int.Parse(split[^1]);

          if (healthRegen > 19)
          {
            healthRegen = 20;
            break;
          }
        }
      }

      if (healthRegen < -20)
        healthRegen = -20;

      if (creature.HP >= maxHP && healthRegen >= 0)
        return;

      if (creature.HP + healthRegen >= maxHP)
      {
        creature.HP = maxHP;
        return;
      }

      if(healthRegen > -1)
        creature.HP += healthRegen;
      else
        creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(healthRegen, DamageType.Slashing));
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
      ModuleSystem.Log.Info("On death triggered - make inventory undroppable");
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
    public static int GetUnarmedDamage(int monkLevel)
    {
     return monkLevel switch
      {
        1 or 2 or 3 or 4 => 4,
        5 or 6 or 7 or 8 or 9 or 10 => 6,
        11 or 12 or 13 or 14 or 15 or 16 => 8,
        17 or 18 or 19 or 20 or 21 or 22 => 10,
        _ => 1,
      };
    }
    public static int OverrideSizeAttackAndACBonus(Native.API.CNWSCreature attacker)
    {
      return (CreatureSize)attacker.m_nCreatureSize switch
      {
        CreatureSize.Tiny => -2,
        CreatureSize.Small => -1,
        CreatureSize.Medium => 0,
        CreatureSize.Large => 1,
        CreatureSize.Huge => 2,
        _ => 0,
      };
    }
    public static async void TestGetInvi(Native.API.CNWSCreature attacker, Native.API.CNWSCreature target)
    {
      //LogUtils.LogMessage($"movement rate {attacker.m_fMovementRateFactor}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"movement rate NWNX {Core.NWNX.CreaturePlugin.GetMovementType(attacker.m_idSelf)}", LogUtils.LogType.Combat);

      await NwTask.Delay(TimeSpan.FromSeconds(1));
      TestGetInvi(attacker, target);
    }
  }
}
