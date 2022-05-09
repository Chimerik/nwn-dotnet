using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ArmorColorWindow : PlayerWindow
      {
        private NwItem item { get; set; }
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChildren = new ();
        private readonly NuiBind<bool> symmetry = new ("symmetry");
        private readonly NuiBind<string> currentColor = new ("currentColor");
        private readonly NuiBind<int> channelSelection = new ("channelSelection");
        private readonly NuiBind<int> spotSelection = new ("spotSelection");
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
        private readonly List<NuiComboEntry> spotCombo = new ()
        {
          new NuiComboEntry("Global", 0),
          new NuiComboEntry("Robe", 19),
          new NuiComboEntry("Cou", 10),
          new NuiComboEntry("Torse", 8),
          new NuiComboEntry("Pelvis", 7),
          new NuiComboEntry("Ceinture", 9),
          new NuiComboEntry("Epaule droite", 15),
          new NuiComboEntry("Epaule gauche", 16),
          new NuiComboEntry("Biceps droit", 13),
          new NuiComboEntry("Biceps gauche", 14),
          new NuiComboEntry("Avant-bras droit", 11),
          new NuiComboEntry("Avant-bras gauche", 12),
          new NuiComboEntry("Main droite", 17),
          new NuiComboEntry("Main gauche", 18),
          new NuiComboEntry("Cuisse droite", 5),
          new NuiComboEntry("Cuisse gauche", 6),
          new NuiComboEntry("Tibia droit", 3),
          new NuiComboEntry("Tibia gauche", 4),
          new NuiComboEntry("Pied droit", 1),
          new NuiComboEntry("Pied gauche", 2),
        };
        public ArmorColorWindow(Player player, NwItem item) : base(player)
        {
          windowId = "itemColorsModifier";

          for (int i = 0; i < 256; i++)
            colorBindings[i] = new NuiBind<string>($"color{i}");

          NuiRow comboRow = new NuiRow()
          {
            Children = new List<NuiElement>
            {
              new NuiLabel("Actuelle") { Width = 65 },
              new NuiButtonImage(currentColor) { Width = 25, Height = 25, Margin = 10 },
              new NuiCombo
              {
                Id = "colorChannel", Width = 120,
                Entries = comboChannel,
                Selected = channelSelection
              },
              new NuiCombo
              {
                Id = "spotCombo", Width = 135,
                Entries = spotCombo,
                Selected = spotSelection
              },
              new NuiCheck("Symétrie", symmetry) { Id = "applySymmetry" }
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

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, $"Modification des couleurs de {item.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleItemColorsEvents;
          player.oid.OnNuiEvent += HandleItemColorsEvents;

          player.ActivateSpotLight(player.oid.ControlledCreature);

          token = player.oid.CreateNuiWindow(window, windowId);

          currentColor.SetBindValue(player.oid, token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather1)}");
          channelSelection.SetBindValue(player.oid, token, 0);
          spotSelection.SetBindValue(player.oid, token, 0);
          symmetry.SetBindValue(player.oid, token, false);

          channelSelection.SetBindWatch(player.oid, token, true);
          spotSelection.SetBindWatch(player.oid, token, true);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          for (int i = 0; i < 256; i++)
            colorBindings[i].SetBindValue(player.oid, token, NWScript.ResManGetAliasFor($"leather{i + 1}", NWScript.RESTYPE_TGA));

          player.openedWindows[windowId] = token;
        }
        private void HandleItemColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "itemColorsModifier")
            return;

          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.RemoveSpotLight(player.oid.ControlledCreature);
            player.EnableItemAppearanceFeedbackMessages();
            return;
          }

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            player.EnableItemAppearanceFeedbackMessages();
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              if (nuiEvent.ElementId == "openItemAppearance")
              {
                CloseWindow();

                if (player.windows.ContainsKey("itemAppearanceModifier"))
                  ((ArmorAppearanceWindow)player.windows["itemAppearanceModifier"]).CreateWindow(item);
                else
                  player.windows.Add("itemAppearanceModifier", new ArmorAppearanceWindow(player, item));

                return;
              }

              int spot = spotSelection.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken) - 1;
              ItemAppearanceArmorColor colorChanel = (ItemAppearanceArmorColor)channelSelection.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
              switch (spot)
              {
                case -1:
                  item.Appearance.SetArmorColor(colorChanel, byte.Parse(nuiEvent.ElementId));
                  break;
                default:

                  int modelSymmetry = spot;
                  if (symmetry.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken) && (spot < 6 || (spot > 9 && spot < 18)))
                  {
                    if (spot % 2 == 0)
                      modelSymmetry += 1;
                    else
                      modelSymmetry -= 1;
                  };

                  item.Appearance.SetArmorPieceColor((ItemAppearanceArmorModel)spot, colorChanel, byte.Parse(nuiEvent.ElementId));
                  if (spot != modelSymmetry)
                    item.Appearance.SetArmorPieceColor((ItemAppearanceArmorModel)modelSymmetry, colorChanel, byte.Parse(nuiEvent.ElementId));

                  break;
              }

              NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);
              nuiEvent.Player.ControlledCreature.RunEquip(newItem, InventorySlot.Chest);
              item.Destroy();
              item = newItem;

              currentColor.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, $"leather{int.Parse(nuiEvent.ElementId) + 1}");

              break;

            case NuiEventType.Watch:

              if (nuiEvent.ElementId == "channelSelection")
              {
                string channelChoice = "leather";
                ItemAppearanceArmorColor selectedChannel = (ItemAppearanceArmorColor)channelSelection.GetBindValue(player.oid, token);
                if (selectedChannel == ItemAppearanceArmorColor.Metal1 || selectedChannel == ItemAppearanceArmorColor.Metal2)
                  channelChoice = "metal";

                for (int i = 0; i < 4; i++)
                  colorBindings[i].SetBindValue(player.oid, token, NWScript.ResManGetAliasFor($"{channelChoice}{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{i + 1}" : $"leather{i + 1}");

                int newCurrentColor = item.Appearance.GetArmorColor(selectedChannel) + 1;
                currentColor.SetBindValue(player.oid, token, NWScript.ResManGetAliasFor($"{channelChoice}{newCurrentColor}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{newCurrentColor}" : $"leather{newCurrentColor}");
              }

              if (nuiEvent.ElementId == "channelSelection" || nuiEvent.ElementId == "spotSelection")
              {
                string channelChoice = "leather";
                ItemAppearanceArmorColor selectedChannel = (ItemAppearanceArmorColor)channelSelection.GetBindValue(player.oid, token);
                if (selectedChannel == ItemAppearanceArmorColor.Metal1 || selectedChannel == ItemAppearanceArmorColor.Metal2)
                  channelChoice = "metal";

                int selectedSpot = spotSelection.GetBindValue(nuiEvent.Player, nuiEvent.WindowToken) - 1;
                int color;

                switch (selectedSpot)
                {
                  case -1:
                    color = item.Appearance.GetArmorColor(selectedChannel) + 1;
                    break;
                  default:
                    color = item.Appearance.GetArmorPieceColor((ItemAppearanceArmorModel)selectedSpot, selectedChannel) + 1;
                    break;
                }

                currentColor.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, NWScript.ResManGetAliasFor($"{channelChoice}{color}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{color}" : $"leather{color}");
              }
              break;
          }
        }
      }
    }
  }
}
