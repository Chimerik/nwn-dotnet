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
      public class ArcaneMystiqueSelectionWindow : PlayerWindow
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

        private int spellLevel;

        public ArcaneMystiqueSelectionWindow(Player player, int spellLevel) : base(player)
        {
          windowId = "arcaneMystiqueSelection";

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

          CreateWindow(spellLevel);
        }
        public void CreateWindow(int spellLevel)
        {
          this.spellLevel = spellLevel;

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          window = new NuiWindow(rootColumn, $"Occultiste - Arcane Mystique")
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

            nuiToken.OnNuiEvent += HandleArcaneMystiqueSelectionEvents;

            InitSpellsBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if(availableSpells.Count == 0)
            {
              CloseWindow();
              player.oid.SendServerMessage($"Vous connaissez déjà tous les sorts", ColorConstants.Orange);
            }
            else
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ARCANE_MYSTIQUE_SELECTION").Value = spellLevel;
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleArcaneMystiqueSelectionEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "selectSpell":

                  NwSpell selectedSpell = availableSpells[nuiEvent.ArrayIndex];

                  acquiredSpells.Add(selectedSpell);
                  availableSpells.Remove(selectedSpell);

                  BindAvailableSpells();
                  BindAcquiredSpells();

                  break;

                case "removeSpell":

                  NwSpell clickedSpell = acquiredSpells[nuiEvent.ArrayIndex];
                  acquiredSpells.Remove(clickedSpell);

                  if (acquiredSpells.Count != 0)
                    availableSpells.Add(clickedSpell);
                  else
                  {
                    availableSpells.AddRange(NwRuleset.Spells.Where(s => s.GetSpellLevelForClass((ClassType)CustomClass.Occultiste) == spellLevel
                    && !acquiredSpells.Contains(s)
                    && (!player.learnableSpells.TryGetValue(s.Id, out var learnable) || learnable.currentLevel < 1)));
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
                    if (player.learnableSpells.TryGetValue(spell.Id, out var learnable))
                    {
                      learnable.learntFromClasses.Add(CustomClass.Occultiste);

                      if(learnable.currentLevel < 1)
                        learnable.LevelUp(player);                      
                    }
                    else
                    {
                      LearnableSpell learnableSpell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spell.Id], CustomClass.Occultiste);
                      player.learnableSpells.Add(learnableSpell.id, learnableSpell);
                      learnableSpell.LevelUp(player);
                    }

                    player.oid.SendServerMessage($"Vous apprenez l'arcane mystique {StringUtils.ToWhitecolor(spell.Name.ToString())}", ColorConstants.Orange);
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_ARCANE_MYSTIQUE_SELECTION").Delete();

                  CloseWindow();

                  break;
              }

              break;
          }
        }
        private void InitSpellsBinding()
        {
          List<string> availableIconsList = new();
          List<string> availableNamesList = new();
          List<bool> selectableList = new();

          availableSpells.Clear();
          acquiredSpells.Clear();

          foreach (var spell in NwRuleset.Spells.OrderByDescending(s => s.Name.ToString()))
          {
            if (spell.GetSpellLevelForClass((ClassType)CustomClass.Occultiste) != spellLevel)
              continue;

            if (player.learnableSpells.TryGetValue(spell.Id, out var learnable) && learnable.currentLevel > 0
              && learnable.learntFromClasses.Contains(CustomClass.Occultiste))
              continue;

            availableSpells.Add(spell);

            availableIconsList.Add(spell.IconResRef);
            availableNamesList.Add(spell.Name.ToString().Replace("’", "'"));
            selectableList.Add(true);
          }

          availableSpellIcons.SetBindValues(player.oid, nuiToken.Token, availableIconsList);
          availableSpellNames.SetBindValues(player.oid, nuiToken.Token, availableNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, availableSpells.Count);
          enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void BindAvailableSpells()
        {
          List<string> availableIconsList = new();
          List<string> availableNamesList = new();

          if (acquiredSpells.Any())
            availableSpells.Clear();

          foreach (var spell in availableSpells.OrderByDescending(s => s.Name.ToString()))
          {
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

          foreach (var spell in acquiredSpells.OrderByDescending(s => s.Name.ToString()))
          {
            acquiredIconsList.Add(spell.IconResRef);
            acquiredNamesList.Add(spell.Name.ToString().Replace("’", "'"));
          }

          acquiredSpellIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredSpellNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listAcquiredSpellCount.SetBindValue(player.oid, nuiToken.Token, acquiredSpells.Count);
          
          if (acquiredSpells.Any())
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
      }
    }
  }
}
