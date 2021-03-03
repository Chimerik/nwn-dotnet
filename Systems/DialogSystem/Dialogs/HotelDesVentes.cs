using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems
{
  class HotelDesVentes
  {
    private Dictionary<int, int> ordersDictionnary { get; set; }
    public HotelDesVentes(Player player)
    {
      ordersDictionnary = new Dictionary<int, int>();
      this.DrawWelcomePage(player);
    }
    private void DrawWelcomePage(Player player)
    {
      if (ordersDictionnary.Count > 0)
        ordersDictionnary.Clear();

      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        "Bienvenue sur l'hôtel des ventes de Similisse.",
        "Que souhaitez-vous faire ?"
      };

      player.menu.choices.Add(("Créer un nouvel ordre de vente.", () => DrawSellOrderPage(player)));
      player.menu.choices.Add(("Créer un nouvel ordre d'achat.", () => DrawBuyOrderPage(player)));
      player.menu.choices.Add(("Consulter les ordres de vente actifs.", () => DrawSellOrderList(player)));
      player.menu.choices.Add(("Consulter les ordres d'achat actifs.", () => DrawBuyOrderList(player)));
      player.menu.choices.Add(("Consulter mes ordres de vente.", () => DrawMySellOrderPage(player)));
      player.menu.choices.Add(("Consulter mes ordres d'achat.", () => DrawMyBuyOrderPage(player)));
      player.menu.choices.Add(("Quitter.", () => player.menu.Close()));
      player.menu.Draw();
    }
    private void DrawSellOrderPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {"Pour quelle ressources souhaitez-vous créer un ordre de vente ?"};

      foreach (var entry in player.materialStock)
          player.menu.choices.Add(($"* {entry.Key} : {entry.Value}", () => HandleValidateMaterialSelection(player, entry.Key)));

      player.menu.choices.Add((
        "Retour",
        () => DrawWelcomePage(player)
      ));

      player.menu.Draw();
    }
    private void HandleValidateMaterialSelection(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
          $"Quelle quantité de {material} doit être comprise dans cet ordre de vente ?",
          "(Indiquez simplement la valeur à l'oral)"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        HandleSetupSellOrderPrice(player, material);
        player.setValue = Config.invalidInput;
      });

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void HandleSetupSellOrderPrice(Player player, string material)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.titleLines.Add($"La quantité indiquée n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateMaterialSelection(player, material)));
      }
      else
      {
        if (player.setValue >= player.materialStock[material])
          player.setValue = player.materialStock[material];
        else
        {
          player.menu.titleLines = new List<string> {
          $"{player.setValue} de {material}. A quel prix unitaire ?",
          "Pour tout ordre non immédiat, il convient de s'acquiter à l'avance de 5 % du prix de vente",
          $"(Indiquez à l'oral le prix que vous souhaitez pour chaque unité de {material}"
          };

          Task playerInput = NwTask.Run(async () =>
          {
            int quantity = player.setValue;
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
            player.setValue = Config.invalidInput;
            await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
            CreateSellOrderPage(player, material, quantity);
            player.setValue = Config.invalidInput;
          });
        }
      }

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void CreateSellOrderPage(Player player, string material, int quantity)
    {
      if (player.setValue < 0)
      {
        player.menu.Clear();
        player.menu.titleLines.Add($"Le prix indiqué n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateMaterialSelection(player, material)));
        player.menu.Draw();
      }
      else
      {
        var buyOrdersQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT rowid, quantity, unitPrice from playerBuyOrders where material = @material AND unitPrice >= @unitPrice AND expirationDate < @now");
        NWScript.SqlBindString(buyOrdersQuery, "@expirationDate", DateTime.Now.ToString());
        NWScript.SqlBindString(buyOrdersQuery, "@material", material);
        NWScript.SqlBindInt(buyOrdersQuery, "@unitPrice", player.setValue);

        int remainingQuantity = quantity;

        while(NWScript.SqlStep(buyOrdersQuery) > 0 || remainingQuantity > 0)
        {
          int buyOrderQuantity = NWScript.SqlGetInt(buyOrdersQuery, 1);
          int boughtQuantity = 0;

          if (remainingQuantity < buyOrderQuantity)
          {
            boughtQuantity = buyOrderQuantity - remainingQuantity;
            ordersDictionnary.Add(NWScript.SqlGetInt(buyOrdersQuery, 0), boughtQuantity);
          }
          else
          {
            boughtQuantity = remainingQuantity - buyOrderQuantity;
            ordersDictionnary.Add(NWScript.SqlGetInt(buyOrdersQuery, 0), -boughtQuantity);
          }

          remainingQuantity -= boughtQuantity;

          player.bankGold += boughtQuantity * NWScript.SqlGetInt(buyOrdersQuery, 2);
          player.oid.SendServerMessage($"Vous venez de vendre {boughtQuantity} unité(s) de {material} en vente directe. L'or a été versé directement à votre banque.", Color.PINK);
        }

        foreach (var entry in ordersDictionnary)
        {
          int transferedQuantity = 0;

          var selectCharacterId = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterId from playerBuyOrders where rowid = @rowid");
          NWScript.SqlBindInt(selectCharacterId, "@rowid", entry.Key);
          NWScript.SqlStep(selectCharacterId);

          int buyerID = NWScript.SqlGetInt(selectCharacterId, 0);

          if (entry.Value > 0)
          {
            var updateQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerBuyOrders SET quantity = @quantity where rowid = @rowid");
            NWScript.SqlBindInt(updateQuery, "@quantity", entry.Value);
            NWScript.SqlBindInt(updateQuery, "@rowid", entry.Key);
            NWScript.SqlStep(updateQuery);
            transferedQuantity = entry.Value;
          }
          else
          {
            var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerBuyOrders where rowid = @rowid");
            NWScript.SqlBindInt(deletionQuery, "@rowid", entry.Key);
            NWScript.SqlStep(deletionQuery);
            transferedQuantity = -entry.Value;
          }

          NwPlayer oBuyer = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == buyerID);
          if(oBuyer != null && Players.TryGetValue(oBuyer, out Player buyer))
          {
            if (buyer.materialStock.ContainsKey(material))
              buyer.materialStock[material] += entry.Value;
            else
              buyer.materialStock.Add(material, entry.Value);

            oBuyer.SendServerMessage($"Votre ordre d'achat {entry.Key} vous a permis d'acquérir {transferedQuantity} unité(s) de {material}", Color.PINK);
          }
          else
          {
            // TODO : A la prochaine connexion du joueur, lui envoyer un courrier afin de lui indiquer que son ordre d'achat a porté ses fruits
            var buyerQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerMaterialStorage (characterId, materialName, materialStock) VALUES (@characterId, @materialName, @materialStock)" +
              $"ON CONFLICT (characterId, materialName) DO UPDATE SET materialStock = materialStock + @materialStock where characterId = @characterId and materialName = @materialName");
            NWScript.SqlBindInt(buyerQuery, "@characterId", buyerID);
            NWScript.SqlBindString(buyerQuery, "@materialName", material);
            NWScript.SqlBindInt(buyerQuery, "@materialStock", transferedQuantity);
            NWScript.SqlStep(buyerQuery);
          }
        }

        if(remainingQuantity <= 0)
        {
          player.oid.SendServerMessage($"Votre ordre de vente a été entièrement traité en transaction directe. Il n'est pas nécessaire de placer un ordre différée.", Color.PINK);
          player.menu.Close();
          return;
        }

        int brokerFee = remainingQuantity * player.setValue * 5 / 100;
        if (player.bankGold < brokerFee)
        {
          player.oid.SendServerMessage($"Vous ne disposez pas de suffisament d'or en banque pour placer un ordre de vente différé.", Color.LIME);
          player.menu.Close();
          return;
        }

        player.bankGold -= brokerFee;
        player.oid.SendServerMessage($"{brokerFee} pièce(s) d'or ont été prélevées de votre banque pour la taxe de courtage.", Color.PINK);

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerSellOrders (characterId, expirationDate, material, quantity, unitPrice) VALUES (@characterId, @expirationDate, @material, @quantity, @unitPrice)");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@expirationDate", DateTime.Now.AddDays(30).ToString());
        NWScript.SqlBindString(query, "@material", material);
        NWScript.SqlBindInt(query, "@quantity", remainingQuantity);
        NWScript.SqlBindInt(query, "@unitPrice", player.setValue);
        NWScript.SqlStep(query);


        player.oid.SendServerMessage($"Votre ordre de vente de {remainingQuantity} unité(s) de {material} a bien été enregistré.", Color.PINK);
        player.menu.Close();
      }
    }
    private void DrawBuyOrderPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "Pour quelle ressources souhaitez-vous créer un ordre d'achat ?" };

      foreach (var entry in oresDictionnary)
          player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in mineralDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in woodDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in plankDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in peltDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in leatherDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(OreType), entry.Key))));

      player.menu.choices.Add((
        "Retour",
        () => DrawWelcomePage(player)
      ));

      player.menu.Draw();
    }
    private void HandleValidateBuyOrderMaterialSelection(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
          $"Quelle quantité de {material} doit être comprise dans cet ordre de vente ?",
          "(Indiquez simplement la valeur à l'oral)"
        };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        HandleSetupBuyOrderPrice(player, material);
        player.setValue = Config.invalidInput;
      });

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void HandleSetupBuyOrderPrice(Player player, string material)
    {
      player.menu.Clear();

      if (player.setValue <= 0)
      {
        player.menu.titleLines.Add($"La quantité indiquée n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateBuyOrderMaterialSelection(player, material)));
      }
      else
      {
        if (player.setValue >= player.materialStock[material])
          player.setValue = player.materialStock[material];
        else
        {
          player.menu.titleLines = new List<string> {
          $"{player.setValue} de {material}. A quel prix unitaire ?",
          "Pour tout ordre non immédiat, il convient de s'acquiter à l'avance de 5 % du prix de vente",
          $"(Indiquez à l'oral le prix que vous souhaitez pour chaque unité de {material}"
          };

          Task playerInput = NwTask.Run(async () =>
          {
            int quantity = player.setValue;
            player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
            player.setValue = Config.invalidInput;
            await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
            CreateBuyOrderPage(player, material, quantity);
            player.setValue = Config.invalidInput;
          });
        }
      }

      player.setValue = Config.invalidInput;
      player.menu.Draw();
    }
    private void CreateBuyOrderPage(Player player, string material, int quantity)
    {
      if (player.setValue < 0)
      {
        player.menu.Clear();
        player.menu.titleLines.Add($"Le prix indiqué n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateBuyOrderMaterialSelection(player, material)));
        player.menu.Draw();
      }
      else
      {
        if(player.bankGold < quantity * player.setValue)
        {
          player.menu.Clear();
          player.menu.titleLines.Add($"Votre compte en banque n'est pas créditeur des {quantity * player.setValue + quantity * player.setValue * 0.05} pièce(s) d'or nécessaire au placement de cet ordre d'achat.");
          player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateBuyOrderMaterialSelection(player, material)));
          player.menu.Draw();
        }

        player.bankGold -= quantity * player.setValue;

        var sellOrdersQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT rowid, quantity, unitPrice from playerSellOrders where material = @material AND unitPrice <= @unitPrice AND expirationDate < @now");
        NWScript.SqlBindString(sellOrdersQuery, "@expirationDate", DateTime.Now.ToString());
        NWScript.SqlBindString(sellOrdersQuery, "@material", material);
        NWScript.SqlBindInt(sellOrdersQuery, "@unitPrice", player.setValue);

        int remainingQuantity = quantity;

        while (NWScript.SqlStep(sellOrdersQuery) > 0 || remainingQuantity > 0)
        {
          int sellOrderQuantity = NWScript.SqlGetInt(sellOrdersQuery, 1);
          int soldQuantity = 0;

          if (remainingQuantity < sellOrderQuantity)
          {
            soldQuantity = sellOrderQuantity - remainingQuantity;
            ordersDictionnary.Add(NWScript.SqlGetInt(sellOrdersQuery, 0), soldQuantity);
          }
          else
          {
            soldQuantity = remainingQuantity - sellOrderQuantity;
            ordersDictionnary.Add(NWScript.SqlGetInt(sellOrdersQuery, 0), -soldQuantity);
          }

          remainingQuantity -= soldQuantity;

          if (player.materialStock.ContainsKey(material))
            player.materialStock[material] += soldQuantity;
          else
            player.materialStock.Add(material, soldQuantity);

          player.oid.SendServerMessage($"Vous venez d'acheter {soldQuantity} unité(s) de {material} en achat direct. Les matériaux sont en cours de transport vers votre entrepot.", Color.PINK);
        }

        foreach (var entry in ordersDictionnary)
        {
          int transferedQuantity = 0;

          var selectCharacterId = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT characterId, unitPrice from playerSellOrders where rowid = @rowid");
          NWScript.SqlBindInt(selectCharacterId, "@rowid", entry.Key);
          NWScript.SqlStep(selectCharacterId);

          int sellerID = NWScript.SqlGetInt(selectCharacterId, 0);

          if (entry.Value > 0)
          {
            var updateQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerSellOrders SET quantity = @quantity where rowid = @rowid");
            NWScript.SqlBindInt(updateQuery, "@quantity", entry.Value);
            NWScript.SqlBindInt(updateQuery, "@rowid", entry.Key);
            NWScript.SqlStep(updateQuery);
            transferedQuantity = entry.Value;
          }
          else
          {
            var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerSellOrders where rowid = @rowid");
            NWScript.SqlBindInt(deletionQuery, "@rowid", entry.Key);
            NWScript.SqlStep(deletionQuery);
            transferedQuantity = -entry.Value;
          }

          NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == sellerID);
          int acquiredGold = transferedQuantity * NWScript.SqlGetInt(selectCharacterId, 1);

          if (oSeller != null && Players.TryGetValue(oSeller, out Player seller))
          {
            seller.bankGold += acquiredGold;
            oSeller.SendServerMessage($"Votre ordre d'achat {entry.Key} vous a permis d'acquérir {transferedQuantity} unité(s) de {material}", Color.PINK);
          }
          else
          {
            // TODO : A la prochaine connexion du joueur, lui envoyer un courrier afin de lui indiquer que son ordre de vente a porté ses fruits
            var buyerQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET bankGold = bankGold + @gold where characterId = @characterId");
            NWScript.SqlBindInt(buyerQuery, "@characterId", sellerID);
            NWScript.SqlBindInt(buyerQuery, "@gold", acquiredGold);
            NWScript.SqlStep(buyerQuery);
          }
        }

        if (remainingQuantity <= 0)
        {
          player.oid.SendServerMessage($"Votre ordre d'achat a été entièrement traité en transaction directe. Il n'est pas nécessaire de placer un ordre différée.", Color.PINK);
          player.menu.Close();
          return;
        }

        int brokerFee = remainingQuantity * player.setValue * 5 / 100;
        if (player.bankGold < brokerFee)
        {
          player.oid.SendServerMessage($"Vous ne disposez pas de suffisament d'or en banque pour placer un ordre de vente différé.", Color.LIME);
          player.bankGold += remainingQuantity * player.setValue;
          player.menu.Close();
          return;
        }

        player.bankGold -= brokerFee;
        player.oid.SendServerMessage($"{brokerFee} pièce(s) d'or ont été prélevées de votre banque pour la taxe de courtage.", Color.PINK);

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerBuyOrders (characterId, expirationDate, material, quantity, unitPrice) VALUES (@characterId, @expirationDate, @material, @quantity, @unitPrice)");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@expirationDate", DateTime.Now.AddDays(30).ToString());
        NWScript.SqlBindString(query, "@material", material);
        NWScript.SqlBindInt(query, "@quantity", remainingQuantity);
        NWScript.SqlBindInt(query, "@unitPrice", player.setValue);
        NWScript.SqlStep(query);


        player.oid.SendServerMessage($"Votre ordre d'achat de {remainingQuantity} unité(s) de {material} a bien été enregistré.", Color.PINK);
        player.menu.Close();
      }
    }
    private void DrawMySellOrderPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "Voici la liste de vos ordres de vente actifs. Souhaitez-vous en annuler un ?" };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, material, quantity, unitPrice, rowid from playerSellOrders where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 4);

        TimeSpan remainingTime = (DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now);
        if (remainingTime.TotalMinutes > 0)
          player.menu.choices.Add(($"{contractId} - {NWScript.SqlGetString(query, 1)} - {NWScript.SqlGetInt(query, 2)} - {NWScript.SqlGetInt(query, 3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}", () => CancelSellOrder(player, contractId)));
        else
        {
          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            DeleteExpiredSellOrder(player, contractId);
          });
        }
      }

      player.menu.choices.Add((
          "Retour",
          () => DrawWelcomePage(player)
        ));

      player.menu.Draw();
    }
    private void CancelSellOrder(Player player, int contractId)
    {
      DeleteExpiredSellOrder(player, contractId);
      player.oid.SendServerMessage($"L'ordre de vente {contractId} a été annulé.", Color.MAGENTA);
      DrawMySellOrderPage(player);
    }
    private void DeleteExpiredSellOrder(Player player, int contractId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT material, quantity from playerSellOrders where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      NWScript.SqlStep(query);

      string material = NWScript.SqlGetString(query, 0);
      int quantity = NWScript.SqlGetInt(query, 1);

      if (player.materialStock.ContainsKey(material))
        player.materialStock[material] += quantity;
      else
        player.materialStock.Add(material, quantity);

      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerSellOrders where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", contractId);
      NWScript.SqlStep(deletionQuery);

      player.oid.SendServerMessage($"Expiration de l'ordre de vente {contractId}. {quantity} unité(s) de {material} sont en cours de transfert vers votre entrepôt.", Color.MAGENTA);
    }
    private void DrawMyBuyOrderPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "Voici la liste de vos ordres d'achats actifs. Souhaitez-vous en annuler un ?" };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, material, quantity, unitPrice, rowid from playerBuyOrders where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 4);

        TimeSpan remainingTime = (DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now);
        if (remainingTime.TotalMinutes > 0)
          player.menu.choices.Add(($"{contractId} - {NWScript.SqlGetString(query, 1)} - {NWScript.SqlGetInt(query, 2)} - {NWScript.SqlGetInt(query, 3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}", () => CancelBuyOrder(player, contractId)));
        else
        {
          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            DeleteExpiredBuyOrder(player, contractId);
          });
        }
      }

      player.menu.choices.Add((
          "Retour",
          () => DrawWelcomePage(player)
        ));

      player.menu.Draw();
    }
    private void CancelBuyOrder(Player player, int contractId)
    {
      DeleteExpiredBuyOrder(player, contractId);
      player.oid.SendServerMessage($"L'ordre d'achat {contractId} a été annulé.", Color.MAGENTA);
      DrawMySellOrderPage(player);
    }
    private void DeleteExpiredBuyOrder(Player player, int contractId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT unitPrice, quantity from playerBuyOrders where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      NWScript.SqlStep(query);

      int gold = NWScript.SqlGetInt(query, 0) * NWScript.SqlGetInt(query, 1);
      player.bankGold += gold;

      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerSellOrders where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", contractId);
      NWScript.SqlStep(deletionQuery);

      player.oid.SendServerMessage($"Expiration de l'ordre d'achat {contractId}. {gold} pièces d'or ont été transférées à votre banque.", Color.MAGENTA);
    }
    private void SellOrderListMaterialSelection(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "De quel matériau souhaitez-vous consulter la liste des ordres de vente actifs ?" };

      foreach (var entry in oresDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in mineralDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in woodDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in plankDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in peltDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in leatherDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      player.menu.choices.Add((
          "Retour",
          () => DrawWelcomePage(player)
        ));

      player.menu.Draw();
    }
    private void HandleSelectMaterialSellOrderList(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "Voici la liste de tous les ordres de vente actifs." };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, material, quantity, unitPrice, rowid from playerSellOrders order by unitPrice ASC");

      while (NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 4);

        TimeSpan remainingTime = (DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now);
        if (remainingTime.TotalMinutes > 0)
          player.menu.titleLines.Add(($"{contractId} - {NWScript.SqlGetString(query, 1)} - {NWScript.SqlGetInt(query, 2)} - {NWScript.SqlGetInt(query, 3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}"));
      }

      player.menu.choices.Add((
          "Retour",
          () => DrawWelcomePage(player)
        ));

      player.menu.Draw();
    }
    private void BuyOrderListMaterialSelection(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "De quel matériau souhaitez-vous consulter la liste des ordres d'achat actifs ?" };

      foreach (var entry in oresDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in mineralDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in woodDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in plankDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in peltDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in leatherDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      player.menu.choices.Add((
          "Retour",
          () => DrawWelcomePage(player)
        ));

      player.menu.Draw();
    }
    private void HandleSelectMaterialBuyOrderList(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "Voici la liste de tous les ordres d'achat actifs." };

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, material, quantity, unitPrice, rowid from playerBuyOrders order by unitPrice DESC");

      while (NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 4);

        TimeSpan remainingTime = (DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now);
        if (remainingTime.TotalMinutes > 0)
          player.menu.titleLines.Add(($"{contractId} - {NWScript.SqlGetString(query, 1)} - {NWScript.SqlGetInt(query, 2)} - {NWScript.SqlGetInt(query, 3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}"));
      }

      player.menu.choices.Add((
          "Retour",
          () => DrawWelcomePage(player)
        ));

      player.menu.Draw();
    }
  }
}
