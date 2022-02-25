using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

using NWN.Systems.Craft;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class WorkshopWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn;
        private readonly NuiGroup rootGroup;
        private readonly List<NuiElement> rootChidren = new List<NuiElement>();
        private readonly NuiBind<string> search = new NuiBind<string>("search");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly NuiBind<string> icon = new NuiBind<string>("icon"); // TODO : Utiliser l'icone d'un don correspondant à l'arme ?
        private readonly NuiBind<string> blueprintNames = new NuiBind<string>("blueprintName");
        private readonly NuiBind<string> blueprintTEs = new NuiBind<string>("blueprintTEs");
        private readonly NuiBind<string> blueprintMEs = new NuiBind<string>("blueprintMEs");
        private readonly NuiBind<bool> enable = new NuiBind<bool>("enable");
        private readonly NuiColor white = new NuiColor(255, 255, 255);
        private readonly NuiRect drawListRect = new NuiRect(0, 35, 150, 60);

        public WorkshopWindow(Player player, string placeableTag) : base(player)
        {
          windowId = "forge";

          List<NuiListTemplateCell> blueprintTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(icon) { Tooltip = blueprintNames, Height = 40, Width = 40 }) { Width = 40 },
            new NuiListTemplateCell(new NuiLabel(blueprintMEs)
            {
              Width = 160, Tooltip = blueprintNames,
              DrawList = new List<NuiDrawListItem>() { new NuiDrawListText(white, drawListRect, blueprintTEs) }
            }) { Width = 160 },
            new NuiListTemplateCell(new NuiButton("Produire") { Id = "startCraft", Enabled = enable, Tooltip = "Entame une nouvelle production artisanale. Nécessite d'avoir au moins un niveau d'entrainement dans le métier artisanal correspondant.", Height = 40, Width = 90 }) { Width = 90 }
          };

          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "forgeGroup", Border = true, Layout = rootColumn };

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } });
          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(blueprintTemplate, listCount) { RowHeight = 45 } } });

          CreateWindow(placeableTag);
        }

        public void CreateWindow(string placeableTag)
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, "Production artisanale")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleWorkshopEvents;
          player.oid.OnNuiEvent += HandleWorkshopEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          search.SetBindValue(player.oid, token, "");
          search.SetBindWatch(player.oid, token, true);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          LoadBlueprintList(placeableTag);
        }

        private void HandleWorkshopEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              BankSave();
              break;
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "goldDeposit":

                  if (player.windows.ContainsKey("playerInput"))
                    ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString());
                  else
                    player.windows.Add("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString()));

                  break;
                case "goldWithdraw":

                  if (player.windows.ContainsKey("playerInput"))
                    ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString());
                  else
                    player.windows.Add("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString()));

                  break;

                case "itemDeposit":

                  player.oid.SendServerMessage("Sélectionnez les objets de votre inventaire à déposer au coffre.");
                  player.oid.EnterTargetMode(SelectInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);

                  break;
              }

              break;

            case NuiEventType.MouseDown:

              switch (nuiEvent.ElementId)
              {
                case "examiner":
                  if (player.windows.ContainsKey("itemExamine"))
                    ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(items.ElementAt(nuiEvent.ArrayIndex));
                  else
                    player.windows.Add("itemExamine", new ItemExamineWindow(player, items.ElementAt(nuiEvent.ArrayIndex)));
                  break;

                case "takeItem":
                  player.oid.ControlledCreature.AcquireItem(items.ElementAt(nuiEvent.ArrayIndex));
                  RemoveItemFromList(nuiEvent.ArrayIndex);
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, token).ToLower();
                  var filteredList = items.AsEnumerable();

                  if (!string.IsNullOrEmpty(currentSearch))
                    filteredList = filteredList.Where(s => s.Name.ToLower().Contains(currentSearch));

                  LoadBankItemList(filteredList);

                  break;
              }
              break;
          }
        }
        private void LoadBlueprintList(string workshopTag)
        {
          List<string> blueprintNamesList = new List<string>();
          List<string> iconList = new List<string>();
          List<string> blueprintTEsList = new List<string>();
          List<string> blueprintMEsList = new List<string>();
          List<bool> enabledList = new List<bool>();

          foreach (NwItem item in player.oid.ControlledCreature.Inventory.Items.Where(i => i.Tag == "blueprint"))
          {
            Blueprint blueprint = Craft.Collect.System.blueprintDictionnary[item.GetObjectVariable<LocalVariableInt>("_BASE_ITEM_TYPE").Value];

            if (blueprint.workshopTag != workshopTag)
              continue;

            TimeSpan jobDuration = TimeSpan.FromSeconds(blueprint.GetBlueprintTimeCostForPlayer(player, item));

            blueprintNamesList.Add(item.Name);
            iconList.Add(NwBaseItem.FromItemId(blueprint.baseItemType).WeaponFocusFeat.IconResRef);
            blueprintMEsList.Add($"Recherche en rendement : {item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_MATERIAL_EFFICIENCY").Value} - Coût initial en {blueprint.resourceType.ToDescription()} : {blueprint.GetBlueprintMineralCostForPlayer(player, item)}");
            blueprintTEsList.Add($"Recherche en efficacité : {item.GetObjectVariable<LocalVariableInt>("_BLUEPRINT_TIME_EFFICIENCY").Value} - Temps de fabrication et d'amélioration : {new TimeSpan(jobDuration.Days, jobDuration.Hours, jobDuration.Minutes, jobDuration.Seconds)}");
            enabledList.Add(player.learnableSkills.ContainsKey(blueprint.jobFeat));
          }

          blueprintNames.SetBindValues(player.oid, token, blueprintNamesList);
          listCount.SetBindValue(player.oid, token, blueprintNamesList.Count);

          icon.SetBindValues(player.oid, token, iconList);
          blueprintMEs.SetBindValues(player.oid, token, blueprintMEsList);
          blueprintTEs.SetBindValues(player.oid, token, blueprintTEsList);
          enable.SetBindValues(player.oid, token, enabledList);
        }
      }
    }
  }
}
