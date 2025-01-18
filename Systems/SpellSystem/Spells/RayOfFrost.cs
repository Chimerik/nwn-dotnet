using Anvil.API;
using NWN.Core;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RayOfFrost(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, casterClass.SpellCastingAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
          case TouchAttackResult.Hit: break;
          default:
            NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand, true), TimeSpan.FromSeconds(1.7)));
            continue;
        }

        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7)));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFrostS));
        target.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(30), NwTimeSpan.FromRounds(spellEntry.duration));
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(casterClass));
      }
    }
  }
}
