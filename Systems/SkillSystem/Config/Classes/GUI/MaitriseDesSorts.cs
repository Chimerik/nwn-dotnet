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
      public class MaitriseDesSortsWindow : PlayerWindow
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

        public MaitriseDesSortsWindow(Player player) : base(player)
        {
          windowId = "maitriseDesSorts";

          List<NuiListTemplateCell> rowTemplateAvailableSpells = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(availableSpellIcons) { Id = "availableSpellDescription", Tooltip = availableSpellNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(availableSpellNames) { Id = "availableSpellDescription", Tooltip = availableSpellNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("add_arrow") { Id = "selectSpell", Tooltip = "Sélectionner" }) { Width = 35 }
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
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          window = new NuiWindow(rootColumn, "Choisissez un sort de niveau 1 et un sort de niveau 2 à maîtriser")
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

            nuiToken.OnNuiEvent += HandleMaitriseDesSortsEvents;

            InitSpellsBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleMaitriseDesSortsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "selectSpell":

                  NwSpell selectedSpell = availableSpells[nuiEvent.ArrayIndex];
                  acquiredSpells.Add(selectedSpell);

                  availableSpells.RemoveAll((s => s.GetSpellLevelForClass(ClassType.Wizard) == selectedSpell.GetSpellLevelForClass(ClassType.Wizard)));

                  BindAvailableSpells();
                  BindAcquiredSpells();
                  
                  break;

                case "removeSpell":

                  NwSpell clickedSpell = acquiredSpells[nuiEvent.ArrayIndex];

                  acquiredSpells.Remove(clickedSpell);

                  InitSpellsBinding();
                  BindAcquiredSpells();

                  break;

                case "availableSpellDescription":

                  if (!player.windows.TryGetValue("spellDescription", out var spell)) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, availableSpells[nuiEvent.ArrayIndex]));
                  else ((SpellDescriptionWindow)spell).CreateWindow(availableSpells[nuiEvent.ArrayIndex]);

                  break;

                case "acquiredSpellDescription":

                  if (!player.windows.TryGetValue("spellDescription", out var value)) player.windows.Add("spellDescription", new SpellDescriptionWindow(player, acquiredSpells[nuiEvent.ArrayIndex]));
                  else ((SpellDescriptionWindow)value).CreateWindow(acquiredSpells[nuiEvent.ArrayIndex]);

                  break;

                case "validate":

                  foreach (var masterSpell in player.learnableSpells.Values.Where(s => s.mastery))
                  {
                    masterSpell.mastery = false;
                    player.oid.SendServerMessage($"Vous abandonnez la maîtrise du sort {StringUtils.ToWhitecolor(masterSpell.name)}", ColorConstants.Orange);
                  }

                  foreach (var masterSpell in acquiredSpells)
                  {
                    player.learnableSpells[masterSpell.Id].mastery = true;
                    player.oid.SendServerMessage($"Vous maîtrisez désormais le sort {StringUtils.ToWhitecolor(masterSpell.Name.ToString())} et ne consommez pas d'emplacement de sort tant que vous l'utilisez à son niveau normal", ColorConstants.Orange);
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_MAITRISE_DES_SORTS_SELECTION").Delete();
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

          availableSpells.Clear();

          foreach(var learnable in player.learnableSpells.Values)
          {
            if (learnable.currentLevel < 1 || !learnable.learntFromClasses.Contains((int)ClassType.Wizard))
              continue;

            NwSpell spell = NwSpell.FromSpellId(learnable.id);

            if (spell.GetSpellLevelForClass(ClassType.Wizard) < 1 || spell.GetSpellLevelForClass(ClassType.Wizard) > 2)
              continue;

            if (acquiredSpells.Any() && spell.GetSpellLevelForClass(ClassType.Wizard) == acquiredSpells[0].GetSpellLevelForClass(ClassType.Wizard))
              continue;

            availableSpells.Add(spell);

            availableIconsList.Add(spell.IconResRef);
            availableNamesList.Add(spell.Name.ToString().Replace("’", "'"));       
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

          foreach (var spell in availableSpells)
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

          foreach (var spell in acquiredSpells)
          {
            acquiredIconsList.Add(spell.IconResRef);
            acquiredNamesList.Add(spell.Name.ToString().Replace("’", "'")); 
          }
          acquiredSpellIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredSpellNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listAcquiredSpellCount.SetBindValue(player.oid, nuiToken.Token, acquiredSpells.Count);
          enabled.SetBindValue(player.oid, nuiToken.Token, acquiredSpells.Count == 2);
        }
      }
    }
  }
}
