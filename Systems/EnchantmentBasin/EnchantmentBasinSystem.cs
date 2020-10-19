using NWN.Core;

namespace NWN.Systems
{
  public static partial class EnchantmentBasinSystem
  {
    public static int HandleClose(uint oid)
    {
      var oPC = NWScript.GetLastClosedBy();
      PlayerSystem.Player player;

      if (!PlayerSystem.Players.TryGetValue(oPC, out player)) return 0;

      var oItem = NWScript.GetFirstItemInInventory();

      if (oItem == NWScript.OBJECT_INVALID) return 0;
      if (!ItemUtils.IsEquipable(oItem)) return 0;

      if (NWScript.GetPlotFlag(oItem) == 1)
      {
        NWScript.SendMessageToPC(oPC, "Cannot enchant a plot item.");
        return 0;
      }

      var oSecondItem = NWScript.GetNextItemInInventory();
      if (oSecondItem != NWScript.OBJECT_INVALID)
      {
        NWScript.SendMessageToPC(oPC, "Invalid number of items.");
        return 0;
      }

      var enchantmentBasin = new EnchantmentBasin(player, oItem);
      enchantmentBasin.DrawMenu();

      return 0;
    }
  }
}
