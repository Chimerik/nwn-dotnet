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

      switch (SpellUtils.GetSpellAttackRoll(onSpellCast.TargetObject, caster, onSpellCast.Spell, onSpellCast.SpellCastClass.SpellCastingAbility, 0))
      {
        case TouchAttackResult.CriticalHit: SpellUtils.GetCriticalSpellDamageDiceNumber(caster, spellEntry, nbDice); ; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, EffectSystem.noReactions, NwTimeSpan.FromRounds(1)) ;
      onSpellCast.TargetObject.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value = 0;

      SpellUtils.DealSpellDamage(onSpellCast.TargetObject, caster.CasterLevel, spellEntry, nbDice, caster, onSpellCast.Spell.GetSpellLevelForClass(onSpellCast.SpellCastClass));
    }
  }
}
