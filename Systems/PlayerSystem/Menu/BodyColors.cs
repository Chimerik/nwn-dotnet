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
      public void CreateBodyColorsWindow()
      {
        string windowId = "bodyColorsModifier";
        EnableItemAppearanceFeedbackMessages();
        NuiBind<string> currentColor = new NuiBind<string>("currentColor");
        NuiBind<int> channelSelection = new NuiBind<int>("channelSelection");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 ? windowRectangles[windowId] : new NuiRect(oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiComboEntry> comboChannel = new List<NuiComboEntry>
        {
          new NuiComboEntry("Cheveux", 1),
          new NuiComboEntry("Peau", 0),
          new NuiComboEntry("Yeux / Tattoo 1", 3),
          new NuiComboEntry("Lèvres / Tattoo 2", 2),
        };

        List<NuiElement> colChildren = new List<NuiElement>();

        NuiRow comboRow = new NuiRow()
        {
          Children = new List<NuiElement>
          {
            new NuiSpacer { },
            new NuiLabel("Actuelle") { Width = 65, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiButtonImage(currentColor) { Margin = 10, Width = 25, Height = 25 },
            new NuiCombo
            {
              Id = "colorChannel", Width = 240,
              Entries = comboChannel,
              Selected = channelSelection
            },
            new NuiSpacer { }
          }
        };

        colChildren.Add(comboRow);

        int nbButton = 0;

        for (int i = 0; i < 11; i++)
        {
          NuiGroup paletteGroup = new NuiGroup();
          paletteGroup.Id = $"paletteGroup{i}"; paletteGroup.Height = 26; paletteGroup.Margin = 0; paletteGroup.Padding = 0; paletteGroup.Scrollbars = NuiScrollbars.None; paletteGroup.Border = false;

          NuiRow row = new NuiRow();
          List<NuiElement> rowChildren = new List<NuiElement>();
          
          for (int j = 0; j < 16; j++)
          {          
            NuiButtonImage button = new NuiButtonImage(NWScript.ResManGetAliasFor($"hair{nbButton + 1}", NWScript.RESTYPE_TGA) != "" ? $"hair{nbButton + 1}" : $"leather{nbButton + 1}")
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
            new NuiButton("Modifications corporelles") { Id = "openBodyAppearance", Height = 35, Width = 350 },
            new NuiSpacer {}
          }
        };

        colChildren.Add(buttonRow);

        // Construct the window layout.
        NuiColumn root = new NuiColumn
        {
          Children = colChildren
        };

        NuiWindow window = new NuiWindow(root, "Vous contemplez votre reflet dans le miroir")
        {
          Geometry = geometry,
          Resizable = true, Collapsed = false, Closable = true, Transparent = true, Border = true,
        };

        oid.OnNuiEvent -= HandleBodyColorsEvents;
        oid.OnNuiEvent += HandleBodyColorsEvents;

        PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);

        int token = oid.CreateNuiWindow(window, windowId);

        currentColor.SetBindValue(oid, token, $"hair{oid.ControlledCreature.GetColor(ColorChannel.Hair) + 1}");
        channelSelection.SetBindValue(oid, token, 1);
        channelSelection.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
      private void HandleBodyColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "bodyColorsModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        if (nuiEvent.EventType == NuiEventType.Close)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(nuiEvent.Player.ControlledCreature, nuiEvent.Player.ControlledCreature, 173);
          EnableItemAppearanceFeedbackMessages();
          return;
        }

        switch (nuiEvent.EventType)
        {
          case NuiEventType.Click:

            if (nuiEvent.ElementId == "openBodyAppearance")
            {
              nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
              player.CreateBodyAppearanceWindow();
              return;
            }

            nuiEvent.Player.ControlledCreature.SetColor((ColorChannel)new NuiBind<int>("channelSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken), int.Parse(nuiEvent.ElementId));

            string chanChoice = "hair";
            if (new NuiBind<int>("channelSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken) != 1)
              chanChoice = "skin";

            new NuiBind<string>("currentColor").SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, NWScript.ResManGetAliasFor($"{chanChoice}{int.Parse(nuiEvent.ElementId) + 1}", NWScript.RESTYPE_TGA) != "" ? $"{chanChoice}{int.Parse(nuiEvent.ElementId) + 1}" : $"leather{int.Parse(nuiEvent.ElementId) + 1}");

            break;

          case NuiEventType.Watch:

            if (nuiEvent.ElementId == "channelSelection")
            {
              string channelChoice = "hair";
              ColorChannel selectedChannel = (ColorChannel)new NuiBind<int>("channelSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
              if (selectedChannel != ColorChannel.Hair)
                channelChoice = "skin";

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

              int currentColor = nuiEvent.Player.ControlledCreature.GetColor(selectedChannel) + 1;
              new NuiBind<string>("currentColor").SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, NWScript.ResManGetAliasFor($"{channelChoice}{currentColor}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{currentColor}" : $"leather{currentColor}");
            }
            break;
        }
      }
    }
  }
}
