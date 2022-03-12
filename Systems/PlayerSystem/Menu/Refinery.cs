﻿using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class RefineryWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren = new List<NuiElement>();
        private readonly List<NuiListTemplateCell> rowTemplate;
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly NuiBind<string> materiaIcon = new NuiBind<string>("materiaIcon");
        private readonly NuiBind<string> materiaNames = new NuiBind<string>("materiaNames");
        private readonly NuiBind<string> input = new NuiBind<string>("input");
        private readonly NuiBind<string> refineLabel = new NuiBind<string>("label");
        private IEnumerable<CraftResource> playerCraftResourceList = new List<CraftResource>();
        private CraftResource selectedResource;
        private ResourceType resourceType;

        public RefineryWindow(Player player, ResourceType resourceType) : base(player)
        {
          windowId = "refinery";

          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiImage(materiaIcon) { Height = 40, Width = 40 }) { Width = 45 },
            new NuiListTemplateCell(new NuiLabel(materiaNames) { VerticalAlign = NuiVAlign.Middle, Height = 40 }),
            new NuiListTemplateCell(new NuiButton("Raffiner") { Id = "refine", Tooltip = "Permet de raffiner la matéria brute en un matériau utilisable", Height = 40 }),
            new NuiListTemplateCell(new NuiButton("Concentrer") { Id = "upgrade", Tooltip = "Concentre une grande quantité de matéria brute en une meilleure qualité", Height = 40, Enabled = player.learnableSkills.ContainsKey(CustomSkill.MateriaGradeConcentration) })
          };

          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 40 });
          CreateWindow(resourceType);
        }
        public void CreateWindow(ResourceType resourceType)
        {
          this.resourceType = resourceType;
          rootChidren.Clear();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 600);

          window = new NuiWindow(rootGroup, "Raffinerie")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleRefineryEvents;
          player.oid.OnNuiEvent += HandleRefineryEvents;
          player.oid.OnServerSendArea -= OnAreaChangeCloseWindow;
          player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

          token = player.oid.CreateNuiWindow(window, windowId);

          LoadMateriaList();

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleRefineryEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "refine":
                  selectedResource = playerCraftResourceList.ElementAt(nuiEvent.ArrayIndex);
                  LoadResourceRefinementGUI();
                  return;

                case "back":
                  LoadMainRefinementGUI();
                  return;

                case "validate":

                  if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").HasValue && selectedResource.quantity > 0)
                  {
                    int inputQuantity = 0;

                    if (!int.TryParse(input.GetBindValue(player.oid, token), out inputQuantity) || inputQuantity > selectedResource.quantity)
                      inputQuantity = selectedResource.quantity;
                  
                    selectedResource.quantity -= inputQuantity;

                    int grade = selectedResource.grade;
                    int refinedAmount = player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").Value;

                    if (HandleRefinerLuck())
                      if (grade == 8)
                        refinedAmount *= 50 / 100;
                      else
                        grade += 1;

                    CraftResource refinedMateria = player.craftResourceStock.FirstOrDefault(r => r.type == ResourceType.Ingot && r.grade == grade);
                    
                    if (refinedMateria != null)
                      refinedMateria.quantity += refinedAmount;
                    else
                      player.craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.Ingot && r.grade == grade), refinedAmount));

                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").Delete();
                  }
                  else
                    player.oid.SendServerMessage("Votre tentative de raffinage n'est plus valide. Veuillez vérifier que vous disposez toujours de la quantité adéquate.", ColorConstants.Red);

                  LoadMainRefinementGUI();

                  return;

                case "upgrade":
                  selectedResource = playerCraftResourceList.ElementAt(nuiEvent.ArrayIndex);

                  if (selectedResource.grade < 8)
                    LoadResourceUpgradeGUI();
                  else
                    player.oid.SendServerMessage("Veuillez sélectionner un niveau de qualité inférieur pour la concentration de matéria.", ColorConstants.Red);

                  return;

                case "validateUpgrade":

                  if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").HasValue && selectedResource.quantity > 0)
                  {
                    int inputQuantity = 0;

                    if (!int.TryParse(input.GetBindValue(player.oid, token), out inputQuantity) || inputQuantity > selectedResource.quantity)
                      inputQuantity = selectedResource.quantity;

                    selectedResource.quantity -= inputQuantity;

                    int grade = selectedResource.grade;
                    int refinedAmount = player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").Value;

                    if (HandleRefinerLuck())
                      if (grade == 8)
                        refinedAmount *= 50 / 100;
                      else
                        grade += 1;

                    CraftResource refinedMateria = player.craftResourceStock.FirstOrDefault(r => r.type == selectedResource.type && r.grade == grade + 1);
                   
                    if (refinedMateria != null)
                      refinedMateria.quantity += refinedAmount;
                    else
                      player.craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == selectedResource.type && r.grade == grade + 1), refinedAmount));

                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").Delete();
                  }
                  else
                    player.oid.SendServerMessage("Votre tentative de concentration n'est plus valide. Veuillez vérifier que vous disposez toujours de la quantité adéquate.", ColorConstants.Red);

                  LoadMainRefinementGUI();

                  return;
              }
              break;

            case NuiEventType.Watch:

              if (nuiEvent.ElementId == "input")
                UpdateRefinementGUI();

              break;
          }
        }
        private void LoadMateriaList()
        {
          List<string> materiaNamesList = new List<string>();
          List<string> materiaIconList = new List<string>();
          playerCraftResourceList = player.craftResourceStock.Where(c => c.type == resourceType && c.quantity > 0).OrderBy(c => c.grade);

          foreach (CraftResource resource in playerCraftResourceList)
          {
            materiaIconList.Add(resource.iconString);
            materiaNamesList.Add($"{resource.name} (x{resource.quantity})");
          }

          materiaIcon.SetBindValues(player.oid, token, materiaIconList);
          materiaNames.SetBindValues(player.oid, token, materiaNamesList);
          listCount.SetBindValue(player.oid, token, playerCraftResourceList.Count());
        }
        private void LoadResourceRefinementGUI()
        {
          rootChidren.Clear();
          rootChidren.Add(new NuiTextEdit("", input, (ushort)selectedResource.quantity.ToString().Length, false) { Tooltip = "En cas de saisie invalide, la totalité de ce type de matéria disponible sera raffinée." });
          rootChidren.Add(new NuiLabel(refineLabel) { Height = 120 });

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Retour") { Id = "back", Height = 30 },
              new NuiButton("Valider") { Id = "validate", Height = 30 },
              new NuiSpacer()
            }
          });

          input.SetBindValue(player.oid, token, selectedResource.quantity.ToString());
          input.SetBindWatch(player.oid, token, true);

          UpdateRefinementGUI();

          rootGroup.SetLayout(player.oid, token, rootColumn);
        }
        private void LoadMainRefinementGUI()
        {
          rootChidren.Clear();
          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 40 });
          input.SetBindWatch(player.oid, token, false);
          LoadMateriaList();
          rootGroup.SetLayout(player.oid, token, rootColumn);
        }
        private void UpdateRefinementGUI()
        {
          int inputQuantity = 0;

          if (!int.TryParse(input.GetBindValue(player.oid, token), out inputQuantity) || inputQuantity > selectedResource.quantity)
            inputQuantity = selectedResource.quantity;

          double reprocessingSkill = player.learnableSkills.ContainsKey(selectedResource.reprocessingLearnable) ? 1.00 + 3 * player.learnableSkills[selectedResource.reprocessingLearnable].totalPoints / 100 : 1.00;
          double efficiencySkill = player.learnableSkills.ContainsKey(selectedResource.reprocessingEfficiencyLearnable) ? 1.00 + 2 * player.learnableSkills[selectedResource.reprocessingEfficiencyLearnable].totalPoints / 100 : 1.00;
          double reproGradeSkill = player.learnableSkills.ContainsKey(selectedResource.reprocessingGradeLearnable) ? 1.00 + 2 * player.learnableSkills[selectedResource.reprocessingGradeLearnable].totalPoints / 100 : 1.00;
          double connectionSkill = player.learnableSkills.ContainsKey(CustomSkill.ConnectionsPromenade) ? 0.95 + player.learnableSkills[CustomSkill.ConnectionsPromenade].totalPoints / 100 : 1.00;
          double expertSkill = player.learnableSkills.ContainsKey(selectedResource.reprocessingExpertiseLearnable) ? 12 * player.learnableSkills[selectedResource.reprocessingExpertiseLearnable].totalPoints / 100 : 0;
          double total = 2 * inputQuantity;
            total -= inputQuantity * selectedResource.grade * 0.15 * expertSkill;
            total *= 0.3 * reprocessingSkill * efficiencySkill * reproGradeSkill;
            total *= connectionSkill;

            player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").Value = (int)total;

            //TODO : Variabiliser le -30% de fonderie de base, quand il sera possible de fabriquer son propre atelier
            string label = "Efficacité raffinage -30 % (base atelier Impériale)\n" +
              $"x{reprocessingSkill} (Raffinage)\n" +
              $"x{efficiencySkill} (Raffinage Efficace)\n" +
              $"x{reproGradeSkill} (Raffinage Expert)\n" +
              $"x{connectionSkill} (Taxes Quartier Promenade)\n" +
              $"Total : {total} matérias de qualité {selectedResource.grade}";

            refineLabel.SetBindValue(player.oid, token, label);
        }
        private void LoadResourceUpgradeGUI()
        {
          rootChidren.Clear();
          rootChidren.Add(new NuiTextEdit("", input, (ushort)selectedResource.quantity.ToString().Length, false) { Tooltip = "En cas de saisie invalide, la totalité de ce type de matéria disponible sera raffinée." });
          rootChidren.Add(new NuiLabel(refineLabel) { Height = 120 });

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Retour") { Id = "back", Height = 30 },
              new NuiButton("Valider") { Id = "validateUpgrade", Height = 30 },
              new NuiSpacer()
            }
          });

          input.SetBindValue(player.oid, token, selectedResource.quantity.ToString());
          input.SetBindWatch(player.oid, token, true);

          UpdateUpgradeGUI();

          rootGroup.SetLayout(player.oid, token, rootColumn);
        }
        private void UpdateUpgradeGUI()
        {
          int inputQuantity = 0;

          if (!int.TryParse(input.GetBindValue(player.oid, token), out inputQuantity) || inputQuantity > selectedResource.quantity)
            inputQuantity = selectedResource.quantity;

          double concentrationSkill = 1.00 + 5 * player.learnableSkills[CustomSkill.MateriaGradeConcentration].totalPoints / 100;
          double connectionSkill = player.learnableSkills.ContainsKey(CustomSkill.ConnectionsPromenade) ? 0.95 + player.learnableSkills[CustomSkill.ConnectionsPromenade].totalPoints / 100 : 1.00;
          double total = inputQuantity / 3  * concentrationSkill * 0.3;
          total *= connectionSkill;

          player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_REFINERY_TOTAL").Value = (int)total;

          //TODO : Variabiliser le -30% de fonderie de base, quand il sera possible de fabriquer son propre atelier
          string label = "Efficacité concentration -30 % (base atelier Impérial)\n" +
            $"x{concentrationSkill} (Concentration)\n" +
            $"x{connectionSkill} (Taxes Quartier Promenade)\n" +
            $"Total : {total} matérias raffinées de qualité {selectedResource.grade + 1}";

          refineLabel.SetBindValue(player.oid, token, label);
        }
        private bool HandleRefinerLuck()
        {
          if (!player.learnableSkills.ContainsKey(CustomSkill.MateriaGradeConcentration))
            return false;

          if (NwRandom.Roll(Utils.random, 100) > player.learnableSkills[CustomSkill.MateriaGradeConcentration].totalPoints)
            return false;

          player.oid.SendServerMessage("Votre concentration hors du commun vous permet d'obtenir un bien meilleur résultat qu'attendu dans votre ouvrage !", ColorConstants.Rose);
          return true;
        }
      }
    }
  }
}