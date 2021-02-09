using NWN.Core;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("X0_S0_AcidSplash")]
        private void HandleAcidSplash(CallInfo callInfo)
        {
            var oTarget = (NWScript.GetSpellTargetObject());
            var oCaster = callInfo.ObjectSelf;
            int nCasterLevel = NWScript.GetCasterLevel(oCaster);
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            API.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_ACID_S);

            //Make SR Check
            if (SpellUtils.MyResistSpell(oCaster, oTarget) == 0)
            {
                //Set damage effect
                int iDamage = 3;
                int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, nMetaMagic);
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectLinkEffects(eVis, NWScript.EffectDamage(nDamage, NWScript.DAMAGE_TYPE_ACID)), oTarget);
            }
            NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
        }
    }
}
