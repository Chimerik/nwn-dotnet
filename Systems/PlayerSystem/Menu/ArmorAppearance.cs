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
      public class ArmorAppearanceWindow : PlayerWindow
      {
        private readonly NuiBind<int> robeSelection = new ("robeSelection");
        private readonly NuiBind<int> robeSlider = new ("robeSliderValue");
        private readonly NuiBind<int> neckSelection = new ("neckSelection");
        private readonly NuiBind<int> neckSlider = new ("neckSliderValue");
        private readonly NuiBind<int> torsoSelection = new ("torsoSelection");
        private readonly NuiBind<int> torsoSlider = new ("torsoSliderValue");
        private readonly NuiBind<int> beltSelection = new ("beltSelection");
        private readonly NuiBind<int> beltSlider = new ("beltSliderValue");
        private readonly NuiBind<int> pelvisSelection = new ("pelvisSelection");
        private readonly NuiBind<int> pelvisSlider = new ("pelvisSliderValue");
        private readonly NuiBind<int> rightShoulderSelection = new ("rightShoulderSelection");
        private readonly NuiBind<int> rightShoulderSlider = new ("rightShoulderSliderValue");
        private readonly NuiBind<int> leftShoulderSelection = new ("leftShoulderSelection");
        private readonly NuiBind<int> leftShoulderSlider = new ("leftShoulderSliderValue");
        private readonly NuiBind<int> rightBicepSelection = new ("rightBicepSelection");
        private readonly NuiBind<int> rightBicepSlider = new ("rightBicepSliderValue");
        private readonly NuiBind<int> leftBicepSelection = new ("leftBicepSelection");
        private readonly NuiBind<int> leftBicepSlider = new ("leftBicepSliderValue");
        private readonly NuiBind<int> rightForearmSelection = new ("rightForearmSelection");
        private readonly NuiBind<int> rightForearmSlider = new ("rightForearmSliderValue");
        private readonly NuiBind<int> leftForearmSelection = new ("leftForearmSelection");
        private readonly NuiBind<int> leftForearmSlider = new ("leftForearmSliderValue");
        private readonly NuiBind<int> rightHandSelection = new ("rightHandSelection");
        private readonly NuiBind<int> rightHandSlider = new ("rightHandSliderValue");
        private readonly NuiBind<int> leftHandSelection = new ("leftHandSelection");
        private readonly NuiBind<int> leftHandSlider = new ("leftHandSliderValue");
        private readonly NuiBind<int> rightTighSelection = new ("rightTighSelection");
        private readonly NuiBind<int> rightTighSlider = new ("rightTighSliderValue");
        private readonly NuiBind<int> leftTighSelection = new ("leftTighSelection");
        private readonly NuiBind<int> leftTighSlider = new ("leftTighSliderValue");
        private readonly NuiBind<int> rightShinSelection = new ("rightShinSelection");
        private readonly NuiBind<int> rightShinSlider = new ("rightShinSliderValue");
        private readonly NuiBind<int> leftShinSelection = new ("leftShinSelection");
        private readonly NuiBind<int> leftShinSlider = new ("leftShinSliderValue");
        private readonly NuiBind<int> rightFootSelection = new ("rightFootSelection");
        private readonly NuiBind<int> rightFootSlider = new ("rightFootSliderValue");
        private readonly NuiBind<int> leftFootSelection = new ("leftFootSelection");
        private readonly NuiBind<int> leftFootSlider = new ("leftFootSliderValue");
        private readonly NuiBind<bool> symmetry = new ("symmetry");
        private readonly NuiBind<int> areaSelection = new ("areaSelection");
        private List<NuiComboEntry> robeList;
        private List<NuiComboEntry> torsoList;
        private List<NuiComboEntry> pelvisList;
        private List<NuiComboEntry> shoulderList;
        private List<NuiComboEntry> bicepList;
        private List<NuiComboEntry> forearmList;
        private List<NuiComboEntry> handList;
        private List<NuiComboEntry> tighList;
        private List<NuiComboEntry> shinList;
        private List<NuiComboEntry> footList;
        private NwItem item { get; set; }

        public ArmorAppearanceWindow(Player player, NwItem item) : base (player)
        {
          windowId = "itemAppearanceModifier";
          CreateWindow(item);
        }

        public void CreateWindow(NwItem item)
        {
          robeList = player.oid.ControlledCreature.Gender == Gender.Male ? RobeParts2da.maleCombo : RobeParts2da.femaleCombo;
          bicepList = player.oid.ControlledCreature.Gender == Gender.Male ? BicepParts2da.maleCombo : BicepParts2da.femaleCombo;
          footList = player.oid.ControlledCreature.Gender == Gender.Male ? FootParts2da.maleCombo : FootParts2da.femaleCombo;
          forearmList = player.oid.ControlledCreature.Gender == Gender.Male ? ForearmParts2da.maleCombo : ForearmParts2da.femaleCombo;
          handList = player.oid.ControlledCreature.Gender == Gender.Male ? HandParts2da.maleCombo : HandParts2da.femaleCombo;
          tighList = player.oid.ControlledCreature.Gender == Gender.Male ? LegParts2da.maleCombo : LegParts2da.femaleCombo;
          shinList = player.oid.ControlledCreature.Gender == Gender.Male ? ShinParts2da.maleCombo : ShinParts2da.femaleCombo;
          shoulderList = player.oid.ControlledCreature.Gender == Gender.Male ? ShoulderParts2da.maleCombo : ShoulderParts2da.femaleCombo;
          pelvisList = player.oid.ControlledCreature.Gender == Gender.Male ? PelvisParts2da.maleCombo : PelvisParts2da.femaleCombo;
          torsoList = player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleCombo : TorsoParts2da.femaleCombo;

          player.DisableItemAppearanceFeedbackMessages();
          this.item = item;
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.7f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          NuiColumn root = new NuiColumn
          {
            Children = new List<NuiElement>
          {
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiSpacer { Width = 35 },
                new NuiCheck("Symétrie", symmetry) { Id = "applySymmetry", Width = 75 },
                new NuiSpacer { Width = windowRectangle.Width/2 - 220 },
                new NuiButton("Nom & Description") { Id = "openNameDescription", Height = 35, Width = 150 },
                new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 70 },
                new NuiSpacer(),
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Robe") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = robeList,
                  Selected = robeSelection
                },
                new NuiSlider(robeSlider, 0, robeList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200)  * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Cou") {  Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = NeckParts2da.combo,
                  Selected = neckSelection
                },
                new NuiSlider(neckSlider, 0, NeckParts2da.combo.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Torse") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = torsoList,
                  Selected = torsoSelection
                },
                new NuiSlider(torsoSlider, 0, torsoList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Ceinture") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = BeltParts2da.combo,
                  Selected = beltSelection
                },
                new NuiSlider(beltSlider, 0, BeltParts2da.combo.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Pelvis") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = pelvisList,
                  Selected = pelvisSelection
                },
                new NuiSlider(pelvisSlider, 0, pelvisList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Epaule gauche") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = shoulderList,
                  Selected = leftShoulderSelection
                },
                new NuiSlider(leftShoulderSlider, 0, shoulderList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Epaule droite") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = shoulderList,
                  Selected = rightShoulderSelection
                },
                new NuiSlider(rightShoulderSlider, 0, shoulderList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Biceps gauche") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = bicepList,
                  Selected = leftBicepSelection
                },
                new NuiSlider(leftBicepSlider, 0, bicepList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Biceps droite") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = bicepList,
                  Selected = rightBicepSelection
                },
                new NuiSlider(rightBicepSlider, 0, bicepList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Avant-bras gauche") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = forearmList,
                  Selected = leftForearmSelection
                },
                new NuiSlider(leftForearmSlider, 0, forearmList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Avant-bras droit") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = forearmList,
                  Selected = rightForearmSelection
                },
                new NuiSlider(rightForearmSlider, 0, forearmList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Main gauche") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = handList,
                  Selected = leftHandSelection
                },
                new NuiSlider(leftHandSlider, 0, handList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Main droite") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = handList,
                  Selected = rightHandSelection
                },
                new NuiSlider(rightHandSlider, 0, handList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                },
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Cuisse gauche") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = tighList,
                  Selected = leftTighSelection
                },
                new NuiSlider(leftTighSlider, 0, tighList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Cuisse droite") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = tighList,
                  Selected = rightTighSelection
                },
                new NuiSlider(rightTighSlider,0 , tighList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Tibia gauche") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = shinList,
                  Selected = leftShinSelection
                },
                new NuiSlider(leftShinSlider, 0, shinList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Tibia droit") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = shinList,
                  Selected = rightShinSelection
                },
                new NuiSlider(rightShinSlider, 0 , shinList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Pied gauche") { Width = 130 },
                new NuiCombo
                {
                   Width = 70,
                   Entries = footList,
                   Selected = leftFootSelection
                },
                new NuiSlider(leftFootSlider, 0, footList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel("Pied droit") { Width = 130 },
                new NuiCombo
                {
                  Width = 70,
                  Entries = footList,
                  Selected = rightFootSelection
                },
                new NuiSlider(rightFootSlider, 0, footList.Count - 1)
                {
                  Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                }
              }
            },
          }
          };

          window = new NuiWindow(root, $"Modifier l'apparence de {item.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleItemAppearanceEvents;

            PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 173);

            symmetry.SetBindValue(player.oid, nuiToken.Token, false);
            areaSelection.SetBindValue(player.oid, nuiToken.Token, 0);

            robeSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Robe));
            robeSlider.SetBindValue(player.oid, nuiToken.Token, robeList.IndexOf(robeList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Robe))));

            neckSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Neck));
            neckSlider.SetBindValue(player.oid, nuiToken.Token, NeckParts2da.combo.IndexOf(NeckParts2da.combo.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Neck))));

            torsoSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso));
            torsoSlider.SetBindValue(player.oid, nuiToken.Token, torsoList.IndexOf(torsoList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso))));

            beltSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Belt));
            beltSlider.SetBindValue(player.oid, nuiToken.Token, BeltParts2da.combo.IndexOf(BeltParts2da.combo.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Belt))));

            pelvisSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Pelvis));
            pelvisSlider.SetBindValue(player.oid, nuiToken.Token, pelvisList.IndexOf(pelvisList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Pelvis))));

            rightShoulderSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder));
            rightShoulderSlider.SetBindValue(player.oid, nuiToken.Token, shoulderList.IndexOf(shoulderList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder))));
            leftShoulderSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder));
            leftShoulderSlider.SetBindValue(player.oid, nuiToken.Token, shoulderList.IndexOf(shoulderList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder))));

            rightBicepSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep));
            rightBicepSlider.SetBindValue(player.oid, nuiToken.Token, bicepList.IndexOf(bicepList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep))));
            leftBicepSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep));
            leftBicepSlider.SetBindValue(player.oid, nuiToken.Token, bicepList.IndexOf(bicepList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep))));

            rightForearmSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm));
            rightForearmSlider.SetBindValue(player.oid, nuiToken.Token, forearmList.IndexOf(forearmList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm))));
            leftForearmSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm));
            leftForearmSlider.SetBindValue(player.oid, nuiToken.Token, forearmList.IndexOf(forearmList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm))));

            rightHandSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand));
            rightHandSlider.SetBindValue(player.oid, nuiToken.Token, handList.IndexOf(handList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand))));
            leftHandSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand));
            leftHandSlider.SetBindValue(player.oid, nuiToken.Token, handList.IndexOf(handList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand))));

            rightTighSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh));
            rightTighSlider.SetBindValue(player.oid, nuiToken.Token, tighList.IndexOf(tighList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh))));
            leftTighSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh));
            leftTighSlider.SetBindValue(player.oid, nuiToken.Token, tighList.IndexOf(tighList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh))));

            rightShinSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin));
            rightShinSlider.SetBindValue(player.oid, nuiToken.Token, shinList.IndexOf(shinList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin))));
            leftShinSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin));
            leftShinSlider.SetBindValue(player.oid, nuiToken.Token, shinList.IndexOf(shinList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin))));

            rightFootSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot));
            rightFootSlider.SetBindValue(player.oid, nuiToken.Token, footList.IndexOf(footList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot))));
            leftFootSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot));
            leftFootSlider.SetBindValue(player.oid, nuiToken.Token, footList.IndexOf(footList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot))));

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            areaSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            robeSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            robeSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            neckSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            neckSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            torsoSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            torsoSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            beltSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            beltSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            pelvisSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            pelvisSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            rightShoulderSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightShoulderSlider.SetBindWatch(player.oid, nuiToken.Token, true);
            leftShoulderSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftShoulderSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            rightBicepSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightBicepSlider.SetBindWatch(player.oid, nuiToken.Token, true);
            leftBicepSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftBicepSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            rightForearmSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightForearmSlider.SetBindWatch(player.oid, nuiToken.Token, true);
            leftForearmSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftForearmSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            rightHandSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightHandSlider.SetBindWatch(player.oid, nuiToken.Token, true);
            leftHandSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftHandSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            rightTighSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightTighSlider.SetBindWatch(player.oid, nuiToken.Token, true);
            leftTighSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftTighSlider.SetBindWatch(player.oid, nuiToken.Token, true);

            rightFootSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightFootSlider.SetBindWatch(player.oid, nuiToken.Token, true);
            leftFootSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftFootSlider.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleArmorSliderChange(NuiBind<int> slider, NuiBind<int> selector, ItemAppearanceArmorModel model)
        {
          int sliderValue = slider.GetBindValue(player.oid, nuiToken.Token);
          int selectedValue = selector.GetBindValue(player.oid, nuiToken.Token);

          if (sliderValue == selectedValue || item == null)
            return;

          int result = 0;
          int nModel = (int)model;
          ItemAppearanceArmorModel modelSymmetry = model;
          if (symmetry.GetBindValue(player.oid, nuiToken.Token) && (nModel < 6 || (nModel > 9 && nModel < 18)))
          {
            if (nModel % 2 == 0)
              modelSymmetry = (ItemAppearanceArmorModel)(nModel + 1);
            else
              modelSymmetry = (ItemAppearanceArmorModel)(nModel - 1);
          }

          switch (model)
          {
            case ItemAppearanceArmorModel.Robe:
              result = robeList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Neck:
              result = NeckParts2da.combo.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Torso:
              result = torsoList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Belt:
              result = BeltParts2da.combo.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Pelvis:
              result = pelvisList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftShoulder:
            case ItemAppearanceArmorModel.RightShoulder:
              result = shoulderList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftBicep:
            case ItemAppearanceArmorModel.RightBicep:
              result = bicepList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftForearm:
            case ItemAppearanceArmorModel.RightForearm:
              result = forearmList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftHand:
            case ItemAppearanceArmorModel.RightHand:
              result = handList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftThigh:
            case ItemAppearanceArmorModel.RightThigh:
              result = tighList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftShin:
            case ItemAppearanceArmorModel.RightShin:
              result = shinList.ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftFoot:
            case ItemAppearanceArmorModel.RightFoot:
              result = footList.ElementAt(sliderValue).Value;
              break;
          }

          item.Appearance.SetArmorModel(model, (byte)result);
          if (modelSymmetry != model)
            item.Appearance.SetArmorModel(modelSymmetry, (byte)result);

          player.oid.ControlledCreature.RunUnequip(item);

          Task waitUnequip = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            player.oid.ControlledCreature.RunEquip(item, InventorySlot.Chest);
          });

          selector.SetBindWatch(player.oid, nuiToken.Token, false);
          selector.SetBindValue(player.oid, nuiToken.Token, result);


          if (modelSymmetry != model)
          {
            string sliderSimId;
            string selectorSimId;
            
            if (slider.Key.Contains("left"))
            {
              sliderSimId = slider.Key.Replace("left", "right");
              selectorSimId = slider.Key.Replace("left", "right");
            }
            else
            {
              sliderSimId = slider.Key.Replace("right", "left");
              selectorSimId = slider.Key.Replace("right", "left");
            }

            NuiBind<int> symmetrySlider = new (sliderSimId);
            NuiBind<int> symmetrySelector = new (selectorSimId);
            
            symmetrySelector.SetBindWatch(player.oid, nuiToken.Token, false);
            symmetrySelector.SetBindValue(player.oid, nuiToken.Token, result);

            symmetrySlider.SetBindWatch(player.oid, nuiToken.Token, false);
            symmetrySlider.SetBindValue(player.oid, nuiToken.Token, sliderValue);

            symmetrySelector.SetBindWatch(player.oid, nuiToken.Token, true);
            symmetrySlider.SetBindWatch(player.oid, nuiToken.Token, true);
          }

          selector.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        public void HandleArmorSelectorChange(NuiBind<int> slider, NuiBind<int> selector, ItemAppearanceArmorModel model)
        {
          int selectorValue = selector.GetBindValue(player.oid, nuiToken.Token);
          int sliderValue = slider.GetBindValue(player.oid, nuiToken.Token);

          if (selectorValue == sliderValue || item == null)
            return;

          int nModel = (int)model;
          ItemAppearanceArmorModel modelSymmetry = model;
          if (symmetry.GetBindValue(player.oid, nuiToken.Token) && (nModel < 6 || (nModel > 9 && nModel < 18)))
          {
            if (nModel % 2 == 0)
              modelSymmetry = (ItemAppearanceArmorModel)(nModel + 1);
            else
              modelSymmetry = (ItemAppearanceArmorModel)(nModel - 1);
          }

          item.Appearance.SetArmorModel(model, (byte)selectorValue);
          if (modelSymmetry != model)
            item.Appearance.SetArmorModel(modelSymmetry, (byte)selectorValue);

          player.oid.ControlledCreature.RunUnequip(item);

          Task waitUnequip = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            player.oid.ControlledCreature.RunEquip(item, InventorySlot.Chest);
          });

          slider.SetBindWatch(player.oid, nuiToken.Token, false);
          slider.SetBindValue(player.oid, nuiToken.Token, selectorValue);

          if (modelSymmetry != model)
          {
            string sliderSimId;
            string selectorSimId;

            if (slider.Key.Contains("left"))
            {
              sliderSimId = slider.Key.Replace("left", "right");
              selectorSimId = slider.Key.Replace("left", "right");
            }
            else
            {
              sliderSimId = slider.Key.Replace("right", "left");
              selectorSimId = slider.Key.Replace("right", "left");
            }

            NuiBind<int> symmetrySlider = new (sliderSimId);
            NuiBind<int> symmetrySelector = new (selectorSimId);

            symmetrySelector.SetBindWatch(player.oid, nuiToken.Token, false);
            symmetrySelector.SetBindValue(player.oid, nuiToken.Token, selectorValue);

            symmetrySlider.SetBindWatch(player.oid, nuiToken.Token, false);
            symmetrySlider.SetBindValue(player.oid, nuiToken.Token, sliderValue);

            symmetrySelector.SetBindWatch(player.oid, nuiToken.Token, true);
            symmetrySlider.SetBindWatch(player.oid, nuiToken.Token, true);
          }

          slider.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void HandleItemAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.EnableItemAppearanceFeedbackMessages();
            PlayerPlugin.ApplyLoopingVisualEffectToObject(nuiEvent.Player.ControlledCreature, nuiEvent.Player.ControlledCreature, 173);
            return;
          }

          if (!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
          {
            nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            CloseWindow();
            return;
          }

          if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "openColors")
          {
            CloseWindow();

            if (player.windows.ContainsKey("itemColorsModifier"))
              ((ArmorColorWindow)player.windows["itemColorsModifier"]).CreateWindow(item);
            else
              player.windows.Add("itemColorsModifier", new ArmorColorWindow(player, item));
            return;
          }

          if (nuiEvent.EventType == NuiEventType.Watch)
            switch (nuiEvent.ElementId)
            {
              case "robeSliderValue":
                HandleArmorSliderChange(robeSlider, robeSelection, ItemAppearanceArmorModel.Robe);
                break;

              case "robeSelection":
                HandleArmorSelectorChange(robeSlider, robeSelection, ItemAppearanceArmorModel.Robe);
                break;

              case "neckSliderValue":
                HandleArmorSliderChange(neckSlider, neckSelection, ItemAppearanceArmorModel.Neck);
                break;

              case "neckSelection":
                HandleArmorSelectorChange(neckSlider, neckSelection, ItemAppearanceArmorModel.Neck);
                break;

              case "torsoSliderValue":
                HandleArmorSliderChange(torsoSlider, torsoSelection, ItemAppearanceArmorModel.Torso);
                break;

              case "torsoSelection":
                HandleArmorSelectorChange(torsoSlider, torsoSelection, ItemAppearanceArmorModel.Torso);
                break;

              case "beltSliderValue":
                HandleArmorSliderChange(beltSlider, beltSelection, ItemAppearanceArmorModel.Belt);
                break;

              case "beltSelection":
                HandleArmorSelectorChange(beltSlider, beltSelection, ItemAppearanceArmorModel.Belt);
                break;

              case "pelvisSliderValue":
                HandleArmorSliderChange(pelvisSlider, pelvisSelection, ItemAppearanceArmorModel.Pelvis);
                break;

              case "pelvisSelection":
                HandleArmorSelectorChange(pelvisSlider, pelvisSelection, ItemAppearanceArmorModel.Pelvis);
                break;

              case "rightShoulderSliderValue":
                HandleArmorSliderChange(rightShoulderSlider, rightShoulderSelection, ItemAppearanceArmorModel.RightShoulder);
                break;

              case "rightShoulderSelection":
                HandleArmorSelectorChange(rightShoulderSlider, rightShoulderSelection, ItemAppearanceArmorModel.RightShoulder);
                break;

              case "leftShoulderSliderValue":
                HandleArmorSliderChange(leftShoulderSlider, leftShoulderSelection, ItemAppearanceArmorModel.LeftShoulder);
                break;

              case "leftShoulderSelection":
                HandleArmorSelectorChange(leftShoulderSlider, leftShoulderSelection, ItemAppearanceArmorModel.LeftShoulder);
                break;

              case "rightBicepSliderValue":
                HandleArmorSliderChange(rightBicepSlider, rightBicepSelection, ItemAppearanceArmorModel.RightBicep);
                break;

              case "rightBicepSelection":
                HandleArmorSelectorChange(rightBicepSlider, rightBicepSelection, ItemAppearanceArmorModel.RightBicep);
                break;

              case "leftBicepSliderValue":
                HandleArmorSliderChange(leftBicepSlider, leftBicepSelection, ItemAppearanceArmorModel.LeftBicep);
                break;

              case "leftBicepSelection":
                HandleArmorSelectorChange(leftBicepSlider, leftBicepSelection, ItemAppearanceArmorModel.LeftBicep);
                break;

              case "rightForearmSliderValue":
                HandleArmorSliderChange(rightForearmSlider, rightForearmSelection, ItemAppearanceArmorModel.RightForearm);
                break;

              case "rightForearmSelection":
                HandleArmorSelectorChange(rightForearmSlider, rightForearmSelection, ItemAppearanceArmorModel.RightForearm);
                break;

              case "leftForearmSliderValue":
                HandleArmorSliderChange(leftFootSlider, leftFootSelection, ItemAppearanceArmorModel.LeftForearm);
                break;

              case "leftForearmSelection":
                HandleArmorSelectorChange(leftForearmSlider, leftForearmSelection, ItemAppearanceArmorModel.LeftForearm);
                break;

              case "rightHandSliderValue":
                HandleArmorSliderChange(rightHandSlider, rightHandSelection, ItemAppearanceArmorModel.RightHand);
                break;

              case "rightHandSelection":
                HandleArmorSelectorChange(rightHandSlider, rightHandSelection, ItemAppearanceArmorModel.RightHand);
                break;

              case "leftHandSliderValue":
                HandleArmorSliderChange(leftForearmSlider, leftForearmSelection, ItemAppearanceArmorModel.LeftHand);
                break;

              case "leftHandSelection":
                HandleArmorSelectorChange(leftForearmSlider, leftForearmSelection, ItemAppearanceArmorModel.LeftHand);
                break;

              case "rightTighSliderValue":
                HandleArmorSliderChange(rightTighSlider, rightTighSelection, ItemAppearanceArmorModel.RightThigh);
                break;

              case "rightTighSelection":
                HandleArmorSelectorChange(rightTighSlider, rightTighSelection, ItemAppearanceArmorModel.RightThigh);
                break;

              case "leftTighSliderValue":
                HandleArmorSliderChange(leftTighSlider, leftTighSelection, ItemAppearanceArmorModel.LeftThigh);
                break;

              case "leftTighSelection":
                HandleArmorSelectorChange(leftTighSlider, leftTighSelection, ItemAppearanceArmorModel.LeftThigh);
                break;

              case "rightShinSliderValue":
                HandleArmorSliderChange(rightShinSlider, rightShinSelection, ItemAppearanceArmorModel.RightShin);
                break;

              case "rightShinSelection":
                HandleArmorSelectorChange(rightShinSlider, rightShinSelection, ItemAppearanceArmorModel.RightShin);
                break;

              case "leftShinSliderValue":
                HandleArmorSliderChange(leftShinSlider, leftShinSelection, ItemAppearanceArmorModel.LeftShin);
                break;

              case "leftShinSelection":
                HandleArmorSelectorChange(leftShinSlider, leftShinSelection, ItemAppearanceArmorModel.LeftShin);
                break;

              case "rightFootSliderValue":
                HandleArmorSliderChange(rightFootSlider, rightFootSelection, ItemAppearanceArmorModel.RightFoot);
                break;

              case "rightFootSelection":
                HandleArmorSelectorChange(rightFootSlider, rightFootSelection, ItemAppearanceArmorModel.RightFoot);
                break;

              case "leftFootSliderValue":
                HandleArmorSliderChange(leftFootSlider, leftFootSelection, ItemAppearanceArmorModel.LeftFoot);
                break;

              case "leftFootSelection":
                HandleArmorSelectorChange(leftFootSlider, leftFootSelection, ItemAppearanceArmorModel.LeftFoot);
                break;
            }
        }
      }
    }
  }
}
