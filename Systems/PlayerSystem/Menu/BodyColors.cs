using System.Collections.Generic;

using Anvil.API;

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

        NuiBind<string> currentColor = new NuiBind<string>("currentColor");
        NuiBind<int> channelSelection = new NuiBind<int>("channelSelection");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) ? windowRectangles[windowId] : new NuiRect(oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

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
            new NuiLabel
            {
              Value = "Actuelle", Width = 65,
            },
            new NuiButtonImage
            {
              ResRef = currentColor, Margin = 10,
              Width = 25,
              Height = 25
            },
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
          List<NuiElement> groupChildren = new List<NuiElement>();

          NuiRow row = new NuiRow();
          List<NuiElement> rowChildren = new List<NuiElement>();
          
          for (int j = 0; j < 16; j++)
          {          
            NuiButtonImage button = new NuiButtonImage
            {
              ResRef = NWScript.ResManGetAliasFor($"hair{nbButton + 1}", NWScript.RESTYPE_TGA) != "" ? $"hair{nbButton + 1}" : $"leather{nbButton + 1}",
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
            new NuiButton
            {
              Id = "openBodyAppearance",
              Height = 35,
              Width = 350,
              Label = "Modifications corporelles"
            },
            new NuiSpacer {}
          }
        };

        colChildren.Add(buttonRow);

        // Construct the window layout.
        NuiCol root = new NuiCol
        {
          Children = colChildren
        };

        NuiWindow window = new NuiWindow
        {
          Root = root,
          Title = $"Vous contemplez votre reflet dans le miroir",
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
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
    }
  }
}
