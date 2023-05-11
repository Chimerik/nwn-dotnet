
using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class Config
  {
    public class Context
    {
      public OnCreatureAttack onAttack { get; set; }
      public OnCreatureDamage onDamage { get; set; }
      //public List<DamageType> weaponBaseDamageType { get; set; }
      public NwCreature oAttacker { get; }
      public PlayerSystem.Player attackingPlayer { get; }
      public NwCreature oTarget { get; }
      public PlayerSystem.Player targetPlayer { get; }
      public bool isUnarmedAttack { get; }
      public bool isRangedAttack { get; }
      public NwItem attackWeapon { get; set; }
      public NwItem targetArmor { get; set; }
      //public int maxBaseAC { get; set; }
      public int baseArmorPenetration { get; set; }
      public int bonusArmorPenetration { get; set; }
      public AttackPosition attackPosition { get; set; }
      public Dictionary<DamageType, int> targetAC { get; set; }
      public int physicalReduction { get; set; }
      public int adrenalineGainModifier { get; set; }


      public Context(OnCreatureAttack onAttack, NwCreature oTarget, OnCreatureDamage onDamage = null)
      {
        this.onAttack = onAttack;
        this.onDamage = onDamage;
        this.oAttacker = null;

        if (onAttack != null)
        {
          this.oAttacker = onAttack.Attacker;
          this.attackingPlayer = PlayerSystem.Players.TryGetValue(oAttacker, out PlayerSystem.Player attackerPlayer) ? attackerPlayer : null;
        }

        else if (onDamage != null && onDamage.DamagedBy is NwCreature oCreature)
        {
          this.oAttacker = oCreature;
          this.attackingPlayer = PlayerSystem.Players.TryGetValue(oAttacker, out PlayerSystem.Player attackerPlayer) ? attackerPlayer : null;
        }

        this.oTarget = oTarget;
        this.targetPlayer = PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player defendingPlayer) ? defendingPlayer : null;

        this.attackWeapon = null;
        this.targetArmor = null;
        //this.weaponBaseDamageType = new List<DamageType>(); // Slashing par défaut
        this.baseArmorPenetration = 0;
        this.bonusArmorPenetration = 0;
        //this.maxBaseAC = 0;
        this.attackPosition = AttackPosition.NormalOrRanged;
        this.isUnarmedAttack = oAttacker != null && oAttacker.GetItemInSlot(InventorySlot.RightHand) == null;
        //PlayerSystem.Log.Info($"config - unarmed {isUnarmedAttack}");
        //PlayerSystem.Log.Info($"config - oAttacker {oAttacker}");
        //PlayerSystem.Log.Info($"config - right hand {oAttacker.GetItemInSlot(InventorySlot.RightHand)}");
        this.isRangedAttack = oAttacker != null && oAttacker.GetItemInSlot(InventorySlot.RightHand) != null
        && ItemUtils.GetItemCategory(oAttacker.GetItemInSlot(InventorySlot.RightHand).BaseItem.ItemType) == ItemUtils.ItemCategory.RangedWeapon;
        this.targetAC = new Dictionary<DamageType, int>();
        targetAC.Add(DamageType.BaseWeapon, 0);
        this.physicalReduction = 0;
        this.adrenalineGainModifier = 1;
      }
    }
    public enum AttackPosition
    {
      Low = 1,
      NormalOrRanged,
      High,
    }
    public static int GetIPSpecificAlignmentSubTypeAsInt(NwCreature oCreature)
    {
      return (oCreature.LawChaosAlignment + "_" + oCreature.GoodEvilAlignment) switch
      {
        "Lawful_Good" => 0,
        "Lawful_Neutral" => 1,
        "Lawful_Evil" => 2,
        "Neutral_Good" => 3,
        "Neutral_Neutral" => 4,
        "Neutral_Evil" => 5,
        "Chaotic_Good" => 6,
        "Chaotic_Neutral" => 7,
        "Chaotic_Evil" => 8,
        _ => 4,
      };
    }
    public static short RollDamage(int costValue)
    {
      return costValue switch
      {
        1 or 2 or 3 or 4 or 5 => (short)costValue,
        6 => (short)NwRandom.Roll(Utils.random, 4),
        7 => (short)NwRandom.Roll(Utils.random, 6),
        8 => (short)NwRandom.Roll(Utils.random, 8),
        9 => (short)NwRandom.Roll(Utils.random, 10),
        10 => (short)NwRandom.Roll(Utils.random, 6, 2),
        11 => (short)NwRandom.Roll(Utils.random, 8, 2),
        12 => (short)NwRandom.Roll(Utils.random, 4, 2),
        13 => (short)NwRandom.Roll(Utils.random, 10, 2),
        14 => (short)NwRandom.Roll(Utils.random, 12, 1),
        15 => (short)NwRandom.Roll(Utils.random, 12, 2),
        16 or 17 or 18 or 19 or 20 or 21 or 22 or 23 or 24 or 25 or 26 or 27 or 28 or 29 or 30 => (short)(costValue - 10),
        _ => 0,
      };
    }
    public static int GetContextDamage(Context ctx, DamageType damageType)
    {
      if (ctx.onAttack != null)
        return ctx.onAttack.DamageData.GetDamageByType(damageType);
      else if (ctx.onDamage != null)
        return ctx.onDamage.DamageData.GetDamageByType(damageType);

      LogUtils.LogMessage("ERROR : trying to get damage without any event context.", LogUtils.LogType.Combat);
      return -1;
    }
    public static void SetContextDamage(Context ctx, DamageType damageType, int value)
    {
      if (ctx.onAttack != null)
        SetDamage(ctx.onAttack.DamageData, damageType, (short)value);
      else if (ctx.onDamage != null)
        SetDamage(ctx.onDamage.DamageData, damageType, value);
      else
        LogUtils.LogMessage("ERROR : trying to set damage without any event context.", LogUtils.LogType.Combat);
    }
    public static void SetDamage<T>(DamageData<T> damageData, DamageType damageType, T value) where T : unmanaged
    {
      damageData.SetDamageByType(damageType, value);
    }
    public static void GetArmorValueFromArmorPiece(Context ctx, NwItem item, PlayerSystem.Player player, NwCreature attacker)
    {
      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value))
        ctx.targetAC[DamageType.BaseWeapon] = item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;

      if (!ctx.targetAC.TryAdd((DamageType)8192, item.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value))
        ctx.targetAC[(DamageType)8192] = item.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value;

      if (!ctx.targetAC.TryAdd((DamageType)16384, item.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value))
        ctx.targetAC[(DamageType)16384] = item.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value;


      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Cuirassé: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
          case CustomInscription.Absorption:
            if (ctx.physicalReduction < 3)
              ctx.physicalReduction += 1;
            break;

          case CustomInscription.Prismatique:
            if (player.GetAirMagicSkillScore() > 14 && player.GetFireMagicSkillScore() > 14 && player.GetWaterMagicSkillScore() > 14 && player.GetAirMagicSkillScore() > 14)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Artisan:

            for (byte slotId = 0; slotId < 13; slotId++)
            {
              var slot = player.oid.LoginCreature.GetQuickBarButton(slotId);

              if (slot.ObjectType == QuickBarButtonType.Feat && SkillSystem.learnableDictionary.ContainsKey(slot.Param1) && ((LearnableSkill)SkillSystem.learnableDictionary[slot.Param1]).type == SkillSystem.Type.Signet)
                ctx.targetAC[DamageType.BaseWeapon] += 1;
            }
            break;

          case CustomInscription.GardeDragon:
            if (attacker.Race.RacialType == RacialType.Dragon)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeExtérieur:
            if (attacker.Race.RacialType == RacialType.Outsider)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeAberration:
            if (attacker.Race.RacialType == RacialType.Aberration)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeElementaire:
            if (attacker.Race.RacialType == RacialType.Elemental)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Inflexible: ctx.targetAC[(DamageType)8192] += 1; break;
          case CustomInscription.Redoutable: ctx.targetAC[(DamageType)16384] += 1; break;
          case CustomInscription.Marchevent:

            int positiveEffect = 0;
            foreach (Effect eff in player.oid.LoginCreature.ActiveEffects)
            {
              if (eff.Tag.Contains("CUSTOM_POSITIVE_SPELL_"))
              {
                positiveEffect++;
                if (positiveEffect > 2)
                  ctx.targetAC[DamageType.BaseWeapon] += 1;

                if (positiveEffect > 5)
                  break;
              }
            }

            break;

          case CustomInscription.GardeGeant:
            if (attacker.Race.RacialType == RacialType.Giant)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeMagie:
            if (attacker.Race.RacialType == RacialType.MagicalBeast)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeBon:
            if (attacker.GoodEvilAlignment == Alignment.Good)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeChaos:
            if (attacker.LawChaosAlignment == Alignment.Chaotic)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeMal:
            if (attacker.GoodEvilAlignment == Alignment.Evil)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeNeutre:
            if (attacker.GoodEvilAlignment == Alignment.Neutral && attacker.LawChaosAlignment == Alignment.Neutral)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeLoi:
            if (attacker.LawChaosAlignment == Alignment.Lawful)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Hivernal:
            if (!ctx.targetAC.TryAdd(DamageType.Cold, 2))
              ctx.targetAC[DamageType.Cold] += 2; 
            break;
          case CustomInscription.Ignifugé:
            if (!ctx.targetAC.TryAdd(DamageType.Fire, 2))
                ctx.targetAC[DamageType.Fire] += 2; 
            break;
          case CustomInscription.Paratonnerre: 
            if (!ctx.targetAC.TryAdd(DamageType.Electrical, 2))
              ctx.targetAC[DamageType.Electrical] += 2; 
            break;
          case CustomInscription.Tectonique: 
            if (!ctx.targetAC.TryAdd(DamageType.Acid, 2))
              ctx.targetAC[DamageType.Acid] += 2; 
            break;
          case CustomInscription.Infiltrateur:
            if (!ctx.targetAC.TryAdd(DamageType.Piercing, 2)) 
              ctx.targetAC[DamageType.Piercing] += 1; 
            break;
          case CustomInscription.Saboteur: 
            if (!ctx.targetAC.TryAdd(DamageType.Slashing, 2))
              ctx.targetAC[DamageType.Slashing] += 1; 
            break;
          case CustomInscription.AvantGarde: 
            if (!ctx.targetAC.TryAdd(DamageType.Bludgeoning, 2))
              ctx.targetAC[DamageType.Bludgeoning] += 1; 
            break;
          case CustomInscription.GardeHalfelin:
            if (attacker.Race.RacialType == RacialType.Halfling)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeHumain:
            if (attacker.Race.RacialType == RacialType.Human)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeDemiElfe:
            if (attacker.Race.RacialType == RacialType.HalfElf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeDemiOrc:
            if (attacker.Race.RacialType == RacialType.HalfOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeElfe:
            if (attacker.Race.RacialType == RacialType.Elf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeGnome:
            if (attacker.Race.RacialType == RacialType.Gnome)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeNain:
            if (attacker.Race.RacialType == RacialType.Dwarf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Agitateur:
            if (player.oid.LoginCreature.AttackTarget is not null && (player.oid.LoginCreature.AnimationState != AnimationState.Walking
              || player.oid.LoginCreature.AnimationState != AnimationState.Running))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Sentinelle:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("_STANCE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Belluaire:
            if (player.oid.LoginCreature.Faction.GetMembers().Any(a => a.Master == player.oid.LoginCreature && a.Tag.Contains("_ANIMAL_COMPANION_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Eclaireur:
            if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_IS_USING_PREPARATION").HasValue)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Disciple:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_CONDITION_")))
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Virtuose:
            if (player.oid.LoginCreature.AnimationState == AnimationState.Cast1 || player.oid.LoginCreature.AnimationState == AnimationState.Cast2
              || player.oid.LoginCreature.AnimationState == AnimationState.Cast3 || player.oid.LoginCreature.AnimationState == AnimationState.Cast4
              || player.oid.LoginCreature.AnimationState == AnimationState.Cast5 || player.oid.LoginCreature.AnimationState == AnimationState.CastCreature
              || player.oid.LoginCreature.AnimationState == AnimationState.Conjure1 || player.oid.LoginCreature.AnimationState == AnimationState.Conjure2)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Fossoyeur:
            double hpLeft = player.oid.LoginCreature.HP / player.MaxHP;

            if (hpLeft > 0.6)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            else if (hpLeft > 0.4)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            else if (hpLeft > 0.2)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            else
              ctx.targetAC[DamageType.BaseWeapon] += 4;

            break;

          case CustomInscription.Prodige:

            int nbSkillInCD = 0;

            for (byte slotId = 0; slotId < 13; slotId++)
            {
              var slot = player.oid.LoginCreature.GetQuickBarButton(slotId);

              if (slot.ObjectType == QuickBarButtonType.Feat && player.oid.LoginCreature.GetFeatRemainingUses(NwFeat.FromFeatId(slot.Param1)) < 1)
                nbSkillInCD += 1;
            }

            switch (nbSkillInCD)
            {
              case 2: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 4: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 6: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.Destructeur:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Bénédiction:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Centurion:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_SHOUT_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Oublié:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => !e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.GardeNonVie:
            if (attacker.Race.RacialType == RacialType.Undead)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeArtifice:
            if (attacker.Race.RacialType == RacialType.Construct)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeOrc:
            if (attacker.Race.RacialType == RacialType.HumanoidOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Lieutenant:
            ctx.targetAC[DamageType.BaseWeapon] -= 4;
            break;

          case CustomInscription.MaîtreBlème:
            int controlledUndead = player.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == player.oid.LoginCreature && a.Race.RacialType == RacialType.Undead);

            switch (controlledUndead)
            {
              case 1: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 3: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 5: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.Marionnettiste:
            int controlledSummons = player.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == player.oid.LoginCreature && a.Race.RacialType != RacialType.Undead && !a.Tag.Contains("_ANIMAL_COMPANION_"));

            switch (controlledSummons)
            {
              case 1: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 3: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 5: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.GardeMonstre:
            if (attacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeHumanoïde:
            if (attacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeMétamorphe:
            if (attacker.Race.RacialType == RacialType.ShapeChanger)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeGoblinoïde:
            if (attacker.Race.RacialType == RacialType.HumanoidGoblinoid)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeAnimal:
            if (attacker.Race.RacialType == RacialType.Animal)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeReptilien:
            if (attacker.Race.RacialType == RacialType.HumanoidReptilian)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeVermine:
            if (attacker.Race.RacialType == RacialType.Vermin)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;
        }
      }
    }
    public static void GetArmorValueFromShield(Context ctx, NwItem item, PlayerSystem.Player player, NwCreature attacker)
    {
      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value))
        ctx.targetAC[DamageType.BaseWeapon] = item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;

      if (!ctx.targetAC.TryAdd(DamageType.Piercing, item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value))
        ctx.targetAC[DamageType.Piercing] = item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Blindé: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
          case CustomInscription.RepousseDragon:
            if (attacker.Race.RacialType == RacialType.Dragon)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseExtérieur:
            if (attacker.Race.RacialType == RacialType.Outsider)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseAberration:
            if (attacker.Race.RacialType == RacialType.Aberration)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.LongueVieAuRoi:
            if (player.oid.LoginCreature.HP > player.MaxHP / 2)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.LaFoiEstMonBouclier:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.LaSurvieDuMieuxEquipé:
            if (!ctx.targetAC.TryAdd((DamageType)8192, 1))
              ctx.targetAC[(DamageType)8192] += 1;
            break;

          case CustomInscription.ParéEnTouteSaison:
            if (!ctx.targetAC.TryAdd((DamageType)16384, 1))
              ctx.targetAC[(DamageType)16384] += 1;
            break;

          case CustomInscription.RepousseGéant:
            if (attacker.Race.RacialType == RacialType.Giant)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseMagie:
            if (attacker.Race.RacialType == RacialType.MagicalBeast)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseBon:
            if (attacker.GoodEvilAlignment == Alignment.Good)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseChaos:
            if (attacker.LawChaosAlignment == Alignment.Chaotic)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseMal:
            if (attacker.GoodEvilAlignment == Alignment.Evil)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseNeutre:
            if (attacker.GoodEvilAlignment == Alignment.Neutral && attacker.LawChaosAlignment == Alignment.Neutral)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseLoi:
            if (attacker.LawChaosAlignment == Alignment.Lawful)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.ContreVentsEtMarées: ctx.targetAC[DamageType.Piercing] += 2;
            break;

          case CustomInscription.lEnigmeDelAcier:
            if (!ctx.targetAC.TryAdd(DamageType.Slashing, 2))
              ctx.targetAC[DamageType.Slashing] += 2;
            break;

          case CustomInscription.PasLeVisage:
            if (!ctx.targetAC.TryAdd(DamageType.Bludgeoning, 2))
              ctx.targetAC[DamageType.Bludgeoning] += 2;
            break;

          case CustomInscription.PortéParLeVent:
            if (!ctx.targetAC.TryAdd(DamageType.Cold, 2))
              ctx.targetAC[DamageType.Cold] += 2;
            break;

          case CustomInscription.CommeUnRoc:
            if (!ctx.targetAC.TryAdd(DamageType.Acid, 2))
              ctx.targetAC[DamageType.Acid] += 2;
            break;

          case CustomInscription.Illumination:
            if (!ctx.targetAC.TryAdd(DamageType.Fire, 2))
              ctx.targetAC[DamageType.Fire] += 2;
            break;

          case CustomInscription.ChevaucheLaTempête:
            if (!ctx.targetAC.TryAdd(DamageType.Electrical, 2))
              ctx.targetAC[DamageType.Electrical] += 2;
            break;

          case CustomInscription.RepousseHalfelin:
            if (attacker.Race.RacialType == RacialType.Halfling)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseHumain:
            if (attacker.Race.RacialType == RacialType.Human)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseDemiElfe:
            if (attacker.Race.RacialType == RacialType.HalfElf)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseDemiOrc:
            if (attacker.Race.RacialType == RacialType.HalfOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseElfe:
            if (attacker.Race.RacialType == RacialType.Elf)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseGnome:
            if (attacker.Race.RacialType == RacialType.Gnome)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseNain:
            if (attacker.Race.RacialType == RacialType.Dwarf)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseElementaire:
            if (attacker.Race.RacialType == RacialType.Elemental)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.LaRaisonDuPlusFort:
            if (player.oid.LoginCreature.AttackTarget is not null && (player.oid.LoginCreature.AnimationState != AnimationState.Walking
              || player.oid.LoginCreature.AnimationState != AnimationState.Running))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.SavoirNestQueLaMoitiéDuChemin:
            if (player.oid.LoginCreature.AnimationState == AnimationState.Cast1 || player.oid.LoginCreature.AnimationState == AnimationState.Cast2
              || player.oid.LoginCreature.AnimationState == AnimationState.Cast3 || player.oid.LoginCreature.AnimationState == AnimationState.Cast4
              || player.oid.LoginCreature.AnimationState == AnimationState.Cast5 || player.oid.LoginCreature.AnimationState == AnimationState.CastCreature
              || player.oid.LoginCreature.AnimationState == AnimationState.Conjure1 || player.oid.LoginCreature.AnimationState == AnimationState.Conjure2)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.CeNestQuuneEgratignure:
            if (player.oid.LoginCreature.HP < player.MaxHP / 2)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.NeTremblezPas:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.RepousseNonVie:
            if (attacker.Race.RacialType == RacialType.Undead)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseArtifice:
            if (attacker.Race.RacialType == RacialType.Construct)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseOrc:
            if (attacker.Race.RacialType == RacialType.HumanoidOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.HeureuxLesSimplesdEsprits:
          case CustomInscription.LaVieNestQueDouleur:
            ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.RepousseMonstre:
            if (attacker.Race.RacialType == RacialType.HumanoidOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseHumanoïde:
            if (attacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseMétamorphe:
            if (attacker.Race.RacialType == RacialType.ShapeChanger)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseGobelinoïde:
            if (attacker.Race.RacialType == RacialType.HumanoidGoblinoid)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseAnimal:
            if (attacker.Race.RacialType == RacialType.Animal)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseReptilien:
            if (attacker.Race.RacialType == RacialType.HumanoidReptilian)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseVermine:
            if (attacker.Race.RacialType == RacialType.Vermin)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;
        }
      }
    }
    public static void GetArmorValueFromWeapon(Context ctx, NwItem item)
    {
      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Défense:
            if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, 1))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Masochisme:
            if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, -1))
              ctx.targetAC[DamageType.BaseWeapon] -= 1;
            break;

          case CustomInscription.Refuge:
            if (!ctx.targetAC.TryAdd((DamageType)8192, 1))
              ctx.targetAC[(DamageType)8192] += 1;
            break;

          case CustomInscription.Protecteur:
            if (!ctx.targetAC.TryAdd((DamageType)16384, 1))
              ctx.targetAC[(DamageType)16384] += 1;
            break;
        }
      }
    }
    public static void GetDamageValueFromWeapon(Context ctx, NwItem item, PlayerSystem.Player player, NwCreature defender)
    {
      double baseDamage = GetContextDamage(ctx, DamageType.BaseWeapon);
      double coldDamage = GetContextDamage(ctx, DamageType.Cold);
      double fireDamage = GetContextDamage(ctx, DamageType.Fire);
      double elecDamage = GetContextDamage(ctx, DamageType.Electrical);
      double acidDamage = GetContextDamage(ctx, DamageType.Acid);
      double piercingDamage = GetContextDamage(ctx, DamageType.Piercing);
      double slashDamage = GetContextDamage(ctx, DamageType.Slashing);
      double bluntDamage = GetContextDamage(ctx, DamageType.Bludgeoning);
      bool vampirism = false;
      bool zele = false;
      int doubleAdrenaline = 0;
      int bonusPenetration = 0;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Sismique:
            if (baseDamage > -1)
            {
              acidDamage += baseDamage;
              baseDamage = -1;
            }
            break;

          case CustomInscription.Incendiaire:
            if (baseDamage > -1)
            {
              fireDamage += baseDamage;
              baseDamage = -1;
            }
            break;

          case CustomInscription.Polaire:
            if (baseDamage > -1)
            {
              coldDamage += baseDamage;
              baseDamage = -1;
            }
            break;

          case CustomInscription.Electrocution:
            if (baseDamage > -1)
            {
              elecDamage += baseDamage;
              baseDamage = -1;
            }
            break;
        }
      }

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Pourfendeur: baseDamage *= 1.01; break;
          case CustomInscription.QueDuMuscle:
          case CustomInscription.Masochisme: baseDamage *= 1.02; break;
          case CustomInscription.Vampirisme: vampirism = true; break;
          case CustomInscription.Zèle: zele = true; break;

          case CustomInscription.PourfendeurDragon:
            if (defender.Race.RacialType == RacialType.Dragon) 
              baseDamage *= 1.03; 
            break;

          case CustomInscription.PourfendeurExtérieur:
            if (defender.Race.RacialType == RacialType.Outsider)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurAberration:
            if (defender.Race.RacialType == RacialType.Aberration)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurGéant:
            if (defender.Race.RacialType == RacialType.Giant)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurElementaire:
            if (defender.Race.RacialType == RacialType.Elemental)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurMagie:
            if (defender.Race.RacialType == RacialType.MagicalBeast)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurHalfelin:
            if (defender.Race.RacialType == RacialType.Halfling)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurHumain:
            if (defender.Race.RacialType == RacialType.Human)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurDemiElfe:
            if (defender.Race.RacialType == RacialType.HalfElf)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurDemiOrc:
            if (defender.Race.RacialType == RacialType.HalfOrc)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurElfe:
            if (defender.Race.RacialType == RacialType.Elf)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurGnome:
            if (defender.Race.RacialType == RacialType.Gnome)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurNain:
            if (defender.Race.RacialType == RacialType.Dwarf)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurNonVie:
            if (defender.Race.RacialType == RacialType.Undead)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurArtificiel:
            if (defender.Race.RacialType == RacialType.Construct)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurOrc:
            if (defender.Race.RacialType == RacialType.HumanoidOrc)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurMonstres:
            if (defender.Race.RacialType == RacialType.Beast)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurHumanoïdes:
            if (defender.Race.RacialType == RacialType.HumanoidMonstrous)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurMétamorphes:
            if (defender.Race.RacialType == RacialType.ShapeChanger)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurGobelins:
            if (defender.Race.RacialType == RacialType.HumanoidGoblinoid)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurAnimal:
            if (defender.Race.RacialType == RacialType.Animal)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurReptilien:
            if (defender.Race.RacialType == RacialType.HumanoidReptilian)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurVermine:
            if (defender.Race.RacialType == RacialType.Vermin)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurBien:
            if (defender.GoodEvilAlignment == Alignment.Good)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurChaos:
            if (defender.LawChaosAlignment == Alignment.Chaotic)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurMal:
            if (defender.GoodEvilAlignment == Alignment.Evil)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurNeutralité:
            if (defender.GoodEvilAlignment == Alignment.Neutral && defender.LawChaosAlignment == Alignment.Neutral)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurLoi:
            if (defender.LawChaosAlignment == Alignment.Lawful)
              baseDamage *= 1.02;
            break;

          case CustomInscription.ForceEtHonneur:
            if (player.oid.LoginCreature.HP > player.MaxHP / 2)
              baseDamage *= 1.02;
            break;

          case CustomInscription.VengeanceSeraMienne:
            if (player.oid.LoginCreature.HP < player.MaxHP / 2)
              baseDamage *= 1.03;
            break;

          case CustomInscription.MaîtreDeSonDestin:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              baseDamage *= 1.02;
            break;
            
          case CustomInscription.DanseAvecLaMort:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("_STANCE_")))
              baseDamage *= 1.02;
            break;

          case CustomInscription.Sadisme:
            if (player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              baseDamage *= 1.03;
            break;

          case CustomInscription.Givroclaste: coldDamage *= 1.02; break;
          case CustomInscription.Pyroclaste: fireDamage *= 1.02; break;
          case CustomInscription.Electroclaste: elecDamage *= 1.02; break;
          case CustomInscription.Séismoclaste: acidDamage *= 1.02; break;
          case CustomInscription.Aiguillon: piercingDamage *= 1.02; break;
          case CustomInscription.Rasoir: slashDamage *= 1.02; break;
          case CustomInscription.Fracasseur: bluntDamage *= 1.02; break;

          case CustomInscription.Fureur: doubleAdrenaline += 1; break;
          case CustomInscription.Pénétration: bonusPenetration += 3; break;
        }
      }

      if (vampirism)
      {
        if (GetContextDamage(ctx, DamageType.Negative) < 0)
          SetContextDamage(ctx, DamageType.Negative, 3);
        else
          SetContextDamage(ctx, DamageType.Negative, GetContextDamage(ctx, DamageType.Negative) + 3);

        player.oid.LoginCreature.HP = player.oid.LoginCreature.HP + 3 >= player.MaxHP ? player.MaxHP : player.oid.LoginCreature.HP + 3;
      }

      if(zele)
        player.endurance.currentMana = player.endurance.currentMana + 1 >= player.endurance.maxMana ? player.endurance.maxMana : player.endurance.currentMana + 1;

      if (doubleAdrenaline > 0 && doubleAdrenaline > NwRandom.Roll(Utils.random, 100))
        ctx.adrenalineGainModifier = 2;

      if (bonusPenetration > 0 && bonusPenetration > NwRandom.Roll(Utils.random, 100))
        ctx.baseArmorPenetration += 20;

      if(baseDamage > -1) 
        SetContextDamage(ctx, DamageType.BaseWeapon, (int)Math.Round(baseDamage, MidpointRounding.ToEven));

      if (coldDamage > -1)
        SetContextDamage(ctx, DamageType.Cold, (int)Math.Round(coldDamage, MidpointRounding.ToEven));

      if (fireDamage > -1)
        SetContextDamage(ctx, DamageType.Fire, (int)Math.Round(fireDamage, MidpointRounding.ToEven));

      if (elecDamage > -1)
        SetContextDamage(ctx, DamageType.Electrical, (int)Math.Round(elecDamage, MidpointRounding.ToEven));

      if (acidDamage > -1)
        SetContextDamage(ctx, DamageType.Acid, (int)Math.Round(acidDamage, MidpointRounding.ToEven));

      if (piercingDamage > -1)
        SetContextDamage(ctx, DamageType.Piercing, (int)Math.Round(piercingDamage, MidpointRounding.ToEven));

      if (slashDamage > -1)
        SetContextDamage(ctx, DamageType.Slashing, (int)Math.Round(slashDamage, MidpointRounding.ToEven));

      if (bluntDamage > -1)
        SetContextDamage(ctx, DamageType.Bludgeoning, (int)Math.Round(bluntDamage, MidpointRounding.ToEven));
    }
  }
}
