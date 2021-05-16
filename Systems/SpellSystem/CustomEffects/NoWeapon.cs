using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class NoWeapon
  {
    public NoWeapon(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipWeaponMalus;
      oTarget.OnItemValidateUse -= NoUseWeaponMalus;
      oTarget.OnItemValidateEquip += NoEquipWeaponMalus;
      oTarget.OnItemValidateUse += NoUseWeaponMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.RightHand));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.LeftHand));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Arms));
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipWeaponMalus;
      oTarget.OnItemValidateUse -= NoUseWeaponMalus;
    }
    private void NoEquipWeaponMalus(OnItemValidateEquip onItemValidateEquip)
    {
      switch(onItemValidateEquip.Slot)
      {
        case InventorySlot.RightHand:
        case InventorySlot.LeftHand:
        case InventorySlot.Arms:
          onItemValidateEquip.Result = EquipValidationResult.Denied;
          ((NwPlayer)onItemValidateEquip.UsedBy).SendServerMessage("L'interdiction de port d'arme est en vigueur.", Color.RED);
          break;
      }
    }
    private void NoUseWeaponMalus(OnItemValidateUse onItemValidateUse)
    {
      switch (ItemUtils.GetItemCategory(onItemValidateUse.Item.BaseItemType))
      {
        case ItemUtils.ItemCategory.OneHandedMeleeWeapon:
        case ItemUtils.ItemCategory.TwoHandedMeleeWeapon:
        case ItemUtils.ItemCategory.RangedWeapon:
          onItemValidateUse.CanUse = false;
          return;
      }

      switch (onItemValidateUse.Item.BaseItemType)
      {
        case BaseItemType.Gloves:
        case BaseItemType.Bracer:
          onItemValidateUse.CanUse = false;
          return;
      }
    }
  }
}
