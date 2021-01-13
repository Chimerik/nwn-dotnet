using System;
using NWN.Core;
using System.Numerics;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFollowCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        if (NWScript.GetMovementRate(player.oid) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE
          || NWScript.GetWeight(player.oid) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(player.oid, NWScript.ABILITY_STRENGTH))))
        { 
          NWScript.SendMessageToPC(player.oid, "Cette commande ne peut être utilisée en étant surchargé."); 
          return; 
        }

        Action<uint, Vector3> callback = (uint oTarget, Vector3 position) =>
        {
          if (NWScript.GetMovementRate(player.oid) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE
            || NWScript.GetWeight(player.oid) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", NWScript.GetAbilityScore(player.oid, NWScript.ABILITY_STRENGTH))))
          {
            NWScript.SendMessageToPC(player.oid, "Cette commande ne peut être utilisée en étant surchargé.");
            return;
          }

          NWScript.ActionForceFollowObject(oTarget, 5.0f);
        };

        player.targetEvent = TargetEvent.LootSaverTarget;
        player.SelectTarget(callback);
      }
    }
  }
}
