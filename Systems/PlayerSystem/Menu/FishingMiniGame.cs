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
        NuiRect windowRectangle = new NuiRect(oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);
        NuiBind<NuiRect> circle = new NuiBind<NuiRect>("circle");
        // Construct the window layout.
        NuiColumn root = new NuiColumn
        {
          Children = new List<NuiElement>
          {
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiCheck("Figer", true) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran", Width = 60 },
                new NuiRow
                {
                  Children = new List<NuiElement>
                  {
                    new NuiSpacer()
                {
                  DrawList = new List<NuiDrawListItem>
                  {
                    /*new NuiDrawListArc
                    {
                      Color = new NuiColor(255, 50, 0), //Rect = circle,
                      AngleMax = 160,
                      AngleMin = 0,
                      LineThickness = 50,
                      Enabled = true,
                      Radius = 60,
                      Center = new NuiVector(150, 150),
                      Fill = true
                    },*/
                    new NuiDrawListText(new NuiColor(150, 50, 200), new NuiRect(0, 0, 25, 25), "a")
                    {

                    },
                    new NuiDrawListText(new NuiColor(150, 50, 200), new NuiRect(25, 25, 25, 25), "b")
                    {

                    },
                    new NuiDrawListImage("menu_exit", new NuiRect(50, 50, 25, 25))
                    {
                        Aspect = NuiAspect.Fill,
                    },
                    new NuiDrawListImage("menu_up", new NuiRect(75, 75, 25, 25))
                    {
                        Aspect = NuiAspect.Fill
                    },
                  }
                }
                  }
                }
               }
            }
          }
        };

        NuiWindow window = new NuiWindow(root, "")
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = false,
          Border = true,
        };

        oid.OnNuiEvent -= HandleFishingMiniGameEvents;
        oid.OnNuiEvent += HandleFishingMiniGameEvents;

        int token = oid.CreateNuiWindow(window, "fishingMiniGame");

        color.SetBindWatch(oid, token, true);
        circle.SetBindValue(oid, token, new NuiRect(50, 50, 150, 150));

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
