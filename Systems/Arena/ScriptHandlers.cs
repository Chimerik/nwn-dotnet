using System;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  public static class ScriptHandlers
  {
    public static int HandlePullRopeChainUse(uint oidSelf)
    {
      var oPC = NWScript.GetLastUsedBy();

      if (Players.TryGetValue(oPC, out Player player))
      {
        if (Utils.GetIsRoundInProgress(player))
        {
          NWScript.SendMessageToPC(player.oid, "Vous n'avez pas encore terminé le combat !");
          return 0;
        }

        ArenaMenu.DrawMainPage(player);
      }

      return 0;
    }

    public static int HandleCreatureOnDeath(uint oidSelf)
    {
      var oPC = (uint)NWScript.GetLocalInt(oidSelf, Config.PVE_ARENA_CHALLENGER_VARNAME);

      NWScript.DestroyObject(oidSelf);

      if (Players.TryGetValue(oPC, out Player player))
      {
        Utils.CheckRoundEnded(player);
      }

      return 0;
    }
  }
}
