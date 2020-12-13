using System;
using System.Numerics;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFixStorageCommand(ChatSystem.Context ctx, Options.Result options)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out player))
      {
        if (NWScript.GetPCPlayerName(player.oid) == "Chim")
        {
          Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
          {
            uint storage = NWScript.GetFirstObjectInArea(NWScript.GetObjectByTag("entrepotpersonnel"));

            if (NWScript.GetTag(storage) != "ps_entrepot")
              storage = NWScript.GetNearestObjectByTag("ps_entrepot", storage);

            PlayerSystem.Player targetPlayer;
            if (PlayerSystem.Players.TryGetValue(oTarget, out targetPlayer))
            {
              Utils.DestroyInventory(storage); 
              NWScript.CreateItemOnObject("NW_AARCL009", storage);
              NWScript.CreateItemOnObject("NW_WBLCL001", storage);
              NWScript.CreateItemOnObject("NW_ASHSW001", storage);
              NWScript.CreateItemOnObject("NW_WBWSL001", storage);
              NWScript.CreateItemOnObject("NW_WAMBU001", storage, 99);
              
              var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
              NWScript.SqlBindInt(query, "@characterId", targetPlayer.characterId);
              NWScript.SqlBindObject(query, "@storage", storage);
              NWScript.SqlStep(query);
            }
          };

          player.targetEvent = TargetEvent.LootSaverTarget;
          player.SelectTarget(callback);
        }
      }
    }
  }
}
