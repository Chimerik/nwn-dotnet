using System;
using NWN.Core;
using NWN.API.Constants;
using NWN.Services;
using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFollowCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.MovementRate == MovementRate.Immobile
        || ctx.oSender.TotalWeight > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", ctx.oSender.GetAbilityScore(Ability.Strength))))
      {
        ctx.oSender.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", Color.RED);
        return;
      }

      PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, FollowTarget, ObjectTypes.Creature, MouseCursor.Follow);
    }
    private static void FollowTarget(CursorTargetData selection)
    {
      if (selection.Player.MovementRate == MovementRate.Immobile
            || selection.Player.TotalWeight > int.Parse(NWScript.Get2DAString("encumbrance", "Heavy", selection.Player.GetAbilityScore(Ability.Strength))))
      {
        selection.Player.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", Color.RED);
        return;
      }

      selection.Player.ActionForceFollowObject((NwGameObject)selection.TargetObj, 3.0f);
    }
  }
}
