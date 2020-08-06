using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSkillMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var configOpt = (bool)options.named.GetValueOrDefault("config");

      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        player.Locals.Int.Set("_MENU_SKILL_REFRESH", 1);
       }
    }

    private static void __DrawSkillPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Liste des skills disponibles pour entrainement.";
      foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.LearnableSkills)
      {
        SkillSystem.Skill skill = SkillListEntry.Value;
        // TODO :  afficher le skill en cours en premier ?

        skill.GetTimeToNextLevel(player);
        player.menu.choices.Add(($"{skill.Name} {skill.CurrentLevel} - Temps restant : {skill.GetTimeToNextLevelAsString(player)}", () => __HandleSkillSelection(player, skill)));
          
        // TODO : Suivant, précédent et quitter
      }

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __HandleSkillSelection(PlayerSystem.Player player, SkillSystem.Skill SelectedSkill)
    {
      if (NWNX.Object.GetInt(player, "_CURRENT_JOB") != 0)
      {
        SkillSystem.Skill CurrentSkill = player.LearnableSkills[NWNX.Object.GetInt(player, "_CURRENT_JOB")];

        if (CurrentSkill.GetTimeToNextLevel(player) < 600) // TODO : Pour l'instant, j'interdis la pause et le changement si le skill est censé se terminer dans le prochain intervalle. Mais y a ptet mieux à faire
        {
          player.SendMessage($"L'entrainement de {CurrentSkill.Name} est sur le point de se terminer. Impossible de changer d'entrainement ou de le mettre en pause pour le moment.");
        }
        else if (SelectedSkill.CurrentJob) // Job en cours sélectionné => mise en pause
        {
          SelectedSkill.CurrentJob = false;
          NWNX.Object.DeleteInt(player, "_CURRENT_JOB");
        }
        else
        {
          CurrentSkill.CurrentJob = false;
          SelectedSkill.CurrentJob = true;
          NWNX.Object.SetInt(player, "_CURRENT_JOB", SelectedSkill.oid, true);
        }
      }
      else
      {
        SelectedSkill.CurrentJob = true;
        NWNX.Object.SetInt(player, "_CURRENT_JOB", SelectedSkill.oid, true);
      }

      __DrawSkillPage(player);
    }
    private static void __HandleClose(PlayerSystem.Player player)
    {
      player.Locals.Int.Delete("_MENU_SKILL_REFRESH");
      player.menu.Close();
    }
  }
}
