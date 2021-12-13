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
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        NuiRow buttonRow { get; }
        NuiRow searchRow { get; }
        NuiRow textRow { get; }
        List<NuiElement> rootChidren { get; }
        NuiBind<string> search { get; }
        NuiColor white { get; }
        NuiRect drawListRect { get; }

        public IntroBackgroundWindow(Player player) : base(player)
        {
          windowId = "introBackground";

          white = new NuiColor(255, 255, 255);
          drawListRect = new NuiRect(0, 35, 150, 60);

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "learnableGroup", Border = true, Layout = rootColumn };

          buttonRow = new NuiRow()
            {
              Children = new List<NuiElement>() {
              new NuiButton("Retour") { Id = "retour", Width = 193}
            }
          };

          textRow = new NuiRow()
          {
            Children = new List<NuiElement>() {
              new NuiText("L'espace de quelques instants, la galère disparait de votre esprit.\n" +
              "Plus jeune, vous vous revoyez ...\n" +
              "(Attention, il ne sera possible de choisir qu'un seul historique par personnage et il ne sera pas possible de revenir dessus une fois votre choix effectué !)")
            }
          };

          searchRow = new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 388 } } };

          CreateWindow();
        }
        public void CreateWindow()
        {
          if (player.learnableSkills.Any(s => s.Value.category == SkillSystem.Category.StartingTraits))
          {
            player.oid.SendServerMessage("Vous avez déjà effectué votre choix d'historique.", ColorConstants.Red);

            if(!player.openedWindows.ContainsKey("introMirror"))
              ((IntroMirroWindow)player.windows["introMirror"]).CreateWindow();

            return;
          }

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

          search.SetBindValue(player.oid, token, "");
          search.SetBindWatch(player.oid, token, true);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

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

              if(nuiEvent.ElementId == "retour")
              {
                player.oid.NuiDestroy(token);
                if (!player.openedWindows.ContainsKey("introMirror"))
                  ((IntroMirroWindow)player.windows["introMirror"]).CreateWindow();

                return;
              }

              if (nuiEvent.ElementId.StartsWith("learn_"))
              {
                int learnableId = int.Parse(nuiEvent.ElementId.Substring(nuiEvent.ElementId.IndexOf("_") + 1));

                LearnableSkill background = new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[learnableId]);
                player.learnableSkills.Add(learnableId, background);
                background.LevelUp(player);

                player.oid.SendServerMessage($"Vous venez de sélectionner l'historique {background.name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
                player.oid.NuiDestroy(token);
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
          var filteredList = SkillSystem.learnableDictionary.Where(s => s.Value is LearnableSkill skill && skill.category == SkillSystem.Category.StartingTraits).AsEnumerable();

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
                new NuiDrawListText(white, drawListRect, "Instantané") } },
                new NuiLabel("Niveau/Max") { Id = kvp.Key.ToString(), Width = 90, HorizontalAlign = NuiHAlign.Left, DrawList = new List<NuiDrawListItem>() {
                new NuiDrawListText(white, drawListRect, $"{kvp.Value.currentLevel}/{kvp.Value.maxLevel}") } },
                new NuiButton("Sélectionner") { Id = $"learn_{kvp.Key}", Height = 40, Width = 90, Enabled = kvp.Value.currentLevel < kvp.Value.maxLevel && !kvp.Value.active, Tooltip = "Cet historique sera sélectionné et avancera directement au niveau 1." }
              }
            };

            rootChidren.Add(row);
          }
        }
      }
    }
  }
}
