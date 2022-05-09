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
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<int> headSelection = new ("headSelection");
        private readonly NuiBind<int> headSlider = new ("headSlider");
        private readonly NuiBind<int> sizeSlider = new ("sizeSlider");
        private readonly NuiBind<int> sizeSelection = new ("sizeSelection");

        private readonly NuiBind<int> chestSelection = new ("chestSelection");
        private readonly NuiBind<int> bicepRightSelection = new ("bicepRightSelection");
        private readonly NuiBind<int> forearmRightSelection = new ("forearmRightSelection");
        private readonly NuiBind<int> thighRightSelection = new ("thighRightSelection");
        private readonly NuiBind<int> shinRightSelection = new ("shinRightSelection");
        private readonly NuiBind<int> bicepLeftSelection = new ("bicepLeftSelection");
        private readonly NuiBind<int> forearmLeftSelection = new ("forearmLeftSelection");
        private readonly NuiBind<int> thighLeftSelection = new ("thighLeftSelection");
        private readonly NuiBind<int> shinLeftSelection = new ("shinLeftSelection");

        private readonly NuiSlider sizeSliderWidget;
        private readonly NuiSlider headSliderWidget;

        private readonly List<NuiComboEntry> comboChest = new () { new NuiComboEntry("1", 1) };
        private readonly List<NuiComboEntry> comboBicep = new ()
          {
            new NuiComboEntry("1", 1),
            new NuiComboEntry("2", 2)
          };

        private NwCreature targetCreature;

        public BodyAppearanceWindow(Player player, NwCreature targetCreature) : base(player)
        {
          windowId = "bodyAppearanceModifier";

          sizeSliderWidget = new(sizeSlider, 0, 50) { Step = 1 };
          headSliderWidget = new(headSlider, 0, ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads.Count - 1) { Step = 1 };

          rootColumn.Children = rootChildren;
          rootChildren.Add(new NuiRow { Children = new List<NuiElement> { new NuiSpacer { }, new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 70 }, new NuiSpacer { } } });
          rootChildren.Add(new NuiRow { Children = new List<NuiElement> 
          { 
            new NuiLabel("Taille") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = Utils.sizeList, Selected = sizeSelection },
            sizeSliderWidget
          } });
          rootChildren.Add(new NuiRow  { Children = new List<NuiElement>
          {
            new NuiLabel("Tête") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads, Selected = headSelection },
            headSliderWidget
          } });
          rootChildren.Add(new NuiRow { Children = new List<NuiElement> {
            new NuiLabel("Torse") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboChest, Selected = chestSelection },
            new NuiLabel("Biceps droit") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = bicepRightSelection },
            new NuiLabel("Biceps gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = bicepLeftSelection },
            new NuiLabel("Avant-bras droit") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = forearmRightSelection },
            new NuiLabel("Avant-bras gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = forearmLeftSelection }
          } });
          rootChildren.Add(new NuiRow { Children = new List<NuiElement> {
            new NuiLabel("Cuisse droite") { Width = 60, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = thighRightSelection },
            new NuiLabel("Cuisse gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = thighLeftSelection },
            new NuiLabel("Tibia droit") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = shinRightSelection },
            new NuiLabel("Tibia gauche") { Width = 120, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiCombo { Width = 80, Entries = comboBicep, Selected = shinLeftSelection }
          } });

          CreateWindow(targetCreature);
        }
        public void CreateWindow(NwCreature targetCreature)
        {
          this.targetCreature = targetCreature;
          player.DisableItemAppearanceFeedbackMessages();
          CloseWindow();

          if (targetCreature.Gender == Gender.Male && comboChest.Count < 2)
            comboChest.Add(new NuiComboEntry("2", 2));
          else if (targetCreature.Gender == Gender.Female && comboChest.Count > 1)
            comboChest.RemoveAt(1);

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          sizeSliderWidget.Width = (windowRectangle.Width - 140) * 0.98f;
          headSliderWidget.Width = (windowRectangle.Width - 140) * 0.98f;

          window = new NuiWindow(rootColumn, $"{targetCreature.Name} - Modification corporelle")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true
          };

          player.oid.OnNuiEvent -= HandleBodyAppearanceEvents;
          player.oid.OnNuiEvent += HandleBodyAppearanceEvents;

          player.ActivateSpotLight(targetCreature);

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            headSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.Head));
            headSlider.SetBindValue(player.oid, nuiToken.Token, ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads.IndexOf(ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads.FirstOrDefault(l => l.Value == targetCreature.GetCreatureBodyPart(CreaturePart.Head))));

            sizeSelection.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.VisualTransform.Scale * 100 - 75);
            sizeSlider.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.VisualTransform.Scale * 100 - 75);

            chestSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.Torso));
            bicepRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightBicep));
            bicepLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftBicep));
            forearmRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightForearm));
            forearmLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftForearm));
            thighRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightThigh));
            thighLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftThigh));
            shinRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightShin));
            shinLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftShin));

            headSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            headSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            sizeSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            sizeSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            chestSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            bicepRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            bicepLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            forearmRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            forearmLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            thighRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            thighLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            shinRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            shinLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            player.openedWindows[windowId] = nuiToken.Token;
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleBodyAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (player.oid.NuiGetWindowId(nuiToken.Token) != windowId)
            return;

          if (targetCreature == null)
          {
            player.oid.SendServerMessage("La créature éditée n'est plus valide.", ColorConstants.Red);
            player.EnableItemAppearanceFeedbackMessages();
            CloseWindow();
            return;
          }

          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.EnableItemAppearanceFeedbackMessages();
            player.RemoveSpotLight(targetCreature);

            return;
          }

          if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "openColors")
          {
            CloseWindow();

            if (player.windows.ContainsKey("bodyColorsModifier"))
              ((BodyColorWindow)player.windows["bodyColorsModifier"]).CreateWindow(targetCreature);
            else
              player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, targetCreature));

            return;
          }

          if (nuiEvent.EventType == NuiEventType.Watch)
            switch (nuiEvent.ElementId)
            {
              case "headSlider":

                int head = ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads.ElementAt(headSlider.GetBindValue(player.oid, nuiToken.Token)).Value;
                targetCreature.SetCreatureBodyPart(CreaturePart.Head, head);
                headSelection.SetBindWatch(player.oid, nuiToken.Token, false);
                headSelection.SetBindValue(player.oid, nuiToken.Token, head);
                headSelection.SetBindWatch(player.oid, nuiToken.Token, true);

                break;

              case "headSelection":

                targetCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                headSlider.SetBindWatch(player.oid, nuiToken.Token, false);
                headSlider.SetBindValue(player.oid, nuiToken.Token, ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads.ElementAt(headSelection.GetBindValue(player.oid, nuiToken.Token)).Value);
                headSlider.SetBindWatch(player.oid, nuiToken.Token, true);

                break;

              case "sizeSlider":

                targetCreature.VisualTransform.Scale = (float)(sizeSlider.GetBindValue(player.oid, nuiToken.Token) + 75) / 100;
                sizeSelection.SetBindWatch(player.oid, nuiToken.Token, false);
                sizeSelection.SetBindValue(player.oid, nuiToken.Token, sizeSlider.GetBindValue(player.oid, nuiToken.Token));
                sizeSelection.SetBindWatch(player.oid, nuiToken.Token, true);

                break;

              case "sizeSelection":

                targetCreature.VisualTransform.Scale = (sizeSelection.GetBindValue(player.oid, nuiToken.Token) + 75) / 100;
                sizeSlider.SetBindWatch(player.oid, nuiToken.Token, false);
                sizeSlider.SetBindValue(player.oid, nuiToken.Token, sizeSelection.GetBindValue(player.oid, nuiToken.Token));
                sizeSlider.SetBindWatch(player.oid, nuiToken.Token, true);

                break;

              case "chestSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.Torso, chestSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "bicepRightSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.RightBicep, bicepRightSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "bicepLeftSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.LeftBicep, bicepLeftSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "forearmRightSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.RightForearm, forearmRightSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "forearmLeftSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.LeftForearm, forearmLeftSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "thighRightSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.RightThigh, thighRightSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "thighLeftSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.LeftThigh, thighLeftSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "shinRightSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.RightShin, shinRightSelection.GetBindValue(player.oid, nuiToken.Token));
                break;

              case "shinLeftSelection":
                targetCreature.SetCreatureBodyPart(CreaturePart.LeftShin, shinLeftSelection.GetBindValue(player.oid, nuiToken.Token));
                break;
            }
        }
      }
    }
  }
}
