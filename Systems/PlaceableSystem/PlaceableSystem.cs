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
      { "event_refinery_add_item_before", HandleBeforeItemAddedToRefinery },
      { "refinery_add_item", HandleItemAddedToRefinery },
      { "refinery_close", HandleRefineryClose },
      { "ondeath_clean_dm_plc", HandleCleanDMPLC },
      { "plc_used", HandlePlaceableUsed },
      { "os_statuemaker", HandleStatufyCreature },
      { "oc_statue", HandleCancelStatueConversation },
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
        switch (NWScript.GetTag(oidSelf))
        {
          case "theater_rope":

            if (!Convert.ToBoolean(NWScript.GetLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN")))
            {
              for (int i = 0; i < 4; i++)
                VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("theater_curtain", i), VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

              NWScript.SetLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN", 1);
            }
            else
            {
              for (int i = 0; i < 4; i++)
                VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, NWScript.GetObjectByTag("theater_curtain", i), VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);

              NWScript.DeleteLocalInt(NWScript.GetArea(oidSelf), "_THEATER_CURTAIN_OPEN");
            }
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
        NWScript.PlayAnimation(Utils.random.Next(100, 116));
        NWScript.SetEventScript(oidSelf, NWScript.EVENT_SCRIPT_CREATURE_ON_NOTICE, "");
        NWScript.SetAILevel(oidSelf, NWScript.AI_LEVEL_VERY_LOW);
        NWScript.DelayCommand(1.0f, () => FreezeCreature(oidSelf));
      }
      
      return 0;
    }
    private static void FreezeCreature(uint creature)
    {
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, NWScript.EffectPetrify(), creature);
      NWScript.SetObjectHiliteColor(creature, 0x000000);
      NWScript.SetObjectMouseCursor(creature, NWScript.MOUSECURSOR_WALK);
      NWScript.SetPlotFlag(creature, 1);
    }
  }
}
