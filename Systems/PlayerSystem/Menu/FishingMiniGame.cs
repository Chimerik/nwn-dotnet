using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateFishingMiniGameWindow()
      {
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey("chat") ? windowRectangles["chat"] : new NuiRect(oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiElement> colChildren = new List<NuiElement>();

        int nbButton = 1;

        for (int i = 0; i < 16; i++)
        {
          NuiRow row = new NuiRow();
          List<NuiElement> rowChildren = new List<NuiElement>();

          for (int j = 0; j < 16; j++)
          {
            NuiButtonImage button = new NuiButtonImage($"leather{nbButton}") { Id = $"{nbButton}", Width = 25, Height = 25 };

            rowChildren.Add(button);
            nbButton++;
          }

          row.Children = rowChildren;
          colChildren.Add(row);
        }

        // Construct the window layout.
        NuiColumn root = new NuiColumn { Children = colChildren };

        NuiWindow window = new NuiWindow(root, "")
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
        };

        oid.OnNuiEvent += HandleFishingMiniGameEvents;

        int token = oid.CreateNuiWindow(window, "fishingMiniGame");

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
    }
  }
}
