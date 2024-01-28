using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipUnarmoredDefence(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null)
        return;

      if(oItem.BaseItem.ItemType == BaseItemType.Armor && (oPC.GetItemInSlot(InventorySlot.Chest) is null
        || oPC.GetItemInSlot(InventorySlot.Chest).BaseACValue < 1))
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;

        if (oPC.GetAbilityModifier(Ability.Constitution) > 0)
          oPC.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetUnarmoredDefenseEffect(oPC.GetAbilityModifier(Ability.Constitution)), NwTimeSpan.FromRounds(1));
      }
    }
  }
}
