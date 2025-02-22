using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;
using NWN.Native.API;
using DamageType = Anvil.API.DamageType;
using InventorySlot = Anvil.API.InventorySlot;
using ItemProperty = Anvil.API.ItemProperty;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public const string CurrentAttackTarget = "_CURRENT_ATTACK_TARGET";
    public const string CancelDamageDoublonVariable = "_CANCEL_DAMAGE_DOUBLON";
    public const string AnimalCompanionTag = "animal_companion";
    public static readonly CExoString AnimalCompanionTagExo = AnimalCompanionTag.ToExoString();
    public static readonly CExoString CancelDamageDoublonVariableExo = CancelDamageDoublonVariable.ToExoString();
    public const string OpportunityAttackTypeVariable = "_OPPORTUNITY_ATTACK_TYPE";
    public static readonly CExoString OpportunityAttackTypeVariableExo = OpportunityAttackTypeVariable.ToExoString();
    public const string SneakAttackCooldownVariable = "_SNEAK_ATTACK_COOLDOWN";
    public static readonly CExoString SneakAttackCooldownVariableExo = SneakAttackCooldownVariable.ToExoString();
    public const string BersekerRepresaillesVariable = "_BERSEKER_REPRESAILLES";
    public static readonly CExoString BersekerRepresaillesVariableExo = BersekerRepresaillesVariable.ToExoString();
    public const string HastMasterCooldownVariable = "_HAST_MASTER_IN_COOLDOWN";
    public static readonly CExoString HastMasterCooldownVariableExo = HastMasterCooldownVariable.ToExoString();
    public static readonly CExoString HastMasterSpecialAttackExo = "_HAST_MASTER_SPECIAL_ATTACK".ToExoString();
    public const string HastMasterOpportunityVariable = "_HAST_MASTER_OPPORTUNITY";
    public static readonly CExoString HastMasterOpportunityVariableExo = HastMasterOpportunityVariable.ToExoString();
    public const string SentinelleOpportunityVariable = "_SENTINELLE_OPPORTUNITY";
    public static readonly CExoString SentinelleOpportunityVariableExo = SentinelleOpportunityVariable.ToExoString();
    public const string SentinelleOpportunityTargetVariable = "_SENTINELLE_OPPORTUNITY_TARGET";
    public static readonly CExoString SentinelleOpportunityTargetVariableExo = SentinelleOpportunityTargetVariable.ToExoString();
    public const string FureurOrcBonusDamageVariable = "_FUREUR_ORC_DAMAGE";
    public static readonly CExoString FureurOrcBonusDamageVariableExo = FureurOrcBonusDamageVariable.ToExoString();
    public const string FureurOrcBonusAttackVariable = "_FUREUR_ORC_ATTACK";
    public static readonly CExoString FureurOrcBonusAttackVariableExo = FureurOrcBonusAttackVariable.ToExoString();
    public const string SecondeChanceVariable = "_FEAT_SECONDE_CHANCE";
    public static readonly CExoString SecondeChanceVariableExo = SecondeChanceVariable.ToExoString();
    public const string CastAbilityVariable = "_CAST_ABILITY";
    public static readonly CExoString CastAbilityVariableExo = CastAbilityVariable.ToExoString();
    public const string ArmeSpirituelleTag = "arme_spirituelle";
    public static readonly CExoString ArmeSpirituelleTagExo = ArmeSpirituelleTag.ToExoString();

    public const string FendreAttackVariable = "_EXPERTISE_FENDRE_ATTAQUE";
    public static readonly CExoString FendreAttackVariableExo = FendreAttackVariable.ToExoString();
    public static readonly CExoString FendreDamage1VariableExo = "_EXPERTISE_FENDRE_DAMAGE_1".ToExoString();
    public static readonly CExoString FendreDamage2VariableExo = "_EXPERTISE_FENDRE_DAMAGE_2".ToExoString();

    public const string TirArcaniqueVariable = "_TIR_ARCANIQUE";
    public const string TirArcaniqueCooldownVariable = "_TIR_ARCANIQUE_COOLDOWN";
    public const string TirChercheurVariable = "_TIR_CHERCHEUR";
    public const string TirAffaiblissantVariable = "_TIR_AFFAIBLISSANT";
    public static readonly CExoString TirAffaiblissantVariableExo = TirAffaiblissantVariable.ToExoString();
    public const string TirAgrippantVariable = "_TIR_AGRIPPANT";
    public const string TirIncurveVariable = "_TIR_INCURVE";
    public static readonly CExoString TirIncurveVariableExo = TirIncurveVariable.ToExoString();

    public const string ManoeuvreTypeVariable = "_MANOEUVRE_TYPE";
    public static readonly CExoString ManoeuvreTypeVariableExo = ManoeuvreTypeVariable.ToExoString();
    public const string ManoeuvreDiceVariable = "_MANOEUVRE_DICE";
    public static readonly CExoString ManoeuvreDiceVariableExo = ManoeuvreDiceVariable.ToExoString();
    public const string ManoeuvreBalayageTargetVariable = "_MANOEUVRE_BALAYAGE_TARGET";
    public static readonly CExoString ManoeuvreBalayageTargetVariableExo = ManoeuvreBalayageTargetVariable.ToExoString();
    public static readonly CExoString ManoeuvreDiversionVariableExo = "_MANOEUVRE_DIVERSION".ToExoString();
    public static readonly CExoString ManoeuvreDiversionExpiredVariableExo = "_MANOEUVRE_DIVERSION_EXPIRED".ToExoString();
    public static readonly CExoString ManoeuvreRiposteVariableExo = "_MANOEUVRE_RIPOSTE".ToExoString();

    public const string VoeuHostileVariable = "_VOEU_HOSTILE";
    public static readonly CExoString VoeuHostileVariableExo = VoeuHostileVariable.ToExoString();

    public const string EmpaleurCooldownVariable = "_EMPALEUR_COOLDOWN";
    public static readonly CExoString EmpaleurCooldownVariableExo = EmpaleurCooldownVariable.ToExoString();

    public const string OpportunisteVariable = "_OPPORTUNISTE";
    public static readonly CExoString OpportunisteVariableExo = OpportunisteVariable.ToExoString();

    public const string MonkBonusAttackVariable = "_MONK_BONUS_ATTACK";
    public static readonly CExoString MonkBonusAttackVariableExo = MonkBonusAttackVariable.ToExoString();

    public const string MonkDelugeVariable = "_MONK_DELUGE";
    public static readonly CExoString MonkDelugeVariableExo = MonkDelugeVariable.ToExoString();

    public const string MonkUnarmedDamageVariable = "_MONK_UNARMED_DAMAGE";
    public static readonly CExoString MonkUnarmedDamageVariableExo = MonkUnarmedDamageVariable.ToExoString();

    public const string MonkPaumeTechniqueVariable = "_MONK_PAUME_TECHNIQUE";

    public const string PresageVariable = "_PRESAGE";
    public static readonly CExoString PresageVariableExo = PresageVariable.ToExoString();

    public const string Presage1Variable = "_PRESAGE1";
    public const string Presage2Variable = "_PRESAGE2";
    public const string Presage3Variable = "_PRESAGE3";

    public const string CharmeInstinctifVariable = "_CHARME_INSTINCTIF";
    public static readonly CExoString CharmeInstinctifVariableExo = CharmeInstinctifVariable.ToExoString();

    public const string BriseurDeHordesVariable = "_BRISEUR_DE_HORDES";
    public static readonly CExoString BriseurDeHordesVariableExo = BriseurDeHordesVariable.ToExoString();

    public const string RafaleDuTraqueurVariable = "_RAFALE_DU_TRAQUEUR";
    public static readonly CExoString RafaleDuTraqueurVariableExo = RafaleDuTraqueurVariable.ToExoString();

    public const string EsquiveDuTraqueurVariable = "_ESQUIVE_DU_TRAQUEUR";
    public static readonly CExoString EsquiveDuTraqueurVariableExo = EsquiveDuTraqueurVariable.ToExoString();

    public const string AttaqueCoordonneeVariable = "_ATTAQUE_COORDONNEE";
    public static readonly CExoString AttaqueCoordonneeVariableExo = AttaqueCoordonneeVariable.ToExoString();

    public const string AttaqueCoordonneCoolDownVariable = "_ATTAQUE_COORDONNEE_COOLDOWN";
    public static readonly CExoString AttaqueCoordonneCoolDownVariableExo = AttaqueCoordonneCoolDownVariable.ToExoString();

    public const string EnsoApotheoseVariable = "_ENSO_APOTHEOSE";

    public const string FurieBestialeVariable = "_FURIE_BESTIALE";
    public static readonly CExoString FurieBestialeVariableExo = FurieBestialeVariable.ToExoString();

    public const string AbjurationWardForcedTriggerVariable = "_ABJURATION_WARD_FORCED_TRIGGER";
    public static readonly CExoString AbjurationWardForcedTriggerVariableExo = AbjurationWardForcedTriggerVariable.ToExoString();

    public const string RegardHypnotiqueTargetListVariable = "_REGARD_HYPNOTIQUE_TARGET_LIST";

    public const string VigueurNaineHDVariable = "_FEAT_VIGUEUR_NAINE_HD";
    public const string OriginalSizeVariable = "_ORIGINAL_SIZE";

    public const string ClercMartialVariable = "_CLERC_MARTIAL";
    public static readonly CExoString ClercMartialVariableExo = ClercMartialVariable.ToExoString();

    public const string PacteDeLaLameVariable = "_PACTE_DE_LA_LAME";
    public static readonly CExoString PacteDeLaLameVariableExo = PacteDeLaLameVariable.ToExoString();

    public const string BuveuseDeVieVariable = "_BUVEUSE_DE_VIE_DAMAGE_TYPE";
    public const string FrappeDivineVariable = "_FRAPPE_DIVINE_DAMAGE_TYPE";

    public static readonly Dictionary<string, NwCreature> creatureSpawnDictionary = new();
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
      //ModuleSystem.Log.Info("On death triggered - OnMobDeathResetSpawn");

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
      //ModuleSystem.Log.Info("On death triggered - make inventory undroppable");
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
    public static int GetUnarmedDamage(CNWSCreature creature)
    {
      int monkLevel = creature.m_pStats.GetNumLevelsOfClass(CustomClass.Monk);

      if (monkLevel > 0)
        return monkLevel > 16 ? 12 : monkLevel > 10 ? 10 : monkLevel > 4 ? 8 : 6;
      else if (creature.m_pStats.HasFeat(CustomSkill.FightingStyleUnarmed).ToBool())
      {
        if (creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.RightHand) is not null
          || creature.m_pInventory.GetItemInSlot((uint)EquipmentSlot.LeftHand) is not null)
          return 6;
        else
          return 8;
      }
      else if (creature.m_pStats.HasFeat(CustomSkill.BagarreurDeTaverne).ToBool())
        return 4;

      return 1;
    }
    public static async void TestGetInvi(CNWSCreature attacker, CNWSCreature target)
    {
      //LogUtils.LogMessage($"movement rate {attacker.m_fMovementRateFactor}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"movement rate NWNX {Core.NWNX.CreaturePlugin.GetMovementType(attacker.m_idSelf)}", LogUtils.LogType.Combat);

      await NwTask.Delay(TimeSpan.FromSeconds(1));
      TestGetInvi(attacker, target);
    }
  }
}
