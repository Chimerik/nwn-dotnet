using NWN.Core;

namespace NWN.Systems
{
  public static partial class EnchantmentBasinSystem
  {
    private static EnchantmentBasin GetEnchantmentBasinFromTag(string tag)
    {
      switch (tag)
      {
        default:
          return new EnchantmentBasin(
            minSuccessPercent: 5,
            maxSuccessPercent: 95,
            maxAttackBonus: 3,
            maxACBonus: 3,
            maxAbilityBonus: 3,
            maxDamageBonus: NWScript.DAMAGE_BONUS_1d12,
            maxSavingThrowBonus: 3,
            maxRegenBonus: 2
          );

        case "enchantment_basin_expensive":
          return new EnchantmentBasin(
            costRate: 2,
            minSuccessPercent: 5,
            maxSuccessPercent: 95,
            maxCostRateForSuccessRateMin: 500000,
            maxAttackBonus: 5,
            maxACBonus: 5,
            maxAbilityBonus: 6,
            maxDamageBonus: NWScript.DAMAGE_BONUS_1d8,
            maxSavingThrowBonus: 4,
            maxRegenBonus: 4
          );
      }
    }

    public static int HandleClose(uint oid)
    {
      var oPC = NWScript.GetLastClosedBy();
      PlayerSystem.Player player;

      if (!PlayerSystem.Players.TryGetValue(oPC, out player))
      {
        NWScript.SendMessageToPC(oPC, "Player is not valid.");
        return 0;
      }

      var oItem = NWScript.GetFirstItemInInventory(oid);

      if (oItem == NWScript.OBJECT_INVALID)
      {
        NWScript.SendMessageToPC(oPC, "Item is not valid.");
        return 0;
      }

      if (!ItemUtils.IsEquipable(oItem))
      {
        NWScript.SendMessageToPC(oPC, "Item is not equipable.");
        return 0;
      }

      if (NWScript.GetPlotFlag(oItem) == 1)
      {
        NWScript.SendMessageToPC(oPC, "Cannot enchant a plot item.");
        return 0;
      }

      var oSecondItem = NWScript.GetNextItemInInventory(oid);
      if (oSecondItem != NWScript.OBJECT_INVALID)
      {
        NWScript.SendMessageToPC(oPC, "Invalid number of items.");
        return 0;
      }

      var tag = NWScript.GetTag(oItem);
      GetEnchantmentBasinFromTag(tag).DrawMenu(player, oItem, oid);

      return 0;
    }
  }
}
