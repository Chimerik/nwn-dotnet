using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class NoAccessory
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipAccessoryMalus;
      oTarget.OnItemValidateUse -= NoUseAccessoryMalus;
      oTarget.OnItemValidateEquip += NoEquipAccessoryMalus;
      oTarget.OnItemValidateUse += NoUseAccessoryMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      if (oTarget.GetItemInSlot(InventorySlot.Head) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.Head));
      if (oTarget.GetItemInSlot(InventorySlot.Boots) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.Boots));
      if (oTarget.GetItemInSlot(InventorySlot.Cloak) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.Cloak));
      if (oTarget.GetItemInSlot(InventorySlot.RightRing) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.RightRing));
      if (oTarget.GetItemInSlot(InventorySlot.LeftRing) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.LeftRing));
      if (oTarget.GetItemInSlot(InventorySlot.Belt) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.Belt));
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipAccessoryMalus;
      oTarget.OnItemValidateUse -= NoUseAccessoryMalus;
    }
    private static void NoEquipAccessoryMalus(OnItemValidateEquip onItemValidateEquip)
    {
      switch(onItemValidateEquip.Slot)
      {
        case InventorySlot.Head:
        case InventorySlot.Belt:
        case InventorySlot.Boots:
        case InventorySlot.Cloak:
        case InventorySlot.LeftRing:
        case InventorySlot.RightRing:
          onItemValidateEquip.Result = EquipValidationResult.Denied;

          if (onItemValidateEquip.UsedBy.IsPlayerControlled)
            onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage("L'interdiction de port d'accessoire est en vigueur.", ColorConstants.Red);
          break;
      }
    }
    private static void NoUseAccessoryMalus(OnItemValidateUse onItemValidateUse)
    {
      switch (onItemValidateUse.Item.BaseItem.ItemType)
      {
        case BaseItemType.Belt:
        case BaseItemType.Helmet:
        case BaseItemType.Boots:
        case BaseItemType.Ring:
        case BaseItemType.Cloak:
          onItemValidateUse.CanUse = false;
          break;
      }
    }
  }
}
