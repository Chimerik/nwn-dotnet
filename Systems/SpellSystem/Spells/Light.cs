using NWN.Core;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("NW_S0_Light")]
        private void HandleLight(CallInfo callInfo)
        {
            var oTarget = (NWScript.GetSpellTargetObject());
            var oCaster = callInfo.ObjectSelf;
            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(oCaster, NWScript.GetSpellId()));
            int nMetaMagic = NWScript.GetMetaMagicFeat();

            if (NWScript.GetObjectType(oTarget) == NWScript.OBJECT_TYPE_ITEM)
            {
                // Do not allow casting on not equippable items
                if (!ItemUtils.GetIsItemEquipable(oTarget))
                    NWScript.FloatingTextStrRefOnCreature(83326, oCaster);
                else
                {
                    Core.ItemProperty ip = NWScript.ItemPropertyLight(NWScript.IP_CONST_LIGHTBRIGHTNESS_NORMAL, NWScript.IP_CONST_LIGHTCOLOR_WHITE);

                    if (NWScript.GetItemHasItemProperty(oTarget, NWScript.ITEM_PROPERTY_LIGHT) == 1)
                      ItemUtils.RemoveMatchingItemProperties(oTarget, NWScript.ITEM_PROPERTY_LIGHT, NWScript.DURATION_TYPE_TEMPORARY);

                    int nDuration = NWScript.GetCasterLevel(oCaster);
                    //Enter Metamagic conditions
                    if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
                        nDuration = nDuration * 2; //Duration is +100%

                    NWScript.AddItemProperty(NWScript.DURATION_TYPE_TEMPORARY, ip, oTarget, NWScript.HoursToSeconds(nDuration));
                }
            }
            else
            {
                Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_DUR_LIGHT_WHITE_20);
                Core.Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
                Core.Effect eLink = NWScript.EffectLinkEffects(eVis, eDur);

                int nDuration = NWScript.GetCasterLevel(oCaster);
                //Enter Metamagic conditions
                if (nMetaMagic == NWScript.METAMAGIC_EXTEND)
                    nDuration = nDuration * 2; //Duration is +100%

                //Apply the VFX impact and effects
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, oTarget, NWScript.HoursToSeconds(nDuration));

                NWScript.DelayCommand(0.2f, () => RestoreSpell(oCaster, NWScript.GetSpellId()));
            }
        }
    }
}
