using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipApplyAmbiMaster(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      if (onEquip.Slot != InventorySlot.LeftHand || !ItemUtils.IsMeleeWeapon(onEquip.Item.BaseItem) 
        || onEquip.Player.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        return;

      onEquip.Player.ApplyEffect(EffectDuration.Permanent, EffectSystem.ambiMaster);
    }
    public static void OnUnEquipRemoveAmbiMaster(ModuleEvents.OnPlayerUnequipItem onUnEquip)
    {
      NwCreature oPC = onUnEquip.UnequippedBy;
      NwItem offhandWeapon = onUnEquip.Item;

      if (oPC.GetSlotFromItem(offhandWeapon) != EquipmentSlots.LeftHand || !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        return;

      EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.AmbiMasterEffectTag);
    }
  }
}
