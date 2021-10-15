using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateSimpleItemAppearanceWindow(NwItem item)
      {
        string modelKey = BaseItems2da.baseItemTable.GetBaseItemDataEntry(item.BaseItemType).defaultIcon;

        if (string.IsNullOrEmpty(modelKey))
        {
          oid.SendServerMessage($"Il n'existe actuellement pas de modèle modifiable pour {item.Name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
          return;
        }

        string windowId = "simpleItemAppearanceModifier";
        NuiBind<string> title = new NuiBind<string>("title");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 && windowRectangles[windowId].Width <= oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiElement> colChildren = new List<NuiElement>();

        int i = 0;
        NuiRow row = new NuiRow();
        row.Children = new List<NuiElement>();

        foreach (int model in BaseItems2da.baseItemTable.simpleItemModels[modelKey])
        {
          if (i == 3)
          {
            colChildren.Add(row);
            row = new NuiRow();
            row.Children = new List<NuiElement>();
            i = 0;
          }

          row.Children.Add(
            new NuiButtonImage($"{modelKey}_{model.ToString().PadLeft(3, '0')}")
            {
              Id = $"{model}", Margin = 0, Padding = 0, Width = 75, Height = 100
            });

          i++;
        }

        NuiColumn root = new NuiColumn
        {
          Children = colChildren
        };

        NuiWindow window = new NuiWindow(root, title)
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
        };

        oid.OnNuiEvent -= HandleSimpleItemAppearanceEvents;
        oid.OnNuiEvent += HandleSimpleItemAppearanceEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        title.SetBindValue(oid, token, $"Modifier l'apparence de {item.Name}");
        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }

      private void HandleSimpleItemAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "simpleItemAppearanceModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        if (nuiEvent.EventType == NuiEventType.Click)
        {
          NwItem item = nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value;

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
            return;
          }

          item.Appearance.SetSimpleModel(byte.Parse(nuiEvent.ElementId));
          NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);

          for (int i = 0; i < 13; i++)
          {
            if (player.oid.LoginCreature.GetItemInSlot((InventorySlot)i) == item)
            {
              player.oid.LoginCreature.RunEquip(newItem, (InventorySlot)i);
              break;
            }
          }

          item.Destroy();
        }
      }
    }
  }
}
