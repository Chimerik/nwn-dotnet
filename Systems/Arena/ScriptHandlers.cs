using System;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems.Arena
{
  [ServiceBinding(typeof(ScriptHandlers))]
  public class ScriptHandlers
  {
    public static void HandlePullRopeChainUse()
    {
      var oPC = NWScript.GetLastUsedBy();

      if (Players.TryGetValue(oPC, out Player player))
      {
        if (Utils.GetIsRoundInProgress(player))
        {
          NWScript.SendMessageToPC(player.oid, "Vous n'avez pas encore terminé le combat !");
          return;
        }

        ArenaMenu.DrawMainPage(player);
      }

    }

    public static void HandleCreatureOnDeath(CreatureEvents.OnDeath onDeath)
    {
      var oPC = (uint)NWScript.GetLocalInt(onDeath.KilledCreature, Config.PVE_ARENA_CHALLENGER_VARNAME);

      onDeath.KilledCreature.Destroy();

      if (Players.TryGetValue(oPC, out Player player))
      {
        Utils.CheckRoundEnded(player);
      }
    }
  }
}
