using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ArmorCustomizationWindow : PlayerWindow
      {
        private NwItem item { get; set; }

        private const float BUTTONSIZE = 12;
        private const float COMBOSIZE = 100;

        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChildren = new ();

        private NuiBind<string> lastClickedColorButton;

        private readonly NuiBind<string> globalLeather1 = new("globalLeather1");
        private readonly NuiBind<string> globalCloth1 = new("globalCloth1");
        private readonly NuiBind<string> globalMetal1 = new("globalMetal1");
        private readonly NuiBind<string> globalLeather2 = new("globalLeather2");
        private readonly NuiBind<string> globalCloth2 = new("globalCloth2");
        private readonly NuiBind<string> globalMetal2 = new("globalMetal2");

        private readonly NuiBind<int> robeSelection = new("robeSelection");
        private readonly NuiBind<int> neckSelection = new("neckSelection");
        private readonly NuiBind<int> torsoSelection = new("torsoSelection");
        private readonly NuiBind<int> beltSelection = new("beltSelection");
        private readonly NuiBind<int> pelvisSelection = new("pelvisSelection");
        private readonly NuiBind<int> rightShoulderSelection = new("rightShoulderSelection");
        private readonly NuiBind<int> leftShoulderSelection = new("leftShoulderSelection");
        private readonly NuiBind<int> rightBicepSelection = new("rightBicepSelection");
        private readonly NuiBind<int> leftBicepSelection = new("leftBicepSelection");
        private readonly NuiBind<int> rightForearmSelection = new("rightForearmSelection");
        private readonly NuiBind<int> leftForearmSelection = new("leftForearmSelection");
        private readonly NuiBind<int> rightHandSelection = new("rightHandSelection");
        private readonly NuiBind<int> leftHandSelection = new("leftHandSelection");
        private readonly NuiBind<int> rightThighSelection = new("rightTighSelection");
        private readonly NuiBind<int> leftThighSelection = new("leftTighSelection");
        private readonly NuiBind<int> rightShinSelection = new("rightShinSelection");
        private readonly NuiBind<int> leftShinSelection = new("leftShinSelection");
        private readonly NuiBind<int> rightFootSelection = new("rightFootSelection");
        private readonly NuiBind<int> leftFootSelection = new("leftFootSelection");

        private readonly NuiBind<string> leftShoulderLeather1 = new("leftShoulderLeather1");
        private readonly NuiBind<string> leftShoulderCloth1 = new("leftShoulderCloth1");
        private readonly NuiBind<string> leftShoulderMetal1 = new("leftShoulderMetal1");
        private readonly NuiBind<string> leftShoulderLeather2 = new("leftShoulderLeather2");
        private readonly NuiBind<string> leftShoulderCloth2 = new("leftShoulderCloth2");
        private readonly NuiBind<string> leftShoulderMetal2 = new("leftShoulderMetal2");

        private readonly NuiBind<string> rightShoulderLeather1 = new("rightShoulderLeather1");
        private readonly NuiBind<string> rightShoulderCloth1 = new("rightShoulderCloth1");
        private readonly NuiBind<string> rightShoulderMetal1 = new("rightShoulderMetal1");
        private readonly NuiBind<string> rightShoulderLeather2 = new("rightShoulderLeather2");
        private readonly NuiBind<string> rightShoulderCloth2 = new("rightShoulderCloth2");
        private readonly NuiBind<string> rightShoulderMetal2 = new("rightShoulderMetal2");

        private readonly NuiBind<string> neckLeather1 = new("neckLeather1");
        private readonly NuiBind<string> neckCloth1 = new("neckCloth1");
        private readonly NuiBind<string> neckMetal1 = new("neckMetal1");
        private readonly NuiBind<string> neckLeather2 = new("neckLeather2");
        private readonly NuiBind<string> neckCloth2 = new("neckCloth2");
        private readonly NuiBind<string> neckMetal2 = new("neckMetal2");

        private readonly NuiBind<string> leftBicepLeather1 = new("leftBicepLeather1");
        private readonly NuiBind<string> leftBicepCloth1 = new("leftBicepCloth1");
        private readonly NuiBind<string> leftBicepMetal1 = new("leftBicepMetal1");
        private readonly NuiBind<string> leftBicepLeather2 = new("leftBicepLeather2");
        private readonly NuiBind<string> leftBicepCloth2 = new("leftBicepCloth2");
        private readonly NuiBind<string> leftBicepMetal2 = new("leftBicepMetal2");

        private readonly NuiBind<string> rightBicepLeather1 = new("rightBicepLeather1");
        private readonly NuiBind<string> rightBicepCloth1 = new("rightBicepCloth1");
        private readonly NuiBind<string> rightBicepMetal1 = new("rightBicepMetal1");
        private readonly NuiBind<string> rightBicepLeather2 = new("rightBicepLeather2");
        private readonly NuiBind<string> rightBicepCloth2 = new("rightBicepCloth2");
        private readonly NuiBind<string> rightBicepMetal2 = new("rightBicepMetal2");

        private readonly NuiBind<string> torsoLeather1 = new("torsoLeather1");
        private readonly NuiBind<string> torsoCloth1 = new("torsoCloth1");
        private readonly NuiBind<string> torsoMetal1 = new("torsoMetal1");
        private readonly NuiBind<string> torsoLeather2 = new("torsoLeather2");
        private readonly NuiBind<string> torsoCloth2 = new("torsoCloth2");
        private readonly NuiBind<string> torsoMetal2 = new("torsoMetal2");

        private readonly NuiBind<string> leftForearmLeather1 = new("leftForearmLeather1");
        private readonly NuiBind<string> leftForearmCloth1 = new("leftForearmCloth1");
        private readonly NuiBind<string> leftForearmMetal1 = new("leftForearmMetal1");
        private readonly NuiBind<string> leftForearmLeather2 = new("leftForearmLeather2");
        private readonly NuiBind<string> leftForearmCloth2 = new("leftForearmCloth2");
        private readonly NuiBind<string> leftForearmMetal2 = new("leftForearmMetal2");

        private readonly NuiBind<string> rightForearmLeather1 = new("rightForearmLeather1");
        private readonly NuiBind<string> rightForearmCloth1 = new("rightForearmCloth1");
        private readonly NuiBind<string> rightForearmMetal1 = new("rightForearmMetal1");
        private readonly NuiBind<string> rightForearmLeather2 = new("rightForearmLeather2");
        private readonly NuiBind<string> rightForearmCloth2 = new("rightForearmCloth2");
        private readonly NuiBind<string> rightForearmMetal2 = new("rightForearmMetal2");

        private readonly NuiBind<string> beltLeather1 = new("beltLeather1");
        private readonly NuiBind<string> beltCloth1 = new("beltCloth1");
        private readonly NuiBind<string> beltMetal1 = new("beltMetal1");
        private readonly NuiBind<string> beltLeather2 = new("beltLeather2");
        private readonly NuiBind<string> beltCloth2 = new("beltCloth2");
        private readonly NuiBind<string> beltMetal2 = new("beltMetal2");

        private readonly NuiBind<string> leftHandLeather1 = new("leftHandLeather1");
        private readonly NuiBind<string> leftHandCloth1 = new("leftHandCloth1");
        private readonly NuiBind<string> leftHandMetal1 = new("leftHandMetal1");
        private readonly NuiBind<string> leftHandLeather2 = new("leftHandLeather2");
        private readonly NuiBind<string> leftHandCloth2 = new("leftHandCloth2");
        private readonly NuiBind<string> leftHandMetal2 = new("leftHandMetal2");

        private readonly NuiBind<string> rightHandLeather1 = new("rightHandLeather1");
        private readonly NuiBind<string> rightHandCloth1 = new("rightHandCloth1");
        private readonly NuiBind<string> rightHandMetal1 = new("rightHandMetal1");
        private readonly NuiBind<string> rightHandLeather2 = new("rightHandLeather2");
        private readonly NuiBind<string> rightHandCloth2 = new("rightHandCloth2");
        private readonly NuiBind<string> rightHandMetal2 = new("rightHandMetal2");

        private readonly NuiBind<string> pelvisLeather1 = new("pelvisLeather1");
        private readonly NuiBind<string> pelvisCloth1 = new("pelvisCloth1");
        private readonly NuiBind<string> pelvisMetal1 = new("pelvisMetal1");
        private readonly NuiBind<string> pelvisLeather2 = new("pelvisLeather2");
        private readonly NuiBind<string> pelvisCloth2 = new("pelvisCloth2");
        private readonly NuiBind<string> pelvisMetal2 = new("pelvisMetal2");

        private readonly NuiBind<string> leftThighLeather1 = new("leftThighLeather1");
        private readonly NuiBind<string> leftThighCloth1 = new("leftThighCloth1");
        private readonly NuiBind<string> leftThighMetal1 = new("leftThighMetal1");
        private readonly NuiBind<string> leftThighLeather2 = new("leftThighLeather2");
        private readonly NuiBind<string> leftThighCloth2 = new("leftThighCloth2");
        private readonly NuiBind<string> leftThighMetal2 = new("leftThighMetal2");

        private readonly NuiBind<string> rightThighLeather1 = new("rightThighLeather1");
        private readonly NuiBind<string> rightThighCloth1 = new("rightThighCloth1");
        private readonly NuiBind<string> rightThighMetal1 = new("rightThighMetal1");
        private readonly NuiBind<string> rightThighLeather2 = new("rightThighLeather2");
        private readonly NuiBind<string> rightThighCloth2 = new("rightThighCloth2");
        private readonly NuiBind<string> rightThighMetal2 = new("rightThighMetal2");

        private readonly NuiBind<string> robeLeather1 = new("robeLeather1");
        private readonly NuiBind<string> robeCloth1 = new("robeCloth1");
        private readonly NuiBind<string> robeMetal1 = new("robeMetal1");
        private readonly NuiBind<string> robeLeather2 = new("robeLeather2");
        private readonly NuiBind<string> robeCloth2 = new("robeCloth2");
        private readonly NuiBind<string> robeMetal2 = new("robeMetal2");

        private readonly NuiBind<string> leftShinLeather1 = new("leftShinLeather1");
        private readonly NuiBind<string> leftShinCloth1 = new("leftShinCloth1");
        private readonly NuiBind<string> leftShinMetal1 = new("leftShinMetal1");
        private readonly NuiBind<string> leftShinLeather2 = new("leftShinLeather2");
        private readonly NuiBind<string> leftShinCloth2 = new("leftShinCloth2");
        private readonly NuiBind<string> leftShinMetal2 = new("leftShinMetal2");

        private readonly NuiBind<string> rightShinLeather1 = new("rightShinLeather1");
        private readonly NuiBind<string> rightShinCloth1 = new("rightShinCloth1");
        private readonly NuiBind<string> rightShinMetal1 = new("rightShinMetal1");
        private readonly NuiBind<string> rightShinLeather2 = new("rightShinLeather2");
        private readonly NuiBind<string> rightShinCloth2 = new("rightShinCloth2");
        private readonly NuiBind<string> rightShinMetal2 = new("rightShinMetal2");

        private readonly NuiBind<string> leftFootLeather1 = new("leftFootLeather1");
        private readonly NuiBind<string> leftFootCloth1 = new("leftFootCloth1");
        private readonly NuiBind<string> leftFootMetal1 = new("leftFootMetal1");
        private readonly NuiBind<string> leftFootLeather2 = new("leftFootLeather2");
        private readonly NuiBind<string> leftFootCloth2 = new("leftFootCloth2");
        private readonly NuiBind<string> leftFootMetal2 = new("leftFootMetal2");

        private readonly NuiBind<string> rightFootLeather1 = new("rightFootLeather1");
        private readonly NuiBind<string> rightFootCloth1 = new("rightFootCloth1");
        private readonly NuiBind<string> rightFootMetal1 = new("rightFootMetal1");
        private readonly NuiBind<string> rightFootLeather2 = new("rightFootLeather2");
        private readonly NuiBind<string> rightFootCloth2 = new("rightFootCloth2");
        private readonly NuiBind<string> rightFootMetal2 = new("rightFootMetal2");

        private readonly NuiBind<List<NuiComboEntry>> shoulderList = new NuiBind<List<NuiComboEntry>>("shoulderList");
        private readonly NuiBind<List<NuiComboEntry>> robeList = new NuiBind<List<NuiComboEntry>>("robeList");
        private readonly NuiBind<List<NuiComboEntry>> torsoList = new NuiBind<List<NuiComboEntry>>("torsoList");
        private readonly NuiBind<List<NuiComboEntry>> pelvisList = new NuiBind<List<NuiComboEntry>>("pelvisList");
        private readonly NuiBind<List<NuiComboEntry>> bicepList = new NuiBind<List<NuiComboEntry>>("bicepList");
        private readonly NuiBind<List<NuiComboEntry>> forearmList = new NuiBind<List<NuiComboEntry>>("forearmList");
        private readonly NuiBind<List<NuiComboEntry>> handList = new NuiBind<List<NuiComboEntry>>("handList");
        private readonly NuiBind<List<NuiComboEntry>> thighList = new NuiBind<List<NuiComboEntry>>("thighList");
        private readonly NuiBind<List<NuiComboEntry>> shinList = new NuiBind<List<NuiComboEntry>>("shinList");
        private readonly NuiBind<List<NuiComboEntry>> footList = new NuiBind<List<NuiComboEntry>>("footList");
        private readonly NuiBind<List<NuiComboEntry>> neckList = new NuiBind<List<NuiComboEntry>>("neckList");
        private readonly NuiBind<List<NuiComboEntry>> beltList = new NuiBind<List<NuiComboEntry>>("beltList");

        private ItemAppearanceArmorColor selectedColorChannel { get; set; }
        private CreaturePart selectedArmorPart { get; set; }

        public ArmorCustomizationWindow(Player player, NwItem item) : base(player)
        {
          windowId = "itemColorsModifier";

          List<NuiElement> paletteColumnChildren = new List<NuiElement>();
          NuiColumn paletteColumn = new() { Margin = 0.0f, Children = paletteColumnChildren };
          List<NuiElement> globalColorColumnChildren = new List<NuiElement>();
          NuiColumn globalColorColumn = new() { Margin = 0.0f, Children = globalColorColumnChildren };
          List<NuiElement> paletteRowChildren = new List<NuiElement>();
          NuiRow paletteRow = new() { Margin = 0.0f, Children = paletteRowChildren, Height = 190 };
          paletteRowChildren.Add(paletteColumn);
          paletteRowChildren.Add(globalColorColumn);

          rootChildren.Add(paletteRow);

          int nbButton = 0;

          for (int i = 0; i < 13; i++)
          {
            NuiRow row = new NuiRow() { Margin = 0.0f };
            List<NuiElement> rowChildren = new List<NuiElement>();

            for (int j = 0; j < 22; j++)
            {
              if (nbButton > 255)
                break;

              rowChildren.Add(new NuiButton("")
              {
                Id = $"{nbButton}",
                Width = 15,
                Height = 15,
                Margin = 0.0f,
                DrawList = new() { new NuiDrawListImage(Utils.paletteColorBindings[nbButton], new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } }
              });

              nbButton++;
            }

            row.Children = rowChildren;
            paletteColumnChildren.Add(row);
          }

          List<NuiElement> globalColorRowChildren1 = new List<NuiElement>();
          NuiRow globalColorRow1 = new() { Margin = 0.0f, Children = globalColorRowChildren1 };

          List<NuiElement> globalColorRowChildren2 = new List<NuiElement>();
          NuiRow globalColorRow2 = new() { Margin = 0.0f, Children = globalColorRowChildren2 };

          globalColorColumnChildren.Add(globalColorRow1);
          globalColorColumnChildren.Add(globalColorRow2);

          globalColorColumnChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Nom & Description") { Id = "loadItemNameEditor", Width = 150, Height = 50} } });

          globalColorRowChildren1.Add(new NuiSpacer());
          globalColorRowChildren1.Add(new NuiButton("")
          {
            Id = "leather1",
            Width = 50,
            Height = 50,
            Margin = 0.0f,
            Tooltip = "Cuir 1 global",
            DrawList = new() { new NuiDrawListImage(globalLeather1, new NuiRect(4, 4, 45, 45)) { Aspect = NuiAspect.Stretch } }
          });
          globalColorRowChildren1.Add(new NuiButton("")
          {
            Id = "cloth1",
            Width = 50,
            Height = 50,
            Margin = 0.0f,
            Tooltip = "Tissu 1 global",
            DrawList = new() { new NuiDrawListImage(globalCloth1, new NuiRect(4, 4, 45, 45)) { Aspect = NuiAspect.Stretch } }
          });
          globalColorRowChildren1.Add(new NuiButton("")
          {
            Id = "metal1",
            Width = 50,
            Height = 50,
            Margin = 0.0f,
            Tooltip = "Métal 1 global",
            DrawList = new() { new NuiDrawListImage(globalMetal1, new NuiRect(4, 4, 45, 45)) { Aspect = NuiAspect.Stretch } }
          });
          globalColorRowChildren1.Add(new NuiSpacer());

          globalColorRowChildren2.Add(new NuiSpacer());
          globalColorRowChildren2.Add(new NuiButton("")
          {
            Id = "leather2",
            Width = 50,
            Height = 50,
            Margin = 0.0f,
            Tooltip = "Cuir 2 global",
            DrawList = new() { new NuiDrawListImage(globalLeather2, new NuiRect(4, 4, 45, 45)) { Aspect = NuiAspect.Stretch } }
          });
          globalColorRowChildren2.Add(new NuiButton("")
          {
            Id = "cloth2",
            Width = 50,
            Height = 50,
            Margin = 0.0f,
            Tooltip = "Tissu 2 global",
            DrawList = new() { new NuiDrawListImage(globalCloth2, new NuiRect(4, 4, 45, 45)) { Aspect = NuiAspect.Stretch } }
          });
          globalColorRowChildren2.Add(new NuiButton("")
          {
            Id = "metal2",
            Width = 50,
            Height = 50,
            Margin = 0.0f,
            Tooltip = "Métal 2 global",
            DrawList = new() { new NuiDrawListImage(globalMetal2, new NuiRect(4, 4, 45, 45)) { Aspect = NuiAspect.Stretch } }
          });
          globalColorRowChildren2.Add(new NuiSpacer());

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Epaule gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "leftShoulderDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = shoulderList, Selected = leftShoulderSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "leftShoulderIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftShoulderLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule gauche - Cuir 1", DrawList = new() { new NuiDrawListImage(leftShoulderLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShoulderCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule gauche - Tissu 1", DrawList = new() { new NuiDrawListImage(leftShoulderCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShoulderMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule gauche - Métal 1", DrawList = new() { new NuiDrawListImage(leftShoulderMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftShoulderLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule gauche - Cuir 2", DrawList = new() { new NuiDrawListImage(leftShoulderLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShoulderCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule gauche - Tissu 2", DrawList = new() { new NuiDrawListImage(leftShoulderCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShoulerdMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule gauche - Métal 2", DrawList = new() { new NuiDrawListImage(leftShoulderMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Cou") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f,Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "neckDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = neckList, Selected = neckSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "neckIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f,Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "neckLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cou - Cuir 1", DrawList = new() { new NuiDrawListImage(neckLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "neckCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cou - Tissu 1", DrawList = new() { new NuiDrawListImage(neckCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "neckMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cou - Métal 1", DrawList = new() { new NuiDrawListImage(neckMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "neckLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cou - Cuir 2", DrawList = new() { new NuiDrawListImage(neckLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "neckCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cou - Tissu 2", DrawList = new() { new NuiDrawListImage(neckCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "neckMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cou - Métal 2", DrawList = new() { new NuiDrawListImage(neckMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Epaule droite") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "rightShoulderDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = shoulderList, Selected = rightShoulderSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "rightShoulderIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightShoulderLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule droite - Cuir 1", DrawList = new() { new NuiDrawListImage(rightShoulderLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShoulderCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule droite - Tissu 1", DrawList = new() { new NuiDrawListImage(rightShoulderCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShoulderMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule droite - Métal 1", DrawList = new() { new NuiDrawListImage(rightShoulderMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightShoulderLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule droite - Cuir 2", DrawList = new() { new NuiDrawListImage(rightShoulderLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShoulderCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule droite - Tissu 2", DrawList = new() { new NuiDrawListImage(rightShoulderCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShoulderMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Epaule droite - Métal 2", DrawList = new() { new NuiDrawListImage(rightShoulderMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Biceps gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "leftBicepDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = bicepList, Selected = leftBicepSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "leftBicepIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftBicepLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps gauche - Cuir 1", DrawList = new() { new NuiDrawListImage(leftBicepLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftBicepCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps gauche - Tissu 1", DrawList = new() { new NuiDrawListImage(leftBicepCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftBicepMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps gauche - Métal 1", DrawList = new() { new NuiDrawListImage(leftBicepMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftBicepLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps gauche - Cuir 2", DrawList = new() { new NuiDrawListImage(leftBicepLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftBicepCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps gauche - Tissu 2", DrawList = new() { new NuiDrawListImage(leftBicepCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftBicepMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps gauche - Métal 2", DrawList = new() { new NuiDrawListImage(leftBicepMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Torse") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "torsoDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = torsoList, Selected = torsoSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "torsoIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "torsoLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Torse - Cuir 1", DrawList = new() { new NuiDrawListImage(torsoLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "torsoCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Torse - Tissu 1", DrawList = new() { new NuiDrawListImage(torsoCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "torsoMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Torse - Métal 1", DrawList = new() { new NuiDrawListImage(torsoMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "torsoLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Torse - Cuir 2", DrawList = new() { new NuiDrawListImage(torsoLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "torsoCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Torse - Tissu 2", DrawList = new() { new NuiDrawListImage(torsoCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "torsoMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Torse - Métal 2", DrawList = new() { new NuiDrawListImage(torsoMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Biceps droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "rightBicepDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = bicepList, Selected = rightBicepSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "rightBicepIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightBicepLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps droit - Cuir 1", DrawList = new() { new NuiDrawListImage(rightBicepLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightBicepCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps droit - Tissu 1", DrawList = new() { new NuiDrawListImage(rightBicepCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightBicepMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps droit - Métal 1", DrawList = new() { new NuiDrawListImage(rightBicepMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightBicepLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps droit - Cuir 2", DrawList = new() { new NuiDrawListImage(rightBicepLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightBicepCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps droit - Tissu 2", DrawList = new() { new NuiDrawListImage(rightBicepCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightBicepMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Biceps droit - Métal 2", DrawList = new() { new NuiDrawListImage(rightBicepMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Avant-bras gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "leftForearmDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = forearmList, Selected = leftForearmSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "leftForearmIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftForearmLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras gauche - Cuir 1", DrawList = new() { new NuiDrawListImage(leftForearmLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftForearmCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras gauche - Tissu 1", DrawList = new() { new NuiDrawListImage(leftForearmCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftForearmMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras gauche - Métal 1", DrawList = new() { new NuiDrawListImage(leftForearmMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftForearmLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras gauche - Cuir 2", DrawList = new() { new NuiDrawListImage(leftForearmLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftForearmCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras gauche - Tissu 2", DrawList = new() { new NuiDrawListImage(leftForearmCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftForearmMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras gauche - Métal 2", DrawList = new() { new NuiDrawListImage(leftForearmMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Ceinture") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "beltDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = beltList, Selected = beltSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "beltIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "beltLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Ceinture - Cuir 1", DrawList = new() { new NuiDrawListImage(beltLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "beltCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Ceinture - Tissu 1", DrawList = new() { new NuiDrawListImage(beltCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "beltMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Ceinture - Métal 1", DrawList = new() { new NuiDrawListImage(beltMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "beltLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Ceinture - Cuir 2", DrawList = new() { new NuiDrawListImage(beltLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "beltCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Ceinture - Tissu 2", DrawList = new() { new NuiDrawListImage(beltCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "beltMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Ceinture - Métal 2", DrawList = new() { new NuiDrawListImage(beltMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Avant-bras droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "rightForearmDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries =forearmList, Selected = rightForearmSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "rightForearmIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightForearmLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras droit - Cuir 1", DrawList = new() { new NuiDrawListImage(rightForearmLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightForearmCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras droit - Tissu 1", DrawList = new() { new NuiDrawListImage(rightForearmCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightForearmMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras droit - Métal 1", DrawList = new() { new NuiDrawListImage(rightForearmMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightForearmLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras droit - Cuir 2", DrawList = new() { new NuiDrawListImage(rightForearmLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightForearmCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras droit - Tissu 2", DrawList = new() { new NuiDrawListImage(rightForearmCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightForearmMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Avant-bras droit - Métal 2", DrawList = new() { new NuiDrawListImage(rightForearmMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Main gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "leftHandDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = handList, Selected = leftHandSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "leftHandIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftHandLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main gauche - Cuir 1", DrawList = new() { new NuiDrawListImage(leftHandLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftHandCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main gauche - Tissu 1", DrawList = new() { new NuiDrawListImage(leftHandCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftHandMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main gauche - Métal 1", DrawList = new() { new NuiDrawListImage(leftHandMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftHandLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main gauche - Cuir 2", DrawList = new() { new NuiDrawListImage(leftHandLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftHandCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main gauche - Tissu 2", DrawList = new() { new NuiDrawListImage(leftHandCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftHandMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main gauche - Métal 2", DrawList = new() { new NuiDrawListImage(leftHandMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Pelvis") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "pelvisDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = pelvisList, Selected = pelvisSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "pelvisIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "pelvisLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pelvis - Cuir 1", DrawList = new() { new NuiDrawListImage(pelvisLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "pelvisCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pelvis - Tissu 1", DrawList = new() { new NuiDrawListImage(pelvisCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "pelvisMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pelvis - Métal 1", DrawList = new() { new NuiDrawListImage(pelvisMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "pelvisLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pelvis - Cuir 2", DrawList = new() { new NuiDrawListImage(pelvisLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "pelvisCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pelvis - Tissu 2", DrawList = new() { new NuiDrawListImage(pelvisCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "pelvisMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pelvis - Métal 2", DrawList = new() { new NuiDrawListImage(pelvisMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Main droite") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "rightHandDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = handList, Selected = rightHandSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "rightHandIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightHandLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main droite - Cuir 1", DrawList = new() { new NuiDrawListImage(rightHandLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightHandCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main droite - Tissu 1", DrawList = new() { new NuiDrawListImage(rightHandCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightHandMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main droite - Métal 1", DrawList = new() { new NuiDrawListImage(rightHandMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightHandLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main droite - Cuir 2", DrawList = new() { new NuiDrawListImage(rightHandLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightHandCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main droite - Tissu 2", DrawList = new() { new NuiDrawListImage(rightHandCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightHandMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Main droite - Métal 2", DrawList = new() { new NuiDrawListImage(rightHandMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Cuisse gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "leftThighDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = thighList, Selected = leftThighSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "leftThighIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftThighLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse gauche - Cuir 1", DrawList = new() { new NuiDrawListImage(leftThighLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftThighCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse gauche - Tissu 1", DrawList = new() { new NuiDrawListImage(leftThighCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftThighMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse gauche - Métal 1", DrawList = new() { new NuiDrawListImage(leftThighMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftThighLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse gauche - Cuir 2", DrawList = new() { new NuiDrawListImage(leftThighLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftThighCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse gauche - Tissu 2", DrawList = new() { new NuiDrawListImage(leftThighCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftThighMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse gauche - Métal 2", DrawList = new() { new NuiDrawListImage(leftThighMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Robe") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "robeDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = robeList, Selected = robeSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "robeIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "robeLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Robe - Cuir 1", DrawList = new() { new NuiDrawListImage(robeLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "robeCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Robe - Tissu 1", DrawList = new() { new NuiDrawListImage(robeCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "robeMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Robe - Métal 1", DrawList = new() { new NuiDrawListImage(robeMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "robeLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Robe - Cuir 2", DrawList = new() { new NuiDrawListImage(robeLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "robeCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Robe - Tissu 2", DrawList = new() { new NuiDrawListImage(robeCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "robeMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Robe - Métal 2", DrawList = new() { new NuiDrawListImage(robeMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Cuisse droite") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "rightThighDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = thighList, Selected = rightThighSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "rightThighIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightThighLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse droite - Cuir 1", DrawList = new() { new NuiDrawListImage(rightThighLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightThighCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse droite - Tissu 1", DrawList = new() { new NuiDrawListImage(rightThighCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightThighMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse droite - Métal 1", DrawList = new() { new NuiDrawListImage(rightThighMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightThighLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse droite - Cuir 2", DrawList = new() { new NuiDrawListImage(rightThighLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightThighCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse droite - Tissu 2", DrawList = new() { new NuiDrawListImage(rightThighCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightThighMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Cuisse droite - Métal 2", DrawList = new() { new NuiDrawListImage(rightThighMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Tibias gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "leftShinDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = shinList, Selected = leftShinSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "leftShinIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftShinLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias gauche - Cuir 1", DrawList = new() { new NuiDrawListImage(leftShinLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShinCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias gauche - Tissu 1", DrawList = new() { new NuiDrawListImage(leftShinCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShinMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias gauche - Métal 1", DrawList = new() { new NuiDrawListImage(leftShinMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftShinLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias gauche - Cuir 2", DrawList = new() { new NuiDrawListImage(leftShinLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShinCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias gauche - Tissu 2", DrawList = new() { new NuiDrawListImage(leftShinCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftShinMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias gauche - Métal 2", DrawList = new() { new NuiDrawListImage(leftShinMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Visible = false, Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Cou") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Tibias droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "rightShinDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = shinList, Selected = rightShinSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "rightShinIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightShinLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias droit - Cuir 1", DrawList = new() { new NuiDrawListImage(rightShinLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShinCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias droit - Tissu 1", DrawList = new() { new NuiDrawListImage(rightShinCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShinMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias droit - Métal 1", DrawList = new() { new NuiDrawListImage(rightShinMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightShinLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias droit - Cuir 2", DrawList = new() { new NuiDrawListImage(rightShinLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShinCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias droit - Tissu 2", DrawList = new() { new NuiDrawListImage(rightShinCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightShinMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Tibias droit - Métal 2", DrawList = new() { new NuiDrawListImage(rightShinMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } }
            }
          });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Pied gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "leftFootDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = footList, Selected = leftFootSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "leftFootIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftFootLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied gauche - Cuir 1", DrawList = new() { new NuiDrawListImage(leftFootLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftFootCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied gauche - Tissu 1", DrawList = new() { new NuiDrawListImage(leftFootCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftFootMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied gauche - Métal 1", DrawList = new() { new NuiDrawListImage(leftFootMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "leftFootLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied gauche - Cuir 2", DrawList = new() { new NuiDrawListImage(leftFootLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftFootCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied gauche - Tissu 2", DrawList = new() { new NuiDrawListImage(leftFootCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "leftFootMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied gauche - Métal 2", DrawList = new() { new NuiDrawListImage(leftFootMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Visible = false, Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Cou") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiButton("") { Width = 15, Height = 15, Margin = 0.0f },
                new NuiSpacer()
              } }
            } },
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiLabel("Pied droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiButton("<") { Id = "rightFootDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                new NuiCombo(){ Entries = footList, Selected = rightFootSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                new NuiButton(">") { Id = "rightFootIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f }
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightFootLeather1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied droit - Cuir 1", DrawList = new() { new NuiDrawListImage(rightFootLeather1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightFootCloth1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied droit - Tissu 1", DrawList = new() { new NuiDrawListImage(rightFootCloth1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightFootMetal1", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied droit - Métal 1", DrawList = new() { new NuiDrawListImage(rightFootMetal1, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiButton("") { Id = "rightFootLeather2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied droit - Cuir 2", DrawList = new() { new NuiDrawListImage(rightFootLeather2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightFootCloth2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied droit - Tissu 2", DrawList = new() { new NuiDrawListImage(rightFootCloth2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiButton("") { Id = "rightFootMetal2", Width = 15, Height = 15, Margin = 0.0f, Tooltip = "Pied droit - Métal 2", DrawList = new() { new NuiDrawListImage(rightFootMetal2, new NuiRect(4, 4, 9, 9)) { Aspect = NuiAspect.Stretch } } },
                new NuiSpacer()
              } }
            } },
          } });

          rootColumn = new NuiColumn { Children = rootChildren, Margin = 0.0f };

          CreateWindow(item);
        }
        public void CreateWindow(NwItem item)
        {
          this.item = item;
          selectedColorChannel = ItemAppearanceArmorColor.Leather1;
          selectedArmorPart = CreaturePart.Head;

          player.DisableItemAppearanceFeedbackMessages();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 8, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) / 3);

          window = new NuiWindow(rootColumn, $"Modification des couleurs de {item.Name}")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleItemColorsEvents;

            player.ActivateSpotLight(player.oid.ControlledCreature);

            robeList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? RobeParts2da.maleCombo : RobeParts2da.femaleCombo);
            bicepList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? BicepParts2da.maleCombo : BicepParts2da.femaleCombo);
            footList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? FootParts2da.maleCombo : FootParts2da.femaleCombo);
            forearmList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? ForearmParts2da.maleCombo : ForearmParts2da.femaleCombo);
            handList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? HandParts2da.maleCombo : HandParts2da.femaleCombo);
            thighList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? LegParts2da.maleCombo : LegParts2da.femaleCombo);
            shinList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? ShinParts2da.maleCombo : ShinParts2da.femaleCombo);
            shoulderList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? ShoulderParts2da.maleCombo : ShoulderParts2da.femaleCombo);
            pelvisList.SetBindValue(player.oid, nuiToken.Token, player.oid.ControlledCreature.Gender == Gender.Male ? PelvisParts2da.maleCombo : PelvisParts2da.femaleCombo);
            torsoList.SetBindValue(player.oid, nuiToken.Token, GetTorsoComboList());
            neckList.SetBindValue(player.oid, nuiToken.Token, NeckParts2da.combo);
            beltList.SetBindValue(player.oid, nuiToken.Token, BeltParts2da.combo);

            globalLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather1) + 1}");
            globalCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Cloth1) + 1}");
            globalMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Metal1) + 1));
            globalLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Leather2) + 1}");
            globalCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Cloth2) + 1}");
            globalMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorColor(ItemAppearanceArmorColor.Metal2) + 1));

            leftShoulderSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.LeftShoulder));
            rightShoulderSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.RightShoulder));
            neckSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.Neck));
            leftBicepSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.LeftBicep));
            rightBicepSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.RightBicep));
            torsoSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.Torso));
            leftForearmSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.LeftForearm));
            rightForearmSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.RightForearm));
            beltSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.Belt));
            leftHandSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.LeftHand));
            rightHandSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.RightHand));
            pelvisSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.Pelvis));
            leftThighSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.LeftThigh));
            rightThighSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.RightThigh));
            robeSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.Robe));
            leftShinSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.LeftShin));
            rightShinSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.RightShin));
            leftFootSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.LeftFoot));
            rightFootSelection.SetBindValue(player.oid, nuiToken.Token, item.Appearance.GetArmorModel(CreaturePart.RightFoot));

            leftShoulderSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightShoulderSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            neckSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftBicepSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightBicepSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            torsoSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftForearmSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightForearmSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            beltSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftHandSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightHandSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            pelvisSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftThighSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightThighSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            robeSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftShinSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightShinSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            leftFootSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            rightFootSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            leftShoulderLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShoulder, ItemAppearanceArmorColor.Leather1) + 1}");
            leftShoulderCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShoulder, ItemAppearanceArmorColor.Cloth1) + 1}");
            leftShoulderMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftShoulder, ItemAppearanceArmorColor.Metal1) + 1));
            leftShoulderLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShoulder, ItemAppearanceArmorColor.Leather2) + 1}");
            leftShoulderCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShoulder, ItemAppearanceArmorColor.Cloth2) + 1}");
            leftShoulderMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftShoulder, ItemAppearanceArmorColor.Metal2) + 1));

            rightShoulderLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShoulder, ItemAppearanceArmorColor.Leather1) + 1}");
            rightShoulderCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShoulder, ItemAppearanceArmorColor.Cloth1) + 1}");
            rightShoulderMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightShoulder, ItemAppearanceArmorColor.Metal1) + 1));
            rightShoulderLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShoulder, ItemAppearanceArmorColor.Leather2) + 1}");
            rightShoulderCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShoulder, ItemAppearanceArmorColor.Cloth2) + 1}");
            rightShoulderMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightShoulder, ItemAppearanceArmorColor.Metal2) + 1));

            neckLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Neck, ItemAppearanceArmorColor.Leather1) + 1}");
            neckCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Neck, ItemAppearanceArmorColor.Cloth1) + 1}");
            neckMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Neck, ItemAppearanceArmorColor.Metal1) + 1));
            neckLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Neck, ItemAppearanceArmorColor.Leather2) + 1}");
            neckCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Neck, ItemAppearanceArmorColor.Cloth2) + 1}");
            neckMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Neck, ItemAppearanceArmorColor.Metal2) + 1));

            leftBicepLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftBicep, ItemAppearanceArmorColor.Leather1) + 1}");
            leftBicepCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftBicep, ItemAppearanceArmorColor.Cloth1) + 1}");
            leftBicepMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftBicep, ItemAppearanceArmorColor.Metal1) + 1));
            leftBicepLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftBicep, ItemAppearanceArmorColor.Leather2) + 1}");
            leftBicepCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftBicep, ItemAppearanceArmorColor.Cloth2) + 1}");
            leftBicepMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftBicep, ItemAppearanceArmorColor.Metal2) + 1));

            rightBicepLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightBicep, ItemAppearanceArmorColor.Leather1) + 1}");
            rightBicepCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightBicep, ItemAppearanceArmorColor.Cloth1) + 1}");
            rightBicepMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightBicep, ItemAppearanceArmorColor.Metal1) + 1));
            rightBicepLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightBicep, ItemAppearanceArmorColor.Leather2) + 1}");
            rightBicepCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightBicep, ItemAppearanceArmorColor.Cloth2) + 1}");
            rightBicepMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightBicep, ItemAppearanceArmorColor.Metal2) + 1));

            torsoLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Torso, ItemAppearanceArmorColor.Leather1) + 1}");
            torsoCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Torso, ItemAppearanceArmorColor.Cloth1) + 1}");
            torsoMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Torso, ItemAppearanceArmorColor.Metal1) + 1));
            torsoLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Torso, ItemAppearanceArmorColor.Leather2) + 1}");
            torsoCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Torso, ItemAppearanceArmorColor.Cloth2) + 1}");
            torsoMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Torso, ItemAppearanceArmorColor.Metal2) + 1));

            leftForearmLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftForearm, ItemAppearanceArmorColor.Leather1) + 1}");
            leftForearmCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftForearm, ItemAppearanceArmorColor.Cloth1) + 1}");
            leftForearmMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftForearm, ItemAppearanceArmorColor.Metal1) + 1));
            leftForearmLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftForearm, ItemAppearanceArmorColor.Leather2) + 1}");
            leftForearmCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftForearm, ItemAppearanceArmorColor.Cloth2) + 1}");
            leftForearmMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftForearm, ItemAppearanceArmorColor.Metal2) + 1));

            rightForearmLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightForearm, ItemAppearanceArmorColor.Leather1) + 1}");
            rightForearmCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightForearm, ItemAppearanceArmorColor.Cloth1) + 1}");
            rightForearmMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightForearm, ItemAppearanceArmorColor.Metal1) + 1));
            rightForearmLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightForearm, ItemAppearanceArmorColor.Leather2) + 1}");
            rightForearmCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightForearm, ItemAppearanceArmorColor.Cloth2) + 1}");
            rightForearmMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightForearm, ItemAppearanceArmorColor.Metal2) + 1));

            beltLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Belt, ItemAppearanceArmorColor.Leather1) + 1}");
            beltCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Belt, ItemAppearanceArmorColor.Cloth1) + 1}");
            beltMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Belt, ItemAppearanceArmorColor.Metal1) + 1));
            beltLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Belt, ItemAppearanceArmorColor.Leather2) + 1}");
            beltCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Belt, ItemAppearanceArmorColor.Cloth2) + 1}");
            beltMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Belt, ItemAppearanceArmorColor.Metal2) + 1));

            leftHandLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftHand, ItemAppearanceArmorColor.Leather1) + 1}");
            leftHandCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftHand, ItemAppearanceArmorColor.Cloth1) + 1}");
            leftHandMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftHand, ItemAppearanceArmorColor.Metal1) + 1));
            leftHandLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftHand, ItemAppearanceArmorColor.Leather2) + 1}");
            leftHandCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftHand, ItemAppearanceArmorColor.Cloth2) + 1}");
            leftHandMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftHand, ItemAppearanceArmorColor.Metal2) + 1));

            rightHandLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightHand, ItemAppearanceArmorColor.Leather1) + 1}");
            rightHandCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightHand, ItemAppearanceArmorColor.Cloth1) + 1}");
            rightHandMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightHand, ItemAppearanceArmorColor.Metal1) + 1));
            rightHandLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightHand, ItemAppearanceArmorColor.Leather2) + 1}");
            rightHandCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightHand, ItemAppearanceArmorColor.Cloth2) + 1}");
            rightHandMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightHand, ItemAppearanceArmorColor.Metal2) + 1));

            pelvisLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Pelvis, ItemAppearanceArmorColor.Leather1) + 1}");
            pelvisCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Pelvis, ItemAppearanceArmorColor.Cloth1) + 1}");
            pelvisMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Pelvis, ItemAppearanceArmorColor.Metal1) + 1));
            pelvisLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Pelvis, ItemAppearanceArmorColor.Leather2) + 1}");
            pelvisCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Pelvis, ItemAppearanceArmorColor.Cloth2) + 1}");
            pelvisMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Pelvis, ItemAppearanceArmorColor.Metal2) + 1));

            leftThighLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftThigh, ItemAppearanceArmorColor.Leather1) + 1}");
            leftThighCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftThigh, ItemAppearanceArmorColor.Cloth1) + 1}");
            leftThighMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftThigh, ItemAppearanceArmorColor.Metal1) + 1));
            leftThighLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftThigh, ItemAppearanceArmorColor.Leather2) + 1}");
            leftThighCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftThigh, ItemAppearanceArmorColor.Cloth2) + 1}");
            leftThighMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftThigh, ItemAppearanceArmorColor.Metal2) + 1));

            rightThighLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightThigh, ItemAppearanceArmorColor.Leather1) + 1}");
            rightThighCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightThigh, ItemAppearanceArmorColor.Cloth1) + 1}");
            rightThighMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightThigh, ItemAppearanceArmorColor.Metal1) + 1));
            rightThighLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightThigh, ItemAppearanceArmorColor.Leather2) + 1}");
            rightThighCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightThigh, ItemAppearanceArmorColor.Cloth2) + 1}");
            rightThighMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightThigh, ItemAppearanceArmorColor.Metal2) + 1));

            robeLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Robe, ItemAppearanceArmorColor.Leather1) + 1}");
            robeCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Robe, ItemAppearanceArmorColor.Cloth1) + 1}");
            robeMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Robe, ItemAppearanceArmorColor.Metal1) + 1));
            robeLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Robe, ItemAppearanceArmorColor.Leather2) + 1}");
            robeCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.Robe, ItemAppearanceArmorColor.Cloth2) + 1}");
            robeMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.Robe, ItemAppearanceArmorColor.Metal2) + 1));

            leftShinLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShin, ItemAppearanceArmorColor.Leather1) + 1}");
            leftShinCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShin, ItemAppearanceArmorColor.Cloth1) + 1}");
            leftShinMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftShin, ItemAppearanceArmorColor.Metal1) + 1));
            leftShinLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShin, ItemAppearanceArmorColor.Leather2) + 1}");
            leftShinCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftShin, ItemAppearanceArmorColor.Cloth2) + 1}");
            leftShinMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftShin, ItemAppearanceArmorColor.Metal2) + 1));

            rightShinLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShin, ItemAppearanceArmorColor.Leather1) + 1}");
            rightShinCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShin, ItemAppearanceArmorColor.Cloth1) + 1}");
            rightShinMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightShin, ItemAppearanceArmorColor.Metal1) + 1));
            rightShinLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShin, ItemAppearanceArmorColor.Leather2) + 1}");
            rightShinCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightShin, ItemAppearanceArmorColor.Cloth2) + 1}");
            rightShinMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightShin, ItemAppearanceArmorColor.Metal2) + 1));

            leftFootLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftFoot, ItemAppearanceArmorColor.Leather1) + 1}");
            leftFootCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftFoot, ItemAppearanceArmorColor.Cloth1) + 1}");
            leftFootMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftFoot, ItemAppearanceArmorColor.Metal1) + 1));
            leftFootLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftFoot, ItemAppearanceArmorColor.Leather2) + 1}");
            leftFootCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.LeftFoot, ItemAppearanceArmorColor.Cloth2) + 1}");
            leftFootMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.LeftFoot, ItemAppearanceArmorColor.Metal2) + 1));

            rightFootLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightFoot, ItemAppearanceArmorColor.Leather1) + 1}");
            rightFootCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightFoot, ItemAppearanceArmorColor.Cloth1) + 1}");
            rightFootMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightFoot, ItemAppearanceArmorColor.Metal1) + 1));
            rightFootLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightFoot, ItemAppearanceArmorColor.Leather2) + 1}");
            rightFootCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{item.Appearance.GetArmorPieceColor(CreaturePart.RightFoot, ItemAppearanceArmorColor.Cloth2) + 1}");
            rightFootMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(item.Appearance.GetArmorPieceColor(CreaturePart.RightFoot, ItemAppearanceArmorColor.Metal2) + 1));

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            for (int i = 0; i < 256; i++)
              Utils.paletteColorBindings[i].SetBindValue(player.oid, nuiToken.Token, $"leather{i + 1}");
          }   
        }
        private void HandleItemColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.EventType == NuiEventType.Close)
          {
            player.RemoveSpotLight(player.oid.ControlledCreature);
            player.EnableItemAppearanceFeedbackMessages();
            return;
          }

          if (item == null || !item.IsValid || (item.RootPossessor != player.oid.ControlledCreature && !player.IsDm()))
          {
            player.oid.SendServerMessage("L'objet en cours de modification n'est plus en votre possession !", ColorConstants.Red);
            player.EnableItemAppearanceFeedbackMessages();
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch(nuiEvent.ElementId)
              {
                case "leather1":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = globalLeather1;
                  return;

                case "cloth1":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = globalCloth1;
                  return;

                case "metal1":
                  HandlePaletteSwap("metal");
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = globalMetal1;
                  return;

                case "leather2":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = globalLeather2;
                  return;

                case "cloth2":
                  HandlePaletteSwap();
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = globalCloth2;
                  return;

                case "metal2":
                  HandlePaletteSwap("metal");
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = globalMetal2;
                  return;

                case "leftShoulderDecrease":
                  HandleArmorSelectorChange(leftShoulderSelection, CreaturePart.LeftShoulder, player.oid.ControlledCreature.Gender == Gender.Male ? ShoulderParts2da.maleCombo : ShoulderParts2da.femaleCombo, -1);
                  return;

                case "leftShoulderIncrease":
                  HandleArmorSelectorChange(leftShoulderSelection, CreaturePart.LeftShoulder, player.oid.ControlledCreature.Gender == Gender.Male ? ShoulderParts2da.maleCombo : ShoulderParts2da.femaleCombo, 1);
                  return;

                case "neckDecrease":
                  HandleArmorSelectorChange(neckSelection, CreaturePart.Neck, NeckParts2da.combo, -1);
                  return;

                case "neckIncrease":
                  HandleArmorSelectorChange(neckSelection, CreaturePart.Neck, NeckParts2da.combo, 1);
                  return;

                case "rightShoulderDecrease":
                  HandleArmorSelectorChange(rightShoulderSelection, CreaturePart.RightShoulder, player.oid.ControlledCreature.Gender == Gender.Male ? ShoulderParts2da.maleCombo : ShoulderParts2da.femaleCombo, -1);
                  return;

                case "rightShoulderIncrease":
                  HandleArmorSelectorChange(rightShoulderSelection, CreaturePart.RightShoulder, player.oid.ControlledCreature.Gender == Gender.Male ? ShoulderParts2da.maleCombo : ShoulderParts2da.femaleCombo, 1);
                  return;

                case "leftBicepDecrease":
                  HandleArmorSelectorChange(leftBicepSelection, CreaturePart.LeftBicep, player.oid.ControlledCreature.Gender == Gender.Male ? BicepParts2da.maleCombo : BicepParts2da.femaleCombo, -1);
                  return;

                case "leftBicepIncrease":
                  HandleArmorSelectorChange(leftBicepSelection, CreaturePart.LeftBicep, player.oid.ControlledCreature.Gender == Gender.Male ? BicepParts2da.maleCombo : BicepParts2da.femaleCombo, 1);
                  return;

                case "torsoDecrease":
                  HandleArmorSelectorChange(torsoSelection, CreaturePart.Torso, GetTorsoComboList(), -1);
                  return;

                case "torsoIncrease":
                  HandleArmorSelectorChange(torsoSelection, CreaturePart.Torso, GetTorsoComboList(), 1);
                  return;

                case "rightBicepDecrease":
                  HandleArmorSelectorChange(rightBicepSelection, CreaturePart.RightBicep, player.oid.ControlledCreature.Gender == Gender.Male ? BicepParts2da.maleCombo : BicepParts2da.femaleCombo, -1);
                  return;

                case "rightBicepIncrease":
                  HandleArmorSelectorChange(rightBicepSelection, CreaturePart.RightBicep, player.oid.ControlledCreature.Gender == Gender.Male ? BicepParts2da.maleCombo : BicepParts2da.femaleCombo, 1);
                  return;

                case "leftForearmDecrease":
                  HandleArmorSelectorChange(leftForearmSelection, CreaturePart.LeftForearm, player.oid.ControlledCreature.Gender == Gender.Male ? ForearmParts2da.maleCombo : ForearmParts2da.femaleCombo, -1);
                  return;

                case "leftForearmIncrease":
                  HandleArmorSelectorChange(leftForearmSelection, CreaturePart.LeftForearm, player.oid.ControlledCreature.Gender == Gender.Male ? ForearmParts2da.maleCombo : ForearmParts2da.femaleCombo, 1);
                  return;

                case "beltDecrease":
                  HandleArmorSelectorChange(beltSelection, CreaturePart.Belt, BeltParts2da.combo, -1);
                  return;

                case "beltIncrease":
                  HandleArmorSelectorChange(beltSelection, CreaturePart.Belt, BeltParts2da.combo, 1);
                  return;

                case "rightForearmDecrease":
                  HandleArmorSelectorChange(rightForearmSelection, CreaturePart.RightForearm, player.oid.ControlledCreature.Gender == Gender.Male ? ForearmParts2da.maleCombo : ForearmParts2da.femaleCombo, -1);
                  return;

                case "rightForearmIncrease":
                  HandleArmorSelectorChange(rightForearmSelection, CreaturePart.RightForearm, player.oid.ControlledCreature.Gender == Gender.Male ? ForearmParts2da.maleCombo : ForearmParts2da.femaleCombo, 1);
                  return;

                case "leftHandDecrease":
                  HandleArmorSelectorChange(leftHandSelection, CreaturePart.LeftHand, player.oid.ControlledCreature.Gender == Gender.Male ? HandParts2da.maleCombo : HandParts2da.femaleCombo, -1);
                  return;

                case "leftHandIncrease":
                  HandleArmorSelectorChange(leftHandSelection, CreaturePart.LeftHand, player.oid.ControlledCreature.Gender == Gender.Male ? HandParts2da.maleCombo : HandParts2da.femaleCombo, 1);
                  return;

                case "pelvisDecrease":
                  HandleArmorSelectorChange(pelvisSelection, CreaturePart.Pelvis, player.oid.ControlledCreature.Gender == Gender.Male ? PelvisParts2da.maleCombo : PelvisParts2da.femaleCombo, -1);
                  return;

                case "pelvisIncrease":
                  HandleArmorSelectorChange(pelvisSelection, CreaturePart.Pelvis, player.oid.ControlledCreature.Gender == Gender.Male ? HandParts2da.maleCombo : HandParts2da.femaleCombo, 1);
                  return;

                case "rightHandDecrease":
                  HandleArmorSelectorChange(rightHandSelection, CreaturePart.RightHand, player.oid.ControlledCreature.Gender == Gender.Male ? HandParts2da.maleCombo : HandParts2da.femaleCombo, -1);
                  return;

                case "rightHandIncrease":
                  HandleArmorSelectorChange(rightHandSelection, CreaturePart.RightHand, player.oid.ControlledCreature.Gender == Gender.Male ? HandParts2da.maleCombo : HandParts2da.femaleCombo, 1);
                  return;

                case "leftThighDecrease":
                  HandleArmorSelectorChange(leftThighSelection, CreaturePart.LeftThigh, player.oid.ControlledCreature.Gender == Gender.Male ? LegParts2da.maleCombo : LegParts2da.femaleCombo, -1);
                  return;

                case "leftThighIncrease":
                  HandleArmorSelectorChange(leftThighSelection, CreaturePart.LeftThigh, player.oid.ControlledCreature.Gender == Gender.Male ? LegParts2da.maleCombo : LegParts2da.femaleCombo, 1);
                  return;

                case "robeDecrease":
                  HandleArmorSelectorChange(robeSelection, CreaturePart.Robe, player.oid.ControlledCreature.Gender == Gender.Male ? RobeParts2da.maleCombo : RobeParts2da.femaleCombo, -1);
                  return;

                case "robeIncrease":
                  HandleArmorSelectorChange(robeSelection, CreaturePart.Robe, player.oid.ControlledCreature.Gender == Gender.Male ? RobeParts2da.maleCombo : RobeParts2da.femaleCombo, 1);
                  return;

                case "rightThighDecrease":
                  HandleArmorSelectorChange(rightThighSelection, CreaturePart.RightThigh, player.oid.ControlledCreature.Gender == Gender.Male ? LegParts2da.maleCombo : LegParts2da.femaleCombo, -1);
                  return;

                case "rightThighIncrease":
                  HandleArmorSelectorChange(rightThighSelection, CreaturePart.RightThigh, player.oid.ControlledCreature.Gender == Gender.Male ? LegParts2da.maleCombo : LegParts2da.femaleCombo, 1);
                  return;

                case "leftShinDecrease":
                  HandleArmorSelectorChange(leftShinSelection, CreaturePart.LeftShin, player.oid.ControlledCreature.Gender == Gender.Male ? ShinParts2da.maleCombo : ShinParts2da.femaleCombo, -1);
                  return;

                case "leftShinIncrease":
                  HandleArmorSelectorChange(leftShinSelection, CreaturePart.LeftShin, player.oid.ControlledCreature.Gender == Gender.Male ? ShinParts2da.maleCombo : ShinParts2da.femaleCombo, 1);
                  return;

                case "rightShinDecrease":
                  HandleArmorSelectorChange(rightShinSelection, CreaturePart.RightShin, player.oid.ControlledCreature.Gender == Gender.Male ? ShinParts2da.maleCombo : ShinParts2da.femaleCombo, -1);
                  return;

                case "rightShinIncrease":
                  HandleArmorSelectorChange(rightShinSelection, CreaturePart.RightShin, player.oid.ControlledCreature.Gender == Gender.Male ? ShinParts2da.maleCombo : ShinParts2da.femaleCombo, 1);
                  return;

                case "leftFootDecrease":
                  HandleArmorSelectorChange(leftFootSelection, CreaturePart.LeftFoot, player.oid.ControlledCreature.Gender == Gender.Male ? FootParts2da.maleCombo : FootParts2da.femaleCombo, -1);
                  return;

                case "leftFootIncrease":
                  HandleArmorSelectorChange(leftFootSelection, CreaturePart.LeftFoot, player.oid.ControlledCreature.Gender == Gender.Male ? FootParts2da.maleCombo : FootParts2da.femaleCombo, 1);
                  return;

                case "rightFootDecrease":
                  HandleArmorSelectorChange(rightFootSelection, CreaturePart.RightFoot, player.oid.ControlledCreature.Gender == Gender.Male ? FootParts2da.maleCombo : FootParts2da.femaleCombo, -1);
                  return;

                case "rightFootIncrease":
                  HandleArmorSelectorChange(rightFootSelection, CreaturePart.RightFoot, player.oid.ControlledCreature.Gender == Gender.Male ? FootParts2da.maleCombo : FootParts2da.femaleCombo, 1);
                  return;

                case "leftShoulderLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = leftShoulderLeather1;
                  return;

                case "leftShoulderCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = leftShoulderCloth1;
                  return;

                case "leftShoulderMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = leftShoulderMetal1;
                  return;

                case "leftShoulderLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = leftShoulderLeather2;
                  return;

                case "leftShoulderCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = leftShoulderCloth2;
                  return;

                case "leftShoulderMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = leftShoulderMetal2;
                  return;

                case "neckLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Neck;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = neckLeather1;
                  return;

                case "neckCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Neck;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = neckCloth1;
                  return;

                case "neckMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Neck;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  return;

                case "neckLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Neck;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = neckLeather2;
                  return;

                case "neckCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Neck;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = neckCloth2;
                  return;

                case "neckMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Neck;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = neckMetal2;
                  return;

                case "rightShoulderLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = rightShoulderLeather1;
                  return;

                case "rightShoulderCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = rightShoulderCloth1;
                  return;

                case "rightShoulderMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = rightShoulderMetal1;
                  return;

                case "rightShoulderLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = rightShoulderLeather2;
                  return;

                case "rightShoulderCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = rightShoulderCloth2;
                  return;

                case "rightShoulderMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightShoulder;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = rightShoulderMetal2;
                  return;

                case "leftBicepLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = leftBicepLeather1;
                  return;

                case "leftBicepCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = leftBicepCloth1;
                  return;

                case "leftBicepMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = leftBicepMetal1;
                  return;

                case "leftBicepLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = leftBicepLeather2;
                  return;

                case "leftBicepCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = leftBicepCloth2;
                  return;

                case "leftBicepMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = leftBicepMetal2;
                  return;

                case "torsoLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Torso;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = torsoLeather1;
                  return;

                case "torsoCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Torso;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = torsoCloth1;
                  return;

                case "torsoMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Torso;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = torsoMetal1;
                  return;

                case "torsoLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Torso;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = torsoLeather2;
                  return;

                case "torsoCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Torso;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = torsoCloth2;
                  return;

                case "torsoMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Torso;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = torsoMetal2;
                  return;

                case "rightBicepLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = rightBicepLeather1;
                  return;

                case "rightBicepCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = rightBicepCloth1;
                  return;

                case "rightBicepMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = rightBicepMetal1;
                  return;

                case "rightBicepLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = rightBicepLeather2;
                  return;

                case "rightBicepCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = rightBicepCloth2;
                  return;

                case "rightBicepMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightBicep;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = rightBicepMetal2;
                  return;

                case "leftForearmLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = leftForearmLeather1;
                  return;

                case "leftForearmCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = leftForearmCloth1;
                  return;

                case "leftForearmMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = leftForearmMetal1;
                  return;

                case "leftForearmLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = leftForearmLeather2;
                  return;

                case "leftForearmCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = leftForearmCloth2;
                  return;

                case "leftForearmMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = leftForearmMetal2;
                  return;

                case "beltLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Belt;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = beltLeather1;
                  return;

                case "beltCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Belt;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = beltCloth1;
                  return;

                case "beltMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Belt;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = beltMetal1;
                  return;

                case "beltLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Belt;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = beltLeather2;
                  return;

                case "beltCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Belt;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = beltCloth2;
                  return;

                case "beltMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Belt;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = beltMetal2;
                  return;

                case "rightForearmLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = rightForearmLeather1;
                  return;

                case "rightForearmCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = rightForearmCloth1;
                  return;

                case "rightForearmMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = rightForearmMetal1;
                  return;

                case "rightForearmLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = rightForearmLeather2;
                  return;

                case "rightForearmCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = rightForearmCloth2;
                  return;

                case "rightForearmMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightForearm;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = rightForearmMetal2;
                  return;

                case "leftHandLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = leftHandLeather1;
                  return;

                case "leftHandCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = leftHandCloth1;
                  return;

                case "leftHandMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = leftHandMetal1;
                  return;

                case "leftHandLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = leftHandLeather2;
                  return;

                case "leftHandCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = leftHandCloth2;
                  return;

                case "leftHandMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = leftHandMetal2;
                  return;

                case "pelvisLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Pelvis;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = pelvisLeather1;
                  return;

                case "pelvisCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Pelvis;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = pelvisCloth1;
                  return;

                case "pelvisMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Pelvis;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = pelvisMetal1;
                  return;

                case "pelvisLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Pelvis;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = pelvisLeather2;
                  return;

                case "pelvisCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Pelvis;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = pelvisCloth2;
                  return;

                case "pelvisMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Pelvis;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = pelvisMetal2;
                  return;

                case "rightHandLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = rightHandLeather1;
                  return;

                case "rightHandCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = rightHandCloth1;
                  return;

                case "rightHandMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = rightHandMetal1;
                  return;

                case "rightHandLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = rightHandLeather2;
                  return;

                case "rightHandCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = rightHandCloth2;
                  return;

                case "rightHandMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightHand;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = rightHandMetal2;
                  return;

                case "leftThighLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = leftThighLeather1;
                  return;

                case "leftThighCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = leftThighCloth1;
                  return;

                case "leftThighMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = leftThighMetal1;
                  return;

                case "leftThighLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = leftThighLeather2;
                  return;

                case "leftThighCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = leftThighCloth2;
                  return;

                case "leftThighMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = leftThighMetal2;
                  return;

                case "robeLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Robe;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = robeLeather1;
                  return;

                case "robeCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Robe;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = robeCloth1;
                  return;

                case "robeMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Robe;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = robeMetal1;
                  return;

                case "robeLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Robe;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = robeLeather2;
                  return;

                case "robeCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.Robe;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = robeCloth2;
                  return;

                case "robeMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.Robe;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = robeMetal2;
                  return;

                case "rightThighLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = rightThighLeather1;
                  return;

                case "rightThighCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = rightThighCloth1;
                  return;

                case "rightThighMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = rightThighMetal1;
                  return;

                case "rightThighLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = rightThighLeather2;
                  return;

                case "rightThighCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = rightThighCloth2;
                  return;

                case "rightThighMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightThigh;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = rightThighMetal2;
                  return;

                case "leftShinLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = leftShinLeather1;
                  return;

                case "leftShinCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = leftShinCloth1;
                  return;

                case "leftShinMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = leftShinMetal1;
                  return;

                case "leftShinLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = leftShinLeather2;
                  return;

                case "leftShinCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = leftShinCloth2;
                  return;

                case "leftShinMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = leftShinMetal2;
                  return;

                case "rightShinLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = rightShinLeather1;
                  return;

                case "rightShinCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = rightShinCloth1;
                  return;

                case "rightShinMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = rightShinMetal1;
                  return;

                case "rightShinLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = rightShinLeather2;
                  return;

                case "rightShinCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = rightShinCloth2;
                  return;

                case "rightShinMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightShin;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = rightShinMetal2;
                  return;

                case "leftFootLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = leftFootLeather1;
                  return;

                case "leftFootCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = leftFootCloth1;
                  return;

                case "leftFootMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = leftFootMetal1;
                  return;

                case "leftFootLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = leftFootLeather2;
                  return;

                case "leftFootCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.LeftFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = leftFootCloth2;
                  return;

                case "leftFootMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.LeftFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = leftFootMetal2;
                  return;

                case "rightFootLeather1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather1;
                  lastClickedColorButton = rightFootLeather1;
                  return;

                case "rightFootCloth1":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth1;
                  lastClickedColorButton = rightFootCloth1;
                  return;

                case "rightFootMetal1":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal1;
                  lastClickedColorButton = rightFootMetal1;
                  return;

                case "rightFootLeather2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Leather2;
                  lastClickedColorButton = rightFootLeather2;
                  return;

                case "rightFootCloth2":
                  HandlePaletteSwap();
                  selectedArmorPart = CreaturePart.RightFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Cloth2;
                  lastClickedColorButton = rightFootCloth2;
                  return;

                case "rightFootMetal2":
                  HandlePaletteSwap("metal");
                  selectedArmorPart = CreaturePart.RightFoot;
                  selectedColorChannel = ItemAppearanceArmorColor.Metal2;
                  lastClickedColorButton = rightFootMetal2;
                  return;

                case "loadItemNameEditor":
                  if (!player.windows.ContainsKey("editorItemName")) player.windows.Add("editorItemName", new EditorItemName(player, item));
                  else ((EditorItemName)player.windows["editorItemName"]).CreateWindow(item);
                  return;
              }

              int resRef = int.Parse(nuiEvent.ElementId) + 1;

              if (selectedArmorPart == CreaturePart.Head) // ici, Head = Global plutôt qu'une partie de l'armure
              {
                item.Appearance.SetArmorColor(selectedColorChannel, byte.Parse(nuiEvent.ElementId));

                /*switch (selectedColorChannel)
                {
                  case ItemAppearanceArmorColor.Leather1:
                    globalLeather1.SetBindValue(player.oid, nuiToken.Token, $"leather{resRef}");
                    break;

                  case ItemAppearanceArmorColor.Cloth1:
                    globalCloth1.SetBindValue(player.oid, nuiToken.Token, $"leather{resRef}");
                    break;

                  case ItemAppearanceArmorColor.Metal1:
                    globalMetal1.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(resRef));
                    break;

                  case ItemAppearanceArmorColor.Leather2:
                    globalLeather2.SetBindValue(player.oid, nuiToken.Token, $"leather{resRef}");
                    break;

                  case ItemAppearanceArmorColor.Cloth2:
                    globalCloth2.SetBindValue(player.oid, nuiToken.Token, $"leather{resRef}");
                    break;

                  case ItemAppearanceArmorColor.Metal2:
                    globalMetal2.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(resRef));
                    break;
                }*/
              }
              else
                item.Appearance.SetArmorPieceColor(selectedArmorPart, selectedColorChannel, byte.Parse(nuiEvent.ElementId));

              if (lastClickedColorButton.Key.Contains("Metal"))
                lastClickedColorButton.SetBindValue(player.oid, nuiToken.Token, StringUtils.GetMetalPaletteResRef(resRef));
              else
                lastClickedColorButton.SetBindValue(player.oid, nuiToken.Token, $"leather{resRef}");

              NwItem newItem = item.Clone(nuiEvent.Player.ControlledCreature);
              nuiEvent.Player.ControlledCreature.RunEquip(newItem, InventorySlot.Chest);
              item.Destroy();
              item = newItem;

              break;

            case NuiEventType.Watch:

              switch(nuiEvent.ElementId)
              {
                case "leftShoulderSelection":
                  item.Appearance.SetArmorModel(CreaturePart.LeftShoulder, (byte)leftShoulderSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "rightShoulderSelection":
                  item.Appearance.SetArmorModel(CreaturePart.RightShoulder, (byte)rightShoulderSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "neckSelection":
                  item.Appearance.SetArmorModel(CreaturePart.Neck, (byte)neckSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "leftBicepSelection":
                  item.Appearance.SetArmorModel(CreaturePart.LeftBicep, (byte)leftBicepSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "rightBicepSelection":
                  item.Appearance.SetArmorModel(CreaturePart.RightBicep, (byte)rightBicepSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "torsoSelection":
                  item.Appearance.SetArmorModel(CreaturePart.Torso, (byte)torsoSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "leftForearmSelection":
                  item.Appearance.SetArmorModel(CreaturePart.LeftForearm, (byte)leftForearmSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "rightForearmSelection":
                  item.Appearance.SetArmorModel(CreaturePart.RightForearm, (byte)rightForearmSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "beltSelection":
                  item.Appearance.SetArmorModel(CreaturePart.Belt, (byte)beltSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "leftHandSelection":
                  item.Appearance.SetArmorModel(CreaturePart.LeftHand, (byte)leftHandSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "rightHandSelection":
                  item.Appearance.SetArmorModel(CreaturePart.RightHand, (byte)rightHandSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "pelvisSelection":
                  item.Appearance.SetArmorModel(CreaturePart.Pelvis, (byte)pelvisSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "leftThighSelection":
                  item.Appearance.SetArmorModel(CreaturePart.LeftThigh, (byte)leftThighSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "rightThighSelection":
                  item.Appearance.SetArmorModel(CreaturePart.RightThigh, (byte)rightThighSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "robeSelection":
                  item.Appearance.SetArmorModel(CreaturePart.Robe, (byte)robeSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "leftShinSelection":
                  item.Appearance.SetArmorModel(CreaturePart.LeftShin, (byte)leftShinSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "rightShinSelection":
                  item.Appearance.SetArmorModel(CreaturePart.RightShin, (byte)rightShinSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "leftFootSelection":
                  item.Appearance.SetArmorModel(CreaturePart.LeftFoot, (byte)leftFootSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;

                case "rightFootSelection":
                  item.Appearance.SetArmorModel(CreaturePart.RightFoot, (byte)rightFootSelection.GetBindValue(player.oid, nuiToken.Token));
                  CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
                  break;
              }

              break;
          }
        }
        private void LoadPaletteBindings(string paletteType = "")
        {
          if(paletteType == "metal")
              for (int i = 0; i < 56; i++)
                Utils.paletteColorBindings[i].SetBindValue(player.oid, nuiToken.Token,$"metal{i + 1}");
          else
              for (int i = 0; i < 56; i++)
                Utils.paletteColorBindings[i].SetBindValue(player.oid, nuiToken.Token, $"leather{i + 1}");
        }
        private void HandleArmorSelectorChange(NuiBind<int> selector, CreaturePart part, List<NuiComboEntry> list, int change)
        {
          selector.SetBindWatch(player.oid, nuiToken.Token, false);

          int newValue = list.IndexOf(list.FirstOrDefault(p => p.Value == selector.GetBindValue(player.oid, nuiToken.Token))) + change;

          if (newValue >= list.Count)
            newValue = 0;

          if (newValue < 0)
            newValue = list.Count - 1;

          selector.SetBindValue(player.oid, nuiToken.Token, list[newValue].Value);
          item.Appearance.SetArmorModel(part, (byte)selector.GetBindValue(player.oid, nuiToken.Token));
          CreatureUtils.ForceSlotReEquip(player.oid.ControlledCreature, item);
          selector.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void HandlePaletteSwap(string type = "")
        {
          if (type == "metal")
          {
            if (selectedColorChannel != ItemAppearanceArmorColor.Metal1 && selectedColorChannel != ItemAppearanceArmorColor.Metal2)
              LoadPaletteBindings("metal");
          }
          else
          {
            if (selectedColorChannel == ItemAppearanceArmorColor.Metal1 || selectedColorChannel == ItemAppearanceArmorColor.Metal2)
              LoadPaletteBindings();
          }
        }
        private List<NuiComboEntry> GetTorsoComboList()
        {
          switch(item.BaseACValue)
          {
            default: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleClothCombo : TorsoParts2da.femaleClothCombo;
            case 1: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.malePaddedCombo : TorsoParts2da.femalePaddedCombo;
            case 2: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleLeatherCombo : TorsoParts2da.femaleLeatherCombo;
            case 3: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleHideCombo : TorsoParts2da.femaleHideCombo;
            case 4: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleChainCombo : TorsoParts2da.femaleChainCombo;
            case 5: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleBreastplateCombo : TorsoParts2da.femaleBreastplateCombo;
            case 6: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleBandedCombo : TorsoParts2da.femaleBandedCombo;
            case 7: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleHalfplateCombo : TorsoParts2da.femaleHalfplateCombo;
            case 8: return player.oid.ControlledCreature.Gender == Gender.Male ? TorsoParts2da.maleFullplateCombo : TorsoParts2da.femaleFullplateCombo;
          }
        }
      }
    }
  }
}
