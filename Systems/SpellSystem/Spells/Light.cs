using NWN.Core;
using NWN.API;
using NWN.API.Events;
using NWN.API.Constants;
using System.Threading.Tasks;
using System;

namespace NWN.Systems
{
  class Light
  {
    public Light(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, (int)onSpellCast.Spell));
      
      if(onSpellCast.TargetObject is NwItem)
      {
        // Do not allow casting on not equippable items
        if (!ItemUtils.GetIsItemEquipable(onSpellCast.TargetObject))
          oCaster.ControllingPlayer.FloatingTextStrRef(83326);
        else
        {
          Core.ItemProperty ip = NWScript.ItemPropertyLight(NWScript.IP_CONST_LIGHTBRIGHTNESS_NORMAL, NWScript.IP_CONST_LIGHTCOLOR_WHITE);

          if (NWScript.GetItemHasItemProperty(onSpellCast.TargetObject, NWScript.ITEM_PROPERTY_LIGHT) == 1)
            ItemUtils.RemoveMatchingItemProperties(onSpellCast.TargetObject, NWScript.ITEM_PROPERTY_LIGHT, NWScript.DURATION_TYPE_TEMPORARY);

          int nDuration = NWScript.GetCasterLevel(oCaster);
          //Enter Metamagic conditions
          if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
            nDuration = nDuration * 2; //Duration is +100%

          NWScript.AddItemProperty(NWScript.DURATION_TYPE_TEMPORARY, ip, onSpellCast.TargetObject, NWScript.HoursToSeconds(nDuration));
        }
      }
      else
      {
        Core.Effect eVis = NWScript.EffectVisualEffect(NWScript.VFX_DUR_LIGHT_WHITE_20);
        Core.Effect eDur = NWScript.EffectVisualEffect(NWScript.VFX_DUR_CESSATE_POSITIVE);
        Core.Effect eLink = NWScript.EffectLinkEffects(eVis, eDur);

        int nDuration = nCasterLevel;
        //Enter Metamagic conditions
        if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
          nDuration = nDuration * 2; //Duration is +100%

        //Apply the VFX impact and effects
        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, onSpellCast.TargetObject, NWScript.HoursToSeconds(nDuration));

        if (onSpellCast.MetaMagicFeat == MetaMagic.None)
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
}
