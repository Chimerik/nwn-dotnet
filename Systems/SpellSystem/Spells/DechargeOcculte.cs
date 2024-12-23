using Anvil.API;
using NWN.Core;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void DechargeOcculte(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        int nbDice = 1;

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, casterClass.SpellCastingAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
          case TouchAttackResult.Hit: break;
          default:
            target.ApplyEffect(EffectDuration.Temporary, NWScript.EffectBeam((int)VfxType.BeamOdd, oCaster, (int)BodyNode.Hand, 1, 2), TimeSpan.FromSeconds(1.7));
            continue;
        }

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpNegativeEnergy));
        target.ApplyEffect(EffectDuration.Temporary, NWScript.EffectBeam((int)VfxType.BeamOdd, oCaster, (int)BodyNode.Hand, 0, 2), TimeSpan.FromSeconds(1.7));

        if (oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.DechargeRepulsive))
          target.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(50), NwTimeSpan.FromRounds(1));

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(casterClass), casterClass:casterClass);
      }
    }
  }
}
