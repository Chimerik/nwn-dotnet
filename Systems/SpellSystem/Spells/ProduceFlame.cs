using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ProduceFlame(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

      if(onSpellCast.TargetObject is not NwCreature target || target == caster)
      {
        if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ProduceFlameEffectTag))
        {
          foreach (var eff in caster.ActiveEffects)
            if (eff.Tag == EffectSystem.ProduceFlameEffectTag)
              caster.RemoveEffect(eff);
        }
        else
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.produceFlameEffect, NwTimeSpan.FromRounds(spellEntry.duration));

        return;
      }

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(caster, onSpellCast.Spell);

      switch(SpellUtils.GetSpellAttackRoll(onSpellCast.TargetObject, caster, onSpellCast.Spell, Ability.Wisdom))
      {
        case TouchAttackResult.CriticalHit: nbDice *= 2; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      SpellUtils.DealSpellDamage(onSpellCast.TargetObject, caster.LastSpellCasterLevel, spellEntry, nbDice);
    }
  }
}
