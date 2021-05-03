using NWN.Core;
using NWN.API;
using NWN.API.Events;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  class Resistance
  {
    public Resistance(SpellEvents.OnSpellCast onSpellCast)
    {
      NwPlayer oCaster = (NwPlayer)onSpellCast.Caster;
      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));

      Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_HEAD_HOLY);
      Core.Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);

      int nBonus = 1 + nCasterLevel / 6; //Saving throw bonus to be applied
      int nDuration = 2 + nCasterLevel / 6; // Turns

      //Check for metamagic extend
      if (onSpellCast.MetaMagicFeat == API.Constants.MetaMagic.Extend)
        nDuration = nDuration * 2;
      //Set the bonus save effect
      Core.Effect eSave = NWScript.EffectSavingThrowIncrease(NWScript.SAVING_THROW_ALL, nBonus);
      Core.Effect eLink = NWScript.EffectLinkEffects(eSave, eDur);

      //Apply the bonus effect and VFX impact
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, onSpellCast.TargetObject, NWScript.TurnsToSeconds(nDuration));
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, onSpellCast.TargetObject);

      if (onSpellCast.MetaMagicFeat == API.Constants.MetaMagic.None)
      {
        Task waitSpellUsed = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          SpellSystem.RestoreSpell(oCaster, onSpellCast.Spell);
        });
      }
    }
  }
}
