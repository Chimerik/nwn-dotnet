
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
    /*public static void SetArmorValueFromArmorPiece(Context ctx)
    {
      if (ctx.targetPlayer is null || ctx.targetArmor is null || ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
        return;

      int armorBaseAC = 0;
      int armorElementalAC = 0;
      int armorPhysicalAC = 0;
      int armorPercingAC = 0;
      int armorSlashingAC = 0;
      int armorBludgeoningAC = 0;
      int armorFireAC = 0;
      int armorColdAC = 0;
      int armorAcidAC = 0;
      int armorElectricalAC = 0;

      if (ctx.targetArmor.BaseItem.ItemType == BaseItemType.Armor)
      {
        armorBaseAC += ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;
        armorPhysicalAC += ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_PHYSICAL_ARMOR").Value;
        armorElementalAC += ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ELEMENTAL_ARMOR").Value;
      }
      else // Dans le cas où il s'agit d'une pièce d'armure, on définit le type d'armure à partir de la pièce principale du torse
      {
        NwItem baseArmor = ctx.oTarget.GetItemInSlot(InventorySlot.Chest);

        if (baseArmor is not null)
        {
          armorBaseAC += baseArmor.BaseACValue * 3 * ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;

          switch (baseArmor.BaseACValue)
          {
            case 1:
            case 2:
            case 3:
            case 4:
              armorElementalAC += baseArmor.BaseACValue * 5;
              break;
            case 5:
                armorElementalAC += 30;
              armorPhysicalAC += 5;
              break;
            case 6:
              armorPhysicalAC += 10;
              break;
            case 7:
                armorPhysicalAC += 15;
              break;
            case 8:
                armorPhysicalAC += 20;
              break;
          }
        }
      }

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
              armorBaseAC += 1;
            break;

          case CustomInscription.Artisan:

            for (byte slotId = 0; slotId < 13; slotId++)
            {
              var slot = ctx.targetPlayer.oid.LoginCreature.GetQuickBarButton(slotId);

              if (slot.ObjectType == QuickBarButtonType.Feat && SkillSystem.learnableDictionary.ContainsKey(slot.Param1) && (SkillSystem.learnableDictionary[slot.Param1]).type == SkillSystem.Type.Signet)
                armorBaseAC += 1;
            }
            break;

          case CustomInscription.GardeDragon:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dragon)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeExtérieur:
            if (ctx.oAttacker.Race.RacialType == RacialType.Outsider)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeAberration:
            if (ctx.oAttacker.Race.RacialType == RacialType.Aberration)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeElementaire:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elemental)
              armorBaseAC += 3;
            break;

          case CustomInscription.Inflexible: armorPhysicalAC += 1; break;
          case CustomInscription.Redoutable: armorElementalAC += 1; break;
          case CustomInscription.Marchevent:

            int positiveEffect = 0;
            foreach (Effect eff in ctx.targetPlayer.oid.LoginCreature.ActiveEffects)
            {
              if (eff.Tag.Contains("CUSTOM_POSITIVE_SPELL_"))
              {
                positiveEffect++;
                if (positiveEffect > 2)
                  armorBaseAC += 1;

                if (positiveEffect > 5)
                  break;
              }
            }

            break;

          case CustomInscription.GardeGeant:
            if (ctx.oAttacker.Race.RacialType == RacialType.Giant)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeMagie:
            if (ctx.oAttacker.Race.RacialType == RacialType.MagicalBeast)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeBon:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Good)
              armorBaseAC += 2;
            break;

          case CustomInscription.GardeChaos:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Chaotic)
              armorBaseAC += 2;
            break;

          case CustomInscription.GardeMal:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Evil)
              armorBaseAC += 2;
            break;

          case CustomInscription.GardeNeutre:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Neutral && ctx.oAttacker.LawChaosAlignment == Alignment.Neutral)
              armorBaseAC += 2;
            break;

          case CustomInscription.GardeLoi:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Lawful)
              armorBaseAC += 2;
            break;

          case CustomInscription.Hivernal:
            if (!ctx.targetAC.TryAdd(DamageType.Cold, 2))
              armorColdAC += 2; 
            break;
          case CustomInscription.Ignifugé:
            if (!ctx.targetAC.TryAdd(DamageType.Fire, 2))
              armorFireAC += 2; 
            break;
          case CustomInscription.Paratonnerre: 
            if (!ctx.targetAC.TryAdd(DamageType.Electrical, 2))
              armorElectricalAC += 2; 
            break;
          case CustomInscription.Tectonique: 
            if (!ctx.targetAC.TryAdd(DamageType.Acid, 2))
              armorAcidAC += 2; 
            break;
          case CustomInscription.Infiltrateur:
            if (!ctx.targetAC.TryAdd(DamageType.Piercing, 2)) 
              armorPercingAC += 1; 
            break;
          case CustomInscription.Saboteur: 
            if (!ctx.targetAC.TryAdd(DamageType.Slashing, 2))
              armorSlashingAC += 1; 
            break;
          case CustomInscription.AvantGarde: 
            if (!ctx.targetAC.TryAdd(DamageType.Bludgeoning, 2))
              armorBludgeoningAC += 1; 
            break;
          case CustomInscription.GardeHalfelin:
            if (ctx.oAttacker.Race.RacialType == RacialType.Halfling)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeHumain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Human)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeDemiElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfElf)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeDemiOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfOrc)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elf)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeGnome:
            if (ctx.oAttacker.Race.RacialType == RacialType.Gnome)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeNain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dwarf)
              armorBaseAC += 3;
            break;

          case CustomInscription.Agitateur:
            if (ctx.targetPlayer.oid.LoginCreature.AttackTarget is not null && (ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Walking
              || ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Running))
              armorBaseAC += 1;
            break;

          case CustomInscription.Sentinelle:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("_STANCE_")))
              armorBaseAC += 1;
            break;

          case CustomInscription.Belluaire:
            if (ctx.targetPlayer.oid.LoginCreature.Faction.GetMembers().Any(a => a.Master == ctx.targetPlayer.oid.LoginCreature && a.Tag.Contains("_ANIMAL_COMPANION_")))
              armorBaseAC += 1;
            break;

          case CustomInscription.Eclaireur:
            if (ctx.targetPlayer.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_IS_USING_PREPARATION").HasValue)
              armorBaseAC += 1;
            break;

          case CustomInscription.Disciple:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_CONDITION_")))
              armorBaseAC += 2;
            break;

          case CustomInscription.Virtuose:
            if (ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast2
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast3 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast4
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast5 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.CastCreature
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure2)
              armorBaseAC += 2;
            break;

          case CustomInscription.Fossoyeur:
            double hpLeft = ctx.targetPlayer.oid.LoginCreature.HP / ctx.targetPlayer.MaxHP;

            if (hpLeft > 0.6)
              armorBaseAC += 1;
            else if (hpLeft > 0.4)
              armorBaseAC += 2;
            else if (hpLeft > 0.2)
              armorBaseAC += 3;
            else
              armorBaseAC += 4;

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
              case 2: armorBaseAC += 1; break;
              case 4: armorBaseAC += 2; break;
              case 6: armorBaseAC += 3; break;
              case 8: armorBaseAC += 4; break;
            }

            break;

          case CustomInscription.Destructeur:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              armorBaseAC += 3;
            break;

          case CustomInscription.Bénédiction:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              armorBaseAC += 1;
            break;

          case CustomInscription.Centurion:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_SHOUT_")))
              armorBaseAC += 1;
            break;

          case CustomInscription.Oublié:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => !e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              armorBaseAC += 1;
            break;

          case CustomInscription.GardeNonVie:
            if (ctx.oAttacker.Race.RacialType == RacialType.Undead)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeArtifice:
            if (ctx.oAttacker.Race.RacialType == RacialType.Construct)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidOrc)
              armorBaseAC += 3;
            break;

          case CustomInscription.Lieutenant:
            armorBaseAC -= 4;
            break;

          case CustomInscription.MaîtreBlème:
            int controlledUndead = ctx.targetPlayer.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == ctx.targetPlayer.oid.LoginCreature && a.Race.RacialType == RacialType.Undead);

            switch (controlledUndead)
            {
              case 1: armorBaseAC += 1; break;
              case 3: armorBaseAC += 2; break;
              case 5: armorBaseAC += 3; break;
              case 8: armorBaseAC += 4; break;
            }

            break;

          case CustomInscription.Marionnettiste:
            int controlledSummons = ctx.targetPlayer.oid.LoginCreature.Faction.GetMembers().Count(a => a.Master == ctx.targetPlayer.oid.LoginCreature && a.Race.RacialType != RacialType.Undead && !a.Tag.Contains("_ANIMAL_COMPANION_"));

            switch (controlledSummons)
            {
              case 1: armorBaseAC += 1; break;
              case 3: armorBaseAC += 2; break;
              case 5: armorBaseAC += 3; break;
              case 8: armorBaseAC += 4; break;
            }

            break;

          case CustomInscription.GardeMonstre:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidMonstrous)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeHumanoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidMonstrous)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeMétamorphe:
            if (ctx.oAttacker.Race.RacialType == RacialType.ShapeChanger)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeGoblinoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidGoblinoid)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeAnimal:
            if (ctx.oAttacker.Race.RacialType == RacialType.Animal)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeReptilien:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidReptilian)
              armorBaseAC += 3;
            break;

          case CustomInscription.GardeVermine:
            if (ctx.oAttacker.Race.RacialType == RacialType.Vermin)
              armorBaseAC += 3;
            break;
        }
      }
      int armorProficiency = ctx.targetPlayer.GetArmorProficiencyLevel(ctx.targetArmor.BaseACValue) < ItemUtils.GetItemProficiencyRequirement(ctx.targetArmor) ? 3 : 1;

      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, armorBaseAC / armorProficiency))
        ctx.targetAC[DamageType.BaseWeapon] += armorBaseAC / armorProficiency;

      if (!ctx.targetAC.TryAdd((DamageType)8192, armorPhysicalAC / armorProficiency))
        ctx.targetAC[(DamageType)8192] += armorPhysicalAC / armorProficiency;

      if (!ctx.targetAC.TryAdd((DamageType)16384, armorElementalAC / armorProficiency))
        ctx.targetAC[(DamageType)16384] += armorElementalAC / armorProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Piercing, armorPercingAC / armorProficiency))
        ctx.targetAC[DamageType.Piercing] += armorPercingAC / armorProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Slashing, armorPercingAC / armorProficiency))
        ctx.targetAC[DamageType.Slashing] += armorPercingAC / armorProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Bludgeoning, armorPercingAC / armorProficiency))
        ctx.targetAC[DamageType.Bludgeoning] += armorPercingAC / armorProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Fire, armorPercingAC / armorProficiency))
        ctx.targetAC[DamageType.Fire] += armorPercingAC / armorProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Acid, armorPercingAC / armorProficiency))
        ctx.targetAC[DamageType.Acid] += armorPercingAC / armorProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Cold, armorPercingAC / armorProficiency))
        ctx.targetAC[DamageType.Cold] += armorPercingAC / armorProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Electrical, armorPercingAC / armorProficiency))
        ctx.targetAC[DamageType.Electrical] += armorPercingAC / armorProficiency;
    }
    public static void SetArmorValueFromShield(Context ctx)
    {
      NwItem item = ctx.targetPlayer?.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if (item is null || item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
        return;

      int shieldBaseAC = item.GetObjectVariable<LocalVariableInt>("_BASE_ARMOR").Value;
      int shieldElementalAC = 0;
      int shieldPhysicalAC = 0;
      int shieldPercingAC = item.GetObjectVariable<LocalVariableInt>("_BASE_PIERCING_ARMOR").Value;
      int shieldSlashingAC = 0;
      int shieldBludgeoningAC = 0;
      int shieldFireAC = 0;
      int shieldColdAC = 0;
      int shieldAcidAC = 0;
      int shieldElectricalAC = 0;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Blindé: shieldBaseAC += 1; break;
          case CustomInscription.RepousseDragon:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dragon)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseExtérieur:
            if (ctx.oAttacker.Race.RacialType == RacialType.Outsider)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseAberration:
            if (ctx.oAttacker.Race.RacialType == RacialType.Aberration)
              shieldBaseAC += 2;
            break;

          case CustomInscription.LongueVieAuRoi:
            if (ctx.targetPlayer.oid.LoginCreature.HP > ctx.targetPlayer.MaxHP / 2)
              shieldBaseAC += 1;
            break;

          case CustomInscription.LaFoiEstMonBouclier:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_POSITIVE_SPELL_")))
              shieldBaseAC += 1;
            break;

          case CustomInscription.LaSurvieDuMieuxEquipé:
            if (!ctx.targetAC.TryAdd((DamageType)8192, 1))
              shieldPhysicalAC += 1;
            break;

          case CustomInscription.ParéEnTouteSaison:
            if (!ctx.targetAC.TryAdd((DamageType)16384, 1))
              shieldElementalAC += 1;
            break;

          case CustomInscription.RepousseGéant:
            if (ctx.oAttacker.Race.RacialType == RacialType.Giant)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseMagie:
            if (ctx.oAttacker.Race.RacialType == RacialType.MagicalBeast)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseBon:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Good)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseChaos:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Chaotic)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseMal:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Evil)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseNeutre:
            if (ctx.oAttacker.GoodEvilAlignment == Alignment.Neutral && ctx.oAttacker.LawChaosAlignment == Alignment.Neutral)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseLoi:
            if (ctx.oAttacker.LawChaosAlignment == Alignment.Lawful)
              shieldBaseAC += 2;
            break;

          case CustomInscription.ContreVentsEtMarées: ctx.targetAC[DamageType.Piercing] += 2;
            break;

          case CustomInscription.lEnigmeDelAcier:
            if (!ctx.targetAC.TryAdd(DamageType.Slashing, 2))
              shieldSlashingAC += 2;
            break;

          case CustomInscription.PasLeVisage:
            if (!ctx.targetAC.TryAdd(DamageType.Bludgeoning, 2))
              shieldBludgeoningAC += 2;
            break;

          case CustomInscription.PortéParLeVent:
            if (!ctx.targetAC.TryAdd(DamageType.Cold, 2))
              shieldColdAC += 2;
            break;

          case CustomInscription.CommeUnRoc:
            if (!ctx.targetAC.TryAdd(DamageType.Acid, 2))
              shieldAcidAC += 2;
            break;

          case CustomInscription.Illumination:
            if (!ctx.targetAC.TryAdd(DamageType.Fire, 2))
              shieldFireAC += 2;
            break;

          case CustomInscription.ChevaucheLaTempête:
            if (!ctx.targetAC.TryAdd(DamageType.Electrical, 2))
              shieldElectricalAC += 2;
            break;

          case CustomInscription.RepousseHalfelin:
            if (ctx.oAttacker.Race.RacialType == RacialType.Halfling)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseHumain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Human)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseDemiElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfElf)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseDemiOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HalfOrc)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseElfe:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elf)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseGnome:
            if (ctx.oAttacker.Race.RacialType == RacialType.Gnome)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseNain:
            if (ctx.oAttacker.Race.RacialType == RacialType.Dwarf)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseElementaire:
            if (ctx.oAttacker.Race.RacialType == RacialType.Elemental)
              shieldBaseAC += 2;
            break;

          case CustomInscription.LaRaisonDuPlusFort:
            if (ctx.targetPlayer.oid.LoginCreature.AttackTarget is not null && (ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Walking
              || ctx.targetPlayer.oid.LoginCreature.AnimationState != AnimationState.Running))
              shieldBaseAC += 1;
            break;

          case CustomInscription.SavoirNestQueLaMoitiéDuChemin:
            if (ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast2
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast3 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast4
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Cast5 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.CastCreature
              || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure1 || ctx.targetPlayer.oid.LoginCreature.AnimationState == AnimationState.Conjure2)
              shieldBaseAC += 1;
            break;

          case CustomInscription.CeNestQuuneEgratignure:
            if (ctx.targetPlayer.oid.LoginCreature.HP < ctx.targetPlayer.MaxHP / 2)
              shieldBaseAC += 1;
            break;

          case CustomInscription.NeTremblezPas:
            if (ctx.targetPlayer.oid.LoginCreature.ActiveEffects.Any(e => e.Tag.Contains("CUSTOM_MALEFICE_")))
              shieldBaseAC += 3;
            break;

          case CustomInscription.RepousseNonVie:
            if (ctx.oAttacker.Race.RacialType == RacialType.Undead)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseArtifice:
            if (ctx.oAttacker.Race.RacialType == RacialType.Construct)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseOrc:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidOrc)
              shieldBaseAC += 2;
            break;

          case CustomInscription.HeureuxLesSimplesdEsprits:
          case CustomInscription.LaVieNestQueDouleur:
            shieldBaseAC += 1;
            break;

          case CustomInscription.RepousseMonstre:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidOrc)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseHumanoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidMonstrous)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseMétamorphe:
            if (ctx.oAttacker.Race.RacialType == RacialType.ShapeChanger)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseGobelinoïde:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidGoblinoid)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseAnimal:
            if (ctx.oAttacker.Race.RacialType == RacialType.Animal)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseReptilien:
            if (ctx.oAttacker.Race.RacialType == RacialType.HumanoidReptilian)
              shieldBaseAC += 2;
            break;

          case CustomInscription.RepousseVermine:
            if (ctx.oAttacker.Race.RacialType == RacialType.Vermin)
              shieldBaseAC += 2;
            break;
        }
      }

      int shieldProficiency = ctx.targetPlayer.GetShieldProficiencyLevel(item.BaseItem.ItemType) < ItemUtils.GetItemProficiencyRequirement(item) ? 3 : 1;

      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, shieldBaseAC / shieldProficiency))
        ctx.targetAC[DamageType.BaseWeapon] += shieldBaseAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd((DamageType)8192, shieldPhysicalAC / shieldProficiency))
        ctx.targetAC[(DamageType)8192] += shieldPhysicalAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd((DamageType)16384, shieldElementalAC / shieldProficiency))
        ctx.targetAC[(DamageType)16384] += shieldElementalAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Piercing, shieldPercingAC / shieldProficiency))
        ctx.targetAC[DamageType.Piercing] += shieldPercingAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Slashing, shieldPercingAC / shieldProficiency))
        ctx.targetAC[DamageType.Slashing] += shieldPercingAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Bludgeoning, shieldPercingAC / shieldProficiency))
        ctx.targetAC[DamageType.Bludgeoning] += shieldPercingAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Fire, shieldPercingAC / shieldProficiency))
        ctx.targetAC[DamageType.Fire] += shieldPercingAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Acid, shieldPercingAC / shieldProficiency))
        ctx.targetAC[DamageType.Acid] += shieldPercingAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Cold, shieldPercingAC / shieldProficiency))
        ctx.targetAC[DamageType.Cold] += shieldPercingAC / shieldProficiency;

      if (!ctx.targetAC.TryAdd(DamageType.Electrical, shieldPercingAC / shieldProficiency))
        ctx.targetAC[DamageType.Electrical] += shieldPercingAC / shieldProficiency;
    }
    public static void GetArmorValueFromWeapon(Context ctx, NwItem item)
    {
      if (item is null || item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
        return;

      int weaponBaseArmor = 0;
      int weaponPhysicalArmor = 0;
      int weaponElementalArmor = 0;

      for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
      {
        if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").HasNothing)
          continue;

        switch (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value)
        {
          case CustomInscription.Défense: weaponBaseArmor += 1; break;
          case CustomInscription.Masochisme: weaponBaseArmor -= 1; break;
          case CustomInscription.Refuge: weaponPhysicalArmor += 1; break;
          case CustomInscription.Protecteur: weaponElementalArmor += 1; break;
        }
      }

      int weaponProficiency = ctx.targetPlayer.GetWeaponMasteryLevel(item) < ItemUtils.GetItemProficiencyRequirement(item) ? 3 : 1;

      if (!ctx.targetAC.TryAdd(DamageType.BaseWeapon, weaponBaseArmor / weaponProficiency))
        ctx.targetAC[DamageType.BaseWeapon] += weaponBaseArmor / weaponProficiency;

      if (!ctx.targetAC.TryAdd((DamageType)8192, weaponPhysicalArmor / weaponProficiency))
        ctx.targetAC[(DamageType)8192] += weaponPhysicalArmor / weaponProficiency;

      if (!ctx.targetAC.TryAdd((DamageType)16384, weaponElementalArmor / weaponProficiency))
        ctx.targetAC[(DamageType)16384] += weaponElementalArmor / weaponProficiency;
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

      int weaponProficiency = ctx.attackingPlayer.GetWeaponMasteryLevel(ctx.attackWeapon) < ItemUtils.GetItemProficiencyRequirement(ctx.attackWeapon) ? 3 : 1;

      if (vampirism)
      {
        if (GetContextDamage(ctx, DamageType.Negative) < 0)
          SetContextDamage(ctx, DamageType.Negative, 3 / weaponProficiency);
        else
          SetContextDamage(ctx, DamageType.Negative, GetContextDamage(ctx, DamageType.Negative) + 3 / weaponProficiency);

        ctx.attackingPlayer.oid.LoginCreature.HP = ctx.attackingPlayer.oid.LoginCreature.HP + 3 >= ctx.attackingPlayer.MaxHP ? ctx.attackingPlayer.MaxHP : ctx.attackingPlayer.oid.LoginCreature.HP + 3;
      }

      if(zele)
        ctx.attackingPlayer.endurance.currentMana = ctx.attackingPlayer.endurance.currentMana + 1 >= ctx.attackingPlayer.endurance.maxMana ? ctx.attackingPlayer.endurance.maxMana : ctx.attackingPlayer.endurance.currentMana + 1;

      if (doubleAdrenaline > 0 && doubleAdrenaline > NwRandom.Roll(Utils.random, 100))
        ctx.adrenalineGainModifier = 2;

      if (bonusPenetration > 0 && bonusPenetration > NwRandom.Roll(Utils.random, 100))
        ctx.baseArmorPenetration += 20 / weaponProficiency;

      if(baseDamage > -1) 
        SetContextDamage(ctx, DamageType.BaseWeapon, (int)Math.Round(baseDamage / weaponProficiency, MidpointRounding.ToEven));

      if (coldDamage > -1)
        SetContextDamage(ctx, DamageType.Cold, (int)Math.Round(coldDamage / weaponProficiency, MidpointRounding.ToEven));

      if (fireDamage > -1)
        SetContextDamage(ctx, DamageType.Fire, (int)Math.Round(fireDamage / weaponProficiency, MidpointRounding.ToEven));

      if (elecDamage > -1)
        SetContextDamage(ctx, DamageType.Electrical, (int)Math.Round(elecDamage / weaponProficiency, MidpointRounding.ToEven));

      if (acidDamage > -1)
        SetContextDamage(ctx, DamageType.Acid, (int)Math.Round(acidDamage / weaponProficiency, MidpointRounding.ToEven));

      if (piercingDamage > -1)
        SetContextDamage(ctx, DamageType.Piercing, (int)Math.Round(piercingDamage / weaponProficiency, MidpointRounding.ToEven));

      if (slashDamage > -1)
        SetContextDamage(ctx, DamageType.Slashing, (int)Math.Round(slashDamage / weaponProficiency, MidpointRounding.ToEven));

      if (bluntDamage > -1)
        SetContextDamage(ctx, DamageType.Bludgeoning, (int)Math.Round(bluntDamage / weaponProficiency, MidpointRounding.ToEven));
    }*/
  }
}
