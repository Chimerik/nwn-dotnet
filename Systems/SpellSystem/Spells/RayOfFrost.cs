using Anvil.API;
using System;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RayOfFrost(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFrostS));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell);

      switch(SpellUtils.GetSpellAttackRoll(onSpellCast.TargetObject, oCaster, onSpellCast.Spell, onSpellCast.SpellCastClass.SpellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(30), NwTimeSpan.FromRounds(spellEntry.duration));
      SpellUtils.DealSpellDamage(onSpellCast.TargetObject, oCaster.CasterLevel, spellEntry, nbDice, oCaster, onSpellCast.Spell.GetSpellLevelForClass(onSpellCast.SpellCastClass));
    }
  }
}
