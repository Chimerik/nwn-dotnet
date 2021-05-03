﻿using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.System;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static class PlayerOwnedShop
  {
    public static void DrawMainPage(Player player, NwPlaceable storePanel)
    {
      player.menu.Clear();

      NwStore store = storePanel.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.Tag == $"_PLAYER_SHOP_{player.oid.CDKey}");

      player.menu.titleLines = new List<string>() {
        $"Quelle modification souhaitez-vous apporter à votre échoppe {storePanel.Name.ColorString(Color.GREEN)} ?"
      };

      player.menu.choices.Add((
          "Visualiser et modifier le contenu",
          () => OpenStore(player, store)
        ));

      player.menu.choices.Add((
        "Changer le nom",
        () => GetNewName(player, storePanel)
      ));
      player.menu.choices.Add((
        "Modifier la description",
        () => GetNewDescription(player, storePanel)
      ));
      player.menu.choices.Add((
        "Faire tourner de 20°",
        () => HandleRotation(player, storePanel)
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
    private static async void GetNewName(Player player, NwPlaceable shop)
    {
      player.menu.titleLines = new List<string>() {
        $"Nom actuel : {shop.Name.ColorString(Color.GREEN)}",
        "Veuillez prononcer le nouveau nom à l'oral."
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        shop.Name = player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"Votre objet est désormais nommé {shop.Name.ColorString(Color.GREEN)}.");
        DrawMainPage(player, shop);
      }
    }
    private static async void GetNewDescription(PlayerSystem.Player player, NwPlaceable shop)
    {
      player.menu.titleLines = new List<string>() {
        "Veuillez prononcer la nouvelle description à l'oral."
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        shop.Description = player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Value;
        player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"La description de votre échoppe a été modifiée.", Color.ROSE);
        DrawMainPage(player, shop);
      }
    }

    private static async void DestroyShop(Player player, NwStore shop, NwPlaceable panel)
    {
      foreach (NwItem item in shop.Items)
      {
        if(!player.oid.Inventory.CheckFit(item))
        {
          player.oid.SendServerMessage("Attention, tous les objets ne rentrent pas dans votre inventaire. Impossible de détruire votre échoppe pour le moment !", Color.ORANGE);
          return;
        }

        item.Clone(player.oid);
        item.Destroy();
      }

      await NwModule.Instance.WaitForObjectContext();
      NwItem authorization = NwItem.Create("shop_clearance", player.oid);

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
    private static void HandleRotation(Player player, NwPlaceable shop)
    {
      shop.Rotation += 20;
    }
    public static async void DrawItemAddedPage(Player player, NwItem item, NwStore shop)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"A quel prix souhaitez-vous vendre {item.Name} ?",
        "(prononcez simplement le prix à haute voix)"
      };

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        SetItemPrice(player, item, shop);
        player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }

      player.menu.Draw();
    }

    private static void SetItemPrice(Player player, NwItem item, NwStore shop)
    {
      player.menu.Clear();
      int goldValue;
      int input = int.Parse(player.oid.GetLocalVariable<string>("_PLAYER_INPUT"));

      if (input <= 0)
      {
        goldValue = item.GoldValue;
        player.oid.SendServerMessage($"Le prix saisi est invalide. {item.Name.ColorString(Color.ORANGE)} est désormais en vente au prix de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }
      else
      {
        goldValue = input;
        player.oid.SendServerMessage($"{item.Name.ColorString(Color.ORANGE)} est désormais en vente au prix de {goldValue.ToString().ColorString(Color.GREEN)} pièce(s) d'or.");
      }

      NwItem copy = item.Clone(shop);
      ItemPlugin.SetBaseGoldPieceValue(copy, goldValue / item.StackSize);
      copy.GetLocalVariable<int>("_SET_SELL_PRICE").Value = goldValue / item.StackSize;
      item.Destroy();

      player.menu.Close();
    }
    private static void OpenStore(Player player, NwStore shop)
    {
      shop.OnOpen -= StoreSystem.OnOpenOwnedPlayerShop;
      shop.OnOpen += StoreSystem.OnOpenOwnedPlayerShop;
      shop.Open(player.oid);
    }
  }
}
