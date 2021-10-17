using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateFishingMiniGameWindow()
      {
        NuiBind<NuiColor> color = new NuiBind<NuiColor>("color");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey("chat") ? windowRectangles["chat"] : new NuiRect(oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiElement> colChildren = new List<NuiElement>();

        // Construct the window layout.
        NuiColumn root = new NuiColumn
        {
          Children = new List<NuiElement>
          {
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiColorPicker(color) { },
              }
            },
          }
        };

        NuiWindow window = new NuiWindow(root, "")
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
        };

        oid.OnNuiEvent -= HandleFishingMiniGameEvents;
        oid.OnNuiEvent += HandleFishingMiniGameEvents;

        int token = oid.CreateNuiWindow(window, "fishingMiniGame");

        color.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
      private static void HandleFishingMiniGameEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "fishingMiniGame")
          return;

        switch (nuiEvent.ElementId)
        {
          case "color":

            switch (nuiEvent.EventType)
            {
              case NuiEventType.Watch:

                Log.Info($"color : {new NuiBind<NuiColor>("color").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken)}");
                break;
            }

            break;
        }
      }
    }
  }
}
