using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipApplyAmbiMaster(OnItemEquip onEquip)
    {
      if (onEquip.Slot != InventorySlot.LeftHand || !ItemUtils.IsWeapon(onEquip.Item.BaseItem) 
        || onEquip.EquippedBy.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        return;

      onEquip.EquippedBy.ApplyEffect(EffectDuration.Permanent, EffectSystem.ambiMaster);
    }
    public static void OnUnEquipRemoveAmbiMaster(OnItemUnequip onUnEquip)
    {
      NwCreature oPC = onUnEquip.Creature;
      NwItem offhandWeapon = onUnEquip.Item;

      if (oPC.GetSlotFromItem(offhandWeapon) != EquipmentSlots.LeftHand || !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        return;

      EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.AmbiMasterEffectTag);
    }
  }
}
