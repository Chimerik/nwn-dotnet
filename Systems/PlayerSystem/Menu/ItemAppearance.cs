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
      public void CreateItemAppearanceWindow(NwItem item)
      {
        string windowId = "itemAppearanceModifier";
        NuiBind<string> title = new NuiBind<string>("title");
        NuiBind<int> robeSelection = new NuiBind<int>("robeSelection");
        NuiBind<int> robeSlider = new NuiBind<int>("robeSliderValue");
        NuiBind<int> neckSelection = new NuiBind<int>("neckSelection");
        NuiBind<int> neckSlider = new NuiBind<int>("neckSliderValue");
        NuiBind<int> torsoSelection = new NuiBind<int>("torsoSelection");
        NuiBind<int> torsoSlider = new NuiBind<int>("torsoSliderValue");
        NuiBind<int> beltSelection = new NuiBind<int>("beltSelection");
        NuiBind<int> beltSlider = new NuiBind<int>("beltSliderValue");
        NuiBind<int> pelvisSelection = new NuiBind<int>("pelvisSelection");
        NuiBind<int> pelvisSlider = new NuiBind<int>("pelvisSliderValue");
        NuiBind<int> rightShoulderSelection = new NuiBind<int>("rightShoulderSelection");
        NuiBind<int> rightShoulderSlider = new NuiBind<int>("rightShoulderSliderValue");
        NuiBind<int> leftShoulderSelection = new NuiBind<int>("leftShoulderSelection");
        NuiBind<int> leftShoulderSlider = new NuiBind<int>("leftShoulderSliderValue");
        NuiBind<int> rightBicepSelection = new NuiBind<int>("rightBicepSelection");
        NuiBind<int> rightBicepSlider = new NuiBind<int>("rightBicepSliderValue");
        NuiBind<int> leftBicepSelection = new NuiBind<int>("leftBicepSelection");
        NuiBind<int> leftBicepSlider = new NuiBind<int>("leftBicepSliderValue");
        NuiBind<int> rightForearmSelection = new NuiBind<int>("rightForearmSelection");
        NuiBind<int> rightForearmSlider = new NuiBind<int>("rightForearmSliderValue");
        NuiBind<int> leftForearmSelection = new NuiBind<int>("leftForearmSelection");
        NuiBind<int> leftForearmSlider = new NuiBind<int>("leftForearmSliderValue");
        NuiBind<int> rightHandSelection = new NuiBind<int>("rightHandSelection");
        NuiBind<int> rightHandSlider = new NuiBind<int>("rightHandSliderValue");
        NuiBind<int> leftHandSelection = new NuiBind<int>("leftHandSelection");
        NuiBind<int> leftHandSlider = new NuiBind<int>("leftHandSliderValue");
        NuiBind<int> rightTighSelection = new NuiBind<int>("rightTighSelection");
        NuiBind<int> rightTighSlider = new NuiBind<int>("rightTighSliderValue");
        NuiBind<int> leftTighSelection = new NuiBind<int>("leftTighSelection");
        NuiBind<int> leftTighSlider = new NuiBind<int>("leftTighSliderValue");
        NuiBind<int> rightShinSelection = new NuiBind<int>("rightShinSelection");
        NuiBind<int> rightShinSlider = new NuiBind<int>("rightShinSliderValue");
        NuiBind<int> leftShinSelection = new NuiBind<int>("leftShinSelection");
        NuiBind<int> leftShinSlider = new NuiBind<int>("leftShinSliderValue");
        NuiBind<int> rightFootSelection = new NuiBind<int>("rightFootSelection");
        NuiBind<int> rightFootSlider = new NuiBind<int>("rightFootSliderValue");
        NuiBind<int> leftFootSelection = new NuiBind<int>("leftFootSelection");
        NuiBind<int> leftFootSlider = new NuiBind<int>("leftFootSliderValue");
        NuiBind<bool> symmetry = new NuiBind<bool>("symmetry");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey(windowId) && windowRectangles[windowId].Width > 0 ? windowRectangles[windowId] : new NuiRect(10, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) * 0.97f, oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

        oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARMOR_APPEARANCE_MODIFIER").Value = item.BaseACValue;

        List<NuiComboEntry> robeList = RobeParts2da.robePartsTable.GetValidRobeAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> neckList = NeckParts2da.neckPartsTable.GetValidNeckAppearances();
        List<NuiComboEntry> torsoList = TorsoParts2da.torsoPartsTable.GetValidChestAppearancesForGenderAndAC(oid.ControlledCreature.Gender, item.BaseACValue);
        List<NuiComboEntry> beltList = BeltParts2da.beltPartsTable.GetValidBeltAppearances();
        List<NuiComboEntry> pelvisList = PelvisParts2da.pelvisPartsTable.GetValidPelvisAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> shoulderList = ShoulderParts2da.shoulderPartsTable.GetValidShoulderAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> bicepList = BicepParts2da.bicepPartsTable.GetValidBicepAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> forearmList = ForearmParts2da.forearmPartsTable.GetValidForearmAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> handList = HandParts2da.handPartsTable.GetValidHandAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> tighList = LegParts2da.legPartsTable.GetValidLegAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> shinList = ShinParts2da.shinPartsTable.GetValidShinAppearancesForGender(oid.ControlledCreature.Gender);
        List<NuiComboEntry> footList = FootParts2da.footPartsTable.GetValidFootAppearancesForGender(oid.ControlledCreature.Gender);

        NuiBind<int> areaSelection = new NuiBind<int>("areaSelection");

        // Construct the window layout.
        NuiCol root = new NuiCol
        {
          Children = new List<NuiElement>
          {
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiSpacer { Width = 35 },
                new NuiCheck
                {
                  Id = "applySymmetry", Width = 75, 
                  Label = "Symétrie",
                  Value = symmetry
                },
                new NuiSpacer { Width = windowRectangle.Width - 220 },
                new NuiButton
                {
                  Id = "openColors", Height = 35, Width = 70,
                  Label = "Couleurs"
                },
              }
            },
            new NuiRow
            {  
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Robe",
                },
                new NuiCombo
                {
                   Width = 70, 
                   Entries = robeList,
                   Selected = robeSelection
                },
                new NuiSlider
                {
                    Min = 0, Max = robeList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200)  * 0.96f,
                    Value = robeSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Cou"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = neckList,
                   Selected = neckSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = neckList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = neckSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Torse"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = torsoList,
                   Selected = torsoSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = torsoList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = torsoSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Ceinture"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = beltList,
                   Selected = beltSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = beltList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = beltSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Pelvis"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = pelvisList,
                   Selected = pelvisSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = pelvisList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = pelvisSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Epaule gauche"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = shoulderList,
                   Selected = leftShoulderSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = shoulderList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = leftShoulderSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Epaule droite"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = shoulderList,
                   Selected = rightShoulderSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = shoulderList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = rightShoulderSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Biceps gauche"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = bicepList,
                   Selected = leftBicepSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = bicepList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = leftBicepSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Biceps droite"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = bicepList,
                   Selected = rightBicepSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = bicepList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = rightBicepSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Avant-bras gauche"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = forearmList,
                   Selected = leftForearmSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = forearmList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = leftForearmSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                   Width = 130,
                   Value = "Avant-bras droit"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = forearmList,
                   Selected = rightForearmSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = forearmList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = rightForearmSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Main gauche"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = handList,
                   Selected = leftHandSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = handList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = leftHandSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Main droite"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = handList,
                   Selected = rightHandSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = handList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = rightHandSlider
                },
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Cuisse gauche"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = tighList,
                   Selected = leftTighSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = tighList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = leftTighSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Cuisse droite"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = tighList,
                   Selected = rightTighSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = tighList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = rightTighSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Tibia gauche"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = shinList,
                   Selected = leftShinSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = shinList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = leftShinSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Tibia droit"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = shinList,
                   Selected = rightShinSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = shinList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = rightShinSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Pied gauche"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = footList,
                   Selected = leftFootSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = footList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = leftFootSlider
                }
              }
            },
            new NuiRow
            {
              Children = new List<NuiElement>
              {
                new NuiLabel
                {
                  Width = 130,
                  Value = "Pied droit"
                },
                new NuiCombo
                {
                   Width = 70,
                   Entries = footList,
                   Selected = rightFootSelection
                },
                new NuiSlider
                {
                   Min = 0, Max = footList.Count - 1, Step = 1,  Width = (windowRectangle.Width - 200) * 0.96f,
                   Value = rightFootSlider
                }
              }
            },
          }
        };
        NuiWindow window = new NuiWindow
        {
          Root = root,
          Geometry = geometry,
          Title = title,
          Resizable = true,
          Collapsed = false,
          Closable = true,
          Transparent = true,
          Border = true,
        };

        oid.OnNuiEvent -= HandleItemAppearanceEvents;
        oid.OnNuiEvent += HandleItemAppearanceEvents;

        int token = oid.CreateNuiWindow(window, windowId);

        PlayerPlugin.ApplyLoopingVisualEffectToObject(oid.ControlledCreature, oid.ControlledCreature, 173);
        
        title.SetBindValue(oid, token, $"Modifier l'apparence de {item.Name}");
        symmetry.SetBindValue(oid, token, false);
        areaSelection.SetBindValue(oid, token, 0);

        robeSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Robe));
        robeSlider.SetBindValue(oid, token, robeList.IndexOf(robeList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Robe))));
        
        neckSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Neck));
        neckSlider.SetBindValue(oid, token, neckList.IndexOf(neckList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Neck))));

        torsoSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso));
        torsoSlider.SetBindValue(oid, token, torsoList.IndexOf(torsoList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Torso))));

        beltSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Belt));
        beltSlider.SetBindValue(oid, token, beltList.IndexOf(beltList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Belt))));

        pelvisSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Pelvis));
        pelvisSlider.SetBindValue(oid, token, pelvisList.IndexOf(pelvisList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.Pelvis))));

        rightShoulderSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder));
        rightShoulderSlider.SetBindValue(oid, token, shoulderList.IndexOf(shoulderList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShoulder))));
        leftShoulderSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder));
        leftShoulderSlider.SetBindValue(oid, token, shoulderList.IndexOf(shoulderList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShoulder))));

        rightBicepSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep));
        rightBicepSlider.SetBindValue(oid, token, bicepList.IndexOf(bicepList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightBicep))));
        leftBicepSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep));
        leftBicepSlider.SetBindValue(oid, token, bicepList.IndexOf(bicepList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftBicep))));

        rightForearmSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm));
        rightForearmSlider.SetBindValue(oid, token, forearmList.IndexOf(forearmList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightForearm))));
        leftForearmSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm));
        leftForearmSlider.SetBindValue(oid, token, forearmList.IndexOf(forearmList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftForearm))));

        rightHandSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand));
        rightHandSlider.SetBindValue(oid, token, handList.IndexOf(handList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightHand))));
        leftHandSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand));
        leftHandSlider.SetBindValue(oid, token, handList.IndexOf(handList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftHand))));

        rightTighSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh));
        rightTighSlider.SetBindValue(oid, token, tighList.IndexOf(tighList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightThigh))));
        leftTighSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh));
        leftTighSlider.SetBindValue(oid, token, tighList.IndexOf(tighList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftThigh))));

        rightShinSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin));
        rightShinSlider.SetBindValue(oid, token, shinList.IndexOf(shinList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightShin))));
        leftShinSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin));
        leftShinSlider.SetBindValue(oid, token, shinList.IndexOf(shinList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftShin))));

        rightFootSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot));
        rightFootSlider.SetBindValue(oid, token, footList.IndexOf(footList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.RightFoot))));
        leftFootSelection.SetBindValue(oid, token, item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot));
        leftFootSlider.SetBindValue(oid, token, footList.IndexOf(footList.FirstOrDefault(l => l.Value == item.Appearance.GetArmorModel(ItemAppearanceArmorModel.LeftFoot))));

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);

        Task waitWindowOpened = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.6));

          areaSelection.SetBindWatch(oid, token, true);

          robeSelection.SetBindWatch(oid, token, true);
          robeSlider.SetBindWatch(oid, token, true);

          neckSelection.SetBindWatch(oid, token, true);
          neckSlider.SetBindWatch(oid, token, true);

          torsoSelection.SetBindWatch(oid, token, true);
          torsoSlider.SetBindWatch(oid, token, true);

          beltSelection.SetBindWatch(oid, token, true);
          beltSlider.SetBindWatch(oid, token, true);

          pelvisSelection.SetBindWatch(oid, token, true);
          pelvisSlider.SetBindWatch(oid, token, true);

          rightShoulderSelection.SetBindWatch(oid, token, true);
          rightShoulderSlider.SetBindWatch(oid, token, true);
          leftShoulderSelection.SetBindWatch(oid, token, true);
          leftShoulderSlider.SetBindWatch(oid, token, true);

          rightBicepSelection.SetBindWatch(oid, token, true);
          rightBicepSlider.SetBindWatch(oid, token, true);
          leftBicepSelection.SetBindWatch(oid, token, true);
          leftBicepSlider.SetBindWatch(oid, token, true);

          rightForearmSelection.SetBindWatch(oid, token, true);
          rightForearmSlider.SetBindWatch(oid, token, true);
          leftForearmSelection.SetBindWatch(oid, token, true);
          leftForearmSlider.SetBindWatch(oid, token, true);

          rightHandSelection.SetBindWatch(oid, token, true);
          rightHandSlider.SetBindWatch(oid, token, true);
          leftHandSelection.SetBindWatch(oid, token, true);
          leftHandSlider.SetBindWatch(oid, token, true);

          rightTighSelection.SetBindWatch(oid, token, true);
          rightTighSlider.SetBindWatch(oid, token, true);
          leftTighSelection.SetBindWatch(oid, token, true);
          leftTighSlider.SetBindWatch(oid, token, true);

          rightFootSelection.SetBindWatch(oid, token, true);
          rightFootSlider.SetBindWatch(oid, token, true);
          leftFootSelection.SetBindWatch(oid, token, true);
          leftFootSlider.SetBindWatch(oid, token, true);
        });
      }

      public void HandleArmorSliderChange(string sliderId, string selectorId, int windowToken, ItemAppearanceArmorModel model)
      {
        int sliderValue = new NuiBind<int>(sliderId).GetBindValue(oid, windowToken);
        NuiBind<int> selector = new NuiBind<int>(selectorId);
        int selectedValue = selector.GetBindValue(oid, windowToken);

        if (sliderValue == selectedValue)
          return;

        NwItem armor = oid.ControlledCreature.GetItemInSlot(InventorySlot.Chest);

        if (armor == null)
          return;

        if (armor.BaseACValue != oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARMOR_APPEARANCE_MODIFIER").Value)
        {
          oid.NuiDestroy(windowToken);
          CreateItemAppearanceWindow(armor);
          return;
        }

        int result = 0;
        int nModel = (int)model;
        ItemAppearanceArmorModel modelSymmetry = model;
        if (new NuiBind<bool>("symmetry").GetBindValue(oid, windowToken) && (nModel < 6 || (nModel > 9 && nModel < 18)))
        {
          if (nModel % 2 == 0)
            modelSymmetry = (ItemAppearanceArmorModel)(nModel + 1);
          else
            modelSymmetry = (ItemAppearanceArmorModel)(nModel - 1);
        }

        switch (model)
        {
          case ItemAppearanceArmorModel.Robe:
            result = RobeParts2da.robePartsTable.GetValidRobeAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.Neck:
            result = NeckParts2da.neckPartsTable.GetValidNeckAppearances().ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.Torso:
            result = TorsoParts2da.torsoPartsTable.GetValidChestAppearancesForGenderAndAC(oid.ControlledCreature.Gender, armor.BaseACValue).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.Belt:
            result = BeltParts2da.beltPartsTable.GetValidBeltAppearances().ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.Pelvis:
            result = PelvisParts2da.pelvisPartsTable.GetValidPelvisAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.LeftShoulder:
          case ItemAppearanceArmorModel.RightShoulder:
            result = ShoulderParts2da.shoulderPartsTable.GetValidShoulderAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.LeftBicep:
          case ItemAppearanceArmorModel.RightBicep:
            result = BicepParts2da.bicepPartsTable.GetValidBicepAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.LeftForearm:
          case ItemAppearanceArmorModel.RightForearm:
            result = ForearmParts2da.forearmPartsTable.GetValidForearmAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.LeftHand:
          case ItemAppearanceArmorModel.RightHand:
            result = HandParts2da.handPartsTable.GetValidHandAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.LeftThigh:
          case ItemAppearanceArmorModel.RightThigh:
            result = LegParts2da.legPartsTable.GetValidLegAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.LeftShin:
          case ItemAppearanceArmorModel.RightShin:
            result = ShinParts2da.shinPartsTable.GetValidShinAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
          case ItemAppearanceArmorModel.LeftFoot:
          case ItemAppearanceArmorModel.RightFoot:
            result = FootParts2da.footPartsTable.GetValidFootAppearancesForGender(oid.ControlledCreature.Gender).ElementAt(sliderValue).Value;
            break;
        }

        armor.Appearance.SetArmorModel(model, (byte)result);
        if(modelSymmetry != model)
          armor.Appearance.SetArmorModel(modelSymmetry, (byte)result);

        oid.ControlledCreature.RunUnequip(armor);

        Task waitUnequip = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oid.ControlledCreature.RunEquip(armor, InventorySlot.Chest);
        });

        selector.SetBindWatch(oid, windowToken, false);
        selector.SetBindValue(oid, windowToken, result);
        

        if (modelSymmetry != model)
        {
          string sliderSimId;
          string selectorSimId;

          if(sliderId.Contains("left"))
          {
            sliderSimId = sliderId.Replace("left", "right");
            selectorSimId = selectorId.Replace("left", "right");
          }
          else
          {
            sliderSimId = sliderId.Replace("right", "left");
            selectorSimId = selectorId.Replace("right", "left");
          }

          NuiBind<int> symmetrySlider = new NuiBind<int>(sliderSimId);
          NuiBind<int> symmetrySelector = new NuiBind<int>(selectorSimId);

          symmetrySelector.SetBindWatch(oid, windowToken, false);
          symmetrySelector.SetBindValue(oid, windowToken, result);
  
          symmetrySlider.SetBindWatch(oid, windowToken, false);
          symmetrySlider.SetBindValue(oid, windowToken, sliderValue);

          symmetrySelector.SetBindWatch(oid, windowToken, true);
          symmetrySlider.SetBindWatch(oid, windowToken, true);
        }

        selector.SetBindWatch(oid, windowToken, true);
      }
      public void HandleArmorSelectorChange(string sliderId, string selectorId, int windowToken, ItemAppearanceArmorModel model)
      {
        int selectorValue = new NuiBind<int>(selectorId).GetBindValue(oid, windowToken);
        NuiBind<int> slider = new NuiBind<int>(sliderId);
        int sliderValue = slider.GetBindValue(oid, windowToken);

        if (selectorValue == sliderValue)
          return;

        NwItem armor = oid.ControlledCreature.GetItemInSlot(InventorySlot.Chest);

        if (armor == null)
          return;

        if (armor.BaseACValue != oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARMOR_APPEARANCE_MODIFIER").Value)
        {
          oid.NuiDestroy(windowToken);
          CreateItemAppearanceWindow(armor);
          return;
        }

        int nModel = (int)model;
        ItemAppearanceArmorModel modelSymmetry = model;
        if (new NuiBind<bool>("symmetry").GetBindValue(oid, windowToken) && (nModel < 6 || (nModel > 9 && nModel < 18)))
        {
          if (nModel % 2 == 0)
            modelSymmetry = (ItemAppearanceArmorModel)(nModel + 1);
          else
            modelSymmetry = (ItemAppearanceArmorModel)(nModel - 1);
        }

        armor.Appearance.SetArmorModel(model, (byte)selectorValue);
        if (modelSymmetry != model)
          armor.Appearance.SetArmorModel(modelSymmetry, (byte)selectorValue);

        oid.ControlledCreature.RunUnequip(armor);

        Task waitUnequip = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          oid.ControlledCreature.RunEquip(armor, InventorySlot.Chest);
        });

        slider.SetBindWatch(oid, windowToken, false);
        slider.SetBindValue(oid, windowToken, selectorValue);
        
        if (modelSymmetry != model)
        {
          string sliderSimId;
          string selectorSimId;

          if (sliderId.Contains("left"))
          {
            sliderSimId = sliderId.Replace("left", "right");
            selectorSimId = selectorId.Replace("left", "right");
          }
          else
          {
            sliderSimId = sliderId.Replace("right", "left");
            selectorSimId = selectorId.Replace("right", "left");
          }

          NuiBind<int> symmetrySlider = new NuiBind<int>(sliderSimId);
          NuiBind<int> symmetrySelector = new NuiBind<int>(selectorSimId);

          symmetrySelector.SetBindWatch(oid, windowToken, false);
          symmetrySelector.SetBindValue(oid, windowToken, selectorValue);

          symmetrySlider.SetBindWatch(oid, windowToken, false);
          symmetrySlider.SetBindValue(oid, windowToken, sliderValue);

          symmetrySelector.SetBindWatch(oid, windowToken, true);
          symmetrySlider.SetBindWatch(oid, windowToken, true);
        }

        slider.SetBindWatch(oid, windowToken, true);
      }
    }
  }
}
