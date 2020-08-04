using System;
using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteJobsCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (NWNX.Object.GetInt(player, "_CURRENT_JOB") != 0)
        {
          player.LearnableSkills[NWNX.Object.GetInt(player, "_CURRENT_JOB")].DisplayTimeToNextLevel(player);
        }
        else
          player.SendMessage("Vous n'avez pas d'entrainement en cours.");
      }
    }
  }
}
