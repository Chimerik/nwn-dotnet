using System;
using System.Numerics;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    private static int HandlePlayerDeath(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastPlayerDied(), out player))
      {
        NWScript.SendMessageToPC(player.oid, "Tout se brouille autour de vous. Avant de perdre connaissance, vous sentez comme un étrange maëlstrom vous aspirer.");
        
        CreatePlayerCorpse(player);
        StripPlayerGoldAfterDeath(player);
        StripPlayerOfCraftResources(player);
        SavePlayerCorpseToDatabase(player.characterId, player.deathCorpse, NWScript.GetTag(NWScript.GetArea(player.deathCorpse)), NWScript.GetPosition(player.deathCorpse));
        NWScript.DelayCommand(5.0f, () => SendPlayerToLimbo(player));
      }

      return 0;
    }
    private static void CreatePlayerCorpse(Player player)
    {
      var oPCCorpse = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "pccorpse", NWScript.GetLocation(player.oid));
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after", oPCCorpse);

      NWScript.SetName(oPCCorpse, $"Cadavre de {NWScript.GetName(player.oid)}");
      NWScript.SetDescription(oPCCorpse, $"Cadavre de {NWScript.GetName(player.oid)}");
      NWScript.SetLocalInt(oPCCorpse, "_PC_ID", player.characterId);
      NWScript.SetLocalInt(NWScript.CreateItemOnObject("item_pccorpse", oPCCorpse), "PC_ID", player.characterId);

      player.deathCorpse = oPCCorpse;
    }
    private static void StripPlayerGoldAfterDeath(Player player)
    {
      int remainingGold = NWScript.GetGold(player.oid);

      while (remainingGold > 0)
      {
        if (remainingGold >= 50000)
        {
          NWScript.CreateItemOnObject("nw_it_gold001", player.oid, 50000);
          remainingGold -= 50000;
        }
        else
        {
          NWScript.CreateItemOnObject("nw_it_gold001", player.oid, remainingGold);
          break;
        }
      }

      CreaturePlugin.SetGold(player.oid, 0);
    }
    private static void StripPlayerOfCraftResources(Player player)
    {
      var oItem = NWScript.GetFirstItemInInventory(player.oid);
      while(Convert.ToBoolean(oItem))
      {
        if(CollectSystem.IsItemCraftMaterial(NWScript.GetTag(oItem)) || NWScript.GetTag(oItem) == "blueprint")
          ObjectPlugin.AcquireItem(player.deathCorpse, oItem);
        oItem = NWScript.GetNextItemInInventory(player.oid);
      }
    }
    public static void SavePlayerCorpseToDatabase(int characterId, uint deathCorpse, string areaTag, Vector3 position)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO playerDeathCorpses (characterId, deathCorpse, areaTag, position) VALUES (@characterId, @deathCorpse, @areaTag, @position)");
      NWScript.SqlBindInt(query, "@characterId", characterId);
      NWScript.SqlBindObject(query, "@deathCorpse", deathCorpse);
      NWScript.SqlBindString(query, "@areaTag", areaTag);
      NWScript.SqlBindVector(query, "@position", position);
      NWScript.SqlStep(query);
    }
    private static void SendPlayerToLimbo(Player player)
    {
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_RESTORATION_GREATER), player.oid);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectResurrection(), player.oid);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectHeal(NWScript.GetMaxHitPoints(player.oid)), player.oid);

      NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP__RESPAWN_AREA"))));
    }
    private static void Respawn(Player player)
    {
      // TODO : Appliquer les bonus en fonction de l'entité choisie pour respawn (+augmentation du niveau d'influence de l'entité)
      // TODO : Diminuer la durabilité de tous les objets équipés et dans l'inventaire du PJ

      DestroyPlayerCorpse(player);
      NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_START_NEW_CHAR")))); // TODO : le respawn se fera plutôt à l'hospice des taudis
      NWScript.SendMessageToPC(player.oid, "Votre récente déconvenue vous a affligé d'une blessure durable. Il va falloir passer du temps en rééducation pour vous en débarrasser");
      // TODO : refaire le système de malus en utilisant le plugin feat de nwnx
      int iRandomMalus = Utils.random.Next(1130, 1130); // TODO : il faudra mettre en paramètre de conf le range des feat ID pour les malus

      if (CreaturePlugin.GetHighestLevelOfFeat(player.oid, iRandomMalus) != (int)Feat.Invalid)
      {
        int successorId;
        if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", iRandomMalus), out successorId))
        {
          CreaturePlugin.AddFeat(player.oid, successorId);
          iRandomMalus = successorId;
        }
      }
      else
        CreaturePlugin.AddFeat(player.oid, iRandomMalus);

      Func<Player, int, int> handler;
      if (SkillSystem.RegisterAddCustomFeatEffect.TryGetValue(iRandomMalus, out handler))
      {
        try
        {
          handler.Invoke(player, iRandomMalus);
        }
        catch (Exception e)
        {
          Utils.LogException(e);
        }
      }
    }
    private static void DestroyPlayerCorpse(Player player)
    {
      DeletePlayerCorpseFromDatabase(player.characterId);

      var oCorpse = NWScript.GetObjectByTag("pccorpse");
      int i = 1;
      while (Convert.ToBoolean(NWScript.GetIsObjectValid(oCorpse)))
      {
        if (player.characterId == NWScript.GetLocalInt(oCorpse, "_PC_ID"))
        {
          NWScript.DestroyObject(oCorpse);
          break;
        }
        oCorpse = NWScript.GetObjectByTag("pccorpse", i++);
      }

      var oCorpseItem = NWScript.GetObjectByTag("item_pccorpse");
      i = 1;
      while (NWScript.GetIsObjectValid(oCorpseItem) == 1)
      {
        if (player.characterId == NWScript.GetLocalInt(oCorpseItem, "_PC_ID"))
        {
          NWScript.DestroyObject(oCorpseItem);
          break;
        }
        oCorpseItem = NWScript.GetObjectByTag("item_pccorpse", i++);
      }
    }
    public static void DeletePlayerCorpseFromDatabase(int characterId)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"DELETE FROM playerDeathCorpses WHERE characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", characterId);
      NWScript.SqlStep(query);
    }
    private static int HandleAfterPCCorpseRemovedFromInventory(uint oidSelf)
    {
      var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));

      if (Convert.ToBoolean(NWScript.GetIsObjectValid(oItem)) && NWScript.GetTag(oItem) == "item_pccorpse"
        && !Convert.ToBoolean(NWScript.GetIsObjectValid(NWScript.GetItemPossessor(oItem))))
      {
        var oPCCorpse = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "pccorpse", NWScript.GetLocation(oidSelf));
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_pccorpse_remove_item_after", oPCCorpse);
        EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_inventory_pccorpse_removed_after", oidSelf);

        int characterId = NWScript.GetLocalInt(oItem, "_PC_ID");
        NWScript.SetLocalInt(oPCCorpse, "_PC_ID", characterId);
        ObjectPlugin.AcquireItem(oPCCorpse, oItem);

        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT characterName from playerCharacters where rowid = @characterId");
        NWScript.SqlBindInt(query, "@characterId", characterId);
        NWScript.SqlStep(query);

        string corpseName = NWScript.SqlGetString(query, 0);
        NWScript.SetName(oPCCorpse, $"Cadavre de {corpseName}");
        NWScript.SetDescription(oPCCorpse, $"Cadavre de {corpseName}");

        SavePlayerCorpseToDatabase(characterId, oPCCorpse, NWScript.GetTag(NWScript.GetArea(oPCCorpse)), NWScript.GetPosition(oPCCorpse));
      }
      return 0;
    }
    private static int HandleAfterItemRemovedFromPCCorpse(uint oidSelf)
    {
      var oItem = NWScript.StringToObject(EventsPlugin.GetEventData("ITEM"));

      if (NWScript.GetTag(oItem) == "item_pccorpse")
      {
        DeletePlayerCorpseFromDatabase(NWScript.GetLocalInt(oItem, "_PC_ID"));
        NWScript.DestroyObject(oidSelf);

        var oPossessor = NWScript.GetItemPossessor(oItem);
        if (Convert.ToBoolean(NWScript.GetIsObjectValid(oPossessor)))
          EventsPlugin.AddObjectToDispatchList("NWNX_ON_INVENTORY_REMOVE_ITEM_AFTER", "event_inventory_pccorpse_removed_after", oPossessor);
      }

      return 0;
    }
  }
}
