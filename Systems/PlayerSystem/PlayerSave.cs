using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void HandleBeforePlayerSave(OnServerCharacterSave onSaveBefore)
    {
      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à parcourir tous ses buffs et à les réappliquer dans l'event AFTER de la sauvegarde*/

      Log.Info($"Before saving {onSaveBefore.Player.LoginCreature.Name}");

      if (Players.TryGetValue(onSaveBefore.Player.LoginCreature, out Player player))
      {
        if (onSaveBefore.Player.LoginCreature.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph)
          && onSaveBefore.Player.LoginCreature.GetObjectVariable<LocalVariableInt>("_DISCONNECTING").HasNothing)
        {
          player.effectList = onSaveBefore.Player.LoginCreature.ActiveEffects.ToList();
          Log.Info($"Polymorph detected, saving effect list");

          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.1));
            RestoreEffectList(player);
          });
        }

        // TODO : probablement faire pour chaque joueur tous les check faim / soif / jobs etc ici

        // AFK detection
        if (player.location == player.oid.LoginCreature.Location)
        {
          player.isAFK = true;
          Log.Info("Player AFK");
        }
        else if(player.oid.LoginCreature.Location.Area != null)
          player.location = player.oid.LoginCreature.Location;

        if (player.location.Area?.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value == 0)
        {
          player.CraftJobProgression();
        }
        
        player.AcquireSkillPoints();

        player.dateLastSaved = DateTime.Now;

        SavePlayerCharacterToDatabase(player);
        SavePlayerLearnableSkillsToDatabase(player);
        SavePlayerLearnableSpellsToDatabase(player);
        SavePlayerStoredMaterialsToDatabase(player);
        SavePlayerMapPinsToDatabase(player);
        SavePlayerAreaExplorationStateToDatabase(player);
        SavePlayerChatColorsToDatabase(player);
        HandleExpiredContracts(player);
        HandleExpiredBuyOrders(player);
        HandleExpiredSellOrders(player);
        HandleNewMails(player);

        Log.Info("Finished saving player");
      }
    }
    public static void RestoreEffectList(Player player)
    {
      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
       * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
       * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/

      Log.Info($"Polymorph detected, restoring effect list on {player.oid.LoginCreature.Name}");

      foreach (Effect eff in player.effectList)
        player.oid.LoginCreature.ApplyEffect(eff.DurationType, eff, TimeSpan.FromSeconds((double)eff.DurationRemaining));
    }
    private static void SavePlayerCharacterToDatabase(Player player)
    {
      string areaTag;
      string position;
      string facing;

      if (player.location.Area != null)
      {
        areaTag = player.location.Area.Tag;
        position = player.location.Position.ToString();
        facing = player.location.Rotation.ToString();
      }
      else
      {
        areaTag = player.previousLocation.Area.Tag;
        position = player.previousLocation.Position.ToString();
        facing = player.previousLocation.Rotation.ToString();
      }

      SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "characterName", $"{player.oid.LoginCreature.OriginalFirstName} {player.oid.LoginCreature.OriginalLastName}" },
          new string[] { "areaTag", areaTag }, new string[] { "position", position }, new string[] { "facing", facing }, new string[] { "currentHP", player.oid.LoginCreature.HP.ToString() }, new string[] { "bankGold", player.bankGold.ToString() },
          new string[] { "dateLastSaved", player.dateLastSaved.ToString() }, new string[] { "currentSkillType", ((int)player.currentSkillType).ToString() }, new string[] { "currentCraftJob", player.currentSkillJob.ToString() },
          new string[] { "currentCraftJob", player.craftJob.baseItemType.ToString() }, new string[] { "currentCraftObject", player.craftJob.craftedItem }, new string[] { "currentCraftJobRemainingTime", player.craftJob.remainingTime.ToString() },
          new string[] { "currentCraftJobMaterial", player.craftJob.material }, new string[] { "currentCraftJobMaterial", player.craftJob.material }, new string[] { "pveArenaCurrentPoints", player.pveArena.currentPoints.ToString() },
          new string[] { "menuOriginTop", player.menu.originTop.ToString() }, new string[] { "menuOriginLeft", player.menu.originLeft.ToString() },
          new string[] { "alchemyCauldron", JsonConvert.SerializeObject(player.alchemyCauldron) } },
          new List<string[]>() { new string[]  { "rowid", player.characterId.ToString() } });
    }
    private static void SavePlayerLearnableSkillsToDatabase(Player player)
    {
      foreach (KeyValuePair<Feat, SkillSystem.Skill> skillListEntry in player.learnableSkills)
      {
        SqLiteUtils.InsertQuery("playerLearnableSkills",
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() },
            new string[] { "skillId", ((int)skillListEntry.Key).ToString() },
            new string[] { "skillPoints", skillListEntry.Value.acquiredPoints.ToString() },
            new string[] { "trained", skillListEntry.Value.trained.ToString() } },
            new List<string>() { "characterId", "skillId" },
            new List<string[]>() { new string[] { "skillPoints" }, new string[] { "trained" } });
      }

      // Ici on vire de la liste tout les skills trained et sauvegardés
      player.learnableSkills = player.learnableSkills.Where(kv => !kv.Value.trained).ToDictionary(kv => kv.Key, KeyValuePair => KeyValuePair.Value);
    }
    private static void SavePlayerLearnableSpellsToDatabase(Player player)
    {
      foreach (KeyValuePair<int, SkillSystem.LearnableSpell> skillListEntry in player.learnableSpells)
      {
        SqLiteUtils.InsertQuery("playerLearnableSpells",
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() },
            new string[] { "skillId", (skillListEntry.Key).ToString() },
            new string[] { "skillPoints", skillListEntry.Value.acquiredPoints.ToString() },
            new string[] { "trained", skillListEntry.Value.trained.ToString() },
            new string[] { "nbScrolls", skillListEntry.Value.nbScrollsUsed.ToString() } },
            new List<string>() { "characterId", "skillId" },
            new List<string[]>() { new string[] { "skillPoints" }, new string[] { "trained" }, new string[] { "nbScrolls" } });
      }

      // Ici on vire de la liste tout les skills trained et sauvegardés
      player.learnableSpells = player.learnableSpells.Where(kv => !kv.Value.trained).ToDictionary(kv => kv.Key, KeyValuePair => KeyValuePair.Value);
    }
    private static void SavePlayerStoredMaterialsToDatabase(Player player)
    {
      if (player.materialStock.Count > 0)
      {
        foreach (string material in player.materialStock.Keys)  
        {
          SqLiteUtils.InsertQuery("playerMaterialStorage",
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() },
            new string[] { "materialName", material },
            new string[] { "materialStock", player.materialStock[material].ToString() } },
            new List<string>() { "characterId", "materialName" },
            new List<string[]>() { new string[] { "materialStock" } },
            new List<string>() { { "characterId" }, { "materialName" } });
        }
      }
    }
    private static void SavePlayerMapPinsToDatabase(Player player)
    {
      if (player.mapPinDictionnary.Count > 0)
      {
        foreach (MapPin mapPin in player.mapPinDictionnary.Values)
        {
          SqLiteUtils.InsertQuery("playerMapPins",
          new List<string[]>() { 
            new string[] { "characterId", player.characterId.ToString() },
            new string[] { "mapPinId", mapPin.id.ToString()},
            new string[] { "areaTag", mapPin.areaTag },
            new string[] { "x", mapPin.x.ToString() },
            new string[] { "y", mapPin.y.ToString() },
            new string[] { "note", mapPin.note } },
          new List<string>() { "characterId", "mapPinId" },
          new List<string[]>() { new string[] { "x" }, new string[] { "y" }, new string[] { "note" } });
        }
      }
    }
    private static void SavePlayerAreaExplorationStateToDatabase(Player player)
    {
      if (player.areaExplorationStateDictionnary.Count > 0)
      {
        foreach (KeyValuePair<string, byte[]> explorationStateListEntry in player.areaExplorationStateDictionnary)
        {
          SqLiteUtils.InsertQuery("playerAreaExplorationState",
          new List<string[]>() {
            new string[] { "characterId", player.characterId.ToString() },
            new string[] { "areaTag", explorationStateListEntry.Key},
            new string[] { "explorationState", explorationStateListEntry.Value.ToBase64EncodedString() } },
          new List<string>() { "characterId", "areaTag" },
          new List<string[]>() { new string[] { "explorationState" } });
        }
      }
    }
    private static void SavePlayerChatColorsToDatabase(Player player)
    {
      if (player.chatColors.Count > 0)
      {
        foreach (KeyValuePair<ChatChannel, Color> chatColorEntry in player.chatColors)
        {
          SqLiteUtils.InsertQuery("chatColors",
          new List<string[]>() {
            new string[] { "accountId", player.accountId.ToString() },
            new string[] { "channel", ((int)chatColorEntry.Key).ToString()},
            new string[] { "color", chatColorEntry.Value.ToInt().ToString() } },
          new List<string>() { "accountId", "channel" },
          new List<string[]>() { new string[] { "color" } });
        }
      }
    }
    private static void HandleExpiredContracts(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
          new List<string>() { { "expirationDate" }, { "rowid" } },
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      foreach (var contract in result.Results)
      {
        int contractId = contract.GetInt(1);

        if ((DateTime.Parse(contract.GetString(0)) - DateTime.Now).TotalSeconds < 0)
        {
          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            DeleteExpiredContract(player, contractId);
          });
        }
      }
    }
    private static void DeleteExpiredContract(Player player, int contractId)
    {
      var result = SqLiteUtils.SelectQuery("playerPrivateContracts",
          new List<string>() { { "serializedContract" } },
          new List<string[]>() { new string[] { "ROWID", contractId.ToString() } });

      if (result.Result != null)
      {
        foreach (string materialString in result.Result.GetString(0).Split("|"))
        {
          string[] descriptionString = materialString.Split("$");
          if (descriptionString.Length == 3)
          {
            if (player.materialStock.ContainsKey(descriptionString[0]))
              player.materialStock[descriptionString[0]] += Int32.Parse(descriptionString[1]);
            else
              player.materialStock.Add(descriptionString[0], Int32.Parse(descriptionString[1]));

            player.oid.SendServerMessage($"Expiration du contrat {contractId} - {descriptionString[1]} unité(s) de {descriptionString[0]} ont été réintégrées à votre entrepôt.");
          }
        }

        SqLiteUtils.DeletionQuery("playerPrivateContracts",
          new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
      }
    }
    private static void HandleExpiredBuyOrders(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerBuyOrders",
          new List<string>() { { "expirationDate" }, { "rowid" } },
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      foreach (var contract in result.Results)
      {
        int contractId = contract.GetInt(1);

        if ((DateTime.Parse(contract.GetString(0)) - DateTime.Now).TotalSeconds < 0)
        {
          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            DeleteExpiredBuyOrder(player, contractId);
          });
        }
      }
    }
    private static void DeleteExpiredBuyOrder(Player player, int contractId)
    {
      var result = SqLiteUtils.SelectQuery("playerBuyOrders",
          new List<string>() { { "quantity" }, { "unitPrice" } },
          new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });

      if(result.Result != null)
      {
        int gold = result.Result.GetInt(0) * result.Result.GetInt(1);
        player.bankGold += gold;
        player.oid.SendServerMessage($"Expiration de l'ordre d'achat {contractId} - {gold} pièce(s) d'or ont été reversées à votre banque.");

        SqLiteUtils.DeletionQuery("playerBuyOrders",
            new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
      }
    }
    private static void HandleExpiredSellOrders(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerSellOrders",
          new List<string>() { { "expirationDate" }, { "rowid" } },
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() } });

      foreach (var sellOrder in result.Results)
      {
        int contractId = sellOrder.GetInt(1);

        if ((DateTime.Parse(sellOrder.GetString(0)) - DateTime.Now).TotalSeconds < 0)
        {
          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            DeleteExpiredSellOrder(player, contractId);
          });
        }
      }
    }
    private static void DeleteExpiredSellOrder(Player player, int contractId)
    {
      var result = SqLiteUtils.SelectQuery("playerSellOrders",
          new List<string>() { { "playerSellOrders" }, { "quantity" } },
          new List<string[]>() { new string[] { "rowid", contractId.ToString() } });

      if(result.Result != null)
      {
        string material = result.Result.GetString(0);
        int quantity = result.Result.GetInt(1);

        if (player.materialStock.ContainsKey(material))
          player.materialStock[material] += quantity;
        else
          player.materialStock.Add(material, quantity);

        player.oid.SendServerMessage($"Expiration de l'ordre de vente {contractId} - {quantity} unité(s) de {material} sont en cours de transfert vers votre entrepôt.");

        SqLiteUtils.DeletionQuery("playerSellOrders",
           new Dictionary<string, string>() { { "rowid", contractId.ToString() } });
      }
    }
    private static void HandleNewMails(Player player)
    {
      var result = SqLiteUtils.SelectQuery("messenger",
          new List<string>() { { "count (*)" } },
          new List<string[]>() { new string[] { "characterId", player.characterId.ToString() }, new string[] { "read","0" } });

      if(result.Result != null)
      {
        int nbMails = result.Result.GetInt(0);
        if(nbMails > 0)
          player.oid.SendServerMessage($"{nbMails.ToString().ColorString(ColorConstants.White)} lettres non lues se trouvent dans votre boîte aux lettres.", ColorConstants.Pink);
      }
    }
  }
}
