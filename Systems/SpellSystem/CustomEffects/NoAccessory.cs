using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class NoAccessory
  {
    public NoAccessory(NwCreature oTarget, bool apply = true)
    {
      if (apply)
        ApplyEffectToTarget(oTarget);
      else
        RemoveEffectFromTarget(oTarget);
    }
    private void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipAccessoryMalus;
      oTarget.OnItemValidateUse -= NoUseAccessoryMalus;
      oTarget.OnItemValidateEquip += NoEquipAccessoryMalus;
      oTarget.OnItemValidateUse += NoUseAccessoryMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Head));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Boots));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Cloak));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.RightRing));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.LeftRing));
      CreaturePlugin.RunUnequip(oTarget, oTarget.GetItemInSlot(InventorySlot.Belt));
    }
    private void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipAccessoryMalus;
      oTarget.OnItemValidateUse -= NoUseAccessoryMalus;
    }
    private void NoEquipAccessoryMalus(OnItemValidateEquip onItemValidateEquip)
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
          ((NwPlayer)onItemValidateEquip.UsedBy).SendServerMessage("L'interdiction de port d'accessoire est en vigueur.", Color.RED);
          break;
      }
    }
    private void NoUseAccessoryMalus(OnItemValidateUse onItemValidateUse)
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
