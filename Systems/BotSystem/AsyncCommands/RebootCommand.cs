using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    private static void ExecuteRebootCommand(string prout)
    {
      foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
      {
        if (NWScript.GetIsDM(PlayerListEntry.Key) != 1)
          NWScript.BootPC(PlayerListEntry.Key, "Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
      }

      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO moduleInfo (year, month, day, hour, minute, second) VALUES (@year, @month, @day, @hour, @minute, @second)");
      NWScript.SqlBindInt(query, "@year", NWScript.GetCalendarYear());
      NWScript.SqlBindInt(query, "@month", NWScript.GetCalendarMonth());
      NWScript.SqlBindInt(query, "@day", NWScript.GetCalendarDay());
      NWScript.SqlBindInt(query, "@hour", NWScript.GetTimeHour());
      NWScript.SqlBindInt(query, "@minute", NWScript.GetTimeMinute());
      NWScript.SqlBindInt(query, "@second", NWScript.GetTimeSecond());
      NWScript.SqlStep(query);

      AdminPlugin.ShutdownServer();
    }
  }
}
