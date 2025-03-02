using System.Linq;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int DealSpellDamage(NwGameObject target, int casterLevel, SpellEntry spellEntry, int nbDices, NwGameObject oCaster, byte spellLevel, SavingThrowResult saveResult = SavingThrowResult.Failure, bool noLogs = false, NwClass casterClass = null, int damageDice = 0, DamageType forcedDamage = DamageType.BaseWeapon)
    {
      if (saveResult == SavingThrowResult.Immune)
        return 0;

      NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
      NwCreature castingCreature = oCaster is NwCreature tempCaster ? tempCaster : null;
      damageDice = damageDice > 0 ? damageDice : spellEntry.damageDice;
      int roll = NwRandom.Roll(Utils.random, damageDice, nbDices);
      int totalDamage = 0;
      bool isEvocateurSurcharge = castingCreature is not null && castingCreature.KnowsFeat((Feat)CustomSkill.EvocateurSurcharge) && spell.SpellSchool == SpellSchool.Evocation
          && 0 < spellLevel && spellLevel < 6 && castingCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.EvocateurSurchargeEffectTag); ;
      bool isFureurDestructrice = oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.FureurDestructriceEffectTag);
      bool moissonDuFielTriggered = target.HP > 0 && castingCreature is not null && !Utils.In(castingCreature.Race.RacialType, RacialType.Undead, RacialType.Construct);
      int amplifiedDices = 0;
      int maxSorcBurst = 0;
      string logString = "Jet : ";
      List<int> rolls = new();

      if (castingCreature is not null && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoAmplification))
      {
        amplifiedDices = castingCreature.GetAbilityModifier(Ability.Charisma) < 1 ? 1 : castingCreature.GetAbilityModifier(Ability.Charisma);
        EffectUtils.RemoveTaggedParamEffect(castingCreature, CustomSkill.EnsoAmplification, EffectSystem.MetamagieEffectTag);
      }

      DamageType transmutedDamage = DamageType.BaseWeapon;

      if(casterClass is not null)
      {
        maxSorcBurst = castingCreature.GetAbilityModifier(casterClass.SpellCastingAbility);

        if(casterClass.Id == CustomClass.Occultiste && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.SortsPsychiquesEffectTag))
          transmutedDamage = CustomDamageType.Psychic;
        else if (Players.TryGetValue(oCaster, out var transmuter)
        && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoTransmutation))
        {
          transmutedDamage = transmuter.windows.TryGetValue("ensoMetaTransmutationSelection", out var transmu)
            ? ((EnsoMetaTransmutationSelectionWindow)transmu).selectedDamageType : DamageType.BaseWeapon;

          EffectUtils.RemoveTaggedParamEffect(oCaster, CustomSkill.EnsoTransmutation, EffectSystem.MetamagieEffectTag);
        }
      }      

      foreach (DamageType damageType in spellEntry.damageType)
      {
        DamageType appliedDamage = forcedDamage == DamageType.BaseWeapon ? damageType : forcedDamage;
        appliedDamage = transmutedDamage == DamageType.BaseWeapon ? damageType : transmutedDamage;
        int damage = 0;
        bool isElementalist = Players.TryGetValue(castingCreature, out Player player)
          && player.learnableSkills.TryGetValue(CustomSkill.Elementaliste, out LearnableSkill elementalist)
          && elementalist.featOptions.Any(e => e.Value.Any(d => d == (int)appliedDamage));

        for (int i = 0; i < nbDices; i++)
        {
          roll = isEvocateurSurcharge ? damageDice : NwRandom.Roll(Utils.random, damageDice);

          if(isFureurDestructrice && Utils.In(appliedDamage, DamageType.Electrical, DamageType.Sonic))
          {
            roll = damageDice;
            EffectUtils.RemoveTaggedEffect(oCaster, 0.2, EffectSystem.FureurDestructriceEffectTag);
          }

          roll = isElementalist && roll < 2 ? 2 : roll;

          if (castingCreature is not null)
          {
            switch (appliedDamage)
            {
              case DamageType.Piercing:
                if (castingCreature.KnowsFeat((Feat)CustomSkill.Empaleur))
                  roll = roll < 3 ? NwRandom.Roll(Utils.random, damageDice) : roll;
                break;

              case DamageType.Fire:
                if (castingCreature.KnowsFeat((Feat)CustomSkill.FlammesDePhlegetos))
                  roll = roll < 2 ? NwRandom.Roll(Utils.random, damageDice) : roll;
                break;
            }
          }

          if (amplifiedDices > 0)
          {
            int tempRoll = NwRandom.Roll(Utils.random, damageDice);
            LogUtils.LogMessage($"Amplification : {roll} et {tempRoll}", LogUtils.LogType.Combat);
            roll = tempRoll > roll ? tempRoll : roll;
            amplifiedDices -= 1;
          }

          if (spell.MasterSpell?.Id == CustomSpell.OrbeChromatique && oCaster.GetObjectVariable<LocalVariableInt>("_CHROMATIC_ORB_BOUNCE").HasNothing)
            rolls.Add(roll);

          if (spell.MasterSpell?.Id == CustomSpell.EclatSorcier && roll == 8 && maxSorcBurst > 0)
          {
            maxSorcBurst -= 1;
            int bonusBurst = Utils.Roll(8);
            damage += bonusBurst;

            if(bonusBurst == 8)
            {
              maxSorcBurst -= 1;
              bonusBurst = Utils.Roll(8);
              damage += bonusBurst;
            }
          }

          damage += roll;
          logString += $"{roll} + ";
        }

        int bonusDamage = HandleEvocateurSuperieur(castingCreature, spell);

        if (bonusDamage > 0)
        {
          logString += $"{bonusDamage} + ";
          damage += bonusDamage;
        }

        bonusDamage = HandleIncantationPuissante(castingCreature, spell);

        if (bonusDamage > 0)
        {
          logString += $"{bonusDamage} + ";
          damage += bonusDamage;
        }

        bonusDamage = OccultisteUtils.HandleAmeRadieuse(castingCreature, damageType);

        if (bonusDamage > 0)
        {
          logString += $"{bonusDamage} + ";
          damage += bonusDamage;
        }

        bonusDamage = OccultisteUtils.DechargeDechirante(castingCreature, spellLevel, casterClass);

        if (bonusDamage > 0)
        {
          logString += $"{bonusDamage} + ";
          damage += bonusDamage;
        }

        bonusDamage = EnsoUtils.HandleElementalAffinity(castingCreature, appliedDamage);

        if (bonusDamage > 0)
        {
          logString += $"{bonusDamage} + ";
          damage += bonusDamage;
        }

        bonusDamage += spell.Id switch
        {
          CustomSpell.EtincelleDivineNecrotique or CustomSpell.EtincelleDivineRadiant => CreatureUtils.GetAbilityModifierMin1(castingCreature, Ability.Wisdom),
          CustomSpell.LameArdente => castingCreature.GetAbilityModifier(casterClass is null ? Ability.Charisma : casterClass.SpellCastingAbility),
          _ => 0,
        };

        if (bonusDamage > 0)
        {
          logString += $"{bonusDamage} + ";
          damage += bonusDamage;
        }

        if (rolls.Count > 0 && rolls.GroupBy(r => r).Any(g => g.Count() > 1))
          oCaster.GetObjectVariable<LocalVariableInt>("_CHROMATIC_ORB_BOUNCE").Value = 1;

        LogUtils.LogMessage($"Dégâts initiaux : {logString.Remove(logString.Length - 2)} = {damage}", LogUtils.LogType.Combat);
        logString = "";

        if (target is NwCreature targetCreature)
        {
          damage = HandleSpellEvasion(targetCreature, damage, spellEntry.savingThrowAbility, saveResult, spell.Id, spellLevel);
          damage = ItemUtils.GetShieldMasterReducedDamage(targetCreature, damage, saveResult, spellEntry.savingThrowAbility);
          damage = WizardUtils.GetAbjurationReducedDamage(targetCreature, damage);
          damage = ClercUtils.GetAttenuationElementaireReducedDamage(targetCreature, damage, appliedDamage);
        }

        if (oCaster is not null)
          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, appliedDamage))));
        else
          target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, appliedDamage)));

        if (!noLogs)
          LogUtils.LogMessage($"Dégâts sur {target.Name} : {nbDices}d{damageDice} (caster lvl {casterLevel}) = {damage} {StringUtils.GetDamageTypeTraduction(appliedDamage)}", LogUtils.LogType.Combat);

        totalDamage += damage;
      }

      if (spell.Id == CustomSpell.RadianceDelAube && castingCreature is not null && castingCreature.GetClassInfo(ClassType.Cleric) is not null)
        totalDamage += castingCreature.GetClassInfo(ClassType.Cleric).Level;

      WizardUtils.HandleMoissonDuFiel(oCaster, target, moissonDuFielTriggered, spell, spellLevel);
      ClercUtils.HandleIncantationPuissante(oCaster, target, totalDamage, spell);

      return totalDamage;
    }
  }
}
