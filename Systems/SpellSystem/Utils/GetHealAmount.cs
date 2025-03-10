using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetHealAmount(NwCreature caster, NwCreature target, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, int nbDices)
    {
      int healAmount = caster.KnowsFeat((Feat)CustomSkill.ClercGuerisonSupreme) || target.ActiveEffects.Any(e => e.Tag == EffectSystem.LueurDespoirEffectTag)
        ? (spellEntry.damageDice * nbDices) + caster.GetAbilityModifier(castingClass.SpellCastingAbility)
        : HandleHealerFeat(caster, spellEntry.damageDice, nbDices);

      if (castingClass.ClassType == ClassType.Cleric)
      {
        if (caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie))
          healAmount += 2 + spell.InnateSpellLevel;

        if (caster.KnowsFeat((Feat)CustomSkill.ClercGuerriseurBeni) && caster != target)
          NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Instant, Effect.Heal(2 + spell.GetSpellLevelForClass(castingClass))));
      }

      return healAmount;
    }
  }
}
