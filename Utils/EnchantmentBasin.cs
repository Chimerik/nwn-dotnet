using NWN.Systems;

namespace NWN
{
  public static class EnchantmentBasin
  {
    public static int HandleClose(uint oid)
    {
      var oPC = NWScript.GetLastClosedBy();
      PlayerSystem.Player player;

      if (PlayerSystem.Players.TryGetValue(oPC, out player))
      {
        EnchantmentBasin.DrawMenu(player);
      }

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static void DrawMenu (PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Menu du bassin d'enchantement";
      player.menu.choices.Add(("Toto", () => HandleToto()));

      player.menu.Draw();
    }

    private static void HandleToto()
    {

    }
  }
}
