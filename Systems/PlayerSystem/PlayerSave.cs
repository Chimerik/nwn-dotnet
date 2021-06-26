using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

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
          && onSaveBefore.Player.LoginCreature.GetLocalVariable<int>("_DISCONNECTING").HasNothing)
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

        player.currentHP = onSaveBefore.Player.LoginCreature.HP;

        if (player.location.Area?.GetLocalVariable<int>("_AREA_LEVEL").Value == 0)
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

      foreach (API.Effect eff in player.effectList)
        player.oid.LoginCreature.ApplyEffect(eff.DurationType, eff, TimeSpan.FromSeconds((double)eff.DurationRemaining));
    }
    private static void SavePlayerCharacterToDatabase(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET characterName = @characterName, areaTag = @areaTag, position = @position, facing = @facing, currentHP = @currentHP, bankGold = @bankGold, dateLastSaved = @dateLastSaved, currentSkillType = @currentSkillType, currentSkillJob = @currentSkillJob, currentCraftJob = @currentCraftJob, currentCraftObject = @currentCraftObject, currentCraftJobRemainingTime = @currentCraftJobRemainingTime, currentCraftJobMaterial = @currentCraftJobMaterial, pveArenaCurrentPoints = @pveArenaCurrentPoints, menuOriginTop = @menuOriginTop, menuOriginLeft = @menuOriginLeft where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      if (player.location.Area != null)
      {
        NWScript.SqlBindString(query, "@areaTag", player.location.Area.Tag);
        NWScript.SqlBindVector(query, "@position", player.location.Position);
        NWScript.SqlBindFloat(query, "@facing", player.location.Rotation);
      }
      else
      {
        NWScript.SqlBindString(query, "@areaTag", player.previousLocation.Area.Tag);
        NWScript.SqlBindVector(query, "@position", player.previousLocation.Position);
        NWScript.SqlBindFloat(query, "@facing", player.previousLocation.Rotation);
      }

      NWScript.SqlBindString(query, "@characterName", $"{player.oid.LoginCreature.OriginalFirstName} {player.oid.LoginCreature.OriginalLastName}");
      NWScript.SqlBindInt(query, "@currentHP", player.currentHP);
      NWScript.SqlBindInt(query, "@bankGold", player.bankGold);
      NWScript.SqlBindString(query, "@dateLastSaved", player.dateLastSaved.ToString());
      NWScript.SqlBindInt(query, "@currentSkillType", (int)player.currentSkillType);
      NWScript.SqlBindInt(query, "@currentSkillJob", player.currentSkillJob);
      NWScript.SqlBindInt(query, "@currentCraftJob", player.craftJob.baseItemType);
      NWScript.SqlBindString(query, "@currentCraftObject", player.craftJob.craftedItem);
      NWScript.SqlBindFloat(query, "@currentCraftJobRemainingTime", player.craftJob.remainingTime);
      NWScript.SqlBindString(query, "@currentCraftJobMaterial", player.craftJob.material);
      NWScript.SqlBindInt(query, "@pveArenaCurrentPoints", (int)player.pveArena.currentPoints);
      NWScript.SqlBindInt(query, "@menuOriginTop", player.menu.originTop);
      NWScript.SqlBindInt(query, "@menuOriginLeft", player.menu.originLeft);
      NWScript.SqlStep(query);
    }
    private static void SavePlayerLearnableSkillsToDatabase(Player player)
    {
      foreach (KeyValuePair<Feat, SkillSystem.Skill> skillListEntry in player.learnableSkills)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerLearnableSkills (characterId, skillId, skillPoints, trained) VALUES (@characterId, @skillId, @skillPoints, @trained)" +
        "ON CONFLICT (characterId, skillId) DO UPDATE SET skillPoints = @skillPoints, trained = @trained");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindInt(query, "@skillId", (int)skillListEntry.Key);
        NWScript.SqlBindFloat(query, "@skillPoints", Convert.ToInt32(skillListEntry.Value.acquiredPoints));
        NWScript.SqlBindInt(query, "@trained", Convert.ToInt32(skillListEntry.Value.trained));
        NWScript.SqlStep(query);
      }

      // Ici on vire de la liste tout les skills trained et sauvegardés
      player.learnableSkills = player.learnableSkills.Where(kv => !kv.Value.trained).ToDictionary(kv => kv.Key, KeyValuePair => KeyValuePair.Value);
    }
    private static void SavePlayerLearnableSpellsToDatabase(Player player)
    {
      foreach (KeyValuePair<int, SkillSystem.LearnableSpell> skillListEntry in player.learnableSpells)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerLearnableSpells (characterId, skillId, skillPoints, trained, nbScrolls) VALUES (@characterId, @skillId, @skillPoints, @trained, @nbScrolls)" +
        "ON CONFLICT (characterId, skillId) DO UPDATE SET skillPoints = @skillPoints, trained = @trained, nbScrolls = @nbScrolls");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindInt(query, "@skillId", skillListEntry.Key);
        NWScript.SqlBindFloat(query, "@skillPoints", Convert.ToInt32(skillListEntry.Value.acquiredPoints));
        NWScript.SqlBindInt(query, "@trained", Convert.ToInt32(skillListEntry.Value.trained));
        NWScript.SqlBindInt(query, "@nbScrolls", skillListEntry.Value.nbScrollsUsed);
        NWScript.SqlStep(query);
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
          var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerMaterialStorage (characterId, materialName, materialStock) VALUES (@characterId, @materialName, @materialStock)" +
              $"ON CONFLICT (characterId, materialName) DO UPDATE SET materialStock = @materialStock where characterId = @characterId and materialName = @materialName");
          NWScript.SqlBindInt(query, "@characterId", player.characterId);
          NWScript.SqlBindString(query, "@materialName", material);
          NWScript.SqlBindInt(query, "@materialStock", player.materialStock[material]);
          NWScript.SqlStep(query);
        }
      }
    }
    private static void SavePlayerMapPinsToDatabase(Player player)
    {
      if (player.mapPinDictionnary.Count > 0)
      {
        string queryString = "INSERT INTO playerMapPins (characterId, mapPinId, areaTag, x, y, note) VALUES (@characterId, @mapPinId, @areaTag, @x, @y, @note)" +
          "ON CONFLICT (characterId, mapPinId) DO UPDATE SET x = @x, y = @y, note = @note";

        foreach (MapPin mapPin in player.mapPinDictionnary.Values)
        {
          var query = NWScript.SqlPrepareQueryCampaign(Config.database, queryString);
          NWScript.SqlBindInt(query, "@characterId", player.characterId);
          NWScript.SqlBindInt(query, "@mapPinId", mapPin.id);
          NWScript.SqlBindString(query, "@areaTag", mapPin.areaTag);
          NWScript.SqlBindFloat(query, "@x", mapPin.x);
          NWScript.SqlBindFloat(query, "@y", mapPin.y);
          NWScript.SqlBindString(query, "@note", mapPin.note);
          NWScript.SqlStep(query);
        }
      }
    }
    private static void SavePlayerAreaExplorationStateToDatabase(Player player)
    {
      if (player.areaExplorationStateDictionnary.Count > 0)
      {
        string queryString = "INSERT INTO playerAreaExplorationState (characterId, areaTag, explorationState) VALUES (@characterId, @areaTag, @explorationState)" +
          "ON CONFLICT (characterId, areaTag) DO UPDATE SET explorationState = @explorationState";

        foreach (KeyValuePair<string, byte[]> explorationStateListEntry in player.areaExplorationStateDictionnary)
        {
          var query = NWScript.SqlPrepareQueryCampaign(Config.database, queryString);
          NWScript.SqlBindInt(query, "@characterId", player.characterId);
          NWScript.SqlBindString(query, "@areaTag", explorationStateListEntry.Key);
          NWScript.SqlBindString(query, "@explorationState", explorationStateListEntry.Value.ToBase64EncodedString());
          NWScript.SqlStep(query);
        }
      }
    }
    private static void SavePlayerChatColorsToDatabase(Player player)
    {
      if (player.chatColors.Count > 0)
      {
        string queryString = "INSERT INTO chatColors (accountId, channel, color) VALUES (@accountId, @channel, @color)" +
          "ON CONFLICT (accountId, channel) DO UPDATE SET color = @color";

        foreach (KeyValuePair<ChatChannel, Color> chatColorEntry in player.chatColors)
        {
          var query = NWScript.SqlPrepareQueryCampaign(Config.database, queryString);
          NWScript.SqlBindInt(query, "@accountId", player.accountId);
          NWScript.SqlBindInt(query, "@channel", (int)chatColorEntry.Key);
          NWScript.SqlBindInt(query, "@color", chatColorEntry.Value.ToInt());
          NWScript.SqlStep(query);
        }
      }
    }
    private static void HandleExpiredContracts(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, rowid from playerPrivateContracts where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 1);

        if ((DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now).TotalSeconds < 0)
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
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedContract from playerPrivateContracts where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      if (NWScript.SqlStep(query) > 0)
      {
        foreach (string materialString in NWScript.SqlGetString(query, 0).Split("|"))
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

        var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerPrivateContracts where rowid = @rowid");
        NWScript.SqlBindInt(deletionQuery, "@rowid", contractId);
        NWScript.SqlStep(deletionQuery);
      }
    }
    private static void HandleExpiredBuyOrders(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, rowid from playerBuyOrders where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 1);

        if ((DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now).TotalSeconds < 0)
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
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT quantity, unitPrice from playerBuyOrders where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      NWScript.SqlStep(query);

      int gold = NWScript.SqlGetInt(query, 0) + NWScript.SqlGetInt(query, 1);
      player.bankGold += gold;
      player.oid.SendServerMessage($"Expiration de l'ordre d'achat {contractId} - {gold} pièce(s) d'or ont été reversées à votre banque.");

      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerBuyOrders where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", contractId);
      NWScript.SqlStep(deletionQuery);
    }
    private static void HandleExpiredSellOrders(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT expirationDate, rowid from playerSellOrders where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (NWScript.SqlStep(query) > 0)
      {
        int contractId = NWScript.SqlGetInt(query, 1);

        if ((DateTime.Parse(NWScript.SqlGetString(query, 0)) - DateTime.Now).TotalSeconds < 0)
        {
          Task contractExpiration = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.2));
            DeleteExpiredBuyOrder(player, contractId);
          });
        }
      }
    }
    private static void DeleteExpiredSellOrder(Player player, int contractId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT playerSellOrders, quantity from playerSellOrders where rowid = @rowid");
      NWScript.SqlBindInt(query, "@rowid", contractId);
      NWScript.SqlStep(query);

      string material = NWScript.SqlGetString(query, 0);
      int quantity = NWScript.SqlGetInt(query, 1);

      if (player.materialStock.ContainsKey(material))
        player.materialStock[material] += quantity;
      else
        player.materialStock.Add(material, quantity);

      player.oid.SendServerMessage($"Expiration de l'ordre de vente {contractId} - {quantity} unité(s) de {material} sont en cours de transfert vers votre entrepôt.");

      var deletionQuery = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerSellOrders where rowid = @rowid");
      NWScript.SqlBindInt(deletionQuery, "@rowid", contractId);
      NWScript.SqlStep(deletionQuery);
    }
    private static void HandleNewMails(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT count (*) from messenger where characterId = @characterId and read = 0");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      if(NWScript.SqlStep(query) != 0 && NWScript.SqlGetInt(query, 0) > 0)
      {
        player.oid.SendServerMessage($"{NWScript.SqlGetString(query, 0).ColorString(ColorConstants.White)} lettres non lues se trouvent dans votre boîte aux lettres.", ColorConstants.Pink);
      }
    }
  }
}
