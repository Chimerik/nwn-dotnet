using System.Collections.Generic;
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

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiLabel("Choisissez un maximum de quatre armes") { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle } } });
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
              case BaseItemType.MagicStaff: 
              case BaseItemType.Kukri: break;
              default:

                if(baseItem.DieToRoll > 0)
                {
                  List<int> proficenciesRequirements = ItemUtils.GetItemProficiencies(baseItem.ItemType);

                  foreach (int requiredProficiency in proficenciesRequirements)
                    if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(requiredProficiency))
                      || player.learnableSkills.TryGetValue(requiredProficiency, out LearnableSkill proficiency) && proficiency.currentLevel < 1)
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

                  var checkedList = weaponChecked.GetBindValues(player.oid, nuiToken.Token);
                  List<int> toLearn = new();

                  for (int i = 0; i < checkedList.Count; i++)
                    if (checkedList[i])
                      toLearn.Add(i);

                  if (toLearn.Count > 4)
                  {
                    player.oid.SendServerMessage($"Veuillez sélectionner jusqu'à 4 maîtrises maximum");
                    return;
                  }

                  if (toLearn.Count > 3 || weaponProficiencies.Count == toLearn.Count || weaponProficiencies.Count < 1)
                  {
                    CloseWindow();

                    foreach (int weapon in toLearn)
                    {
                      switch (NwBaseItem.FromItemId(weaponProficiencies[weapon])?.ItemType)
                      {
                        case BaseItemType.Club:
                          player.learnableSkills.TryAdd(CustomSkill.ClubProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ClubProficiency], player));
                          player.learnableSkills[CustomSkill.ClubProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Dagger:
                          player.learnableSkills.TryAdd(CustomSkill.DaggerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DaggerProficiency], player));
                          player.learnableSkills[CustomSkill.DaggerProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Handaxe:
                          player.learnableSkills.TryAdd(CustomSkill.HandAxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HandAxeProficiency], player));
                          player.learnableSkills[CustomSkill.HandAxeProficiency].LevelUp(player);
                          break;
                        case BaseItemType.LightHammer:
                          player.learnableSkills.TryAdd(CustomSkill.LightHammerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightHammerProficiency], player));
                          player.learnableSkills[CustomSkill.LightHammerProficiency].LevelUp(player);
                          break;
                        case BaseItemType.LightMace:
                          player.learnableSkills.TryAdd(CustomSkill.LightMaceProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightMaceProficiency], player));
                          player.learnableSkills[CustomSkill.LightMaceProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Quarterstaff:
                        case BaseItemType.MagicStaff:
                          player.learnableSkills.TryAdd(CustomSkill.QuarterstaffProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.QuarterstaffProficiency], player));
                          player.learnableSkills[CustomSkill.QuarterstaffProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Sickle:
                          player.learnableSkills.TryAdd(CustomSkill.SickleProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SickleProficiency], player));
                          player.learnableSkills[CustomSkill.SickleProficiency].LevelUp(player);
                          break;
                        case BaseItemType.LightCrossbow:
                          player.learnableSkills.TryAdd(CustomSkill.LightCrossbowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightCrossbowProficiency], player));
                          player.learnableSkills[CustomSkill.LightCrossbowProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Dart:
                          player.learnableSkills.TryAdd(CustomSkill.DartProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DartProficiency], player));
                          player.learnableSkills[CustomSkill.DartProficiency].LevelUp(player);
                          break;
                        case BaseItemType.LightFlail:
                          player.learnableSkills.TryAdd(CustomSkill.LightFlailProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightFlailProficiency], player));
                          player.learnableSkills[CustomSkill.LightFlailProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Morningstar:
                          player.learnableSkills.TryAdd(CustomSkill.MorningstarProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.MorningstarProficiency], player));
                          player.learnableSkills[CustomSkill.MorningstarProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Sling:
                          player.learnableSkills.TryAdd(CustomSkill.SlingProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SlingProficiency], player));
                          player.learnableSkills[CustomSkill.SlingProficiency].LevelUp(player);
                          break;
                        case BaseItemType.ShortSpear:
                          player.learnableSkills.TryAdd(CustomSkill.SpearProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SpearProficiency], player));
                          player.learnableSkills[CustomSkill.SpearProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Shortbow:
                          player.learnableSkills.TryAdd(CustomSkill.ShortBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortBowProficiency], player));
                          player.learnableSkills[CustomSkill.ShortBowProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Battleaxe:
                          player.learnableSkills.TryAdd(CustomSkill.BattleaxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.BattleaxeProficiency], player));
                          player.learnableSkills[CustomSkill.BattleaxeProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Greataxe:
                          player.learnableSkills.TryAdd(CustomSkill.GreataxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.GreataxeProficiency], player));
                          player.learnableSkills[CustomSkill.GreataxeProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Greatsword:
                          player.learnableSkills.TryAdd(CustomSkill.GreatswordProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.GreatswordProficiency], player));
                          player.learnableSkills[CustomSkill.GreatswordProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Scimitar:
                          player.learnableSkills.TryAdd(CustomSkill.ScimitarProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ScimitarProficiency], player));
                          player.learnableSkills[CustomSkill.ScimitarProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Halberd:
                          player.learnableSkills.TryAdd(CustomSkill.HalberdProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HalberdProficiency], player));
                          player.learnableSkills[CustomSkill.HalberdProficiency].LevelUp(player);
                          break;
                        case BaseItemType.HeavyFlail:
                          player.learnableSkills.TryAdd(CustomSkill.HeavyFlailProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HeavyFlailProficiency], player));
                          player.learnableSkills[CustomSkill.HeavyFlailProficiency].LevelUp(player);
                          break;
                        case BaseItemType.ThrowingAxe:
                          player.learnableSkills.TryAdd(CustomSkill.ThrowingAxeProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ThrowingAxeProficiency], player));
                          player.learnableSkills[CustomSkill.ThrowingAxeProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Warhammer:
                          player.learnableSkills.TryAdd(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.WarHammerProficiency], player));
                          player.learnableSkills[CustomSkill.WarHammerProficiency].LevelUp(player);
                          break;
                        case BaseItemType.HeavyCrossbow:
                          player.learnableSkills.TryAdd(CustomSkill.HeavyCrossbowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.HeavyCrossbowProficiency], player));
                          player.learnableSkills[CustomSkill.HeavyCrossbowProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Rapier:
                          player.learnableSkills.TryAdd(CustomSkill.RapierProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.RapierProficiency], player));
                          player.learnableSkills[CustomSkill.RapierProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Shortsword:
                          player.learnableSkills.TryAdd(CustomSkill.ShortSwordProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortSwordProficiency], player));
                          player.learnableSkills[CustomSkill.ShortSwordProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Longsword:
                          player.learnableSkills.TryAdd(CustomSkill.LongSwordProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LongSwordProficiency], player));
                          player.learnableSkills[CustomSkill.LongSwordProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Longbow:
                          player.learnableSkills.TryAdd(CustomSkill.LongBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LongBowProficiency], player));
                          player.learnableSkills[CustomSkill.LongBowProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Shuriken:
                          player.learnableSkills.TryAdd(CustomSkill.ShurikenProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShurikenProficiency], player));
                          player.learnableSkills[CustomSkill.ShurikenProficiency].LevelUp(player);
                          break;
                        case BaseItemType.Whip:
                          player.learnableSkills.TryAdd(CustomSkill.WhipProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.WhipProficiency], player));
                          player.learnableSkills[CustomSkill.WhipProficiency].LevelUp(player);
                          break;
                      }
                    }

                    player.oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

                    List<NuiComboEntry> abilities = new();

                    if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
                      abilities.Add(new("Force", (int)Ability.Strength));

                    if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
                      abilities.Add(new("Dextérité", (int)Ability.Dexterity));

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

          foreach (var weapon in weaponProficiencies)
          {
            weaponNameList.Add(NwBaseItem.FromItemId(weapon).Name.ToString());
            weaponCheckList.Add(weaponCheckBinding is not null && weaponCheckBinding[i]);
          }

          weaponName.SetBindValues(player.oid, nuiToken.Token, weaponNameList);
          weaponChecked.SetBindValues(player.oid, nuiToken.Token, weaponCheckList);
          rowCount.SetBindValue(player.oid, nuiToken.Token, weaponProficiencies.Count);
        }
      }
    }
  }
}
