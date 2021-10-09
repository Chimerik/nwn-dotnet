using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;

using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateBodyAppearanceWindow()
      {
        string windowId = "BodyAppearanceModifier";
        NuiBind<int> headSelection = new NuiBind<int>("headSelection");
        NuiBind<int> headSlider = new NuiBind<int>("headSlider");
        NuiBind<int> sizeSlider = new NuiBind<int>("sizeSlider");
        NuiBind<int> sizeSelection = new NuiBind<int>("sizeSelection");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiComboEntry> sizeCombo = new List<NuiComboEntry>();

        for (int i = 0; i < 51; i++)
          sizeCombo.Add(new NuiComboEntry($"x{((float)(i + 75))/100}", i));

        // Construct the window layout.
        NuiCol root = new NuiCol
        {
          Children =  new List<NuiElement>
          {
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 60,
                  Value = "Taille",
                },
                new NuiCombo
                {
                   Width = 80,
                   Entries = sizeCombo,
                   Selected = sizeSelection
                },
                new NuiSlider
                {
                    Min = 0, Max = 50, Step = 1,  Width = (windowRectangle.Width - 140)  * 0.96f,
                    Value = sizeSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 60,
                  Value = "Tête",
                },
                new NuiCombo
                {
                   Width = 80,
                   Entries = ModuleSystem.headModels.FirstOrDefault(h => h.gender == oid.ControlledCreature.Gender && h.appearance == oid.ControlledCreature.CreatureAppearanceType).heads,
                   Selected = headSelection
                },
                new NuiSlider
                {
                    Min = 0, Max = ModuleSystem.headModels.FirstOrDefault(h => h.gender == oid.ControlledCreature.Gender && h.appearance == oid.ControlledCreature.CreatureAppearanceType).heads.Count - 1, Step = 1,  Width = (windowRectangle.Width - 140)  * 0.96f,
                    Value = headSlider
                }
              }
            },
          }
        };
        Log.Info($"count : {ModuleSystem.headModels.FirstOrDefault(h => h.gender == oid.ControlledCreature.Gender && h.appearance == oid.ControlledCreature.CreatureAppearanceType).heads.Count}");

        NuiWindow window = new NuiWindow
        {
          Root = root,
          Title = "Vous contemplez votre reflet",
          Geometry = geometry,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
        };

        oid.OnNuiEvent -= HandleBodyAppearanceEvents;
        oid.OnNuiEvent += HandleBodyAppearanceEvents;

        PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);

        int token = oid.CreateNuiWindow(window, windowId);

        headSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.Head));
        headSlider.SetBindValue(oid, token, ModuleSystem.headModels.FirstOrDefault(h => h.gender == oid.ControlledCreature.Gender && h.appearance == oid.ControlledCreature.CreatureAppearanceType).heads.IndexOf(ModuleSystem.headModels.FirstOrDefault(h => h.gender == oid.ControlledCreature.Gender && h.appearance == oid.ControlledCreature.CreatureAppearanceType).heads.FirstOrDefault(l => l.Value == oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.Head))));

        sizeSelection.SetBindValue(oid, token, (int)oid.ControlledCreature.VisualTransform.Scale * 100 - 75);
        sizeSlider.SetBindValue(oid, token, (int)oid.ControlledCreature.VisualTransform.Scale * 100 - 75);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        Task waitWindowOpened = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.6));

          headSelection.SetBindWatch(oid, token, true);
          headSlider.SetBindWatch(oid, token, true);

          sizeSelection.SetBindWatch(oid, token, true);
          sizeSlider.SetBindWatch(oid, token, true);
        });
      }
    }
  }
}
