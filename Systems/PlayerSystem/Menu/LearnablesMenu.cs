using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        NuiRow buttonRow { get; }
        NuiRow comboRow { get; }
        NuiRow searchRow { get; }
        List<NuiElement> rootChidren { get; }
        NuiBind<List<NuiComboEntry>> categories { get; }
        List<NuiComboEntry> skillCategories { get; }
        List<NuiComboEntry> spellCategories { get; }
        NuiBind<int> selectedCategory { get; }
        NuiBind<bool> enableSkillButton { get; }
        NuiBind<bool> enableSpellButton { get; }
        bool displaySkill { get; set; }
        NuiBind<string> search { get; }
        NuiColor white { get; }
        NuiRect drawListRect { get; }

        public LearnableWindow(Player player) : base(player)
        {
          windowId = "learnables";

          white = new NuiColor(255, 255, 255);
          drawListRect = new NuiRect(0, 35, 150, 60);

          displaySkill = true;

          selectedCategory = new NuiBind<int>("category");
          search = new NuiBind<string>("search");
          enableSkillButton = new NuiBind<bool>("enableSkillButton");
          enableSpellButton = new NuiBind<bool>("enableSpellButton");

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };

          buttonRow = new NuiRow() { Children = new List<NuiElement>() {
            new NuiButton("Compétences") { Id = "loadSkills", Width = 193, Enabled = enableSkillButton },
            new NuiButton("Sorts") { Id = "loadSpells", Width = 193, Enabled = enableSpellButton }
          } };

          categories = new NuiBind<List<NuiComboEntry>>("categories");

          comboRow = new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = categories, Selected = selectedCategory, Width = 388 } } };
          searchRow = new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 388 } } };

          skillCategories = new List<NuiComboEntry>();
          foreach (var cat in (SkillSystem.Category[])Enum.GetValues(typeof(SkillSystem.Category)))
            skillCategories.Add(new NuiComboEntry(cat.ToDescription(), (int)cat));

          spellCategories = new List<NuiComboEntry>
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

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          //RefreshWindow();

          rootChidren.Add(buttonRow);
          rootChidren.Add(comboRow);
          rootChidren.Add(searchRow);

          window = new NuiWindow(rootGroup, "Journal d'apprentissage")
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

          RefreshWindow();
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
                  RefreshWindow();
                  return;
                case "loadSpells":
                  enableSkillButton.SetBindValue(player.oid, token, true);
                  enableSpellButton.SetBindValue(player.oid, token, false);
                  selectedCategory.SetBindWatch(player.oid, token, false);
                  selectedCategory.SetBindValue(player.oid, token, 0);
                  selectedCategory.SetBindWatch(player.oid, token, true);
                  categories.SetBindValue(player.oid, token, spellCategories);
                  displaySkill = false;
                  RefreshWindow();
                  return;
              }

              if(nuiEvent.ElementId.StartsWith("learn_"))
              {
                int learnableId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf("_") + 1));

                if (player.openedWindows.ContainsKey("activeLearnable"))
                  player.oid.NuiDestroy(player.openedWindows["activeLearnable"]);
                
                if (player.windows.ContainsKey("activeLearnable"))
                  ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow(learnableId);
                else
                  player.windows.Add("activeLearnable", new ActiveLearnableWindow(player, learnableId));

                RefreshWindow();
              }
              else if(int.TryParse(nuiEvent.ElementId, out int learnableId))
              {
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
                case "category":
                case "search":
                  RefreshWindow();
                  break;
              }

              break;
          }
        }
        public void RefreshWindow()
        {
          rootChidren.Clear();
          rootChidren.Add(buttonRow);
          rootChidren.Add(comboRow);
          rootChidren.Add(searchRow);

          if (token < 0)
            return;

          if (displaySkill)
            CreateSkillRows();
          else
            CreateSpellRows();

          rootGroup.SetLayout(player.oid, token, rootColumn);
        }
        private void CreateSkillRows()
        {
          int categorySelected = selectedCategory.GetBindValue(player.oid, token);
          string currentSearch = search.GetBindValue(player.oid, token).ToLower();
          var filteredList = player.learnableSkills.AsEnumerable();

          filteredList = filteredList.Where(s => s.Value.category == (SkillSystem.Category)categorySelected);

          if (currentSearch != "")
            filteredList = filteredList.Where(s => s.Value.name.ToLower().Contains(currentSearch));

          foreach (var kvp in filteredList)
          {
            bool canLearn = kvp.Value.attackBonusPrerequisite > 0 && player.oid.LoginCreature.BaseAttackBonus < kvp.Value.attackBonusPrerequisite ? false : true;
            
            if(canLearn)
              foreach(var abilityPreReq in kvp.Value.abilityPrerequisites)
                if(player.oid.LoginCreature.GetAbilityScore(abilityPreReq.Key, true) < abilityPreReq.Value)
                {
                  canLearn = false;
                  break;
                }

            if(canLearn)
              foreach (var skillPreReq in kvp.Value.skillPrerequisites)
                if (player.learnableSkills[skillPreReq.Key].currentLevel < skillPreReq.Value)
                {
                  canLearn = false;
                  break;
                }

            string buttonText = kvp.Value.active ? "En cours" : "Apprendre";

            if (!canLearn)
              buttonText = "Prérequis Manquant";

            NuiRow row = new NuiRow()
            {
              Children = new List<NuiElement>()
              {
                new NuiButtonImage(kvp.Value.icon) { Id = kvp.Key.ToString(), Tooltip = "Description", Height = 40, Width = 40 },
                new NuiLabel(kvp.Value.name) { Id = kvp.Key.ToString(), Tooltip = kvp.Value.name, Width = 160, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, kvp.Value.GetReadableTimeSpanToNextLevel(player)) } },
                new NuiLabel("Niveau/Max") { Id = kvp.Key.ToString(), Width = 90, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, $"{kvp.Value.currentLevel}/{kvp.Value.maxLevel}") } },
                new NuiButton(buttonText) { Id = $"learn_{kvp.Key}", Height = 40, Width = 90, Enabled = kvp.Value.currentLevel < kvp.Value.maxLevel && !kvp.Value.active, Tooltip = "Remplace l'apprentissage actif. L'avancement de l'apprentissage précédent sera sauvegardé." }
              }
            };

            rootChidren.Add(row);
          }
        }
        private void CreateSpellRows()
        {
          int spellLevelSelected = selectedCategory.GetBindValue(player.oid, token);
          string currentSearch = search.GetBindValue(player.oid, token).ToLower();
          var filteredList = player.learnableSpells.AsEnumerable();

          if (spellLevelSelected > 0)
            filteredList = filteredList.Where(s => s.Value.spellLevel == spellLevelSelected - 1);

          if(currentSearch != "")
            filteredList = filteredList.Where(s => s.Value.name.ToLower().Contains(currentSearch));

          foreach (var kvp in filteredList)
          {
            NuiRow row = new NuiRow()
            {
              Children = new List<NuiElement>()
              {
                new NuiButtonImage(kvp.Value.icon) { Id = kvp.Key.ToString(), Tooltip = "Description", Height = 40, Width = 40 },
                new NuiLabel(kvp.Value.name) { Id = kvp.Key.ToString(), Tooltip = kvp.Value.name, Width = 160, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, kvp.Value.GetReadableTimeSpanToNextLevel(player)) } },
                new NuiLabel("Niveau/Max") { Id = kvp.Key.ToString(), Width = 90, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, $"{kvp.Value.currentLevel}/{kvp.Value.maxLevel}") } },
                new NuiButton(kvp.Value.active ? "En cours" : "Apprendre") { Id = $"learn_{kvp.Key}", Height = 40, Width = 90, Enabled = kvp.Value.currentLevel < kvp.Value.maxLevel && !kvp.Value.active, Tooltip = "Remplace l'apprentissage actif. L'avancement de l'apprentissage précédent sera sauvegardé." }
              }
            };

            rootChidren.Add(row);
          }
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

          RefreshWindow();
          RefreshWindowOnAbilityChange();
        }
      }
    }
  }
}
