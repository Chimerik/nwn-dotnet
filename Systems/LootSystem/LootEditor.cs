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
      public class LootEditorWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        
        private readonly NuiBind<string> search = new("search");
        private readonly NuiBind<string> itemNames = new("itemNames");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> topIcon = new("topIcon");
        private readonly NuiBind<string> midIcon = new("midIcon");
        private readonly NuiBind<string> botIcon = new("botIcon");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly NuiBind<NuiRect> imagePosition = new("rect");
        private readonly NuiBind<int> selectedCategory = new("selectedCategory");

        private readonly List<NuiComboEntry> lootCategories = new()
          {
            new NuiComboEntry("Inutile", 0), // gris
            new NuiComboEntry("Simple", 1), // blanc
            new NuiComboEntry("Raffiné", 2), // bleu
            new NuiComboEntry("Superbe", 3), // vert
            new NuiComboEntry("Rare", 4), // or
            new NuiComboEntry("Exotique", 5), // orange
            new NuiComboEntry("Élevé", 6), // rose
            new NuiComboEntry("Légendaire", 7), // violet
          };

        private IEnumerable<NwItem> filteredList;

        private bool AuthorizeSave { get; set; }
        private int nbDebounce { get; set; }

        public LootEditorWindow(Player player) : base(player)
        {
          windowId = "lootEditor";
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

          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiCombo() { Entries = lootCategories, Selected = selectedCategory, Width = 410, Height = 35 } } });

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 410 } } });
          rootChildren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 75 });
          rootChildren.Add(new NuiRow() { Height = 35, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Activer mode dépôt") { Id = "itemDeposit", Width = 160 },
            new NuiSpacer()
          } });

          CreateWindow();
        }

        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, 600);

          window = new NuiWindow(rootColumn, "Editeur de loots")
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

            nuiToken.OnNuiEvent += HandleLootEditorEvents;
            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);
            selectedCategory.SetBindValue(player.oid, nuiToken.Token, 0);
            selectedCategory.SetBindWatch(player.oid, nuiToken.Token, true);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            filteredList = LootSystem.lootDictionary[LootSystem.LootQuality.Inutile];
            LoadItemList(filteredList);
          }
        }

        private void HandleLootEditorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Close:
              LootSave();
              break;
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "itemDeposit":

                  player.oid.SendServerMessage("Sélectionnez les objets de votre inventaire à ajouter à la liste.");
                  player.oid.EnterTargetMode(SelectInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);

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
                  LootSystem.LootQuality selectedLootCategory = (LootSystem.LootQuality)selectedCategory.GetBindValue(player.oid, nuiToken.Token);

                  if (item != null && item.IsValid)
                  {
                    player.oid.ControlledCreature.AcquireItem(item);

                    LogUtils.LogMessage($"{player.oid.PlayerName} retire {item.Name} de la liste {selectedLootCategory}", LogUtils.LogType.DMAction);
                  }
                  else
                    LogUtils.LogMessage($"{player.oid.PlayerName} tente de retirer un objet invalide de la liste {selectedLootCategory}.", LogUtils.LogType.DMAction);

                  LootSystem.lootDictionary[selectedLootCategory].Remove(item);
                  UpdateItemList();
                  LootSave();
                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "selectedCategory":
                case "search": UpdateItemList(); break;
              }

              break;
          }
        }
        private void UpdateItemList()
        {
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          LootSystem.LootQuality selectedLootCategory = (LootSystem.LootQuality)selectedCategory.GetBindValue(player.oid, nuiToken.Token);

          filteredList = LootSystem.lootDictionary[selectedLootCategory];

          if (!string.IsNullOrEmpty(currentSearch))
            filteredList = filteredList.Where(s => s.Name.ToLower().Contains(currentSearch));

          LoadItemList(filteredList);
        }
        private void SelectInventoryItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwItem item || item == null || !item.IsValid || item.Possessor != player.oid.LoginCreature)
            return;

          LootSystem.LootQuality selectedLootCategory = (LootSystem.LootQuality)selectedCategory.GetBindValue(player.oid, nuiToken.Token);
          LootSystem.lootDictionary[selectedLootCategory].Add(NwItem.Deserialize(item.Serialize()));

          item.Destroy();
          UpdateItemList();
          LootSave();

          player.oid.EnterTargetMode(SelectInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);

          LogUtils.LogMessage($"{player.oid.PlayerName} ajoute {item.Name} à la liste {selectedLootCategory}", LogUtils.LogType.DMAction);
        }
        public void LootSave()
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
              LogUtils.LogMessage($"Account {player.accountId} : scheduling loot save in 10s", LogUtils.LogType.DMAction);
              DebounceLootSave(nbDebounce);
              return;
            }
          }
          else
            HandleLootSave();

          LogUtils.LogMessage($"Account {player.accountId} loot saved in : {(DateTime.Now - elapsed).TotalSeconds} s", LogUtils.LogType.DMAction);
        }

        private async void DebounceLootSave(int initialNbDebounce)
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
            DebounceLootSave(nbDebounce);
            return;
          }

          if (awaitPlayerLeave.IsCompletedSuccessfully || awaitSaveAuthorized.IsCompletedSuccessfully)
          {
            nbDebounce = 0;
            AuthorizeSave = true;
            LogUtils.LogMessage($"Account {player.accountId} : debounce done after {nbDebounce} triggers, loot save authorized", LogUtils.LogType.DMAction);
            LootSave();
          }
        }

        private async void HandleLootSave()
        {
          Dictionary<LootSystem.LootQuality, List<string>> serializedLootDictionary = new();

          foreach (var lootList in LootSystem.lootDictionary) 
          {
            List<string> serializedItems = new();

            await NwTask.Run(async () => { await SerializeItems(serializedItems, lootList.Value); });

            serializedLootDictionary.Add(lootList.Key, serializedItems);
          }

          Task<string> serializedLoots = Task.Run(() => JsonConvert.SerializeObject(serializedLootDictionary));
          await serializedLoots;

          SqLiteUtils.UpdateQuery("lootSystem",
          new List<string[]>() { new string[] { "loot", serializedLoots.Result } },
          new List<string[]>() { new string[] { "rowid", "1" } });

          nbDebounce = 0;
          AuthorizeSave = false;
        }
        private static async Task SerializeItems(List<string> serializedItems, List<NwItem> items)
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
        private void LoadItemList(IEnumerable<NwItem> filteredList)
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
