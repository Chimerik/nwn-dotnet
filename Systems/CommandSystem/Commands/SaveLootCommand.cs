using System;
using NWN.Core;
using System.Numerics;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSaveLootCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (Convert.ToBoolean(NWScript.GetIsDM(player.oid)))
        {
          Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
          {
            if (Convert.ToBoolean(NWScript.GetIsDM(player.oid))
              && NWScript.GetTag(NWScript.GetArea(player.oid)) == LootSystem.CHEST_AREA_TAG
              && NWScript.GetObjectType(oTarget) == NWScript.OBJECT_TYPE_PLACEABLE
              && Convert.ToBoolean(NWScript.GetHasInventory(oTarget)))
            {
              NWScript.SetEventScript(oTarget, NWScript.EVENT_SCRIPT_PLACEABLE_ON_CLOSED, LootSystem.LOOT_CONTAINER_ON_CLOSE_SCRIPT);

              var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "REPLACE INTO loot_containers(chestTag, accountID, serializedChest, position, facing)" +
              " VALUES(@chestTag, @accountId, @serializedChest, @position, @facing)");
              NWScript.SqlBindString(query, "@chestTag", NWScript.GetTag(oTarget));
              NWScript.SqlBindInt(query, "@accountId", player.accountId);
              NWScript.SqlBindObject(query, "@serializedChest", oTarget);
              NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(oTarget));
              NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(oTarget));
              NWScript.SqlStep(query);
            }
            else
              NWScript.SendMessageToPC(player.oid, "Cet objet ne peut être utilisé que par un dm, dans la zone de configuration des loots, sur un coffre disposant d'un inventaire.");

            Utils.LogMessageToDMs($"Loot Saver - Utilisation par {NWScript.GetName(player.oid)} ({NWScript.GetPCPlayerName(player.oid)}) dans la zone {NWScript.GetName(NWScript.GetArea(player.oid))} sur {NWScript.GetName(oTarget)}");
          };

          player.targetEvent = TargetEvent.LootSaverTarget;
          player.SelectTarget(callback);
        }
        else
          NWScript.SendMessageToPC(player.oid, "Il s'agit d'une commande DM, vous ne pouvez pas en faire usage en PJ.");
      }
    }
  }
}
