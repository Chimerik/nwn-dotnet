using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Discord.Net;

using Newtonsoft.Json;

using NWN.Systems.TradeSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class AuctionHouseWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup = new() { Id = "rootGroup", Border = false, Padding = 0, Margin = 0 };
        private readonly NuiColumn layoutColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<int> listCount = new("listCount");

        private readonly NuiBind<string> itemNames = new("itemNames");
        private readonly NuiBind<string> topIcon = new("topIcon");
        private readonly NuiBind<string> midIcon = new("midIcon");
        private readonly NuiBind<string> botIcon = new("botIcon");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly NuiBind<NuiRect> imagePosition = new("rect");

        private readonly NuiBind<string> requestName = new("requestName");
        private readonly NuiBind<string> search = new("search");

        private readonly NuiBind<string> highestBid = new("highestBid");
        private readonly NuiBind<string> highestBidToolTip = new("highestBidToolTip");
        private readonly NuiBind<string> buyoutPrice = new("buyoutPrice");
        private readonly NuiBind<string> expireDate = new("expireDate");
        private readonly NuiBind<string> proposal = new("proposal");
        private readonly NuiBind<bool> isAuctionCreator = new("isAuctionCreator");
        private readonly NuiBind<bool> biddingEnabled = new("biddingEnabled");

        private readonly NuiBind<string> orderUnitPrice = new("orderUnitPrice");
        private readonly NuiBind<string> orderUnitPriceTooltip = new("orderUnitPriceTooltip");
        private readonly NuiBind<string> orderQuantity = new("orderQuantity");
        private readonly NuiBind<bool> displayBuyOrder = new("displayBuyOrder");
        private readonly NuiBind<bool> displaySellOrder = new("displaySellOrder");
        private readonly NuiBind<string> unitPrice = new("unitPrice");
        private readonly NuiBind<string> quantity = new("quantity"); 
        private readonly NuiBind<bool> cancelOrderVisible = new("cancelOrderVisible");

        private readonly NuiBind<int> selectedMaterial = new("selectedMaterial");
        //private readonly NuiBind<int> selectedLevel = new("selectedLevel");

        private readonly List<NuiComboEntry> resourcesCombo = new();
        /*private readonly List<NuiComboEntry> resourceLevelCombo = new()
          {
            new NuiComboEntry("1", 1),
            new NuiComboEntry("2", 2),
            new NuiComboEntry("3", 3),
            new NuiComboEntry("4", 4),
            new NuiComboEntry("5", 5),
            new NuiComboEntry("6", 6),
            new NuiComboEntry("7", 7),
            new NuiComboEntry("8", 8),
          };*/

        public List<TradeRequest> tradeRequests { get; set; }
        private IEnumerable<TradeRequest> filteredTradeRequests;

        public List<Auction> auctions { get; set; }
        private IEnumerable<Auction> filteredAuctions;

        public List<BuyOrder> buyOrders { get; set; }
        private IEnumerable<BuyOrder> filteredBuyOrders;

        public List<SellOrder> sellOrders { get; set; }
        private IEnumerable<SellOrder> filteredSellOrders;

        public AuctionHouseWindow(Player player) : base(player)
        {
          windowId = "auctionHouse";

          rootGroup.Layout = layoutColumn;
          layoutColumn.Children = rootChildren;

          int i = 0;

          foreach (CraftResource resource in Craft.Collect.System.craftResourceArray)
          {
            resourcesCombo.Add(new NuiComboEntry(resource.name, i));
            i++;
          }

          CreateWindow();
        }
        public void CreateWindow()
        {
          LoadRequestsLayout();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? new NuiRect(player.windowRectangles[windowId].X, player.windowRectangles[windowId].Y, 410, 500) : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootGroup, "Hôtel des ventes")
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
            nuiToken.OnNuiEvent += HandleAuctionHouseEvents;

            LoadRequestsBinding();

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
          else
            player.oid.SendServerMessage($"Impossible d'ouvrir la fenêtre {window.Title}. Celle-ci est-elle déjà ouverte ?", ColorConstants.Orange);
        }
        private void HandleAuctionHouseEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "requests":
                  LoadRequestsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadRequestsBinding();
                  break;

                case "auctions":
                  LoadAuctionsLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  LoadAuctionsBinding();
                  break;

                case "market":
                  LoadMarketLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  LoadBuyOrdersBinding();
                  break;

                case "displaySellOrders": LoadSellOrderBindings(); break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "searchRequest": UpdateTradeRequestsList(); break;
                case "searchAuction": UpdateAuctionsList(); break;
              }

              break;
          }
        }

        private void LoadButtons()
        {
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Requêtes") { Id = "requests", Height = 35, Width = 90 },
            new NuiButton("Enchères") { Id = "description", Height = 35, Width = 90 },
            new NuiButton("Marché de gros") { Id = "Market", Height = 35, Width = 90 },
            new NuiSpacer()
          } });
        }

        private void LoadRequestsLayout()
        {
          rootChildren.Clear();
          rowTemplate.Clear();
          LoadButtons();

          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(requestName) { Id = "openRequest", Tooltip = requestName }) { VariableSize = true });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiTextEdit("Recherche", search, 20, false) { Id = "searchRequest" },
              new NuiButtonImage("ir_split") { Id = "newRequest", Tooltip = "Afficher une nouvelle requête", Height = 35, Width = 35 },
            } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380 } } }
          } });
        }
        private void StopAllWatchBindings()
        {
          search.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedMaterial.SetBindWatch(player.oid, nuiToken.Token, false);
        }
        private async void LoadRequestsBinding()
        {
          StopAllWatchBindings();

          search.SetBindValue(player.oid, nuiToken.Token, "");
          search.SetBindWatch(player.oid, nuiToken.Token, true);

          if (tradeRequests == null)
          {
            await DeserializeTradeRequests();
            await NwTask.SwitchToMainThread();
          }

          filteredTradeRequests = tradeRequests;
          LoadTradeRequests(filteredTradeRequests);
        }
        private void LoadTradeRequests(IEnumerable<TradeRequest> filteredList)
        {
          List<string> requestNameList = new();

          foreach (var request in tradeRequests)
            if(request.expirationDate > DateTime.Now)
              requestNameList.Add(request.description);

          requestName.SetBindValues(player.oid, nuiToken.Token, requestNameList);
          listCount.SetBindValue(player.oid, nuiToken.Token, requestNameList.Count());
        }
        private void UpdateTradeRequestsList()
        {
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          filteredTradeRequests = tradeRequests;

          if (!string.IsNullOrEmpty(currentSearch))
            filteredTradeRequests = filteredTradeRequests.Where(s => s.description.ToLower().Contains(currentSearch));

          LoadTradeRequests(filteredTradeRequests);
        }
        private void UpdateAuctionsList()
        {
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          filteredAuctions = auctions;

          if (!string.IsNullOrEmpty(currentSearch))
            filteredAuctions = filteredAuctions.Where(s => s.itemName.ToLower().Contains(currentSearch));

          LoadAuctions(filteredAuctions);
        }
        private void LoadAuctionsLayout()
        {
          rootChildren.Clear();
          rowTemplate.Clear();
          LoadButtons();

          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()
          {
            Height = 125,
            Id = "examineItem",
            DrawList = new List<NuiDrawListItem>()
            {
              new NuiDrawListImage(topIcon, imagePosition),
              new NuiDrawListImage(midIcon, imagePosition) { Enabled = enabled },
              new NuiDrawListImage(botIcon, imagePosition) { Enabled = enabled }

            }
          }) { Width = 45 });

          rowTemplate.Add(new NuiListTemplateCell(new NuiText(highestBid) { Tooltip = highestBidToolTip }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(buyoutPrice) { Tooltip = buyoutPrice }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(expireDate) { Tooltip = expireDate }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", proposal, 20, false) { Tooltip = "Montant proposé pour enchérir" }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_split") { Id = "auctionBid", Enabled = biddingEnabled, Tooltip = "Enchérir" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_charsheet") { Id = "closeBid", Tooltip = "Clore l'enchère", Visible = isAuctionCreator }) { Width = 35 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiTextEdit("Recherche", search, 20, false) { Id = "searchAuction" },
              new NuiButtonImage("ir_split") { Id = "newAuction", Tooltip = "Afficher une nouvelle enchère", Height = 35, Width = 35 },
            } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380 } } }
          } });
        }

        private async void LoadAuctionsBinding()
        {
          StopAllWatchBindings();

          search.SetBindValue(player.oid, nuiToken.Token, "");
          search.SetBindWatch(player.oid, nuiToken.Token, true);

          if (auctions == null)
          {
            await DeserializeAuctions();
            await NwTask.SwitchToMainThread();
          }

          filteredAuctions = auctions;
          LoadAuctions(filteredAuctions);
        }

        private void LoadAuctions(IEnumerable<Auction> filteredList)
        {
          List<string> highestBidList = new();
          List<string> highestBidTooltipList = new();
          List<string> buyoutPriceList = new();
          List<string> expireDateList = new();
          List<string> proposalList = new();
          List<bool> isAuctionCreatorList = new();
          List<bool> biddingEnabledList = new();

          foreach (var auction in filteredList)
          {
            if (auction.expirationDate > DateTime.Now)
            {
              highestBidList.Add(auction.highestBid.ToString());
              highestBidTooltipList.Add(auction.highestBidderId == player.characterId ? $"{auction.highestBid} : C'est vous !" : auction.highestBid.ToString());
              buyoutPriceList.Add(auction.buyoutPrice.ToString());
              expireDateList.Add(auction.expirationDate.ToString());
              proposalList.Add("");
              isAuctionCreatorList.Add(auction.auctionerId == player.characterId);
              biddingEnabledList.Add(auction.auctionerId != player.characterId);
            }
          }

          highestBid.SetBindValues(player.oid, nuiToken.Token, highestBidList);
          highestBidToolTip.SetBindValues(player.oid, nuiToken.Token, highestBidTooltipList);
          buyoutPrice.SetBindValues(player.oid, nuiToken.Token, buyoutPriceList);
          expireDate.SetBindValues(player.oid, nuiToken.Token, expireDateList);
          proposal.SetBindValues(player.oid, nuiToken.Token, proposalList);
          isAuctionCreator.SetBindValues(player.oid, nuiToken.Token, isAuctionCreatorList);
          biddingEnabled.SetBindValues(player.oid, nuiToken.Token, biddingEnabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, highestBidList.Count());
        }
        private void LoadMarketLayout()
        {
          rootChildren.Clear();
          LoadButtons();
          rowTemplate.Clear();

          rowTemplate.Add(new NuiListTemplateCell(new NuiText(orderUnitPrice) { Tooltip = orderUnitPriceTooltip }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(orderQuantity) { Tooltip = orderQuantity }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(expireDate) { Tooltip = expireDate }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_abort") { Id = "cancelOrder", Tooltip = "Annuler votre ordre", Visible = cancelOrderVisible }) { Width = 35 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiButtonImage("ir_learnscroll") { Id = "displayBuyOrders", Tooltip = "Consulter les ordres d'achat", Enabled = displayBuyOrder },
              new NuiButtonImage("ir_barter") { Id = "displaySellOrders", Tooltip = "Consulter les ordres de vente", Enabled = displaySellOrder }
            } },
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiCombo() { Id = "materialType", Entries = resourcesCombo, Selected = selectedMaterial, Tooltip = "Type de matériau" },
              //new NuiCombo() { Id = "materialLevel", Entries = resourceLevelCombo, Selected = selectedLevel, Tooltip = "Niveau d'infusion" }
            } },
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiTextEdit("Prix unitaire", unitPrice, 20, false) { Id = "unitPrice", Tooltip = "Prix unitaire de votre nouvel ordre" },
              new NuiTextEdit("Quantité", quantity, 20, false) { Id = "quantity", Tooltip = "Quantité de votre nouvel ordre" },
              new NuiButtonImage("ir_buy") { Id = "newBuyOrder", Tooltip = "Placer un nouvel ordre d'achat", Height = 35, Width = 35 },
              new NuiButtonImage("ir_sell") { Id = "newSellOrder", Tooltip = "Placer un nouvel ordre de vente", Height = 35, Width = 35 }
            } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380 } } }
          } });;
        }
        private async void LoadBuyOrdersBinding()
        {
          StopAllWatchBindings();

          displayBuyOrder.SetBindValue(player.oid, nuiToken.Token, false);
          displaySellOrder.SetBindValue(player.oid, nuiToken.Token, true);

          selectedMaterial.SetBindValue(player.oid, nuiToken.Token, 0);
          selectedMaterial.SetBindWatch(player.oid, nuiToken.Token, true);

          if (buyOrders == null)
          {
            await DeserializeBuyOrders();
            await NwTask.SwitchToMainThread();
          }

          filteredBuyOrders = buyOrders;
          LoadBuyOrders(filteredBuyOrders);
        }
        private void LoadBuyOrders(IEnumerable<BuyOrder> filteredList)
        {
          List<string> orderUnitPriceList = new();
          List<string> orderUnitPriceTooltipList = new();
          List<string> orderQuantityList = new();
          List<string> expireDateList = new();
          List<bool> cancelOrderVisibleList = new();

          CraftResource resource = Craft.Collect.System.craftResourceArray[selectedMaterial.GetBindValue(player.oid, nuiToken.Token)];

          foreach (var order in filteredList)
          {
            if (resource.type == order.resourceType && resource.grade == order.resourceLevel && order.expirationDate > DateTime.Now)
            {
              orderUnitPriceList.Add(order.unitPrice.ToString());
              orderQuantityList.Add(order.quantity.ToString());
              expireDateList.Add(order.expirationDate.ToString());
              cancelOrderVisibleList.Add(order.buyerId == player.characterId ? true : false);
              orderUnitPriceTooltipList.Add($"{order.unitPrice} - Total : {order.GetTotalCost()}");
            }
          }

          orderUnitPrice.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceList);
          orderQuantity.SetBindValues(player.oid, nuiToken.Token, orderQuantityList);
          expireDate.SetBindValues(player.oid, nuiToken.Token, expireDateList);
          cancelOrderVisible.SetBindValues(player.oid, nuiToken.Token, cancelOrderVisibleList);
          orderUnitPriceTooltip.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceTooltipList);
          listCount.SetBindValue(player.oid, nuiToken.Token, orderUnitPriceList.Count());
        }
        private async void LoadSellOrderBindings()
        {
          StopAllWatchBindings();

          displayBuyOrder.SetBindValue(player.oid, nuiToken.Token, true);
          displaySellOrder.SetBindValue(player.oid, nuiToken.Token, false);

          selectedMaterial.SetBindValue(player.oid, nuiToken.Token, 0);
          selectedMaterial.SetBindWatch(player.oid, nuiToken.Token, true);

          if (sellOrders == null)
          {
            await DeserializeSellOrders();
            await NwTask.SwitchToMainThread();
          }

          filteredSellOrders = sellOrders.OrderByDescending(b => b.unitPrice);
          LoadSellOrders(filteredSellOrders);
        }
        private void LoadSellOrders(IEnumerable<SellOrder> filteredList)
        {
          List<string> orderUnitPriceList = new();
          List<string> orderUnitPriceTooltipList = new();
          List<string> orderQuantityList = new();
          List<string> expireDateList = new();
          List<bool> cancelOrderVisibleList = new();

          CraftResource resource = Craft.Collect.System.craftResourceArray[selectedMaterial.GetBindValue(player.oid, nuiToken.Token)];

          foreach (var order in filteredList)
          {
            if (resource.type == order.resourceType && resource.grade == order.resourceLevel && order.expirationDate > DateTime.Now)
            {
              orderUnitPriceList.Add(order.unitPrice.ToString());
              orderQuantityList.Add(order.quantity.ToString());
              expireDateList.Add(order.expirationDate.ToString());
              cancelOrderVisibleList.Add(order.sellerId == player.characterId ? true : false);
              orderUnitPriceTooltipList.Add($"{order.unitPrice} - Total : {order.GetTotalCost()}");
            }
          }

          orderUnitPrice.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceList);
          orderQuantity.SetBindValues(player.oid, nuiToken.Token, orderQuantityList);
          expireDate.SetBindValues(player.oid, nuiToken.Token, expireDateList);
          cancelOrderVisible.SetBindValues(player.oid, nuiToken.Token, cancelOrderVisibleList);
          orderUnitPriceTooltip.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceTooltipList);
          listCount.SetBindValue(player.oid, nuiToken.Token, orderUnitPriceList.Count());
        }
        private async Task DeserializeTradeRequests()
        {
          var result = await SqLiteUtils.SelectQueryAsync("trade",
            new List<string>() { { "requests" } },
            new List<string[]>() { { new string[] { } } });

          if (result != null)
          {
            string serializedRequests = result.FirstOrDefault()[0];
            List<TradeRequest> serializedTradeRequests= new();

            Task loadRequests = Task.Run(() =>
            {
              if (string.IsNullOrEmpty(serializedRequests))
                return;

              serializedTradeRequests = JsonConvert.DeserializeObject<List<TradeRequest>>(serializedRequests);
            });

            await loadRequests;

            tradeRequests = new();

            foreach (TradeRequest serializedTradeRequest in serializedTradeRequests)
              tradeRequests.Add(serializedTradeRequest);
          }
        }
        private async Task DeserializeAuctions()
        {
          var result = await SqLiteUtils.SelectQueryAsync("trade",
            new List<string>() { { "auctions" } },
            new List<string[]>() { { new string[] { } } });

          if (result != null)
          {
            string serializedRequests = result.FirstOrDefault()[0];
            List<Auction> serializedTradeRequests = new();

            Task loadRequests = Task.Run(() =>
            {
              if (string.IsNullOrEmpty(serializedRequests))
                return;

              serializedTradeRequests = JsonConvert.DeserializeObject<List<Auction>>(serializedRequests);
            });

            await loadRequests;

            auctions = new();

            foreach (Auction serializedTradeRequest in serializedTradeRequests)
              auctions.Add(serializedTradeRequest);
          }
        }
        private async Task DeserializeBuyOrders()
        {
          var result = await SqLiteUtils.SelectQueryAsync("trade",
            new List<string>() { { "buyOrders" } },
            new List<string[]>() { { new string[] { } } });

          if (result != null)
          {
            string serializedRequests = result.FirstOrDefault()[0];
            List<BuyOrder> serializedTradeRequests = new();

            Task loadRequests = Task.Run(() =>
            {
              if (string.IsNullOrEmpty(serializedRequests))
                return;

              serializedTradeRequests = JsonConvert.DeserializeObject<List<BuyOrder>>(serializedRequests);
            });

            await loadRequests;

            auctions = new();

            foreach (BuyOrder serializedTradeRequest in serializedTradeRequests)
              buyOrders.Add(serializedTradeRequest);
          }
        }
        private async Task DeserializeSellOrders()
        {
          var result = await SqLiteUtils.SelectQueryAsync("trade",
            new List<string>() { { "sellOrders" } },
            new List<string[]>() { { new string[] { } } });

          if (result != null)
          {
            string serializedRequests = result.FirstOrDefault()[0];
            List<SellOrder> serializedTradeRequests = new();

            Task loadRequests = Task.Run(() =>
            {
              if (string.IsNullOrEmpty(serializedRequests))
                return;

              serializedTradeRequests = JsonConvert.DeserializeObject<List<SellOrder>>(serializedRequests);
            });

            await loadRequests;

            auctions = new();

            foreach (SellOrder serializedTradeRequest in serializedTradeRequests)
              sellOrders.Add(serializedTradeRequest);
          }
        }
      }
    }
  }
}
