﻿using System;
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
        private readonly NuiBind<int> robeSelection = new NuiBind<int>("robeSelection");
        private readonly NuiBind<int> robeSlider = new NuiBind<int>("robeSliderValue");
        private readonly NuiBind<int> neckSelection = new NuiBind<int>("neckSelection");
        private readonly NuiBind<int> neckSlider = new NuiBind<int>("neckSliderValue");
        private readonly NuiBind<int> torsoSelection = new NuiBind<int>("torsoSelection");
        private readonly NuiBind<int> torsoSlider = new NuiBind<int>("torsoSliderValue");
        private readonly NuiBind<int> beltSelection = new NuiBind<int>("beltSelection");
        private readonly NuiBind<int> beltSlider = new NuiBind<int>("beltSliderValue");
        private readonly NuiBind<int> pelvisSelection = new NuiBind<int>("pelvisSelection");
        private readonly NuiBind<int> pelvisSlider = new NuiBind<int>("pelvisSliderValue");
        private readonly NuiBind<int> rightShoulderSelection = new NuiBind<int>("rightShoulderSelection");
        private readonly NuiBind<int> rightShoulderSlider = new NuiBind<int>("rightShoulderSliderValue");
        private readonly NuiBind<int> leftShoulderSelection = new NuiBind<int>("leftShoulderSelection");
        private readonly NuiBind<int> leftShoulderSlider = new NuiBind<int>("leftShoulderSliderValue");
        private readonly NuiBind<int> rightBicepSelection = new NuiBind<int>("rightBicepSelection");
        private readonly NuiBind<int> rightBicepSlider = new NuiBind<int>("rightBicepSliderValue");
        private readonly NuiBind<int> leftBicepSelection = new NuiBind<int>("leftBicepSelection");
        private readonly NuiBind<int> leftBicepSlider = new NuiBind<int>("leftBicepSliderValue");
        private readonly NuiBind<int> rightForearmSelection = new NuiBind<int>("rightForearmSelection");
        private readonly NuiBind<int> rightForearmSlider = new NuiBind<int>("rightForearmSliderValue");
        private readonly NuiBind<int> leftForearmSelection = new NuiBind<int>("leftForearmSelection");
        private readonly NuiBind<int> leftForearmSlider = new NuiBind<int>("leftForearmSliderValue");
        private readonly NuiBind<int> rightHandSelection = new NuiBind<int>("rightHandSelection");
        private readonly NuiBind<int> rightHandSlider = new NuiBind<int>("rightHandSliderValue");
        private readonly NuiBind<int> leftHandSelection = new NuiBind<int>("leftHandSelection");
        private readonly NuiBind<int> leftHandSlider = new NuiBind<int>("leftHandSliderValue");
        private readonly NuiBind<int> rightTighSelection = new NuiBind<int>("rightTighSelection");
        private readonly NuiBind<int> rightTighSlider = new NuiBind<int>("rightTighSliderValue");
        private readonly NuiBind<int> leftTighSelection = new NuiBind<int>("leftTighSelection");
        private readonly NuiBind<int> leftTighSlider = new NuiBind<int>("leftTighSliderValue");
        private readonly NuiBind<int> rightShinSelection = new NuiBind<int>("rightShinSelection");
        private readonly NuiBind<int> rightShinSlider = new NuiBind<int>("rightShinSliderValue");
        private readonly NuiBind<int> leftShinSelection = new NuiBind<int>("leftShinSelection");
        private readonly NuiBind<int> leftShinSlider = new NuiBind<int>("leftShinSliderValue");
        private readonly NuiBind<int> rightFootSelection = new NuiBind<int>("rightFootSelection");
        private readonly NuiBind<int> rightFootSlider = new NuiBind<int>("rightFootSliderValue");
        private readonly NuiBind<int> leftFootSelection = new NuiBind<int>("leftFootSelection");
        private readonly NuiBind<int> leftFootSlider = new NuiBind<int>("leftFootSliderValue");
        private readonly NuiBind<bool> symmetry = new NuiBind<bool>("symmetry");
        private readonly NuiBind<int> areaSelection = new NuiBind<int>("areaSelection");
        private readonly List<NuiComboEntry> robeList;
        private readonly List<NuiComboEntry> neckList;
        private readonly List<NuiComboEntry> torsoList;
        private readonly List<NuiComboEntry> beltList;
        private readonly List<NuiComboEntry> pelvisList;
        private readonly List<NuiComboEntry> shoulderList;
        private readonly List<NuiComboEntry> bicepList;
        private readonly List<NuiComboEntry> forearmList;
        private readonly List<NuiComboEntry> handList;
        private readonly List<NuiComboEntry> tighList;
        private readonly List<NuiComboEntry> shinList;
        private readonly List<NuiComboEntry> footList;
        private NwItem item { get; set; }

        public ArmorAppearanceWindow(Player player, NwItem item) : base (player)
        {
          windowId = "itemAppearanceModifier";

          robeList = RobeParts2da.robePartsTable.GetValidRobeAppearancesForGender(player.oid.ControlledCreature.Gender);
          neckList = NeckParts2da.neckPartsTable.GetValidNeckAppearances();
          torsoList = TorsoParts2da.torsoPartsTable.GetValidChestAppearancesForGenderAndAC(player.oid.ControlledCreature.Gender, item.BaseACValue);
          beltList = BeltParts2da.beltPartsTable.GetValidBeltAppearances();
          pelvisList = PelvisParts2da.pelvisPartsTable.GetValidPelvisAppearancesForGender(player.oid.ControlledCreature.Gender);
          shoulderList = ShoulderParts2da.shoulderPartsTable.GetValidShoulderAppearancesForGender(player.oid.ControlledCreature.Gender);
          bicepList = BicepParts2da.bicepPartsTable.GetValidBicepAppearancesForGender(player.oid.ControlledCreature.Gender);
          forearmList = ForearmParts2da.forearmPartsTable.GetValidForearmAppearancesForGender(player.oid.ControlledCreature.Gender);
          handList = HandParts2da.handPartsTable.GetValidHandAppearancesForGender(player.oid.ControlledCreature.Gender);
          tighList = LegParts2da.legPartsTable.GetValidLegAppearancesForGender(player.oid.ControlledCreature.Gender);
          shinList = ShinParts2da.shinPartsTable.GetValidShinAppearancesForGender(player.oid.ControlledCreature.Gender);
          footList = FootParts2da.footPartsTable.GetValidFootAppearancesForGender(player.oid.ControlledCreature.Gender);

          CreateWindow(item);
        }

        public void CreateWindow(NwItem item)
        {
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
                  Entries = neckList,
                  Selected = neckSelection
                },
                new NuiSlider(neckSlider, 0, neckList.Count - 1)
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
                  Entries = beltList,
                  Selected = beltSelection
                },
                new NuiSlider(beltSlider, 0, beltList.Count - 1)
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

          player.oid.OnNuiEvent -= HandleItemAppearanceEvents;
          player.oid.OnNuiEvent += HandleItemAppearanceEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          PlayerPlugin.ApplyLoopingVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 173);

          symmetry.SetBindValue(player.oid, token, false);
          areaSelection.SetBindValue(player.oid, token, 0);

          robeSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Robe));
          robeSlider.SetBindValue(player.oid, token, robeList.IndexOf(robeList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Robe))));

          neckSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Neck));
          neckSlider.SetBindValue(player.oid, token, neckList.IndexOf(neckList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Neck))));

          torsoSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso));
          torsoSlider.SetBindValue(player.oid, token, torsoList.IndexOf(torsoList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso))));

          beltSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Belt));
          beltSlider.SetBindValue(player.oid, token, beltList.IndexOf(beltList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Belt))));

          pelvisSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Pelvis));
          pelvisSlider.SetBindValue(player.oid, token, pelvisList.IndexOf(pelvisList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Pelvis))));

          rightShoulderSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder));
          rightShoulderSlider.SetBindValue(player.oid, token, shoulderList.IndexOf(shoulderList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder))));
          leftShoulderSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder));
          leftShoulderSlider.SetBindValue(player.oid, token, shoulderList.IndexOf(shoulderList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder))));

          rightBicepSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep));
          rightBicepSlider.SetBindValue(player.oid, token, bicepList.IndexOf(bicepList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep))));
          leftBicepSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep));
          leftBicepSlider.SetBindValue(player.oid, token, bicepList.IndexOf(bicepList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep))));

          rightForearmSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm));
          rightForearmSlider.SetBindValue(player.oid, token, forearmList.IndexOf(forearmList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm))));
          leftForearmSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm));
          leftForearmSlider.SetBindValue(player.oid, token, forearmList.IndexOf(forearmList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm))));

          rightHandSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand));
          rightHandSlider.SetBindValue(player.oid, token, handList.IndexOf(handList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand))));
          leftHandSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand));
          leftHandSlider.SetBindValue(player.oid, token, handList.IndexOf(handList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand))));

          rightTighSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh));
          rightTighSlider.SetBindValue(player.oid, token, tighList.IndexOf(tighList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh))));
          leftTighSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh));
          leftTighSlider.SetBindValue(player.oid, token, tighList.IndexOf(tighList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh))));

          rightShinSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin));
          rightShinSlider.SetBindValue(player.oid, token, shinList.IndexOf(shinList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin))));
          leftShinSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin));
          leftShinSlider.SetBindValue(player.oid, token, shinList.IndexOf(shinList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin))));

          rightFootSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot));
          rightFootSlider.SetBindValue(player.oid, token, footList.IndexOf(footList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot))));
          leftFootSelection.SetBindValue(player.oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot));
          leftFootSlider.SetBindValue(player.oid, token, footList.IndexOf(footList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot))));

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          /*Task waitWindowOpened = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.6));*/

            areaSelection.SetBindWatch(player.oid, token, true);

            robeSelection.SetBindWatch(player.oid, token, true);
            robeSlider.SetBindWatch(player.oid, token, true);

            neckSelection.SetBindWatch(player.oid, token, true);
            neckSlider.SetBindWatch(player.oid, token, true);

            torsoSelection.SetBindWatch(player.oid, token, true);
            torsoSlider.SetBindWatch(player.oid, token, true);

            beltSelection.SetBindWatch(player.oid, token, true);
            beltSlider.SetBindWatch(player.oid, token, true);

            pelvisSelection.SetBindWatch(player.oid, token, true);
            pelvisSlider.SetBindWatch(player.oid, token, true);

            rightShoulderSelection.SetBindWatch(player.oid, token, true);
            rightShoulderSlider.SetBindWatch(player.oid, token, true);
            leftShoulderSelection.SetBindWatch(player.oid, token, true);
            leftShoulderSlider.SetBindWatch(player.oid, token, true);

            rightBicepSelection.SetBindWatch(player.oid, token, true);
            rightBicepSlider.SetBindWatch(player.oid, token, true);
            leftBicepSelection.SetBindWatch(player.oid, token, true);
            leftBicepSlider.SetBindWatch(player.oid, token, true);

            rightForearmSelection.SetBindWatch(player.oid, token, true);
            rightForearmSlider.SetBindWatch(player.oid, token, true);
            leftForearmSelection.SetBindWatch(player.oid, token, true);
            leftForearmSlider.SetBindWatch(player.oid, token, true);

            rightHandSelection.SetBindWatch(player.oid, token, true);
            rightHandSlider.SetBindWatch(player.oid, token, true);
            leftHandSelection.SetBindWatch(player.oid, token, true);
            leftHandSlider.SetBindWatch(player.oid, token, true);

            rightTighSelection.SetBindWatch(player.oid, token, true);
            rightTighSlider.SetBindWatch(player.oid, token, true);
            leftTighSelection.SetBindWatch(player.oid, token, true);
            leftTighSlider.SetBindWatch(player.oid, token, true);

            rightFootSelection.SetBindWatch(player.oid, token, true);
            rightFootSlider.SetBindWatch(player.oid, token, true);
            leftFootSelection.SetBindWatch(player.oid, token, true);
            leftFootSlider.SetBindWatch(player.oid, token, true);
         // });
        }
        private void HandleArmorSliderChange(NuiBind<int> slider, NuiBind<int> selector, ItemAppearanceArmorModel model)
        {
          int sliderValue = slider.GetBindValue(player.oid, token);
          int selectedValue = selector.GetBindValue(player.oid, token);

          if (sliderValue == selectedValue || item == null)
            return;

          int result = 0;
          int nModel = (int)model;
          ItemAppearanceArmorModel modelSymmetry = model;
          if (symmetry.GetBindValue(player.oid, token) && (nModel < 6 || (nModel > 9 && nModel < 18)))
          {
            if (nModel % 2 == 0)
              modelSymmetry = (ItemAppearanceArmorModel)(nModel + 1);
            else
              modelSymmetry = (ItemAppearanceArmorModel)(nModel - 1);
          }

          switch (model)
          {
            case ItemAppearanceArmorModel.Robe:
              result = RobeParts2da.robePartsTable.GetValidRobeAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Neck:
              result = NeckParts2da.neckPartsTable.GetValidNeckAppearances().ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Torso:
              result = TorsoParts2da.torsoPartsTable.GetValidChestAppearancesForGenderAndAC(player.oid.ControlledCreature.Gender, item.BaseACValue).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Belt:
              result = BeltParts2da.beltPartsTable.GetValidBeltAppearances().ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.Pelvis:
              result = PelvisParts2da.pelvisPartsTable.GetValidPelvisAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftShoulder:
            case ItemAppearanceArmorModel.RightShoulder:
              result = ShoulderParts2da.shoulderPartsTable.GetValidShoulderAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftBicep:
            case ItemAppearanceArmorModel.RightBicep:
              result = BicepParts2da.bicepPartsTable.GetValidBicepAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftForearm:
            case ItemAppearanceArmorModel.RightForearm:
              result = ForearmParts2da.forearmPartsTable.GetValidForearmAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftHand:
            case ItemAppearanceArmorModel.RightHand:
              result = HandParts2da.handPartsTable.GetValidHandAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftThigh:
            case ItemAppearanceArmorModel.RightThigh:
              result = LegParts2da.legPartsTable.GetValidLegAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftShin:
            case ItemAppearanceArmorModel.RightShin:
              result = ShinParts2da.shinPartsTable.GetValidShinAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
              break;
            case ItemAppearanceArmorModel.LeftFoot:
            case ItemAppearanceArmorModel.RightFoot:
              result = FootParts2da.footPartsTable.GetValidFootAppearancesForGender(player.oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
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

          selector.SetBindWatch(player.oid, token, false);
          selector.SetBindValue(player.oid, token, result);


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

            NuiBind<int> symmetrySlider = new NuiBind<int>(sliderSimId);
            NuiBind<int> symmetrySelector = new NuiBind<int>(selectorSimId);
            
            symmetrySelector.SetBindWatch(player.oid, token, false);
            symmetrySelector.SetBindValue(player.oid, token, result);

            symmetrySlider.SetBindWatch(player.oid, token, false);
            symmetrySlider.SetBindValue(player.oid, token, sliderValue);

            symmetrySelector.SetBindWatch(player.oid, token, true);
            symmetrySlider.SetBindWatch(player.oid, token, true);
          }

          selector.SetBindWatch(player.oid, token, true);
        }
        public void HandleArmorSelectorChange(NuiBind<int> slider, NuiBind<int> selector, ItemAppearanceArmorModel model)
        {
          int selectorValue = selector.GetBindValue(player.oid, token);
          int sliderValue = slider.GetBindValue(player.oid, token);

          if (selectorValue == sliderValue || item == null)
            return;

          int nModel = (int)model;
          ItemAppearanceArmorModel modelSymmetry = model;
          if (symmetry.GetBindValue(player.oid, token) && (nModel < 6 || (nModel > 9 && nModel < 18)))
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

          slider.SetBindWatch(player.oid, token, false);
          slider.SetBindValue(player.oid, token, selectorValue);

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

            NuiBind<int> symmetrySlider = new NuiBind<int>(sliderSimId);
            NuiBind<int> symmetrySelector = new NuiBind<int>(selectorSimId);

            symmetrySelector.SetBindWatch(player.oid, token, false);
            symmetrySelector.SetBindValue(player.oid, token, selectorValue);

            symmetrySlider.SetBindWatch(player.oid, token, false);
            symmetrySlider.SetBindValue(player.oid, token, sliderValue);

            symmetrySelector.SetBindWatch(player.oid, token, true);
            symmetrySlider.SetBindWatch(player.oid, token, true);
          }

          slider.SetBindWatch(player.oid, token, true);
        }
        private void HandleItemAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "itemAppearanceModifier")
            return;

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
