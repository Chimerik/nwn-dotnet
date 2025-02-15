using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PriereDeGuerison(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster)
        return;

      if(caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Sort non utilisable en combat", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      int bonusHeal = castingClass.ClassType == ClassType.Cleric && caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie) 
        ? 2 + spell.GetSpellLevelForClass(castingClass) : 0;

      bool triggerBoon = false;

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(caster, oTarget, spellEntry);

      foreach (var targetObject in targets)
      {
        if (targetObject is not NwCreature target || Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct))
          continue;

        int healAmount = caster.KnowsFeat((Feat)CustomSkill.ClercGuerisonSupreme) || oTarget.ActiveEffects.Any(e => e.Tag == EffectSystem.LueurDespoirEffectTag)
        ? (spellEntry.damageDice * spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility) + bonusHeal
        : NwRandom.Roll(Utils.random, spellEntry.damageDice, spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility) + bonusHeal;

        if (target != caster)
          triggerBoon = true;

        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount)));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
      }

      if (triggerBoon && castingClass.ClassType == ClassType.Cleric && caster.KnowsFeat((Feat)CustomSkill.ClercGuerriseurBeni))
        NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Instant, Effect.Heal(2 + spell.GetSpellLevelForClass(castingClass))));
    }  
  }
}
