using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class SimpleItemAppearanceWindow : PlayerWindow
      {
        private NwItem item { get; set; }
        private readonly NuiColumn rootColumn;

        public SimpleItemAppearanceWindow(Player player, NwItem item) : base(player)
        {
          string modelKey = item.BaseItem.DefaultIcon;

          if (string.IsNullOrEmpty(modelKey))
          {
            player.oid.SendServerMessage($"Il n'existe actuellement pas de modèle modifiable pour {item.Name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
            return;
          }

          windowId = "simpleItemAppearanceModifier";

          List<NuiElement> colChildren = new List<NuiElement>();

          colChildren.Add(new NuiRow
          {
            Children = new List<NuiElement>
              {
                new NuiSpacer(),
                new NuiButton("Nom & Description") { Id = "openNameDescription", Height = 35, Width = 150 },
                new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 150 },
                new NuiSpacer()
              }
          });

          int i = 0;
          NuiRow row = new NuiRow();
          row.Children = new List<NuiElement>();

          foreach (int model in BaseItems2da.simpleItemModels[modelKey])
          {
            if (i == 3)
            {
              colChildren.Add(row);
              row = new NuiRow();
              row.Children = new List<NuiElement>();
              i = 0;
            }

            row.Children.Add(new NuiButtonImage($"{modelKey}_{model.ToString().PadLeft(3, '0')}") { Id = $"{model}", Margin = 0, Padding = 0, Width = 75, Height = 100 });

            i++;
          }

          rootColumn = new NuiColumn { Children = colChildren };

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          this.item = item;
          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, $"Modifier l'apparence de {item.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleSimpleItemAppearanceEvents;
          player.oid.OnNuiEvent += HandleSimpleItemAppearanceEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);
        }
        private void HandleSimpleItemAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "simpleItemAppearanceModifier")
            return;

          if (nuiEvent.EventType == NuiEventType.Close)
            player.EnableItemAppearanceFeedbackMessages();

          if (nuiEvent.EventType == NuiEventType.Click)
          {
            if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
            {
              nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
              CloseWindow();
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
            item = newItem;
          }
        }
      }
    }
  }
}
