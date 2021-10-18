using System;
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
      public void CreateItemNameDescriptionWindow(NwItem item)
      {
        string windowId = "itemNameDescriptionModifier";
        NuiBind<string> name = new NuiBind<string>("name");
        NuiBind<string> description = new NuiBind<string>("description");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width <= oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        // Construct the window layout.
        NuiColumn root = new NuiColumn
        {
          Children = new List<NuiElement>
          {
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiTextEdit("Nom", name, 30, true) { Height = 50},
                new NuiButton("Valider") { Width = 60, Height = 50, Id = "nameButton", Tooltip = $"Appuyez sur entrer ou valider pour modifier le nom de {item.Name}" }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiTextEdit("Description", description, 3000, true) { Height = 100 },
                new NuiButton("Valider") { Width = 60, Height = 100, Id = "descriptionButton", Tooltip = $"Appuyez sur valider pour modifier la description de {item.Name}" }
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

        oid.OnNuiEvent -= HandleItemNameDescriptionEvents;
        oid.OnNuiEvent += HandleItemNameDescriptionEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        name.SetBindValue(oid, token, item.Name);
        description.SetBindValue(oid, token, item.Description);

        name.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }

      private void HandleItemNameDescriptionEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "itemNameDescriptionModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        NwItem item = nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value;

        if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
        {
          nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
          nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
          return;
        }

        switch(nuiEvent.EventType)
        {
          case NuiEventType.Click:
            switch (nuiEvent.ElementId)
            {
              case "nameButton":
                item.Name = new NuiBind<string>("name").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
                break;
              case "descriptionButton":
                item.Description = new NuiBind<string>("description").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
                break;
            }
            break;
          case NuiEventType.Watch:
            switch (nuiEvent.ElementId)
            {
              case "name":
                NuiBind<string> nameBind = new NuiBind<string>("name");
                string name = nameBind.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

                if (name.Contains(Environment.NewLine))
                {
                  name = name.Replace(Environment.NewLine, "");
                  nameBind.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
                  nameBind.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, name);
                  nameBind.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);

                  item.Name = name;
                }
                break;
            }
            break;
        }
      }
    }
  }
}
