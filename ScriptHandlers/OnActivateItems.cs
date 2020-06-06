using System;
using System.Collections.Generic;

namespace NWN.ScriptHandlers
{
    static public class OnActivateItems
    {
        public static Dictionary<string, Func<uint, uint, int>> Register = new Dictionary<string, Func<uint, uint, int>>
        {
            { "MenuTester", OnMenuTesterActivate },
        };
        private static int OnMenuTesterActivate (uint oItem, uint oActivator)
        {
            Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

            return Entrypoints.SCRIPT_HANDLED;
        }
    }
}
