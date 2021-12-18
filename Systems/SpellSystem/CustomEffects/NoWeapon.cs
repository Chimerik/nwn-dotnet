using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class NoWeapon
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipWeaponMalus;
      oTarget.OnItemValidateUse -= NoUseWeaponMalus;
      oTarget.OnItemValidateEquip += NoEquipWeaponMalus;
      oTarget.OnItemValidateUse += NoUseWeaponMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      if (oTarget.GetItemInSlot(InventorySlot.RightHand) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.RightHand));
      if (oTarget.GetItemInSlot(InventorySlot.LeftHand) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.LeftHand));
      if (oTarget.GetItemInSlot(InventorySlot.Arms) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.Arms));
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipWeaponMalus;
      oTarget.OnItemValidateUse -= NoUseWeaponMalus;
    }
    public static void NoEquipWeaponMalus(OnItemValidateEquip onItemValidateEquip)
    {
      switch(onItemValidateEquip.Slot)
      {
        case InventorySlot.RightHand:
        case InventorySlot.LeftHand:
        case InventorySlot.Arms:
          onItemValidateEquip.Result = EquipValidationResult.Denied;

          if (onItemValidateEquip.UsedBy.IsPlayerControlled)
            onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage("L'interdiction de port d'arme est en vigueur.", ColorConstants.Red);
          break;
      }
    }
    private static void NoUseWeaponMalus(OnItemValidateUse onItemValidateUse)
    {
      switch (ItemUtils.GetItemCategory(onItemValidateUse.Item.BaseItem.ItemType))
      {
        case ItemUtils.ItemCategory.OneHandedMeleeWeapon:
        case ItemUtils.ItemCategory.TwoHandedMeleeWeapon:
        case ItemUtils.ItemCategory.RangedWeapon:
          onItemValidateUse.CanUse = false;
          return;
      }

      switch (onItemValidateUse.Item.BaseItem.ItemType)
      {
        case BaseItemType.Gloves:
        case BaseItemType.Bracer:
          onItemValidateUse.CanUse = false;
          return;
      }
    }
  }
}
