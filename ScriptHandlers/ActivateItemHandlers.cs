using System;
using System.Collections.Generic;

namespace NWN.ScriptHandlers
{
  static public class ActivateItemHandlers
  {
    public static Dictionary<string, Func<uint, uint, int>> Register = new Dictionary<string, Func<uint, uint, int>>
        {
            { "MenuTester", HandleMenuTesterActivate },
        };
    private static int HandleMenuTesterActivate(uint oItem, uint oActivator)
    {
      Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
