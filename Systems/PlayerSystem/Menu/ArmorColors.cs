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
      public void CreateArmorColorsWindow(NwItem item)
      {
        string windowId = "itemColorsModifier";
        NuiBind<bool> symmetry = new NuiBind<bool>("symmetry");
        NuiBind<string> currentColor = new NuiBind<string>("currentColor");
        NuiBind<int> channelSelection = new NuiBind<int>("channelSelection");
        NuiBind<int> spotSelection = new NuiBind<int>("spotSelection");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) ? windowRectangles[windowId] : new NuiRect(oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiComboEntry> comboChannel = new List<NuiComboEntry>
        {
          new NuiComboEntry("Cuir 1", 0),
          new NuiComboEntry("Cuir 2", 1),
          new NuiComboEntry("Tissu 1", 2),
          new NuiComboEntry("Tissu 2", 3),
          new NuiComboEntry("Métal 1", 4),
          new NuiComboEntry("Métal 2", 5)
        };

        List<NuiComboEntry> spotCombo = new List<NuiComboEntry>
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

        List<NuiElement> colChildren = new List<NuiElement>();

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

        colChildren.Add(comboRow);

        int nbButton = 0;

        for (int i = 0; i < 16; i++)
        {
          NuiGroup paletteGroup = new NuiGroup();
          paletteGroup.Id = $"paletteGroup{i}"; paletteGroup.Height = 26; paletteGroup.Margin = 0; paletteGroup.Padding = 0; paletteGroup.Scrollbars = NuiScrollbars.None; paletteGroup.Border = false;

          NuiRow row = new NuiRow();
          List<NuiElement> rowChildren = new List<NuiElement>();

          for (int j = 0; j < 16; j++)
          {
            NuiButtonImage button = new NuiButtonImage($"leather{nbButton + 1}")
            {
              Id = $"{nbButton}", Width = 25, Height = 25
            };

            rowChildren.Add(button);
            nbButton++;
          }

          row.Children = rowChildren;
          paletteGroup.Layout = row;
          colChildren.Add(paletteGroup);
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

        colChildren.Add(buttonRow);

        // Construct the window layout.
        NuiColumn root = new NuiColumn { Children = colChildren };

        NuiWindow window = new NuiWindow(root, $"Modification des couleurs de {item.Name}")
        {
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
        };

        oid.OnNuiEvent -= HandleItemColorsEvents;
        oid.OnNuiEvent += HandleItemColorsEvents;

        PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);

        int token = oid.CreateNuiWindow(window, windowId);

        currentColor.SetBindValue(oid, token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather1)}");
        channelSelection.SetBindValue(oid, token, 0);
        spotSelection.SetBindValue(oid, token, 0);
        symmetry.SetBindValue(oid, token, false);

        channelSelection.SetBindWatch(oid, token, true);
        spotSelection.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
      private void HandleItemColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "itemColorsModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        if (nuiEvent.EventType == NuiEventType.Close)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(nuiEvent.Player.ControlledCreature, nuiEvent.Player.ControlledCreature, 173);
          return;
        }

        NwItem item = nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value;

        if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
        {
          nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
          nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
          return;
        }

        switch (nuiEvent.EventType)
        {
          case NuiEventType.Click:

            if (nuiEvent.ElementId == "openItemAppearance")
            {
              nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
              player.CreateArmorAppearanceWindow(item);
              return;
            }

            int spotSelection = new NuiBind<int>("spotSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken) - 1;
            ItemAppearanceArmorColor colorChanel = (ItemAppearanceArmorColor)new NuiBind<int>("channelSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
            switch (spotSelection)
            {
              case -1:
                item.Appearance.SetArmorColor(colorChanel, byte.Parse(nuiEvent.ElementId));
                break;
              default:

                int modelSymmetry = spotSelection;
                if (new NuiBind<bool>("symmetry").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken) && (spotSelection < 6 || (spotSelection > 9 && spotSelection < 18)))
                {
                  if (spotSelection % 2 == 0)
                    modelSymmetry += 1;
                  else
                    modelSymmetry -= 1;
                };

                item.Appearance.SetArmorPieceColor((ItemAppearanceArmorModel)spotSelection, colorChanel, byte.Parse(nuiEvent.ElementId));
                if (spotSelection != modelSymmetry)
                  item.Appearance.SetArmorPieceColor((ItemAppearanceArmorModel)modelSymmetry, colorChanel, byte.Parse(nuiEvent.ElementId));

                break;
            }

            NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);
            nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = newItem;
            nuiEvent.Player.ControlledCreature.RunEquip(newItem, InventorySlot.Chest);
            item.Destroy();


            new NuiBind<string>("currentColor").SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, $"leather{int.Parse(nuiEvent.ElementId) + 1}");

            break;

          case NuiEventType.Watch:

            ItemAppearanceArmorColor channel = (ItemAppearanceArmorColor)new NuiBind<int>("channelSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);

            string channelChoice = "leather";
            if (channel == ItemAppearanceArmorColor.Metal1 || channel == ItemAppearanceArmorColor.Metal2)
              channelChoice = "metal";

            if (nuiEvent.ElementId == "channelSelection")
            {
              int nbButton = 0;

              for (int i = 0; i < 4; i++)
              {
                NuiGroup paletteGroup = new NuiGroup();
                paletteGroup.Id = $"paletteGroup{i}"; paletteGroup.Height = 26; paletteGroup.Margin = 0; paletteGroup.Padding = 0; paletteGroup.Scrollbars = NuiScrollbars.None; paletteGroup.Border = false;

                NuiRow row = new NuiRow();
                List<NuiElement> rowChildren = new List<NuiElement>();

                for (int j = 0; j < 16; j++)
                {
                  NuiButtonImage button = new NuiButtonImage(NWScript.ResManGetAliasFor($"{channelChoice}{nbButton + 1}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{nbButton + 1}" : $"leather{nbButton + 1}")
                  {
                    Id = $"{nbButton}",
                    Width = 25,
                    Height = 25
                  };

                  rowChildren.Add(button);
                  nbButton++;
                }

                row.Children = rowChildren;
                paletteGroup.Layout = row;
                paletteGroup.SetLayout(player.oid, nuiEvent.WindowToken, row);
              }
            }

            if (nuiEvent.ElementId == "channelSelection" || nuiEvent.ElementId == "spotSelection")
            {
              int spot = new NuiBind<int>("spotSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken) - 1;
              int currentColor;
             
              switch (spot)
              {
                case -1:
                  currentColor = ((int)item.Appearance.GetArmorColor(channel)) + 1;
                  break;
                default:
                  currentColor = ((int)item.Appearance.GetArmorPieceColor((ItemAppearanceArmorModel)spot, channel)) + 1;
                  break;
              }

              new NuiBind<string>("currentColor").SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, NWScript.ResManGetAliasFor($"{channelChoice}{currentColor}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{currentColor}" : $"leather{currentColor}");
            }
            break;
        }
      }
    }
  }
}
