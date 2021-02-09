using NWN.Core;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("NW_S0_Virtue")]
        private void HandleVirtue(CallInfo callInfo)
        {
            var oTarget = (NWScript.GetSpellTargetObject());
            var oCaster = callInfo.ObjectSelf;
            int nCasterLevel = NWScript.GetCasterLevel(oCaster);
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            int nDuration = nCasterLevel;
            Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_HOLY_AID);
            Core.Effect eHP = NWScript.EffectTemporaryHitpoints(1);
            Core.Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
            Core.Effect eLink = NWScript.EffectLinkEffects(eHP, eDur);

            //Enter Metamagic conditions
            if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
                nDuration = nDuration * 2; //Duration is +100%

            //Apply the VFX impact and effects
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.TurnsToSeconds(nDuration));

            NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
        }
    }
}