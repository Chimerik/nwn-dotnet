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
      public class IntroAbilitiesWindow : PlayerWindow
      {
        private readonly string[] icons = new string[] { "strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma" };
        private readonly string[] names = new string[] { "Force", "Dextérité", "Constitution", "Intelligence", "Sagesse", "Charisme" };

        private readonly bool[] abilities2 = new bool[] { false, false, false, false, false, false };
        private readonly bool[] abilities1 = new bool[] { false, false, false, false, false, false };

        private readonly int[] levels = new int[] { 0, 0, 0, 0, 0, 0};

        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> availableStats = new("availableStats");

        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> abilityName = new("abilityName");
        private readonly NuiBind<string> abilityLevel = new("abilityLevel");
        private readonly NuiBind<bool> bonus2Checked = new("bonus2Checked");
        private readonly NuiBind<bool> bonus1Checked = new("bonus1Checked");


        public IntroAbilitiesWindow(Player player) : base(player)
        {
          windowId = "introAbilities";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiSpacer()),
            new NuiListTemplateCell(new NuiButtonImage(icon)) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(abilityName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 120 },
            new NuiListTemplateCell(new NuiButton("<") { Id = "abilityDecrease" }) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(abilityLevel) { Margin = 0.0f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 40 },
            new NuiListTemplateCell(new NuiButton(">") { Id = "abilityIncrease" }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer()),
            new NuiListTemplateCell(new NuiCheck("", bonus2Checked) { Tooltip = "Attribuer le bonus de +2 à cette caractéristique", Margin = 0.0f }) { Width = 40 },
            new NuiListTemplateCell(new NuiCheck("", bonus1Checked) { Tooltip = "Attribuer le bonus de +1 à cette caractéristique", Margin = 0.0f }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = 70, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").HasValue },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").HasValue },
            new NuiButton("Stats") { Id = "stats", Height = 35, Width = 70 , Encouraged = player.oid.LoginCreature.GetObjectVariable < PersistentVariableInt >("_IN_CHARACTER_CREATION_STATS").HasValue},
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiLabel(availableStats) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, 6) { RowHeight = 40, Scrollbars = NuiScrollbars.None } } });
          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiScaledWidth * 0.3f, player.guiHeight * 0.25f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Votre reflet - Choisissez vos caractéristiques")
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

            LoadAbilityList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
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
                case "welcome":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyAppearanceModifier")) player.windows.Add("bodyAppearanceModifier", new IntroMirrorWindow(player));
                  else ((IntroMirrorWindow)player.windows["bodyAppearanceModifier"]).CreateWindow();

                  return;

                case "beauty":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyColorsModifier")) player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, player.oid.LoginCreature));
                  else ((BodyColorWindow)player.windows["bodyColorsModifier"]).CreateWindow(player.oid.LoginCreature);

                  break;

                case "class":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introClassSelector")) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)player.windows["introClassSelector"]).CreateWindow();

                  break;

                case "race":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introRaceSelector")) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)player.windows["introRaceSelector"]).CreateWindow();

                  break;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introPortrait")) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)player.windows["introPortrait"]).CreateWindow();

                  break;

                case "histo":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introHistorySelector")) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)player.windows["introHistorySelector"]).CreateWindow();

                  break;

                case "abilityDecrease":

                  int currentStatDecrease = levels[nuiEvent.ArrayIndex] - abilities1[nuiEvent.ArrayIndex].ToInt() - abilities2[nuiEvent.ArrayIndex].ToInt() * 2;
                  
                  if (currentStatDecrease < 9)
                    return;

                  int pointGained = currentStatDecrease > 13 ? 2 : 1;

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_REMAINING_ABILITY_POINTS").Value += pointGained;
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_CHARACTER_SET_ABILITY_{nuiEvent.ArrayIndex}").Value -= 1;
                  player.oid.LoginCreature.SetsRawAbilityScore((Ability)nuiEvent.ArrayIndex, (byte)(player.oid.LoginCreature.GetRawAbilityScore((Ability)nuiEvent.ArrayIndex) - 1));

                  LoadAbilityList();

                  break;

                case "abilityIncrease":

                  int currentStatIncrease = levels[nuiEvent.ArrayIndex] - abilities1[nuiEvent.ArrayIndex].ToInt() - abilities2[nuiEvent.ArrayIndex].ToInt() * 2;

                  if (currentStatIncrease > 14)
                    return;

                  int pointCost = currentStatIncrease > 12 ? 2 : 1;

                  if (pointCost > player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_REMAINING_ABILITY_POINTS").Value)
                    return;

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_REMAINING_ABILITY_POINTS").Value -= pointCost;
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_CHARACTER_SET_ABILITY_{nuiEvent.ArrayIndex}").Value += 1;
                  player.oid.LoginCreature.SetsRawAbilityScore((Ability)nuiEvent.ArrayIndex, (byte)(player.oid.LoginCreature.GetRawAbilityScore((Ability)nuiEvent.ArrayIndex) + 1));

                  LoadAbilityList();

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "bonus1Checked":

                  Ability secAbility;

                  if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_SECONDARY_BONUS").Value > -1)
                  {
                    secAbility = (Ability)player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_SECONDARY_BONUS").Value;
                    player.oid.LoginCreature.SetsRawAbilityScore(secAbility, (byte)(player.oid.LoginCreature.GetRawAbilityScore(secAbility) - 1));
                  }

                  secAbility = (Ability)nuiEvent.ArrayIndex;
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_SECONDARY_BONUS").Value = nuiEvent.ArrayIndex;
                  player.oid.LoginCreature.SetsRawAbilityScore(secAbility, (byte)(player.oid.LoginCreature.GetRawAbilityScore(secAbility) + 1));

                  if(bonus2Checked.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex])
                  {
                    Ability mAbility = (Ability)player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_MAIN_BONUS").Value;
                    player.oid.LoginCreature.SetsRawAbilityScore(mAbility, (byte)(player.oid.LoginCreature.GetRawAbilityScore(mAbility) - 2));
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_MAIN_BONUS").Value = -1;
                  }

                  LoadAbilityList();
                  break;

                case "bonus2Checked":

                  Ability mainAbility;

                  if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_MAIN_BONUS").Value > -1)
                  {
                    mainAbility = (Ability)player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_MAIN_BONUS").Value;
                    player.oid.LoginCreature.SetsRawAbilityScore(mainAbility, (byte)(player.oid.LoginCreature.GetRawAbilityScore(mainAbility) - 2));
                  }
                  
                  mainAbility = (Ability)nuiEvent.ArrayIndex;
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_MAIN_BONUS").Value = nuiEvent.ArrayIndex;
                  player.oid.LoginCreature.SetsRawAbilityScore(mainAbility, (byte)(player.oid.LoginCreature.GetRawAbilityScore(mainAbility) + 2));

                  if (bonus1Checked.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex])
                  {
                    Ability secondaryAbility = (Ability)player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_SECONDARY_BONUS").Value;
                    player.oid.LoginCreature.SetsRawAbilityScore(secondaryAbility, (byte)(player.oid.LoginCreature.GetRawAbilityScore(secondaryAbility) - 1));
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_SECONDARY_BONUS").Value = -1;
                  }

                  LoadAbilityList();
                  break;
              }

              break;

          }
        }
        private void LoadAbilityList()
        {
          bonus1Checked.SetBindWatch(player.oid, nuiToken.Token, false);
          bonus2Checked.SetBindWatch(player.oid, nuiToken.Token, false);

          int mainBonus = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_MAIN_BONUS").Value;
          int secondaryBonus = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_SECONDARY_BONUS").Value;

          availableStats.SetBindValue(player.oid, nuiToken.Token, $"Points à dépenser : {player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_REMAINING_ABILITY_POINTS").Value}");

          for (int i = 0; i < 6; i++)
          {
            abilities1[i] = false;
            abilities2[i] = false;
          }

          if (mainBonus > -1)
            abilities2[mainBonus] = true;

          if (secondaryBonus > -1)
            abilities1[secondaryBonus] = true;

          for(int i = 0; i < 6; i++)
            levels[i] = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_CHARACTER_SET_ABILITY_{i}").Value + abilities1[i].ToInt() + abilities2[i].ToInt() * 2;

          icon.SetBindValues(player.oid, nuiToken.Token, icons);
          abilityName.SetBindValues(player.oid, nuiToken.Token, names);
          abilityLevel.SetBindValues(player.oid, nuiToken.Token, levels.Select(x => x.ToString()).ToArray());
          bonus2Checked.SetBindValues(player.oid, nuiToken.Token, abilities2);
          bonus1Checked.SetBindValues(player.oid, nuiToken.Token, abilities1);

          bonus1Checked.SetBindWatch(player.oid, nuiToken.Token, true);
          bonus2Checked.SetBindWatch(player.oid, nuiToken.Token, true);

          if (mainBonus > -1 && secondaryBonus > -1 && player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_REMAINING_ABILITY_POINTS").Value < 1)
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").Delete();
          else
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").Value = 1;
        }
      }
    }
  }
}
