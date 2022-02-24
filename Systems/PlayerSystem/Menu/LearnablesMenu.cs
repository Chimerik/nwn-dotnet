using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class LearnableWindow : PlayerWindow
      {
        bool displaySkill { get; set; }
        bool refreshOn { get; set; }
        private readonly NuiColumn rootColumn;
        private readonly NuiBind<List<NuiComboEntry>> categories = new NuiBind<List<NuiComboEntry>>("categories");
        private readonly List<NuiComboEntry> skillCategories;

        private readonly List<NuiComboEntry> spellCategories = new List<NuiComboEntry>
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

        private readonly NuiBind<int> selectedCategory = new NuiBind<int>("selectedCategory");
        private readonly NuiBind<bool> enableSkillButton = new NuiBind<bool>("enableSkillButton");
        private readonly NuiBind<bool> enableSpellButton = new NuiBind<bool>("enableSpellButton");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly NuiBind<string> icon = new NuiBind<string>("icon");
        private readonly NuiBind<string> skillName = new NuiBind<string>("skillName");
        private readonly NuiBind<string> remainingTime = new NuiBind<string>("remainingTime");
        private readonly NuiBind<string> level = new NuiBind<string>("level");
        private readonly NuiBind<string> learnButtonText = new NuiBind<string>("learnButtonText");
        private readonly NuiBind<bool> learnButtonEnabled = new NuiBind<bool>("learnButtonEnabled");
        private readonly NuiBind<string> search = new NuiBind<string>("search");
        private readonly NuiColor white = new NuiColor(255, 255, 255);
        private readonly NuiRect drawListRect = new NuiRect(0, 35, 150, 60);

        public IEnumerable<Learnable> currentList;

        public LearnableWindow(Player player) : base(player)
        {
          windowId = "learnables"; 

          displaySkill = true;

          List<NuiListTemplateCell> learnableTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(icon) { Id = "description", Tooltip = "Description", Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(skillName)
            {
              Width = 160, Id = "description", Tooltip = skillName,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, remainingTime) }
            }) { Width = 160 },
            new NuiListTemplateCell(new NuiLabel("Niveau/Max") { Id = "description", Width = 90,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, level) }
            } ) { Width = 90 },
            new NuiListTemplateCell(new NuiButton(learnButtonText) { Id = "learn", Enabled = learnButtonEnabled, Tooltip = "Remplace l'apprentissage actif. L'avancement de l'apprentissage précédent sera sauvegardé.", Height = 40, Width = 90 }) { Width = 90 }
          };

          rootColumn = new NuiColumn() 
          { 
            Children = new List<NuiElement>()
            {
              new NuiRow()
              {
                Children = new List<NuiElement>()
                {
                  new NuiButton("Compétences") { Id = "loadSkills", Width = 209, Enabled = enableSkillButton },
                  new NuiButton("Sorts") { Id = "loadSpells", Width = 209, Enabled = enableSpellButton }
                }
              },
              new NuiRow()
              {
                Children = new List<NuiElement>()
                {
                  new NuiCombo() { Entries = categories, Selected = selectedCategory, Width = 419 }
                }
              },
              new NuiRow()
              {
                Children = new List<NuiElement>()
                {
                   new NuiTextEdit("Recherche", search, 50, false) { Width = 420 }
                }
              },
              new NuiList(learnableTemplate, listCount) { RowHeight = 40, Width = 420 },
            }
          };

          skillCategories = new List<NuiComboEntry>();
          foreach (var cat in (SkillSystem.Category[])Enum.GetValues(typeof(SkillSystem.Category)))
            skillCategories.Add(new NuiComboEntry(cat.ToDescription(), (int)cat));

          CreateWindow();
        }
        public void CreateWindow()
        {
          refreshOn = false;

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, "Journal d'apprentissage")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleLearnableEvents;
          player.oid.OnNuiEvent += HandleLearnableEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          selectedCategory.SetBindValue(player.oid, token, 0);
          selectedCategory.SetBindWatch(player.oid, token, true);

          search.SetBindValue(player.oid, token, "");
          search.SetBindWatch(player.oid, token, true);

          enableSkillButton.SetBindValue(player.oid, token, false);
          enableSpellButton.SetBindValue(player.oid, token, true);

          categories.SetBindValue(player.oid, token, skillCategories); 

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;

          if ((player.learnableSkills.Any(l => l.Value.active) || player.learnableSpells.Any(l => l.Value.active)) && !player.openedWindows.ContainsKey("activeLearnable"))
            if (player.windows.ContainsKey("activeLearnable"))
              ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow();
            else
              player.windows.Add("activeLearnable", new ActiveLearnableWindow(player));

          currentList = player.learnableSkills.Values.Where(s => s.category == SkillSystem.Category.MindBody);
          LoadLearnableList(currentList);
          RefreshWindowOnAbilityChange();
        }

        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch(nuiEvent.ElementId)
              {
                case "loadSkills":
                  enableSkillButton.SetBindValue(player.oid, token, false);
                  enableSpellButton.SetBindValue(player.oid, token, true);
                  selectedCategory.SetBindWatch(player.oid, token, false);
                  selectedCategory.SetBindValue(player.oid, token, 0);
                  selectedCategory.SetBindWatch(player.oid, token, true);
                  categories.SetBindValue(player.oid, token, skillCategories);
                  displaySkill = true;
                  currentList = player.learnableSkills.Values.Where(s => s.category == SkillSystem.Category.MindBody);
                  LoadLearnableList(currentList);
                  return;
                case "loadSpells":
                  enableSkillButton.SetBindValue(player.oid, token, true);
                  enableSpellButton.SetBindValue(player.oid, token, false);
                  selectedCategory.SetBindWatch(player.oid, token, false);
                  selectedCategory.SetBindValue(player.oid, token, 0);
                  selectedCategory.SetBindWatch(player.oid, token, true);
                  categories.SetBindValue(player.oid, token, spellCategories);
                  displaySkill = false;
                  currentList = player.learnableSpells.Values;
                  LoadLearnableList(currentList);
                  return;
              }

              if(nuiEvent.ElementId.StartsWith("learn"))
              {
                player.learnableSkills[currentList.ElementAt(nuiEvent.ArrayIndex).id].StartLearning(player);

                if (player.openedWindows.ContainsKey("activeLearnable"))
                  player.oid.NuiDestroy(player.openedWindows["activeLearnable"]);
                
                if (player.windows.ContainsKey("activeLearnable"))
                  ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow();
                else
                  player.windows.Add("activeLearnable", new ActiveLearnableWindow(player));

                LoadLearnableList(currentList);
              }
              else
              {
                int learnableId = currentList.ElementAt(nuiEvent.ArrayIndex).id;

                if (player.openedWindows.ContainsKey("learnableDescription"))
                  player.oid.NuiDestroy(player.openedWindows["learnableDescription"]);
                
                if (player.windows.ContainsKey("learnableDescription"))
                  ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(learnableId);
                else
                  player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, learnableId));
              }

              break;

            case NuiEventType.Watch:

              switch(nuiEvent.ElementId)
              {
                case "selectedCategory":
                case "search":

                  int categorySelected = selectedCategory.GetBindValue(player.oid, token);
                  string currentSearch = search.GetBindValue(player.oid, token).ToLower();

                  if (displaySkill)
                    currentList = player.learnableSkills.Values.Where(s => s.category == (SkillSystem.Category)categorySelected);
                  else if (categorySelected > 0)
                    currentList = player.learnableSpells.Values.Where(s => s.spellLevel == categorySelected - 1);
                  else
                    currentList = player.learnableSpells.Values;

                  if (!string.IsNullOrEmpty(currentSearch))
                    currentList = currentList.Where(s => s.name.ToLower().Contains(currentSearch));

                  LoadLearnableList(currentList);

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
            remainingTimeList.Add(learnable.GetReadableTimeSpanToNextLevel(player));
            levelList.Add($"{learnable.currentLevel}/{learnable.maxLevel}");
            bool canLearn = true;

            if (learnable is LearnableSkill skill)
            {
              canLearn = skill.attackBonusPrerequisite > 0 && player.oid.LoginCreature.BaseAttackBonus < skill.attackBonusPrerequisite ? false : true;

              if (canLearn)
                foreach (var abilityPreReq in skill.abilityPrerequisites)
                  if (player.oid.LoginCreature.GetAbilityScore(abilityPreReq.Key, true) < abilityPreReq.Value)
                  {
                    canLearn = false;
                    break;
                  }

              if (canLearn)
                foreach (var skillPreReq in skill.skillPrerequisites)
                  if (player.learnableSkills[skillPreReq.Key].currentLevel < skillPreReq.Value)
                  {
                    canLearn = false;
                    break;
                  }
            }

            string buttonText = learnable.active ? "En cours" : "Apprendre";
            if (!canLearn)
              buttonText = "Prérequis Manquant";

            learnButtonTextList.Add(buttonText);
            learnButtonEnabledList.Add(canLearn);
          }

          icon.SetBindValues(player.oid, token, iconList);
          skillName.SetBindValues(player.oid, token, skillNameList);
          remainingTime.SetBindValues(player.oid, token, remainingTimeList);
          level.SetBindValues(player.oid, token, levelList);
          learnButtonText.SetBindValues(player.oid, token, learnButtonTextList);
          learnButtonEnabled.SetBindValues(player.oid, token, learnButtonEnabledList);
          listCount.SetBindValue(player.oid, token, filteredList.Count());

          if (filteredList.Any(l => l.active) && !refreshOn)
            RefreshActiveLearnable();
        }
        private async void RefreshWindowOnAbilityChange()
        {
          int strength = player.oid.LoginCreature.GetAbilityScore(Ability.Strength);
          int dexterity = player.oid.LoginCreature.GetAbilityScore(Ability.Dexterity);
          int constitution = player.oid.LoginCreature.GetAbilityScore(Ability.Constitution);
          int intelligence = player.oid.LoginCreature.GetAbilityScore(Ability.Intelligence);
          int wisdom = player.oid.LoginCreature.GetAbilityScore(Ability.Wisdom);
          int charisma = player.oid.LoginCreature.GetAbilityScore(Ability.Charisma);

          CancellationTokenSource tokenSource = new CancellationTokenSource();
          await NwTask.WaitUntil(() => player.oid.LoginCreature == null || !player.openedWindows.ContainsKey(windowId) || strength != player.oid.LoginCreature.GetAbilityScore(Ability.Strength) || dexterity != player.oid.LoginCreature.GetAbilityScore(Ability.Dexterity) || constitution != player.oid.LoginCreature.GetAbilityScore(Ability.Constitution) || intelligence != player.oid.LoginCreature.GetAbilityScore(Ability.Intelligence) || wisdom != player.oid.LoginCreature.GetAbilityScore(Ability.Wisdom) || charisma != player.oid.LoginCreature.GetAbilityScore(Ability.Charisma), tokenSource.Token);
          tokenSource.Cancel();

          if (player.oid.LoginCreature == null || !player.openedWindows.ContainsKey(windowId))
            return;

          LoadLearnableList(currentList);
          RefreshWindowOnAbilityChange();
        }
        private async void RefreshActiveLearnable()
        {
          refreshOn = true;

          CancellationTokenSource tokenSource = new CancellationTokenSource();
          Task awaitInactive = NwTask.WaitUntil(() => player.oid.LoginCreature == null || !player.openedWindows.ContainsKey(windowId) || !currentList.Any(l => l.active), tokenSource.Token);
          Task awaitOneSecond = NwTask.Delay(TimeSpan.FromSeconds(1), tokenSource.Token);

          await NwTask.WhenAny(awaitInactive, awaitOneSecond);
          tokenSource.Cancel();

          if (awaitInactive.IsCompletedSuccessfully)
          {
            refreshOn = false;
            return;
          }

          Learnable activeLearnable = currentList.FirstOrDefault(l => l.active);
          List<string> time = remainingTime.GetBindValues(player.oid, token);
          time[currentList.ToList().IndexOf(activeLearnable)] = activeLearnable.GetReadableTimeSpanToNextLevel(player);
          remainingTime.SetBindValues(player.oid, token, time);

          RefreshActiveLearnable();
        }
      }
    }
  }
}
