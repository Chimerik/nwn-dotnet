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
        private readonly NuiBind<List<NuiComboEntry>> availableAbilities = new("availableAbilities");

        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<int> selectedAbility = new("selectedAbility");
        private readonly NuiBind<bool> enabled = new("enabled");

        private bool giveSaveProficiency;

        public AbilityBonusChoiceWindow(Player player, List<NuiComboEntry> abilityChoice, bool giveSaveProficiency = false) : base(player)
        {
          windowId = "abilityBonusChoice";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = availableAbilities, Selected = selectedAbility } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80, Encouraged = enabled }, new NuiSpacer() } });
          
          CreateWindow(abilityChoice, giveSaveProficiency);
        }
        public void CreateWindow(List<NuiComboEntry> abilitychoice, bool giveSaveProficiency = false)
        {
          this.giveSaveProficiency = giveSaveProficiency;

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_BONUS_CHOICE_FEAT").Value = 1;

          foreach(var ability in abilitychoice)
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_IN_ABILITY_BONUS_CHOICE_FEAT_{ability.Value}").Value = ability.Value;


          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Choisissez une caractéristique à augmenter de 1 (max 20)")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleAbilityImprovementEvents;

            availableAbilities.SetBindValue(player.oid, nuiToken.Token, abilitychoice);
            ModuleSystem.Log.Info($"bind : {abilitychoice.FirstOrDefault().Value}");
            selectedAbility.SetBindValue(player.oid, nuiToken.Token, abilitychoice.FirstOrDefault().Value);
            ModuleSystem.Log.Info($"selected : {selectedAbility.GetBindValue(player.oid, nuiToken.Token)}");
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

                  Ability ability = (Ability)selectedAbility.GetBindValue(player.oid, nuiToken.Token);

                  player.oid.LoginCreature.SetsRawAbilityScore(ability, (byte)(player.oid.LoginCreature.GetRawAbilityScore(ability) + 1 < 20
                    ? player.oid.LoginCreature.GetRawAbilityScore(ability) + 1
                    : 20));
                  
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ABILITY_BONUS_CHOICE_FEAT").Delete();

                  for (int i = 0; i < 6; i++)
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_IN_ABILITY_BONUS_CHOICE_FEAT_{i}").Delete();

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

                  CloseWindow();

                  return;
              }

              break;

          }
        }
      }
    }
  }
}
