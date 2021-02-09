using NWN.Core;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("NW_S0_Resis")]
        private void HandleResistance(CallInfo callInfo)
        {
            var oTarget = NWScript.GetSpellTargetObject();
            var oCaster = callInfo.ObjectSelf;
            int nCasterLevel = NWScript.GetCasterLevel(oCaster);
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_HEAD_HOLY);
            Core.Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);

            int nBonus = 1 + nCasterLevel / 6; //Saving throw bonus to be applied
            int nDuration = 2 + nCasterLevel / 6; // Turns

            //Check for metamagic extend
            if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
                nDuration = nDuration * 2;
            //Set the bonus save effect
            Core.Effect eSave = NWScript.EffectSavingThrowIncrease(NWScript.SAVING_THROW_ALL, nBonus);
            Core.Effect eLink = NWScript.EffectLinkEffects(eSave, eDur);

            //Apply the bonus effect and VFX impact
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.TurnsToSeconds(nDuration));
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);

            NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
        }
    }
}