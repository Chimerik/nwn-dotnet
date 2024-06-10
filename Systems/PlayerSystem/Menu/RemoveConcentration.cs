using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class RemoveConcentrationWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        public RemoveConcentrationWindow(Player player) : base(player)
        {
          windowId = "removeConcentration";
          windowWidth = player.guiScaledWidth * 0.18f;
          windowHeight = player.guiScaledHeight * 0.1f;
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Oui") { Id = "dispel", Height = windowHeight * 0.4f, Width = windowWidth / 3 },
            new NuiButton("Non") { Id = "cancel", Height = windowHeight * 0.4f, Width = windowWidth / 3 },
            new NuiSpacer()
          } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.45f, player.guiHeight * 0.25f, windowWidth, windowHeight);

          window = new NuiWindow(rootColumn, "Annuler votre concentration ?")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleConcentrationEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleConcentrationEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "dispel": SpellUtils.DispelConcentrationEffects(player.oid.LoginCreature); break;
              }

              CloseWindow();

              break;
          }
        }
      }
    }
  }
}
