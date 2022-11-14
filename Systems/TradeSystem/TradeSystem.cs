using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.Services;

using Newtonsoft.Json;

using NLog;

using static NWN.Systems.CraftResource;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(TradeSystem))]
  public partial class TradeSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private readonly SchedulerService scheduler;
    public static readonly List<TradeRequest> tradeRequestList = new();
    public static readonly List<Auction> auctionList = new();
    public static readonly List<BuyOrder> buyOrderList = new();
    public static readonly List<SellOrder> sellOrderList = new();
    public static bool saveScheduled { get; set; }
    public TradeSystem(SchedulerService schedulerService)
    {
      saveScheduled = false;

      DeserializeTradeRequests();
      DeserializeAuctions();
      DeserializeBuyOrders();
      DeserializeSellOrders();

      scheduler = schedulerService;
      scheduler.ScheduleRepeating(DeleteExpiredTrades, TimeSpan.FromMinutes(1));
    }

    private static async void DeserializeTradeRequests()
    {
      var result = await SqLiteUtils.SelectQueryAsync("trade",
        new List<string>() { { "requests" } },
        new List<string[]>() { });

      if (result != null && result.Count > 0)
      {
        string serializedRequests = result.FirstOrDefault()[0];

        Task loadRequests = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedRequests))
            return;

          List<TradeRequest.SerializableTradeRequest> serializedTradeRequest = JsonConvert.DeserializeObject<List<TradeRequest.SerializableTradeRequest>>(serializedRequests);

          foreach (var tradeRequest in serializedTradeRequest)
            tradeRequestList.Add(new TradeRequest(tradeRequest));
        });
      }
      else
      {
        await SqLiteUtils.InsertQueryAsync("trade",
        new List<string[]>() { new string[] { "requests", "" }, new string[] { "auctions", "" }, new string[] { "buyOrders", "" }, new string[] { "sellOrders", "" } });
      }
    }
    private static async void DeserializeAuctions()
    {
      var result = await SqLiteUtils.SelectQueryAsync("trade",
        new List<string>() { { "auctions" } },
        new List<string[]>() { });

      if (result != null && result.Count > 0)
      {
        string serializedRequests = result.FirstOrDefault()[0];

        Task loadRequests = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedRequests))
            return;

          List<Auction.SerializableAuction> serializedAuction = JsonConvert.DeserializeObject<List<Auction.SerializableAuction>>(serializedRequests);

          foreach (var auction in serializedAuction)
            auctionList.Add(new Auction(auction));
        });
      }
    }
    private static async void DeserializeBuyOrders()
    {
      var result = await SqLiteUtils.SelectQueryAsync("trade",
        new List<string>() { { "buyOrders" } },
        new List<string[]>() { });

      if (result != null && result.Count > 0)
      {
        string serializedRequests = result.FirstOrDefault()[0];
        
        Task loadRequests = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedRequests))
            return;

          List<BuyOrder.SerializableBuyOrder> serializedBuyOrders = JsonConvert.DeserializeObject<List<BuyOrder.SerializableBuyOrder>>(serializedRequests);
          
          foreach (var buyOrder in serializedBuyOrders)
            buyOrderList.Add(new BuyOrder(buyOrder));
        });
      }
    }
    private static async void DeserializeSellOrders()
    {
      var result = await SqLiteUtils.SelectQueryAsync("trade",
        new List<string>() { { "sellOrders" } },
        new List<string[]>() { });

      if (result != null && result.Count > 0)
      {
        string serializedRequests = result.FirstOrDefault()[0];
        
        Task loadRequests = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedRequests))
            return;

          List<SellOrder.SerializableSellOrder> serializedSellOrders = JsonConvert.DeserializeObject<List<SellOrder.SerializableSellOrder>>(serializedRequests);
          
          foreach (var sellOrder in serializedSellOrders)
            sellOrderList.Add(new SellOrder(sellOrder));
        });
      }
    }

    public static async void ScheduleSaveToDatabase()
    {
      if (!saveScheduled)
      {
        saveScheduled = true;
        //Log.Info("TRADE SYSTEM - Scheduling save in 10 seconds");
        await NwTask.Delay(TimeSpan.FromSeconds(10));
        SaveToDatabase();
      }
    }

    public static async void SaveToDatabase()
    {
      /*Log.Info("TRADE SYSTEM - STARTING SAVE PROCESS");
      DateTime elapsed = DateTime.Now;*/

      Task<string> serializeRequests = Task.Run(() =>
      {
        List<TradeRequest.SerializableTradeRequest> serializableTradeRequests = new();
        foreach (var tradeRequest in tradeRequestList)
          serializableTradeRequests.Add(new TradeRequest.SerializableTradeRequest(tradeRequest));

        return JsonConvert.SerializeObject(serializableTradeRequests);
      });

      Task<string> serializeAuctions = Task.Run(() =>
      {
        List<Auction.SerializableAuction> serializableAuctions = new();
        foreach (var auction in auctionList)
          serializableAuctions.Add(new Auction.SerializableAuction(auction));

        return JsonConvert.SerializeObject(auctionList);
      });

      Task<string> serializeBuyOrders = Task.Run(() =>
      {
        List<BuyOrder.SerializableBuyOrder> serializableBuyOrders = new();
        foreach (var buyOrder in buyOrderList)
          serializableBuyOrders.Add(new BuyOrder.SerializableBuyOrder(buyOrder));

        return JsonConvert.SerializeObject(serializableBuyOrders);
      });

      Task<string> serializeSellOrders = Task.Run(() =>
      {
        List<SellOrder.SerializableSellOrder> serializableSellOrders = new();
        foreach (var sellOrder in sellOrderList)
          serializableSellOrders.Add(new SellOrder.SerializableSellOrder(sellOrder));

        return JsonConvert.SerializeObject(serializableSellOrders);
      });

      await Task.WhenAll(serializeRequests, serializeAuctions, serializeBuyOrders, serializeSellOrders);

      SqLiteUtils.UpdateQuery("trade",
        new List<string[]>() { new string[] { "requests", serializeRequests.Result }, new string[] { "auctions", serializeAuctions.Result }, new string[] { "buyOrders", serializeBuyOrders.Result }, new string[] { "sellOrders", serializeSellOrders.Result } },
        new List<string[]>() { new string[] { "rowid", "1" } });

      saveScheduled = false;

      //Log.Info($"TRADE SYSTEM - SAVED FINALIZED in {(DateTime.Now - elapsed).TotalSeconds} s");
    }
    private async void DeleteExpiredTrades()
    {
      Task deleteExpiredRequests = Task.Run(() => { DeleteExpiredRequests(); });
      Task deleteExpiredBuyOrders = Task.Run(() => { DeleteExpiredBuyOrders(); });
      Task deleteExpiredSellOrders = Task.Run(() => { DeleteExpiredSellOrders(); });
      Task deleteExpiredAuctions = Task.Run(() => { DeleteExpiredAuctions(); });

      await Task.WhenAll(deleteExpiredRequests, deleteExpiredAuctions, deleteExpiredBuyOrders, deleteExpiredSellOrders);

      if (!saveScheduled)
        ScheduleSaveToDatabase();
    }
    private static void DeleteExpiredRequests()
    {
      foreach(var request in tradeRequestList)
      {
        if(request.expirationDate < DateTime.Now)
        {
          foreach(var proposal in request.proposalList)
          {
            if(!proposal.cancelled)
            {
              proposal.cancelled = true;

              UpdatePlayerBankAccount(proposal.characterId.ToString(), proposal.sellPrice.ToString(), "Proposal cancelled - Request Expired");
              AddItemToPlayerDataBaseBank(proposal.characterId.ToString(), proposal.serializedItems, "Proposal cancelled - Request Expired");
            }
          }
        }
      }

      tradeRequestList.RemoveAll(trade => trade.expirationDate < DateTime.Now);
    }
    private static void DeleteExpiredAuctions()
    {
      foreach (var auction in auctionList)
      {
        if (auction.expirationDate < DateTime.Now)
        {
          if (auction.highestBid == 0)
            GiveItemAuction(auction);
          else if(auction.buyoutPrice > 0 && auction.highestBid < auction.buyoutPrice)
            ResolveSuccessfulAuction(auction);

          Log.Info($"TRADE SYSTEM - Auction expired of {auction.itemName} from {auction.auctionerId} won by {auction.highestBidderId} for {auction.highestBid}");
        }
      }

      auctionList.RemoveAll(trade => trade.expirationDate < DateTime.Now);
    }
    private static void DeleteExpiredSellOrders()
    {
      foreach (var sellOrder in sellOrderList)
      {
        if (sellOrder.expirationDate < DateTime.Now)
        {
          AddResourceToPlayerStock(Players.FirstOrDefault(p => p.Value.characterId == sellOrder.sellerId).Value, sellOrder.sellerId, 
            sellOrder.resourceType, sellOrder.resourceLevel, sellOrder.quantity, $"Votre ordre de vente de {sellOrder.quantity} {sellOrder.resourceType.ToDescription()} {sellOrder.resourceLevel} a expiré. Les ressources libérées sont de nouveau disponibles dans votre entrepôt.",
            "Sell Order Expired");

          Log.Info($"TRADE SYSTEM - Sell order expired of {sellOrder.quantity} {sellOrder.resourceType.ToDescription()} {sellOrder.resourceLevel} from {sellOrder.sellerId}");
        }
      }

      sellOrderList.RemoveAll(trade => trade.expirationDate < DateTime.Now);
    }
    private static void DeleteExpiredBuyOrders()
    {
      foreach (var buyOrder in buyOrderList)
      {
        if (buyOrder.expirationDate < DateTime.Now)
        {
          Player buyer = Players.FirstOrDefault(p => p.Value.characterId == buyOrder.buyerId).Value;
          int sellPrice = buyOrder.unitPrice * buyOrder.quantity;

          if (buyer != null)
          {
            buyer.bankGold += sellPrice;

            if (buyer.pcState != Player.PcState.Offline)
              buyer.oid.SendServerMessage($"Votre ordre d'achat pour {StringUtils.ToWhitecolor(buyOrder.quantity)} {StringUtils.ToWhitecolor(buyOrder.resourceType.ToDescription())} {StringUtils.ToWhitecolor(buyOrder.resourceLevel)} a expiré. La banque Skalsgard a débloqué les fond immobilisés pour l'opération (solde {StringUtils.ToWhitecolor(buyer.bankGold)})", ColorConstants.Orange);
          }

          UpdatePlayerBankAccount(buyOrder.buyerId.ToString(), sellPrice.ToString(), "Buy Order Expired");

          Log.Info($"TRADE SYSTEM - Buy order expired of {buyOrder.quantity} {buyOrder.resourceType.ToDescription()} {buyOrder.resourceLevel} from {buyOrder.buyerId}");
        }
      }

      buyOrderList.RemoveAll(trade => trade.expirationDate < DateTime.Now);
    }
    public static void GiveItemAuction(Auction auction)
    {
      Player auctionner = Players.FirstOrDefault(p => p.Value.characterId == auction.auctionerId).Value;

      if (auction.highestBid < 1) // L'enchère est expirée et n'a pas trouvé d'acheteur
      {
        if (auctionner != null && auctionner.pcState != Player.PcState.Offline)
          auctionner.oid.SendServerMessage($"Votre enchère pour {auction.itemName.ColorString(ColorConstants.White)} n'a pas trouvé d'acheteur. L'objet a été envoyé dans votre coffre Skalsgard.", ColorConstants.Orange);

        AddItemToPlayerDataBaseBank(auction.auctionerId.ToString(), new List<string>() { auction.serializedItem }, "Auction expired, no bidder");
        return;
      }
      else // L'enchère est remportée par le highest bidder
      {
        Player bidder = Players.FirstOrDefault(p => p.Value.characterId == auction.highestBidderId).Value;

        if (bidder != null && bidder.pcState != Player.PcState.Offline)
        {
          if (auction.highestBid >= auction.buyoutPrice) // cas achat direct
          {
            ItemUtils.DeserializeAndAcquireItem(auction.serializedItem, bidder.oid.LoginCreature);
            bidder.oid.SendServerMessage($"Vous venez d'acquérir {auction.itemName.ColorString(ColorConstants.White)} pour un prix de {auction.highestBid.ToString().ColorString(ColorConstants.White)}.", ColorConstants.Orange);
            return;
          }
          else
            bidder.oid.SendServerMessage($"Votre avez remportez l'enchère pour {auction.itemName.ColorString(ColorConstants.White)} pour un prix de {auction.highestBid.ToString().ColorString(ColorConstants.White)}. L'objet a été envoyé dans votre coffre Skalsgard.", ColorConstants.Orange);
        }

        AddItemToPlayerDataBaseBank(bidder.characterId.ToString(), new List<string>() { auction.serializedItem }, "Auction successful");
        // TODO : ajouter notification par lettre
      }
    }
    public static async void AddResourceToPlayerStock(Player player, int characterId, ResourceType type, int grade, int quantity, string playerMessage, string logUseCase)
    {
      CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == type && r.grade == grade);

      if (player != null)
      {
        CraftResource playerResource = player.craftResourceStock.FirstOrDefault(r => r.type == type && r.grade == grade);
        int messageQuantity;

        if (playerResource != null)
        {
          playerResource.quantity += quantity;
          messageQuantity = playerResource.quantity;
        }
        else
        {
          player.craftResourceStock.Add(new CraftResource(resource, quantity));
          messageQuantity = quantity;
        }

        if (player.pcState != Player.PcState.Offline)
        {
          await NwTask.SwitchToMainThread();
          player.oid.SendServerMessage($"{playerMessage} (solde {StringUtils.ToWhitecolor(messageQuantity)})", ColorConstants.Orange);
        }
      }

      UpdatePlayerResourceStock(characterId.ToString(), resource, quantity, logUseCase);
    }
    public static void ResolveSuccessfulAuction(Auction auction)
    {
      Player seller = Players.FirstOrDefault(p => p.Value.characterId == auction.auctionerId).Value;
      int sellPrice = GetTaxedSellPrice(seller, auction.highestBid);

      if (seller != null)
      {
        seller.bankGold += sellPrice;

        if (seller.pcState != Player.PcState.Offline)
          seller.oid.SendServerMessage($"La vente aux enchères de votre {StringUtils.ToWhitecolor(auction.itemName)} vient de vous rapporter {StringUtils.ToWhitecolor(sellPrice)} (solde {StringUtils.ToWhitecolor(seller.bankGold)})", ColorConstants.Orange);
      }

      UpdatePlayerBankAccount(auction.auctionerId.ToString(), sellPrice.ToString(), "Auction successful");
      GiveItemAuction(auction);
    }
    public static async void AddItemToPlayerDataBaseBank(string characterId, List<string> serializedItemstoAdd, string logUseCase)
    {
      var result = await SqLiteUtils.SelectQueryAsync("playerCharacters",
        new List<string>() { { "persistantStorage" } },
        new List<string[]>() { { new string[] { "rowid", characterId } } });

      if (result != null && result.Any())
      {
        string serializedBank = result.FirstOrDefault()[0];
        List<string> serializedItems = new List<string>();

        Task loadBank = Task.Run(async () =>
        {
          if (string.IsNullOrEmpty(serializedBank))
            return;

          serializedItems = JsonConvert.DeserializeObject<List<string>>(serializedBank);
          serializedItems.AddRange(serializedItemstoAdd);

          Task<string> serializeBank = Task.Run(() => JsonConvert.SerializeObject(serializedItems));

          await serializeBank;

          SqLiteUtils.UpdateQuery("playerCharacters",
            new List<string[]>() { new string[] { "persistantStorage", serializeBank.Result } },
            new List<string[]>() { new string[] { "rowid", characterId } });

          Log.Info($"TRADE SYSTEM - {logUseCase} - serialized bank for {characterId}");
        });
      }
      else
        Utils.LogMessageToDMs($"TRADE SYSTEM ERROR - {logUseCase} - Impossible de trouver le personnage {characterId}");
    }
    public static int GetTaxedSellPrice(Player seller, int sellPrice)
    {
      double tax = sellPrice * 0.92;

      if (seller != null && seller.learnableSkills.ContainsKey(CustomSkill.Comptabilite))
        tax *= (1 - (seller.learnableSkills[CustomSkill.Comptabilite].totalPoints * 0.11));

      return (int)tax;
    }
    public static void UpdatePlayerBankAccount(string characterId, string sellPrice, string logUseCase)
    {
      SqLiteUtils.UpdateQuery("playerCharacters",
        new List<string[]>() { new string[] { "bankGold", sellPrice, "+" } },
        new List<string[]>() { new string[] { "ROWID", characterId } });

      Log.Info($"TRADE SYSTEM - {logUseCase} - Updated bank account {characterId} of {sellPrice}");

      // TODO : ajouter notification par lettre
    }
    private static async void UpdatePlayerResourceStock(string characterId, CraftResource resource, int quantity, string logUseCase)
    {
      var result = await SqLiteUtils.SelectQueryAsync("playerCharacters",
        new List<string>() { { "materialStorage" } },
        new List<string[]>() { new string[] { "ROWID", characterId } });

      if (result != null && result.Any())
      {
        string serializedCraftResources = result[0][0];

        Task loadMateriaTask = Task.Run(() =>
        {
          List<SerializableCraftResource> serializableCraftResource = JsonConvert.DeserializeObject<List<SerializableCraftResource>>(serializedCraftResources);

          SerializableCraftResource playerResource = serializableCraftResource.FirstOrDefault(r => r.type == (int)resource.type && r.grade == resource.grade);

          if (playerResource != null)
            playerResource.quantity += quantity;
          else
            serializableCraftResource.Add(new SerializableCraftResource(new CraftResource(resource, quantity)));

          Task serializeCraftResource = Task.Run(() =>
          {
            SqLiteUtils.UpdateQuery("playerCharacters",
              new List<string[]>() { new string[] { "materialStorage", JsonConvert.SerializeObject(serializableCraftResource) } },
              new List<string[]>() { new string[] { "ROWID", characterId } });
          });
        });

        Log.Info($"TRADE SYSTEM - {logUseCase} - Updated resource stock of {characterId} of {quantity}");
        // TODO : ajouter notification par lettre
      }
      else
        Log.Info($"TRADE SYSTEM ERROR - {logUseCase} - Impossible de trouver le personnage {characterId}");
    }
  }
}
