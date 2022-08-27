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
      public class BankStorageWindow : PlayerWindow
      {
        NuiColumn rootColumn { get; }
        private readonly NuiBind<string> gold = new ("gold");
        private readonly NuiBind<string> search = new ("search");
        private readonly NuiBind<string> itemNames = new ("itemNames");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> topIcon = new ("topIcon");
        private readonly NuiBind<string> midIcon = new ("midIcon");
        private readonly NuiBind<string> botIcon = new ("botIcon");
        private readonly NuiBind<bool> enabled = new ("enabled");
        private readonly NuiBind<NuiRect> imagePosition = new ("rect");
        private readonly List<NwItem> items = new ();
        private IEnumerable<NwItem> filteredList;

        private bool AuthorizeSave { get; set; }
        private int nbDebounce { get; set; }

        public BankStorageWindow(Player player) : base(player)
        {
          windowId = "bankStorage";
          AuthorizeSave = false;
          nbDebounce = 0;

          DeserializeBankItemList();

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButton("Examiner") { Id = "examiner", Height = 35 }) { Width = 80 },
            new NuiListTemplateCell(new NuiSpacer()
            {
              Height = 125, Id = "takeItem",
              DrawList = new List<NuiDrawListItem>()
              {
                new NuiDrawListImage(topIcon, imagePosition),
                new NuiDrawListImage(midIcon, imagePosition) { Enabled = enabled },
                new NuiDrawListImage(botIcon, imagePosition) { Enabled = enabled }

              }
            }) { Width = 45 },
            new NuiListTemplateCell(new NuiLabel(itemNames) { Id = "takeItem", VerticalAlign = NuiVAlign.Middle } )
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
                  new NuiLabel("Pièces d'or : ") { Width = 120, VerticalAlign = NuiVAlign.Middle },
                  new NuiLabel(gold) { Width = 120, VerticalAlign = NuiVAlign.Middle },
                  new NuiButton("Dépôt") { Id = "goldDeposit", Width = 80 },
                  new NuiButton("Retrait") { Id = "goldWithdraw", Width = 80 }
                }
              },
              new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } },
              new NuiList(rowTemplate, listCount) { RowHeight = 75 },
              new NuiRow()
              {
                Height = 35,
                Children = new List<NuiElement>()
                {
                  new NuiSpacer(),
                  new NuiButton("Activer mode dépôt") { Id = "itemDeposit", Width = 160 },
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

          window = new NuiWindow(rootColumn, $"Coffre privé de {player.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            nuiToken.OnNuiEvent += HandleBankStorageEvents;
            player.oid.OnServerSendArea += OnAreaChangeCloseWindow;
          }

            search.SetBindValue(player.oid, nuiToken.Token, "");
          search.SetBindWatch(player.oid, nuiToken.Token, true);
          geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
          geometry.SetBindWatch(player.oid, nuiToken.Token, true);

          filteredList = items;
          LoadBankItemList(filteredList);
        }

        private void HandleBankStorageEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch(nuiEvent.EventType)
          {
            case NuiEventType.Close:
              BankSave();
              break;
            case NuiEventType.Click:

              switch(nuiEvent.ElementId)
              {
                case "goldDeposit":

                  if (!player.windows.TryAdd("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString())))
                    ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString());

                  break;
                case "goldWithdraw":

                  if (!player.windows.TryAdd("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString())))
                    ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString());

                  break;

                case "itemDeposit":

                  player.oid.SendServerMessage("Sélectionnez les objets de votre inventaire à déposer au coffre.");
                  player.oid.EnterTargetMode(SelectInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);

                  break;
              }

            break;

            case NuiEventType.MouseDown:

              switch(nuiEvent.ElementId)
              {
                case "examiner":
                  if (!player.windows.TryAdd("itemExamine", new ItemExamineWindow(player, filteredList.ElementAt(nuiEvent.ArrayIndex))))
                    ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(filteredList.ElementAt(nuiEvent.ArrayIndex));
                  break;

                case "takeItem":
                  player.oid.ControlledCreature.AcquireItem(filteredList.ElementAt(nuiEvent.ArrayIndex));
                  RemoveItemFromList(nuiEvent.ArrayIndex);
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch(nuiEvent.ElementId)
              {
                case "search":

                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  filteredList = items;

                  if (!string.IsNullOrEmpty(currentSearch))
                    filteredList = filteredList.Where(s => s.Name.ToLower().Contains(currentSearch));

                  LoadBankItemList(filteredList);

                  break;
              }
              break;
          }
        }
        private bool DepositGold(string inputValue)
        {
          if(!uint.TryParse(inputValue, out uint inputGold) || inputGold > player.oid.LoginCreature.Gold)
          {
            player.oid.SendServerMessage("Vous ne disposez pas d'autant d'or.", ColorConstants.Red);
            return true;
          }

          player.oid.LoginCreature.Gold -= inputGold;
          player.bankGold += (int)inputGold;
          gold.SetBindValue(player.oid, nuiToken.Token, player.bankGold.ToString());

          return true;
        }
        private bool WithdrawGold(string inputValue)
        {
          if (!int.TryParse(inputValue, out int inputGold) || inputGold > player.bankGold)
          {
            player.oid.SendServerMessage("Vous ne disposez pas d'autant d'or.", ColorConstants.Red);
            return true;
          }

          player.bankGold -= inputGold;
          player.oid.LoginCreature.Gold += (uint)inputGold;
          gold.SetBindValue(player.oid, nuiToken.Token, player.bankGold.ToString());

          return true;
        }
        private void RemoveItemFromList(int index)
        {
          items.Remove(filteredList.ElementAt(index));

          List<string> tempList = itemNames.GetBindValues(player.oid, nuiToken.Token);
          tempList.RemoveAt(index);
          itemNames.SetBindValues(player.oid, nuiToken.Token, tempList);

          tempList = topIcon.GetBindValues(player.oid, nuiToken.Token);
          tempList.RemoveAt(index);
          topIcon.SetBindValues(player.oid, nuiToken.Token, tempList);

          tempList = midIcon.GetBindValues(player.oid, nuiToken.Token);
          tempList.RemoveAt(index);
          midIcon.SetBindValues(player.oid, nuiToken.Token, tempList);

          tempList = botIcon.GetBindValues(player.oid, nuiToken.Token);
          tempList.RemoveAt(index);
          botIcon.SetBindValues(player.oid, nuiToken.Token, tempList);

          List<bool> tempEnableList = enabled.GetBindValues(player.oid, nuiToken.Token);
          tempEnableList.RemoveAt(index);
          enabled.SetBindValues(player.oid, nuiToken.Token, tempEnableList);

          listCount.SetBindValue(player.oid, nuiToken.Token, listCount.GetBindValue(player.oid, nuiToken.Token) - 1);
        }
        private void AddItemToList(NwItem item)
        {
          items.Add(item);
          List<string> tempList = itemNames.GetBindValues(player.oid, nuiToken.Token);
          tempList.Add(item.Name);
          itemNames.SetBindValues(player.oid, nuiToken.Token, tempList);

          string[] tempArray = Utils.GetIconResref(item);

          tempList = topIcon.GetBindValues(player.oid, nuiToken.Token);
          tempList.Add(tempArray[0]);
          topIcon.SetBindValues(player.oid, nuiToken.Token, tempList);

          tempList = midIcon.GetBindValues(player.oid, nuiToken.Token);
          tempList.Add(tempArray[1]);
          midIcon.SetBindValues(player.oid, nuiToken.Token, tempList);

          tempList = botIcon.GetBindValues(player.oid, nuiToken.Token);
          tempList.Add(tempArray[2]);
          botIcon.SetBindValues(player.oid, nuiToken.Token, tempList);

          List<bool> tempEnableList = enabled.GetBindValues(player.oid, nuiToken.Token);
          tempEnableList.Add(!string.IsNullOrEmpty(tempArray[1]));
          enabled.SetBindValues(player.oid, nuiToken.Token, tempEnableList);

          List<NuiRect> imagePosList = imagePosition.GetBindValues(player.oid, nuiToken.Token);

          switch (item.BaseItem.ModelType)
          {
            case BaseItemModelType.Simple:
              imagePosList.Add(ItemUtils.GetItemCategory(item.BaseItem.ItemType) != ItemUtils.ItemCategory.Shield ? new NuiRect(0, 25, 25, 25) : new NuiRect(0, 15, 25, 25));
              break;
            case BaseItemModelType.Composite:
              imagePosList.Add(ItemUtils.GetItemCategory(item.BaseItem.ItemType) != ItemUtils.ItemCategory.Ammunition ? new NuiRect(0, 0, 25, 25) : new NuiRect(0, 25, 25, 25));
              break;
            case BaseItemModelType.Armor:
            case BaseItemModelType.Layered:
              imagePosList.Add(new NuiRect(0, 0, 25, 25));
              break;
          }

          imagePosition.SetBindValues(player.oid, nuiToken.Token, imagePosList);
          listCount.SetBindValue(player.oid, nuiToken.Token, listCount.GetBindValue(player.oid, nuiToken.Token) + 1);
        }
        private void SelectInventoryItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwItem item)
            return;
          
          AddItemToList(item);
          item.Destroy();
          player.oid.EnterTargetMode(SelectInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);
        }
        public void BankSave()
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
              Log.Info($"Character {player.characterId} : scheduling bank save in 10s");
              DebounceBankSave(nbDebounce);
              return;
            }
          }
          else
            HandleBankSave();

          Log.Info($"Character {player.characterId} bank saved in : {(DateTime.Now - elapsed).TotalSeconds} s");
        }

        private async void DebounceBankSave(int initialNbDebounce)
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task awaitDebounce = NwTask.WaitUntil(() => nbDebounce != initialNbDebounce, tokenSource.Token);
          Task awaitSaveAuthorized = NwTask.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);

          await NwTask.WhenAny( awaitDebounce, awaitSaveAuthorized);
          tokenSource.Cancel();

          if (awaitDebounce.IsCompletedSuccessfully)
          {
            nbDebounce += 1;
            DebounceBankSave(initialNbDebounce + 1);
            return;
          }

          if (awaitSaveAuthorized.IsCompletedSuccessfully)
          {
            nbDebounce = 0;
            AuthorizeSave = true;
            Log.Info($"Character {player.characterId} : debounce done after {nbDebounce} triggers, bank save authorized");
            BankSave();
          }
        }

        private async void HandleBankSave()
        {
          List<string> serializedItems = new List<string>();
          await Task.Run(() => SerializeItemList(serializedItems));

          Task<string> serializeBank = Task.Run(() => JsonConvert.SerializeObject(serializedItems));
          await serializeBank;

          SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "persistantStorage", serializeBank.Result } },
          new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });

          nbDebounce = 0;
          AuthorizeSave = false;
        }
        private List<string> SerializeItemList(List<string> serializedItems)
        {
          foreach (NwItem item in items)
            serializedItems.Add(item.Serialize().ToBase64EncodedString());

          return serializedItems;
        }
        private async void DeserializeBankItemList()
        {
          var result = SqLiteUtils.SelectQuery("playerCharacters",
            new List<string>() { { "persistantStorage" } },
            new List<string[]>() { { new string[] { "rowid", player.characterId.ToString() } } });

          if (result.Result != null)
          {
            string serializedBank = result.Result.GetString(0);
            List<string> serializedItems = new List<string>();

            Task loadBank = Task.Run(() =>
            {
              if (string.IsNullOrEmpty(serializedBank))
                return;

              serializedItems = JsonConvert.DeserializeObject<List<string>>(serializedBank);
            });

            await loadBank;

            foreach (string serializedItem in serializedItems)
              items.Add(NwItem.Deserialize(serializedItem.ToByteArray()));

            await NwTask.SwitchToMainThread();
          }
        }

        private void LoadBankItemList(IEnumerable<NwItem> filteredList)
        {
          List<string> itemNameList = new();
          List<string> topIconList = new();
          List<string> midIconList = new();
          List<string> botIconList = new();
          List<bool> enabledList = new();
          List<NuiRect> imagePosList = new();

          foreach (NwItem item in filteredList)
          {
            itemNameList.Add(item.BaseItem.IsStackable ? $"{item.Name} (x{item.StackSize})" : item.Name);
            string[] tempArray = Utils.GetIconResref(item);
            topIconList.Add(tempArray[0]);
            midIconList.Add(tempArray[1]);
            botIconList.Add(tempArray[2]);
            enabledList.Add(!string.IsNullOrEmpty(tempArray[1]));

            switch (item.BaseItem.ModelType)
            {
              case BaseItemModelType.Simple:
                imagePosList.Add(ItemUtils.GetItemCategory(item.BaseItem.ItemType) != ItemUtils.ItemCategory.Shield ? new NuiRect(0, 25, 25, 25) : new NuiRect(0, 15, 25, 25));
                break;
              case BaseItemModelType.Composite:
                imagePosList.Add(ItemUtils.GetItemCategory(item.BaseItem.ItemType) != ItemUtils.ItemCategory.Ammunition ? new NuiRect(0, 0, 25, 25) : new NuiRect(0, 25, 25, 25));
                break;
              case BaseItemModelType.Armor:
              case BaseItemModelType.Layered:
                imagePosList.Add(new NuiRect(0, 0, 25, 25));
                break;
            }
          }

          gold.SetBindValue(player.oid, nuiToken.Token, player.bankGold.ToString());

          itemNames.SetBindValues(player.oid, nuiToken.Token, itemNameList);
          listCount.SetBindValue(player.oid, nuiToken.Token, itemNameList.Count);

          topIcon.SetBindValues(player.oid, nuiToken.Token, topIconList);
          midIcon.SetBindValues(player.oid, nuiToken.Token, midIconList);
          botIcon.SetBindValues(player.oid, nuiToken.Token, botIconList);
          enabled.SetBindValues(player.oid, nuiToken.Token, enabledList);
          imagePosition.SetBindValues(player.oid, nuiToken.Token, imagePosList);
        }
      }
    }
  }
}
