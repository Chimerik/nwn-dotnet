using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipApplyProtectionStyle(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem shield = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if (onEquip.Slot != InventorySlot.LeftHand || oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionStyleAuraEffectTag))
        return;

      switch(shield.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield: NWScript.AssignCommand(oPC, () => oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.protectionStyleAura)); break;
      }
    }
    public static void OnUnEquipRemoveProtectionStyle(OnItemUnequip onUnEquip)
    {
      NwCreature oPC = onUnEquip.Creature;
      NwItem shield = onUnEquip.Item;

      if (oPC.GetSlotFromItem(shield) != EquipmentSlots.LeftHand || !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionStyleAuraEffectTag))
        return;

      EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ProtectionStyleAuraEffectTag);
    }
  }
}
