using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.Services;

using Newtonsoft.Json;

using NLog;

using static NWN.Systems.CraftResource;
using static NWN.Systems.LootSystem.Lootable;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(TradeSystem))]
  public partial class TradeSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private readonly SchedulerService scheduler;
    public static List<TradeRequest> tradeRequestList;
    public static List<Auction> auctionList;
    public static List<BuyOrder> buyOrderList;
    public static List<SellOrder> sellOrderList;
    private static bool saveScheduled = false;
    public TradeSystem(SchedulerService schedulerService)
    {
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
        new List<string[]>() { { Array.Empty<string>() } });

      if (result != null && result.Count > 0)
      {
        string serializedRequests = result.FirstOrDefault()[0];
        List<TradeRequest> serializedTradeRequests = new();

        Task loadRequests = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedRequests))
            return;

          serializedTradeRequests = JsonConvert.DeserializeObject<List<TradeRequest>>(serializedRequests);
        });

        await loadRequests;
        tradeRequestList = serializedTradeRequests;
      }
      else
      {
        tradeRequestList = new List<TradeRequest>();

        await SqLiteUtils.InsertQueryAsync("trade",
        new List<string[]>() { new string[] { "requests", "" }, new string[] { "auctions", "" }, new string[] { "buyOrders", "" }, new string[] { "sellOrders", "" } });
      }
    }
    private static async void DeserializeAuctions()
    {
      var result = await SqLiteUtils.SelectQueryAsync("trade",
        new List<string>() { { "auctions" } },
        new List<string[]>() { { Array.Empty<string>() } });

      if (result != null && result.Count > 0)
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
        auctionList = serializedTradeRequests;
      }
      else
        auctionList = new();
    }
    private static async void DeserializeBuyOrders()
    {
      var result = await SqLiteUtils.SelectQueryAsync("trade",
        new List<string>() { { "buyOrders" } },
        new List<string[]>() { { Array.Empty<string>() } });

      if (result != null && result.Count > 0)
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
        buyOrderList = serializedTradeRequests;
      }
      else
        buyOrderList = new();
    }
    private static async void DeserializeSellOrders()
    {
      var result = await SqLiteUtils.SelectQueryAsync("trade",
        new List<string>() { { "sellOrders" } },
        new List<string[]>() { { Array.Empty<string>() } });

      if (result != null && result.Count > 0)
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
        sellOrderList = serializedTradeRequests;
      }
      else
        sellOrderList = new();
    }

    public static async void ScheduleSaveToDatabase()
    {
      if (!saveScheduled)
      {
        saveScheduled = true;
        Log.Info("TRADE SYSTEM - Scheduling save in 10 seconds");
        await NwTask.Delay(TimeSpan.FromSeconds(10));
        SaveToDatabase();
      }
    }

    public static async void SaveToDatabase()
    {
      Log.Info("TRADE SYSTEM - STARTING SAVE PROCESS");
      DateTime elapsed = DateTime.Now;

      Task<string> serializeRequests = Task.Run(() => JsonConvert.SerializeObject(tradeRequestList));
      Task<string> serializeAuctions = Task.Run(() => JsonConvert.SerializeObject(auctionList));
      Task<string> serializeBuyOrders = Task.Run(() => JsonConvert.SerializeObject(buyOrderList));
      Task<string> serializeSellOrders = Task.Run(() => JsonConvert.SerializeObject(sellOrderList));

      await Task.WhenAll(serializeRequests, serializeAuctions, serializeBuyOrders, serializeSellOrders);

      SqLiteUtils.UpdateQuery("trade",
        new List<string[]>() { new string[] { "requests", serializeRequests.Result }, new string[] { "auctions", serializeAuctions.Result }, new string[] { "buyOrders", serializeBuyOrders.Result }, new string[] { "sellOrders", serializeSellOrders.Result } },
        new List<string[]>() { new string[] { "rowid", "1" } });

      saveScheduled = false;

      Log.Info($"TRADE SYSTEM - SAVED FINALIZED in {(DateTime.Now - elapsed).TotalSeconds} s");
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
            GiveItemAuction(auction.auctionerId, auction.itemName, auction.serializedItem);
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
          GiveBackResources(sellOrder);
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
            if (buyer.pcState != Player.PcState.Offline)
              buyer.oid.SendServerMessage($"Votre ordre d'achat pour {buyOrder.quantity} {buyOrder.resourceType.ToDescription().ColorString(ColorConstants.White)} {buyOrder.resourceLevel} a expiré. La banque Skalsgard a débloqué les fond immobilisés pour l'opération", ColorConstants.Orange);

            buyer.bankGold += sellPrice;
          }

          UpdatePlayerBankAccount(buyOrder.buyerId.ToString(), sellPrice.ToString(), "Buy Order Expired");

          Log.Info($"TRADE SYSTEM - Buy order expired of {buyOrder.quantity} {buyOrder.resourceType.ToDescription()} {buyOrder.resourceLevel} from {buyOrder.buyerId}");
        }
      }

      buyOrderList.RemoveAll(trade => trade.expirationDate < DateTime.Now);
    }
    public static void GiveItemAuction(int characterId, string itemName, string serializedItem, int highestBid = 0)
    {
      Player player = Players.FirstOrDefault(p => p.Value.characterId == characterId).Value;

      if (player != null && player.pcState != Player.PcState.Offline && player.windows.ContainsKey("bankStorage") && 
        ((Player.BankStorageWindow)player.windows["bankStorage"]).items != null) 
      {
        Player.BankStorageWindow bankWindow = ((Player.BankStorageWindow)player.windows["bankStorage"]);
        bankWindow.items.Add(NwItem.Deserialize(serializedItem.ToByteArray()));
        bankWindow.BankSave();

        if(highestBid < 1)
          player.oid.SendServerMessage($"Votre enchère pour {itemName.ColorString(ColorConstants.White)} n'a pas trouvé d'acheteur. L'objet a été envoyé dans votre coffre Skalsgard.", ColorConstants.Orange);
        else
          player.oid.SendServerMessage($"Votre avez remportez l'enchère pour {itemName.ColorString(ColorConstants.White)} pour un prix de {highestBid.ToString().ColorString(ColorConstants.White)}. L'objet a été envoyé dans votre coffre Skalsgard.", ColorConstants.Orange);

        return;
      }

      AddItemToPlayerDataBaseBank(characterId.ToString(), new List<string>() { serializedItem }, "Auction expired");
      // TODO : ajouter notification par lettre
    }
    private static void GiveBackResources(SellOrder sellOrder)
    {
      Player seller = Players.FirstOrDefault(p => p.Value.characterId == sellOrder.sellerId).Value;
      CraftResource resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == sellOrder.resourceType && r.grade == sellOrder.resourceLevel);

      if (seller != null)
      {
        if (seller.pcState != Player.PcState.Offline)
          seller.oid.SendServerMessage($"Votre ordre de vente de {sellOrder.quantity} {sellOrder.resourceType.ToDescription()} {sellOrder.resourceLevel} a expiré. Les ressources libérées sont de nouveau disponibles dans votre entrepôt.", ColorConstants.Orange);

        CraftResource playerResource = seller.craftResourceStock.FirstOrDefault(r => r.type == sellOrder.resourceType && r.grade == sellOrder.resourceLevel);

        if (playerResource != null)
          playerResource.quantity += sellOrder.quantity;
        else
          seller.craftResourceStock.Add(new CraftResource(resource, sellOrder.quantity));
      }

      UpdatePlayerResourceStock(sellOrder.sellerId.ToString(), resource, sellOrder.quantity);
      // TODO : ajouter notification par lettre
    }
    public static void ResolveSuccessfulAuction(Auction auction)
    {
      Player seller = Players.FirstOrDefault(p => p.Value.characterId == auction.auctionerId).Value;
      int sellPrice = GetTaxedSellPrice(seller, auction.highestBid);

      if (seller != null)
      {
        if (seller.pcState != Player.PcState.Offline)
          seller.oid.SendServerMessage($"La vente aux enchères de votre {auction.itemName.ColorString(ColorConstants.White)} vient de vous rapporter {sellPrice}. Félicitations !", ColorConstants.Orange);

        seller.bankGold += sellPrice;
      }

      UpdatePlayerBankAccount(auction.auctionerId.ToString(), sellPrice.ToString(), "Auction successful");
      GiveItemAuction(auction.highestBidderId, auction.itemName, auction.serializedItem, auction.highestBid);
    }
    public static async void AddItemToPlayerDataBaseBank(string characterId, List<string> serializedItemstoAdd, string logUseCase)
    {
      var result = await SqLiteUtils.SelectQueryAsync("playerCharacters",
        new List<string>() { { "persistantStorage" } },
        new List<string[]>() { { new string[] { "rowid", characterId } } });

      if (result != null)
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
    }
    public static int GetTaxedSellPrice(Player seller, int sellPrice)
    {
      double tax = sellPrice * 0.92;

      if (seller != null && seller.learnableSkills.ContainsKey(CustomSkill.Comptabilite))
        tax *= (1 - (seller.learnableSkills[CustomSkill.Comptabilite].totalPoints * 0.11));

      return sellPrice - (int)tax;
    }
    public static void UpdatePlayerBankAccount(string characterId, string sellPrice, string logUseCase)
    {
      SqLiteUtils.UpdateQuery("playerCharacters",
        new List<string[]>() { new string[] { "bankGold", sellPrice, "+" } },
        new List<string[]>() { new string[] { "ROWID", characterId } });

      Log.Info($"TRADE SYSTEM - {logUseCase} - Updated bank account {characterId} of {sellPrice}");

      // TODO : ajouter notification par lettre
    }
    private static async void UpdatePlayerResourceStock(string characterId, CraftResource resource, int quantity)
    {
      var result = await SqLiteUtils.SelectQueryAsync("playerCharacters",
        new List<string>() { { "materialStorage" } },
        new List<string[]>() { new string[] { "ROWID", characterId } });

      if (result != null && result.Count > 0)
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

        Log.Info($"TRADE SYSTEM - Expired Sell Order - Updated resource stock of {characterId} of {quantity}");

        // TODO : ajouter notification par lettre
      }
    }
    public static async void AddResourceToPlayerStock(Player player, ResourceType type, int grade, int quantity)
    {
      if (player != null)
      {
        CraftResource resource = player.craftResourceStock.FirstOrDefault(r => r.type == type && r.grade == grade);

        if (resource != null)
          resource.quantity += quantity;
        else
        {
          resource = Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == type && r.grade == grade);
          player.craftResourceStock.Add(new CraftResource(resource, quantity));
        }

        // TODO : si player est null, aller chercher la correspondance en base de données et lui filer ses ressources puis réenregistrer le tout
      }
    }
  }
}
