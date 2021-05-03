using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems;
using System;
using NWN.API;
using System.Linq;
using NWN.API.Events;
using System.Collections.Generic;

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
    public static void OnOpenOtherPlayerShop(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseOtherPlayerShop;
      onOpen.Player.OnStoreRequestBuy += HandleOtherPlayerShopBuy;
      onOpen.Player.OnStoreRequestSell += HandleOtherPlayerShopSell;
    }
    public static void OnCloseOtherPlayerShop(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenOtherPlayerShop;
      onClose.Store.OnClose -= OnCloseOtherPlayerShop;
      onClose.Creature.OnStoreRequestBuy -= HandleOtherPlayerShopBuy;
      onClose.Creature.OnStoreRequestSell -= HandleOtherPlayerShopSell;

      if (!PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
        return;

      if (onClose.Store.Area.Tag != "Promenadetest")
        return;

      NwPlaceable panel = onClose.Store.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag.StartsWith($"_PLAYER_SHOP_PLC_"));

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerShops set shop = @shop, panel = @panel where rowid = @shopId");
      NWScript.SqlBindInt(query, "@shopId", onClose.Store.GetLocalVariable<int>("_SHOP_ID").Value);
      NWScript.SqlBindString(query, "@shop", onClose.Store.Serialize().ToBase64EncodedString());
      NWScript.SqlBindString(query, "@panel", panel.Serialize().ToBase64EncodedString());
      NWScript.SqlStep(query);
    }
    public static void HandleOtherPlayerShopBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player buyer))
        return;

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
    public static void HandleOtherPlayerShopSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Impossible de vendre dans ce type d'échoppe.", Color.ORANGE);
    }
    public static void OnOpenOwnedPlayerShop(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseOwnedPlayerShop;
      onOpen.Player.OnStoreRequestBuy += HandleOwnedPlayerShopBuy;
      onOpen.Player.OnStoreRequestSell += HandleOwnedPlayerShopSell;
    }
    public static void OnCloseOwnedPlayerShop(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenOwnedPlayerShop;
      onClose.Store.OnClose -= OnCloseOwnedPlayerShop;
      onClose.Creature.OnStoreRequestBuy -= HandleOwnedPlayerShopBuy;
      onClose.Creature.OnStoreRequestSell -= HandleOwnedPlayerShopSell;

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
    public static void HandleOwnedPlayerShopBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player buyer))
        return;

      onStoreRequestBuy.PreventBuy = true;
      onStoreRequestBuy.Item.Clone(buyer.oid);
      onStoreRequestBuy.Item.Destroy();
    }
    public static async void HandleOwnedPlayerShopSell(OnStoreRequestSell onStoreRequestSell)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
        return;

      onStoreRequestSell.PreventSell = true;

      player.menu.Clear();

      int traderLevel = 1;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.Marchand))
        traderLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Marchand, player.learntCustomFeats[CustomFeats.Marchand]);

      if(onStoreRequestSell.Store.Items.Count() > traderLevel * 5)
      {
        player.oid.SendServerMessage($"Votre niveau de marchand actuel vous permet de mettre en vente {traderLevel * 5} objets dans cette boutique, or il s'en trouve déjà {onStoreRequestSell.Store.Items.Count()}", Color.ORANGE);
        player.menu.Close();
        return;
      }

      player.menu.titleLines = new List<string> {
        $"A quel prix souhaitez-vous mettre {onStoreRequestSell.Item.Name.ColorString(Color.LIME)} en vente ?",
        "(prononcez simplement la valeur à haute voix)"
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        SetItemPrice(player, onStoreRequestSell.Item, onStoreRequestSell.Store);
        player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private static void SetItemPrice(PlayerSystem.Player player, NwItem item, NwStore shop)
    {
      player.menu.Clear();
      int pointValue;

      int input = int.Parse(player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Value);

      if (input <= 0)
      {
        pointValue = item.GoldValue;
        player.oid.SendServerMessage($"Le prix ne peut être inférieur à 1. Veuillez saisir une valeur correcte.");
      }
      else
      {
        pointValue = input;
        player.oid.SendServerMessage($"{item.Name.ColorString(Color.ORANGE)} est désormais en vente au prix de {pointValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");

        NwItem copy = item.Clone(shop);
        ItemPlugin.SetBaseGoldPieceValue(copy, pointValue / item.StackSize);
        copy.GetLocalVariable<int>("_SET_SELL_PRICE").Value = pointValue / item.StackSize;
        item.Destroy();
      }

      player.menu.Close();
    }
    public static void OnOpenOtherPlayerAuction(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseOtherPlayerAuction;
      onOpen.Player.OnStoreRequestBuy += HandleOtherPlayerAuctionBuy;
      onOpen.Player.OnStoreRequestSell += HandleOtherPlayerAuctionSell;
    }
    public static void OnCloseOtherPlayerAuction(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenOtherPlayerAuction;
      onClose.Store.OnClose -= OnCloseOtherPlayerAuction;
      onClose.Creature.OnStoreRequestBuy -= HandleOtherPlayerAuctionBuy;
      onClose.Creature.OnStoreRequestSell -= HandleOtherPlayerAuctionSell;

      if (!PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
        return;

      if (onClose.Store.Area.Tag != "Promenadetest")
        return;

      NwPlaceable panel = onClose.Store.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag.StartsWith("_PLAYER_AUCTION_PLC_"));
      panel.Name = "[ENCHERES] ".ColorString(Color.ORANGE) +
        onClose.Store.Items.FirstOrDefault().Name.ColorString(Color.OLIVE) + " " +
        onClose.Store.GetLocalVariable<int>("_CURRENT_AUCTION").Value + " Fin : " + onClose.Store.GetLocalVariable<int>("_AUCTION_END_DATE").Value;

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerShops set shop = @shop, panel = @panel, highestAuction = @highestAuction, highestAuctionner = @highestAuctionner where rowid = @shopId");
      NWScript.SqlBindInt(query, "@shopId", onClose.Store.GetLocalVariable<int>("_AUCTION_ID").Value);
      NWScript.SqlBindInt(query, "@highestAuction", onClose.Store.GetLocalVariable<int>("_CURRENT_AUCTION").Value);
      NWScript.SqlBindInt(query, "@highestAuctionner", onClose.Store.GetLocalVariable<int>("_CURRENT_AUCTIONNER").Value);
      NWScript.SqlBindString(query, "@shop", onClose.Store.Serialize().ToBase64EncodedString());
      NWScript.SqlBindString(query, "@panel", panel.Serialize().ToBase64EncodedString());
      NWScript.SqlStep(query);
    }
    public static void HandleOtherPlayerAuctionBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      onStoreRequestBuy.PreventBuy = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Veuillez attendre la fin de l'enchère avant de pouvoir obtenir ce bien.", Color.ROSE);
    }
    public static void HandleOtherPlayerAuctionSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Impossible de vendre dans ce type d'échoppe.", Color.ORANGE);
    }
    public static void OnOpenOwnedPlayerAuction(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseOwnedPlayerAuction;
      onOpen.Player.OnStoreRequestBuy += HandleOwnedPlayerAuctionBuy;
      onOpen.Player.OnStoreRequestSell += HandleOwnedPlayerAuctionSell;
    }
    public static void OnCloseOwnedPlayerAuction(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenOwnedPlayerAuction;
      onClose.Store.OnClose -= OnCloseOwnedPlayerAuction;
      onClose.Creature.OnStoreRequestBuy -= HandleOwnedPlayerAuctionBuy;
      onClose.Creature.OnStoreRequestSell -= HandleOwnedPlayerAuctionSell;

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
    public static void HandleOwnedPlayerAuctionBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      onStoreRequestBuy.PreventBuy = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Veuillez attendre la fin de l'enchère avant de pouvoir obtenir ce bien.", Color.ROSE);
    }
    public static void HandleOwnedPlayerAuctionSell(OnStoreRequestSell onStoreRequestSell)
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
      onOpen.Player.OnStoreRequestSell += HandleModifyArenaRewardStoreSell;
    }
    public static void OnCloseModifyArenaRewardStore(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenModifyArenaRewardStore;
      onClose.Store.OnClose -= OnCloseModifyArenaRewardStore;
      onClose.Creature.OnStoreRequestBuy -= HandleModifyArenaRewardStoreBuy;
      onClose.Creature.OnStoreRequestSell -= HandleModifyArenaRewardStoreSell;

      if (PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO arenaRewardShop (id, shop) VALUES (@id, @shop)" +
              $"ON CONFLICT (id) DO UPDATE SET shop = @shop where id = @id");
        NWScript.SqlBindInt(query, "@id", 1);
        NWScript.SqlBindObject(query, "@shop", onClose.Store);
        NWScript.SqlStep(query);
      }
    }
    public static void HandleModifyArenaRewardStoreBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player buyer))
        return;

      onStoreRequestBuy.PreventBuy = true;
      onStoreRequestBuy.Item.Clone(buyer.oid);
      onStoreRequestBuy.Item.Destroy();
    }
    public static async void HandleModifyArenaRewardStoreSell(OnStoreRequestSell onStoreRequestSell)
    {
      if (!PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
        return;

      onStoreRequestSell.PreventSell = true;

      player.menu.Clear();

      player.menu.titleLines = new List<string> {
        $"A quel nombre de points {onStoreRequestSell.Item.Name.ColorString(Color.LIME)} doit-il pouvoir être acheté ?",
        "(prononcez simplement la valeur à haute voix)"
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        SetItemPointValue(player, onStoreRequestSell.Item, onStoreRequestSell.Store);
        player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }

    private static void SetItemPointValue(PlayerSystem.Player player, NwItem item, NwStore shop)
    {
      player.menu.Clear();
      int pointValue;

      int input = int.Parse(player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Value);

      if (input <= 0)
      {
        pointValue = item.GoldValue;
        player.oid.SendServerMessage($"Le valeur en point ne peut pas être inférieure à 1. Veuillez saisir une valeur correcte.");
      }
      else
      {
        pointValue = input;
        player.oid.SendServerMessage($"{item.Name.ColorString(Color.ORANGE)} peut désormais être échangé contre {pointValue.ToString().ColorString(Color.GREEN)} point(s).");
        
        NwItem copy = item.Clone(shop);
        ItemPlugin.SetBaseGoldPieceValue(copy, pointValue / item.StackSize);
        copy.GetLocalVariable<int>("_SET_SELL_PRICE").Value = pointValue / item.StackSize;
        item.Destroy();
      }

      player.menu.Close();
    }
    public static void OnOpenArenaRewardStore(StoreEvents.OnOpen onOpen)
    {
      onOpen.Store.OnClose += OnCloseArenaRewardStore;
      onOpen.Player.OnStoreRequestBuy += HandleArenaRewardStoreBuy;
      onOpen.Player.OnStoreRequestSell += HandleArenaRewardStoreSell;
    }
    public static void OnCloseArenaRewardStore(StoreEvents.OnClose onClose)
    {
      onClose.Store.OnOpen -= OnOpenArenaRewardStore;
      onClose.Store.OnClose -= OnCloseArenaRewardStore;
      onClose.Creature.OnStoreRequestBuy -= HandleArenaRewardStoreBuy;
      onClose.Creature.OnStoreRequestSell -= HandleArenaRewardStoreSell;

      if (!PlayerSystem.Players.TryGetValue(onClose.Creature, out PlayerSystem.Player seller))
        return;

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO arenaRewardShop (id, shop) VALUES (@id, @shop)" +
              $"ON CONFLICT (id) DO UPDATE SET shop = @shop where id = @id");
      NWScript.SqlBindInt(query, "@id", 1);
      NWScript.SqlBindObject(query, "@shop", onClose.Store);
      NWScript.SqlStep(query);
    }
    public static void HandleArenaRewardStoreBuy(OnStoreRequestBuy onStoreRequestBuy)
    {
      onStoreRequestBuy.PreventBuy = true;

      if (!PlayerSystem.Players.TryGetValue(onStoreRequestBuy.Creature, out PlayerSystem.Player buyer))
        return;

     if(buyer.pveArena.totalPoints < onStoreRequestBuy.Price)
      {
        buyer.oid.SendServerMessage($"{onStoreRequestBuy.Item.Name.ColorString(Color.WHITE)} coûte " +
          $"{onStoreRequestBuy.Price.ToString().ColorString(Color.WHITE)} point(s), or vous n'en possédez que {buyer.pveArena.totalPoints.ToString().ColorString(Color.WHITE)}", Color.ORANGE);
      }
     else
      {
        buyer.pveArena.totalPoints -= (uint)onStoreRequestBuy.Price;

        NwItem acquiredItem = onStoreRequestBuy.Item.Clone(buyer.oid);
        acquiredItem.GetLocalVariable<int>("_DURABILITY").Value = ItemUtils.GetBaseItemCost(acquiredItem) * 50;

        buyer.oid.SendServerMessage($"Vous venez d'acquérir {onStoreRequestBuy.Item.Name.ColorString(Color.WHITE)}" +
          $"pour {onStoreRequestBuy.Price.ToString().ColorString(Color.WHITE)} point(s). Points restants : {buyer.pveArena.totalPoints.ToString().ColorString(Color.WHITE)}", Color.ORANGE);
      }
    }
    public static void HandleArenaRewardStoreSell(OnStoreRequestSell onStoreRequestSell)
    {
      onStoreRequestSell.PreventSell = true;

      if (PlayerSystem.Players.TryGetValue(onStoreRequestSell.Creature, out PlayerSystem.Player player))
        player.oid.SendServerMessage("Impossible de vendre dans ce type d'échoppe.", Color.ORANGE);
    }
  }
}
