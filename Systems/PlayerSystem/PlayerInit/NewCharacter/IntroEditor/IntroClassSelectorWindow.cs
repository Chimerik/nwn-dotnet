﻿using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class IntroClassSelectorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> selectedItemTitle = new("selectedItemTitle");
        private readonly NuiBind<string> selectedItemDescription = new("selectedItemDescription");
        private readonly NuiBind<string> selectedItemIcon = new("selectedItemIcon");
        private readonly NuiBind<bool> selectedItemVisibility = new("selectedItemVisibility");
        private readonly NuiBind<bool> combatStyleVisibility = new("combatStyleVisibility");
        private readonly NuiBind<bool> rogueSkillVisibility = new("rogueSkillVisibility");
        private readonly NuiBind<bool> selectedTitleVisibility = new("selectedTitleVisibility");

        private readonly NuiBind<string> validationText = new("validationText");
        private readonly NuiBind<bool> validationEnabled = new("validationEnabled");

        private readonly NuiBind<bool> encouraged = new("encouraged");
        private readonly NuiBind<Color> color = new("color");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> skillName = new("skillName");

        private readonly NuiBind<int> selectedSkill1 = new("selectedSkill1");
        private readonly NuiBind<List<NuiComboEntry>> skillSelection1 = new("skillSelection1");

        private readonly NuiBind<int> selectedSkill2 = new("selectedSkill2");
        private readonly NuiBind<List<NuiComboEntry>> skillSelection2 = new("skillSelection2");

        private readonly NuiBind<int> selectedSkill3 = new("selectedSkill3");
        private readonly NuiBind<List<NuiComboEntry>> skillSelection3 = new("skillSelection3");

        private readonly NuiBind<int> selectedSkill4 = new("selectedSkill4");
        private readonly NuiBind<List<NuiComboEntry>> skillSelection4 = new("skillSelection4");

        private readonly NuiBind<int> selectedExpertise1 = new("selectedExpertise1");
        private readonly NuiBind<List<NuiComboEntry>> expertiseSelection1 = new("expertiseSelection1");

        private readonly NuiBind<int> selectedExpertise2 = new("selectedExpertise2");
        private readonly NuiBind<List<NuiComboEntry>> expertiseSelection2 = new("expertiseSelection2");

        private readonly NuiBind<int> selectedCombatStyle = new("selectedCombatStyle");

        private IEnumerable<Learnable> currentList;
        private Learnable selectedLearnable;
        private int validatedLearnableId;

        public IntroClassSelectorWindow(Player player) : base(player)
        {
          windowId = "introClassSelector";
          rootColumn.Children = rootChildren;

          List<NuiComboEntry> styles = new();

          foreach (var style in learnableDictionary.Values.Where(s => s is LearnableSkill skill && skill.category == Category.FightingStyle))
            styles.Add(new NuiComboEntry(style.name, style.id));

          List<NuiListTemplateCell> learnableTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(icon) { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(skillName) { Width = 200, Id = "select", ForegroundColor = color, Encouraged = encouraged, Tooltip = skillName, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 220 },
            new NuiListTemplateCell(new NuiButtonImage("select_right") { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").HasValue },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Stats") { Id = "stats", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").HasValue },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiColumn() { Children = new List<NuiElement>() { new NuiList(learnableTemplate, listCount) { RowHeight = 40 } }, Width = 340 },
            new NuiSpacer(),
            new NuiColumn() { Children = new List<NuiElement>()
            {
              new NuiRow() { Children = new List<NuiElement>() 
              {
                new NuiSpacer(),
                new NuiButtonImage(selectedItemIcon) { Height = 40, Width = 40, Visible = selectedTitleVisibility },
                new NuiLabel(selectedItemTitle) { Height = 40, Width = 200, Visible = selectedTitleVisibility, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
                new NuiSpacer()
              } },
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiCombo() { Entries = styles, Selected = selectedCombatStyle, Visible = combatStyleVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 1.5f },
                new NuiSpacer()
              } },
              new NuiRow() { Children = new List<NuiElement>() 
              { 
                new NuiSpacer(),
                new NuiCombo() { Entries = skillSelection1, Selected = selectedSkill1, Visible = selectedItemVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 2 },
                new NuiCombo() { Entries = skillSelection2, Selected = selectedSkill2, Visible = selectedItemVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 2 },
                new NuiSpacer()
              } },
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiCombo() { Entries = skillSelection3, Selected = selectedSkill3, Visible = rogueSkillVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 2 },
                new NuiCombo() { Entries = skillSelection4, Selected = selectedSkill4, Visible = rogueSkillVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 2 },
                new NuiSpacer()
              } },
              new NuiRow() { Children = new List<NuiElement>()
              {
                new NuiSpacer(),
                new NuiCombo() { Entries = expertiseSelection1, Selected = selectedExpertise1, Visible = rogueSkillVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 2 },
                new NuiCombo() { Entries = expertiseSelection2, Selected = selectedExpertise2, Visible = rogueSkillVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 2 },
                new NuiSpacer()
              } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiText(selectedItemDescription) } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton(validationText) { Id = "validate", Height = 40, Width = player.guiScaledWidth * 0.6f - 370, Enabled = validationEnabled }, new NuiSpacer() } }
            }, Width = player.guiScaledWidth * 0.6f - 370 }
          } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          validatedLearnableId = player.learnableSkills.Any(l => l.Value.category == Category.Class)
            ? player.learnableSkills.FirstOrDefault(l => l.Value.category == Category.Class).Value.id
          : -1;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f);
          selectedLearnable = null;

          window = new NuiWindow(rootColumn, "Choisissez votre classe initiale")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, "");
            selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, "Sélectionner une classe pour afficher ses détails.\n\nAttention, lorsque vous aurez quitté ce navire, ce choix deviendra définitif.");
            selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, "ir_examine");
            selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, false);
            combatStyleVisibility.SetBindValue(player.oid, nuiToken.Token, false);
            rogueSkillVisibility.SetBindValue(player.oid, nuiToken.Token, false);
            selectedTitleVisibility.SetBindValue(player.oid, nuiToken.Token, false);
            validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);

            selectedCombatStyle.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedSkill1.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedSkill2.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedSkill3.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedSkill4.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedExpertise1.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedExpertise2.SetBindValue(player.oid, nuiToken.Token, -1);
            skillSelection1.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());
            skillSelection2.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());
            skillSelection3.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());
            skillSelection4.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());
            expertiseSelection1.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());
            expertiseSelection2.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());

            selectedCombatStyle.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            currentList = learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.Class).OrderBy(s => s.name);
            LoadLearnableList(currentList);
          }
        }

        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch(nuiEvent.ElementId)
              {
                case "select":

                  if (selectedLearnable == currentList.ElementAt(nuiEvent.ArrayIndex))
                    return;

                  selectedLearnable = currentList.ElementAt(nuiEvent.ArrayIndex);
                  selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.name);
                  selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.description);
                  selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.icon);
                  selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, true);
                  selectedTitleVisibility.SetBindValue(player.oid, nuiToken.Token, true);

                  if (player.learnableSkills.ContainsKey(selectedLearnable.id))
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"Classe {selectedLearnable.name} déjà sélectionnée");
                  }
                  else
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"Valider la classe {selectedLearnable.name}");
                  }

                  InitSelectableSkills();
                  LoadLearnableList(currentList);

                  break;

                case "validate":

                  RemovePreviousClass();

                  validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  validatedLearnableId = selectedLearnable.id;

                  if (player.learnableSkills.TryAdd(selectedSkill1.GetBindValue(player.oid, nuiToken.Token), new LearnableSkill((LearnableSkill)learnableDictionary[selectedSkill1.GetBindValue(player.oid, nuiToken.Token)], player)))
                    player.learnableSkills[selectedSkill1.GetBindValue(player.oid, nuiToken.Token)].LevelUp(player);

                  player.learnableSkills[selectedSkill1.GetBindValue(player.oid, nuiToken.Token)].source.Add(Category.Class);

                  if (player.learnableSkills.TryAdd(selectedSkill2.GetBindValue(player.oid, nuiToken.Token), new LearnableSkill((LearnableSkill)learnableDictionary[selectedSkill2.GetBindValue(player.oid, nuiToken.Token)], player)))
                    player.learnableSkills[selectedSkill2.GetBindValue(player.oid, nuiToken.Token)].LevelUp(player);

                  player.learnableSkills[selectedSkill2.GetBindValue(player.oid, nuiToken.Token)].source.Add(Category.Class);

                  switch(validatedLearnableId)
                  {
                    case CustomSkill.Fighter: player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CHOSEN_FIGHTER_STYLE").Value = selectedCombatStyle.GetBindValue(player.oid, nuiToken.Token); break;
                    case CustomSkill.Rogue:

                      break;
                  }

                  player.learnableSkills.TryAdd(selectedLearnable.id, new LearnableSkill((LearnableSkill)learnableDictionary[selectedLearnable.id], player, (int)Category.Class));
                  player.learnableSkills[selectedLearnable.id].LevelUp(player);

                  CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, 0, Classes2da.classTable.FirstOrDefault(c => c.classLearnableId == validatedLearnableId).RowIndex);

                  validationText.SetBindValue(player.oid, nuiToken.Token, $"Votre classe est désormais : {selectedLearnable.name}");
                  player.oid.SendServerMessage($"Votre classe initiale est désormais {StringUtils.ToWhitecolor(selectedLearnable.name)} !", ColorConstants.Orange);

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").Delete();
                  LoadLearnableList(currentList);

                  break;

                case "welcome":

                  CloseWindow();

                  if (!player.windows.TryGetValue("bodyAppearanceModifier", out var value)) player.windows.Add("bodyAppearanceModifier", new IntroMirrorWindow(player));
                  else ((IntroMirrorWindow)value).CreateWindow();

                  return;

                case "beauty":

                  CloseWindow();

                  if (!player.windows.TryGetValue("bodyColorsModifier", out var beauty)) player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, player.oid.LoginCreature));
                  else ((BodyColorWindow)beauty).CreateWindow(player.oid.LoginCreature);

                  break;

                case "race":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introRaceSelector", out var race)) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)race).CreateWindow();

                  break;

                case "histo":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introHistorySelector", out var histo)) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)histo).CreateWindow();

                  return;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introPortrait", out var portrait)) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)portrait).CreateWindow();

                  break;

                case "stats":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introAbilities", out var stats)) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)stats).CreateWindow();

                  return;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "selectedSkill1":
                case "selectedSkill2":
                case "selectedSkill3":
                case "selectedSkill4": LoadSelectableSkills(); break;
                case "selectedExpertise1": 
                case "selectedExpertise2": LoadSelectableExpertises(); break;
                case "selectedCombatStyle": validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);break;
              }

              break;
          }
        }
        private void LoadLearnableList(IEnumerable<Learnable> filteredList)
        {
          List<string> iconList = new List<string>();
          List<string> skillNameList = new List<string>();
          List<Color> colorList = new();
          List<bool> encouragedList = new();

          foreach (Learnable learnable in filteredList)
          {
            iconList.Add(learnable.icon);
            skillNameList.Add(learnable.name);
            encouragedList.Add(validatedLearnableId == learnable.id);

            if (selectedLearnable is not null)
              colorList.Add(selectedLearnable == learnable ? ColorConstants.White : ColorConstants.Gray);
            else
              colorList.Add(ColorConstants.White);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          skillName.SetBindValues(player.oid, nuiToken.Token, skillNameList);
          color.SetBindValues(player.oid, nuiToken.Token, colorList);
          encouraged.SetBindValues(player.oid, nuiToken.Token, encouragedList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
        private void RemovePreviousClass()
        {
          List<LearnableSkill> profienciesToRemove = new();

          player.oid.LoginCreature.RemoveFeat(NwFeat.FromFeatType(Feat.SneakAttack));

          if (player.windows.TryGetValue("expertiseChoice", out var expertise) && expertise.IsOpen)
          {
            expertise.CloseWindow();
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_CHOICE").Delete();
          }

          foreach (var skill in player.learnableSkills.Where(l => l.Value.source.Any(s => s == Category.Class)))
            profienciesToRemove.Add(skill.Value);

          foreach (var proficiency in profienciesToRemove)
          {
            if (proficiency.source.Count < 2)
              player.learnableSkills.Remove(proficiency.id);
            else
              proficiency.source.Remove(Category.Class);

            NwFeat feat = NwFeat.FromFeatId(proficiency.id);

            if (feat is not null && player.oid.LoginCreature.KnowsFeat(feat))
              player.oid.LoginCreature.RemoveFeat(feat);
          }
        }
        private void InitSelectableSkills()
        {
          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill3.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill4.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedExpertise1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedExpertise2.SetBindWatch(player.oid, nuiToken.Token, true);

          if (selectedLearnable.id == CustomSkill.Fighter)
          {
            combatStyleVisibility.SetBindValue(player.oid, nuiToken.Token, true);

            LearnableSkill style = player.learnableSkills.Values.FirstOrDefault(s => s is LearnableSkill style && style.category == Category.FightingStyle);
            if (style is not null)
              selectedCombatStyle.SetBindValue(player.oid, nuiToken.Token, style.id);
            else
              selectedCombatStyle.SetBindValue(player.oid, nuiToken.Token, CustomSkill.FighterCombatStyleArchery);
          }
          else
            combatStyleVisibility.SetBindValue(player.oid, nuiToken.Token, false);

          var startingPackageList = selectedLearnable.id switch
          {
            CustomSkill.Barbarian => Barbarian.startingPackage.skillChoiceList,
            CustomSkill.Rogue => Rogue.startingPackage.skillChoiceList,
            _ => Fighter.startingPackage.skillChoiceList,
          };

          List<NuiComboEntry> skillList1 = new();
          List<NuiComboEntry> skillList2 = new();

          foreach (var learnable in startingPackageList)
          {
            if (!player.learnableSkills.TryGetValue(learnable.id, out var value) || value.source.Any(so => so == Category.Class))
            {
              skillList1.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
              skillList2.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
            }
          }

          var bonusSkills = player.learnableSkills.Values.Where(s => s.category == Category.Skill && s.source.Any(so => so == Category.Class));

          if (bonusSkills.Count() > 1)
          {
            skillList1.Remove(skillList1.First(s => s.Value == bonusSkills.ElementAt(1).id));
            skillList2.Remove(skillList2.First(s => s.Value == bonusSkills.ElementAt(0).id));
          }
          else
          {
            skillList1.RemoveAt(1);
            skillList2.RemoveAt(0);
          }

          skillSelection1.SetBindValue(player.oid, nuiToken.Token, skillList1);
          selectedSkill1.SetBindValue(player.oid, nuiToken.Token, bonusSkills.Any() ? bonusSkills.ElementAt(0).id : skillList1.First().Value);

          skillSelection2.SetBindValue(player.oid, nuiToken.Token, skillList2);
          selectedSkill2.SetBindValue(player.oid, nuiToken.Token, bonusSkills.Count() > 1 ? bonusSkills.ElementAt(1).id : skillList2.First().Value);

          if (selectedLearnable.id == CustomSkill.Rogue)
          {
            List<NuiComboEntry> skillList3 = new();
            List<NuiComboEntry> skillList4 = new();

            foreach (var learnable in Rogue.startingPackage.skillChoiceList)
            {
              if (!player.learnableSkills.TryGetValue(learnable.id, out var value) || value.source.Any(so => so == Category.Class))
              {
                skillList3.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
                skillList4.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
              }
            }

            var additionnalSkills = player.learnableSkills.Values.Where(s => s.category == Category.Skill && s.source.Any(so => so == Category.Class));

            if(additionnalSkills.Count() > 3)
            {
              skillList1.Remove(skillList1.First(s => s.Value == additionnalSkills.ElementAt(2).id));
              skillList1.Remove(skillList1.First(s => s.Value == additionnalSkills.ElementAt(3).id));
              skillList2.Remove(skillList2.First(s => s.Value == additionnalSkills.ElementAt(2).id));
              skillList2.Remove(skillList2.First(s => s.Value == additionnalSkills.ElementAt(3).id));
              skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(0).id));
              skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(1).id));
              skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(3).id));
              skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(0).id));
              skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(1).id));
              skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(2).id));
              
            }
            else if(additionnalSkills.Count() > 1)
            {
              skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(0).id));
              skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(1).id));
              skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(0).id));
              skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(1).id));
            }
            else
            {
              skillList3.RemoveAt(0);
              skillList3.RemoveAt(1);
              skillList3.RemoveAt(3);
              skillList4.RemoveAt(0);
              skillList4.RemoveAt(1);
              skillList4.RemoveAt(2);
            }

            skillSelection1.SetBindValue(player.oid, nuiToken.Token, skillList1);
            selectedSkill1.SetBindValue(player.oid, nuiToken.Token, bonusSkills.Any() ? bonusSkills.ElementAt(0).id : skillList1.First().Value);

            skillSelection2.SetBindValue(player.oid, nuiToken.Token, skillList2);
            selectedSkill2.SetBindValue(player.oid, nuiToken.Token, bonusSkills.Count() > 1 ? bonusSkills.ElementAt(1).id : skillList2.First().Value);

            skillSelection3.SetBindValue(player.oid, nuiToken.Token, skillList3);
            selectedSkill3.SetBindValue(player.oid, nuiToken.Token, additionnalSkills.Any() ? additionnalSkills.ElementAt(2).id : skillList3.First().Value);

            skillSelection4.SetBindValue(player.oid, nuiToken.Token, skillList4);
            selectedSkill4.SetBindValue(player.oid, nuiToken.Token, additionnalSkills.Count() > 1 ? additionnalSkills.ElementAt(3).id : skillList4.First().Value);


            List<NuiComboEntry> expertiseList1 = new();
            List<NuiComboEntry> expertiseList2 = new();

            foreach (var learnable in learnableDictionary.Values.Where(e => e is LearnableSkill expertise && expertise.category == Category.Expertise))
            {
              if (learnable.id == selectedSkill1.GetBindValue(player.oid, nuiToken.Token) || learnable.id == selectedSkill2.GetBindValue(player.oid, nuiToken.Token)
                || learnable.id == selectedSkill3.GetBindValue(player.oid, nuiToken.Token) || learnable.id == selectedSkill4.GetBindValue(player.oid, nuiToken.Token)
                || (player.learnableSkills.TryGetValue(learnable.id - 1, out var proficiency) && proficiency.currentLevel > 0))
              {
                expertiseList1.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
                expertiseList2.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
              }
            }

            var bonusExpertise = player.learnableSkills.Values.Where(s => s.category == Category.Expertise && s.source.Any(so => so == Category.Class));

            if (bonusExpertise.Count() > 1)
            {
              expertiseList1.Remove(expertiseList1.First(s => s.Value == bonusExpertise.ElementAt(1).id));
              expertiseList2.Remove(expertiseList2.First(s => s.Value == bonusExpertise.ElementAt(0).id));
            }
            else
            {
              expertiseList1.RemoveAt(1);
              expertiseList2.RemoveAt(0);
            }

            expertiseSelection1.SetBindValue(player.oid, nuiToken.Token, expertiseList1);
            selectedExpertise1.SetBindValue(player.oid, nuiToken.Token, bonusExpertise.Any() ? bonusExpertise.ElementAt(0).id : expertiseList1.First().Value);

            expertiseSelection2.SetBindValue(player.oid, nuiToken.Token, expertiseList2);
            selectedExpertise2.SetBindValue(player.oid, nuiToken.Token, bonusExpertise.Count() > 1 ? bonusExpertise.ElementAt(1).id : expertiseList2.First().Value);
          }

          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill3.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill4.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedExpertise1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedExpertise2.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void LoadSelectableSkills()
        {
          validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);

          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedSkill3.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedSkill4.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedExpertise1.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedExpertise2.SetBindWatch(player.oid, nuiToken.Token, false);

          List<NuiComboEntry> skillList1 = new();
          List<NuiComboEntry> skillList2 = new();

          var startingPackageList = selectedLearnable.id switch
          {
            CustomSkill.Barbarian => Barbarian.startingPackage.skillChoiceList,
            CustomSkill.Rogue => Rogue.startingPackage.skillChoiceList,
            _ => Fighter.startingPackage.skillChoiceList,
          };

          foreach (var learnable in startingPackageList)
          {
            if (!player.learnableSkills.TryGetValue(learnable.id, out var value) || value.source.Any(so => so == Category.Class))
            {
              skillList1.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
              skillList2.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
            }
          }

          skillList1.Remove(skillList1.First(s => s.Value == selectedSkill2.GetBindValue(player.oid, nuiToken.Token)));
          skillList2.Remove(skillList2.First(s => s.Value == selectedSkill1.GetBindValue(player.oid, nuiToken.Token)));

          skillSelection1.SetBindValue(player.oid, nuiToken.Token, skillList1);
          skillSelection2.SetBindValue(player.oid, nuiToken.Token, skillList2);

          if (selectedLearnable.id == CustomSkill.Rogue)
          {
            List<NuiComboEntry> skillList3 = new();
            List<NuiComboEntry> skillList4 = new();

            foreach (var learnable in Rogue.startingPackage.skillChoiceList)
            {
              if (!player.learnableSkills.TryGetValue(learnable.id, out var value) || value.source.Any(so => so == Category.Class))
              {
                skillList3.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
                skillList4.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
              }
            }

            var additionnalSkills = player.learnableSkills.Values.Where(s => s.category == Category.Skill && s.source.Any(so => so == Category.Class));

            skillList1.Remove(skillList1.First(s => s.Value == additionnalSkills.ElementAt(2).id));
            skillList1.Remove(skillList1.First(s => s.Value == additionnalSkills.ElementAt(3).id));
            skillList2.Remove(skillList2.First(s => s.Value == additionnalSkills.ElementAt(2).id));
            skillList2.Remove(skillList2.First(s => s.Value == additionnalSkills.ElementAt(3).id));
            skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(0).id));
            skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(1).id));
            skillList3.Remove(skillList3.First(s => s.Value == additionnalSkills.ElementAt(3).id));
            skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(0).id));
            skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(1).id));
            skillList4.Remove(skillList4.First(s => s.Value == additionnalSkills.ElementAt(2).id));

            skillSelection1.SetBindValue(player.oid, nuiToken.Token, skillList1);
            skillSelection2.SetBindValue(player.oid, nuiToken.Token, skillList2);
            skillSelection3.SetBindValue(player.oid, nuiToken.Token, skillList3);
            skillSelection4.SetBindValue(player.oid, nuiToken.Token, skillList4);

            List<NuiComboEntry> expertiseList1 = new();
            List<NuiComboEntry> expertiseList2 = new();

            foreach (var learnable in learnableDictionary.Values.Where(e => e is LearnableSkill expertise && expertise.category == Category.Expertise))
            {
              if (learnable.id == selectedSkill1.GetBindValue(player.oid, nuiToken.Token) || learnable.id == selectedSkill2.GetBindValue(player.oid, nuiToken.Token)
                || learnable.id == selectedSkill3.GetBindValue(player.oid, nuiToken.Token) || learnable.id == selectedSkill4.GetBindValue(player.oid, nuiToken.Token)
                || (player.learnableSkills.TryGetValue(learnable.id - 1, out var proficiency) && proficiency.currentLevel > 0))
              {
                expertiseList1.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
                expertiseList2.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
              }
            }

            var bonusExpertise = player.learnableSkills.Values.Where(s => s.category == Category.Expertise && s.source.Any(so => so == Category.Class));

            if (bonusExpertise.Count() > 1)
            {
              expertiseList1.Remove(expertiseList1.First(s => s.Value == bonusExpertise.ElementAt(1).id));
              expertiseList2.Remove(expertiseList2.First(s => s.Value == bonusExpertise.ElementAt(0).id));
            }
            else
            {
              expertiseList1.RemoveAt(1);
              expertiseList2.RemoveAt(0);
            }

            expertiseSelection1.SetBindValue(player.oid, nuiToken.Token, expertiseList1);
            expertiseSelection2.SetBindValue(player.oid, nuiToken.Token, expertiseList2);
          }

          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill3.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill4.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedExpertise1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedExpertise2.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void LoadSelectableExpertises()
        {
          if (selectedLearnable.id == CustomSkill.Rogue)
          {
            validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);

            selectedExpertise1.SetBindWatch(player.oid, nuiToken.Token, false);
            selectedExpertise2.SetBindWatch(player.oid, nuiToken.Token, false);

            List<NuiComboEntry> expertiseList1 = new();
            List<NuiComboEntry> expertiseList2 = new();

            foreach (var learnable in learnableDictionary.Values.Where(e => e is LearnableSkill expertise && expertise.category == Category.Expertise))
            {
              if (learnable.id == selectedSkill1.GetBindValue(player.oid, nuiToken.Token) || learnable.id == selectedSkill2.GetBindValue(player.oid, nuiToken.Token)
                || learnable.id == selectedSkill3.GetBindValue(player.oid, nuiToken.Token) || learnable.id == selectedSkill4.GetBindValue(player.oid, nuiToken.Token)
                || (player.learnableSkills.TryGetValue(learnable.id - 1, out var proficiency) && proficiency.currentLevel > 0))
              {
                expertiseList1.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
                expertiseList2.Add(new NuiComboEntry(learnableDictionary[learnable.id].name, learnable.id));
              }
            }

            var bonusExpertise = player.learnableSkills.Values.Where(s => s.category == Category.Expertise && s.source.Any(so => so == Category.Class));

            if (bonusExpertise.Count() > 1)
            {
              expertiseList1.Remove(expertiseList1.First(s => s.Value == bonusExpertise.ElementAt(1).id));
              expertiseList2.Remove(expertiseList2.First(s => s.Value == bonusExpertise.ElementAt(0).id));
            }
            else
            {
              expertiseList1.RemoveAt(1);
              expertiseList2.RemoveAt(0);
            }

            expertiseSelection1.SetBindValue(player.oid, nuiToken.Token, expertiseList1);
            expertiseSelection2.SetBindValue(player.oid, nuiToken.Token, expertiseList2);

            selectedExpertise1.SetBindWatch(player.oid, nuiToken.Token, true);
            selectedExpertise2.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
      }
    }
  }
}
