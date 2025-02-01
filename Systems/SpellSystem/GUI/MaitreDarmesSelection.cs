using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MaitreDarmesSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();

        private readonly NuiBind<string> availableTechIcons = new("availableTechIcons");
        private readonly NuiBind<string> availableTechNames = new("availableTechNames");
        private readonly NuiBind<string> acquiredTechIcons = new("acquiredTechIcons");
        private readonly NuiBind<string> acquiredTechNames = new("acquiredTechNames");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<int> listAcquiredTechCount = new("listAcquiredTechCount");

        private readonly NuiBind<bool> enabled = new("enabled");

        private readonly List<int> availableTechs = new();
        private readonly List<int> acquiredTechs = new();

        public MaitreDarmesSelectionWindow(Player player) : base(player)
        {
          windowId = "maitreDarmesSelection";

          List<NuiListTemplateCell> rowTemplateAvailableTechs = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(availableTechIcons) { Tooltip = availableTechNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(availableTechNames) { Tooltip = availableTechNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("add_arrow") { Id = "selectTech", Tooltip = "Sélectionner", DisabledTooltip = "Vous ne pouvez pas choisir davantage de techniques" }) { Width = 35 }
          };

          List<NuiListTemplateCell> rowTemplateAcquiredTechs = new()
          {
            new NuiListTemplateCell(new NuiButtonImage("remove_arrow") { Id = "removeTech", Tooltip = "Retirer" }) { Width = 35 },
            new NuiListTemplateCell(new NuiButtonImage(acquiredTechIcons) { Tooltip = acquiredTechNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(acquiredTechNames) { Tooltip = acquiredTechNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true }
          };

          rootColumn.Children = new()
          {
            new NuiRow() { Children = new() {
              new NuiList(rowTemplateAvailableTechs, listCount) { RowHeight = 35,  Width = 240  },
              new NuiList(rowTemplateAcquiredTechs, listAcquiredTechCount) { RowHeight = 35, Width = 240 } } },
            new NuiRow() { Children = new() {
              new NuiSpacer(),
              new NuiButton("Valider") { Id = "validate", Tooltip = "Valider", Enabled = enabled, Encouraged = enabled, Width = 180, Height = 35 },
              new NuiSpacer() } }
          };

          CreateWindow();
        }
        public async void CreateWindow()
        {
          await NwTask.NextFrame();

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          window = new NuiWindow(rootColumn, "Choix de 4 maîtrises d'armes")
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

            nuiToken.OnNuiEvent += HandleTechSelectionEvents;

            InitTechsBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);          
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleTechSelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "selectTech":

                  if (availableTechs.Count < 1)
                    return;

                  int selectedTech = availableTechs[nuiEvent.ArrayIndex];

                  acquiredTechs.Add(selectedTech);
                  availableTechs.Remove(selectedTech);

                  BindAvailableTechs();
                  BindAcquiredTechs();

                  break;

                case "removeTech":

                  if (acquiredTechs.Count < 1)
                    return;

                  int clickedTech = acquiredTechs[nuiEvent.ArrayIndex];
                  acquiredTechs.Remove(clickedTech);

                  GetAvailableInvocations();

                  BindAvailableTechs();
                  BindAcquiredTechs();

                  break;

                case "validate":

                  foreach (var tech in acquiredTechs)
                  {
                    var baseItem = NwBaseItem.FromItemId(tech);
                    switch (baseItem?.ItemType)
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

                    player.oid.SendServerMessage($"Vous apprenez la maîtrise de {baseItem.Name.ToString()}", ColorConstants.Orange);
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_WEAPON_MASTER_CHOICE_FEAT").Delete();

                  CloseWindow();

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

                  break;
              }

              break;
          }
        }
        private void InitTechsBinding()
        {
          acquiredTechs.Clear();

          GetAvailableInvocations();
          BindAvailableTechs();

          enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void GetAvailableInvocations()
        {
          availableTechs.Clear();

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

                if (baseItem.DieToRoll > 0)
                {
                  if (!acquiredTechs.Contains(entry.RowIndex) 
                    && NativeUtils.GetCreatureWeaponProficiencyBonus(player.oid.LoginCreature, baseItem.ItemType) < 1)
                    availableTechs.Add(entry.RowIndex);
                }

                break;
            }
          }
        }
        private void BindAvailableTechs()
        {
          List<string> availableIconsList = new();
          List<string> availableNamesList = new();

          if (acquiredTechs.Count >= 4)
            availableTechs.Clear();

          foreach (var tech in availableTechs)
          {
            var baseItem = NwBaseItem.FromItemId(tech);

            availableIconsList.Add(baseItem.DefaultIcon);
            availableNamesList.Add(baseItem.Name.ToString());
          }

          availableTechIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableTechNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableTechs.Count);
        }
        private void BindAcquiredTechs()
        {
          List<string> acquiredIconsList = new();
          List<string> acquiredNamesList = new();

          foreach (var tech in acquiredTechs)
          {
            var baseItem = NwBaseItem.FromItemId(tech);

            acquiredIconsList.Add(baseItem.DefaultIcon);
            acquiredNamesList.Add(baseItem.Name.ToString());
          }

          acquiredTechIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredTechNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listAcquiredTechCount.SetBindValue(player.oid, nuiToken.Token, acquiredTechs.Count);
          
          if (acquiredTechs.Count >= 4)
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
      }
    }
  }
}
