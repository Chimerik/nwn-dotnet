using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWNX.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static class PlayerOwnedShop
  {
    public static void DrawMainPage(PlayerSystem.Player player, NwPlaceable storePanel)
    {
      player.menu.Clear();

      NwStore store = storePanel.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == $"_PLAYER_SHOP_{player.oid.CDKey}");

      player.menu.titleLines = new List<string>() {
        $"Quelle modification souhaitez-vous apporter à votre échoppe {storePanel.Name.ColorString(Color.GREEN)} ?"
      };

      int traderLevel = 1;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.Marchand))
        traderLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.Marchand, player.learntCustomFeats[CustomFeats.Marchand]);

      if (store.Items.Count() < traderLevel * 5)
      {
        player.menu.choices.Add((
          "Ajouter un objet",
          () => GetObjectToAdd(player, store)
        ));
      }

      if (store.GetLocalVariable<int>("_SHOP_ID").HasValue)
      {
        player.menu.choices.Add((
          "Visualiser et modifier le contenu",
          () => OpenStore(player, store)
        ));
      }

      player.menu.choices.Add((
        "Changer le nom",
        () => GetNewName(player, storePanel)
      ));
      player.menu.choices.Add((
        "Modifier la description",
        () => GetNewDescription(player, storePanel)
      ));
      player.menu.choices.Add((
        "Fermer boutique (destruction de l'échoppe)",
        () => DestroyShop(player, store, storePanel)
      ));
      player.menu.choices.Add((
        "Quitter",
        () => player.menu.Close()
      ));

      player.menu.Draw();
    }
    private static void GetObjectToAdd(PlayerSystem.Player player, NwStore store)
    {
      player.oid.SendServerMessage("Veuillez maintenant sélectionnner l'objet que vous souhaitez mettre en vente.", Color.ROSE);
      player.oid.GetLocalVariable<NwObject>("_ACTIVE_STORE").Value = store;
      cursorTargetService.EnterTargetMode(player.oid, OnSellItemSelected, API.Constants.ObjectTypes.Item, API.Constants.MouseCursor.Pickup);
    }
    private static void OnSellItemSelected(CursorTargetData selection)
    {
      if (!Players.TryGetValue(selection.Player, out PlayerSystem.Player player))
        return;

      if (selection.TargetObj is null || !(selection.TargetObj is NwItem))
        return;

      NwStore store = (NwStore)player.oid.GetLocalVariable<NwObject>("_ACTIVE_STORE").Value;
      player.oid.GetLocalVariable<NwObject>("_ACTIVE_STORE").Delete();

      if (store == null)
        return;

      DrawItemAddedPage(player, (NwItem)selection.TargetObj, store);
    }
    private static void GetNewName(PlayerSystem.Player player, NwPlaceable shop)
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
    private static void DestroyShop(PlayerSystem.Player player, NwStore shop, NwPlaceable panel)
    {
      foreach (NwItem item in shop.Items)
      {
        if(!player.oid.Inventory.CheckFit(item))
        {
          player.oid.SendServerMessage("Attention, tous les objets ne rentrent pas dans votre inventaire. Impossible de détruire votre échoppe pour le moment !", Color.ORANGE);
          return;
        }

        item.Clone(player.oid, "", true);
        item.Destroy();
      }

      NwItem authorization = NwItem.Create("shop_clearance", player.oid.Location);
      player.oid.AcquireItem(authorization);

      if (shop.GetLocalVariable<int>("_SHOP_ID").HasValue)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, "DELETE FROM playerShops where rowid = @shopID");
        NWScript.SqlBindInt(query, "@shopID", shop.GetLocalVariable<int>("_SHOP_ID").Value);
        NWScript.SqlStep(query);
      }

      player.oid.SendServerMessage($"Votre échoppe a été supprimée et votre autorisation et vos objets ont été restitués.", Color.ORANGE);

      player.menu.Close();
      shop.Destroy();
      panel.Destroy();
    }
    public static void SaveShop(PlayerSystem.Player player, NwStore shop)
    {
      if (shop.Area.Tag != "Promenadetest") // TODO : Plutôt que de ne pas enregistrer les shops or de la Promenade, rendre leurs inventaires accessibles à n'importe qui (ceux-ci n'étant pas protégés par Polpo)
        return;

      NwPlaceable panel = shop.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag == $"_PLAYER_SHOP_PLC_{player.oid.CDKey}");

      if (shop.GetLocalVariable<int>("_SHOP_ID").HasNothing)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerShops (characterId, shop, panel, expirationDate, areaTag, position, facing) VALUES (@characterId, @shop, @panel, @expirationDate, @areaTag, @position, @facing)");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@shop", shop.Serialize());
        NWScript.SqlBindString(query, "@panel", panel.Serialize());
        NWScript.SqlBindString(query, "@expirationDate", DateTime.Now.AddDays(30).ToString());
        NWScript.SqlBindString(query, "@areaTag", shop.Area.Tag);
        NWScript.SqlBindVector(query, "@position", shop.Position);
        NWScript.SqlBindFloat(query, "@facing", shop.Rotation);
        NWScript.SqlStep(query);

        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
        NWScript.SqlStep(query);

        shop.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 0);
        panel.GetLocalVariable<int>("_SHOP_ID").Value = NWScript.SqlGetInt(query, 0);
      }
      else
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerShops set shop = @shop, panel = @panel where rowid = @shopId");
        NWScript.SqlBindInt(query, "@shopId", shop.GetLocalVariable<int>("_SHOP_ID").Value);
        NWScript.SqlBindString(query, "@shop", shop.Serialize());
        NWScript.SqlBindString(query, "@panel", panel.Serialize());
        NWScript.SqlStep(query);
      }
    }

    public static void DrawItemAddedPage(PlayerSystem.Player player, NwItem item, NwStore shop)
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

    private static void SetItemPrice(PlayerSystem.Player player, NwItem item, NwStore shop)
    {
      player.menu.Clear();
      int goldValue;

      if (player.setValue <= 0)
      {
        goldValue = item.GoldValue;
        player.oid.SendServerMessage($"Le prix saisi est invalide. {item.Name.ColorString(Color.ORANGE)} est désormais en vente au prix de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }
      else
      {
        goldValue = player.setValue;
        player.oid.SendServerMessage($"{item.Name.ColorString(Color.ORANGE)} est désormais en vente au prix de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }

      NwItem copy = item.Clone(shop, "", true);
      ItemPlugin.SetBaseGoldPieceValue(copy, goldValue / item.StackSize);
      copy.GetLocalVariable<int>("_SET_SELL_PRICE").Value = goldValue / item.StackSize;
      item.Destroy();

      SaveShop(player, shop);
      player.menu.Close();
    }
    private static void OpenStore(PlayerSystem.Player player, NwStore shop)
    {
      shop.Open(player.oid);
    }
  }
}
