using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipApplyAmbiMaster(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      if (ItemUtils.IsMeleeWeapon(onEquip.Player.GetItemInSlot(InventorySlot.RightHand)?.BaseItem)
        && ItemUtils.IsMeleeWeapon(onEquip.Player.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem)
        && !onEquip.Player.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        onEquip.Player.ApplyEffect(EffectDuration.Permanent, EffectSystem.ambiMaster);
    }
    public static void OnUnEquipRemoveAmbiMaster(ModuleEvents.OnPlayerUnequipItem onUnEquip)
    {
      NwCreature oPC = onUnEquip.UnequippedBy;

      if (!ItemUtils.IsMeleeWeapon(oPC.GetItemInSlot(InventorySlot.RightHand)?.BaseItem)
        || !ItemUtils.IsMeleeWeapon(oPC.GetItemInSlot(InventorySlot.LeftHand)?.BaseItem))
        EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.AmbiMasterEffectTag);
    }
  }
}
