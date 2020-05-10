using System;
using System.Collections.Generic;
using System.Text;

namespace NWN
{
    class ScriptHandlers
    {
        public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "craft_onatk", Craft_OnAtk },
        };

        private static int Craft_OnAtk(uint oidSelf)
        {
            var objName = NWScript.GetName(oidSelf);
            var oPC = NWScript.GetLastAttacker(oidSelf);
            var pcName = NWScript.GetName(oPC);
            NWScript.SendMessageToPC(oPC, $"{pcName} is attacking a poor {objName}");
            return Entrypoints.SCRIPT_HANDLED;
        }
    }
}
