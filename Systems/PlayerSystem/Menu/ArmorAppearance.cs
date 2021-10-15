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
      public void CreateArmorAppearanceWindow(NwItem item)
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
                new NuiSpacer { Width = windowRectangle.Width - 220 },
                new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 70 }
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
        NuiWindow window = new NuiWindow(root, title)
        {
          Geometry = geometry,
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

      public void HandleArmorSliderChange(string sliderId, string selectorId, int windowToken, ItemAppearanceArmorModel model, NwItem armor)
      {
        int sliderValue = new NuiBind<int>(sliderId).GetBindValue(oid, windowToken);
        NuiBind<int> selector = new NuiBind<int>(selectorId);
        int selectedValue = selector.GetBindValue(oid, windowToken);

        if (sliderValue == selectedValue || armor == null)
          return;

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
      public void HandleArmorSelectorChange(string sliderId, string selectorId, int windowToken, ItemAppearanceArmorModel model, NwItem armor)
      {
        int selectorValue = new NuiBind<int>(selectorId).GetBindValue(oid, windowToken);
        NuiBind<int> slider = new NuiBind<int>(sliderId);
        int sliderValue = slider.GetBindValue(oid, windowToken);

        if (selectorValue == sliderValue || armor == null)
          return;

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
      private void HandleItemAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
      {
        if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != "itemAppearanceModifier" || !Players.TryGetValue(nuiEvent.Player.LoginCreature, out Player player))
          return;

        if (nuiEvent.EventType == NuiEventType.Close)
        {
          PlayerPlugin.ApplyLoopingVisualEffectToObject(nuiEvent.Player.ControlledCreature, nuiEvent.Player.ControlledCreature, 173);
          return;
        }

        NwItem item = nuiEvent.Player.LoginCreature.GetObjectVariable<LocalVariableObject<NwItem>>("_ITEM_SELECTED_FOR_MODIFICATION").Value;

        if(!item.IsValid || item.Possessor != nuiEvent.Player.ControlledCreature)
        {
          nuiEvent.Player.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
          nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
          return;
        }

        if (nuiEvent.EventType == NuiEventType.Click && nuiEvent.ElementId == "openColors")
        {
          nuiEvent.Player.NuiDestroy(nuiEvent.WindowToken);
          player.CreateArmorColorsWindow(item);
          return;
        }

        if (nuiEvent.EventType == NuiEventType.Watch)
          switch (nuiEvent.ElementId)
          {
            case "robeSliderValue":
              player.HandleArmorSliderChange("robeSliderValue", "robeSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Robe, item);
              break;

            case "robeSelection":
              player.HandleArmorSelectorChange("robeSliderValue", "robeSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Robe, item);
              break;

            case "neckSliderValue":
              player.HandleArmorSliderChange("neckSliderValue", "neckSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Neck, item);
              break;

            case "neckSelection":
              player.HandleArmorSelectorChange("neckSliderValue", "neckSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Neck, item);
              break;

            case "torsoSliderValue":
              player.HandleArmorSliderChange("torsoSliderValue", "torsoSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Torso, item);
              break;

            case "torsoSelection":
              player.HandleArmorSelectorChange("torsoSliderValue", "torsoSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Torso, item);
              break;

            case "beltSliderValue":
              player.HandleArmorSliderChange("beltSliderValue", "beltSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Belt, item);
              break;

            case "beltSelection":
              player.HandleArmorSelectorChange("beltSliderValue", "beltSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Belt, item);
              break;

            case "pelvisSliderValue":
              player.HandleArmorSliderChange("pelvisSliderValue", "pelvisSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Pelvis, item);
              break;

            case "pelvisSelection":
              player.HandleArmorSelectorChange("pelvisSliderValue", "pelvisSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.Pelvis, item);
              break;

            case "rightShoulderSliderValue":
              player.HandleArmorSliderChange("rightShoulderSliderValue", "rightShoulderSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightShoulder, item);
              break;

            case "rightShoulderSelection":
              player.HandleArmorSelectorChange("rightShoulderSliderValue", "rightShoulderSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightShoulder, item);
              break;

            case "leftShoulderSliderValue":
              player.HandleArmorSliderChange("leftShoulderSliderValue", "leftShoulderSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftShoulder, item);
              break;

            case "leftShoulderSelection":
              player.HandleArmorSelectorChange("leftShoulderSliderValue", "leftShoulderSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftShoulder, item);
              break;

            case "rightBicepSliderValue":
              player.HandleArmorSliderChange("rightBicepSliderValue", "rightBicepSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightBicep, item);
              break;

            case "rightBicepSelection":
              player.HandleArmorSelectorChange("rightBicepSliderValue", "rightBicepSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightBicep, item);
              break;

            case "leftBicepSliderValue":
              player.HandleArmorSliderChange("leftBicepSliderValue", "leftBicepSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftBicep, item);
              break;

            case "leftBicepSelection":
              player.HandleArmorSelectorChange("leftBicepSliderValue", "leftBicepSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftBicep, item);
              break;

            case "rightForearmSliderValue":
              player.HandleArmorSliderChange("rightForearmSliderValue", "rightForearmSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightForearm, item);
              break;

            case "rightForearmSelection":
              player.HandleArmorSelectorChange("rightForearmSliderValue", "rightForearmSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightForearm, item);
              break;

            case "leftForearmSliderValue":
              player.HandleArmorSliderChange("leftForearmSliderValue", "leftForearmSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftForearm, item);
              break;

            case "leftForearmSelection":
              player.HandleArmorSelectorChange("leftForearmSliderValue", "leftForearmSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftForearm, item);
              break;

            case "rightHandSliderValue":
              player.HandleArmorSliderChange("rightHandSliderValue", "rightHandSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightHand, item);
              break;

            case "rightHandSelection":
              player.HandleArmorSelectorChange("rightHandSliderValue", "rightHandSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightHand, item);
              break;

            case "leftHandSliderValue":
              player.HandleArmorSliderChange("leftForearmSliderValue", "leftHandSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftHand, item);
              break;

            case "leftHandSelection":
              player.HandleArmorSelectorChange("leftForearmSliderValue", "leftHandSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftHand, item);
              break;

            case "rightTighSliderValue":
              player.HandleArmorSliderChange("rightTighSliderValue", "rightTighSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightThigh, item);
              break;

            case "rightTighSelection":
              player.HandleArmorSelectorChange("rightTighSliderValue", "rightTighSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightThigh, item);
              break;

            case "leftTighSliderValue":
              player.HandleArmorSliderChange("leftTighSliderValue", "leftTighSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftThigh, item);
              break;

            case "leftTighSelection":
              player.HandleArmorSelectorChange("leftTighSliderValue", "leftTighSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftThigh, item);
              break;

            case "rightShinSliderValue":
              player.HandleArmorSliderChange("rightShinSliderValue", "rightShinSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightShin, item);
              break;

            case "rightShinSelection":
              player.HandleArmorSelectorChange("rightShinSliderValue", "rightShinSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightShin, item);
              break;

            case "leftShinSliderValue":
              player.HandleArmorSliderChange("leftShinSliderValue", "leftShinSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftShin, item);
              break;

            case "leftShinSelection":
              player.HandleArmorSelectorChange("leftShinSliderValue", "leftShinSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftShin, item);
              break;

            case "rightFootSliderValue":
              player.HandleArmorSliderChange("rightFootSliderValue", "rightFootSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightFoot, item);
              break;

            case "rightFootSelection":
              player.HandleArmorSelectorChange("rightFootSliderValue", "rightFootSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.RightFoot, item);
              break;

            case "leftFootSliderValue":
              player.HandleArmorSliderChange("leftFootSliderValue", "leftFootSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftFoot, item);
              break;

            case "leftFootSelection":
              player.HandleArmorSelectorChange("leftFootSliderValue", "leftFootSelection", nuiEvent.WindowToken, ItemAppearanceArmorModel.LeftFoot, item);
              break;
          }
      }
    }
  }
}
