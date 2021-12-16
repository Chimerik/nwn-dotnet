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
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        NuiRow buttonRow { get; }
        NuiRow searchRow { get; }
        NuiRow textRow { get; }
        List<NuiElement> rootChidren { get; }
        NuiBind<string> search { get; }
        NuiColor white { get; }
        NuiRect drawListRect { get; }
        NuiBind<string> displayText { get; }

        public IntroLearnableWindow(Player player) : base(player)
        {
          windowId = "introLearnable";

          displayText = new NuiBind<string>("text");
          white = new NuiColor(255, 255, 255);
          drawListRect = new NuiRect(0, 35, 150, 60);
          search = new NuiBind<string>("search");

          rootChidren = new List<NuiElement>();
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

          player.oid.OnNuiEvent -= HandleLearnableEvents;
          player.oid.OnNuiEvent += HandleLearnableEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          search.SetBindValue(player.oid, token, "");
          search.SetBindWatch(player.oid, token, true);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          displayText.SetBindValue(player.oid, token, $"Vous disposez actuellement de {player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value} points de compétence.\n\n" +
            $"Quelles capacités initiales votre personnage possède-t-il ?");

          player.openedWindows[windowId] = token;
          RefreshWindow();
        }

        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (nuiEvent.ElementId == "retour")
              {
                player.oid.NuiDestroy(token);
                if (!player.openedWindows.ContainsKey("introMirror"))
                  ((IntroMirroWindow)player.windows["introMirror"]).CreateWindow();

                return;
              }

              if (nuiEvent.ElementId.StartsWith("learn_"))
              {
                int learnableId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf("_") + 1));
                LearnableSkill skill = player.learnableSkills[learnableId];

                if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value >= skill.GetPointsToNextLevel())
                {
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value -= (int)skill.GetPointsToNextLevel();
                  skill.LevelUp(player);

                  displayText.SetBindValue(player.oid, token, $"Vous disposez actuellement de {player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value} points de compétence.\n\n" +
                  $"Quelles capacités initiales votre personnage possède-t-il ?");

                  RefreshWindow();
                }
                else
                {
                  player.oid.NuiDestroy(token);
                  player.oid.SendServerMessage("Vous avez épuisé vos points de compétence de création de personnage. Vous pouvez dès à présent commencer l'aventure en parlant au capitaine !");

                  if (player.openedWindows.ContainsKey("activeLearnable"))
                    player.oid.NuiDestroy(player.openedWindows["activeLearnable"]);

                  if (player.windows.ContainsKey("activeLearnable"))
                    ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow(learnableId);
                  else
                    player.windows.Add("activeLearnable", new ActiveLearnableWindow(player, learnableId));

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Delete();
                  player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_GO").Value = 1;
                }
              }
              else if (int.TryParse(nuiEvent.ElementId, out int learnableId))
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

          if (token < 0)
            return;

          CreateSkillRows();

          rootGroup.SetLayout(player.oid, token, rootColumn);
        }
        private void CreateSkillRows()
        {
          string currentSearch = search.GetBindValue(player.oid, token).ToLower();
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
