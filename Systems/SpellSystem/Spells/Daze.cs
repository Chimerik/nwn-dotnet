using NWN.API;
using NWN.Core;
using NWN.Services;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("NW_S0_Daze")]
        private void HandleDaze(CallInfo callInfo)
        {
            var oTarget = (NWScript.GetSpellTargetObject());
            var oCaster = callInfo.ObjectSelf;
            int nCasterLevel = NWScript.GetCasterLevel(oCaster);
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            Core.Effect eMind = NWScript.EffectVisualEffect(NWScript.VFX_DUR_MIND_AFFECTING_NEGATIVE);
            Core.Effect eDaze = NWScript.EffectDazed();
            Core.Effect eDur = NWScript.EffectVisualEffect((NWScript.VFX_DUR_CESSATE_NEGATIVE));

            Core.Effect eLink = NWScript.EffectLinkEffects(eMind, eDaze);
            eLink = NWScript.EffectLinkEffects(eLink, eDur);

            Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_DAZED_S);

            int nDuration = 2;
            //check meta magic for extend
            if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
            {
                nDuration = 4;
            }

            if (NWScript.GetHitDice(oTarget) <= 5 + nCasterLevel / 6)
            {
                //Make SR check
                if (SpellUtils.MyResistSpell(oCaster, oTarget) == 0)
                {
                    //Make Will Save to negate effect
                    if (SpellUtils.MySavingThrow(NWScript.SAVING_THROW_WILL, oTarget, NWScript.GetSpellSaveDC(), NWScript.SAVING_THROW_TYPE_MIND_SPELLS) == 0) // 0 = SAVE FAILED
                    {
                        //Apply VFX Impact and daze effect
                        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.RoundsToSeconds(nDuration));
                        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
                    }
                }
            }
            NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
        }
    }
}
