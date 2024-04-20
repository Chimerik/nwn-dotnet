using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class IntroHistorySelectorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> selectedItemTitle = new("selectedItemTitle");
        private readonly NuiBind<string> selectedItemDescription = new("selectedItemDescription");
        private readonly NuiBind<string> selectedItemIcon = new("selectedItemIcon");
        private readonly NuiBind<bool> selectedItemVisibility = new("selectedItemVisibility");

        private readonly NuiBind<bool> scholarBonusVisibility = new("scholarBonusVisibility");
        private readonly NuiBind<int> selectedScholarBonus = new("selectedScholarBonus");
        private readonly NuiBind<List<NuiComboEntry>> scholarBonusList = new("scholarBonusList");

        private readonly NuiBind<string> validationText = new("validationText");
        private readonly NuiBind<bool> validationEnabled = new("validationEnabled");

        private readonly NuiBind<bool> encouraged = new("encouraged");
        private readonly NuiBind<Color> color = new("color");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> skillName = new("skillName");

        private IEnumerable<Learnable> currentList;
        private Learnable selectedLearnable;
        private int validatedLearnableId;

        public IntroHistorySelectorWindow(Player player) : base(player)
        {
          windowId = "introHistorySelector";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> learnableTemplate = new()
          {
            new(new NuiButtonImage(icon) { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new(new NuiLabel(skillName) { Width = 200, Id = "select", ForegroundColor = color, Encouraged = encouraged, Tooltip = skillName, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 220 },
            new(new NuiButtonImage("select_right") { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").HasValue },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").HasValue },
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
                new NuiCombo() { Entries = scholarBonusList, Selected = selectedScholarBonus, Visible = scholarBonusVisibility, Height = 40, Width = (player.guiScaledWidth * 0.6f - 380) / 1.5f },
                new NuiSpacer()
              } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiText(selectedItemDescription) {  } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton(validationText) { Id = "validate", Height = 40, Width = player.guiScaledWidth * 0.6f - 370, Enabled = validationEnabled }, new NuiSpacer() } }
            }, Width = player.guiScaledWidth * 0.6f - 370 }
          } });

          CreateWindow();
        }
        public void CreateWindow()
        {

          selectedLearnable = null;

          validatedLearnableId = player.learnableSkills.Any(l => l.Value.category == SkillSystem.Category.StartingTraits)
            ? player.learnableSkills.FirstOrDefault(l => l.Value.category == SkillSystem.Category.StartingTraits).Value.id
            : -1;

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f);
          window = new NuiWindow(rootColumn, "Votre reflet - Choisissez votre origine")
          {
            Geometry = geometry,
            Closable = true,
            Border = true,
            Resizable = false,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, "");
            selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, "Sélectionner une origine pour afficher ses détails.\n\nAttention, lorsque vous aurez quitté ce navire, ce choix deviendra définitif.");
            selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, "ir_examine");
            selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, false);
            validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            currentList = learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.StartingTraits).OrderBy(s => s.name);
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
                  selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.icon);

                  if (selectedLearnable.id == CustomSkill.CloisteredScholar)
                  {
                    List<NuiComboEntry> bonusList = new();

                    if (!player.learnableSkills.ContainsKey(CustomSkill.ArcanaProficiency))
                      bonusList.Add(new("Maîtrise - Arcanes", CustomSkill.ArcanaProficiency));

                    if (!player.learnableSkills.ContainsKey(CustomSkill.NatureProficiency))
                      bonusList.Add(new("Maîtrise - Nature", CustomSkill.NatureProficiency));

                    if (!player.learnableSkills.ContainsKey(CustomSkill.ReligionProficiency))
                      bonusList.Add(new("Maîtrise - Religion", CustomSkill.ReligionProficiency));

                    scholarBonusList.SetBindValue(player.oid, nuiToken.Token, bonusList);
                    selectedScholarBonus.SetBindValue(player.oid, nuiToken.Token, bonusList.FirstOrDefault().Value);

                    scholarBonusVisibility.SetBindValue(player.oid, nuiToken.Token, true);
                  }
                  else
                    scholarBonusVisibility.SetBindValue(player.oid, nuiToken.Token, false);

                  if (player.learnableSkills.ContainsKey(selectedLearnable.id))
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"Origine {selectedLearnable.name} déjà sélectionnée");
                  }
                  else
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"Valider l'origine {selectedLearnable.name}");
                  }

                  selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, selectedLearnable.description);
                  LoadLearnableList(currentList);

                  return;

                case "validate":
  
                  RemovePreviousHistory();

                  validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  validatedLearnableId = selectedLearnable.id;

                  if (selectedLearnable.id == CustomSkill.CloisteredScholar)
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_SELECTED_SCHOLAR_BONUS").Value = selectedScholarBonus.GetBindValue(player.oid, nuiToken.Token);

                  player.learnableSkills.TryAdd(selectedLearnable.id, new LearnableSkill((LearnableSkill)learnableDictionary[selectedLearnable.id], player, (int)Category.StartingTraits));
                  player.learnableSkills[selectedLearnable.id].LevelUp(player);

                  validationText.SetBindValue(player.oid, nuiToken.Token, $"Votre origine est désormais : {selectedLearnable.name}");
                  player.oid.SendServerMessage($"L'origine {StringUtils.ToWhitecolor(selectedLearnable.name)} vous a bien été affectée !", ColorConstants.Orange);

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").Delete();
                  LoadLearnableList(currentList);

                  return;

                case "welcome":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyAppearanceModifier")) player.windows.Add("bodyAppearanceModifier", new IntroMirrorWindow(player));
                  else ((IntroMirrorWindow)player.windows["bodyAppearanceModifier"]).CreateWindow();

                  return;

                case "beauty":

                  CloseWindow();

                  if (!player.windows.ContainsKey("bodyColorsModifier")) player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, player.oid.LoginCreature));
                  else ((BodyColorWindow)player.windows["bodyColorsModifier"]).CreateWindow(player.oid.LoginCreature);

                  return;

                case "race":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introRaceSelector")) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)player.windows["introRaceSelector"]).CreateWindow();

                  break;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introPortrait", out var value)) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)value).CreateWindow();

                  break;

                case "class":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introClassSelector")) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)player.windows["introClassSelector"]).CreateWindow();
                  
                  return;

                case "stats":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introAbilities")) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)player.windows["introAbilities"]).CreateWindow();

                  return;
              }

              return;
          }
        }
        private void LoadLearnableList(IEnumerable<Learnable> filteredList)
        {
          List<string> iconList = new();
          List<string> skillNameList = new();
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
        private void RemovePreviousHistory()  
        {
          List<LearnableSkill> profienciesToRemove = new();

          foreach (var skill in player.learnableSkills.Values.Where(l => l.source.Any(s => s == SkillSystem.Category.StartingTraits)))
            profienciesToRemove.Add(skill);

          foreach (var proficiency in profienciesToRemove)
          { 
            if (proficiency.source.Count < 2)
              player.learnableSkills.Remove(proficiency.id);
            else
              proficiency.source.Remove(SkillSystem.Category.StartingTraits);
          }
        }
      }
    }
  }
}
