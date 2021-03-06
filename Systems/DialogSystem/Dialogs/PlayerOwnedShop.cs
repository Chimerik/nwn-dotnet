using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static class PlayerOwnedShop
  {
    public static void DrawMainPage(Player player, NwPlaceable shop)
    {
      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        $"Souhaitez-vous modifier l'échoppe {shop.Name.ColorString(Color.GREEN)} ?"
      };
      player.menu.choices.Add((
        "Changer le nom",
        () => GetNewName(player, shop)
      ));
      player.menu.choices.Add((
        "Modifier la description",
        () => GetNewDescription(player, shop)
      ));
      player.menu.choices.Add((
        "Fermer l'échoppe",
        () => DestroyShop(player, shop)
      ));
      player.menu.choices.Add((
        "Quitter",
        () => player.menu.Close()
      ));

      player.oid.GetLocalVariable<NwPlaceable>("_SETTING_SHOP").Value = shop;

      player.menu.Draw();
    }
    private static void GetNewName(Player player, NwPlaceable shop)
    {
      player.menu.titleLines = new List<string>() {
        $"Nom actuel : {shop.Name.ColorString(Color.GREEN)}",
        "Veuillez prononcer le nouveau nom à l'oral."
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");
        shop.Name = player.setString;
        player.oid.SendServerMessage($"Votre échoppe est désormais nommée {player.setString.ColorString(Color.GREEN)}.");
        player.setString = "";
        DrawMainPage(player, shop);
      });

      player.menu.Draw();
    }
    private static void GetNewDescription(Player player, NwPlaceable shop)
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
    private static void DestroyShop(Player player, NwPlaceable shop)
    {
      foreach (NwItem item in shop.Inventory.Items)
        player.oid.AcquireItem(item);

      NwItem authorization = NwItem.Create("shop_authorization", player.oid.Location);
      player.oid.AcquireItem(authorization);

      if (shop.GetLocalVariable<int>("_SHOP_ID").HasValue)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, "DELETE FROM playerShops where rowid = @shopID");
        NWScript.SqlBindInt(query, "@shopID", shop.GetLocalVariable<int>("_SHOP_ID").Value);
        NWScript.SqlStep(query);
      }

      player.oid.SendServerMessage($"Votre échoppe a été supprimée et votre autorisation et vos objets ont été restitués.", Color.ORANGE);
      player.oid.GetLocalVariable<int>("_SETTING_SHOP").Delete();
      player.menu.Close();
    }
    public static void SaveShop(Player player, NwPlaceable shop)
    {
      if (shop.Area.Tag != "Promenadetest") // TODO : Plutôt que de ne pas enregistrer les shops or de la Promenade, rendre leurs inventaires accessibles à n'importe qui (ceux-ci n'étant pas protégés par Polpo)
        return;

      if (shop.GetLocalVariable<int>("_SHOP_ID").HasNothing)
      {
        VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, shop, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerShops (characterId, shop, expirationDate, areaTag, position, facing) VALUES (@characterId, @shop, @expirationDate, @areaTag, @position, @facing)");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindObject(query, "@shop", shop);
        NWScript.SqlBindString(query, "@expirationDate", DateTime.Now.AddDays(30).ToString());
        NWScript.SqlBindString(query, "@areaTag", shop.Area.Tag);
        NWScript.SqlBindVector(query, "@position", shop.Position);
        NWScript.SqlBindFloat(query, "@facing", shop.Rotation);
        NWScript.SqlStep(query);

        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
        NWScript.SqlStep(query);

        shop.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 0);
      }
      else
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerShops set shop = @shop where rowid = @shopId");
        NWScript.SqlBindInt(query, "@shopId", shop.GetLocalVariable<int>("_SHOP_ID").Value);
        NWScript.SqlBindString(query, "@shop", ObjectPlugin.Serialize(shop));
        NWScript.SqlStep(query);
      }
    }

    public static void DrawItemAddedPage(Player player, NwItem item, NwPlaceable shop)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"A quel prix souhaitez-vous vendre {item.Name} ?",
        "(prononcez simplement le prix à haute voix)"
      };

      player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Delete();

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT").Value = 1;
        player.setValue = Config.invalidInput;
        await NwTask.WaitUntil(() => player.setValue != Config.invalidInput);
        SetItemPrice(player, item, shop);
        player.setValue = Config.invalidInput;


      });

      player.menu.Draw();
    }

    private static void SetItemPrice(Player player, NwItem item, NwPlaceable shop)
    {
      player.menu.Clear();
      uint availableGold = player.oid.Gold;

      if (player.setValue <= 0)
      {
        player.menu.titleLines.Add($"Le prix fourni est invalide.");
        player.menu.choices.Add(($"Entrer une nouvelle valeur.", () => DrawItemAddedPage(player, item, shop)));
        player.menu.Draw();
      }
      else
      {
        ItemPlugin.SetBaseGoldPieceValue(item, player.setValue);
        player.oid.SendServerMessage($"{item.Name} est désormais en vente au prix de {player.setValue} pièce(s) d'or.", Color.PINK);
        SaveShop(player, shop);
        player.menu.Close();
      }
    }
  }
}
