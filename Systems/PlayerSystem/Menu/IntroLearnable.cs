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
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly Color white = new(255, 255, 255);
        private readonly NuiRect drawListRect = new(0, 35, 150, 60);
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<string> displayText = new("text");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> skillName = new("skillName");
        private readonly NuiBind<string> remainingTime = new("remainingTime");
        private readonly NuiBind<string> level = new("level");
        private readonly NuiBind<bool> learnButtonEnabled = new("learnButtonEnabled");

        public IEnumerable<LearnableSkill> currentList;

        public IntroLearnableWindow(Player player) : base(player)
        {
          windowId = "introLearnables";
          rootColumn.Children = rootChidren;

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
            new NuiListTemplateCell(new NuiButton("Apprendre") { Id = "learn", Enabled = learnButtonEnabled, Tooltip = "Si vous ne disposez pas d'assez de points, cette compétence sera sélectionnée pour entrainement.", Height = 40, Width = 90 }) { Width = 90 }
          };

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiText(displayText) { Height = 130 } } });
          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 420 } } });
          rootChidren.Add(new NuiList(learnableTemplate, listCount) { RowHeight = 40, Width = 420 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, "Sélection des compétences de départ")
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

            currentList = player.learnableSkills.Values;
            LoadLearnableList(currentList);
          }
        }

        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (nuiEvent.ElementId.StartsWith("learn"))
              {
                LearnableSkill skill = currentList.ElementAt(nuiEvent.ArrayIndex);

                if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value >= skill.pointsToNextLevel)
                {
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value -= (int)skill.pointsToNextLevel;
                  skill.LevelUp(player);

                  displayText.SetBindValue(player.oid, nuiToken.Token, $"Vous disposez actuellement de {player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value} points de compétence.\n\n" +
                  $"Quelles capacités initiales votre personnage possède-t-il ?");

                  LoadLearnableList(currentList);
                }
                else
                {
                  currentList.ElementAt(nuiEvent.ArrayIndex).StartLearning(player);

                  if (!player.windows.ContainsKey("activeLearnable")) player.windows.Add("activeLearnable", new ActiveLearnableWindow(player));
                  else ((ActiveLearnableWindow)player.windows["activeLearnable"]).CreateWindow();

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Delete();
                  player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>("_GO").Value = 1;
                  player.oid.SendServerMessage("Vous avez épuisé vos points de compétence de création de personnage. Vous pouvez dès à présent commencer l'aventure en parlant au capitaine !");

                  CloseWindow();
                }
              }
              else
              {
                Learnable skill = currentList.ElementAt(nuiEvent.ArrayIndex);

                if (!player.windows.ContainsKey("learnableDescription")) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, skill.id));
                else ((LearnableDescriptionWindow)player.windows["learnableDescription"]).CreateWindow(skill.id);
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":
                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  currentList = !string.IsNullOrEmpty(currentSearch) ? currentList.Where(s => s.name.ToLower().Contains(currentSearch)) : player.learnableSkills.Values;
                  LoadLearnableList(currentList);
                  break;
              }

              break;
          }
        }
        public void LoadLearnableList(IEnumerable<LearnableSkill> filteredList)
        {
          List<string> iconList = new List<string>();
          List<string> skillNameList = new List<string>();
          List<string> remainingTimeList = new List<string>();
          List<string> levelList = new List<string>();
          List<bool> learnButtonEnabledList = new List<bool>();

          foreach (LearnableSkill learnable in filteredList)
          {
            iconList.Add(learnable.icon);
            skillNameList.Add(learnable.name);
            remainingTimeList.Add(learnable.pointsToNextLevel.ToString());
            levelList.Add($"{learnable.currentLevel}/{learnable.maxLevel}");
            learnButtonEnabledList.Add(learnable.currentLevel < learnable.maxLevel);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          skillName.SetBindValues(player.oid, nuiToken.Token, skillNameList);
          remainingTime.SetBindValues(player.oid, nuiToken.Token, remainingTimeList);
          level.SetBindValues(player.oid, nuiToken.Token, levelList);
          learnButtonEnabled.SetBindValues(player.oid, nuiToken.Token, learnButtonEnabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
      }
    }
  }
}
