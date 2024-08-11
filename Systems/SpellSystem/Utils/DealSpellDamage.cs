using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int DealSpellDamage(NwGameObject target, int casterLevel, SpellEntry spellEntry, int nbDices, NwGameObject oCaster, byte spellLevel, SavingThrowResult saveResult = SavingThrowResult.Failure, bool noLogs = false)
    {
      if (saveResult == SavingThrowResult.Immune)
        return 0;

      NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);
      int roll = NwRandom.Roll(Utils.random, spellEntry.damageDice, nbDices);
      int totalDamage = 0;
      bool isEvocateurSurcharge = false;
      bool isFureurDestructrice = oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.FureurDestructriceEffectTag);
      bool moissonDuFielTriggered = target.HP > 0 && target is NwCreature creature && !Utils.In(creature.Race.RacialType, RacialType.Undead, RacialType.Construct);
      int amplifiedDices = 0;

      if (oCaster is NwCreature enso && oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoAmplification))
      {
        amplifiedDices = enso.GetAbilityModifier(Ability.Charisma) < 1 ? 1 : enso.GetAbilityModifier(Ability.Charisma);
        EffectUtils.RemoveTaggedParamEffect(enso, CustomSkill.EnsoAmplification, EffectSystem.MetamagieEffectTag);
      }

      foreach (DamageType damageType in spellEntry.damageType)
      {
        int damage = 0;
        bool isElementalist = false;
       
        if (oCaster is NwCreature caster)
        {
          isElementalist = PlayerSystem.Players.TryGetValue(caster, out PlayerSystem.Player player)
            && player.learnableSkills.TryGetValue(CustomSkill.Elementaliste, out LearnableSkill elementalist)
            && elementalist.featOptions.Any(e => e.Value.Any(d => d == (int)damageType));

          isEvocateurSurcharge = caster.KnowsFeat((Feat)CustomSkill.EvocateurSurcharge) && spell.SpellSchool == SpellSchool.Evocation
            && 0 < spellLevel && spellLevel < 6 && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.EvocateurSurchargeEffectTag);

          for (int i = 0; i < nbDices; i++)
          {
            roll = isEvocateurSurcharge ? spellEntry.damageDice : NwRandom.Roll(Utils.random, spellEntry.damageDice);
            roll = isFureurDestructrice && Utils.In(damageType, DamageType.Electrical, DamageType.Sonic) ? spellEntry.damageDice : NwRandom.Roll(Utils.random, spellEntry.damageDice);

            switch (damageType)
            {
              case DamageType.Piercing:
                if (caster.KnowsFeat((Feat)CustomSkill.Empaleur))
                  roll = roll < 3 ? NwRandom.Roll(Utils.random, spellEntry.damageDice) : roll;
                break;

              case DamageType.Fire:
                if (caster.KnowsFeat((Feat)CustomSkill.FlammesDePhlegetos))
                  roll = roll < 2 ? NwRandom.Roll(Utils.random, spellEntry.damageDice) : roll;
                break;
            }

            roll = isElementalist && roll < 2 ? 2 : roll;

            if(amplifiedDices > 0)
            {
              int tempRoll = NwRandom.Roll(Utils.random, spellEntry.damageDice);
              roll = tempRoll > roll ? tempRoll : roll;
              amplifiedDices -= 1;
            }

            if (caster.KnowsFeat((Feat)CustomSkill.EvocateurSuperieur) && spell.SpellSchool == SpellSchool.Evocation)
            {
              damage += caster.GetAbilityModifier(Ability.Intelligence);
              LogUtils.LogMessage($"Evocation supérieure : ajout de {caster.GetAbilityModifier(Ability.Intelligence)} dégâts au jet", LogUtils.LogType.Combat);
            }

            damage += roll;
          }

          damage = HandleIncantationPuissante(caster, damage, spell);
        }

        LogUtils.LogMessage($"Dégâts initiaux : {damage}", LogUtils.LogType.Combat);

        if (target is NwCreature targetCreature)
        {
          damage = HandleSpellEvasion(targetCreature, damage, spellEntry.savingThrowAbility, saveResult, spell.Id, spellLevel);
          damage = ItemUtils.GetShieldMasterReducedDamage(targetCreature, damage, saveResult, spellEntry.savingThrowAbility);
          damage = WizardUtils.GetAbjurationReducedDamage(targetCreature, damage);
          damage = PaladinUtils.GetAuraDeGardeReducedDamage(targetCreature, damage);
          damage = ClercUtils.GetAttenuationElementaireReducedDamage(targetCreature, damage, damageType);
          damage = HandleResistanceBypass(targetCreature, isElementalist, isEvocateurSurcharge, damage, damageType);
        }

        if (oCaster is not null)
          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, damageType))));
        else
          target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, damageType)));

        if (!noLogs)
          LogUtils.LogMessage($"Dégâts sur {target.Name} : {nbDices}d{spellEntry.damageDice} (caster lvl {casterLevel}) = {damage} {StringUtils.GetDamageTypeTraduction(damageType)}", LogUtils.LogType.Combat);

        totalDamage += damage;
      }

      WizardUtils.HandleMoissonDuFiel(oCaster, target, moissonDuFielTriggered, spell, spellLevel);

      return totalDamage;
    }
  }
}
