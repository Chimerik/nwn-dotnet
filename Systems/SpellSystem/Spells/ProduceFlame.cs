using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ProduceFlame(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if(oTarget is not NwCreature target || target == oCaster)
      {
        if (oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.ProduceFlameEffectTag))
          EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.ProduceFlameEffectTag);
        else
          oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.produceFlameEffect, SpellUtils.GetSpellDuration(oCaster, spellEntry));

        return;
      }

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      Ability spellCastAbility = castingClass is null ? Ability.Charisma : castingClass.SpellCastingAbility;

      switch (SpellUtils.GetSpellAttackRoll(oTarget, oCaster, spell, spellCastAbility))
      {
        case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      SpellUtils.DealSpellDamage(oTarget, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(ClassType.Druid));

      if (oCaster.ActiveEffects.Any(e => e.Tag == EffectSystem.ProduceFlameEffectTag))
        EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.ProduceFlameEffectTag);
    }
  }
}
