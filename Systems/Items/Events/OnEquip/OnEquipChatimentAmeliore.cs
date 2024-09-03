using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipChatimentAmeliore(OnItemEquip onEquip)
    {
      if (!onEquip.Item.IsRangedWeapon)
        return;

      EffectUtils.RemoveTaggedEffect(onEquip.EquippedBy, EffectSystem.ChatimentAmelioreEffectTag);
      onEquip.EquippedBy.OnItemEquip -= OnEquipChatimentAmeliore;
      onEquip.EquippedBy.OnItemUnequip -= OnUnequipChatimentAmeliore;
      onEquip.EquippedBy.OnItemUnequip += OnUnequipChatimentAmeliore;
    }
    public static void OnUnequipChatimentAmeliore(OnItemUnequip onUnEquip)
    {
      NwCreature oPC = onUnEquip.Creature;
      NwItem weapon = onUnEquip.Creature.GetItemInSlot(InventorySlot.RightHand);
      
      if (weapon is null || weapon.IsRangedWeapon)
      {
        oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.ChatimentAmeliore(oPC));
        oPC.OnItemUnequip -= OnUnequipChatimentAmeliore;
      }
    }
  }
}
