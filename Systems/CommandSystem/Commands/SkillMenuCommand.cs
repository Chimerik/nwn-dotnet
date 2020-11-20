using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using System.Linq;
using System;
using System.Security.Cryptography;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSkillMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
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
        {
          if(skill.currentJob)
            player.RefreshAcquiredSkillPoints(skill.oid);
          
          player.menu.choices.Add(($"{skill.name} - Temps restant : {Utils.StripTimeSpanMilliseconds((TimeSpan)(DateTime.Now.AddSeconds(skill.GetTimeToNextLevel(player.CalculateSkillPointsPerSecond(skill))) - DateTime.Now))}", () => __HandleSkillSelection(player, skill)));
        }
        // TODO : Suivant, précédent
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

        player.RefreshAcquiredSkillPoints(skill.oid);
        player.menu.choices.Add(($"{skill.name} - Temps restant : {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.skillJobCountDown - DateTime.Now))}", () => __HandleSkillSelection(player, skill)));

        // TODO : Suivant, précédent et quitter
      }

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __HandleSkillSelection(PlayerSystem.Player player, SkillSystem.Skill SelectedSkill)
    {
      if (player.currentSkillJob != (int)Feat.Invalid)
      {
        SkillSystem.Skill CurrentSkill = player.learnableSkills[player.currentSkillJob];
        if(CurrentSkill == null)
          CurrentSkill = player.removeableMalus[player.currentSkillJob];

        if (SelectedSkill.currentJob) // Job en cours sélectionné => mise en pause
        {
          SelectedSkill.currentJob = false;
          player.currentSkillJob = (int)Feat.Invalid;
          CurrentSkill.CancelSkillJournalEntry();
        }
        else
        {
          CurrentSkill.currentJob = false;
          SelectedSkill.currentJob = true;
          player.currentSkillJob = SelectedSkill.oid;
          CurrentSkill.CancelSkillJournalEntry();
          SelectedSkill.CreateSkillJournalEntry();
        }
      }
      else
      {
        SelectedSkill.currentJob = true;
        player.currentSkillJob = SelectedSkill.oid;
        SelectedSkill.CreateSkillJournalEntry();
      }

      __DrawSkillPage(player);
    }
    private static void __HandleClose(PlayerSystem.Player player)
    {
      player.menu.Close();
    }
  }
}
