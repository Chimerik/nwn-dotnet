using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class Light
  {
    public Light(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);

      if (onSpellCast.TargetObject is NwItem item)
      {
        // Do not allow casting on not equippable items

        if (item.BaseItem.EquipmentSlots == EquipmentSlots.None)
          oCaster.ControllingPlayer.FloatingTextStrRef(83326);
        else
        {
          ItemUtils.RemoveMatchingItemProperties(item, ItemPropertyType.Light, EffectDuration.Temporary);

          int nDuration = oCaster.LastSpellCasterLevel;
          //Enter Metamagic conditions
          if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
            nDuration = nDuration * 2; //Duration is +100%

          item.AddItemProperty(ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.White), EffectDuration.Temporary, NwTimeSpan.FromHours(nDuration));
        }
      }
      else
      {
        Effect eVis = Effect.VisualEffect(VfxType.DurLightWhite20);
        Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
        Effect eLink = Effect.LinkEffects(eVis, eDur);

        int nDuration = nCasterLevel;
        //Enter Metamagic conditions
        if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
          nDuration = nDuration * 2; //Duration is +100%

        //Apply the VFX impact and effects
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, eLink, NwTimeSpan.FromHours(nDuration));
      }
    }
  }
}
