using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

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
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Head));
      if (oTarget.GetItemInSlot(InventorySlot.Boots) != null)
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Boots));
      if (oTarget.GetItemInSlot(InventorySlot.Cloak) != null)
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Cloak));
      if (oTarget.GetItemInSlot(InventorySlot.RightRing) != null)
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.RightRing));
      if (oTarget.GetItemInSlot(InventorySlot.LeftRing) != null)
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.LeftRing));
      if (oTarget.GetItemInSlot(InventorySlot.Belt) != null)
        CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Belt));
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
            onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage("L'interdiction de port d'accessoire est en vigueur.", Color.RED);
          break;
      }
    }
    private static void NoUseAccessoryMalus(OnItemValidateUse onItemValidateUse)
    {
      switch (onItemValidateUse.Item.BaseItemType)
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
