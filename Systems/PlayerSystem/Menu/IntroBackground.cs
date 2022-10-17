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
      public class IntroBackgroundWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly Color white = new(255, 255, 255);
        private readonly NuiBind<NuiRect> drawListRect = new("drawListRect");
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<string> displayText = new("text");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> skillName = new("skillName");
        private readonly NuiBind<string> remainingTime = new("remainingTime");
        private readonly NuiBind<string> level = new("level");
        private readonly NuiBind<bool> learnButtonEnabled = new("learnButtonEnabled");

        public IEnumerable<Learnable> currentList;

        public IntroBackgroundWindow(Player player) : base(player)
        {
          windowId = "introBackground";
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

            drawListRect.SetBindValue(player.oid, nuiToken.Token, Utils.GetDrawListTextScaleFromPlayerUI(player));

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            displayText.SetBindValue(player.oid, nuiToken.Token, "L'espace de quelques instants, la galère disparait de votre esprit.\n" +
              "Plus jeune, vous vous revoyez ...\n" +
              "(Attention, il ne sera possible de choisir qu'un seul historique par personnage et il ne sera pas possible de revenir dessus une fois votre choix effectué !)");

            currentList = SkillSystem.learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == SkillSystem.Category.StartingTraits).OrderBy(s => s.name);
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
                LearnableSkill background = new LearnableSkill(((LearnableSkill)currentList.ElementAt(nuiEvent.ArrayIndex)));
                player.learnableSkills.Add(background.id, background);
                background.LevelUp(player);
                player.oid.SendServerMessage($"Vous venez de sélectionner l'historique {background.name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
                CloseWindow();
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
                  currentList = !string.IsNullOrEmpty(currentSearch) ? currentList = currentList.Where(s => s.name.ToLower().Contains(currentSearch)) : SkillSystem.learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == SkillSystem.Category.StartingTraits).OrderBy(s => s.name);
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
          List<bool> learnButtonEnabledList = new List<bool>();

          foreach (Learnable learnable in filteredList)
          {
            iconList.Add(learnable.icon);
            skillNameList.Add(learnable.name);
            remainingTimeList.Add("Gratuit");
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
