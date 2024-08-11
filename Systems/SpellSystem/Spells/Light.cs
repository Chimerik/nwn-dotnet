using Anvil.API;
using NWN.Core;
namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Light(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType, false);

      if (oTarget is NwItem item)
      {
        // Do not allow casting on not equippable items

        if (item.BaseItem.EquipmentSlots == EquipmentSlots.None && oCaster is NwCreature casterCreature)
          casterCreature?.ControllingPlayer.FloatingTextStrRef(83326);
        else
        {
          ItemUtils.RemoveMatchingItemProperties(item, ItemPropertyType.Light, EffectDuration.Temporary);
          item.AddItemProperty(ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.White), EffectDuration.Temporary, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }
      else if (oTarget is NwCreature targetCreature)
      {
        if (oCaster is NwCreature caster && targetCreature.IsReactionTypeHostile(caster))
        {
          int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, casterClass.SpellCastingAbility);
          
          if (CreatureUtils.GetSavingThrow(oCaster, targetCreature, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
            ApplyLightEffect(caster, targetCreature, spellEntry);
        }
        else
          ApplyLightEffect(oCaster, targetCreature, spellEntry);
      }

      NwGameObject previousTarget = oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_PREVIOUS_LIGHT_TARGET").Value;
      oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_PREVIOUS_LIGHT_TARGET").Value = oTarget;

      if(previousTarget is not null && previousTarget.IsValid)
      {
        if (previousTarget is NwItem previousItem)
        {
          foreach (var ip in previousItem.ItemProperties)
            if (ip.Property.PropertyType == ItemPropertyType.Light && ip.DurationType == EffectDuration.Temporary && ip.Creator == oCaster)
              previousItem.RemoveItemProperty(ip);
        }
        else if (previousTarget is NwCreature previousCreature)
        {
          foreach (var eff in previousCreature.ActiveEffects)
            if (eff.Spell.SpellType == Spell.Light && eff.Creator == oCaster)
              previousCreature.RemoveEffect(eff);
        }
      }
    }
    public static void ApplyLightEffect(NwGameObject caster, NwCreature target, SpellEntry spellEntry)
    {
      Effect eLink = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurLightWhite20));
      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, eLink, SpellUtils.GetSpellDuration(caster, spellEntry)));
    }
  }
}
