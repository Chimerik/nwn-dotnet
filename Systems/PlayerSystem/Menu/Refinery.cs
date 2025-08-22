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
      public class RefineryWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChidren = new();
        private readonly List<NuiListTemplateCell> rowTemplate;
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> materiaIcon = new("materiaIcon");
        private readonly NuiBind<string> materiaNames = new("materiaNames");
        private readonly NuiBind<string> input = new("input");
        private readonly NuiBind<string> refineLabel = new("label");
        private readonly NuiBind<bool> concentrateEnabled = new("concentrateEnabled");
        private IEnumerable<CraftResource> playerCraftResourceList;
        private CraftResource selectedResource;

        public RefineryWindow(Player player) : base(player)
        {
          windowId = "refinery";

          rootColumn.Children = rootChidren;

          rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(materiaIcon) { Height = 35, Width = 35 }) { Width = 35 },
            new NuiListTemplateCell(new NuiLabel(materiaNames) { VerticalAlign = NuiVAlign.Middle, Height = 35 }) { VariableSize = true },
            new NuiListTemplateCell(new NuiTextEdit("Quantité", input, 6, false) { Height = 40 }) { Width = 80 },
            new NuiListTemplateCell(new NuiButtonImage("refine") { Id = "refine", Tooltip = "Permet de raffiner la matéria brute afin de la rendre utilisable", Height = 35 }) { Width = 35},
            new NuiListTemplateCell(new NuiButtonImage("concentrate") { Id = "upgrade", Tooltip = "Concentre une grande quantité de matéria brute en une meilleure qualité", Height = 35, Enabled = concentrateEnabled }) { Width = 35 }
          };

          //TODO : Variabiliser le -30% de fonderie de base, quand il sera possible de fabriquer son propre atelier
          //TODO : Refine button tooltip : afficher ces infos dans le Tooltip avec la .35 de NwN:EE (+ plutôt prendre exemple sur le OnExamine de craft_resource)
          //TODO : Aussi, afficher le résultat en fonction de la quantité saisie
          /* string label = "Efficacité raffinage -30 % (base atelier Impériale)\n" +
            $"x{reprocessingSkill} (Raffinage)\n" +
            $"x{efficiencySkill} (Raffinage Efficace)\n" +
            $"x{reproGradeSkill} (Raffinage Expert)\n" +
            $"x{connectionSkill} (Taxes Quartier Promenade)\n" +
            $"Total : {total} matérias de qualité {selectedResource.grade}";*/
          //TODO : Upgrade button tooltip : afficher ces infos dans le tooltip avec la .35 de nwn
          /*string label = "Efficacité concentration -30 % (base atelier Impérial)\n" +
            $"x{concentrationSkill} (Concentration)\n" +
            $"x{connectionSkill} (Taxes Quartier Promenade)\n" +
            $"Total : {total} matérias raffinées de qualité {selectedResource.grade + 1}";*/

          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 40 });
          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = /*player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] :*/ new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 400);

          window = new NuiWindow(rootColumn, "Raffinerie")
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
            nuiToken.OnNuiEvent += HandleRefineryEvents;

            LoadMateriaList();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleRefineryEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "refine":

                  selectedResource = playerCraftResourceList.ElementAt(nuiEvent.ArrayIndex);
                  string selectedQuantity = input.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!int.TryParse(selectedQuantity, out int quantity) || quantity > selectedResource.quantity || quantity < 0)
                    quantity = selectedResource.quantity;

                  selectedResource.quantity -= quantity;

                  //int grade = selectedResource.grade;
                  int refinedAmount = player.GetMateriaYieldFromResource(quantity, selectedResource);

                  /*if (HandleRefinerLuck())
                    if (grade == 8)
                    {
                      refinedAmount *= 50 / 100;
                      player.oid.SendServerMessage("Quelle chance, vous parvenez à raffiner 50 % plus de matéria que d'ordinaire !", new Color(32, 255, 32));
                    }
                    else
                    {
                      grade += 1;
                      player.oid.SendServerMessage("Quelle chance, vous parvenez à obtenir une matéria raffinée de plus grande qualité !", new Color(32, 255, 32));
                    }
                  */
                  CraftResource refinedMateria = player.craftResourceStock.FirstOrDefault(r => r.type ==  ResourceType.InfluxRaffine);

                  if (refinedMateria != null)
                    refinedMateria.quantity += refinedAmount;
                  else
                    player.craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == ResourceType.InfluxRaffine), refinedAmount));

                  player.oid.SendServerMessage($"Vous raffinez {refinedAmount} unité(s) d'influx", ColorConstants.Orange);

                  LoadMateriaList();

                  return;

                case "upgrade":

                  selectedResource = playerCraftResourceList.ElementAt(nuiEvent.ArrayIndex);
                  string inputQuantity = input.GetBindValues(player.oid, nuiToken.Token)[nuiEvent.ArrayIndex];

                  if (!double.TryParse(inputQuantity, out double amount) || amount > selectedResource.quantity || amount < 0)
                    amount = selectedResource.quantity;

                  selectedResource.quantity -= (int)amount;

                  //int selectedGrade = selectedResource.grade + 1;
                  double concentrationSkill = 1.00 + 5 * player.learnableSkills[CustomSkill.MateriaGradeConcentration].totalPoints / 100.0;
                  double connectionSkill = player.learnableSkills.TryGetValue(CustomSkill.ConnectionsPromenade, out var value) ? 0.95 + value.totalPoints / 100.0 : 1.00;
                  double total = amount / 3.0 * concentrationSkill * 0.3; // le * 0.3 représente la qualité de la fonderie de base. Il faudra le variabiliser lorsqu'il sera possible de créer des ateliers de différente qualité
                  total *= connectionSkill;

                  /*if (HandleRefinerLuck())
                    selectedGrade += 1;*/

                  CraftResource materia = player.craftResourceStock.FirstOrDefault(r => r.type == selectedResource.type);

                  if (materia != null)
                    materia.quantity += (int)total;
                  else
                    player.craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == selectedResource.type), (int)total));

                  player.oid.SendServerMessage($"Vous parvenez à concentrer {((int)total).ToString().ColorString(ColorConstants.White)} unité(s) de {selectedResource.type.ToDescription().ColorString(ColorConstants.White)}", ColorConstants.Orange);

                  LoadMateriaList();

                  return;
              }
              break;
          }
        }

        private void LoadMateriaList()
        {
          List<string> materiaNamesList = new List<string>();
          List<string> materiaIconList = new List<string>();
          List<string> quantityList = new List<string>();
          List<bool> enabledList = new List<bool>();
          playerCraftResourceList = player.craftResourceStock.Where(c => c.type == ResourceType.InfluxBrut && c.quantity > 0);

          foreach (CraftResource resource in playerCraftResourceList)
          {
            materiaIconList.Add(resource.iconString);
            materiaNamesList.Add($"{resource.name} (x{resource.quantity})");
            quantityList.Add(resource.quantity.ToString());
            enabledList.Add(player.learnableSkills.ContainsKey(CustomSkill.MateriaGradeConcentration));
          }

          materiaIcon.SetBindValues(player.oid, nuiToken.Token, materiaIconList);
          materiaNames.SetBindValues(player.oid, nuiToken.Token, materiaNamesList);
          input.SetBindValues(player.oid, nuiToken.Token, quantityList);
          concentrateEnabled.SetBindValues(player.oid, nuiToken.Token, enabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, playerCraftResourceList.Count());
        }
        private bool HandleRefinerLuck()
        {
          if (!player.learnableSkills.TryGetValue(CustomSkill.ReprocessingInfluxLuck, out var value))
            return false;

          if (NwRandom.Roll(Utils.random, 100) > value.totalPoints)
            return false;

          player.oid.SendServerMessage("Votre concentration hors du commun vous permet d'obtenir un influx bien plus concentrée qu'attendu !", ColorConstants.Rose);
          return true;
        }
      }
    }
  }
}
