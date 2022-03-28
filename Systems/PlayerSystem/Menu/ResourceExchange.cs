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
      public class ResourceExchangeWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new NuiColumn();
        private readonly List<NuiElement> rootChildren = new List<NuiElement>();
        private readonly List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>();
        private readonly List<NuiListTemplateCell> targetRowTemplate = new List<NuiListTemplateCell>();

        private readonly NuiBind<string> myGold = new NuiBind<string>("myGold");
        private readonly NuiBind<string> myResourceNames = new NuiBind<string>("myResourceNames");
        private readonly NuiBind<int> myListCount = new NuiBind<int>("myListCount");
        private readonly NuiBind<string> myResourceIcon = new NuiBind<string>("myResourceIcon");
        private readonly NuiBind<string> myQuantity = new NuiBind<string>("myQuantity");

        private readonly NuiBind<string> targetGold = new NuiBind<string>("targetGold");
        private readonly NuiBind<string> targetResourceNames = new NuiBind<string>("targetResourceNames");
        private readonly NuiBind<int> targetListCount = new NuiBind<int>("targetListCount");
        private readonly NuiBind<string> targetResourceIcon = new NuiBind<string>("targetResourceIcon");
        private readonly NuiBind<string> targetQuantity = new NuiBind<string>("targetQuantity");

        private bool AuthorizeSave { get; set; }
        private int nbDebounce { get; set; }
        List<CraftResource> myResourceList;
        Player targetPlayer;

        public ResourceExchangeWindow(Player player, Player target) : base(player)
        {
          windowId = "resourceExchange";
          AuthorizeSave = false;
          nbDebounce = 0;

          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(myResourceIcon) { Tooltip = myResourceNames, Height = 35 }) { Width = 80 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiLabel(myResourceNames) { Tooltip = myResourceNames, VerticalAlign = NuiVAlign.Middle }));
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", myQuantity, 60, false)));

          targetRowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage(targetResourceIcon) { Tooltip = targetResourceNames, Height = 35 }) { Width = 80 });
          targetRowTemplate.Add(new NuiListTemplateCell(new NuiLabel(targetResourceNames) { Tooltip = targetResourceNames, VerticalAlign = NuiVAlign.Middle }));
          targetRowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", targetQuantity, 60, false)));

          rootColumn.Children = rootChildren;

          CreateWindow(target);
        }

        public void CreateWindow(Player target)
        {
          targetPlayer = target;
          rootChildren.Clear();

          rootChildren.Add(new NuiRow() { Height = 35, Children = new List<NuiElement>() 
          { 
            new NuiSpacer(),
            new NuiText("Votre proposition"),
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow()
          {
            Height = 35,
            Children = new List<NuiElement>()
            {
              new NuiLabel($"Or ({player.bankGold}) : "),
              new NuiTextEdit("", myGold, 60, false)
            }
          });

          rootChildren.Add(new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiList(rowTemplate, myListCount) { RowHeight = 35 } } });

          rootChildren.Add(new NuiRow()
          {
            Height = 35,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiText($"Proposition de {targetPlayer.oid.LoginCreature.Name}"),
              new NuiSpacer()
            }
          });

          rootChildren.Add(new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiLabel(targetGold) } });
          rootChildren.Add(new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiList(targetRowTemplate, targetListCount) { RowHeight = 35 } } });

          rootChildren.Add(new NuiRow()
          {
            Height = 35,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Envoyer") { Id = "send", Tooltip = "Affiche votre proposition dans la fenêtre du destinataire." },
              new NuiButton("Valider") { Id = "confirm", Tooltip = "Finalise la transaction après confirmation des deux parties impliquées." },
              new NuiButton("Annuler") { Id = "confirm", Tooltip = "Annule la validation et permet de modifier à nouveau la proposition." },
              new NuiSpacer()
            }
          });

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.65f);

          window = new NuiWindow(rootColumn, $"Proposition d'échange de ressources entre {player.oid.LoginCreature.Name} et {target.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleResourceExchangeEvents;
          player.oid.OnNuiEvent += HandleResourceExchangeEvents;
          player.oid.OnServerSendArea -= OnAreaChangeCloseWindow;
          player.oid.OnServerSendArea += OnAreaChangeCloseWindow;

          token = player.oid.CreateNuiWindow(window, windowId);

          myGold.SetBindValue(player.oid, token, "0");
          targetGold.SetBindValue(player.oid, token, "0");

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          LoadResourceList();
        }

        private void HandleResourceExchangeEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              // OnClose => Fermer la fenêtre du destinataire si celle-ci est toujours ouverte
              break;
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "send":

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

          while (input > 0)
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

          if (!Enum.TryParse(item.GetObjectVariable<LocalVariableString>("CRAFT_RESOURCE").Value, out ResourceType type))
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
          catch (Exception)
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
          List<string> resourceQuantityList = new List<string>();
          myResourceList = player.craftResourceStock.Where(r => r.quantity > 0).OrderBy(r => r.type).ThenBy(r => r.grade).ToList();

          foreach (CraftResource resource in myResourceList)
          {
            resourceNameList.Add($"{resource.name} (x{resource.quantity})");
            resourceIconList.Add(resource.iconString);
            resourceQuantityList.Add("0");
          }

          myResourceNames.SetBindValues(player.oid, token, resourceNameList);
          myResourceIcon.SetBindValues(player.oid, token, resourceIconList);
          myQuantity.SetBindValues(player.oid, token, resourceQuantityList);
          myListCount.SetBindValue(player.oid, token, myResourceList.Count());
        }
      }
    }
  }
}
