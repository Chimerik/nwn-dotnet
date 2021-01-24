using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class PlaceableSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
      { "ench_bsn_onclose", EnchantmentBasinSystem.HandleClose },
      { "ondeath_clean_dm_plc", HandleCleanDMPLC },
      { "plc_used", HandlePlaceableUsed },
      { "os_statuemaker", HandleStatufyCreature },
      { "oc_statue", HandleCancelStatueConversation },
      { "door_auto_close", HandleDoorAutoClose },
    };
    private static int HandleCleanDMPLC(uint oidSelf)
    {
      int plcID = NWScript.GetLocalInt(oidSelf, "_ID");
      if (plcID > 0)
      {
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "DELETE FROM dm_persistant_placeable where rowid = @plcID");
        NWScript.SqlBindInt(query, "@rowid", plcID);
        NWScript.SqlStep(query);
      }
      else
        Utils.LogMessageToDMs($"Persistent placeable {NWScript.GetName(oidSelf)} in area {NWScript.GetName(NWScript.GetArea(oidSelf))} does not have a valid ID !");

      return 0;
    }
    private static int HandlePlaceableUsed(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(NWScript.GetLastUsedBy(), out player))
      {
        int i;

        switch (NWScript.GetTag(oidSelf))
        {
          case "respawn_neutral":
            Respawn(player, "neutral");
            break;  
          case "respawn_radiant":
            Respawn(player, "radiant");
            break;
          case "respawn_dire":
            Respawn(player, "radiant");
            break;
          case "theater_rope":

            if (!Convert.ToBoolean(NWScript.GetLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN")))
            {
              for (i = 0; i < 4; i++)
                VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("theater_curtain", i), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

              NWScript.SetLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN", 1);
            }
            else
            {
              for (i = 0; i < 4; i++)
                VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("theater_curtain", i), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

              NWScript.DeleteLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN");
            }
            break;
            case "portal_start":
            NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
            NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_START_NEW_CHAR"))));
            break;
          case "portal_storage_in":
            uint aEntrepot = NWScript.CreateArea("entrepotperso", $"entrepotpersonnel_{NWScript.GetName(player.oid)}", $"Entrepot dimensionnel de {NWScript.GetName(player.oid)}");
            AreaSystem.CreateArea(aEntrepot);

            uint storage = NWScript.GetFirstObjectInArea(aEntrepot);
            if (NWScript.GetTag(storage) != "ps_entrepot")
              storage = NWScript.GetNearestObjectByTag("ps_entrepot", storage);

            var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT storage from playerCharacters where rowid = @characterId");
            NWScript.SqlBindInt(query, "@characterId", player.characterId);
            NWScript.SqlStep(query);

            NWScript.SqlGetObject(query, 0, NWScript.GetLocation(storage));
            NWScript.DestroyObject(storage);

            NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
            NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("wp_inentrepot"))));
            break;
          case "portal_storage_out":

            uint storageToSave = NWScript.GetFirstObjectInArea(NWScript.GetArea(player.oid));
            if (NWScript.GetTag(storageToSave) != "ps_entrepot")
              storageToSave = NWScript.GetNearestObjectByTag("ps_entrepot", storageToSave);

            var saveStorage = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
            NWScript.SqlBindInt(saveStorage, "@characterId", player.characterId);
            NWScript.SqlBindObject(saveStorage, "@storage", storageToSave);
            NWScript.SqlStep(saveStorage);

            if (AreaSystem.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(player.oid)), out Area area))
              AreaSystem.RemoveArea(area);

            NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions());
            NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetObjectByTag("wp_outentrepot"))));
            break;
        }
      }
      return 0;
    }
    private static int HandleCancelStatueConversation(uint oidSelf)
    {
      return 0;
    }
    private static int HandleStatufyCreature(uint oidSelf)
    {
      if (Convert.ToBoolean(NWScript.GetIsPC(NWScript.GetLastPerceived())))
      {
        if (NWScript.GetName(oidSelf) != "Statue draconique")
        {
          NWScript.PlayAnimation(Utils.random.Next(100, 116));
          NWScript.DelayCommand(1.0f, () => FreezeCreature(oidSelf));
        }
        else
          FreezeCreature(oidSelf);

        NWScript.SetEventScript(oidSelf, NWScript.EVENT_SCRIPT_CREATURE_ON_NOTICE, "");
        NWScript.SetAILevel(oidSelf, NWScript.AI_LEVEL_VERY_LOW);
      }
      
      return 0;
    }
    private static void FreezeCreature(uint creature)
    {
      //NWScript.SendMessageToPC(NWScript.GetFirstPC(), $"freezing : {NWScript.GetTag(creature)}");  
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.EffectVisualEffect(NWScript.VFX_DUR_FREEZE_ANIMATION), creature);
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.EffectVisualEffect(NWScript.VFX_DUR_ICESKIN), creature);
      NWScript.SetObjectHiliteColor(creature, 0xFFFFFF);
      NWScript.SetObjectMouseCursor(creature, NWScript.MOUSECURSOR_WALK);
      NWScript.SetPlotFlag(creature, 1);
    }
    private static int HandleDoorAutoClose(uint oidSelf)
    {
      NWScript.DelayCommand(5.0f, () => NWScript.PlayAnimation(NWScript.ANIMATION_DOOR_CLOSE));

      return 0;
    }
    private static int HandleCityGatesClick(uint oidSelf)
    {
      NWScript.PlayAnimation(NWScript.ANIMATION_DOOR_OPEN1);

      return 0;
    }
  }
}
