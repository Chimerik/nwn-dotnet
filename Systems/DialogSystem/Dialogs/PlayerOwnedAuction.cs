using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static class PlayerOwnedAuction
  {
    public static void DrawMainPage(PlayerSystem.Player player, NwPlaceable storePanel)
    {
      player.menu.Clear();

      NwStore store = storePanel.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == $"_PLAYER_AUCTION_{player.oid.CDKey}");

      player.menu.titleLines = new List<string>() {
        $"Quelle modification souhaitez-vous apporter à votre échoppe {storePanel.Name.ColorString(Color.GREEN)} ?"
      };

      if (store.Items.Count() < 1)
      {
        player.menu.choices.Add((
          "Sélectionner l'objet à mettre aux enchères",
          () => GetObjectToAdd(player, store, storePanel)
        ));
      }

      if (store.GetLocalVariable<int>("_AUCTION_ID").HasValue)
      {
        player.menu.choices.Add((
          "Visualiser le contenu",
          () => OpenStore(player, store)
        ));
      }

      player.menu.choices.Add((
        "Modifier la description",
        () => GetNewDescription(player, storePanel)
      ));

      player.menu.choices.Add((
        "Faire tourner de 20°",
        () => HandleRotation(player, storePanel)
      ));

      player.menu.choices.Add((
        "Quitter",
        () => player.menu.Close()
      ));

      player.menu.Draw();
    }
    private static void GetObjectToAdd(PlayerSystem.Player player, NwStore store, NwPlaceable panel)
    {
      player.oid.SendServerMessage("Veuillez maintenant sélectionnner l'objet que vous souhaitez mettre en vente.", Color.ROSE);
      player.oid.GetLocalVariable<NwObject>("_ACTIVE_STORE").Value = store;
      player.oid.GetLocalVariable<NwObject>("_ACTIVE_PANEL").Value = panel;
      cursorTargetService.EnterTargetMode(player.oid, OnSellItemSelected, API.Constants.ObjectTypes.Item, API.Constants.MouseCursor.Pickup);
    }
    private static void OnSellItemSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (!Players.TryGetValue(selection.Player, out PlayerSystem.Player player))
        return;

      if (selection.TargetObject is null || !(selection.TargetObject is NwItem))
        return;

      NwStore store = (NwStore)player.oid.GetLocalVariable<NwObject>("_ACTIVE_STORE").Value;
      NwPlaceable panel = (NwPlaceable)player.oid.GetLocalVariable<NwObject>("_ACTIVE_PANEL").Value;
      player.oid.GetLocalVariable<NwObject>("_ACTIVE_STORE").Delete();
      player.oid.GetLocalVariable<NwObject>("_ACTIVE_PANEL").Delete();

      if (store == null || panel == null)
        return;

      DrawItemAddedPage(player, (NwItem)selection.TargetObject, store, panel);
    }
    private static void GetNewDescription(PlayerSystem.Player player, NwPlaceable shop)
    {
      player.menu.titleLines = new List<string>() {
        "Veuillez prononcer la nouvelle description à l'oral."
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");
        shop.Description = player.setString;
        player.oid.SendServerMessage($"La description de votre échoppe a été modifiée.", Color.ROSE);
        player.setString = "";
        DrawMainPage(player, shop);
      });

      player.menu.Draw();
    }
    public static void SaveAuction(PlayerSystem.Player player, NwStore shop)
    {
      if (shop.Area.Tag != "Promenadetest") // TODO : Plutôt que de ne pas enregistrer les shops or de la Promenade, rendre leurs inventaires accessibles à n'importe qui (ceux-ci n'étant pas protégés par Polpo)
        return;

      NwPlaceable panel = shop.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag == $"_PLAYER_AUCTION_PLC_{player.oid.CDKey}");
      panel.Name = "[ENCHERES] ".ColorString(Color.ORANGE) +
        shop.Items.FirstOrDefault().Name.ColorString(Color.OLIVE) + " " +
        shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value + " Fin : " + shop.GetLocalVariable<int>("_AUCTION_END_DATE").Value;
      
      if (shop.GetLocalVariable<int>("_AUCTION_ID").HasNothing)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerAuctions (characterId, shop, panel, expirationDate, highestAuction, highestAuctionner, areaTag, position, facing) VALUES (@characterId, @shop, @panel, @expirationDate, @highestAuction, @highestAuctionner, @areaTag, @position, @facing)");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@shop", shop.Serialize());
        NWScript.SqlBindString(query, "@panel", panel.Serialize());
        NWScript.SqlBindString(query, "@expirationDate", shop.GetLocalVariable<string>("_AUCTION_END_DATE").Value);
        NWScript.SqlBindInt(query, "@highestAuction", shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value);
        NWScript.SqlBindInt(query, "@highestAuctionner", 0);
        NWScript.SqlBindString(query, "@areaTag", panel.Area.Tag);
        NWScript.SqlBindVector(query, "@position", panel.Position);
        NWScript.SqlBindFloat(query, "@facing", panel.Rotation);
        NWScript.SqlStep(query);

        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
        NWScript.SqlStep(query);

        shop.GetLocalVariable<int>("_AUCTION_ID").Value = NWScript.SqlGetInt(query, 0);
        panel.GetLocalVariable<int>("_AUCTION_ID").Value = NWScript.SqlGetInt(query, 0);
      }
      else
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerShops set shop = @shop, panel = @panel, highestAuction = @highestAuction, highestAuctionner = @highestAuctionner where rowid = @shopId");
        NWScript.SqlBindInt(query, "@shopId", shop.GetLocalVariable<int>("_AUCTION_ID").Value);
        NWScript.SqlBindInt(query, "@highestAuction", shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value);
        NWScript.SqlBindInt(query, "@highestAuctionner", shop.GetLocalVariable<int>("_CURRENT_AUCTIONNER").Value);
        NWScript.SqlBindString(query, "@shop", shop.Serialize());
        NWScript.SqlBindString(query, "@panel", panel.Serialize());
        NWScript.SqlStep(query);
      }
    }

    public static void DrawItemAddedPage(PlayerSystem.Player player, NwItem item, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quelle doit être la mise à prix de base pour {item.Name} ?",
        "(prononcez simplement la mise à prix à haute voix)"
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        GetAuctionDuration(player, item, shop, panel);
        player.setValue = Config.invalidInput;
      });

      player.menu.Draw();
    }
    public static void GetAuctionDuration(PlayerSystem.Player player, NwItem item, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      int goldValue;

      if (player.setValue <= 0)
      {
        goldValue = item.GoldValue;
        player.oid.SendServerMessage($"La mise à prix saisie est invalide. {item.Name.ColorString(Color.ORANGE)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }
      else
      {
        goldValue = player.setValue;
        player.oid.SendServerMessage($"{item.Name.ColorString(Color.ORANGE)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }

      player.menu.titleLines = new List<string> {
        $"Quelle doit être la durée de cette enchère ?",
        "(prononcez simplement le nombre de jours, compris entre 1 et 30, à haute voix)"
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        ValidateAuction(player, item, shop, panel, goldValue);
        player.setValue = Config.invalidInput;
      });

      player.menu.Draw();
    }

    private static void ValidateAuction(PlayerSystem.Player player, NwItem item, NwStore shop, NwPlaceable panel, int goldValue)
    {
      player.menu.Clear();
      int auctionDuration;

      if (player.setValue <= 0 || player.setValue > 30)
      {
        auctionDuration = 1;
        player.oid.SendServerMessage($"La durée saisie est invalide. {item.Name.ColorString(Color.ORANGE)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or pour une durée de {auctionDuration} jour.");
      }
      else
      {
        auctionDuration = player.setValue;
        player.oid.SendServerMessage($"{item.Name.ColorString(Color.ORANGE)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or pour une durée de {auctionDuration} jour(s).");
      }

      NwItem copy = item.Clone(shop);
      ItemPlugin.SetBaseGoldPieceValue(copy, goldValue / item.StackSize);
      copy.GetLocalVariable<int>("_CURRENT_AUCTION").Value = goldValue / item.StackSize;
      item.Destroy();

      panel.GetLocalVariable<string>("_AUCTION_END_DATE").Value = DateTime.Now.AddDays(auctionDuration).ToString();

      SaveAuction(player, shop);
      player.menu.Close();
    }
    private static void OpenStore(PlayerSystem.Player player, NwStore shop)
    {
      shop.Open(player.oid);
    }
    private static void HandleRotation(PlayerSystem.Player player, NwPlaceable shop)
    {
      shop.Rotation += 20;
    }
    public static void GetAuctionPrice(PlayerSystem.Player player, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"L'enchère actuelle pour {shop.Items.FirstOrDefault().Name.ColorString(Color.ORANGE)} est de {shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value.ToString().ColorString(Color.GREEN)}.",
        "A hauteur de combien souhaitez-vous surenchérir ?",
        "(prononcez simplement la valeur de votre enchère à haute voix)"
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        SetAuctionPrice(player, shop, panel);
        player.setValue = Config.invalidInput;
      });

      player.menu.Draw();
    }
    private static void SetAuctionPrice(PlayerSystem.Player player, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      int auctionSetPrice;

      if(player.setValue > player.oid.Gold)
      {
        player.oid.SendServerMessage($"Vous n'avez pas {player.setValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or en poche !");
        player.menu.Close();
        return;
      }

      if (player.setValue <= shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value)
      {
        auctionSetPrice = shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value + 1;
        player.oid.SendServerMessage($"La valeur saisie est invalide.  Par défaut, vous avez donc surenchérit à hauteur de {auctionSetPrice.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }
      else
      {
        auctionSetPrice = player.setValue;
        player.oid.SendServerMessage($"Vous venez de surenchérir à hauteur de {auctionSetPrice.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }

      UpdateHighestAuctionner(shop, auctionSetPrice, player);

      SaveAuction(player, shop);
      player.menu.Close();
    }
    private static void UpdateHighestAuctionner(NwStore shop, int auctionSetPrice, PlayerSystem.Player player)
    {
      player.oid.TakeGold(auctionSetPrice, true);

      NwItem item = shop.Items.FirstOrDefault();
      ItemPlugin.SetBaseGoldPieceValue(item, auctionSetPrice / item.StackSize);

      var buyerQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET bankGold = bankGold + @gold where characterId = @characterId");
      NWScript.SqlBindInt(buyerQuery, "@characterId", shop.GetLocalVariable<int>("_CURRENT_AUCTIONNER").Value);
      NWScript.SqlBindInt(buyerQuery, "@gold", shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value);
      NWScript.SqlStep(buyerQuery);

      NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p, "characterId") == shop.GetLocalVariable<int>("_CURRENT_AUCTIONNER").Value);
      if (oSeller != null)
        oSeller.SendServerMessage($"Votre enchère sur {item.Name.ColorString(Color.ORANGE)} vient d'être battue. Le nouveau prix est de : {auctionSetPrice.ToString().ColorString(Color.ORANGE)}. La valeur de votre enchère a été versée à votre banque.");

      shop.GetLocalVariable<int>("_CURRENT_AUCTION").Value = auctionSetPrice;
      shop.GetLocalVariable<int>("_CURRENT_AUCTIONNER").Value = player.characterId;
    }
  }
}
