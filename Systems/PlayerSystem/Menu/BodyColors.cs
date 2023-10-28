using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class BodyColorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> currentColor = new ("currentColor");
        private readonly NuiBind<int> channelSelection = new ("channelSelection");
        private readonly NuiBind<int> headSelection = new("headSelection");
        private readonly NuiBind<int> sizeSelection = new("sizeSelection");
        private readonly NuiBind<int> genderSelection = new("genderSelection");

        private readonly NuiBind<List<NuiComboEntry>> headList = new("headList");
        private List<NuiComboEntry> headPlaceholderList;

        private readonly NuiBind<string>[] colorBindings = new NuiBind<string>[176];

        private readonly List<NuiComboEntry> comboChannel = new()
        {
          new NuiComboEntry("Cheveux", 1),
          new NuiComboEntry("Peau", 0),
          new NuiComboEntry("Yeux / Tattoo 1", 3),
          new NuiComboEntry("Lèvres / Tattoo 2", 2),
        };

        private NwCreature targetCreature;

        public BodyColorWindow(Player player, NwCreature targetCreature) : base(player)
        {
          windowId = "bodyColorsModifier";

          for (int i = 0; i < 176; i++)
            colorBindings[i] = new ($"color{i}");

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Apparence") { Id = "beauty", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = 90 , Encouraged = player.oid.LoginCreature.GetObjectVariable < PersistentVariableInt >("_IN_CHARACTER_CREATION_CLASS").HasValue},
            new NuiButton("Stats") { Id = "stats", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").HasValue },
            new NuiSpacer()
          } });

          int nbButton = 0;

          NuiColumn colorColumn = new NuiColumn() { Children = new List<NuiElement>() { new NuiRow() { Children = new List<NuiElement>
          {
            new NuiSpacer(),
            new NuiLabel("Actuelle") { Width = 65, Height = 35, VerticalAlign = NuiVAlign.Middle },
            new NuiButtonImage(currentColor) { Margin = 10, Width = 25, Height = 25 },
            new NuiCombo
            {
              Id = "colorChannel", Width = 240, Height = 35,
              Entries = comboChannel,
              Selected = channelSelection
            },
            new NuiSpacer()
          } } } };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
          {
            colorColumn,
            new NuiColumn() { Children = new List<NuiElement>() 
            {
              new NuiRow() { Height = 25, Children = new List<NuiElement> 
              { 
                new NuiSpacer(),
                new NuiLabel("Genre") { Width = 50, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                new NuiCombo(){ Entries = new List<NuiComboEntry> { new NuiComboEntry("Féminin", 0), new NuiComboEntry("Masculin", 1) }, Selected = genderSelection, Width = 150, Margin = 0.0f },
                new NuiSpacer()
              } },
              new NuiRow() { Height = 25, Children = new List<NuiElement> { new NuiSpacer(), new NuiLabel("Taille") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }, new NuiSpacer() } },
              new NuiRow() { Height = 25, Margin = 0.0f, Children = new List<NuiElement>
              {
                new NuiSpacer(),
                new NuiButton("<") { Id = "sizeDecrease", Width = 35, Margin = 0.0f },
                new NuiCombo(){ Entries = Utils.sizeList, Selected = sizeSelection, Width = 150, Margin = 0.0f },
                new NuiButton(">") { Id = "sizeIncrease", Width = 35, Margin = 0.0f },
                new NuiSpacer()
              } },
              new NuiRow() { Height = 25, Children = new List<NuiElement> { new NuiSpacer(), new NuiLabel("Tête") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }, new NuiSpacer() } },
              new NuiRow() { Height = 25, Margin = 0.0f, Children = new List<NuiElement>
              {
                new NuiSpacer(),
                new NuiButton("<") { Id = "headDecrease", Width = 35, Margin = 0.0f },
                new NuiCombo(){ Entries = headList, Selected = headSelection, Width = 150, Margin = 0.0f },
                new NuiButton(">") { Id = "headIncrease", Width = 35, Margin = 0.0f },
                new NuiSpacer()
              } },
              new NuiSpacer()
            } }
          } });

          for (int i = 0; i < 11; i++)
          {
            NuiRow row = new NuiRow();
            List<NuiElement> rowChildren = new List<NuiElement>();

            for (int j = 0; j < 16; j++)
            {
              NuiButtonImage button = new NuiButtonImage(colorBindings[nbButton])
              {
                Id = $"{nbButton}",
                Width = 25,
                Height = 25
              };

              rowChildren.Add(button);
              nbButton++;
            }

            row.Children = rowChildren;
            colorColumn.Children.Add(row);
          }

          rootColumn = new NuiColumn { Children = rootChildren };

          CreateWindow(targetCreature);
        }
        public void CreateWindow(NwCreature targetCreature)
        {
          this.targetCreature = targetCreature;
          player.DisableItemAppearanceFeedbackMessages();

          if (IsOpen)
            CloseWindow();

          NuiRect savedRectangle = player.windowRectangles[windowId];
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId)
            ? new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.55f, player.guiScaledHeight * 0.6f)
            : new NuiRect(player.guiWidth * 0.25f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.55f, player.guiScaledHeight * 0.6f);

          window = new NuiWindow(rootColumn, "Vous contemplez votre reflet dans le miroir")
          {
            Geometry = geometry,
            Closable = true,
            Border = true,
            Transparent = true,
            Resizable = false
          };

          player.ActivateSpotLight(targetCreature);

          Task wait = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromMilliseconds(10));

            if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
            {
              nuiToken = tempToken;
              nuiToken.OnNuiEvent += HandleBodyColorsEvents;

              headPlaceholderList = ModuleSystem.headModels.FirstOrDefault(h => h.gender == targetCreature.Gender && h.appearanceRow == targetCreature.Appearance.RowIndex).heads;
              headList.SetBindValue(player.oid, nuiToken.Token, headPlaceholderList);

              headSelection.SetBindValue(player.oid, nuiToken.Token, targetCreature.GetCreatureBodyPart(CreaturePart.Head));
              sizeSelection.SetBindValue(player.oid, nuiToken.Token, (int)targetCreature.VisualTransform.Scale * 100 - 75);
              genderSelection.SetBindValue(player.oid, nuiToken.Token, 1);

              headSelection.SetBindWatch(player.oid, nuiToken.Token, true);
              sizeSelection.SetBindWatch(player.oid, nuiToken.Token, true);
              genderSelection.SetBindWatch(player.oid, nuiToken.Token, true);

              currentColor.SetBindValue(player.oid, nuiToken.Token, $"hair{targetCreature.GetColor(ColorChannel.Hair) + 1}");
              channelSelection.SetBindValue(player.oid, nuiToken.Token, 1);
              channelSelection.SetBindWatch(player.oid, nuiToken.Token, true);

              geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
              geometry.SetBindWatch(player.oid, nuiToken.Token, true);

              for (int i = 0; i < 176; i++)
                colorBindings[i].SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"hair{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"hair{i + 1}" : $"leather{i + 1}");
            }
            else
              player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
          });
        }
        private void HandleBodyColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
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
            player.RemoveSpotLight(targetCreature);
            player.EnableItemAppearanceFeedbackMessages();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "welcome":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyAppearanceModifier")) player.windows.Add("bodyAppearanceModifier", new IntroMirrorWindow(player));
                  else ((IntroMirrorWindow)player.windows["bodyAppearanceModifier"]).CreateWindow();

                  return;

                case "race":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introRaceSelector")) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)player.windows["introRaceSelector"]).CreateWindow();

                  return;

                case "histo":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introHistorySelector")) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)player.windows["introHistorySelector"]).CreateWindow();

                  return;

                case "class":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introClassSelector")) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)player.windows["introClassSelector"]).CreateWindow();

                  return;

                case "stats":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introLearnables")) player.windows.Add("introLearnables", new IntroLearnableWindow(player));
                  else ((IntroLearnableWindow)player.windows["introLearnables"]).CreateWindow();

                  return;

                case "sizeDecrease":
                  HandleSelectorChange(sizeSelection, Utils.sizeList, -1);

                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("x", ""), out float newSize))
                  {
                    targetCreature.VisualTransform.Scale = newSize;
                    targetCreature.GetObjectVariable<PersistentVariableFloat>("_ORIGINAL_SIZE").Value = newSize;
                  }

                  return;

                case "sizeIncrease":
                  HandleSelectorChange(sizeSelection, Utils.sizeList, 1);

                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("x", ""), out float newScale))
                  {
                    targetCreature.VisualTransform.Scale = newScale;
                    targetCreature.GetObjectVariable<PersistentVariableFloat>("_ORIGINAL_SIZE").Value = newScale;
                  }

                  return;

                case "headDecrease":
                  HandleSelectorChange(headSelection, headPlaceholderList, -1);
                  targetCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                  return;

                case "headIncrease":
                  HandleSelectorChange(headSelection, headPlaceholderList, 1);
                  targetCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                  return;
              }

              if (nuiEvent.ElementId is null)
                return;

              targetCreature.SetColor((ColorChannel)channelSelection.GetBindValue(player.oid, nuiToken.Token), int.Parse(nuiEvent.ElementId));

              string chanChoice = "hair";
              if (channelSelection.GetBindValue(player.oid, nuiToken.Token) != 1)
                chanChoice = "skin";

              currentColor.SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"{chanChoice}{int.Parse(nuiEvent.ElementId) + 1}", NWScript.RESTYPE_TGA) != "" ? $"{chanChoice}{int.Parse(nuiEvent.ElementId) + 1}" : $"leather{int.Parse(nuiEvent.ElementId) + 1}");

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "genderSelection": targetCreature.Gender = genderSelection.GetBindValue(player.oid, nuiToken.Token) > 0 ? Gender.Male: Gender.Female; return;
                case "headSelection": targetCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token)); return;

                case "sizeSelection":
                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("x", ""), out float newScale))
                  {
                    targetCreature.VisualTransform.Scale = newScale;
                    targetCreature.GetObjectVariable<PersistentVariableFloat>("_ORIGINAL_SIZE").Value = newScale;
                  }

                  return;

                case "channelSelection":

                  string channelChoice = "hair";
                  ColorChannel selectedChannel = (ColorChannel)channelSelection.GetBindValue(player.oid, nuiToken.Token);
                  if (selectedChannel != ColorChannel.Hair)
                    channelChoice = "skin";

                  for (int i = 0; i < 4; i++)
                    colorBindings[i].SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"{channelChoice}{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{i + 1}" : $"leather{i + 1}");

                  int newCurrentColor = targetCreature.GetColor(selectedChannel) + 1;
                  currentColor.SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"{channelChoice}{newCurrentColor}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{newCurrentColor}" : $"leather{newCurrentColor}");

                  return;
              }

              return;
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
