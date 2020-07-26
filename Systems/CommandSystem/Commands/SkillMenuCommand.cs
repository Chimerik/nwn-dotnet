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

        __DrawSkillPage(player);
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
      
      player.menu.Draw();
      NWScript.DelayCommand(1.0f, () => __DrawSkillPage(player)); // Pas bon du tout de faire comme ça. Il faudrait ne rafraichir que la ligne correspondant au job actif. C'est envisageable ça ?
    }
    private static void __HandleSkillSelection(PlayerSystem.Player player, SkillSystem.Skill skill)
    {
      var ElapsedMinutes = (float)(DateTime.Now - DateTime.Parse(NWNX.Object.GetString(player, "_DATE_LAST_SAVED"))).TotalMinutes;
      skill.AcquiredPoints += (float)(NWScript.GetAbilityScore(player, skill.PrimaryAbility) + (NWScript.GetAbilityScore(player, skill.SecondaryAbility) / 2)) * ElapsedMinutes;
      double RemainingTime = skill.GetTimeToNextLevel(player);
      NWNX.Object.SetFloat(player, $"_JOB_SP_{skill.oid}", skill.AcquiredPoints, true);
      NWNX.Object.SetString(player, "_DATE_LAST_SAVED", DateTime.Now.ToString(), true);

      if (RemainingTime < 600) // TODO : Pour l'instant, j'interdis la pause et le changement si le skill est censé se terminer dans le prochain intervalle. Mais y a ptet mieux à faire
      {
        player.SendMessage($"L'entrainement de {skill.Nom} est sur le point de se terminer. Impossible de changer d'entrainement ou de le mettre en pause pour le moment.");
      }
      else if (skill.CurrentJob) // Job en cours sélectionné => mise en pause
      {
        skill.CurrentJob = false;
        NWNX.Object.DeleteInt(player, "_CURRENT_JOB");
      }
      else
      {
        if (NWNX.Object.GetInt(player, "_CURRENT_JOB") != 0)
          player.LearnableSkills[NWNX.Object.GetInt(player, "_CURRENT_JOB")].CurrentJob = false;
        skill.CurrentJob = true;
        NWNX.Object.SetInt(player, "_CURRENT_JOB", skill.oid, true);
      }

      __DrawSkillPage(player);
    }
  }
}
