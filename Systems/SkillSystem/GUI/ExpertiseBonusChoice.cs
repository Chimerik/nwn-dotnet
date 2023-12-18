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
      public class ExpertiseBonusChoiceWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<List<NuiComboEntry>> skills = new("skills");
        private readonly NuiBind<int> selectedSkill = new("selectedSkill");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly List<NuiComboEntry> skillList = new();

        private int acquiredLevel;
        private int sourceFeat;

        public ExpertiseBonusChoiceWindow(Player player, int level, int source) : base(player)
        {
          windowId = "expertiseBonusChoice";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = skills, Selected = selectedSkill } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80, Encouraged = enabled }, new NuiSpacer() } });
          
          CreateWindow(level, source);
        }
        public void CreateWindow(int level, int source)
        {
          acquiredLevel = level;
          sourceFeat = source;

          foreach (var skill in SkillSystem.learnableDictionary.Values.Where(s => ((LearnableSkill)s).category == SkillSystem.Category.Expertise))
            if (player.learnableSkills.TryGetValue(skill.id, out LearnableSkill value) && value.currentLevel > 0)
              skillList.Add(new NuiComboEntry(skill.name, skill.id));

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_BONUS_CHOICE_FEAT").Value = source;

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, $"{SkillSystem.learnableDictionary[source].name} - Choisissez une expertise de compétence")
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
            nuiToken.OnNuiEvent += HandleExpertiseBonusChoiceEvents;

            skills.SetBindValue(player.oid, nuiToken.Token, skillList);
            selectedSkill.SetBindValue(player.oid, nuiToken.Token, skillList.FirstOrDefault().Value);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleExpertiseBonusChoiceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  CloseWindow();
                  int selection = selectedSkill.GetBindValue(player.oid, nuiToken.Token);
                  
                  if (player.learnableSkills[sourceFeat].featOptions.Count < 0)
                    player.learnableSkills[sourceFeat].featOptions.Add(acquiredLevel, new int[] { -1, selection, -1 } );
                  else
                    player.learnableSkills[sourceFeat].featOptions[acquiredLevel][1] = selection;

                  if (!player.learnableSkills.TryGetValue(selection, out _))
                    player.learnableSkills.Add(selection, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[selection]));
                  
                  player.learnableSkills[selection].LevelUp(player);
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_BONUS_CHOICE_FEAT").Delete();

                  if (sourceFeat == CustomSkill.Expert)
                    HandleExpertStatBonus();

                  return;
              }

              break;
          }
        }
        private void HandleExpertStatBonus()
        {
          List<Ability> abilities = new();

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
            abilities.Add(Ability.Strength);

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
            abilities.Add(Ability.Dexterity);

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
            abilities.Add(Ability.Constitution);

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Intelligence) < 20)
            abilities.Add(Ability.Intelligence);

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Wisdom) < 20)
            abilities.Add(Ability.Wisdom);

          if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) < 20)
            abilities.Add(Ability.Charisma);

          if (abilities.Count > 0)
          {
            if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
            else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
          }
        }
      }
    }
  }
}
