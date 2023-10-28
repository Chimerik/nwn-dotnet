using Anvil.API;
using System;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FireBolt(SpellEvents.OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamFire, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell);

      Ability spellCastingAbility = oCaster.GetAbilityModifier(Ability.Intelligence) > oCaster.GetAbilityModifier(Ability.Charisma)
            ? Ability.Intelligence : Ability.Charisma;

      switch(SpellUtils.GetSpellAttackRoll(onSpellCast, oCaster, spellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: nbDice *= 2; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      SpellUtils.DealSpellDamage(onSpellCast.TargetObject, oCaster.LastSpellCasterLevel, Spells2da.spellTable[onSpellCast.Spell.Id].damageDice, nbDice, DamageType.Fire, VfxType.ImpFlameS);
    }
  }
}
