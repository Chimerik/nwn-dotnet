using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public static int HandleBeforePlayerSave(uint oidSelf)
    {
      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
       * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
       * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/

      if (!Convert.ToBoolean(NWScript.GetIsDM(oidSelf)) && !Convert.ToBoolean(NWScript.GetIsDMPossessed(oidSelf)))
      {
        Player player;
        if (Players.TryGetValue(oidSelf, out player))
        {
          if (player.isConnected)
          {
            if (Utils.HasAnyEffect(player.oid, NWScript.EFFECT_TYPE_POLYMORPH))
            {
              Effect eff = NWScript.GetFirstEffect(player.oid);

              while (Convert.ToBoolean(NWScript.GetIsEffectValid(eff)))
              {
                if (NWScript.GetEffectType(eff) != NWScript.EFFECT_TYPE_POLYMORPH)
                  player.effectList.Add(eff);
                eff = NWScript.GetNextEffect(player.oid);
              }

              //EventsPlugin.SkipEvent();
              return 0;
            }
          }

          // TODO : probablement faire pour chaque joueur tous les check faim / soif / jobs etc ici

          // AFK detection
          if (player.location == NWScript.GetLocation(player.oid))
            player.isAFK = true;
          else
            player.location = NWScript.GetLocation(player.oid);

          player.currentHP = NWScript.GetCurrentHitPoints(player.oid);

          Area area;
          if (Module.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(player.oid)), out area) && area.level == 0)
            player.CraftJobProgression();

          player.AcquireSkillPoints();

          player.dateLastSaved = DateTime.Now;

          SavePlayerCharacterToDatabase(player);
          SavePlayerLearnableSkillsToDatabase(player);
          SavePlayerLearnableSpellsToDatabase(player);
          SavePlayerStoredMaterialsToDatabase(player);
          SavePlayerMapPinsToDatabase(player);
        }
      }
      return 0;
    }
    public static int HandleAfterPlayerSave(uint oidSelf)
    {
      /* Fix polymorph bug : Lorsqu'un PJ métamorphosé est sauvegardé, toutes ses buffs sont supprimées afin que les stats de 
       * la nouvelle forme ne remplace pas celles du PJ dans son fichier .bic. Après sauvegarde, les stats de la métamorphose 
       * sont réappliquées. 
       * Bug 1 : les PV temporaires de la forme se cumulent avec chaque sauvegarde, ce qui permet d'avoir PV infinis
       * BUG 2 : Les buffs ne faisant pas partie de la métamorphose (appliquées par sort par exemple), ne sont pas réappliquées
       * Ici, la correction consiste à ne pas sauvegarder le PJ s'il est métamorphosé, sauf s'il s'agit d'une déconnexion.
       * Mais il se peut que dans ce cas, ses buffs soient perdues à la reco. A vérifier. Si c'est le cas, une meilleure
       * correction pourrait être de parcourir tous ses buffs et de les réappliquer dans l'event AFTER de la sauvegarde*/

      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        if (player.isConnected)
        {
          if (Utils.HasAnyEffect(player.oid, NWScript.EFFECT_TYPE_POLYMORPH))
          {
            foreach (Effect eff in player.effectList)
            {
              float duration = EffectPlugin.UnpackEffect(eff).fDuration;
              if (duration > 0)
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eff, player.oid, duration);
              else
                NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, eff, player.oid);
            }
          }
        }
      }

      return 0;
    }
    private static void SavePlayerCharacterToDatabase(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"UPDATE playerCharacters SET areaTag = @areaTag, position = @position, facing = @facing, currentHP = @currentHP, bankGold = @bankGold, dateLastSaved = @dateLastSaved, currentSkillType = @currentSkillType, currentSkillJob = @currentSkillJob, currentCraftJob = @currentCraftJob, currentCraftObject = @currentCraftObject, currentCraftJobRemainingTime = @currentCraftJobRemainingTime, currentCraftJobMaterial = @currentCraftJobMaterial, menuOriginTop = @menuOriginTop, menuOriginLeft = @menuOriginLeft where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);
      NWScript.SqlBindString(query, "@areaTag", NWScript.GetTag(NWScript.GetArea(player.oid)));
      NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(player.oid));
      NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(player.oid));
      NWScript.SqlBindInt(query, "@currentHP", player.currentHP);
      NWScript.SqlBindInt(query, "@bankGold", player.bankGold);
      NWScript.SqlBindString(query, "@dateLastSaved", player.dateLastSaved.ToString());
      NWScript.SqlBindInt(query, "@currentSkillType", (int)player.currentSkillType);
      NWScript.SqlBindInt(query, "@currentSkillJob", player.currentSkillJob);
      NWScript.SqlBindInt(query, "@currentCraftJob", player.craftJob.baseItemType);
      NWScript.SqlBindString(query, "@currentCraftObject", player.craftJob.craftedItem);
      if(player.playerJournal.craftJobCountDown != null)
        NWScript.SqlBindFloat(query, "@currentCraftJobRemainingTime", (float)((TimeSpan)(player.playerJournal.craftJobCountDown - DateTime.Now)).TotalSeconds);
      else
        NWScript.SqlBindFloat(query, "@currentCraftJobRemainingTime", 0);
      NWScript.SqlBindString(query, "@currentCraftJobMaterial", player.craftJob.material);
      NWScript.SqlBindInt(query, "@menuOriginTop", player.menu.originTop);
      NWScript.SqlBindInt(query, "@menuOriginLeft", player.menu.originLeft);
      NWScript.SqlStep(query);
    }
    private static void SavePlayerLearnableSkillsToDatabase(Player player)
    {
      foreach(KeyValuePair<int, SkillSystem.Skill> skillListEntry in player.learnableSkills)
      {
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO playerLearnableSkills (characterId, skillId, skillPoints, trained) VALUES (@characterId, @skillId, @skillPoints, @trained)" +
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
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO playerLearnableSpells (characterId, skillId, skillPoints, trained) VALUES (@characterId, @skillId, @skillPoints, @trained)" +
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
        string queryString = "UPDATE playerMaterialStorage SET ";

        foreach (string material in player.materialStock.Keys)
          queryString += $"{material} = @{material}, ";

        queryString = queryString.Remove(queryString.Length - 2);
        queryString += " where characterId = @characterId";

        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, queryString);
        NWScript.SqlBindInt(query, "@characterId", player.characterId);

        foreach (string material in player.materialStock.Keys)
          NWScript.SqlBindInt(query, $"@{material}", player.materialStock[material]);

        NWScript.SqlStep(query);
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
          var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, queryString);
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
  }
}
