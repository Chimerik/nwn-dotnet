using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Events;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Action = System.Action;
using NWN.Core;
using ItemProperty = NWN.API.ItemProperty;
using Effect = NWN.API.Effect;
using Context = NWN.Systems.Config.Context;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
            IsAttackDodged,
            ProcessBaseDamageTypeAndAttackWeapon,
            ProcessAutomaticHit,
            ProcessTargetDamageAbsorption,
            ProcessBaseArmorPenetration,
            ProcessBonusArmorPenetration,
            ProcessAttackPosition,
            ProcessArmorSlotHit,
            ProcessTargetSpecificAC,
            ProcessTargetShieldAC,
            ProcessArmorPenetrationCalculations,
            ProcessDamageCalculations,
            ProcessAttackerItemDurability,
            ProcessTargetItemDurability,
      }
    );
    public static void HandleAttackEvent(OnCreatureAttack onAttack)
    {
      PlayerSystem.Log.Info("Entering Attack Event");
      PlayerSystem.Log.Info("Attacker : " + onAttack.Attacker.Name);
      PlayerSystem.Log.Info("Target : " + onAttack.Target.Name);
      PlayerSystem.Log.Info("Result : " + onAttack.AttackResult);
      PlayerSystem.Log.Info("Base : " + onAttack.DamageData.Base);
      PlayerSystem.Log.Info("attackNumber : " + onAttack.AttackNumber);
      PlayerSystem.Log.Info("attack type : " + onAttack.WeaponAttackType);

      if (!(onAttack.Target is NwCreature oTarget))
        return;

      pipeline.Execute(new Context(
        onAttack: onAttack,
        oTarget: oTarget
      ));
    }
    private static void HandleItemRuined(NwCreature oPC, NwItem oItem)
    {
      CreaturePlugin.RunUnequip(oPC, oItem);
      oItem.GetLocalVariable<int>("_DURABILITY").Value = -1;
      foreach (ItemProperty ip in oItem.ItemProperties.Where(ip => ip.Tag.StartsWith("ENCHANTEMENT")))
      {
        Task waitLoopEnd = NwTask.Run(async () =>
        {
          ItemProperty deactivatedIP = ip;
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          oItem.RemoveItemProperty(deactivatedIP);
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          deactivatedIP.Tag += "_INACTIVE";
          oItem.AddItemProperty(deactivatedIP, EffectDuration.Permanent);
        });
      }

      oPC.ControllingPlayer.SendServerMessage($"Il ne reste plus que des ruines de votre {oItem.Name.ColorString(ColorConstants.White)}. Des réparations s'imposent !", ColorConstants.Red);
    }

    private static void IsAttackDodged(Context ctx, Action next)
    {
      int skillBonusDodge = 0;

      if (PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player) && player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedDodge))
        skillBonusDodge += 2 * SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedDodge, player.learntCustomFeats[CustomFeats.ImprovedDodge]);

      if (ctx.oTarget.KnowsFeat(Feat.Dodge))
        skillBonusDodge += 2;

      int dodgeRoll = NwRandom.Roll(Utils.random, 100);

      if (dodgeRoll <= ctx.oTarget.GetAbilityModifier(Ability.Dexterity) + skillBonusDodge - ctx.oTarget.ArmorCheckPenalty - ctx.oTarget.ShieldCheckPenalty)
      {
        ctx.onAttack.AttackResult = AttackResult.Miss;
        if (ctx.oAttacker.IsPlayerControlled)
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"{ctx.oTarget.Name} a esquivé votre attaque.");

        if (ctx.oTarget.IsPlayerControlled)
          ctx.oTarget.ControllingPlayer.SendServerMessage($"Attaque de {ctx.oAttacker.Name} esquivée.");
      }
      else
        next();
    }
    private static void ProcessAutomaticHit(Context ctx, Action next)
    {
      if (ctx.onAttack.AttackResult == AttackResult.Miss)
      {
        ctx.onAttack.AttackResult = AttackResult.AutomaticHit;
        int strModifier = ctx.oAttacker.GetAbilityModifier(Ability.Strength);

        if (ctx.attackWeapon == null)
          ctx.onAttack.DamageData.Base = (short)(NwRandom.Roll(Utils.random, 3) + strModifier);
        else
        {
          NwItem damageSlot = null;

          switch (ItemUtils.GetItemCategory(ctx.attackWeapon.BaseItemType))
          {
            case ItemUtils.ItemCategory.RangedWeapon:

              switch (ctx.attackWeapon.BaseItemType)
              {
                case BaseItemType.LightCrossbow:
                case BaseItemType.HeavyCrossbow:
                  damageSlot = ctx.oAttacker.GetItemInSlot(InventorySlot.Bolts);
                  strModifier = 0;
                  break;
                case BaseItemType.Shortbow:
                case BaseItemType.Longbow:
                  damageSlot = ctx.oAttacker.GetItemInSlot(InventorySlot.Arrows);

                  ItemProperty mighty = ctx.attackWeapon.ItemProperties.Where(ip => ip.PropertyType == ItemPropertyType.Mighty).OrderByDescending(ip => ip.CostTableValue).FirstOrDefault();
                  if (mighty != null && strModifier > mighty.CostTableValue)
                    strModifier = mighty.CostTableValue;

                  break;
                case BaseItemType.Sling:
                  damageSlot = ctx.oAttacker.GetItemInSlot(InventorySlot.Bullets);
                  strModifier = 0;
                  break;
                case BaseItemType.Dart:
                  strModifier = 0;
                  break;
              }
              break;

            case ItemUtils.ItemCategory.TwoHandedMeleeWeapon:
              strModifier = (int)(strModifier * 1.5);
              damageSlot = ctx.attackWeapon;
              break;

            default:
              damageSlot = ctx.attackWeapon;
              break;
          }

          if (damageSlot != null)
          {
            foreach (var propType in damageSlot.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.DamageBonus
            || (i.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup && i.SubType == (int)ctx.oTarget.RacialType)
            || (i.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.GoodEvilAlignment)
            || (i.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.LawChaosAlignment)
            || (i.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget))
            || (i.PropertyType == ItemPropertyType.EnhancementBonus)
            || (i.PropertyType == ItemPropertyType.EnhancementBonusVsRacialGroup && i.SubType == (int)ctx.oTarget.RacialType)
            || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.GoodEvilAlignment)
            || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.LawChaosAlignment)
            || (i.PropertyType == ItemPropertyType.EnhancementBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget)))
              .GroupBy(i => i.PropertyType))
            {
              ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
              DamageType damageType = DamageType.Slashing;
              short rolledDamage = Config.RollDamage(maxIP.CostTableValue);
              short currentDamage = 0;

              switch (maxIP.Param1TableValue)
              {
                case -1: // Cas des dégâts simples
                  damageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.SubType);
                  break;
                default: // Case des dégâts spécifiques, le type de dégât se trouve dans Param1TableValue au lieu de SubType. Ouais, c'est chiant
                  damageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.Param1TableValue);
                  break;
              }

              currentDamage = ctx.onAttack.DamageData.GetDamageByType(damageType);

              if (rolledDamage > currentDamage)
                ctx.onAttack.DamageData.SetDamageByType(damageType, rolledDamage);
            }
          }

          if (int.TryParse(NWScript.Get2DAString("baseitems", "NumDice", (int)ctx.attackWeapon.BaseItemType), out int NumDice)
            && int.TryParse(NWScript.Get2DAString("baseitems", "DieToRoll", (int)ctx.attackWeapon.BaseItemType), out int DieToRoll))
            ctx.onAttack.DamageData.Base = (short)(NwRandom.Roll(Utils.random, DieToRoll, NumDice) + strModifier);
          else
            ctx.onAttack.DamageData.Base = 0;
        }
      }

      next();
    }
    private static void ProcessBaseDamageTypeAndAttackWeapon(Context ctx, Action next)
    {
      ctx.weaponBaseDamageType = 3; // Slashing par défaut

      if (ctx.isUnarmedAttack)
      {
        ctx.weaponBaseDamageType = 2;

        if (ctx.oAttacker.GetItemInSlot(InventorySlot.Arms) != null)
          ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.Arms);
      }

      switch (ctx.onAttack.WeaponAttackType)
      {
        case WeaponAttackType.MainHand:

          if (ctx.oAttacker.GetItemInSlot(InventorySlot.RightHand) != null)
          {
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.RightHand);
            ctx.weaponBaseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          }

          break;

        case WeaponAttackType.Offhand:

          if (ctx.oAttacker.GetItemInSlot(InventorySlot.LeftHand) != null)
          {
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.LeftHand);
            ctx.weaponBaseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          }

          break;

        case WeaponAttackType.CreatureBite:

          if (ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureBiteWeapon) != null)
          {
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureBiteWeapon);
            ctx.weaponBaseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          }
          break;

        case WeaponAttackType.CreatureLeft:

          if (ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureLeftWeapon) != null)
          {
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
            ctx.weaponBaseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          }
          break;

        case WeaponAttackType.CreatureRight:

          if (ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureRightWeapon) != null)
          {
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.CreatureRightWeapon);
            ctx.weaponBaseDamageType = ItemUtils.GetItemDamageType(ctx.attackWeapon);
          }
          break;
      }

      next();
    }
    private static void ProcessTargetDamageAbsorption(Context ctx, Action next)
    {
      if (ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin) != null)
      {
        List<ItemProperty> absorbIP = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin).ItemProperties.Where(i => i.PropertyType == ItemPropertyType.ImmunityDamageType && i.CostTableValue > 7).ToList();

        foreach (var propType in absorbIP.GroupBy(i => i.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          DamageType ipDamageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.SubType);

          switch ((IPDamageType)maxIP.SubType)
          {
            case IPDamageType.Bludgeoning:

              if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0 || Config.GetContextDamage(ctx, DamageType.Bludgeoning) > 0)
              {
                if (absorbIP.Any(i => (IPDamageType)i.SubType == IPDamageType.Physical && i.CostTableValue > maxIP.CostTableValue))
                  break;

                switch (ctx.weaponBaseDamageType)
                {
                  case 0: // Dégâts non causés par une arme, mais par un sort
                  case 2: // Weapon Type Bludgeoning
                    HandleDamageAbsorbed(ctx, DamageType.Bludgeoning, maxIP.CostTableValue);
                    break;

                  case 5: // Weapon type Bludgeoning/Piercing
                    ItemProperty bonusAbsorbIP = absorbIP.Where(i => (IPDamageType)i.SubType == IPDamageType.Piercing).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

                    if (bonusAbsorbIP != null && bonusAbsorbIP.CostTableValue < maxIP.CostTable)
                      HandleDamageAbsorbed(ctx, DamageType.Piercing, bonusAbsorbIP.CostTableValue, DamageType.Bludgeoning);
                    else
                      HandleDamageAbsorbed(ctx, DamageType.Bludgeoning, maxIP.CostTableValue);
                    break;
                }
              }
              break;

            case IPDamageType.Slashing:

              if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0 || Config.GetContextDamage(ctx, DamageType.Slashing) > 0)
              {
                if (absorbIP.Any(i => (IPDamageType)i.SubType == IPDamageType.Physical && i.CostTableValue > maxIP.CostTableValue))
                  break;

                switch (ctx.weaponBaseDamageType)
                {
                  case 0: // Dégâts non causés par une arme, mais par un sort
                  case 3: // Weapon Type Slashing
                    HandleDamageAbsorbed(ctx, DamageType.Slashing, maxIP.CostTableValue);
                    break;

                  case 4: // Weapon type Slashing/Piercing
                    ItemProperty bonusAbsorbIP = absorbIP.Where(i => (IPDamageType)i.SubType == IPDamageType.Piercing).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

                    if (bonusAbsorbIP != null && bonusAbsorbIP.CostTableValue < maxIP.CostTable)
                      HandleDamageAbsorbed(ctx, DamageType.Piercing, bonusAbsorbIP.CostTableValue, DamageType.Slashing);
                    else
                      HandleDamageAbsorbed(ctx, DamageType.Slashing, maxIP.CostTableValue);
                    break;
                }
              }
              break;

            case IPDamageType.Piercing:

              if (Config.GetContextDamage(ctx, DamageType.BaseWeapon) > 0 || Config.GetContextDamage(ctx, DamageType.Piercing) > 0)
              {
                if (absorbIP.Any(i => (IPDamageType)i.SubType == IPDamageType.Physical && i.CostTableValue > maxIP.CostTableValue))
                  break;

                ItemProperty bonusAbsorbIP;

                switch (ctx.weaponBaseDamageType)
                {
                  case 0: // Dégâts non causés par une arme, mais par un sort
                  case 1: // Weapon Type Slashing
                    HandleDamageAbsorbed(ctx, DamageType.Piercing, maxIP.CostTableValue);
                    break;

                  case 4: // Weapon type Slashing/Piercing
                    bonusAbsorbIP = absorbIP.Where(i => (IPDamageType)i.SubType == IPDamageType.Slashing).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

                    if (bonusAbsorbIP != null && bonusAbsorbIP.CostTableValue < maxIP.CostTable)
                      HandleDamageAbsorbed(ctx, DamageType.Slashing, bonusAbsorbIP.CostTableValue, DamageType.Piercing);
                    else
                      HandleDamageAbsorbed(ctx, DamageType.Piercing, maxIP.CostTableValue);
                    break;

                  case 5: // Weapon type Bludgeoning/Piercing
                    bonusAbsorbIP = absorbIP.Where(i => (IPDamageType)i.SubType == IPDamageType.Bludgeoning).OrderByDescending(i => i.CostTableValue).FirstOrDefault();

                    if (bonusAbsorbIP != null && bonusAbsorbIP.CostTableValue < maxIP.CostTable)
                      HandleDamageAbsorbed(ctx, DamageType.Bludgeoning, bonusAbsorbIP.CostTableValue, DamageType.Piercing);
                    else
                      HandleDamageAbsorbed(ctx, DamageType.Piercing, maxIP.CostTableValue);
                    break;
                }
              }
              break;

            case IPDamageType.Physical:

              if (int.TryParse(NWScript.Get2DAString("iprp_immuncost", "Value", maxIP.CostTableValue), out int absorptionValue))
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

                foreach (NwCreature oPC in ctx.oTarget.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(ctx.oTarget) < 35))
                  oPC.ControllingPlayer.DisplayFloatingTextStringOnCreature(ctx.oTarget, totalAbsorbedDamage.ToString().ColorString(new Color(32, 255, 32)));
              }
              break;

            case IPDamageType.Acid:
              if (Config.GetContextDamage(ctx, DamageType.Acid) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType == (IPDamageType)14 && i.CostTableValue > maxIP.CostTableValue))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Acid);
              break;

            case IPDamageType.Cold:
              if (Config.GetContextDamage(ctx, DamageType.Cold) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType == (IPDamageType)14 && i.CostTableValue > maxIP.CostTableValue))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Cold);
              break;

            case IPDamageType.Electrical:
              if (Config.GetContextDamage(ctx, DamageType.Electrical) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType == (IPDamageType)14 && i.CostTableValue > maxIP.CostTableValue))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Electrical);
              break;

            case IPDamageType.Fire:
              if (Config.GetContextDamage(ctx, DamageType.Fire) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType == (IPDamageType)14 && i.CostTableValue > maxIP.CostTableValue))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Fire);
              break;

            case IPDamageType.Magical:
              if (Config.GetContextDamage(ctx, DamageType.Magical) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType == (IPDamageType)14 && i.CostTableValue > maxIP.CostTableValue))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Magical);
              break;

            case IPDamageType.Sonic:
              if (Config.GetContextDamage(ctx, DamageType.Sonic) > 0 && !absorbIP.Any(i => (IPDamageType)i.SubType == (IPDamageType)14 && i.CostTableValue > maxIP.CostTableValue))
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Sonic);
              break;

            case IPDamageType.Negative:
              if (Config.GetContextDamage(ctx, DamageType.Negative) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Negative);
              break;

            case IPDamageType.Positive:
              if (Config.GetContextDamage(ctx, DamageType.Positive) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Positive);
              break;

            case IPDamageType.Divine:
              if (Config.GetContextDamage(ctx, DamageType.Divine) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Divine);
              break;

            case (IPDamageType)14: // Damage type élémentaire

              if (Config.GetContextDamage(ctx, DamageType.Acid) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Acid);

              if (Config.GetContextDamage(ctx, DamageType.Cold) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Cold);

              if (Config.GetContextDamage(ctx, DamageType.Electrical) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Electrical);

              if (Config.GetContextDamage(ctx, DamageType.Fire) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Fire);

              if (Config.GetContextDamage(ctx, DamageType.Magical) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Magical);

              if (Config.GetContextDamage(ctx, DamageType.Sonic) > 0)
                HandleDamageAbsorbed(ctx, DamageType.BaseWeapon, maxIP.CostTableValue, DamageType.Sonic);

              break;
          }
        }
      }

      next();
    }
    private static void HandleDamageAbsorbed(Context ctx, DamageType damageType, int ipCostValue, DamageType secondaryDamageType = DamageType.BaseWeapon)
    {
      if (int.TryParse(NWScript.Get2DAString("iprp_immuncost", "Value", ipCostValue), out int absorptionValue))
      {
        absorptionValue = 100 / (absorptionValue - 100);
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

        foreach (NwCreature oPC in ctx.oTarget.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(ctx.oTarget) < 35))
          oPC.ControllingPlayer.DisplayFloatingTextStringOnCreature(ctx.oTarget, totalAbsorbedDamage.ToString().ColorString(new Color(32, 255, 32)));
      }
    }
    private static void ProcessBaseArmorPenetration(Context ctx, Action next)
    {
      if (ctx.onAttack.WeaponAttackType == WeaponAttackType.Offhand)
        ctx.baseArmorPenetration = CreaturePlugin.GetAttackBonus(ctx.oAttacker, -1, 1);
      else
        ctx.baseArmorPenetration = CreaturePlugin.GetAttackBonus(ctx.oAttacker);

      if (ctx.attackWeapon != null && ctx.attackWeapon.BaseItemType == BaseItemType.Gloves) // la fonction CreaturePlugin.GetAttackBonus ne prend pas en compte le + AB des gants, donc je le rajoute
      {
        ItemProperty maxAttackBonus = ctx.attackWeapon.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AttackBonus).OrderByDescending(i => i.CostTableValue).FirstOrDefault();
        if (maxAttackBonus != null)
          ctx.baseArmorPenetration += maxAttackBonus.CostTableValue;
      }

      next();
    }
    private static void ProcessBonusArmorPenetration(Context ctx, Action next)
    {
      if (ctx.attackWeapon != null)
      {
        foreach (var propType in ctx.attackWeapon.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.EnhancementBonus
           || (i.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType == (int)ctx.oTarget.RacialType)
           || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.GoodEvilAlignment)
           || (i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.LawChaosAlignment)
           || (i.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget))
           || (i.PropertyType == ItemPropertyType.EnhancementBonusVsRacialGroup && i.SubType == (int)ctx.oTarget.RacialType)
           || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.GoodEvilAlignment)
           || (i.PropertyType == ItemPropertyType.EnhancementBonusVsAlignmentGroup && i.SubType == (int)ctx.oTarget.LawChaosAlignment)
           || (i.PropertyType == ItemPropertyType.EnhancementBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oTarget)))
          .GroupBy(i => i.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          ctx.baseArmorPenetration += maxIP.CostTableValue;
        }
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

      if (ctx.targetArmor == null && !ctx.oTarget.IsLoginPlayerCharacter) // Dans le cas où la créature n'a pas d'armure et n'est pas un joueur, alors on simplifie et on prend directement sa CA de base pour la CA générique et les propriétés de sa peau pour la CA spécifique
      {
        ctx.targetArmor = ctx.oTarget.GetItemInSlot(InventorySlot.CreatureSkin);
        ctx.targetAC[DamageType.BaseWeapon] = ctx.oTarget.AC;
      }
      else if (hitSlot != InventorySlot.Chest)
      {
        ItemProperty baseArmor = ctx.targetArmor.ItemProperties.Where(i
       => i.PropertyType == ItemPropertyType.AcBonus).OrderByDescending(i => i.CostTableValue).FirstOrDefault();
        {
          ctx.maxBaseAC += baseArmor.CostTableValue;
        }
      }

      next();
    }
    private static void ProcessTargetSpecificAC(Context ctx, Action next)
    {
      if (ctx.targetArmor != null)
      {
        foreach (var propType in ctx.targetArmor.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonus
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType == (int)ctx.oAttacker.RacialType)
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oAttacker.GoodEvilAlignment)
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oAttacker.LawChaosAlignment)
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oAttacker)))
          .GroupBy(i => i.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          ctx.targetAC[DamageType.BaseWeapon] += maxIP.CostTableValue;

          if (ctx.targetAC[DamageType.BaseWeapon] > ctx.maxBaseAC)
            ctx.targetAC[DamageType.BaseWeapon] = ctx.maxBaseAC;
        }

        foreach (var propType in ctx.targetArmor.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonusVsDamageType)
        .GroupBy(i => i.SubType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          DamageType damageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.SubType);
          ctx.targetAC.Add(damageType, maxIP.CostTableValue);

          if (ctx.targetAC[damageType] > ctx.maxBaseAC)
            ctx.targetAC[damageType] = ctx.maxBaseAC;
        }
      }

      next();
    }
    private static void ProcessTargetShieldAC(Context ctx, Action next)
    {
      NwItem targetShield = ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand);

      if (targetShield != null) // Même si l'objet n'est pas à proprement parler un bouclier, tout item dans la main gauche procure un bonus de protection global
      {
        foreach (var propType in targetShield.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonus
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup && i.SubType == (int)ctx.oAttacker.RacialType)
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oAttacker.GoodEvilAlignment)
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup && i.SubType == (int)ctx.oAttacker.LawChaosAlignment)
         || (ctx.oAttacker != null && i.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment && i.SubType == Config.GetIPSpecificAlignmentSubTypeAsInt(ctx.oAttacker)))
          .GroupBy(i => i.PropertyType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          ctx.targetAC[DamageType.BaseWeapon] += maxIP.CostTableValue;
        }

        foreach (var propType in targetShield.ItemProperties.Where(i => i.PropertyType == ItemPropertyType.AcBonusVsDamageType)
        .GroupBy(i => i.SubType))
        {
          ItemProperty maxIP = propType.OrderByDescending(i => i.CostTableValue).FirstOrDefault();
          DamageType damageType = ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)maxIP.SubType);
          
          if (ctx.targetAC.ContainsKey(damageType))
            ctx.targetAC[damageType] += maxIP.CostTableValue;
          else
            ctx.targetAC.Add(damageType, maxIP.CostTableValue);
        }
      }

      next();
    }
    private static void ProcessArmorPenetrationCalculations(Context ctx, Action next)
    {
      ctx.targetAC[DamageType.BaseWeapon] = ctx.targetAC[DamageType.BaseWeapon] * (100 - ctx.baseArmorPenetration - ctx.bonusArmorPenetration) / 100;
      next();
    }
    private static void ProcessDamageCalculations(Context ctx, Action next)
    {
      int targetAC = 0;

      foreach (DamageType damageType in (DamageType[]) Enum.GetValues(typeof(DamageType)))
      {
        if (ctx.onAttack.DamageData.GetDamageByType(damageType) < 1)
          continue;

        switch(damageType)
        {
          case DamageType.BaseWeapon: // Base weapon damage

            int bonusAC = 0;

            switch (ctx.weaponBaseDamageType)
            {
              case 1: // Piercing
                targetAC = ctx.targetAC[damageType] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + ctx.targetAC.GetValueOrDefault(DamageType.Piercing);
                break;
              case 2: // Bludgeoning
                targetAC = ctx.targetAC[damageType] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning);
                break;
              case 3: // Slashing
                targetAC = ctx.targetAC[damageType] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + ctx.targetAC.GetValueOrDefault(DamageType.Slashing);
                break;
              case 4: // Slashing and Piercing

                if (ctx.targetAC.GetValueOrDefault(DamageType.Slashing) > ctx.targetAC.GetValueOrDefault(DamageType.Piercing))
                  bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Piercing);
                else
                  bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Slashing);

                targetAC = ctx.targetAC[damageType] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + bonusAC;

                break;
              case 5: //Piercing and bludgeoning

                if (ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning) > ctx.targetAC.GetValueOrDefault(DamageType.Piercing))
                  bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Piercing);
                else
                  bonusAC = ctx.targetAC.GetValueOrDefault(DamageType.Bludgeoning);

                targetAC = ctx.targetAC[damageType] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + bonusAC;

                break;
            }

            break;

          case DamageType.Bludgeoning: // Physical bonus damage
          case DamageType.Piercing:
          case DamageType.Slashing:
            targetAC = ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)8192) + ctx.targetAC.GetValueOrDefault(damageType);
            break;

          default: // Elemental bonus damage
            targetAC = ctx.targetAC[DamageType.BaseWeapon] + ctx.targetAC.GetValueOrDefault((DamageType)16384) + ctx.targetAC.GetValueOrDefault(damageType);
            break;
        }

        if (targetAC < 0)
          targetAC = 0;

        //ctx.damageData.SetDamageByType(damageType, (short)(ctx.damageData.GetDamageByType(damageType) * Math.Pow(0.5, (targetAC - 60) / 40)));
        Config.SetContextDamage(ctx, damageType, (int)(Config.GetContextDamage(ctx, damageType) * Math.Pow(0.5, (targetAC - 60) / 40)));
        PlayerSystem.Log.Info($"{damageType} - AC {targetAC} - Damage {Config.GetContextDamage(ctx, damageType)}");
      }

      ctx.oTarget.GetLocalVariable<int>($"_DAMAGE_HANDLED_FROM_{ctx.oAttacker}").Value = 1;

      next();
    }
    private static void ProcessAttackerItemDurability(Context ctx, Action next)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oAttacker, out PlayerSystem.Player attacker) && ctx.attackWeapon != null)
      {
        // L'attaquant est un joueur. On diminue la durabilité de son arme

        int durabilityChance = 30;

        int dexBonus = ctx.oAttacker.GetAbilityModifier(Ability.Dexterity) - (ctx.oAttacker.ArmorCheckPenalty + ctx.oAttacker.ShieldCheckPenalty);
        if (dexBonus < 0)
          dexBonus = 0;

        int safetyLevel = 0;
        if (attacker.learntCustomFeats.ContainsKey(CustomFeats.CombattantPrecautionneux))
          safetyLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.CombattantPrecautionneux, attacker.learntCustomFeats[CustomFeats.CombattantPrecautionneux]);

        durabilityChance -= dexBonus + safetyLevel;

        if (NwRandom.Roll(Utils.random, 100, 1) < 2 && NwRandom.Roll(Utils.random, 100, 1) < durabilityChance)
        {
          ctx.attackWeapon.GetLocalVariable<int>("_DURABILITY").Value -= 1;
          if (ctx.attackWeapon.GetLocalVariable<int>("_DURABILITY").Value <= 0)
          {
            if (ctx.attackWeapon.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasNothing)
            {
              ctx.attackWeapon.Destroy();
              attacker.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {ctx.attackWeapon.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);
            }
            else
              HandleItemRuined(ctx.oAttacker, ctx.attackWeapon);
          }
        }
      }

      next();
    }
    private static void ProcessTargetItemDurability(Context ctx, Action next)
    {
      if (!PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player))
        return;

      PlayerSystem.Log.Info("Entered item durability");
      PlayerSystem.Log.Info($"player : {player.oid.PlayerName}");

      // La cible de l'attaque est un joueur, on fait diminuer la durabilité

      int dexBonus = ctx.oTarget.GetAbilityModifier(Ability.Dexterity) - (ctx.oTarget.ArmorCheckPenalty + ctx.oTarget.ShieldCheckPenalty);
      if (dexBonus < 0)
        dexBonus = 0;

      int safetyLevel = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.CombattantPrecautionneux))
        safetyLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.CombattantPrecautionneux, player.learntCustomFeats[CustomFeats.CombattantPrecautionneux]);

      int durabilityRate = 30 - dexBonus - safetyLevel;
      if (durabilityRate < 1)
        durabilityRate = 1;

      if (NwRandom.Roll(Utils.random, 100, 1) > 1 && NwRandom.Roll(Utils.random, 100, 1) > durabilityRate)
        return;

      int random = NwRandom.Roll(Utils.random, 11, 1) - 1;
      int loop = random + 1;
      NwItem item = ctx.oTarget.GetItemInSlot((InventorySlot)random);

      while (item == null && loop != random)
      {
        if (loop > 10)
          loop = 0;

        item = ctx.oTarget.GetItemInSlot((InventorySlot)loop);
        loop++;
      }

      if (item == null || item.Tag == "amulettorillink")
        return;

      item.GetLocalVariable<int>("_DURABILITY").Value -= 1;
      if (item.GetLocalVariable<int>("_DURABILITY").Value <= 0)
      {
        if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasNothing)
        {
          item.Destroy();
          player.oid.SendServerMessage($"Il ne reste plus que des ruines de votre {item.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);
        }
        else
          HandleItemRuined(ctx.oTarget, item);
      }
    }
  }
}
