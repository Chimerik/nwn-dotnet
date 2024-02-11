using Anvil.API;
using Anvil.API.Events;
namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Light(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);

      if (onSpellCast.TargetObject is NwItem item)
      {
        // Do not allow casting on not equippable items

        if (item.BaseItem.EquipmentSlots == EquipmentSlots.None)
          oCaster.ControllingPlayer.FloatingTextStrRef(83326);
        else
        {
          ItemUtils.RemoveMatchingItemProperties(item, ItemPropertyType.Light, EffectDuration.Temporary);
          item.AddItemProperty(ItemProperty.Light(IPLightBrightness.Normal, IPLightColor.White), EffectDuration.Temporary, NwTimeSpan.FromHours(1));
        }
      }
      else if (onSpellCast.TargetObject is NwCreature targetCreature)
      {
        if (targetCreature.IsReactionTypeHostile(oCaster))
        {
          SpellConfig.SavingThrowFeedback feedback = new();
          int spellDC = SpellUtils.GetCasterSpellDC(oCaster, onSpellCast.Spell);
          int advantage = CreatureUtils.GetCreatureAbilityAdvantage(targetCreature, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);
          
          if (advantage < -900)
          {
            int totalSave = SpellUtils.GetSavingThrowRoll(targetCreature, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
            bool saveFailed = totalSave < spellDC;

            SpellUtils.SendSavingThrowFeedbackMessage(oCaster, targetCreature, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

            if (saveFailed)
              ApplyLightEffect(targetCreature);
          }
        }
        else
          ApplyLightEffect(targetCreature);
      }

      NwGameObject previousTarget = oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_PREVIOUS_LIGHT_TARGET").Value;
      oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_PREVIOUS_LIGHT_TARGET").Value = onSpellCast.TargetObject;

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
    public static void ApplyLightEffect(NwCreature target)
    {
      Effect eLink = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurLightWhite20), Effect.VisualEffect(VfxType.DurCessatePositive));
      target.ApplyEffect(EffectDuration.Temporary, eLink, NwTimeSpan.FromHours(1));
    }
  }
}
