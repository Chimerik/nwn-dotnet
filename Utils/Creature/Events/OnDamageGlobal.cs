using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using NWN.Core;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageGlobal(OnCreatureDamage onDamage)
    {
      if (onDamage.Target is not NwCreature target)
        return;

      NwCreature damager = onDamage.DamagedBy is NwCreature tempDamager ? tempDamager : null;
      var damageData = onDamage.DamageData;
      int baseDamage = damageData.GetDamageByType(DamageType.BaseWeapon);
      bool isWeaponAttack = baseDamage > -1;
      int totalDamage = 0;
      int nbDice = 0;
      int roll;
      bool isSurcharge = false;
      bool isCritical = damager is not null && damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue;

      if (isWeaponAttack)
      {
        if (Utils.In(onDamage.DamagedBy.ResRef, "undeadspiritskel", "loupcompagnon"))
          NativeUtils.ReplaceWeaponDamage(damageData, CustomDamageType.Necrotic);

        if (damager is not null)
        {
          if (!damager.IsRangedWeaponEquipped)
          {
            NwItem pactWeapon = damager.GetObjectVariable<LocalVariableObject<NwItem>>(PacteDeLaLameVariable).Value;

            if (pactWeapon is not null && pactWeapon == damager.GetItemInSlot(InventorySlot.RightHand))
            {
              switch (pactWeapon.GetObjectVariable<LocalVariableInt>(PacteDeLaLameVariable).Value)
              {
                case CustomSpell.PacteDeLaLameRadiant: NativeUtils.ReplaceWeaponDamage(damageData, DamageType.Divine); break;
                case CustomSpell.PacteDeLaLameNecrotique: NativeUtils.ReplaceWeaponDamage(damageData, CustomDamageType.Necrotic); break;
                case CustomSpell.PacteDeLaLamePsychique: NativeUtils.ReplaceWeaponDamage(damageData, CustomDamageType.Psychic); break;
              }

              foreach (var eff in damager.ActiveEffects)
              {
                switch (eff.Tag)
                {
                  case EffectSystem.ChatimentOcculteEffectTag:

                    var occultisteClass = damager.GetClassInfo((ClassType)CustomClass.Occultiste);
                    byte remainingSlots = occultisteClass.GetRemainingSpellSlots(1);
                    byte consumedSlots = (byte)(remainingSlots - 1);

                    if (remainingSlots > 0)
                    {
                      for (byte i = 1; i < 10; i++)
                        occultisteClass.SetRemainingSpellSlots(i, consumedSlots);

                      damager.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, consumedSlots);

                      nbDice = 1 + SpellUtils.GetMaxSpellSlotLevelKnown(damager, (ClassType)CustomClass.Occultiste);
                      nbDice *= isCritical ? 2 : 1;

                      string logString = "";
                      int damage = 0;

                      for (int i = 0; i < nbDice; i++)
                      {
                        roll = Utils.Roll(8);
                        logString += $"{roll} + ";
                        damage += roll;
                      }

                      totalDamage += damage;
                      NativeUtils.AddWeaponDamage(damageData, DamageType.Magical, damage);
                      EffectSystem.ApplyKnockdown(target, damager);

                      EffectUtils.RemoveTaggedEffect(damager, EffectSystem.ChatimentOcculteEffectTag);

                      StringUtils.DisplayStringToAllPlayersNearTarget(damager, $"{damager.Name.ColorString(ColorConstants.Cyan)} " +
                        $"Châtiment Occulte {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.brightPurple, true, true);

                      LogUtils.LogMessage($"Châtiment Occulte - {nbDice}d8 : {logString.Remove(logString.Length - 2)} = {damage}", LogUtils.LogType.Combat);
                    }

                    break;

                  case EffectSystem.BuveuseDeVieEffectTag:

                    var damageType = eff.Spell.Id switch
                    {
                      CustomSpell.BuveuseDeVieRadiant => DamageType.Divine,
                      CustomSpell.BuveuseDeVieNecrotique => CustomDamageType.Necrotic,
                      _ => CustomDamageType.Psychic,
                    };

                    roll = Utils.Roll(6, 1 + isCritical.ToInt());

                    NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Instant,
                      Effect.Heal(roll + GetAbilityModifierMin1(damager, Ability.Constitution))));

                    totalDamage += roll;
                    NativeUtils.AddWeaponDamage(damageData, damageType, roll);

                    EffectUtils.RemoveTaggedEffect(damager, EffectSystem.BuveuseDeVieEffectTag);

                    NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Temporary,
                      EffectSystem.Cooldown(damager, 6, CustomSkill.BuveuseDeVie, eff.Spell), TimeSpan.FromSeconds(5)));

                    break;
                }
              }
            }
          }

          foreach (var eff in damager.ActiveEffects)
          {
            switch (eff.Tag)
            {
              case EffectSystem.ChatimentAmelioreEffectTag:

                roll = Utils.Roll(8, 1 + isCritical.ToInt());
                totalDamage += roll;
                NativeUtils.AddWeaponDamage(damageData, DamageType.Divine, roll); 
                break;

              case EffectSystem.ShillelaghEffectTag:

                Effect shillelagEffect = damager.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ShillelaghEffectTag);

                if (shillelagEffect is not null && shillelagEffect.Spell.Id == CustomSpell.ShillelaghForce)
                {
                  NwItem shillelaghWeapon = damager.GetItemInSlot(InventorySlot.RightHand);

                  if (shillelaghWeapon is not null && Utils.In(shillelaghWeapon.BaseItem.ItemType, BaseItemType.Club, BaseItemType.Quarterstaff, BaseItemType.MagicStaff))
                    NativeUtils.ReplaceWeaponDamage(damageData, DamageType.Magical);
                }

                break;

              case EffectSystem.ChatimentDivinEffectTag:

                if (damager.IsRangedWeaponEquipped)
                  break;

                Effect chatimentEffect = damager.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ChatimentDivinEffectTag);

                if (chatimentEffect is not null)
                {
                  nbDice = (1 + chatimentEffect.CasterLevel
                    + Utils.In(target.Race.RacialType, RacialType.Undead, CustomRacialType.Fielon).ToInt())
                    * (1 + isCritical.ToInt());

                  string logString = "";
                  int damage = 0;

                  for (int i = 0; i < nbDice; i++)
                  {
                    roll = Utils.Roll(8);
                    logString += $"{roll} + ";
                    damage += roll;
                  }

                  totalDamage += damage;
                  NativeUtils.AddWeaponDamage(damageData, DamageType.Divine, damage);

                  LogUtils.LogMessage($"Châtiment Divin - {nbDice}d8 : {logString.Remove(logString.Length - 2)} = {damage}", LogUtils.LogType.Combat);

                  EffectUtils.RemoveTaggedEffect(damager, EffectSystem.ChatimentDivinEffectTag);

                  StringUtils.DisplayStringToAllPlayersNearTarget(damager, $"{damager.Name.ColorString(ColorConstants.Cyan)} " +
                    $"Châtiment Divin {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
                }

                break;

              case EffectSystem.FrappeDivineEffectTag:

                NwItem mainWeapon = damager.GetItemInSlot(InventorySlot.RightHand);

                if (mainWeapon is not null && ItemUtils.IsWeapon(mainWeapon.BaseItem))
                {
                  nbDice = (1 + (damager.GetClassInfo(ClassType.Cleric).Level > 13).ToInt()) * (1 + isCritical.ToInt());
                  roll = Utils.Roll(8, nbDice);

                  totalDamage += roll;
                  NativeUtils.AddWeaponDamage(damageData, eff.Spell.Id == CustomSpell.FrappeDivineNecrotique ? CustomDamageType.Necrotic : DamageType.Divine, roll);

                  EffectUtils.RemoveTaggedEffect(damager, EffectSystem.FrappeDivineEffectTag);

                  NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Temporary,
                    EffectSystem.Cooldown(damager, 5, CustomSkill.ClercFrappeDivine, eff.Spell), TimeSpan.FromSeconds(5)));
                }

                break;

              case EffectSystem.FrappesRenforceesEffectTag:

                if (damager.GetItemInSlot(InventorySlot.RightHand) is null)
                  NativeUtils.ReplaceWeaponDamage(damageData, DamageType.Magical);

                break;

              case EffectSystem.FrappeRedoutableEffectTag:

                nbDice = isCritical ? 4 : 2;
                int damageDice = damager.KnowsFeat((Feat)CustomSkill.TraqueurRafale) ? 8 : 6;
                roll = Utils.Roll(damageDice, nbDice);

                totalDamage += roll;
                NativeUtils.AddWeaponDamage(damageData, CustomDamageType.Psychic, roll);

                NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Temporary,
                  EffectSystem.Cooldown(damager, 6, CustomSkill.ProfondeursFrappeRedoutable), TimeSpan.FromSeconds(5)));

                EffectUtils.RemoveTaggedEffect(damager, EffectSystem.FrappeRedoutableEffectTag);

                break;

              case EffectSystem.PourfendeurDeColossesEffectTag:

                if (target.HP < target.MaxHP)
                {
                  nbDice = 1 + isCritical.ToInt();
                  roll = Utils.Roll(8, nbDice);

                  totalDamage += roll;
                  NativeUtils.AddWeaponDamage(damageData, DamageType.BaseWeapon, roll);

                  NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Temporary,
                    EffectSystem.Cooldown(damager, 6, CustomSkill.ChasseurProie, NwSpell.FromSpellId(CustomSpell.PourfendeurDeColosses)), NwTimeSpan.FromRounds(1)));

                  EffectUtils.RemoveTaggedEffect(damager, EffectSystem.PourfendeurDeColossesEffectTag);
                }

                break;

              case EffectSystem.BrandingSmiteAttackEffectTag:

                var spellEntry = Spells2da.spellTable[CustomSpell.BrandingSmite];
                var duration = SpellUtils.GetSpellDuration(damager, spellEntry);
                nbDice = isCritical ? 4 : 2;
                roll = Utils.Roll(6, nbDice);

                totalDamage += roll;
                NativeUtils.AddWeaponDamage(damageData, DamageType.Divine, roll);
                NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDivineStrikeHoly)));
                NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.brandingSmiteReveal, duration));

                target.SetActionMode(ActionMode.Stealth, false);
                target.OnStealthModeUpdate -= EffectSystem.OnBrandingSmiteReveal;
                target.OnStealthModeUpdate += EffectSystem.OnBrandingSmiteReveal;

                target.OnEffectApply -= EffectSystem.OnBrandingSmiteReveal;
                target.OnEffectApply += EffectSystem.OnBrandingSmiteReveal;

                EffectUtils.RemoveEffectType(target, EffectType.Invisibility, EffectType.ImprovedInvisibility);
                EffectSystem.ApplyConcentrationEffect(damager, spellEntry.RowIndex, new List<NwGameObject>() { target }, duration);

                break;

              case EffectSystem.searingSmiteAttackEffectTag:

                if (damager.IsRangedWeaponEquipped)
                  break;

                nbDice = 1 + isCritical.ToInt();
                roll = Utils.Roll(6, nbDice);

                totalDamage += roll;
                NativeUtils.AddWeaponDamage(damageData, DamageType.Fire, roll);

                NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS)));
                NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.searingSmiteBurn, NwTimeSpan.FromRounds(Spells2da.spellTable[CustomSpell.SearingSmite].duration)));

                target.OnHeartbeat -= EffectSystem.OnSearingSmiteBurn;
                target.OnHeartbeat += EffectSystem.OnSearingSmiteBurn;

                break;
            }
          }

          damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).Delete();
        }
      }
      else
        isSurcharge = damager is not null && damager.ActiveEffects.Any(e => e.Tag == EffectSystem.EvocateurSurchargeEffectTag);

      if(damageData.GetDamageByType(DamageType.BaseWeapon) > 0)
      {
        DamageType weaponDamageType = (DamageType)damager.GetObjectVariable<LocalVariableInt>(BaseWeaponDamageTypeVariable).Value;
        var damage = damageData.GetDamageByType(DamageType.BaseWeapon);

        if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetImmunityEffectTagByDamageType(weaponDamageType)))
        {
          totalDamage -= damage;
          damageData.SetDamageByType(DamageType.BaseWeapon, 0);
          LogUtils.LogMessage($"{target.Name} immunisé aux dégâts {weaponDamageType}", LogUtils.LogType.Combat);
        }
        else
        {
          if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetVulnerabilityEffectTagByDamageType(weaponDamageType)))
          {
            damageData.SetDamageByType(DamageType.BaseWeapon, damage * 2);
            LogUtils.LogMessage($"{target.Name} vulnérable aux dégâts {weaponDamageType} : {damage} * 2 = {damageData.GetDamageByType(DamageType.BaseWeapon)}", LogUtils.LogType.Combat);
          }

          if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetResistanceEffectTagByDamageType(weaponDamageType)))
          {
            damageData.SetDamageByType(DamageType.BaseWeapon, damage / 2);
            LogUtils.LogMessage($"{target.Name} résistant aux dégâts {weaponDamageType} : {damage} / 2 = {damageData.GetDamageByType(DamageType.BaseWeapon)}", LogUtils.LogType.Combat);
          }
        }
      }

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
      {
        var damage = damageData.GetDamageByType(damageType);

        if (damage > 0)
        {
          if (!isSurcharge && target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetImmunityEffectTagByDamageType(damageType)))
          {
            totalDamage -= damage;
            damageData.SetDamageByType(damageType, 0);
            LogUtils.LogMessage($"{target.Name} immunisé aux dégâts {damageType}", LogUtils.LogType.Combat);
            continue;
          }

          if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetVulnerabilityEffectTagByDamageType(damageType)))
          {
            LogUtils.LogMessage($"{target.Name} vulnérable aux dégâts {damageType} : {damage} * 2 = {damage * 2}", LogUtils.LogType.Combat);
            damage *= 2;
            damageData.SetDamageByType(damageType, damage);
          }

          if (!isSurcharge && target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetResistanceEffectTagByDamageType(damageType)))
          {
            LogUtils.LogMessage($"{target.Name} vulnérable aux dégâts {damageType} : {damage} / 2 = {damage / 2}", LogUtils.LogType.Combat);
            damage /= 2;
            damageData.SetDamageByType(damageType, damage);
          }

          totalDamage += damage;
        }
      }

      bool abjurationWardTriggered = false;

      foreach (var eff in target.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case EffectSystem.ResistanceEffectTag:

            if (isSurcharge || target.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSpell.Resistance))
              break;

            switch (eff.Spell.Id)
            {
              case CustomSpell.ResistanceContondant: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Bludgeoning, target, eff.Spell); break;
              case CustomSpell.ResistanceAcide: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Acid, target, eff.Spell); break;
              case CustomSpell.ResistanceElec: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Electrical, target, eff.Spell); break;
              case CustomSpell.ResistanceFeu: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Fire, target, eff.Spell); break;
              case CustomSpell.ResistanceFroid: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Cold, target, eff.Spell); break;
              case CustomSpell.ResistancePercant: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Piercing, target, eff.Spell); break;
              case CustomSpell.ResistanceTranchant: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Slashing, target, eff.Spell); break;
              case CustomSpell.ResistancePoison: totalDamage -= ApplyResistanceReduction(damageData, DamageType.Custom1, target, eff.Spell); break;
            }

            break;

          case EffectSystem.AbjurationWardEffectTag:

            if (abjurationWardTriggered)
              break;

            int wardIntensity = eff.CasterLevel;

            foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
            {
              var damage = damageData.GetDamageByType(damageType);

              if(damage > 0)
              {
                abjurationWardTriggered = true;

                if(damage > wardIntensity)
                {
                  damageData.SetDamageByType(damageType, damage - wardIntensity);
                  break;
                }
                else
                {
                  damageData.SetDamageByType(damageType, -1);
                  wardIntensity -= damage;
                }
              }
            }

            if(abjurationWardTriggered)
              EffectSystem.AbjurationWardIntensityReduction(eff, target);

            break;
        }
      }

      if (damager is not null)
      {
        int baseWeaponDamage = damageData.GetDamageByType(DamageType.BaseWeapon);

        if (isWeaponAttack && baseWeaponDamage > 0)
        {
          foreach (var eff in target.ActiveEffects)
          {
            switch (eff.Tag)
            {
              case EffectSystem.DefensesEnjoleusesEffectTag:

                EffectUtils.RemoveTaggedEffect(target, EffectSystem.DefensesEnjoleusesEffectTag);

                int spellDC = SpellUtils.GetCasterSpellDC(target, Ability.Charisma);
                int reducedDamage = baseWeaponDamage / 2;
                totalDamage -= reducedDamage;
                damageData.SetDamageByType(DamageType.BaseWeapon, reducedDamage);

                LogUtils.LogMessage($"Defenses Enjoleuses - Dégâts initiaux : {baseWeaponDamage} / 2 = {reducedDamage}", LogUtils.LogType.Combat);
                StringUtils.DisplayStringToAllPlayersNearTarget(target, "Défenses Enjôleuses", ColorConstants.Pink, true, true);

                if (GetSavingThrowResult(damager, Ability.Wisdom, target, spellDC) == SavingThrowResult.Failure)
                {
                  NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(reducedDamage, CustomDamageType.Psychic)));
                }

                break;

              case EffectSystem.FureurDelOuraganEffectTag:

                var weapon = damager.GetItemInSlot(InventorySlot.RightHand);

                if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
                {
                  if (HandleReactionUse(target))
                  {
                    int ouraganDC = SpellUtils.GetCasterSpellDC(target, Ability.Wisdom);
                    int ouraganDamage = GetSavingThrowResult(damager, Ability.Dexterity, target, ouraganDC) == SavingThrowResult.Failure ? Utils.Roll(8, 2) : Utils.Roll(8, 1);
                    NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(ouraganDamage, eff.Spell == NwSpell.FromSpellId(CustomSpell.FureurDelOuraganFoudre) ? DamageType.Electrical : DamageType.Sonic)));

                    EffectUtils.RemoveTaggedEffect(target, EffectSystem.FureurDelOuraganEffectTag);
                  }
                }

                break;
            }
          }
        }
        else
        {
          int psyDamage = damageData.GetDamageByType(CustomDamageType.Psychic);

          if (psyDamage > 0 && target.ActiveEffects.Any(e => e.Tag == EffectSystem.BouclierPsychiqueEffectTag))
          {
            StringUtils.DisplayStringToAllPlayersNearTarget(target, "Bouclier Psychique", ColorConstants.Pink, true, true);
            NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(psyDamage, CustomDamageType.Psychic)));
          }
        }

        if (damager.GetClassInfo((ClassType.Rogue))?.Level > 16 && NativeUtils.IsAssassinate(damager, target))
        {
          int spellDC = SpellUtils.GetCasterSpellDC(damager, Ability.Dexterity);

          if (GetSavingThrowResult(target, Ability.Constitution, damager, spellDC) == SavingThrowResult.Failure)
          {
            foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
            {
              var damage = damageData.GetDamageByType(damageType);

              if (damage > 0)
                damageData.SetDamageByType(damageType, damage * 2);
            }

            totalDamage *= 2;
          }
        }
          

        NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.DamagedBy, NwTimeSpan.FromRounds(1)));
      }

      if (totalDamage >= target.HP)
      {
        foreach(var effect in target.ActiveEffects)
        {
          switch (effect.Tag)
          {
            case EffectSystem.PolymorphEffectTag:

              target.Immortal = true;

              int remainingDamage = totalDamage - target.HP;
              target.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value -= remainingDamage;
              EffectUtils.RemoveEffectType(target, EffectType.Polymorph);
              
              return;

            case EffectSystem.BarbarianRageEffectTag:

              var barbarianClass = target.GetClassInfo(ClassType.Barbarian);

              if(barbarianClass is not null && barbarianClass.Level > 10)
              {
                int saveDC = target.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value;
                target.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value += 5;

                if (GetSavingThrowResult(target, Ability.Constitution, damager, saveDC) != SavingThrowResult.Failure)
                {
                  target.Immortal = true;
                  StringUtils.DisplayStringToAllPlayersNearTarget(target, "Rage Implacable", StringUtils.gold, true, true);
                  BarbarianUtils.DelayImplacableRage(target, barbarianClass.Level);
                  return;
                }
              }

              break;

            case EffectSystem.ProtectionContreLaMortEffectTag:

              target.Immortal = true;
              target.RemoveEffect(effect);
              StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{target.Name.ColorString(ColorConstants.Cyan)} - Protection contre la Mort", StringUtils.gold, true, true);

              return;

            case EffectSystem.EnduranceImplacableEffectTag:

              target.Immortal = true;
              target.RemoveEffect(effect);
              StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{target.Name.ColorString(ColorConstants.Cyan)} - Endurance Implacable", StringUtils.gold, true, true);

              if (target.KnowsFeat((Feat)CustomSkill.FureurOrc)
                && target.CurrentAction == Anvil.API.Action.AttackObject)
              {
                var reaction = target.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

                if (reaction is not null)
                {
                  target.GetObjectVariable<LocalVariableInt>(CreatureUtils.FureurOrcBonusAttackVariable).Value = 1;
                  target.RemoveEffect(reaction);
                }
              }

              return;

            case EffectSystem.SentinelleImmortelleEffectTag:

              target.Immortal = true;
              target.RemoveEffect(effect);
              StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{target.Name.ColorString(ColorConstants.Cyan)} - Sentinelle Immortelle", StringUtils.gold, true, true);

              return;
          }
        }
      }
    }

    private static int ApplyResistanceReduction(DamageData<int> damageData, DamageType damageType, NwCreature target, NwSpell spell)
    {
      int damage = damageData.GetDamageByType(damageType);
      
      if (damage > 0)
      {
        int damageReduction = Utils.Roll(4);
        int total = damage - damageReduction;
        damageData.SetDamageByType(damageType, total);

        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(target, 6, CustomSpell.Resistance, spell));

        LogUtils.LogMessage($"Résistance : {StringUtils.GetDamageTypeTraduction(damageType)} réduits de {damageReduction} ({damage} - {damageReduction} = {total})", LogUtils.LogType.Combat);

        return damageReduction;
      }

      return 0;
    }
  }
}
