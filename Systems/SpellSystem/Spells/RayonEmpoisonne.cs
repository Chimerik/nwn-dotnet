﻿using Anvil.API;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RayonEmpoisonne(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        if (target is not NwCreature targetCreature)
          continue;

        int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

        switch (SpellUtils.GetSpellAttackRoll(target, oCaster, spell, casterClass.SpellCastingAbility))
        {
          case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
          case TouchAttackResult.Hit: break;
          default:
            target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamDisintegrate, oCaster, BodyNode.Hand, true), TimeSpan.FromSeconds(1.7)); 
            continue;
        }

        target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamDisintegrate, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPoisonS));
        EffectSystem.ApplyPoison(targetCreature, caster, NwTimeSpan.FromRounds(spellEntry.duration), spellEntry.savingThrowAbility);  
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(casterClass));
      }
    }
  }
}
