using System;
using NWN.Core.NWNX;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteJobsCommand(ChatSystem.Context ctx, Options.Result options)
    {
      /*PlayerSystem.Player player; // TODO : revoir la méthode d'affichage du temps restant pour skill + craft job
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (NWScript.GetLocalInt(player.oid, "_DISPLAY_JOBS") == 0)
        {
          if (ObjectPlugin.GetInt(player.oid, "_CURRENT_JOB") != 0)
          {
            NWScript.SetLocalInt(player.oid, "_DISPLAY_JOBS", 1);
            player.learnableSkills[ObjectPlugin.GetInt(player.oid, "_CURRENT_JOB")].DisplayTimeToNextLevel(player);
          }
          else
            NWScript.SendMessageToPC(player.oid, "Vous n'avez pas d'entrainement en cours.");
        }
        else
        {
          NWScript.DeleteLocalInt(player.oid, "_DISPLAY_JOBS");
        }
      }*/
    }
  }
}
