using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using Action = System.Action;
using Context = NWN.Systems.Config.Context;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    private static ScriptHandleFactory scriptHandleFactory;

    public AttackSystem(ScriptHandleFactory scriptFactory)
    {
      scriptHandleFactory = scriptFactory;
    }

    public static Pipeline<Context> pipeline = new(
      new Action<Context, Action>[]
      {
        ProcessDazed,
        ProcessBaseDamageTypeAndAttackWeapon,
        ProcessCriticalHit,
        ProcessTargetDamageAbsorption,
        ProcessBaseArmorPenetration,
        //ProcessBonusArmorPenetration,
        ProcessAttackPosition,
        ProcessArmorSlotHit,
        ProcessTargetSpecificAC,
        ProcessTargetShieldAC,
        ProcessArmorPenetrationCalculations,
        ProcessDamageFromWeaponInscriptions,
        ProcessWeakness,
        ProcessDamageCalculations,
        ProcessAdrenaline,
        //ProcessSpecialAttack,
        //ProcessDoubleStrike,
        ProcessAttackerItemDurability,
        ProcessTargetItemDurability,
      }
    );
    public static void HandleAttackEvent(OnCreatureAttack onAttack)
    {
     // LogUtils.LogMessage($"Attack Event - Attacker {onAttack.Attacker.Name} - Target {onAttack.Target.Name} - Result {onAttack.AttackResult}" +
       // $" - Base damage {onAttack.DamageData.Base} - attack number {onAttack.AttackNumber} - attack type {onAttack.WeaponAttackType}", LogUtils.LogType.Combat);

      /*pipeline.Execute(new Context(
        onAttack: onAttack,
        oTarget: oTarget
      ));*/
    }
    private static void HandleItemRuined(NwCreature oPC, NwItem oItem)
    {
      //oPC.RunUnequip(oItem);
      oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = -1;
      oPC.ControllingPlayer.SendServerMessage($"Il ne reste plus que des ruines de votre {oItem.Name.ColorString(ColorConstants.White)}. Des réparations s'imposent !", ColorConstants.Red);
      LogUtils.LogMessage("Objet de qualité artisanale ruiné : à réparer !", LogUtils.LogType.Durability);
    }
    private static void ProcessDazed(Context ctx, Action next)
    {
      if (ctx.oTarget.AnimationState == AnimationState.Cast1 || ctx.oTarget.AnimationState == AnimationState.Cast2 || ctx.oTarget.AnimationState == AnimationState.Cast3 ||
        ctx.oTarget.AnimationState == AnimationState.Cast4 || ctx.oTarget.AnimationState == AnimationState.Cast5 || ctx.oTarget.AnimationState == AnimationState.CastCreature)
      {
        foreach (var eff in ctx.oTarget.ActiveEffects)
          if (eff.Tag == "CUSTOM_CONDITION_DAZED")
          {
            _ = ctx.oTarget.ClearActionQueue();
           ctx.oTarget.GetObjectVariable<LocalVariableInt>("_INTERRUPTED").Value = 1;
            // TODO : ajouter un effet d'echec du sort
            break;
          }
      }

      next();
    }
    private static void ProcessBaseDamageTypeAndAttackWeapon(Context ctx, Action next)
    {
      if (ctx.isUnarmedAttack && ctx.oAttacker.GetItemInSlot(InventorySlot.Arms) != null)
          ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.Arms);

      switch (ctx.onAttack.WeaponAttackType)
      {
        case WeaponAttackType.MainHand:
        case WeaponAttackType.HastedAttack:
          if (ctx.oAttacker.GetItemInSlot(InventorySlot.RightHand) != null)
          {
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.RightHand);

            if (ItemUtils.GetItemCategory(ctx.attackWeapon.BaseItem.ItemType) == ItemUtils.ItemCategory.RangedWeapon) // Pour les armes à distance, attaquer depuis une élévation augmente les dégâts (max 20 = + 200 %) 
              Config.SetContextDamage(ctx, DamageType.BaseWeapon, Config.GetContextDamage(ctx, DamageType.BaseWeapon) + (int)(Config.GetContextDamage(ctx, DamageType.BaseWeapon) * ((ctx.oAttacker?.Location.GroundHeight - ctx.oTarget?.Location.GroundHeight) * 0.1f)));
          }
          break;

        case WeaponAttackType.Offhand:
          if (ctx.oAttacker.GetItemInSlot(InventorySlot.LeftHand) != null)
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.LeftHand);
          break;

        case WeaponAttackType.CreatureBite:
          if (ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureBiteWeapon) != null)
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureBiteWeapon);
          break;

        case WeaponAttackType.CreatureLeft:
          if (ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureLeftWeapon) != null)
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
          break;

        case WeaponAttackType.CreatureRight:
          if (ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureRightWeapon) != null)
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureRightWeapon);
          break;
      }

      next();
    }
    private static void ProcessCriticalHit(Context ctx, Action next)
    {
      if (ctx.onAttack.AttackResult == AttackResult.CriticalHit)
      {
        ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComBloodCrtRed));
        StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, "Critique", StringUtils.gold);

        /*ctx.baseArmorPenetration += 20;
        ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComBloodCrtRed));
        StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, "Critique", ColorConstants.Orange);

        if (ctx.attackingPlayer.endurance.regenerableMana > 0 && ctx.attackingPlayer.endurance.currentMana < ctx.attackingPlayer.endurance.maxMana)
        {
          int mana = 0;
          int criticalMastery = ctx.attackingPlayer.GetAttributeLevel(SkillSystem.Attribut.CriticalStrikes);
          criticalMastery = criticalMastery > (ctx.oAttacker.GetAbilityScore(Ability.Dexterity, true) - 10) / 2 ? (ctx.oAttacker.GetAbilityScore(Ability.Dexterity, true) - 10) / 2 : criticalMastery;

          switch (criticalMastery)
          {
            case 3:
            case 4:
            case 5:
            case 6:
            case 7: mana = 1; break;
            case 8:
            case 9: 
            case 10:
            case 11:
            case 12: mana = 2; break;
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:  mana = 3;break;
            case 18:
            case 19:
            case 20:
            case 21:
            case 22:
            case 23:
            case 24:
            case 25: mana = 4; break;
          }

          ctx.attackingPlayer.endurance.currentMana = ctx.attackingPlayer.endurance.currentMana + mana > ctx.attackingPlayer.endurance.maxMana ? ctx.attackingPlayer.endurance.maxMana : ctx.attackingPlayer.endurance.currentMana + mana;
          ctx.attackingPlayer.endurance.regenerableMana -= mana;
        }*/
      }

      next();
    }
    private static void ProcessTargetDamageAbsorption(Context ctx, Action next)
    {
      if (ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin) != null)
      {
        List<ItemProperty> absorbIP = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.ImmunityDamageType && i.IntParams[3] > 7).ToList();

        foreach (var propType in absorbIP.GroupBy(i => i.Property.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          DamageType ipDamageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.SubType.RowIndex);

          switch ((IPDamageType)maxIP.SubType.RowIndex)
          {
            case IPDamageType.Bludgeoning:

              if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0 || Config.GetContextDamage(ctx, DamageType.Bludgeoning) > 0)
              {
                if (absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == IPDamageType.Physical && i.IntParams[3] > maxIP.IntParams[3]))
                  break;

                HandleAbsorbedDamageFromWeaponDamageType(ctx, DamageType.Bludgeoning, absorbIP, maxIP);
              }
              break;

            case IPDamageType.Slashing:

              if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0 || Config.GetContextDamage(ctx, DamageType.Slashing) > 0)
              {
                if (absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == IPDamageType.Physical && i.IntParams[3] > maxIP.IntParams[3]))
                  break;

                HandleAbsorbedDamageFromWeaponDamageType(ctx, DamageType.Slashing, absorbIP, maxIP);
              }
              break;

            case IPDamageType.Piercing:

              if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0 || Config.GetContextDamage(ctx, DamageType.Piercing) > 0)
              {
                if (absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == IPDamageType.Physical && i.IntParams[3] > maxIP.IntParams[3]))
                  break;

                HandleAbsorbedDamageFromWeaponDamageType(ctx, DamageType.Piercing, absorbIP, maxIP);
              }
              break;

            case IPDamageType.Physical:

              int absorptionValue = DamageImmunityCost2da.damageImmunityTable[maxIP.IntParams[3]].value;
              if (absorptionValue > 0)
              {
                absorptionValue = 100 / (absorptionValue - 100);
                int totalAbsorbedDamage = 0;
                int absorbedDamage = 0;

                if (Config.GetContextDamage(ctx, DamageType.Bludgeoning) > 0)
                {
                  absorbedDamage = Config.GetContextDamage(ctx, DamageType.Bludgeoning) / absorptionValue;
                  Config.SetContextDamage(ctx, DamageType.Bludgeoning, Config.GetContextDamage(ctx, DamageType.Bludgeoning) - absorbedDamage);
                  totalAbsorbedDamage += absorbedDamage;
                }
                
                if (Config.GetContextDamage(ctx, DamageType.Slashing) > 0)
                {
                  absorbedDamage = Config.GetContextDamage(ctx, DamageType.Slashing) / absorptionValue;
                  Config.SetContextDamage(ctx, DamageType.Slashing, Config.GetContextDamage(ctx, DamageType.Slashing) - absorbedDamage);
                  totalAbsorbedDamage += absorbedDamage;
                }
                
                if (Config.GetContextDamage(ctx, DamageType.Piercing) > 0)
                {
                  absorbedDamage = Config.GetContextDamage(ctx, DamageType.Piercing) / absorptionValue;
                  Config.SetContextDamage(ctx, DamageType.Piercing, Config.GetContextDamage(ctx, DamageType.Piercing) - absorbedDamage);
                  totalAbsorbedDamage += absorbedDamage;
                }

                if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0)
                {
                  absorbedDamage = Config.GetContextDamage(ctx, DamageType.BaseWeapon) / absorptionValue;
                  Config.SetContextDamage(ctx, DamageType.BaseWeapon,absorbedDamage);
                  totalAbsorbedDamage += absorbedDamage;
                }

                ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(absorbedDamage));
                ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHeal));

                StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, totalAbsorbedDamage.ToString(), new Color(32, 255, 32));
              }
              break;

            case IPDamageType.Acid:
              if (Config.GetContextDamage(ctx, DamageType.Acid) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == (IPDamageType)14 && i.IntParams[3] > maxIP.IntParams[3]))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Acid);
              break;

            case IPDamageType.Cold:
              if (Config.GetContextDamage(ctx, DamageType.Cold) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == (IPDamageType)14 && i.IntParams[3] > maxIP.IntParams[3]))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Cold);
              break;

            case IPDamageType.Electrical:
              if (Config.GetContextDamage(ctx, DamageType.Electrical) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == (IPDamageType)14 && i.IntParams[3] > maxIP.IntParams[3]))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Electrical);
              break;

            case IPDamageType.Fire:
              if (Config.GetContextDamage(ctx, DamageType.Fire) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == (IPDamageType)14 && i.IntParams[3] > maxIP.IntParams[3]))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Fire);
              break;

            case IPDamageType.Magical:
              if (Config.GetContextDamage(ctx, DamageType.Magical) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == (IPDamageType)14 && i.IntParams[3] > maxIP.IntParams[3]))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Magical);
              break;

            case IPDamageType.Sonic:
              if (Config.GetContextDamage(ctx, DamageType.Sonic) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType.RowIndex == (IPDamageType)14 && i.IntParams[3] > maxIP.IntParams[3]))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Sonic);
              break;

            case IPDamageType.Negative:
              if (Config.GetContextDamage(ctx, DamageType.Negative) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Negative);
              break;

            case IPDamageType.Positive:
              if (Config.GetContextDamage(ctx, DamageType.Positive) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Positive);
              break;

            case IPDamageType.Divine:
              if (Config.GetContextDamage(ctx, DamageType.Divine) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Divine);
              break;

            case (IPDamageType)14: // Damage type élémentaire

              if (Config.GetContextDamage(ctx, DamageType.Acid) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Acid);

              if (Config.GetContextDamage(ctx, DamageType.Cold) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Cold);

              if (Config.GetContextDamage(ctx, DamageType.Electrical) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Electrical);

              if (Config.GetContextDamage(ctx, DamageType.Fire) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Fire);

              if (Config.GetContextDamage(ctx, DamageType.Magical) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Magical);

              if (Config.GetContextDamage(ctx, DamageType.Sonic) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.IntParams[3], DamageType.Sonic);

              break;
          }
        }
      }

      next();
    }
    private static void HandleDamageAbsorbed(Context ctx, DamageType damageType, int ipCostValue, DamageType secondaryDamageType = DamageType.BaseWeapon)
    {
      int absorptionValue = DamageImmunityCost2da.damageImmunityTable[ipCostValue].value;

      foreach (Effect effect in ctx.oTarget.ActiveEffects)
      {
        if (effect.EffectType == EffectType.DamageImmunityIncrease && effect.IntParams[0] == (int)secondaryDamageType)
          absorptionValue += effect.IntParams[1];
        else if (effect.EffectType == EffectType.DamageImmunityDecrease && effect.IntParams[0] == (int)secondaryDamageType)
          absorptionValue -= effect.IntParams[1];
      }

      if (absorptionValue > 0)
      {
        absorptionValue = absorptionValue < 200 ? 100 / (absorptionValue - 100) : 200;
        int totalAbsorbedDamage = 0;
        int absorbedDamage = 0;

        if (secondaryDamageType == DamageType.BaseWeapon)
          secondaryDamageType = damageType;

        int damage = Config.GetContextDamage(ctx, secondaryDamageType);

        if (damage > 0)
        {
          absorbedDamage = damage / absorptionValue;
          Config.SetContextDamage(ctx, secondaryDamageType, damage - absorbedDamage);
          totalAbsorbedDamage += absorbedDamage;
        }

        if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0 && damageType != DamageType.BaseWeapon)
        {
          absorbedDamage = Config.GetContextDamage(ctx, DamageType.BaseWeapon) / absorptionValue;
          Config.SetContextDamage(ctx, DamageType.BaseWeapon, -absorbedDamage);
          totalAbsorbedDamage += absorbedDamage;
        }

        ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(absorbedDamage));
        ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHeal));

        StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, $"Absorbe {StringUtils.ToWhitecolor(totalAbsorbedDamage)}", new Color(32, 255, 32));
        LogUtils.LogMessage($"{ctx.oTarget.Name} absorbe {totalAbsorbedDamage} dégâts de type {secondaryDamageType}", LogUtils.LogType.Combat);
      }
    }
    private static void ProcessBaseArmorPenetration(Context ctx, Action next)
    {
     // if (ctx.oAttacker.IsFlanking(ctx.oTarget))
       // ctx.baseArmorPenetration += 5;

      // Les armes perforantes disposent de 20 % de pénétration supplémentaire contre les armures lourdes
      /*if (ctx.attackWeapon.BaseItem.WeaponType.Contains(DamageType.Piercing) && ctx.oTarget.GetItemInSlot(InventorySlot.Chest) is not null && ctx.oTarget.GetItemInSlot(InventorySlot.Chest).BaseACValue > 5)
        ctx.baseArmorPenetration += 20;*/

     /* if (ctx.attackingPlayer is not null && ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_NEXT_ATTACK").HasValue)
      {
        int athleticsLevel = ctx.attackingPlayer.learnableSkills.ContainsKey(CustomSkill.Athletics) ? ctx.attackingPlayer.learnableSkills[CustomSkill.Athletics].totalPoints : 0;
        athleticsLevel += ctx.attackingPlayer.learnableSkills.ContainsKey(CustomSkill.AthleticsExpert) ? ctx.attackingPlayer.learnableSkills[CustomSkill.AthleticsExpert].totalPoints : 0;
        athleticsLevel += ctx.attackingPlayer.learnableSkills.ContainsKey(CustomSkill.AthleticsScience) ? ctx.attackingPlayer.learnableSkills[CustomSkill.AthleticsScience].totalPoints : 0;
        athleticsLevel += ctx.attackingPlayer.learnableSkills.ContainsKey(CustomSkill.AthleticsMaster) ? ctx.attackingPlayer.learnableSkills[CustomSkill.AthleticsMaster].totalPoints : 0;
        athleticsLevel = athleticsLevel > (ctx.oAttacker.GetAbilityScore(Ability.Strength, true) - 10) / 2 ? (ctx.oAttacker.GetAbilityScore(Ability.Strength, true) - 10) / 2 : athleticsLevel;

        ctx.baseArmorPenetration += athleticsLevel * 4;
      }*/

      next();
    }
    /*private static void ProcessBonusArmorPenetration(Context ctx, Action next)
    {
      if (ctx.attackWeapon != null && ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value != -1)
      {
        int bonusDamage = 0; 

        foreach (var propType in ctx.attackWeapon.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.EnhancementBonus
           || i.Property.PropertyType == ItemPropertyType.DecreasedEnhancementModifier || i.Property.PropertyType == ItemPropertyType.DecreasedDamage
           || (i.Property.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i?.SubType?.RowIndex == (int)ctx.oTarget.Race.RacialType)
           || (i.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i?.SubType?.RowIndex == (int)ctx.oTarget.GoodEvilAlignment)
           || (i.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i?.SubType?.RowIndex == (int)ctx.oTarget.LawChaosAlignment)
           || (i.Property.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i?.SubType?.RowIndex == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget))
           || (i.Property.PropertyType == ItemPropertyType.EnhancementBonusVsRacialGroup && i?.SubType?.RowIndex == (int)ctx.oTarget.Race.RacialType)
           || (i.Property.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i?.SubType?.RowIndex == (int)ctx.oTarget.GoodEvilAlignment)
           || (i.Property.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i?.SubType?.RowIndex == (int)ctx.oTarget.LawChaosAlignment)
           || (i.Property.PropertyType == ItemPropertyType.EnhancementBonusVsSpecificAlignment && i?.SubType?.RowIndex == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget)))
          .GroupBy(i => i.Property.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          ctx.baseArmorPenetration += maxIP.IntParams[3];

          if (maxIP.Property.PropertyType is ItemPropertyType.EnhancementBonus or ItemPropertyType.DecreasedEnhancementModifier
            or ItemPropertyType.EnhancementBonusVsRacialGroup or ItemPropertyType.EnhancementBonusVsAlignmentGroup
            or ItemPropertyType.EnhancementBonusVsSpecificAlignment or ItemPropertyType.DecreasedEnhancementModifier)
            bonusDamage += maxIP.IntParams[3];
        }

        foreach (var effect in ctx.oAttacker.ActiveEffects)
        {
          if(effect.EffectType == EffectType.DamageIncrease)
          {
            if ((DamageType)effect.IntParams[1] == DamageType.BaseWeapon)
              bonusDamage += effect.IntParams[0];
            else
              Config.SetContextDamage(ctx, (DamageType)effect.IntParams[1], Config.GetContextDamage(ctx, (DamageType)effect.IntParams[1]) + effect.IntParams[0]);
          }
          else if (effect.EffectType == EffectType.DamageDecrease)
          {
            if ((DamageType)effect.IntParams[1] == DamageType.BaseWeapon)
              bonusDamage -= effect.IntParams[0];
            else
              Config.SetContextDamage(ctx, (DamageType)effect.IntParams[1], Config.GetContextDamage(ctx, (DamageType)effect.IntParams[1]) - effect.IntParams[0]);
          }
        }

        Config.SetContextDamage(ctx, DamageType.BaseWeapon, Config.GetContextDamage(ctx, DamageType.BaseWeapon) + bonusDamage);
      }

      next();
    }*/
    private static void ProcessAttackPosition(Context ctx, Action next)
    {
      if ((ctx.isUnarmedAttack || !ctx.isRangedAttack)
        && ctx.oAttacker.Size != ctx.oTarget.Size
        && !((ctx.oAttacker.Size == CreatureSize.Small && ctx.oTarget.Size == CreatureSize.Medium) || (ctx.oAttacker.Size == CreatureSize.Medium && ctx.oTarget.Size == CreatureSize.Small)))
      {
        if (ctx.oAttacker.Size > ctx.oTarget.Size)
          ctx.attackPosition = Config.AttackPosition.High;
        else
          ctx.attackPosition = Config.AttackPosition.Low;
      }

      next();
    }
    private static void ProcessArmorSlotHit(Context ctx, Action next)
    {
      InventorySlot hitSlot;
      int randLocation = NwRandom.Roll(Utils.random, 100);

      switch (ctx.attackPosition)
      {
        case Config.AttackPosition.NormalOrRanged:

          if (randLocation < 13)
            hitSlot = InventorySlot.Boots;
          else if (randLocation < 25)
            hitSlot = InventorySlot.Belt;
          else if (randLocation < 38)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 51)
            hitSlot = InventorySlot.Arms;
          else if (randLocation < 88)
            hitSlot = InventorySlot.Chest;
          else
            hitSlot = InventorySlot.Head;

          break;

        case Config.AttackPosition.Low:

          if (randLocation < 28)
            hitSlot = InventorySlot.Boots;
          else if (randLocation < 58)
            hitSlot = InventorySlot.Belt;
          else if (randLocation < 69)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 82)
            hitSlot = InventorySlot.Arms;
          else
            hitSlot = InventorySlot.Chest;

          break;

        case Config.AttackPosition.High:

          if (randLocation < 19)
            hitSlot = InventorySlot.Cloak;
          else if (randLocation < 33)
            hitSlot = InventorySlot.Arms;
          else if (randLocation < 74)
            hitSlot = InventorySlot.Chest;
          else
            hitSlot = InventorySlot.Head;

          break;

        default:
          return;
      }

      ctx.targetArmor = ctx.oTarget.GetItemInSlot(hitSlot);

      if (ctx.oAttacker != null && ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").HasValue)
        ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").Delete();

      // Dans le cas où la créature n'est pas un joueur, alors on simplifie et on prend directement sa CA de base pour la CA générique
      // TODO : ajouter une option dans l'éditeur de créature pour saisir de la CA spécifique ou de l'absorption, etc
      if (!ctx.oTarget.IsLoginPlayerCharacter) 
        ctx.targetAC[DamageType.BaseWeapon] = ctx.oTarget.AC - ctx.oTarget.GetAbilityModifier(Ability.Dexterity) - 10;

      //if (ctx.oTarget.Tag == "damage_trainer" && ctx.oAttacker.IsPlayerControlled)
        //ctx.oAttacker.ControllingPlayer.SendServerMessage($"Hit slot : {hitSlot.ToString().ColorString(ColorConstants.White)}", ColorConstants.Brown);

      next();
    }
    private static void ProcessTargetSpecificAC(Context ctx, Action next)
    {
      //Config.SetArmorValueFromArmorPiece(ctx);
      // TODO : Gérer les EFFECT qui ajoutent de l'armure (nécessite la refacto des sorts)

      /*if (ctx.targetArmor != null && ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
      {
        int baseArmorACValue = ctx.oTarget.GetItemInSlot(InventorySlot.Chest) != null ? ctx.oTarget.GetItemInSlot(InventorySlot.Chest).BaseACValue : -1;
        int armorProficiency = ctx.attackingPlayer is not null ? ctx.attackingPlayer.GetArmorProficiencyLevel(baseArmorACValue) / 10 : -1;
        
        foreach (var propType in ctx.targetArmor.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.AcBonus || i.Property.PropertyType == ItemPropertyType.DecreasedAc
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType.RowIndex == (int)ctx.oAttacker.Race.RacialType)
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType.RowIndex == (int)ctx.oAttacker.GoodEvilAlignment)
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType.RowIndex == (int)ctx.oAttacker.LawChaosAlignment)
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType.RowIndex == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oAttacker)))
          .GroupBy(i => i.Property.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();

          if (ctx.targetAC.ContainsKey(DamageType.BaseWeapon))
            ctx.targetAC[DamageType.BaseWeapon] += armorProficiency > -1 ? maxIP.IntParams[3] * armorProficiency : maxIP.IntParams[3];
          else
            ctx.targetAC.Add(DamageType.BaseWeapon, armorProficiency > -1 ? maxIP.IntParams[3] * armorProficiency : maxIP.IntParams[3]);

          //if (ctx.targetAC[DamageType.BaseWeapon] > ctx.maxBaseAC)
          //ctx.targetAC[DamageType.BaseWeapon] = ctx.maxBaseAC;
        }
        
        foreach (var propType in ctx.targetArmor.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.AcBonusVsDamageType)
        .GroupBy(i => i.SubType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          DamageType damageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.SubType.RowIndex);

          if (ctx.targetAC.ContainsKey(damageType))
            ctx.targetAC[damageType] += armorProficiency > -1 ? maxIP.IntParams[3] * armorProficiency : maxIP.IntParams[3];
          else
            ctx.targetAC.Add(damageType, armorProficiency > -1 ? maxIP.IntParams[3] * armorProficiency : maxIP.IntParams[3]);

          //if (ctx.targetAC[damageType] > ctx.maxBaseAC)
          //ctx.targetAC[damageType] = ctx.maxBaseAC;
        }
      }*/

      /*foreach (var effect in ctx.oTarget.ActiveEffects)
      {
        if(effect.EffectType == EffectType.AcIncrease)
          ctx.targetAC[DamageType.BaseWeapon] += effect.IntParams[1];
        else if (effect.EffectType == EffectType.AcDecrease)
          ctx.targetAC[DamageType.BaseWeapon] -= effect.IntParams[1];
      }*/
      
      next();
    }
    private static void ProcessTargetShieldAC(Context ctx, Action next)
    {
      //Config.SetArmorValueFromShield(ctx);
      //Config.GetArmorValueFromWeapon(ctx, ctx.oTarget.GetItemInSlot(InventorySlot.RightHand));
      //Config.GetArmorValueFromWeapon(ctx, ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand));
      /*NwItem targetShield = ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand);

      if (targetShield != null && targetShield.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1) // Même si l'objet n'est pas à proprement parler un bouclier, tout item dans la main gauche procure un bonus de protection global
      {
        int shieldProficiency = ctx.attackingPlayer is not null ? ctx.attackingPlayer.GetShieldProficiencyLevel(targetShield.BaseItem.ItemType) / 10 : -1;
        
        foreach (var propType in targetShield.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.AcBonus
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType.RowIndex == (int)ctx.oAttacker.Race.RacialType)
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType.RowIndex == (int)ctx.oAttacker.GoodEvilAlignment)
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType.RowIndex == (int)ctx.oAttacker.LawChaosAlignment)
         || (ctx.oAttacker != null && i.Property.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType.RowIndex == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oAttacker)))
          .GroupBy(i => i.Property.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          ctx.targetAC[DamageType.BaseWeapon] += shieldProficiency > -1 ? maxIP.IntParams[3] * shieldProficiency : maxIP.IntParams[3];
        }

        foreach (var propType in targetShield.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.AcBonusVsDamageType)
        .GroupBy(i => i.SubType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          DamageType damageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.SubType.RowIndex);
          
          if (ctx.targetAC.ContainsKey(damageType))
            ctx.targetAC[damageType] += shieldProficiency > -1 ? maxIP.IntParams[3] * shieldProficiency : maxIP.IntParams[3];
          else
            ctx.targetAC.Add(damageType, shieldProficiency > -1 ? maxIP.IntParams[3] * shieldProficiency : maxIP.IntParams[3]);
        }
      }*/

      next();
    }
    private static void ProcessArmorPenetrationCalculations(Context ctx, Action next)
    {
      int armorPenetration = ctx.baseArmorPenetration + ctx.bonusArmorPenetration;

      if (ctx.oTarget.Tag == "damage_trainer" && ctx.oAttacker.IsPlayerControlled)
      {
        ctx.oAttacker.ControllingPlayer.SendServerMessage($"Armure de base de la cible : {ctx.targetAC[DamageType.BaseWeapon].ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);
        if (armorPenetration > 0)
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Pénétration d'armure : {armorPenetration.ToString().ColorString(ColorConstants.White)}% ({ctx.baseArmorPenetration.ToString().ColorString(ColorConstants.White)} + {ctx.bonusArmorPenetration.ToString().ColorString(ColorConstants.White)})", ColorConstants.Orange);
        else
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Pénétration d'armure : 0%", ColorConstants.Orange);
      }

      ctx.targetAC[DamageType.BaseWeapon] = ctx.targetAC[DamageType.BaseWeapon] * (100 - armorPenetration) / 100;

      next();
    }
    private static void ProcessDamageFromWeaponInscriptions(Context ctx, Action next)
    {
      //Config.SetDamageValueFromWeapon(ctx);
      next();
    }
    private static void ProcessWeakness(Context ctx, Action next)
    {
      foreach (var eff in ctx.oAttacker.ActiveEffects)
        if (eff.Tag == "CUSTOM_CONDITION_WEAKNESS")
        {
          Config.SetContextDamage(ctx, DamageType.BaseWeapon, (int)Math.Round((double)Config.GetContextDamage(ctx, DamageType.BaseWeapon) * 0.33, MidpointRounding.ToEven));
          next();
          return;
        }

      next();
    }
    private static void ProcessDamageCalculations(Context ctx, Action next)
    {
      double targetAC = 0;

      if(ctx.attackWeapon is not null && ctx.attackWeapon.ItemProperties.Any(i => i.Property.PropertyType == ItemPropertyType.NoDamage))
      {
        foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
          Config.SetContextDamage(ctx, damageType, 0);

        next();
        return;
      }

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
      {
        if (Config.GetContextDamage(ctx, damageType) < 1)
          continue;

        targetAC = damageType switch
        {
          // Base weapon damage
          DamageType.BaseWeapon => ctx.isUnarmedAttack ? HandleReducedDamageFromWeaponDamageType(ctx, new List<DamageType>() { DamageType.Bludgeoning }) : HandleReducedDamageFromWeaponDamageType(ctx, ctx.attackWeapon.BaseItem.WeaponType),
          
          // Les dégâts électiques ont 25 % de pénétration d'armure supplémentaires
          DamageType.Electrical => ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)16384) + ctx.targetAC.GetValueOrDefault(damageType) - 25,

          /*ModuleSystem.Log.Info($"unarmed : {ctx.isUnarmedAttack}");
            if (!ctx.isUnarmedAttack)
              ModuleSystem.Log.Info($"weapon : {ctx.attackWeapon.Name}");*/
          // Physical bonus damage
          DamageType.Bludgeoning or DamageType.Piercing or DamageType.Slashing => ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + ctx.targetAC.GetValueOrDefault(damageType),
          // Elemental bonus damage
          _ => ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)16384) + ctx.targetAC.GetValueOrDefault(damageType),
        };

        if (targetAC < 0) targetAC = 0;

        double initialDamage = Config.GetContextDamage(ctx, damageType);
        //double skillModifier = damageType == DamageType.BaseWeapon && ctx.attackingPlayer is not null ? ctx.attackingPlayer.GetWeaponMasteryLevel(ctx.attackWeapon) : 0;
        //double modifiedDamage = initialDamage * Utils.GetDamageMultiplier(targetAC, skillModifier);

        if (ctx.oTarget.Tag == "damage_trainer" && ctx.oAttacker.IsPlayerControlled)
        {
          //ctx.oAttacker.ControllingPlayer.SendServerMessage($"Initial : {damageType.ToString().ColorString(ColorConstants.White)} - {initialDamage.ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);
          //ctx.oAttacker.ControllingPlayer.SendServerMessage($"Armure totale vs {damageType.ToString().ColorString(ColorConstants.White)} : {targetAC.ToString().ColorString(ColorConstants.White)} - Dégâts {string.Format("{0:0}", (int)modifiedDamage).ColorString(ColorConstants.White)}", ColorConstants.Orange);
          //ctx.oAttacker.ControllingPlayer.SendServerMessage($"Réduction : {string.Format("{0:0.000}", ((initialDamage - modifiedDamage) / modifiedDamage) * 100).ColorString(ColorConstants.White)}%", ColorConstants.Orange);
        }

        //modifiedDamage = Math.Round(modifiedDamage, MidpointRounding.ToEven);
        //Config.SetContextDamage(ctx, damageType, (int)modifiedDamage);
        //LogUtils.LogMessage($"Final : {damageType} - AC {targetAC} - Initial {initialDamage} - Final Damage {modifiedDamage}", LogUtils.LogType.Combat);
      }

      if(ctx.physicalReduction > 0)
      {
        if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > -1)
          Config.SetContextDamage(ctx, DamageType.BaseWeapon, Config.GetContextDamage(ctx, DamageType.BaseWeapon) - ctx.physicalReduction);
        else if (Config.GetContextDamage(ctx, DamageType.Piercing) > -1)
          Config.SetContextDamage(ctx, DamageType.Piercing, Config.GetContextDamage(ctx, DamageType.Piercing) - ctx.physicalReduction);
        else if (Config.GetContextDamage(ctx, DamageType.Slashing) > -1)
          Config.SetContextDamage(ctx, DamageType.Slashing, Config.GetContextDamage(ctx, DamageType.Slashing) - ctx.physicalReduction);
        else if (Config.GetContextDamage(ctx, DamageType.Bludgeoning) > -1)
          Config.SetContextDamage(ctx, DamageType.Bludgeoning, Config.GetContextDamage(ctx, DamageType.Bludgeoning) - ctx.physicalReduction);
      }

      next();
    }
    private static void ProcessSpecialAttack(Context ctx, Action next)
    {
      if (ctx.attackingPlayer is not null && ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_NEXT_ATTACK").HasValue)
      {
        int skillId = ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_NEXT_ATTACK").Value;
        NwFeat usedFeat = NwFeat.FromFeatId(skillId - 10000);

        switch (skillId)
        {
          case CustomSkill.SeverArtery:
            
            break;
        }

        ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_NEXT_ATTACK").Delete();

        foreach (NwPlayer player in NwModule.Instance.Players)
          if (player?.ControlledCreature?.Area == ctx.oAttacker.Area && player.ControlledCreature.IsCreatureHeard(ctx.oAttacker))
            player.DisplayFloatingTextStringOnCreature(ctx.oAttacker, StringUtils.ToWhitecolor($"{ctx.oAttacker.Name.ColorString(ColorConstants.Cyan)} utilise {usedFeat.Name.ToString().ColorString(ColorConstants.Red)}"));

        ctx.oAttacker.DecrementRemainingFeatUses(usedFeat);
        ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{usedFeat.Id}").Value = 0;
        StringUtils.UpdateQuickbarPostring(ctx.attackingPlayer, usedFeat.Id, 0);

        foreach (var feat in ctx.oAttacker.Feats)
        {
          if (feat.MaxLevel > 0 && feat.MaxLevel < 255 && ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value > 0)
          {
            ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value -= 25;

            if (ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value < 0)
              ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value = 0;
            ctx.oAttacker.DecrementRemainingFeatUses(feat, 0);
            StringUtils.UpdateQuickbarPostring(ctx.attackingPlayer, feat.Id, ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value / 25);
          }
        }
      }

      next();
    }
    private static void ProcessAdrenaline(Context ctx, Action next)
    {
      if(ctx.attackingPlayer is not null && Config.GetContextDamage(ctx, DamageType.BaseWeapon) > -1)
      {
        foreach(var feat in ctx.oAttacker.Feats)
        {
          if (feat.MaxLevel > 0 && feat.MaxLevel < 255 && ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value < feat.MaxLevel)
          {
            ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value += 25 * ctx.adrenalineGainModifier > feat.MaxLevel ? feat.MaxLevel : 25 * ctx.adrenalineGainModifier;

            if (ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value >= feat.MaxLevel)
              ctx.oAttacker.IncrementRemainingFeatUses(feat);

            StringUtils.UpdateQuickbarPostring(ctx.attackingPlayer, feat.Id, ctx.oAttacker.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value / 25);
          }
        }

        ctx.oAttacker.GetObjectVariable<DateTimeLocalVariable>($"_LAST_DAMAGE_ON").Value = DateTime.Now;
      }

      if(ctx.targetPlayer is not null)
      {
        int totalDamage = 0;

        foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
        {
          int damage = Config.GetContextDamage(ctx, damageType);

          if (damage > 0)
            totalDamage += damage;
        }

        int adrenalineCharge = totalDamage / ctx.targetPlayer.endurance.maxHP * 100;

        if(adrenalineCharge > 0)
          foreach (var feat in ctx.oTarget.Feats)
          {
            if (feat.MaxLevel > 0 && feat.MaxLevel < 255 && ctx.oTarget.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value < feat.MaxLevel)
            {
              ctx.oTarget.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value += adrenalineCharge;

              if (ctx.oTarget.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value >= feat.MaxLevel)
                ctx.oTarget.IncrementRemainingFeatUses(feat);

              if(ctx.oTarget.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value / 25 > 0)
                StringUtils.UpdateQuickbarPostring(ctx.targetPlayer, feat.Id, ctx.oTarget.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Value / 25);
            }
          }

        ctx.oTarget.GetObjectVariable<DateTimeLocalVariable>($"_LAST_DAMAGE_ON").Value = DateTime.Now;
      }

      next();
    }
    /*private static void ProcessDoubleStrike(Context ctx, Action next)
    {
      NwItem leftSlot = ctx.oAttacker.GetItemInSlot(InventorySlot.LeftHand);

      if (((leftSlot is not null && ItemUtils.GetItemCategory(leftSlot.BaseItem.ItemType) != ItemUtils.ItemCategory.Shield ) || ctx.isUnarmedAttack) 
        && ctx.attackWeapon is not null && ctx.attackingPlayer is not null) 
      {
        double doubleStrikeChance = ctx.attackingPlayer.GetWeaponMasteryLevel(ctx.attackWeapon);
        int randomChance = Utils.random.Next(100);

        LogUtils.LogMessage($"Double Strike Chance : {doubleStrikeChance} vs {randomChance}", LogUtils.LogType.Combat);

        if (randomChance < doubleStrikeChance)
        {
          StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, "Frappe double", ColorConstants.Orange);
          LogUtils.LogMessage("Frappe double confirmée !", LogUtils.LogType.Combat);

          int damage = 0;
          int minDamage = ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("_MIN_WEAPON_DAMAGE").Value;
          int maxDamage = ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("_MAX_WEAPON_DAMAGE").Value;

          if (minDamage < 1)
            minDamage = 1;

          if (maxDamage < 1)
            maxDamage = 3;

          damage += ctx.onAttack.AttackResult == AttackResult.CriticalHit ? maxDamage : Utils.random.Next(minDamage, maxDamage + 1);

          foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
          {
            if (damageType == DamageType.BaseWeapon)
            {
              double targetAC = ctx.isUnarmedAttack ? HandleReducedDamageFromWeaponDamageType(ctx, new List<DamageType>() { DamageType.Bludgeoning }) : HandleReducedDamageFromWeaponDamageType(ctx, ctx.attackWeapon.BaseItem.WeaponType);
              double modifiedDamage = damage * Utils.GetDamageMultiplier(targetAC, ctx.attackingPlayer.GetWeaponMasteryLevel(ctx.attackWeapon));
              damage = (int)Math.Round(modifiedDamage, MidpointRounding.ToEven);

              if (damage < 1)
                damage = 1;
            }
            else
              damage = Config.GetContextDamage(ctx, damageType);

            ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, damageType));
          }

          ProcessAttackerItemDurability(ctx, next);
          ProcessTargetItemDurability(ctx, next);
        }
      }

      next();
    }*/
    private static void ProcessAttackerItemDurability(Context ctx, Action next)
    {
      if (ctx.attackWeapon is not null &&  ctx.attackingPlayer is not null)
      {
        if (ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
        {
          // L'attaquant est un joueur. On diminue la durabilité de son arme

          int dexBonus = ctx.oAttacker.GetAbilityModifier(Ability.Dexterity) - (ctx.oAttacker.ArmorCheckPenalty + ctx.oAttacker.ShieldCheckPenalty);
          if (dexBonus < 0)
            dexBonus = 0;

          // TODO : Plutôt faire dépendre ça de la maîtrise de l'arme
          int safetyLevel = ctx.attackingPlayer.learnableSkills.ContainsKey(CustomSkill.CombattantPrecautionneux) ? ctx.attackingPlayer.learnableSkills[CustomSkill.CombattantPrecautionneux].totalPoints : 0;
          int durabilityRate = 30 - (dexBonus - safetyLevel);
          if (durabilityRate < 1)
            durabilityRate = 1;

          int durabilityRoll = Utils.random.Next(500);

          LogUtils.LogMessage($"Jet - {ctx.attackingPlayer.oid.LoginCreature.Name} - {ctx.attackWeapon.Name} - 30 (base) - {dexBonus} (DEX) - {safetyLevel} (Compétence) = {durabilityRate} VS {durabilityRoll}", LogUtils.LogType.Durability);

          if (durabilityRoll < durabilityRate)
            DecreaseItemDurability(ctx.attackWeapon, ctx.attackingPlayer.oid, GetWeaponDurabilityLoss(ctx));
        }
        else
          ctx.attackingPlayer.oid.SendServerMessage($"Votre {StringUtils.ToWhitecolor(ctx.attackWeapon.Name)} n'est plus en état !", ColorConstants.Red);
      }

      next();
    }
    private static void ProcessTargetItemDurability(Context ctx, Action next)
    {
      if (ctx.targetPlayer is null)
        return;

      // La cible de l'attaque est un joueur, on fait diminuer la durabilité

      int dexBonus = ctx.oTarget.GetAbilityModifier(Ability.Dexterity) - (ctx.oTarget.ArmorCheckPenalty + ctx.oTarget.ShieldCheckPenalty);
      if (dexBonus < 0)
        dexBonus = 0;

      // TODO : Plutôt faire dépendre ça de la maîtrise de l'arme, de l'armure ou du bouclier
      int safetyLevel = ctx.targetPlayer.learnableSkills.ContainsKey(CustomSkill.CombattantPrecautionneux) ? ctx.targetPlayer.learnableSkills[CustomSkill.CombattantPrecautionneux].totalPoints : 0;

      int durabilityRate = 30 - dexBonus - safetyLevel;
      if (durabilityRate < 1)
        durabilityRate = 1;

      int durabilityRoll = Utils.random.Next(500);

      LogUtils.LogMessage($"Jet - {ctx.targetPlayer.oid.LoginCreature.Name} - Armure - 30 (base) - {dexBonus} (DEX) - {safetyLevel} (Compétence) = {durabilityRate} VS {durabilityRoll}", LogUtils.LogType.Durability);

      if (durabilityRoll < durabilityRate)
      {
        if (ctx.targetArmor is not null)
        {
          if(ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
          {
            LogUtils.LogMessage($"Armure : {ctx.targetArmor.Name}", LogUtils.LogType.Durability);
            DecreaseItemDurability(ctx.targetArmor, ctx.targetPlayer.oid, GetArmorDurabilityLoss(ctx));
          }
          else
            ctx.targetPlayer.oid.SendServerMessage($"Votre {StringUtils.ToWhitecolor(ctx.targetArmor.Name)} n'est plus en état !", ColorConstants.Red);
        }
        else
          LogUtils.LogMessage("Pas d'armure équipée sur l'emplacement touché", LogUtils.LogType.Durability);

        NwItem leftSlot = ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand);

        if (leftSlot is not null)
        {
          if (leftSlot.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
          {
            LogUtils.LogMessage($"Bouclier / Parade : {leftSlot.Name}", LogUtils.LogType.Durability);
            DecreaseItemDurability(leftSlot, ctx.targetPlayer.oid, GetShieldDurabilityLoss(ctx));
          }
          else
            ctx.targetPlayer.oid.SendServerMessage($"Votre {StringUtils.ToWhitecolor(leftSlot.Name)} n'est plus en état !", ColorConstants.Red);
        }

        List<NwItem> slots = new();

        if (ctx.oTarget.GetItemInSlot(InventorySlot.Belt) != null)
          slots.Add(ctx.oTarget.GetItemInSlot(InventorySlot.Belt));

        if (ctx.oTarget.GetItemInSlot(InventorySlot.LeftRing) != null)
          slots.Add(ctx.oTarget.GetItemInSlot(InventorySlot.LeftRing));

        if (ctx.oTarget.GetItemInSlot(InventorySlot.RightRing) != null)
          slots.Add(ctx.oTarget.GetItemInSlot(InventorySlot.RightRing));

        if (ctx.oTarget.GetItemInSlot(InventorySlot.Neck) != null)
          slots.Add(ctx.oTarget.GetItemInSlot(InventorySlot.Neck));

        if (slots.Count > 0)
        {
          int random = NwRandom.Roll(Utils.random, slots.Count) - 1;
          NwItem randomItem = slots.ElementAt(random);

          if (randomItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
          {
            LogUtils.LogMessage($"Equipement aléatoire : {randomItem.Name}", LogUtils.LogType.Durability);
            DecreaseItemDurability(randomItem, ctx.targetPlayer.oid, GetItemDurabilityLoss(ctx));
          }
          else
            ctx.targetPlayer.oid.SendServerMessage($"Votre {StringUtils.ToWhitecolor(randomItem.Name)} n'est plus en état !", ColorConstants.Red);
        }
      }
    }
    private static int GetArmorDurabilityLoss(Context ctx)
    {
      int durabilityLoss = 1;

      if(ctx.attackWeapon is not null)
      {
        if (ctx.oTarget.GetItemInSlot(InventorySlot.Chest)?.BaseACValue < 6 && ctx.attackWeapon.BaseItem.WeaponType.Contains(DamageType.Slashing))
          durabilityLoss++;

        if (ctx.oTarget.GetItemInSlot(InventorySlot.Chest)?.BaseACValue > 5 && ctx.attackWeapon.BaseItem.WeaponType.Contains(DamageType.Bludgeoning))
          durabilityLoss++;

        if (Config.GetContextDamage(ctx, DamageType.Acid) > 0)
          durabilityLoss++;
      }

      return durabilityLoss;
    }
    private static int GetShieldDurabilityLoss(Context ctx)
    {
      int durabilityLoss = 1;

      if (ctx.attackWeapon is not null && ctx.attackWeapon.BaseItem.WeaponType.Contains(DamageType.Bludgeoning))
        durabilityLoss++;

      if (Config.GetContextDamage(ctx, DamageType.Acid) > 0)
        durabilityLoss++;

      return durabilityLoss;
    }
    private static int GetWeaponDurabilityLoss(Context ctx)
    {
      int durabilityLoss = 1;

      if ((ctx.oTarget.GetItemInSlot(InventorySlot.Chest)?.BaseACValue > 5 
        || (ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand) is not null && ItemUtils.GetItemCategory(ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand).BaseItem.ItemType) == ItemUtils.ItemCategory.Shield))
        && ctx.attackWeapon.BaseItem.WeaponType.Count() < 2 && ctx.attackWeapon.BaseItem.WeaponType.Contains(DamageType.Slashing))
        durabilityLoss++;

      if (Config.GetContextDamage(ctx, DamageType.Acid) > 0)
        durabilityLoss++;

      return durabilityLoss;
    }
    private static int GetItemDurabilityLoss(Context ctx)
    {
      int durabilityLoss = 1;

      if (Config.GetContextDamage(ctx, DamageType.Acid) > 0)
        durabilityLoss++;

      return durabilityLoss;
    }
    private static async void DecreaseItemDurability(NwItem item, NwPlayer oPC, int durabilityLoss)
    {
      item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value -= durabilityLoss;
      LogUtils.LogMessage($"Durabilité perdue : {durabilityLoss} - Reste : {item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value}", LogUtils.LogType.Durability);

      if (item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 1)
      {
        if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasNothing)
        {
          item.Destroy();
          oPC.SendServerMessage($"Il ne reste plus que des ruines de votre {item.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);

          if (oPC.LoginCreature.GetSlotFromItem(item) == EquipmentSlots.Chest)
          {
            NwItem rags = await NwItem.Create("rags", oPC.LoginCreature);
            oPC.LoginCreature.RunEquip(rags, EquipmentSlots.Chest);
          }

          LogUtils.LogMessage("Objet de qualité non artisanale détruit.", LogUtils.LogType.Durability);
        }
        else
          HandleItemRuined(oPC.ControlledCreature, item);
      }
    }

    private static void HandleAbsorbedDamageFromWeaponDamageType(Context ctx, DamageType damageType, List<ItemProperty> absorbIP, ItemProperty maxIP)
    {
      if (ctx.attackWeapon == null || ctx.isUnarmedAttack || (ctx.attackWeapon.BaseItem.WeaponType.Count() == 1 && ctx.attackWeapon.BaseItem.WeaponType.Any(d => d == damageType)))
        HandleDamageAbsorbed(ctx, damageType, maxIP.IntParams[3]);
      else if (ctx.attackWeapon.BaseItem.WeaponType.Count() > 1 && ctx.attackWeapon.BaseItem.WeaponType.Any(d => d == damageType))
      {
        DamageType subAbsorbedDamageType = ctx.attackWeapon.BaseItem.WeaponType.FirstOrDefault(d => d != damageType);
        IPDamageType ipDamageTypeToFind = IPDamageType.Piercing;

        switch(subAbsorbedDamageType)
        {
          case DamageType.Bludgeoning:
            ipDamageTypeToFind = IPDamageType.Bludgeoning;
            break;
          case DamageType.Slashing:
            ipDamageTypeToFind = IPDamageType.Slashing;
            break;
        }

        ItemProperty bonusAbsorbIP = absorbIP.Where(i => (IPDamageType)i.SubType.RowIndex == ipDamageTypeToFind).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

        if (bonusAbsorbIP != null && bonusAbsorbIP.IntParams[3] < maxIP.IntParams[3])
          HandleDamageAbsorbed(ctx, subAbsorbedDamageType, bonusAbsorbIP.IntParams[3], damageType);
        else
          HandleDamageAbsorbed(ctx, damageType, maxIP.IntParams[3]);
      }
    }
    private static double HandleReducedDamageFromWeaponDamageType(Context ctx, IEnumerable<DamageType> damageType)
    {
      if (damageType.Count() == 1)
        return ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + ctx.targetAC.GetValueOrDefault(damageType.FirstOrDefault());
      else
      {
        return ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)8192) +
          ctx.targetAC.GetValueOrDefault(damageType.ElementAt(0)) > ctx.targetAC.GetValueOrDefault(damageType.ElementAt(1)) ? ctx.targetAC.GetValueOrDefault(damageType.ElementAt(1)) : ctx.targetAC.GetValueOrDefault(damageType.ElementAt(0)); ;
      }
    }
  }
}
