using NWN.Core;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("NW_S0_RayFrost")]
        private void HandleRayOfFrost(CallInfo callInfo)
        {
            var oTarget = (NWScript.GetSpellTargetObject());
            var oCaster = callInfo.ObjectSelf;
            int nCasterLevel = NWScript.GetCasterLevel(oCaster);
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            Core.Effect eDam;
            Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_IMP_FROST_S);
            Core.Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_COLD, oCaster, 0);

            //Make SR Check
            if (SpellUtils.MyResistSpell(oCaster, oTarget) == 0)
            {
                int nDamage = SpellUtils.MaximizeOrEmpower(4, 1 + nCasterLevel / 6, nMetaMagic);
                //Set damage effect
                eDam = NWScript.EffectDamage(nDamage, NWScript.DAMAGE_TYPE_COLD);
                //Apply the VFX impact and damage effect
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eVis, oTarget);
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, eDam, oTarget);
            }

            NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, oTarget, 1.7f);

            NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
        }
    }
}