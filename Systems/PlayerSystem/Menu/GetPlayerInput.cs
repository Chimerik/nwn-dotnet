using System;
using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class PlayerInputWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private string title { get; set; }
        private string quantity { get; set; }
        private Func<string, bool> handler { get; set; }
        private readonly NuiBind<string> input = new ("input");

        public PlayerInputWindow(Player player, string windowTitle, Func<string, bool> handler , string availableQuantity = "") : base(player)
        {
          windowId = "playerInput";

          rootColumn = new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("", input, 60, false) } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Confirmer") { Id = "confirm" } } }
            }
          };

          CreateWindow(windowTitle, handler, availableQuantity);
        }

        public void CreateWindow(string windowTitle, Func<string, bool> handler, string availableQuantity = "")
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) - 40, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) - 30, 80, 80);

          title = windowTitle;
          quantity = availableQuantity;
          this.handler = handler;

          window = new NuiWindow(rootColumn, title)
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandlePlayerInputEvents;

            input.SetBindValue(player.oid, nuiToken.Token, quantity);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandlePlayerInputEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "confirm")
          {
            handler(input.GetBindValue(player.oid, nuiToken.Token));
            CloseWindow();
          }
        }
      }
    }
  }
}
