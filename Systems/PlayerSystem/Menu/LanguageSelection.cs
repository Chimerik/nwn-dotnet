using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class LanguageSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly NuiBind<List<NuiComboEntry>> languageList = new NuiBind<List<NuiComboEntry>>("languageList");
        private readonly NuiBind<int> languageSelection = new NuiBind<int>("languageSelection");

        public LanguageSelectionWindow(Player player) : base(player)
        {
          windowId = "areaDescription";
          rootColumn.Children = rootChidren;

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = languageList, Selected = languageSelection, Height = 35, Width = 120, Margin = 0.0f } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 200, 70);

          window = new NuiWindow(rootColumn, "Sélection de langue")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLanguageSelectionEvents;

            languageList.SetBindValue(player.oid, nuiToken.Token, GetPlayerLanguageList());
            languageSelection.SetBindValue(player.oid, nuiToken.Token, player.currentLanguage);

            languageSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private List<NuiComboEntry> GetPlayerLanguageList()
        {
          List<NuiComboEntry> languageList = new List<NuiComboEntry>();

          foreach (var language in player.learnableSkills.Values)
            if (language.category == SkillSystem.Category.Language && language.currentLevel > 0)
              languageList.Add(new NuiComboEntry(language.name, language.id));

          return languageList;
        }
        private void HandleLanguageSelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Watch:

              if (nuiEvent.ElementId == "languageSelection")
                player.currentLanguage = languageSelection.GetBindValue(player.oid, nuiToken.Token);
              break;
          }
        }
      }
    }
  }
}
