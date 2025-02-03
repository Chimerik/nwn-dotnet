using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SouffleDeGuerisonDeGroupe(NwGameObject oCaster, NwGameObject oTarget, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      int bonusHeal = castingClass.ClassType == ClassType.Cleric && caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie)
        ? 2 + spell.GetSpellLevelForClass(castingClass) : 0;

      bool triggerBoon = false;

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry);

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature && !Utils.In(targetCreature.Race.RacialType, RacialType.Undead, RacialType.Construct))
        {
          int healAmount = caster.KnowsFeat((Feat)CustomSkill.ClercGuerisonSupreme)
        ? (spellEntry.damageDice * spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility) + bonusHeal
        : NwRandom.Roll(Utils.random, spellEntry.damageDice, spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility) + bonusHeal;

          if (target != caster)
            triggerBoon = true;

          if (castingClass.ClassType == ClassType.Cleric && caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie))
            healAmount += 2 + spell.GetSpellLevelForClass(castingClass);

          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount)));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));
        }
      }

      if (triggerBoon && castingClass.ClassType == ClassType.Cleric && caster.KnowsFeat((Feat)CustomSkill.ClercGuerriseurBeni))
        NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Instant, Effect.Heal(2 + spell.GetSpellLevelForClass(castingClass))));
    }  
  }
}
