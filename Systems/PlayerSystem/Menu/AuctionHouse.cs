using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

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

        //private readonly NuiBind<string> itemNames = new("itemNames");
        private readonly NuiBind<string> topIcon = new("topIcon");
        private readonly NuiBind<string> midIcon = new("midIcon");
        private readonly NuiBind<string> botIcon = new("botIcon");
        private readonly NuiBind<bool> enabled = new("enabled");
        private readonly NuiBind<NuiRect> imagePosition = new("rect");

        private readonly NuiBind<string> requestName = new("requestName");
        private readonly NuiBind<string> search = new("search");

        private readonly NuiBind<string> highestBid = new("highestBid");
        private readonly NuiBind<string> highestBidToolTip = new("highestBidToolTip");
        private readonly NuiBind<string> startingPrice = new("startingPrice");
        private readonly NuiBind<string> buyoutPrice = new("buyoutPrice");
        private readonly NuiBind<string> expireDate = new("expireDate");
        private readonly NuiBind<string> proposal = new("proposal");
        private readonly NuiBind<bool> isAuctionCreator = new("isAuctionCreator");
        private readonly NuiBind<bool> biddingEnabled = new("biddingEnabled");
        private readonly NuiBind<string> auctionSellPrice = new("auctionSellPrice");
        private readonly NuiBind<string> auctionBuyoutPrice = new("auctionBuyoutPrice");
        private readonly NuiBind<bool> isAuctionItemSelected = new("isAuctionItemSelected");

        private NwItem auctionItemSelected = null;

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

        public List<NwItem> tradeProposalItemScheduledForDestruction = new();

        private TradeRequest lastRequestClicked { get; set; }
        private readonly List<NwItem> newProposalItems = new();

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
          if (!NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").Any(b => b.GetObjectVariable<LocalVariableInt>("ownerId").Value == player.characterId))
          {
            player.oid.SendServerMessage("L'utilisation de l'hôtel des ventes nécessite la signature préalable d'un contrat auprès de Skalsgard Investissements", ColorConstants.Red);
            return;
          }
          
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
            case NuiEventType.Close:
              CleanUpProposalItems();
              break;

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

                case "newRequest": 
                  LoadNewRequestLayout();
                  rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
                  break;

                case "createRequest":

                  string description = search.GetBindValue(player.oid, nuiToken.Token);

                  if (string.IsNullOrEmpty(description))
                  {
                    player.oid.SendServerMessage("Votre commande doit disposer d'une description pour être affichée au tableau", ColorConstants.Red);
                    return;
                  }

                  SaveNewRequestToDatabase(description);

                  LoadRequestsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadRequestsBinding();

                  break; 

                case "myRequests":
                  string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  filteredTradeRequests = !string.IsNullOrEmpty(currentSearch) ? TradeSystem.tradeRequestList.Where(s => s.description.ToLower().Contains(currentSearch) && s.requesterId == player.characterId) : TradeSystem.tradeRequestList.Where(s => s.requesterId == player.characterId);
                  LoadTradeRequests(filteredTradeRequests);
                  break;

                case "myProposals":
                  string thisSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                  filteredTradeRequests = !string.IsNullOrEmpty(thisSearch) ? TradeSystem.tradeRequestList.Where(s => s.description.ToLower().Contains(thisSearch) && s.proposalList.Any(p => p.characterId == player.characterId)) : TradeSystem.tradeRequestList.Where(s => s.proposalList.Any(p => p.characterId == player.characterId));
                  LoadTradeRequests(filteredTradeRequests);
                  break;

                case "proposalItemDeposit":
                  player.oid.SendServerMessage("Veuillez sélectionner les objets de votre inventaire que vous souhaitez proposer pour cette commande.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectProposalInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);
                  break;

                case "deleteRequest": HandleRequestCancellation(filteredTradeRequests.ElementAt(nuiEvent.ArrayIndex));  break;
                case "cancelRequest": HandleRequestCancellation(lastRequestClicked); break;

                case "acceptProposal":

                  TradeProposal acceptedProposal = lastRequestClicked.proposalList[nuiEvent.ArrayIndex];

                  if (lastRequestClicked.expirationDate < DateTime.Now)
                    player.oid.SendServerMessage("Cette commande a malheureusement expiré !", ColorConstants.Red);
                  else if (acceptedProposal.cancelled)
                    player.oid.SendServerMessage("Cette proposition a malheureusement été retirée !", ColorConstants.Red);
                  else if (acceptedProposal.sellPrice > player.bankGold)
                    player.oid.SendServerMessage("Vous ne disposez malheureusement pas de suffisamment d'or sur votre compte Skalsgard !", ColorConstants.Red);
                  else
                  {
                    player.bankGold -= acceptedProposal.sellPrice;
                    TradeSystem.AddItemToPlayerDataBaseBank(player.characterId.ToString(), acceptedProposal.serializedItems, "Accepted proposal");

                    int taxedSellPrice = TradeSystem.GetTaxedSellPrice(Players.FirstOrDefault(p => p.Value.characterId == acceptedProposal.characterId).Value, acceptedProposal.sellPrice);
                    TradeSystem.UpdatePlayerBankAccount(acceptedProposal.characterId.ToString(), taxedSellPrice.ToString(), "Proposal accepted - Request accepted");

                    acceptedProposal.cancelled = true;

                    foreach(var proposal in lastRequestClicked.proposalList)
                    {
                      if (proposal.cancelled)
                        continue;

                      TradeSystem.UpdatePlayerBankAccount(proposal.characterId.ToString(), proposal.sellPrice.ToString(), "Proposal cancelled - Request accepted");
                      TradeSystem.AddItemToPlayerDataBaseBank(proposal.characterId.ToString(), proposal.serializedItems, "Proposal cancelled - Request accepted");
                      proposal.cancelled = true;
                    }

                    lastRequestClicked.expirationDate = DateTime.Now;
                    TradeSystem.tradeRequestList.Remove(lastRequestClicked);
                    lastRequestClicked = null;
                    player.oid.SendServerMessage("La proposition a bien été acceptée. Les objets ont été déposés dans votre compte Skalsgard !", ColorConstants.Orange);
                  }

                  LoadRequestsLayout();
                  rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                  LoadRequestsBinding();

                  break;

                case "removeProposalItem":

                  NwItem proposalItem = newProposalItems[nuiEvent.ArrayIndex];

                  if (proposalItem != null && proposalItem.IsValid)
                  {
                    player.oid.ControlledCreature.AcquireItem(proposalItem);
                    Log.Info($"TRADE SYSTEM - {player.oid.LoginCreature.Name} ({player.oid.PlayerName}) retire {proposalItem.Name}");
                  }
                  else
                    Utils.LogMessageToDMs($"TRADE SYSTEM - {player.oid.LoginCreature.Name} trying to take an invalid item.");

                  newProposalItems.Remove(proposalItem);
                  LoadCreateProposalItemList();

                  break;

                case "createNewProposal":

                  if(lastRequestClicked.expirationDate < DateTime.Now)
                  {
                    player.oid.SendServerMessage("La commande pour laquelle vous souhaitiez faire une proposition n'est malheureusement plus valide !");

                    LoadRequestsLayout();
                    rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                    LoadRequestsBinding();

                    foreach(NwItem item in newProposalItems)
                      player.oid.ControlledCreature.AcquireItem(item);

                    return;
                  }

                  if (int.TryParse(search.GetBindValue(player.oid, nuiToken.Token), out int sellPrice) || sellPrice < 1)
                  {
                    if(player.bankGold < sellPrice)
                    {
                      player.oid.SendServerMessage("Vous ne disposez pas de suffisamment d'or sur votre compte Skalsgard pour faire une proposition aussi élevée", ColorConstants.Red);
                      return;
                    }

                    List<string> serializedItems = new();

                    foreach (NwItem item in newProposalItems)
                      serializedItems.Add(item.Serialize().ToBase64EncodedString());

                    player.bankGold -= sellPrice;

                    TradeProposal proposal = new TradeProposal(player.characterId, sellPrice, serializedItems);
                    lastRequestClicked.proposalList.Add(proposal);
                    LoadRequestDetailsLayout(lastRequestClicked);
                    player.oid.SendServerMessage($"Votre nouvelle proposition a bien été enregistrée, {sellPrice} pièces ont été prélevées de votre compte Skalsgard !", ColorConstants.Orange);
                  }
                  else
                    player.oid.SendServerMessage("Le prix de vente indiqué est incorrect. Impossible d'enregistrer votre proposition", ColorConstants.Red);

                  break;

                case "cancelProposal":

                  if (lastRequestClicked.expirationDate < DateTime.Now)
                  {
                    player.oid.SendServerMessage("Cette proposition commerciale a déjà été annulée !", ColorConstants.Orange);

                    LoadRequestsLayout();
                    rootGroup.SetLayout(player.oid, nuiEvent.Token.Token, layoutColumn);
                    LoadRequestsBinding();

                    return;
                  }

                  TradeProposal deletedProposal = lastRequestClicked.proposalList[nuiEvent.ArrayIndex];
                  deletedProposal.cancelled = true;

                  player.bankGold += deletedProposal.sellPrice;
                  TradeSystem.AddItemToPlayerDataBaseBank(player.characterId.ToString(), deletedProposal.serializedItems, $"Proposal cancelled by {player.oid.LoginCreature.Name}");

                  player.oid.SendServerMessage($"La proposition a bien été annulée. Vos objets et {deletedProposal.sellPrice} pièces ont été débloquées sur votre compte Skalsgard.", ColorConstants.Orange);

                  break;

                case "openRequest": LoadRequestDetailsLayout(filteredTradeRequests.ElementAt(nuiEvent.ArrayIndex)); break;
                case "loadProposalLayout": LoadCreateProposalLayout(); break;

                case "displayBuyOrders":

                  displayBuyOrder.SetBindValue(player.oid, nuiToken.Token, true);
                  displaySellOrder.SetBindValue(player.oid, nuiToken.Token, false);
                  LoadBuyOrders(filteredBuyOrders); 

                  break;
                case "displaySellOrders":

                  displayBuyOrder.SetBindValue(player.oid, nuiToken.Token, false);
                  displaySellOrder.SetBindValue(player.oid, nuiToken.Token, true);
                  LoadSellOrders(filteredSellOrders); 
                  
                  break;

                case "examineItem":

                  NwItem auctionItem = NwItem.Deserialize(filteredAuctions.ElementAt(nuiEvent.ArrayIndex).serializedItem.ToByteArray());
                  tradeProposalItemScheduledForDestruction.Add(auctionItem);

                  if (!player.windows.ContainsKey("itemExamine")) player.windows.Add("itemExamine", new ItemExamineWindow(player, auctionItem));
                  else ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(auctionItem);

                  break;

                case "auctionBid":

                  if(!int.TryParse(proposal.GetBindValue(player.oid, nuiToken.Token), out int bid) || bid < 1)
                  {
                    player.oid.SendServerMessage("L'enchère saisie est incorrecte. Veuillez saisir une valeur valide.", ColorConstants.Red);
                    return;
                  }

                  Auction auction = filteredAuctions.ElementAt(nuiEvent.ArrayIndex);

                  if(auction.expirationDate < DateTime.Now || (auction.buyoutPrice > 0 && auction.highestBid >= auction.buyoutPrice))
                  {
                    player.oid.SendServerMessage("Cette enchère est désormais close. Impossible de surenchérir !", ColorConstants.Red);
                    return;
                  }

                  if (bid <= auction.startingPrice)
                  {
                    player.oid.SendServerMessage("L'enchère saisie doit obligatoirement être supérieur à la mise à prix", ColorConstants.Red);
                    return;
                  }

                  if (bid <= auction.highestBid)
                  {
                    player.oid.SendServerMessage("L'enchère saisie doit obligatoirement être supérieure à l'enchère maximale", ColorConstants.Red);
                    return;
                  }

                  if(bid > player.bankGold)
                  {
                    player.oid.SendServerMessage("Vous ne disposez pas de la somme nécessaire sur votre compte Skalsgard pour enchérir autant", ColorConstants.Red);
                    return;
                  }

                  player.bankGold -= bid;

                  if(auction.highestBid > 0) // TODO : prévoir notification en cas d'outbid
                    TradeSystem.UpdatePlayerBankAccount(auction.highestBidderId.ToString(), auction.highestBid.ToString(), "Auction outbid");

                  auction.highestBid = bid;
                  auction.highestBidderId = player.characterId;

                  if (auction.buyoutPrice > 0 && bid >= auction.buyoutPrice)
                  {
                    TradeSystem.auctionList.Remove(auction);
                    TradeSystem.ResolveSuccessfulAuction(auction);
                    TradeSystem.ScheduleSaveToDatabase();
                  }
                  else if((DateTime.Now - auction.expirationDate).TotalMinutes < 5)
                    auction.expirationDate.AddMinutes(5);

                  LoadAuctions(filteredAuctions);

                  break;

                case "closeBid":

                  Auction closedAuction = filteredAuctions.ElementAt(nuiEvent.ArrayIndex);

                  if(closedAuction.highestBid > 0)
                  {
                    player.oid.SendServerMessage("Des enchères sont déjà en cours sur cette offre, il n'est plus possible de l'annuler", ColorConstants.Red);
                    return;
                  }

                  if (closedAuction.expirationDate < DateTime.Now)
                  {
                    player.oid.SendServerMessage("Cette enchère est déjà close", ColorConstants.Red);
                    return;
                  }

                  TradeSystem.GiveItemAuction(closedAuction.auctionerId, closedAuction.itemName, closedAuction.serializedItem);
                  TradeSystem.auctionList.Remove(closedAuction);
                  TradeSystem.ScheduleSaveToDatabase();

                  LoadAuctions(filteredAuctions);

                  break;

                case "auctionItemSelect":
                  player.oid.SendServerMessage("Veuillez sélectionner l'objet de votre inventaire que vous souhaitez mettre aux enchères.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectAuctionInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);
                  break;

                case "newAuction":

                  if(auctionItemSelected == null || !auctionItemSelected.IsValid || auctionItemSelected.Possessor != player.oid.LoginCreature)
                  {
                    player.oid.SendServerMessage("L'object sélectionné n'existe plus ou n'est plus en votre possession", ColorConstants.Red);
                    isAuctionItemSelected.SetBindValue(player.oid, nuiToken.Token, false);
                    return;
                  }

                  if(!int.TryParse(auctionBuyoutPrice.GetBindValue(player.oid, nuiToken.Token), out int buyoutPrice) || buyoutPrice < 1)
                    buyoutPrice = 0;

                  if (int.TryParse(auctionSellPrice.GetBindValue(player.oid, nuiToken.Token), out int startPrice))
                  {
                    int brokerFee = (int)(startPrice * 0.03);
                    if (player.bankGold < brokerFee)
                    {
                      player.oid.SendServerMessage("Vous ne disposez pas d'assez d'or sur votre compte Skalsgard pour vous acquiter des frais de dossier de 3 %", ColorConstants.Red);
                      return;
                    }

                    player.bankGold -= brokerFee;
                    player.oid.SendServerMessage($"{brokerFee} pièces viennent d'être prélevées de votre comtpe Skalsgard en tant que frais de dossier", ColorConstants.Orange);
                  }
                  else
                    startPrice = 0;

                  TradeSystem.auctionList.Add(new Auction(player.characterId, auctionItemSelected.Serialize().ToBase64EncodedString(), auctionItemSelected.Name, DateTime.Now.AddDays(7), startPrice, buyoutPrice));

                  auctionItemSelected.Destroy();
                  auctionItemSelected = null;
                  isAuctionItemSelected.SetBindValue(player.oid, nuiToken.Token, false);

                  LoadAuctions(filteredAuctions);
                  TradeSystem.ScheduleSaveToDatabase();

                  player.oid.SendServerMessage($"Votre enchère pour {auctionItemSelected.Name.ColorString(ColorConstants.White)} a bien été enregistrée pour une mise à prix à {startPrice.ToString().ColorString(ColorConstants.White)} et une valeur d'achat immédiat de {buyoutPrice.ToString().ColorString(ColorConstants.White)}", ColorConstants.Orange);

                  break;

                case "cancelOrder":

                  if(displayBuyOrder.GetBindValue(player.oid, nuiToken.Token)) // cas Buy Order
                  {
                    BuyOrder cancelledOrder = filteredBuyOrders.ElementAt(nuiEvent.ArrayIndex);

                    if (cancelledOrder.expirationDate < DateTime.Now)
                      player.oid.SendServerMessage("Cet ordre d'achat a déjà expiré", ColorConstants.Red);
                    else
                    {
                      int refund = cancelledOrder.unitPrice * cancelledOrder.quantity;
                      player.bankGold += refund;
                      cancelledOrder.expirationDate = DateTime.Now;
                      TradeSystem.buyOrderList.Remove(cancelledOrder);

                      player.oid.SendServerMessage($"Votre ordre d'achat a bien été annulé, {refund.ToString().ColorString(ColorConstants.White)} pièces ont été débloquées sur votre compte Skalsgard", ColorConstants.Orange);
                    }

                    LoadBuyOrders(filteredBuyOrders);
                  }
                  else // cas Sell Order
                  {
                    SellOrder cancelledOrder = filteredSellOrders.ElementAt(nuiEvent.ArrayIndex);

                    if (cancelledOrder.expirationDate < DateTime.Now)
                      player.oid.SendServerMessage("Cet ordre de vente a déjà expiré", ColorConstants.Red);
                    else
                    {
                      TradeSystem.AddResourceToPlayerStock(player, player.characterId, cancelledOrder.resourceType, 
                        cancelledOrder.resourceLevel, cancelledOrder.quantity, $"Votre ordre de vente a bien été annulé, {cancelledOrder.quantity.ToString().ColorString(ColorConstants.White)} unités de {cancelledOrder.resourceType.ToDescription().ColorString(ColorConstants.White)} {cancelledOrder.resourceLevel.ToString().ColorString(ColorConstants.White)} ont été débloquées sur votre compte Skalsgard",
                        "Sell Order Cancelled");

                      cancelledOrder.expirationDate = DateTime.Now;
                      TradeSystem.sellOrderList.Remove(cancelledOrder);
                    }

                    LoadSellOrders(filteredSellOrders);
                  }

                  break;

                case "newBuyOrder":

                  if(!int.TryParse(unitPrice.GetBindValue(player.oid, nuiToken.Token), out int boUnitPrice) || boUnitPrice < 1)
                  {
                    player.oid.SendServerMessage("Veuillez saisir un prix d'achat unitaire valide", ColorConstants.Red);
                    return;
                  }

                  if (!int.TryParse(quantity.GetBindValue(player.oid, nuiToken.Token), out int boQuantity) || boQuantity < 1)
                  {
                    player.oid.SendServerMessage("Veuillez saisir une quantité valide", ColorConstants.Red);
                    return;
                  }

                  int transactionCost = boQuantity * boUnitPrice;
                  int taxCost = (int)(transactionCost * 0.03);

                  if (player.bankGold < transactionCost + taxCost)
                  {
                    player.oid.SendServerMessage("Vous ne disposez pas de suffisament d'or sur votre compte Skalsgard pour vous permettre de passer cet ordre d'achat", ColorConstants.Red);
                    return;
                  }

                  player.bankGold -= taxCost;
                  player.oid.SendServerMessage($"{taxCost.ToString().ColorString(ColorConstants.White)} pièces ont été prélevées de votre compte Skalsgard afin d'assurer les frais de dossier de votre ordre", ColorConstants.Orange);
                  ResolveBuyOrderAsync(boUnitPrice, boQuantity, Craft.Collect.System.craftResourceArray[selectedMaterial.GetBindValue(player.oid, nuiToken.Token)]);

                  break;

                case "newSellOrder":

                  if (!int.TryParse(unitPrice.GetBindValue(player.oid, nuiToken.Token), out int soUnitPrice) || soUnitPrice < 1)
                  {
                    player.oid.SendServerMessage("Veuillez saisir un prix de vente unitaire valide", ColorConstants.Red);
                    return;
                  }

                  if (!int.TryParse(quantity.GetBindValue(player.oid, nuiToken.Token), out int soQuantity) || soQuantity < 1)
                  {
                    player.oid.SendServerMessage("Veuillez saisir une quantité valide", ColorConstants.Red);
                    return;
                  }

                  CraftResource boughtResource = Craft.Collect.System.craftResourceArray[selectedMaterial.GetBindValue(player.oid, nuiToken.Token)];
                  CraftResource playerResource = player.craftResourceStock.FirstOrDefault(r => r.type == boughtResource.type && r.grade == boughtResource.grade);

                  if(playerResource == null || playerResource.quantity < soQuantity)
                  {
                    player.oid.SendServerMessage("Vous ne disposez pas des stocks de matérias nécessaires pour passer cet ordre de vente", ColorConstants.Red);
                    return;
                  }

                  int soTax = (int)(soQuantity * soUnitPrice * 0.03);

                  if (player.bankGold < soTax)
                  {
                    player.oid.SendServerMessage("Vous ne disposez pas de suffisament d'or sur votre compte Skalsgard pour vous permettre de passer cet ordre de vente", ColorConstants.Red);
                    return;
                  }

                  player.bankGold -= soTax;
                  playerResource.quantity -= soQuantity;
                  player.oid.SendServerMessage($"{soTax.ToString().ColorString(ColorConstants.White)} pièces ont été prélevées de votre compte Skalsgard afin d'assurer les frais de dossier de votre ordre", ColorConstants.Orange);
                  ResolveSellOrderAsync(soUnitPrice, soQuantity, boughtResource);

                  break;

                default:

                  if (nuiEvent.ElementId.Contains("examineProposalItem"))
                  {
                    NwItem item = NwItem.Deserialize(lastRequestClicked.proposalList[nuiEvent.ArrayIndex].serializedItems[int.Parse(nuiEvent.ElementId[^1..])].ToByteArray());
                    tradeProposalItemScheduledForDestruction.Add(item);

                    if (!player.windows.ContainsKey("itemExamine")) player.windows.Add("itemExamine", new ItemExamineWindow(player, item));
                    else ((ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(item);
                  }

                  break;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "searchRequest": UpdateTradeRequestsList(); break;
                case "searchAuction": UpdateAuctionsList(); break;
                case "selectedMaterial": UpdateOrderList(); break;
              }

              break;
          }
        }

        private void LoadButtons()
        {
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Commandes") { Id = "requests", Height = 35, Width = 90 },
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

          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(requestName) { Id = "openRequest", Tooltip = "Détails de la commande" }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(expireDate) { Tooltip = "Date d'expiration" }) { Width = 80 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_abort") { Id = "deleteRequest", Visible = isAuctionCreator, Tooltip = "Annuler ma commande" }) { Width = 35 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_charsheet") { Id = "newRequest", Tooltip = "Rédiger une nouvelle commande", Height = 35, Width = 35 },
              new NuiSpacer(),
              new NuiButtonImage("ir_split") { Id = "myRequests", Tooltip = "Consulter mes commandes en cours", Height = 35, Width = 35 },
              new NuiSpacer(),
              new NuiButtonImage("ir_accept") { Id = "myProposals", Tooltip = "Consulter les commandes auxquelles vous avez répondu", Height = 35, Width = 35 },
              new NuiSpacer(),
            } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 20, false) { Id = "searchRequest" } } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380 } } }
          } });
        }
        private void StopAllWatchBindings()
        {
          search.SetBindWatch(player.oid, nuiToken.Token, false);
          selectedMaterial.SetBindWatch(player.oid, nuiToken.Token, false);
          newProposalItems.Clear();
        }
        private void LoadRequestsBinding()
        {
          StopAllWatchBindings();

          search.SetBindValue(player.oid, nuiToken.Token, "");
          search.SetBindWatch(player.oid, nuiToken.Token, true);

          filteredTradeRequests = TradeSystem.tradeRequestList;
          LoadTradeRequests(filteredTradeRequests);
        }
        private void LoadTradeRequests(IEnumerable<TradeRequest> filteredList)
        {
          List<string> requestNameList = new();
          List<string> expireDateList = new();
          List<bool> isCreatorList = new();

          foreach (var request in filteredList)
            if (request.expirationDate > DateTime.Now)
            {
              requestNameList.Add(request.description);
              expireDateList.Add(request.expirationDate.ToString());
              isCreatorList.Add(request.requesterId == player.characterId);
            }

          requestName.SetBindValues(player.oid, nuiToken.Token, requestNameList);
          expireDate.SetBindValues(player.oid, nuiToken.Token, expireDateList);
          isAuctionCreator.SetBindValues(player.oid, nuiToken.Token, isCreatorList);
          listCount.SetBindValue(player.oid, nuiToken.Token, requestNameList.Count);
        }
        private void UpdateTradeRequestsList()
        {
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          filteredTradeRequests = !string.IsNullOrEmpty(currentSearch) ? TradeSystem.tradeRequestList.Where(s => s.description.ToLower().Contains(currentSearch)) : TradeSystem.tradeRequestList;
          LoadTradeRequests(filteredTradeRequests);
        }
        private void UpdateAuctionsList()
        {
          string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
          filteredAuctions = !string.IsNullOrEmpty(currentSearch) ? TradeSystem.auctionList.Where(s => s.itemName.ToLower().Contains(currentSearch)) : TradeSystem.auctionList;
          LoadAuctions(filteredAuctions);
        }
        private void UpdateOrderList()
        {
          CraftResource selectedResource = Craft.Collect.System.craftResourceArray[selectedMaterial.GetBindValue(player.oid, nuiToken.Token)];

          if (displayBuyOrder.GetBindValue(player.oid, nuiToken.Token)) // cas Buy Order
          {
            filteredBuyOrders = filteredBuyOrders.Where(b => b.resourceType == selectedResource.type && b.resourceLevel == selectedResource.grade);
            LoadBuyOrders(filteredBuyOrders);
          }
          else // cas Sell Order
          {
            filteredSellOrders = filteredSellOrders.Where(b => b.resourceType == selectedResource.type && b.resourceLevel == selectedResource.grade);
            LoadSellOrders(filteredSellOrders);
          }
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
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(startingPrice) { Tooltip = "Enchère minimum" }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(buyoutPrice) { Tooltip = "Prix d'achat immédiat" }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiText(expireDate) { Tooltip = expireDate }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiTextEdit("", proposal, 20, false) { Tooltip = "Montant proposé pour enchérir" }) { VariableSize = true });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_split") { Id = "auctionBid", Enabled = biddingEnabled, Tooltip = "Enchérir" }) { Width = 35 });
          rowTemplate.Add(new NuiListTemplateCell(new NuiButtonImage("ir_charsheet") { Id = "closeBid", Tooltip = "Clore l'enchère. Uniquement si aucune enchère n'a été enregistrée", Visible = isAuctionCreator }) { Width = 35 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Height = 35, Children = new List<NuiElement>()
            {
              new NuiTextEdit("Mise à prix", auctionSellPrice, 10, false) { Width = 60, Tooltip = "Prix de vente minimal" },
              new NuiTextEdit("Achat direct", auctionBuyoutPrice, 10, false) { Width = 60, Tooltip = "Prix d'achat immédiat" },
              new NuiButton("Sélection d'objet") { Id = "auctionItemSelect", Tooltip = "Sélectionner l'objet à mettre aux enchères", Width = 80 },
              new NuiButtonImage("ir_split") { Id = "newAuction", Enabled = isAuctionItemSelected, Tooltip = "Afficher une nouvelle enchère", Width = 35 },
            } },
            new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 20, false) } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380 } } }
          } });
        }

        private void LoadAuctionsBinding()
        {
          StopAllWatchBindings();

          auctionItemSelected = null;

          search.SetBindValue(player.oid, nuiToken.Token, "");
          search.SetBindWatch(player.oid, nuiToken.Token, true);

          auctionSellPrice.SetBindValue(player.oid, nuiToken.Token, "0");
          auctionBuyoutPrice.SetBindValue(player.oid, nuiToken.Token, "0");
          isAuctionItemSelected.SetBindValue(player.oid, nuiToken.Token, false);

          filteredAuctions = TradeSystem.auctionList;
          LoadAuctions(filteredAuctions);
        }

        private void LoadAuctions(IEnumerable<Auction> filteredList)
        {
          List<string> highestBidList = new();
          List<string> highestBidTooltipList = new();
          List<string> startingPriceList = new();
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
              startingPriceList.Add(auction.startingPrice.ToString());
              buyoutPriceList.Add(auction.buyoutPrice.ToString());
              expireDateList.Add(auction.expirationDate.ToString());
              proposalList.Add("");
              isAuctionCreatorList.Add(auction.auctionerId == player.characterId && auction.highestBid < 1);
              biddingEnabledList.Add(auction.auctionerId != player.characterId);
            }
          }

          highestBid.SetBindValues(player.oid, nuiToken.Token, highestBidList);
          highestBidToolTip.SetBindValues(player.oid, nuiToken.Token, highestBidTooltipList);
          startingPrice.SetBindValues(player.oid, nuiToken.Token, startingPriceList);
          buyoutPrice.SetBindValues(player.oid, nuiToken.Token, buyoutPriceList);
          expireDate.SetBindValues(player.oid, nuiToken.Token, expireDateList);
          proposal.SetBindValues(player.oid, nuiToken.Token, proposalList);
          isAuctionCreator.SetBindValues(player.oid, nuiToken.Token, isAuctionCreatorList);
          biddingEnabled.SetBindValues(player.oid, nuiToken.Token, biddingEnabledList);
          listCount.SetBindValue(player.oid, nuiToken.Token, highestBidList.Count);
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
              new NuiCombo() { Entries = resourcesCombo, Selected = selectedMaterial, Tooltip = "Type de matériau" },
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
        private void LoadBuyOrdersBinding()
        {
          StopAllWatchBindings();

          displayBuyOrder.SetBindValue(player.oid, nuiToken.Token, false);
          displaySellOrder.SetBindValue(player.oid, nuiToken.Token, true);

          selectedMaterial.SetBindValue(player.oid, nuiToken.Token, 0);
          selectedMaterial.SetBindWatch(player.oid, nuiToken.Token, true);

          LoadBuyOrders(filteredBuyOrders);
        }
        private void LoadBuyOrders(IEnumerable<BuyOrder> filteredList)
        {
          filteredList = filteredList.OrderBy(b => b.unitPrice);

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
              cancelOrderVisibleList.Add(order.buyerId == player.characterId);
              orderUnitPriceTooltipList.Add($"{order.unitPrice} - Total : {order.GetTotalCost()}");
            }
          }

          orderUnitPrice.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceList);
          orderQuantity.SetBindValues(player.oid, nuiToken.Token, orderQuantityList);
          expireDate.SetBindValues(player.oid, nuiToken.Token, expireDateList);
          cancelOrderVisible.SetBindValues(player.oid, nuiToken.Token, cancelOrderVisibleList);
          orderUnitPriceTooltip.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceTooltipList);
          listCount.SetBindValue(player.oid, nuiToken.Token, orderUnitPriceList.Count);
        }
        private void LoadSellOrders(IEnumerable<SellOrder> filteredList)
        {
          filteredList = filteredList.OrderByDescending(b => b.unitPrice);

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
              cancelOrderVisibleList.Add(order.sellerId == player.characterId);
              orderUnitPriceTooltipList.Add($"{order.unitPrice} - Total : {order.GetTotalCost()}");
            }
          }

          orderUnitPrice.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceList);
          orderQuantity.SetBindValues(player.oid, nuiToken.Token, orderQuantityList);
          expireDate.SetBindValues(player.oid, nuiToken.Token, expireDateList);
          cancelOrderVisible.SetBindValues(player.oid, nuiToken.Token, cancelOrderVisibleList);
          orderUnitPriceTooltip.SetBindValues(player.oid, nuiToken.Token, orderUnitPriceTooltipList);
          listCount.SetBindValue(player.oid, nuiToken.Token, orderUnitPriceList.Count);
        }
        
        private void LoadNewRequestLayout()
        {
          rootChildren.Clear();
          LoadButtons();

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn() { Children = new List<NuiElement>()
          {
            new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Description de la commande", search, 1000, true) } },
            new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiButtonImage("ir_learnscroll") { Id = "createRequest", Tooltip = "Afficher cette commande sur le tableau", Height = 35, Width = 35 }, new NuiSpacer() } },
          } });

          StopAllWatchBindings();
          search.SetBindValue(player.oid, nuiToken.Token, "");
        }
        private void SaveNewRequestToDatabase(string description)
        {
          TradeSystem.tradeRequestList.Add(new TradeRequest(player.characterId, description, DateTime.Now.AddMonths(1), new List<TradeProposal>()));
          TradeSystem.ScheduleSaveToDatabase();
        }
        private void LoadRequestDetailsLayout(TradeRequest request)
        {
          lastRequestClicked = request;

          player.oid.OnClientLeave -= OnClientLeave;
          player.oid.OnClientLeave += OnClientLeave;

          rootChildren.Clear();
          LoadButtons();

          StopAllWatchBindings();
          search.SetBindValue(player.oid, nuiToken.Token, "");

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          List<NuiElement> requestChildren = new();
          columnsChildren.Add(new NuiColumn() { Children = requestChildren });

          bool requestCreator = player.characterId == request.requesterId;

          requestChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiSpacer(), new NuiText(request.expirationDate.ToString()), new NuiSpacer() } });
          requestChildren.Add(new NuiRow() { Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButtonImage("ir_barter") { Id = "loadProposalLayout", Enabled = !requestCreator && !request.proposalList.Any(p => p.characterId == player.characterId && !p.cancelled), Tooltip = "Répondre par une nouvelle proposition commerciale", Height = 35, Width = 35 },
              new NuiSpacer(),
              new NuiButtonImage("ir_abort") { Id = "cancelRequest", Enabled = requestCreator, Tooltip = "Annuler cette commande", Height = 35, Width = 35 },
              new NuiSpacer()
            } });

          foreach(var proposal in request.proposalList)
          {
            if (proposal.cancelled)
              continue;

            NuiRow proposalRow = new();
            List<NuiElement> proposalChildren = new();
            proposalRow.Children = proposalChildren;

            proposalChildren.Add(new NuiText(proposal.sellPrice.ToString()) { Tooltip = proposal.sellPrice.ToString(), Width = 60 });
            proposalChildren.Add(new NuiButtonImage(requestCreator ? "ir_accept" : "ir_abort") 
            { 
              Id = requestCreator ? "acceptProposal" : "cancelProposal", 
              Tooltip = requestCreator ? "Accepter cette proposition" : "Annuler cette proposition", 
              Enabled = requestCreator || player.characterId == proposal.characterId, 
              Height = 35,
              Width = 35 
            });

            int i = 0;

            foreach (string serializedItem in proposal.serializedItems)
            {
              NwItem item = NwItem.Deserialize(serializedItem.ToByteArray());
              string[] tempArray = Utils.GetIconResref(item);

              var imagePos = item.BaseItem.ModelType switch
              {
                BaseItemModelType.Simple => ItemUtils.GetItemCategory(item.BaseItem.ItemType) != ItemUtils.ItemCategory.Shield ? new NuiRect(0, 25, 25, 25) : new NuiRect(0, 15, 25, 25),
                BaseItemModelType.Composite => ItemUtils.GetItemCategory(item.BaseItem.ItemType) != ItemUtils.ItemCategory.Ammunition ? new NuiRect(0, 0, 25, 25) : new NuiRect(0, 25, 25, 25),
                _ => new NuiRect(0, 0, 25, 25),
              };

              proposalChildren.Add(new NuiSpacer()
              {
                Height = 125,
                Width = 45,
                Id = "examineProposalItem" + i,
                Tooltip = item.Name,
                DrawList = new List<NuiDrawListItem>()
                {
                  new NuiDrawListImage(tempArray[0], imagePos),
                  new NuiDrawListImage(tempArray[1], imagePos) { Enabled = !string.IsNullOrEmpty(tempArray[1]) },
                  new NuiDrawListImage(tempArray[2], imagePos) { Enabled = !string.IsNullOrEmpty(tempArray[1]) }
                }
              });

              i++;
            }
          }

          rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
        }
        private void LoadCreateProposalLayout()
        {
          rootChildren.Clear();
          rowTemplate.Clear();
          LoadButtons();

          StopAllWatchBindings();
          search.SetBindValue(player.oid, nuiToken.Token, "0");

          rowTemplate.Add(new NuiListTemplateCell(new NuiSpacer()
          {
            Height = 125,
            Id = "removeProposalItem",
            Tooltip = requestName,
            DrawList = new List<NuiDrawListItem>()
              {
                new NuiDrawListImage(topIcon, imagePosition),
                new NuiDrawListImage(midIcon, imagePosition) { Enabled = enabled },
                new NuiDrawListImage(botIcon, imagePosition) { Enabled = enabled }

              }
          })
          { Width = 45 });

          List<NuiElement> columnsChildren = new();
          NuiRow columnsRow = new() { Children = columnsChildren };
          rootChildren.Add(columnsRow);

          columnsChildren.Add(new NuiColumn() { Children = new List<NuiElement>() { new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiTextEdit("Prix proposé", search, 10, false) { Tooltip = "Prix proposé" },
            new NuiButtonImage("ir_barter") { Id = "createNewProposal", Tooltip = "Valider la proposition commerciale", Enabled = enabled }
          } },
          new NuiRow() { Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35,  Width = 380 } } },
          new NuiRow() { Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Activer mode sélection") { Id = "proposalItemDeposit", Width = 160 }, new NuiSpacer() } }
          } });

          rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
          LoadCreateProposalItemList();
        }
        private void LoadCreateProposalItemList()
        {
          List<string> itemNameList = new();
          List<string> topIconList = new();
          List<string> midIconList = new();
          List<string> botIconList = new();
          List<bool> enabledList = new();
          List<NuiRect> imagePosList = new();

          foreach (NwItem item in newProposalItems)
          {
            itemNameList.Add(item.BaseItem.IsStackable ? $"Retirer {item.Name} (x{item.StackSize})" : $"Retirer {item.Name}");
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

          requestName.SetBindValues(player.oid, nuiToken.Token, itemNameList);
          listCount.SetBindValue(player.oid, nuiToken.Token, itemNameList.Count);

          topIcon.SetBindValues(player.oid, nuiToken.Token, topIconList);
          midIcon.SetBindValues(player.oid, nuiToken.Token, midIconList);
          botIcon.SetBindValues(player.oid, nuiToken.Token, botIconList);
          enabled.SetBindValues(player.oid, nuiToken.Token, enabledList);
          imagePosition.SetBindValues(player.oid, nuiToken.Token, imagePosList);
        }
        
        private void SelectProposalInventoryItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwItem item || item == null || !item.IsValid || item.Possessor != player.oid.LoginCreature)
            return;

          newProposalItems.Add(NwItem.Deserialize(item.Serialize()));
          item.Destroy();

          LoadCreateProposalItemList();
          player.oid.EnterTargetMode(SelectProposalInventoryItem, ObjectTypes.Item, MouseCursor.PickupDown);
        }
        private void SelectAuctionInventoryItem(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled || selection.TargetObject is not NwItem item || item == null || !item.IsValid || item.Possessor != player.oid.LoginCreature)
            return;

          auctionItemSelected = item;
          isAuctionItemSelected.SetBindValue(player.oid, nuiToken.Token, true);

          player.oid.SendServerMessage($"Vous venez de sélectionner {item.Name.ColorString(ColorConstants.White)} pour être mis aux enchères.", ColorConstants.Orange);
        }
        private void HandleRequestCancellation(TradeRequest request)
        {
          if (request.expirationDate < DateTime.Now)
            player.oid.SendServerMessage("Cette commande a déjà expiré", ColorConstants.Orange);
          else
          {
            foreach (TradeProposal proposal in request.proposalList)
            {
              TradeSystem.UpdatePlayerBankAccount(proposal.characterId.ToString(), proposal.sellPrice.ToString(), "Proposal cancelled");
              TradeSystem.AddItemToPlayerDataBaseBank(proposal.characterId.ToString(), proposal.serializedItems, "Proposal cancelled");
            }

            request.expirationDate = DateTime.Now;
            TradeSystem.tradeRequestList.Remove(request);
            lastRequestClicked = null;

            TradeSystem.ScheduleSaveToDatabase();
            player.oid.SendServerMessage("Votre commande a bien été annulée", ColorConstants.Orange);
          }

          LoadRequestsLayout();
          rootGroup.SetLayout(player.oid, nuiToken.Token, layoutColumn);
          LoadRequestsBinding();
        }
        private async void ResolveBuyOrderAsync(int unitPrice, int quantity, CraftResource resource)
        {
          Task<int> buyOrderTask = Task.Run(() =>
          {
            foreach (var sellOrder in TradeSystem.sellOrderList.OrderBy(b => b.unitPrice))
            {
              if (sellOrder.unitPrice > unitPrice)
                break;

              if (sellOrder.sellerId == player.characterId || sellOrder.resourceType != resource.type || sellOrder.resourceLevel != resource.grade 
                || sellOrder.quantity < 1 || sellOrder.expirationDate < DateTime.Now)
                continue;

              if (sellOrder.quantity < quantity)
              {
                quantity -= sellOrder.quantity;
                int transactionPrice = sellOrder.quantity * sellOrder.unitPrice;

                TradeSystem.AddResourceToPlayerStock(player, player.characterId, sellOrder.resourceType, sellOrder.resourceLevel, sellOrder.quantity, 
                  $"Vous venez d'acheter {sellOrder.quantity.ToString().ColorString(ColorConstants.White)} unités de {resource.type.ToDescription().ColorString(ColorConstants.White)} {resource.grade.ToString().ColorString(ColorConstants.White)} à un prix unitaire de {sellOrder.unitPrice.ToString().ColorString(ColorConstants.White)} (coût total : {transactionPrice.ToString().ColorString(ColorConstants.White)})",
                  "Successful Buy Order");

                TradeSystem.UpdatePlayerBankAccount(sellOrder.sellerId.ToString(), 
                  TradeSystem.GetTaxedSellPrice(Players.FirstOrDefault(p => p.Value.characterId == sellOrder.sellerId).Value, transactionPrice).ToString(), "Sucessful sell order");

                player.bankGold -= transactionPrice;

                sellOrder.quantity = 0;
                TradeSystem.sellOrderList.Remove(sellOrder);
              }
              else
              {
                int transactionPrice = sellOrder.quantity * sellOrder.unitPrice;
 
                TradeSystem.AddResourceToPlayerStock(player, player.characterId, sellOrder.resourceType, sellOrder.resourceLevel, quantity,
                  $"Vous venez d'acheter {sellOrder.quantity.ToString().ColorString(ColorConstants.White)} unités de {resource.type.ToDescription().ColorString(ColorConstants.White)} {resource.grade.ToString().ColorString(ColorConstants.White)} à un prix unitaire de {sellOrder.unitPrice.ToString().ColorString(ColorConstants.White)} (coût total : {transactionPrice.ToString().ColorString(ColorConstants.White)})",
                  "Successful Buy Order");

                TradeSystem.UpdatePlayerBankAccount(sellOrder.sellerId.ToString(), 
                  TradeSystem.GetTaxedSellPrice(Players.FirstOrDefault(p => p.Value.characterId == sellOrder.sellerId).Value, transactionPrice).ToString(), "Sucessful sell order");

                sellOrder.quantity -= quantity;
                player.bankGold -= transactionPrice;

                if (sellOrder.quantity == 0)
                  TradeSystem.sellOrderList.Remove(sellOrder);

                return 0;
              }
            }

            return quantity;
          });

          await buyOrderTask;
          await NwTask.SwitchToMainThread();

          if (quantity > 0)
          {
            TradeSystem.buyOrderList.Add(new BuyOrder(player.characterId, resource.type, resource.grade, quantity, DateTime.Now.AddMonths(1), unitPrice));
            player.bankGold -= (quantity * unitPrice);

            player.oid.SendServerMessage($"Un ordre d'achat a été créé pour la quantité restante de {quantity.ToString().ColorString(ColorConstants.White)} unités", ColorConstants.Orange);
          }
            
          TradeSystem.ScheduleSaveToDatabase();

          LoadBuyOrders(filteredBuyOrders);
        }
        private async void ResolveSellOrderAsync(int unitPrice, int quantity, CraftResource resource)
        {
          Task<int> sellOrderTask = Task.Run(() =>
          {
            foreach (var buyOrder in TradeSystem.buyOrderList.OrderByDescending(b => b.unitPrice))
            {
              if (buyOrder.unitPrice < unitPrice)
                break;

              if (buyOrder.buyerId == player.characterId || buyOrder.resourceType != resource.type || buyOrder.resourceLevel != resource.grade
                || buyOrder.quantity < 1 || buyOrder.expirationDate < DateTime.Now)
                continue;

              if (buyOrder.quantity < quantity)
              {
                quantity -= buyOrder.quantity;
                int transactionPrice = buyOrder.quantity * buyOrder.unitPrice;

                TradeSystem.AddResourceToPlayerStock(Players.FirstOrDefault(p => p.Value.characterId == buyOrder.buyerId).Value, buyOrder.buyerId,
                   buyOrder.resourceType, buyOrder.resourceLevel, buyOrder.quantity,
                   $"Vous venez de vendre {buyOrder.quantity.ToString().ColorString(ColorConstants.White)} unités de {resource.type.ToDescription().ColorString(ColorConstants.White)} {resource.grade.ToString().ColorString(ColorConstants.White)} à un prix unitaire de {buyOrder.unitPrice.ToString().ColorString(ColorConstants.White)} (gain total : {transactionPrice.ToString().ColorString(ColorConstants.White)})", 
                  "Successful Sell Order");
                
                player.bankGold += TradeSystem.GetTaxedSellPrice(player, transactionPrice);

                buyOrder.quantity = 0;
                TradeSystem.buyOrderList.Remove(buyOrder);
              }
              else
              {
                int transactionPrice = buyOrder.quantity * buyOrder.unitPrice;

                TradeSystem.AddResourceToPlayerStock(Players.FirstOrDefault(p => p.Value.characterId == buyOrder.buyerId).Value, buyOrder.buyerId,
                  buyOrder.resourceType, buyOrder.resourceLevel, quantity,
                  $"Vous venez de vendre {buyOrder.quantity.ToString().ColorString(ColorConstants.White)} unités de {resource.type.ToDescription().ColorString(ColorConstants.White)} {resource.grade.ToString().ColorString(ColorConstants.White)} à un prix unitaire de {buyOrder.unitPrice.ToString().ColorString(ColorConstants.White)} (gain total : {transactionPrice.ToString().ColorString(ColorConstants.White)})",
                  "Successful Sell Order");

                player.bankGold += TradeSystem.GetTaxedSellPrice(player, transactionPrice);
                buyOrder.quantity -= quantity;

                if (buyOrder.quantity == 0)
                  TradeSystem.buyOrderList.Remove(buyOrder);

                return 0;
              }
            }

            return quantity;
          });

          await sellOrderTask;
          await NwTask.SwitchToMainThread();

          if (quantity > 0)
          {
            TradeSystem.sellOrderList.Add(new SellOrder(player.characterId, resource.type, resource.grade, quantity, DateTime.Now.AddMonths(1), unitPrice));
            player.oid.SendServerMessage($"Un ordre de vente a été créé pour la quantité restante de {quantity.ToString().ColorString(ColorConstants.White)} unités", ColorConstants.Orange);
          }

          TradeSystem.ScheduleSaveToDatabase();

          LoadSellOrders(filteredSellOrders);
        }
        private void CleanUpProposalItems()
        {
          foreach (NwItem item in tradeProposalItemScheduledForDestruction)
            if (item.IsValid)
            {
              Log.Info($"TRADE SYSTEM - Player {player.characterId} - Cleaning proposal item {item.Name}");
              item.Destroy();
            }

          foreach (NwItem item in newProposalItems)
            if (item.IsValid)
            {
              Log.Info($"TRADE SYSTEM - Player {player.characterId} - Cleaning new proposal item {item.Name}");
              item.Destroy();
            }

          player.oid.OnClientLeave -= OnClientLeave;
        }
        private void OnClientLeave(ModuleEvents.OnClientLeave onLeave)
        {
          CleanUpProposalItems();
        }
      }
    }
  }
}
