using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ResourceStorageWindow : PlayerWindow
      {
        NuiColumn rootColumn { get; }
        private readonly NuiBind<string> resourceNames = new NuiBind<string>("resourceNames");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly NuiBind<string> resourceIcon = new NuiBind<string>("resourceIcon");
        private readonly NuiBind<int> resourceType = new NuiBind<int>("resourceType");

        private bool AuthorizeSave { get; set; }
        private int nbDebounce { get; set; }
        private CraftResource resourceSelection { get; set; }

        public ResourceStorageWindow(Player player) : base(player)
        {
          windowId = "resourceStorage";
          AuthorizeSave = false;
          nbDebounce = 0;

          List<NuiComboEntry> comboValues = new List<NuiComboEntry>
          {
            new NuiComboEntry("Tous", 0),
            new NuiComboEntry("Minerai", 1),
            new NuiComboEntry("Lingots", 2),
            new NuiComboEntry("Bûches", 3),
            new NuiComboEntry("Planches", 4),
            new NuiComboEntry("Peaux", 5),
            new NuiComboEntry("Cuirs", 6),
            new NuiComboEntry("Plantes", 7),
            new NuiComboEntry("Poudres", 8)
          };

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage(resourceIcon) { Id = "dropThis", Tooltip = "Dépôt", Height = 35 }) { Width = 80 },
            new NuiListTemplateCell(new NuiLabel(resourceNames) { Id = "dropThisMouseDown", Tooltip = resourceNames, VerticalAlign = NuiVAlign.Middle } ),
            new NuiListTemplateCell(new NuiButton("Retrait") { Id = "withraw"} ),
          };

          rootColumn = new NuiColumn()
          {
            Children = new List<NuiElement>()
            {
              new NuiRow() 
              { 
                Height = 35, 
                Children = new List<NuiElement>() 
                { 
                  new NuiCombo 
                  {
                    Selected = resourceType,
                    Entries = new List<NuiComboEntry>
                    {
                      new NuiComboEntry("Tous", 0),
                      new NuiComboEntry("Minerai", 1),
                      new NuiComboEntry("Lingots", 2),
                      new NuiComboEntry("Bûches", 3),
                      new NuiComboEntry("Planches", 4),
                      new NuiComboEntry("Peaux", 5),
                      new NuiComboEntry("Cuirs", 6),
                      new NuiComboEntry("Plantes", 7),
                    }, 
                  },
                  new NuiButton("Tout déposer") { Id = "dropCategory", Tooltip = "Dépose toutes les resources de la catégories sélectionnée." }
                } 
              },
              new NuiList(rowTemplate, listCount) { RowHeight = 35 },
              new NuiRow()
              {
                Height = 35,
                Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiButton("Activer mode dépôt") { Id = "activateDropSelection", Width = 160 },
                  new NuiSpacer()
                }
              },
            }
          };

          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, $"Entrepôt de ressources de {player.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleResourceStorageEvents;
          player.oid.OnNuiEvent += HandleResourceStorageEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          resourceType.SetBindValue(player.oid, token, 0);
          resourceType.SetBindWatch(player.oid, token, true);
          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          resourceSelection = player.craftResourceStock.FirstOrDefault();
          LoadResourceList();
        }

        private void HandleResourceStorageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              StorageSave();
              break;
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "withdraw":

                  resourceSelection = player.craftResourceStock[nuiEvent.ArrayIndex];

                  if (player.windows.ContainsKey("playerInput"))
                    ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Retirer combien d'unités ?", WithdrawResource, resourceSelection.quantity.ToString());
                  else
                    player.windows.Add("playerInput", new PlayerInputWindow(player, "Retirer combien d'unités ?", WithdrawResource, resourceSelection.quantity.ToString()));

                  break;

                case "itemDeposit":

                  player.oid.SendServerMessage("Sélectionnez les objets de votre inventaire à déposer au coffre.");
                  player.oid.EnterTargetMode(SelectInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);
                  
                  break;

                case "dropThis":
                  StoreSelectedResource(player.craftResourceStock[nuiEvent.ArrayIndex]);
                  break;
              }

              break;

            case NuiEventType.MouseDown:

              switch (nuiEvent.ElementId)
              {
                case "dropThisMouseDown":
                  StoreSelectedResource(player.craftResourceStock[nuiEvent.ArrayIndex]);
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "resourceType":
                  LoadResourceList();
                  break;
              }
              break;
          }
        }
        private void StoreSelectedResource(CraftResource selectedResource)
        {
          foreach (NwItem resource in player.oid.LoginCreature.Inventory.Items.
            Where(r => r.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value == selectedResource.name
            && r.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value == selectedResource.grade))
          {
            selectedResource.quantity += resource.StackSize;
            resource.Destroy();
          }

          StorageSave();
          LoadResourceList();
        }
        private bool WithdrawResource(string inputValue)
        {
          if (!int.TryParse(inputValue, out int input) || resourceSelection == null || resourceSelection.quantity < input)
          {
            player.oid.SendServerMessage("Vous ne disposez pas d'une telle quantité de cette ressource.", ColorConstants.Red);
            return true;
          }

          while(input > 0)
          {
            if (input > 50000)
            {
              Craft.Collect.System.CreateSelectedResourceInInventory(resourceSelection, player, 50000);
              input -= 50000;
              resourceSelection.quantity -= 50000;
            }
            else
            {
              Craft.Collect.System.CreateSelectedResourceInInventory(resourceSelection, player, input);
              resourceSelection.quantity -= input;
              input = 0;
            }
          }

          StorageSave();
          LoadResourceList();

          return true;
        }
        private void SelectInventoryItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || !(selection.TargetObject is NwItem item))
            return;

          if (item.Tag != "craft_resource")
          {
            player.oid.SendServerMessage($"{item.Name.ColorString(ColorConstants.White)} n'est pas une resource artisanale !", ColorConstants.Red);
            return;
          }

          if(!Enum.TryParse(item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value, out ResourceType type))
          {
            player.oid.SendServerMessage($"ERREUR TECHNIQUE - {item.Name.ColorString(ColorConstants.White)} n'a pas été identifié comme une resource artisanale. Le staff a été averti", ColorConstants.Red);
            Utils.LogMessageToDMs($"{item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value} utilisé par {player.oid.LoginCreature.Name} n'a pas pu être parsé comme ressource de craft.");
            return;
          }

          try
          {
            CraftResource resource = player.craftResourceStock.First(r => r.type == type && r.grade == item.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value);
            resource.quantity += item.StackSize;
          }
          catch(Exception)
          {
            player.craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == type && r.grade == item.GetObjectVariable<LocalVariableInt>("CRAFT_GRADE").Value), item.StackSize));
          }

          item.Destroy();

          StorageSave();
          LoadResourceList();

          player.oid.EnterTargetMode(SelectInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);
        }
        public void StorageSave()
        {
          DateTime elapsed = DateTime.Now;

          if (!AuthorizeSave)
          {
            if (nbDebounce > 0)
            {
              nbDebounce += 1;
              return;
            }
            else
            {
              nbDebounce = 1;
              Log.Info($"Character {player.characterId} : scheduling storage save in 10s");
              DebounceStorageSave(nbDebounce);
              return;
            }
          }
          else
            HandleStorageSave();

          Log.Info($"Character {player.characterId} storage saved in : {(DateTime.Now - elapsed).TotalSeconds} s");
        }

        private async void DebounceStorageSave(int initialNbDebounce)
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task awaitDebounce = NwTask.WaitUntil(() => nbDebounce != initialNbDebounce, tokenSource.Token);
          Task awaitSaveAuthorized = NwTask.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);

          await NwTask.WhenAny(awaitDebounce, awaitSaveAuthorized);
          tokenSource.Cancel();

          if (awaitDebounce.IsCompletedSuccessfully)
          {
            nbDebounce += 1;
            DebounceStorageSave(initialNbDebounce + 1);
            return;
          }

          if (awaitSaveAuthorized.IsCompletedSuccessfully)
          {
            nbDebounce = 0;
            AuthorizeSave = true;
            Log.Info($"Character {player.characterId} : debounce done after {nbDebounce} triggers, storage save authorized");
            StorageSave();
          }
        }

        private async void HandleStorageSave()
        {
          Task<string> serializeCraftResource = Task.Run(() => JsonConvert.SerializeObject(player.craftResourceStock));
          await Task.WhenAll(serializeCraftResource);

          SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "materialStorage", serializeCraftResource.Result } },
          new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });

          nbDebounce = 0;
          AuthorizeSave = false;
        }
        private void LoadResourceList()
        {
          List<string> resourceNameList = new List<string>();
          List<string> resourceIconList = new List<string>();
          var tempList = resourceType.GetBindValue(player.oid, token) > 0 ? player.craftResourceStock.Where(r => r.type == (ResourceType)resourceType.GetBindValue(player.oid, token)) : player.craftResourceStock;

          foreach (CraftResource resource in tempList)
          {
            resourceNameList.Add($"{resource.name} (x{resource.quantity})");
            resourceIconList.Add(resource.iconString);
          }

          resourceNames.SetBindValues(player.oid, token, resourceNameList);
          resourceIcon.SetBindValues(player.oid, token, resourceIconList);
          listCount.SetBindValue(player.oid, token, tempList.Count());
        }
      }
    }
  }
}
