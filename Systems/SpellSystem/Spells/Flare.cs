﻿using NWN.Core;
using NWN.API;
using NWN.API.Events;
using NWN.API.Constants;

namespace NWN.Systems
{
  class Flare
  {
    public Flare(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell);

      Effect eVis = Effect.VisualEffect(VfxType.ImpFlameS);
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);

      if (oCaster.CheckResistSpell(onSpellCast.TargetObject) == ResistSpellResult.Failed 
        && onSpellCast.TargetObject.RollSavingThrow(SavingThrow.Fortitude, onSpellCast.SaveDC, SavingThrowType.Spell) == SavingThrowResult.Failure)
      {
        Effect eBad = Effect.AttackDecrease(1 + nCasterLevel / 6);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eBad, NwTimeSpan.FromRounds(10 + nCasterLevel));
      }

      if (onSpellCast.MetaMagicFeat == MetaMagic.None)
      {
        oCaster.GetLocalVariable<int>("_AUTO_SPELL").Value = (int)onSpellCast.Spell;
        oCaster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Value = onSpellCast.TargetObject;
        oCaster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        oCaster.OnCombatRoundEnd += PlayerSystem.HandleCombatRoundEndForAutoSpells;

        SpellUtils.CancelCastOnMovement(oCaster);
        SpellUtils.RestoreSpell(oCaster, onSpellCast.Spell);
      }
    }
  }
}
