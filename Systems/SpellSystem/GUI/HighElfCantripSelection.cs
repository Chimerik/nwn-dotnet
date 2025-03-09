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
      public class HighElfCantripSelectionWindow : PlayerWindow
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
        private readonly List<NwSpell> acquiredSpells = new();


        public HighElfCantripSelectionWindow(Player player) : base(player)
        {
          windowId = "highElfCantripSelection";

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

          CreateWindow();
        }
        public async void CreateWindow()
        {
          await NwTask.NextFrame();

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          string title = $"Haut Elfe - Modification du tour de magie";

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

            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_HIGHELF_CANTRIP_SELECTION").Value = 1;
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

                  if (acquiredSpells.Count(s => s.GetSpellLevelForClass(ClassType.Wizard) == 0) + 1 < 1)
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

                  var racialFeat = player.learnableSkills.Values.FirstOrDefault(l => l.source.Contains(SkillSystem.Category.Race)
                  && l.source.Contains(SkillSystem.Category.StartingTraits));

                  if(racialFeat.source.Count > 2)
                  {
                    racialFeat.source.Remove(SkillSystem.Category.Race);
                    racialFeat.source.Remove(SkillSystem.Category.StartingTraits);
                  }
                  else
                  {
                    player.oid.LoginCreature.RemoveFeat((Feat)racialFeat.id);
                    player.learnableSkills.Remove(racialFeat.id);
                  }

                  foreach (var spell in acquiredSpells)
                  {
                    if (player.learnableSkills.TryAdd(spell.FeatReference.Id, new LearnableSkill((LearnableSkill)learnableDictionary[spell.FeatReference.Id], player)))
                      player.learnableSkills[spell.FeatReference.Id].LevelUp(player);

                    player.learnableSkills[spell.FeatReference.Id].source.Add(Category.Race);
                    player.learnableSkills[spell.FeatReference.Id].source.Add(Category.StartingTraits);

                    player.oid.SendServerMessage($"Votre tour de magie de Haut Elfe est désormais {StringUtils.ToWhitecolor(spell.Name.ToString())}", ColorConstants.Orange);
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_HIGHELF_CANTRIP_SELECTION").Delete();

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

          if(acquiredSpells.Count(s => s.GetSpellLevelForClass(ClassType.Wizard) == 0) == 1)
            availableSpells.RemoveAll(s => s.GetSpellLevelForClass(ClassType.Wizard) == 0);

          List<NwSpell> tempList = new();
          tempList.AddRange(availableSpells.OrderByDescending(s => s.GetSpellLevelForClass(ClassType.Wizard)).ThenBy(s => s.Name.ToString()));
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
          tempList.AddRange(acquiredSpells.OrderByDescending(s => s.GetSpellLevelForClass(ClassType.Wizard)).ThenBy(s => s.Name.ToString()));
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
          
          if (acquiredSpells.Count == 1)
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void GetAvailableSpells()
        {
          foreach (var spell in NwRuleset.Spells.OrderByDescending(s => s.Name.ToString()))
          {
            SpellEntry entry = Spells2da.spellTable[spell.Id];

            if (acquiredSpells.Contains(spell)
              || spell.GetSpellLevelForClass(ClassType.Wizard) > 0)
              continue;

            if(player.learnableSkills.TryGetValue(spell.FeatReference.Id, out var racialSpell))
            {
              bool isFromOtherSource = false;

              foreach(var source in racialSpell.source)
              {
                if (!Utils.In(source, SkillSystem.Category.Race, SkillSystem.Category.StartingTraits))
                {
                  isFromOtherSource = true;
                  break;
                }
              }

              if (isFromOtherSource)
                continue;
            }

            availableSpells.Add(spell);
          }
        }
      }
    }
  }
}
