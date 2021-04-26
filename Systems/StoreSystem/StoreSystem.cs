using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems;
using System;
using NWN.API;
using System.Linq;
using NWN.API.Events;

namespace NWN.System
{
  [ServiceBinding(typeof(StoreSystem))]
  public class StoreSystem
  {
    public static void OnOpenPersonnalStorage(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnClosePersonnalStorage;
      onOpen.Player.OnStoreRequestBuy += HandlePersonnalStorageBuy;
      onOpen.Player.OnStoreRequestSell += HandlePersonnalStorageSell;
    }
    public static void OnClosePersonnalStorage(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenPersonnalStorage;
      onClose.Store.OnClose -= OnClosePersonnalStorage;
      onClose.Creature.OnStoreRequestBuy -= HandlePersonnalStorageBuy;
      onClose.Creature.OnStoreRequestSell -= HandlePersonnalStorageSell;

      if (PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
      {
        var saveStorage = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
        NWScript.SqlBindInt(saveStorage, "@characterId", seller.characterId);
        NWScript.SqlBindObject(saveStorage, "@storage", onClose.Store);
        NWScript.SqlStep(saveStorage);
      }
    }
    public static void HandlePersonnalStorageBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      onStoreRequestBuy.PreventBuy = true;
      onStoreRequestBuy.Item.Clone(onStoreRequestBuy.Creature);
      onStoreRequestBuy.Item.Destroy();
    }
    public static void HandlePersonnalStorageSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;
      onStoreRequestSell.Item.Clone(onStoreRequestSell.Store);
      onStoreRequestSell.Item.Destroy();
    }
    public static void OnOpenPlayerShop(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnClosePlayerShop;
      onOpen.Player.OnStoreRequestBuy += HandlePlayerShopBuy;
      onOpen.Player.OnStoreRequestSell += HandlePlayerShopSell;
    }
    public static void OnClosePlayerShop(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenPlayerShop;
      onClose.Store.OnClose -= OnClosePlayerShop;
      onClose.Creature.OnStoreRequestBuy -= HandlePlayerShopBuy;
      onClose.Creature.OnStoreRequestSell -= HandlePlayerShopSell;

      if (!PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
        return;

      if (onClose.Store.Area.Tag != "Promenadetest")
        return;

      NwPlaceable panel = onClose.Store.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag.StartsWith($"_PLAYER_SHOP_PLC_"));

      if (onClose.Store.GetLocalVariable<int>("_SHOP_ID").HasNothing)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerShops (characterId, shop, panel, expirationDate, areaTag, position, facing) VALUES (@characterId, @shop, @panel, @expirationDate, @areaTag, @position, @facing)");
        NWScript.SqlBindInt(query, "@characterId", seller.characterId);
        NWScript.SqlBindString(query, "@shop", onClose.Store.Serialize().ToBase64EncodedString());
        NWScript.SqlBindString(query, "@panel", panel.Serialize().ToBase64EncodedString());
        NWScript.SqlBindString(query, "@expirationDate", DateTime.Now.AddDays(30).ToString());
        NWScript.SqlBindString(query, "@areaTag", onClose.Store.Area.Tag);
        NWScript.SqlBindVector(query, "@position", onClose.Store.Position);
        NWScript.SqlBindFloat(query, "@facing", onClose.Store.Rotation);
        NWScript.SqlStep(query);

        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
        NWScript.SqlStep(query);

        onClose.Store.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 0);
        panel.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 0);
      }
      else
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerShops set shop = @shop, panel = @panel where rowid = @shopId");
        NWScript.SqlBindInt(query, "@shopId", onClose.Store.GetLocalVariable<int>("_SHOP_ID").Value);
        NWScript.SqlBindString(query, "@shop", onClose.Store.Serialize().ToBase64EncodedString());
        NWScript.SqlBindString(query, "@panel", panel.Serialize().ToBase64EncodedString());
        NWScript.SqlStep(query);
      }
    }
    public static void HandlePlayerShopBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player buyer))
        return;

      if (onStoreRequestBuy.Store.GetLocalVariable<int>("_OWNER_ID").Value != buyer.characterId)
      {
        if (onStoreRequestBuy.Result.Value)
        {
          int price = onStoreRequestBuy.Price * 95 / 100;
          int ownerId = onStoreRequestBuy.Store.GetLocalVariable<int>("_OWNER_ID").Value;
          int shopId = onStoreRequestBuy.Store.GetLocalVariable<int>("_SHOP_ID").Value;

          NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == ownerId);
          if (oSeller != null)
          {
            if (PlayerSystem.Players.TryGetValue(oSeller, out PlayerSystem.Player seller))
              seller.bankGold += price;

            oSeller.SendServerMessage($"La vente de votre {onStoreRequestBuy.Item.Name.ColorString(Color.ORANGE)} à {buyer.oid.Name.ColorString(Color.PINK)} vient de vous rapporter {price.ToString().ColorString(Color.GREEN)}");
          }
          else
          {
            // TODO : envoyer un courrier à la prochaine connexion du seller pour le prévenir de sa vente.

            var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET bankGold = bankGold + @bankGold where rowid = @characterId");
            NWScript.SqlBindInt(query, "@characterId", ownerId);
            NWScript.SqlBindInt(query, "@bankGold", price);
            NWScript.SqlStep(query);
          }
        }
      }
      else
      {
        onStoreRequestBuy.PreventBuy = true;
        onStoreRequestBuy.Item.Clone(buyer.oid);
        onStoreRequestBuy.Item.Destroy();
      }
    }
    public static void HandlePlayerShopSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Impossible de vendre dans ce type d'échoppe.", Color.ORANGE);
    }
    public static void OnOpenPlayerAuction(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnClosePlayerAuction;
      onOpen.Player.OnStoreRequestBuy += HandlePlayerAuctionBuy;
      onOpen.Player.OnStoreRequestSell += HandlePlayerAuctionSell;
    }
    public static void OnClosePlayerAuction(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenPlayerAuction;
      onClose.Store.OnClose -= OnClosePlayerAuction;
      onClose.Creature.OnStoreRequestBuy -= HandlePlayerAuctionBuy;
      onClose.Creature.OnStoreRequestSell -= HandlePlayerAuctionSell;

      if (!PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
        return;

      if (onClose.Store.Area.Tag != "Promenadetest")
        return;

      NwPlaceable panel = onClose.Store.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag.StartsWith("_PLAYER_AUCTION_PLC_"));
      panel.Name = "[ENCHERES] ".ColorString(Color.ORANGE) +
        onClose.Store.Items.FirstOrDefault().Name.ColorString(Color.OLIVE) + " " +
        onClose.Store.GetLocalVariable<int>("_CURRENT_AUCTION").Value + " Fin : " + onClose.Store.GetLocalVariable<int>("_AUCTION_END_DATE").Value;

      if (onClose.Store.GetLocalVariable<int>("_AUCTION_ID").HasNothing)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerAuctions (characterId, shop, panel, expirationDate, highestAuction, highestAuctionner, areaTag, position, facing) VALUES (@characterId, @shop, @panel, @expirationDate, @highestAuction, @highestAuctionner, @areaTag, @position, @facing)");
        NWScript.SqlBindInt(query, "@characterId", seller.characterId);
        NWScript.SqlBindString(query, "@shop", onClose.Store.Serialize().ToBase64EncodedString());
        NWScript.SqlBindString(query, "@panel", panel.Serialize().ToBase64EncodedString());
        NWScript.SqlBindString(query, "@expirationDate", onClose.Store.GetLocalVariable<string>("_AUCTION_END_DATE").Value);
        NWScript.SqlBindInt(query, "@highestAuction", onClose.Store.GetLocalVariable<int>("_CURRENT_AUCTION").Value);
        NWScript.SqlBindInt(query, "@highestAuctionner", 0);
        NWScript.SqlBindString(query, "@areaTag", panel.Area.Tag);
        NWScript.SqlBindVector(query, "@position", panel.Position);
        NWScript.SqlBindFloat(query, "@facing", panel.Rotation);
        NWScript.SqlStep(query);

        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
        NWScript.SqlStep(query);

        onClose.Store.GetLocalVariable<int>("_AUCTION_ID").Value = NWScript.SqlGetInt(query, 0);
        panel.GetLocalVariable<int>("_AUCTION_ID").Value = NWScript.SqlGetInt(query, 0);
      }
      else
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerShops set shop = @shop, panel = @panel, highestAuction = @highestAuction, highestAuctionner = @highestAuctionner where rowid = @shopId");
        NWScript.SqlBindInt(query, "@shopId", onClose.Store.GetLocalVariable<int>("_AUCTION_ID").Value);
        NWScript.SqlBindInt(query, "@highestAuction", onClose.Store.GetLocalVariable<int>("_CURRENT_AUCTION").Value);
        NWScript.SqlBindInt(query, "@highestAuctionner", onClose.Store.GetLocalVariable<int>("_CURRENT_AUCTIONNER").Value);
        NWScript.SqlBindString(query, "@shop", onClose.Store.Serialize().ToBase64EncodedString());
        NWScript.SqlBindString(query, "@panel", panel.Serialize().ToBase64EncodedString());
        NWScript.SqlStep(query);
      }
    }
    public static void HandlePlayerAuctionBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      onStoreRequestBuy.PreventBuy = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Veuillez attendre la fin de l'enchère avant de pouvoir obtenir ce bien.", Color.ROSE);
    }
    public static void HandlePlayerAuctionSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Impossible de vendre dans ce type d'échoppe.", Color.ORANGE);
    }

    public static void OnOpenGenericStore(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseGenericStore;
      onOpen.Player.OnStoreRequestBuy += HandleGenericStoreBuy;
      onOpen.Player.OnStoreRequestSell += HandleGenericStoreSell;
    }
    public static void OnCloseGenericStore(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenGenericStore;
      onClose.Store.OnClose -= OnCloseGenericStore;
      onClose.Creature.OnStoreRequestBuy -= HandleGenericStoreBuy;
      onClose.Creature.OnStoreRequestSell -= HandleGenericStoreSell;
    }
    public static void HandleGenericStoreBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player player))
        return;

      int pocketGold = (int)player.oid.Gold;

      if (pocketGold < onStoreRequestBuy.Price)
      {
        if (pocketGold + player.bankGold < onStoreRequestBuy.Price)
        {
          player.oid.Gold = 0;
          int bankGold = 0;

          if (player.bankGold > 0)
            bankGold = player.bankGold;

          int debt = onStoreRequestBuy.Price - (pocketGold + bankGold);
          player.bankGold -= debt;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, $"Très bien, je demanderai à la banque de vous faire un crédit sur {debt}. N'oubliez pas que les intérêts sont de 30 % par semaine.",
          onStoreRequestBuy.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestBuy.Creature);
        }
        else
        {
          player.oid.Gold = 0;
          player.bankGold -= onStoreRequestBuy.Price - pocketGold;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Très bien, je demanderai à la banque de prélever l'or sur votre compte.",
          onStoreRequestBuy.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestBuy.Creature);
        }
      }
      else
        player.oid.TakeGold(onStoreRequestBuy.Price);

      onStoreRequestBuy.PreventBuy = true;
      onStoreRequestBuy.Item.Clone(player.oid);
    }
    public static void HandleGenericStoreSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;

      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, "Navré, je n'achète rien. J'arrive déjà tout juste à m'acquiter de ma dette.",
      onStoreRequestSell.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestSell.Creature);
    }
    public static void OnOpenBiblioStore(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseBiblioStore;
      onOpen.Player.OnStoreRequestBuy += HandleBiblioStoreBuy;
      onOpen.Player.OnStoreRequestSell += HandleGenericStoreSell;
    }
    public static void OnCloseBiblioStore(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenBiblioStore;
      onClose.Store.OnClose -= OnCloseBiblioStore;
      onClose.Creature.OnStoreRequestBuy -= HandleBiblioStoreBuy;
      onClose.Creature.OnStoreRequestSell -= HandleGenericStoreSell;
    }
    public static void HandleBiblioStoreBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player player))
        return;

      int pocketGold = (int)player.oid.Gold;

      if (pocketGold < onStoreRequestBuy.Price)
      {
        if (pocketGold + player.bankGold < onStoreRequestBuy.Price)
        {
          player.oid.Gold = 0;
          int bankGold = 0;

          if (player.bankGold > 0)
            bankGold = player.bankGold;

          int debt = onStoreRequestBuy.Price - (pocketGold + bankGold);
          player.bankGold -= debt;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, Languages.GetLangueStringConvertedHRPProtection("Lisez ... Apprennez ... Aidez-moi ... Aidez-nous ...", CustomFeats.Primordiale),
          onStoreRequestBuy.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestBuy.Creature);
        }
        else
        {
          player.oid.Gold = 0;
          player.bankGold -= onStoreRequestBuy.Price - pocketGold;

          ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, Languages.GetLangueStringConvertedHRPProtection("Lisez ... Apprennez ... Aidez-moi ... Aidez-nous ...", CustomFeats.Primordiale),
          onStoreRequestBuy.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestBuy.Creature);
        }
      }
      else
        player.oid.TakeGold(onStoreRequestBuy.Price);

      onStoreRequestBuy.PreventBuy = true;
      onStoreRequestBuy.Item.Clone(player.oid);
      onStoreRequestBuy.Item.Destroy();
    }
    public static void HandleBiblioStoreSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;

      ChatPlugin.SendMessage(ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK, Languages.GetLangueStringConvertedHRPProtection("Que pourrions-nous bien en faire ?", CustomFeats.Primordiale),
      onStoreRequestSell.Store.GetLocalVariable<NwObject>("_STORE_NPC").Value, onStoreRequestSell.Creature);
    }
    public static void OnOpenModifyArenaRewardStore(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseModifyArenaRewardStore;
      onOpen.Player.OnStoreRequestBuy += HandleModifyArenaRewardStoreBuy;
      //onOpen.Player.OnStoreRequestSell += HandleArenaRewardStoreSell;
    }
    public static void OnCloseModifyArenaRewardStore(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenModifyArenaRewardStore;
      onClose.Store.OnClose -= OnCloseModifyArenaRewardStore;
      onClose.Creature.OnStoreRequestBuy -= HandleModifyArenaRewardStoreBuy;
      //onClose.Creature.OnStoreRequestSell -= HandleArenaRewardStoreSell;

      if (PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO arenaRewardShop (id, shop) VALUES (@id, @shop)" +
              $"ON CONFLICT (id) DO UPDATE SET shop = @shop where id = @id");
        NWScript.SqlBindInt(query, "@id", 1);
        NWScript.SqlBindString(query, "@shop", onClose.Store.Serialize().ToBase64EncodedString());
        NWScript.SqlStep(query);
      }
    }
    public static void HandleModifyArenaRewardStoreBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player player))
        return;

      onStoreRequestBuy.PreventBuy = true;

      player.menu.Clear();
    }
  }
}
