using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Guerison(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target || Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct))
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int healAmount = spellEntry.damageDice + caster.GetAbilityModifier(castingClass.SpellCastingAbility);

      if (castingClass.ClassType == ClassType.Cleric)
      { 
        if(caster.KnowsFeat((Feat)CustomSkill.ClercDiscipleDeLaVie))
          healAmount += 2 + spell.GetSpellLevelForClass(castingClass);

        if (caster.KnowsFeat((Feat)CustomSkill.ClercGuerriseurBeni) && oCaster != oTarget)
          NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Instant, Effect.Heal(2 * spell.GetSpellLevelForClass(castingClass))));
      }

      NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount)));
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));

      EffectUtils.RemoveEffectType(target, EffectType.Blindness, EffectType.Disease, EffectType.Deaf);

      
    }  
  }
}
