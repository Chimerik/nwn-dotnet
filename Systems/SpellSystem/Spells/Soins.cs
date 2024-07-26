using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Soins(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target || Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct))
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int healAmount = caster.KnowsFeat((Feat)CustomSkill.ClercGuerisonSupreme)
        ? (spellEntry.damageDice * spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility)
        : NwRandom.Roll(Utils.random, spellEntry.damageDice, spellEntry.numDice) + caster.GetAbilityModifier(castingClass.SpellCastingAbility);

      if (castingClass.ClassType == ClassType.Cleric)
      {
        if (caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie))
          healAmount += 2 + spell.GetSpellLevelForClass(castingClass);

        if (caster.KnowsFeat((Feat)CustomSkill.ClercGuerriseurBeni) && oCaster != oTarget)
          NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Instant, Effect.Heal(2 * spell.GetSpellLevelForClass(castingClass))));
      }

      NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount)));
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));
    }  
  }
}
