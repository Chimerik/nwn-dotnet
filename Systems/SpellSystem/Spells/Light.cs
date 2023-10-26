using Anvil.API;
using Anvil.API.Events;
using NLog.Targets;
using static Anvil.API.Events.SpellEvents;
using static NWN.Native.API.CVirtualMachineScript.JmpData;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Light(SpellEvents.OnSpellCast onSpellCast)
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
          SpellEntry spellEntry = Spells2da.spellTable[onSpellCast.Spell.Id];
          SpellConfig.SavingThrowFeedback feedback = new();
          int spellDC = SpellUtils.GetCasterSpellDC(oCaster);
          int advantage = 0;
          bool targetHandled = false;

          foreach (var eff in targetCreature.ActiveEffects)
          {
            targetHandled = SpellUtils.HandleSpellTargetIncapacitated(oCaster, targetCreature, eff.EffectType, spellEntry);

            if (targetHandled)
              break;

            advantage += SpellUtils.GetAbilityAdvantageFromEffect(spellEntry.savingThrowAbility, eff.Tag);
          }

          if (!targetHandled)
          {
            int totalSave = SpellUtils.GetSavingThrowRoll(targetCreature, spellEntry, advantage, feedback);
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
