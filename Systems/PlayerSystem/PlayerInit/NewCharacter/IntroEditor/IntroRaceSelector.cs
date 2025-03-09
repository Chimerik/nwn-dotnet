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
      public class IntroRaceSelectorWindow : PlayerWindow
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
        private readonly NuiBind<string> raceName = new("raceName");

        private readonly NuiBind<int> selectedBonusSkill = new("selectedBonusSkill");
        private readonly NuiBind<List<NuiComboEntry>> bonusSelectionList = new("bonusSelectionList");

        private IEnumerable<RaceEntry> currentList;
        private NwRace selectedRace;

        public IntroRaceSelectorWindow(Player player) : base(player)
        {
          windowId = "introRaceSelector";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> learnableTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(icon) { Id = "select", Tooltip = raceName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(raceName) { Width = 200, Id = "select", ForegroundColor = color, Encouraged = encouraged, Tooltip = raceName, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 220 },
            new NuiListTemplateCell(new NuiButtonImage("select_right") { Id = "select", Tooltip = raceName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = 90, ForegroundColor = ColorConstants.Gray  },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = 70, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").HasValue },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = 90, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
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
              new NuiRow() { Children = new List<NuiElement>() { new NuiText(selectedItemDescription) { } } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton(validationText) { Id = "validate", Height = 40, Width = player.guiScaledWidth * 0.6f - 370, Enabled = validationEnabled }, new NuiSpacer() } }
            }, Width = player.guiScaledWidth * 0.6f - 370 }
          } });
          
          CreateWindow();
        }
        public void CreateWindow()
        {
          selectedRace = null;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f);
          window = new NuiWindow(rootColumn, "Votre reflet - Choississez votre race")
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
            selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, "Sélectionner une race pour afficher ses détails.\n\nAttention, lorsque vous aurez quitté ce navire, ce choix deviendra définitif.");
            selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, "ir_examine");
            selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, false);

            validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);

            selectedBonusSkill.SetBindValue(player.oid, nuiToken.Token, -1);
            selectedBonusSkill.SetBindWatch(player.oid, nuiToken.Token, true);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            currentList = Races2da.playableRaces;
            LoadRaceList(currentList);
          }
        }
        private void HandleLearnableEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "select":

                  selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, true);

                  if (selectedRace?.Id == currentList.ElementAt(nuiEvent.ArrayIndex).RowIndex)
                    return;

                  selectedRace = NwRace.FromRaceId(currentList.ElementAt(nuiEvent.ArrayIndex).RowIndex);
                  selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, selectedRace.Name);
                  selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, Races2da.raceTable[selectedRace.Id].icon);

                  if (selectedRace.RacialType == RacialType.Human)
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                    validationText.SetBindValue(player.oid, nuiToken.Token, "Valider le choix : Humain");
                             
                  }
                  else if (player.oid.LoginCreature.Race.Id == selectedRace.Id)
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"{selectedRace.Name} déjà sélectionnée");
                  }
                  else
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"Valider le choix : {selectedRace.Name}");
                  }

                  selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, selectedRace.Description);
                  LoadRaceList(currentList);

                  return;

                case "validate":

                  RemovePreviousHistory();

                  validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                  player.ApplyRacePackage(selectedRace, selectedBonusSkill.GetBindValue(player.oid, nuiToken.Token));

                  validationText.SetBindValue(player.oid, nuiToken.Token, $"Vous êtes désormais {selectedRace.Name}");
                  player.oid.SendServerMessage($"La race {StringUtils.ToWhitecolor(selectedRace.Name)} vous a bien été affectée !", ColorConstants.Orange);

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").Delete();
                  LoadRaceList(currentList);

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

                case "histo":

                  CloseWindow();

                  if (!player.windows.ContainsKey("introHistorySelector")) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)player.windows["introHistorySelector"]).CreateWindow();

                  break;

                case "class":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introClassSelector", out var classSel)) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)classSel).CreateWindow();

                  return;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introPortrait", out var portraitSel)) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)portraitSel).CreateWindow();

                  break;

                case "stats":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introAbilities", out var statSel)) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)statSel).CreateWindow();
                  
                  return;
              }

              return;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "selectedBonusSkill":

                  if(selectedBonusSkill.GetBindValue(player.oid, nuiToken.Token) > -1) 
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"Valider le choix : {selectedRace.Name}");
                  }
                  else
                  {
                    validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);
                    validationText.SetBindValue(player.oid, nuiToken.Token, $"{selectedRace.Name} : Veuillez sélectionner une compétence bonus");
                  }
                  
                  return;
              }

              return;
          }
        }
        private void LoadRaceList(IEnumerable<RaceEntry> filteredList)
        {
          List<string> iconList = new();
          List<string> skillNameList = new();
          List<Color> colorList = new();
          List<bool> encouragedList = new();

          foreach (RaceEntry raceEntry in filteredList)
          {
            NwRace race = NwRace.FromRaceId(raceEntry.RowIndex);
            iconList.Add(raceEntry.icon);
            skillNameList.Add(race.Name);
            encouragedList.Add(race.Id == player.oid.LoginCreature.Race.Id);

            if (selectedRace is not null)
              colorList.Add(selectedRace.Id == race.Id ? ColorConstants.White : ColorConstants.Gray);
            else
              colorList.Add(ColorConstants.White);
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          raceName.SetBindValues(player.oid, nuiToken.Token, skillNameList);
          color.SetBindValues(player.oid, nuiToken.Token, colorList);
          encouraged.SetBindValues(player.oid, nuiToken.Token, encouragedList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
        private void RemovePreviousHistory()
        {
          List<LearnableSkill> profienciesToRemove = new();

          foreach (var skill in player.learnableSkills.Values.Where(l => l.source.Any(s => s == SkillSystem.Category.Race)))
            profienciesToRemove.Add(skill);

          foreach (var proficiency in profienciesToRemove)
          {
            if (proficiency.source.Count < 2)
              player.learnableSkills.Remove(proficiency.id);
            else
              proficiency.source.Remove(SkillSystem.Category.Race);
          }

          player.oid.OnCombatStatusChange -= OnCombatEndRestoreDuergarInvisibility;

          foreach (var eff in player.oid.LoginCreature.ActiveEffects)
          {
            switch(eff.Tag)
            {
              case EffectSystem.DwarfPoisonResistanceEffectTag:
              case EffectSystem.ThieflingResistanceEffectTag:
              case EffectSystem.SleepImmunityEffectTag:
              case EffectSystem.woodElfEffectTag:
              case EffectSystem.EnduranceImplacableEffectTag: player.oid.LoginCreature.RemoveEffect(eff); break;
            }
          }

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Delete();
          player.oid.LoginCreature.OnDamaged -= CreatureUtils.HandleImplacableEndurance;

          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.MageHand);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.RayOfFrost);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.AcidSplash);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.ElectricJolt);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.BladeWard);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.FireBolt);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.Friends);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.BoneChill);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.TrueStrike);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.PoisonSpray);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.Light);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.LightDrow);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.ProduceFlame);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.FlameBlade);
          player.oid.LoginCreature.RemoveFeat((Feat)CustomSkill.Thaumaturgy);
          player.oid.LoginCreature.RemoveFeat(Feat.HardinessVersusEnchantments);

          player.oid.LoginCreature.TailType = CreatureTailType.None;
          player.oid.LoginCreature.WingType = CreatureWingType.None;

          if (player.windows.TryGetValue("skillBonusChoice", out var bonusSkill) && bonusSkill.IsOpen)
          {
            bonusSkill.CloseWindow();
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Delete();
            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Delete();
          }
        }
      }
    }
  }
}
