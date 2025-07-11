using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly NuiBind<string> gold = new("gold");
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<string> itemNames = new("itemNames");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> topIcon = new("topIcon");
        private readonly NuiBind<string> midIcon = new("midIcon");
        private readonly NuiBind<string> botIcon = new("botIcon");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly NuiBind<NuiRect> imagePosition = new("rect");
        public List<NwItem> items { get; set; }
        private IEnumerable<NwItem> filteredList;

        private bool AuthorizeSave { get; set; }
        private int nbDebounce { get; set; }

        public BankStorageWindow(Player player) : base(player)
        {
          windowId = "bankStorage";
          AuthorizeSave = false;
          nbDebounce = 0;

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButtonImage("ir_examine") { Id = "examiner", Height = 35 }) { Width = 35 },
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
                  new NuiSpacer(),
                  new NuiButton("Missives") { Id = "mailBox", Tooltip = "Consulter la boîte aux lettres Skalsgard", Width = 160 },
                  new NuiSpacer()
                }
              },
              new NuiRow()
              {
                Height = 35,
                Children = new List<NuiElement>()
                {
                  new NuiLabel("Pièces d'or : ") { Width = 120, VerticalAlign = NuiVAlign.Middle },
                  new NuiLabel(gold) { Width = 120, VerticalAlign = NuiVAlign.Middle },
                  new NuiButton("Dépôt") { Id = "goldDeposit", Width = 80, Tooltip = "Frais de transaction : 5 %" },
                  new NuiButton("Retrait") { Id = "goldWithdraw", Width = 80, Tooltip = "Frais de transaction : 5 %" }
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
              } 
            }
          };

          CreateWindow();
        }

        public async void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, 600);

          window = new NuiWindow(rootColumn, $"Coffre privé de {player.oid.LoginCreature.Name}")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (items == null)
          {
            await DeserializeBankItemList();
            await NwTask.SwitchToMainThread();
          }

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;

            nuiToken.OnNuiEvent += HandleBankStorageEvents;
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
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              BankSave();
              break;
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "goldDeposit":

                  if (!player.windows.ContainsKey("playerInput")) player.windows.Add("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString()));
                  else ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", DepositGold, player.oid.LoginCreature.Gold.ToString());

                  break;
                case "goldWithdraw":

                  if (!player.windows.ContainsKey("playerInput")) player.windows.Add("playerInput", new PlayerInputWindow(player, "Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString()));
                  else ((PlayerInputWindow)player.windows["playerInput"]).CreateWindow("Déposer combien d'or ?", WithdrawGold, player.bankGold.ToString());

                  break;

                case "itemDeposit":

                  player.oid.SendServerMessage("Sélectionnez les objets de votre inventaire à déposer au coffre.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectInventoryItem, Config.selectItemTargetMode);

                  break;

                case "mailBox":

                  CloseWindow();

                  if (!player.windows.TryGetValue("mailBox", out var mailBox)) player.windows.Add("mailBox", new MailBox(player, player));
                  else ((MailBox)mailBox).CreateWindow(player);


                  break;
              }

              break;

            case NuiEventType.MouseDown:

              switch (nuiEvent.ElementId)
              {
                case "examiner":

                  if (!player.windows.ContainsKey("itemExamine")) player.windows.Add("itemExamine", new ItemExamineWindow(player, filteredList.ElementAt(nuiEvent.ArrayIndex)));
                  else ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(filteredList.ElementAt(nuiEvent.ArrayIndex));

                  break;

                case "takeItem":
                  NwItem item = filteredList.ElementAt(nuiEvent.ArrayIndex);

                  if (item != null && item.IsValid)
                  {
                    player.oid.ControlledCreature.AcquireItem(item);
                    LogUtils.LogMessage($"{player.oid.LoginCreature.Name} ({player.oid.PlayerName}) retire {item.Name}", LogUtils.LogType.PersonalStorageSystem);
                  }
                  else
                    LogUtils.LogMessage($"{player.oid.LoginCreature.Name} trying to take an invalid item.", LogUtils.LogType.PersonalStorageSystem);

                  items.Remove(item);
                  UpdateItemList();
                  BankSave();
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "search": UpdateItemList(); break;
              }

              break;
          }
        }
        private void UpdateItemList()
        {
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          filteredList = items;

          if (!string.IsNullOrEmpty(currentSearch))
            filteredList = filteredList.Where(s => s.Name.ToLower().Contains(currentSearch));

          LoadBankItemList(filteredList);
        }
        private bool DepositGold(string inputValue)
        {
          if (!uint.TryParse(inputValue, out uint inputGold) || inputGold > player.oid.LoginCreature.Gold)
          {
            player.oid.SendServerMessage("Vous ne disposez pas d'autant d'or.", ColorConstants.Red);
            return true;
          }



          player.oid.LoginCreature.Gold -= inputGold;
          player.bankGold += (int)(inputGold * 0.95);
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
          player.oid.LoginCreature.Gold += (uint)(inputGold * 0.95);
          gold.SetBindValue(player.oid, nuiToken.Token, player.bankGold.ToString());

          return true;
        }
        private void SelectInventoryItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwItem item || item == null || !item.IsValid || item.RootPossessor != player.oid.LoginCreature)
            return;

          if(item.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").HasValue)
          {
            player.oid.SendServerMessage("Impossible de déposer une arme liée", ColorConstants.Red);
            player.oid.EnterTargetMode(SelectInventoryItem, Config.selectItemTargetMode);
            return;
          }

          LogUtils.LogMessage($"{player.oid.LoginCreature.Name} ({player.oid.PlayerName}) dépose {item.Name}", LogUtils.LogType.PersonalStorageSystem);
          items.Add(NwItem.Deserialize(item.Serialize()));
          item.Destroy();
          UpdateItemList();
          BankSave();

          player.oid.EnterTargetMode(SelectInventoryItem, Config.selectItemTargetMode);
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
              LogUtils.LogMessage($"Character {player.characterId} : scheduling bank save in 10s", LogUtils.LogType.PersonalStorageSystem);
              DebounceBankSave(nbDebounce);
              return;
            }
          }
          else
            HandleBankSave();

          LogUtils.LogMessage($"Character {player.characterId} bank saved in : {(DateTime.Now - elapsed).TotalSeconds} s", LogUtils.LogType.PersonalStorageSystem);
        }
        
        private async void DebounceBankSave(int initialNbDebounce)
        {
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task awaitPlayerLeave = NwTask.WaitUntilValueChanged(() => player.pcState == PcState.Offline, tokenSource.Token);
          Task awaitDebounce = NwTask.WaitUntil(() => nbDebounce != initialNbDebounce, tokenSource.Token);
          Task awaitSaveAuthorized = NwTask.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);

          await NwTask.WhenAny(awaitPlayerLeave, awaitDebounce, awaitSaveAuthorized);
          tokenSource.Cancel();

          if (awaitDebounce.IsCompletedSuccessfully)
          {
            nbDebounce += 1;
            DebounceBankSave(nbDebounce);
            return;
          }

          if (awaitPlayerLeave.IsCompletedSuccessfully || awaitSaveAuthorized.IsCompletedSuccessfully)
          {
            nbDebounce = 0;
            AuthorizeSave = true;
            LogUtils.LogMessage($"Character {player.characterId} : debounce done after {nbDebounce} triggers, bank save authorized", LogUtils.LogType.PersonalStorageSystem);
            BankSave();
          }
        }

        private async void HandleBankSave()
        {
          List<string> serializedItems = new List<string>();

          await NwTask.Run(async () => { await SerializeItems(serializedItems); });

          Task<string> serializeBank = Task.Run(() => JsonConvert.SerializeObject(serializedItems));
          await serializeBank;

          SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "persistantStorage", serializeBank.Result } },
          new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });

          nbDebounce = 0;
          AuthorizeSave = false;
        }
        private async Task SerializeItems(List<string> serializedItems)
        {
          Queue<NwItem> serializeQueue = new Queue<NwItem>(items);
          while (serializeQueue.Count > 0)
          {
            NwItem item = serializeQueue.Dequeue();
            serializedItems.Add(item.Serialize().ToBase64EncodedString());

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (serializeQueue.Count > 0 && stopwatch.Elapsed.TotalMilliseconds < Config.MaxSerializeTimeMs)
            {
              item = serializeQueue.Dequeue();
              serializedItems.Add(item.Serialize().ToBase64EncodedString());
            }

            await NwTask.NextFrame();
          }
        }
        private async Task DeserializeBankItemList()
        {
          var result = await SqLiteUtils.SelectQueryAsync("playerCharacters",
            new List<string>() { { "persistantStorage" } },
            new List<string[]>() { { new string[] { "rowid", player.characterId.ToString() } } });

          if (result != null)
          {
            string serializedBank = result.FirstOrDefault()[0];
            List<string> serializedItems = new List<string>();

            Task loadBank = Task.Run(() =>
            {
              if (string.IsNullOrEmpty(serializedBank))
                return;

              serializedItems = JsonConvert.DeserializeObject<List<string>>(serializedBank);
            });

            await loadBank;

            items = new();

            foreach (string serializedItem in serializedItems)
              items.Add(NwItem.Deserialize(serializedItem.ToByteArray()));
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
