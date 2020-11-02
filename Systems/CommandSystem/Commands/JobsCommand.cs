using System;
using NWN.Core.NWNX;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteJobsCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player; // TODO : revoir la méthode d'affichage du temps restant pour skill + craft job
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (player.learnableSkills.ContainsKey(player.currentSkillJob))
        {
          SkillSystem.Skill currentSkill = player.learnableSkills[player.currentSkillJob];
          NWScript.SendMessageToPC(player.oid, $"L'entrainement de {currentSkill.name} sera terminé dans {currentSkill.GetTimeToNextLevelAsString(player)}");
        }
        else
          NWScript.SendMessageToPC(player.oid, "Vous n'avez pas d'entrainement en cours.");

        if(player.craftJob.isActive)
        {
          NWScript.SendMessageToPC(player.oid, $"Votre travail en cours : {player.craftJob.name} sera terminé dans {player.craftJob.GetJobEndTimeAsFormattedString()}");
        }
      }
    }
  }
}
