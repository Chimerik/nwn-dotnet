using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ElectricJolt(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(caster, onSpellCast.Spell);

      Ability spellCastingAbility = caster.GetAbilityModifier(Ability.Intelligence) > caster.GetAbilityModifier(Ability.Charisma)
        ? Ability.Intelligence : Ability.Charisma;

      switch (SpellUtils.GetSpellAttackRoll(onSpellCast.TargetObject, caster, onSpellCast.Spell, spellCastingAbility, 0))
      {
        case TouchAttackResult.CriticalHit: nbDice *= 2; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.noReactions, NwTimeSpan.FromRounds(1)) ;
      onSpellCast.TargetObject.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value = 0;

      SpellUtils.DealSpellDamage(onSpellCast.TargetObject, caster.LastSpellCasterLevel, spellEntry, nbDice, caster);
    }
  }
}
