using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void DealSpellDamage(NwGameObject target, int casterLevel, SpellEntry spellEntry, int nbDices, NwCreature caster, byte spellLevel, bool saveFailed = true, bool noLogs = false)
    {
      NwSpell spell = NwSpell.FromSpellId(spellEntry.RowIndex);

      bool isElementalist = PlayerSystem.Players.TryGetValue(caster, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.Elementaliste, out LearnableSkill elementalist)
        && elementalist.featOptions.Any(e => e.Value.Any(d => d == (int)spellEntry.damageType));

      bool isEvocateurSurcharge = caster.KnowsFeat((Feat)CustomSkill.EvocateurSurcharge) && spell.SpellSchool == SpellSchool.Evocation
        && spellLevel > 0 && spellLevel < 6 && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.EvocateurSurchargeEffectTag);
      
      int damage = 0;

      for(int i = 0; i < nbDices; i++) 
      {
        int roll = isEvocateurSurcharge ? spellEntry.damageDice : NwRandom.Roll(Utils.random, spellEntry.damageDice);

        switch(spellEntry.damageType)
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

        if (caster.KnowsFeat((Feat)CustomSkill.EvocateurSuperieur) && spell.SpellSchool == SpellSchool.Evocation)
        {
          damage += caster.GetAbilityModifier(Ability.Intelligence);
          LogUtils.LogMessage($"Evocation supérieure : ajout de {caster.GetAbilityModifier(Ability.Intelligence)} dégâts au jet", LogUtils.LogType.Combat);
        }

        damage += roll;
      }

      damage = ItemUtils.GetShieldMasterReducedDamage(target, damage, saveFailed, spellEntry.savingThrowAbility);
      damage = WizardUtils.GetAbjurationReducedDamage(target, damage);
      damage /= saveFailed ? 1 : 2;

      //int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);

      damage = HandleResistanceBypass(target, isElementalist, damage, spellEntry);

      if (caster is not null)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, spellEntry.damageType))));
      else
        target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, spellEntry.damageType)));
      
      if(!noLogs)
        LogUtils.LogMessage($"Dégâts sur {target.Name} : {nbDices}d{spellEntry.damageDice} (caster lvl {casterLevel}) = {damage} {spellEntry.damageType}", LogUtils.LogType.Combat);

      if (isEvocateurSurcharge)
        WizardUtils.HandleEvocateurSurchargeSelfDamage(caster, spellLevel);
    }
  }
}
