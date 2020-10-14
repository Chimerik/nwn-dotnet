namespace NWN.Systems
{
  public static partial class EnchantmentBasinSystem
  {
    public static int HandleClose(uint oid)
    {
      var oPC = NWScript.GetLastClosedBy();
      PlayerSystem.Player player;

      if (!PlayerSystem.Players.TryGetValue(oPC, out player)) return Entrypoints.SCRIPT_HANDLED;

      var oItem = NWScript.GetFirstItemInInventory();

      if (oItem == NWScript.OBJECT_INVALID) return Entrypoints.SCRIPT_HANDLED;
      if (!ItemUtils.IsEquipable(oItem)) return Entrypoints.SCRIPT_HANDLED;

      if (NWScript.GetPlotFlag(oItem))
      {
        NWScript.SendMessageToPC(oPC, "Cannot enchant a plot item.");
        return Entrypoints.SCRIPT_HANDLED;
      }

      var oSecondItem = NWScript.GetNextItemInInventory();
      if (oSecondItem != NWScript.OBJECT_INVALID)
      {
        NWScript.SendMessageToPC(oPC, "Invalid number of items.");
        return Entrypoints.SCRIPT_HANDLED;
      }

      var enchantmentBasin = new EnchantmentBasin(player, oItem);
      enchantmentBasin.DrawMenu();

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
