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
      public class BodyAppearanceWindow : PlayerWindow
      {
        NuiBind<int> headSelection { get; }
        NuiBind<int> headSlider { get; }
        NuiBind<int> sizeSlider { get; }
        NuiBind<int> sizeSelection { get; }

        NuiBind<int> chestSelection { get; }
        NuiBind<int> bicepRightSelection { get; }
        NuiBind<int> forearmRightSelection { get; }
        NuiBind<int> thighRightSelection { get; }
        NuiBind<int> shinRightSelection { get; }
        NuiBind<int> bicepLeftSelection { get; }
        NuiBind<int> forearmLeftSelection { get; }
        NuiBind<int> thighLeftSelection { get; }
        NuiBind<int> shinLeftSelection { get; }
        List<NuiComboEntry> sizeCombo { get; }
        List<NuiComboEntry> comboBicep { get; }
        List<NuiComboEntry> comboChest { get; }

        public BodyAppearanceWindow(Player player) : base(player)
        {
          windowId = "bodyAppearanceModifier";

          headSlider = new NuiBind<int>("headSlider");
          sizeSlider = new NuiBind<int>("sizeSlider");
          sizeSelection = new NuiBind<int>("sizeSelection");

          chestSelection = new NuiBind<int>("chestSelection");
          bicepRightSelection = new NuiBind<int>("bicepRightSelection");
          forearmRightSelection = new NuiBind<int>("forearmRightSelection");
          thighRightSelection = new NuiBind<int>("thighRightSelection");
          shinRightSelection = new NuiBind<int>("shinRightSelection");
          bicepLeftSelection = new NuiBind<int>("bicepLeftSelection");
          forearmLeftSelection = new NuiBind<int>("forearmLeftSelection");
          thighLeftSelection = new NuiBind<int>("thighLeftSelection");

          sizeCombo = new List<NuiComboEntry>();

          for (int i = 0; i < 51; i++)
            sizeCombo.Add(new NuiComboEntry($"x{((float)(i + 75)) / 100}", i));

          comboChest = new List<NuiComboEntry> { new NuiComboEntry("1", 1) };

          if (player.oid.ControlledCreature.Gender == Gender.Male)
            comboChest.Add(new NuiComboEntry("2", 2));

          comboBicep = new List<NuiComboEntry>
          {
            new NuiComboEntry("1", 1),
            new NuiComboEntry("2", 2)
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) && player.windowRectangles[windowId].Width > 0 ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          NuiColumn root = new NuiColumn
          {
            Children = new List<NuiElement>
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
                    Entries = ModuleSystem.headModels.FirstOrDefault(h => h.gender == player.oid.ControlledCreature.Gender && h.appearance == player.oid.ControlledCreature.CreatureAppearanceType).heads,
                    Selected = headSelection
                  },
                  new NuiSlider(headSlider, 0, ModuleSystem.headModels.FirstOrDefault(h => h.gender == player.oid.ControlledCreature.Gender && h.appearance == player.oid.ControlledCreature.CreatureAppearanceType).heads.Count - 1)
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

          window = new NuiWindow(root, "Vous contemplez votre reflet")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleBodyAppearanceEvents;
          player.oid.OnNuiEvent += HandleBodyAppearanceEvents;

          if (player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasNothing)
          {
            PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 173);
            player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Value = true;
          }

          token = player.oid.CreateNuiWindow(window, windowId);

          headSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.Head));
          headSlider.SetBindValue(player.oid, token, ModuleSystem.headModels.FirstOrDefault(h => h.gender == player.oid.ControlledCreature.Gender && h.appearance == player.oid.ControlledCreature.CreatureAppearanceType).heads.IndexOf(ModuleSystem.headModels.FirstOrDefault(h => h.gender == player.oid.ControlledCreature.Gender && h.appearance == player.oid.ControlledCreature.CreatureAppearanceType).heads.FirstOrDefault(l => l.Value == player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.Head))));

          sizeSelection.SetBindValue(player.oid, token, (int)player.oid.ControlledCreature.VisualTransform.Scale * 100 - 75);
          sizeSlider.SetBindValue(player.oid, token, (int)player.oid.ControlledCreature.VisualTransform.Scale * 100 - 75);

          chestSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.Torso));
          bicepRightSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightBicep));
          bicepLeftSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftBicep));
          forearmRightSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightForearm));
          forearmLeftSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftForearm));
          thighRightSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightThigh));
          thighLeftSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftThigh));
          shinRightSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.RightShin));
          shinLeftSelection.SetBindValue(player.oid, token, player.oid.ControlledCreature.GetCreatureBodyPart(CreaturePart.LeftShin));

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          Task waitWindowOpened = Task.Run(async () =>
          {
            await Task.Delay(TimeSpan.FromSeconds(0.6));

            headSelection.SetBindWatch(player.oid, token, true);
            headSlider.SetBindWatch(player.oid, token, true);

            sizeSelection.SetBindWatch(player.oid, token, true);
            sizeSlider.SetBindWatch(player.oid, token, true);

            chestSelection.SetBindWatch(player.oid, token, true);
            bicepRightSelection.SetBindWatch(player.oid, token, true);
            bicepLeftSelection.SetBindWatch(player.oid, token, true);
            forearmRightSelection.SetBindWatch(player.oid, token, true);
            forearmLeftSelection.SetBindWatch(player.oid, token, true);
            thighRightSelection.SetBindWatch(player.oid, token, true);
            thighLeftSelection.SetBindWatch(player.oid, token, true);
            shinRightSelection.SetBindWatch(player.oid, token, true);
            shinLeftSelection.SetBindWatch(player.oid, token, true);
          });

          player.openedWindows[windowId] = token;
        }
        private void HandleBodyAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (player.oid.NuiGetWindowId(token) != windowId)
            return;

          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.EnableItemAppearanceFeedbackMessages();

            if (player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").HasValue)
            {
              PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 173);
              player.oid.ControlledCreature.GetObjectVariable<LocalVariableBool>("SPOTLIGHT_ON").Delete();
            }

            return;
          }

          if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "openColors")
          {
            player.oid.NuiDestroy(token);

            if (player.windows.ContainsKey("bodyColorsModifier"))
              ((BodyColorWindow)player.windows["bodyColorsModifier"]).CreateWindow();
            else
              player.windows.Add("bodyColorsModifier", new BodyColorWindow(player));

            return;
          }

          if (nuiEvent.EventType == NuiEventType.Watch)
            switch (nuiEvent.ElementId)
            {
              case "headSlider":

                int head = ModuleSystem.headModels.FirstOrDefault(h => h.gender == player.oid.ControlledCreature.Gender && h.appearance == player.oid.ControlledCreature.CreatureAppearanceType).heads.ElementAt(headSlider.GetBindValue(player.oid, token)).Value;
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.Head, head);
                headSelection.SetBindWatch(player.oid, token, false);
                headSelection.SetBindValue(player.oid, token, head);
                headSelection.SetBindWatch(player.oid, token, true);

                break;

              case "headSelection":

                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, token));
                headSlider.SetBindWatch(player.oid, token, false);
                headSlider.SetBindValue(player.oid, token, ModuleSystem.headModels.FirstOrDefault(h => h.gender == player.oid.ControlledCreature.Gender && h.appearance == player.oid.ControlledCreature.CreatureAppearanceType).heads.ElementAt(headSelection.GetBindValue(player.oid, token)).Value);
                headSlider.SetBindWatch(player.oid, token, true);

                break;

              case "sizeSlider":
                
                player.oid.ControlledCreature.VisualTransform.Scale = (float)(sizeSlider.GetBindValue(player.oid, token) + 75) / 100;
                sizeSelection.SetBindWatch(player.oid, token, false);
                sizeSelection.SetBindValue(player.oid, token, sizeSlider.GetBindValue(player.oid, token));
                sizeSelection.SetBindWatch(player.oid, token, true);

                break;

              case "sizeSelection":
  
                player.oid.ControlledCreature.VisualTransform.Scale = (sizeSelection.GetBindValue(player.oid, token) + 75) / 100;
                sizeSlider.SetBindWatch(player.oid, token, false);
                sizeSlider.SetBindValue(player.oid, token, sizeSelection.GetBindValue(player.oid, token));
                sizeSlider.SetBindWatch(player.oid, token, true);

                break;

              case "chestSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.Torso, chestSelection.GetBindValue(player.oid, token));
                break;

              case "bicepRightSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightBicep, bicepRightSelection.GetBindValue(player.oid, token));
                break;

              case "bicepLeftSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftBicep, bicepLeftSelection.GetBindValue(player.oid, token));
                break;

              case "forearmRightSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightForearm, forearmRightSelection.GetBindValue(player.oid, token));
                break;

              case "forearmLeftSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftForearm, forearmLeftSelection.GetBindValue(player.oid, token));
                break;

              case "thighRightSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightThigh, thighRightSelection.GetBindValue(player.oid, token));
                break;

              case "thighLeftSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftThigh, thighLeftSelection.GetBindValue(player.oid, token));
                break;

              case "shinRightSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.RightShin, shinRightSelection.GetBindValue(player.oid, token));
                break;

              case "shinLeftSelection":
                player.oid.ControlledCreature.SetCreatureBodyPart(CreaturePart.LeftShin, shinLeftSelection.GetBindValue(player.oid, token));
                break;
            }
        }
      }
    }
  }
}
