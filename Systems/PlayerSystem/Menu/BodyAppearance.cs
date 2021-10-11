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
      public void CreateBodyAppearanceWindow()
      {
        string windowId = "BodyAppearanceModifier";
        NuiBind<int> headSelection = new NuiBind<int>("headSelection");
        NuiBind<int> headSlider = new NuiBind<int>("headSlider");
        NuiBind<int> sizeSlider = new NuiBind<int>("sizeSlider");
        NuiBind<int> sizeSelection = new NuiBind<int>("sizeSelection");

        NuiBind<int> chestSelection = new NuiBind<int>("chestSelection");
        NuiBind<int> bicepRightSelection = new NuiBind<int>("bicepRightSelection");
        NuiBind<int> forearmRightSelection = new NuiBind<int>("forearmRightSelection");
        NuiBind<int> thighRightSelection = new NuiBind<int>("thighRightSelection");
        NuiBind<int> shinRightSelection = new NuiBind<int>("shinRightSelection");
        NuiBind<int> bicepLeftSelection = new NuiBind<int>("bicepLeftSelection");
        NuiBind<int> forearmLeftSelection = new NuiBind<int>("forearmLeftSelection");
        NuiBind<int> thighLeftSelection = new NuiBind<int>("thighLeftSelection");
        NuiBind<int> shinLeftSelection = new NuiBind<int>("shinLeftSelection");

        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        List<NuiComboEntry> sizeCombo = new List<NuiComboEntry>();

        for (int i = 0; i < 51; i++)
          sizeCombo.Add(new NuiComboEntry($"x{((float)(i + 75))/100}", i));

        List<NuiComboEntry> comboChest = new List<NuiComboEntry> { new NuiComboEntry("1", 1) };

        if (oid.ControlledCreature.Gender == Gender.Male)
          comboChest.Add(new NuiComboEntry("2", 2));

        List<NuiComboEntry> comboBicep = new List<NuiComboEntry> 
        { 
          new NuiComboEntry("1", 1),
          new NuiComboEntry("2", 2)
        };

        // Construct the window layout.
        NuiColumn root = new NuiColumn
        {
          Children =  new List<NuiElement>
          {
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiSpacer { },
                new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 70 },
                new NuiSpacer { }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Taille") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle  },
                new NuiCombo
                {
                   Width = 80,
                   Entries = sizeCombo,
                   Selected = sizeSelection
                },
                new NuiSlider(sizeSlider, 0, 50) { Step = 1,  Width = (windowRectangle.Width - 140)  * 0.98f }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Tête") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = ModuleSystem.headModels.FirstOrDefault(h => h.gender == oid.ControlledCreature.Gender && h.appearance == oid.ControlledCreature.CreatureAppearanceType).heads,
                  Selected = headSelection
                },
                new NuiSlider(headSlider, 0, ModuleSystem.headModels.FirstOrDefault(h => h.gender == oid.ControlledCreature.Gender && h.appearance == oid.ControlledCreature.CreatureAppearanceType).heads.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 140)  * 0.98f,
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Torse") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboChest,
                  Selected = chestSelection
                },
                new NuiLabel("Biceps droit") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = bicepRightSelection
                },
                new NuiLabel("Biceps gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = bicepLeftSelection
                },
                new NuiLabel("Avant-bras droit") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = forearmRightSelection
                },
                new NuiLabel("Avant-bras gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = forearmLeftSelection
                },
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Cuisse droite") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = thighRightSelection
                },
                new NuiLabel("Cuisse gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = thighLeftSelection
                },
                new NuiLabel("Tibia droit") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = shinRightSelection
                },
                new NuiLabel("Tibia gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo
                {
                  Width = 80,
                  Entries = comboBicep,
                  Selected = shinLeftSelection
                }
              }
            }
          }
        };

        NuiWindow window = new NuiWindow(root, "Vous contemplez votre reflet")
        {
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

        chestSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.Torso));
        bicepRightSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightBicep));
        bicepLeftSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftBicep));
        forearmRightSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightForearm));
        forearmLeftSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftForearm));
        thighRightSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightThigh));
        thighLeftSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftThigh));
        shinRightSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightShin));
        shinLeftSelection.SetBindValue(oid, token, oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftShin));

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        Task waitWindowOpened = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.6));

          headSelection.SetBindWatch(oid, token, true);
          headSlider.SetBindWatch(oid, token, true);

          sizeSelection.SetBindWatch(oid, token, true);
          sizeSlider.SetBindWatch(oid, token, true);

          chestSelection.SetBindWatch(oid, token, true);
          bicepRightSelection.SetBindWatch(oid, token, true);
          bicepLeftSelection.SetBindWatch(oid, token, true);
          forearmRightSelection.SetBindWatch(oid, token, true);
          forearmLeftSelection.SetBindWatch(oid, token, true);
          thighRightSelection.SetBindWatch(oid, token, true);
          thighLeftSelection.SetBindWatch(oid, token, true);
          shinRightSelection.SetBindWatch(oid, token, true);
          shinLeftSelection.SetBindWatch(oid, token, true);
        });
      }
      private void HandleBodyAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "BodyAppearanceModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;
        
        if (nuiEvent.EventType == NuiEventType.Close)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(nuiEvent.Player.ControlledCreature, nuiEvent.Player.ControlledCreature, 173);
          return;
        }

        if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "openColors")
        {
          nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
          player.CreateBodyColorsWindow();
          return;
        }

        if (nuiEvent.EventType == NuiEventType.Watch)
          switch (nuiEvent.ElementId)
          {
            case "headSlider":
              int headSliderSelection = new NuiBind<int>("headSlider").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
              int head = ModuleSystem.headModels.FirstOrDefault(h => h.gender == nuiEvent.Player.ControlledCreature.Gender && h.appearance == nuiEvent.Player.ControlledCreature.CreatureAppearanceType).heads.ElementAt(headSliderSelection).Value;
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.Head, head);

              NuiBind<int> headSelector = new NuiBind<int>("headSelection");
              headSelector.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
              headSelector.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, head);
              headSelector.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);

              break;

            case "headSelection":
              int headSelected = new NuiBind<int>("headSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.Head, headSelected);

              NuiBind<int> headSlider = new NuiBind<int>("headSlider");
              headSlider.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
              headSlider.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, ModuleSystem.headModels.FirstOrDefault(h => h.gender == nuiEvent.Player.ControlledCreature.Gender && h.appearance == nuiEvent.Player.ControlledCreature.CreatureAppearanceType).heads.ElementAt(headSelected).Value);
              headSlider.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);

              break;

            case "sizeSlider":
              int sizeSliderSelection = new NuiBind<int>("sizeSlider").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
              nuiEvent.Player.ControlledCreature.VisualTransform.Scale = ((float)(sizeSliderSelection + 75) / 100);

              NuiBind<int> sizeSelector = new NuiBind<int>("sizeSelection");
              sizeSelector.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
              sizeSelector.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, sizeSliderSelection);
              sizeSelector.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);

              break;

            case "sizeSelection":
              int sizeSelected = new NuiBind<int>("sizeSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken);
              nuiEvent.Player.ControlledCreature.VisualTransform.Scale = (float)((sizeSelected + 75) / 100);

              NuiBind<int> sizeSlider = new NuiBind<int>("sizeSlider");
              sizeSlider.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, false);
              sizeSlider.SetBindValue(nuiEvent.Player, nuiEvent.WindowToken, sizeSelected);
              sizeSlider.SetBindWatch(nuiEvent.Player, nuiEvent.WindowToken, true);

              break;

            case "chestSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.Torso, new NuiBind<int>("chestSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "bicepRightSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightBicep, new NuiBind<int>("bicepRightSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "bicepLeftSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftBicep, new NuiBind<int>("bicepLeftSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "forearmRightSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightForearm, new NuiBind<int>("forearmRightSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "forearmLeftSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftForearm, new NuiBind<int>("forearmLeftSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "thighRightSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightThigh, new NuiBind<int>("thighRightSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "thighLeftSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftThigh, new NuiBind<int>("thighLeftSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "shinRightSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightShin, new NuiBind<int>("shinRightSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;

            case "shinLeftSelection":
              nuiEvent.Player.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftShin, new NuiBind<int>("shinLeftSelection").GetBindValue(nuiEvent.Player, nuiEvent.WindowToken));
              break;
          }
      }
    }
  }
}
