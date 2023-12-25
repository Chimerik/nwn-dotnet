﻿using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class WeaponBonusChoiceWindow : PlayerWindow
      {
        private readonly List<bool> selection = new();
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> weaponName = new("weaponName");
        private readonly NuiBind<bool> weaponChecked = new("weaponChecked");
        private readonly NuiBind<int> rowCount = new("rowCount");

        private readonly List<int> weaponProficiencies = new();
        private int weaponsChecked;

        public WeaponBonusChoiceWindow(Player player) : base(player)
        {
          windowId = "weaponBonusChoice";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiSpacer()),
            new(new NuiLabel(weaponName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 120 },
            new(new NuiCheck("", weaponChecked) { Tooltip = "Apprendre la maîtrise de cette arme", Margin = 0.0f }) { Width = 40 },
            new(new NuiSpacer())
          };

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiLabel("Attribuez +1 à une caractéristique de cette liste (max 20)") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, rowCount) { RowHeight = 40, Scrollbars = NuiScrollbars.None } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80 }, new NuiSpacer() } });
          
          CreateWindow();
        }
        public void CreateWindow()
        {
          weaponProficiencies.Clear();

          foreach (var entry in BaseItems2da.baseItemTable)
          {
            NwBaseItem baseItem = NwBaseItem.FromItemId(entry.RowIndex);

            switch (baseItem?.ItemType)
            {
              case BaseItemType.Bastardsword:
              case BaseItemType.Scythe:
              case BaseItemType.DireMace: 
              case BaseItemType.Doubleaxe: 
              case BaseItemType.DwarvenWaraxe: 
              case BaseItemType.TwoBladedSword:
              case BaseItemType.Kama:
              case BaseItemType.Katana: 
              case BaseItemType.Kukri: break;
              default:

                if(baseItem.DieToRoll > 0)
                {
                  List<int> proficenciesRequirements = ItemUtils.GetItemProficiencies(baseItem.ItemType);

                  foreach (int requiredProficiency in proficenciesRequirements)
                    if (player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(requiredProficiency))
                      || player.learnableSkills.TryGetValue(requiredProficiency, out LearnableSkill proficiency) && proficiency.currentLevel > 0)
                      weaponProficiencies.Add(entry.RowIndex);
                }

                break;
            }
          }

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_WEAPON_MASTER_CHOICE_FEAT").Value = 1;

          NuiRect savedRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(player.guiScaledWidth * 0.4f, player.guiHeight * 0.15f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Don - Maître d'arme - Choisissez 4 maîtrises")
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
            nuiToken.OnNuiEvent += HandleWeaponMasterEvents;

            LoadWeaponList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleWeaponMasterEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  if (weaponsChecked > 3 || weaponProficiencies.Count == weaponsChecked || weaponProficiencies.Count < 1)
                  {
                    CloseWindow();

                    var checkedList = weaponChecked.GetBindValues(player.oid, nuiToken.Token);

                    for (int i = 0; i < checkedList.Count; i++)
                    {
                      if (checkedList[i])
                      {
                        switch(NwBaseItem.FromItemId(weaponProficiencies[i])?.ItemType)
                        {
                          case BaseItemType.Club:
                            player.learnableSkills.Add(CustomSkill.ClubProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ClubProficiency]));
                            player.learnableSkills[CustomSkill.ClubProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Dagger:
                            player.learnableSkills.Add(CustomSkill.DaggerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DaggerProficiency]));
                            player.learnableSkills[CustomSkill.DaggerProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Handaxe:
                            player.learnableSkills.Add(CustomSkill.HandAxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HandAxeProficiency]));
                            player.learnableSkills[CustomSkill.HandAxeProficiency].LevelUp(player);
                            break;
                          case BaseItemType.LightHammer:
                            player.learnableSkills.Add(CustomSkill.LightHammerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightHammerProficiency]));
                            player.learnableSkills[CustomSkill.LightHammerProficiency].LevelUp(player);
                            break;
                          case BaseItemType.LightMace:
                            player.learnableSkills.Add(CustomSkill.LightMaceProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightMaceProficiency]));
                            player.learnableSkills[CustomSkill.LightMaceProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Quarterstaff:
                            player.learnableSkills.Add(CustomSkill.QuarterstaffProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.QuarterstaffProficiency]));
                            player.learnableSkills[CustomSkill.QuarterstaffProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Sickle:
                            player.learnableSkills.Add(CustomSkill.SickleProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SickleProficiency]));
                            player.learnableSkills[CustomSkill.SickleProficiency].LevelUp(player);
                            break;
                          case BaseItemType.LightCrossbow:
                            player.learnableSkills.Add(CustomSkill.LightCrossbowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightCrossbowProficiency]));
                            player.learnableSkills[CustomSkill.LightCrossbowProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Dart:
                            player.learnableSkills.Add(CustomSkill.DartProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DartProficiency]));
                            player.learnableSkills[CustomSkill.DartProficiency].LevelUp(player);
                            break;
                          case BaseItemType.MagicStaff:
                            player.learnableSkills.Add(CustomSkill.MagicStaffProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.MagicStaffProficiency]));
                            player.learnableSkills[CustomSkill.MagicStaffProficiency].LevelUp(player);
                            break;
                          case BaseItemType.LightFlail:
                            player.learnableSkills.Add(CustomSkill.LightFlailProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightFlailProficiency]));
                            player.learnableSkills[CustomSkill.LightFlailProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Morningstar:
                            player.learnableSkills.Add(CustomSkill.MorningstarProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.MorningstarProficiency]));
                            player.learnableSkills[CustomSkill.MorningstarProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Sling:
                            player.learnableSkills.Add(CustomSkill.SlingProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SlingProficiency]));
                            player.learnableSkills[CustomSkill.SlingProficiency].LevelUp(player);
                            break;
                          case BaseItemType.ShortSpear:
                            player.learnableSkills.Add(CustomSkill.SpearProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SpearProficiency]));
                            player.learnableSkills[CustomSkill.SpearProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Shortbow:
                            player.learnableSkills.Add(CustomSkill.ShortBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortBowProficiency]));
                            player.learnableSkills[CustomSkill.ShortBowProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Battleaxe:
                            player.learnableSkills.Add(CustomSkill.BattleaxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.BattleaxeProficiency]));
                            player.learnableSkills[CustomSkill.BattleaxeProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Greataxe:
                            player.learnableSkills.Add(CustomSkill.GreataxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.GreataxeProficiency]));
                            player.learnableSkills[CustomSkill.GreataxeProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Greatsword:
                            player.learnableSkills.Add(CustomSkill.GreatswordProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.GreatswordProficiency]));
                            player.learnableSkills[CustomSkill.GreatswordProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Scimitar:
                            player.learnableSkills.Add(CustomSkill.ScimitarProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ScimitarProficiency]));
                            player.learnableSkills[CustomSkill.ScimitarProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Halberd:
                            player.learnableSkills.Add(CustomSkill.HalberdProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HalberdProficiency]));
                            player.learnableSkills[CustomSkill.HalberdProficiency].LevelUp(player);
                            break;
                          case BaseItemType.HeavyFlail:
                            player.learnableSkills.Add(CustomSkill.HeavyFlailProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HeavyFlailProficiency]));
                            player.learnableSkills[CustomSkill.HeavyFlailProficiency].LevelUp(player);
                            break;
                          case BaseItemType.ThrowingAxe:
                            player.learnableSkills.Add(CustomSkill.ThrowingAxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ThrowingAxeProficiency]));
                            player.learnableSkills[CustomSkill.ThrowingAxeProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Trident:
                            player.learnableSkills.Add(CustomSkill.TridentProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.TridentProficiency]));
                            player.learnableSkills[CustomSkill.TridentProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Warhammer:
                            player.learnableSkills.Add(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.WarHammerProficiency]));
                            player.learnableSkills[CustomSkill.WarHammerProficiency].LevelUp(player);
                            break;
                          case BaseItemType.HeavyCrossbow:
                            player.learnableSkills.Add(CustomSkill.HeavyCrossbowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HeavyCrossbowProficiency]));
                            player.learnableSkills[CustomSkill.HeavyCrossbowProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Rapier:
                            player.learnableSkills.Add(CustomSkill.RapierProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.RapierProficiency]));
                            player.learnableSkills[CustomSkill.RapierProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Shortsword:
                            player.learnableSkills.Add(CustomSkill.ShortSwordProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortSwordProficiency]));
                            player.learnableSkills[CustomSkill.ShortSwordProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Longsword:
                            player.learnableSkills.Add(CustomSkill.LongSwordProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LongSwordProficiency]));
                            player.learnableSkills[CustomSkill.LongSwordProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Longbow:
                            player.learnableSkills.Add(CustomSkill.LongBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LongBowProficiency]));
                            player.learnableSkills[CustomSkill.LongBowProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Shuriken:
                            player.learnableSkills.Add(CustomSkill.ShurikenProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShurikenProficiency]));
                            player.learnableSkills[CustomSkill.ShurikenProficiency].LevelUp(player);
                            break;
                          case BaseItemType.Whip:
                            player.learnableSkills.Add(CustomSkill.WhipProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.WhipProficiency]));
                            player.learnableSkills[CustomSkill.WhipProficiency].LevelUp(player);
                            break;
                        }
                      }
                    }

                    player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

                    List<Ability> abilities = new();

                    if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
                      abilities.Add(Ability.Strength);

                    if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
                      abilities.Add(Ability.Dexterity);

                    if (abilities.Count > 0)
                    {
                      if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
                      else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
                    }
                  }
                  else
                  {
                    int nbMaitrises = weaponProficiencies.Count > 3 ? 4 : weaponProficiencies.Count;
                    player.oid.SendServerMessage($"Veuillez sélectionner jusqu'à {nbMaitrises} maîtrises avant de valider");
                  }

                  return;
              }

              break;
          }
        }
        private void LoadWeaponList()
        {
          List<string> weaponNameList = new();
          List<bool> weaponCheckList = new();
          List<bool> weaponCheckBinding = weaponChecked?.GetBindValues(player.oid, nuiToken.Token);
          int i = 0;
          weaponsChecked = 0;

          foreach (var weapon in weaponProficiencies)
          {
            weaponNameList.Add(NwBaseItem.FromItemId(weapon).Name.ToString());
            if (weaponCheckBinding is not null && weaponCheckBinding[i])
            {
              weaponsChecked++;
              weaponCheckList.Add(true);
            }
          }

          weaponName.SetBindValues(player.oid, nuiToken.Token, weaponNameList);
          weaponChecked.SetBindValues(player.oid, nuiToken.Token, weaponCheckList);
          rowCount.SetBindValue(player.oid, nuiToken.Token, weaponProficiencies.Count);
        }
      }
    }
  }
}
