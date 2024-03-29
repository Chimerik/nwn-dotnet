﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  static class NoArmor
  {
    public static void ApplyEffectToTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipArmorMalus;
      oTarget.OnItemValidateUse -= NoUseArmorMalus;
      oTarget.OnItemValidateEquip += NoEquipArmorMalus;
      oTarget.OnItemValidateUse += NoUseArmorMalus;
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));

      if (oTarget.GetItemInSlot(InventorySlot.Chest) != null)
        oTarget.RunUnequip(oTarget.GetItemInSlot(InventorySlot.Chest));
    }
    public static void RemoveEffectFromTarget(NwCreature oTarget)
    {
      oTarget.OnItemValidateEquip -= NoEquipArmorMalus;
      oTarget.OnItemValidateUse -= NoUseArmorMalus;
      //PlayerSystem.Log.Info($"removed no armor from {oTarget.Name} - oTarget: {oTarget} - LoginCreature: {oTarget.LoginPlayer.LoginCreature} - ControllingPlayer.ControllingCreature: {oTarget.ControllingPlayer.ControlledCreature} - ControllingPlayer.LoginCreature {oTarget.ControllingPlayer.ControlledCreature}");
    }
    private static void NoEquipArmorMalus(OnItemValidateEquip onItemValidateEquip)
    {
      if (onItemValidateEquip.Slot == InventorySlot.Chest)
      {
        onItemValidateEquip.Result = EquipValidationResult.Denied;

        if (onItemValidateEquip.UsedBy.IsPlayerControlled)
          onItemValidateEquip.UsedBy.ControllingPlayer.SendServerMessage("L'interdiction de port d'armure est en vigueur.", ColorConstants.Red);
      }
    }
    private static void NoUseArmorMalus(OnItemValidateUse onItemValidateUse)
    {
      if (onItemValidateUse.Item.BaseItem.ItemType == BaseItemType.Armor)
      {
        onItemValidateUse.CanUse = false;
      }
    }
  }
}
