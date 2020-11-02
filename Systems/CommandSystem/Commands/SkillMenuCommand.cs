using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using System.Linq;

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
        NWScript.SetLocalInt(player.oid, "_MENU_SKILL_REFRESH", 1);
        __DrawSkillPage(player);
      }
    }

    private static void __DrawSkillPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Liste des skills disponibles pour entrainement.";

      //var sortedDict = from entry in player.learnableSkills orderby entry.Value ascending select entry;
      foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.learnableSkills.OrderByDescending(key => key.Value.currentJob))
      {
        SkillSystem.Skill skill = SkillListEntry.Value;

        if (!skill.trained)
          player.menu.choices.Add(($"{skill.name} - Temps restant : {skill.GetTimeToNextLevelAsString(player)}", () => __HandleSkillSelection(player, skill)));

        // TODO : Suivant, précédent et quitter
      }

      /*foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.learnableSkills)
      {
        SkillSystem.Skill skill = SkillListEntry.Value;
        // TODO :  afficher le skill en cours en premier ?

        if(!skill.trained)
          player.menu.choices.Add(($"{skill.name} - Temps restant : {skill.GetTimeToNextLevelAsString(player)}", () => __HandleSkillSelection(player, skill)));
          
        // TODO : Suivant, précédent et quitter
      }*/

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __DrawMalusPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Liste des blessures persistantes nécessitant une rééducation.";
      foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.removeableMalus)
      {
        SkillSystem.Skill skill = SkillListEntry.Value;
        // TODO :  afficher le skill en cours en premier ?

        skill.GetTimeToNextLevel(player);
        player.menu.choices.Add(($"{skill.name} - Temps restant : {skill.GetTimeToNextLevelAsString(player)}", () => __HandleSkillSelection(player, skill)));

        // TODO : Suivant, précédent et quitter
      }

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __HandleSkillSelection(PlayerSystem.Player player, SkillSystem.Skill SelectedSkill)
    {
      if (ObjectPlugin.GetInt(player.oid, "_CURRENT_JOB") != 0)
      {
        SkillSystem.Skill CurrentSkill = player.learnableSkills[ObjectPlugin.GetInt(player.oid, "_CURRENT_JOB")];
        if(CurrentSkill == null)
          CurrentSkill = player.removeableMalus[ObjectPlugin.GetInt(player.oid, "_CURRENT_JOB")];

        if (CurrentSkill.GetTimeToNextLevel(player) < 600) // TODO : Pour l'instant, j'interdis la pause et le changement si le skill est censé se terminer dans le prochain intervalle. Mais y a ptet mieux à faire
        {
          NWScript.SendMessageToPC(player.oid, $"L'entrainement de {CurrentSkill.name} est sur le point de se terminer. Impossible de changer d'entrainement ou de le mettre en pause pour le moment.");
        }
        else if (SelectedSkill.currentJob) // Job en cours sélectionné => mise en pause
        {
          SelectedSkill.currentJob = false;
          ObjectPlugin.DeleteInt(player.oid, "_CURRENT_JOB");
        }
        else
        {
          CurrentSkill.currentJob = false;
          SelectedSkill.currentJob = true;
          ObjectPlugin.SetInt(player.oid, "_CURRENT_JOB", SelectedSkill.oid, 1);
        }
      }
      else
      {
        SelectedSkill.currentJob = true;
        ObjectPlugin.SetInt(player.oid, "_CURRENT_JOB", SelectedSkill.oid, 1);
      }

      __DrawSkillPage(player);
    }
    private static void __HandleClose(PlayerSystem.Player player)
    {
      NWScript.DeleteLocalInt(player.oid, "_MENU_SKILL_REFRESH");
      player.menu.Close();
    }
  }
}
