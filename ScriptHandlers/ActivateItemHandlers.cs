using System;
using System.Collections.Generic;
using NWN.Systems;

namespace NWN.ScriptHandlers
{
  static public class ActivateItemHandlers
  {
    public static Dictionary<string, Func<uint, uint, int>> Register = new Dictionary<string, Func<uint, uint, int>>
        {
            { "MenuTester", HandleMenuTesterActivate },
            { "test_block", HandleBlockTesterActivate },
        };
    private static int HandleMenuTesterActivate(uint oItem, uint oActivator)
    {
      Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleBlockTesterActivate(uint oItem, uint oActivator)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        player.BoulderBlock();
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
