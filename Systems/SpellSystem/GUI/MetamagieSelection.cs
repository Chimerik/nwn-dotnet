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
      public class MetamagieSelectionWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();

        private readonly NuiBind<string> availableTechIcons = new("availableTechIcons");
        private readonly NuiBind<string> availableTechNames = new("availableTechNames");
        private readonly NuiBind<string> acquiredTechIcons = new("acquiredTechIcons");
        private readonly NuiBind<string> acquiredTechNames = new("acquiredTechNames");

        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<int> listAcquiredTechCount = new("listAcquiredTechCount");

        private readonly NuiBind<bool> enabled = new("enabled");

        private readonly List<LearnableSkill> availableTechs = new();
        private readonly List<LearnableSkill> acquiredTechs = new();

        private int nbTechs;

        public MetamagieSelectionWindow(Player player, int nbTechs = 2) : base(player)
        {
          windowId = "metamagieSelection";

          List<NuiListTemplateCell> rowTemplateAvailableTechs = new()
          {
            new NuiListTemplateCell(new NuiButtonImage(availableTechIcons) { Id = "availableTechDescription", Tooltip = availableTechNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(availableTechNames) { Id = "availableTechDescription", Tooltip = availableTechNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("add_arrow") { Id = "selectTech", Tooltip = "Sélectionner", DisabledTooltip = "Vous ne pouvez pas choisir davantage de techniques" }) { Width = 35 }
          };

          List<NuiListTemplateCell> rowTemplateAcquiredTechs = new()
          {
            new NuiListTemplateCell(new NuiButtonImage("remove_arrow") { Id = "removeTech", Tooltip = "Retirer" }) { Width = 35 },
            new NuiListTemplateCell(new NuiButtonImage(acquiredTechIcons) { Id = "acquiredTechDescription", Tooltip = acquiredTechNames }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(acquiredTechNames) { Id = "acquiredTechDescription", Tooltip = acquiredTechNames, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center }) { VariableSize = true }
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

          CreateWindow(nbTechs);
        }
        public async void CreateWindow(int nbTechs = 2)
        {
          await NwTask.NextFrame();

          this.nbTechs = nbTechs;

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          window = new NuiWindow(rootColumn, $"Choix de {nbTechs} options de métamagie(s)")
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

            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_METAMAGIE_SELECTION").Value = nbTechs;            
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

                  LearnableSkill selectedTech = availableTechs[nuiEvent.ArrayIndex];

                  acquiredTechs.Add(selectedTech);
                  availableTechs.Remove(selectedTech);

                  BindAvailableTechs();
                  BindAcquiredTechs();

                  break;

                case "removeTech":

                  if (acquiredTechs.Count < 1)
                    return;

                  LearnableSkill clickedTech = acquiredTechs[nuiEvent.ArrayIndex];
                  acquiredTechs.Remove(clickedTech);

                  GetAvailableInvocations();

                  BindAvailableTechs();
                  BindAcquiredTechs();

                  break;

                case "availableTechDescription":

                  if (!player.windows.TryGetValue("learnableDescription", out var spellWindow)) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, availableTechs[nuiEvent.ArrayIndex].id));
                  else ((LearnableDescriptionWindow)spellWindow).CreateWindow(availableTechs[nuiEvent.ArrayIndex].id);

                  break;

                case "acquiredTechDescription":

                  if (!player.windows.TryGetValue("learnableDescription", out var window)) player.windows.Add("learnableDescription", new LearnableDescriptionWindow(player, acquiredTechs[nuiEvent.ArrayIndex].id));
                  else ((LearnableDescriptionWindow)window).CreateWindow(acquiredTechs[nuiEvent.ArrayIndex].id);

                  break;

                case "validate":

                  foreach (var tech in acquiredTechs)
                  {
                    player.learnableSkills.TryAdd(tech.id, new LearnableSkill((LearnableSkill)learnableDictionary[tech.id], player));
                    player.learnableSkills[tech.id].LevelUp(player);
                    player.learnableSkills[tech.id].source.Add(Category.Class);

                    player.oid.SendServerMessage($"Vous maîtrisez l'option de métamagie {StringUtils.ToWhitecolor(tech.name)}", ColorConstants.Orange);
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_METAMAGIE_SELECTION").Delete();

                  CloseWindow();

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

          foreach (LearnableSkill tech in learnableDictionary.Values.Where(l => l is LearnableSkill skill
          && skill.category == Category.Metamagic
          && !player.learnableSkills.ContainsKey(skill.id)).OrderBy(s => s.name).Cast<LearnableSkill>())
          {
            availableTechs.Add(tech);
          }
        }
        private void BindAvailableTechs()
        {
          List<string> availableIconsList = new();
          List<string> availableNamesList = new();

          if (acquiredTechs.Count >= nbTechs)
            availableTechs.Clear();

          foreach (var tech in availableTechs)
          {
            availableIconsList.Add(tech.icon);
            availableNamesList.Add(tech.name);
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
            acquiredIconsList.Add(tech.icon);
            acquiredNamesList.Add(tech.name);
          }

          acquiredTechIcons.SetBindValues(player.oid, nuiToken.Token, acquiredIconsList);
          acquiredTechNames.SetBindValues(player.oid, nuiToken.Token, acquiredNamesList);
          listAcquiredTechCount.SetBindValue(player.oid, nuiToken.Token, acquiredTechs.Count);
          
          if (acquiredTechs.Count >= nbTechs)
            enabled.SetBindValue(player.oid, nuiToken.Token, true);
          else
            enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
      }
    }
  }
}
