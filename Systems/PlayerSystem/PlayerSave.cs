using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWNX.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public void HandleBeforePlayerSave(ServerVaultEvents.OnServerCharacterSaveBefore onSaveBefore)
    {
      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
       * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
       * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/

      Log.Info($"Before saving {onSaveBefore.Player.Name}");

      if (onSaveBefore.Player.IsDM || onSaveBefore.Player.IsDMPossessed || onSaveBefore.Player.IsPlayerDM)
      {
        Log.Info("DM detected. Skipping save");
        onSaveBefore.Skip = true;
        return;
      }

      if (Players.TryGetValue(onSaveBefore.Player, out Player player))
      {
        if (onSaveBefore.Player.GetLocalVariable<int>("_DISCONNECTING").HasNothing)
        {
          if (onSaveBefore.Player.ActiveEffects.Any(e => e.EffectType == API.Constants.EffectType.Polymorph))
          {
            player.effectList = onSaveBefore.Player.ActiveEffects.ToList();
            Log.Info($"Polymorph detected, saving effect list");
          }
        }

        // TODO : probablement faire pour chaque joueur tous les check faim / soif / jobs etc ici

        // AFK detection
        if (player.location == player.oid?.Location)
        {
          player.isAFK = true;
          Log.Info("Player AFK");
        }
        else
          player.location = player.oid.Location;

        Log.Info("saved Location");

        player.currentHP = onSaveBefore.Player.HP;

        Log.Info("Saved HP");

        if (player.location.Area.GetLocalVariable<int>("_AREA_LEVEL").Value == 0)
          player.CraftJobProgression();

        Log.Info("Craft job progression done");

        player.AcquireSkillPoints();
        Log.Info("Acquire skill points done");

        player.dateLastSaved = DateTime.Now;

        SavePlayerCharacterToDatabase(player);
        Log.Info("Save player to DB done");
        SavePlayerLearnableSkillsToDatabase(player);
        Log.Info("Saved skills to DB");
        SavePlayerLearnableSpellsToDatabase(player);
        Log.Info("Saved Spells to DB");
        SavePlayerStoredMaterialsToDatabase(player);
        Log.Info("Saved materials to DB");
        SavePlayerMapPinsToDatabase(player);
        Log.Info("Saved map pin to DB");
        SavePlayerAreaExplorationStateToDatabase(player);
        Log.Info("Saved area exploration state to DB");
      }
    }
    public void HandleAfterPlayerSave(ServerVaultEvents.OnServerCharacterSaveAfter onSaveAfter)
    {
      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
       * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
       * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/

      Log.Info($"After saving {onSaveAfter.Player.Name}");

      if (Players.TryGetValue(onSaveAfter.Player, out Player player))
      {
        if (onSaveAfter.Player.GetLocalVariable<int>("_DISCONNECTING").HasNothing)
        {
          if (onSaveAfter.Player.ActiveEffects.Any(e => e.EffectType == API.Constants.EffectType.Polymorph))
          {
            Log.Info("Polymorph detected. Reapplying effect list");
            foreach (API.Effect eff in player.effectList)
              onSaveAfter.Player.ApplyEffect(eff.DurationType, eff, TimeSpan.FromSeconds((double)eff.DurationRemaining));
            Log.Info("Reapplied effect list");
          }
        }
      }
    }
    private static void SavePlayerCharacterToDatabase(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET areaTag = @areaTag, position = @position, facing = @facing, currentHP = @currentHP, bankGold = @bankGold, dateLastSaved = @dateLastSaved, currentSkillType = @currentSkillType, currentSkillJob = @currentSkillJob, currentCraftJob = @currentCraftJob, currentCraftObject = @currentCraftObject, currentCraftJobRemainingTime = @currentCraftJobRemainingTime, currentCraftJobMaterial = @currentCraftJobMaterial, menuOriginTop = @menuOriginTop, menuOriginLeft = @menuOriginLeft where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      if (player.location != null)
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

      NWScript.SqlBindInt(query, "@currentHP", player.currentHP);
      NWScript.SqlBindInt(query, "@bankGold", player.bankGold);
      NWScript.SqlBindString(query, "@dateLastSaved", player.dateLastSaved.ToString());
      NWScript.SqlBindInt(query, "@currentSkillType", (int)player.currentSkillType);
      NWScript.SqlBindInt(query, "@currentSkillJob", player.currentSkillJob);
      NWScript.SqlBindInt(query, "@currentCraftJob", player.craftJob.baseItemType);
      NWScript.SqlBindString(query, "@currentCraftObject", player.craftJob.craftedItem);
      NWScript.SqlBindFloat(query, "@currentCraftJobRemainingTime", player.craftJob.remainingTime);
      NWScript.SqlBindString(query, "@currentCraftJobMaterial", player.craftJob.material);
      NWScript.SqlBindInt(query, "@menuOriginTop", player.menu.originTop);
      NWScript.SqlBindInt(query, "@menuOriginLeft", player.menu.originLeft);
      NWScript.SqlStep(query);

      Log.Info($"{NWScript.GetName(player.oid)} saved location : {NWScript.GetTag(NWScript.GetAreaFromLocation(player.location))} - {NWScript.GetPositionFromLocation(player.location)} - {NWScript.GetFacingFromLocation(player.location)}");
    }
    private static void SavePlayerLearnableSkillsToDatabase(Player player)
    {
      foreach (KeyValuePair<int, SkillSystem.Skill> skillListEntry in player.learnableSkills)
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerLearnableSkills (characterId, skillId, skillPoints, trained) VALUES (@characterId, @skillId, @skillPoints, @trained)" +
        "ON CONFLICT (characterId, skillId) DO UPDATE SET skillPoints = @skillPoints, trained = @trained");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindInt(query, "@skillId", skillListEntry.Key);
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
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerLearnableSpells (characterId, skillId, skillPoints, trained) VALUES (@characterId, @skillId, @skillPoints, @trained)" +
        "ON CONFLICT (characterId, skillId) DO UPDATE SET skillPoints = @skillPoints, trained = @trained");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindInt(query, "@skillId", skillListEntry.Key);
        NWScript.SqlBindFloat(query, "@skillPoints", Convert.ToInt32(skillListEntry.Value.acquiredPoints));
        NWScript.SqlBindInt(query, "@trained", Convert.ToInt32(skillListEntry.Value.trained));
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
              $"ON CONFLICT (characterId, materialName) DO UPDATE SET materialStock = @materialStock where characterId = @characterId and materialName = @{material}");
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

        foreach (KeyValuePair<string, string> explorationStateListEntry in player.areaExplorationStateDictionnary)
        {
          var query = NWScript.SqlPrepareQueryCampaign(Config.database, queryString);
          NWScript.SqlBindInt(query, "@characterId", player.characterId);
          NWScript.SqlBindString(query, "@areaTag", explorationStateListEntry.Key);
          NWScript.SqlBindString(query, "@explorationState", explorationStateListEntry.Value);
          NWScript.SqlStep(query);
        }
      }
    }
  }
}
