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
            new NuiColumn
            { 
              Children = new List<NuiElement>
              {
                new NuiGroup() 
                { 
                  Id = "somegroupid" ,
                  Children = new List<NuiElement>
                  {
                    new NuiButton("Test Update") { Id = "testUpdate", Tooltip = "test" }
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
      private void HandleFishingMiniGameEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        switch (nuiEvent.ElementId)
        {
          case "testUpdate":

            if (nuiEvent.EventType == NuiEventType.Click)
            {
              int nbClick = nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("NUI_TEST_UPDATE").Value + 1;
              nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("NUI_TEST_UPDATE").Value = nbClick;

              NuiGroup group = new NuiGroup()
              {
                Id = "somegroupid",
                Children = new List<NuiElement>
                  {
                    new NuiButton($"Test Update {nbClick}") { Id = "testUpdate", Tooltip = "test" }
                  }
              };

              oid.NuiSetGroupLayout(nuiEvent.WindowToken, "somegroupid", group);
            }

            break;
        }
      }
    }
  }
}
