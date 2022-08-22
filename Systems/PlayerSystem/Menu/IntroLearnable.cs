using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class IntroLearnableWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly NuiRow buttonRow;
        private readonly NuiRow searchRow;
        private readonly NuiRow textRow;
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<string> search = new ("search");
        private readonly Color white = new(255, 255, 255);
        private readonly NuiRect drawListRect = new(0, 35, 150, 60);
        private readonly NuiBind<string> displayText = new ("text");

        public IntroLearnableWindow(Player player) : base(player)
        {
          windowId = "introLearnable";

          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };

          buttonRow = new NuiRow()
          {
            Children = new List<NuiElement>() {
              new NuiSpacer(),
              new NuiButton("Retour") { Id = "retour", Width = 193 },
              new NuiSpacer()
            }
          };

          textRow = new NuiRow()
          {
            Children = new List<NuiElement>() {
              new NuiText(displayText) { Height = 130 }
            }
          };
          
          searchRow = new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 388 } } };

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          rootChidren.Add(buttonRow);
          rootChidren.Add(textRow);
          rootChidren.Add(searchRow);

          window = new NuiWindow(rootGroup, "Sélection des compétences de départ")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            displayText.SetBindValue(player.oid, nuiToken.Token, $"Vous disposez actuellement de {player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value} points de compétence.\n\n" +
              $"Quelles capacités initiales votre personnage possède-t-il ?");

            RefreshWindow();
          }
        }

        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (nuiEvent.ElementId == "retour")
              {
                CloseWindow();

                if (!player.TryGetOpenedWindow("introMirror", out PlayerWindow introWindow))
                  ((IntroMirroWindow)introWindow).CreateWindow();

                return;
              }

              if (nuiEvent.ElementId.StartsWith("learn_"))
              {
                int learnableId = int.Parse(nuiEvent.ElementId[(nuiEvent.ElementId.IndexOf("_") + 1)..]);
                LearnableSkill skill = player.learnableSkills[learnableId];

                if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value >= skill.GetPointsToNextLevel())
                {
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value -= (int)skill.GetPointsToNextLevel();
                  skill.LevelUp(player);

                  displayText.SetBindValue(player.oid, nuiToken.Token, $"Vous disposez actuellement de {player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value} points de compétence.\n\n" +
                  $"Quelles capacités initiales votre personnage possède-t-il ?");

                  RefreshWindow();
                }
                else
                {
                  CloseWindow();
                  player.oid.SendServerMessage("Vous avez épuisé vos points de compétence de création de personnage. Vous pouvez dès à présent commencer l'aventure en parlant au capitaine !");

                  if (player.TryGetOpenedWindow("activeLearnable", out PlayerWindow activeLearnableWindow))
                    activeLearnableWindow.CloseWindow();

                  player.learnableSkills[learnableId].StartLearning(player);

                  if (!player.windows.ContainsKey("activeLearnable")) player.windows.Add("activeLearnable", new ActiveLearnableWindow(player));
                  else ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow();

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Delete();
                  player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_GO").Value = 1;
                }
              }
              else if (int.TryParse(nuiEvent.ElementId, out int learnableId))
              {
                if (player.TryGetOpenedWindow("learnableDescription", out PlayerWindow descriptionWindow))
                  descriptionWindow.CloseWindow();

                if (!player.windows.ContainsKey("learnableDescription")) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, learnableId));
                else ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(learnableId);
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
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
          rootChidren.Add(textRow);
          rootChidren.Add(searchRow);

          if (nuiToken.Token < 0)
            return;

          CreateSkillRows();

          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
        private void CreateSkillRows()
        {
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          var filteredList = player.learnableSkills.AsEnumerable();

          if (currentSearch != "")
            filteredList = filteredList.Where(s => s.Value.name.ToLower().Contains(currentSearch));

          foreach (var kvp in filteredList)
          {
            NuiRow row = new NuiRow()
            {
              Children = new List<NuiElement>()
              {
                new NuiButtonImage(kvp.Value.icon) { Id = kvp.Key.ToString(), Tooltip = "Description", Height = 40, Width = 40 },
                new NuiLabel(kvp.Value.name) { Id = kvp.Key.ToString(), Tooltip = kvp.Value.name, Width = 160, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, kvp.Value.GetPointsToNextLevel().ToString()) } },
                new NuiLabel("Niveau/Max") { Id = kvp.Key.ToString(), Width = 90, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, $"{kvp.Value.currentLevel}/{kvp.Value.maxLevel}") } },
                new NuiButton("Acheter") { Id = $"learn_{kvp.Key}", Height = 40, Width = 90, Enabled = kvp.Value.currentLevel < kvp.Value.maxLevel && !kvp.Value.active, Tooltip = "Si vous ne disposez pas d'assez de points, cette compétence sera sélectionnée pour entrainement." }
              }
            };

            rootChidren.Add(row);
          }
        }
      }
    }
  }
}
