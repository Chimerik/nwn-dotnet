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
        if (configOpt)
        {
          __DrawSkillConfigPage(player);
        }

        player.Locals.Int.Set("_MENU_SKILL_REFRESH", 1);
        AutoRefresh(player);
       }
    }

    private static void __DrawSkillConfigPage(PlayerSystem.Player player)
    {
      player.menu.title = "Configuration du menu des skills.";
      player.menu.choices.Clear();
      player.menu.choices.Add(("Deplacer vers la gauche.", () => __HandleMoveLeft(player)));
      player.menu.choices.Add(("Deplacer vers la droite.", () => __HandleMoveRight(player)));
      player.menu.choices.Add(("Deplacer vers le haut.", () => __HandleMoveUp(player)));
      player.menu.choices.Add(("Deplacer vers le bas.", () => __HandleMoveDown(player)));
      player.menu.choices.Add(("Reset la position à la valeur par defaut", () => __HandleReset(player)));
      player.menu.choices.Add(("Sauvegarder et quitter", () => __HandleSaveAndClose(player)));
      player.menu.Draw();
    }

    private static void __DrawSkillPage(PlayerSystem.Player player)
    {
      player.menu.title = "Liste des skills disponibles pour entrainement.";
      player.menu.choices.Clear();
      foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.LearnableSkills)
      {
        SkillSystem.Skill skill = SkillListEntry.Value;
        // TODO :  afficher le skill en cours en premier ?

        skill.GetTimeToNextLevel(player);
        player.menu.choices.Add(($"{skill.Nom} - Temps restant : {skill.GetTimeToNextLevelAsString(player)}", () => __HandleSkillSelection(player, skill)));
          
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
        var ElapsedSeconds = (float)(DateTime.Now - DateTime.Parse(NWNX.Object.GetString(player, "_DATE_LAST_SAVED"))).TotalSeconds;
        CurrentSkill.AcquiredPoints += (float)(NWScript.GetAbilityScore(player, CurrentSkill.PrimaryAbility) + (NWScript.GetAbilityScore(player, CurrentSkill.SecondaryAbility) / 2)) * ElapsedSeconds/60;
        double RemainingTime = CurrentSkill.GetTimeToNextLevel(player);
        NWNX.Object.SetFloat(player, $"_JOB_SP_{CurrentSkill.oid}", CurrentSkill.AcquiredPoints, true);
        NWNX.Object.SetString(player, "_DATE_LAST_SAVED", DateTime.Now.ToString(), true);

        if (RemainingTime < 600) // TODO : Pour l'instant, j'interdis la pause et le changement si le skill est censé se terminer dans le prochain intervalle. Mais y a ptet mieux à faire
        {
          player.SendMessage($"L'entrainement de {CurrentSkill.Nom} est sur le point de se terminer. Impossible de changer d'entrainement ou de le mettre en pause pour le moment.");
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
    private static void AutoRefresh(PlayerSystem.Player player)
    {
      __DrawSkillPage(player);
      if(player.Locals.Int.Get("_MENU_SKILL_REFRESH") != 0)
        NWScript.DelayCommand(1.0f, () => AutoRefresh(player)); // Pas bon du tout de faire comme ça. Il faudrait ne rafraichir que la ligne correspondant au job actif. C'est envisageable ça ?
    }
    private static void __HandleClose(PlayerSystem.Player player)
    {
      player.Locals.Int.Delete("_MENU_SKILL_REFRESH");
      player.menu.Close();
    }
  }
}
