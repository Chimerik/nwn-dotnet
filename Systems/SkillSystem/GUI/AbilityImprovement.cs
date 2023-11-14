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
      public class AbilityImprovementWindow : PlayerWindow
      {
        private readonly string[] icons = new string[] { "strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma" };
        private readonly string[] names = new string[] { "Force", "Dextérité", "Constitution", "Intelligence", "Sagesse", "Charisme" };

        private readonly bool[] abilities2 = new bool[] { false, false, false, false, false, false };
        private readonly bool[] abilities1 = new bool[] { false, false, false, false, false, false };

        private readonly int[] levels = new int[] { 0, 0, 0, 0, 0, 0};

        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> abilityName = new("abilityName");
        private readonly NuiBind<string> abilityLevel = new("abilityLevel");
        private readonly NuiBind<bool> plus2Checked = new("plus2Checked");
        private readonly NuiBind<bool> plus1Checked = new("plus1Checked");
        private readonly NuiBind<bool> enabled = new("enabled");

        public AbilityImprovementWindow(Player player) : base(player)
        {
          windowId = "abilityImprovement";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiSpacer()),
            new NuiListTemplateCell(new NuiButtonImage(icon)) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(abilityName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 120 },
            new NuiListTemplateCell(new NuiLabel(abilityLevel) { Margin = 0.0f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer()),
            new NuiListTemplateCell(new NuiCheck("", plus2Checked) { Tooltip = "Attribuer un bonus de +2 à cette caractéristique", Margin = 0.0f }) { Width = 40 },
            new NuiListTemplateCell(new NuiCheck("", plus1Checked) { Tooltip = "Attribuer un bonus de +1 à deux caractéristiques", Margin = 0.0f }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiLabel("Attribuez +2 à une caractéristique, ou +1 à deux caractéristiques (max 20)") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, 6) { RowHeight = 40, Scrollbars = NuiScrollbars.None } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80, Enabled = enabled, Encouraged = enabled }, new NuiSpacer() } });
          CreateWindow();
        }
        public void CreateWindow()
        {
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_IMPROVEMENT_FEAT").Value = 1;

          if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").HasNothing)
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").Value = -1;

          if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").HasNothing)
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value = -1;

          if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").HasNothing)
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Value = -1;

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.3f, player.guiHeight * 0.25f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Don - Amélioration de caractéristiques")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleAbilityImprovementEvents;

            LoadAbilityList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleAbilityImprovementEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  CloseWindow();

                  Ability ability;

                  if(player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").Value > -1)
                  {
                    ability = (Ability)player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").Value;
                    player.oid.LoginCreature.SetsRawAbilityScore(ability, (byte)(player.oid.LoginCreature.GetRawAbilityScore(ability) + 2 < 20
                      ? player.oid.LoginCreature.GetRawAbilityScore(ability) + 2
                      : 20));
                  }
                  else
                  {
                    ability = (Ability)player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value;
                    player.oid.LoginCreature.SetsRawAbilityScore(ability, (byte)(player.oid.LoginCreature.GetRawAbilityScore(ability) + 1 < 20
                      ? player.oid.LoginCreature.GetRawAbilityScore(ability) + 1
                      : 20));

                    ability = (Ability)player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Value;
                    player.oid.LoginCreature.SetsRawAbilityScore(ability, (byte)(player.oid.LoginCreature.GetRawAbilityScore(ability) + 1 < 20 
                      ? player.oid.LoginCreature.GetRawAbilityScore(ability) + 1 
                      : 20));
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").Delete();
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Delete();
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Delete();
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_IMPROVEMENT_FEAT").Delete();

                  player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

                  return;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "plus1Checked":

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").Value = -1;

                  int ability1 = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value;
                  int ability2 = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Value;

                  if (abilities1[nuiEvent.ArrayIndex])
                  {
                    if (nuiEvent.ArrayIndex == ability1)
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value = -1;

                    if (nuiEvent.ArrayIndex == ability2)
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Value = -1;
                  }
                  else
                  {
                    if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value > -1)
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Value = nuiEvent.ArrayIndex;
                    else
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value = nuiEvent.ArrayIndex;
                  }

                  LoadAbilityList();

                  break;

                case "plus2Checked":

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").Value = nuiEvent.ArrayIndex;
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value = -1;
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Value = -1;
                  
                  LoadAbilityList();
                  break;
              }

              break;

          }
        }
        private void LoadAbilityList()
        {
          plus1Checked.SetBindWatch(player.oid, nuiToken.Token, false);
          plus2Checked.SetBindWatch(player.oid, nuiToken.Token, false);

          int plus2 = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+2_BONUS").Value;
          int mainPlus1 = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_1_BONUS").Value;
          int secondPlus1 = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_+1_2_BONUS").Value;

          for (int i = 0; i < 6; i++)
          {
            abilities1[i] = false;
            abilities2[i] = false;
          }

          if (plus2 > -1)
            abilities2[plus2] = true;
          else
          {
            if (mainPlus1 > -1)
              abilities1[mainPlus1] = true;

            if (secondPlus1 > -1)
              abilities1[secondPlus1] = true;
          }

          for (int i = 0; i < 6; i++)
            levels[i] = player.oid.LoginCreature.GetRawAbilityScore((Ability)i) + abilities1[i].ToInt() + abilities2[i].ToInt() * 2 < 20
              ? player.oid.LoginCreature.GetRawAbilityScore((Ability)i) + abilities1[i].ToInt() + abilities2[i].ToInt() * 2 
              : 20;

          icon.SetBindValues(player.oid, nuiToken.Token, icons);
          abilityName.SetBindValues(player.oid, nuiToken.Token, names);
          abilityLevel.SetBindValues(player.oid, nuiToken.Token, levels.Select(x => x.ToString()).ToArray());
          plus2Checked.SetBindValues(player.oid, nuiToken.Token, abilities2);
          plus1Checked.SetBindValues(player.oid, nuiToken.Token, abilities1);

          plus1Checked.SetBindWatch(player.oid, nuiToken.Token, true);
          plus2Checked.SetBindWatch(player.oid, nuiToken.Token, true);

          if (plus2 > -1 || (mainPlus1 > -1 && secondPlus1 > -1))
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
      }
    }
  }
}
