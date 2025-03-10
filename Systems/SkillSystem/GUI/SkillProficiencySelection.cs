using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class SkillProficiencySelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();

        private readonly NuiBind<string> availableSkillIcons = new("availableSkillIcons");
        private readonly NuiBind<string> availableSkillNames = new("availableSkillNames");
        private readonly NuiBind<string> acquiredSkillIcons = new("acquiredSkillIcons");
        private readonly NuiBind<string> acquiredSkillNames = new("acquiredSkillNames");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<int> listAcquiredSkillCount = new("listAcquiredSkillCount");

        private readonly NuiBind<bool> enabled = new("enabled");

        private readonly List<LearnableSkill> availableSkills = new();
        private readonly List<LearnableSkill> acquiredSkills = new();

        private List<int> skillList;
        private int nbSkills;
        private int learningClass;
        private int learningFeat;

        public SkillProficiencySelectionWindow(Player player, List<int> skillList, int nbSkills, int learningClass = 0, int learningFeat = 0) : base(player)
        {
          windowId = "skillProficiencySelection";

          List<NuiListTemplateCell> rowTemplateAvailableSkills = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(availableSkillIcons) { Id = "availableSkillDescription", Tooltip = availableSkillNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(availableSkillNames) { Id = "availableSkillDescription", Tooltip = availableSkillNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("add_arrow") { Id = "selectSkill", Tooltip = "Sélectionner", DisabledTooltip = "Vous ne pouvez pas choisir davantage de maîtrises" }) { Width = 35 }
          };

          List<NuiListTemplateCell> rowTemplateAcquiredSkills = new()
          {
            new NuiListTemplateCell(new NuiButtonImage("remove_arrow") { Id = "removeSkill", Tooltip = "Retirer" }) { Width = 35 },
            new NuiListTemplateCell(new NuiButtonImage(acquiredSkillIcons) { Id = "acquiredSkillDescription", Tooltip = acquiredSkillNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(acquiredSkillNames) { Id = "acquiredSkillDescription", Tooltip = acquiredSkillNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true }
          };

          rootColumn.Children = new()
          {
            new NuiRow() { Children = new() {
              new NuiList(rowTemplateAvailableSkills, listCount) { RowHeight = 35,  Width = 240  },
              new NuiList(rowTemplateAcquiredSkills, listAcquiredSkillCount) { RowHeight = 35, Width = 240 } } },
            new NuiRow() { Children = new() {
              new NuiSpacer(),
              new NuiButton("Valider") { Id = "validate", Tooltip = "Valider", Enabled = enabled, Encouraged = enabled, Width = 180, Height = 35 },
              new NuiSpacer() } }
          };

          CreateWindow(skillList, nbSkills, learningClass, learningFeat);
        }
        public void CreateWindow(List<int> skillList, int nbSkills, int learningClass = 0, int learningFeat = 0)
        {
          this.skillList = skillList;
          this.nbSkills = nbSkills;
          this.learningClass = learningClass;
          this.learningFeat = learningFeat;

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          string title = $"Veuillez choisir {nbSkills} maîtrise(s)";

          window = new NuiWindow(rootColumn, title.Remove(title.Length))
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

            nuiToken.OnNuiEvent += HandleSkillProficiencySelectionEvents;

            InitSkillsBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if(availableSkills.Count < 1)
            {
              CloseWindow();
              player.oid.SendServerMessage($"Vous maîtrisez déjà toutes les compétences", ColorConstants.Orange);
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_NB_SKILL_PROFICIENCY_SELECTION").Delete();
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_IN_SKILL_PROFICIENCY_SELECTION").Delete();
            }
            else
            {
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_NB_SKILL_PROFICIENCY_SELECTION").Value = nbSkills;

              string availableSkillList = "";

              foreach (int skill in skillList)
                availableSkillList += $"{skill}_";

              availableSkillList = availableSkillList[..^1];
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_IN_SKILL_PROFICIENCY_SELECTION").Value = availableSkillList;
            }
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleSkillProficiencySelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "selectSkill":

                  LearnableSkill selectedSkill = availableSkills[nuiEvent.ArrayIndex];

                  acquiredSkills.Add(selectedSkill);
                  availableSkills.Remove(selectedSkill);

                  BindAvailableSkills();
                  BindAcquiredSkills();

                  break;

                case "removeSkill":

                  LearnableSkill clickedSkill = acquiredSkills[nuiEvent.ArrayIndex];

                  if (acquiredSkills.Count < nbSkills)
                    availableSkills.Add(clickedSkill);
                  else
                  {
                    foreach (var skillId in skillList)
                      if(!player.learnableSkills.TryGetValue(skillId, out LearnableSkill learnable) || learnable.currentLevel < 1)
                        availableSkills.Add((LearnableSkill)SkillSystem.learnableDictionary[skillId]);
                  }
                  
                  acquiredSkills.Remove(clickedSkill);

                  BindAvailableSkills();
                  BindAcquiredSkills();

                  break;

                case "availableSkillDescription":

                  if (!player.windows.TryGetValue("learnableDescription", out var spellWindow)) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, availableSkills[nuiEvent.ArrayIndex].id));
                  else ((LearnableDescriptionWindow)spellWindow).CreateWindow(availableSkills[nuiEvent.ArrayIndex].id);

                  break;

                case "acquiredSpellDescription":

                  if (!player.windows.TryGetValue("learnableDescription", out var window)) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, availableSkills[nuiEvent.ArrayIndex].id));
                  else ((LearnableDescriptionWindow)window).CreateWindow(availableSkills[nuiEvent.ArrayIndex].id);

                  break;

                case "validate":

                  foreach (var skill in acquiredSkills)
                  {
                    if (learningClass == CustomSkill.ClercSavoir)
                    {
                      player.LearnClassSkill(skill.id);
                      player.LearnClassSkill(skill.id + 1);
                      player.oid.SendServerMessage($"Vous apprenez l'expertise {StringUtils.ToWhitecolor(skill.name)}", ColorConstants.Orange);
                    }
                    else if (Utils.In(learningFeat, CustomSkill.EspritVif, CustomSkill.Observateur))
                    {
                      if (player.learnableSkills.TryGetValue(skill.id, out var mastery) && mastery.currentLevel > 0)
                      {
                        player.LearnClassSkill(skill.id + 1);
                        player.oid.SendServerMessage($"Vous apprenez l'expertise {StringUtils.ToWhitecolor(skill.name)}", ColorConstants.Orange);
                      }
                      else
                      {
                        player.LearnClassSkill(skill.id);
                        player.oid.SendServerMessage($"Vous apprenez la maîtrise {StringUtils.ToWhitecolor(skill.name)}", ColorConstants.Orange);
                      }
                    }
                    else
                    {
                      player.LearnClassSkill(skill.id);
                      player.oid.SendServerMessage($"Vous apprenez la maîtrise {StringUtils.ToWhitecolor(skill.name)}", ColorConstants.Orange);
                    }
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_NB_SKILL_PROFICIENCY_SELECTION").Delete();
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_IN_SKILL_PROFICIENCY_SELECTION").Delete();

                  CloseWindow();

                  break;
              }

              break;
          }
        }
        private void InitSkillsBinding()
        {
          List<string> availableIconsList = new();
          List<string> availableNamesList = new();
          List<bool> selectableList = new();

          availableSkills.Clear();
          acquiredSkills.Clear();            

          foreach (var skillId in skillList)
          {
            if (!player.learnableSkills.TryGetValue(skillId, out LearnableSkill learnable) || learnable.currentLevel < 1)
            {
              learnable = (LearnableSkill)SkillSystem.learnableDictionary[skillId];
              availableSkills.Add(learnable);

              availableIconsList.Add(learnable.icon);
              availableNamesList.Add(learnable.name);
              selectableList.Add(true);
            }
          }

          availableSkillIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableSkillNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableSkills.Count);
          enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void BindAvailableSkills()
        {
          List<string> availableIconsList = new();
          List<string> availableNamesList = new();

          if (acquiredSkills.Count == nbSkills)
            availableSkills.Clear();

          foreach (var skill in availableSkills)
          {
            availableIconsList.Add(skill.icon);
            availableNamesList.Add(skill.name);
          }

          availableSkillIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableSkillNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableSkills.Count);
        }
        private void BindAcquiredSkills()
        {
          List<string> acquiredIconsList = new();
          List<string> acquiredNamesList = new();

          foreach (var skill in acquiredSkills)
          {
            acquiredIconsList.Add(skill.icon);
            acquiredNamesList.Add(skill.name);
          }

          acquiredSkillIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredSkillNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listAcquiredSkillCount.SetBindValue(player.oid, nuiToken.Token, acquiredSkills.Count);
          
          if (acquiredSkills.Count == nbSkills || availableSkills.Count < 1)
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
      }
    }
  }
}
