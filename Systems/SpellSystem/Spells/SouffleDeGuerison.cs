using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SouffleDeGuerison(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      int healAmount = caster.KnowsFeat((Feat)CustomSkill.ClercGuerisonSupreme) || oTarget.ActiveEffects.Any(e => e.Tag == EffectSystem.LueurDespoirEffectTag)
        ? (spellEntry.damageDice * spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility)
        : NwRandom.Roll(Utils.random, spellEntry.damageDice, spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility);

      if (castingClass.ClassType == ClassType.Cleric)
      {
        if (caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie))
          healAmount += 2 + spell.GetSpellLevelForClass(castingClass);

        if (caster.KnowsFeat((Feat)CustomSkill.ClercGuerriseurBeni) && oCaster != oTarget)
          NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Instant, Effect.Heal(2 + spell.GetSpellLevelForClass(castingClass))));
      }

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
        {
          NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount)));
          targetCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));
        } 
      }
    }
  }
}
