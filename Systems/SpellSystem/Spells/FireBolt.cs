using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FireBolt(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell);

      Ability spellCastingAbility = oCaster.GetAbilityModifier(Ability.Intelligence) > oCaster.GetAbilityModifier(Ability.Charisma)
            ? Ability.Intelligence : Ability.Charisma;

      switch(SpellUtils.GetSpellAttackRoll(onSpellCast.TargetObject, oCaster, onSpellCast.Spell, spellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); ; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      SpellUtils.DealSpellDamage(onSpellCast.TargetObject, oCaster.CasterLevel, spellEntry, nbDice, oCaster);
    }
  }
}
