using NWN.Core;
using NWN.API;
using NWN.API.Events;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  class Virtue
  {
    public Virtue(SpellEvents.OnSpellCast onSpellCast)
    {
      NwPlayer oCaster = (NwPlayer)onSpellCast.Caster;
      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));

      int nDuration = nCasterLevel;
      Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_HOLY_AID);
      Core.Effect eHP = NWScript.EffectTemporaryHitpoints(1);
      Core.Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
      Core.Effect eLink = NWScript.EffectLinkEffects(eHP, eDur);

      //Enter Metamagic conditions
      if (onSpellCast.MetaMagicFeat == API.Constants.MetaMagic.Extend)
        nDuration = nDuration * 2; //Duration is +100%

      //Apply the VFX impact and effects
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, onSpellCast.TargetObject);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, onSpellCast.TargetObject, NWScript.TurnsToSeconds(nDuration));

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
