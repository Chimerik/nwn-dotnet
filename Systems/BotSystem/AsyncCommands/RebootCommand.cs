using System.Collections.Generic;
using NWN.Core;

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
    }
  }
}
