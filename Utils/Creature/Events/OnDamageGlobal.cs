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
      int baseDamage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);
      bool isWeaponAttack = baseDamage > -1;
      int totalDamage = 0;
      int nbDice = 0;
      bool isSurcharge = false;

      if (isWeaponAttack)
      {
        if (Utils.In(onDamage.DamagedBy.ResRef, "undeadspiritskel", "loupcompagnon"))
          NativeUtils.ReplaceWeaponDamage(onDamage.DamageData, CustomDamageType.Necrotic);

        if (damager is not null)
        {
          if (!damager.IsRangedWeaponEquipped)
          {
            NwItem pactWeapon = damager.GetObjectVariable<LocalVariableObject<NwItem>>(PacteDeLaLameVariable).Value;

            if (pactWeapon is not null && pactWeapon == damager.GetItemInSlot(InventorySlot.RightHand))
            {
              switch (pactWeapon.GetObjectVariable<LocalVariableInt>(PacteDeLaLameVariable).Value)
              {
                case CustomSpell.PacteDeLaLameRadiant: NativeUtils.ReplaceWeaponDamage(onDamage.DamageData, DamageType.Divine); break;
                case CustomSpell.PacteDeLaLameNecrotique: NativeUtils.ReplaceWeaponDamage(onDamage.DamageData, CustomDamageType.Necrotic); break;
                case CustomSpell.PacteDeLaLamePsychique: NativeUtils.ReplaceWeaponDamage(onDamage.DamageData, CustomDamageType.Psychic); break;
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

                      if (damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue)
                        nbDice *= 2;

                      string logString = "";
                      int damage = 0;

                      for (int i = 0; i < nbDice; i++)
                      {
                        int roll = Utils.Roll(8);
                        logString += $"{roll} + ";
                        damage += roll;
                      }

                      NativeUtils.AddWeaponDamage(onDamage.DamageData, DamageType.Magical, damage);
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

                    NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Instant,
                      Effect.Heal(Utils.Roll(6) + GetAbilityModifierMin1(damager, Ability.Constitution))));

                    NativeUtils.AddWeaponDamage(onDamage.DamageData, damageType, Utils.Roll(6, 1 + damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue.ToInt()));

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
              case EffectSystem.ShillelaghEffectTag:

                Effect shillelagEffect = damager.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ShillelaghEffectTag);

                if (shillelagEffect is not null && shillelagEffect.Spell.Id == CustomSpell.ShillelaghForce)
                {
                  NwItem shillelaghWeapon = damager.GetItemInSlot(InventorySlot.RightHand);

                  if (shillelaghWeapon is not null && Utils.In(shillelaghWeapon.BaseItem.ItemType, BaseItemType.Club, BaseItemType.Quarterstaff, BaseItemType.MagicStaff))
                    NativeUtils.ReplaceWeaponDamage(onDamage.DamageData, DamageType.Magical);
                }

                break;

              case EffectSystem.ChatimentDivinEffectTag:

                if (damager.IsRangedWeaponEquipped)
                  break;

                Effect chatimentEffect = damager.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ChatimentDivinEffectTag);

                if (chatimentEffect is not null)
                {
                  nbDice = 1 + chatimentEffect.CasterLevel;

                  if (Utils.In(target.Race.RacialType, RacialType.Undead, CustomRacialType.Fielon))
                  {
                    LogUtils.LogMessage($"Châtiment Divin - Cible mort-vivant ou extérieur : +1d8", LogUtils.LogType.Combat);
                    nbDice += 1;
                  }

                  nbDice *= damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue ? 2 : 1;

                  string logString = "";
                  int damage = 0;

                  for (int i = 0; i < nbDice; i++)
                  {
                    int roll = Utils.Roll(8);
                    logString += $"{roll} + ";
                    damage += roll;
                  }

                  NativeUtils.AddWeaponDamage(onDamage.DamageData, DamageType.Divine, damage);

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
                  nbDice = damager.GetClassInfo(ClassType.Cleric).Level > 13 ? 2 : 1;
                  if (damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue)
                    nbDice *= 2;

                  NativeUtils.AddWeaponDamage(onDamage.DamageData, eff.Spell.Id == CustomSpell.FrappeDivineNecrotique ? CustomDamageType.Necrotic : DamageType.Divine,
                    Utils.Roll(8, nbDice));

                  EffectUtils.RemoveTaggedEffect(damager, EffectSystem.FrappeDivineEffectTag);

                  NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Temporary,
                    EffectSystem.Cooldown(damager, 5, CustomSkill.ClercFrappeDivine, eff.Spell), TimeSpan.FromSeconds(5)));
                }

                break;

              case EffectSystem.FrappesRenforceesEffectTag:

                if (damager.GetItemInSlot(InventorySlot.RightHand) is null)
                  NativeUtils.ReplaceWeaponDamage(onDamage.DamageData, DamageType.Magical);

                break;

              case EffectSystem.FrappeRedoutableEffectTag:

                nbDice = damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue ? 4 : 2;
                int damageDice = damager.KnowsFeat((Feat)CustomSkill.TraqueurRafale) ? 8 : 6;

                NativeUtils.AddWeaponDamage(onDamage.DamageData, CustomDamageType.Psychic, Utils.Roll(damageDice, nbDice));

                NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Temporary,
                  EffectSystem.Cooldown(damager, 6, CustomSkill.ProfondeursFrappeRedoutable), TimeSpan.FromSeconds(5)));

                EffectUtils.RemoveTaggedEffect(damager, EffectSystem.FrappeRedoutableEffectTag);

                break;

              case EffectSystem.PourfendeurDeColossesEffectTag:

                if (target.HP < target.MaxHP)
                {
                  nbDice = damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue ? 2 : 1;
                  NativeUtils.AddWeaponDamage(onDamage.DamageData, DamageType.BaseWeapon, Utils.Roll(8, nbDice));

                  NWScript.AssignCommand(damager, () => damager.ApplyEffect(EffectDuration.Temporary,
                    EffectSystem.Cooldown(damager, 6, CustomSkill.ChasseurProie, NwSpell.FromSpellId(CustomSpell.PourfendeurDeColosses)), NwTimeSpan.FromRounds(1)));

                  EffectUtils.RemoveTaggedEffect(damager, EffectSystem.PourfendeurDeColossesEffectTag);
                }

                break;

              case EffectSystem.BrandingSmiteAttackEffectTag:

                var spellEntry = Spells2da.spellTable[CustomSpell.BrandingSmite];
                var duration = SpellUtils.GetSpellDuration(damager, spellEntry);
                nbDice = damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue ? 4 : 2;

                NativeUtils.AddWeaponDamage(onDamage.DamageData, DamageType.Divine, Utils.Roll(6, nbDice));
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

                nbDice = damager.GetObjectVariable<LocalVariableInt>(CriticalHitVariable).HasValue ? 2 : 1;

                NativeUtils.AddWeaponDamage(onDamage.DamageData, DamageType.Fire, Utils.Roll(6, nbDice));

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

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
      {
        var damage = onDamage.DamageData.GetDamageByType(damageType);

        if (damage > 0)
        {
          if (!isSurcharge && target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetImmunityEffectTagByDamageType(damageType)))
          {
            onDamage.DamageData.SetDamageByType(damageType, 0);
            continue;
          }

          if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetVulnerabilityEffectTagByDamageType(damageType)))
          {
            damage *= 2;
            onDamage.DamageData.SetDamageByType(damageType, damage);
          }

          if (!isSurcharge && target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetResistanceEffectTagByDamageType(damageType)))
          {
            damage /= 2;
            onDamage.DamageData.SetDamageByType(damageType, damage);
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
              case CustomSpell.ResistanceContondant: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Bludgeoning, target, eff.Spell); break;
              case CustomSpell.ResistanceAcide: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Acid, target, eff.Spell); break;
              case CustomSpell.ResistanceElec: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Electrical, target, eff.Spell); break;
              case CustomSpell.ResistanceFeu: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Fire, target, eff.Spell); break;
              case CustomSpell.ResistanceFroid: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Cold, target, eff.Spell); break;
              case CustomSpell.ResistancePercant: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Piercing, target, eff.Spell); break;
              case CustomSpell.ResistanceTranchant: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Slashing, target, eff.Spell); break;
              case CustomSpell.ResistancePoison: totalDamage -= ApplyResistanceReduction(onDamage.DamageData, DamageType.Custom1, target, eff.Spell); break;
            }

            break;

          case EffectSystem.AbjurationWardEffectTag:

            if (abjurationWardTriggered)
              break;

            int wardIntensity = eff.CasterLevel;

            foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
            {
              var damage = onDamage.DamageData.GetDamageByType(damageType);

              if(damage > 0)
              {
                abjurationWardTriggered = true;

                if(damage > wardIntensity)
                {
                  onDamage.DamageData.SetDamageByType(damageType, damage - wardIntensity);
                  break;
                }
                else
                {
                  onDamage.DamageData.SetDamageByType(damageType, -1);
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
        int baseWeaponDamage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);

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
                onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, reducedDamage);

                LogUtils.LogMessage($"Defenses Enjoleuses - Dégâts initiaux : {baseWeaponDamage} / 2 = {reducedDamage}", LogUtils.LogType.Combat);
                StringUtils.DisplayStringToAllPlayersNearTarget(target, "Défenses Enjôleuses", ColorConstants.Pink, true, true);

                if (GetSavingThrow(target, damager, Ability.Wisdom, spellDC) == SavingThrowResult.Failure)
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
                    int ouraganDamage = GetSavingThrow(target, damager, Ability.Dexterity, ouraganDC) == SavingThrowResult.Failure ? Utils.Roll(8, 2) : Utils.Roll(8, 1);
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
          int psyDamage = onDamage.DamageData.GetDamageByType(CustomDamageType.Psychic);

          if (psyDamage > 0 && target.ActiveEffects.Any(e => e.Tag == EffectSystem.BouclierPsychiqueEffectTag))
          {
            StringUtils.DisplayStringToAllPlayersNearTarget(target, "Bouclier Psychique", ColorConstants.Pink, true, true);
            NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(psyDamage, CustomDamageType.Psychic)));
          }
        }

        NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.DamagedBy, NwTimeSpan.FromRounds(1)));
      }

      if (totalDamage >= target.HP && target.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
      {
        int remainingDamage = totalDamage - target.HP;
        target.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").Value -= remainingDamage;
        EffectUtils.RemoveTaggedEffect(target, EffectSystem.PolymorphEffectTag);
        target.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(totalDamage), TimeSpan.FromSeconds(0.59f));
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
