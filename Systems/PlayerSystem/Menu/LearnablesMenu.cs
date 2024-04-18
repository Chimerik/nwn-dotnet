using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class LearnableWindow : PlayerWindow
      {
        bool displaySkill { get; set; }
        //bool refreshOn { get; set; }
        private readonly NuiColumn rootColumn;
        private readonly NuiBind<List<NuiComboEntry>> categories = new("categories");
        private readonly List<NuiComboEntry> skillCategories = new();

        private readonly List<NuiComboEntry> spellCategories = new()
          {
            new NuiComboEntry("Tous", 0),
            new NuiComboEntry("Niveau 0", 1),
            new NuiComboEntry("Niveau 1", 2),
            new NuiComboEntry("Niveau 2", 3),
            new NuiComboEntry("Niveau 3", 4),
            new NuiComboEntry("Niveau 4", 5),
            new NuiComboEntry("Niveau 5", 6),
            new NuiComboEntry("Niveau 6", 7),
            new NuiComboEntry("Niveau 7", 8),
            new NuiComboEntry("Niveau 8", 9),
            new NuiComboEntry("Niveau 9", 10),
          };

        private readonly NuiBind<int> selectedCategory = new("selectedCategory");
        private readonly NuiBind<bool> enableSkillButton = new("enableSkillButton");
        private readonly NuiBind<bool> enableSpellButton = new("enableSpellButton");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> skillName = new("skillName");
        public readonly NuiBind<string> remainingTime = new("remainingTime");
        private readonly NuiBind<string> level = new("level");
        private readonly NuiBind<string> learnButtonText = new("learnButtonText");
        private readonly NuiBind<bool> learnButtonEnabled = new("learnButtonEnabled");
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private readonly Color white = new(255, 255, 255);

        public IEnumerable<Learnable> currentList;
        private Player target { get; set; }
        private bool isReadOnly = false;

        public LearnableWindow(Player player, Player target = null) : base(player)
        {
          windowId = "learnables";
          displaySkill = true;

          List<NuiListTemplateCell> learnableTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiButtonImage(icon) { Id = "description", Tooltip = "Description", Height = 40, Width = 40 }) { Width = 40 },
            new(new NuiLabel(skillName)
            {
              Width = 160, Id = "description", Tooltip = skillName,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, remainingTime) }
            }) { Width = 160 },
            new(new NuiLabel("Niveau/Max") { Id = "description", Width = 90,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, level) }
            } ) { Width = 90 },
            new(new NuiButton(learnButtonText) { Id = "learn", Enabled = learnButtonEnabled, Tooltip = "Remplace l'apprentissage actif. L'avancement de l'apprentissage précédent sera sauvegardé.", Height = 40, Width = 90 }) { Width = 90 }
          };

          rootColumn = new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiButton("Compétences") { Id = "loadSkills", Width = 209, Enabled = enableSkillButton },
                new NuiButton("Sorts") { Id = "loadSpells", Width = 209, Enabled = enableSpellButton }
              }
            },
            new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = categories, Selected = selectedCategory, Width = 419 } } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 420 } } },
            new NuiList(learnableTemplate, listCount) { RowHeight = 40, Width = 420 },
          } };

          CreateWindow(target);
        }
        public void CreateWindow(Player playerTarget = null)
        {
          //refreshOn = false;
          target = playerTarget ?? player;
          skillCategories.Clear();

          foreach (var cat in target.learnableSkills.Values.GroupBy(l => l.category))
            skillCategories.Add(new NuiComboEntry(cat.Key.ToDescription(), (int)cat.Key));

          window = new NuiWindow(rootColumn, "Journal d'apprentissage")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = collapsed,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            isReadOnly = playerTarget != null;

            drawListRect.SetBindValue(player.oid, nuiToken.Token, Utils.GetDrawListTextScaleFromPlayerUI(player));

            selectedCategory.SetBindValue(player.oid, nuiToken.Token, 0);
            selectedCategory.SetBindWatch(player.oid, nuiToken.Token, true);

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            enableSkillButton.SetBindValue(player.oid, nuiToken.Token, false);
            enableSpellButton.SetBindValue(player.oid, nuiToken.Token, true);

            categories.SetBindValue(player.oid, nuiToken.Token, skillCategories);

            collapsed.SetBindValue(player.oid, nuiToken.Token, false);
            geometry.SetBindValue(player.oid, nuiToken.Token, player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if (target.activeLearnable != null && target.activeLearnable.active && !player.TryGetOpenedWindow("activeLearnable", out PlayerWindow activeWindow))
              if (!player.windows.TryGetValue("activeLearnable", out var rectangle)) player.windows.Add("activeLearnable", new ActiveLearnableWindow(player, target));
              else ((ActiveLearnableWindow)rectangle).CreateWindow(target);

            currentList = target.learnableSkills.Values.Where(s => s.category == SkillSystem.Category.MindBody);
            LoadLearnableList(currentList);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }

        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "loadSkills":

                  enableSkillButton.SetBindValue(player.oid, nuiToken.Token, false);
                  enableSpellButton.SetBindValue(player.oid, nuiToken.Token, true);
                  selectedCategory.SetBindWatch(player.oid, nuiToken.Token, false);
                  selectedCategory.SetBindValue(player.oid, nuiToken.Token, 0);
                  selectedCategory.SetBindWatch(player.oid, nuiToken.Token, true);
                  categories.SetBindValue(player.oid, nuiToken.Token, skillCategories);
                  displaySkill = true;
                  currentList = target.learnableSkills.Values.Where(s => s.category == Category.MindBody);
                  LoadLearnableList(currentList);

                  return;

                case "loadSpells":

                  enableSkillButton.SetBindValue(player.oid, nuiToken.Token, true);
                  enableSpellButton.SetBindValue(player.oid, nuiToken.Token, false);
                  selectedCategory.SetBindWatch(player.oid, nuiToken.Token, false);
                  selectedCategory.SetBindValue(player.oid, nuiToken.Token, 0);
                  selectedCategory.SetBindWatch(player.oid, nuiToken.Token, true);
                  categories.SetBindValue(player.oid, nuiToken.Token, spellCategories);
                  displaySkill = false;
                  currentList = target.learnableSpells.Values;
                  LoadLearnableList(currentList);

                  return;
              }

              if (nuiEvent.ElementId.StartsWith("learn") && !isReadOnly)
              {
                currentList.ElementAt(nuiEvent.ArrayIndex).StartLearning(target);

                if (player.TryGetOpenedWindow("activeLearnable", out PlayerWindow activeWindow))
                  activeWindow.CloseWindow();

                if (!player.windows.TryGetValue("activeLearnable", out var value)) player.windows.Add("activeLearnable", new ActiveLearnableWindow(player, target));
                else ((ActiveLearnableWindow)value).CreateWindow(target);

                LoadLearnableList(currentList);
              }
              else
              {
                int learnableId = currentList.ElementAt(nuiEvent.ArrayIndex).id;

                if (!player.windows.TryGetValue("learnableDescription", out var value)) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, learnableId));
                else ((LearnableDescriptionWindow)value).CreateWindow(learnableId);
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "selectedCategory":
                case "search":
                  HandleLearnableSearch();
                  break;
              }

              break;
          }
        }
        public void LoadLearnableList(IEnumerable<Learnable> filteredList)
        {
          List<string> iconList = new List<string>();
          List<string> skillNameList = new List<string>();
          List<string> remainingTimeList = new List<string>();
          List<string> levelList = new List<string>();
          List<string> learnButtonTextList = new List<string>();
          List<bool> learnButtonEnabledList = new List<bool>();

          foreach (Learnable learnable in filteredList)
          {
            iconList.Add(learnable.icon);
            skillNameList.Add(learnable.name);
            remainingTimeList.Add(learnable.GetReadableTimeSpanToNextLevel(target));
            levelList.Add($"{learnable.currentLevel}/{learnable.maxLevel}");
            bool canLearn = true;

            if (learnable is LearnableSkill)
            {
              if (learnable.currentLevel >= learnable.maxLevel)
                canLearn = false;
            }
            else if (learnable is LearnableSpell spell && !spell.canLearn)
              canLearn = false;

            string buttonText = learnable.active ? "En cours" : "Apprendre";
            //if (!canLearn)
            //buttonText = "Prérequis Manquant";

            learnButtonTextList.Add(buttonText);
            learnButtonEnabledList.Add(canLearn);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          skillName.SetBindValues(player.oid, nuiToken.Token, skillNameList);
          remainingTime.SetBindValues(player.oid, nuiToken.Token, remainingTimeList);
          level.SetBindValues(player.oid, nuiToken.Token, levelList);
          learnButtonText.SetBindValues(player.oid, nuiToken.Token, learnButtonTextList);
          learnButtonEnabled.SetBindValues(player.oid, nuiToken.Token, learnButtonEnabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
        public void HandleLearnableSearch()
        {
          int categorySelected = selectedCategory.GetBindValue(player.oid, nuiToken.Token);
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();

          if (displaySkill)
            currentList = target.learnableSkills.Values.Where(s => s.category == (Category)categorySelected);
          else if (categorySelected > 0)
            currentList = target.learnableSpells.Values.Where(s => s.multiplier == categorySelected - 2);
          else
            currentList = target.learnableSpells.Values;

          if (!string.IsNullOrEmpty(currentSearch))
            currentList = currentList.Where(s => s.name.ToLower().Contains(currentSearch));

          LoadLearnableList(currentList);
        }
        public void RefreshCategories(Category category)
        {
          skillCategories.Clear();

          foreach (var cat in player.learnableSkills.Values.GroupBy(l => l.category))
            skillCategories.Add(new NuiComboEntry(cat.Key.ToDescription(), (int)cat.Key));

          selectedCategory.SetBindWatch(player.oid, nuiToken.Token, false);
          categories.SetBindValue(player.oid, nuiToken.Token, skillCategories);
          selectedCategory.SetBindValue(player.oid, nuiToken.Token, (int)category);
          selectedCategory.SetBindWatch(player.oid, nuiToken.Token, true);

          HandleLearnableSearch();
        }
      }
    }
  }
}
