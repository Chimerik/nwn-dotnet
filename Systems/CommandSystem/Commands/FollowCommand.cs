using System;
using NWN.Core;
using System.Numerics;
using NWN.Core.NWNX;
using NWN.API.Constants;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFollowCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        if (NWScript.GetMovementRate(player.oid) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE
          || NWScript.GetWeight(player.oid) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", player.oid.GetAbilityScore(Ability.Strength))))
        { 
          NWScript.SendMessageToPC(player.oid, "Cette commande ne peut être utilisée en étant surchargé."); 
          return; 
        }

        PlayerSystem.cursorTargetService.EnterTargetMode(player.oid, FollowTarget, ObjectTypes.Creature, MouseCursor.Follow);
      }
    }
    private static void FollowTarget(CursorTargetData selection)
    {
      if (NWScript.GetMovementRate(selection.Player) == CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_IMMOBILE
            || NWScript.GetWeight(selection.Player) > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", selection.Player.GetAbilityScore(Ability.Strength))))
      {
        selection.Player.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", Color.RED);
        return;
      }

      selection.Player.ActionForceFollowObject((NwGameObject)selection.TargetObj, 3.0f);
    }
  }
}
