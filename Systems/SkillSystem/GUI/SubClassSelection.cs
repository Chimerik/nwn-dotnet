﻿using System.Collections.Generic;
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
      public class SubClassSelectionWindow : PlayerWindow
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

        private IEnumerable<Learnable> currentList;
        private Learnable selectedLearnable;

        public SubClassSelectionWindow(Player player) : base(player)
        {
          windowId = "subClassSelection";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> learnableTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiButtonImage(icon) { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new(new NuiLabel(skillName) { Width = 200, Id = "select", ForegroundColor = color, Encouraged = encouraged, Tooltip = skillName, HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 220 },
            new(new NuiButtonImage("select_right") { Id = "select", Tooltip = skillName, Encouraged = encouraged, Height = 40, Width = 40 }) { Width = 40 },
            new(new NuiSpacer())
          };

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
              new NuiRow() { Children = new List<NuiElement>() { new NuiText(selectedItemDescription) } },
              new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton(validationText) { Id = "validate", Height = 40, Width = player.guiScaledWidth * 0.6f - 370, Enabled = validationEnabled }, new NuiSpacer() } }
            }, Width = player.guiScaledWidth * 0.6f - 370 }
          } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiWidth * 0.2f, player.guiHeight * 0.05f, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f);
          selectedLearnable = null;

          window = new NuiWindow(rootColumn, "Choisissez une voie")
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
            nuiToken.OnNuiEvent += HandleLearnableEvents;

            selectedItemTitle.SetBindValue(player.oid, nuiToken.Token, "");
            selectedItemDescription.SetBindValue(player.oid, nuiToken.Token, "Sélectionner une voie pour afficher ses détails.\n\nAttention, le choix est définitif.");
            selectedItemIcon.SetBindValue(player.oid, nuiToken.Token, "ir_examine");
            selectedItemVisibility.SetBindValue(player.oid, nuiToken.Token, false);
            validationEnabled.SetBindValue(player.oid, nuiToken.Token, false);

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.6f, player.guiScaledHeight * 0.9f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            currentList = LoadSubClassList();

            if(currentList is null)
            {
              CloseWindow();
              return;
            }

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

                  validationEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  validationText.SetBindValue(player.oid, nuiToken.Token, $"Valider la voie {selectedLearnable.name}");

                  LoadLearnableList(currentList);

                  break;

                case "validate":

                  if (player.learnableSkills.TryAdd(selectedLearnable.id, new LearnableSkill((LearnableSkill)learnableDictionary[selectedLearnable.id], player, levelTaken: player.oid.LoginCreature.Level)))
                  {
                    player.learnableSkills[selectedLearnable.id].currentLevel = 3;
                    player.learnableSkills[selectedLearnable.id].acquiredPoints = 114640;
                  }

                  Category category = Category.FighterSubClass;

                  switch(selectedLearnable.id)
                  {
                    case CustomSkill.FighterArcaneArcher: Fighter.HandleArcherMageLevelUp(player, 3); break;
                    case CustomSkill.FighterWarMaster: Fighter.HandleWarMasterLevelUp(player, 3); break;
                    case CustomSkill.FighterChampion: Fighter.HandleChampionLevelUp(player, 3); break;
                    case CustomSkill.FighterEldritchKnight: Fighter.HandleEldritchKnightLevelUp(player, 3); break;
                    case CustomSkill.BarbarianBerseker: 
                      Barbarian.HandleBersekerLevelUp(player, 3);
                      category = Category.BarbarianSubClass;
                      break;
                    case CustomSkill.BarbarianTotem: 
                      Barbarian.HandleTotemLevelUp(player, 3);
                      category = Category.BarbarianSubClass; 
                      break;
                    case CustomSkill.BarbarianWildMagic: 
                      Barbarian.HandleWildMagicLevelUp(player, 3);
                      category = Category.BarbarianSubClass; 
                      break;
                    case CustomSkill.RogueThief: 
                      Rogue.HandleThiefLevelUp(player, 3);
                      category = Category.RogueSubClass; 
                      break;
                    case CustomSkill.RogueConspirateur: 
                      Rogue.HandleConspirateurLevelUp(player, 3);
                      category = Category.RogueSubClass; 
                      break;
                    case CustomSkill.RogueAssassin: 
                      Rogue.HandleAssassinLevelUp(player, 3);
                      category = Category.RogueSubClass; 
                      break;
                    case CustomSkill.RogueArcaneTrickster:
                      Rogue.HandleArcaneTricksterLevelUp(player, 3);
                      category = Category.RogueSubClass;
                      break;
                    case CustomSkill.MonkPaume: 
                      Monk.HandlePaumeLevelUp(player, 3);
                      category = Category.MonkSubClass; 
                      break;
                    case CustomSkill.MonkOmbre: 
                      Monk.HandleOmbreLevelUp(player, 3);
                      category = Category.MonkSubClass; 
                      break;
                    case CustomSkill.MonkElements:
                      Monk.HandleElementsLevelUp(player, 3);
                      category = Category.MonkSubClass;
                      break;
                    case CustomSkill.WizardAbjuration:
                      Wizard.HandleAbjurationLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.WizardDivination:
                      Wizard.HandleDivinationLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.WizardEnchantement:
                      Wizard.HandleEnchantementLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.WizardEvocation:
                      Wizard.HandleEvocationLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.WizardIllusion:
                      Wizard.HandleIllusionLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.WizardInvocation:
                      Wizard.HandleInvocationLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.WizardNecromancie:
                      Wizard.HandleNecromancieLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.WizardTransmutation:
                      Wizard.HandleTransmutationLevelUp(player, 3);
                      category = Category.WizardSubClass;
                      break;
                    case CustomSkill.BardCollegeDuSavoir:
                      Bard.HandleCollegeDuSavoirLevelUp(player, 3);
                      category = Category.BardSubClass;
                      break;
                    case CustomSkill.BardCollegeDeLaVaillance:
                      Bard.HandleCollegeDeLaVaillanceLevelUp(player, 3);
                      category = Category.BardSubClass;
                      break;
                    case CustomSkill.BardCollegeDeLescrime:
                      Bard.HandleCollegeDeLescrimeLevelUp(player, 3);
                      category = Category.BardSubClass;
                      break;
                    case CustomSkill.RangerChasseur:
                      Ranger.HandleChasseurLevelUp(player, 3);
                      category = Category.RangerSubClass;
                      break;
                    case CustomSkill.RangerBelluaire:
                      Ranger.HandleBelluaireLevelUp(player, 3);
                      category = Category.RangerSubClass;
                      break;
                    case CustomSkill.RangerProfondeurs:
                      Ranger.HandleProfondeursLevelUp(player, 3);
                      category = Category.RangerSubClass;
                      break;
                    case CustomSkill.PaladinSermentDevotion:
                      Paladin.HandleDevotionLevelUp(player, 3);
                      category = Category.PaladinSubClass;
                      break;
                    case CustomSkill.PaladinSermentDesAnciens:
                      Paladin.HandleAnciensLevelUp(player, 3);
                      category = Category.PaladinSubClass;
                      break;
                    case CustomSkill.PaladinSermentVengeance:
                      Paladin.HandleVengeanceLevelUp(player, 3);
                      category = Category.PaladinSubClass;
                      break;
                    case CustomSkill.ClercDuperie:
                      Clerc.HandleDuperieLevelUp(player, 3);
                      category = Category.ClercSubClass;
                      break;
                    case CustomSkill.ClercGuerre:
                      Clerc.HandleGuerreLevelUp(player, 3);
                      category = Category.ClercSubClass;
                      break;
                    case CustomSkill.ClercLumiere:
                      Clerc.HandleLumiereLevelUp(player, 3);
                      category = Category.ClercSubClass;
                      break;
                    case CustomSkill.ClercNature:
                      Clerc.HandleNatureLevelUp(player, 3);
                      category = Category.ClercSubClass;
                      break;
                    case CustomSkill.ClercSavoir:
                      Clerc.HandleSavoirLevelUp(player, 3);
                      category = Category.ClercSubClass;
                      break;
                    case CustomSkill.ClercTempete:
                      Clerc.HandleTempeteLevelUp(player, 3);
                      category = Category.ClercSubClass;
                      break;
                    case CustomSkill.ClercVie:
                      Clerc.HandleVieLevelUp(player, 3);
                      category = Category.ClercSubClass;
                      break;
                    case CustomSkill.EnsorceleurLigneeDraconique:
                      Ensorceleur.HandleDraconiqueLevelUp(player, 3);
                      category = Category.EnsorceleurSubClass;
                      break;
                    case CustomSkill.EnsorceleurTempete:
                      Ensorceleur.HandleTempeteLevelUp(player, 3);
                      category = Category.EnsorceleurSubClass;
                      break;
                    case CustomSkill.DruideCercleTellurique:
                      Druide.HandleCercleTerreLevelUp(player, 3);
                      category = Category.DruidSubclass;
                      break;
                    case CustomSkill.DruideCercleSelenite:
                      Druide.HandleCercleSeleniteLevelUp(player, 3);
                      category = Category.DruidSubclass;
                      break;
                    case CustomSkill.DruideCerclePelagique:
                      Druide.HandleCerclePelagiqueLevelUp(player, 3);
                      category = Category.DruidSubclass;
                      break;
                    case CustomSkill.OccultisteArchifee:
                      Occultiste.HandleArchifeeLevelUp(player, 3);
                      category = Category.OccultisteSubClass;
                      break;
                    case CustomSkill.OccultisteCeleste:
                      Occultiste.HandleCelesteLevelUp(player, 3);
                      category = Category.OccultisteSubClass;
                      break;
                    case CustomSkill.OccultisteFielon:
                      Occultiste.HandleFielonLevelUp(player, 3);
                      category = Category.OccultisteSubClass;
                      break;
                    case CustomSkill.OccultisteGrandAncien:
                      Occultiste.HandleGrandAncienLevelUp(player, 3);
                      category = Category.OccultisteSubClass;
                      break;
                  }

                  player.learnableSkills.Remove(player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value);
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Delete();
                  player.oid.SendServerMessage($"Vous adoptez la voie {StringUtils.ToWhitecolor(selectedLearnable.name)} !", ColorConstants.Orange);

                  if (player.TryGetOpenedWindow("learnables", out PlayerWindow learnableWindow))
                    ((LearnableWindow)learnableWindow).RefreshCategories(category);

                  CloseWindow();

                  break;
              }

              break;
          }
        }
        private IEnumerable<Learnable> LoadSubClassList()
        {
          return player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SUBCLASS_SELECTION").Value switch
          {
            CustomSkill.Fighter => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.FighterSubClass),
            CustomSkill.Barbarian => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.BarbarianSubClass),
            CustomSkill.Rogue => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.RogueSubClass),
            CustomSkill.Monk => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.MonkSubClass),
            CustomSkill.Wizard => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.WizardSubClass),
            CustomSkill.Bard => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.BardSubClass),
            CustomSkill.Ranger => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.RangerSubClass),
            CustomSkill.Paladin => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.PaladinSubClass),
            CustomSkill.Clerc => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.ClercSubClass),
            CustomSkill.Ensorceleur => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.EnsorceleurSubClass),
            CustomSkill.Druide => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.DruidSubclass),
            CustomSkill.Occultiste => learnableDictionary.Values.Where(s => s is LearnableSkill ls && ls.category == Category.OccultisteSubClass),
            _ => null,
          };
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

            if (selectedLearnable is not null)
            {
              encouragedList.Add(selectedLearnable == learnable);
              colorList.Add(selectedLearnable == learnable ? ColorConstants.White : ColorConstants.Gray);
            }
            else
            {
              colorList.Add(ColorConstants.White);
              encouragedList.Add(false);
            }
          }

          icon.SetBindValues(player.oid, nuiToken.Token, iconList);
          skillName.SetBindValues(player.oid, nuiToken.Token, skillNameList);
          color.SetBindValues(player.oid, nuiToken.Token, colorList);
          encouraged.SetBindValues(player.oid, nuiToken.Token, encouragedList);
          listCount.SetBindValue(player.oid, nuiToken.Token, filteredList.Count());
        }
      }
    }
  }
}
