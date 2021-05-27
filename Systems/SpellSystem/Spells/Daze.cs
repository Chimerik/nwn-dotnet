using System;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  class Daze
  {
    public Daze(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));

      Core.Effect eMind = NWScript.EffectVisualEffect(NWScript.VFX_DUR_MIND_AFFECTING_NEGATIVE);
      Core.Effect eDaze = NWScript.EffectDazed();
      Core.Effect eDur = NWScript.EffectVisualEffect((NWScript.VFX_DUR_CESSATE_NEGATIVE));

      Core.Effect eLink = NWScript.EffectLinkEffects(eMind, eDaze);
      eLink = NWScript.EffectLinkEffects(eLink, eDur);

      Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_DAZED_S);

      int nDuration = 2;

      //check meta magic for extend
      if (onSpellCast.MetaMagicFeat == API.Constants.MetaMagic.Extend)
        nDuration = 4;

      if (NWScript.GetHitDice(onSpellCast.TargetObject) <= 5 + nCasterLevel / 6)
      {
        //Make SR check
        if (SpellUtils.MyResistSpell(oCaster, onSpellCast.TargetObject) == 0)
        {
          //Make Will Save to negate effect
          if (SpellUtils.MySavingThrow(NWScript.SAVING_THROW_WILL, onSpellCast.TargetObject, NWScript.GetSpellSaveDC(), NWScript.SAVING_THROW_TYPE_MIND_SPELLS) == 0) // 0 = SAVE FAILED
          {
            //Apply VFX Impact and daze effect
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, onSpellCast.TargetObject, NWScript.RoundsToSeconds(nDuration));
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, onSpellCast.TargetObject);
          }
        }
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
