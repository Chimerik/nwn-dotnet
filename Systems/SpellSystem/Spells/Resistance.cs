using NWN.Core;
using NWN.API;
using NWN.API.Events;

namespace NWN.Systems
{
  class Resistance
  {
    public Resistance(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));

      Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_HEAD_HOLY);
      Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);

      int nBonus = 1 + nCasterLevel / 6; //Saving throw bonus to be applied
      int nDuration = 2 + nCasterLevel / 6; // Turns

      //Check for metamagic extend
      if (onSpellCast.MetaMagicFeat == API.Constants.MetaMagic.Extend)
        nDuration = nDuration * 2;
      //Set the bonus save effect
      Effect eSave = NWScript.EffectSavingThrowIncrease(NWScript.SAVING_THROW_ALL, nBonus);
      Effect eLink = NWScript.EffectLinkEffects(eSave, eDur);

      //Apply the bonus effect and VFX impact
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, onSpellCast.TargetObject, NWScript.TurnsToSeconds(nDuration));
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, onSpellCast.TargetObject);

      if (onSpellCast.MetaMagicFeat == API.Constants.MetaMagic.None)
      {
        SpellUtils.RestoreSpell(oCaster, onSpellCast.Spell);
      }
    }
  }
}
