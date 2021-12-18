using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.System;
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
        $"Quelle modification souhaitez-vous apporter à votre échoppe {storePanel.Name.ColorString(ColorConstants.Green)} ?"
      };

      if (store.Items.Count() < 1)
      {
        player.menu.choices.Add((
          "Sélectionner l'objet à mettre aux enchères",
          () => GetObjectToAdd(player, store, storePanel)
        ));
      }

      if (store.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").HasValue)
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
    private static void GetObjectToAdd(Player player, NwStore store, NwPlaceable panel)
    {
      player.oid.SendServerMessage("Veuillez maintenant sélectionnner l'objet que vous souhaitez mettre en vente.", ColorConstants.Rose);
      player.oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwStore>>("_ACTIVE_STORE").Value = store;
      player.oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwPlaceable>>("_ACTIVE_PANEL").Value = panel;
      player.oid.EnterTargetMode(OnSellItemSelected, ObjectTypes.Item, MouseCursor.Pickup);
    }
    private static void OnSellItemSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || !Players.TryGetValue(selection.Player.LoginCreature, out Player player) || selection.TargetObject is null || !(selection.TargetObject is NwItem))
        return;

      NwStore store = player.oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwStore>>("_ACTIVE_STORE").Value;
      NwPlaceable panel = player.oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwPlaceable>>("_ACTIVE_PANEL").Value;
      player.oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwStore>>("_ACTIVE_STORE").Delete();
      player.oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwPlaceable>>("_ACTIVE_PANEL").Delete();

      if (store == null || panel == null)
        return;

      DrawItemAddedPage(player, (NwItem)selection.TargetObject, store, panel);
    }
    private static async void GetNewDescription(Player player, NwPlaceable shop)
    {
      player.menu.titleLines = new List<string>() {
        "Veuillez prononcer la nouvelle description à l'oral."
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputString();

      if (awaitedValue)
      {
        shop.Description = player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Value;
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
        player.oid.SendServerMessage($"La description de votre échoppe a été modifiée.", ColorConstants.Rose);
        DrawMainPage(player, shop);
      }
    }
    public static void SaveAuction(Player player, NwStore shop)
    {
      if (shop.Area.Tag != "Promenadetest")
        return;

      NwPlaceable panel = shop.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag == $"_PLAYER_AUCTION_PLC_{player.oid.CDKey}");
      panel.Name = "[ENCHERES] ".ColorString(ColorConstants.Orange) +
        shop.Items.FirstOrDefault().Name.ColorString(ColorConstants.Red) + " " +
        shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value + " Fin : " + shop.GetObjectVariable<LocalVariableInt>("_AUCTION_END_DATE").Value;

      if (shop.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").HasNothing)
      {
        SqLiteUtils.InsertQuery("playerAuctions",
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() },
            new string[] { "shop", shop.Serialize().ToBase64EncodedString() },
            new string[] { "panel", panel.Serialize().ToBase64EncodedString() },
            new string[] { "expirationDate", shop.GetObjectVariable<LocalVariableString>("_AUCTION_END_DATE").Value.ToString() },
            new string[] { "highestAuction", shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value.ToString() },
            new string[] { "highestAuctionner", "0" },
            new string[] { "areaTag", panel.Area.Tag },
            new string[] { "position", panel.Position.ToString() },
            new string[] { "facing", panel.Rotation.ToString() } });

        var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
        query.Execute();

        shop.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value = query.Result.GetInt(0);
        panel.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value = query.Result.GetInt(0);
      }
      else
      {
        SqLiteUtils.UpdateQuery("playerShops",
          new List<string[]>() { new string[] { "shop", shop.Serialize().ToBase64EncodedString() }, new string[] { "panel", panel.Serialize().ToBase64EncodedString() }, new string[] { "highestAuction", shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value.ToString() }, new string[] { "highestAuctionner", shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTIONNER").Value.ToString() } },
          new List<string[]> { new string[] { "rowid", shop.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value.ToString() } });
      }
    }

    public static async void DrawItemAddedPage(Player player, NwItem item, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"Quelle doit être la mise à prix de base pour {item.Name} ?",
        "(prononcez simplement la mise à prix à haute voix)"
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        GetAuctionDuration(player, item, shop, panel);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    public static async void GetAuctionDuration(Player player, NwItem item, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      int goldValue;
      int input = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));

      if (input <= 0)
      {
        goldValue = item.GoldValue;
        player.oid.SendServerMessage($"La mise à prix saisie est invalide. {item.Name.ColorString(ColorConstants.Orange)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(ColorConstants.Green)} pièce(s) d'or.");
      }
      else
      {
        goldValue = input;
        player.oid.SendServerMessage($"{item.Name.ColorString(ColorConstants.Orange)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(ColorConstants.Green)} pièce(s) d'or.");
      }

      player.menu.titleLines = new List<string> {
        $"Quelle doit être la durée de cette enchère ?",
        "(prononcez simplement le nombre de jours, compris entre 1 et 30, à haute voix)"
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        ValidateAuction(player, item, shop, panel, goldValue);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }

    private static void ValidateAuction(Player player, NwItem item, NwStore shop, NwPlaceable panel, int goldValue)
    {
      player.menu.Clear();
      int auctionDuration;
      int input = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));

      if (input <= 0 || input > 30)
      {
        auctionDuration = 1;
        player.oid.SendServerMessage($"La durée saisie est invalide. {item.Name.ColorString(ColorConstants.Orange)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(ColorConstants.Green)} pièce(s) d'or pour une durée de {auctionDuration} jour.");
      }
      else
      {
        auctionDuration = input;
        player.oid.SendServerMessage($"{item.Name.ColorString(ColorConstants.Orange)} est désormais aux enchères avec une mise à prix de base de {goldValue.ToString().ColorString(ColorConstants.Green)} pièce(s) d'or pour une durée de {auctionDuration} jour(s).");
      }

      NwItem copy = item.Clone(shop);
      copy.BaseGoldValue = (uint)(goldValue / item.StackSize);
      copy.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value = goldValue / item.StackSize;
      item.Destroy();

      panel.GetObjectVariable<LocalVariableString>("_AUCTION_END_DATE").Value = DateTime.Now.AddDays(auctionDuration).ToString();

      SaveAuction(player, shop);
      player.menu.Close();
    }
    private static void OpenStore(PlayerSystem.Player player, NwStore shop)
    {
      shop.OnOpen -= StoreSystem.OnOpenOwnedPlayerAuction;
      shop.OnOpen += StoreSystem.OnOpenOwnedPlayerAuction;
      shop.Open(player.oid);
    }
    private static void HandleRotation(PlayerSystem.Player player, NwPlaceable shop)
    {
      shop.Rotation += 20;
    }
    public static async void GetAuctionPrice(Player player, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      player.menu.titleLines = new List<string> {
        $"L'enchère actuelle pour {shop.Items.FirstOrDefault().Name.ColorString(ColorConstants.Orange)} est de {shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value.ToString().ColorString(ColorConstants.Green)}.",
        "A hauteur de combien souhaitez-vous surenchérir ?",
        "(prononcez simplement la valeur de votre enchère à haute voix)"
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        SetAuctionPrice(player, shop, panel);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT").Delete();
      }
    }
    private static void SetAuctionPrice(Player player, NwStore shop, NwPlaceable panel)
    {
      player.menu.Clear();
      int auctionSetPrice;
      int input = int.Parse(player.oid.LoginCreature.GetObjectVariable<LocalVariableString>("_PLAYER_INPUT"));

      if (input > player.oid.LoginCreature.Gold)
      {
        player.oid.SendServerMessage($"Vous n'avez pas {input.ToString().ColorString(ColorConstants.Green)} pièce(s) d'or en poche !");
        player.menu.Close();
        return;
      }

      if (input <= shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value)
      {
        auctionSetPrice = shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value + 1;
        player.oid.SendServerMessage($"La valeur saisie est invalide.  Par défaut, vous avez donc surenchérit à hauteur de {auctionSetPrice.ToString().ColorString(ColorConstants.Green)} pièce(s) d'or.");
      }
      else
      {
        auctionSetPrice = input;
        player.oid.SendServerMessage($"Vous venez de surenchérir à hauteur de {auctionSetPrice.ToString().ColorString(ColorConstants.Green)} pièce(s) d'or.");
      }

      UpdateHighestAuctionner(shop, auctionSetPrice, player);

      SaveAuction(player, shop);
      player.menu.Close();
    }
    private static void UpdateHighestAuctionner(NwStore shop, int auctionSetPrice, PlayerSystem.Player player)
    {
      player.oid.LoginCreature.TakeGold(auctionSetPrice, true);

      NwItem item = shop.Items.FirstOrDefault();
      item.BaseGoldValue = (uint)(auctionSetPrice / item.StackSize);

      SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "bankGold", shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value.ToString(), "+" } },
          new List<string[]>() { new string[] { "rowid", shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTIONNER").Value.ToString() } });

      NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => p.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value == shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTIONNER").Value);
      if (oSeller != null)
        oSeller.SendServerMessage($"Votre enchère sur {item.Name.ColorString(ColorConstants.Orange)} vient d'être battue. Le nouveau prix est de : {auctionSetPrice.ToString().ColorString(ColorConstants.Orange)}. La valeur de votre enchère a été versée à votre banque.");

      shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value = auctionSetPrice;
      shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTIONNER").Value = player.characterId;
    }
  }
}
