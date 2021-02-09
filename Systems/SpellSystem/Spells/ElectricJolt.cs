using NWN.Core;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("X0_S0_ElecJolt")]
        private void HandleElectricJolt(CallInfo callInfo)
        {
            var oTarget = (NWScript.GetSpellTargetObject());
            var oCaster = callInfo.ObjectSelf;
            int nCasterLevel = NWScript.GetCasterLevel(oCaster);
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_LIGHTNING_S);
            //Make SR Check
            if (SpellUtils.MyResistSpell(oCaster, oTarget) == 0)
            {
                //Set damage effect
                int iDamage = 3;
                Core.Effect eBad = NWScript.EffectDamage(SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic), NWScript.DAMAGE_TYPE_ELECTRICAL);
                //Apply the VFX impact and damage effect
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eBad, oTarget);
            }

            NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
        }
    }
}
