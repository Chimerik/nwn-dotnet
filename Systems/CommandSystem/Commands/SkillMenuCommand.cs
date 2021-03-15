using System.Collections.Generic;
using System.Linq;
using System;
using static NWN.Systems.SkillSystem;
using NWN.API.Constants;
using Skill = NWN.Systems.SkillSystem.Skill;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private const int _PAGINATION = 35;
    private static int page = 0;
    private static void ExecuteSkillMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        player.menu.Close();
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

      if (player.menu.choices.Count > 0)
        player.menu.titleLines.Add("Que souhaitez-vous faire ?.");
      else
        player.menu.titleLines = new List<string>() {
          "Il semble que vous n'ayez plus rien à apprendre.",
          "Peut-être trouverez-vous de nouveaux éléments d'améliorations dans certains ouvrages ?"
        };

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __DrawSkillPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Liste des talents disponibles pour entrainement.");

      if (page < 0)
        page = 0;

      //var sortedDict = from entry in player.learnableSkills orderby entry.Value ascending select entry;
      foreach (KeyValuePair<Feat, Skill> SkillListEntry in player.learnableSkills.OrderByDescending(key => key.Value.currentJob).Skip(page).Take(_PAGINATION))
      {
        Skill skill = SkillListEntry.Value;

        if (!skill.trained)
        {
          if(skill.currentJob)
            skill.RefreshAcquiredSkillPoints();

          player.menu.choices.Add(($"{skill.name} - Temps restant : {Utils.StripTimeSpanMilliseconds((DateTime.Now.AddSeconds(skill.GetTimeToNextLevel(skill.CalculateSkillPointsPerSecond())) - DateTime.Now))}", () => __HandleSkillSelection(player, skill)));
        }
      }

      /*foreach (KeyValuePair<int, SkillSystem.Skill> SkillListEntry in player.learnableSkills)
      {
        SkillSystem.Skill skill = SkillListEntry.Value;
        // TODO :  afficher le skill en cours en premier ?

        if(!skill.trained)
          player.menu.choices.Add(($"{skill.name} - Temps restant : {skill.GetTimeToNextLevelAsString(player)}", () => __HandleSkillSelection(player, skill))); 
      }*/

      if(page > 0)
        player.menu.choices.Add(("Retour", () => __DrawPreviousPage(player)));

      if(player.learnableSkills.Count > page + _PAGINATION)
        player.menu.choices.Add(("Suivant", () => __DrawNextPage(player)));

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __DrawPreviousPage(PlayerSystem.Player player)
    {
      page -= _PAGINATION;
      __DrawSkillPage(player);
    }
    private static void __DrawNextPage(PlayerSystem.Player player)
    {
      page += _PAGINATION;
      __DrawSkillPage(player);
    }
    private static void __DrawPreviousSpellPage(PlayerSystem.Player player)
    {
      page -= _PAGINATION;
      __DrawSpellPage(player);
    }
    private static void __DrawNextSpellPage(PlayerSystem.Player player)
    {
      page += _PAGINATION;
      __DrawSpellPage(player);
    }
    private static void __DrawSpellPage(PlayerSystem.Player player)
    {
      if (page < 0)
        page = 0;

      player.menu.Clear();
      player.menu.titleLines.Add("Liste des sorts disponibles pour étude.");

      foreach (KeyValuePair<int, LearnableSpell> SpellListEntry in player.learnableSpells.OrderByDescending(key => key.Value.currentJob).Skip(page).Take(_PAGINATION))
      {
        LearnableSpell spell = SpellListEntry.Value;

        if (!spell.trained)
        {
          if (spell.currentJob)
            spell.RefreshAcquiredSkillPoints();

          player.menu.choices.Add(($"{spell.name} - Temps restant : {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddSeconds(spell.GetTimeToNextLevel(spell.CalculateSkillPointsPerSecond())) - DateTime.Now)}", () => __HandleSpellSelection(player, spell)));
        }
        // TODO : Suivant, précédent
      }
      if (page > 0)
        player.menu.choices.Add(("Retour", () => __DrawPreviousSpellPage(player)));

      if (player.learnableSkills.Count > page + _PAGINATION)
        player.menu.choices.Add(("Suivant", () => __DrawNextSpellPage(player)));

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __DrawMalusPage(PlayerSystem.Player player)
    {
      player.menu.Clear();
      player.menu.titleLines.Add("Liste des blessures persistantes nécessitant une rééducation.");

      foreach (KeyValuePair<Feat, SkillSystem.Skill> SkillListEntry in player.removeableMalus)
      {
        SkillSystem.Skill skill = SkillListEntry.Value;
        // TODO :  afficher le skill en cours en premier ?

        skill.RefreshAcquiredSkillPoints();
        player.menu.choices.Add(($"{skill.name} - Temps restant : {NWN.Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.skillJobCountDown - DateTime.Now))}", () => __HandleSkillSelection(player, skill)));

        // TODO : Suivant, précédent et quitter
      }

      player.menu.choices.Add(("Quitter", () => __HandleClose(player)));
      player.menu.Draw();
    }
    private static void __HandleSkillSelection(PlayerSystem.Player player, SkillSystem.Skill SelectedSkill)
    {
      if (player.currentSkillJob != (int)CustomFeats.Invalid)
      {
        //if(CurrentSkill == null)
        //CurrentSkill = player.removeableMalus[player.currentSkillJob];

        player.oid.ExportCharacter();

        if (SelectedSkill.currentJob) // Job en cours sélectionné => mise en pause
        {
          SelectedSkill.currentJob = false;
          player.currentSkillJob = (int)CustomFeats.Invalid;
          player.currentSkillType = SkillType.Invalid;
          SelectedSkill.CancelSkillJournalEntry();
        }
        else
        {
          switch (player.currentSkillType)
          {
            case SkillType.Skill:
              Skill currentSkill = player.learnableSkills[(Feat)player.currentSkillJob];
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
          player.currentSkillJob = (int)SelectedSkill.oid;
          player.currentSkillType = SkillType.Skill;
          SelectedSkill.CreateSkillJournalEntry();
        }
      }
      else
      {
        SelectedSkill.currentJob = true;
        player.currentSkillJob = (int)SelectedSkill.oid;
        player.currentSkillType = SkillType.Skill;
        SelectedSkill.CreateSkillJournalEntry();
      }

      __HandleClose(player);
    }
    private static void __HandleSpellSelection(PlayerSystem.Player player, LearnableSpell selectedSpell)
    {
      if (player.currentSkillJob != (int)CustomFeats.Invalid)
      {
        player.oid.ExportCharacter();

        if (selectedSpell.currentJob) // Job en cours sélectionné => mise en pause
        {
          selectedSpell.currentJob = false;
          player.currentSkillJob = (int)CustomFeats.Invalid;
          player.currentSkillType = SkillType.Invalid;
          selectedSpell.CancelSkillJournalEntry();
        }
        else
        {
          switch(player.currentSkillType)
          {
            case SkillType.Skill:
              Skill currentSkill = player.learnableSkills[(Feat)player.currentSkillJob];
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

      __HandleClose(player);
    }
    private static void __HandleClose(PlayerSystem.Player player)
    {
      player.menu.Close();
    }
  }
}
