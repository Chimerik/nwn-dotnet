using System.Collections.Generic;

using Anvil.API;

using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateItemColorsWindow(NwItem item)
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
            new NuiCheck
            {
              Id = "applySymmetry",
              Label = "Symétrie",
              Value = symmetry
            },
          }
        };

        colChildren.Add(comboRow);

        int nbButton = 0;

        for (int i = 0; i < 16; i++)
        {
          NuiRow row = new NuiRow();
          List<NuiElement> rowChildren = new List<NuiElement>();

          for (int j = 0; j < 16; j++)
          {
            NuiButtonImage button = new NuiButtonImage
            {
              ResRef = $"leather{nbButton + 1}",
              Id = $"{nbButton}",
              Width = 25,
              Height = 25
            };

            rowChildren.Add(button);
            nbButton++;
          }

          row.Children = rowChildren;
          colChildren.Add(row);
        }

        NuiRow buttonRow = new NuiRow()
        {
          Children = new List<NuiElement>
          {
            new NuiSpacer {},
            new NuiButton
            {
              Id = "openItemAppearance",
              Height = 35,
              Width = 80,
              Label = "Apparence"
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
          Title = $"Modification des couleurs de {item.Name}",
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
    }
  }
}
