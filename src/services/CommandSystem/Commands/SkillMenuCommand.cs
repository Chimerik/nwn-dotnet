using System.Collections.Generic;
using System.Linq;
using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private const int _PAGINATION = 35;
    private static int page = 0;
    private static void ExecuteSkillMenuCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender.LoginCreature, out PlayerSystem.Player player))
      {
        player.menu.Close();
        __DrawWelcomePage(player);
      }
    }
    private static void __DrawWelcomePage(PlayerSystem.Player player)
    {
      player.menu.Clear();

      if (player.learnables.Any(l => l.Value.type == LearnableType.Feat && !l.Value.trained))
        player.menu.choices.Add(($"Afficher la liste des talents disponibles pour entrainement", () => __DrawSkillPage(player, LearnableType.Feat)));

      if (player.learnables.Any(l => l.Value.type == LearnableType.Spell && !l.Value.trained))
        player.menu.choices.Add(($"Afficher la liste des sorts disponibles pour entrainement", () => __DrawSkillPage(player, LearnableType.Spell)));

      if (player.menu.choices.Count > 0)
        player.menu.titleLines.Add("Que souhaitez-vous faire ?.");
      else
        player.menu.titleLines = new List<string>() {
          "Il semble que vous n'ayez plus rien à apprendre.",
          "Peut-être trouverez-vous de nouveaux éléments d'améliorations dans certains ouvrages ?"
        };

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void __DrawSkillPage(PlayerSystem.Player player, LearnableType selectedType)
    {
      player.menu.Clear();
      
      if (page < 0)
        page = 0;

      Dictionary<string, Learnable> displayList = new Dictionary<string, Learnable>();

      switch (selectedType)
      {
        case LearnableType.Feat:
          player.menu.titleLines.Add("Liste des talents disponibles pour entrainement.");
          displayList = player.learnables.OrderByDescending(key => key.Value.active).Where(l => l.Value.type == LearnableType.Feat && !l.Value.trained).Skip(page).Take(_PAGINATION).ToDictionary(kv => kv.Key, KeyValuePair => KeyValuePair.Value);
          break;
        case LearnableType.Spell:
          displayList = player.learnables.OrderByDescending(key => key.Value.active).Where(l => l.Value.type == LearnableType.Spell && !l.Value.trained).Skip(page).Take(_PAGINATION).ToDictionary(kv => kv.Key, KeyValuePair => KeyValuePair.Value);
          player.menu.titleLines.Add("Liste des sorts disponibles pour étude.");
          break;
      }

      foreach (KeyValuePair<string, Learnable> SkillListEntry in displayList)
      {
        SkillListEntry.Value.levelUpDate = DateTime.Now.AddSeconds((SkillListEntry.Value.pointsToNextLevel - SkillListEntry.Value.acquiredPoints) / player.GetSkillPointsPerSecond(SkillListEntry.Value));
        player.menu.choices.Add(($"{SkillListEntry.Value.name} - Temps restant : {Utils.StripTimeSpanMilliseconds((SkillListEntry.Value.levelUpDate - DateTime.Now))}", () => __HandleLearnableSelection(player, SkillListEntry.Value)));
      }

      if(page > 0)
        player.menu.choices.Add(("Retour", () => __DrawPreviousPage(player, selectedType)));

      if(displayList.Count > page + _PAGINATION)
        player.menu.choices.Add(("Suivant", () => __DrawNextPage(player, selectedType)));

      player.menu.choices.Add(("Quitter", () => player.menu.Close()));
      player.menu.Draw();
    }
    private static void __DrawPreviousPage(PlayerSystem.Player player, LearnableType selectedType)
    {
      page -= _PAGINATION;
      __DrawSkillPage(player, selectedType);
    }
    private static void __DrawNextPage(PlayerSystem.Player player, LearnableType selectedType)
    {
      page += _PAGINATION;
      __DrawSkillPage(player, selectedType);
    }
    private static void __HandleLearnableSelection(PlayerSystem.Player player, Learnable selectedLearnable)
    {
      if (player.learnables.Any(l => l.Value.active))
      {
        //player.oid.ExportCharacter();

        if (selectedLearnable.active) // Job en cours sélectionné => mise en pause
        {
          selectedLearnable.active = false;
          player.CancelSkillJournalEntry(selectedLearnable);
        }
        else
        {
          Learnable currentLearnable = player.learnables.First(l => l.Value.active).Value;
          currentLearnable.active = false;
          player.CancelSkillJournalEntry(currentLearnable);

          selectedLearnable.active = true;
          player.AwaitPlayerStateChangeToCalculateSPGain(selectedLearnable);
          player.CreateSkillJournalEntry(selectedLearnable);
        }
      }
      else
      {
        selectedLearnable.active = true;
        player.AwaitPlayerStateChangeToCalculateSPGain(selectedLearnable);
        player.CreateSkillJournalEntry(selectedLearnable);
      }

      player.menu.Close();
    }
  }
}
