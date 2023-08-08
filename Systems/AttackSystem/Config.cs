
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
    public static void SetArmorValueFromArmorPiece(Context ctx)
    {
      if (ctx.targetPlayer is null || ctx.targetArmor is null || ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
        return;

      if (ctx.targetArmor.BaseItem.ItemType == BaseItemType.Armor)
      {
        if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value))
          ctx.targetAC[DamageType.BaseWeapon] += ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;

        if (!ctx.targetAC.TryAdd((DamageType)8192, ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value))
          ctx.targetAC[(DamageType)8192] += ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value;

        if (!ctx.targetAC.TryAdd((DamageType)16384, ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value))
          ctx.targetAC[(DamageType)16384] += ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value;
      }
      else
      {
        // Dans le cas où il s'agit d'une pièce d'armure, on définit le type d'armure à partir de la pièce principale du torse
        NwItem baseArmor = ctx.oTarget.GetItemInSlot(InventorySlot.Chest);

        if (baseArmor is not null)
        {
          if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, baseArmor.BaseACValue * 3 * ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value))
            ctx.targetAC[DamageType.BaseWeapon] += baseArmor.BaseACValue * 3 * ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;

          ctx.targetAC.TryAdd((DamageType)16384, 0);
          ctx.targetAC.TryAdd((DamageType)8192, 0);

          switch (baseArmor.BaseACValue)
          {
            case 1:
            case 2:
            case 3:
            case 4:
                ctx.targetAC[(DamageType)16384] += baseArmor.BaseACValue * 5;
              break;
            case 5:
                ctx.targetAC[(DamageType)16384] += 30;
                ctx.targetAC[(DamageType)8192] += 5;
              break;
            case 6:
                ctx.targetAC[(DamageType)8192] += 10;
              break;
            case 7:
                ctx.targetAC[(DamageType)8192] += 15;
              break;
            case 8:
                ctx.targetAC[(DamageType)8192] += 20;
              break;
          }
        }
      }

      int armorProficiencyLevel = ctx.targetPlayer.GetArmorProficiencyLevel(ctx.targetArmor.BaseACValue) / 10;

      ctx.targetAC[DamageType.BaseWeapon] *= armorProficiencyLevel;
      ctx.targetAC[(DamageType)16384] *= armorProficiencyLevel;
      ctx.targetAC[(DamageType)8192] *= armorProficiencyLevel;

      for (int i = 0; i < ctx.targetArmor.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (ctx.targetArmor.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (ctx.targetArmor.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Cuirassé: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
          case CustomInscription.Absorption:
            if (ctx.physicalReduction < 3)
              ctx.physicalReduction += 1;
            break;

          case CustomInscription.Prismatique:
            if (ctx.targetPlayer.GetAirMagicSkillScore() > 14 && ctx.targetPlayer.GetFireMagicSkillScore() > 14 && ctx.targetPlayer.GetWaterMagicSkillScore() > 14 && ctx.targetPlayer.GetAirMagicSkillScore() > 14)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Artisan:

            for (byte slotId = 0; slotId < 13; slotId++)
            {
              var slot = ctx.targetPlayer.oid.LoginCreature.GetQuickBarButton(slotId);

              if (slot.ObjectType == QuickBarButtonType.Feat && SkillSystem.learnableDictionary.ContainsKey(slot.Param1) && (SkillSystem.learnableDictionary[slot.Param1]).type == SkillSystem.Type.Signet)
                ctx.targetAC[DamageType.BaseWeapon] += 1;
            }
            break;

          case CustomInscription.GardeDragon:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dragon)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeExtérieur:
            if (ctx.oAttacker.Race.RacialType == RacialType.Outsider)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeAberration:
            if (ctx.oAttacker.Race.RacialType == RacialType.Aberration)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeElementaire:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elemental)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Inflexible: ctx.targetAC[(DamageType)8192] += 1; break;
          case CustomInscription.Redoutable: ctx.targetAC[(DamageType)16384] += 1; break;
          case CustomInscription.Marchevent:

            int positiveEffect = 0;
            foreach (Effect eff in ctx.targetPlayer.oid.LoginCreature.ActiveEffects)
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
            if (ctx.oAttacker.Race.RacialType == RacialType.Giant)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeMagie:
            if (ctx.oAttacker.Race.RacialType == RacialType.MagicalBeast)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeBon:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Good)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeChaos:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Chaotic)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeMal:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Evil)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeNeutre:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Neutral && ctx.oAttacker.LawChaosAlignment == Alignment.Neutral)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.GardeLoi:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Lawful)
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
            if (ctx.oAttacker.Race.RacialType == RacialType.Halfling)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeHumain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Human)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeDemiElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfElf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeDemiOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeGnome:
            if (ctx.oAttacker.Race.RacialType == RacialType.Gnome)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeNain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dwarf)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Agitateur:
            if (ctx.targetPlayer.oid.LoginCreature.AttackTarget is not null && (ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Walking
              || ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Running))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Sentinelle:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("_STANCE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Belluaire:
            if (ctx.targetPlayer.oid.LoginCreature.Faction.GetMembers().Any(a => a.Master == ctx.targetPlayer.oid.LoginCreature && a.Tag.Contains("_ANIMAL_COMPANION_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Eclaireur:
            if (ctx.targetPlayer.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_IS_USING_PREPARATION").HasValue)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Disciple:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_CONDITION_")))
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Virtuose:
            if (ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast2
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast3 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast4
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast5 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.CastCreature
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure2)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.Fossoyeur:
            double hpLeft = ctx.targetPlayer.oid.LoginCreature.HP / ctx.targetPlayer.MaxHP;

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
              var slot = ctx.targetPlayer.oid.LoginCreature.GetQuickBarButton(slotId);

              if (slot.ObjectType == QuickBarButtonType.Feat && ctx.targetPlayer.oid.LoginCreature.GetFeatRemainingUses(NwFeat.FromFeatId(slot.Param1)) < 1)
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
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Bénédiction:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Centurion:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_SHOUT_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.Oublié:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => !e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.GardeNonVie:
            if (ctx.oAttacker.Race.RacialType == RacialType.Undead)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeArtifice:
            if (ctx.oAttacker.Race.RacialType == RacialType.Construct)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.Lieutenant:
            ctx.targetAC[DamageType.BaseWeapon] -= 4;
            break;

          case CustomInscription.MaîtreBlème:
            int controlledUndead = ctx.targetPlayer.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == ctx.targetPlayer.oid.LoginCreature && a.Race.RacialType == RacialType.Undead);

            switch (controlledUndead)
            {
              case 1: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 3: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 5: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.Marionnettiste:
            int controlledSummons = ctx.targetPlayer.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == ctx.targetPlayer.oid.LoginCreature && a.Race.RacialType != RacialType.Undead && !a.Tag.Contains("_ANIMAL_COMPANION_"));

            switch (controlledSummons)
            {
              case 1: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
              case 3: ctx.targetAC[DamageType.BaseWeapon] += 2; break;
              case 5: ctx.targetAC[DamageType.BaseWeapon] += 3; break;
              case 8: ctx.targetAC[DamageType.BaseWeapon] += 4; break;
            }

            break;

          case CustomInscription.GardeMonstre:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeHumanoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeMétamorphe:
            if (ctx.oAttacker.Race.RacialType == RacialType.ShapeChanger)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeGoblinoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidGoblinoid)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeAnimal:
            if (ctx.oAttacker.Race.RacialType == RacialType.Animal)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeReptilien:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidReptilian)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.GardeVermine:
            if (ctx.oAttacker.Race.RacialType == RacialType.Vermin)
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;
        }
      }
    }
    public static void SetArmorValueFromShield(Context ctx)
    {
      NwItem item = ctx.targetPlayer?.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if (item is null || item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
        return;

      int shieldProficiency = ctx.targetPlayer.GetShieldProficiencyLevel(item.BaseItem.ItemType) / 10;

      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value * shieldProficiency))
        ctx.targetAC[DamageType.BaseWeapon] += item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value * shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Piercing, item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value * shieldProficiency))
        ctx.targetAC[DamageType.Piercing] += item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value * shieldProficiency;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Blindé: ctx.targetAC[DamageType.BaseWeapon] += 1; break;
          case CustomInscription.RepousseDragon:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dragon)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseExtérieur:
            if (ctx.oAttacker.Race.RacialType == RacialType.Outsider)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseAberration:
            if (ctx.oAttacker.Race.RacialType == RacialType.Aberration)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.LongueVieAuRoi:
            if (ctx.targetPlayer.oid.LoginCreature.HP > ctx.targetPlayer.MaxHP / 2)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.LaFoiEstMonBouclier:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
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
            if (ctx.oAttacker.Race.RacialType == RacialType.Giant)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseMagie:
            if (ctx.oAttacker.Race.RacialType == RacialType.MagicalBeast)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseBon:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Good)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseChaos:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Chaotic)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseMal:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Evil)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseNeutre:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Neutral && ctx.oAttacker.LawChaosAlignment == Alignment.Neutral)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseLoi:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Lawful)
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
            if (ctx.oAttacker.Race.RacialType == RacialType.Halfling)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseHumain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Human)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseDemiElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfElf)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseDemiOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elf)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseGnome:
            if (ctx.oAttacker.Race.RacialType == RacialType.Gnome)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseNain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dwarf)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseElementaire:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elemental)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.LaRaisonDuPlusFort:
            if (ctx.targetPlayer.oid.LoginCreature.AttackTarget is not null && (ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Walking
              || ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Running))
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.SavoirNestQueLaMoitiéDuChemin:
            if (ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast2
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast3 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast4
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast5 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.CastCreature
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure2)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.CeNestQuuneEgratignure:
            if (ctx.targetPlayer.oid.LoginCreature.HP < ctx.targetPlayer.MaxHP / 2)
              ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.NeTremblezPas:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              ctx.targetAC[DamageType.BaseWeapon] += 3;
            break;

          case CustomInscription.RepousseNonVie:
            if (ctx.oAttacker.Race.RacialType == RacialType.Undead)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseArtifice:
            if (ctx.oAttacker.Race.RacialType == RacialType.Construct)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.HeureuxLesSimplesdEsprits:
          case CustomInscription.LaVieNestQueDouleur:
            ctx.targetAC[DamageType.BaseWeapon] += 1;
            break;

          case CustomInscription.RepousseMonstre:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidOrc)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseHumanoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidMonstrous)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseMétamorphe:
            if (ctx.oAttacker.Race.RacialType == RacialType.ShapeChanger)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseGobelinoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidGoblinoid)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseAnimal:
            if (ctx.oAttacker.Race.RacialType == RacialType.Animal)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseReptilien:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidReptilian)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;

          case CustomInscription.RepousseVermine:
            if (ctx.oAttacker.Race.RacialType == RacialType.Vermin)
              ctx.targetAC[DamageType.BaseWeapon] += 2;
            break;
        }
      }
    }
    public static void GetArmorValueFromWeapon(Context ctx, NwItem item)
    {
      if (item is null || item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
        return;

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
    public static void SetDamageValueFromWeapon(Context ctx, NwItem focus = null)
    {
      if (ctx.attackWeapon is null || ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
        return;
      
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

      for (int i = 0; i < ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (ctx.attackWeapon.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (ctx.attackWeapon.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
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

      for (int i = 0; i < ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (ctx.attackWeapon.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (ctx.attackWeapon.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Pourfendeur: baseDamage *= 1.01; break;
          case CustomInscription.QueDuMuscle:
          case CustomInscription.Masochisme: baseDamage *= 1.02; break;
          case CustomInscription.Vampirisme: vampirism = true; break;
          case CustomInscription.Zèle: zele = true; break;

          case CustomInscription.PourfendeurDragon:
            if (ctx.oTarget.Race.RacialType == RacialType.Dragon) 
              baseDamage *= 1.03; 
            break;

          case CustomInscription.PourfendeurExtérieur:
            if (ctx.oTarget.Race.RacialType == RacialType.Outsider)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurAberration:
            if (ctx.oTarget.Race.RacialType == RacialType.Aberration)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurGéant:
            if (ctx.oTarget.Race.RacialType == RacialType.Giant)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurElementaire:
            if (ctx.oTarget.Race.RacialType == RacialType.Elemental)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurMagie:
            if (ctx.oTarget.Race.RacialType == RacialType.MagicalBeast)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurHalfelin:
            if (ctx.oTarget.Race.RacialType == RacialType.Halfling)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurHumain:
            if (ctx.oTarget.Race.RacialType == RacialType.Human)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurDemiElfe:
            if (ctx.oTarget.Race.RacialType == RacialType.HalfElf)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurDemiOrc:
            if (ctx.oTarget.Race.RacialType == RacialType.HalfOrc)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurElfe:
            if (ctx.oTarget.Race.RacialType == RacialType.Elf)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurGnome:
            if (ctx.oTarget.Race.RacialType == RacialType.Gnome)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurNain:
            if (ctx.oTarget.Race.RacialType == RacialType.Dwarf)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurNonVie:
            if (ctx.oTarget.Race.RacialType == RacialType.Undead)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurArtificiel:
            if (ctx.oTarget.Race.RacialType == RacialType.Construct)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurOrc:
            if (ctx.oTarget.Race.RacialType == RacialType.HumanoidOrc)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurMonstres:
            if (ctx.oTarget.Race.RacialType == RacialType.Beast)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurHumanoïdes:
            if (ctx.oTarget.Race.RacialType == RacialType.HumanoidMonstrous)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurMétamorphes:
            if (ctx.oTarget.Race.RacialType == RacialType.ShapeChanger)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurGobelins:
            if (ctx.oTarget.Race.RacialType == RacialType.HumanoidGoblinoid)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurAnimal:
            if (ctx.oTarget.Race.RacialType == RacialType.Animal)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurReptilien:
            if (ctx.oTarget.Race.RacialType == RacialType.HumanoidReptilian)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurVermine:
            if (ctx.oTarget.Race.RacialType == RacialType.Vermin)
              baseDamage *= 1.03;
            break;

          case CustomInscription.PourfendeurBien:
            if (ctx.oTarget.GoodEvilAlignment == Alignment.Good)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurChaos:
            if (ctx.oTarget.LawChaosAlignment == Alignment.Chaotic)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurMal:
            if (ctx.oTarget.GoodEvilAlignment == Alignment.Evil)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurNeutralité:
            if (ctx.oTarget.GoodEvilAlignment == Alignment.Neutral && ctx.oTarget.LawChaosAlignment == Alignment.Neutral)
              baseDamage *= 1.02;
            break;

          case CustomInscription.PourfendeurLoi:
            if (ctx.oTarget.LawChaosAlignment == Alignment.Lawful)
              baseDamage *= 1.02;
            break;

          case CustomInscription.ForceEtHonneur:
            if (ctx.attackingPlayer.oid.LoginCreature.HP > ctx.attackingPlayer.MaxHP / 2)
              baseDamage *= 1.02;
            break;

          case CustomInscription.VengeanceSeraMienne:
            if (ctx.attackingPlayer.oid.LoginCreature.HP < ctx.attackingPlayer.MaxHP / 2)
              baseDamage *= 1.03;
            break;

          case CustomInscription.MaîtreDeSonDestin:
            if (ctx.attackingPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              baseDamage *= 1.02;
            break;
            
          case CustomInscription.DanseAvecLaMort:
            if (ctx.attackingPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("_STANCE_")))
              baseDamage *= 1.02;
            break;

          case CustomInscription.Sadisme:
            if (ctx.attackingPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
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

        ctx.attackingPlayer.oid.LoginCreature.HP = ctx.attackingPlayer.oid.LoginCreature.HP + 3 >= ctx.attackingPlayer.MaxHP ? ctx.attackingPlayer.MaxHP : ctx.attackingPlayer.oid.LoginCreature.HP + 3;
      }

      if(zele)
        ctx.attackingPlayer.endurance.currentMana = ctx.attackingPlayer.endurance.currentMana + 1 >= ctx.attackingPlayer.endurance.maxMana ? ctx.attackingPlayer.endurance.maxMana : ctx.attackingPlayer.endurance.currentMana + 1;

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
