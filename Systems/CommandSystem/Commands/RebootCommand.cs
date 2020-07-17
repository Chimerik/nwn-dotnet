using System.Collections.Generic;
using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRebootCommand(ChatSystem.Context chatContext)
    {
      if (NWScript.GetIsDM(chatContext.oSender))
      {
        NWScript.ExportAllCharacters();

        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          NWScript.FloatingTextStringOnCreature("Attention - Le serveur va redémarrer dans 30 secondes.", PlayerListEntry.Key, false);
          Utils.RebootTimer(PlayerListEntry.Key, 30);
        }

        NWNX.Administrator.SetPlayerPassword("REBOOT");
        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(30.0f, () => Utils.BootAllPC()));
        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(35.0f, () => NWNX.Administrator.ShutdownServer()));
      }
    }
  }
}
