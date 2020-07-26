using System;
using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteTestCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (((string)options.positional[0]).Length == 0)
        {
          if (NWNX.Object.GetInt(player, "_CURRENT_JOB") != 0)
            player.LearnableSkills[NWNX.Object.GetInt(player, "_CURRENT_JOB")].DisplayTimeToNextLevel(player);
          else
            player.SendMessage("Vous n'avez pas d'entrainement en cours.");
        }
        else
        {
          int SkillId;
          if (int.TryParse((string)options.positional[0], out SkillId))
          {
            player.LearnableSkills.Add(SkillId, new SkillSystem.Skill(SkillId, 0));
            /*player.testCurrentJob = new SkillSystem.Skill(0);
            NWNX.Object.SetInt(player, "_CURRENT_JOB", SkillId, true);
            NWNX.Object.SetString(player, "_DATE_LAST_SAVED", DateTime.Now.ToString(), true);*/
          }
          else
            player.SendMessage($"{(string)options.positional[0]} n'est pas une valeur acceptée pour cette commande.");
        } 
      }
    }
  }
}
