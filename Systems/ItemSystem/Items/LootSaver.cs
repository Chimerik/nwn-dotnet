using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class ItemSystem
  {
    private static void HandleLootSaverActivate(Player player, uint oTarget)
    {
      if (Convert.ToBoolean(NWScript.GetIsDM(player.oid))
              && NWScript.GetTag(NWScript.GetArea(player.oid)) == LootSystem.CHEST_AREA_TAG
              && NWScript.GetObjectType(oTarget) == NWScript.OBJECT_TYPE_PLACEABLE
              && Convert.ToBoolean(NWScript.GetHasInventory(oTarget)))
      {
        NWScript.SetEventScript(oTarget, NWScript.EVENT_SCRIPT_PLACEABLE_ON_CLOSED, LootSystem.LOOT_CONTAINER_ON_CLOSE_SCRIPT);

        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "INSERT INTO loot_containers(chestTag, accountID, serializedPlaceable, position, facing)" +
        " VALUES(@chestTag, @accountId, @serializedPlaceable, @position, @facing)");
        NWScript.SqlBindString(query, "@chestTag", NWScript.GetTag(oTarget));
        NWScript.SqlBindInt(query, "@accountId", player.accountId);
        NWScript.SqlBindObject(query, "@serializedPlaceable", oTarget);
        NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(oTarget));
        NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(oTarget));
        NWScript.SqlStep(query);
      }
      else
        NWScript.SendMessageToPC(player.oid, "Cet objet ne peut être utilisé que par un dm, dans la zone de configuration des loots, sur un coffre disposant d'un inventaire.");

      Utils.LogMessageToDMs($"Loot Saver - Utilisation par {NWScript.GetName(player.oid)} ({NWScript.GetPCPlayerName(player.oid)}) dans la zone {NWScript.GetName(NWScript.GetArea(player.oid))} sur {NWScript.GetName(oTarget)}");
    }
  }
}
