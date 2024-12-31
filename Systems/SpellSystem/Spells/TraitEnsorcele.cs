using Anvil.API;
using NWN.Core;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> TraitEnsorcele(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.TraitEnsorcele, SpellUtils.GetSpellDuration(oCaster, spellEntry)));

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, casterClass.SpellCastingAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
          case TouchAttackResult.Hit: break;
          default:
            target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamCold, oCaster, BodyNode.Hand, true), TimeSpan.FromSeconds(1.7));
            continue;
        }

        target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamLightning, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(casterClass));
      }

      return targets;
    }
  }
}
