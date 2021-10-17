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
      public void CreateHelmetColorsWindow(NwItem item)
      {
        string windowId = "helmetColorsModifier";
        DisableItemAppearanceFeedbackMessages();
        NuiBind<string> currentColor = new NuiBind<string>("currentColor");
        NuiBind<int> channelSelection = new NuiBind<int>("channelSelection");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) ? windowRectangles[windowId] : new NuiRect(oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 4, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 2);

        List<NuiComboEntry> comboChannel = new List<NuiComboEntry>
        {
          new NuiComboEntry("Cuir 1", 0),
          new NuiComboEntry("Cuir 2", 1),
          new NuiComboEntry("Tissu 1", 2),
          new NuiComboEntry("Tissu 2", 3),
          new NuiComboEntry("Métal 1", 4),
          new NuiComboEntry("Métal 2", 5)
        };

        List<NuiElement> colChildren = new List<NuiElement>();

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

        colChildren.Add(comboRow);

        int nbButton = 0;

        for (int i = 0; i < 16; i++)
        {
          NuiGroup paletteGroup = new NuiGroup();
          paletteGroup.Id = $"paletteGroup{i}"; paletteGroup.Height = 26; paletteGroup.Margin = 0; paletteGroup.Padding = 0; paletteGroup.Scrollbars = NuiScrollbars.None; paletteGroup.Border = false;
          List<NuiElement> groupChildren = new List<NuiElement>();

          NuiRow row = new NuiRow();
          List<NuiElement> rowChildren = new List<NuiElement>();

          for (int j = 0; j < 16; j++)
          {
            NuiButtonImage button = new NuiButtonImage($"leather{nbButton + 1}")
            {
              Id = $"{nbButton}",
              Width = 25,
              Height = 25
            };

            rowChildren.Add(button);
            nbButton++;
          }

          row.Children = rowChildren;
          groupChildren.Add(row);
          paletteGroup.Children = groupChildren;
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

        oid.OnNuiEvent -= HandleHelmetColorsEvents;
        oid.OnNuiEvent += HandleHelmetColorsEvents;

        PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);

        int token = oid.CreateNuiWindow(window, windowId);

        currentColor.SetBindValue(oid, token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather1)}");
        channelSelection.SetBindValue(oid, token, 0);

        channelSelection.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
      private void HandleHelmetColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "helmetColorsModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        if (nuiEvent.EventType == NuiEventType.Close)
        {
          EnableItemAppearanceFeedbackMessages();
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
              player.CreateHelmetAppearanceWindow(item);
              return;
            }

            ItemAppearanceArmorColor colorChanel = (ItemAppearanceArmorColor)new NuiBind<int>("channelSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
            item.Appearance.SetArmorColor(colorChanel, byte.Parse(nuiEvent.ElementId));
            
            NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);
            nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value = newItem;
            nuiEvent.Player.ControlledCreature.RunEquip(newItem, InventorySlot.Head);
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
                List<NuiElement> groupChildren = new List<NuiElement>();

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
                groupChildren.Add(row);
                paletteGroup.Children = groupChildren;
                nuiEvent.Player.NuiSetGroupLayout(nuiEvent.WindowToken, paletteGroup.Id, paletteGroup);
              }
            }

            if (nuiEvent.ElementId == "channelSelection" )
            {
              int currentColor = ((int)item.Appearance.GetArmorColor(channel)) + 1;
              new NuiBind<string>("currentColor").SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, NWScript.ResManGetAliasFor($"{channelChoice}{currentColor}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{currentColor}" : $"leather{currentColor}");
            }
            break;
        }
      }
    }
  }
}
