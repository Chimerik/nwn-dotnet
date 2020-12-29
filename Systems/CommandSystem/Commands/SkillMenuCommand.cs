using System.Collections.Generic;
using System.Linq;
using System;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSkillMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        __DrawWelcomePage(player);
      }
    }
    private static void __DrawWelcomePage(PlayerSystem.Player player)
    {
      player.menu.Clear();

      if (player.learnableSkills.Count > 0)
        player.menu.choices.Add(($"Afficher la liste des talents disponibles pour entrainement", () => __DrawSkillPage(player)));

      if (player.learnableSpells.Count > 0)
        player.menu.choices.Add(($"Afficher la liste des sorts disponibles pour entrainement", () => __DrawSpellPage(player)));

      if(player.menu.choices.Count > 0)
        player.menu.title = "Que souhaitez-vous faire ?.";
      else
        player.menu.title = "Il semble que vous n'ayez plus rien à apprendre. Peut-être trouverez-vous de nouveaux éléments d'améliorations dans certains ouvrages ?";

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __DrawSkillPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Liste des talents disponibles pour entrainement.";

      //var sortedDict = from entry in player.learnableSkills orderby entry.Value ascending select entry;
      foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.learnableSkills.OrderByDescending(key => key.Value.currentJob))
      {
        SkillSystem.Skill skill = SkillListEntry.Value;

        if (!skill.trained)
        {
          if(skill.currentJob)
            skill.RefreshAcquiredSkillPoints();
          
          player.menu.choices.Add(($"{skill.name} - Temps restant : {Utils.StripTimeSpanMilliseconds((TimeSpan)(DateTime.Now.AddSeconds(skill.GetTimeToNextLevel(skill.CalculateSkillPointsPerSecond())) - DateTime.Now))}", () => __HandleSkillSelection(player, skill)));
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
    private static void __DrawSpellPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.title = "Liste des sorts disponibles pour étude.";

      foreach (KeyValuePair<int, SkillSystem.LearnableSpell> SpellListEntry in player.learnableSpells.OrderByDescending(key => key.Value.currentJob))
      {
        SkillSystem.LearnableSpell spell = SpellListEntry.Value;

        if (!spell.trained)
        {
          if (spell.currentJob)
            spell.RefreshAcquiredSkillPoints();

          player.menu.choices.Add(($"{spell.name} - Temps restant : {Utils.StripTimeSpanMilliseconds((TimeSpan)(DateTime.Now.AddSeconds(spell.GetTimeToNextLevel(spell.CalculateSkillPointsPerSecond())) - DateTime.Now))}", () => __HandleSpellSelection(player, spell)));
        }
        // TODO : Suivant, précédent
      }

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

        skill.RefreshAcquiredSkillPoints();
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
        //if(CurrentSkill == null)
          //CurrentSkill = player.removeableMalus[player.currentSkillJob];

        if (SelectedSkill.currentJob) // Job en cours sélectionné => mise en pause
        {
          SelectedSkill.currentJob = false;
          player.currentSkillJob = (int)Feat.Invalid;
          player.currentSkillType = SkillType.Invalid;
          SelectedSkill.CancelSkillJournalEntry();
        }
        else
        {
          switch (player.currentSkillType)
          {
            case SkillType.Skill:
              Skill currentSkill = player.learnableSkills[player.currentSkillJob];
              currentSkill.currentJob = false;
              currentSkill.CancelSkillJournalEntry();
              break;
            case SkillType.Spell:
              LearnableSpell currentSpell = player.learnableSpells[player.currentSkillJob];
              currentSpell.currentJob = false;
              currentSpell.CancelSkillJournalEntry();
              break;
          }

          SelectedSkill.currentJob = true;
          player.currentSkillJob = SelectedSkill.oid;
          player.currentSkillType = SkillType.Skill;
          SelectedSkill.CreateSkillJournalEntry();
        }
      }
      else
      {
        SelectedSkill.currentJob = true;
        player.currentSkillJob = SelectedSkill.oid;
        player.currentSkillType = SkillType.Skill;
        SelectedSkill.CreateSkillJournalEntry();
      }

      __DrawSkillPage(player);
    }
    private static void __HandleSpellSelection(PlayerSystem.Player player, SkillSystem.LearnableSpell selectedSpell)
    {
      if (player.currentSkillJob != (int)Feat.Invalid)
      {
        if (selectedSpell.currentJob) // Job en cours sélectionné => mise en pause
        {
          selectedSpell.currentJob = false;
          player.currentSkillJob = (int)Feat.Invalid;
          player.currentSkillType = SkillType.Invalid;
          selectedSpell.CancelSkillJournalEntry();
        }
        else
        {
          switch(player.currentSkillType)
          {
            case SkillType.Skill:
              Skill currentSkill = player.learnableSkills[player.currentSkillJob];
              currentSkill.currentJob = false;
              currentSkill.CancelSkillJournalEntry();
              break;
            case SkillType.Spell:
              LearnableSpell currentSpell = player.learnableSpells[player.currentSkillJob];
              currentSpell.currentJob = false;
              currentSpell.CancelSkillJournalEntry();
              break;
          }

          selectedSpell.currentJob = true;
          player.currentSkillJob = selectedSpell.oid;
          player.currentSkillType = SkillType.Spell;
          selectedSpell.CreateSkillJournalEntry();
        }
      }
      else
      {
        selectedSpell.currentJob = true;
        player.currentSkillJob = selectedSpell.oid;
        player.currentSkillType = SkillType.Spell;
        selectedSpell.CreateSkillJournalEntry();
      }

      __DrawSpellPage(player);
    }
    private static void __HandleClose(PlayerSystem.Player player)
    {
      player.menu.Close();
    }
  }
}
