using System;
using System.Collections.Generic;
using NWN.Core;

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
  }
}
