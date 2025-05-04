using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false };

        private NwCreature target;
        private Player targetPlayer;

        public FicheDePersoWindow(Player player, NwCreature target) : base(player)
        {
          windowId = "ficheDePerso";
          windowWidth = player.guiScaledWidth * 0.3f;
          windowHeight = player.guiScaledHeight * 0.9f;
          rootColumn.Children = rootChildren;
          rootGroup.Layout = rootColumn;

          menuRow.Height = windowWidth / 10;
          menuGroup.Height = menuRow.Height;
          menuGroup.Layout = menuRow;

          menuRow.Children.Add(new NuiSpacer());
          menuRow.Children.Add(new NuiButtonImage("sheet_main_view") { Id = "sheetMainView", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiButtonImage("sheet_conditions") { Id = "sheetConditions", Height = windowWidth / 12, Width = windowWidth / 12 });
          menuRow.Children.Add(new NuiSpacer());

          conditionsTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiButtonImage(conditionIcon) { }) { Width = windowWidth / 12 },
            new(new NuiLabel(conditionName) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { Width = windowWidth * 11 / 12  },
          };

          LoadMainLayout();
          CreateWindow(target);
        }

        public void CreateWindow(NwCreature target)
        {
          this.target = target;
          if(!Players.TryGetValue(target, out targetPlayer))
            targetPlayer = player;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(0, 0, windowWidth, windowHeight);

          window = new NuiWindow(rootGroup, $"{target.Name} - Fiche de perso")
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
            nuiToken.OnNuiEvent += HandleCharacterSheetEvents;

            MainBindings();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleCharacterSheetEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "sheetMainView":

                  LoadMainLayout();
                  MainBindings();

                  break;

                case "sheetConditions":

                  LoadConditionsLayout();
                  ConditionsBindings();

                  break;
              }

              break; 
          }
        }
      }
    }
  }
}
