﻿using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class IntroMirrorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> prenom = new("prenom");
        private readonly NuiBind<string> name = new("name");
        private readonly NuiBind<string> description = new("description");
        private readonly NuiBind<int> genderSelection = new("genderSelection"); 
        private readonly NuiBind<int> voiceSelection = new("voiceSelection");
        private readonly NuiBind<List<NuiComboEntry>> voiceList = new("voiceList");

        private readonly NuiBind<int> headSelection = new("headSelection");
        private readonly NuiBind<int> sizeSelection = new("sizeSelection");
        private readonly NuiBind<List<NuiComboEntry>> headList = new("headList");
        private List<NuiComboEntry> headPlaceholderList;

        public IntroMirrorWindow(Player player) : base(player)
        {
          windowId = "introMirror";
          windowWidth = player.guiScaledWidth * 0.4f;
          windowHeight = player.guiScaledHeight * 0.63f;
          rootColumn.Children = rootChildren;
          
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = windowWidth / 7.5f, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").HasValue },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").HasValue },
            new NuiButton("Stats") { Id = "stats", Height = 35, Width = windowWidth / 7.5f , Encouraged = player.oid.LoginCreature.GetObjectVariable < PersistentVariableInt >("_IN_CHARACTER_CREATION_STATS").HasValue},
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 140, Children = new List<NuiElement>() { new NuiText("HRP - Ce miroir vous permet de personnaliser votre personnage. Vous pourrez lui choisir une apparence, un historique, une classe et ses statistiques.\n\nLorsque vous aurez validé le tout, parlez au capitaine afin de passer à l'étape suivante.") { Scrollbars = NuiScrollbars.None } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() 
          { 
            new NuiSpacer(),
            new NuiTextEdit("Prénom", prenom, 28, false) { Width = windowWidth / 5.2f },
            new NuiTextEdit("Nom", name, 40, false) { Width = windowWidth / 5.2f },
            new NuiSpacer(),
            new NuiCombo(){ Entries = new List<NuiComboEntry> { new NuiComboEntry("Féminin", 1), new NuiComboEntry("Masculin", 0) }, Selected = genderSelection, Width = windowWidth / 4, Margin = 0.0f },
            new NuiCombo(){ Entries = voiceList, Selected = voiceSelection, Width = windowWidth / 4, Margin = 0.0f },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("<") { Id = "sizeDecrease", Width = 35, Height = 35, Margin = 0.0f },
            new NuiCombo(){ Entries = Utils.sizeList, Selected = sizeSelection, Width = 170, Margin = 0.0f },
            new NuiButton(">") { Id = "sizeIncrease", Width = 35, Height = 35, Margin = 0.0f },
            new NuiSpacer(),
            new NuiButton("<") { Id = "headDecrease", Width = 35, Height = 35, Margin = 0.0f },
            new NuiCombo(){ Entries = headList, Selected = headSelection, Width = 170, Margin = 0.0f },
            new NuiButton(">") { Id = "headIncrease", Width = 35, Height = 35, Margin = 0.0f },
            new NuiSpacer()
          } }); ;

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiTextEdit("Description", description, 5000, true) {} } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(5, player.guiHeight * 0.15f, windowWidth, windowHeight);

          window = new NuiWindow(rootColumn, "Votre reflet - Editeur de personnage")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleIntroMirrorEvents;

            prenom.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.OriginalFirstName);
            name.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.OriginalLastName);
            description.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.Description);

            voiceList.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.Gender == Gender.Male 
              ? SoundSet2da.playerMaleVoiceSet : SoundSet2da.playerFemaleVoiceSet);

            genderSelection.SetBindValue(player.oid, nuiToken.Token, (int)player.oid.LoginCreature.Gender);
            genderSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            voiceSelection.SetBindValue(player.oid, nuiToken.Token,  player.oid.LoginCreature.SoundSet);
            voiceSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            prenom.SetBindWatch(player.oid, nuiToken.Token, true);
            name.SetBindWatch(player.oid, nuiToken.Token, true);
            description.SetBindWatch(player.oid, nuiToken.Token, true);

            headPlaceholderList = ModuleSystem.headModels.FirstOrDefault(h => h.gender == player.oid.LoginCreature.Gender && h.appearanceRow == player.oid.LoginCreature.Appearance.RowIndex).heads;
            headList.SetBindValue(player.oid, nuiToken.Token, headPlaceholderList);

            headSelection.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.GetCreatureBodyPart(CreaturePart.Head));
            sizeSelection.SetBindValue(player.oid, nuiToken.Token, (int)player.oid.LoginCreature.VisualTransform.Scale * 100 - 75);

            headSelection.SetBindWatch(player.oid, nuiToken.Token, true);
            sizeSelection.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleIntroMirrorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "beauty":

                  CloseWindow();

                  if (!player.windows.TryGetValue("bodyColorsModifier", out var body)) player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, player.oid.LoginCreature));
                  else ((BodyColorWindow)body).CreateWindow(player.oid.LoginCreature);

                  break;

                case "class":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introClassSelector", out var classe)) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)classe).CreateWindow();

                  break;

                case "race":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introRaceSelector", out var race)) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)race).CreateWindow();

                  break;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introPortrait", out var portrait)) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)portrait).CreateWindow();

                  break;

                case "histo":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introHistorySelector", out var histo)) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)histo).CreateWindow();

                  break;

                case "stats":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introAbilities", out var stats)) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)stats).CreateWindow();

                  break;

                case "sizeDecrease":
                  HandleSelectorChange(sizeSelection, Utils.sizeList, -1);

                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("Taille : x", ""), out float newSize))
                  {
                    player.oid.LoginCreature.VisualTransform.Scale = newSize;
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value = newSize;
                  }

                  return;

                case "sizeIncrease":
                  HandleSelectorChange(sizeSelection, Utils.sizeList, 1);

                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("Taille : x", ""), out float newScale))
                  {
                    player.oid.LoginCreature.VisualTransform.Scale = newScale;
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value = newScale;
                  }

                  return;

                case "headDecrease":
                  HandleSelectorChange(headSelection, headPlaceholderList, -1);
                  player.oid.LoginCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                  return;

                case "headIncrease":
                  HandleSelectorChange(headSelection, headPlaceholderList, 1);
                  player.oid.LoginCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token));
                  return;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "genderSelection":

                  if (genderSelection.GetBindValue(player.oid, nuiToken.Token) > 0)
                  {
                    player.oid.LoginCreature.Gender = Gender.Female;
                    voiceList.SetBindValue(player.oid, nuiToken.Token, SoundSet2da.playerFemaleVoiceSet);
                  }
                  else
                  {
                    player.oid.LoginCreature.Gender = Gender.Male;
                    voiceList.SetBindValue(player.oid, nuiToken.Token, SoundSet2da.playerMaleVoiceSet);
                  }

                  break;

                case "prenom": 
                  
                  player.oid.LoginCreature.OriginalFirstName = prenom.GetBindValue(player.oid, nuiToken.Token);
                  player.oid.LoginCreature.Name = $"{player.oid.LoginCreature.OriginalFirstName} {player.oid.LoginCreature.OriginalLastName}";
                  player.oid.LoginCreature.Area.Name = $"La galère de {player.oid.LoginCreature.Name}";
                  break;

                case "name": 
                  
                  player.oid.LoginCreature.OriginalLastName = name.GetBindValue(player.oid, nuiToken.Token);
                  player.oid.LoginCreature.Name = $"{player.oid.LoginCreature.OriginalFirstName} {player.oid.LoginCreature.OriginalLastName}";
                  player.oid.LoginCreature.Area.Name = $"La galère de {player.oid.LoginCreature.Name}";
                  break;

                case "description": player.oid.LoginCreature.Description = description.GetBindValue(player.oid, nuiToken.Token); break;
                case "voiceSelection": player.oid.LoginCreature.SoundSet = (ushort)voiceSelection.GetBindValue(player.oid, nuiToken.Token); break;

                case "headSelection": player.oid.LoginCreature.SetCreatureBodyPart(CreaturePart.Head, headSelection.GetBindValue(player.oid, nuiToken.Token)); return;

                case "sizeSelection":
                  if (float.TryParse(Utils.sizeList[sizeSelection.GetBindValue(player.oid, nuiToken.Token)].Label.Replace("Taille : x", ""), out float newScale))
                  {
                    player.oid.LoginCreature.VisualTransform.Scale = newScale;
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableFloat>(CreatureUtils.OriginalSizeVariable).Value = newScale;
                  }

                  return;
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
