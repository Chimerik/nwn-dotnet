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
      public class CantripSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();

        private readonly NuiBind<string> availableSpellIcons = new("availableSpellIcons");
        private readonly NuiBind<string> availableSpellNames = new("availableSpellNames");
        private readonly NuiBind<string> acquiredSpellIcons = new("acquiredSpellIcons");
        private readonly NuiBind<string> acquiredSpellNames = new("acquiredSpellNames");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<int> listAcquiredSpellCount = new("listAcquiredSpellCount");

        private readonly NuiBind<bool> enabled = new("enabled");

        private readonly List<NwSpell> availableSpells = new();
        private readonly List<NwSpell> forcedSpells = new();
        private readonly List<NwSpell> acquiredSpells = new();

        private ClassType spellClass;
        private int nbCantrips;
        private int featId;

        public CantripSelectionWindow(Player player, ClassType spellClass, int nbCantrips, List<NwSpell> forcedList = null, int featId = 0) : base(player)
        {
          windowId = "cantripSelection";

          List<NuiListTemplateCell> rowTemplateAvailableSpells = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(availableSpellIcons) { Id = "availableSpellDescription", Tooltip = availableSpellNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(availableSpellNames) { Id = "availableSpellDescription", Tooltip = availableSpellNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("add_arrow") { Id = "selectSpell", Tooltip = "Sélectionner", DisabledTooltip = "Vous ne pouvez pas choisir davantage de sorts" }) { Width = 35 }
          };

          List<NuiListTemplateCell> rowTemplateAcquiredSpells = new()
          {
            new NuiListTemplateCell(new NuiButtonImage("remove_arrow") { Id = "removeSpell", Tooltip = "Retirer" }) { Width = 35 },
            new NuiListTemplateCell(new NuiButtonImage(acquiredSpellIcons) { Id = "acquiredSpellDescription", Tooltip = acquiredSpellNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(acquiredSpellNames) { Id = "acquiredSpellDescription", Tooltip = acquiredSpellNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true }
          };

          rootColumn.Children = new()
          {
            new NuiRow() { Children = new() {
              new NuiList(rowTemplateAvailableSpells, listCount) { RowHeight = 35,  Width = 240  },
              new NuiList(rowTemplateAcquiredSpells, listAcquiredSpellCount) { RowHeight = 35, Width = 240 } } },
            new NuiRow() { Children = new() {
              new NuiSpacer(),
              new NuiButton("Valider") { Id = "validate", Tooltip = "Valider", Enabled = enabled, Encouraged = enabled, Width = 180, Height = 35 },
              new NuiSpacer() } }
          };

          CreateWindow(spellClass, nbCantrips, forcedList, featId);
        }
        public async void CreateWindow(ClassType spellClass, int nbCantrips, List<NwSpell> forcedList = null, int featId = 0)
        {
          await NwTask.NextFrame();

          this.spellClass = spellClass;
          this.nbCantrips = nbCantrips;
          this.featId = featId;

          forcedSpells.Clear();
          if(forcedList != null)
            this.forcedSpells.AddRange(forcedList);

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          string title = $"{NwClass.FromClassType(spellClass).Name.ToString()} - Choix de {nbCantrips} tour(s) de magie";

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

            nuiToken.OnNuiEvent += HandleSpellSelectionEvents;

            availableSpells.Clear();
            acquiredSpells.Clear();

            GetAvailableSpells();
            BindAvailableSpells();
            BindAcquiredSpells();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if(availableSpells.Count < 1)
            {
              CloseWindow();
              player.oid.SendServerMessage($"Vous connaissez déjà tous les sorts de {StringUtils.ToWhitecolor(NwClass.FromClassType(spellClass).Name.ToString())} de ce niveau", ColorConstants.Orange);
            }
            else
            {
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CANTRIP_SELECTION").Value = nbCantrips;
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").Value = (int)spellClass;
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_INITIATE_CANTRIP_FEAT_ID").Value = featId;
            }
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleSpellSelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "selectSpell":

                  if (availableSpells.Count == 0)
                    return;

                  NwSpell selectedSpell = availableSpells[nuiEvent.ArrayIndex];

                  acquiredSpells.Add(selectedSpell);
                  availableSpells.Remove(selectedSpell);

                  BindAvailableSpells();
                  BindAcquiredSpells();

                  break;

                case "removeSpell":

                  if (acquiredSpells.Count == 0)
                    return;

                  NwSpell clickedSpell = acquiredSpells[nuiEvent.ArrayIndex];
                  acquiredSpells.Remove(clickedSpell);

                  if (acquiredSpells.Count(s => s.GetSpellLevelForClass(spellClass) == 0) + 1 < nbCantrips)
                    availableSpells.Add(clickedSpell);
                  else
                  {
                    availableSpells.Clear();
                    GetAvailableSpells();
                  }
                  
                  BindAvailableSpells();
                  BindAcquiredSpells();

                  break;

                case "availableSpellDescription":

                  if (!player.windows.TryGetValue("spellDescription", out var spellWindow)) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, availableSpells[nuiEvent.ArrayIndex]));
                  else ((SpellDescriptionWindow)spellWindow).CreateWindow(availableSpells[nuiEvent.ArrayIndex]);

                  break;

                case "acquiredSpellDescription":

                  if (!player.windows.TryGetValue("spellDescription", out var window)) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, acquiredSpells[nuiEvent.ArrayIndex]));
                  else ((SpellDescriptionWindow)window).CreateWindow(acquiredSpells[nuiEvent.ArrayIndex]);

                  break;

                case "validate":

                  foreach (var spell in acquiredSpells)
                  {
                    if (featId < 1)
                    {
                      if (player.learnableSpells.TryGetValue(spell.Id, out var learnable))
                      {
                        learnable.learntFromClasses.Add((int)spellClass);

                        if (learnable.currentLevel < 1)
                          learnable.LevelUp(player);
                      }
                      else
                      {
                        LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spell.Id], (int)spellClass);
                        player.learnableSpells.Add(learnableSpell.id, learnableSpell);
                        learnableSpell.LevelUp(player);
                      }
                    }
                    else if(player.learnableSkills.TryAdd(spell.FeatReference.Id, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[spell.FeatReference.Id], player, levelTaken: player.oid.LoginCreature.Level)))
                    {
                      player.learnableSkills[spell.FeatReference.Id].LevelUp(player);
                    }

                    player.oid.SendServerMessage($"Vous apprenez le tour de magie {StringUtils.ToWhitecolor(spell.Name.ToString())}", ColorConstants.Orange);
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CANTRIP_SELECTION").Delete();
                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_INITIATE_CANTRIP_FEAT_ID").Delete();

                  CloseWindow();

                  break;
              }

              break;
          }
        }
        private void BindAvailableSpells()
        {
          List<string> availableIconsList = new();
          List<string> availableNamesList = new();

          if(acquiredSpells.Count(s => s.GetSpellLevelForClass(spellClass) == 0) == nbCantrips)
            availableSpells.RemoveAll(s => s.GetSpellLevelForClass(spellClass) == 0);

          List<NwSpell> tempList = new();
          tempList.AddRange(availableSpells.OrderByDescending(s => s.GetSpellLevelForClass(spellClass)).ThenBy(s => s.Name.ToString()));
          availableSpells.Clear();

          foreach (var spell in tempList)
          {
            availableSpells.Add(spell);
            availableIconsList.Add(spell.IconResRef);
            availableNamesList.Add(spell.Name.ToString().Replace("’", "'"));
          }

          availableSpellIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableSpellNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableSpells.Count);
        }
        private void BindAcquiredSpells()
        {
          List<string> acquiredIconsList = new();
          List<string> acquiredNamesList = new();

          List<NwSpell> tempList = new();
          tempList.AddRange(acquiredSpells.OrderByDescending(s => s.GetSpellLevelForClass(spellClass)).ThenBy(s => s.Name.ToString()));
          acquiredSpells.Clear();

          foreach (var spell in tempList)
          {
            acquiredSpells.Add(spell);
            acquiredIconsList.Add(spell.IconResRef);
            acquiredNamesList.Add(spell.Name.ToString().Replace("’", "'"));
          }

          acquiredSpellIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredSpellNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listAcquiredSpellCount.SetBindValue(player.oid, nuiToken.Token, acquiredSpells.Count);
          
          if (acquiredSpells.Count == nbCantrips)
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void GetAvailableSpells()
        {
          if (forcedSpells.Count > 0)
          {
            availableSpells.AddRange(forcedSpells);
            return;
          }

          foreach (var spell in NwRuleset.Spells.OrderByDescending(s => s.Name.ToString()))
          {
            SpellEntry entry = Spells2da.spellTable[spell.Id];

            if (acquiredSpells.Contains(spell))
              continue;

            if (spell.GetSpellLevelForClass(spellClass) > 0)
              continue;

            if (player.learnableSpells.TryGetValue(spell.Id, out var learnable) && learnable.currentLevel > 0)
              continue;

            if (spellClass == ClassType.Cleric && Utils.In(spell.Id, CustomSpell.FireBolt, CustomSpell.PoisonSpray, CustomSpell.Druidisme, CustomSpell.Elementalisme, CustomSpell.Shillelagh, (int)Spell.RayOfFrost, CustomSpell.Message, (int)Spell.ElectricJolt, (int)Spell.GreatThunderclap, CustomSpell.ProduceFlame))
              continue;

            if (entry.hideFromClasses is not null)
            {
              switch (spellClass)
              {
                case ClassType.Bard: if (entry.hideFromClasses.Contains(ClassType.Bard) && !player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.SecretsMagiques)) continue; break;
                case ClassType.Ranger: if (entry.hideFromClasses.Contains(ClassType.Ranger)) continue; break;
                case ClassType.Wizard: if (entry.hideFromClasses.Contains(ClassType.Wizard)) continue; break;
                case ClassType.Sorcerer: if (entry.hideFromClasses.Contains(ClassType.Sorcerer)) continue; break;
                case (ClassType)CustomClass.Occultiste: if (entry.hideFromClasses.Contains((ClassType)CustomClass.Occultiste)) continue; break;
              }
            }

            availableSpells.Add(spell);
          }
        }
      }
    }
  }
}
