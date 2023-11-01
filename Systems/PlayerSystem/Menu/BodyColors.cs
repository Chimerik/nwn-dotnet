using System;
using System.Collections.Generic;
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
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = 70, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").HasValue },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = 70, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = 70 , Encouraged = player.oid.LoginCreature.GetObjectVariable < PersistentVariableInt >("_IN_CHARACTER_CREATION_CLASS").HasValue},
            new NuiButton("Stats") { Id = "stats", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").HasValue },
            new NuiSpacer()
          } });

          int nbButton = 0;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>
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
            rootChildren.Add(row);
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

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiWidth * 0.25f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.60f);

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

              currentColor.SetBindValue(player.oid, nuiToken.Token, $"hair{targetCreature.GetColor(ColorChannel.Hair) + 1}");
              channelSelection.SetBindValue(player.oid, nuiToken.Token, 1);
              channelSelection.SetBindWatch(player.oid, nuiToken.Token, true);

              geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.6f));
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

                case "portrait":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introPortrait")) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)player.windows["introPortrait"]).CreateWindow();

                  return;

                case "class":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introClassSelector")) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)player.windows["introClassSelector"]).CreateWindow();

                  return;

                case "stats":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introAbilities")) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)player.windows["introAbilities"]).CreateWindow();

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
      }
    }
  }
}
