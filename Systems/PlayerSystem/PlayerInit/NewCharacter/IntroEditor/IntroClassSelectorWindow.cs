using System.Collections.Generic;
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
                new NuiButtonImage(selectedItemIcon) { Height = 40, Width = 40, Visible = selectedItemVisibility },
                new NuiLabel(selectedItemTitle) { Height = 40, Width = 200, Visible = selectedItemVisibility, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle },
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
                new NuiCombo() { Entries = styles, Selected = selectedCombatStyle, Visible = selectedItemVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 1.5f },
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
          validatedLearnableId = player.learnableSkills.Any(l => l.Value.category == SkillSystem.Category.Class)
            ? player.learnableSkills.FirstOrDefault(l => l.Value.category == SkillSystem.Category.Class).Value.id
          : -1;

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f);
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
            validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);

            selectedCombatStyle.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedSkill1.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedSkill2.SetBindValue(player.oid, nuiToken.Token, -1);
            skillSelection1.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());
            skillSelection2.SetBindValue(player.oid, nuiToken.Token, new List<NuiComboEntry>());

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

                  selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, true);

                  if (selectedLearnable == currentList.ElementAt(nuiEvent.ArrayIndex))
                    return;

                  selectedLearnable = currentList.ElementAt(nuiEvent.ArrayIndex);
                  selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.name);
                  selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.description);
                  selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.icon);

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

                  LearnableSkill style = player.learnableSkills.Values.FirstOrDefault(s => s is LearnableSkill style && style.category == Category.FightingStyle);
                  if (style is not null)
                    selectedCombatStyle.SetBindValue(player.oid, nuiToken.Token, style.id);

                  InitSelectableSkills();
                  LoadLearnableList(currentList);

                  break;

                case "validate":

                  RemovePreviousClass();

                  validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  validatedLearnableId = selectedLearnable.id;

                  player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CHOSEN_FIGHTER_STYLE").Value = selectedCombatStyle.GetBindValue(player.oid, nuiToken.Token);

                  if (player.learnableSkills.TryAdd(selectedLearnable.id, new LearnableSkill((LearnableSkill)learnableDictionary[selectedLearnable.id], player, (int)Category.Class)))
                    player.learnableSkills[selectedLearnable.id].LevelUp(player);

                  CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, 0, Classes2da.classTable.FirstOrDefault(c => c.classLearnableId == validatedLearnableId).RowIndex);

                  if (player.learnableSkills.TryAdd(selectedSkill1.GetBindValue(player.oid, nuiToken.Token), new LearnableSkill((LearnableSkill)learnableDictionary[selectedSkill1.GetBindValue(player.oid, nuiToken.Token)], player)))
                    player.learnableSkills[selectedSkill1.GetBindValue(player.oid, nuiToken.Token)].LevelUp(player);

                  player.learnableSkills[selectedSkill1.GetBindValue(player.oid, nuiToken.Token)].source.Add(Category.Class);

                  if (player.learnableSkills.TryAdd(selectedSkill2.GetBindValue(player.oid, nuiToken.Token), new LearnableSkill((LearnableSkill)learnableDictionary[selectedSkill2.GetBindValue(player.oid, nuiToken.Token)], player)))
                    player.learnableSkills[selectedSkill2.GetBindValue(player.oid, nuiToken.Token)].LevelUp(player);

                  player.learnableSkills[selectedSkill2.GetBindValue(player.oid, nuiToken.Token)].source.Add(Category.Class);

                  validationText.SetBindValue(player.oid, nuiToken.Token, $"Votre classe est désormais : {selectedLearnable.name}");
                  player.oid.SendServerMessage($"Votre classe initiale est désormais {StringUtils.ToWhitecolor(selectedLearnable.name)} !", ColorConstants.Orange);

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").Delete();
                  LoadLearnableList(currentList);

                  break;

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

                case "race":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introRaceSelector")) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)player.windows["introRaceSelector"]).CreateWindow();

                  break;

                case "histo":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introHistorySelector")) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)player.windows["introHistorySelector"]).CreateWindow();

                  return;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introPortrait")) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)player.windows["introPortrait"]).CreateWindow();

                  break;

                case "stats":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introAbilities")) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)player.windows["introAbilities"]).CreateWindow();

                  return;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "selectedSkill1":
                case "selectedSkill2": LoadSelectableSkills(); break;
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

          player.oid.LoginCreature.RemoveFeat(NwFeat.FromFeatId(CustomSkill.FighterSecondWind));

          foreach (var skill in player.learnableSkills.Where(l => l.Value.source.Any(s => s == SkillSystem.Category.Class)))
            profienciesToRemove.Add(skill.Value);

          foreach (var proficiency in profienciesToRemove)
          {
            if (proficiency.source.Count < 2)
              player.learnableSkills.Remove(proficiency.id);
            else
              proficiency.source.Remove(SkillSystem.Category.Class);
          }
        }
        private void InitSelectableSkills()
        {
          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, false);

          List<NuiComboEntry> skillList1 = new();
          List<NuiComboEntry> skillList2 = new();

          switch (selectedLearnable.id)
          {
            case CustomSkill.Fighter:

              foreach(int skill in Fighter.availableSkills)
              {
                if (!player.learnableSkills.ContainsKey(skill) || player.learnableSkills[skill].source.Any(so => so == Category.Class))
                {
                  skillList1.Add(new NuiComboEntry(learnableDictionary[skill].name, skill));
                  skillList2.Add(new NuiComboEntry(learnableDictionary[skill].name, skill));
                }
              }

              var bonusSkills = player.learnableSkills.Values.Where(s => s.category == Category.Skill && s.source.Any(so => so == Category.Class));

              if(bonusSkills.Count() > 1)
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

              break;
          }

          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void LoadSelectableSkills()
        {
          validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);

          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, false);

          List<NuiComboEntry> skillList1 = new();
          List<NuiComboEntry> skillList2 = new();

          switch (selectedLearnable.id)
          {
            case CustomSkill.Fighter:

              foreach (int skill in Fighter.availableSkills)
              {
                if (!player.learnableSkills.ContainsKey(skill) || player.learnableSkills[skill].source.Any(so => so == Category.Class))
                {
                  skillList1.Add(new NuiComboEntry(learnableDictionary[skill].name, skill));
                  skillList2.Add(new NuiComboEntry(learnableDictionary[skill].name, skill));
                }
              }

              skillList1.Remove(skillList1.First(s => s.Value == selectedSkill2.GetBindValue(player.oid, nuiToken.Token)));
              skillSelection1.SetBindValue(player.oid, nuiToken.Token, skillList1);

              skillList2.Remove(skillList2.First(s => s.Value == selectedSkill1.GetBindValue(player.oid, nuiToken.Token)));
              skillSelection2.SetBindValue(player.oid, nuiToken.Token, skillList2);

              break;
          }

          selectedSkill1.SetBindWatch(player.oid, nuiToken.Token, true);
          selectedSkill2.SetBindWatch(player.oid, nuiToken.Token, true);
        }
      }
    }
  }
}
