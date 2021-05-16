using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class NoArmor
  {
    public NoArmor(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipArmorMalus;
      oTarget.OnItemValidateUse -= NoUseArmorMalus;
      oTarget.OnItemValidateEquip += NoEquipArmorMalus;
      oTarget.OnItemValidateUse += NoUseArmorMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Chest));
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipArmorMalus;
      oTarget.OnItemValidateUse -= NoUseArmorMalus;
    }
    private void NoEquipArmorMalus(OnItemValidateEquip onItemValidateEquip)
    {
      if (onItemValidateEquip.Slot == InventorySlot.Chest)
      {
        onItemValidateEquip.Result = EquipValidationResult.Denied;
        ((NwPlayer)onItemValidateEquip.UsedBy).SendServerMessage("L'interdiction de port d'armure est en vigueur.", Color.RED);
      }
    }
    private void NoUseArmorMalus(OnItemValidateUse onItemValidateUse)
    {
      if (onItemValidateUse.Item.BaseItemType == BaseItemType.Armor)
        onItemValidateUse.CanUse = false;
    }
  }
}
