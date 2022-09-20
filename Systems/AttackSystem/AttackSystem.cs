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
    public static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
            IsAttackDodged,
            ProcessBaseDamageTypeAndAttackWeapon,
            ProcessCriticalHit,
            ProcessBaseDamage,
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
      oPC.RunUnequip(oItem);
      oItem.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value = -1;
      oPC.ControllingPlayer.SendServerMessage($"Il ne reste plus que des ruines de votre {oItem.Name.ColorString(ColorConstants.White)}. Des réparations s'imposent !", ColorConstants.Red);
    }

    private static void IsAttackDodged(Context ctx, Action next)
    {
      if (ctx.onAttack.AttackResult == AttackResult.Miss)
      {
        ctx.onAttack.AttackResult = AttackResult.Miss;
        if (ctx.oAttacker.IsPlayerControlled)
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"{ctx.oTarget.Name} a esquivé votre attaque.");

        if (ctx.oTarget.IsPlayerControlled)
          ctx.oTarget.ControllingPlayer.SendServerMessage($"Attaque de {ctx.oAttacker.Name} esquivée.");
      }

      next();
    }
    /*private static void ProcessAdditionnalDamageEffect(Context ctx, Action next)
    {
      foreach (var effectType in ctx.oAttacker.ActiveEffects.Where(e => e.EffectType == EffectType.DamageIncrease).GroupBy(e => e.IntParams.ElementAt(1)))
      {
        Effect maxEffect = effectType.OrderByDescending(e => e.IntParams.ElementAt(0)).FirstOrDefault();
        DamageType damageType = (DamageType)maxEffect.IntParams.ElementAt(1);
        Config.SetContextDamage(ctx, damageType, Config.GetContextDamage(ctx, damageType) + maxEffect.IntParams.ElementAt(0));
      }
       
      next();
    }*/
    private static void ProcessBaseDamageTypeAndAttackWeapon(Context ctx, Action next)
    {
      if (ctx.isUnarmedAttack && ctx.oAttacker.GetItemInSlot(InventorySlot.Arms) != null)
          ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.Arms);

      switch (ctx.onAttack.WeaponAttackType)
      {
        case WeaponAttackType.MainHand:
        case WeaponAttackType.HastedAttack:
          if (ctx.oAttacker.GetItemInSlot(InventorySlot.RightHand) != null)
            ctx.attackWeapon = ctx.oAttacker.GetItemInSlot(InventorySlot.RightHand);
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
      if (IsHitCritical(ctx))
      {
        ctx.baseArmorPenetration += 20;

        if (ctx.attackWeapon != null && !ctx.isUnarmedAttack)
        {
          switch (ctx.attackWeapon.BaseItem.ItemType)
          {
            case BaseItemType.CreatureBludgeoningWeapon:
            case BaseItemType.CreaturePiercingWeapon:
            case BaseItemType.CreatureSlashingAndPiercingWeapon:
            case BaseItemType.CreatureSlashingWeapon:

              ItemProperty monsterDamage = ctx.attackWeapon.ItemProperties.FirstOrDefault(i => i.Property.PropertyType == ItemPropertyType.MonsterDamage);
              if (monsterDamage != null)
                ctx.onAttack.DamageData.Base = (short)(CreatureUtils.GetCriticalMonsterDamage(monsterDamage.IntParams[3]) + ctx.oAttacker.GetAbilityModifier(Ability.Strength));

              break;

            default:
              ctx.onAttack.DamageData.Base = (short)ItemUtils.GetMaxDamage(ctx.attackWeapon.BaseItem, ctx.oAttacker, ctx.isRangedAttack);
              break;
          }
        }
        else
          ctx.baseArmorPenetration += 20;

        ctx.oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComBloodCrtRed, false, 1.3f));
      }

      next();
    }
    private static bool IsHitCritical(Context ctx)
    {
      int critChance = 0;

      if (ctx.oTarget.FlatFooted)
        critChance += 10;

      if (!ctx.oAttacker.IsLoginPlayerCharacter)
      {
        if (ctx.oAttacker.ChallengeRating < 11)
          critChance += 5;
        else
          critChance += (int)ctx.oAttacker.ChallengeRating - 5;
      }
      else
      {
        PlayerSystem.Players.TryGetValue(ctx.oAttacker, out PlayerSystem.Player player);       
        critChance += ctx.attackWeapon == null ? player.GetWeaponCritScienceLevel(BaseItemType.Gloves) : player.GetWeaponCritScienceLevel(ctx.attackWeapon.BaseItem.ItemType);
        critChance += 5;
      }

      if (NwRandom.Roll(Utils.random, 100) < critChance)
        return true;
      else
        return false;
    }
    private static void ProcessBaseDamage(Context ctx, Action next)
    {
      if (!ctx.oAttacker.IsLoginPlayerCharacter) // si ce n'est pas un joueur, alors ses dégâts sont modifiés selon le facteur de puissance de la créature
      {
        if (ctx.oAttacker.ChallengeRating < 1)
          ctx.onAttack.DamageData.Base /= 10;
        else
          ctx.onAttack.DamageData.Base *= (short)(ctx.oAttacker.ChallengeRating / 10);
      }
      else if(PlayerSystem.Players.TryGetValue(ctx.oAttacker, out PlayerSystem.Player player)) // si c'est un joueur, alors ses dégâts sont modifiés selon sa maîtrise de l'arme actuelle
      {
        int weaponMasteryLevel = ctx.attackWeapon == null ? player.GetWeaponMasteryLevel(BaseItemType.Gloves) : player.GetWeaponMasteryLevel(ctx.attackWeapon.BaseItem.ItemType);
        ctx.onAttack.DamageData.Base *= (short)(weaponMasteryLevel / 10);
      }

      if (ctx.onAttack.DamageData.Base < 1)
        ctx.onAttack.DamageData.Base = 1;

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

                foreach (NwCreature oPC in ctx.oTarget.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(ctx.oTarget) < 35))
                  oPC.ControllingPlayer.DisplayFloatingTextStringOnCreature(ctx.oTarget, totalAbsorbedDamage.ToString().ColorString(new Color(32, 255, 32)));
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
      if (absorptionValue > 0)
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
        ctx.baseArmorPenetration += ctx.oAttacker.GetAttackBonus(true, false, true);
      else if(ctx.isRangedAttack)
        ctx.baseArmorPenetration += ctx.oAttacker.GetAttackBonus();
      else
        ctx.baseArmorPenetration += ctx.oAttacker.GetAttackBonus(true);

      if (ctx.attackWeapon != null && ctx.attackWeapon.BaseItem.ItemType == BaseItemType.Gloves) // la fonction GetAttackBonus ne prend pas en compte le + AB des gants, donc je le rajoute
      {
        ItemProperty maxAttackBonus = ctx.attackWeapon.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.AttackBonus).OrderByDescending(i => i.CostTableValue).FirstOrDefault();
        if (maxAttackBonus != null)
          ctx.baseArmorPenetration += maxAttackBonus.IntParams[3];
      }

      next();
    }
    private static void ProcessBonusArmorPenetration(Context ctx, Action next)
    {
      if (ctx.attackWeapon != null)
      {
        foreach (var propType in ctx.attackWeapon.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.EnhancementBonus
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
        }

        foreach (var effectType in ctx.oAttacker.ActiveEffects.Where(e => e.EffectType == EffectType.AttackIncrease).GroupBy(e => e.IntParams.ElementAt(1)))
        {
          Effect maxEffect = effectType.OrderByDescending(e => e.IntParams.ElementAt(0)).FirstOrDefault();
          ctx.baseArmorPenetration += maxEffect.IntParams.ElementAt(0);
        }
        
        foreach (var effectType in ctx.oAttacker.ActiveEffects.Where(e => e.EffectType == EffectType.AttackDecrease).GroupBy(e => e.IntParams.ElementAt(1)))
        {
          Effect maxEffect = effectType.OrderByDescending(e => e.IntParams.ElementAt(0)).FirstOrDefault();
          ctx.baseArmorPenetration -= maxEffect.IntParams.ElementAt(0);
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
        ctx.targetAC[DamageType.BaseWeapon] = ctx.oTarget.AC - ctx.oTarget.GetAbilityModifier(Ability.Dexterity) - 10;
      }
      else if (hitSlot != InventorySlot.Chest)
      {
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
      {
        ctx.oAttacker.ControllingPlayer.SendServerMessage($"Hit slot : {hitSlot.ToString().ColorString(ColorConstants.White)}", ColorConstants.Brown);
      }

      if (ctx.oAttacker != null && ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").HasValue)
        ctx.oAttacker.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").Delete();

      next();
    }
    private static void ProcessTargetSpecificAC(Context ctx, Action next)
    {
      if (ctx.targetArmor != null)
      {
        int baseArmorACValue = ctx.oTarget.GetItemInSlot(InventorySlot.Chest) != null ? ctx.oTarget.GetItemInSlot(InventorySlot.Chest).BaseACValue : -1;
        int armorProficiency = PlayerSystem.Players.TryGetValue(ctx.oTarget, out PlayerSystem.Player player) ? player.GetArmorProficiencyLevel(baseArmorACValue) / 10 : -1;

        foreach (var propType in ctx.targetArmor.ItemProperties.Where(i => i.Property.PropertyType == ItemPropertyType.AcBonus
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
      if (ctx.oAttacker != null)
      {
        foreach (var effectType in ctx.oAttacker.ActiveEffects.Where(e => e.EffectType == EffectType.AcIncrease).GroupBy(e => e.IntParams.ElementAt(1)))
        {
          Effect maxEffect = effectType.OrderByDescending(e => e.IntParams.ElementAt(0)).FirstOrDefault();
          ctx.targetAC[DamageType.BaseWeapon] += maxEffect.IntParams.ElementAt(0);
        }
      }

      foreach (var effectType in ctx.oTarget.ActiveEffects.Where(e => e.EffectType == EffectType.AcDecrease).GroupBy(e => e.IntParams.ElementAt(1)))
      {
        Effect maxEffect = effectType.OrderByDescending(e => e.IntParams.ElementAt(0)).FirstOrDefault();
        ctx.targetAC[DamageType.BaseWeapon] -= maxEffect.IntParams.ElementAt(0);
      }
      
      next();
    }
    private static void ProcessTargetShieldAC(Context ctx, Action next)
    {
      NwItem targetShield = ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand);

      if (targetShield != null) // Même si l'objet n'est pas à proprement parler un bouclier, tout item dans la main gauche procure un bonus de protection global
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

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
      {
        if (Config.GetContextDamage(ctx, damageType) < 1)
          continue;

        switch (damageType)
        {
          case DamageType.BaseWeapon: // Base weapon damage

            /*ModuleSystem.Log.Info($"unarmed : {ctx.isUnarmedAttack}");

            if (!ctx.isUnarmedAttack)
              ModuleSystem.Log.Info($"weapon : {ctx.attackWeapon.Name}");*/

            targetAC = ctx.isUnarmedAttack ? HandleReducedDamageFromWeaponDamageType(ctx, new List<DamageType>() { DamageType.Bludgeoning }) : HandleReducedDamageFromWeaponDamageType(ctx, ctx.attackWeapon.BaseItem.WeaponType);
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

        if (targetAC < 0) targetAC = 0;

        double initialDamage = Config.GetContextDamage(ctx, damageType);
        double modifiedDamage = initialDamage * Utils.GetDamageMultiplier(targetAC);

        if (ctx.oTarget.Tag == "damage_trainer" && ctx.oAttacker.IsPlayerControlled)
        {
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Initial : {damageType.ToString().ColorString(ColorConstants.White)} - {initialDamage.ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Armure totale vs {damageType.ToString().ColorString(ColorConstants.White)} : {targetAC.ToString().ColorString(ColorConstants.White)} - Dégâts {string.Format("{0:0}", modifiedDamage).ColorString(ColorConstants.White)}", ColorConstants.Orange);
          ctx.oAttacker.ControllingPlayer.SendServerMessage($"Réduction : {string.Format("{0:0.000}", ((initialDamage - modifiedDamage) / modifiedDamage) * 100).ColorString(ColorConstants.White)}%", ColorConstants.Orange);
        }

        Config.SetContextDamage(ctx, damageType, (int)modifiedDamage);
        PlayerSystem.Log.Info($"Final : {damageType} - AC {targetAC} - Initial {initialDamage} - Final Damage {modifiedDamage}");
      }

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
    private static void ProcessAttackerItemDurability(Context ctx, Action next)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oAttacker, out PlayerSystem.Player attacker) && ctx.attackWeapon != null)
      {
        // L'attaquant est un joueur. On diminue la durabilité de son arme
        PlayerSystem.Log.Info("Entered item durability");
        PlayerSystem.Log.Info($"player : {attacker.oid.PlayerName}");

        int durabilityChance = 30;

        int dexBonus = ctx.oAttacker.GetAbilityModifier(Ability.Dexterity) - (ctx.oAttacker.ArmorCheckPenalty + ctx.oAttacker.ShieldCheckPenalty);
        if (dexBonus < 0)
          dexBonus = 0;

        int safetyLevel = attacker.learnableSkills.ContainsKey(CustomSkill.CombattantPrecautionneux) ? attacker.learnableSkills[CustomSkill.CombattantPrecautionneux].totalPoints : 0;
        durabilityChance -= dexBonus + safetyLevel;

        if (NwRandom.Roll(Utils.random, 100, 1) < 2 && NwRandom.Roll(Utils.random, 100, 1) < durabilityChance)
          DecreaseItemDurability(ctx.attackWeapon, attacker.oid);
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

      int safetyLevel = player.learnableSkills.ContainsKey(CustomSkill.CombattantPrecautionneux) ? player.learnableSkills[CustomSkill.CombattantPrecautionneux].totalPoints : 0;

      int durabilityRate = 30 - dexBonus - safetyLevel;
      if (durabilityRate < 1)
        durabilityRate = 1;

      if (NwRandom.Roll(Utils.random, 100, 1) > 1 && NwRandom.Roll(Utils.random, 100, 1) > durabilityRate)
        return;

      if (ctx.targetArmor != null)
        DecreaseItemDurability(ctx.targetArmor, player.oid);

      if(ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand) != null)
        DecreaseItemDurability(ctx.oTarget.GetItemInSlot(InventorySlot.LeftHand), player.oid);

      List<NwItem> slots = new List<NwItem>();

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
        DecreaseItemDurability(slots.ElementAt(random), player.oid);
      }
    }

    private static void DecreaseItemDurability(NwItem item, NwPlayer oPC)
    {
      item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value -= 1;
      if (item.GetObjectVariable<LocalVariableInt>("_DURABILITY").Value <= 0)
      {
        if (item.GetObjectVariable<LocalVariableString>("_ORIGINAL_CRAFTER_NAME").HasNothing)
        {
          item.Destroy();
          oPC.SendServerMessage($"Il ne reste plus que des ruines de votre {item.Name.ColorString(ColorConstants.White)}. Ces débris ne sont même pas réparables !", ColorConstants.Red);
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
