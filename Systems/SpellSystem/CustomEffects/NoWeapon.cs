using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

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
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.RightHand));
      if (oTarget.GetItemInSlot(InventorySlot.LeftHand) != null)
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.LeftHand));
      if (oTarget.GetItemInSlot(InventorySlot.Arms) != null)
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Arms));
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
            onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage("L'interdiction de port d'arme est en vigueur.", Color.RED);
          break;
      }
    }
    private static void NoUseWeaponMalus(OnItemValidateUse onItemValidateUse)
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
