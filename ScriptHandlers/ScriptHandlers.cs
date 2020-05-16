using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN
{
    class ScriptHandlers
    {
        public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { "x2_mod_def_load", OnModuleLoad },
            { "craft_onatk", Craft_OnAtk },
        }.Concat(Systems.Loot.Register)
         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        private static int Craft_OnAtk(uint oidSelf)
        {
            var objName = NWScript.GetName(oidSelf);
            var oPC = NWScript.GetLastAttacker(oidSelf);
            var pcName = NWScript.GetName(oPC);
            NWScript.SendMessageToPC(oPC, $"{pcName} is attacking a poor {objName}");
            return Entrypoints.SCRIPT_HANDLED;
        }

        private static int OnModuleLoad (uint oidSelf)
        {
            Systems.Loot.InitChestArea();

            return Entrypoints.SCRIPT_NOT_HANDLED;
        }
    }
}
