using System.Linq;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int DealSpellDamage(NwGameObject target, int casterLevel, SpellEntry spellEntry, int nbDices, NwGameObject oCaster, byte spellLevel, SavingThrowResult saveResult = SavingThrowResult.Failure, bool noLogs = false, NwClass casterClass = null)
    {
      if (saveResult == SavingThrowResult.Immune)
        return 0;

      NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
      NwCreature castingCreature = oCaster is NwCreature tempCaster ? tempCaster : null;
      int roll = NwRandom.Roll(Utils.random, spellEntry.damageDice, nbDices);
      int totalDamage = 0;
      bool isEvocateurSurcharge = castingCreature is not null && castingCreature.KnowsFeat((Feat)CustomSkill.EvocateurSurcharge) && spell.SpellSchool == SpellSchool.Evocation
          && 0 < spellLevel && spellLevel < 6 && castingCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.EvocateurSurchargeEffectTag); ;
      bool isFureurDestructrice = oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.FureurDestructriceEffectTag);
      bool moissonDuFielTriggered = target.HP > 0 && castingCreature is not null && !Utils.In(castingCreature.Race.RacialType, RacialType.Undead, RacialType.Construct);
      int amplifiedDices = 0;
      string logString = "";

      if (castingCreature is not null && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoAmplification))
      {
        amplifiedDices = castingCreature.GetAbilityModifier(Ability.Charisma) < 1 ? 1 : castingCreature.GetAbilityModifier(Ability.Charisma);
        EffectUtils.RemoveTaggedParamEffect(castingCreature, CustomSkill.EnsoAmplification, EffectSystem.MetamagieEffectTag);
      }

      DamageType transmutedDamage = DamageType.BaseWeapon;

      if(casterClass is not null && casterClass.Id == CustomClass.Occultiste && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.SortsPsychiquesEffectTag))
      {
        transmutedDamage = CustomDamageType.Psychic;
      }
      else if (Players.TryGetValue(oCaster, out var transmuter) 
        && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoTransmutation))
      {
        transmutedDamage = transmuter.windows.TryGetValue("ensoMetaTransmutationSelection", out var transmu)
          ? ((EnsoMetaTransmutationSelectionWindow)transmu).selectedDamageType : DamageType.BaseWeapon;

        EffectUtils.RemoveTaggedParamEffect(oCaster, CustomSkill.EnsoTransmutation, EffectSystem.MetamagieEffectTag);
      }

      foreach (DamageType damageType in spellEntry.damageType)
      {
        DamageType appliedDamage = transmutedDamage == DamageType.BaseWeapon ? damageType : transmutedDamage;
        int damage = 0;
        bool isElementalist = Players.TryGetValue(castingCreature, out Player player)
          && player.learnableSkills.TryGetValue(CustomSkill.Elementaliste, out LearnableSkill elementalist)
          && elementalist.featOptions.Any(e => e.Value.Any(d => d == (int)appliedDamage));

        damage += EnsoUtils.HandleElementalAffinity(castingCreature, appliedDamage);

        for (int i = 0; i < nbDices; i++)
        {
          roll = isEvocateurSurcharge ? spellEntry.damageDice : NwRandom.Roll(Utils.random, spellEntry.damageDice);
          roll = isFureurDestructrice && Utils.In(appliedDamage, DamageType.Electrical, DamageType.Sonic) ? spellEntry.damageDice : roll;

          roll = isElementalist && roll < 2 ? 2 : roll;

          if (castingCreature is not null)
          {
            switch (appliedDamage)
            {
              case DamageType.Piercing:
                if (castingCreature.KnowsFeat((Feat)CustomSkill.Empaleur))
                  roll = roll < 3 ? NwRandom.Roll(Utils.random, spellEntry.damageDice) : roll;
                break;

              case DamageType.Fire:
                if (castingCreature.KnowsFeat((Feat)CustomSkill.FlammesDePhlegetos))
                  roll = roll < 2 ? NwRandom.Roll(Utils.random, spellEntry.damageDice) : roll;
                break;
            }

            roll += OccultisteUtils.DechargeDechirante(castingCreature, spellLevel, casterClass);
          } 

          if(amplifiedDices > 0)
          {
            int tempRoll = NwRandom.Roll(Utils.random, spellEntry.damageDice);
            roll = tempRoll > roll ? tempRoll : roll;
            amplifiedDices -= 1;
          }



          damage += roll;
          logString += $"{roll} + ";
        }

        damage += HandleEvocateurSuperieur(castingCreature, spell);
        damage += HandleIncantationPuissante(castingCreature, spell);
        damage += OccultisteUtils.HandleAmeRadieuse(castingCreature, damageType);

        LogUtils.LogMessage($"Dégâts initiaux : {logString.Remove(logString.Length - 2)} = {damage}", LogUtils.LogType.Combat);
        logString = "";

        if (target is NwCreature targetCreature)
        {
          damage = HandleSpellEvasion(targetCreature, damage, spellEntry.savingThrowAbility, saveResult, spell.Id, spellLevel);
          damage = ItemUtils.GetShieldMasterReducedDamage(targetCreature, damage, saveResult, spellEntry.savingThrowAbility);
          damage = WizardUtils.GetAbjurationReducedDamage(targetCreature, damage);
          damage = PaladinUtils.GetAuraDeGardeReducedDamage(targetCreature, damage);
          damage = ClercUtils.GetAttenuationElementaireReducedDamage(targetCreature, damage, appliedDamage);
          damage = HandleResistanceBypass(targetCreature, isElementalist, isEvocateurSurcharge, damage, appliedDamage);
        }

        EnsoUtils.HandleCoeurDeLaTempete(castingCreature, appliedDamage); 

        if (oCaster is not null)
          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, appliedDamage))));
        else
          target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, appliedDamage)));

        if (!noLogs)
          LogUtils.LogMessage($"Dégâts sur {target.Name} : {nbDices}d{spellEntry.damageDice} (caster lvl {casterLevel}) = {damage} {StringUtils.GetDamageTypeTraduction(appliedDamage)}", LogUtils.LogType.Combat);

        totalDamage += damage;
      }

      WizardUtils.HandleMoissonDuFiel(oCaster, target, moissonDuFielTriggered, spell, spellLevel);
      ClercUtils.HandleIncantationPuissante(oCaster, target, totalDamage, spell);

      return totalDamage;
    }
  }
}
