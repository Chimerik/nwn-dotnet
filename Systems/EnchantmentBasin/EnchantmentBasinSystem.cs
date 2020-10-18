namespace NWN.Systems
{
  public static partial class EnchantmentBasinSystem
  {
    public static int HandleClose(uint oid)
    {
      var oPC = NWScript.GetLastClosedBy();
      PlayerSystem.Player player;

      if (!PlayerSystem.Players.TryGetValue(oPC, out player)) return 1;

      var oItem = NWScript.GetFirstItemInInventory();

      if (oItem == NWScript.OBJECT_INVALID) return 1;
      if (!ItemUtils.IsEquipable(oItem)) return 1;

      if (NWScript.GetPlotFlag(oItem))
      {
        NWScript.SendMessageToPC(oPC, "Cannot enchant a plot item.");
        return 1;
      }

      var oSecondItem = NWScript.GetNextItemInInventory();
      if (oSecondItem != NWScript.OBJECT_INVALID)
      {
        NWScript.SendMessageToPC(oPC, "Invalid number of items.");
        return 1;
      }

      var enchantmentBasin = new EnchantmentBasin(player, oItem);
      enchantmentBasin.DrawMenu();

      return 1;
    }
  }
}
