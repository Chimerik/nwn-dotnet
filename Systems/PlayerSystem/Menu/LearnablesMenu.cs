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
        List<NuiComboEntry> categories { get; set; }
        NuiBind<int> selectedCategory { get; }
        NuiBind<string> search { get; }
        NuiColor white { get; }
        NuiRect drawListRect { get; }

        public LearnableWindow(Player player) : base(player)
        {
          windowId = "learnables";

          white = new NuiColor(255, 255, 255);
          drawListRect = new NuiRect(0, 35, 150, 60);

          selectedCategory = new NuiBind<int>("category");
          search = new NuiBind<string>("search");

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };

          buttonRow = new NuiRow() { Children = new List<NuiElement>() {
            new NuiButton("Compétences") { Width = 193 },
            new NuiButton("Sorts") { Width = 193 }
          } };

          categories = new List<NuiComboEntry>
          {
            new NuiComboEntry("Artisanat", 0),
            new NuiComboEntry("Combat", 1),
            new NuiComboEntry("Magie", 1),
          };

          comboRow = new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = categories, Selected = selectedCategory, Width = 388 } } };
          searchRow = new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 388 } } };

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);
          
          RefreshWindow();

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

          //windowRectangle = new NuiRect(windowRectangle.X, windowRectangle.Y, 410, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;

          Log.Info($"skill actif : {player.learnableSkills.Any(l => l.Value.active)}");
          Log.Info($"activeLearnable opened : {player.openedWindows.ContainsKey("activeLearnable")}");

          if ((player.learnableSkills.Any(l => l.Value.active) || player.learnableSpells.Any(l => l.Value.active)) && !player.openedWindows.ContainsKey("activeLearnable"))
            if (player.windows.ContainsKey("activeLearnable"))
              ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow();
            else
              player.windows.Add("activeLearnable", new ActiveLearnableWindow(player));

          RefreshWindowOnAbilityChange();
        }

        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch(nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if(nuiEvent.ElementId.StartsWith("learn_"))
              {
                int learnableId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf("_") + 1));

                if (player.openedWindows.ContainsKey("activeLearnable"))
                {
                  player.oid.NuiDestroy(player.openedWindows["activeLearnable"]);
                  //((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow(learnableId);
                }
                
                if (player.windows.ContainsKey("activeLearnable"))
                  ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow(learnableId);
                else
                  player.windows.Add("activeLearnable", new ActiveLearnableWindow(player, learnableId));

                Log.Info("refresh window called from click event");
                RefreshWindow();
              }
              else if(int.TryParse(nuiEvent.ElementId, out int learnableId))
              {
                if (player.openedWindows.ContainsKey("learnableDescription"))
                {
                  player.oid.NuiDestroy(player.openedWindows["learnableDescription"]);
                  ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(learnableId);
                }
                else if (player.windows.ContainsKey("learnableDescription"))
                  ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(learnableId);
                else
                  player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, learnableId));
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
          CreateRows();
          rootGroup.SetLayout(player.oid, token, rootColumn);
        }
        private void CreateRows()
        {
          foreach (KeyValuePair<int, LearnableSkill> kvp in player.learnableSkills)
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

          Log.Info("refresh window called from ability change event");
          RefreshWindow();
          RefreshWindowOnAbilityChange();
        }
      }
    }
  }
}
