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
      public class AbilityBonusChoiceWindow : PlayerWindow
      {
        private readonly List<Ability> availableAbilities = new();
        private readonly string[] icons = new string[] { "strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma" };
        private readonly string[] names = new string[] { "Force", "Dextérité", "Constitution", "Intelligence", "Sagesse", "Charisme" };

        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> abilityName = new("abilityName");
        private readonly NuiBind<string> abilityLevel = new("abilityLevel");
        private readonly NuiBind<bool> bonusChecked = new("bonusChecked");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly NuiBind<int> rowCount = new("rowCount");

        private int selectedAbility;
        private bool giveSaveProficiency;

        public AbilityBonusChoiceWindow(Player player, List<Ability> abilityChoice, bool giveSaveProficiency = false) : base(player)
        {
          windowId = "abilityBonusChoice";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiSpacer()),
            new NuiListTemplateCell(new NuiButtonImage(icon)) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(abilityName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 120 },
            new NuiListTemplateCell(new NuiLabel(abilityLevel) { Margin = 0.0f, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer()),
            new NuiListTemplateCell(new NuiCheck("", bonusChecked) { Tooltip = "Attribuer un bonus de +1 à cette caractéristique", Margin = 0.0f }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiLabel("Attribuez +1 à une caractéristique de cette liste (max 20)") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, rowCount) { RowHeight = 40, Scrollbars = NuiScrollbars.None } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80, Enabled = enabled, Encouraged = enabled }, new NuiSpacer() } });
          
          CreateWindow(abilityChoice, giveSaveProficiency);
        }
        public void CreateWindow(List<Ability> abilitychoice, bool giveSaveProficiency = false)
        {
          this.giveSaveProficiency = giveSaveProficiency;
          availableAbilities.Clear();
          availableAbilities.AddRange(abilitychoice);

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_BONUS_CHOICE_FEAT").Value = 1;

          foreach(var ability in availableAbilities)
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_IN_ABILITY_BONUS_CHOICE_FEAT_{(int)ability}").Value = (int)ability;


          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

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

                  Ability ability = availableAbilities[selectedAbility];

                  player.oid.LoginCreature.SetsRawAbilityScore(ability, (byte)(player.oid.LoginCreature.GetRawAbilityScore(ability) + 1 < 20
                    ? player.oid.LoginCreature.GetRawAbilityScore(ability) + 1
                    : 20));
                  
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_BONUS_CHOICE_FEAT").Delete();

                  foreach (var stat in availableAbilities)
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_IN_ABILITY_BONUS_CHOICE_FEAT_{(int)stat}").Delete();

                  if (giveSaveProficiency)
                  {
                    var saveSkill = ability switch
                    {
                      Ability.Strength => CustomSkill.StrengthSavesProficiency,
                      Ability.Dexterity => CustomSkill.DexteritySavesProficiency,
                      Ability.Intelligence => CustomSkill.IntelligenceSavesProficiency,
                      Ability.Wisdom => CustomSkill.WisdomSavesProficiency,
                      Ability.Charisma => CustomSkill.CharismaSavesProficiency,
                      _ => CustomSkill.ConstitutionSavesProficiency,
                    };

                    if (player.learnableSkills.TryGetValue(saveSkill, out LearnableSkill saveLearnable))
                    {
                      if (saveLearnable.currentLevel < 1)
                      {
                        saveLearnable.currentLevel = 0;
                        saveLearnable.LevelUp(player);
                      }
                    }
                    else
                    {
                      player.learnableSkills.Add(saveSkill, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[saveSkill], player));
                      player.learnableSkills[saveSkill].LevelUp(player);
                    }
                  }

                  player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

                  return;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "bonusChecked": LoadAbilityList(nuiEvent.ArrayIndex); break;
              }

              break;

          }
        }
        private void LoadAbilityList(int indexChecked = -1)
        {
          selectedAbility = indexChecked;
          bonusChecked.SetBindWatch(player.oid, nuiToken.Token, false);

          List<string> abilityNamesList = new();
          List<string> abilityIconsList = new();
          List<int> abilityLevelList = new();
          List<bool> abilityCheckList = new();

          foreach(var ability in availableAbilities)
          {
            int stat = (int)ability;
            abilityNamesList.Add(names[stat]);
            abilityIconsList.Add(icons[stat]);
            abilityCheckList.Add(stat == indexChecked);
            abilityLevelList.Add(player.oid.LoginCreature.GetRawAbilityScore(ability) + (stat == indexChecked).ToInt() < 20
              ? player.oid.LoginCreature.GetRawAbilityScore(ability) + (stat == indexChecked).ToInt()
              : 20);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, abilityIconsList);
          abilityName.SetBindValues(player.oid, nuiToken.Token, abilityNamesList);
          abilityLevel.SetBindValues(player.oid, nuiToken.Token, abilityLevelList.Select(x => x.ToString()).ToArray());
          bonusChecked.SetBindValues(player.oid, nuiToken.Token, abilityCheckList);
          rowCount.SetBindValue(player.oid, nuiToken.Token, availableAbilities.Count);

          bonusChecked.SetBindWatch(player.oid, nuiToken.Token, true);

          if (selectedAbility > -1)
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
      }
    }
  }
}
