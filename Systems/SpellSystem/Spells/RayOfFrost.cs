using Anvil.API;
using System;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RayOfFrost(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, NwFeat feat = null)
    {
      if (oCaster is NwCreature caster && feat is not null && feat.Id == CustomSkill.MonkFrissonDeLaMontagne)
      {
        caster.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(caster);
        casterClass = NwClass.FromClassId(CustomClass.Monk);
      }

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFrostS));
      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      switch(SpellUtils.GetSpellAttackRoll(oTarget, oCaster, spell, casterClass.SpellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(30), NwTimeSpan.FromRounds(spellEntry.duration));
      SpellUtils.DealSpellDamage(oTarget, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(casterClass));

      
    }
  }
}
