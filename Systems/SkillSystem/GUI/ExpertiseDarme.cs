using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Native.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ExpertiseDarmeSelectionWindow : PlayerWindow
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

        public ExpertiseDarmeSelectionWindow(Player player, int nbTechs = 1) : base(player)
        {
          windowId = "expertiseDarmeSelection";

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
        public async void CreateWindow(int nbTechs = 1)
        {
          await NwTask.NextFrame();

          this.nbTechs = nbTechs;

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 520, 500);

          window = new NuiWindow(rootColumn, $"Choix de {nbTechs} expertise(s) d'arme(s)")
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

            if(availableTechs.Count < 1)
            {
              player.oid.SendServerMessage("Vous êtes déjà expert de toutes les armes que vous maîtrisez", ColorConstants.Orange);
              player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_DARME_SELECTION").Delete();
              return;
            }

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_DARME_SELECTION").Value = nbTechs;            
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

                  GetAvailableExpertises();

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
                    player.LearnClassSkill(tech.id);
                    player.oid.SendServerMessage($"Vous apprenez l'expertise : {StringUtils.ToWhitecolor(tech.name)}", ColorConstants.Orange);
                  }

                  player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_EXPERTISE_DARME_SELECTION").Delete();

                  CloseWindow();

                  break;
              }

              break;
          }
        }
        private void InitTechsBinding()
        {
          acquiredTechs.Clear();

          GetAvailableExpertises();
          BindAvailableTechs();

          enabled.SetBindValue(player.oid, nuiToken.Token, false);
        }
        private void GetAvailableExpertises()
        {
          availableTechs.Clear();

          foreach (LearnableSkill learnable in learnableDictionary.Values.Where(s => s is LearnableSkill skill && skill.category == Category.ExpertiseDarme
             && !acquiredTechs.Contains(skill)
             && NativeUtils.GetCreatureWeaponProficiencyBonus(player.oid.LoginCreature, ItemUtils.GeBaseWeaponFromLearnable(skill.id)) > 0
             && (!player.learnableSkills.TryGetValue(skill.id, out var learntSkill) || learntSkill.currentLevel < 1))
            .OrderBy(s => s.name).Cast<LearnableSkill>())
          {
            availableTechs.Add(learnable);
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
