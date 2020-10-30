using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRebootCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender) == 1)
      {
        //NWScript.ExportAllCharacters();

        foreach (KeyValuePair<uint, PlayerSystem.Player> PlayerListEntry in PlayerSystem.Players)
        {
          NWScript.FloatingTextStringOnCreature("Attention - Le serveur va redémarrer dans 30 secondes.", PlayerListEntry.Key, 0);
          Utils.RebootTimer(PlayerListEntry.Key, 30);
        }

        AdminPlugin.SetPlayerPassword("REBOOT");
        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(30.0f, () => Utils.BootAllPC()));
        NWScript.AssignCommand(NWScript.GetModule(), () => NWScript.DelayCommand(35.0f, () => AdminPlugin.ShutdownServer()));
      }
    }
  }
}
