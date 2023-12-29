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
      public class SkillBonusChoiceWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<List<NuiComboEntry>> skills = new("skills");
        private readonly NuiBind<int> selectedSkill = new("selectedSkill");
        private readonly NuiBind<bool> enabled = new("enabled");

        private int acquiredLevel;
        private int sourceFeat;
        private int featOption;

        public SkillBonusChoiceWindow(Player player, int level, int source, int option, List<NuiComboEntry> skillList) : base(player)
        {
          windowId = "skillBonusChoice";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = skills, Selected = selectedSkill } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80, Encouraged = enabled }, new NuiSpacer() } });
          
          CreateWindow(level, source, option, skillList);
        }
        public void CreateWindow(int level, int source, int option, List<NuiComboEntry> skillList)
        {
          acquiredLevel = level;
          sourceFeat = source;
          featOption = option;

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = source;

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, $"{SkillSystem.learnableDictionary[source].name} - Choisissez une maîtrise")
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
            nuiToken.OnNuiEvent += HandleSkillBonusChoiceEvents;

            skills.SetBindValue(player.oid, nuiToken.Token, skillList);
            selectedSkill.SetBindValue(player.oid, nuiToken.Token, skillList.FirstOrDefault().Value);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleSkillBonusChoiceEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  CloseWindow();
                  int selection = selectedSkill.GetBindValue(player.oid, nuiToken.Token);

                  if (!player.learnableSkills[sourceFeat].featOptions.ContainsKey(acquiredLevel))
                    player.learnableSkills[sourceFeat].featOptions.Add(acquiredLevel, new int[] { -1, -1, -1 });

                  player.learnableSkills[sourceFeat].featOptions[acquiredLevel][featOption] = selection;
                  player.learnableSkills[selection].LevelUp(player);
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Delete();
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Delete();


                  if(sourceFeat == CustomSkill.FighterArcaneArcher && (SkillConfig.SkillOptionType)featOption == SkillConfig.SkillOptionType.Proficiency)
                  {
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomSkill.FighterArcaneArcher;
                    player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Value = (int)SkillConfig.SkillOptionType.Cantrip;
                    player.InitializeBonusSkillChoice();
                  }

                  return;
              }

              break;

          }
        }
      }
    }
  }
}
