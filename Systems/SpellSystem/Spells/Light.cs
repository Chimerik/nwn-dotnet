using NWN.Core;
using NWN.API;
using NWN.API.Events;
using NWN.API.Constants;

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
      
      if(onSpellCast.TargetObject is NwItem item)
      {
        // Do not allow casting on not equippable items
        if (!ItemUtils.GetIsItemEquipable(onSpellCast.TargetObject))
          oCaster.ControllingPlayer.FloatingTextStrRef(83326);
        else
        {
          ItemUtils.RemoveMatchingItemProperties(item, ItemPropertyType.Light, EffectDuration.Temporary);

          int nDuration = NWScript.GetCasterLevel(oCaster);
          //Enter Metamagic conditions
          if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
            nDuration = nDuration * 2; //Duration is +100%

          item.AddItemProperty(ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.White), EffectDuration.Temporary, NwTimeSpan.FromHours(nDuration));
        }
      }
      else
      {
        Effect eVis = Effect.VisualEffect(VfxType.DurLightWhite20) ;
        Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
        Effect eLink = Effect.LinkEffects(eVis, eDur);

        int nDuration = nCasterLevel;
        //Enter Metamagic conditions
        if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
          nDuration = nDuration * 2; //Duration is +100%

        //Apply the VFX impact and effects
        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, onSpellCast.TargetObject, NWScript.HoursToSeconds(nDuration));

        if (onSpellCast.MetaMagicFeat == MetaMagic.None)
        {
          SpellUtils.RestoreSpell(oCaster, onSpellCast.Spell);
        }
      }
    }
  }
}
