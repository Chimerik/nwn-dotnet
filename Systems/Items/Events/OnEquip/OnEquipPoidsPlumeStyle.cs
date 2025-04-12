using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipPoidsPlumeStyle(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      NwCreature oPC = onEquip.Player;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null)
        return;

      var armor = oPC.GetItemInSlot(InventorySlot.Chest);
      var rightWeapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var leftWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((armor is null || !ItemUtils.IsMediumOrHeavyArmor(armor))
        && (rightWeapon is null || ItemUtils.IsLightWeapon(rightWeapon.BaseItem, oPC.Size))
        && (leftWeapon is null || ItemUtils.IsLightWeapon(leftWeapon.BaseItem, oPC.Size)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.PoidsPlumeStyleEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.PoidsPlumeStyle);
      }
      else
        EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.PoidsPlumeStyleEffectTag);
    }

    public static async void OnUnEquipPoidsPlumeStyle(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null)
        return;

      await NwTask.NextFrame();

      var armor = oPC.GetItemInSlot(InventorySlot.Chest);
      var rightWeapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var leftWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((armor is null || !ItemUtils.IsMediumOrHeavyArmor(armor))
        && (rightWeapon is null || ItemUtils.IsLightWeapon(rightWeapon.BaseItem, oPC.Size))
        && (leftWeapon is null || ItemUtils.IsLightWeapon(leftWeapon.BaseItem, oPC.Size)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.PoidsPlumeStyleEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.PoidsPlumeStyle);
      }
      else
        EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.PoidsPlumeStyleEffectTag);
    }
  }
}
