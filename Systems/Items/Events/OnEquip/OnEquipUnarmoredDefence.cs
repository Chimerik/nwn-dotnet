using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipUnarmoredDefence(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || onEquip.Slot != InventorySlot.Chest)
        return;

      if (oItem is null || oItem.BaseACValue < 1)
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;

        if (oPC.GetAbilityModifier(Ability.Constitution) > 0)
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetUnarmoredDefenseEffect(oPC.GetAbilityModifier(Ability.Constitution)));
      }
    }
  }
}
