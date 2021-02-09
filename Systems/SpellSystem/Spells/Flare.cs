using NWN.Core;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("X0_S0_Flare")]
        private void HandleFlare(CallInfo callInfo)
        {
            var oTarget = (NWScript.GetSpellTargetObject());
            var oCaster = callInfo.ObjectSelf;
            int nCasterLevel = NWScript.GetCasterLevel(oCaster);
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_FLAME_S);

            // * Apply the hit effect so player knows something happened
            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);

            //Make SR Check
            if ((SpellUtils.MyResistSpell(oCaster, oTarget)) == 0 && SpellUtils.MySavingThrow(NWScript.SAVING_THROW_FORT, oTarget, NWScript.GetSpellSaveDC()) == 0) // 0 = failed
            {
                //Set damage effect
                Core.Effect eBad = NWScript.EffectAttackDecrease(1 + nCasterLevel / 6);
                //Apply the VFX impact and damage effect
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eBad, oTarget, NWScript.RoundsToSeconds(10 + 10 * nCasterLevel / 6));
            }

            NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
        }
    }
}