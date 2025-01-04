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
      public class BodyAppearanceWindow : PlayerWindow
      {
        private const float BUTTONSIZE = 15.0f;
        private const float COMBOSIZE = 150.0f;

        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<int> headSelection = new ("headSelection");
        private readonly NuiBind<int> sizeSelection = new ("sizeSelection");

        private readonly NuiBind<List<NuiComboEntry>> headList = new("headList");
        private List<NuiComboEntry> headPlaceholderList;

        private readonly NuiBind<int> chestSelection = new ("chestSelection");
        private readonly NuiBind<int> pelvisSelection = new("pelvisSelection");
        private readonly NuiBind<int> neckSelection = new("neckSelection");
        private readonly NuiBind<int> bicepRightSelection = new ("bicepRightSelection");
        private readonly NuiBind<int> forearmRightSelection = new ("forearmRightSelection");
        private readonly NuiBind<int> handRightSelection = new("handRightSelection");
        private readonly NuiBind<int> thighRightSelection = new ("thighRightSelection");
        private readonly NuiBind<int> shinRightSelection = new ("shinRightSelection");
        private readonly NuiBind<int> footRightSelection = new("footRightSelection");
        private readonly NuiBind<int> bicepLeftSelection = new ("bicepLeftSelection");
        private readonly NuiBind<int> forearmLeftSelection = new ("forearmLeftSelection");
        private readonly NuiBind<int> handLeftSelection = new("handLeftSelection");
        private readonly NuiBind<int> thighLeftSelection = new ("thighLeftSelection");
        private readonly NuiBind<int> shinLeftSelection = new ("shinLeftSelection");
        private readonly NuiBind<int> footLeftSelection = new("footLeftSelection");
        private readonly NuiBind<int> wingSelection = new("wingSelection");
        private readonly NuiBind<int> tailSelection = new("tailSelection");

        private readonly List<NuiComboEntry> comboBicep = new ()
          {
            new NuiComboEntry("1", 1),
            new NuiComboEntry("2", 2)
          };

        public NwCreature targetCreature;

        public BodyAppearanceWindow(Player player, NwCreature targetCreature) : base(player)
        {
          windowId = "bodyAppearanceModifier";
          
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow { Height = 40, Children = new List<NuiElement> { new NuiSpacer { }, new NuiButton("Couleurs") { Id = "openColors", Height = 35, Width = 70 }, new NuiSpacer { } } });

          rootChildren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiColumn() { Children = new List<NuiElement>()
              {
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Taille") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiButton("<") { Id = "sizeDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                  new NuiCombo(){ Entries = Utils.sizeList, Selected = sizeSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiButton(">") { Id = "sizeIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Tête") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f,Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiButton("<") { Id = "headDecrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                  new NuiCombo(){ Entries = headList, Selected = headSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiButton(">") { Id = "headIncrease", Height = BUTTONSIZE, Width = BUTTONSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Cou") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = neckSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Torse") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = chestSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Pelvis") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = pelvisSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Biceps Droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiLabel("Biceps Gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = bicepRightSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = bicepLeftSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Avant-bras Droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiLabel("Avant-bras Gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = forearmRightSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = forearmLeftSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Main Droite") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiLabel("Main Gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = handRightSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = handLeftSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Cuisse Droite") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiLabel("Cuisse Gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = thighRightSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = thighLeftSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Tibia Droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiLabel("Tibia Gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = shinRightSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = shinLeftSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Pied Droit") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiLabel("Pied Gauche") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = footRightSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = comboBicep, Selected = footLeftSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiLabel("Ailes") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer(),
                  new NuiLabel("Queue") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                  new NuiSpacer()
                } },
                new NuiRow() { Height = BUTTONSIZE, Margin = 0.0f, Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = WingModels2da.wingCombo, Selected = wingSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer(),
                  new NuiCombo(){ Entries = TailModels2da.tailCombo, Selected = tailSelection, Height = BUTTONSIZE, Width = COMBOSIZE, Margin = 0.0f },
                  new NuiSpacer()
                } },
              } }
            } 
          });

          CreateWindow(targetCreature);
        }
        public void CreateWindow(NwCreature targetCreature)
        {
          this.targetCreature = targetCreature;
          player.DisableItemAppearanceFeedbackMessages();
          CloseWindow();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(0, 0, 650, 450);

          window = new NuiWindow(rootColumn, $"{targetCreature.Name} - Modification corporelle")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true
          };

          player.ActivateSpotLight(targetCreature);

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            nuiToken.OnNuiEvent += HandleBodyAppearanceEvents;

            headPlaceholderList = ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads;
            headList.SetBindValue(player.oid, nuiToken.Token, headPlaceholderList);
            
            headSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.Head));
            sizeSelection.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.VisualTransform.Scale * 100 - 75);

            chestSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.Torso));
            bicepRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightBicep));
            bicepLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftBicep));
            forearmRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightForearm));
            forearmLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftForearm));
            thighRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightThigh));
            thighLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftThigh));
            shinRightSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.RightShin));
            shinLeftSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.LeftShin));
            wingSelection.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.WingType);
            tailSelection.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.TailType);

            headSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            sizeSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            neckSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            chestSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            pelvisSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            bicepRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            bicepLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            forearmRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            forearmLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            handRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            handLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            thighRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            thighLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            shinRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            shinLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            footRightSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            footLeftSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            wingSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            tailSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleBodyAppearanceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
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

          switch(nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "openColors":

                  if (!player.windows.TryGetValue("bodyColorsModifier", out var value)) player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, targetCreature));
                  else ((BodyColorWindow)value).CreateWindow(targetCreature);

                  return;

                case "sizeDecrease":
                  HandleSelectorChange(sizeSelection, Utils.sizeList, - 1);

                  if(float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("Taille : x", ""), out float newSize))
                    targetCreature.VisualTransform.Scale = newSize;
                  break;

                case "sizeIncrease":
                  HandleSelectorChange(sizeSelection, Utils.sizeList, 1);

                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("Taille : x", ""), out float newScale))
                    targetCreature.VisualTransform.Scale = newScale;
                  break;

                case "headDecrease":
                  HandleSelectorChange(headSelection, headPlaceholderList, -1);
                  targetCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "headIncrease":
                  HandleSelectorChange(headSelection, headPlaceholderList, 1);
                  targetCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "headSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "sizeSelection":
                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("Taille : x", ""), out float newScale))
                    targetCreature.VisualTransform.Scale = newScale;
                  break;

                case "neckSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.Neck, neckSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "chestSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.Torso, chestSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "pelvisSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.Pelvis, pelvisSelection.GetBindValue(player.oid, nuiToken.Token));
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

                case "handRightSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.RightHand, handRightSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "handLeftSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.LeftHand, handLeftSelection.GetBindValue(player.oid, nuiToken.Token));
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

                case "footRightSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.RightFoot, footRightSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "footLeftSelection":
                  targetCreature.SetCreatureBodyPart(CreaturePart.LeftFoot, footLeftSelection.GetBindValue(player.oid, nuiToken.Token));
                  break;

                case "wingSelection":
                  targetCreature.WingType = (CreatureWingType)wingSelection.GetBindValue(player.oid, nuiToken.Token);
                  break;

                case "tailSelection":
                  targetCreature.TailType = (CreatureTailType)tailSelection.GetBindValue(player.oid, nuiToken.Token);
                  break;
              }

              break;
          }
        }
        private void HandleSelectorChange(NuiBind<int> selector, List<NuiComboEntry> list, int change)
        {
          selector.SetBindWatch(player.oid, nuiToken.Token, false);

          int newValue = list.IndexOf(list.FirstOrDefault(p => p.Value == selector.GetBindValue(player.oid, nuiToken.Token))) + change;

          if (newValue >= list.Count)
            newValue = 0;

          if (newValue < 0)
            newValue = list.Count - 1;

          selector.SetBindValue(player.oid, nuiToken.Token, list[newValue].Value);
          selector.SetBindWatch(player.oid, nuiToken.Token, true);
        }
      }
    }
  }
}
