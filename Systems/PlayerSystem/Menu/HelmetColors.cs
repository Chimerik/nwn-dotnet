using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class HelmetColorWindow : PlayerWindow
      {
        private NwItem item { get; set; }
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChildren = new ();
        private readonly NuiBind<string> currentColor = new ("currentColor");
        private readonly NuiBind<int> channelSelection = new ("channelSelection");
        private readonly NuiBind<string>[] colorBindings = new NuiBind<string>[256];
        private readonly List<NuiComboEntry> comboChannel = new ()
        {
          new NuiComboEntry("Cuir 1", 0),
          new NuiComboEntry("Cuir 2", 1),
          new NuiComboEntry("Tissu 1", 2),
          new NuiComboEntry("Tissu 2", 3),
          new NuiComboEntry("Métal 1", 4),
          new NuiComboEntry("Métal 2", 5)
        };

        public HelmetColorWindow(Player player, NwItem item) : base(player)
        {
          windowId = "helmetColorsModifier";

          for (int i = 0; i < 256; i++)
            colorBindings[i] = new ($"color{i}");

          NuiRow comboRow = new NuiRow()
          {
            Children = new List<NuiElement>
            {
              new NuiSpacer { },
              new NuiLabel("Actuelle") { Width = 65, VerticalAlign = NuiVAlign.Middle},
              new NuiButtonImage(currentColor) { Width = 25, Height = 25, Margin = 10 },
              new NuiCombo
              {
                Id = "colorChannel", Width = 120,
                Entries = comboChannel,
                Selected = channelSelection
              },
              new NuiSpacer { }
            }
          };

          rootChildren.Add(comboRow);

          int nbButton = 0;

          for (int i = 0; i < 16; i++)
          {
            NuiRow row = new NuiRow();
            List<NuiElement> rowChildren = new List<NuiElement>();

            for (int j = 0; j < 16; j++)
            {
              NuiButtonImage button = new NuiButtonImage(colorBindings[nbButton])
              {
                Id = $"{nbButton}",
                Width = 25,
                Height = 25
              };

              rowChildren.Add(button);
              nbButton++;
            }

            row.Children = rowChildren;
            rootChildren.Add(row);
          }

          NuiRow buttonRow = new NuiRow()
          {
            Children = new List<NuiElement>
            {
              new NuiSpacer {},
              new NuiButton("Apparence")
              {
                Id = "openItemAppearance",
                Width = 80, Height = 35,
              },
              new NuiSpacer {}
            }
          };

          rootChildren.Add(buttonRow);
          rootColumn = new NuiColumn { Children = rootChildren };

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          this.item = item;
          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 4, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 2);

          window = new NuiWindow(rootColumn, $"Modification des couleurs de {item.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          player.ActivateSpotLight(player.oid.ControlledCreature);

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleHelmetColorsEvents;

            currentColor.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather1)}");
            channelSelection.SetBindValue(player.oid, nuiToken.Token, 0);
            channelSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            for (int i = 0; i < 256; i++)
              colorBindings[i].SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"leather{i + 1}", NWScript.RESTYPE_TGA));
          }           
        }
        private void HandleHelmetColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.EnableItemAppearanceFeedbackMessages();
            player.RemoveSpotLight(player.oid.ControlledCreature);
            return;
          }

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (nuiEvent.ElementId == "openItemAppearance")
              {
                CloseWindow();

                if (player.windows.ContainsKey("helmetAppearanceModifier"))
                  ((HelmetAppearanceWindow)player.windows["helmetAppearanceModifier"]).CreateWindow(item);
                else
                  player.windows.Add("helmetAppearanceModifier", new HelmetAppearanceWindow(player, item));

                return;
              }

              ItemAppearanceArmorColor colorChanel = (ItemAppearanceArmorColor)channelSelection.GetBindValue(nuiEvent.Player, nuiToken.Token);
              item.Appearance.SetArmorColor(colorChanel, byte.Parse(nuiEvent.ElementId));

              NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);
              nuiEvent.Player.ControlledCreature.RunEquip(newItem, InventorySlot.Head);
              item.Destroy();
              item = newItem;

              currentColor.SetBindValue(nuiEvent.Player, nuiToken.Token, $"leather{int.Parse(nuiEvent.ElementId) + 1}");

              break;

            case NuiEventType.Watch:

              if (nuiEvent.ElementId == "channelSelection")
              {
                string channelChoice = "leather";
                ItemAppearanceArmorColor selectedChannel = (ItemAppearanceArmorColor)channelSelection.GetBindValue(player.oid, nuiToken.Token);
                if (selectedChannel == ItemAppearanceArmorColor.Metal1 || selectedChannel == ItemAppearanceArmorColor.Metal2)
                  channelChoice = "metal";

                for (int i = 0; i < 4; i++)
                  colorBindings[i].SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"{channelChoice}{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{i + 1}" : $"leather{i + 1}");

                int newCurrentColor = item.Appearance.GetArmorColor(selectedChannel) + 1;
                currentColor.SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"{channelChoice}{newCurrentColor}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{newCurrentColor}" : $"leather{newCurrentColor}");
              }

              break;
          }
        }
      }
    }
  }
}
