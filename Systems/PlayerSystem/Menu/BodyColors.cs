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
          new NuiComboEntry("Yeux", 3),
          new NuiComboEntry("Lèvres", 2),
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
            Transparent = false,
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

                  if (!player.windows.TryGetValue("introMirror", out var welcome)) player.windows.Add("introMirror", new IntroMirrorWindow(player));
                  else ((IntroMirrorWindow)welcome).CreateWindow();

                  return;

                case "race":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introRaceSelector", out var race)) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)race).CreateWindow();

                  return;

                case "histo":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introHistorySelector", out var histo)) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)histo).CreateWindow();

                  return;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introPortrait", out var portrait)) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)portrait).CreateWindow();

                  return;

                case "class":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introClassSelector", out var classe)) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)classe).CreateWindow();

                  return;

                case "stats":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introAbilities", out var stats)) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)stats).CreateWindow();

                  return;

              }

              if (nuiEvent.ElementId is null)
                return;

              int selectedColor = int.Parse(nuiEvent.ElementId);

              targetCreature.SetColor((ColorChannel)channelSelection.GetBindValue(player.oid, nuiToken.Token), selectedColor);

              string chanChoice = "hair";
              if (channelSelection.GetBindValue(player.oid, nuiToken.Token) != 1)
                chanChoice = "skin";

              if (chanChoice == "skin" && selectedColor == 1)
                currentColor.SetBindValue(player.oid, nuiToken.Token, "skintest");
              else
                currentColor.SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"{chanChoice}{selectedColor + 1}", NWScript.RESTYPE_TGA) != "" ? $"{chanChoice}{selectedColor + 1}" : $"leather{selectedColor + 1}");

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "channelSelection":

                  ColorChannel selectedChannel = (ColorChannel)channelSelection.GetBindValue(player.oid, nuiToken.Token);

                  string channelChoice = selectedChannel switch
                  {
                    ColorChannel.Hair => "hair",
                    ColorChannel.Skin => "skin",
                    _ => "leather",
                  };

                  for (int i = 0; i < 56; i++)
                  {
                    if(channelChoice == "skin" && i == 1)
                      colorBindings[i].SetBindValue(player.oid, nuiToken.Token, "skintest");
                    else
                      colorBindings[i].SetBindValue(player.oid, nuiToken.Token, NWScript.ResManGetAliasFor($"{channelChoice}{i + 1}", NWScript.RESTYPE_TGA) != "" ? $"{channelChoice}{i + 1}" : $"leather{i + 1}");
                  }

                  int newCurrentColor = targetCreature.GetColor(selectedChannel) + 1;

                  if (channelChoice == "skin" && newCurrentColor == 2)
                    currentColor.SetBindValue(player.oid, nuiToken.Token, "skintest");
                  else
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
