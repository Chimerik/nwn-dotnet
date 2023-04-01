using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System.Linq;
using System;
using System.Collections.Generic;
using Action = System.Action;
using Context = NWN.Systems.Config.Context;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static Pipeline<Context> pipeline = new(
      new Action<Context, Action>[]
      {
        IsAttackOfOpportunity,
        IsAttackDodged,
        ProcessBaseDamageTypeAndAttackWeapon,
        ProcessCriticalHit,
        ProcessTargetDamageAbsorption,
        ProcessBaseArmorPenetration,
        ProcessBonusArmorPenetration,
        ProcessAttackPosition,
        ProcessArmorSlotHit,
        ProcessTargetSpecificAC,
        ProcessTargetShieldAC,
        ProcessArmorPenetrationCalculations,
        ProcessDamageCalculations,
        ProcessDoubleStrike,
        ProcessAttackerItemDurability,
        ProcessTargetItemDurability,
      }
    );
    public static void HandleAttackEvent(OnCreatureAttack onAttack)
    {
      LogUtils.LogMessage($"Attack Event - Attacker {onAttack.Attacker.Name} - Target {onAttack.Target.Name} - Result {onAttack.AttackResult}" +
        $" - Base damage {onAttack.DamageData.Base} - attack number {onAttack.AttackNumber} - attack type {onAttack.WeaponAttackType}", LogUtils.LogType.Combat);

      if (onAttack.Target is not NwCreature oTarget)
        return;
      
      pipeline.Execute(new Context(
        onAttack: onAttack,
        oTarget: oTarget
      ));
    }
    private static void HandleItemRuined(NwCreature oPC, NwItem oItem)
    {
      //oPC.RunUnequip(oItem);
      oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = -1;
      oPC.ControllingPlayer.SendServerMessage($"Il ne reste plus que des ruines de votre {oItem.Name.ColorString(ColorConstants.White)}. Des réparations s'imposent !", ColorConstants.Red);
      LogUtils.LogMessage("Objet de qualité artisanale ruiné : à réparer !", LogUtils.LogType.Durability);
    }
    private static void IsAttackOfOpportunity(Context ctx, Action next) // A réfléchir : faut-il supprimer totalement les attaques d'opportunité ?
    {
      if (ctx.onAttack.AttackType == 65002)
        StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, "Attaque d'opportunité !");

      next();
    }
    private static void IsAttackDodged(Context ctx, Action next)
    {
      if (ctx.onAttack.AttackResult == AttackResult.Miss)
        StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, "Attaque esquivée !");
        
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
        ctx.baseArmorPenetration += 20;
        ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComBloodCrtRed));
        StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, "Coup critique !");
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

                StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, totalAbsorbedDamage.ToString().ColorString(new Color(32, 255, 32)));
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

        StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, $"Absorbe {StringUtils.ToWhitecolor(totalAbsorbedDamage)}".ColorString(new Color(32, 255, 32)));
        LogUtils.LogMessage($"{ctx.oTarget.Name} absorbe {totalAbsorbedDamage} dégâts de type {secondaryDamageType}", LogUtils.LogType.Combat);
      }
    }
    private static void ProcessBaseArmorPenetration(Context ctx, Action next)
    {
      if (ctx.oAttacker.IsFlanking(ctx.oTarget))
        ctx.baseArmorPenetration += 5;

      if (ctx.attackWeapon is null)
      {
        next();
        return;
      }
      else if(ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 0 && ctx.oAttacker.IsLoginPlayerCharacter)
      {
        ctx.oAttacker.LoginPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(ctx.attackWeapon.Name)} est en ruines. Sans réparations, cette arme est inutile.", ColorConstants.Red);
        next();
        return;
      }

      if (ctx.isRangedAttack)
        ctx.baseArmorPenetration += ctx.oAttacker.GetAttackBonus();
      else if (ctx.onAttack.WeaponAttackType == WeaponAttackType.Offhand)
        ctx.baseArmorPenetration += ctx.oAttacker.GetAttackBonus(true, false, true);
      else
      {
        int versatileModifier = ItemUtils.IsVersatileWeapon(ctx.attackWeapon.BaseItem.ItemType) ? 2 : 1;

        ctx.baseArmorPenetration += ctx.oAttacker.GetAttackBonus(true);
        ctx.baseArmorPenetration += ctx.oAttacker.GetAbilityModifier(Ability.Strength) < ctx.oAttacker.GetAbilityModifier(Ability.Dexterity)
          && ctx.attackWeapon.BaseItem.WeaponFinesseMinimumCreatureSize >= ctx.oAttacker.Size
          ? ctx.oAttacker.GetAbilityModifier(Ability.Dexterity) * versatileModifier
          : ctx.oAttacker.GetAbilityModifier(Ability.Strength) * versatileModifier;
      }

      if (ctx.attackWeapon.BaseItem.ItemType == BaseItemType.Gloves || ctx.attackWeapon.BaseItem.ItemType == BaseItemType.Bracer) // la fonction GetAttackBonus ne prend pas en compte le + AB des gants, donc je le rajoute
      {
        ItemProperty maxAttackBonus = ctx.attackWeapon.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.AttackBonus).OrderByDescending(i => i.CostTableValue).FirstOrDefault();
        if (maxAttackBonus is not null)
          ctx.baseArmorPenetration += maxAttackBonus.IntParams[3];
      }

      // Les armes perforantes disposent de 20 % de pénétration supplémentaire contre les armures lourdes
      if (ctx.attackWeapon.BaseItem.WeaponType.Contains(DamageType.Piercing) && ctx.oTarget.GetItemInSlot(InventorySlot.Chest) is not null && ctx.oTarget.GetItemInSlot(InventorySlot.Chest).BaseACValue > 5)
        ctx.baseArmorPenetration += 20;

      next();
    }
    private static void ProcessBonusArmorPenetration(Context ctx, Action next)
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
    }
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

      if (ctx.targetArmor == null && !ctx.oTarget.IsLoginPlayerCharacter) // Dans le cas où la créature n'a pas d'armure et n'est pas un joueur, alors on simplifie et on prend directement sa CA de base pour la CA générique et les propriétés de sa peau pour la CA spécifique
      {
        ctx.targetArmor = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin);
        ctx.targetAC[DamageType.BaseWeapon] = ctx.oTarget.AC - ctx.oTarget.GetAbilityModifier(Ability.Dexterity) - 10;
      }
      else if (hitSlot != InventorySlot.Chest)
      {
        if(ctx.targetArmor != null && ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value < 0) 
        {
          ctx.oTarget.LoginPlayer.SendServerMessage($"{StringUtils.ToWhitecolor(ctx.targetArmor.Name)} est en ruines. Sans réparations, cette pièce d'armure ne vous apporte aucune protection.", ColorConstants.Red);
          ctx.targetAC[DamageType.BaseWeapon] = 0;
          next();
          return;
        }

        NwItem baseArmor = ctx.oTarget.GetItemInSlot(InventorySlot.Chest);

        if (baseArmor != null)
        {
          /*if(baseArmor.ItemProperties.Any(i => i.PropertyType == ItemPropertyType.AcBonus))
          ctx.maxBaseAC = baseArmor.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonus)
            .OrderByDescending(i => i.CostTableValue).FirstOrDefault().CostTableValue;*/

          ctx.targetAC[DamageType.BaseWeapon] = PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player) ? baseArmor.BaseACValue * 3 * player.GetArmorProficiencyLevel(baseArmor.BaseACValue) / 10 : baseArmor.BaseACValue * 3;

          switch (baseArmor.BaseACValue)
          {
            case 1:
            case 2:
            case 3:
            case 4:
              ctx.targetAC.Add((DamageType)16384, baseArmor.BaseACValue * 5);
              break;
            case 5:
              ctx.targetAC.Add((DamageType)16384, 30);
              ctx.targetAC.Add((DamageType)8192, 5);
              break;
            case 6:
              ctx.targetAC.Add((DamageType)8192, 10);
              break;
            case 7:
              ctx.targetAC.Add((DamageType)8192, 15);
              break;
            case 8:
              ctx.targetAC.Add((DamageType)8192, 20);
              break;
          }
        }
      }

      if (ctx.oTarget.Tag == "damage_trainer" && ctx.oAttacker.IsPlayerControlled)
        ctx.oAttacker.ControllingPlayer.SendServerMessage($"Hit slot : {hitSlot.ToString().ColorString(ColorConstants.White)}", ColorConstants.Brown);

      next();
    }
    private static void ProcessTargetSpecificAC(Context ctx, Action next)
    {
      if (ctx.targetArmor != null && ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
      {
        int baseArmorACValue = ctx.oTarget.GetItemInSlot(InventorySlot.Chest) != null ? ctx.oTarget.GetItemInSlot(InventorySlot.Chest).BaseACValue : -1;
        int armorProficiency = PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player) ? player.GetArmorProficiencyLevel(baseArmorACValue) / 10 : -1;
        
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
      }

      foreach (var effect in ctx.oTarget.ActiveEffects)
      {
        if(effect.EffectType == EffectType.AcIncrease)
          ctx.targetAC[DamageType.BaseWeapon] += effect.IntParams[1];
        else if (effect.EffectType == EffectType.AcDecrease)
          ctx.targetAC[DamageType.BaseWeapon] -= effect.IntParams[1];
      }
      
      next();
    }
    private static void ProcessTargetShieldAC(Context ctx, Action next)
    {
      NwItem targetShield = ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand);

      if (targetShield != null && targetShield.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1) // Même si l'objet n'est pas à proprement parler un bouclier, tout item dans la main gauche procure un bonus de protection global
      {
        int shieldProficiency = PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player) ? player.GetShieldProficiencyLevel(targetShield.BaseItem.ItemType) / 10 : -1;
        
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
      }

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
        double modifiedDamage = initialDamage * Utils.GetDamageMultiplier(targetAC);
        double skillDamage = -1;
        double skillModifier = -1;

        if (damageType == DamageType.BaseWeapon && PlayerSystem.Players.TryGetValue(ctx.oAttacker, out PlayerSystem.Player attackerPlayer))
        {
          skillModifier = ctx.attackWeapon is not null ? attackerPlayer.GetWeaponMasteryLevel(ctx.attackWeapon.BaseItem.ItemType) : 0;
          skillDamage = modifiedDamage * skillModifier;

          if (skillDamage < 1)
            skillDamage = 1;
        }

        if (ctx.oTarget.Tag == "damage_trainer" && ctx.oAttacker.IsPlayerControlled)
        {
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Initial : {damageType.ToString().ColorString(ColorConstants.White)} - {initialDamage.ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Armure totale vs {damageType.ToString().ColorString(ColorConstants.White)} : {targetAC.ToString().ColorString(ColorConstants.White)} - Dégâts {string.Format("{0:0}", (int)modifiedDamage).ColorString(ColorConstants.White)}", ColorConstants.Orange);
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Réduction : {string.Format("{0:0.000}", ((initialDamage - modifiedDamage) / modifiedDamage) * 100).ColorString(ColorConstants.White)}%", ColorConstants.Orange);

          if (skillDamage > -1)
            ctx.oAttacker.ControllingPlayer.SendServerMessage($"Compétence d'arme : {Math.Round(modifiedDamage, 2).ToString().ColorString(ColorConstants.White)} * {(skillModifier * 100).ToString().ColorString(ColorConstants.White)}% = {Math.Round(skillDamage, 2).ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);
        }

        if (skillDamage < 0)
          skillDamage = modifiedDamage;
          
        Config.SetContextDamage(ctx, damageType, (int)Math.Round(skillDamage, MidpointRounding.ToEven));
        LogUtils.LogMessage($"Final : {damageType} - AC {targetAC} - Initial {initialDamage} - Final Damage {skillDamage}", LogUtils.LogType.Combat);
      }

      /*if (ctx.oTarget.IsLoginPlayerCharacter) // TODO : a supprimer quand les potions de core seront en place
        ctx.oTarget.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(500), TimeSpan.FromSeconds(10));*/


    /*if(ctx.onAttack != null)
      ctx.oTarget.GetObjectVariable<LocalVariableInt>($"_DAMAGE_HANDLED_FROM_{ctx.oAttacker}").Value = 1;*/

    /*if (ctx.oTarget.Tag == "damage_trainer")
    {
      Task HealAfterDamage = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
        ctx.oTarget.HP = ctx.oTarget.MaxHP;
      });
    }*/

    next();
    }
    private static void ProcessDoubleStrike(Context ctx, Action next)
    {
      NwItem leftSlot = ctx.oAttacker.GetItemInSlot(InventorySlot.LeftHand);

      if (((leftSlot is not null && ItemUtils.GetItemCategory(leftSlot.BaseItem.ItemType) != ItemUtils.ItemCategory.Shield ) || ctx.isUnarmedAttack) 
        && ctx.attackWeapon is not null
        && PlayerSystem.Players.TryGetValue(ctx.oAttacker, out PlayerSystem.Player attackerPlayer)) 
      {
        double doubleStrikeChance = attackerPlayer.GetWeaponDoubleStrikeChance(ctx.attackWeapon.BaseItem.ItemType);
        int randomChance = Utils.random.Next(100);

        LogUtils.LogMessage($"Double Strike Chance : {doubleStrikeChance} vs {randomChance}", LogUtils.LogType.Combat);

        if (randomChance < doubleStrikeChance)
        {
          StringUtils.DisplayStringToAllPlayersNearTarget(ctx.oTarget, "Frappe double !");
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
              double modifiedDamage = damage * Utils.GetDamageMultiplier(targetAC);
              double skillDamage = attackerPlayer.GetWeaponMasteryLevel(ctx.attackWeapon.BaseItem.ItemType);
              double skillModifier = attackerPlayer.GetWeaponMasteryLevel(ctx.attackWeapon.BaseItem.ItemType) * modifiedDamage;
              damage = (int)Math.Round(skillDamage, MidpointRounding.ToEven);

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
    }
    private static void ProcessAttackerItemDurability(Context ctx, Action next)
    {
      if (ctx.attackWeapon is not null && ctx.attackWeapon.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1 && PlayerSystem.Players.TryGetValue(ctx.oAttacker, out PlayerSystem.Player attacker))
      {
        // L'attaquant est un joueur. On diminue la durabilité de son arme

        int dexBonus = ctx.oAttacker.GetAbilityModifier(Ability.Dexterity) - (ctx.oAttacker.ArmorCheckPenalty + ctx.oAttacker.ShieldCheckPenalty);
        if (dexBonus < 0)
          dexBonus = 0;

        // TODO : Plutôt faire dépendre ça de la maîtrise de l'arme
        int safetyLevel = attacker.learnableSkills.ContainsKey(CustomSkill.CombattantPrecautionneux) ? attacker.learnableSkills[CustomSkill.CombattantPrecautionneux].totalPoints : 0;
        int durabilityRate = 30 - (dexBonus - safetyLevel);
        if (durabilityRate < 1)
          durabilityRate = 1;

        int durabilityRoll = Utils.random.Next(1000);

        LogUtils.LogMessage($"Jet de durabilité - {attacker.oid.LoginCreature.Name} - {ctx.attackWeapon.Name}", LogUtils.LogType.Durability);
        LogUtils.LogMessage($"30 (base) - {dexBonus} (DEX) - {safetyLevel} (Compétence) = {durabilityRate} VS {durabilityRoll}", LogUtils.LogType.Durability);

        if (durabilityRoll < durabilityRate)
          DecreaseItemDurability(ctx.attackWeapon, attacker.oid, GetWeaponDurabilityLoss(ctx));
      }

      next();
    }
    private static void ProcessTargetItemDurability(Context ctx, Action next)
    {
      if (!PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player))
        return;

      // La cible de l'attaque est un joueur, on fait diminuer la durabilité

      int dexBonus = ctx.oTarget.GetAbilityModifier(Ability.Dexterity) - (ctx.oTarget.ArmorCheckPenalty + ctx.oTarget.ShieldCheckPenalty);
      if (dexBonus < 0)
        dexBonus = 0;

      // TODO : Plutôt faire dépendre ça de la maîtrise de l'arme, de l'armure ou du bouclier
      int safetyLevel = player.learnableSkills.ContainsKey(CustomSkill.CombattantPrecautionneux) ? player.learnableSkills[CustomSkill.CombattantPrecautionneux].totalPoints : 0;

      int durabilityRate = 30 - dexBonus - safetyLevel;
      if (durabilityRate < 1)
        durabilityRate = 1;

      int durabilityRoll = Utils.random.Next(1000);

      LogUtils.LogMessage($"Jet de durabilité - {player.oid.LoginCreature.Name}", LogUtils.LogType.Durability);
      LogUtils.LogMessage($"30 (base) - {dexBonus} (DEX) - {safetyLevel} (Compétence) = {durabilityRate} VS {durabilityRoll}", LogUtils.LogType.Durability);

      if (durabilityRoll < durabilityRate)
      {
        if (ctx.targetArmor is not null && ctx.targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
        {
          LogUtils.LogMessage($"Armure : {ctx.targetArmor.Name}", LogUtils.LogType.Durability);
          DecreaseItemDurability(ctx.targetArmor, player.oid, GetArmorDurabilityLoss(ctx));
        }

        NwItem leftSlot = ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand);

        if (leftSlot is not null && leftSlot.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value > -1)
        {
          LogUtils.LogMessage($"Bouclier / Parade : {leftSlot.Name}", LogUtils.LogType.Durability);
          DecreaseItemDurability(leftSlot, player.oid, GetShieldDurabilityLoss(ctx));
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
            DecreaseItemDurability(randomItem, player.oid, GetItemDurabilityLoss(ctx));
          }
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

      if (item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value <= 0)
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
