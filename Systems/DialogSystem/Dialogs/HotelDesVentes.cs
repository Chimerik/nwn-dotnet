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

      int negociateurLevel = 1;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.Negociateur))
        negociateurLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Negociateur, player.learntCustomFeats[CustomFeats.Negociateur]);

      int currentOrders = 0;

      var result = SqLiteUtils.SelectQuery("playerBuyOrders",
        new List<string>() { { "count(rowid)" } },
        new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } } });


      if (result != null && result.Count() > 0)
        currentOrders += result.FirstOrDefault().GetInt(0);

      if (currentOrders <= negociateurLevel * 3)
      {
        player.menu.choices.Add(("Créer un nouvel ordre de vente.", () => DrawSellOrderPage(player)));
        player.menu.choices.Add(("Créer un nouvel ordre d'achat.", () => DrawBuyOrderPage(player)));
      }

      player.menu.choices.Add(("Consulter les ordres de vente actifs.", () => SellOrderListMaterialSelection(player)));
      player.menu.choices.Add(("Consulter les ordres d'achat actifs.", () => BuyOrderListMaterialSelection(player)));
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
    private async void HandleValidateMaterialSelection(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
          $"Quelle quantité de {material} doit être comprise dans cet ordre de vente ?",
          "(Indiquez simplement la valeur à l'oral)"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        HandleSetupSellOrderPrice(player, material);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void HandleSetupSellOrderPrice(Player player, string material)
    {
      player.menu.Clear();

      int input = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input <= 0)
      {
        player.menu.titleLines.Add($"La quantité indiquée n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateMaterialSelection(player, material)));
      }
      else
      {
        if (input >= player.materialStock[material])
          input = player.materialStock[material];

        player.menu.titleLines = new List<string> {
          $"{input} de {material}. A quel prix unitaire ?",
          "Pour tout ordre non immédiat, il convient de s'acquiter à l'avance de 5 % du prix de vente",
          $"(Indiquez à l'oral le prix que vous souhaitez pour chaque unité de {material}"
          };

        bool awaitedValue = await player.WaitForPlayerInputInt();

        if (awaitedValue)
        {
          CreateSellOrderPage(player, material, input);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }
      }
    }
    private void CreateSellOrderPage(Player player, string material, int quantity)
    {
      int input = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input < 0)
      {
        player.menu.Clear();
        player.menu.titleLines.Add($"Le prix indiqué n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateMaterialSelection(player, material)));
        player.menu.Draw();
      }
      else
      {
        var result = SqLiteUtils.SelectQuery("playerBuyOrders",
        new List<string>() { { "rowid" }, { "quantity" }, { "unitPrice" } },
        new List<string[]>() { new string[] { "material", material }, new string[] { "unitPrice", input.ToString(), ">=" }, new string[] { "expirationDate", DateTime.Now.ToString(), "<" }, new string[] { "characterId", player.characterId.ToString(), "!=" } } 
        , " order by unitPrice DESC");

        int remainingQuantity = quantity;

        int comptabiliteLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.Comptabilite))
          comptabiliteLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Comptabilite, player.learntCustomFeats[CustomFeats.Comptabilite]);

        if(result != null)
        {
          foreach (var order in result)
          {
            int buyOrderQuantity = order.GetInt(1);
            int boughtQuantity = 0;

            if (remainingQuantity < buyOrderQuantity)
            {
              boughtQuantity = buyOrderQuantity - remainingQuantity;
              ordersDictionnary.Add(order.GetInt(0), boughtQuantity);
            }
            else
            {
              boughtQuantity = remainingQuantity - buyOrderQuantity;
              ordersDictionnary.Add(order.GetInt(0), -boughtQuantity);
            }

            remainingQuantity -= boughtQuantity;

            player.bankGold += boughtQuantity * order.GetInt(2) * (95 + comptabiliteLevel) / 100;
            player.oid.SendServerMessage($"Vous venez de vendre {boughtQuantity} unité(s) de {material} en vente directe. L'or a été versé directement à votre banque.", ColorConstants.Pink);

            if (remainingQuantity <= 0)
              break;
          }          
        }

        foreach (var entry in ordersDictionnary)
        {
          int transferedQuantity = 0;

          var buyOrderResult = SqLiteUtils.SelectQuery("playerBuyOrders",
            new List<string>() { { "characterId" } },
            new List<string[]>() { new string[] { "rowid", entry.Key.ToString() } });

          if (buyOrderResult != null && buyOrderResult.Count() > 0)
          {

            int buyerID = buyOrderResult.First().GetInt(0);

            if (entry.Value > 0)
            {
              SqLiteUtils.UpdateQuery("playerBuyOrders",
                new List<string[]>() { { new string[] { "quantity", entry.Value.ToString() } } },
                new List<string[]>() { { new string[] { "rowid", entry.Key.ToString() } } });

              transferedQuantity = entry.Value;
            }
            else
            {
              SqLiteUtils.DeletionQuery("playerBuyOrders",
              new Dictionary<string, string>() { { "rowid", entry.Key.ToString() } });

              transferedQuantity = -entry.Value;
            }

            NwPlayer oBuyer = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p.LoginCreature, "characterId") == buyerID);
            if (oBuyer != null && Players.TryGetValue(oBuyer.LoginCreature, out Player buyer))
            {
              if (buyer.materialStock.ContainsKey(material))
                buyer.materialStock[material] += entry.Value;
              else
                buyer.materialStock.Add(material, entry.Value);

              oBuyer.SendServerMessage($"Votre ordre d'achat {entry.Key} vous a permis d'acquérir {transferedQuantity} unité(s) de {material}", ColorConstants.Pink);
            }
            else
            {
              SqLiteUtils.InsertQuery("playerMaterialStorage",
                new List<string[]>() { new string[] { "characterId", buyerID.ToString() }, new string[] { "materialName", material }, new string[] { "materialStock", transferedQuantity.ToString() } },
                new List<string>() { "characterId", "materialName" },
                new List<string[]>() { new string[] { "materialStock", "+" } },
                new List<string>() { "characterId", "materialName" });

              Utils.SendMailToPC(buyerID, "Hotel des ventes de Similisse", $"Succès de votre d'achat {entry.Key}",
                $"Très honoré acheteur, \n\n Nous avons l'immense plaisir de vous annoncer que votre d'achat numéro {entry.Key} a porté ses fruits. \n\n Celui-ci vous a permis d'acquérir {transferedQuantity} unité(s) de {material} ! \n\n Signé : Polpo");
            }
          }
        }

        if(remainingQuantity <= 0)
        {
          player.oid.SendServerMessage($"Votre ordre de vente a été entièrement traité en transaction directe. Il n'est pas nécessaire de placer un ordre différée.", ColorConstants.Pink);
          player.menu.Close();
          return;
        }

        int brokerLevel = 0;
        if (player.learntCustomFeats.ContainsKey(CustomFeats.BrokerRelations))
          brokerLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BrokerRelations, player.learntCustomFeats[CustomFeats.BrokerRelations]);

        if (player.learntCustomFeats.ContainsKey(CustomFeats.BrokerAffinity))
          brokerLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BrokerAffinity, player.learntCustomFeats[CustomFeats.BrokerAffinity]);

        int brokerFee = remainingQuantity * input * 5 / 100;
        brokerFee -= brokerFee * 6 * brokerLevel / 100;
        if (brokerFee < 1)
          brokerFee = 1;

        if (player.bankGold < brokerFee)
        {
          player.oid.SendServerMessage($"Vous ne disposez pas de suffisament d'or en banque pour placer un ordre de vente différé.", ColorConstants.Lime);
          player.menu.Close();
          return;
        }

        player.bankGold -= brokerFee;
        player.oid.SendServerMessage($"{brokerFee} pièce(s) d'or ont été prélevées de votre banque pour la taxe de courtage.", ColorConstants.Pink);

        SqLiteUtils.InsertQuery("playerSellOrders",
                new List<string[]>() { new string[] { "characterId", player.characterId.ToString() }, new string[] { "expirationDate", DateTime.Now.AddDays(30).ToString() }, new string[] { "material", material }, new string[] { "quantity", remainingQuantity.ToString() }, new string[] { "unitPrice", input.ToString() } });

        player.oid.SendServerMessage($"Votre ordre de vente de {remainingQuantity} unité(s) de {material} a bien été enregistré.", ColorConstants.Pink);
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
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(MineralType), entry.Key))));

      foreach (var entry in woodDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(WoodType), entry.Key))));

      foreach (var entry in plankDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(PlankType), entry.Key))));

      foreach (var entry in peltDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(PeltType), entry.Key))));

      foreach (var entry in leatherDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleValidateBuyOrderMaterialSelection(player, Enum.GetName(typeof(LeatherType), entry.Key))));

      player.menu.choices.Add((
        "Retour",
        () => DrawWelcomePage(player)
      ));

      player.menu.Draw();
    }
    private async void HandleValidateBuyOrderMaterialSelection(Player player, string material)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
          $"Quelle quantité de {material} doit être comprise dans cet ordre d'achat ?",
          "(Indiquez simplement la valeur à l'oral)"
        };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        HandleSetupBuyOrderPrice(player, material);
        player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
    private async void HandleSetupBuyOrderPrice(Player player, string material)
    {
      player.menu.Clear();

      int input = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input <= 0)
      {
        player.menu.titleLines.Add($"La quantité indiquée n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateBuyOrderMaterialSelection(player, material)));

        player.menu.Draw();
      }
      else
      {
          player.menu.titleLines = new List<string> {
          $"{input} de {material}. A quel prix unitaire ?",
          "Pour tout ordre non immédiat, il convient de s'acquiter à l'avance de 5 % du prix de vente",
          $"(Indiquez à l'oral le prix que vous souhaitez pour chaque unité de {material}"
          };

        player.menu.Draw();

        bool awaitedValue = await player.WaitForPlayerInputInt();

        if (awaitedValue)
        {
          CreateBuyOrderPage(player, material, input);
          player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        }
      }
    }
    private void CreateBuyOrderPage(Player player, string material, int quantity)
    {
      int input = int.Parse(player.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input < 0)
      {
        player.menu.Clear();
        player.menu.titleLines.Add($"Le prix indiqué n'est pas valide, veuillez ré-essayer.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateBuyOrderMaterialSelection(player, material)));
        player.menu.Draw();
      }
      else
      {
        if (player.bankGold < quantity * input)
        {
          player.menu.Clear();
          player.menu.titleLines.Add($"Votre compte en banque n'est pas créditeur des {quantity * input + quantity * input * 0.05} pièce(s) d'or nécessaire au placement de cet ordre d'achat.");
          player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => HandleValidateBuyOrderMaterialSelection(player, material)));
          player.menu.Draw();
        }

        player.bankGold -= quantity * input;
        player.oid.SendServerMessage($"Afin de placer votre d'achat, {quantity * input} pièce(s) d'or ont été retenues du solde de votre compte.", ColorConstants.Magenta);

        var result = SqLiteUtils.SelectQuery("playerSellOrders",
        new List<string>() { { "rowid" }, { "quantity" }, { "unitPrice" } },
        new List<string[]>() { new string[] { "material", material }, new string[] { "unitPrice", input.ToString(), "<=" }, new string[] { "expirationDate", DateTime.Now.ToString(), "<" }, new string[] { "characterId", player.characterId.ToString(), "!=" } }
        , " order by unitPrice ASC");

        if (result != null)
        {
          int remainingQuantity = quantity;
          foreach (var order in result)
          {
            int sellOrderQuantity = order.GetInt(1);
            int soldQuantity = 0;

            if (remainingQuantity < sellOrderQuantity)
            {
              soldQuantity = sellOrderQuantity - remainingQuantity;
              ordersDictionnary.Add(order.GetInt(0), soldQuantity);
            }
            else
            {
              soldQuantity = remainingQuantity - sellOrderQuantity;
              ordersDictionnary.Add(order.GetInt(0), -soldQuantity);
            }

            remainingQuantity -= soldQuantity;

            if (player.materialStock.ContainsKey(material))
              player.materialStock[material] += soldQuantity;
            else
              player.materialStock.Add(material, soldQuantity);

            player.oid.SendServerMessage($"Vous venez d'acheter {soldQuantity} unité(s) de {material} en achat direct. Les matériaux sont en cours de transport vers votre entrepot.", ColorConstants.Pink);

            if (remainingQuantity <= 0)
              break;
          }

          foreach (var entry in ordersDictionnary)
          {
            int transferedQuantity = 0;

            var buyOrderResult = SqLiteUtils.SelectQuery("playerSellOrders",
              new List<string>() { { "characterId" }, { "unitPrice" } },
              new List<string[]>() { new string[] { "rowid", entry.Key.ToString() } });

            if (buyOrderResult != null && buyOrderResult.Count() > 0)
            {
              int sellerID = buyOrderResult.FirstOrDefault().GetInt(0);

              if (entry.Value > 0)
              {
                SqLiteUtils.UpdateQuery("playerSellOrders",
                  new List<string[]>() { { new string[] { "rowid", entry.Value.ToString() } } },
                  new List<string[]>() { { new string[] { "rowid", entry.Key.ToString() } } });

                transferedQuantity = entry.Value;
              }
              else
              {
                SqLiteUtils.DeletionQuery("playerSellOrders",
                  new Dictionary<string, string>() { { "rowid", entry.Key.ToString() } });

                transferedQuantity = -entry.Value;
              }

              NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p.LoginCreature, "characterId") == sellerID);

              int acquiredGold;

              if (oSeller != null && Players.TryGetValue(oSeller.LoginCreature, out Player seller))
              {
                int comptabiliteLevel = 0;
                if (seller.learntCustomFeats.ContainsKey(CustomFeats.Comptabilite))
                  comptabiliteLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Comptabilite, seller.learntCustomFeats[CustomFeats.Comptabilite]);

                acquiredGold = transferedQuantity * buyOrderResult.FirstOrDefault().GetInt(1) * (95 + comptabiliteLevel) / 100;

                seller.bankGold += acquiredGold;
                oSeller.SendServerMessage($"Votre ordre d'achat {entry.Key} vous a permis d'acquérir {transferedQuantity} unité(s) de {material}", ColorConstants.Pink);
              }
              else
              {
                // TODO : A la prochaine connexion du joueur, lui envoyer un courrier afin de lui indiquer que son ordre de vente a porté ses fruits
                acquiredGold = transferedQuantity * buyOrderResult.FirstOrDefault().GetInt(1) * 95 / 100;

                if (SqLiteUtils.UpdateQuery("playerCharacters",
                  new List<string[]>() { { new string[] { "bankGold", acquiredGold.ToString(), "+" } } },
                  new List<string[]>() { { new string[] { "rowid", sellerID.ToString() } } }))

                  Utils.SendMailToPC(sellerID, "Hotel des ventes de Similisse", $"Succès de votre ordre de vente {entry.Key}",
                    $"Très honoré vendeur, \n\n Nous avons l'immense plaisir de vous annoncer que votre de vente numéro {entry.Key} a porté ses fruits. \n\n Celui-ci vous a permis d'acquérir {acquiredGold} pièce(s) d'or ! \n\n Signé : Polpo");
              }
            }
          }

          if (remainingQuantity <= 0)
          {
            player.oid.SendServerMessage($"Votre ordre d'achat a été entièrement traité en transaction directe. Il n'est pas nécessaire de placer un ordre différée.", ColorConstants.Pink);
            player.menu.Close();
            return;
          }

          int brokerLevel = 0;
          if (player.learntCustomFeats.ContainsKey(CustomFeats.BrokerRelations))
            brokerLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BrokerRelations, player.learntCustomFeats[CustomFeats.BrokerRelations]);

          if (player.learntCustomFeats.ContainsKey(CustomFeats.BrokerAffinity))
            brokerLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.BrokerAffinity, player.learntCustomFeats[CustomFeats.BrokerAffinity]);

          int brokerFee = remainingQuantity * input * 5 / 100;
          brokerFee -= brokerFee * 6 * brokerLevel / 100;
          if (brokerFee < 1)
            brokerFee = 1;

          if (player.bankGold < brokerFee)
          {
            player.oid.SendServerMessage($"Vous ne disposez pas de suffisament d'or en banque pour placer un ordre de vente différé.", ColorConstants.Lime);
            player.bankGold += remainingQuantity * input;
            player.menu.Close();
            return;
          }

          player.bankGold -= brokerFee;
          player.oid.SendServerMessage($"{brokerFee} pièce(s) d'or ont été prélevées de votre banque pour la taxe de courtage.", ColorConstants.Pink);

          SqLiteUtils.InsertQuery("playerBuyOrders",
                new List<string[]>() { new string[] { "characterId", player.characterId.ToString() }, new string[] { "expirationDate", DateTime.Now.AddDays(30).ToString() }, new string[] { "material", material }, new string[] { "quantity", remainingQuantity.ToString() }, new string[] { "unitPrice", input.ToString() } });

          player.oid.SendServerMessage($"Votre ordre d'achat de {remainingQuantity} unité(s) de {material} a bien été enregistré.", ColorConstants.Pink);
          player.menu.Close();
        }
      }
    }
    private void DrawMySellOrderPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "Voici la liste de vos ordres de vente actifs. Souhaitez-vous en annuler un ?" };

      var result = SqLiteUtils.SelectQuery("playerSellOrders",
        new List<string>() { { "expirationDate" }, { "material" }, { "quantity" }, { "unitPrice" }, { "rowid" } },
        new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } } });
              
      if (result != null)
      {
        foreach(var order in result)
        {
          int contractId = order.GetInt(4);

          TimeSpan remainingTime = (DateTime.Parse(order.GetString(0)) - DateTime.Now);
          if (remainingTime.TotalMinutes > 0)
            player.menu.choices.Add(($"{contractId} - {order.GetString(1)} - {order.GetInt(1)} - {order.GetInt(3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}", () => CancelSellOrder(player, contractId)));
          else
          {
            Task contractExpiration = NwTask.Run(async () =>
            {
              await NwTask.Delay(TimeSpan.FromSeconds(0.2));
              DeleteExpiredSellOrder(player, contractId);
            });
          }
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
      player.oid.SendServerMessage($"L'ordre de vente {contractId} a été annulé.", ColorConstants.Magenta);
      DrawMySellOrderPage(player);
    }
    private void DeleteExpiredSellOrder(Player player, int contractId)
    {
      var result = SqLiteUtils.SelectQuery("playerSellOrders",
        new List<string>() { { "material" }, { "quantity" } },
        new List<string[]>() { { new string[] { "rowid", contractId.ToString() } } });

      if (result != null && result.Count() > 0)
      {
        string material = result.FirstOrDefault().GetString(0);
        int quantity = result.FirstOrDefault().GetInt(1);

        if (player.materialStock.ContainsKey(material))
          player.materialStock[material] += quantity;
        else
          player.materialStock.Add(material, quantity);

        if (SqLiteUtils.DeletionQuery("playerSellOrders",
          new Dictionary<string, string>() { { "rowid", contractId.ToString() } }))
          player.oid.SendServerMessage($"Expiration de l'ordre de vente {contractId}. {quantity} unité(s) de {material} sont en cours de transfert vers votre entrepôt.", ColorConstants.Magenta);
      }
    }
    private void DrawMyBuyOrderPage(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "Voici la liste de vos ordres d'achats actifs. Souhaitez-vous en annuler un ?" };

      var result = SqLiteUtils.SelectQuery("playerSellOrders",
        new List<string>() { { "expirationDate" }, { "material" }, { "quantity" }, { "unitPrice" }, { "rowid" } },
        new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      if(result != null)
      foreach (var order in result)
      {
        int contractId = order.GetInt(4);

        TimeSpan remainingTime = (DateTime.Parse(order.GetString(0)) - DateTime.Now);
        if (remainingTime.TotalMinutes > 0) 
          player.menu.choices.Add(($"{contractId} - {order.GetString(1)} - {order.GetInt(2)} - {order.GetInt(3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}", () => CancelBuyOrder(player, contractId)));
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
      player.oid.SendServerMessage($"L'ordre d'achat {contractId} a été annulé.", ColorConstants.Magenta);
      DrawMySellOrderPage(player);
    }
    private void DeleteExpiredBuyOrder(Player player, int contractId)
    {
      var result = SqLiteUtils.SelectQuery("playerBuyOrders",
        new List<string>() { { "unitPrice" }, { "quantity" } },
        new List<string[]>() { new string[] { "rowid", contractId.ToString() } });

      if (result != null & result.Count() > 0)
      {
        int gold = result.FirstOrDefault().GetInt(0) * result.FirstOrDefault().GetInt(1);
        player.bankGold += gold;

        if (SqLiteUtils.DeletionQuery("playerSellOrders",
          new Dictionary<string, string>() { { "rowid", contractId.ToString() } }))
          player.oid.SendServerMessage($"Expiration de l'ordre d'achat {contractId}. {gold} pièces d'or ont été transférées à votre banque.", ColorConstants.Magenta);
      }
    }
    private void SellOrderListMaterialSelection(Player player)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> { "De quel matériau souhaitez-vous consulter la liste des ordres de vente actifs ?" };

      foreach (var entry in oresDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(OreType), entry.Key))));

      foreach (var entry in mineralDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(MineralType), entry.Key))));

      foreach (var entry in woodDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(WoodType), entry.Key))));

      foreach (var entry in plankDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(PlankType), entry.Key))));

      foreach (var entry in peltDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(PeltType), entry.Key))));

      foreach (var entry in leatherDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialSellOrderList(player, Enum.GetName(typeof(LeatherType), entry.Key))));

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

      var result = SqLiteUtils.SelectQuery("playerSellOrders",
        new List<string>() { { "expirationDate" }, { "expirationDate" }, { "material" }, { "quantity" }, { "unitPrice" }, { "rowid" } },
        new List<string[]>(),
        " order by unitPrice ASC");

      if (result != null)
        foreach (var order in result)
        {
          int contractId = order.GetInt(4);

          TimeSpan remainingTime = (DateTime.Parse(order.GetString(0)) - DateTime.Now);
          if (remainingTime.TotalMinutes > 0)
            player.menu.titleLines.Add(($"{contractId} - {order.GetString(1)} - {order.GetInt(2)} - {order.GetInt(3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}"));
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
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(MineralType), entry.Key))));

      foreach (var entry in woodDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(WoodType), entry.Key))));

      foreach (var entry in plankDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(PlankType), entry.Key))));

      foreach (var entry in peltDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(PeltType), entry.Key))));

      foreach (var entry in leatherDictionnary)
        player.menu.choices.Add(($"* {entry.Value.name}", () => HandleSelectMaterialBuyOrderList(player, Enum.GetName(typeof(LeatherType), entry.Key))));

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

      var result = SqLiteUtils.SelectQuery("playerBuyOrders",
        new List<string>() { { "expirationDate" }, { "expirationDate" }, { "material" }, { "quantity" }, { "unitPrice" }, { "rowid" } },
        new List<string[]>(),
        " order by unitPrice DESC");

      if(result != null)
      foreach (var order in result)
      {
        int contractId = order.GetInt(4);

        TimeSpan remainingTime = (DateTime.Parse(order.GetString(0)) - DateTime.Now);
        if (remainingTime.TotalMinutes > 0)
          player.menu.titleLines.Add(($"{contractId} - {order.GetString(1)} - {order.GetInt(2)} - {order.GetInt(3)} po/u - Expire dans : {remainingTime.Days}:{remainingTime.Hours}:{remainingTime.Minutes}:{remainingTime.Seconds}"));
      }

      player.menu.choices.Add((
          "Retour",
          () => DrawWelcomePage(player)
        ));

      player.menu.Draw();
    }
  }
}
