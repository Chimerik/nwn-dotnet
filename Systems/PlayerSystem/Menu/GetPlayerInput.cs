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
        NuiColumn rootColumn { get; }
        private string title { get; set; }
        private string quantity { get; set; }
        private Func<string, bool> handler { get; set; }
        private readonly NuiBind<string> input = new NuiBind<string>("input");

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

          player.oid.OnNuiEvent -= HandlePlayerInputEvents;
          player.oid.OnNuiEvent += HandlePlayerInputEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          input.SetBindValue(player.oid, token, quantity);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);
        }

        private void HandlePlayerInputEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "confirm")
          {
            handler(input.GetBindValue(player.oid, token));
            player.oid.NuiDestroy(token);
          }
        }
      }
    }
  }
}
